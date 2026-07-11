using System;
using System.Collections.Generic;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>Class defines network topology for host (socket opened by Networking.NetworkTransport.AddHost function). This topology defines: (1) how many connection with default config will be supported and (2) what will be special connections (connections with config different from default).</para>
	/// </summary>
	[Serializable]
	public class HostTopology
	{
		/// <summary>
		///   <para>Create topology.</para>
		/// </summary>
		/// <param name="defaultConfig">Default config.</param>
		/// <param name="maxDefaultConnections">Maximum default connections.</param>
		public HostTopology(ConnectionConfig defaultConfig, int maxDefaultConnections)
		{
			if (defaultConfig == null)
			{
				throw new NullReferenceException("config is not defined");
			}
			if (maxDefaultConnections <= 0)
			{
				throw new ArgumentOutOfRangeException("maxConnections", "Number of connections should be > 0");
			}
			if (maxDefaultConnections >= 65535)
			{
				throw new ArgumentOutOfRangeException("maxConnections", "Number of connections should be < 65535");
			}
			ConnectionConfig.Validate(defaultConfig);
			this.m_DefConfig = new ConnectionConfig(defaultConfig);
			this.m_MaxDefConnections = maxDefaultConnections;
		}

		private HostTopology()
		{
		}

		/// <summary>
		///   <para>Defines config for default connections in the topology.</para>
		/// </summary>
		public ConnectionConfig DefaultConfig
		{
			get
			{
				return this.m_DefConfig;
			}
		}

		/// <summary>
		///   <para>Defines how many connection with default config be permitted.</para>
		/// </summary>
		public int MaxDefaultConnections
		{
			get
			{
				return this.m_MaxDefConnections;
			}
		}

		/// <summary>
		///   <para>Returns count of special connection added to topology.</para>
		/// </summary>
		public int SpecialConnectionConfigsCount
		{
			get
			{
				return this.m_SpecialConnections.Count;
			}
		}

		/// <summary>
		///   <para>List of special connection configs.</para>
		/// </summary>
		public List<ConnectionConfig> SpecialConnectionConfigs
		{
			get
			{
				return this.m_SpecialConnections;
			}
		}

		/// <summary>
		///   <para>Return reference to special connection config. Parameters of this config can be changed.</para>
		/// </summary>
		/// <param name="i">Config id.</param>
		/// <returns>
		///   <para>Connection config.</para>
		/// </returns>
		public ConnectionConfig GetSpecialConnectionConfig(int i)
		{
			if (i > this.m_SpecialConnections.Count || i == 0)
			{
				throw new ArgumentException("special configuration index is out of valid range");
			}
			return this.m_SpecialConnections[i - 1];
		}

		/// <summary>
		///   <para>Defines the maximum number of messages that each host can hold in its pool of received messages. The default size is 128.</para>
		/// </summary>
		public ushort ReceivedMessagePoolSize
		{
			get
			{
				return this.m_ReceivedMessagePoolSize;
			}
			set
			{
				this.m_ReceivedMessagePoolSize = value;
			}
		}

		/// <summary>
		///   <para>Defines the maximum number of messages that each host can hold in its pool of messages waiting to be sent. The default size is 128.</para>
		/// </summary>
		public ushort SentMessagePoolSize
		{
			get
			{
				return this.m_SentMessagePoolSize;
			}
			set
			{
				this.m_SentMessagePoolSize = value;
			}
		}

		public float MessagePoolSizeGrowthFactor
		{
			get
			{
				return this.m_MessagePoolSizeGrowthFactor;
			}
			set
			{
				if ((double)value <= 0.5 || (double)value > 1.0)
				{
					throw new ArgumentException("pool growth factor should be varied between 0.5 and 1.0");
				}
				this.m_MessagePoolSizeGrowthFactor = value;
			}
		}

		/// <summary>
		///   <para>Add special connection to topology (for example if you need to keep connection to standalone chat server you will need to use this function). Returned id should be use as one of parameters (with ip and port) to establish connection to this server.</para>
		/// </summary>
		/// <param name="config">Connection config for special connection.</param>
		/// <returns>
		///   <para>Id of this connection. You should use this id when you call Networking.NetworkTransport.Connect.</para>
		/// </returns>
		public int AddSpecialConnectionConfig(ConnectionConfig config)
		{
			if (this.m_MaxDefConnections + this.m_SpecialConnections.Count + 1 >= 65535)
			{
				throw new ArgumentOutOfRangeException("maxConnections", "Number of connections should be < 65535");
			}
			this.m_SpecialConnections.Add(new ConnectionConfig(config));
			return this.m_SpecialConnections.Count;
		}

		[SerializeField]
		private ConnectionConfig m_DefConfig = null;

		[SerializeField]
		private int m_MaxDefConnections = 0;

		[SerializeField]
		private List<ConnectionConfig> m_SpecialConnections = new List<ConnectionConfig>();

		[SerializeField]
		private ushort m_ReceivedMessagePoolSize = 1024;

		[SerializeField]
		private ushort m_SentMessagePoolSize = 1024;

		[SerializeField]
		private float m_MessagePoolSizeGrowthFactor = 0.75f;
	}
}
