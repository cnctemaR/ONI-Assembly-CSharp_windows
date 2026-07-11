using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient.SNI;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.SqlServer.Server;

namespace System.Data.SqlClient
{
	internal sealed class TdsParser
	{
		internal void PostReadAsyncForMars()
		{
		}

		private void LoadSSPILibrary()
		{
		}

		private void WaitForSSLHandShakeToComplete(ref uint error)
		{
		}

		private SNIErrorDetails GetSniErrorDetails()
		{
			SNIError lastError = SNIProxy.Singleton.GetLastError();
			SNIErrorDetails snierrorDetails;
			snierrorDetails.sniErrorNumber = lastError.sniError;
			snierrorDetails.errorMessage = lastError.errorMessage;
			snierrorDetails.nativeError = lastError.nativeError;
			snierrorDetails.provider = (int)lastError.provider;
			snierrorDetails.lineNumber = lastError.lineNumber;
			snierrorDetails.function = lastError.function;
			snierrorDetails.exception = lastError.exception;
			return snierrorDetails;
		}

		internal TdsParser(bool MARS, bool fAsynchronous)
		{
			this._fMARS = MARS;
			this._physicalStateObj = TdsParserStateObjectFactory.Singleton.CreateTdsParserStateObject(this);
		}

		internal SqlInternalConnectionTds Connection
		{
			get
			{
				return this._connHandler;
			}
		}

		internal SqlInternalTransaction CurrentTransaction
		{
			get
			{
				return this._currentTransaction;
			}
			set
			{
				if ((this._currentTransaction == null && value != null) || (this._currentTransaction != null && value == null))
				{
					this._currentTransaction = value;
				}
			}
		}

		internal int DefaultLCID
		{
			get
			{
				return this._defaultLCID;
			}
		}

		internal EncryptionOptions EncryptionOptions
		{
			get
			{
				return this._encryptionOption;
			}
			set
			{
				this._encryptionOption = value;
			}
		}

		internal bool IsKatmaiOrNewer
		{
			get
			{
				return this._isKatmai;
			}
		}

		internal bool MARSOn
		{
			get
			{
				return this._fMARS;
			}
		}

		internal SqlInternalTransaction PendingTransaction
		{
			get
			{
				return this._pendingTransaction;
			}
			set
			{
				this._pendingTransaction = value;
			}
		}

		internal string Server
		{
			get
			{
				return this._server;
			}
		}

		internal TdsParserState State
		{
			get
			{
				return this._state;
			}
			set
			{
				this._state = value;
			}
		}

		internal SqlStatistics Statistics
		{
			get
			{
				return this._statistics;
			}
			set
			{
				this._statistics = value;
			}
		}

		internal int IncrementNonTransactedOpenResultCount()
		{
			return Interlocked.Increment(ref this._nonTransactedOpenResultCount);
		}

		internal void DecrementNonTransactedOpenResultCount()
		{
			Interlocked.Decrement(ref this._nonTransactedOpenResultCount);
		}

		internal void ProcessPendingAck(TdsParserStateObject stateObj)
		{
			if (stateObj._attentionSent)
			{
				this.ProcessAttention(stateObj);
			}
		}

		internal void Connect(ServerInfo serverInfo, SqlInternalConnectionTds connHandler, bool ignoreSniOpenTimeout, long timerExpire, bool encrypt, bool trustServerCert, bool integratedSecurity, bool withFailover)
		{
			if (this._state != TdsParserState.Closed)
			{
				return;
			}
			this._connHandler = connHandler;
			this._loginWithFailover = withFailover;
			if (TdsParserStateObjectFactory.Singleton.SNIStatus != 0U)
			{
				this._physicalStateObj.AddError(this.ProcessSNIError(this._physicalStateObj));
				this._physicalStateObj.Dispose();
				this.ThrowExceptionAndWarning(this._physicalStateObj, false, false);
			}
			this._sniSpnBuffer = null;
			if (integratedSecurity)
			{
				this.LoadSSPILibrary();
			}
			byte[] array = null;
			this._connHandler.TimeoutErrorInternal.EndPhase(SqlConnectionTimeoutErrorPhase.PreLoginBegin);
			this._connHandler.TimeoutErrorInternal.SetAndBeginPhase(SqlConnectionTimeoutErrorPhase.InitializeConnection);
			bool multiSubnetFailover = this._connHandler.ConnectionOptions.MultiSubnetFailover;
			this._physicalStateObj.CreatePhysicalSNIHandle(serverInfo.ExtendedServerName, ignoreSniOpenTimeout, timerExpire, out array, ref this._sniSpnBuffer, false, true, multiSubnetFailover, integratedSecurity);
			if (this._physicalStateObj.Status != 0U)
			{
				this._physicalStateObj.AddError(this.ProcessSNIError(this._physicalStateObj));
				this._physicalStateObj.Dispose();
				this.ThrowExceptionAndWarning(this._physicalStateObj, false, false);
			}
			this._server = serverInfo.ResolvedServerName;
			if (connHandler.PoolGroupProviderInfo != null)
			{
				connHandler.PoolGroupProviderInfo.AliasCheck((serverInfo.PreRoutingServerName == null) ? serverInfo.ResolvedServerName : serverInfo.PreRoutingServerName);
			}
			this._state = TdsParserState.OpenNotLoggedIn;
			this._physicalStateObj.SniContext = SniContext.Snix_PreLoginBeforeSuccessfulWrite;
			this._physicalStateObj.TimeoutTime = timerExpire;
			bool flag = false;
			this._connHandler.TimeoutErrorInternal.EndPhase(SqlConnectionTimeoutErrorPhase.InitializeConnection);
			this._connHandler.TimeoutErrorInternal.SetAndBeginPhase(SqlConnectionTimeoutErrorPhase.SendPreLoginHandshake);
			this._physicalStateObj.SniGetConnectionId(ref this._connHandler._clientConnectionId);
			this.SendPreLoginHandshake(array, encrypt);
			this._connHandler.TimeoutErrorInternal.EndPhase(SqlConnectionTimeoutErrorPhase.SendPreLoginHandshake);
			this._connHandler.TimeoutErrorInternal.SetAndBeginPhase(SqlConnectionTimeoutErrorPhase.ConsumePreLoginHandshake);
			this._physicalStateObj.SniContext = SniContext.Snix_PreLogin;
			if (this.ConsumePreLoginHandshake(encrypt, trustServerCert, integratedSecurity, out flag) == PreLoginHandshakeStatus.InstanceFailure)
			{
				this._physicalStateObj.Dispose();
				this._physicalStateObj.SniContext = SniContext.Snix_Connect;
				this._physicalStateObj.CreatePhysicalSNIHandle(serverInfo.ExtendedServerName, ignoreSniOpenTimeout, timerExpire, out array, ref this._sniSpnBuffer, true, true, multiSubnetFailover, integratedSecurity);
				if (this._physicalStateObj.Status != 0U)
				{
					this._physicalStateObj.AddError(this.ProcessSNIError(this._physicalStateObj));
					this.ThrowExceptionAndWarning(this._physicalStateObj, false, false);
				}
				this._physicalStateObj.SniGetConnectionId(ref this._connHandler._clientConnectionId);
				this.SendPreLoginHandshake(array, encrypt);
				if (this.ConsumePreLoginHandshake(encrypt, trustServerCert, integratedSecurity, out flag) == PreLoginHandshakeStatus.InstanceFailure)
				{
					throw SQL.InstanceFailure();
				}
			}
			if (this._fMARS && flag)
			{
				this._sessionPool = new TdsParserSessionPool(this);
				return;
			}
			this._fMARS = false;
		}

		internal void RemoveEncryption()
		{
			if (this._physicalStateObj.DisabeSsl() != 0U)
			{
				this._physicalStateObj.AddError(this.ProcessSNIError(this._physicalStateObj));
				this.ThrowExceptionAndWarning(this._physicalStateObj, false, false);
			}
			this._physicalStateObj.ClearAllWritePackets();
		}

		internal void EnableMars()
		{
			if (this._fMARS)
			{
				this._pMarsPhysicalConObj = this._physicalStateObj;
				if (TdsParserStateObjectFactory.UseManagedSNI)
				{
					this._pMarsPhysicalConObj.IncrementPendingCallbacks();
				}
				uint num = 0U;
				if (this._pMarsPhysicalConObj.EnableMars(ref num) != 0U)
				{
					this._physicalStateObj.AddError(this.ProcessSNIError(this._physicalStateObj));
					this.ThrowExceptionAndWarning(this._physicalStateObj, false, false);
				}
				this.PostReadAsyncForMars();
				this._physicalStateObj = this.CreateSession();
			}
		}

		internal TdsParserStateObject CreateSession()
		{
			return TdsParserStateObjectFactory.Singleton.CreateSessionObject(this, this._pMarsPhysicalConObj, true);
		}

		internal TdsParserStateObject GetSession(object owner)
		{
			TdsParserStateObject tdsParserStateObject;
			if (this.MARSOn)
			{
				tdsParserStateObject = this._sessionPool.GetSession(owner);
			}
			else
			{
				tdsParserStateObject = this._physicalStateObj;
			}
			return tdsParserStateObject;
		}

		internal void PutSession(TdsParserStateObject session)
		{
			if (this.MARSOn)
			{
				this._sessionPool.PutSession(session);
				return;
			}
			if (this._state == TdsParserState.Closed || this._state == TdsParserState.Broken)
			{
				this._physicalStateObj.SniContext = SniContext.Snix_Close;
				this._physicalStateObj.Dispose();
				return;
			}
			this._physicalStateObj.Owner = null;
		}

		private void SendPreLoginHandshake(byte[] instanceName, bool encrypt)
		{
			this._physicalStateObj._outputMessageType = 18;
			int num = 31;
			byte[] array = new byte[1054];
			int num2 = 0;
			for (int i = 0; i < 6; i++)
			{
				int num3 = 0;
				this._physicalStateObj.WriteByte((byte)i);
				this._physicalStateObj.WriteByte((byte)((num & 65280) >> 8));
				this._physicalStateObj.WriteByte((byte)(num & 255));
				switch (i)
				{
				case 0:
				{
					Version assemblyVersion = ADP.GetAssemblyVersion();
					array[num2++] = (byte)(assemblyVersion.Major & 255);
					array[num2++] = (byte)(assemblyVersion.Minor & 255);
					array[num2++] = (byte)((assemblyVersion.Build & 65280) >> 8);
					array[num2++] = (byte)(assemblyVersion.Build & 255);
					array[num2++] = (byte)(assemblyVersion.Revision & 255);
					array[num2++] = (byte)((assemblyVersion.Revision & 65280) >> 8);
					num += 6;
					num3 = 6;
					break;
				}
				case 1:
					if (this._encryptionOption == EncryptionOptions.NOT_SUP)
					{
						array[num2] = 2;
					}
					else if (encrypt)
					{
						array[num2] = 1;
						this._encryptionOption = EncryptionOptions.ON;
					}
					else
					{
						array[num2] = 0;
						this._encryptionOption = EncryptionOptions.OFF;
					}
					num2++;
					num++;
					num3 = 1;
					break;
				case 2:
				{
					int num4 = 0;
					while (instanceName[num4] != 0)
					{
						array[num2] = instanceName[num4];
						num2++;
						num4++;
					}
					array[num2] = 0;
					num2++;
					num4++;
					num += num4;
					num3 = num4;
					break;
				}
				case 3:
				{
					int currentThreadIdForTdsLoginOnly = TdsParserStaticMethods.GetCurrentThreadIdForTdsLoginOnly();
					array[num2++] = (byte)(((ulong)(-16777216) & (ulong)((long)currentThreadIdForTdsLoginOnly)) >> 24);
					array[num2++] = (byte)((16711680 & currentThreadIdForTdsLoginOnly) >> 16);
					array[num2++] = (byte)((65280 & currentThreadIdForTdsLoginOnly) >> 8);
					array[num2++] = (byte)(255 & currentThreadIdForTdsLoginOnly);
					num += 4;
					num3 = 4;
					break;
				}
				case 4:
					array[num2++] = (this._fMARS ? 1 : 0);
					num++;
					num3++;
					break;
				case 5:
				{
					Buffer.BlockCopy(this._connHandler._clientConnectionId.ToByteArray(), 0, array, num2, 16);
					num2 += 16;
					num += 16;
					num3 = 16;
					ActivityCorrelator.ActivityId activityId = ActivityCorrelator.Next();
					Buffer.BlockCopy(activityId.Id.ToByteArray(), 0, array, num2, 16);
					num2 += 16;
					array[num2++] = (byte)(255U & activityId.Sequence);
					array[num2++] = (byte)((65280U & activityId.Sequence) >> 8);
					array[num2++] = (byte)((16711680U & activityId.Sequence) >> 16);
					array[num2++] = (byte)((4278190080U & activityId.Sequence) >> 24);
					int num5 = 20;
					num += num5;
					num3 += num5;
					break;
				}
				}
				this._physicalStateObj.WriteByte((byte)((num3 & 65280) >> 8));
				this._physicalStateObj.WriteByte((byte)(num3 & 255));
			}
			this._physicalStateObj.WriteByte(byte.MaxValue);
			this._physicalStateObj.WriteByteArray(array, num2, 0, true, null);
			this._physicalStateObj.WritePacket(1, false);
		}

		private PreLoginHandshakeStatus ConsumePreLoginHandshake(bool encrypt, bool trustServerCert, bool integratedSecurity, out bool marsCapable)
		{
			marsCapable = this._fMARS;
			bool flag = false;
			if (!this._physicalStateObj.TryReadNetworkPacket())
			{
				throw SQL.SynchronousCallMayNotPend();
			}
			if (this._physicalStateObj._inBytesRead == 0)
			{
				this._physicalStateObj.AddError(new SqlError(0, 0, 20, this._server, SQLMessage.PreloginError(), "", 0, null));
				this._physicalStateObj.Dispose();
				this.ThrowExceptionAndWarning(this._physicalStateObj, false, false);
			}
			if (!this._physicalStateObj.TryProcessHeader())
			{
				throw SQL.SynchronousCallMayNotPend();
			}
			if (this._physicalStateObj._inBytesPacket > 32768 || this._physicalStateObj._inBytesPacket <= 0)
			{
				throw SQL.ParsingError();
			}
			byte[] array = new byte[this._physicalStateObj._inBytesPacket];
			if (!this._physicalStateObj.TryReadByteArray(array, 0, array.Length))
			{
				throw SQL.SynchronousCallMayNotPend();
			}
			if (array[0] == 170)
			{
				throw SQL.InvalidSQLServerVersionUnknown();
			}
			int num = 0;
			for (int num2 = (int)array[num++]; num2 != 255; num2 = (int)array[num++])
			{
				switch (num2)
				{
				case 0:
				{
					int num3 = ((int)array[num++] << 8) | (int)array[num++];
					byte b = array[num++];
					byte b2 = array[num++];
					int num4 = (int)array[num3];
					byte b3 = array[num3 + 1];
					byte b4 = array[num3 + 2];
					byte b5 = array[num3 + 3];
					flag = num4 >= 9;
					if (!flag)
					{
						marsCapable = false;
					}
					break;
				}
				case 1:
				{
					int num3 = ((int)array[num++] << 8) | (int)array[num++];
					byte b6 = array[num++];
					byte b7 = array[num++];
					EncryptionOptions encryptionOptions = (EncryptionOptions)array[num3];
					switch (this._encryptionOption)
					{
					case EncryptionOptions.OFF:
						if (encryptionOptions == EncryptionOptions.OFF)
						{
							this._encryptionOption = EncryptionOptions.LOGIN;
						}
						else if (encryptionOptions == EncryptionOptions.REQ)
						{
							this._encryptionOption = EncryptionOptions.ON;
						}
						break;
					case EncryptionOptions.ON:
						if (encryptionOptions == EncryptionOptions.NOT_SUP)
						{
							this._physicalStateObj.AddError(new SqlError(20, 0, 20, this._server, SQLMessage.EncryptionNotSupportedByServer(), "", 0, null));
							this._physicalStateObj.Dispose();
							this.ThrowExceptionAndWarning(this._physicalStateObj, false, false);
						}
						break;
					case EncryptionOptions.NOT_SUP:
						if (encryptionOptions == EncryptionOptions.REQ)
						{
							this._physicalStateObj.AddError(new SqlError(20, 0, 20, this._server, SQLMessage.EncryptionNotSupportedByClient(), "", 0, null));
							this._physicalStateObj.Dispose();
							this.ThrowExceptionAndWarning(this._physicalStateObj, false, false);
						}
						break;
					}
					if (this._encryptionOption == EncryptionOptions.ON || this._encryptionOption == EncryptionOptions.LOGIN)
					{
						uint num5 = 0U;
						uint num6 = ((encrypt && !trustServerCert) ? 1U : 0U) | (flag ? 2U : 0U);
						if (encrypt && !integratedSecurity)
						{
							num6 |= 16U;
						}
						num5 = this._physicalStateObj.EnableSsl(ref num6);
						if (num5 != 0U)
						{
							this._physicalStateObj.AddError(this.ProcessSNIError(this._physicalStateObj));
							this.ThrowExceptionAndWarning(this._physicalStateObj, false, false);
						}
						this.WaitForSSLHandShakeToComplete(ref num5);
						this._physicalStateObj.ClearAllWritePackets();
					}
					break;
				}
				case 2:
				{
					int num3 = ((int)array[num++] << 8) | (int)array[num++];
					byte b8 = array[num++];
					byte b9 = array[num++];
					byte b10 = 1;
					if (array[num3] == b10)
					{
						return PreLoginHandshakeStatus.InstanceFailure;
					}
					break;
				}
				case 3:
					num += 4;
					break;
				case 4:
				{
					int num3 = ((int)array[num++] << 8) | (int)array[num++];
					byte b11 = array[num++];
					byte b12 = array[num++];
					marsCapable = array[num3] != 0;
					break;
				}
				case 5:
					num += 4;
					break;
				default:
					num += 4;
					break;
				}
				if (num >= array.Length)
				{
					break;
				}
			}
			return PreLoginHandshakeStatus.Successful;
		}

		internal void Deactivate(bool connectionIsDoomed)
		{
			if (this.MARSOn)
			{
				this._sessionPool.Deactivate();
			}
			if (!connectionIsDoomed && this._physicalStateObj != null)
			{
				if (this._physicalStateObj._pendingData)
				{
					this.DrainData(this._physicalStateObj);
				}
				if (this._physicalStateObj.HasOpenResult)
				{
					this._physicalStateObj.DecrementOpenResultCount();
				}
			}
			SqlInternalTransaction currentTransaction = this.CurrentTransaction;
			if (currentTransaction != null && currentTransaction.HasParentTransaction)
			{
				currentTransaction.CloseFromConnection();
			}
			this.Statistics = null;
		}

		internal void Disconnect()
		{
			if (this._sessionPool != null)
			{
				this._sessionPool.Dispose();
			}
			if (this._state != TdsParserState.Closed)
			{
				this._state = TdsParserState.Closed;
				try
				{
					if (!this._physicalStateObj.HasOwner)
					{
						this._physicalStateObj.SniContext = SniContext.Snix_Close;
						this._physicalStateObj.Dispose();
					}
					else
					{
						this._physicalStateObj.DecrementPendingCallbacks(false);
					}
					if (this._pMarsPhysicalConObj != null)
					{
						this._pMarsPhysicalConObj.Dispose();
					}
				}
				finally
				{
					this._pMarsPhysicalConObj = null;
				}
			}
		}

		private void FireInfoMessageEvent(SqlConnection connection, TdsParserStateObject stateObj, SqlError error)
		{
			string text = null;
			if (this._state == TdsParserState.OpenLoggedIn)
			{
				text = this._connHandler.ServerVersion;
			}
			SqlException ex = SqlException.CreateException(new SqlErrorCollection { error }, text, this._connHandler, null);
			bool flag;
			connection.OnInfoMessage(new SqlInfoMessageEventArgs(ex), out flag);
			if (flag)
			{
				stateObj._syncOverAsync = true;
			}
		}

		internal void DisconnectTransaction(SqlInternalTransaction internalTransaction)
		{
			if (this._currentTransaction != null && this._currentTransaction == internalTransaction)
			{
				this._currentTransaction = null;
			}
		}

		internal void RollbackOrphanedAPITransactions()
		{
			SqlInternalTransaction currentTransaction = this.CurrentTransaction;
			if (currentTransaction != null && currentTransaction.HasParentTransaction && currentTransaction.IsOrphaned)
			{
				currentTransaction.CloseFromConnection();
			}
		}

		internal void ThrowExceptionAndWarning(TdsParserStateObject stateObj, bool callerHasConnectionLock = false, bool asyncClose = false)
		{
			SqlException ex = null;
			bool flag;
			SqlErrorCollection fullErrorAndWarningCollection = stateObj.GetFullErrorAndWarningCollection(out flag);
			flag &= this._state > TdsParserState.Closed;
			if (flag)
			{
				if (this._state == TdsParserState.OpenNotLoggedIn && (this._connHandler.ConnectionOptions.MultiSubnetFailover || this._loginWithFailover) && fullErrorAndWarningCollection.Count == 1 && (fullErrorAndWarningCollection[0].Number == -2 || (long)fullErrorAndWarningCollection[0].Number == 258L))
				{
					flag = false;
					this.Disconnect();
				}
				else
				{
					this._state = TdsParserState.Broken;
				}
			}
			if (fullErrorAndWarningCollection != null && fullErrorAndWarningCollection.Count > 0)
			{
				string text = null;
				if (this._state == TdsParserState.OpenLoggedIn)
				{
					text = this._connHandler.ServerVersion;
				}
				if (fullErrorAndWarningCollection.Count == 1 && fullErrorAndWarningCollection[0].Exception != null)
				{
					ex = SqlException.CreateException(fullErrorAndWarningCollection, text, this._connHandler, fullErrorAndWarningCollection[0].Exception);
				}
				else
				{
					ex = SqlException.CreateException(fullErrorAndWarningCollection, text, this._connHandler, null);
				}
			}
			if (ex != null)
			{
				if (flag)
				{
					TaskCompletionSource<object> networkPacketTaskSource = stateObj._networkPacketTaskSource;
					if (networkPacketTaskSource != null)
					{
						networkPacketTaskSource.TrySetException(ADP.ExceptionWithStackTrace(ex));
					}
				}
				if (asyncClose)
				{
					SqlInternalConnectionTds connHandler = this._connHandler;
					Action<Action> action = delegate(Action closeAction)
					{
						Task.Factory.StartNew(delegate
						{
							connHandler._parserLock.Wait(false);
							connHandler.ThreadHasParserLockForClose = true;
							try
							{
								closeAction();
							}
							finally
							{
								connHandler.ThreadHasParserLockForClose = false;
								connHandler._parserLock.Release();
							}
						});
					};
					this._connHandler.OnError(ex, flag, action);
					return;
				}
				bool threadHasParserLockForClose = this._connHandler.ThreadHasParserLockForClose;
				if (callerHasConnectionLock)
				{
					this._connHandler.ThreadHasParserLockForClose = true;
				}
				try
				{
					this._connHandler.OnError(ex, flag, null);
				}
				finally
				{
					if (callerHasConnectionLock)
					{
						this._connHandler.ThreadHasParserLockForClose = threadHasParserLockForClose;
					}
				}
			}
		}

		internal SqlError ProcessSNIError(TdsParserStateObject stateObj)
		{
			SNIErrorDetails sniErrorDetails = this.GetSniErrorDetails();
			if (sniErrorDetails.sniErrorNumber != 0U)
			{
				switch (sniErrorDetails.sniErrorNumber)
				{
				case 47U:
					throw SQL.MultiSubnetFailoverWithMoreThan64IPs();
				case 48U:
					throw SQL.MultiSubnetFailoverWithInstanceSpecified();
				case 49U:
					throw SQL.MultiSubnetFailoverWithNonTcpProtocol();
				}
			}
			string text = sniErrorDetails.errorMessage;
			bool useManagedSNI = TdsParserStateObjectFactory.UseManagedSNI;
			string sniContextEnumName = TdsEnums.GetSniContextEnumName(stateObj.SniContext);
			string resourceString = SR.GetResourceString(sniContextEnumName, sniContextEnumName);
			string text2 = string.Format(null, "SNI_PN{0}", sniErrorDetails.provider);
			string resourceString2 = SR.GetResourceString(text2, text2);
			if (sniErrorDetails.sniErrorNumber == 0U)
			{
				int num = text.IndexOf(':');
				if (0 <= num)
				{
					int num2 = text.Length;
					num2 -= Environment.NewLine.Length;
					num += 2;
					num2 -= num;
					if (num2 > 0)
					{
						text = text.Substring(num, num2);
					}
				}
			}
			else if (TdsParserStateObjectFactory.UseManagedSNI)
			{
				string snierrorMessage = SQL.GetSNIErrorMessage((int)sniErrorDetails.sniErrorNumber);
				text = ((text != string.Empty) ? (snierrorMessage + ": " + text) : snierrorMessage);
			}
			else
			{
				text = SQL.GetSNIErrorMessage((int)sniErrorDetails.sniErrorNumber);
				if (sniErrorDetails.sniErrorNumber == 50U)
				{
					text += LocalDBAPI.GetLocalDBMessage((int)sniErrorDetails.nativeError);
				}
			}
			text = string.Format(null, "{0} (provider: {1}, error: {2} - {3})", new object[]
			{
				resourceString,
				resourceString2,
				(int)sniErrorDetails.sniErrorNumber,
				text
			});
			return new SqlError((int)sniErrorDetails.nativeError, 0, 20, this._server, text, sniErrorDetails.function, (int)sniErrorDetails.lineNumber, sniErrorDetails.nativeError, sniErrorDetails.exception);
		}

		internal void CheckResetConnection(TdsParserStateObject stateObj)
		{
			if (this._fResetConnection && !stateObj._fResetConnectionSent)
			{
				try
				{
					if (this._fMARS && !stateObj._fResetEventOwned)
					{
						stateObj._fResetEventOwned = this._resetConnectionEvent.WaitOne(stateObj.GetTimeoutRemaining());
						if (stateObj._fResetEventOwned && stateObj.TimeoutHasExpired)
						{
							stateObj._fResetEventOwned = !this._resetConnectionEvent.Set();
							stateObj.TimeoutTime = 0L;
						}
						if (!stateObj._fResetEventOwned)
						{
							stateObj.ResetBuffer();
							stateObj.AddError(new SqlError(-2, 0, 11, this._server, this._connHandler.TimeoutErrorInternal.GetErrorMessage(), "", 0, 258U, null));
							this.ThrowExceptionAndWarning(stateObj, true, false);
						}
					}
					if (this._fResetConnection)
					{
						if (this._fPreserveTransaction)
						{
							stateObj._outBuff[1] = stateObj._outBuff[1] | 16;
						}
						else
						{
							stateObj._outBuff[1] = stateObj._outBuff[1] | 8;
						}
						if (!this._fMARS)
						{
							this._fResetConnection = false;
							this._fPreserveTransaction = false;
						}
						else
						{
							stateObj._fResetConnectionSent = true;
						}
					}
					else if (this._fMARS && stateObj._fResetEventOwned)
					{
						stateObj._fResetEventOwned = !this._resetConnectionEvent.Set();
					}
				}
				catch (Exception)
				{
					if (this._fMARS && stateObj._fResetEventOwned)
					{
						stateObj._fResetConnectionSent = false;
						stateObj._fResetEventOwned = !this._resetConnectionEvent.Set();
					}
					throw;
				}
			}
		}

		internal void WriteShort(int v, TdsParserStateObject stateObj)
		{
			if (stateObj._outBytesUsed + 2 > stateObj._outBuff.Length)
			{
				stateObj.WriteByte((byte)(v & 255));
				stateObj.WriteByte((byte)((v >> 8) & 255));
				return;
			}
			stateObj._outBuff[stateObj._outBytesUsed] = (byte)(v & 255);
			stateObj._outBuff[stateObj._outBytesUsed + 1] = (byte)((v >> 8) & 255);
			stateObj._outBytesUsed += 2;
		}

