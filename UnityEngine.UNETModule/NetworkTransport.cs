using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Networking.Types;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>Transport Layer API.</para>
	/// </summary>
	[NativeHeader("Runtime/Networking/UNETManager.h")]
	[NativeHeader("Runtime/Networking/UNetTypes.h")]
	[NativeConditional("ENABLE_NETWORK && ENABLE_UNET", true)]
	[NativeHeader("Runtime/Networking/UNETConfiguration.h")]
	public sealed class NetworkTransport
	{
		private NetworkTransport()
		{
		}

		/// <summary>
		///   <para>Initializes the NetworkTransport. Should be called before any other operations on the NetworkTransport are done.</para>
		/// </summary>
		public static void Init()
		{
			NetworkTransport.InitWithNoParameters();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitWithNoParameters();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitWithParameters(GlobalConfigInternal config);

		/// <summary>
		///   <para>Shut down the NetworkTransport.</para>
		/// </summary>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Shutdown();

		/// <summary>
		///   <para>The Unity Multiplayer spawning system uses assetIds to identify what remote objects to spawn. This function allows you to get the assetId for the prefab associated with an object.</para>
		/// </summary>
		/// <param name="go">Target GameObject to get assetId for.</param>
		/// <returns>
		///   <para>The assetId of the game object's prefab.</para>
		/// </returns>
		[Obsolete("This function has been deprecated. Use AssetDatabase utilities instead.")]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetAssetId(GameObject go);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AddSceneId(int id);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNextSceneId();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ConnectAsNetworkHost(int hostId, string address, int port, NetworkID network, SourceID source, NodeID node, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisconnectNetworkHost(int hostId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern NetworkEventType ReceiveRelayEventFromHost(int hostId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int ConnectToNetworkPeer(int hostId, string address, int port, int exceptionConnectionId, int relaySlotId, NetworkID network, SourceID source, NodeID node, int bytesPerSec, float bucketSizeFactor, out byte error);

		public static int ConnectToNetworkPeer(int hostId, string address, int port, int exceptionConnectionId, int relaySlotId, NetworkID network, SourceID source, NodeID node, out byte error)
		{
			return NetworkTransport.ConnectToNetworkPeer(hostId, address, port, exceptionConnectionId, relaySlotId, network, source, node, 0, 0f, out error);
		}

		/// <summary>
		///   <para>Returns the number of unread messages in the read-queue.</para>
		/// </summary>
		[GeneratedByOldBindingsGenerator]
		[Obsolete("GetCurrentIncomingMessageAmount has been deprecated.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCurrentIncomingMessageAmount();

		/// <summary>
		///   <para>Returns the total number of messages still in the write-queue.</para>
		/// </summary>
		[GeneratedByOldBindingsGenerator]
		[Obsolete("GetCurrentOutgoingMessageAmount has been deprecated.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCurrentOutgoingMessageAmount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetIncomingMessageQueueSize(int hostId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingMessageQueueSize(int hostId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCurrentRTT(int hostId, int connectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[Obsolete("GetCurrentRtt() has been deprecated.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCurrentRtt(int hostId, int connectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetIncomingPacketLossCount(int hostId, int connectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[Obsolete("GetNetworkLostPacketNum() has been deprecated.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNetworkLostPacketNum(int hostId, int connectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetIncomingPacketCount(int hostId, int connectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingPacketNetworkLossPercent(int hostId, int connectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingPacketOverflowLossPercent(int hostId, int connectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetMaxAllowedBandwidth(int hostId, int connectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetAckBufferCount(int hostId, int connectionId, out byte error);

		/// <summary>
		///   <para>How many packets have been dropped due lack space in incoming queue (absolute value, countinf from start).</para>
		/// </summary>
		/// <returns>
		///   <para>Dropping packet count.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetIncomingPacketDropCountForAllHosts();

		/// <summary>
		///   <para>Returns how many packets have been received from start. (from Networking.NetworkTransport.Init call).</para>
		/// </summary>
		/// <returns>
		///   <para>Packets count received from start for all hosts.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetIncomingPacketCountForAllHosts();

		/// <summary>
		///   <para>Returns how many packets have been sent from start (from call Networking.NetworkTransport.Init) for all hosts.</para>
		/// </summary>
		/// <returns>
		///   <para>Packets count sent from networking library start (from call Networking.NetworkTransport.Init)  for all hosts.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingPacketCount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingPacketCountForHost(int hostId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingPacketCountForConnection(int hostId, int connectionId, out byte error);

		/// <summary>
		///   <para>Returns how many messages have been sent from start (from Networking.NetworkTransport.Init call).</para>
		/// </summary>
		/// <returns>
		///   <para>Messages count sent from start (from call Networking.NetworkTransport.Init) for all hosts.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingMessageCount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingMessageCountForHost(int hostId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingMessageCountForConnection(int hostId, int connectionId, out byte error);

		/// <summary>
		///   <para>Returns how much payload (user) bytes have been sent from start (from Networking.NetworkTransport.Init call).</para>
		/// </summary>
		/// <returns>
		///   <para>Total payload (in bytes) sent from start for all hosts.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingUserBytesCount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingUserBytesCountForHost(int hostId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingUserBytesCountForConnection(int hostId, int connectionId, out byte error);

		/// <summary>
		///   <para>Returns how much user payload and protocol system headers (in bytes)  have been sent from start (from Networking.NetworkTransport.Init call).</para>
		/// </summary>
		/// <returns>
		///   <para>Total payload and protocol system headers (in bytes) sent from start for all hosts.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingSystemBytesCount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingSystemBytesCountForHost(int hostId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingSystemBytesCountForConnection(int hostId, int connectionId, out byte error);

		/// <summary>
		///   <para>Returns how much raw data (in bytes) have been sent from start for all hosts (from Networking.NetworkTransport.Init call).</para>
		/// </summary>
		/// <returns>
		///   <para>Total data (user payload, protocol specific data, ip and udp headers) (in bytes) sent from start for all hosts.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingFullBytesCount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingFullBytesCountForHost(int hostId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingFullBytesCountForConnection(int hostId, int connectionId, out byte error);

		[Obsolete("GetPacketSentRate has been deprecated.")]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetPacketSentRate(int hostId, int connectionId, out byte error);

		[Obsolete("GetPacketReceivedRate has been deprecated.")]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetPacketReceivedRate(int hostId, int connectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[Obsolete("GetRemotePacketReceivedRate has been deprecated.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetRemotePacketReceivedRate(int hostId, int connectionId, out byte error);

		/// <summary>
		///   <para>Function returns time spent on network I/O operations in microseconds.</para>
		/// </summary>
		/// <returns>
		///   <para>Time in micro seconds.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[Obsolete("GetNetIOTimeuS has been deprecated.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNetIOTimeuS();

		public static void GetConnectionInfo(int hostId, int connectionId, out string address, out int port, out NetworkID network, out NodeID dstNode, out byte error)
		{
			ulong num;
			ushort num2;
			address = NetworkTransport.GetConnectionInfo(hostId, connectionId, out port, out num, out num2, out error);
			network = (NetworkID)num;
			dstNode = (NodeID)num2;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetConnectionInfo(int hostId, int connectionId, out int port, out ulong network, out ushort dstNode, out byte error);

		/// <summary>
		///   <para>Get a network timestamp. Can be used in your messages to investigate network delays together with Networking.GetRemoteDelayTimeMS.</para>
		/// </summary>
		/// <returns>
		///   <para>Timestamp.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNetworkTimestamp();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetRemoteDelayTimeMS(int hostId, int connectionId, int remoteTime, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool StartSendMulticast(int hostId, int channelId, byte[] buffer, int size, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SendMulticast(int hostId, int connectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool FinishSendMulticast(int hostId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMaxPacketSize();

		private static void CheckTopology(HostTopology topology)
		{
			int maxPacketSize = NetworkTransport.GetMaxPacketSize();
			if ((int)topology.DefaultConfig.PacketSize > maxPacketSize)
			{
				throw new ArgumentOutOfRangeException("Default config: packet size should be less than packet size defined in global config: " + maxPacketSize.ToString());
			}
			for (int i = 0; i < topology.SpecialConnectionConfigs.Count; i++)
			{
				if ((int)topology.SpecialConnectionConfigs[i].PacketSize > maxPacketSize)
				{
					throw new ArgumentOutOfRangeException("Special config " + i.ToString() + ": packet size should be less than packet size defined in global config: " + maxPacketSize.ToString());
				}
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddWsHostWrapper(HostTopologyInternal topologyInt, string ip, int port);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddWsHostWrapperWithoutIp(HostTopologyInternal topologyInt, int port);

		private static bool IsPortOpen(string ip, int port)
		{
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(500.0);
			string text = ((ip != null) ? ip : "127.0.0.1");
			try
			{
				using (TcpClient tcpClient = new TcpClient())
				{
					IAsyncResult asyncResult = tcpClient.BeginConnect(text, port, null, null);
					if (!asyncResult.AsyncWaitHandle.WaitOne(timeSpan))
					{
						return false;
					}
					tcpClient.EndConnect(asyncResult);
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		/// <summary>
		///   <para>Created web socket host.</para>
		/// </summary>
		/// <param name="port">Port to bind to.</param>
		/// <param name="topology">The Networking.HostTopology associated with the host.</param>
		/// <param name="ip">IP address to bind to.</param>
		/// <returns>
		///   <para>Web socket host id.</para>
		/// </returns>
		[ExcludeFromDocs]
		public static int AddWebsocketHost(HostTopology topology, int port)
		{
			string text = null;
			return NetworkTransport.AddWebsocketHost(topology, port, text);
		}

		/// <summary>
		///   <para>Created web socket host.</para>
		/// </summary>
		/// <param name="port">Port to bind to.</param>
		/// <param name="topology">The Networking.HostTopology associated with the host.</param>
		/// <param name="ip">IP address to bind to.</param>
		/// <returns>
		///   <para>Web socket host id.</para>
		/// </returns>
		public static int AddWebsocketHost(HostTopology topology, int port, [DefaultValue("null")] string ip)
		{
			if (port != 0)
			{
				if (NetworkTransport.IsPortOpen(ip, port))
				{
					throw new InvalidOperationException("Cannot open web socket on port " + port + " It has been already occupied.");
				}
			}
			if (topology == null)
			{
				throw new NullReferenceException("topology is not defined");
			}
			NetworkTransport.CheckTopology(topology);
			int num;
			if (ip == null)
			{
				num = NetworkTransport.AddWsHostWrapperWithoutIp(new HostTopologyInternal(topology), port);
			}
			else
			{
				num = NetworkTransport.AddWsHostWrapper(new HostTopologyInternal(topology), ip, port);
			}
			return num;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddHostWrapper(HostTopologyInternal topologyInt, string ip, int port, int minTimeout, int maxTimeout);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddHostWrapperWithoutIp(HostTopologyInternal topologyInt, int port, int minTimeout, int maxTimeout);

		[ExcludeFromDocs]
		public static int AddHost(HostTopology topology, int port)
		{
			string text = null;
			return NetworkTransport.AddHost(topology, port, text);
		}

		[ExcludeFromDocs]
		public static int AddHost(HostTopology topology)
		{
			string text = null;
			int num = 0;
			return NetworkTransport.AddHost(topology, num, text);
		}

		/// <summary>
		///   <para>Creates a host based on Networking.HostTopology.</para>
		/// </summary>
		/// <param name="topology">The Networking.HostTopology associated with the host.</param>
		/// <param name="port">Port to bind to (when 0 is selected, the OS will choose a port at random).</param>
		/// <param name="ip">IP address to bind to.</param>
		/// <returns>
		///   <para>Returns the ID of the host that was created.</para>
		/// </returns>
		public static int AddHost(HostTopology topology, [DefaultValue("0")] int port, [DefaultValue("null")] string ip)
		{
			if (topology == null)
			{
				throw new NullReferenceException("topology is not defined");
			}
			NetworkTransport.CheckTopology(topology);
			int num;
			if (ip == null)
			{
				num = NetworkTransport.AddHostWrapperWithoutIp(new HostTopologyInternal(topology), port, 0, 0);
			}
			else
			{
				num = NetworkTransport.AddHostWrapper(new HostTopologyInternal(topology), ip, port, 0, 0);
			}
			return num;
		}

		[ExcludeFromDocs]
		public static int AddHostWithSimulator(HostTopology topology, int minTimeout, int maxTimeout, int port)
		{
			string text = null;
			return NetworkTransport.AddHostWithSimulator(topology, minTimeout, maxTimeout, port, text);
		}

		[ExcludeFromDocs]
		public static int AddHostWithSimulator(HostTopology topology, int minTimeout, int maxTimeout)
		{
			string text = null;
			int num = 0;
			return NetworkTransport.AddHostWithSimulator(topology, minTimeout, maxTimeout, num, text);
		}

		/// <summary>
		///   <para>Create a host and configure them to simulate Internet latency (works on Editor and development build only).</para>
		/// </summary>
		/// <param name="topology">The Networking.HostTopology associated with the host.</param>
		/// <param name="minTimeout">Minimum simulated delay in milliseconds.</param>
		/// <param name="maxTimeout">Maximum simulated delay in milliseconds.</param>
		/// <param name="port">Port to bind to (when 0 is selected, the OS will choose a port at random).</param>
		/// <param name="ip">IP address to bind to.</param>
		/// <returns>
		///   <para>Returns host ID just created.</para>
		/// </returns>
		public static int AddHostWithSimulator(HostTopology topology, int minTimeout, int maxTimeout, [DefaultValue("0")] int port, [DefaultValue("null")] string ip)
		{
			if (topology == null)
			{
				throw new NullReferenceException("topology is not defined");
			}
			int num;
			if (ip == null)
			{
				num = NetworkTransport.AddHostWrapperWithoutIp(new HostTopologyInternal(topology), port, minTimeout, maxTimeout);
			}
			else
			{
				num = NetworkTransport.AddHostWrapper(new HostTopologyInternal(topology), ip, port, minTimeout, maxTimeout);
			}
			return num;
		}

		/// <summary>
		///   <para>Closes the opened socket, and closes all connections belonging to that socket.</para>
		/// </summary>
		/// <param name="hostId">Host ID to remove.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RemoveHost(int hostId);

		/// <summary>
		///   <para>Deprecated.</para>
		/// </summary>
		public static extern bool IsStarted
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int Connect(int hostId, string address, int port, int exeptionConnectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_ConnectEndPoint(int hostId, IntPtr sockAddrStorage, int sockAddrStorageLen, int exceptionConnectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int ConnectWithSimulator(int hostId, string address, int port, int exeptionConnectionId, out byte error, ConnectionSimulatorConfig conf);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Disconnect(int hostId, int connectionId, out byte error);

		public static bool Send(int hostId, int connectionId, int channelId, byte[] buffer, int size, out byte error)
		{
			if (buffer == null)
			{
				throw new NullReferenceException("send buffer is not initialized");
			}
			return NetworkTransport.SendWrapper(hostId, connectionId, channelId, buffer, size, out error);
		}

		public static bool QueueMessageForSending(int hostId, int connectionId, int channelId, byte[] buffer, int size, out byte error)
		{
			if (buffer == null)
			{
				throw new NullReferenceException("send buffer is not initialized");
			}
			return NetworkTransport.QueueMessageForSendingWrapper(hostId, connectionId, channelId, buffer, size, out error);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SendQueuedMessages(int hostId, int connectionId, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SendWrapper(int hostId, int connectionId, int channelId, byte[] buffer, int size, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool QueueMessageForSendingWrapper(int hostId, int connectionId, int channelId, byte[] buffer, int size, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern NetworkEventType Receive(out int hostId, out int connectionId, out int channelId, byte[] buffer, int bufferSize, out int receivedSize, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern NetworkEventType ReceiveFromHost(int hostId, out int connectionId, out int channelId, byte[] buffer, int bufferSize, out int receivedSize, out byte error);

		/// <summary>
		///   <para>Used to inform the profiler of network packet statistics.</para>
		/// </summary>
		/// <param name="packetStatId">The ID of the message being reported.</param>
		/// <param name="numMsgs">Number of messages being reported.</param>
		/// <param name="numBytes">Number of bytes used by reported messages.</param>
		/// <param name="direction">Whether the packet is outgoing (-1) or incoming (0).</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetPacketStat(int direction, int packetStatId, int numMsgs, int numBytes);

		public static bool StartBroadcastDiscovery(int hostId, int broadcastPort, int key, int version, int subversion, byte[] buffer, int size, int timeout, out byte error)
		{
			if (buffer != null)
			{
				if (buffer.Length < size)
				{
					throw new ArgumentOutOfRangeException(string.Concat(new object[] { "Size: ", size, " > buffer.Length ", buffer.Length }));
				}
				if (size == 0)
				{
					throw new ArgumentOutOfRangeException("Size is zero while buffer exists, please pass null and 0 as buffer and size parameters");
				}
			}
			bool flag;
			if (buffer == null)
			{
				flag = NetworkTransport.StartBroadcastDiscoveryWithoutData(hostId, broadcastPort, key, version, subversion, timeout, out error);
			}
			else
			{
				flag = NetworkTransport.StartBroadcastDiscoveryWithData(hostId, broadcastPort, key, version, subversion, buffer, size, timeout, out error);
			}
			return flag;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool StartBroadcastDiscoveryWithoutData(int hostId, int broadcastPort, int key, int version, int subversion, int timeout, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool StartBroadcastDiscoveryWithData(int hostId, int broadcastPort, int key, int version, int subversion, byte[] buffer, int size, int timeout, out byte error);

		/// <summary>
		///   <para>Stop sending the broadcast discovery message.</para>
		/// </summary>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopBroadcastDiscovery();

		/// <summary>
		///   <para>Check if the broadcast discovery sender is running.</para>
		/// </summary>
		/// <returns>
		///   <para>True if it is running. False if it is not running.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsBroadcastDiscoveryRunning();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetBroadcastCredentials(int hostId, int key, int version, int subversion, out byte error);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetBroadcastConnectionInfo(int hostId, out int port, out byte error);

		public static void GetBroadcastConnectionInfo(int hostId, out string address, out int port, out byte error)
		{
			address = NetworkTransport.GetBroadcastConnectionInfo(hostId, out port, out error);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetBroadcastConnectionMessage(int hostId, byte[] buffer, int bufferSize, out int receivedSize, out byte error);

		internal static bool DoesEndPointUsePlatformProtocols(EndPoint endPoint)
		{
			if (endPoint.GetType().FullName == "UnityEngine.PS4.SceEndPoint" || endPoint.GetType().FullName == "UnityEngine.PSVita.SceEndPoint")
			{
				SocketAddress socketAddress = endPoint.Serialize();
				if (socketAddress[8] != 0 || socketAddress[9] != 0)
				{
					return true;
				}
			}
			return false;
		}

		public static int ConnectEndPoint(int hostId, EndPoint endPoint, int exceptionConnectionId, out byte error)
		{
			error = 0;
			byte[] array = new byte[] { 95, 36, 19, 246 };
			if (endPoint == null)
			{
				throw new NullReferenceException("Null EndPoint provided");
			}
			if (endPoint.GetType().FullName != "UnityEngine.XboxOne.XboxOneEndPoint" && endPoint.GetType().FullName != "UnityEngine.PS4.SceEndPoint" && endPoint.GetType().FullName != "UnityEngine.PSVita.SceEndPoint")
			{
				throw new ArgumentException("Endpoint of type XboxOneEndPoint or SceEndPoint  required");
			}
			int num;
			if (endPoint.GetType().FullName == "UnityEngine.XboxOne.XboxOneEndPoint")
			{
				if (endPoint.AddressFamily != AddressFamily.InterNetworkV6)
				{
					throw new ArgumentException("XboxOneEndPoint has an invalid family");
				}
				SocketAddress socketAddress = endPoint.Serialize();
				if (socketAddress.Size != 14)
				{
					throw new ArgumentException("XboxOneEndPoint has an invalid size");
				}
				if (socketAddress[0] != 0 || socketAddress[1] != 0)
				{
					throw new ArgumentException("XboxOneEndPoint has an invalid family signature");
				}
				if (socketAddress[2] != array[0] || socketAddress[3] != array[1] || socketAddress[4] != array[2] || socketAddress[5] != array[3])
				{
					throw new ArgumentException("XboxOneEndPoint has an invalid signature");
				}
				byte[] array2 = new byte[8];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = socketAddress[6 + i];
				}
				IntPtr intPtr = new IntPtr(BitConverter.ToInt64(array2, 0));
				if (intPtr == IntPtr.Zero)
				{
					throw new ArgumentException("XboxOneEndPoint has an invalid SOCKET_STORAGE pointer");
				}
				byte[] array3 = new byte[2];
				Marshal.Copy(intPtr, array3, 0, array3.Length);
				AddressFamily addressFamily = (AddressFamily)(((int)array3[1] << 8) + (int)array3[0]);
				if (addressFamily != AddressFamily.InterNetworkV6)
				{
					throw new ArgumentException("XboxOneEndPoint has corrupt or invalid SOCKET_STORAGE pointer");
				}
				num = NetworkTransport.Internal_ConnectEndPoint(hostId, intPtr, 128, exceptionConnectionId, out error);
			}
			else
			{
				SocketAddress socketAddress2 = endPoint.Serialize();
				if (socketAddress2.Size != 16)
				{
					throw new ArgumentException("EndPoint has an invalid size");
				}
				if ((int)socketAddress2[0] != socketAddress2.Size)
				{
					throw new ArgumentException("EndPoint has an invalid size value");
				}
				if (socketAddress2[1] != 2)
				{
					throw new ArgumentException("EndPoint has an invalid family value");
				}
				byte[] array4 = new byte[16];
				for (int j = 0; j < array4.Length; j++)
				{
					array4[j] = socketAddress2[j];
				}
				IntPtr intPtr2 = Marshal.AllocHGlobal(array4.Length);
				Marshal.Copy(array4, 0, intPtr2, array4.Length);
				int num2 = NetworkTransport.Internal_ConnectEndPoint(hostId, intPtr2, 16, exceptionConnectionId, out error);
				Marshal.FreeHGlobal(intPtr2);
				num = num2;
			}
			return num;
		}

		public static void Init(GlobalConfig config)
		{
			if (config.NetworkEventAvailable != null)
			{
				NetworkTransport.SetNetworkEventAvailableCallback(config.NetworkEventAvailable);
			}
			if (config.ConnectionReadyForSend != null)
			{
				NetworkTransport.SetConnectionReadyForSendCallback(config.ConnectionReadyForSend);
			}
			NetworkTransport.InitWithParameters(new GlobalConfigInternal(config));
		}

		[NativeThrows]
		[FreeFunction("UNETManager::SetNetworkEventAvailableCallback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetNetworkEventAvailableCallback(Action<int> callback);

		[FreeFunction("UNETManager::SetConnectionReadyForSendCallback")]
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetConnectionReadyForSendCallback(Action<int, int> callback);

		[FreeFunction("UNETManager::Get()->NotifyWhenConnectionReadyForSend")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool NotifyWhenConnectionReadyForSend(int hostId, int connectionId, int notificationLevel, out byte error);

		/// <summary>
		///   <para>Returns the port number assigned to the host.</para>
		/// </summary>
		/// <param name="hostId">Host ID.</param>
		/// <returns>
		///   <para>The UDP port number, or -1 if an error occurred.</para>
		/// </returns>
		[FreeFunction("UNETManager::Get()->GetHostPort")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetHostPort(int hostId);
	}
}
