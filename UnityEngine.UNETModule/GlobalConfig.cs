using System;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>Defines global paramters for network library.</para>
	/// </summary>
	[Serializable]
	public class GlobalConfig
	{
		/// <summary>
		///   <para>Create new global config object.</para>
		/// </summary>
		public GlobalConfig()
		{
			this.m_ThreadAwakeTimeout = 1U;
			this.m_ReactorModel = ReactorModel.SelectReactor;
			this.m_ReactorMaximumReceivedMessages = 1024;
			this.m_ReactorMaximumSentMessages = 1024;
			this.m_MaxPacketSize = 2000;
			this.m_MaxHosts = 16;
			this.m_ThreadPoolSize = 1;
			this.m_MinTimerTimeout = 1U;
			this.m_MaxTimerTimeout = 12000U;
			this.m_MinNetSimulatorTimeout = 1U;
			this.m_MaxNetSimulatorTimeout = 12000U;
			this.m_ConnectionReadyForSend = null;
			this.m_NetworkEventAvailable = null;
		}

		/// <summary>
		///   <para>Defines (1) for select reactor, minimum time period, when system will check if there are any messages for send (2) for fixrate reactor, minimum interval of time, when system will check for sending and receiving messages.</para>
		/// </summary>
		public uint ThreadAwakeTimeout
		{
			get
			{
				return this.m_ThreadAwakeTimeout;
			}
			set
			{
				if (value == 0U)
				{
					throw new ArgumentOutOfRangeException("Minimal thread awake timeout should be > 0");
				}
				this.m_ThreadAwakeTimeout = value;
			}
		}

		/// <summary>
		///   <para>Defines reactor model for the network library.</para>
		/// </summary>
		public ReactorModel ReactorModel
		{
			get
			{
				return this.m_ReactorModel;
			}
			set
			{
				this.m_ReactorModel = value;
			}
		}

		/// <summary>
		///   <para>This property determines the initial size of the queue that holds messages received by Unity Multiplayer before they are processed.</para>
		/// </summary>
		public ushort ReactorMaximumReceivedMessages
		{
			get
			{
				return this.m_ReactorMaximumReceivedMessages;
			}
			set
			{
				this.m_ReactorMaximumReceivedMessages = value;
			}
		}

		/// <summary>
		///   <para>Defines the initial size of the send queue. Messages are placed in this queue ready to be sent in packets to their destination.</para>
		/// </summary>
		public ushort ReactorMaximumSentMessages
		{
			get
			{
				return this.m_ReactorMaximumSentMessages;
			}
			set
			{
				this.m_ReactorMaximumSentMessages = value;
			}
		}

		/// <summary>
		///   <para>Defines maximum possible packet size in bytes for all network connections.</para>
		/// </summary>
		public ushort MaxPacketSize
		{
			get
			{
				return this.m_MaxPacketSize;
			}
			set
			{
				this.m_MaxPacketSize = value;
			}
		}

		/// <summary>
		///   <para>Defines how many hosts you can use. Default Value = 16. Max value = 128.</para>
		/// </summary>
		public ushort MaxHosts
		{
			get
			{
				return this.m_MaxHosts;
			}
			set
			{
				if (value == 0)
				{
					throw new ArgumentOutOfRangeException("MaxHosts", "Maximum hosts number should be > 0");
				}
				if (value > 128)
				{
					throw new ArgumentOutOfRangeException("MaxHosts", "Maximum hosts number should be <= " + 128.ToString());
				}
				this.m_MaxHosts = value;
			}
		}

		/// <summary>
		///   <para>Defines how many worker threads are available to handle incoming and outgoing messages.</para>
		/// </summary>
		public byte ThreadPoolSize
		{
			get
			{
				return this.m_ThreadPoolSize;
			}
			set
			{
				this.m_ThreadPoolSize = value;
			}
		}

		/// <summary>
		///   <para>Defines the minimum timeout in milliseconds recognised by the system. The default value is 1 ms.</para>
		/// </summary>
		public uint MinTimerTimeout
		{
			get
			{
				return this.m_MinTimerTimeout;
			}
			set
			{
				if (value > this.MaxTimerTimeout)
				{
					throw new ArgumentOutOfRangeException("MinTimerTimeout should be < MaxTimerTimeout");
				}
				if (value == 0U)
				{
					throw new ArgumentOutOfRangeException("MinTimerTimeout should be > 0");
				}
				this.m_MinTimerTimeout = value;
			}
		}

		/// <summary>
		///   <para>Defines the maximum timeout in milliseconds for any configuration. The default value is 12 seconds (12000ms).</para>
		/// </summary>
		public uint MaxTimerTimeout
		{
			get
			{
				return this.m_MaxTimerTimeout;
			}
			set
			{
				if (value == 0U)
				{
					throw new ArgumentOutOfRangeException("MaxTimerTimeout should be > 0");
				}
				if (value > 12000U)
				{
					throw new ArgumentOutOfRangeException("MaxTimerTimeout should be <=" + 12000U.ToString());
				}
				this.m_MaxTimerTimeout = value;
			}
		}

		/// <summary>
		///   <para>Deprecated. Defines the minimal timeout for network simulator. You cannot set up any delay less than this value. See Also: MinTimerTimeout.</para>
		/// </summary>
		public uint MinNetSimulatorTimeout
		{
			get
			{
				return this.m_MinNetSimulatorTimeout;
			}
			set
			{
				if (value > this.MaxNetSimulatorTimeout)
				{
					throw new ArgumentOutOfRangeException("MinNetSimulatorTimeout should be < MaxTimerTimeout");
				}
				if (value == 0U)
				{
					throw new ArgumentOutOfRangeException("MinNetSimulatorTimeout should be > 0");
				}
				this.m_MinNetSimulatorTimeout = value;
			}
		}

		/// <summary>
		///   <para>Deprecated. Defines maximum delay for network simulator. See Also: MaxTimerTimeout.</para>
		/// </summary>
		public uint MaxNetSimulatorTimeout
		{
			get
			{
				return this.m_MaxNetSimulatorTimeout;
			}
			set
			{
				if (value == 0U)
				{
					throw new ArgumentOutOfRangeException("MaxNetSimulatorTimeout should be > 0");
				}
				if (value > 12000U)
				{
					throw new ArgumentOutOfRangeException("MaxNetSimulatorTimeout should be <=" + 12000U.ToString());
				}
				this.m_MaxNetSimulatorTimeout = value;
			}
		}

		/// <summary>
		///   <para>Defines the callback delegate which you can use to get a notification when the host (defined by hostID) has a network event. The callback is called for all event types except Networking.NetworkEventType.Nothing.
		///
		/// See Also: Networking.NetworkEventType</para>
		/// </summary>
		public Action<int> NetworkEventAvailable
		{
			get
			{
				return this.m_NetworkEventAvailable;
			}
			set
			{
				this.m_NetworkEventAvailable = value;
			}
		}

		/// <summary>
		///   <para>Defines the callback delegate which you can use to get a notification when a connection is ready to send data.</para>
		/// </summary>
		public Action<int, int> ConnectionReadyForSend
		{
			get
			{
				return this.m_ConnectionReadyForSend;
			}
			set
			{
				this.m_ConnectionReadyForSend = value;
			}
		}

		private const uint g_MaxTimerTimeout = 12000U;

		private const uint g_MaxNetSimulatorTimeout = 12000U;

		private const ushort g_MaxHosts = 128;

		[SerializeField]
		private uint m_ThreadAwakeTimeout;

		[SerializeField]
		private ReactorModel m_ReactorModel;

		[SerializeField]
		private ushort m_ReactorMaximumReceivedMessages;

		[SerializeField]
		private ushort m_ReactorMaximumSentMessages;

		[SerializeField]
		private ushort m_MaxPacketSize;

		[SerializeField]
		private ushort m_MaxHosts;

		[SerializeField]
		private byte m_ThreadPoolSize;

		[SerializeField]
		private uint m_MinTimerTimeout;

		[SerializeField]
		private uint m_MaxTimerTimeout;

		[SerializeField]
		private uint m_MinNetSimulatorTimeout;

		[SerializeField]
		private uint m_MaxNetSimulatorTimeout;

		[SerializeField]
		private Action<int, int> m_ConnectionReadyForSend;

		[SerializeField]
		private Action<int> m_NetworkEventAvailable;
	}
}
