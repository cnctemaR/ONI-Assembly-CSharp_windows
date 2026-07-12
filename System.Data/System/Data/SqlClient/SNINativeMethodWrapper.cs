using System;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace System.Data.SqlClient
{
	internal static class SNINativeMethodWrapper
	{
		internal static int SniMaxComposedSpnLength
		{
			get
			{
				if (SNINativeMethodWrapper.s_sniMaxComposedSpnLength == -1)
				{
					SNINativeMethodWrapper.s_sniMaxComposedSpnLength = checked((int)SNINativeMethodWrapper.GetSniMaxComposedSpnLength());
				}
				return SNINativeMethodWrapper.s_sniMaxComposedSpnLength;
			}
		}

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIAddProviderWrapper")]
		internal static extern uint SNIAddProvider(SNIHandle pConn, SNINativeMethodWrapper.ProviderEnum ProvNum, [In] ref uint pInfo);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNICheckConnectionWrapper")]
		internal static extern uint SNICheckConnection([In] SNIHandle pConn);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNICloseWrapper")]
		internal static extern uint SNIClose(IntPtr pConn);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SNIGetLastError(out SNINativeMethodWrapper.SNI_Error pErrorStruct);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SNIPacketRelease(IntPtr pPacket);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIPacketResetWrapper")]
		internal static extern void SNIPacketReset([In] SNIHandle pConn, SNINativeMethodWrapper.IOType IOType, SNIPacket pPacket, SNINativeMethodWrapper.ConsumerNumber ConsNum);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint SNIQueryInfo(SNINativeMethodWrapper.QTypes QType, ref uint pbQInfo);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint SNIQueryInfo(SNINativeMethodWrapper.QTypes QType, ref IntPtr pbQInfo);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIReadAsyncWrapper")]
		internal static extern uint SNIReadAsync(SNIHandle pConn, ref IntPtr ppNewPacket);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint SNIReadSyncOverAsync(SNIHandle pConn, ref IntPtr ppNewPacket, int timeout);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIRemoveProviderWrapper")]
		internal static extern uint SNIRemoveProvider(SNIHandle pConn, SNINativeMethodWrapper.ProviderEnum ProvNum);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint SNISecInitPackage(ref uint pcbMaxToken);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNISetInfoWrapper")]
		internal static extern uint SNISetInfo(SNIHandle pConn, SNINativeMethodWrapper.QTypes QType, [In] ref uint pbQInfo);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint SNITerminate();

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIWaitForSSLHandshakeToCompleteWrapper")]
		internal static extern uint SNIWaitForSSLHandshakeToComplete([In] SNIHandle pConn, int dwMilliseconds);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint UnmanagedIsTokenRestricted([In] IntPtr token, [MarshalAs(UnmanagedType.Bool)] out bool isRestricted);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint GetSniMaxComposedSpnLength();

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint SNIGetInfoWrapper([In] SNIHandle pConn, SNINativeMethodWrapper.QTypes QType, out Guid pbQInfo);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint SNIInitialize([In] IntPtr pmo);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint SNIOpenSyncExWrapper(ref SNINativeMethodWrapper.SNI_CLIENT_CONSUMER_INFO pClientConsumerInfo, out IntPtr ppConn);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint SNIOpenWrapper([In] ref SNINativeMethodWrapper.Sni_Consumer_Info pConsumerInfo, [MarshalAs(UnmanagedType.LPStr)] string szConnect, [In] SNIHandle pConn, out IntPtr ppConn, [MarshalAs(UnmanagedType.Bool)] bool fSync);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SNIPacketAllocateWrapper([In] SafeHandle pConn, SNINativeMethodWrapper.IOType IOType);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint SNIPacketGetDataWrapper([In] IntPtr packet, [In] [Out] byte[] readBuffer, uint readBufferLength, out uint dataSize);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void SNIPacketSetData(SNIPacket pPacket, [In] byte* pbBuf, uint cbBuf);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern uint SNISecGenClientContextWrapper([In] SNIHandle pConn, [In] [Out] byte[] pIn, uint cbIn, [In] [Out] byte[] pOut, [In] ref uint pcbOut, [MarshalAs(UnmanagedType.Bool)] out bool pfDone, byte* szServerInfo, uint cbServerInfo, [MarshalAs(UnmanagedType.LPWStr)] string pwszUserName, [MarshalAs(UnmanagedType.LPWStr)] string pwszPassword);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint SNIWriteAsyncWrapper(SNIHandle pConn, [In] SNIPacket pPacket);

		[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint SNIWriteSyncOverAsync(SNIHandle pConn, [In] SNIPacket pPacket);

		internal static uint SniGetConnectionId(SNIHandle pConn, ref Guid connId)
		{
			return SNINativeMethodWrapper.SNIGetInfoWrapper(pConn, SNINativeMethodWrapper.QTypes.SNI_QUERY_CONN_CONNID, out connId);
		}

		internal static uint SNIInitialize()
		{
			return SNINativeMethodWrapper.SNIInitialize(IntPtr.Zero);
		}

		internal static uint SNIOpenMarsSession(SNINativeMethodWrapper.ConsumerInfo consumerInfo, SNIHandle parent, ref IntPtr pConn, bool fSync)
		{
			SNINativeMethodWrapper.Sni_Consumer_Info sni_Consumer_Info = default(SNINativeMethodWrapper.Sni_Consumer_Info);
			SNINativeMethodWrapper.MarshalConsumerInfo(consumerInfo, ref sni_Consumer_Info);
			return SNINativeMethodWrapper.SNIOpenWrapper(ref sni_Consumer_Info, "session:", parent, out pConn, fSync);
		}

		internal unsafe static uint SNIOpenSyncEx(SNINativeMethodWrapper.ConsumerInfo consumerInfo, string constring, ref IntPtr pConn, byte[] spnBuffer, byte[] instanceName, bool fOverrideCache, bool fSync, int timeout, bool fParallel)
		{
			fixed (byte* ptr = &instanceName[0])
			{
				byte* ptr2 = ptr;
				SNINativeMethodWrapper.SNI_CLIENT_CONSUMER_INFO sni_CLIENT_CONSUMER_INFO = default(SNINativeMethodWrapper.SNI_CLIENT_CONSUMER_INFO);
				SNINativeMethodWrapper.MarshalConsumerInfo(consumerInfo, ref sni_CLIENT_CONSUMER_INFO.ConsumerInfo);
				sni_CLIENT_CONSUMER_INFO.wszConnectionString = constring;
				sni_CLIENT_CONSUMER_INFO.networkLibrary = SNINativeMethodWrapper.PrefixEnum.UNKNOWN_PREFIX;
				sni_CLIENT_CONSUMER_INFO.szInstanceName = ptr2;
				sni_CLIENT_CONSUMER_INFO.cchInstanceName = (uint)instanceName.Length;
				sni_CLIENT_CONSUMER_INFO.fOverrideLastConnectCache = fOverrideCache;
				sni_CLIENT_CONSUMER_INFO.fSynchronousConnection = fSync;
				sni_CLIENT_CONSUMER_INFO.timeout = timeout;
				sni_CLIENT_CONSUMER_INFO.fParallel = fParallel;
				sni_CLIENT_CONSUMER_INFO.transparentNetworkResolution = SNINativeMethodWrapper.TransparentNetworkResolutionMode.DisabledMode;
				sni_CLIENT_CONSUMER_INFO.totalTimeout = -1;
				sni_CLIENT_CONSUMER_INFO.isAzureSqlServerEndpoint = ADP.IsAzureSqlServerEndpoint(constring);
				if (spnBuffer != null)
				{
					fixed (byte* ptr3 = &spnBuffer[0])
					{
						byte* ptr4 = ptr3;
						sni_CLIENT_CONSUMER_INFO.szSPN = ptr4;
						sni_CLIENT_CONSUMER_INFO.cchSPN = (uint)spnBuffer.Length;
						return SNINativeMethodWrapper.SNIOpenSyncExWrapper(ref sni_CLIENT_CONSUMER_INFO, out pConn);
					}
				}
				return SNINativeMethodWrapper.SNIOpenSyncExWrapper(ref sni_CLIENT_CONSUMER_INFO, out pConn);
			}
		}

		internal static void SNIPacketAllocate(SafeHandle pConn, SNINativeMethodWrapper.IOType IOType, ref IntPtr pPacket)
		{
			pPacket = SNINativeMethodWrapper.SNIPacketAllocateWrapper(pConn, IOType);
		}

		internal static uint SNIPacketGetData(IntPtr packet, byte[] readBuffer, ref uint dataSize)
		{
			return SNINativeMethodWrapper.SNIPacketGetDataWrapper(packet, readBuffer, (uint)readBuffer.Length, out dataSize);
		}

		internal unsafe static void SNIPacketSetData(SNIPacket packet, byte[] data, int length)
		{
			fixed (byte* ptr = &data[0])
			{
				byte* ptr2 = ptr;
				SNINativeMethodWrapper.SNIPacketSetData(packet, ptr2, (uint)length);
			}
		}

		internal unsafe static uint SNISecGenClientContext(SNIHandle pConnectionObject, byte[] inBuff, uint receivedLength, byte[] OutBuff, ref uint sendLength, byte[] serverUserName)
		{
			fixed (byte* ptr = &serverUserName[0])
			{
				byte* ptr2 = ptr;
				bool flag;
				return SNINativeMethodWrapper.SNISecGenClientContextWrapper(pConnectionObject, inBuff, receivedLength, OutBuff, ref sendLength, out flag, ptr2, (uint)serverUserName.Length, null, null);
			}
		}

		internal static uint SNIWritePacket(SNIHandle pConn, SNIPacket packet, bool sync)
		{
			if (sync)
			{
				return SNINativeMethodWrapper.SNIWriteSyncOverAsync(pConn, packet);
			}
			return SNINativeMethodWrapper.SNIWriteAsyncWrapper(pConn, packet);
		}

		private static void MarshalConsumerInfo(SNINativeMethodWrapper.ConsumerInfo consumerInfo, ref SNINativeMethodWrapper.Sni_Consumer_Info native_consumerInfo)
		{
			native_consumerInfo.DefaultUserDataLength = consumerInfo.defaultBufferSize;
			native_consumerInfo.fnReadComp = ((consumerInfo.readDelegate != null) ? Marshal.GetFunctionPointerForDelegate<SNINativeMethodWrapper.SqlAsyncCallbackDelegate>(consumerInfo.readDelegate) : IntPtr.Zero);
			native_consumerInfo.fnWriteComp = ((consumerInfo.writeDelegate != null) ? Marshal.GetFunctionPointerForDelegate<SNINativeMethodWrapper.SqlAsyncCallbackDelegate>(consumerInfo.writeDelegate) : IntPtr.Zero);
			native_consumerInfo.ConsumerKey = consumerInfo.key;
		}

		private const string SNI = "sni.dll";

		private static int s_sniMaxComposedSpnLength = -1;

		private const int SniOpenTimeOut = -1;

		internal enum SniSpecialErrors : uint
		{
			LocalDBErrorCode = 50U,
			MultiSubnetFailoverWithMoreThan64IPs = 47U,
			MultiSubnetFailoverWithInstanceSpecified,
			MultiSubnetFailoverWithNonTcpProtocol,
			MaxErrorValue = 50157U
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void SqlAsyncCallbackDelegate(IntPtr m_ConsKey, IntPtr pPacket, uint dwError);

		internal struct ConsumerInfo
		{
			internal int defaultBufferSize;

			internal SNINativeMethodWrapper.SqlAsyncCallbackDelegate readDelegate;

			internal SNINativeMethodWrapper.SqlAsyncCallbackDelegate writeDelegate;

			internal IntPtr key;
		}

		internal enum ConsumerNumber
		{
			SNI_Consumer_SNI,
			SNI_Consumer_SSB,
			SNI_Consumer_PacketIsReleased,
			SNI_Consumer_Invalid
		}

		internal enum IOType
		{
			READ,
			WRITE
		}

		internal enum PrefixEnum
		{
			UNKNOWN_PREFIX,
			SM_PREFIX,
			TCP_PREFIX,
			NP_PREFIX,
			VIA_PREFIX,
			INVALID_PREFIX
		}

		internal enum ProviderEnum
		{
			HTTP_PROV,
			NP_PROV,
			SESSION_PROV,
			SIGN_PROV,
			SM_PROV,
			SMUX_PROV,
			SSL_PROV,
			TCP_PROV,
			VIA_PROV,
			MAX_PROVS,
			INVALID_PROV
		}

		internal enum QTypes
		{
			SNI_QUERY_CONN_INFO,
			SNI_QUERY_CONN_BUFSIZE,
			SNI_QUERY_CONN_KEY,
			SNI_QUERY_CLIENT_ENCRYPT_POSSIBLE,
			SNI_QUERY_SERVER_ENCRYPT_POSSIBLE,
			SNI_QUERY_CERTIFICATE,
			SNI_QUERY_LOCALDB_HMODULE,
			SNI_QUERY_CONN_ENCRYPT,
			SNI_QUERY_CONN_PROVIDERNUM,
			SNI_QUERY_CONN_CONNID,
			SNI_QUERY_CONN_PARENTCONNID,
			SNI_QUERY_CONN_SECPKG,
			SNI_QUERY_CONN_NETPACKETSIZE,
			SNI_QUERY_CONN_NODENUM,
			SNI_QUERY_CONN_PACKETSRECD,
			SNI_QUERY_CONN_PACKETSSENT,
			SNI_QUERY_CONN_PEERADDR,
			SNI_QUERY_CONN_PEERPORT,
			SNI_QUERY_CONN_LASTREADTIME,
			SNI_QUERY_CONN_LASTWRITETIME,
			SNI_QUERY_CONN_CONSUMER_ID,
			SNI_QUERY_CONN_CONNECTTIME,
			SNI_QUERY_CONN_HTTPENDPOINT,
			SNI_QUERY_CONN_LOCALADDR,
			SNI_QUERY_CONN_LOCALPORT,
			SNI_QUERY_CONN_SSLHANDSHAKESTATE,
			SNI_QUERY_CONN_SOBUFAUTOTUNING,
			SNI_QUERY_CONN_SECPKGNAME,
			SNI_QUERY_CONN_SECPKGMUTUALAUTH,
			SNI_QUERY_CONN_CONSUMERCONNID,
			SNI_QUERY_CONN_SNIUCI,
			SNI_QUERY_CONN_SUPPORTS_EXTENDED_PROTECTION,
			SNI_QUERY_CONN_CHANNEL_PROVIDES_AUTHENTICATION_CONTEXT,
			SNI_QUERY_CONN_PEERID,
			SNI_QUERY_CONN_SUPPORTS_SYNC_OVER_ASYNC
		}

		internal enum TransparentNetworkResolutionMode : byte
		{
			DisabledMode,
			SequentialMode,
			ParallelMode
		}

		private struct Sni_Consumer_Info
		{
			public int DefaultUserDataLength;

			public IntPtr ConsumerKey;

			public IntPtr fnReadComp;

			public IntPtr fnWriteComp;

			public IntPtr fnTrace;

			public IntPtr fnAcceptComp;

			public uint dwNumProts;

			public IntPtr rgListenInfo;

			public IntPtr NodeAffinity;
		}

		private struct SNI_CLIENT_CONSUMER_INFO
		{
			public SNINativeMethodWrapper.Sni_Consumer_Info ConsumerInfo;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string wszConnectionString;

			public SNINativeMethodWrapper.PrefixEnum networkLibrary;

			public unsafe byte* szSPN;

			public uint cchSPN;

			public unsafe byte* szInstanceName;

			public uint cchInstanceName;

			[MarshalAs(UnmanagedType.Bool)]
			public bool fOverrideLastConnectCache;

			[MarshalAs(UnmanagedType.Bool)]
			public bool fSynchronousConnection;

			public int timeout;

			[MarshalAs(UnmanagedType.Bool)]
			public bool fParallel;

			public SNINativeMethodWrapper.TransparentNetworkResolutionMode transparentNetworkResolution;

			public int totalTimeout;

			public bool isAzureSqlServerEndpoint;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct SNI_Error
		{
			internal SNINativeMethodWrapper.ProviderEnum provider;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
			internal string errorMessage;

			internal uint nativeError;

			internal uint sniError;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string fileName;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string function;

			internal uint lineNumber;
		}
	}
}