		internal void WriteUnsignedShort(ushort us, TdsParserStateObject stateObj)
		{
			this.WriteShort((int)((short)us), stateObj);
		}

		internal void WriteUnsignedInt(uint i, TdsParserStateObject stateObj)
		{
			this.WriteInt((int)i, stateObj);
		}

		internal void WriteInt(int v, TdsParserStateObject stateObj)
		{
			if (stateObj._outBytesUsed + 4 > stateObj._outBuff.Length)
			{
				for (int i = 0; i < 32; i += 8)
				{
					stateObj.WriteByte((byte)((v >> i) & 255));
				}
				return;
			}
			stateObj._outBuff[stateObj._outBytesUsed] = (byte)(v & 255);
			stateObj._outBuff[stateObj._outBytesUsed + 1] = (byte)((v >> 8) & 255);
			stateObj._outBuff[stateObj._outBytesUsed + 2] = (byte)((v >> 16) & 255);
			stateObj._outBuff[stateObj._outBytesUsed + 3] = (byte)((v >> 24) & 255);
			stateObj._outBytesUsed += 4;
		}

		internal void WriteFloat(float v, TdsParserStateObject stateObj)
		{
			byte[] bytes = BitConverter.GetBytes(v);
			stateObj.WriteByteArray(bytes, bytes.Length, 0, true, null);
		}

		internal void WriteLong(long v, TdsParserStateObject stateObj)
		{
			if (stateObj._outBytesUsed + 8 > stateObj._outBuff.Length)
			{
				for (int i = 0; i < 64; i += 8)
				{
					stateObj.WriteByte((byte)((v >> i) & 255L));
				}
				return;
			}
			stateObj._outBuff[stateObj._outBytesUsed] = (byte)(v & 255L);
			stateObj._outBuff[stateObj._outBytesUsed + 1] = (byte)((v >> 8) & 255L);
			stateObj._outBuff[stateObj._outBytesUsed + 2] = (byte)((v >> 16) & 255L);
			stateObj._outBuff[stateObj._outBytesUsed + 3] = (byte)((v >> 24) & 255L);
			stateObj._outBuff[stateObj._outBytesUsed + 4] = (byte)((v >> 32) & 255L);
			stateObj._outBuff[stateObj._outBytesUsed + 5] = (byte)((v >> 40) & 255L);
			stateObj._outBuff[stateObj._outBytesUsed + 6] = (byte)((v >> 48) & 255L);
			stateObj._outBuff[stateObj._outBytesUsed + 7] = (byte)((v >> 56) & 255L);
			stateObj._outBytesUsed += 8;
		}

		internal void WritePartialLong(long v, int length, TdsParserStateObject stateObj)
		{
			if (stateObj._outBytesUsed + length > stateObj._outBuff.Length)
			{
				for (int i = 0; i < length * 8; i += 8)
				{
					stateObj.WriteByte((byte)((v >> i) & 255L));
				}
				return;
			}
			for (int j = 0; j < length; j++)
			{
				stateObj._outBuff[stateObj._outBytesUsed + j] = (byte)((v >> j * 8) & 255L);
			}
			stateObj._outBytesUsed += length;
		}

		internal void WriteUnsignedLong(ulong uv, TdsParserStateObject stateObj)
		{
			this.WriteLong((long)uv, stateObj);
		}

		internal void WriteDouble(double v, TdsParserStateObject stateObj)
		{
			byte[] bytes = BitConverter.GetBytes(v);
			stateObj.WriteByteArray(bytes, bytes.Length, 0, true, null);
		}

		internal void PrepareResetConnection(bool preserveTransaction)
		{
			this._fResetConnection = true;
			this._fPreserveTransaction = preserveTransaction;
		}

		internal bool Run(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj)
		{
			bool syncOverAsync = stateObj._syncOverAsync;
			bool flag2;
			try
			{
				stateObj._syncOverAsync = true;
				bool flag;
				this.TryRun(runBehavior, cmdHandler, dataStream, bulkCopyHandler, stateObj, out flag);
				flag2 = flag;
			}
			finally
			{
				stateObj._syncOverAsync = syncOverAsync;
			}
			return flag2;
		}

		internal static bool IsValidTdsToken(byte token)
		{
			return token == 170 || token == 171 || token == 173 || token == 227 || token == 172 || token == 121 || token == 160 || token == 161 || token == 129 || token == 136 || token == 164 || token == 165 || token == 169 || token == 211 || token == 209 || token == 210 || token == 253 || token == 254 || token == byte.MaxValue || token == 57 || token == 237 || token == 124 || token == 120 || token == 237 || token == 174 || token == 228;
		}

