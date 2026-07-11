using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking
{
	[NativeHeader("Runtime/Networking/UNETManager.h")]
	[NativeHeader("Runtime/Networking/UNetTypes.h")]
	[NativeConditional("ENABLE_NETWORK && ENABLE_UNET", true)]
	[NativeHeader("Runtime/Networking/UNETConfiguration.h")]
	[Obsolete("The UNET transport will be removed in the future as soon a replacement is ready.")]
	public sealed class NetworkTransport
	{
		private NetworkTransport()
		{
		}

		internal static bool DoesEndPointUsePlatformProtocols(EndPoint endPoint)
		{
			if (endPoint.GetType().FullName == "UnityEngine.PS4.SceEndPoint")
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
			if (endPoint.GetType().FullName != "UnityEngine.XboxOne.XboxOneEndPoint" && endPoint.GetType().FullName != "UnityEngine.PS4.SceEndPoint")
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
				num = NetworkTransport.Internal_ConnectEndPoint(hostId, array2, 128, exceptionConnectionId, out error);
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
				int num2 = NetworkTransport.Internal_ConnectEndPoint(hostId, array4, 16, exceptionConnectionId, out error);
				num = num2;
			}
			return num;
		}

		public static void Init()
		{
			NetworkTransport.InitializeClass();
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
			NetworkTransport.InitializeClassWithConfig(new GlobalConfigInternal(config));
		}

		[FreeFunction("UNETManager::InitializeClass")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitializeClass();

		[FreeFunction("UNETManager::InitializeClassWithConfig")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitializeClassWithConfig(GlobalConfigInternal config);

		public static void Shutdown()
		{
			NetworkTransport.Cleanup();
		}

		[Obsolete("This function has been deprecated. Use AssetDatabase utilities instead.")]
		public static string GetAssetId(GameObject go)
		{
			return "";
		}

		public static void AddSceneId(int id)
		{
			if (id > NetworkTransport.s_nextSceneId)
			{
				NetworkTransport.s_nextSceneId = id + 1;
			}
		}

		public static int GetNextSceneId()
		{
			return NetworkTransport.s_nextSceneId++;
		}

		public static int AddHostWithSimulator(HostTopology topology, int minTimeout, int maxTimeout, int port, string ip)
		{
			if (topology == null)
			{
				throw new NullReferenceException("topology is not defined");
			}
			NetworkTransport.CheckTopology(topology);
			return NetworkTransport.AddHostInternal(new HostTopologyInternal(topology), ip, port, minTimeout, maxTimeout);
		}

		public static int AddHostWithSimulator(HostTopology topology, int minTimeout, int maxTimeout, int port)
		{
			return NetworkTransport.AddHostWithSimulator(topology, minTimeout, maxTimeout, port, null);
		}

		public static int AddHostWithSimulator(HostTopology topology, int minTimeout, int maxTimeout)
		{
			return NetworkTransport.AddHostWithSimulator(topology, minTimeout, maxTimeout, 0, null);
		}

		public static int AddHost(HostTopology topology, int port, string ip)
		{
			return NetworkTransport.AddHostWithSimulator(topology, 0, 0, port, ip);
		}

		public static int AddHost(HostTopology topology, int port)
		{
			return NetworkTransport.AddHost(topology, port, null);
		}

		public static int AddHost(HostTopology topology)
		{
			return NetworkTransport.AddHost(topology, 0, null);
		}

		[FreeFunction("UNETManager::Get()->AddHost")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddHostInternal(HostTopologyInternal topologyInt, string ip, int port, int minTimeout, int maxTimeout);

		public static int AddWebsocketHost(HostTopology topology, int port, string ip)
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
			return NetworkTransport.AddWsHostInternal(new HostTopologyInternal(topology), ip, port);
		}

		public static int AddWebsocketHost(HostTopology topology, int port)
		{
			return NetworkTransport.AddWebsocketHost(topology, port, null);
		}

		[FreeFunction("UNETManager::Get()->AddWsHost")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddWsHostInternal(HostTopologyInternal topologyInt, string ip, int port);

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

		public static void ConnectAsNetworkHost(int hostId, string address, int port, NetworkID network, SourceID source, NodeID node, out byte error)
		{
			NetworkTransport.ConnectAsNetworkHostInternal(hostId, address, port, (ulong)network, (ulong)source, (ushort)node, out error);
		}

		[FreeFunction("UNETManager::Get()->ConnectAsNetworkHost")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ConnectAsNetworkHostInternal(int hostId, string address, int port, ulong network, ulong source, ushort node, out byte error);

		[FreeFunction("UNETManager::Get()->DisconnectNetworkHost")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisconnectNetworkHost(int hostId, out byte error);

		public static NetworkEventType ReceiveRelayEventFromHost(int hostId, out byte error)
		{
			return (NetworkEventType)NetworkTransport.ReceiveRelayEventFromHostInternal(hostId, out error);
		}

		[FreeFunction("UNETManager::Get()->PopRelayHostData")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ReceiveRelayEventFromHostInternal(int hostId, out byte error);

		public static int ConnectToNetworkPeer(int hostId, string address, int port, int exceptionConnectionId, int relaySlotId, NetworkID network, SourceID source, NodeID node, int bytesPerSec, float bucketSizeFactor, out byte error)
		{
			return NetworkTransport.ConnectToNetworkPeerInternal(hostId, address, port, exceptionConnectionId, relaySlotId, (ulong)network, (ulong)source, (ushort)node, bytesPerSec, bucketSizeFactor, out error);
		}

		public static int ConnectToNetworkPeer(int hostId, string address, int port, int exceptionConnectionId, int relaySlotId, NetworkID network, SourceID source, NodeID node, out byte error)
		{
			return NetworkTransport.ConnectToNetworkPeer(hostId, address, port, exceptionConnectionId, relaySlotId, network, source, node, 0, 0f, out error);
		}

		[FreeFunction("UNETManager::Get()->ConnectToNetworkPeer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ConnectToNetworkPeerInternal(int hostId, string address, int port, int exceptionConnectionId, int relaySlotId, ulong network, ulong source, ushort node, int bytesPerSec, float bucketSizeFactor, out byte error);

		[Obsolete("GetCurrentIncomingMessageAmount has been deprecated.")]
		public static int GetCurrentIncomingMessageAmount()
		{
			return 0;
		}

		[Obsolete("GetCurrentOutgoingMessageAmount has been deprecated.")]
		public static int GetCurrentOutgoingMessageAmount()
		{
			return 0;
		}

		[FreeFunction("UNETManager::Get()->GetIncomingMessageQueueSize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetIncomingMessageQueueSize(int hostId, out byte error);

		[FreeFunction("UNETManager::Get()->GetOutgoingMessageQueueSize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingMessageQueueSize(int hostId, out byte error);

		[FreeFunction("UNETManager::Get()->GetCurrentRTT")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCurrentRTT(int hostId, int connectionId, out byte error);

		[Obsolete("GetCurrentRtt() has been deprecated.")]
		public static int GetCurrentRtt(int hostId, int connectionId, out byte error)
		{
			return NetworkTransport.GetCurrentRTT(hostId, connectionId, out error);
		}

		[FreeFunction("UNETManager::Get()->GetIncomingPacketLossCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetIncomingPacketLossCount(int hostId, int connectionId, out byte error);

		[Obsolete("GetNetworkLostPacketNum() has been deprecated.")]
		public static int GetNetworkLostPacketNum(int hostId, int connectionId, out byte error)
		{
			return NetworkTransport.GetIncomingPacketLossCount(hostId, connectionId, out error);
		}

		[FreeFunction("UNETManager::Get()->GetIncomingPacketCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetIncomingPacketCount(int hostId, int connectionId, out byte error);

		[FreeFunction("UNETManager::Get()->GetOutgoingPacketNetworkLossPercent")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingPacketNetworkLossPercent(int hostId, int connectionId, out byte error);

		[FreeFunction("UNETManager::Get()->GetOutgoingPacketOverflowLossPercent")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingPacketOverflowLossPercent(int hostId, int connectionId, out byte error);

		[FreeFunction("UNETManager::Get()->GetMaxAllowedBandwidth")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetMaxAllowedBandwidth(int hostId, int connectionId, out byte error);

		[FreeFunction("UNETManager::Get()->GetAckBufferCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetAckBufferCount(int hostId, int connectionId, out byte error);

		[FreeFunction("UNETManager::Get()->GetIncomingPacketDropCountForAllHosts")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetIncomingPacketDropCountForAllHosts();

		[FreeFunction("UNETManager::Get()->GetIncomingPacketCountForAllHosts")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetIncomingPacketCountForAllHosts();

		[FreeFunction("UNETManager::Get()->GetOutgoingPacketCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingPacketCount();

		[FreeFunction("UNETManager::Get()->GetOutgoingPacketCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingPacketCountForHost(int hostId, out byte error);

		[FreeFunction("UNETManager::Get()->GetOutgoingPacketCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingPacketCountForConnection(int hostId, int connectionId, out byte error);

		[FreeFunction("UNETManager::Get()->GetOutgoingMessageCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingMessageCount();

		[FreeFunction("UNETManager::Get()->GetOutgoingMessageCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingMessageCountForHost(int hostId, out byte error);

		[FreeFunction("UNETManager::Get()->GetOutgoingMessageCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingMessageCountForConnection(int hostId, int connectionId, out byte error);

		[FreeFunction("UNETManager::Get()->GetOutgoingUserBytesCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingUserBytesCount();

		[FreeFunction("UNETManager::Get()->GetOutgoingUserBytesCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingUserBytesCountForHost(int hostId, out byte error);

		[FreeFunction("UNETManager::Get()->GetOutgoingUserBytesCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingUserBytesCountForConnection(int hostId, int connectionId, out byte error);

		[FreeFunction("UNETManager::Get()->GetOutgoingSystemBytesCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingSystemBytesCount();

		[FreeFunction("UNETManager::Get()->GetOutgoingSystemBytesCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingSystemBytesCountForHost(int hostId, out byte error);

		[FreeFunction("UNETManager::Get()->GetOutgoingSystemBytesCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingSystemBytesCountForConnection(int hostId, int connectionId, out byte error);

		[FreeFunction("UNETManager::Get()->GetOutgoingFullBytesCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingFullBytesCount();

		[FreeFunction("UNETManager::Get()->GetOutgoingFullBytesCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingFullBytesCountForHost(int hostId, out byte error);

		[FreeFunction("UNETManager::Get()->GetOutgoingFullBytesCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetOutgoingFullBytesCountForConnection(int hostId, int connectionId, out byte error);

		[Obsolete("GetPacketSentRate has been deprecated.")]
		public static int GetPacketSentRate(int hostId, int connectionId, out byte error)
		{
			error = 0;
			return 0;
		}

		[Obsolete("GetPacketReceivedRate has been deprecated.")]
		public static int GetPacketReceivedRate(int hostId, int connectionId, out byte error)
		{
			error = 0;
			return 0;
		}

		[Obsolete("GetRemotePacketReceivedRate has been deprecated.")]
		public static int GetRemotePacketReceivedRate(int hostId, int connectionId, out byte error)
		{
			error = 0;
			return 0;
		}

		[Obsolete("GetNetIOTimeuS has been deprecated.")]
		public static int GetNetIOTimeuS()
		{
			return 0;
		}

		[FreeFunction("UNETManager::Get()->GetConnectionInfo")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetConnectionInfo(int hostId, int connectionId, out int port, out ulong network, out ushort dstNode, out byte error);

		public static void GetConnectionInfo(int hostId, int connectionId, out string address, out int port, out NetworkID network, out NodeID dstNode, out byte error)
		{
			ulong num;
			ushort num2;
			address = NetworkTransport.GetConnectionInfo(hostId, connectionId, out port, out num, out num2, out error);
			network = (NetworkID)num;
			dstNode = (NodeID)num2;
		}

		[FreeFunction("UNETManager::Get()->GetNetworkTimestamp")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNetworkTimestamp();

		[FreeFunction("UNETManager::Get()->GetRemoteDelayTimeMS")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetRemoteDelayTimeMS(int hostId, int connectionId, int remoteTime, out byte error);

		public static bool StartSendMulticast(int hostId, int channelId, byte[] buffer, int size, out byte error)
		{
			return NetworkTransport.StartSendMulticastInternal(hostId, channelId, buffer, size, out error);
		}

		[FreeFunction("UNETManager::Get()->StartSendMulticast")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool StartSendMulticastInternal(int hostId, int channelId, [Out] byte[] buffer, int size, out byte error);

		[FreeFunction("UNETManager::Get()->SendMulticast")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SendMulticast(int hostId, int connectionId, out byte error);

		[FreeFunction("UNETManager::Get()->FinishSendMulticast")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool FinishSendMulticast(int hostId, out byte error);

		[FreeFunction("UNETManager::Get()->GetMaxPacketSize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMaxPacketSize();

		[FreeFunction("UNETManager::Get()->RemoveHost")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RemoveHost(int hostId);

		public static bool IsStarted
		{
			get
			{
				return NetworkTransport.IsStartedInternal();
			}
		}

		[FreeFunction("UNETManager::IsStarted")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsStartedInternal();

		[FreeFunction("UNETManager::Get()->Connect")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int Connect(int hostId, string address, int port, int exeptionConnectionId, out byte error);

		[FreeFunction("UNETManager::Get()->ConnectWithSimulator")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ConnectWithSimulatorInternal(int hostId, string address, int port, int exeptionConnectionId, out byte error, ConnectionSimulatorConfigInternal conf);

		public static int ConnectWithSimulator(int hostId, string address, int port, int exeptionConnectionId, out byte error, ConnectionSimulatorConfig conf)
		{
			return NetworkTransport.ConnectWithSimulatorInternal(hostId, address, port, exeptionConnectionId, out error, new ConnectionSimulatorConfigInternal(conf));
		}

		[FreeFunction("UNETManager::Get()->Disconnect")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Disconnect(int hostId, int connectionId, out byte error);

		[FreeFunction("UNETManager::Get()->ConnectSockAddr")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_ConnectEndPoint(int hostId, [Out] byte[] sockAddrStorage, int sockAddrStorageLen, int exceptionConnectionId, out byte error);

		public static bool Send(int hostId, int connectionId, int channelId, byte[] buffer, int size, out byte error)
		{
			if (buffer == null)
			{
				throw new NullReferenceException("send buffer is not initialized");
			}
			return NetworkTransport.SendWrapper(hostId, connectionId, channelId, buffer, size, out error);
		}

		[FreeFunction("UNETManager::Get()->Send")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SendWrapper(int hostId, int connectionId, int channelId, [Out] byte[] buffer, int size, out byte error);

		public static bool QueueMessageForSending(int hostId, int connectionId, int channelId, byte[] buffer, int size, out byte error)
		{
			if (buffer == null)
			{
				throw new NullReferenceException("send buffer is not initialized");
			}
			return NetworkTransport.QueueMessageForSendingWrapper(hostId, connectionId, channelId, buffer, size, out error);
		}

		[FreeFunction("UNETManager::Get()->QueueMessageForSending")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool QueueMessageForSendingWrapper(int hostId, int connectionId, int channelId, [Out] byte[] buffer, int size, out byte error);

		[FreeFunction("UNETManager::Get()->SendQueuedMessages")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SendQueuedMessages(int hostId, int connectionId, out byte error);

		public static NetworkEventType Receive(out int hostId, out int connectionId, out int channelId, byte[] buffer, int bufferSize, out int receivedSize, out byte error)
		{
			return (NetworkEventType)NetworkTransport.PopData(out hostId, out connectionId, out channelId, buffer, bufferSize, out receivedSize, out error);
		}

		[FreeFunction("UNETManager::Get()->PopData")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopData(out int hostId, out int connectionId, out int channelId, [Out] byte[] buffer, int bufferSize, out int receivedSize, out byte error);

		public static NetworkEventType ReceiveFromHost(int hostId, out int connectionId, out int channelId, byte[] buffer, int bufferSize, out int receivedSize, out byte error)
		{
			return (NetworkEventType)NetworkTransport.PopDataFromHost(hostId, out connectionId, out channelId, buffer, bufferSize, out receivedSize, out error);
		}

		[FreeFunction("UNETManager::Get()->PopDataFromHost")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PopDataFromHost(int hostId, out int connectionId, out int channelId, [Out] byte[] buffer, int bufferSize, out int receivedSize, out byte error);

		[FreeFunction("UNETManager::Get()->SetPacketStat")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetPacketStat(int direction, int packetStatId, int numMsgs, int numBytes);

		[NativeThrows]
		[FreeFunction("UNETManager::SetNetworkEventAvailableCallback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetNetworkEventAvailableCallback(Action<int> callback);

		[FreeFunction("UNETManager::Cleanup")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Cleanup();

		[NativeThrows]
		[FreeFunction("UNETManager::SetConnectionReadyForSendCallback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetConnectionReadyForSendCallback(Action<int, int> callback);

		[FreeFunction("UNETManager::Get()->NotifyWhenConnectionReadyForSend")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool NotifyWhenConnectionReadyForSend(int hostId, int connectionId, int notificationLevel, out byte error);

		[FreeFunction("UNETManager::Get()->GetHostPort")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetHostPort(int hostId);

		[FreeFunction("UNETManager::Get()->StartBroadcastDiscoveryWithData")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool StartBroadcastDiscoveryWithData(int hostId, int broadcastPort, int key, int version, int subversion, [Out] byte[] buffer, int size, int timeout, out byte error);

		[FreeFunction("UNETManager::Get()->StartBroadcastDiscoveryWithoutData")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool StartBroadcastDiscoveryWithoutData(int hostId, int broadcastPort, int key, int version, int subversion, int timeout, out byte error);

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

		[FreeFunction("UNETManager::Get()->StopBroadcastDiscovery")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopBroadcastDiscovery();

		[FreeFunction("UNETManager::Get()->IsBroadcastDiscoveryRunning")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsBroadcastDiscoveryRunning();

		[FreeFunction("UNETManager::Get()->SetBroadcastCredentials")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetBroadcastCredentials(int hostId, int key, int version, int subversion, out byte error);

		[FreeFunction("UNETManager::Get()->GetBroadcastConnectionInfoInternal")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetBroadcastConnectionInfo(int hostId, out int port, out byte error);

		public static void GetBroadcastConnectionInfo(int hostId, out string address, out int port, out byte error)
		{
			address = NetworkTransport.GetBroadcastConnectionInfo(hostId, out port, out error);
		}

		public static void GetBroadcastConnectionMessage(int hostId, byte[] buffer, int bufferSize, out int receivedSize, out byte error)
		{
			NetworkTransport.GetBroadcastConnectionMessageInternal(hostId, buffer, bufferSize, out receivedSize, out error);
		}

		[FreeFunction("UNETManager::Get()->GetBroadcastConnectionMessage")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBroadcastConnectionMessageInternal(int hostId, [Out] byte[] buffer, int bufferSize, out int receivedSize, out byte error);

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

		private static int s_nextSceneId = 1;
	}
}
