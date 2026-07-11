using System;
using System.IO;
using System.Threading;
using Mono.Security.Protocol.Tls.Handshake;

namespace Mono.Security.Protocol.Tls
{
	internal abstract class RecordProtocol
	{
		public Context Context
		{
			get
			{
				return this.context;
			}
			set
			{
				this.context = value;
			}
		}

		public RecordProtocol(Stream innerStream, Context context)
		{
			this.innerStream = innerStream;
			this.context = context;
			this.context.RecordProtocol = this;
		}

		public virtual void SendRecord(HandshakeType type)
		{
			IAsyncResult asyncResult = this.BeginSendRecord(type, null, null);
			this.EndSendRecord(asyncResult);
		}

		protected abstract void ProcessHandshakeMessage(TlsStream handMsg);

		protected virtual void ProcessChangeCipherSpec()
		{
			Context context = this.Context;
			context.ReadSequenceNumber = 0UL;
			if (context is ClientContext)
			{
				context.EndSwitchingSecurityParameters(true);
			}
			else
			{
				context.StartSwitchingSecurityParameters(false);
			}
			context.ChangeCipherSpecDone = true;
		}

		public virtual HandshakeMessage GetMessage(HandshakeType type)
		{
			throw new NotSupportedException();
		}

		public IAsyncResult BeginReceiveRecord(Stream record, AsyncCallback callback, object state)
		{
			if (this.context.ReceivedConnectionEnd)
			{
				throw new TlsException(AlertDescription.InternalError, "The session is finished and it's no longer valid.");
			}
			RecordProtocol.record_processing.Reset();
			byte[] array = new byte[1];
			RecordProtocol.ReceiveRecordAsyncResult receiveRecordAsyncResult = new RecordProtocol.ReceiveRecordAsyncResult(callback, state, array, record);
			record.BeginRead(receiveRecordAsyncResult.InitialBuffer, 0, receiveRecordAsyncResult.InitialBuffer.Length, new AsyncCallback(this.InternalReceiveRecordCallback), receiveRecordAsyncResult);
			return receiveRecordAsyncResult;
		}

		private void InternalReceiveRecordCallback(IAsyncResult asyncResult)
		{
			RecordProtocol.ReceiveRecordAsyncResult receiveRecordAsyncResult = asyncResult.AsyncState as RecordProtocol.ReceiveRecordAsyncResult;
			Stream record = receiveRecordAsyncResult.Record;
			try
			{
				if (receiveRecordAsyncResult.Record.EndRead(asyncResult) == 0)
				{
					receiveRecordAsyncResult.SetComplete(null);
				}
				else
				{
					int num = (int)receiveRecordAsyncResult.InitialBuffer[0];
					ContentType contentType = (ContentType)num;
					byte[] array = this.ReadRecordBuffer(num, record);
					if (array == null)
					{
						receiveRecordAsyncResult.SetComplete(null);
					}
					else
					{
						if ((contentType != ContentType.Alert || array.Length != 2) && this.Context.Read != null && this.Context.Read.Cipher != null)
						{
							array = this.decryptRecordFragment(contentType, array);
						}
						switch (contentType)
						{
						case ContentType.ChangeCipherSpec:
							this.ProcessChangeCipherSpec();
							break;
						case ContentType.Alert:
							this.ProcessAlert((AlertLevel)array[0], (AlertDescription)array[1]);
							if (record.CanSeek)
							{
								record.SetLength(0L);
							}
							array = null;
							break;
						case ContentType.Handshake:
						{
							TlsStream tlsStream = new TlsStream(array);
							while (!tlsStream.EOF)
							{
								this.ProcessHandshakeMessage(tlsStream);
							}
							break;
						}
						case ContentType.ApplicationData:
							break;
						default:
							if (contentType != (ContentType)128)
							{
								throw new TlsException(AlertDescription.UnexpectedMessage, "Unknown record received from server.");
							}
							this.context.HandshakeMessages.Write(array);
							break;
						}
						receiveRecordAsyncResult.SetComplete(array);
					}
				}
			}
			catch (Exception ex)
			{
				receiveRecordAsyncResult.SetComplete(ex);
			}
		}