		internal bool TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, out bool dataReady)
		{
			if (TdsParserState.Broken == this.State || this.State == TdsParserState.Closed)
			{
				dataReady = true;
				return true;
			}
			dataReady = false;
			for (;;)
			{
				if (stateObj._internalTimeout)
				{
					runBehavior = RunBehavior.Attention;
				}
				if (TdsParserState.Broken == this.State || this.State == TdsParserState.Closed)
				{
					goto IL_090C;
				}
				if (!stateObj._accumulateInfoEvents && stateObj._pendingInfoEvents != null)
				{
					if (RunBehavior.Clean != (RunBehavior.Clean & runBehavior))
					{
						SqlConnection sqlConnection = null;
						if (this._connHandler != null)
						{
							sqlConnection = this._connHandler.Connection;
						}
						if (sqlConnection != null && sqlConnection.FireInfoMessageEventOnUserErrors)
						{
							using (List<SqlError>.Enumerator enumerator = stateObj._pendingInfoEvents.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									SqlError sqlError = enumerator.Current;
									this.FireInfoMessageEvent(sqlConnection, stateObj, sqlError);
								}
								goto IL_0123;
							}
						}
						foreach (SqlError sqlError2 in stateObj._pendingInfoEvents)
						{
							stateObj.AddWarning(sqlError2);
						}
					}
					IL_0123:
					stateObj._pendingInfoEvents = null;
				}
				byte b;
				if (!stateObj.TryReadByte(out b))
				{
					break;
				}
				if (!TdsParser.IsValidTdsToken(b))
				{
					goto Block_14;
				}
				int num;
				if (!this.TryGetTokenLength(b, stateObj, out num))
				{
					return false;
				}
				if (b <= 210)
				{
					if (b <= 129)
					{
						if (b != 121)
						{
							if (b == 129)
							{
								if (num != 65535)
								{
									_SqlMetaDataSet sqlMetaDataSet;
									if (!this.TryProcessMetaData(num, stateObj, out sqlMetaDataSet))
									{
										return false;
									}
									stateObj._cleanupMetaData = sqlMetaDataSet;
								}
								else if (cmdHandler != null)
								{
									stateObj._cleanupMetaData = cmdHandler.MetaData;
								}
								if (dataStream != null)
								{
									byte b2;
									if (!stateObj.TryPeekByte(out b2))
									{
										return false;
									}
									if (!dataStream.TrySetMetaData(stateObj._cleanupMetaData, 164 == b2 || 165 == b2))
									{
										return false;
									}
								}
								else if (bulkCopyHandler != null)
								{
									bulkCopyHandler.SetMetaData(stateObj._cleanupMetaData);
								}
							}
						}
						else
						{
							int num2;
							if (!stateObj.TryReadInt32(out num2))
							{
								return false;
							}
							if (cmdHandler != null)
							{
								cmdHandler.OnReturnStatus(num2);
							}
						}
					}
					else if (b != 136)
					{
						switch (b)
						{
						case 164:
							if (dataStream != null)
							{
								MultiPartTableName[] array;
								if (!this.TryProcessTableName(num, stateObj, out array))
								{
									return false;
								}
								dataStream.TableNames = array;
							}
							else if (!stateObj.TrySkipBytes(num))
							{
								return false;
							}
							break;
						case 165:
							if (dataStream != null)
							{
								_SqlMetaDataSet sqlMetaDataSet2;
								if (!this.TryProcessColInfo(dataStream.MetaData, dataStream, stateObj, out sqlMetaDataSet2))
								{
									return false;
								}
								if (!dataStream.TrySetMetaData(sqlMetaDataSet2, false))
								{
									return false;
								}
								dataStream.BrowseModeInfoConsumed = true;
							}
							else if (!stateObj.TrySkipBytes(num))
							{
								return false;
							}
							break;
						case 166:
						case 167:
						case 168:
							break;
						case 169:
							if (!stateObj.TrySkipBytes(num))
							{
								return false;
							}
							break;
						case 170:
						case 171:
						{
							if (b == 170)
							{
								stateObj._errorTokenReceived = true;
							}
							SqlError sqlError3;
							if (!this.TryProcessError(b, stateObj, out sqlError3))
							{
								return false;
							}
							if (b == 171 && stateObj._accumulateInfoEvents)
							{
								if (stateObj._pendingInfoEvents == null)
								{
									stateObj._pendingInfoEvents = new List<SqlError>();
								}
								stateObj._pendingInfoEvents.Add(sqlError3);
								stateObj._syncOverAsync = true;
							}
							else if (RunBehavior.Clean != (RunBehavior.Clean & runBehavior))
							{
								SqlConnection sqlConnection2 = null;
								if (this._connHandler != null)
								{
									sqlConnection2 = this._connHandler.Connection;
								}
								if (sqlConnection2 != null && sqlConnection2.FireInfoMessageEventOnUserErrors && sqlError3.Class <= 16)
								{
									this.FireInfoMessageEvent(sqlConnection2, stateObj, sqlError3);
								}
								else if (sqlError3.Class < 11)
								{
									stateObj.AddWarning(sqlError3);
								}
								else if (sqlError3.Class < 20)
								{
									stateObj.AddError(sqlError3);
									if (dataStream != null && !dataStream.IsInitialized)
									{
										runBehavior = RunBehavior.UntilDone;
									}
								}
								else
								{
									stateObj.AddError(sqlError3);
									runBehavior = RunBehavior.UntilDone;
								}
							}
							else if (sqlError3.Class >= 20)
							{
								stateObj.AddError(sqlError3);
							}
							break;
						}
						case 172:
						{
							SqlReturnValue sqlReturnValue;
							if (!this.TryProcessReturnValue(num, stateObj, out sqlReturnValue))
							{
								return false;
							}
							if (cmdHandler != null)
							{
								cmdHandler.OnReturnValue(sqlReturnValue);
							}
							break;
						}
						case 173:
						{
							SqlLoginAck sqlLoginAck;
							if (!this.TryProcessLoginAck(stateObj, out sqlLoginAck))
							{
								return false;
							}
							this._connHandler.OnLoginAck(sqlLoginAck);
							break;
						}
						case 174:
							if (!this.TryProcessFeatureExtAck(stateObj))
							{
								return false;
							}
							break;
						default:
							if (b - 209 <= 1)
							{
								if (b == 210)
								{
									if (!stateObj.TryStartNewRow(true, stateObj._cleanupMetaData.Length))
									{
										return false;
									}
								}
								else if (!stateObj.TryStartNewRow(false, 0))
								{
									return false;
								}
								if (bulkCopyHandler != null)
								{
									if (!this.TryProcessRow(stateObj._cleanupMetaData, bulkCopyHandler.CreateRowBuffer(), bulkCopyHandler.CreateIndexMap(), stateObj))
									{
										return false;
									}
								}
								else if (RunBehavior.ReturnImmediately != (RunBehavior.ReturnImmediately & runBehavior))
								{
									if (!this.TrySkipRow(stateObj._cleanupMetaData, stateObj))
									{
										return false;
									}
								}
								else
								{
									dataReady = true;
								}
								if (this._statistics != null)
								{
									this._statistics.WaitForDoneAfterRow = true;
								}
							}
							break;
						}
					}
					else
					{
						stateObj.CloneCleanupAltMetaDataSetArray();
						if (stateObj._cleanupAltMetaDataSetArray == null)
						{
							stateObj._cleanupAltMetaDataSetArray = new _SqlMetaDataSetCollection();
						}
						_SqlMetaDataSet sqlMetaDataSet3;
						if (!this.TryProcessAltMetaData(num, stateObj, out sqlMetaDataSet3))
						{
							return false;
						}
						stateObj._cleanupAltMetaDataSetArray.SetAltMetaData(sqlMetaDataSet3);
						if (dataStream != null)
						{
							byte b3;
							if (!stateObj.TryPeekByte(out b3))
							{
								return false;
							}
							if (!dataStream.TrySetAltMetaDataSet(sqlMetaDataSet3, 136 != b3))
							{
								return false;
							}
						}
					}
				}
				else if (b <= 227)
				{
					if (b != 211)
					{
						if (b == 227)
						{
							stateObj._syncOverAsync = true;
							SqlEnvChange[] array2;
							if (!this.TryProcessEnvChange(num, stateObj, out array2))
							{
								return false;
							}
							for (int i = 0; i < array2.Length; i++)
							{
								if (array2[i] != null && !this.Connection.IgnoreEnvChange)
								{
									switch (array2[i].type)
									{
									case 8:
									case 11:
										this._currentTransaction = this._pendingTransaction;
										this._pendingTransaction = null;
										if (this._currentTransaction != null)
										{
											this._currentTransaction.TransactionId = array2[i].newLongValue;
										}
										else
										{
											TransactionType transactionType = TransactionType.LocalFromTSQL;
											this._currentTransaction = new SqlInternalTransaction(this._connHandler, transactionType, null, array2[i].newLongValue);
										}
										if (this._statistics != null && !this._statisticsIsInTransaction)
										{
											this._statistics.SafeIncrement(ref this._statistics._transactions);
										}
										this._statisticsIsInTransaction = true;
										this._retainedTransactionId = 0L;
										goto IL_0697;
									case 9:
									case 12:
									case 17:
										this._retainedTransactionId = 0L;
										break;
									case 10:
										break;
									case 13:
									case 14:
									case 15:
									case 16:
										goto IL_0687;
									default:
										goto IL_0687;
									}
									if (this._currentTransaction != null)
									{
										if (9 == array2[i].type)
										{
											this._currentTransaction.Completed(TransactionState.Committed);
										}
										else if (10 == array2[i].type)
										{
											if (this._currentTransaction.IsDistributed && this._currentTransaction.IsActive)
											{
												this._retainedTransactionId = array2[i].oldLongValue;
											}
											this._currentTransaction.Completed(TransactionState.Aborted);
										}
										else
										{
											this._currentTransaction.Completed(TransactionState.Unknown);
										}
										this._currentTransaction = null;
									}
									this._statisticsIsInTransaction = false;
									goto IL_0697;
									IL_0687:
									this._connHandler.OnEnvChange(array2[i]);
								}
								IL_0697:;
							}
						}
					}
					else
					{
						if (!stateObj.TryStartNewRow(false, 0))
						{
							return false;
						}
						if (RunBehavior.ReturnImmediately != (RunBehavior.ReturnImmediately & runBehavior))
						{
							ushort num3;
							if (!stateObj.TryReadUInt16(out num3))
							{
								return false;
							}
							if (!this.TrySkipRow(stateObj._cleanupAltMetaDataSetArray.GetAltMetaData((int)num3), stateObj))
							{
								return false;
							}
						}
						else
						{
							dataReady = true;
						}
					}
				}
				else if (b != 228)
				{
					if (b != 237)
					{
						if (b - 253 <= 2)
						{
							if (!this.TryProcessDone(cmdHandler, dataStream, ref runBehavior, stateObj))
							{
								return false;
							}
							if (b == 254 && cmdHandler != null)
							{
								cmdHandler.OnDoneProc();
							}
						}
					}
					else
					{
						stateObj._syncOverAsync = true;
						this.ProcessSSPI(num);
					}
				}
				else if (!this.TryProcessSessionState(stateObj, num, this._connHandler._currentSessionData))
				{
					return false;
				}
				if ((!stateObj._pendingData || RunBehavior.ReturnImmediately == (RunBehavior.ReturnImmediately & runBehavior)) && (stateObj._pendingData || !stateObj._attentionSent || stateObj._attentionReceived))
				{
					goto IL_090C;
				}
			}
			return false;
			Block_14:
			this._state = TdsParserState.Broken;
			this._connHandler.BreakConnection();
			throw SQL.ParsingError();
			IL_090C:
			if (!stateObj._pendingData && this.CurrentTransaction != null)
			{
				this.CurrentTransaction.Activate();
			}
			if (stateObj._attentionReceived)
			{
				SpinWait.SpinUntil(() => !stateObj._attentionSending);
				if (stateObj._attentionSent)
				{
					stateObj._attentionSent = false;
					stateObj._attentionReceived = false;
					if (RunBehavior.Clean != (RunBehavior.Clean & runBehavior) && !stateObj._internalTimeout)
					{
						stateObj.AddError(new SqlError(0, 0, 11, this._server, SQLMessage.OperationCancelled(), "", 0, null));
					}
				}
			}
			if (stateObj.HasErrorOrWarning)
			{
				this.ThrowExceptionAndWarning(stateObj, false, false);
			}
			return true;
		}

		private bool TryProcessEnvChange(int tokenLength, TdsParserStateObject stateObj, out SqlEnvChange[] sqlEnvChange)
		{
			int num = 0;
			int num2 = 0;
			SqlEnvChange[] array = new SqlEnvChange[3];
			sqlEnvChange = null;
			while (tokenLength > num)
			{
				if (num2 >= array.Length)
				{
					SqlEnvChange[] array2 = new SqlEnvChange[array.Length + 3];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = array[i];
					}
					array = array2;
				}
				SqlEnvChange sqlEnvChange2 = new SqlEnvChange();
				if (!stateObj.TryReadByte(out sqlEnvChange2.type))
				{
					return false;
				}
				array[num2] = sqlEnvChange2;
				num2++;
				switch (sqlEnvChange2.type)
				{
				case 1:
				case 2:
					if (!this.TryReadTwoStringFields(sqlEnvChange2, stateObj))
					{
						return false;
					}
					break;
				case 3:
					if (!this.TryReadTwoStringFields(sqlEnvChange2, stateObj))
					{
						return false;
					}
					if (sqlEnvChange2.newValue == "iso_1")
					{
						this._defaultCodePage = 1252;
						this._defaultEncoding = Encoding.GetEncoding(this._defaultCodePage);
					}
					else
					{
						string text = sqlEnvChange2.newValue.Substring(2);
						this._defaultCodePage = int.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture);
						this._defaultEncoding = Encoding.GetEncoding(this._defaultCodePage);
					}
					break;
				case 4:
				{
					if (!this.TryReadTwoStringFields(sqlEnvChange2, stateObj))
					{
						throw SQL.SynchronousCallMayNotPend();
					}
					int num3 = int.Parse(sqlEnvChange2.newValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
					if (this._physicalStateObj.SetPacketSize(num3))
					{
						this._physicalStateObj.ClearAllWritePackets();
						uint num4 = (uint)num3;
						this._physicalStateObj.SetConnectionBufferSize(ref num4);
					}
					break;
				}
				case 5:
					if (!this.TryReadTwoStringFields(sqlEnvChange2, stateObj))
					{
						return false;
					}
					this._defaultLCID = int.Parse(sqlEnvChange2.newValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
					break;
				case 6:
					if (!this.TryReadTwoStringFields(sqlEnvChange2, stateObj))
					{
						return false;
					}
					break;
				case 7:
				{
					byte b;
					if (!stateObj.TryReadByte(out b))
					{
						return false;
					}
					sqlEnvChange2.newLength = (int)b;
					if (sqlEnvChange2.newLength == 5)
					{
						if (!this.TryProcessCollation(stateObj, out sqlEnvChange2.newCollation))
						{
							return false;
						}
						this._defaultCollation = sqlEnvChange2.newCollation;
						int codePage = this.GetCodePage(sqlEnvChange2.newCollation, stateObj);
						if (codePage != this._defaultCodePage)
						{
							this._defaultCodePage = codePage;
							this._defaultEncoding = Encoding.GetEncoding(this._defaultCodePage);
						}
						this._defaultLCID = sqlEnvChange2.newCollation.LCID;
					}
					if (!stateObj.TryReadByte(out b))
					{
						return false;
					}
					sqlEnvChange2.oldLength = b;
					if (sqlEnvChange2.oldLength == 5 && !this.TryProcessCollation(stateObj, out sqlEnvChange2.oldCollation))
					{
						return false;
					}
					sqlEnvChange2.length = 3 + sqlEnvChange2.newLength + (int)sqlEnvChange2.oldLength;
					break;
				}
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 17:
				{
					byte b;
					if (!stateObj.TryReadByte(out b))
					{
						return false;
					}
					sqlEnvChange2.newLength = (int)b;
					if (sqlEnvChange2.newLength > 0)
					{
						if (!stateObj.TryReadInt64(out sqlEnvChange2.newLongValue))
						{
							return false;
						}
					}
					else
					{
						sqlEnvChange2.newLongValue = 0L;
					}
					if (!stateObj.TryReadByte(out b))
					{
						return false;
					}
					sqlEnvChange2.oldLength = b;
					if (sqlEnvChange2.oldLength > 0)
					{
						if (!stateObj.TryReadInt64(out sqlEnvChange2.oldLongValue))
						{
							return false;
						}
					}
					else
					{
						sqlEnvChange2.oldLongValue = 0L;
					}
					sqlEnvChange2.length = 3 + sqlEnvChange2.newLength + (int)sqlEnvChange2.oldLength;
					break;
				}
				case 13:
					if (!this.TryReadTwoStringFields(sqlEnvChange2, stateObj))
					{
						return false;
					}
					break;
				case 15:
				{
					if (!stateObj.TryReadInt32(out sqlEnvChange2.newLength))
					{
						return false;
					}
					sqlEnvChange2.newBinValue = new byte[sqlEnvChange2.newLength];
					if (!stateObj.TryReadByteArray(sqlEnvChange2.newBinValue, 0, sqlEnvChange2.newLength))
					{
						return false;
					}
					byte b;
					if (!stateObj.TryReadByte(out b))
					{
						return false;
					}
					sqlEnvChange2.oldLength = b;
					sqlEnvChange2.length = 5 + sqlEnvChange2.newLength;
					break;
				}
				case 16:
				case 18:
					if (!this.TryReadTwoBinaryFields(sqlEnvChange2, stateObj))
					{
						return false;
					}
					break;
				case 19:
					if (!this.TryReadTwoStringFields(sqlEnvChange2, stateObj))
					{
						return false;
					}
					break;
				case 20:
				{
					ushort num5;
					if (!stateObj.TryReadUInt16(out num5))
					{
						return false;
					}
					sqlEnvChange2.newLength = (int)num5;
					byte b2;
					if (!stateObj.TryReadByte(out b2))
					{
						return false;
					}
					ushort num6;
					if (!stateObj.TryReadUInt16(out num6))
					{
						return false;
					}
					ushort num7;
					if (!stateObj.TryReadUInt16(out num7))
					{
						return false;
					}
					string text2;
					if (!stateObj.TryReadString((int)num7, out text2))
					{
						return false;
					}
					sqlEnvChange2.newRoutingInfo = new RoutingInfo(b2, num6, text2);
					ushort num8;
					if (!stateObj.TryReadUInt16(out num8))
					{
						return false;
					}
					if (!stateObj.TrySkipBytes((int)num8))
					{
						return false;
					}
					sqlEnvChange2.length = sqlEnvChange2.newLength + (int)num8 + 5;
					break;
				}
				}
				num += sqlEnvChange2.length;
			}
			sqlEnvChange = array;
			return true;
		}

		private bool TryReadTwoBinaryFields(SqlEnvChange env, TdsParserStateObject stateObj)
		{
			byte b;
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			env.newLength = (int)b;
			env.newBinValue = new byte[env.newLength];
			if (!stateObj.TryReadByteArray(env.newBinValue, 0, env.newLength))
			{
				return false;
			}
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			env.oldLength = b;
			env.oldBinValue = new byte[(int)env.oldLength];
			if (!stateObj.TryReadByteArray(env.oldBinValue, 0, (int)env.oldLength))
			{
				return false;
			}
			env.length = 3 + env.newLength + (int)env.oldLength;
			return true;
		}

		private bool TryReadTwoStringFields(SqlEnvChange env, TdsParserStateObject stateObj)
		{
			byte b;
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			string text;
			if (!stateObj.TryReadString((int)b, out text))
			{
				return false;
			}
			byte b2;
			if (!stateObj.TryReadByte(out b2))
			{
				return false;
			}
			string text2;
			if (!stateObj.TryReadString((int)b2, out text2))
			{
				return false;
			}
			env.newLength = (int)b;
			env.newValue = text;
			env.oldLength = b2;
			env.oldValue = text2;
			env.length = 3 + env.newLength * 2 + (int)(env.oldLength * 2);
			return true;
		}

		private bool TryProcessDone(SqlCommand cmd, SqlDataReader reader, ref RunBehavior run, TdsParserStateObject stateObj)
		{
			stateObj._syncOverAsync = true;
			ushort num;
			if (!stateObj.TryReadUInt16(out num))
			{
				return false;
			}
			ushort num2;
			if (!stateObj.TryReadUInt16(out num2))
			{
				return false;
			}
			long num3;
			if (!stateObj.TryReadInt64(out num3))
			{
				return false;
			}
			int num4 = (int)num3;
			if (32 == (num & 32))
			{
				stateObj._attentionReceived = true;
			}
			if (cmd != null && 16 == (num & 16))
			{
				if (num2 != 193)
				{
					cmd.InternalRecordsAffected = num4;
				}
				if (stateObj._receivedColMetaData || num2 != 193)
				{
					cmd.OnStatementCompleted(num4);
				}
			}
			stateObj._receivedColMetaData = false;
			if (2 == (2 & num) && stateObj.ErrorCount == 0 && !stateObj._errorTokenReceived && RunBehavior.Clean != (RunBehavior.Clean & run))
			{
				stateObj.AddError(new SqlError(0, 0, 11, this._server, SQLMessage.SevereError(), "", 0, null));
				if (reader != null && !reader.IsInitialized)
				{
					run = RunBehavior.UntilDone;
				}
			}
			if (256 == (256 & num) && RunBehavior.Clean != (RunBehavior.Clean & run))
			{
				stateObj.AddError(new SqlError(0, 0, 20, this._server, SQLMessage.SevereError(), "", 0, null));
				if (reader != null && !reader.IsInitialized)
				{
					run = RunBehavior.UntilDone;
				}
			}
			this.ProcessSqlStatistics(num2, num, num4);
			if (1 != (num & 1))
			{
				stateObj._errorTokenReceived = false;
				if (stateObj._inBytesUsed >= stateObj._inBytesRead)
				{
					stateObj._pendingData = false;
				}
			}
			if (!stateObj._pendingData && stateObj._hasOpenResult)
			{
				stateObj.DecrementOpenResultCount();
			}
			return true;
		}

		private void ProcessSqlStatistics(ushort curCmd, ushort status, int count)
		{
			if (this._statistics != null)
			{
				if (this._statistics.WaitForDoneAfterRow)
				{
					this._statistics.SafeIncrement(ref this._statistics._sumResultSets);
					this._statistics.WaitForDoneAfterRow = false;
				}
				if (16 != (status & 16))
				{
					count = 0;
				}
				if (curCmd <= 193)
				{
					if (curCmd == 32)
					{
						this._statistics.SafeIncrement(ref this._statistics._cursorOpens);
						return;
					}
					if (curCmd != 193)
					{
						return;
					}
					this._statistics.SafeIncrement(ref this._statistics._selectCount);
					this._statistics.SafeAdd(ref this._statistics._selectRows, (long)count);
					return;
				}
				else
				{
					if (curCmd - 195 > 2)
					{
						switch (curCmd)
						{
						case 210:
							this._statisticsIsInTransaction = false;
							return;
						case 211:
							return;
						case 212:
							if (!this._statisticsIsInTransaction)
							{
								this._statistics.SafeIncrement(ref this._statistics._transactions);
							}
							this._statisticsIsInTransaction = true;
							return;
						case 213:
							this._statisticsIsInTransaction = false;
							return;
						default:
							if (curCmd != 279)
							{
								return;
							}
							break;
						}
					}
					this._statistics.SafeIncrement(ref this._statistics._iduCount);
					this._statistics.SafeAdd(ref this._statistics._iduRows, (long)count);
					if (!this._statisticsIsInTransaction)
					{
						this._statistics.SafeIncrement(ref this._statistics._transactions);
						return;
					}
				}
			}
			else
			{
				switch (curCmd)
				{
				case 210:
				case 213:
					this._statisticsIsInTransaction = false;
					break;
				case 211:
					break;
				case 212:
					this._statisticsIsInTransaction = true;
					return;
				default:
					return;
				}
			}
		}

		private bool TryProcessFeatureExtAck(TdsParserStateObject stateObj)
		{
			byte b;
			while (stateObj.TryReadByte(out b))
			{
				if (b != 255)
				{
					uint num;
					if (!stateObj.TryReadUInt32(out num))
					{
						return false;
					}
					byte[] array = new byte[num];
					if (num > 0U && !stateObj.TryReadByteArray(array, 0, checked((int)num)))
					{
						return false;
					}
					this._connHandler.OnFeatureExtAck((int)b, array);
				}
				if (b == 255)
				{
					return true;
				}
			}
			return false;
		}

		private bool TryProcessSessionState(TdsParserStateObject stateObj, int length, SessionData sdata)
		{
			if (length < 5)
			{
				throw SQL.ParsingError();
			}
			uint num;
			if (!stateObj.TryReadUInt32(out num))
			{
				return false;
			}
			if (num == 4294967295U)
			{
				this._connHandler.DoNotPoolThisConnection();
			}
			byte b;
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			if (b > 1)
			{
				throw SQL.ParsingError();
			}
			bool flag = b > 0;
			length -= 5;
			while (length > 0)
			{
				byte b2;
				if (!stateObj.TryReadByte(out b2))
				{
					return false;
				}
				byte b3;
				if (!stateObj.TryReadByte(out b3))
				{
					return false;
				}
				int num2;
				if (b3 < 255)
				{
					num2 = (int)b3;
				}
				else if (!stateObj.TryReadInt32(out num2))
				{
					return false;
				}
				byte[] array = null;
				SessionStateRecord[] delta = sdata._delta;
				checked
				{
					lock (delta)
					{
						if (sdata._delta[(int)b2] == null)
						{
							array = new byte[num2];
							sdata._delta[(int)b2] = new SessionStateRecord
							{
								_version = num,
								_dataLength = num2,
								_data = array,
								_recoverable = flag
							};
							sdata._deltaDirty = true;
							if (!flag)
							{
								sdata._unrecoverableStatesCount += 1;
							}
						}
						else if (sdata._delta[(int)b2]._version <= num)
						{
							SessionStateRecord sessionStateRecord = sdata._delta[(int)b2];
							sessionStateRecord._version = num;
							sessionStateRecord._dataLength = num2;
							if (sessionStateRecord._recoverable != flag)
							{
								if (flag)
								{
									unchecked
									{
										sdata._unrecoverableStatesCount -= 1;
									}
								}
								else
								{
									sdata._unrecoverableStatesCount += 1;
								}
								sessionStateRecord._recoverable = flag;
							}
							array = sessionStateRecord._data;
							if (array.Length < num2)
							{
								array = new byte[num2];
								sessionStateRecord._data = array;
							}
						}
					}
					if (array != null)
					{
						if (!stateObj.TryReadByteArray(array, 0, num2))
						{
							return false;
						}
					}
					else if (!stateObj.TrySkipBytes(num2))
					{
						return false;
					}
				}
				if (b3 < 255)
				{
					length -= 2 + num2;
				}
				else
				{
					length -= 6 + num2;
				}
			}
			return true;
		}

		private bool TryProcessLoginAck(TdsParserStateObject stateObj, out SqlLoginAck sqlLoginAck)
		{
			SqlLoginAck sqlLoginAck2 = new SqlLoginAck();
			sqlLoginAck = null;
			if (!stateObj.TrySkipBytes(1))
			{
				return false;
			}
			byte[] array = new byte[4];
			if (!stateObj.TryReadByteArray(array, 0, array.Length))
			{
				return false;
			}
			sqlLoginAck2.tdsVersion = (uint)(((((((int)array[0] << 8) | (int)array[1]) << 8) | (int)array[2]) << 8) | (int)array[3]);
			uint num = sqlLoginAck2.tdsVersion & 4278255615U;
			uint num2 = (sqlLoginAck2.tdsVersion >> 16) & 255U;
			if (num != 1912602626U)
			{
				if (num != 1929379843U)
				{
					if (num != 1946157060U)
					{
						throw SQL.InvalidTDSVersion();
					}
					if (num2 != 0U)
					{
						throw SQL.InvalidTDSVersion();
					}
					this._isDenali = true;
				}
				else
				{
					if (num2 != 11U)
					{
						throw SQL.InvalidTDSVersion();
					}
					this._isKatmai = true;
				}
			}
			else
			{
				if (num2 != 9U)
				{
					throw SQL.InvalidTDSVersion();
				}
				this._isYukon = true;
			}
			this._isKatmai |= this._isDenali;
			this._isYukon |= this._isKatmai;
			stateObj._outBytesUsed = stateObj._outputHeaderLen;
			byte b;
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			if (!stateObj.TrySkipBytes((int)(b * 2)))
			{
				return false;
			}
			if (!stateObj.TryReadByte(out sqlLoginAck2.majorVersion))
			{
				return false;
			}
			if (!stateObj.TryReadByte(out sqlLoginAck2.minorVersion))
			{
				return false;
			}
			byte b2;
			if (!stateObj.TryReadByte(out b2))
			{
				return false;
			}
			byte b3;
			if (!stateObj.TryReadByte(out b3))
			{
				return false;
			}
			sqlLoginAck2.buildNum = (short)(((int)b2 << 8) + (int)b3);
			this._state = TdsParserState.OpenLoggedIn;
			if (this._fMARS)
			{
				this._resetConnectionEvent = new AutoResetEvent(true);
			}
			if (this._connHandler.ConnectionOptions.UserInstance && string.IsNullOrEmpty(this._connHandler.InstanceName))
			{
				stateObj.AddError(new SqlError(0, 0, 20, this.Server, SQLMessage.UserInstanceFailure(), "", 0, null));
				this.ThrowExceptionAndWarning(stateObj, false, false);
			}
			sqlLoginAck = sqlLoginAck2;
			return true;
		}

		internal bool TryProcessError(byte token, TdsParserStateObject stateObj, out SqlError error)
		{
			error = null;
			int num;
			if (!stateObj.TryReadInt32(out num))
			{
				return false;
			}
			byte b;
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			byte b2;
			if (!stateObj.TryReadByte(out b2))
			{
				return false;
			}
			ushort num2;
			if (!stateObj.TryReadUInt16(out num2))
			{
				return false;
			}
			string text;
			if (!stateObj.TryReadString((int)num2, out text))
			{
				return false;
			}
			byte b3;
			if (!stateObj.TryReadByte(out b3))
			{
				return false;
			}
			string server;
			if (b3 == 0)
			{
				server = this._server;
			}
			else if (!stateObj.TryReadString((int)b3, out server))
			{
				return false;
			}
			if (!stateObj.TryReadByte(out b3))
			{
				return false;
			}
			string text2;
			if (!stateObj.TryReadString((int)b3, out text2))
			{
				return false;
			}
			int num3;
			if (this._isYukon)
			{
				if (!stateObj.TryReadInt32(out num3))
				{
					return false;
				}
			}
			else
			{
				ushort num4;
				if (!stateObj.TryReadUInt16(out num4))
				{
					return false;
				}
				num3 = (int)num4;
				if (this._state == TdsParserState.OpenNotLoggedIn)
				{
					byte b4;
					if (!stateObj.TryPeekByte(out b4))
					{
						return false;
					}
					if (b4 == 0)
					{
						ushort num5;
						if (!stateObj.TryReadUInt16(out num5))
						{
							return false;
						}
						num3 = (num3 << 16) + (int)num5;
					}
				}
			}
			error = new SqlError(num, b, b2, this._server, text, text2, num3, null);
			return true;
		}

		internal bool TryProcessReturnValue(int length, TdsParserStateObject stateObj, out SqlReturnValue returnValue)
		{
			returnValue = null;
			SqlReturnValue sqlReturnValue = new SqlReturnValue();
			sqlReturnValue.length = length;
			ushort num;
			if (!stateObj.TryReadUInt16(out num))
			{
				return false;
			}
			byte b;
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			if (b > 0 && !stateObj.TryReadString((int)b, out sqlReturnValue.parameter))
			{
				return false;
			}
			byte b2;
			if (!stateObj.TryReadByte(out b2))
			{
				return false;
			}
			uint num2;
			if (!stateObj.TryReadUInt32(out num2))
			{
				return false;
			}
			ushort num3;
			if (!stateObj.TryReadUInt16(out num3))
			{
				return false;
			}
			byte b3;
			if (!stateObj.TryReadByte(out b3))
			{
				return false;
			}
			int num4;
			if (b3 == 241)
			{
				num4 = 65535;
			}
			else if (this.IsVarTimeTds(b3))
			{
				num4 = 0;
			}
			else if (b3 == 40)
			{
				num4 = 3;
			}
			else if (!this.TryGetTokenLength(b3, stateObj, out num4))
			{
				return false;
			}
			sqlReturnValue.metaType = MetaType.GetSqlDataType((int)b3, num2, num4);
			sqlReturnValue.type = sqlReturnValue.metaType.SqlDbType;
			sqlReturnValue.tdsType = sqlReturnValue.metaType.NullableType;
			sqlReturnValue.isNullable = true;
			if (num4 == 65535)
			{
				sqlReturnValue.metaType = MetaType.GetMaxMetaTypeFromMetaType(sqlReturnValue.metaType);
			}
			if (sqlReturnValue.type == SqlDbType.Decimal)
			{
				if (!stateObj.TryReadByte(out sqlReturnValue.precision))
				{
					return false;
				}
				if (!stateObj.TryReadByte(out sqlReturnValue.scale))
				{
					return false;
				}
			}
			if (sqlReturnValue.metaType.IsVarTime && !stateObj.TryReadByte(out sqlReturnValue.scale))
			{
				return false;
			}
			if (b3 == 240 && !this.TryProcessUDTMetaData(sqlReturnValue, stateObj))
			{
				return false;
			}
			if (sqlReturnValue.type == SqlDbType.Xml)
			{
				byte b4;
				if (!stateObj.TryReadByte(out b4))
				{
					return false;
				}
				if ((b4 & 1) != 0)
				{
					if (!stateObj.TryReadByte(out b))
					{
						return false;
					}
					if (b != 0 && !stateObj.TryReadString((int)b, out sqlReturnValue.xmlSchemaCollectionDatabase))
					{
						return false;
					}
					if (!stateObj.TryReadByte(out b))
					{
						return false;
					}
					if (b != 0 && !stateObj.TryReadString((int)b, out sqlReturnValue.xmlSchemaCollectionOwningSchema))
					{
						return false;
					}
					short num5;
					if (!stateObj.TryReadInt16(out num5))
					{
						return false;
					}
					if (num5 != 0 && !stateObj.TryReadString((int)num5, out sqlReturnValue.xmlSchemaCollectionName))
					{
						return false;
					}
				}
			}
			else if (sqlReturnValue.metaType.IsCharType)
			{
				if (!this.TryProcessCollation(stateObj, out sqlReturnValue.collation))
				{
					return false;
				}
				int codePage = this.GetCodePage(sqlReturnValue.collation, stateObj);
				if (codePage == this._defaultCodePage)
				{
					sqlReturnValue.codePage = this._defaultCodePage;
					sqlReturnValue.encoding = this._defaultEncoding;
				}
				else
				{
					sqlReturnValue.codePage = codePage;
					sqlReturnValue.encoding = Encoding.GetEncoding(sqlReturnValue.codePage);
				}
			}
			bool flag = false;
			ulong num6;
			if (!this.TryProcessColumnHeaderNoNBC(sqlReturnValue, stateObj, out flag, out num6))
			{
				return false;
			}
			int num7 = ((num6 > 2147483647UL) ? int.MaxValue : ((int)num6));
			if (sqlReturnValue.metaType.IsPlp)
			{
				num7 = int.MaxValue;
			}
			if (flag)
			{
				this.GetNullSqlValue(sqlReturnValue.value, sqlReturnValue);
			}
			else if (!this.TryReadSqlValue(sqlReturnValue.value, sqlReturnValue, num7, stateObj))
			{
				return false;
			}
			returnValue = sqlReturnValue;
			return true;
		}

		internal bool TryProcessCollation(TdsParserStateObject stateObj, out SqlCollation collation)
		{
			SqlCollation sqlCollation = new SqlCollation();
			if (!stateObj.TryReadUInt32(out sqlCollation.info))
			{
				collation = null;
				return false;
			}
			if (!stateObj.TryReadByte(out sqlCollation.sortId))
			{
				collation = null;
				return false;
			}
			collation = sqlCollation;
			return true;
		}

		private void WriteCollation(SqlCollation collation, TdsParserStateObject stateObj)
		{
			if (collation == null)
			{
				this._physicalStateObj.WriteByte(0);
				return;
			}
			this._physicalStateObj.WriteByte(5);
			this.WriteUnsignedInt(collation.info, this._physicalStateObj);
			this._physicalStateObj.WriteByte(collation.sortId);
		}

		internal int GetCodePage(SqlCollation collation, TdsParserStateObject stateObj)
		{
			int num = 0;
			if (collation.sortId != 0)
			{
				num = (int)TdsEnums.CODE_PAGE_FROM_SORT_ID[(int)collation.sortId];
			}
			else
			{
				int num2 = collation.LCID;
				bool flag = false;
				try
				{
					num = CultureInfo.GetCultureInfo(num2).TextInfo.ANSICodePage;
					flag = true;
				}
				catch (ArgumentException)
				{
				}
				if (!flag || num == 0)
				{
					if (num2 <= 66578)
					{
						if (num2 == 2087)
						{
							goto IL_00B4;
						}
						if (num2 != 66564 && num2 - 66577 > 1)
						{
							goto IL_00D1;
						}
					}
					else if (num2 <= 68612)
					{
						if (num2 != 67588 && num2 != 68612)
						{
							goto IL_00D1;
						}
					}
					else if (num2 != 69636 && num2 != 70660)
					{
						goto IL_00D1;
					}
					num2 &= 16383;
					try
					{
						num = new CultureInfo(num2).TextInfo.ANSICodePage;
						flag = true;
						goto IL_00D1;
					}
					catch (ArgumentException)
					{
						goto IL_00D1;
					}
					IL_00B4:
					try
					{
						num = new CultureInfo(1063).TextInfo.ANSICodePage;
						flag = true;
					}
					catch (ArgumentException)
					{
					}
					IL_00D1:
					if (!flag)
					{
						this.ThrowUnsupportedCollationEncountered(stateObj);
					}
				}
			}
			return num;
		}

		internal void DrainData(TdsParserStateObject stateObj)
		{
			try
			{
				SqlDataReader.SharedState readerState = stateObj._readerState;
				if (readerState != null && readerState._dataReady)
				{
					_SqlMetaDataSet cleanupMetaData = stateObj._cleanupMetaData;
					if (stateObj._partialHeaderBytesRead > 0 && !stateObj.TryProcessHeader())
					{
						throw SQL.SynchronousCallMayNotPend();
					}
					if (readerState._nextColumnHeaderToRead == 0)
					{
						if (!stateObj.Parser.TrySkipRow(stateObj._cleanupMetaData, stateObj))
						{
							throw SQL.SynchronousCallMayNotPend();
						}
					}
					else
					{
						if (readerState._nextColumnDataToRead < readerState._nextColumnHeaderToRead)
						{
							if (readerState._nextColumnHeaderToRead > 0 && cleanupMetaData[readerState._nextColumnHeaderToRead - 1].metaType.IsPlp)
							{
								ulong num;
								if (stateObj._longlen != 0UL && !this.TrySkipPlpValue(18446744073709551615UL, stateObj, out num))
								{
									throw SQL.SynchronousCallMayNotPend();
								}
							}
							else if (0L < readerState._columnDataBytesRemaining && !stateObj.TrySkipLongBytes(readerState._columnDataBytesRemaining))
							{
								throw SQL.SynchronousCallMayNotPend();
							}
						}
						if (!stateObj.Parser.TrySkipRow(cleanupMetaData, readerState._nextColumnHeaderToRead, stateObj))
						{
							throw SQL.SynchronousCallMayNotPend();
						}
					}
				}
				this.Run(RunBehavior.Clean, null, null, null, stateObj);
			}
			catch
			{
				this._connHandler.DoomThisConnection();
				throw;
			}
		}

		internal void ThrowUnsupportedCollationEncountered(TdsParserStateObject stateObj)
		{
			stateObj.AddError(new SqlError(0, 0, 11, this._server, SQLMessage.CultureIdError(), "", 0, null));
			if (stateObj != null)
			{
				this.DrainData(stateObj);
				stateObj._pendingData = false;
			}
			this.ThrowExceptionAndWarning(stateObj, false, false);
		}

		internal bool TryProcessAltMetaData(int cColumns, TdsParserStateObject stateObj, out _SqlMetaDataSet metaData)
		{
			metaData = null;
			_SqlMetaDataSet sqlMetaDataSet = new _SqlMetaDataSet(cColumns);
			int[] array = new int[cColumns];
			if (!stateObj.TryReadUInt16(out sqlMetaDataSet.id))
			{
				return false;
			}
			byte b;
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			while (b > 0)
			{
				if (!stateObj.TrySkipBytes(2))
				{
					return false;
				}
				b -= 1;
			}
			for (int i = 0; i < cColumns; i++)
			{
				_SqlMetaData sqlMetaData = sqlMetaDataSet[i];
				byte b2;
				if (!stateObj.TryReadByte(out b2))
				{
					return false;
				}
				ushort num;
				if (!stateObj.TryReadUInt16(out num))
				{
					return false;
				}
				if (!this.TryCommonProcessMetaData(stateObj, sqlMetaData))
				{
					return false;
				}
				array[i] = i;
			}
			sqlMetaDataSet.indexMap = array;
			sqlMetaDataSet.visibleColumns = cColumns;
			metaData = sqlMetaDataSet;
			return true;
		}

		internal bool TryProcessMetaData(int cColumns, TdsParserStateObject stateObj, out _SqlMetaDataSet metaData)
		{
			_SqlMetaDataSet sqlMetaDataSet = new _SqlMetaDataSet(cColumns);
			for (int i = 0; i < cColumns; i++)
			{
				if (!this.TryCommonProcessMetaData(stateObj, sqlMetaDataSet[i]))
				{
					metaData = null;
					return false;
				}
			}
			metaData = sqlMetaDataSet;
			return true;
		}

		private bool IsVarTimeTds(byte tdsType)
		{
			return tdsType == 41 || tdsType == 42 || tdsType == 43;
		}

		private bool TryCommonProcessMetaData(TdsParserStateObject stateObj, _SqlMetaData col)
		{
			uint num;
			if (!stateObj.TryReadUInt32(out num))
			{
				return false;
			}
			byte b;
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			col.updatability = (byte)((b & 11) >> 2);
			col.isNullable = 1 == (b & 1);
			col.isIdentity = 16 == (b & 16);
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			col.isColumnSet = 4 == (b & 4);
			byte b2;
			if (!stateObj.TryReadByte(out b2))
			{
				return false;
			}
			if (b2 == 241)
			{
				col.length = 65535;
			}
			else if (this.IsVarTimeTds(b2))
			{
				col.length = 0;
			}
			else if (b2 == 40)
			{
				col.length = 3;
			}
			else if (!this.TryGetTokenLength(b2, stateObj, out col.length))
			{
				return false;
			}
			col.metaType = MetaType.GetSqlDataType((int)b2, num, col.length);
			col.type = col.metaType.SqlDbType;
			col.tdsType = (col.isNullable ? col.metaType.NullableType : col.metaType.TDSType);
			if (240 == b2 && !this.TryProcessUDTMetaData(col, stateObj))
			{
				return false;
			}
			byte b4;
			if (col.length == 65535)
			{
				col.metaType = MetaType.GetMaxMetaTypeFromMetaType(col.metaType);
				col.length = int.MaxValue;
				if (b2 == 241)
				{
					byte b3;
					if (!stateObj.TryReadByte(out b3))
					{
						return false;
					}
					if ((b3 & 1) != 0)
					{
						if (!stateObj.TryReadByte(out b4))
						{
							return false;
						}
						if (b4 != 0 && !stateObj.TryReadString((int)b4, out col.xmlSchemaCollectionDatabase))
						{
							return false;
						}
						if (!stateObj.TryReadByte(out b4))
						{
							return false;
						}
						if (b4 != 0 && !stateObj.TryReadString((int)b4, out col.xmlSchemaCollectionOwningSchema))
						{
							return false;
						}
						short num2;
						if (!stateObj.TryReadInt16(out num2))
						{
							return false;
						}
						if (b4 != 0 && !stateObj.TryReadString((int)num2, out col.xmlSchemaCollectionName))
						{
							return false;
						}
					}
				}
			}
			if (col.type == SqlDbType.Decimal)
			{
				if (!stateObj.TryReadByte(out col.precision))
				{
					return false;
				}
				if (!stateObj.TryReadByte(out col.scale))
				{
					return false;
				}
			}
			if (col.metaType.IsVarTime)
			{
				if (!stateObj.TryReadByte(out col.scale))
				{
					return false;
				}
				switch (col.metaType.SqlDbType)
				{
				case SqlDbType.Time:
					col.length = MetaType.GetTimeSizeFromScale(col.scale);
					break;
				case SqlDbType.DateTime2:
					col.length = 3 + MetaType.GetTimeSizeFromScale(col.scale);
					break;
				case SqlDbType.DateTimeOffset:
					col.length = 5 + MetaType.GetTimeSizeFromScale(col.scale);
					break;
				}
			}
			if (col.metaType.IsCharType && b2 != 241)
			{
				if (!this.TryProcessCollation(stateObj, out col.collation))
				{
					return false;
				}
				int codePage = this.GetCodePage(col.collation, stateObj);
				if (codePage == this._defaultCodePage)
				{
					col.codePage = this._defaultCodePage;
					col.encoding = this._defaultEncoding;
				}
				else
				{
					col.codePage = codePage;
					col.encoding = Encoding.GetEncoding(col.codePage);
				}
			}
			if (col.metaType.IsLong && !col.metaType.IsPlp)
			{
				int num3 = 65535;
				if (!this.TryProcessOneTable(stateObj, ref num3, out col.multiPartTableName))
				{
					return false;
				}
			}
			if (!stateObj.TryReadByte(out b4))
			{
				return false;
			}
			if (!stateObj.TryReadString((int)b4, out col.column))
			{
				return false;
			}
			stateObj._receivedColMetaData = true;
			return true;
		}

		internal bool TryProcessTableName(int length, TdsParserStateObject stateObj, out MultiPartTableName[] multiPartTableNames)
		{
			int num = 0;
			MultiPartTableName[] array = new MultiPartTableName[1];
			while (length > 0)
			{
				MultiPartTableName multiPartTableName;
				if (!this.TryProcessOneTable(stateObj, ref length, out multiPartTableName))
				{
					multiPartTableNames = null;
					return false;
				}
				if (num == 0)
				{
					array[num] = multiPartTableName;
				}
				else
				{
					MultiPartTableName[] array2 = new MultiPartTableName[array.Length + 1];
					Array.Copy(array, 0, array2, 0, array.Length);
					array2[array.Length] = multiPartTableName;
					array = array2;
				}
				num++;
			}
			multiPartTableNames = array;
			return true;
		}

		private bool TryProcessOneTable(TdsParserStateObject stateObj, ref int length, out MultiPartTableName multiPartTableName)
		{
			multiPartTableName = default(MultiPartTableName);
			MultiPartTableName multiPartTableName2 = default(MultiPartTableName);
			byte b;
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			length--;
			if (b == 4)
			{
				ushort num;
				if (!stateObj.TryReadUInt16(out num))
				{
					return false;
				}
				length -= 2;
				string text;
				if (!stateObj.TryReadString((int)num, out text))
				{
					return false;
				}
				multiPartTableName2.ServerName = text;
				b -= 1;
				length -= (int)(num * 2);
			}
			if (b == 3)
			{
				ushort num;
				if (!stateObj.TryReadUInt16(out num))
				{
					return false;
				}
				length -= 2;
				string text;
				if (!stateObj.TryReadString((int)num, out text))
				{
					return false;
				}
				multiPartTableName2.CatalogName = text;
				length -= (int)(num * 2);
				b -= 1;
			}
			if (b == 2)
			{
				ushort num;
				if (!stateObj.TryReadUInt16(out num))
				{
					return false;
				}
				length -= 2;
				string text;
				if (!stateObj.TryReadString((int)num, out text))
				{
					return false;
				}
				multiPartTableName2.SchemaName = text;
				length -= (int)(num * 2);
				b -= 1;
			}
			if (b == 1)
			{
				ushort num;
				if (!stateObj.TryReadUInt16(out num))
				{
					return false;
				}
				length -= 2;
				string text;
				if (!stateObj.TryReadString((int)num, out text))
				{
					return false;
				}
				multiPartTableName2.TableName = text;
				length -= (int)(num * 2);
				b -= 1;
			}
			multiPartTableName = multiPartTableName2;
			return true;
		}

		private bool TryProcessColInfo(_SqlMetaDataSet columns, SqlDataReader reader, TdsParserStateObject stateObj, out _SqlMetaDataSet metaData)
		{
			metaData = null;
			for (int i = 0; i < columns.Length; i++)
			{
				_SqlMetaData sqlMetaData = columns[i];
				byte b;
				if (!stateObj.TryReadByte(out b))
				{
					return false;
				}
				if (!stateObj.TryReadByte(out sqlMetaData.tableNum))
				{
					return false;
				}
				byte b2;
				if (!stateObj.TryReadByte(out b2))
				{
					return false;
				}
				sqlMetaData.isDifferentName = 32 == (b2 & 32);
				sqlMetaData.isExpression = 4 == (b2 & 4);
				sqlMetaData.isKey = 8 == (b2 & 8);
				sqlMetaData.isHidden = 16 == (b2 & 16);
				if (sqlMetaData.isDifferentName)
				{
					byte b3;
					if (!stateObj.TryReadByte(out b3))
					{
						return false;
					}
					if (!stateObj.TryReadString((int)b3, out sqlMetaData.baseColumn))
					{
						return false;
					}
				}
				if (reader.TableNames != null && sqlMetaData.tableNum > 0)
				{
					sqlMetaData.multiPartTableName = reader.TableNames[(int)(sqlMetaData.tableNum - 1)];
				}
				if (sqlMetaData.isExpression)
				{
					sqlMetaData.updatability = 0;
				}
			}
			metaData = columns;
			return true;
		}

		internal bool TryProcessColumnHeader(SqlMetaDataPriv col, TdsParserStateObject stateObj, int columnOrdinal, out bool isNull, out ulong length)
		{
			if (stateObj.IsNullCompressionBitSet(columnOrdinal))
			{
				isNull = true;
				length = 0UL;
				return true;
			}
			return this.TryProcessColumnHeaderNoNBC(col, stateObj, out isNull, out length);
		}

		private bool TryProcessColumnHeaderNoNBC(SqlMetaDataPriv col, TdsParserStateObject stateObj, out bool isNull, out ulong length)
		{
			if (col.metaType.IsLong && !col.metaType.IsPlp)
			{
				byte b;
				if (!stateObj.TryReadByte(out b))
				{
					isNull = false;
					length = 0UL;
					return false;
				}
				if (b == 0)
				{
					isNull = true;
					length = 0UL;
					return true;
				}
				if (!stateObj.TrySkipBytes((int)b))
				{
					isNull = false;
					length = 0UL;
					return false;
				}
				if (!stateObj.TrySkipBytes(8))
				{
					isNull = false;
					length = 0UL;
					return false;
				}
				isNull = false;
				return this.TryGetDataLength(col, stateObj, out length);
			}
			else
			{
				ulong num;
				if (!this.TryGetDataLength(col, stateObj, out num))
				{
					isNull = false;
					length = 0UL;
					return false;
				}
				isNull = this.IsNull(col.metaType, num);
				length = (isNull ? 0UL : num);
				return true;
			}
		}

		internal bool TryGetAltRowId(TdsParserStateObject stateObj, out int id)
		{
			byte b;
			if (!stateObj.TryReadByte(out b))
			{
				id = 0;
				return false;
			}
			if (!stateObj.TryStartNewRow(false, 0))
			{
				id = 0;
				return false;
			}
			ushort num;
			if (!stateObj.TryReadUInt16(out num))
			{
				id = 0;
				return false;
			}
			id = (int)num;
			return true;
		}

		private bool TryProcessRow(_SqlMetaDataSet columns, object[] buffer, int[] map, TdsParserStateObject stateObj)
		{
			SqlBuffer sqlBuffer = new SqlBuffer();
			for (int i = 0; i < columns.Length; i++)
			{
				_SqlMetaData sqlMetaData = columns[i];
				bool flag;
				ulong num;
				if (!this.TryProcessColumnHeader(sqlMetaData, stateObj, i, out flag, out num))
				{
					return false;
				}
				if (flag)
				{
					this.GetNullSqlValue(sqlBuffer, sqlMetaData);
					buffer[map[i]] = sqlBuffer.SqlValue;
				}
				else
				{
					if (!this.TryReadSqlValue(sqlBuffer, sqlMetaData, sqlMetaData.metaType.IsPlp ? 2147483647 : ((int)num), stateObj))
					{
						return false;
					}
					buffer[map[i]] = sqlBuffer.SqlValue;
					if (stateObj._longlen != 0UL)
					{
						throw new SqlTruncateException(SR.GetString("Data returned is larger than 2Gb in size. Use SequentialAccess command behavior in order to get all of the data."));
					}
				}
				sqlBuffer.Clear();
			}
			return true;
		}

		internal object GetNullSqlValue(SqlBuffer nullVal, SqlMetaDataPriv md)
		{
			switch (md.type)
			{
			case SqlDbType.BigInt:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.Int64);
				break;
			case SqlDbType.Binary:
			case SqlDbType.Image:
			case SqlDbType.VarBinary:
			case SqlDbType.Udt:
				nullVal.SqlBinary = SqlBinary.Null;
				break;
			case SqlDbType.Bit:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.Boolean);
				break;
			case SqlDbType.Char:
			case SqlDbType.NChar:
			case SqlDbType.NText:
			case SqlDbType.NVarChar:
			case SqlDbType.Text:
			case SqlDbType.VarChar:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.String);
				break;
			case SqlDbType.DateTime:
			case SqlDbType.SmallDateTime:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.DateTime);
				break;
			case SqlDbType.Decimal:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.Decimal);
				break;
			case SqlDbType.Float:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.Double);
				break;
			case SqlDbType.Int:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.Int32);
				break;
			case SqlDbType.Money:
			case SqlDbType.SmallMoney:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.Money);
				break;
			case SqlDbType.Real:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.Single);
				break;
			case SqlDbType.UniqueIdentifier:
				nullVal.SqlGuid = SqlGuid.Null;
				break;
			case SqlDbType.SmallInt:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.Int16);
				break;
			case SqlDbType.TinyInt:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.Byte);
				break;
			case SqlDbType.Variant:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.Empty);
				break;
			case SqlDbType.Xml:
				nullVal.SqlCachedBuffer = SqlCachedBuffer.Null;
				break;
			case SqlDbType.Date:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.Date);
				break;
			case SqlDbType.Time:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.Time);
				break;
			case SqlDbType.DateTime2:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.DateTime2);
				break;
			case SqlDbType.DateTimeOffset:
				nullVal.SetToNullOfType(SqlBuffer.StorageType.DateTimeOffset);
				break;
			}
			return nullVal;
		}

		internal bool TrySkipRow(_SqlMetaDataSet columns, TdsParserStateObject stateObj)
		{
			return this.TrySkipRow(columns, 0, stateObj);
		}

		internal bool TrySkipRow(_SqlMetaDataSet columns, int startCol, TdsParserStateObject stateObj)
		{
			for (int i = startCol; i < columns.Length; i++)
			{
				_SqlMetaData sqlMetaData = columns[i];
				if (!this.TrySkipValue(sqlMetaData, i, stateObj))
				{
					return false;
				}
			}
			return true;
		}

		internal bool TrySkipValue(SqlMetaDataPriv md, int columnOrdinal, TdsParserStateObject stateObj)
		{
			if (stateObj.IsNullCompressionBitSet(columnOrdinal))
			{
				return true;
			}
			if (md.metaType.IsPlp)
			{
				ulong num;
				if (!this.TrySkipPlpValue(18446744073709551615UL, stateObj, out num))
				{
					return false;
				}
			}
			else if (md.metaType.IsLong)
			{
				byte b;
				if (!stateObj.TryReadByte(out b))
				{
					return false;
				}
				if (b != 0)
				{
					if (!stateObj.TrySkipBytes((int)(b + 8)))
					{
						return false;
					}
					int num2;
					if (!this.TryGetTokenLength(md.tdsType, stateObj, out num2))
					{
						return false;
					}
					if (!stateObj.TrySkipBytes(num2))
					{
						return false;
					}
				}
			}
			else
			{
				int num3;
				if (!this.TryGetTokenLength(md.tdsType, stateObj, out num3))
				{
					return false;
				}
				if (!this.IsNull(md.metaType, (ulong)((long)num3)) && !stateObj.TrySkipBytes(num3))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsNull(MetaType mt, ulong length)
		{
			if (mt.IsPlp)
			{
				return ulong.MaxValue == length;
			}
			return (65535UL == length && !mt.IsLong) || (length == 0UL && !mt.IsCharType && !mt.IsBinType);
		}

		private bool TryReadSqlStringValue(SqlBuffer value, byte type, int length, Encoding encoding, bool isPlp, TdsParserStateObject stateObj)
		{
			if (type <= 99)
			{
				if (type <= 39)
				{
					if (type != 35 && type != 39)
					{
						return true;
					}
				}
				else if (type != 47)
				{
					if (type != 99)
					{
						return true;
					}
					goto IL_007E;
				}
			}
			else if (type <= 175)
			{
				if (type != 167 && type != 175)
				{
					return true;
				}
			}
			else
			{
				if (type != 231 && type != 239)
				{
					return true;
				}
				goto IL_007E;
			}
			if (encoding == null)
			{
				encoding = this._defaultEncoding;
			}
			string text;
			if (!stateObj.TryReadStringWithEncoding(length, encoding, isPlp, out text))
			{
				return false;
			}
			value.SetToString(text);
			return true;
			IL_007E:
			string text2 = null;
			if (isPlp)
			{
				char[] array = null;
				if (!this.TryReadPlpUnicodeChars(ref array, 0, length >> 1, stateObj, out length))
				{
					return false;
				}
				if (length > 0)
				{
					text2 = new string(array, 0, length);
				}
				else
				{
					text2 = ADP.StrEmpty;
				}
			}
			else if (!stateObj.TryReadString(length >> 1, out text2))
			{
				return false;
			}
			value.SetToString(text2);
			return true;
		}

		internal bool TryReadSqlValue(SqlBuffer value, SqlMetaDataPriv md, int length, TdsParserStateObject stateObj)
		{
			bool isPlp = md.metaType.IsPlp;
			byte tdsType = md.tdsType;
			if (isPlp)
			{
				length = int.MaxValue;
			}
			if (tdsType <= 165)
			{
				if (tdsType <= 99)
				{
					switch (tdsType)
					{
					case 34:
					case 37:
					case 45:
						break;
					case 35:
					case 39:
					case 47:
						goto IL_0133;
					case 36:
					case 38:
					case 44:
					case 46:
						goto IL_0176;
					case 40:
					case 41:
					case 42:
					case 43:
						if (!this.TryReadSqlDateTime(value, tdsType, length, md.scale, stateObj))
						{
							return false;
						}
						return true;
					default:
						if (tdsType != 99)
						{
							goto IL_0176;
						}
						goto IL_0133;
					}
				}
				else if (tdsType != 106 && tdsType != 108)
				{
					if (tdsType != 165)
					{
						goto IL_0176;
					}
				}
				else
				{
					if (!this.TryReadSqlDecimal(value, length, md.precision, md.scale, stateObj))
					{
						return false;
					}
					return true;
				}
			}
			else if (tdsType <= 173)
			{
				if (tdsType == 167)
				{
					goto IL_0133;
				}
				if (tdsType != 173)
				{
					goto IL_0176;
				}
			}
			else
			{
				if (tdsType == 175 || tdsType == 231)
				{
					goto IL_0133;
				}
				switch (tdsType)
				{
				case 239:
					goto IL_0133;
				case 240:
					break;
				case 241:
				{
					SqlCachedBuffer sqlCachedBuffer;
					if (!SqlCachedBuffer.TryCreate(md, this, stateObj, out sqlCachedBuffer))
					{
						return false;
					}
					value.SqlCachedBuffer = sqlCachedBuffer;
					return true;
				}
				default:
					goto IL_0176;
				}
			}
			byte[] array = null;
			if (isPlp)
			{
				int num;
				if (!stateObj.TryReadPlpBytes(ref array, 0, length, out num))
				{
					return false;
				}
			}
			else
			{
				array = new byte[length];
				if (!stateObj.TryReadByteArray(array, 0, length))
				{
					return false;
				}
			}
			value.SqlBinary = SqlTypeWorkarounds.SqlBinaryCtor(array, true);
			return true;
			IL_0133:
			if (!this.TryReadSqlStringValue(value, tdsType, length, md.encoding, isPlp, stateObj))
			{
				return false;
			}
			return true;
			IL_0176:
			if (!this.TryReadSqlValueInternal(value, tdsType, length, stateObj))
			{
				return false;
			}
			return true;
		}

		private bool TryReadSqlDateTime(SqlBuffer value, byte tdsType, int length, byte scale, TdsParserStateObject stateObj)
		{
			byte[] array = new byte[length];
			if (!stateObj.TryReadByteArray(array, 0, length))
			{
				return false;
			}
			switch (tdsType)
			{
			case 40:
				value.SetToDate(array);
				break;
			case 41:
				value.SetToTime(array, length, scale);
				break;
			case 42:
				value.SetToDateTime2(array, length, scale);
				break;
			case 43:
				value.SetToDateTimeOffset(array, length, scale);
				break;
			}
			return true;
		}

		internal bool TryReadSqlValueInternal(SqlBuffer value, byte tdsType, int length, TdsParserStateObject stateObj)
		{
			if (tdsType <= 104)
			{
				byte b;
				if (tdsType <= 62)
				{
					switch (tdsType)
					{
					case 34:
					case 37:
						goto IL_0273;
					case 35:
						return true;
					case 36:
					{
						byte[] array = new byte[length];
						if (!stateObj.TryReadByteArray(array, 0, length))
						{
							return false;
						}
						value.SqlGuid = SqlTypeWorkarounds.SqlGuidCtor(array, true);
						return true;
					}
					case 38:
						if (length != 1)
						{
							if (length == 2)
							{
								goto IL_011F;
							}
							if (length == 4)
							{
								goto IL_0138;
							}
							goto IL_0151;
						}
						break;
					default:
						switch (tdsType)
						{
						case 45:
							goto IL_0273;
						case 46:
						case 47:
						case 49:
						case 51:
						case 53:
						case 54:
						case 55:
						case 57:
							return true;
						case 48:
							break;
						case 50:
							goto IL_00DC;
						case 52:
							goto IL_011F;
						case 56:
							goto IL_0138;
						case 58:
							goto IL_01F7;
						case 59:
							goto IL_016E;
						case 60:
							goto IL_01A6;
						case 61:
							goto IL_0226;
						case 62:
							goto IL_0188;
						default:
							return true;
						}
						break;
					}
					if (!stateObj.TryReadByte(out b))
					{
						return false;
					}
					value.Byte = b;
					return true;
					IL_011F:
					short num;
					if (!stateObj.TryReadInt16(out num))
					{
						return false;
					}
					value.Int16 = num;
					return true;
					IL_0138:
					int num2;
					if (!stateObj.TryReadInt32(out num2))
					{
						return false;
					}
					value.Int32 = num2;
					return true;
				}
				else if (tdsType != 98)
				{
					if (tdsType != 104)
					{
						return true;
					}
				}
				else
				{
					if (!this.TryReadSqlVariant(value, length, stateObj))
					{
						return false;
					}
					return true;
				}
				IL_00DC:
				if (!stateObj.TryReadByte(out b))
				{
					return false;
				}
				value.Boolean = b > 0;
				return true;
			}
			else if (tdsType <= 122)
			{
				switch (tdsType)
				{
				case 109:
					if (length == 4)
					{
						goto IL_016E;
					}
					goto IL_0188;
				case 110:
					if (length != 4)
					{
						goto IL_01A6;
					}
					break;
				case 111:
					if (length == 4)
					{
						goto IL_01F7;
					}
					goto IL_0226;
				default:
					if (tdsType != 122)
					{
						return true;
					}
					break;
				}
				int num2;
				if (!stateObj.TryReadInt32(out num2))
				{
					return false;
				}
				value.SetToMoney((long)num2);
				return true;
			}
			else if (tdsType != 127)
			{
				if (tdsType != 165 && tdsType != 173)
				{
					return true;
				}
				goto IL_0273;
			}
			IL_0151:
			long num3;
			if (!stateObj.TryReadInt64(out num3))
			{
				return false;
			}
			value.Int64 = num3;
			return true;
			IL_016E:
			float num4;
			if (!stateObj.TryReadSingle(out num4))
			{
				return false;
			}
			value.Single = num4;
			return true;
			IL_0188:
			double num5;
			if (!stateObj.TryReadDouble(out num5))
			{
				return false;
			}
			value.Double = num5;
			return true;
			IL_01A6:
			int num6;
			if (!stateObj.TryReadInt32(out num6))
			{
				return false;
			}
			uint num7;
			if (!stateObj.TryReadUInt32(out num7))
			{
				return false;
			}
			long num8 = ((long)num6 << 32) + (long)((ulong)num7);
			value.SetToMoney(num8);
			return true;
			IL_01F7:
			ushort num9;
			if (!stateObj.TryReadUInt16(out num9))
			{
				return false;
			}
			ushort num10;
			if (!stateObj.TryReadUInt16(out num10))
			{
				return false;
			}
			value.SetToDateTime((int)num9, (int)num10 * SqlDateTime.SQLTicksPerMinute);
			return true;
			IL_0226:
			int num11;
			if (!stateObj.TryReadInt32(out num11))
			{
				return false;
			}
			uint num12;
			if (!stateObj.TryReadUInt32(out num12))
			{
				return false;
			}
			value.SetToDateTime(num11, (int)num12);
			return true;
			IL_0273:
			byte[] array2 = new byte[length];
			if (!stateObj.TryReadByteArray(array2, 0, length))
			{
				return false;
			}
			value.SqlBinary = SqlTypeWorkarounds.SqlBinaryCtor(array2, true);
			return true;
		}

		internal bool TryReadSqlVariant(SqlBuffer value, int lenTotal, TdsParserStateObject stateObj)
		{
			byte b;
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			ushort num = 0;
			byte b2;
			if (!stateObj.TryReadByte(out b2))
			{
				return false;
			}
			byte propBytes = MetaType.GetSqlDataType((int)b, 0U, 0).PropBytes;
			int num2 = (int)(2 + b2);
			int num3 = lenTotal - num2;
			if (b <= 127)
			{
				if (b <= 106)
				{
					switch (b)
					{
					case 36:
					case 48:
					case 50:
					case 52:
					case 56:
					case 58:
					case 59:
					case 60:
					case 61:
					case 62:
						goto IL_011E;
					case 37:
					case 38:
					case 39:
					case 44:
					case 45:
					case 46:
					case 47:
					case 49:
					case 51:
					case 53:
					case 54:
					case 55:
					case 57:
						return true;
					case 40:
						if (!this.TryReadSqlDateTime(value, b, num3, 0, stateObj))
						{
							return false;
						}
						return true;
					case 41:
					case 42:
					case 43:
					{
						byte b3;
						if (!stateObj.TryReadByte(out b3))
						{
							return false;
						}
						if (b2 > propBytes && !stateObj.TrySkipBytes((int)(b2 - propBytes)))
						{
							return false;
						}
						if (!this.TryReadSqlDateTime(value, b, num3, b3, stateObj))
						{
							return false;
						}
						return true;
					}
					default:
						if (b != 106)
						{
							return true;
						}
						break;
					}
				}
				else if (b != 108)
				{
					if (b != 122 && b != 127)
					{
						return true;
					}
					goto IL_011E;
				}
				byte b4;
				if (!stateObj.TryReadByte(out b4))
				{
					return false;
				}
				byte b5;
				if (!stateObj.TryReadByte(out b5))
				{
					return false;
				}
				if (b2 > propBytes && !stateObj.TrySkipBytes((int)(b2 - propBytes)))
				{
					return false;
				}
				if (!this.TryReadSqlDecimal(value, 17, b4, b5, stateObj))
				{
					return false;
				}
				return true;
			}
			else
			{
				if (b <= 173)
				{
					if (b != 165)
					{
						if (b == 167)
						{
							goto IL_018B;
						}
						if (b != 173)
						{
							return true;
						}
					}
					if (!stateObj.TryReadUInt16(out num))
					{
						return false;
					}
					if (b2 > propBytes && !stateObj.TrySkipBytes((int)(b2 - propBytes)))
					{
						return false;
					}
					goto IL_011E;
				}
				else if (b != 175 && b != 231 && b != 239)
				{
					return true;
				}
				IL_018B:
				SqlCollation sqlCollation;
				if (!this.TryProcessCollation(stateObj, out sqlCollation))
				{
					return false;
				}
				if (!stateObj.TryReadUInt16(out num))
				{
					return false;
				}
				if (b2 > propBytes && !stateObj.TrySkipBytes((int)(b2 - propBytes)))
				{
					return false;
				}
				Encoding encoding = Encoding.GetEncoding(this.GetCodePage(sqlCollation, stateObj));
				if (!this.TryReadSqlStringValue(value, b, num3, encoding, false, stateObj))
				{
					return false;
				}
				return true;
			}
			IL_011E:
			if (!this.TryReadSqlValueInternal(value, b, num3, stateObj))
			{
				return false;
			}
			return true;
		}

		internal Task WriteSqlVariantValue(object value, int length, int offset, TdsParserStateObject stateObj, bool canAccumulate = true)
		{
			if (ADP.IsNull(value))
			{
				this.WriteInt(0, stateObj);
				this.WriteInt(0, stateObj);
				return null;
			}
			MetaType metaType = MetaType.GetMetaTypeFromValue(value, true, true);
			if (108 == metaType.TDSType && 8 == length)
			{
				metaType = MetaType.GetMetaTypeFromValue(new SqlMoney((decimal)value), true, true);
			}
			if (metaType.IsAnsiType)
			{
				length = this.GetEncodingCharLength((string)value, length, 0, this._defaultEncoding);
			}
			this.WriteInt((int)(2 + metaType.PropBytes) + length, stateObj);
			this.WriteInt((int)(2 + metaType.PropBytes) + length, stateObj);
			stateObj.WriteByte(metaType.TDSType);
			stateObj.WriteByte(metaType.PropBytes);
			byte tdstype = metaType.TDSType;
			if (tdstype <= 62)
			{
				if (tdstype <= 41)
				{
					if (tdstype != 36)
					{
						if (tdstype == 41)
						{
							stateObj.WriteByte(metaType.Scale);
							this.WriteTime((TimeSpan)value, metaType.Scale, length, stateObj);
						}
					}
					else
					{
						byte[] array = ((Guid)value).ToByteArray();
						stateObj.WriteByteArray(array, length, 0, true, null);
					}
				}
				else if (tdstype != 43)
				{
					switch (tdstype)
					{
					case 48:
						stateObj.WriteByte((byte)value);
						break;
					case 50:
						if ((bool)value)
						{
							stateObj.WriteByte(1);
						}
						else
						{
							stateObj.WriteByte(0);
						}
						break;
					case 52:
						this.WriteShort((int)((short)value), stateObj);
						break;
					case 56:
						this.WriteInt((int)value, stateObj);
						break;
					case 59:
						this.WriteFloat((float)value, stateObj);
						break;
					case 60:
						this.WriteCurrency((decimal)value, 8, stateObj);
						break;
					case 61:
					{
						TdsDateTime tdsDateTime = MetaType.FromDateTime((DateTime)value, 8);
						this.WriteInt(tdsDateTime.days, stateObj);
						this.WriteInt(tdsDateTime.time, stateObj);
						break;
					}
					case 62:
						this.WriteDouble((double)value, stateObj);
						break;
					}
				}
				else
				{
					stateObj.WriteByte(metaType.Scale);
					this.WriteDateTimeOffset((DateTimeOffset)value, metaType.Scale, length, stateObj);
				}
			}
			else if (tdstype <= 127)
			{
				if (tdstype != 108)
				{
					if (tdstype == 127)
					{
						this.WriteLong((long)value, stateObj);
					}
				}
				else
				{
					stateObj.WriteByte(metaType.Precision);
					stateObj.WriteByte((byte)((decimal.GetBits((decimal)value)[3] & 16711680) >> 16));
					this.WriteDecimal((decimal)value, stateObj);
				}
			}
			else
			{
				if (tdstype == 165)
				{
					byte[] array2 = (byte[])value;
					this.WriteShort(length, stateObj);
					return stateObj.WriteByteArray(array2, length, offset, canAccumulate, null);
				}
				if (tdstype == 167)
				{
					string text = (string)value;
					this.WriteUnsignedInt(this._defaultCollation.info, stateObj);
					stateObj.WriteByte(this._defaultCollation.sortId);
					this.WriteShort(length, stateObj);
					return this.WriteEncodingChar(text, this._defaultEncoding, stateObj, canAccumulate);
				}
				if (tdstype == 231)
				{
					string text2 = (string)value;
					this.WriteUnsignedInt(this._defaultCollation.info, stateObj);
					stateObj.WriteByte(this._defaultCollation.sortId);
					this.WriteShort(length, stateObj);
					length >>= 1;
					return this.WriteString(text2, length, offset, stateObj, canAccumulate);
				}
			}
			return null;
		}

		internal Task WriteSqlVariantDataRowValue(object value, TdsParserStateObject stateObj, bool canAccumulate = true)
		{
			if (value == null || DBNull.Value == value)
			{
				this.WriteInt(0, stateObj);
				return null;
			}
			MetaType metaTypeFromValue = MetaType.GetMetaTypeFromValue(value, true, true);
			int num = 0;
			if (metaTypeFromValue.IsAnsiType)
			{
				num = this.GetEncodingCharLength((string)value, num, 0, this._defaultEncoding);
			}
			byte tdstype = metaTypeFromValue.TDSType;
			if (tdstype <= 62)
			{
				if (tdstype <= 41)
				{
					if (tdstype != 36)
					{
						if (tdstype == 41)
						{
							this.WriteSqlVariantHeader(8, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
							stateObj.WriteByte(metaTypeFromValue.Scale);
							this.WriteTime((TimeSpan)value, metaTypeFromValue.Scale, 5, stateObj);
						}
					}
					else
					{
						byte[] array = ((Guid)value).ToByteArray();
						num = array.Length;
						this.WriteSqlVariantHeader(18, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
						stateObj.WriteByteArray(array, num, 0, true, null);
					}
				}
				else if (tdstype != 43)
				{
					switch (tdstype)
					{
					case 48:
						this.WriteSqlVariantHeader(3, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
						stateObj.WriteByte((byte)value);
						break;
					case 50:
						this.WriteSqlVariantHeader(3, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
						if ((bool)value)
						{
							stateObj.WriteByte(1);
						}
						else
						{
							stateObj.WriteByte(0);
						}
						break;
					case 52:
						this.WriteSqlVariantHeader(4, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
						this.WriteShort((int)((short)value), stateObj);
						break;
					case 56:
						this.WriteSqlVariantHeader(6, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
						this.WriteInt((int)value, stateObj);
						break;
					case 59:
						this.WriteSqlVariantHeader(6, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
						this.WriteFloat((float)value, stateObj);
						break;
					case 60:
						this.WriteSqlVariantHeader(10, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
						this.WriteCurrency((decimal)value, 8, stateObj);
						break;
					case 61:
					{
						TdsDateTime tdsDateTime = MetaType.FromDateTime((DateTime)value, 8);
						this.WriteSqlVariantHeader(10, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
						this.WriteInt(tdsDateTime.days, stateObj);
						this.WriteInt(tdsDateTime.time, stateObj);
						break;
					}
					case 62:
						this.WriteSqlVariantHeader(10, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
						this.WriteDouble((double)value, stateObj);
						break;
					}
				}
				else
				{
					this.WriteSqlVariantHeader(13, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
					stateObj.WriteByte(metaTypeFromValue.Scale);
					this.WriteDateTimeOffset((DateTimeOffset)value, metaTypeFromValue.Scale, 10, stateObj);
				}
			}
			else if (tdstype <= 127)
			{
				if (tdstype != 108)
				{
					if (tdstype == 127)
					{
						this.WriteSqlVariantHeader(10, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
						this.WriteLong((long)value, stateObj);
					}
				}
				else
				{
					this.WriteSqlVariantHeader(21, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
					stateObj.WriteByte(metaTypeFromValue.Precision);
					stateObj.WriteByte((byte)((decimal.GetBits((decimal)value)[3] & 16711680) >> 16));
					this.WriteDecimal((decimal)value, stateObj);
				}
			}
			else
			{
				if (tdstype == 165)
				{
					byte[] array2 = (byte[])value;
					num = array2.Length;
					this.WriteSqlVariantHeader(4 + num, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
					this.WriteShort(num, stateObj);
					return stateObj.WriteByteArray(array2, num, 0, canAccumulate, null);
				}
				if (tdstype == 167)
				{
					string text = (string)value;
					num = text.Length;
					this.WriteSqlVariantHeader(9 + num, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
					this.WriteUnsignedInt(this._defaultCollation.info, stateObj);
					stateObj.WriteByte(this._defaultCollation.sortId);
					this.WriteShort(num, stateObj);
					return this.WriteEncodingChar(text, this._defaultEncoding, stateObj, canAccumulate);
				}
				if (tdstype == 231)
				{
					string text2 = (string)value;
					num = text2.Length * 2;
					this.WriteSqlVariantHeader(9 + num, metaTypeFromValue.TDSType, metaTypeFromValue.PropBytes, stateObj);
					this.WriteUnsignedInt(this._defaultCollation.info, stateObj);
					stateObj.WriteByte(this._defaultCollation.sortId);
					this.WriteShort(num, stateObj);
					num >>= 1;
					return this.WriteString(text2, num, 0, stateObj, canAccumulate);
				}
			}
			return null;
		}

		internal void WriteSqlVariantHeader(int length, byte tdstype, byte propbytes, TdsParserStateObject stateObj)
		{
			this.WriteInt(length, stateObj);
			stateObj.WriteByte(tdstype);
			stateObj.WriteByte(propbytes);
		}

		internal void WriteSqlVariantDateTime2(DateTime value, TdsParserStateObject stateObj)
		{
			SmiMetaData defaultDateTime = SmiMetaData.DefaultDateTime2;
			this.WriteSqlVariantHeader((int)(defaultDateTime.MaxLength + 3L), 42, 1, stateObj);
			stateObj.WriteByte(defaultDateTime.Scale);
			this.WriteDateTime2(value, defaultDateTime.Scale, (int)defaultDateTime.MaxLength, stateObj);
		}

		internal void WriteSqlVariantDate(DateTime value, TdsParserStateObject stateObj)
		{
			SmiMetaData defaultDate = SmiMetaData.DefaultDate;
			this.WriteSqlVariantHeader((int)(defaultDate.MaxLength + 2L), 40, 0, stateObj);
			this.WriteDate(value, stateObj);
		}

		private void WriteSqlMoney(SqlMoney value, int length, TdsParserStateObject stateObj)
		{
			int[] bits = decimal.GetBits(value.Value);
			bool flag = (bits[3] & int.MinValue) != 0;
			long num = (long)(((ulong)bits[1] << 32) | (ulong)bits[0]);
			if (flag)
			{
				num = -num;
			}
			if (length != 4)
			{
				this.WriteInt((int)(num >> 32), stateObj);
				this.WriteInt((int)num, stateObj);
				return;
			}
			decimal value2 = value.Value;
			if (value2 < TdsEnums.SQL_SMALL_MONEY_MIN || value2 > TdsEnums.SQL_SMALL_MONEY_MAX)
			{
				throw SQL.MoneyOverflow(value2.ToString(CultureInfo.InvariantCulture));
			}
			this.WriteInt((int)num, stateObj);
		}

		private void WriteCurrency(decimal value, int length, TdsParserStateObject stateObj)
		{
			SqlMoney sqlMoney = new SqlMoney(value);
			int[] bits = decimal.GetBits(sqlMoney.Value);
			bool flag = (bits[3] & int.MinValue) != 0;
			long num = (long)(((ulong)bits[1] << 32) | (ulong)bits[0]);
			if (flag)
			{
				num = -num;
			}
			if (length != 4)
			{
				this.WriteInt((int)(num >> 32), stateObj);
				this.WriteInt((int)num, stateObj);
				return;
			}
			if (value < TdsEnums.SQL_SMALL_MONEY_MIN || value > TdsEnums.SQL_SMALL_MONEY_MAX)
			{
				throw SQL.MoneyOverflow(value.ToString(CultureInfo.InvariantCulture));
			}
			this.WriteInt((int)num, stateObj);
		}

		private void WriteDate(DateTime value, TdsParserStateObject stateObj)
		{
			long num = (long)value.Subtract(DateTime.MinValue).Days;
			this.WritePartialLong(num, 3, stateObj);
		}

		private void WriteTime(TimeSpan value, byte scale, int length, TdsParserStateObject stateObj)
		{
			if (0L > value.Ticks || value.Ticks >= 864000000000L)
			{
				throw SQL.TimeOverflow(value.ToString());
			}
			long num = value.Ticks / TdsEnums.TICKS_FROM_SCALE[(int)scale];
			this.WritePartialLong(num, length, stateObj);
		}

		private void WriteDateTime2(DateTime value, byte scale, int length, TdsParserStateObject stateObj)
		{
			long num = value.TimeOfDay.Ticks / TdsEnums.TICKS_FROM_SCALE[(int)scale];
			this.WritePartialLong(num, length - 3, stateObj);
			this.WriteDate(value, stateObj);
		}

		private void WriteDateTimeOffset(DateTimeOffset value, byte scale, int length, TdsParserStateObject stateObj)
		{
			this.WriteDateTime2(value.UtcDateTime, scale, length - 2, stateObj);
			short num = (short)value.Offset.TotalMinutes;
			stateObj.WriteByte((byte)(num & 255));
			stateObj.WriteByte((byte)((num >> 8) & 255));
		}

		private bool TryReadSqlDecimal(SqlBuffer value, int length, byte precision, byte scale, TdsParserStateObject stateObj)
		{
			byte b;
			if (!stateObj.TryReadByte(out b))
			{
				return false;
			}
			bool flag = 1 == b;
			checked
			{
				length--;
				int[] array;
				if (!this.TryReadDecimalBits(length, stateObj, out array))
				{
					return false;
				}
				value.SetToDecimal(precision, scale, flag, array);
				return true;
			}
		}

		private bool TryReadDecimalBits(int length, TdsParserStateObject stateObj, out int[] bits)
		{
			bits = stateObj._decimalBits;
			if (bits == null)
			{
				bits = new int[4];
				stateObj._decimalBits = bits;
			}
			else
			{
				for (int i = 0; i < bits.Length; i++)
				{
					bits[i] = 0;
				}
			}
			int num = length >> 2;
			for (int i = 0; i < num; i++)
			{
				if (!stateObj.TryReadInt32(out bits[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal static SqlDecimal AdjustSqlDecimalScale(SqlDecimal d, int newScale)
		{
			if ((int)d.Scale != newScale)
			{
				return SqlDecimal.AdjustScale(d, newScale - (int)d.Scale, false);
			}
			return d;
		}

		internal static decimal AdjustDecimalScale(decimal value, int newScale)
		{
			int num = (decimal.GetBits(value)[3] & 16711680) >> 16;
			if (newScale != num)
			{
				SqlDecimal sqlDecimal = new SqlDecimal(value);
				sqlDecimal = SqlDecimal.AdjustScale(sqlDecimal, newScale - num, false);
				return sqlDecimal.Value;
			}
			return value;
		}

		internal void WriteSqlDecimal(SqlDecimal d, TdsParserStateObject stateObj)
		{
			if (d.IsPositive)
			{
				stateObj.WriteByte(1);
			}
			else
			{
				stateObj.WriteByte(0);
			}
			uint num;
			uint num2;
			uint num3;
			uint num4;
			SqlTypeWorkarounds.SqlDecimalExtractData(d, out num, out num2, out num3, out num4);
			this.WriteUnsignedInt(num, stateObj);
			this.WriteUnsignedInt(num2, stateObj);
			this.WriteUnsignedInt(num3, stateObj);
			this.WriteUnsignedInt(num4, stateObj);
		}

		private void WriteDecimal(decimal value, TdsParserStateObject stateObj)
		{
			stateObj._decimalBits = decimal.GetBits(value);
			if ((ulong)(-2147483648) == (ulong)((long)stateObj._decimalBits[3] & (long)((ulong)(-2147483648))))
			{
				stateObj.WriteByte(0);
			}
			else
			{
				stateObj.WriteByte(1);
			}
			this.WriteInt(stateObj._decimalBits[0], stateObj);
			this.WriteInt(stateObj._decimalBits[1], stateObj);
			this.WriteInt(stateObj._decimalBits[2], stateObj);
			this.WriteInt(0, stateObj);
		}

		private void WriteIdentifier(string s, TdsParserStateObject stateObj)
		{
			if (s != null)
			{
				stateObj.WriteByte(checked((byte)s.Length));
				this.WriteString(s, stateObj, true);
				return;
			}
			stateObj.WriteByte(0);
		}

		private void WriteIdentifierWithShortLength(string s, TdsParserStateObject stateObj)
		{
			if (s != null)
			{
				this.WriteShort((int)(checked((short)s.Length)), stateObj);
				this.WriteString(s, stateObj, true);
				return;
			}
			this.WriteShort(0, stateObj);
		}

		private Task WriteString(string s, TdsParserStateObject stateObj, bool canAccumulate = true)
		{
			return this.WriteString(s, s.Length, 0, stateObj, canAccumulate);
		}

		internal Task WriteCharArray(char[] carr, int length, int offset, TdsParserStateObject stateObj, bool canAccumulate = true)
		{
			int num = 2 * length;
			if (num < stateObj._outBuff.Length - stateObj._outBytesUsed)
			{
				TdsParser.CopyCharsToBytes(carr, offset, stateObj._outBuff, stateObj._outBytesUsed, length);
				stateObj._outBytesUsed += num;
				return null;
			}
			if (stateObj._bTmp == null || stateObj._bTmp.Length < num)
			{
				stateObj._bTmp = new byte[num];
			}
			TdsParser.CopyCharsToBytes(carr, offset, stateObj._bTmp, 0, length);
			return stateObj.WriteByteArray(stateObj._bTmp, num, 0, canAccumulate, null);
		}

		internal Task WriteString(string s, int length, int offset, TdsParserStateObject stateObj, bool canAccumulate = true)
		{
			int num = 2 * length;
			if (num < stateObj._outBuff.Length - stateObj._outBytesUsed)
			{
				TdsParser.CopyStringToBytes(s, offset, stateObj._outBuff, stateObj._outBytesUsed, length);
				stateObj._outBytesUsed += num;
				return null;
			}
			if (stateObj._bTmp == null || stateObj._bTmp.Length < num)
			{
				stateObj._bTmp = new byte[num];
			}
			TdsParser.CopyStringToBytes(s, offset, stateObj._bTmp, 0, length);
			return stateObj.WriteByteArray(stateObj._bTmp, num, 0, canAccumulate, null);
		}

		private static void CopyCharsToBytes(char[] source, int sourceOffset, byte[] dest, int destOffset, int charLength)
		{
			Buffer.BlockCopy(source, sourceOffset, dest, destOffset, charLength * 2);
		}

		private static void CopyStringToBytes(string source, int sourceOffset, byte[] dest, int destOffset, int charLength)
		{
			Encoding.Unicode.GetBytes(source, sourceOffset, charLength, dest, destOffset);
		}

		private Task WriteEncodingChar(string s, Encoding encoding, TdsParserStateObject stateObj, bool canAccumulate = true)
		{
			return this.WriteEncodingChar(s, s.Length, 0, encoding, stateObj, canAccumulate);
		}

		private Task WriteEncodingChar(string s, int numChars, int offset, Encoding encoding, TdsParserStateObject stateObj, bool canAccumulate = true)
		{
			if (encoding == null)
			{
				encoding = this._defaultEncoding;
			}
			char[] array = s.ToCharArray(offset, numChars);
			int num = stateObj._outBuff.Length - stateObj._outBytesUsed;
			if (numChars <= num && encoding.GetMaxByteCount(array.Length) <= num)
			{
				int bytes = encoding.GetBytes(array, 0, array.Length, stateObj._outBuff, stateObj._outBytesUsed);
				stateObj._outBytesUsed += bytes;
				return null;
			}
			byte[] bytes2 = encoding.GetBytes(array, 0, numChars);
			return stateObj.WriteByteArray(bytes2, bytes2.Length, 0, canAccumulate, null);
		}

		internal int GetEncodingCharLength(string value, int numChars, int charOffset, Encoding encoding)
		{
			if (value == null || value == ADP.StrEmpty)
			{
				return 0;
			}
			if (encoding == null)
			{
				if (this._defaultEncoding == null)
				{
					this.ThrowUnsupportedCollationEncountered(null);
				}
				encoding = this._defaultEncoding;
			}
			char[] array = value.ToCharArray(charOffset, numChars);
			return encoding.GetByteCount(array, 0, numChars);
		}

		internal bool TryGetDataLength(SqlMetaDataPriv colmeta, TdsParserStateObject stateObj, out ulong length)
		{
			if (colmeta.metaType.IsPlp)
			{
				return stateObj.TryReadPlpLength(true, out length);
			}
			int num;
			if (!this.TryGetTokenLength(colmeta.tdsType, stateObj, out num))
			{
				length = 0UL;
				return false;
			}
			length = (ulong)((long)num);
			return true;
		}

		internal bool TryGetTokenLength(byte token, TdsParserStateObject stateObj, out int tokenLength)
		{
			if (token == 174)
			{
				tokenLength = -1;
				return true;
			}
			if (token == 228)
			{
				return stateObj.TryReadInt32(out tokenLength);
			}
			if (token == 240)
			{
				tokenLength = -1;
				return true;
			}
			if (token == 172)
			{
				tokenLength = -1;
				return true;
			}
			if (token != 241)
			{
				int num = (int)(token & 48);
				if (num <= 16)
				{
					if (num != 0)
					{
						if (num != 16)
						{
							goto IL_00D1;
						}
						tokenLength = 0;
						return true;
					}
				}
				else if (num != 32)
				{
					if (num == 48)
					{
						tokenLength = (1 << ((token & 12) >> 2)) & 255;
						return true;
					}
					goto IL_00D1;
				}
				if ((token & 128) != 0)
				{
					ushort num2;
					if (!stateObj.TryReadUInt16(out num2))
					{
						tokenLength = 0;
						return false;
					}
					tokenLength = (int)num2;
					return true;
				}
				else
				{
					if ((token & 12) == 0)
					{
						return stateObj.TryReadInt32(out tokenLength);
					}
					byte b;
					if (!stateObj.TryReadByte(out b))
					{
						tokenLength = 0;
						return false;
					}
					tokenLength = (int)b;
					return true;
				}
				IL_00D1:
				tokenLength = 0;
				return true;
			}
			ushort num3;
			if (!stateObj.TryReadUInt16(out num3))
			{
				tokenLength = 0;
				return false;
			}
			tokenLength = (int)num3;
			return true;
		}

		private void ProcessAttention(TdsParserStateObject stateObj)
		{
			if (this._state == TdsParserState.Closed || this._state == TdsParserState.Broken)
			{
				return;
			}
			stateObj.StoreErrorAndWarningForAttention();
			try
			{
				this.Run(RunBehavior.Attention, null, null, null, stateObj);
			}
			catch (Exception ex)
			{
				if (!ADP.IsCatchableExceptionType(ex))
				{
					throw;
				}
				this._state = TdsParserState.Broken;
				this._connHandler.BreakConnection();
				throw;
			}
			stateObj.RestoreErrorAndWarningAfterAttention();
		}

		private static int StateValueLength(int dataLen)
		{
			if (dataLen >= 255)
			{
				return dataLen + 5;
			}
			return dataLen + 1;
		}

		internal int WriteSessionRecoveryFeatureRequest(SessionData reconnectData, bool write)
		{
			int num = 1;
			if (write)
			{
				this._physicalStateObj.WriteByte(1);
			}
			if (reconnectData == null)
			{
				if (write)
				{
					this.WriteInt(0, this._physicalStateObj);
				}
				num += 4;
			}
			else
			{
				int num2 = 0;
				num2 += 1 + 2 * TdsParserStaticMethods.NullAwareStringLength(reconnectData._initialDatabase);
				num2 += 1 + 2 * TdsParserStaticMethods.NullAwareStringLength(reconnectData._initialLanguage);
				num2 += ((reconnectData._initialCollation == null) ? 1 : 6);
				for (int i = 0; i < 256; i++)
				{
					if (reconnectData._initialState[i] != null)
					{
						num2 += 1 + TdsParser.StateValueLength(reconnectData._initialState[i].Length);
					}
				}
				int num3 = 0;
				num3 += 1 + 2 * ((reconnectData._initialDatabase == reconnectData._database) ? 0 : TdsParserStaticMethods.NullAwareStringLength(reconnectData._database));
				num3 += 1 + 2 * ((reconnectData._initialLanguage == reconnectData._language) ? 0 : TdsParserStaticMethods.NullAwareStringLength(reconnectData._language));
				num3 += ((reconnectData._collation != null && !SqlCollation.AreSame(reconnectData._collation, reconnectData._initialCollation)) ? 6 : 1);
				bool[] array = new bool[256];
				for (int j = 0; j < 256; j++)
				{
					if (reconnectData._delta[j] != null)
					{
						array[j] = true;
						if (reconnectData._initialState[j] != null && reconnectData._initialState[j].Length == reconnectData._delta[j]._dataLength)
						{
							array[j] = false;
							for (int k = 0; k < reconnectData._delta[j]._dataLength; k++)
							{
								if (reconnectData._initialState[j][k] != reconnectData._delta[j]._data[k])
								{
									array[j] = true;
									break;
								}
							}
						}
						if (array[j])
						{
							num3 += 1 + TdsParser.StateValueLength(reconnectData._delta[j]._dataLength);
						}
					}
				}
				if (write)
				{
					this.WriteInt(8 + num2 + num3, this._physicalStateObj);
					this.WriteInt(num2, this._physicalStateObj);
					this.WriteIdentifier(reconnectData._initialDatabase, this._physicalStateObj);
					this.WriteCollation(reconnectData._initialCollation, this._physicalStateObj);
					this.WriteIdentifier(reconnectData._initialLanguage, this._physicalStateObj);
					for (int l = 0; l < 256; l++)
					{
						if (reconnectData._initialState[l] != null)
						{
							this._physicalStateObj.WriteByte((byte)l);
							if (reconnectData._initialState[l].Length < 255)
							{
								this._physicalStateObj.WriteByte((byte)reconnectData._initialState[l].Length);
							}
							else
							{
								this._physicalStateObj.WriteByte(byte.MaxValue);
								this.WriteInt(reconnectData._initialState[l].Length, this._physicalStateObj);
							}
							this._physicalStateObj.WriteByteArray(reconnectData._initialState[l], reconnectData._initialState[l].Length, 0, true, null);
						}
					}
					this.WriteInt(num3, this._physicalStateObj);
					this.WriteIdentifier((reconnectData._database != reconnectData._initialDatabase) ? reconnectData._database : null, this._physicalStateObj);
					this.WriteCollation(SqlCollation.AreSame(reconnectData._initialCollation, reconnectData._collation) ? null : reconnectData._collation, this._physicalStateObj);
					this.WriteIdentifier((reconnectData._language != reconnectData._initialLanguage) ? reconnectData._language : null, this._physicalStateObj);
					for (int m = 0; m < 256; m++)
					{
						if (array[m])
						{
							this._physicalStateObj.WriteByte((byte)m);
							if (reconnectData._delta[m]._dataLength < 255)
							{
								this._physicalStateObj.WriteByte((byte)reconnectData._delta[m]._dataLength);
							}
							else
							{
								this._physicalStateObj.WriteByte(byte.MaxValue);
								this.WriteInt(reconnectData._delta[m]._dataLength, this._physicalStateObj);
							}
							this._physicalStateObj.WriteByteArray(reconnectData._delta[m]._data, reconnectData._delta[m]._dataLength, 0, true, null);
						}
					}
				}
				num += num2 + num3 + 12;
			}
			return num;
		}

		internal int WriteGlobalTransactionsFeatureRequest(bool write)
		{
			int num = 5;
			if (write)
			{
				this._physicalStateObj.WriteByte(5);
				this.WriteInt(0, this._physicalStateObj);
			}
			return num;
		}

		internal void TdsLogin(SqlLogin rec, TdsEnums.FeatureExtension requestedFeatures, SessionData recoverySessionData)
		{
			this._physicalStateObj.SetTimeoutSeconds(rec.timeout);
			this._connHandler.TimeoutErrorInternal.EndPhase(SqlConnectionTimeoutErrorPhase.LoginBegin);
			this._connHandler.TimeoutErrorInternal.SetAndBeginPhase(SqlConnectionTimeoutErrorPhase.ProcessConnectionAuth);
			bool flag = requestedFeatures > TdsEnums.FeatureExtension.None;
			string userName = rec.userName;
			byte[] array = TdsParserStaticMethods.ObfuscatePassword(rec.password);
			int num = array.Length;
			this._physicalStateObj._outputMessageType = 16;
			int num2 = 94;
			string text = "Core .Net SqlClient Data Provider";
			byte[] array2;
			uint num3;
			int num4;
			checked
			{
				num2 += (rec.hostName.Length + rec.applicationName.Length + rec.serverName.Length + text.Length + rec.language.Length + rec.database.Length + rec.attachDBFilename.Length) * 2;
				if (flag)
				{
					num2 += 4;
				}
				array2 = null;
				num3 = 0U;
				if (!rec.useSSPI)
				{
					num2 += userName.Length * 2 + num;
				}
				else if (rec.useSSPI)
				{
					array2 = new byte[TdsParser.s_maxSSPILength];
					num3 = TdsParser.s_maxSSPILength;
					this._physicalStateObj.SniContext = SniContext.Snix_LoginSspi;
					this.SSPIData(null, 0U, ref array2, ref num3);
					if (num3 > 2147483647U)
					{
						throw SQL.InvalidSSPIPacketSize();
					}
					this._physicalStateObj.SniContext = SniContext.Snix_Login;
					num2 += (int)num3;
				}
				num4 = num2;
			}
			if (flag)
			{
				if ((requestedFeatures & TdsEnums.FeatureExtension.SessionRecovery) != TdsEnums.FeatureExtension.None)
				{
					num2 += this.WriteSessionRecoveryFeatureRequest(recoverySessionData, false);
				}
				if ((requestedFeatures & TdsEnums.FeatureExtension.GlobalTransactions) != TdsEnums.FeatureExtension.None)
				{
					num2 += this.WriteGlobalTransactionsFeatureRequest(false);
				}
				num2++;
			}
			try
			{
				this.WriteInt(num2, this._physicalStateObj);
				if (recoverySessionData == null)
				{
					this.WriteInt(1946157060, this._physicalStateObj);
				}
				else
				{
					this.WriteUnsignedInt(recoverySessionData._tdsVersion, this._physicalStateObj);
				}
				this.WriteInt(rec.packetSize, this._physicalStateObj);
				this.WriteInt(100663296, this._physicalStateObj);
				this.WriteInt(TdsParserStaticMethods.GetCurrentProcessIdForTdsLoginOnly(), this._physicalStateObj);
				this.WriteInt(0, this._physicalStateObj);
				int num5 = 0;
				num5 |= 32;
				num5 |= 64;
				num5 |= 128;
				num5 |= 256;
				num5 |= 512;
				if (rec.useReplication)
				{
					num5 |= 12288;
				}
				if (rec.useSSPI)
				{
					num5 |= 32768;
				}
				if (rec.readOnlyIntent)
				{
					num5 |= 2097152;
				}
				if (rec.userInstance)
				{
					num5 |= 67108864;
				}
				if (flag)
				{
					num5 |= 268435456;
				}
				this.WriteInt(num5, this._physicalStateObj);
				this.WriteInt(0, this._physicalStateObj);
				this.WriteInt(0, this._physicalStateObj);
				int num6 = 94;
				this.WriteShort(num6, this._physicalStateObj);
				this.WriteShort(rec.hostName.Length, this._physicalStateObj);
				num6 += rec.hostName.Length * 2;
				if (!rec.useSSPI)
				{
					this.WriteShort(num6, this._physicalStateObj);
					this.WriteShort(userName.Length, this._physicalStateObj);
					num6 += userName.Length * 2;
					this.WriteShort(num6, this._physicalStateObj);
					this.WriteShort(num / 2, this._physicalStateObj);
					num6 += num;
				}
				else
				{
					this.WriteShort(0, this._physicalStateObj);
					this.WriteShort(0, this._physicalStateObj);
					this.WriteShort(0, this._physicalStateObj);
					this.WriteShort(0, this._physicalStateObj);
				}
				this.WriteShort(num6, this._physicalStateObj);
				this.WriteShort(rec.applicationName.Length, this._physicalStateObj);
				num6 += rec.applicationName.Length * 2;
				this.WriteShort(num6, this._physicalStateObj);
				this.WriteShort(rec.serverName.Length, this._physicalStateObj);
				num6 += rec.serverName.Length * 2;
				this.WriteShort(num6, this._physicalStateObj);
				if (flag)
				{
					this.WriteShort(4, this._physicalStateObj);
					num6 += 4;
				}
				else
				{
					this.WriteShort(0, this._physicalStateObj);
				}
				this.WriteShort(num6, this._physicalStateObj);
				this.WriteShort(text.Length, this._physicalStateObj);
				num6 += text.Length * 2;
				this.WriteShort(num6, this._physicalStateObj);
				this.WriteShort(rec.language.Length, this._physicalStateObj);
				num6 += rec.language.Length * 2;
				this.WriteShort(num6, this._physicalStateObj);
				this.WriteShort(rec.database.Length, this._physicalStateObj);
				num6 += rec.database.Length * 2;
				if (TdsParser.s_nicAddress == null)
				{
					TdsParser.s_nicAddress = TdsParserStaticMethods.GetNetworkPhysicalAddressForTdsLoginOnly();
				}
				this._physicalStateObj.WriteByteArray(TdsParser.s_nicAddress, TdsParser.s_nicAddress.Length, 0, true, null);
				this.WriteShort(num6, this._physicalStateObj);
				if (rec.useSSPI)
				{
					this.WriteShort((int)num3, this._physicalStateObj);
					num6 += (int)num3;
				}
				else
				{
					this.WriteShort(0, this._physicalStateObj);
				}
				this.WriteShort(num6, this._physicalStateObj);
				this.WriteShort(rec.attachDBFilename.Length, this._physicalStateObj);
				num6 += rec.attachDBFilename.Length * 2;
				this.WriteShort(num6, this._physicalStateObj);
				this.WriteShort(0, this._physicalStateObj);
				this.WriteInt(0, this._physicalStateObj);
				this.WriteString(rec.hostName, this._physicalStateObj, true);
				if (!rec.useSSPI)
				{
					this.WriteString(userName, this._physicalStateObj, true);
					this._physicalStateObj.WriteByteArray(array, num, 0, true, null);
				}
				this.WriteString(rec.applicationName, this._physicalStateObj, true);
				this.WriteString(rec.serverName, this._physicalStateObj, true);
				if (flag)
				{
					this.WriteInt(num4, this._physicalStateObj);
				}
				this.WriteString(text, this._physicalStateObj, true);
				this.WriteString(rec.language, this._physicalStateObj, true);
				this.WriteString(rec.database, this._physicalStateObj, true);
				if (rec.useSSPI)
				{
					this._physicalStateObj.WriteByteArray(array2, (int)num3, 0, true, null);
				}
				this.WriteString(rec.attachDBFilename, this._physicalStateObj, true);
				if (flag)
				{
					if ((requestedFeatures & TdsEnums.FeatureExtension.SessionRecovery) != TdsEnums.FeatureExtension.None)
					{
						num2 += this.WriteSessionRecoveryFeatureRequest(recoverySessionData, true);
					}
					if ((requestedFeatures & TdsEnums.FeatureExtension.GlobalTransactions) != TdsEnums.FeatureExtension.None)
					{
						this.WriteGlobalTransactionsFeatureRequest(true);
					}
					this._physicalStateObj.WriteByte(byte.MaxValue);
				}
			}
			catch (Exception ex)
			{
				if (ADP.IsCatchableExceptionType(ex))
				{
					this._physicalStateObj._outputPacketNumber = 1;
					this._physicalStateObj.ResetBuffer();
				}
				throw;
			}
			this._physicalStateObj.WritePacket(1, false);
			this._physicalStateObj._pendingData = true;
			this._physicalStateObj._messageStatus = 0;
		}

		private void SSPIData(byte[] receivedBuff, uint receivedLength, ref byte[] sendBuff, ref uint sendLength)
		{
			this.SNISSPIData(receivedBuff, receivedLength, ref sendBuff, ref sendLength);
		}

		private void SNISSPIData(byte[] receivedBuff, uint receivedLength, ref byte[] sendBuff, ref uint sendLength)
		{
			if (TdsParserStateObjectFactory.UseManagedSNI)
			{
				try
				{
					this._physicalStateObj.GenerateSspiClientContext(receivedBuff, receivedLength, ref sendBuff, ref sendLength, this._sniSpnBuffer);
					return;
				}
				catch (Exception ex)
				{
					this.SSPIError(ex.Message + Environment.NewLine + ex.StackTrace, "GenClientContext");
					return;
				}
			}
			if (receivedBuff == null)
			{
				receivedLength = 0U;
			}
			if (this._physicalStateObj.GenerateSspiClientContext(receivedBuff, receivedLength, ref sendBuff, ref sendLength, this._sniSpnBuffer) != 0U)
			{
				this.SSPIError(SQLMessage.SSPIGenerateError(), "GenClientContext");
			}
		}

		private void ProcessSSPI(int receivedLength)
		{
			SniContext sniContext = this._physicalStateObj.SniContext;
			this._physicalStateObj.SniContext = SniContext.Snix_ProcessSspi;
			byte[] array = new byte[receivedLength];
			if (!this._physicalStateObj.TryReadByteArray(array, 0, receivedLength))
			{
				throw SQL.SynchronousCallMayNotPend();
			}
			byte[] array2 = new byte[TdsParser.s_maxSSPILength];
			uint num = TdsParser.s_maxSSPILength;
			this.SSPIData(array, (uint)receivedLength, ref array2, ref num);
			this._physicalStateObj.WriteByteArray(array2, (int)num, 0, true, null);
			this._physicalStateObj._outputMessageType = 17;
			this._physicalStateObj.WritePacket(1, false);
			this._physicalStateObj.SniContext = sniContext;
		}

		private void SSPIError(string error, string procedure)
		{
			this._physicalStateObj.AddError(new SqlError(0, 0, 11, this._server, error, procedure, 0, null));
			this.ThrowExceptionAndWarning(this._physicalStateObj, false, false);
		}

		internal byte[] GetDTCAddress(int timeout, TdsParserStateObject stateObj)
		{
			byte[] array = null;
			using (SqlDataReader sqlDataReader = this.TdsExecuteTransactionManagerRequest(null, TdsEnums.TransactionManagerRequestType.GetDTCAddress, null, TdsEnums.TransactionManagerIsolationLevel.Unspecified, timeout, null, stateObj, true))
			{
				if (sqlDataReader != null && sqlDataReader.Read())
				{
					long bytes = sqlDataReader.GetBytes(0, 0L, null, 0, 0);
					if (bytes <= 2147483647L)
					{
						int num = (int)bytes;
						array = new byte[num];
						sqlDataReader.GetBytes(0, 0L, array, 0, num);
					}
				}
			}
			return array;
		}

		internal void PropagateDistributedTransaction(byte[] buffer, int timeout, TdsParserStateObject stateObj)
		{
			this.TdsExecuteTransactionManagerRequest(buffer, TdsEnums.TransactionManagerRequestType.Propagate, null, TdsEnums.TransactionManagerIsolationLevel.Unspecified, timeout, null, stateObj, true);
		}

		internal SqlDataReader TdsExecuteTransactionManagerRequest(byte[] buffer, TdsEnums.TransactionManagerRequestType request, string transactionName, TdsEnums.TransactionManagerIsolationLevel isoLevel, int timeout, SqlInternalTransaction transaction, TdsParserStateObject stateObj, bool isDelegateControlRequest)
		{
			if (TdsParserState.Broken == this.State || this.State == TdsParserState.Closed)
			{
				return null;
			}
			bool threadHasParserLockForClose = this._connHandler.ThreadHasParserLockForClose;
			if (!threadHasParserLockForClose)
			{
				this._connHandler._parserLock.Wait(false);
				this._connHandler.ThreadHasParserLockForClose = true;
			}
			bool asyncWrite = this._asyncWrite;
			SqlDataReader sqlDataReader2;
			try
			{
				this._asyncWrite = false;
				if (!isDelegateControlRequest)
				{
					this._connHandler.CheckEnlistedTransactionBinding();
				}
				stateObj._outputMessageType = 14;
				stateObj.SetTimeoutSeconds(timeout);
				stateObj.SniContext = SniContext.Snix_Execute;
				this.WriteInt(22, stateObj);
				this.WriteInt(18, stateObj);
				this.WriteMarsHeaderData(stateObj, this._currentTransaction);
				this.WriteShort((int)((short)request), stateObj);
				bool flag = false;
				switch (request)
				{
				case TdsEnums.TransactionManagerRequestType.GetDTCAddress:
					this.WriteShort(0, stateObj);
					flag = true;
					break;
				case TdsEnums.TransactionManagerRequestType.Propagate:
					if (buffer != null)
					{
						this.WriteShort(buffer.Length, stateObj);
						stateObj.WriteByteArray(buffer, buffer.Length, 0, true, null);
					}
					else
					{
						this.WriteShort(0, stateObj);
					}
					break;
				case TdsEnums.TransactionManagerRequestType.Begin:
					if (this._currentTransaction != transaction)
					{
						this.PendingTransaction = transaction;
					}
					stateObj.WriteByte((byte)isoLevel);
					stateObj.WriteByte((byte)(transactionName.Length * 2));
					this.WriteString(transactionName, stateObj, true);
					break;
				case TdsEnums.TransactionManagerRequestType.Commit:
					stateObj.WriteByte(0);
					stateObj.WriteByte(0);
					break;
				case TdsEnums.TransactionManagerRequestType.Rollback:
					stateObj.WriteByte((byte)(transactionName.Length * 2));
					this.WriteString(transactionName, stateObj, true);
					stateObj.WriteByte(0);
					break;
				case TdsEnums.TransactionManagerRequestType.Save:
					stateObj.WriteByte((byte)(transactionName.Length * 2));
					this.WriteString(transactionName, stateObj, true);
					break;
				}
				stateObj.WritePacket(1, false);
				stateObj._pendingData = true;
				stateObj._messageStatus = 0;
				SqlDataReader sqlDataReader = null;
				stateObj.SniContext = SniContext.Snix_Read;
				if (flag)
				{
					sqlDataReader = new SqlDataReader(null, CommandBehavior.Default);
					sqlDataReader.Bind(stateObj);
					_SqlMetaDataSet metaData = sqlDataReader.MetaData;
				}
				else
				{
					this.Run(RunBehavior.UntilDone, null, null, null, stateObj);
				}
				if ((request == TdsEnums.TransactionManagerRequestType.Begin || request == TdsEnums.TransactionManagerRequestType.Propagate) && (transaction == null || transaction.TransactionId != this._retainedTransactionId))
				{
					this._retainedTransactionId = 0L;
				}
				sqlDataReader2 = sqlDataReader;
			}
			catch (Exception ex)
			{
				if (!ADP.IsCatchableExceptionType(ex))
				{
					throw;
				}
				this.FailureCleanup(stateObj, ex);
				throw;
			}
			finally
			{
				this._pendingTransaction = null;
				this._asyncWrite = asyncWrite;
				if (!threadHasParserLockForClose)
				{
					this._connHandler.ThreadHasParserLockForClose = false;
					this._connHandler._parserLock.Release();
				}
			}
			return sqlDataReader2;
		}

		internal void FailureCleanup(TdsParserStateObject stateObj, Exception e)
		{
			int outputPacketNumber = (int)stateObj._outputPacketNumber;
			if (stateObj.HasOpenResult)
			{
				stateObj.DecrementOpenResultCount();
			}
			stateObj.ResetBuffer();
			stateObj._outputPacketNumber = 1;
			if (outputPacketNumber != 1 && this._state == TdsParserState.OpenLoggedIn)
			{
				bool threadHasParserLockForClose = this._connHandler.ThreadHasParserLockForClose;
				try
				{
					this._connHandler.ThreadHasParserLockForClose = true;
					stateObj.SendAttention(false);
					this.ProcessAttention(stateObj);
				}
				finally
				{
					this._connHandler.ThreadHasParserLockForClose = threadHasParserLockForClose;
				}
			}
		}

		internal Task TdsExecuteSQLBatch(string text, int timeout, SqlNotificationRequest notificationRequest, TdsParserStateObject stateObj, bool sync, bool callerHasConnectionLock = false)
		{
			if (TdsParserState.Broken == this.State || this.State == TdsParserState.Closed)
			{
				return null;
			}
			if (stateObj.BcpLock)
			{
				throw SQL.ConnectionLockedForBcpEvent();
			}
			bool flag = !callerHasConnectionLock && !this._connHandler.ThreadHasParserLockForClose;
			bool flag2 = false;
			if (flag)
			{
				this._connHandler._parserLock.Wait(!sync);
				flag2 = true;
			}
			this._asyncWrite = !sync;
			Task task2;
			try
			{
				if (this._state == TdsParserState.Closed || this._state == TdsParserState.Broken)
				{
					throw ADP.ClosedConnectionError();
				}
				this._connHandler.CheckEnlistedTransactionBinding();
				stateObj.SetTimeoutSeconds(timeout);
				stateObj.SniContext = SniContext.Snix_Execute;
				this.WriteRPCBatchHeaders(stateObj, notificationRequest);
				stateObj._outputMessageType = 1;
				this.WriteString(text, text.Length, 0, stateObj, true);
				Task task = stateObj.ExecuteFlush();
				if (task == null)
				{
					stateObj.SniContext = SniContext.Snix_Read;
					task2 = null;
				}
				else
				{
					bool taskReleaseConnectionLock = flag2;
					flag2 = false;
					task2 = task.ContinueWith(delegate(Task t)
					{
						try
						{
							if (t.IsFaulted)
							{
								this.FailureCleanup(stateObj, t.Exception.InnerException);
								throw t.Exception.InnerException;
							}
							stateObj.SniContext = SniContext.Snix_Read;
						}
						finally
						{
							if (taskReleaseConnectionLock)
							{
								this._connHandler._parserLock.Release();
							}
						}
					}, TaskScheduler.Default);
				}
			}
			catch (Exception ex)
			{
				if (!ADP.IsCatchableExceptionType(ex))
				{
					throw;
				}
				this.FailureCleanup(stateObj, ex);
				throw;
			}
			finally
			{
				if (flag2)
				{
					this._connHandler._parserLock.Release();
				}
			}
			return task2;
		}

		internal Task TdsExecuteRPC(_SqlRPC[] rpcArray, int timeout, bool inSchema, SqlNotificationRequest notificationRequest, TdsParserStateObject stateObj, bool isCommandProc, bool sync = true, TaskCompletionSource<object> completion = null, int startRpc = 0, int startParam = 0)
		{
			bool flag = completion == null;
			bool flag2 = false;
			Task task7;
			try
			{
				if (flag)
				{
					this._connHandler._parserLock.Wait(!sync);
					flag2 = true;
				}
				try
				{
					if (TdsParserState.Broken == this.State || this.State == TdsParserState.Closed)
					{
						throw ADP.ClosedConnectionError();
					}
					if (flag)
					{
						this._asyncWrite = !sync;
						this._connHandler.CheckEnlistedTransactionBinding();
						stateObj.SetTimeoutSeconds(timeout);
						stateObj.SniContext = SniContext.Snix_Execute;
						this.WriteRPCBatchHeaders(stateObj, notificationRequest);
						stateObj._outputMessageType = 3;
					}
					Action<Exception> <>9__1;
					Action<Task> <>9__2;
					int num6;
					int ii;
					for (ii = startRpc; ii < rpcArray.Length; ii = num6 + 1)
					{
						_SqlRPC sqlRPC = rpcArray[ii];
						if (startParam == 0 || ii > startRpc)
						{
							if (sqlRPC.ProcID != 0)
							{
								this.WriteShort(65535, stateObj);
								this.WriteShort((int)((short)sqlRPC.ProcID), stateObj);
							}
							else
							{
								int num = sqlRPC.rpcName.Length;
								this.WriteShort(num, stateObj);
								this.WriteString(sqlRPC.rpcName, num, 0, stateObj, true);
							}
							this.WriteShort((int)((short)sqlRPC.options), stateObj);
						}
						SqlParameter[] parameters = sqlRPC.parameters;
						int i;
						for (i = ((ii == startRpc) ? startParam : 0); i < parameters.Length; i = num6 + 1)
						{
							SqlParameter sqlParameter = parameters[i];
							if (sqlParameter == null)
							{
								break;
							}
							sqlParameter.Validate(i, isCommandProc);
							MetaType internalMetaType = sqlParameter.InternalMetaType;
							if (internalMetaType.IsNewKatmaiType)
							{
								this.WriteSmiParameter(sqlParameter, i, (sqlRPC.paramoptions[i] & 2) > 0, stateObj);
							}
							else
							{
								if (!this._isKatmai && !internalMetaType.Is90Supported)
								{
									throw ADP.VersionDoesNotSupportDataType(internalMetaType.TypeName);
								}
								object obj = null;
								bool flag3 = true;
								bool flag4 = false;
								bool flag5 = false;
								if (sqlParameter.Direction == ParameterDirection.Output)
								{
									flag4 = sqlParameter.ParameterIsSqlType;
									sqlParameter.Value = null;
									sqlParameter.ParameterIsSqlType = flag4;
								}
								else
								{
									obj = sqlParameter.GetCoercedValue();
									flag3 = sqlParameter.IsNull;
									if (!flag3)
									{
										flag4 = sqlParameter.CoercedValueIsSqlType;
										flag5 = sqlParameter.CoercedValueIsDataFeed;
									}
								}
								this.WriteParameterName(sqlParameter.ParameterNameFixed, stateObj);
								stateObj.WriteByte(sqlRPC.paramoptions[i]);
								stateObj.WriteByte(internalMetaType.NullableType);
								if (internalMetaType.TDSType == 98)
								{
									this.WriteSqlVariantValue(flag4 ? MetaType.GetComValueFromSqlVariant(obj) : obj, sqlParameter.GetActualSize(), sqlParameter.Offset, stateObj, true);
								}
								else
								{
									int num2 = (internalMetaType.IsSizeInCharacters ? (sqlParameter.GetParameterSize() * 2) : sqlParameter.GetParameterSize());
									int num3;
									if (internalMetaType.TDSType != 240)
									{
										num3 = sqlParameter.GetActualSize();
									}
									else
									{
										num3 = 0;
									}
									int num4 = 0;
									if (internalMetaType.IsAnsiType)
									{
										if (!flag3 && !flag5)
										{
											string text;
											if (flag4)
											{
												if (obj is SqlString)
												{
													text = ((SqlString)obj).Value;
												}
												else
												{
													text = new string(((SqlChars)obj).Value);
												}
											}
											else
											{
												text = (string)obj;
											}
											num4 = this.GetEncodingCharLength(text, num3, sqlParameter.Offset, this._defaultEncoding);
										}
										if (internalMetaType.IsPlp)
										{
											this.WriteShort(65535, stateObj);
										}
										else
										{
											int num5 = ((num2 > num4) ? num2 : num4);
											if (num5 == 0)
											{
												if (internalMetaType.IsNCharType)
												{
													num5 = 2;
												}
												else
												{
													num5 = 1;
												}
											}
											this.WriteParameterVarLen(internalMetaType, num5, false, stateObj, false);
										}
									}
									else if (internalMetaType.SqlDbType == SqlDbType.Timestamp)
									{
										this.WriteParameterVarLen(internalMetaType, 8, false, stateObj, false);
									}
									else
									{
										if (internalMetaType.SqlDbType == SqlDbType.Udt)
										{
											throw ADP.DbTypeNotSupported(SqlDbType.Udt.ToString());
										}
										if (internalMetaType.IsPlp)
										{
											if (internalMetaType.SqlDbType != SqlDbType.Xml)
											{
												this.WriteShort(65535, stateObj);
											}
										}
										else if (!internalMetaType.IsVarTime && internalMetaType.SqlDbType != SqlDbType.Date)
										{
											int num5 = ((num2 > num3) ? num2 : num3);
											if (num5 == 0)
											{
												if (internalMetaType.IsNCharType)
												{
													num5 = 2;
												}
												else
												{
													num5 = 1;
												}
											}
											this.WriteParameterVarLen(internalMetaType, num5, false, stateObj, false);
										}
									}
									if (internalMetaType.SqlDbType == SqlDbType.Decimal)
									{
										byte actualPrecision = sqlParameter.GetActualPrecision();
										byte actualScale = sqlParameter.GetActualScale();
										if (actualPrecision > 38)
										{
											throw SQL.PrecisionValueOutOfRange(actualPrecision);
										}
										if (!flag3)
										{
											if (flag4)
											{
												obj = TdsParser.AdjustSqlDecimalScale((SqlDecimal)obj, (int)actualScale);
												if (actualPrecision != 0 && actualPrecision < ((SqlDecimal)obj).Precision)
												{
													throw ADP.ParameterValueOutOfRange((SqlDecimal)obj);
												}
											}
											else
											{
												obj = TdsParser.AdjustDecimalScale((decimal)obj, (int)actualScale);
												SqlDecimal sqlDecimal = new SqlDecimal((decimal)obj);
												if (actualPrecision != 0 && actualPrecision < sqlDecimal.Precision)
												{
													throw ADP.ParameterValueOutOfRange((decimal)obj);
												}
											}
										}
										if (actualPrecision == 0)
										{
											stateObj.WriteByte(29);
										}
										else
										{
											stateObj.WriteByte(actualPrecision);
										}
										stateObj.WriteByte(actualScale);
									}
									else if (internalMetaType.IsVarTime)
									{
										stateObj.WriteByte(sqlParameter.GetActualScale());
									}
									if (internalMetaType.SqlDbType == SqlDbType.Xml)
									{
										if ((sqlParameter.XmlSchemaCollectionDatabase != null && sqlParameter.XmlSchemaCollectionDatabase != ADP.StrEmpty) || (sqlParameter.XmlSchemaCollectionOwningSchema != null && sqlParameter.XmlSchemaCollectionOwningSchema != ADP.StrEmpty) || (sqlParameter.XmlSchemaCollectionName != null && sqlParameter.XmlSchemaCollectionName != ADP.StrEmpty))
										{
											stateObj.WriteByte(1);
											if (sqlParameter.XmlSchemaCollectionDatabase != null && sqlParameter.XmlSchemaCollectionDatabase != ADP.StrEmpty)
											{
												int num = sqlParameter.XmlSchemaCollectionDatabase.Length;
												stateObj.WriteByte((byte)num);
												this.WriteString(sqlParameter.XmlSchemaCollectionDatabase, num, 0, stateObj, true);
											}
											else
											{
												stateObj.WriteByte(0);
											}
											if (sqlParameter.XmlSchemaCollectionOwningSchema != null && sqlParameter.XmlSchemaCollectionOwningSchema != ADP.StrEmpty)
											{
												int num = sqlParameter.XmlSchemaCollectionOwningSchema.Length;
												stateObj.WriteByte((byte)num);
												this.WriteString(sqlParameter.XmlSchemaCollectionOwningSchema, num, 0, stateObj, true);
											}
											else
											{
												stateObj.WriteByte(0);
											}
											if (sqlParameter.XmlSchemaCollectionName != null && sqlParameter.XmlSchemaCollectionName != ADP.StrEmpty)
											{
												int num = sqlParameter.XmlSchemaCollectionName.Length;
												this.WriteShort((int)((short)num), stateObj);
												this.WriteString(sqlParameter.XmlSchemaCollectionName, num, 0, stateObj, true);
											}
											else
											{
												this.WriteShort(0, stateObj);
											}
										}
										else
										{
											stateObj.WriteByte(0);
										}
									}
									else if (internalMetaType.IsCharType)
									{
										SqlCollation sqlCollation = ((sqlParameter.Collation != null) ? sqlParameter.Collation : this._defaultCollation);
										this.WriteUnsignedInt(sqlCollation.info, stateObj);
										stateObj.WriteByte(sqlCollation.sortId);
									}
									if (num4 == 0)
									{
										this.WriteParameterVarLen(internalMetaType, num3, flag3, stateObj, flag5);
									}
									else
									{
										this.WriteParameterVarLen(internalMetaType, num4, flag3, stateObj, flag5);
									}
									Task task = null;
									if (!flag3)
									{
										if (flag4)
										{
											task = this.WriteSqlValue(obj, internalMetaType, num3, num4, sqlParameter.Offset, stateObj);
										}
										else
										{
											task = this.WriteValue(obj, internalMetaType, sqlParameter.GetActualScale(), num3, num4, sqlParameter.Offset, stateObj, sqlParameter.Size, flag5);
										}
									}
									if (!sync)
									{
										if (task == null)
										{
											task = stateObj.WaitForAccumulatedWrites();
										}
										if (task != null)
										{
											Task task2 = null;
											if (completion == null)
											{
												completion = new TaskCompletionSource<object>();
												task2 = completion.Task;
											}
											Task task3 = task;
											TaskCompletionSource<object> completion2 = completion;
											Action action = delegate
											{
												this.TdsExecuteRPC(rpcArray, timeout, inSchema, notificationRequest, stateObj, isCommandProc, sync, completion, ii, i + 1);
											};
											SqlInternalConnectionTds connHandler = this._connHandler;
											Action<Exception> action2;
											if ((action2 = <>9__1) == null)
											{
												action2 = (<>9__1 = delegate(Exception exc)
												{
													this.TdsExecuteRPC_OnFailure(exc, stateObj);
												});
											}
											AsyncHelper.ContinueTask(task3, completion2, action, connHandler, action2, null, null, null);
											if (flag2)
											{
												Task task4 = task2;
												Action<Task> action3;
												if ((action3 = <>9__2) == null)
												{
													action3 = (<>9__2 = delegate(Task _)
													{
														this._connHandler._parserLock.Release();
													});
												}
												task4.ContinueWith(action3, TaskScheduler.Default);
												flag2 = false;
											}
											return task2;
										}
									}
								}
							}
							num6 = i;
						}
						if (ii < rpcArray.Length - 1)
						{
							stateObj.WriteByte(byte.MaxValue);
						}
						num6 = ii;
					}
					Task task5 = stateObj.ExecuteFlush();
					if (task5 != null)
					{
						Task task6 = null;
						if (completion == null)
						{
							completion = new TaskCompletionSource<object>();
							task6 = completion.Task;
						}
						bool taskReleaseConnectionLock = flag2;
						task5.ContinueWith(delegate(Task tsk)
						{
							this.ExecuteFlushTaskCallback(tsk, stateObj, completion, taskReleaseConnectionLock);
						}, TaskScheduler.Default);
						flag2 = false;
						return task6;
					}
				}
				catch (Exception ex)
				{
					if (!ADP.IsCatchableExceptionType(ex))
					{
						throw;
					}
					this.FailureCleanup(stateObj, ex);
					throw;
				}
				this.FinalizeExecuteRPC(stateObj);
				if (completion != null)
				{
					completion.SetResult(null);
				}
				task7 = null;
			}
			catch (Exception ex2)
			{
				this.FinalizeExecuteRPC(stateObj);
				if (completion == null)
				{
					throw;
				}
				completion.SetException(ex2);
				task7 = null;
			}
			finally
			{
				if (flag2)
				{
					this._connHandler._parserLock.Release();
				}
			}
			return task7;
		}

		private void FinalizeExecuteRPC(TdsParserStateObject stateObj)
		{
			stateObj.SniContext = SniContext.Snix_Read;
			this._asyncWrite = false;
		}

		private void TdsExecuteRPC_OnFailure(Exception exc, TdsParserStateObject stateObj)
		{
			this.FailureCleanup(stateObj, exc);
		}

		private void ExecuteFlushTaskCallback(Task tsk, TdsParserStateObject stateObj, TaskCompletionSource<object> completion, bool releaseConnectionLock)
		{
			try
			{
				this.FinalizeExecuteRPC(stateObj);
				if (tsk.Exception != null)
				{
					Exception innerException = tsk.Exception.InnerException;
					try
					{
						this.FailureCleanup(stateObj, tsk.Exception);
					}
					catch (Exception innerException)
					{
					}
					completion.SetException(innerException);
				}
				else
				{
					completion.SetResult(null);
				}
			}
			finally
			{
				if (releaseConnectionLock)
				{
					this._connHandler._parserLock.Release();
				}
			}
		}

		private void WriteParameterName(string parameterName, TdsParserStateObject stateObj)
		{
			if (!string.IsNullOrEmpty(parameterName))
			{
				int num = parameterName.Length & 255;
				stateObj.WriteByte((byte)num);
				this.WriteString(parameterName, num, 0, stateObj, true);
				return;
			}
			stateObj.WriteByte(0);
		}

		private void WriteSmiParameter(SqlParameter param, int paramIndex, bool sendDefault, TdsParserStateObject stateObj)
		{
			ParameterPeekAheadValue parameterPeekAheadValue;
			SmiParameterMetaData smiParameterMetaData = param.MetaDataForSmi(out parameterPeekAheadValue);
			if (!this._isKatmai)
			{
				throw ADP.VersionDoesNotSupportDataType(MetaType.GetMetaTypeFromSqlDbType(smiParameterMetaData.SqlDbType, smiParameterMetaData.IsMultiValued).TypeName);
			}
			object obj;
			ExtendedClrTypeCode extendedClrTypeCode;
			if (sendDefault)
			{
				if (SqlDbType.Structured == smiParameterMetaData.SqlDbType && smiParameterMetaData.IsMultiValued)
				{
					obj = TdsParser.s_tvpEmptyValue;
					extendedClrTypeCode = ExtendedClrTypeCode.IEnumerableOfSqlDataRecord;
				}
				else
				{
					obj = null;
					extendedClrTypeCode = ExtendedClrTypeCode.DBNull;
				}
			}
			else if (param.Direction == ParameterDirection.Output)
			{
				bool parameterIsSqlType = param.ParameterIsSqlType;
				param.Value = null;
				obj = null;
				extendedClrTypeCode = ExtendedClrTypeCode.DBNull;
				param.ParameterIsSqlType = parameterIsSqlType;
			}
			else
			{
				obj = param.GetCoercedValue();
				extendedClrTypeCode = MetaDataUtilsSmi.DetermineExtendedTypeCodeForUseWithSqlDbType(smiParameterMetaData.SqlDbType, smiParameterMetaData.IsMultiValued, obj);
			}
			this.WriteSmiParameterMetaData(smiParameterMetaData, sendDefault, stateObj);
			TdsParameterSetter tdsParameterSetter = new TdsParameterSetter(stateObj, smiParameterMetaData);
			ValueUtilsSmi.SetCompatibleValueV200(new SmiEventSink_Default(), tdsParameterSetter, 0, smiParameterMetaData, obj, extendedClrTypeCode, param.Offset, (0 < param.Size) ? param.Size : (-1), parameterPeekAheadValue);
		}

		private void WriteSmiParameterMetaData(SmiParameterMetaData metaData, bool sendDefault, TdsParserStateObject stateObj)
		{
			byte b = 0;
			if (ParameterDirection.Output == metaData.Direction || ParameterDirection.InputOutput == metaData.Direction)
			{
				b |= 1;
			}
			if (sendDefault)
			{
				b |= 2;
			}
			this.WriteParameterName(metaData.Name, stateObj);
			stateObj.WriteByte(b);
			this.WriteSmiTypeInfo(metaData, stateObj);
		}

		private void WriteSmiTypeInfo(SmiExtendedMetaData metaData, TdsParserStateObject stateObj)
		{
			checked
			{
				switch (metaData.SqlDbType)
				{
				case SqlDbType.BigInt:
					stateObj.WriteByte(38);
					stateObj.WriteByte((byte)metaData.MaxLength);
					return;
				case SqlDbType.Binary:
					stateObj.WriteByte(173);
					this.WriteUnsignedShort((ushort)metaData.MaxLength, stateObj);
					return;
				case SqlDbType.Bit:
					stateObj.WriteByte(104);
					stateObj.WriteByte((byte)metaData.MaxLength);
					return;
				case SqlDbType.Char:
					stateObj.WriteByte(175);
					this.WriteUnsignedShort((ushort)metaData.MaxLength, stateObj);
					this.WriteUnsignedInt(this._defaultCollation.info, stateObj);
					stateObj.WriteByte(this._defaultCollation.sortId);
					return;
				case SqlDbType.DateTime:
					stateObj.WriteByte(111);
					stateObj.WriteByte((byte)metaData.MaxLength);
					return;
				case SqlDbType.Decimal:
					stateObj.WriteByte(108);
					stateObj.WriteByte((byte)MetaType.MetaDecimal.FixedLength);
					stateObj.WriteByte((metaData.Precision == 0) ? 1 : metaData.Precision);
					stateObj.WriteByte(metaData.Scale);
					return;
				case SqlDbType.Float:
					stateObj.WriteByte(109);
					stateObj.WriteByte((byte)metaData.MaxLength);
					return;
				case SqlDbType.Image:
					stateObj.WriteByte(165);
					this.WriteUnsignedShort(ushort.MaxValue, stateObj);
					return;
				case SqlDbType.Int:
					stateObj.WriteByte(38);
					stateObj.WriteByte((byte)metaData.MaxLength);
					return;
				case SqlDbType.Money:
					stateObj.WriteByte(110);
					stateObj.WriteByte((byte)metaData.MaxLength);
					return;
				case SqlDbType.NChar:
					stateObj.WriteByte(239);
					this.WriteUnsignedShort((ushort)(metaData.MaxLength * 2L), stateObj);
					this.WriteUnsignedInt(this._defaultCollation.info, stateObj);
					stateObj.WriteByte(this._defaultCollation.sortId);
					return;
				case SqlDbType.NText:
					stateObj.WriteByte(231);
					this.WriteUnsignedShort(ushort.MaxValue, stateObj);
					this.WriteUnsignedInt(this._defaultCollation.info, stateObj);
					stateObj.WriteByte(this._defaultCollation.sortId);
					return;
				case SqlDbType.NVarChar:
					stateObj.WriteByte(231);
					if (-1L == metaData.MaxLength)
					{
						this.WriteUnsignedShort(ushort.MaxValue, stateObj);
					}
					else
					{
						this.WriteUnsignedShort((ushort)(metaData.MaxLength * 2L), stateObj);
					}
					this.WriteUnsignedInt(this._defaultCollation.info, stateObj);
					stateObj.WriteByte(this._defaultCollation.sortId);
					return;
				case SqlDbType.Real:
					stateObj.WriteByte(109);
					stateObj.WriteByte((byte)metaData.MaxLength);
					return;
				case SqlDbType.UniqueIdentifier:
					stateObj.WriteByte(36);
					stateObj.WriteByte((byte)metaData.MaxLength);
					return;
				case SqlDbType.SmallDateTime:
					stateObj.WriteByte(111);
					stateObj.WriteByte((byte)metaData.MaxLength);
					return;
				case SqlDbType.SmallInt:
					stateObj.WriteByte(38);
					stateObj.WriteByte((byte)metaData.MaxLength);
					return;
				case SqlDbType.SmallMoney:
					stateObj.WriteByte(110);
					stateObj.WriteByte((byte)metaData.MaxLength);
					return;
				case SqlDbType.Text:
					stateObj.WriteByte(167);
					this.WriteUnsignedShort(ushort.MaxValue, stateObj);
					this.WriteUnsignedInt(this._defaultCollation.info, stateObj);
					stateObj.WriteByte(this._defaultCollation.sortId);
					return;
				case SqlDbType.Timestamp:
					stateObj.WriteByte(173);
					this.WriteShort((int)metaData.MaxLength, stateObj);
					return;
				case SqlDbType.TinyInt:
					stateObj.WriteByte(38);
					stateObj.WriteByte((byte)metaData.MaxLength);
					return;
				case SqlDbType.VarBinary:
					stateObj.WriteByte(165);
					this.WriteUnsignedShort(unchecked((ushort)metaData.MaxLength), stateObj);
					return;
				case SqlDbType.VarChar:
					stateObj.WriteByte(167);
					this.WriteUnsignedShort(unchecked((ushort)metaData.MaxLength), stateObj);
					this.WriteUnsignedInt(this._defaultCollation.info, stateObj);
					stateObj.WriteByte(this._defaultCollation.sortId);
					return;
				case SqlDbType.Variant:
					stateObj.WriteByte(98);
					this.WriteInt((int)metaData.MaxLength, stateObj);
					return;
				case (SqlDbType)24:
				case (SqlDbType)26:
				case (SqlDbType)27:
				case (SqlDbType)28:
					break;
				case SqlDbType.Xml:
					stateObj.WriteByte(241);
					if (string.IsNullOrEmpty(metaData.TypeSpecificNamePart1) && string.IsNullOrEmpty(metaData.TypeSpecificNamePart2) && string.IsNullOrEmpty(metaData.TypeSpecificNamePart3))
					{
						stateObj.WriteByte(0);
						return;
					}
					stateObj.WriteByte(1);
					this.WriteIdentifier(metaData.TypeSpecificNamePart1, stateObj);
					this.WriteIdentifier(metaData.TypeSpecificNamePart2, stateObj);
					this.WriteIdentifierWithShortLength(metaData.TypeSpecificNamePart3, stateObj);
					return;
				case SqlDbType.Udt:
					stateObj.WriteByte(240);
					this.WriteIdentifier(metaData.TypeSpecificNamePart1, stateObj);
					this.WriteIdentifier(metaData.TypeSpecificNamePart2, stateObj);
					this.WriteIdentifier(metaData.TypeSpecificNamePart3, stateObj);
					return;
				case SqlDbType.Structured:
					if (metaData.IsMultiValued)
					{
						this.WriteTvpTypeInfo(metaData, stateObj);
						return;
					}
					break;
				case SqlDbType.Date:
					stateObj.WriteByte(40);
					return;
				case SqlDbType.Time:
					stateObj.WriteByte(41);
					stateObj.WriteByte(metaData.Scale);
					return;
				case SqlDbType.DateTime2:
					stateObj.WriteByte(42);
					stateObj.WriteByte(metaData.Scale);
					return;
				case SqlDbType.DateTimeOffset:
					stateObj.WriteByte(43);
					stateObj.WriteByte(metaData.Scale);
					break;
				default:
					return;
				}
			}
		}

		private void WriteTvpTypeInfo(SmiExtendedMetaData metaData, TdsParserStateObject stateObj)
		{
			stateObj.WriteByte(243);
			this.WriteIdentifier(metaData.TypeSpecificNamePart1, stateObj);
			this.WriteIdentifier(metaData.TypeSpecificNamePart2, stateObj);
			this.WriteIdentifier(metaData.TypeSpecificNamePart3, stateObj);
			if (metaData.FieldMetaData.Count == 0)
			{
				this.WriteUnsignedShort(ushort.MaxValue, stateObj);
			}
			else
			{
				this.WriteUnsignedShort(checked((ushort)metaData.FieldMetaData.Count), stateObj);
				SmiDefaultFieldsProperty smiDefaultFieldsProperty = (SmiDefaultFieldsProperty)metaData.ExtendedProperties[SmiPropertySelector.DefaultFields];
				for (int i = 0; i < metaData.FieldMetaData.Count; i++)
				{
					this.WriteTvpColumnMetaData(metaData.FieldMetaData[i], smiDefaultFieldsProperty[i], stateObj);
				}
				this.WriteTvpOrderUnique(metaData, stateObj);
			}
			stateObj.WriteByte(0);
		}

		private void WriteTvpColumnMetaData(SmiExtendedMetaData md, bool isDefault, TdsParserStateObject stateObj)
		{
			if (SqlDbType.Timestamp == md.SqlDbType)
			{
				this.WriteUnsignedInt(80U, stateObj);
			}
			else
			{
				this.WriteUnsignedInt(0U, stateObj);
			}
			ushort num = 1;
			if (isDefault)
			{
				num |= 512;
			}
			this.WriteUnsignedShort(num, stateObj);
			this.WriteSmiTypeInfo(md, stateObj);
			this.WriteIdentifier(null, stateObj);
		}

		private void WriteTvpOrderUnique(SmiExtendedMetaData metaData, TdsParserStateObject stateObj)
		{
			SmiOrderProperty smiOrderProperty = (SmiOrderProperty)metaData.ExtendedProperties[SmiPropertySelector.SortOrder];
			SmiUniqueKeyProperty smiUniqueKeyProperty = (SmiUniqueKeyProperty)metaData.ExtendedProperties[SmiPropertySelector.UniqueKey];
			List<TdsParser.TdsOrderUnique> list = new List<TdsParser.TdsOrderUnique>(metaData.FieldMetaData.Count);
			for (int i = 0; i < metaData.FieldMetaData.Count; i++)
			{
				byte b = 0;
				SmiOrderProperty.SmiColumnOrder smiColumnOrder = smiOrderProperty[i];
				if (smiColumnOrder.Order == SortOrder.Ascending)
				{
					b = 1;
				}
				else if (SortOrder.Descending == smiColumnOrder.Order)
				{
					b = 2;
				}
				if (smiUniqueKeyProperty[i])
				{
					b |= 4;
				}
				if (b != 0)
				{
					list.Add(new TdsParser.TdsOrderUnique(checked((short)(i + 1)), b));
				}
			}
			if (0 < list.Count)
			{
				stateObj.WriteByte(16);
				this.WriteShort(list.Count, stateObj);
				foreach (TdsParser.TdsOrderUnique tdsOrderUnique in list)
				{
					this.WriteShort((int)tdsOrderUnique.ColumnOrdinal, stateObj);
					stateObj.WriteByte(tdsOrderUnique.Flags);
				}
			}
		}

		internal Task WriteBulkCopyDone(TdsParserStateObject stateObj)
		{
			if (this.State != TdsParserState.OpenNotLoggedIn && this.State != TdsParserState.OpenLoggedIn)
			{
				throw ADP.ClosedConnectionError();
			}
			stateObj.WriteByte(253);
			this.WriteShort(0, stateObj);
			this.WriteShort(0, stateObj);
			this.WriteInt(0, stateObj);
			stateObj._pendingData = true;
			stateObj._messageStatus = 0;
			return stateObj.WritePacket(1, false);
		}

		internal void WriteBulkCopyMetaData(_SqlMetaDataSet metadataCollection, int count, TdsParserStateObject stateObj)
		{
			if (this.State != TdsParserState.OpenNotLoggedIn && this.State != TdsParserState.OpenLoggedIn)
			{
				throw ADP.ClosedConnectionError();
			}
			stateObj.WriteByte(129);
			this.WriteShort(count, stateObj);
			for (int i = 0; i < metadataCollection.Length; i++)
			{
				if (metadataCollection[i] != null)
				{
					_SqlMetaData sqlMetaData = metadataCollection[i];
					this.WriteInt(0, stateObj);
					ushort num = (ushort)(sqlMetaData.updatability << 2);
					num |= (sqlMetaData.isNullable ? 1 : 0);
					num |= (sqlMetaData.isIdentity ? 16 : 0);
					this.WriteShort((int)num, stateObj);
					SqlDbType type = sqlMetaData.type;
					if (type != SqlDbType.Decimal)
					{
						switch (type)
						{
						case SqlDbType.Xml:
							stateObj.WriteByteArray(TdsParser.s_xmlMetadataSubstituteSequence, TdsParser.s_xmlMetadataSubstituteSequence.Length, 0, true, null);
							goto IL_01A6;
						case SqlDbType.Udt:
							throw ADP.DbTypeNotSupported(SqlDbType.Udt.ToString());
						case SqlDbType.Date:
							stateObj.WriteByte(sqlMetaData.tdsType);
							goto IL_01A6;
						case SqlDbType.Time:
						case SqlDbType.DateTime2:
						case SqlDbType.DateTimeOffset:
							stateObj.WriteByte(sqlMetaData.tdsType);
							stateObj.WriteByte(sqlMetaData.scale);
							goto IL_01A6;
						}
						stateObj.WriteByte(sqlMetaData.tdsType);
						this.WriteTokenLength(sqlMetaData.tdsType, sqlMetaData.length, stateObj);
						if (sqlMetaData.metaType.IsCharType)
						{
							this.WriteUnsignedInt(sqlMetaData.collation.info, stateObj);
							stateObj.WriteByte(sqlMetaData.collation.sortId);
						}
					}
					else
					{
						stateObj.WriteByte(sqlMetaData.tdsType);
						this.WriteTokenLength(sqlMetaData.tdsType, sqlMetaData.length, stateObj);
						stateObj.WriteByte(sqlMetaData.precision);
						stateObj.WriteByte(sqlMetaData.scale);
					}
					IL_01A6:
					if (sqlMetaData.metaType.IsLong && !sqlMetaData.metaType.IsPlp)
					{
						this.WriteShort(sqlMetaData.tableName.Length, stateObj);
						this.WriteString(sqlMetaData.tableName, stateObj, true);
					}
					stateObj.WriteByte((byte)sqlMetaData.column.Length);
					this.WriteString(sqlMetaData.column, stateObj, true);
				}
			}
		}

		internal Task WriteBulkCopyValue(object value, SqlMetaDataPriv metadata, TdsParserStateObject stateObj, bool isSqlType, bool isDataFeed, bool isNull)
		{
			Encoding defaultEncoding = this._defaultEncoding;
			SqlCollation defaultCollation = this._defaultCollation;
			int defaultCodePage = this._defaultCodePage;
			int defaultLCID = this._defaultLCID;
			Task task = null;
			Task task2 = null;
			if (this.State != TdsParserState.OpenNotLoggedIn && this.State != TdsParserState.OpenLoggedIn)
			{
				throw ADP.ClosedConnectionError();
			}
			try
			{
				if (metadata.encoding != null)
				{
					this._defaultEncoding = metadata.encoding;
				}
				if (metadata.collation != null)
				{
					this._defaultCollation = metadata.collation;
					this._defaultLCID = this._defaultCollation.LCID;
				}
				this._defaultCodePage = metadata.codePage;
				MetaType metaType = metadata.metaType;
				int num = 0;
				int num2 = 0;
				if (isNull)
				{
					if (metaType.IsPlp && (metaType.NullableType != 240 || metaType.IsLong))
					{
						this.WriteLong(-1L, stateObj);
					}
					else if (!metaType.IsFixed && !metaType.IsLong && !metaType.IsVarTime)
					{
						this.WriteShort(65535, stateObj);
					}
					else
					{
						stateObj.WriteByte(0);
					}
					return task;
				}
				if (!isDataFeed)
				{
					byte nullableType = metaType.NullableType;
					if (nullableType <= 167)
					{
						if (nullableType <= 99)
						{
							switch (nullableType)
							{
							case 34:
								break;
							case 35:
								goto IL_01C7;
							case 36:
								num = 16;
								goto IL_0285;
							default:
								if (nullableType != 99)
								{
									goto IL_027D;
								}
								goto IL_0212;
							}
						}
						else if (nullableType != 165)
						{
							if (nullableType != 167)
							{
								goto IL_027D;
							}
							goto IL_01C7;
						}
					}
					else if (nullableType <= 175)
					{
						if (nullableType != 173)
						{
							if (nullableType != 175)
							{
								goto IL_027D;
							}
							goto IL_01C7;
						}
					}
					else
					{
						if (nullableType == 231)
						{
							goto IL_0212;
						}
						switch (nullableType)
						{
						case 239:
							goto IL_0212;
						case 240:
							break;
						case 241:
							if (value is XmlReader)
							{
								value = MetaType.GetStringFromXml((XmlReader)value);
							}
							num = (isSqlType ? ((SqlString)value).Value.Length : ((string)value).Length) * 2;
							goto IL_0285;
						default:
							goto IL_027D;
						}
					}
					num = (isSqlType ? ((SqlBinary)value).Length : ((byte[])value).Length);
					goto IL_0285;
					IL_01C7:
					if (this._defaultEncoding == null)
					{
						this.ThrowUnsupportedCollationEncountered(null);
					}
					string text;
					if (isSqlType)
					{
						text = ((SqlString)value).Value;
					}
					else
					{
						text = (string)value;
					}
					num = text.Length;
					num2 = this._defaultEncoding.GetByteCount(text);
					goto IL_0285;
					IL_0212:
					num = (isSqlType ? ((SqlString)value).Value.Length : ((string)value).Length) * 2;
					goto IL_0285;
					IL_027D:
					num = metadata.length;
				}
				IL_0285:
				if (metaType.IsLong)
				{
					SqlDbType sqlDbType = metaType.SqlDbType;
					if (sqlDbType <= SqlDbType.NVarChar)
					{
						if (sqlDbType != SqlDbType.Image && sqlDbType != SqlDbType.NText)
						{
							if (sqlDbType != SqlDbType.NVarChar)
							{
								goto IL_0329;
							}
							goto IL_0306;
						}
					}
					else if (sqlDbType <= SqlDbType.VarChar)
					{
						if (sqlDbType != SqlDbType.Text)
						{
							if (sqlDbType - SqlDbType.VarBinary > 1)
							{
								goto IL_0329;
							}
							goto IL_0306;
						}
					}
					else
					{
						if (sqlDbType != SqlDbType.Xml && sqlDbType != SqlDbType.Udt)
						{
							goto IL_0329;
						}
						goto IL_0306;
					}
					stateObj.WriteByteArray(TdsParser.s_longDataHeader, TdsParser.s_longDataHeader.Length, 0, true, null);
					this.WriteTokenLength(metadata.tdsType, (num2 == 0) ? num : num2, stateObj);
					goto IL_0329;
					IL_0306:
					this.WriteUnsignedLong(18446744073709551614UL, stateObj);
				}
				else
				{
					this.WriteTokenLength(metadata.tdsType, (num2 == 0) ? num : num2, stateObj);
				}
				IL_0329:
				if (isSqlType)
				{
					task2 = this.WriteSqlValue(value, metaType, num, num2, 0, stateObj);
				}
				else if (metaType.SqlDbType != SqlDbType.Udt || metaType.IsLong)
				{
					task2 = this.WriteValue(value, metaType, metadata.scale, num, num2, 0, stateObj, metadata.length, isDataFeed);
					if (task2 == null && this._asyncWrite)
					{
						task2 = stateObj.WaitForAccumulatedWrites();
					}
				}
				else
				{
					this.WriteShort(num, stateObj);
					task2 = stateObj.WriteByteArray((byte[])value, num, 0, true, null);
				}
				if (task2 != null)
				{
					task = this.WriteBulkCopyValueSetupContinuation(task2, defaultEncoding, defaultCollation, defaultCodePage, defaultLCID);
				}
			}
			finally
			{
				if (task2 == null)
				{
					this._defaultEncoding = defaultEncoding;
					this._defaultCollation = defaultCollation;
					this._defaultCodePage = defaultCodePage;
					this._defaultLCID = defaultLCID;
				}
			}
			return task;
		}

		private Task WriteBulkCopyValueSetupContinuation(Task internalWriteTask, Encoding saveEncoding, SqlCollation saveCollation, int saveCodePage, int saveLCID)
		{
			return internalWriteTask.ContinueWith<Task>(delegate(Task t)
			{
				this._defaultEncoding = saveEncoding;
				this._defaultCollation = saveCollation;
				this._defaultCodePage = saveCodePage;
				this._defaultLCID = saveLCID;
				return t;
			}, TaskScheduler.Default).Unwrap();
		}

		private void WriteMarsHeaderData(TdsParserStateObject stateObj, SqlInternalTransaction transaction)
		{
			this.WriteShort(2, stateObj);
			if (transaction != null && transaction.TransactionId != 0L)
			{
				this.WriteLong(transaction.TransactionId, stateObj);
				this.WriteInt(stateObj.IncrementAndObtainOpenResultCount(transaction), stateObj);
				return;
			}
			this.WriteLong(0L, stateObj);
			this.WriteInt(stateObj.IncrementAndObtainOpenResultCount(null), stateObj);
		}

		private int GetNotificationHeaderSize(SqlNotificationRequest notificationRequest)
		{
			if (notificationRequest == null)
			{
				return 0;
			}
			string userData = notificationRequest.UserData;
			string options = notificationRequest.Options;
			int timeout = notificationRequest.Timeout;
			if (userData == null)
			{
				throw ADP.ArgumentNull("callbackId");
			}
			if (65535 < userData.Length)
			{
				throw ADP.ArgumentOutOfRange("callbackId");
			}
			if (options == null)
			{
				throw ADP.ArgumentNull("service");
			}
			if (65535 < options.Length)
			{
				throw ADP.ArgumentOutOfRange("service");
			}
			if (-1 > timeout)
			{
				throw ADP.ArgumentOutOfRange("timeout");
			}
			int num = 8 + userData.Length * 2 + 2 + options.Length * 2;
			if (timeout > 0)
			{
				num += 4;
			}
			return num;
		}

		private void WriteQueryNotificationHeaderData(SqlNotificationRequest notificationRequest, TdsParserStateObject stateObj)
		{
			string userData = notificationRequest.UserData;
			string options = notificationRequest.Options;
			int timeout = notificationRequest.Timeout;
			this.WriteShort(1, stateObj);
			this.WriteShort(userData.Length * 2, stateObj);
			this.WriteString(userData, stateObj, true);
			this.WriteShort(options.Length * 2, stateObj);
			this.WriteString(options, stateObj, true);
			if (timeout > 0)
			{
				this.WriteInt(timeout, stateObj);
			}
		}

		private void WriteRPCBatchHeaders(TdsParserStateObject stateObj, SqlNotificationRequest notificationRequest)
		{
			int notificationHeaderSize = this.GetNotificationHeaderSize(notificationRequest);
			int num = 22 + notificationHeaderSize;
			this.WriteInt(num, stateObj);
			this.WriteInt(18, stateObj);
			this.WriteMarsHeaderData(stateObj, this.CurrentTransaction);
			if (notificationHeaderSize != 0)
			{
				this.WriteInt(notificationHeaderSize, stateObj);
				this.WriteQueryNotificationHeaderData(notificationRequest, stateObj);
			}
		}

		private void WriteTokenLength(byte token, int length, TdsParserStateObject stateObj)
		{
			int num = 0;
			if (240 == token)
			{
				num = 8;
			}
			else if (token == 241)
			{
				num = 8;
			}
			if (num == 0)
			{
				int num2 = (int)(token & 48);
				if (num2 <= 16)
				{
					if (num2 != 0)
					{
						if (num2 != 16)
						{
							goto IL_005D;
						}
						num = 0;
						goto IL_005D;
					}
				}
				else if (num2 != 32)
				{
					if (num2 == 48)
					{
						num = 0;
						goto IL_005D;
					}
					goto IL_005D;
				}
				if ((token & 128) != 0)
				{
					num = 2;
				}
				else if ((token & 12) == 0)
				{
					num = 4;
				}
				else
				{
					num = 1;
				}
				IL_005D:
				switch (num)
				{
				case 1:
					stateObj.WriteByte((byte)length);
					return;
				case 2:
					this.WriteShort(length, stateObj);
					return;
				case 3:
					break;
				case 4:
					this.WriteInt(length, stateObj);
					return;
				default:
					if (num != 8)
					{
						return;
					}
					this.WriteShort(65535, stateObj);
					break;
				}
			}
		}

		private bool IsBOMNeeded(MetaType type, object value)
		{
			if (type.NullableType == 241)
			{
				Type type2 = value.GetType();
				if (type2 == typeof(SqlString))
				{
					if (!((SqlString)value).IsNull && ((SqlString)value).Value.Length > 0 && (((SqlString)value).Value[0] & 'ÿ') != 'ÿ')
					{
						return true;
					}
				}
				else if (type2 == typeof(string) && ((string)value).Length > 0)
				{
					if (value != null && (((string)value)[0] & 'ÿ') != 'ÿ')
					{
						return true;
					}
				}
				else if (type2 == typeof(SqlXml))
				{
					if (!((SqlXml)value).IsNull)
					{
						return true;
					}
				}
				else if (type2 == typeof(XmlDataFeed))
				{
					return true;
				}
			}
			return false;
		}

		private Task GetTerminationTask(Task unterminatedWriteTask, object value, MetaType type, int actualLength, TdsParserStateObject stateObj, bool isDataFeed)
		{
			if (!type.IsPlp || (actualLength <= 0 && !isDataFeed))
			{
				return unterminatedWriteTask;
			}
			if (unterminatedWriteTask == null)
			{
				this.WriteInt(0, stateObj);
				return null;
			}
			return AsyncHelper.CreateContinuationTask<int, TdsParserStateObject>(unterminatedWriteTask, new Action<int, TdsParserStateObject>(this.WriteInt), 0, stateObj, this._connHandler, null);
		}

		private Task WriteSqlValue(object value, MetaType type, int actualLength, int codePageByteSize, int offset, TdsParserStateObject stateObj)
		{
			return this.GetTerminationTask(this.WriteUnterminatedSqlValue(value, type, actualLength, codePageByteSize, offset, stateObj), value, type, actualLength, stateObj, false);
		}

		private Task WriteUnterminatedSqlValue(object value, MetaType type, int actualLength, int codePageByteSize, int offset, TdsParserStateObject stateObj)
		{
			byte nullableType = type.NullableType;
			if (nullableType <= 165)
			{
				if (nullableType <= 99)
				{
					switch (nullableType)
					{
					case 34:
						break;
					case 35:
						goto IL_022F;
					case 36:
					{
						byte[] array = ((SqlGuid)value).ToByteArray();
						stateObj.WriteByteArray(array, actualLength, 0, true, null);
						goto IL_03CC;
					}
					case 37:
						goto IL_03CC;
					case 38:
						if (type.FixedLength == 1)
						{
							stateObj.WriteByte(((SqlByte)value).Value);
							goto IL_03CC;
						}
						if (type.FixedLength == 2)
						{
							this.WriteShort((int)((SqlInt16)value).Value, stateObj);
							goto IL_03CC;
						}
						if (type.FixedLength == 4)
						{
							this.WriteInt(((SqlInt32)value).Value, stateObj);
							goto IL_03CC;
						}
						this.WriteLong(((SqlInt64)value).Value, stateObj);
						goto IL_03CC;
					default:
						if (nullableType != 99)
						{
							goto IL_03CC;
						}
						goto IL_0292;
					}
				}
				else
				{
					switch (nullableType)
					{
					case 104:
						if (((SqlBoolean)value).Value)
						{
							stateObj.WriteByte(1);
							goto IL_03CC;
						}
						stateObj.WriteByte(0);
						goto IL_03CC;
					case 105:
					case 106:
					case 107:
						goto IL_03CC;
					case 108:
						this.WriteSqlDecimal((SqlDecimal)value, stateObj);
						goto IL_03CC;
					case 109:
						if (type.FixedLength == 4)
						{
							this.WriteFloat(((SqlSingle)value).Value, stateObj);
							goto IL_03CC;
						}
						this.WriteDouble(((SqlDouble)value).Value, stateObj);
						goto IL_03CC;
					case 110:
						this.WriteSqlMoney((SqlMoney)value, type.FixedLength, stateObj);
						goto IL_03CC;
					case 111:
					{
						SqlDateTime sqlDateTime = (SqlDateTime)value;
						if (type.FixedLength != 4)
						{
							this.WriteInt(sqlDateTime.DayTicks, stateObj);
							this.WriteInt(sqlDateTime.TimeTicks, stateObj);
							goto IL_03CC;
						}
						if (0 > sqlDateTime.DayTicks || sqlDateTime.DayTicks > 65535)
						{
							throw SQL.SmallDateTimeOverflow(sqlDateTime.ToString());
						}
						this.WriteShort(sqlDateTime.DayTicks, stateObj);
						this.WriteShort(sqlDateTime.TimeTicks / SqlDateTime.SQLTicksPerMinute, stateObj);
						goto IL_03CC;
					}
					default:
						if (nullableType != 165)
						{
							goto IL_03CC;
						}
						break;
					}
				}
			}
			else if (nullableType <= 173)
			{
				if (nullableType == 167)
				{
					goto IL_022F;
				}
				if (nullableType != 173)
				{
					goto IL_03CC;
				}
			}
			else
			{
				if (nullableType == 175)
				{
					goto IL_022F;
				}
				if (nullableType == 231)
				{
					goto IL_0292;
				}
				switch (nullableType)
				{
				case 239:
				case 241:
					goto IL_0292;
				case 240:
					throw ADP.DbTypeNotSupported(SqlDbType.Udt.ToString());
				default:
					goto IL_03CC;
				}
			}
			if (type.IsPlp)
			{
				this.WriteInt(actualLength, stateObj);
			}
			if (value is SqlBinary)
			{
				return stateObj.WriteByteArray(((SqlBinary)value).Value, actualLength, offset, false, null);
			}
			return stateObj.WriteByteArray(((SqlBytes)value).Value, actualLength, offset, false, null);
			IL_022F:
			if (type.IsPlp)
			{
				this.WriteInt(codePageByteSize, stateObj);
			}
			if (value is SqlChars)
			{
				string text = new string(((SqlChars)value).Value);
				return this.WriteEncodingChar(text, actualLength, offset, this._defaultEncoding, stateObj, false);
			}
			return this.WriteEncodingChar(((SqlString)value).Value, actualLength, offset, this._defaultEncoding, stateObj, false);
			IL_0292:
			if (type.IsPlp)
			{
				if (this.IsBOMNeeded(type, value))
				{
					this.WriteInt(actualLength + 2, stateObj);
					this.WriteShort(65279, stateObj);
				}
				else
				{
					this.WriteInt(actualLength, stateObj);
				}
			}
			if (actualLength != 0)
			{
				actualLength >>= 1;
			}
			if (value is SqlChars)
			{
				return this.WriteCharArray(((SqlChars)value).Value, actualLength, offset, stateObj, false);
			}
			return this.WriteString(((SqlString)value).Value, actualLength, offset, stateObj, false);
			IL_03CC:
			return null;
		}

		private async Task WriteXmlFeed(XmlDataFeed feed, TdsParserStateObject stateObj, bool needBom, Encoding encoding, int size)
		{
			byte[] array = null;
			if (!needBom)
			{
				array = encoding.GetPreamble();
			}
			TdsParser.ConstrainedTextWriter writer = new TdsParser.ConstrainedTextWriter(new StreamWriter(new TdsParser.TdsOutputStream(this, stateObj, array), encoding), size);
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.CloseOutput = false;
			xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
			if (this._asyncWrite)
			{
				xmlWriterSettings.Async = true;
			}
			XmlWriter ww = XmlWriter.Create(writer, xmlWriterSettings);
			if (feed._source.ReadState == ReadState.Initial)
			{
				feed._source.Read();
			}
			while (!feed._source.EOF && !writer.IsComplete)
			{
				if (feed._source.NodeType == XmlNodeType.XmlDeclaration)
				{
					feed._source.Read();
				}
				else if (this._asyncWrite)
				{
					await ww.WriteNodeAsync(feed._source, true).ConfigureAwait(false);
				}
				else
				{
					ww.WriteNode(feed._source, true);
				}
			}
			if (this._asyncWrite)
			{
				await ww.FlushAsync().ConfigureAwait(false);
			}
			else
			{
				ww.Flush();
			}
		}

		private async Task WriteTextFeed(TextDataFeed feed, Encoding encoding, bool needBom, TdsParserStateObject stateObj, int size)
		{
			char[] inBuff = new char[4096];
			encoding = encoding ?? new UnicodeEncoding(false, false);
			TdsParser.ConstrainedTextWriter writer = new TdsParser.ConstrainedTextWriter(new StreamWriter(new TdsParser.TdsOutputStream(this, stateObj, null), encoding), size);
			if (needBom)
			{
				if (this._asyncWrite)
				{
					await writer.WriteAsync('\ufeff').ConfigureAwait(false);
				}
				else
				{
					writer.Write('\ufeff');
				}
			}
			int nWritten = 0;
			do
			{
				int nRead = 0;
				if (this._asyncWrite)
				{
					nRead = await feed._source.ReadBlockAsync(inBuff, 0, 4096).ConfigureAwait(false);
				}
				else
				{
					nRead = feed._source.ReadBlock(inBuff, 0, 4096);
				}
				if (nRead == 0)
				{
					break;
				}
				if (this._asyncWrite)
				{
					await writer.WriteAsync(inBuff, 0, nRead).ConfigureAwait(false);
				}
				else
				{
					writer.Write(inBuff, 0, nRead);
				}
				nWritten += nRead;
			}
			while (!writer.IsComplete);
			if (this._asyncWrite)
			{
				await writer.FlushAsync().ConfigureAwait(false);
			}
			else
			{
				writer.Flush();
			}
		}

		private async Task WriteStreamFeed(StreamDataFeed feed, TdsParserStateObject stateObj, int len)
		{
			TdsParser.TdsOutputStream output = new TdsParser.TdsOutputStream(this, stateObj, null);
			byte[] buff = new byte[4096];
			int nWritten = 0;
			do
			{
				int nRead = 0;
				int readSize = 4096;
				if (len > 0 && nWritten + readSize > len)
				{
					readSize = len - nWritten;
				}
				if (this._asyncWrite)
				{
					int num = await feed._source.ReadAsync(buff, 0, readSize).ConfigureAwait(false);
					nRead = num;
				}
				else
				{
					nRead = feed._source.Read(buff, 0, readSize);
				}
				if (nRead == 0)
				{
					break;
				}
				if (this._asyncWrite)
				{
					await output.WriteAsync(buff, 0, nRead).ConfigureAwait(false);
				}
				else
				{
					output.Write(buff, 0, nRead);
				}
				nWritten += nRead;
			}
			while (len <= 0 || nWritten < len);
		}

		private Task NullIfCompletedWriteTask(Task task)
		{
			if (task == null)
			{
				return null;
			}
			switch (task.Status)
			{
			case TaskStatus.RanToCompletion:
				return null;
			case TaskStatus.Canceled:
				throw SQL.OperationCancelled();
			case TaskStatus.Faulted:
				throw task.Exception.InnerException;
			default:
				return task;
			}
		}

		private Task WriteValue(object value, MetaType type, byte scale, int actualLength, int encodingByteSize, int offset, TdsParserStateObject stateObj, int paramSize, bool isDataFeed)
		{
			return this.GetTerminationTask(this.WriteUnterminatedValue(value, type, scale, actualLength, encodingByteSize, offset, stateObj, paramSize, isDataFeed), value, type, actualLength, stateObj, isDataFeed);
		}

		private Task WriteUnterminatedValue(object value, MetaType type, byte scale, int actualLength, int encodingByteSize, int offset, TdsParserStateObject stateObj, int paramSize, bool isDataFeed)
		{
			byte nullableType = type.NullableType;
			if (nullableType <= 165)
			{
				if (nullableType <= 99)
				{
					switch (nullableType)
					{
					case 34:
						break;
					case 35:
						goto IL_01F8;
					case 36:
					{
						byte[] array = ((Guid)value).ToByteArray();
						stateObj.WriteByteArray(array, actualLength, 0, true, null);
						goto IL_0460;
					}
					case 37:
					case 39:
						goto IL_0460;
					case 38:
						if (type.FixedLength == 1)
						{
							stateObj.WriteByte((byte)value);
							goto IL_0460;
						}
						if (type.FixedLength == 2)
						{
							this.WriteShort((int)((short)value), stateObj);
							goto IL_0460;
						}
						if (type.FixedLength == 4)
						{
							this.WriteInt((int)value, stateObj);
							goto IL_0460;
						}
						this.WriteLong((long)value, stateObj);
						goto IL_0460;
					case 40:
						this.WriteDate((DateTime)value, stateObj);
						goto IL_0460;
					case 41:
						if (scale > 7)
						{
							throw SQL.TimeScaleValueOutOfRange(scale);
						}
						this.WriteTime((TimeSpan)value, scale, actualLength, stateObj);
						goto IL_0460;
					case 42:
						if (scale > 7)
						{
							throw SQL.TimeScaleValueOutOfRange(scale);
						}
						this.WriteDateTime2((DateTime)value, scale, actualLength, stateObj);
						goto IL_0460;
					case 43:
						this.WriteDateTimeOffset((DateTimeOffset)value, scale, actualLength, stateObj);
						goto IL_0460;
					default:
						if (nullableType != 99)
						{
							goto IL_0460;
						}
						goto IL_0287;
					}
				}
				else
				{
					switch (nullableType)
					{
					case 104:
						if ((bool)value)
						{
							stateObj.WriteByte(1);
							goto IL_0460;
						}
						stateObj.WriteByte(0);
						goto IL_0460;
					case 105:
					case 106:
					case 107:
						goto IL_0460;
					case 108:
						this.WriteDecimal((decimal)value, stateObj);
						goto IL_0460;
					case 109:
						if (type.FixedLength == 4)
						{
							this.WriteFloat((float)value, stateObj);
							goto IL_0460;
						}
						this.WriteDouble((double)value, stateObj);
						goto IL_0460;
					case 110:
						this.WriteCurrency((decimal)value, type.FixedLength, stateObj);
						goto IL_0460;
					case 111:
					{
						TdsDateTime tdsDateTime = MetaType.FromDateTime((DateTime)value, (byte)type.FixedLength);
						if (type.FixedLength != 4)
						{
							this.WriteInt(tdsDateTime.days, stateObj);
							this.WriteInt(tdsDateTime.time, stateObj);
							goto IL_0460;
						}
						if (0 > tdsDateTime.days || tdsDateTime.days > 65535)
						{
							throw SQL.SmallDateTimeOverflow(MetaType.ToDateTime(tdsDateTime.days, tdsDateTime.time, 4).ToString(CultureInfo.InvariantCulture));
						}
						this.WriteShort(tdsDateTime.days, stateObj);
						this.WriteShort(tdsDateTime.time, stateObj);
						goto IL_0460;
					}
					default:
						if (nullableType != 165)
						{
							goto IL_0460;
						}
						break;
					}
				}
			}
			else if (nullableType <= 173)
			{
				if (nullableType == 167)
				{
					goto IL_01F8;
				}
				if (nullableType != 173)
				{
					goto IL_0460;
				}
			}
			else
			{
				if (nullableType == 175)
				{
					goto IL_01F8;
				}
				if (nullableType == 231)
				{
					goto IL_0287;
				}
				switch (nullableType)
				{
				case 239:
				case 241:
					goto IL_0287;
				case 240:
					break;
				default:
					goto IL_0460;
				}
			}
			if (isDataFeed)
			{
				return this.NullIfCompletedWriteTask(this.WriteStreamFeed((StreamDataFeed)value, stateObj, paramSize));
			}
			if (type.IsPlp)
			{
				this.WriteInt(actualLength, stateObj);
			}
			return stateObj.WriteByteArray((byte[])value, actualLength, offset, false, null);
			IL_01F8:
			if (isDataFeed)
			{
				TextDataFeed textDataFeed = value as TextDataFeed;
				if (textDataFeed == null)
				{
					return this.NullIfCompletedWriteTask(this.WriteXmlFeed((XmlDataFeed)value, stateObj, true, this._defaultEncoding, paramSize));
				}
				return this.NullIfCompletedWriteTask(this.WriteTextFeed(textDataFeed, this._defaultEncoding, false, stateObj, paramSize));
			}
			else
			{
				if (type.IsPlp)
				{
					this.WriteInt(encodingByteSize, stateObj);
				}
				if (value is byte[])
				{
					return stateObj.WriteByteArray((byte[])value, actualLength, 0, false, null);
				}
				return this.WriteEncodingChar((string)value, actualLength, offset, this._defaultEncoding, stateObj, false);
			}
			IL_0287:
			if (isDataFeed)
			{
				TextDataFeed textDataFeed2 = value as TextDataFeed;
				if (textDataFeed2 == null)
				{
					return this.NullIfCompletedWriteTask(this.WriteXmlFeed((XmlDataFeed)value, stateObj, this.IsBOMNeeded(type, value), Encoding.Unicode, paramSize));
				}
				return this.NullIfCompletedWriteTask(this.WriteTextFeed(textDataFeed2, null, this.IsBOMNeeded(type, value), stateObj, paramSize));
			}
			else
			{
				if (type.IsPlp)
				{
					if (this.IsBOMNeeded(type, value))
					{
						this.WriteInt(actualLength + 2, stateObj);
						this.WriteShort(65279, stateObj);
					}
					else
					{
						this.WriteInt(actualLength, stateObj);
					}
				}
				if (value is byte[])
				{
					return stateObj.WriteByteArray((byte[])value, actualLength, 0, false, null);
				}
				actualLength >>= 1;
				return this.WriteString((string)value, actualLength, offset, stateObj, false);
			}
			IL_0460:
			return null;
		}

		internal void WriteParameterVarLen(MetaType type, int size, bool isNull, TdsParserStateObject stateObj, bool unknownLength = false)
		{
			if (type.IsLong)
			{
				if (isNull)
				{
					if (type.IsPlp)
					{
						this.WriteLong(-1L, stateObj);
						return;
					}
					this.WriteInt(-1, stateObj);
					return;
				}
				else
				{
					if (type.NullableType == 241 || unknownLength)
					{
						this.WriteUnsignedLong(18446744073709551614UL, stateObj);
						return;
					}
					if (type.IsPlp)
					{
						this.WriteLong((long)size, stateObj);
						return;
					}
					this.WriteInt(size, stateObj);
					return;
				}
			}
			else if (type.IsVarTime)
			{
				if (isNull)
				{
					stateObj.WriteByte(0);
					return;
				}
				stateObj.WriteByte((byte)size);
				return;
			}
			else if (!type.IsFixed)
			{
				if (isNull)
				{
					this.WriteShort(65535, stateObj);
					return;
				}
				this.WriteShort(size, stateObj);
				return;
			}
			else
			{
				if (isNull)
				{
					stateObj.WriteByte(0);
					return;
				}
				stateObj.WriteByte((byte)(type.FixedLength & 255));
				return;
			}
		}

		private bool TryReadPlpUnicodeCharsChunk(char[] buff, int offst, int len, TdsParserStateObject stateObj, out int charsRead)
		{
			if (stateObj._longlenleft == 0UL)
			{
				charsRead = 0;
				return true;
			}
			charsRead = len;
			if (stateObj._longlenleft >> 1 < (ulong)((long)len))
			{
				charsRead = (int)(stateObj._longlenleft >> 1);
			}
			for (int i = 0; i < charsRead; i++)
			{
				if (!stateObj.TryReadChar(out buff[offst + i]))
				{
					return false;
				}
			}
			stateObj._longlenleft -= (ulong)((ulong)((long)charsRead) << 1);
			return true;
		}

		internal int ReadPlpUnicodeChars(ref char[] buff, int offst, int len, TdsParserStateObject stateObj)
		{
			int num;
			if (!this.TryReadPlpUnicodeChars(ref buff, offst, len, stateObj, out num))
			{
				throw SQL.SynchronousCallMayNotPend();
			}
			return num;
		}

		internal bool TryReadPlpUnicodeChars(ref char[] buff, int offst, int len, TdsParserStateObject stateObj, out int totalCharsRead)
		{
			int num = 0;
			if (stateObj._longlen == 0UL)
			{
				totalCharsRead = 0;
				return true;
			}
			int i = len;
			if (buff == null && stateObj._longlen != 18446744073709551614UL)
			{
				buff = new char[Math.Min((int)stateObj._longlen, len)];
			}
			if (stateObj._longlenleft == 0UL)
			{
				ulong num2;
				if (!stateObj.TryReadPlpLength(false, out num2))
				{
					totalCharsRead = 0;
					return false;
				}
				if (stateObj._longlenleft == 0UL)
				{
					totalCharsRead = 0;
					return true;
				}
			}
			totalCharsRead = 0;
			while (i > 0)
			{
				num = (int)Math.Min(stateObj._longlenleft + 1UL >> 1, (ulong)((long)i));
				if (buff == null || buff.Length < offst + num)
				{
					char[] array = new char[offst + num];
					if (buff != null)
					{
						Buffer.BlockCopy(buff, 0, array, 0, offst * 2);
					}
					buff = array;
				}
				if (num > 0)
				{
					if (!this.TryReadPlpUnicodeCharsChunk(buff, offst, num, stateObj, out num))
					{
						return false;
					}
					i -= num;
					offst += num;
					totalCharsRead += num;
				}
				if (stateObj._longlenleft == 1UL && i > 0)
				{
					byte b;
					if (!stateObj.TryReadByte(out b))
					{
						return false;
					}
					stateObj._longlenleft -= 1UL;
					ulong num3;
					if (!stateObj.TryReadPlpLength(false, out num3))
					{
						return false;
					}
					byte b2;
					if (!stateObj.TryReadByte(out b2))
					{
						return false;
					}
					stateObj._longlenleft -= 1UL;
					buff[offst] = (char)(((int)(b2 & byte.MaxValue) << 8) + (int)(b & byte.MaxValue));
					checked
					{
						offst++;
					}
					num++;
					i--;
					totalCharsRead++;
				}
				ulong num4;
				if (stateObj._longlenleft == 0UL && !stateObj.TryReadPlpLength(false, out num4))
				{
					return false;
				}
				if (stateObj._longlenleft == 0UL)
				{
					break;
				}
			}
			return true;
		}

		internal int ReadPlpAnsiChars(ref char[] buff, int offst, int len, SqlMetaDataPriv metadata, TdsParserStateObject stateObj)
		{
			int num = 0;
			if (stateObj._longlen == 0UL)
			{
				return 0;
			}
			int i = len;
			if (stateObj._longlenleft == 0UL)
			{
				stateObj.ReadPlpLength(false);
				if (stateObj._longlenleft == 0UL)
				{
					stateObj._plpdecoder = null;
					return 0;
				}
			}
			if (stateObj._plpdecoder == null)
			{
				Encoding encoding = metadata.encoding;
				if (encoding == null)
				{
					if (this._defaultEncoding == null)
					{
						this.ThrowUnsupportedCollationEncountered(stateObj);
					}
					encoding = this._defaultEncoding;
				}
				stateObj._plpdecoder = encoding.GetDecoder();
			}
			while (i > 0)
			{
				int num2 = (int)Math.Min(stateObj._longlenleft, (ulong)((long)i));
				if (stateObj._bTmp == null || stateObj._bTmp.Length < num2)
				{
					stateObj._bTmp = new byte[num2];
				}
				num2 = stateObj.ReadPlpBytesChunk(stateObj._bTmp, 0, num2);
				int chars = stateObj._plpdecoder.GetChars(stateObj._bTmp, 0, num2, buff, offst);
				i -= chars;
				offst += chars;
				num += chars;
				if (stateObj._longlenleft == 0UL)
				{
					stateObj.ReadPlpLength(false);
				}
				if (stateObj._longlenleft == 0UL)
				{
					stateObj._plpdecoder = null;
					break;
				}
			}
			return num;
		}

		internal ulong SkipPlpValue(ulong cb, TdsParserStateObject stateObj)
		{
			ulong num;
			if (!this.TrySkipPlpValue(cb, stateObj, out num))
			{
				throw SQL.SynchronousCallMayNotPend();
			}
			return num;
		}

		internal bool TrySkipPlpValue(ulong cb, TdsParserStateObject stateObj, out ulong totalBytesSkipped)
		{
			totalBytesSkipped = 0UL;
			ulong num;
			if (stateObj._longlenleft == 0UL && !stateObj.TryReadPlpLength(false, out num))
			{
				return false;
			}
			while (totalBytesSkipped < cb && stateObj._longlenleft > 0UL)
			{
				int num2;
				if (stateObj._longlenleft > 2147483647UL)
				{
					num2 = int.MaxValue;
				}
				else
				{
					num2 = (int)stateObj._longlenleft;
				}
				num2 = ((cb - totalBytesSkipped < (ulong)((long)num2)) ? ((int)(cb - totalBytesSkipped)) : num2);
				if (!stateObj.TrySkipBytes(num2))
				{
					return false;
				}
				stateObj._longlenleft -= (ulong)((long)num2);
				totalBytesSkipped += (ulong)((long)num2);
				ulong num3;
				if (stateObj._longlenleft == 0UL && !stateObj.TryReadPlpLength(false, out num3))
				{
					return false;
				}
			}
			return true;
		}

		internal ulong PlpBytesLeft(TdsParserStateObject stateObj)
		{
			if (stateObj._longlen != 0UL && stateObj._longlenleft == 0UL)
			{
				stateObj.ReadPlpLength(false);
			}
			return stateObj._longlenleft;
		}

		internal bool TryPlpBytesLeft(TdsParserStateObject stateObj, out ulong left)
		{
			if (stateObj._longlen != 0UL && stateObj._longlenleft == 0UL && !stateObj.TryReadPlpLength(false, out left))
			{
				return false;
			}
			left = stateObj._longlenleft;
			return true;
		}

		internal ulong PlpBytesTotalLength(TdsParserStateObject stateObj)
		{
			if (stateObj._longlen == 18446744073709551614UL)
			{
				return ulong.MaxValue;
			}
			if (stateObj._longlen == 18446744073709551615UL)
			{
				return 0UL;
			}
			return stateObj._longlen;
		}

		private bool TryProcessUDTMetaData(SqlMetaDataPriv metaData, TdsParserStateObject stateObj)
		{
			ushort num;
			if (!stateObj.TryReadUInt16(out num))
			{
				return false;
			}
			metaData.length = (int)num;
			byte b;
			return stateObj.TryReadByte(out b) && (b == 0 || stateObj.TryReadString((int)b, out metaData.udtDatabaseName)) && stateObj.TryReadByte(out b) && (b == 0 || stateObj.TryReadString((int)b, out metaData.udtSchemaName)) && stateObj.TryReadByte(out b) && (b == 0 || stateObj.TryReadString((int)b, out metaData.udtTypeName)) && stateObj.TryReadUInt16(out num) && (num == 0 || stateObj.TryReadString((int)num, out metaData.udtAssemblyQualifiedName));
		}

		internal TdsParserStateObject _physicalStateObj;

		internal TdsParserStateObject _pMarsPhysicalConObj;

		private const int constBinBufferSize = 4096;

		private const int constTextBufferSize = 4096;

		internal TdsParserState _state;

		private string _server = "";

		internal volatile bool _fResetConnection;

		internal volatile bool _fPreserveTransaction;

		private SqlCollation _defaultCollation;

		private int _defaultCodePage;

		private int _defaultLCID;

		internal Encoding _defaultEncoding;

		private static EncryptionOptions s_sniSupportedEncryptionOption = TdsParserStateObjectFactory.Singleton.EncryptionOptions;

		private EncryptionOptions _encryptionOption = TdsParser.s_sniSupportedEncryptionOption;

		private SqlInternalTransaction _currentTransaction;

		private SqlInternalTransaction _pendingTransaction;

		private long _retainedTransactionId;

		private int _nonTransactedOpenResultCount;

		private SqlInternalConnectionTds _connHandler;

		private bool _fMARS;

		internal bool _loginWithFailover;

		internal AutoResetEvent _resetConnectionEvent;

		internal TdsParserSessionPool _sessionPool;

		private bool _isYukon;

		private bool _isKatmai;

		private bool _isDenali;

		private byte[] _sniSpnBuffer;

		private SqlStatistics _statistics;

		private bool _statisticsIsInTransaction;

		private static byte[] s_nicAddress;

		private static volatile uint s_maxSSPILength = 0U;

		private static readonly byte[] s_longDataHeader = new byte[]
		{
			16, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue
		};

		private static object s_tdsParserLock = new object();

		private static readonly byte[] s_xmlMetadataSubstituteSequence = new byte[] { 231, byte.MaxValue, byte.MaxValue, 0, 0, 0, 0, 0 };

		private const int GUID_SIZE = 16;

		internal bool _asyncWrite;

		private static readonly IEnumerable<SqlDataRecord> s_tvpEmptyValue = new SqlDataRecord[0];

		private const ulong _indeterminateSize = 18446744073709551615UL;

		private class TdsOrderUnique
		{
			internal TdsOrderUnique(short ordinal, byte flags)
			{
				this.ColumnOrdinal = ordinal;
				this.Flags = flags;
			}

			internal short ColumnOrdinal;

			internal byte Flags;
		}

		private class TdsOutputStream : Stream
		{
			public TdsOutputStream(TdsParser parser, TdsParserStateObject stateObj, byte[] preambleToStrip)
			{
				this._parser = parser;
				this._stateObj = stateObj;
				this._preambleToStrip = preambleToStrip;
			}

			public override bool CanRead
			{
				get
				{
					return false;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return true;
				}
			}

			public override void Flush()
			{
			}

			public override long Length
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public override long Position
			{
				get
				{
					throw new NotSupportedException();
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			private void StripPreamble(byte[] buffer, ref int offset, ref int count)
			{
				if (this._preambleToStrip != null && count >= this._preambleToStrip.Length)
				{
					for (int i = 0; i < this._preambleToStrip.Length; i++)
					{
						if (this._preambleToStrip[i] != buffer[i])
						{
							this._preambleToStrip = null;
							return;
						}
					}
					offset += this._preambleToStrip.Length;
					count -= this._preambleToStrip.Length;
				}
				this._preambleToStrip = null;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				TdsParser.TdsOutputStream.ValidateWriteParameters(buffer, offset, count);
				this.StripPreamble(buffer, ref offset, ref count);
				if (count > 0)
				{
					this._parser.WriteInt(count, this._stateObj);
					this._stateObj.WriteByteArray(buffer, count, offset, true, null);
				}
			}

			public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				TdsParser.TdsOutputStream.ValidateWriteParameters(buffer, offset, count);
				this.StripPreamble(buffer, ref offset, ref count);
				Task task = null;
				if (count > 0)
				{
					this._parser.WriteInt(count, this._stateObj);
					task = this._stateObj.WriteByteArray(buffer, count, offset, false, null);
				}
				return task ?? Task.CompletedTask;
			}

			internal static void ValidateWriteParameters(byte[] buffer, int offset, int count)
			{
				if (buffer == null)
				{
					throw ADP.ArgumentNull("buffer");
				}
				if (offset < 0)
				{
					throw ADP.ArgumentOutOfRange("offset");
				}
				if (count < 0)
				{
					throw ADP.ArgumentOutOfRange("count");
				}
				try
				{
					if (checked(offset + count) > buffer.Length)
					{
						throw ExceptionBuilder.InvalidOffsetLength();
					}
				}
				catch (OverflowException)
				{
					throw ExceptionBuilder.InvalidOffsetLength();
				}
			}

			private TdsParser _parser;

			private TdsParserStateObject _stateObj;

			private byte[] _preambleToStrip;
		}

		private class ConstrainedTextWriter : TextWriter
		{
			public ConstrainedTextWriter(TextWriter next, int size)
			{
				this._next = next;
				this._size = size;
				this._written = 0;
				if (this._size < 1)
				{
					this._size = int.MaxValue;
				}
			}

			public bool IsComplete
			{
				get
				{
					return this._size > 0 && this._written >= this._size;
				}
			}

			public override Encoding Encoding
			{
				get
				{
					return this._next.Encoding;
				}
			}

			public override void Flush()
			{
				this._next.Flush();
			}

			public override Task FlushAsync()
			{
				return this._next.FlushAsync();
			}

			public override void Write(char value)
			{
				if (this._written < this._size)
				{
					this._next.Write(value);
					this._written++;
				}
			}

			public override void Write(char[] buffer, int index, int count)
			{
				TdsParser.ConstrainedTextWriter.ValidateWriteParameters(buffer, index, count);
				count = Math.Min(this._size - this._written, count);
				if (count > 0)
				{
					this._next.Write(buffer, index, count);
				}
				this._written += count;
			}

			public override Task WriteAsync(char value)
			{
				if (this._written < this._size)
				{
					this._written++;
					return this._next.WriteAsync(value);
				}
				return Task.CompletedTask;
			}

			public override Task WriteAsync(char[] buffer, int index, int count)
			{
				TdsParser.ConstrainedTextWriter.ValidateWriteParameters(buffer, index, count);
				count = Math.Min(this._size - this._written, count);
				if (count > 0)
				{
					this._written += count;
					return this._next.WriteAsync(buffer, index, count);
				}
				return Task.CompletedTask;
			}

			public override Task WriteAsync(string value)
			{
				return base.WriteAsync(value.ToCharArray());
			}

			internal static void ValidateWriteParameters(char[] buffer, int offset, int count)
			{
				if (buffer == null)
				{
					throw ADP.ArgumentNull("buffer");
				}
				if (offset < 0)
				{
					throw ADP.ArgumentOutOfRange("offset");
				}
				if (count < 0)
				{
					throw ADP.ArgumentOutOfRange("count");
				}
				try
				{
					if (checked(offset + count) > buffer.Length)
					{
						throw ExceptionBuilder.InvalidOffsetLength();
					}
				}
				catch (OverflowException)
				{
					throw ExceptionBuilder.InvalidOffsetLength();
				}
			}

			private TextWriter _next;

			private int _size;

			private int _written;
		}
	}
}
