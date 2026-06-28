using System;

namespace UnityEngine.Networking
{
	[Serializable]
	public class GlobalConfig
	{
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
		}

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
	}
}