		public byte[] EndReceiveRecord(IAsyncResult asyncResult)
		{
			RecordProtocol.ReceiveRecordAsyncResult receiveRecordAsyncResult = asyncResult as RecordProtocol.ReceiveRecordAsyncResult;
			if (receiveRecordAsyncResult == null)
			{
				throw new ArgumentException("Either the provided async result is null or was not created by this RecordProtocol.");
			}
			if (!receiveRecordAsyncResult.IsCompleted)
			{
				receiveRecordAsyncResult.AsyncWaitHandle.WaitOne();
			}
			if (receiveRecordAsyncResult.CompletedWithError)
			{
				throw receiveRecordAsyncResult.AsyncException;
			}
			byte[] resultingBuffer = receiveRecordAsyncResult.ResultingBuffer;
			RecordProtocol.record_processing.Set();
			return resultingBuffer;
		}

		public byte[] ReceiveRecord(Stream record)
		{
			if (this.context.ReceivedConnectionEnd)
			{
				throw new TlsException(AlertDescription.InternalError, "The session is finished and it's no longer valid.");
			}
			RecordProtocol.record_processing.Reset();
			byte[] array = new byte[1];
			if (record.Read(array, 0, array.Length) == 0)
			{
				return null;
			}
			int num = (int)array[0];
			ContentType contentType = (ContentType)num;
			byte[] array2 = this.ReadRecordBuffer(num, record);
			if (array2 == null)
			{
				return null;
			}
			if ((contentType != ContentType.Alert || array2.Length != 2) && this.Context.Read != null && this.Context.Read.Cipher != null)
			{
				array2 = this.decryptRecordFragment(contentType, array2);
			}
			switch (contentType)
			{
			case ContentType.ChangeCipherSpec:
				this.ProcessChangeCipherSpec();
				break;
			case ContentType.Alert:
				this.ProcessAlert((AlertLevel)array2[0], (AlertDescription)array2[1]);
				if (record.CanSeek)
				{
					record.SetLength(0L);
				}
				array2 = null;
				break;
			case ContentType.Handshake:
			{
				TlsStream tlsStream = new TlsStream(array2);
				while (!tlsStream.EOF)
				{
					this.ProcessHandshakeMessage(tlsStream);
				}
				break;
			}
			case ContentType.ApplicationData:
				break;
			default:
				if (contentType != (ContentType)128)
				{
					throw new TlsException(AlertDescription.UnexpectedMessage, "Unknown record received from server.");
				}
				this.context.HandshakeMessages.Write(array2);
				break;
			}
			RecordProtocol.record_processing.Set();
			return array2;
		}

		private byte[] ReadRecordBuffer(int contentType, Stream record)
		{
			if (!Enum.IsDefined(typeof(ContentType), (ContentType)contentType))
			{
				throw new TlsException(AlertDescription.DecodeError);
			}
			byte[] array = new byte[4];
			if (record.Read(array, 0, 4) != 4)
			{
				throw new TlsException("buffer underrun");
			}
			short num = (short)(((int)array[0] << 8) | (int)array[1]);
			short num2 = (short)(((int)array[2] << 8) | (int)array[3]);
			if (record.CanSeek && (long)(num2 + 5) > record.Length)
			{
				return null;
			}
			int num3 = 0;
			byte[] array2 = new byte[(int)num2];
			while (num3 != (int)num2)
			{
				int num4 = record.Read(array2, num3, array2.Length - num3);
				if (num4 == 0)
				{
					throw new TlsException(AlertDescription.CloseNotify, "Received 0 bytes from stream. It must be closed.");
				}
				num3 += num4;
			}
			if (num != this.context.Protocol && this.context.ProtocolNegotiated)
			{
				throw new TlsException(AlertDescription.ProtocolVersion, "Invalid protocol version on message received");
			}
			return array2;
		}

		private void ProcessAlert(AlertLevel alertLevel, AlertDescription alertDesc)
		{
			if (alertLevel != AlertLevel.Warning && alertLevel == AlertLevel.Fatal)
			{
				throw new TlsException(alertLevel, alertDesc);
			}
			if (alertDesc == AlertDescription.CloseNotify)
			{
				this.context.ReceivedConnectionEnd = true;
			}
		}

