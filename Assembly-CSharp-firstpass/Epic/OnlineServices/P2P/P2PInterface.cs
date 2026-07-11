using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	public sealed class P2PInterface : Handle
	{
		public P2PInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result SendPacket(SendPacketOptions options)
		{
			SendPacketOptionsInternal sendPacketOptionsInternal = Helper.CopyProperties<SendPacketOptionsInternal>(options);
			Result result = P2PInterface.EOS_P2P_SendPacket(base.InnerHandle, ref sendPacketOptionsInternal);
			Helper.TryMarshalDispose<SendPacketOptionsInternal>(ref sendPacketOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result GetNextReceivedPacketSize(GetNextReceivedPacketSizeOptions options, out uint outPacketSizeBytes)
		{
			GetNextReceivedPacketSizeOptionsInternal getNextReceivedPacketSizeOptionsInternal = Helper.CopyProperties<GetNextReceivedPacketSizeOptionsInternal>(options);
			outPacketSizeBytes = Helper.GetDefault<uint>();
			Result result = P2PInterface.EOS_P2P_GetNextReceivedPacketSize(base.InnerHandle, ref getNextReceivedPacketSizeOptionsInternal, ref outPacketSizeBytes);
			Helper.TryMarshalDispose<GetNextReceivedPacketSizeOptionsInternal>(ref getNextReceivedPacketSizeOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result ReceivePacket(ReceivePacketOptions options, out ProductUserId outPeerId, out SocketId outSocketId, out byte outChannel, ref byte[] outData, out uint outBytesWritten)
		{
			ReceivePacketOptionsInternal receivePacketOptionsInternal = Helper.CopyProperties<ReceivePacketOptionsInternal>(options);
			outPeerId = Helper.GetDefault<ProductUserId>();
			IntPtr zero = IntPtr.Zero;
			outSocketId = Helper.GetDefault<SocketId>();
			SocketIdInternal socketIdInternal = default(SocketIdInternal);
			outChannel = Helper.GetDefault<byte>();
			outBytesWritten = Helper.GetDefault<uint>();
			Result result = P2PInterface.EOS_P2P_ReceivePacket(base.InnerHandle, ref receivePacketOptionsInternal, ref zero, ref socketIdInternal, ref outChannel, outData, ref outBytesWritten);
			Helper.TryMarshalDispose<ReceivePacketOptionsInternal>(ref receivePacketOptionsInternal);
			Helper.TryMarshalGet<ProductUserId>(zero, out outPeerId);
			outSocketId = Helper.CopyProperties<SocketId>(socketIdInternal);
			Helper.TryMarshalDispose<SocketIdInternal>(ref socketIdInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public ulong AddNotifyPeerConnectionRequest(AddNotifyPeerConnectionRequestOptions options, object clientData, OnIncomingConnectionRequestCallback connectionRequestHandler)
		{
			AddNotifyPeerConnectionRequestOptionsInternal addNotifyPeerConnectionRequestOptionsInternal = Helper.CopyProperties<AddNotifyPeerConnectionRequestOptionsInternal>(options);
			OnIncomingConnectionRequestCallbackInternal onIncomingConnectionRequestCallbackInternal = new OnIncomingConnectionRequestCallbackInternal(P2PInterface.OnIncomingConnectionRequest);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, connectionRequestHandler, onIncomingConnectionRequestCallbackInternal, Array.Empty<Delegate>());
			ulong num = P2PInterface.EOS_P2P_AddNotifyPeerConnectionRequest(base.InnerHandle, ref addNotifyPeerConnectionRequestOptionsInternal, zero, onIncomingConnectionRequestCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyPeerConnectionRequestOptionsInternal>(ref addNotifyPeerConnectionRequestOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyPeerConnectionRequest(ulong notificationId)
		{
			Helper.TryRemoveCallbackByNotificationId(notificationId);
			P2PInterface.EOS_P2P_RemoveNotifyPeerConnectionRequest(base.InnerHandle, notificationId);
		}

		public ulong AddNotifyPeerConnectionClosed(AddNotifyPeerConnectionClosedOptions options, object clientData, OnRemoteConnectionClosedCallback connectionClosedHandler)
		{
			AddNotifyPeerConnectionClosedOptionsInternal addNotifyPeerConnectionClosedOptionsInternal = Helper.CopyProperties<AddNotifyPeerConnectionClosedOptionsInternal>(options);
			OnRemoteConnectionClosedCallbackInternal onRemoteConnectionClosedCallbackInternal = new OnRemoteConnectionClosedCallbackInternal(P2PInterface.OnRemoteConnectionClosed);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, connectionClosedHandler, onRemoteConnectionClosedCallbackInternal, Array.Empty<Delegate>());
			ulong num = P2PInterface.EOS_P2P_AddNotifyPeerConnectionClosed(base.InnerHandle, ref addNotifyPeerConnectionClosedOptionsInternal, zero, onRemoteConnectionClosedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyPeerConnectionClosedOptionsInternal>(ref addNotifyPeerConnectionClosedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyPeerConnectionClosed(ulong notificationId)
		{
			Helper.TryRemoveCallbackByNotificationId(notificationId);
			P2PInterface.EOS_P2P_RemoveNotifyPeerConnectionClosed(base.InnerHandle, notificationId);
		}

		public Result AcceptConnection(AcceptConnectionOptions options)
		{
			AcceptConnectionOptionsInternal acceptConnectionOptionsInternal = Helper.CopyProperties<AcceptConnectionOptionsInternal>(options);
			Result result = P2PInterface.EOS_P2P_AcceptConnection(base.InnerHandle, ref acceptConnectionOptionsInternal);
			Helper.TryMarshalDispose<AcceptConnectionOptionsInternal>(ref acceptConnectionOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CloseConnection(CloseConnectionOptions options)
		{
			CloseConnectionOptionsInternal closeConnectionOptionsInternal = Helper.CopyProperties<CloseConnectionOptionsInternal>(options);
			Result result = P2PInterface.EOS_P2P_CloseConnection(base.InnerHandle, ref closeConnectionOptionsInternal);
			Helper.TryMarshalDispose<CloseConnectionOptionsInternal>(ref closeConnectionOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CloseConnections(CloseConnectionsOptions options)
		{
			CloseConnectionsOptionsInternal closeConnectionsOptionsInternal = Helper.CopyProperties<CloseConnectionsOptionsInternal>(options);
			Result result = P2PInterface.EOS_P2P_CloseConnections(base.InnerHandle, ref closeConnectionsOptionsInternal);
			Helper.TryMarshalDispose<CloseConnectionsOptionsInternal>(ref closeConnectionsOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void QueryNATType(QueryNATTypeOptions options, object clientData, OnQueryNATTypeCompleteCallback nATTypeQueriedHandler)
		{
			QueryNATTypeOptionsInternal queryNATTypeOptionsInternal = Helper.CopyProperties<QueryNATTypeOptionsInternal>(options);
			OnQueryNATTypeCompleteCallbackInternal onQueryNATTypeCompleteCallbackInternal = new OnQueryNATTypeCompleteCallbackInternal(P2PInterface.OnQueryNATTypeComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, nATTypeQueriedHandler, onQueryNATTypeCompleteCallbackInternal, Array.Empty<Delegate>());
			P2PInterface.EOS_P2P_QueryNATType(base.InnerHandle, ref queryNATTypeOptionsInternal, zero, onQueryNATTypeCompleteCallbackInternal);
			Helper.TryMarshalDispose<QueryNATTypeOptionsInternal>(ref queryNATTypeOptionsInternal);
		}

		public Result GetNATType(GetNATTypeOptions options, out NATType outNATType)
		{
			GetNATTypeOptionsInternal getNATTypeOptionsInternal = Helper.CopyProperties<GetNATTypeOptionsInternal>(options);
			outNATType = Helper.GetDefault<NATType>();
			Result result = P2PInterface.EOS_P2P_GetNATType(base.InnerHandle, ref getNATTypeOptionsInternal, ref outNATType);
			Helper.TryMarshalDispose<GetNATTypeOptionsInternal>(ref getNATTypeOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetRelayControl(SetRelayControlOptions options)
		{
			SetRelayControlOptionsInternal setRelayControlOptionsInternal = Helper.CopyProperties<SetRelayControlOptionsInternal>(options);
			Result result = P2PInterface.EOS_P2P_SetRelayControl(base.InnerHandle, ref setRelayControlOptionsInternal);
			Helper.TryMarshalDispose<SetRelayControlOptionsInternal>(ref setRelayControlOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result GetRelayControl(GetRelayControlOptions options, out RelayControl outRelayControl)
		{
			GetRelayControlOptionsInternal getRelayControlOptionsInternal = Helper.CopyProperties<GetRelayControlOptionsInternal>(options);
			outRelayControl = Helper.GetDefault<RelayControl>();
			Result result = P2PInterface.EOS_P2P_GetRelayControl(base.InnerHandle, ref getRelayControlOptionsInternal, ref outRelayControl);
			Helper.TryMarshalDispose<GetRelayControlOptionsInternal>(ref getRelayControlOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetPortRange(SetPortRangeOptions options)
		{
			SetPortRangeOptionsInternal setPortRangeOptionsInternal = Helper.CopyProperties<SetPortRangeOptionsInternal>(options);
			Result result = P2PInterface.EOS_P2P_SetPortRange(base.InnerHandle, ref setPortRangeOptionsInternal);
			Helper.TryMarshalDispose<SetPortRangeOptionsInternal>(ref setPortRangeOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result GetPortRange(GetPortRangeOptions options, out ushort outPort, out ushort outNumAdditionalPortsToTry)
		{
			GetPortRangeOptionsInternal getPortRangeOptionsInternal = Helper.CopyProperties<GetPortRangeOptionsInternal>(options);
			outPort = Helper.GetDefault<ushort>();
			outNumAdditionalPortsToTry = Helper.GetDefault<ushort>();
			Result result = P2PInterface.EOS_P2P_GetPortRange(base.InnerHandle, ref getPortRangeOptionsInternal, ref outPort, ref outNumAdditionalPortsToTry);
			Helper.TryMarshalDispose<GetPortRangeOptionsInternal>(ref getPortRangeOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static void OnQueryNATTypeComplete(IntPtr address)
		{
			OnQueryNATTypeCompleteCallback onQueryNATTypeCompleteCallback = null;
			OnQueryNATTypeCompleteInfo onQueryNATTypeCompleteInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryNATTypeCompleteCallback, OnQueryNATTypeCompleteInfoInternal, OnQueryNATTypeCompleteInfo>(address, out onQueryNATTypeCompleteCallback, out onQueryNATTypeCompleteInfo))
			{
				onQueryNATTypeCompleteCallback(onQueryNATTypeCompleteInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnRemoteConnectionClosed(IntPtr address)
		{
			OnRemoteConnectionClosedCallback onRemoteConnectionClosedCallback = null;
			OnRemoteConnectionClosedInfo onRemoteConnectionClosedInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnRemoteConnectionClosedCallback, OnRemoteConnectionClosedInfoInternal, OnRemoteConnectionClosedInfo>(address, out onRemoteConnectionClosedCallback, out onRemoteConnectionClosedInfo))
			{
				onRemoteConnectionClosedCallback(onRemoteConnectionClosedInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnIncomingConnectionRequest(IntPtr address)
		{
			OnIncomingConnectionRequestCallback onIncomingConnectionRequestCallback = null;
			OnIncomingConnectionRequestInfo onIncomingConnectionRequestInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnIncomingConnectionRequestCallback, OnIncomingConnectionRequestInfoInternal, OnIncomingConnectionRequestInfo>(address, out onIncomingConnectionRequestCallback, out onIncomingConnectionRequestInfo))
			{
				onIncomingConnectionRequestCallback(onIncomingConnectionRequestInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_P2P_GetPortRange(IntPtr handle, ref GetPortRangeOptionsInternal options, ref ushort outPort, ref ushort outNumAdditionalPortsToTry);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_P2P_SetPortRange(IntPtr handle, ref SetPortRangeOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_P2P_GetRelayControl(IntPtr handle, ref GetRelayControlOptionsInternal options, ref RelayControl outRelayControl);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_P2P_SetRelayControl(IntPtr handle, ref SetRelayControlOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_P2P_GetNATType(IntPtr handle, ref GetNATTypeOptionsInternal options, ref NATType outNATType);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_P2P_QueryNATType(IntPtr handle, ref QueryNATTypeOptionsInternal options, IntPtr clientData, OnQueryNATTypeCompleteCallbackInternal nATTypeQueriedHandler);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_P2P_CloseConnections(IntPtr handle, ref CloseConnectionsOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_P2P_CloseConnection(IntPtr handle, ref CloseConnectionOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_P2P_AcceptConnection(IntPtr handle, ref AcceptConnectionOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_P2P_RemoveNotifyPeerConnectionClosed(IntPtr handle, ulong notificationId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_P2P_AddNotifyPeerConnectionClosed(IntPtr handle, ref AddNotifyPeerConnectionClosedOptionsInternal options, IntPtr clientData, OnRemoteConnectionClosedCallbackInternal connectionClosedHandler);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_P2P_RemoveNotifyPeerConnectionRequest(IntPtr handle, ulong notificationId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_P2P_AddNotifyPeerConnectionRequest(IntPtr handle, ref AddNotifyPeerConnectionRequestOptionsInternal options, IntPtr clientData, OnIncomingConnectionRequestCallbackInternal connectionRequestHandler);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_P2P_ReceivePacket(IntPtr handle, ref ReceivePacketOptionsInternal options, ref IntPtr outPeerId, ref SocketIdInternal outSocketId, ref byte outChannel, byte[] outData, ref uint outBytesWritten);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_P2P_GetNextReceivedPacketSize(IntPtr handle, ref GetNextReceivedPacketSizeOptionsInternal options, ref uint outPacketSizeBytes);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_P2P_SendPacket(IntPtr handle, ref SendPacketOptionsInternal options);

		public const int GetportrangeApiLatest = 1;

		public const int SetportrangeApiLatest = 1;

		public const int GetrelaycontrolApiLatest = 1;

		public const int SetrelaycontrolApiLatest = 1;

		public const int GetnattypeApiLatest = 1;

		public const int QuerynattypeApiLatest = 1;

		public const int CloseconnectionsApiLatest = 1;

		public const int CloseconnectionApiLatest = 1;

		public const int AcceptconnectionApiLatest = 1;

		public const int AddnotifypeerconnectionclosedApiLatest = 1;

		public const int AddnotifypeerconnectionrequestApiLatest = 1;

		public const int ReceivepacketApiLatest = 2;

		public const int GetnextreceivedpacketsizeApiLatest = 2;

		public const int SendpacketApiLatest = 2;

		public const int SocketidApiLatest = 1;

		public const int MaxConnections = 32;

		public const int MaxPacketSize = 1170;
	}
}
