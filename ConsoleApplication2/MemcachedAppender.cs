using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

using log4net.Layout;
using log4net.Core;
using log4net.Util;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;


namespace logQueue
{

    public class MemcachedAppender : log4net.Appender.AppenderSkeleton
    {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedAppender" /> class.
        /// </summary>
        /// <remarks>
        /// The default constructor initializes all fields to their default values.
        /// </remarks>
        public MemcachedAppender()
        {
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the IP address of the remote host or multicast group to which
        /// the underlying <see cref="MemcachedClient" /> should sent the logging event.
        /// </summary>
        /// <value>
        /// The IP address of the remote host or multicast group to which the logging event 
        /// will be sent.
        /// </value>
        /// <remarks>
        /// <para>
        /// Multicast addresses are identified by IP class <b>D</b> addresses (in the range 224.0.0.0 to
        /// 239.255.255.255).  Multicast packets can pass across different networks through routers, so
        /// it is possible to use multicasts in an Internet scenario as long as your network provider 
        /// supports multicasting.
        /// </para>
        /// <para>
        /// Hosts that want to receive particular multicast messages must register their interest by joining
        /// the multicast group.  Multicast messages are not sent to networks where no host has joined
        /// the multicast group.  Class <b>D</b> IP addresses are used for multicast groups, to differentiate
        /// them from normal host addresses, allowing nodes to easily detect if a message is of interest.
        /// </para>
        /// <para>
        /// Static multicast addresses that are needed globally are assigned by IANA.  A few examples are listed in the table below:
        /// </para>
        /// <para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>IP Address</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>224.0.0.1</term>
        ///         <description>
        ///             <para>
        ///             Sends a message to all system on the subnet.
        ///             </para>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>224.0.0.2</term>
        ///         <description>
        ///             <para>
        ///             Sends a message to all routers on the subnet.
        ///             </para>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>224.0.0.12</term>
        ///         <description>
        ///             <para>
        ///             The DHCP server answers messages on the IP address 224.0.0.12, but only on a subnet.
        ///             </para>
        ///         </description>
        ///     </item>
        /// </list>
        /// </para>
        /// <para>
        /// A complete list of actually reserved multicast addresses and their owners in the ranges
        /// defined by RFC 3171 can be found at the <A href="http://www.iana.org/assignments/multicast-addresses">IANA web site</A>. 
        /// </para>
        /// <para>
        /// The address range 239.0.0.0 to 239.255.255.255 is reserved for administrative scope-relative 
        /// addresses.  These addresses can be reused with other local groups.  Routers are typically 
        /// configured with filters to prevent multicast traffic in this range from flowing outside
        /// of the local network.
        /// </para>
        /// </remarks>
        public IPAddress RemoteAddress
        {
            get { return m_remoteAddress; }
            set { m_remoteAddress = value; }
        }

        /// <summary>
        /// Gets or sets the TCP port number of the remote host or multicast group to which 
        /// the underlying <see cref="MemcachedClient" /> should sent the logging event.
        /// </summary>
        /// <value>
        /// An integer value in the range <see cref="IPEndPoint.MinPort" /> to <see cref="IPEndPoint.MaxPort" /> 
        /// indicating the TCP port number of the remote host or multicast group to which the logging event 
        /// will be sent.
        /// </value>
        /// <remarks>
        /// The underlying <see cref="MemcachedClient" /> will send messages to this TCP port number
        /// on the remote host or multicast group.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">The value specified is less than <see cref="IPEndPoint.MinPort" /> or greater than <see cref="IPEndPoint.MaxPort" />.</exception>
        public int RemotePort
        {
            get { return m_remotePort; }
            set
            {
                if (value < IPEndPoint.MinPort || value > IPEndPoint.MaxPort)
                {
                    throw log4net.Util.SystemInfo.CreateArgumentOutOfRangeException("value", (object)value,
                        "The value specified is less than " +
                        IPEndPoint.MinPort.ToString(NumberFormatInfo.InvariantInfo) +
                        " or greater than " +
                        IPEndPoint.MaxPort.ToString(NumberFormatInfo.InvariantInfo) + ".");
                }
                else
                {
                    m_remotePort = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the TCP port number from which the underlying <see cref="MemcachedClient" /> will communicate.
        /// </summary>
        /// <value>
        /// An integer value in the range <see cref="IPEndPoint.MinPort" /> to <see cref="IPEndPoint.MaxPort" /> 
        /// indicating the TCP port number from which the underlying <see cref="MemcachedClient" /> will communicate.
        /// </value>
        /// <remarks>
        /// <para>
        /// The underlying <see cref="MemcachedClient" /> will bind to this port for sending messages.
        /// </para>
        /// <para>
        /// Setting the value to 0 (the default) will cause the udp client not to bind to
        /// a local port.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">The value specified is less than <see cref="IPEndPoint.MinPort" /> or greater than <see cref="IPEndPoint.MaxPort" />.</exception>

        /// <summary>
        /// Gets or sets <see cref="Encoding"/> used to write the packets.
        /// </summary>
        /// <value>
        /// The <see cref="Encoding"/> used to write the packets.
        /// </value>
        /// <remarks>
        /// <para>
        /// The <see cref="Encoding"/> used to write the packets.
        /// </para>
        /// </remarks>
        public Encoding Encoding
        {
            get { return m_encoding; }
            set { m_encoding = value; }
        }

        public string File
        {
            get { return m_fileName; }
            set { m_fileName = value; }
        }


        #endregion Public Instance Properties

        #region Protected Instance Properties

        /// <summary>
        /// Gets or sets the underlying <see cref="MemcachedClient" />.
        /// </summary>
        /// <value>
        /// The underlying <see cref="MemcachedClient" />.
        /// </value>
        /// <remarks>
        /// <see cref="MemcachedAppender" /> creates a <see cref="MemcachedClient" /> to send logging events 
        /// over a network.  Classes deriving from <see cref="MemcachedAppender" /> can use this
        /// property to get or set this <see cref="MemcachedClient" />.  Use the underlying <see cref="MemcachedClient" />
        /// returned from <see cref="Client" /> if you require access beyond that which 
        /// <see cref="MemcachedAppender" /> provides.
        /// </remarks>
        protected MemcachedClient Client
        {
            get { return this.m_client; }
            set { this.m_client = value; }
        }

        /// <summary>
        /// Gets or sets the cached remote endpoint to which the logging events should be sent.
        /// </summary>
        /// <value>
        /// The cached remote endpoint to which the logging events will be sent.
        /// </value>
        /// <remarks>
        /// The <see cref="ActivateOptions" /> method will initialize the remote endpoint 
        /// with the values of the <see cref="RemoteAddress" /> and <see cref="RemotePort"/>
        /// properties.
        /// </remarks>
        protected IPEndPoint RemoteEndPoint
        {
            get { return this.m_remoteEndPoint; }
            set { this.m_remoteEndPoint = value; }
        }

        #endregion Protected Instance Properties

        #region Implementation of IOptionHandler

        /// <summary>
        /// Initialize the appender based on the options set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="IOptionHandler"/> delayed object
        /// activation scheme. The <see cref="ActivateOptions"/> method must 
        /// be called on this object after the configuration properties have
        /// been set. Until <see cref="ActivateOptions"/> is called this
        /// object is in an undefined state and must not be used. 
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then 
        /// <see cref="ActivateOptions"/> must be called again.
        /// </para>
        /// <para>
        /// The appender will be ignored if no <see cref="RemoteAddress" /> was specified or 
        /// an invalid remote or local TCP port number was specified.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">The required property <see cref="RemoteAddress" /> was not specified.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The TCP port number assigned to <see cref="RemotePort" /> is less than <see cref="IPEndPoint.MinPort" /> or greater than <see cref="IPEndPoint.MaxPort" />.</exception>
        public override void ActivateOptions()
        {
            base.ActivateOptions();

            if (this.RemoteAddress == null)
            {
                throw new ArgumentNullException("The required property 'Address' was not specified.");
            }
            else if (this.RemotePort < IPEndPoint.MinPort || this.RemotePort > IPEndPoint.MaxPort)
            {
                throw log4net.Util.SystemInfo.CreateArgumentOutOfRangeException("this.RemotePort", (object)this.RemotePort,
                    "The RemotePort is less than " +
                    IPEndPoint.MinPort.ToString(NumberFormatInfo.InvariantInfo) +
                    " or greater than " +
                    IPEndPoint.MaxPort.ToString(NumberFormatInfo.InvariantInfo) + ".");
            }
            else
            {
                this.RemoteEndPoint = new IPEndPoint(this.RemoteAddress, this.RemotePort);
                this.InitializeClientConnection();
            }
        }

        #endregion

        #region Override implementation of AppenderSkeleton

        /// <summary>
        /// This method is called by the <see cref="M:AppenderSkeleton.DoAppend(LoggingEvent)"/> method.
        /// </summary>
        /// <param name="loggingEvent">The event to log.</param>
        /// <remarks>
        /// <para>
        /// Sends the event using an UDP datagram.
        /// </para>
        /// <para>
        /// Exceptions are passed to the <see cref="AppenderSkeleton.ErrorHandler"/>.
        /// </para>
        /// </remarks>
        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                Byte[] buffer = m_encoding.GetBytes(RenderLoggingEvent(loggingEvent).ToCharArray());
                this.Client.Store(StoreMode.Set, m_fileName, buffer);
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(
                    "Unable to send logging event to remote host " +
                    this.RemoteAddress.ToString() +
                    " on port " +
                    this.RemotePort + ".",
                    ex,
                    ErrorCode.WriteFailure);
            }
        }

        /// <summary>
        /// This appender requires a <see cref="Layout"/> to be set.
        /// </summary>
        /// <value><c>true</c></value>
        /// <remarks>
        /// <para>
        /// This appender requires a <see cref="Layout"/> to be set.
        /// </para>
        /// </remarks>
        override protected bool RequiresLayout
        {
            get { return true; }
        }

        /// <summary>
        /// Closes the UDP connection and releases all resources associated with 
        /// this <see cref="MemcachedAppender" /> instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Disables the underlying <see cref="MemcachedClient" /> and releases all managed 
        /// and unmanaged resources associated with the <see cref="MemcachedAppender" />.
        /// </para>
        /// </remarks>
        override protected void OnClose()
        {
            base.OnClose();

            if (this.Client != null)
            {
                this.Client.Dispose();
                this.Client = null;
            }
        }

        #endregion Override implementation of AppenderSkeleton

        #region Protected Instance Methods

        /// <summary>
        /// Initializes the underlying  <see cref="MemcachedClient" /> connection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The underlying <see cref="MemcachedClient"/> is initialized and binds to the 
        /// port number from which you intend to communicate.
        /// </para>
        /// <para>
        /// Exceptions are passed to the <see cref="AppenderSkeleton.ErrorHandler"/>.
        /// </para>
        /// </remarks>
        protected virtual void InitializeClientConnection()
        {
            try
            {
                MemcachedClientConfiguration config = new MemcachedClientConfiguration();
                config.Servers.Add(this.RemoteEndPoint);
                config.Protocol = MemcachedProtocol.Text;

                this.Client = new MemcachedClient(config);
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(
                    "Could not initialize the MemcachedClient connection on port " +
                    this.RemotePort.ToString(NumberFormatInfo.InvariantInfo) + ".",
                    ex,
                    ErrorCode.GenericFailure);

                this.Client = null;
            }
        }

        #endregion Protected Instance Methods

        #region Private Instance Fields

        /// <summary>
        /// The IP address of the remote host or multicast group to which 
        /// the logging event will be sent.
        /// </summary>
        private IPAddress m_remoteAddress;

        /// <summary>
        /// The TCP port number of the remote host or multicast group to 
        /// which the logging event will be sent.
        /// </summary>
        private int m_remotePort;

        /// <summary>
        /// The cached remote endpoint to which the logging events will be sent.
        /// </summary>
        private IPEndPoint m_remoteEndPoint;


        /// <summary>
        /// The <see cref="MemcachedClient" /> instance that will be used for sending the 
        /// logging events.
        /// </summary>
        private MemcachedClient m_client;

        /// <summary>
        /// The encoding to use for the packet.
        /// </summary>
        private Encoding m_encoding = Encoding.Default;

        /// <summary>
        /// The Filename.
        /// </summary>
        private string m_fileName = null;


        #endregion Private Instance Fields
    }
}