		internal void SendAlert(ref Exception ex)
		{
			TlsException ex2 = ex as TlsException;
			Alert alert = ((ex2 != null) ? ex2.Alert : new Alert(AlertDescription.InternalError));
			try
			{
				this.SendAlert(alert);
			}
			catch (Exception ex3)
			{
				ex = new IOException(string.Format("Error while sending TLS Alert ({0}:{1}): {2}", alert.Level, alert.Description, ex), ex3);
			}
		}

		public void SendAlert(AlertDescription description)
		{
			this.SendAlert(new Alert(description));
		}

		public void SendAlert(AlertLevel level, AlertDescription description)
		{
			this.SendAlert(new Alert(level, description));
		}

		public void SendAlert(Alert alert)
		{
			AlertLevel alertLevel;
			AlertDescription alertDescription;
			bool flag;
			if (alert == null)
			{
				alertLevel = AlertLevel.Fatal;
				alertDescription = AlertDescription.InternalError;
				flag = true;
			}
			else
			{
				alertLevel = alert.Level;
				alertDescription = alert.Description;
				flag = alert.IsCloseNotify;
			}
			this.SendRecord(ContentType.Alert, new byte[]
			{
				(byte)alertLevel,
				(byte)alertDescription
			});
			if (flag)
			{
				this.context.SentConnectionEnd = true;
			}
		}

		public void SendChangeCipherSpec()
		{
			this.SendRecord(ContentType.ChangeCipherSpec, new byte[] { 1 });
			Context context = this.context;
			context.WriteSequenceNumber = 0UL;
			if (context is ClientContext)
			{
				context.StartSwitchingSecurityParameters(true);
				return;
			}
			context.EndSwitchingSecurityParameters(false);
		}

		public void SendChangeCipherSpec(Stream recordStream)
		{
			byte[] array = this.EncodeRecord(ContentType.ChangeCipherSpec, new byte[] { 1 });
			recordStream.Write(array, 0, array.Length);
			Context context = this.context;
			context.WriteSequenceNumber = 0UL;
			if (context is ClientContext)
			{
				context.StartSwitchingSecurityParameters(true);
				return;
			}
			context.EndSwitchingSecurityParameters(false);
		}

		public IAsyncResult BeginSendChangeCipherSpec(AsyncCallback callback, object state)
		{
			return this.BeginSendRecord(ContentType.ChangeCipherSpec, new byte[] { 1 }, callback, state);
		}

		public void EndSendChangeCipherSpec(IAsyncResult asyncResult)
		{
			this.EndSendRecord(asyncResult);
			Context context = this.context;
			context.WriteSequenceNumber = 0UL;
			if (context is ClientContext)
			{
				context.StartSwitchingSecurityParameters(true);
				return;
			}
			context.EndSwitchingSecurityParameters(false);
		}

		public IAsyncResult BeginSendRecord(HandshakeType handshakeType, AsyncCallback callback, object state)
		{
			HandshakeMessage message = this.GetMessage(handshakeType);
			message.Process();
			RecordProtocol.SendRecordAsyncResult sendRecordAsyncResult = new RecordProtocol.SendRecordAsyncResult(callback, state, message);
			this.BeginSendRecord(message.ContentType, message.EncodeMessage(), new AsyncCallback(this.InternalSendRecordCallback), sendRecordAsyncResult);
			return sendRecordAsyncResult;
		}

		private void InternalSendRecordCallback(IAsyncResult ar)
		{
			RecordProtocol.SendRecordAsyncResult sendRecordAsyncResult = ar.AsyncState as RecordProtocol.SendRecordAsyncResult;
			try
			{
				this.EndSendRecord(ar);
				sendRecordAsyncResult.Message.Update();
				sendRecordAsyncResult.Message.Reset();
				sendRecordAsyncResult.SetComplete();
			}
			catch (Exception ex)
			{
				sendRecordAsyncResult.SetComplete(ex);
			}
		}

		public IAsyncResult BeginSendRecord(ContentType contentType, byte[] recordData, AsyncCallback callback, object state)
		{
			if (this.context.SentConnectionEnd)
			{
				throw new TlsException(AlertDescription.InternalError, "The session is finished and it's no longer valid.");
			}
			byte[] array = this.EncodeRecord(contentType, recordData);
			return this.innerStream.BeginWrite(array, 0, array.Length, callback, state);
		}

