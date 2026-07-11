using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace System.Data.SqlClient.SNI
{
	internal class SNIProxy
	{
		public void Terminate()
		{
		}

		public uint EnableSsl(SNIHandle handle, uint options)
		{
			uint num;
			try
			{
				num = handle.EnableSsl(options);
			}
			catch (Exception ex)
			{
				num = SNICommon.ReportSNIError(SNIProviders.SSL_PROV, 31U, ex);
			}
			return num;
		}

		public uint DisableSsl(SNIHandle handle)
		{
			handle.DisableSsl();
			return 0U;
		}

		public void GenSspiClientContext(SspiClientContextStatus sspiClientContextStatus, byte[] receivedBuff, ref byte[] sendBuff, byte[] serverName)
		{
			SafeDeleteContext securityContext = sspiClientContextStatus.SecurityContext;
			ContextFlagsPal contextFlags = sspiClientContextStatus.ContextFlags;
			SafeFreeCredentials safeFreeCredentials = sspiClientContextStatus.CredentialsHandle;
			string text = "Negotiate";
			if (securityContext == null)
			{
				safeFreeCredentials = NegotiateStreamPal.AcquireDefaultCredential(text, false);
			}
			SecurityBuffer[] array;
			if (receivedBuff != null)
			{
				array = new SecurityBuffer[]
				{
					new SecurityBuffer(receivedBuff, SecurityBufferType.SECBUFFER_TOKEN)
				};
			}
			else
			{
				array = new SecurityBuffer[0];
			}
			SecurityBuffer securityBuffer = new SecurityBuffer(NegotiateStreamPal.QueryMaxTokenSize(text), SecurityBufferType.SECBUFFER_TOKEN);
			ContextFlagsPal contextFlagsPal = ContextFlagsPal.MutualAuth | ContextFlagsPal.Confidentiality | ContextFlagsPal.Connection;
			string @string = Encoding.UTF8.GetString(serverName);
			SecurityStatusPal securityStatusPal = NegotiateStreamPal.InitializeSecurityContext(safeFreeCredentials, ref securityContext, @string, contextFlagsPal, array, securityBuffer, ref contextFlags);
			if (securityStatusPal.ErrorCode == SecurityStatusPalErrorCode.CompleteNeeded || securityStatusPal.ErrorCode == SecurityStatusPalErrorCode.CompAndContinue)
			{
				array = new SecurityBuffer[] { securityBuffer };
				securityStatusPal = NegotiateStreamPal.CompleteAuthToken(ref securityContext, array);
				securityBuffer.token = null;
			}
			sendBuff = securityBuffer.token;
			if (sendBuff == null)
			{
				sendBuff = Array.Empty<byte>();
			}
			sspiClientContextStatus.SecurityContext = securityContext;
			sspiClientContextStatus.ContextFlags = contextFlags;
			sspiClientContextStatus.CredentialsHandle = safeFreeCredentials;
			if (!SNIProxy.IsErrorStatus(securityStatusPal.ErrorCode))
			{
				return;
			}
			if (securityStatusPal.ErrorCode == SecurityStatusPalErrorCode.InternalError)
			{
				throw new Exception(SQLMessage.KerberosTicketMissingError() + "\n" + securityStatusPal);
			}
			throw new Exception(SQLMessage.SSPIGenerateError() + "\n" + securityStatusPal);
		}

		private static bool IsErrorStatus(SecurityStatusPalErrorCode errorCode)
		{
			return errorCode != SecurityStatusPalErrorCode.NotSet && errorCode != SecurityStatusPalErrorCode.OK && errorCode != SecurityStatusPalErrorCode.ContinueNeeded && errorCode != SecurityStatusPalErrorCode.CompleteNeeded && errorCode != SecurityStatusPalErrorCode.CompAndContinue && errorCode != SecurityStatusPalErrorCode.ContextExpired && errorCode != SecurityStatusPalErrorCode.CredentialsNeeded && errorCode != SecurityStatusPalErrorCode.Renegotiate;
		}

		public uint InitializeSspiPackage(ref uint maxLength)
		{
			throw new PlatformNotSupportedException();
		}

		public uint SetConnectionBufferSize(SNIHandle handle, uint bufferSize)
		{
			handle.SetBufferSize((int)bufferSize);
			return 0U;
		}

		public uint PacketGetData(SNIPacket packet, byte[] inBuff, ref uint dataSize)
		{
			int num = 0;
			packet.GetData(inBuff, ref num);
			dataSize = (uint)num;
			return 0U;
		}

		public uint ReadSyncOverAsync(SNIHandle handle, out SNIPacket packet, int timeout)
		{
			return handle.Receive(out packet, timeout);
		}

		public uint GetConnectionId(SNIHandle handle, ref Guid clientConnectionId)
		{
			clientConnectionId = handle.ConnectionId;
			return 0U;
		}

		public uint WritePacket(SNIHandle handle, SNIPacket packet, bool sync)
		{
			if (sync)
			{
				return handle.Send(packet.Clone());
			}
			return handle.SendAsync(packet.Clone(), null);
		}

		public SNIHandle CreateConnectionHandle(object callbackObject, string fullServerName, bool ignoreSniOpenTimeout, long timerExpire, out byte[] instanceName, ref byte[] spnBuffer, bool flushCache, bool async, bool parallel, bool isIntegratedSecurity)
		{
			instanceName = new byte[1];
			bool flag;
			string localDBDataSource = this.GetLocalDBDataSource(fullServerName, out flag);
			if (flag)
			{
				return null;
			}
			fullServerName = localDBDataSource ?? fullServerName;
			DataSource dataSource = DataSource.ParseServerName(fullServerName);
			if (dataSource == null)
			{
				return null;
			}
			SNIHandle snihandle = null;
			switch (dataSource.ConnectionProtocol)
			{
			case DataSource.Protocol.TCP:
			case DataSource.Protocol.None:
			case DataSource.Protocol.Admin:
				snihandle = this.CreateTcpHandle(dataSource, timerExpire, callbackObject, parallel);
				break;
			case DataSource.Protocol.NP:
				snihandle = this.CreateNpHandle(dataSource, timerExpire, callbackObject, parallel);
				break;
			}
			if (isIntegratedSecurity)
			{
				try
				{
					spnBuffer = SNIProxy.GetSqlServerSPN(dataSource);
				}
				catch (Exception ex)
				{
					SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 44U, ex);
				}
			}
			return snihandle;
		}

		private static byte[] GetSqlServerSPN(DataSource dataSource)
		{
			string serverName = dataSource.ServerName;
			string text = null;
			if (dataSource.Port != -1)
			{
				text = dataSource.Port.ToString();
			}
			else if (!string.IsNullOrWhiteSpace(dataSource.InstanceName))
			{
				text = dataSource.InstanceName;
			}
			else if (dataSource.ConnectionProtocol == DataSource.Protocol.TCP)
			{
				text = 1433.ToString();
			}
			return SNIProxy.GetSqlServerSPN(serverName, text);
		}

		private static byte[] GetSqlServerSPN(string hostNameOrAddress, string portOrInstanceName)
		{
			string hostName = Dns.GetHostEntry(hostNameOrAddress).HostName;
			string text = "MSSQLSvc/" + hostName;
			if (!string.IsNullOrWhiteSpace(portOrInstanceName))
			{
				text = text + ":" + portOrInstanceName;
			}
			return Encoding.UTF8.GetBytes(text);
		}

		private SNITCPHandle CreateTcpHandle(DataSource details, long timerExpire, object callbackObject, bool parallel)
		{
			string serverName = details.ServerName;
			if (string.IsNullOrWhiteSpace(serverName))
			{
				SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.TCP_PROV, 0U, 25U, string.Empty);
				return null;
			}
			int num = -1;
			bool flag = details.ConnectionProtocol == DataSource.Protocol.Admin;
			if (details.IsSsrpRequired)
			{
				try
				{
					num = (flag ? SSRP.GetDacPortByInstanceName(serverName, details.InstanceName) : SSRP.GetPortByInstanceName(serverName, details.InstanceName));
					goto IL_0098;
				}
				catch (SocketException ex)
				{
					SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.TCP_PROV, 25U, ex);
					return null;
				}
			}
			if (details.Port != -1)
			{
				num = details.Port;
			}
			else
			{
				num = (flag ? 1434 : 1433);
			}
			IL_0098:
			return new SNITCPHandle(serverName, num, timerExpire, callbackObject, parallel);
		}

		private SNINpHandle CreateNpHandle(DataSource details, long timerExpire, object callbackObject, bool parallel)
		{
			if (parallel)
			{
				SNICommon.ReportSNIError(SNIProviders.NP_PROV, 0U, 49U, string.Empty);
				return null;
			}
			return new SNINpHandle(details.PipeHostName, details.PipeName, timerExpire, callbackObject);
		}

		public uint ReadAsync(SNIHandle handle, out SNIPacket packet)
		{
			packet = new SNIPacket(null);
			return handle.ReceiveAsync(ref packet);
		}

		public void PacketSetData(SNIPacket packet, byte[] data, int length)
		{
			packet.SetData(data, length);
		}

		public void PacketRelease(SNIPacket packet)
		{
			packet.Release();
		}

		public uint CheckConnection(SNIHandle handle)
		{
			return handle.CheckConnection();
		}

		public SNIError GetLastError()
		{
			return SNILoadHandle.SingletonInstance.LastError;
		}

		private string GetLocalDBDataSource(string fullServerName, out bool error)
		{
			string text = null;
			bool flag;
			string localDBInstance = DataSource.GetLocalDBInstance(fullServerName, out flag);
			if (flag)
			{
				error = true;
				return null;
			}
			if (!string.IsNullOrEmpty(localDBInstance))
			{
				text = LocalDB.GetLocalDBConnectionString(localDBInstance);
				if (fullServerName == null)
				{
					error = true;
					return null;
				}
			}
			error = false;
			return text;
		}

		private const int DefaultSqlServerPort = 1433;

		private const int DefaultSqlServerDacPort = 1434;

		private const string SqlServerSpnHeader = "MSSQLSvc";

		public static readonly SNIProxy Singleton = new SNIProxy();

		internal class SspiClientContextResult
		{
			internal const uint OK = 0U;

			internal const uint Failed = 1U;

			internal const uint KerberosTicketMissing = 2U;
		}
	}
}
