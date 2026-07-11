using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	[NativeHeader("Runtime/Networking/UNETManager.h")]
	[NativeHeader("Runtime/Networking/UNETConfiguration.h")]
	[NativeConditional("ENABLE_NETWORK && ENABLE_UNET", true)]
	[NativeHeader("Runtime/Networking/UNetTypes.h")]
	[StructLayout(LayoutKind.Sequential)]
	internal class ConnectionConfigInternal : IDisposable
	{
		public ConnectionConfigInternal(ConnectionConfig config)
		{
			if (config == null)
			{
				throw new NullReferenceException("config is not defined");
			}
			this.m_Ptr = ConnectionConfigInternal.InternalCreate();
			if (!this.SetPacketSize(config.PacketSize))
			{
				throw new ArgumentOutOfRangeException("PacketSize is too small");
			}
			this.FragmentSize = config.FragmentSize;
			this.ResendTimeout = config.ResendTimeout;
			this.DisconnectTimeout = config.DisconnectTimeout;
			this.ConnectTimeout = config.ConnectTimeout;
			this.MinUpdateTimeout = config.MinUpdateTimeout;
			this.PingTimeout = config.PingTimeout;
			this.ReducedPingTimeout = config.ReducedPingTimeout;
			this.AllCostTimeout = config.AllCostTimeout;
			this.NetworkDropThreshold = config.NetworkDropThreshold;
			this.OverflowDropThreshold = config.OverflowDropThreshold;
			this.MaxConnectionAttempt = config.MaxConnectionAttempt;
			this.AckDelay = config.AckDelay;
			this.SendDelay = config.SendDelay;
			this.MaxCombinedReliableMessageSize = config.MaxCombinedReliableMessageSize;
			this.MaxCombinedReliableMessageCount = config.MaxCombinedReliableMessageCount;
			this.MaxSentMessageQueueSize = config.MaxSentMessageQueueSize;
			this.AcksType = (byte)config.AcksType;
			this.UsePlatformSpecificProtocols = config.UsePlatformSpecificProtocols;
			this.InitialBandwidth = config.InitialBandwidth;
			this.BandwidthPeakFactor = config.BandwidthPeakFactor;
			this.WebSocketReceiveBufferMaxSize = config.WebSocketReceiveBufferMaxSize;
			this.UdpSocketReceiveBufferMaxSize = config.UdpSocketReceiveBufferMaxSize;
			if (config.SSLCertFilePath != null)
			{
				int num = this.SetSSLCertFilePath(config.SSLCertFilePath);
				if (num != 0)
				{
					throw new ArgumentOutOfRangeException("SSLCertFilePath cannot be > than " + num.ToString());
				}
			}
			if (config.SSLPrivateKeyFilePath != null)
			{
				int num2 = this.SetSSLPrivateKeyFilePath(config.SSLPrivateKeyFilePath);
				if (num2 != 0)
				{
					throw new ArgumentOutOfRangeException("SSLPrivateKeyFilePath cannot be > than " + num2.ToString());
				}
			}
			if (config.SSLCAFilePath != null)
			{
				int num3 = this.SetSSLCAFilePath(config.SSLCAFilePath);
				if (num3 != 0)
				{
					throw new ArgumentOutOfRangeException("SSLCAFilePath cannot be > than " + num3.ToString());
				}
			}
			byte b = 0;
			while ((int)b < config.ChannelCount)
			{
				this.AddChannel((int)((byte)config.GetChannel(b)));
				b += 1;
			}
			byte b2 = 0;
			while ((int)b2 < config.SharedOrderChannelCount)
			{
				IList<byte> sharedOrderChannels = config.GetSharedOrderChannels(b2);
				byte[] array = new byte[sharedOrderChannels.Count];
				sharedOrderChannels.CopyTo(array, 0);
				this.MakeChannelsSharedOrder(array);
				b2 += 1;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				ConnectionConfigInternal.InternalDestroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		~ConnectionConfigInternal()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				ConnectionConfigInternal.InternalDestroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalCreate();

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalDestroy(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern byte AddChannel(int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetPacketSize(ushort value);

		[NativeProperty("m_ProtocolRequired.m_FragmentSize", TargetType.Field)]
		private extern ushort FragmentSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolRequired.m_ResendTimeout", TargetType.Field)]
		private extern uint ResendTimeout
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolRequired.m_DisconnectTimeout", TargetType.Field)]
		private extern uint DisconnectTimeout
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolRequired.m_ConnectTimeout", TargetType.Field)]
		private extern uint ConnectTimeout
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_MinUpdateTimeout", TargetType.Field)]
		private extern uint MinUpdateTimeout
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolRequired.m_PingTimeout", TargetType.Field)]
		private extern uint PingTimeout
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolRequired.m_ReducedPingTimeout", TargetType.Field)]
		private extern uint ReducedPingTimeout
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolRequired.m_AllCostTimeout", TargetType.Field)]
		private extern uint AllCostTimeout
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_NetworkDropThreshold", TargetType.Field)]
		private extern byte NetworkDropThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_OverflowDropThreshold", TargetType.Field)]
		private extern byte OverflowDropThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_MaxConnectionAttempt", TargetType.Field)]
		private extern byte MaxConnectionAttempt
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_AckDelay", TargetType.Field)]
		private extern uint AckDelay
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_SendDelay", TargetType.Field)]
		private extern uint SendDelay
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_MaxCombinedReliableMessageSize", TargetType.Field)]
		private extern ushort MaxCombinedReliableMessageSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_MaxCombinedReliableMessageAmount", TargetType.Field)]
		private extern ushort MaxCombinedReliableMessageCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_MaxSentMessageQueueSize", TargetType.Field)]
		private extern ushort MaxSentMessageQueueSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolRequired.m_AcksType", TargetType.Field)]
		private extern byte AcksType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolRequired.m_UsePlatformSpecificProtocols", TargetType.Field)]
		private extern bool UsePlatformSpecificProtocols
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_InitialBandwidth", TargetType.Field)]
		private extern uint InitialBandwidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_BandwidthPeakFactor", TargetType.Field)]
		private extern float BandwidthPeakFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_WebSocketReceiveBufferMaxSize", TargetType.Field)]
		private extern ushort WebSocketReceiveBufferMaxSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ProtocolOptional.m_UdpSocketReceiveBufferMaxSize", TargetType.Field)]
		private extern uint UdpSocketReceiveBufferMaxSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeMethod("SetSSLCertFilePath")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int SetSSLCertFilePath(string value);

		[NativeMethod("SetSSLPrivateKeyFilePath")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int SetSSLPrivateKeyFilePath(string value);

		[NativeMethod("SetSSLCAFilePath")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int SetSSLCAFilePath(string value);

		[NativeMethod("MakeChannelsSharedOrder")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool MakeChannelsSharedOrder(byte[] values);

		public IntPtr m_Ptr;
	}
}