		public void EndSendRecord(IAsyncResult asyncResult)
		{
			if (asyncResult is RecordProtocol.SendRecordAsyncResult)
			{
				RecordProtocol.SendRecordAsyncResult sendRecordAsyncResult = asyncResult as RecordProtocol.SendRecordAsyncResult;
				if (!sendRecordAsyncResult.IsCompleted)
				{
					sendRecordAsyncResult.AsyncWaitHandle.WaitOne();
				}
				if (sendRecordAsyncResult.CompletedWithError)
				{
					throw sendRecordAsyncResult.AsyncException;
				}
			}
			else
			{
				this.innerStream.EndWrite(asyncResult);
			}
		}

		public void SendRecord(ContentType contentType, byte[] recordData)
		{
			IAsyncResult asyncResult = this.BeginSendRecord(contentType, recordData, null, null);
			this.EndSendRecord(asyncResult);
		}

		public byte[] EncodeRecord(ContentType contentType, byte[] recordData)
		{
			return this.EncodeRecord(contentType, recordData, 0, recordData.Length);
		}

		public byte[] EncodeRecord(ContentType contentType, byte[] recordData, int offset, int count)
		{
			if (this.context.SentConnectionEnd)
			{
				throw new TlsException(AlertDescription.InternalError, "The session is finished and it's no longer valid.");
			}
			TlsStream tlsStream = new TlsStream();
			short num;
			for (int i = offset; i < offset + count; i += (int)num)
			{
				if (count + offset - i > 16384)
				{
					num = 16384;
				}
				else
				{
					num = (short)(count + offset - i);
				}
				byte[] array = new byte[(int)num];
				Buffer.BlockCopy(recordData, i, array, 0, (int)num);
				if (this.Context.Write != null && this.Context.Write.Cipher != null)
				{
					array = this.encryptRecordFragment(contentType, array);
				}
				tlsStream.Write((byte)contentType);
				tlsStream.Write(this.context.Protocol);
				tlsStream.Write((short)array.Length);
				tlsStream.Write(array);
			}
			return tlsStream.ToArray();
		}

		public byte[] EncodeHandshakeRecord(HandshakeType handshakeType)
		{
			HandshakeMessage message = this.GetMessage(handshakeType);
			message.Process();
			byte[] array = this.EncodeRecord(message.ContentType, message.EncodeMessage());
			message.Update();
			message.Reset();
			return array;
		}

		private byte[] encryptRecordFragment(ContentType contentType, byte[] fragment)
		{
			byte[] array;
			if (this.Context is ClientContext)
			{
				array = this.context.Write.Cipher.ComputeClientRecordMAC(contentType, fragment);
			}
			else
			{
				array = this.context.Write.Cipher.ComputeServerRecordMAC(contentType, fragment);
			}
			byte[] array2 = this.context.Write.Cipher.EncryptRecord(fragment, array);
			Context context = this.context;
			ulong writeSequenceNumber = context.WriteSequenceNumber;
			context.WriteSequenceNumber = writeSequenceNumber + 1UL;
			return array2;
		}

		private byte[] decryptRecordFragment(ContentType contentType, byte[] fragment)
		{
			byte[] array = null;
			byte[] array2 = null;
			try
			{
				this.context.Read.Cipher.DecryptRecord(fragment, out array, out array2);
			}
			catch
			{
				if (this.context is ServerContext)
				{
					this.Context.RecordProtocol.SendAlert(AlertDescription.DecryptionFailed);
				}
				throw;
			}
			byte[] array3;
			if (this.Context is ClientContext)
			{
				array3 = this.context.Read.Cipher.ComputeServerRecordMAC(contentType, array);
			}
			else
			{
				array3 = this.context.Read.Cipher.ComputeClientRecordMAC(contentType, array);
			}
			if (!this.Compare(array3, array2))
			{
				throw new TlsException(AlertDescription.BadRecordMAC, "Bad record MAC");
			}
			Context context = this.context;
			ulong readSequenceNumber = context.ReadSequenceNumber;
			context.ReadSequenceNumber = readSequenceNumber + 1UL;
			return array;
		}

		private bool Compare(byte[] array1, byte[] array2)
		{
			if (array1 == null)
			{
				return array2 == null;
			}
			if (array2 == null)
			{
				return false;
			}
			if (array1.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array1.Length; i++)
			{
				if (array1[i] != array2[i])
				{
					return false;
				}
			}
			return true;
		}

		private static ManualResetEvent record_processing = new ManualResetEvent(true);

		protected Stream innerStream;

		protected Context context;

		private class ReceiveRecordAsyncResult : IAsyncResult
		{
			public ReceiveRecordAsyncResult(AsyncCallback userCallback, object userState, byte[] initialBuffer, Stream record)
			{
				this._userCallback = userCallback;
				this._userState = userState;
				this._initialBuffer = initialBuffer;
				this._record = record;
			}

			public Stream Record
			{
				get
				{
					return this._record;
				}
			}

			public byte[] ResultingBuffer
			{
				get
				{
					return this._resultingBuffer;
				}
			}

			public byte[] InitialBuffer
			{
				get
				{
					return this._initialBuffer;
				}
			}

			public object AsyncState
			{
				get
				{
					return this._userState;
				}
			}

			public Exception AsyncException
			{
				get
				{
					return this._asyncException;
				}
			}

			public bool CompletedWithError
			{
				get
				{
					return this.IsCompleted && this._asyncException != null;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					object obj = this.locker;
					lock (obj)
					{
						if (this.handle == null)
						{
							this.handle = new ManualResetEvent(this.completed);
						}
					}
					return this.handle;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public bool IsCompleted
			{
				get
				{
					object obj = this.locker;
					bool flag2;
					lock (obj)
					{
						flag2 = this.completed;
					}
					return flag2;
				}
			}

			private void SetComplete(Exception ex, byte[] resultingBuffer)
			{
				object obj = this.locker;
				lock (obj)
				{
					if (!this.completed)
					{
						this.completed = true;
						this._asyncException = ex;
						this._resultingBuffer = resultingBuffer;
						if (this.handle != null)
						{
							this.handle.Set();
						}
						if (this._userCallback != null)
						{
							this._userCallback.BeginInvoke(this, null, null);
						}
					}
				}
			}

			public void SetComplete(Exception ex)
			{
				this.SetComplete(ex, null);
			}

			public void SetComplete(byte[] resultingBuffer)
			{
				this.SetComplete(null, resultingBuffer);
			}

			public void SetComplete()
			{
				this.SetComplete(null, null);
			}

			private object locker = new object();

			private AsyncCallback _userCallback;

			private object _userState;

			private Exception _asyncException;

			private ManualResetEvent handle;

			private byte[] _resultingBuffer;

			private Stream _record;

			private bool completed;

			private byte[] _initialBuffer;
		}

		private class SendRecordAsyncResult : IAsyncResult
		{
			public SendRecordAsyncResult(AsyncCallback userCallback, object userState, HandshakeMessage message)
			{
				this._userCallback = userCallback;
				this._userState = userState;
				this._message = message;
			}

			public HandshakeMessage Message
			{
				get
				{
					return this._message;
				}
			}

			public object AsyncState
			{
				get
				{
					return this._userState;
				}
			}

			public Exception AsyncException
			{
				get
				{
					return this._asyncException;
				}
			}

			public bool CompletedWithError
			{
				get
				{
					return this.IsCompleted && this._asyncException != null;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					object obj = this.locker;
					lock (obj)
					{
						if (this.handle == null)
						{
							this.handle = new ManualResetEvent(this.completed);
						}
					}
					return this.handle;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public bool IsCompleted
			{
				get
				{
					object obj = this.locker;
					bool flag2;
					lock (obj)
					{
						flag2 = this.completed;
					}
					return flag2;
				}
			}

			public void SetComplete(Exception ex)
			{
				object obj = this.locker;
				lock (obj)
				{
					if (!this.completed)
					{
						this.completed = true;
						if (this.handle != null)
						{
							this.handle.Set();
						}
						if (this._userCallback != null)
						{
							this._userCallback.BeginInvoke(this, null, null);
						}
						this._asyncException = ex;
					}
				}
			}

			public void SetComplete()
			{
				this.SetComplete(null);
			}

			private object locker = new object();

			private AsyncCallback _userCallback;

			private object _userState;

			private Exception _asyncException;

			private ManualResetEvent handle;

			private HandshakeMessage _message;

			private bool completed;
		}
	}
}
