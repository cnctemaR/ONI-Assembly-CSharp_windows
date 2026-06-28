using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mono.Data.Tds.Protocol
{
	internal sealed class TdsComm
	{
		public TdsComm(string dataSource, int port, int packetSize, int timeout, TdsVersion tdsVersion)
		{
			this.packetSize = packetSize;
			this.tdsVersion = tdsVersion;
			this.dataSource = dataSource;
			this.outBuffer = new byte[packetSize];
			this.inBuffer = new byte[packetSize];
			this.outBufferLength = packetSize;
			this.inBufferLength = packetSize;
			this.lsb = true;
			bool flag = false;
			IPEndPoint ipendPoint;
			try
			{
				IPAddress ipaddress;
				if (IPAddress.TryParse(this.dataSource, out ipaddress))
				{
					ipendPoint = new IPEndPoint(ipaddress, port);
				}
				else
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(this.dataSource);
					ipendPoint = new IPEndPoint(hostEntry.AddressList[0], port);
				}
			}
			catch (SocketException ex)
			{
				throw new TdsInternalException("Server does not exist or connection refused.", ex);
			}
			try
			{
				this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				IAsyncResult asyncResult = this.socket.BeginConnect(ipendPoint, null, null);
				int num = timeout * 1000;
				if (timeout > 0 && !asyncResult.IsCompleted && !asyncResult.AsyncWaitHandle.WaitOne(num, false))
				{
					throw Tds.CreateTimeoutException(dataSource, "Open()");
				}
				this.socket.EndConnect(asyncResult);
				try
				{
					this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
				}
				catch (SocketException)
				{
				}
				try
				{
					this.socket.NoDelay = true;
					this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, num);
					this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, num);
				}
				catch
				{
				}
				this.stream = new NetworkStream(this.socket, true);
			}
			catch (SocketException ex2)
			{
				flag = true;
				throw new TdsInternalException("Server does not exist or connection refused.", ex2);
			}
			catch (Exception)
			{
				flag = true;
				throw;
			}
			finally
			{
				if (flag && this.socket != null)
				{
					try
					{
						Socket socket = this.socket;
						this.socket = null;
						socket.Close();
					}
					catch
					{
					}
				}
			}
			if (!this.socket.Connected)
			{
				throw new TdsInternalException("Server does not exist or connection refused.", null);
			}
			this.packetsSent = 1;
		}

		public int CommandTimeout
		{
			get
			{
				return this.commandTimeout;
			}
			set
			{
				this.commandTimeout = value;
			}
		}

		internal Encoding Encoder
		{
			get
			{
				return this.encoder;
			}
			set
			{
				this.encoder = value;
			}
		}

		public int PacketSize
		{
			get
			{
				return this.packetSize;
			}
			set
			{
				this.packetSize = value;
			}
		}

		public bool TdsByteOrder
		{
			get
			{
				return !this.lsb;
			}
			set
			{
				this.lsb = !value;
			}
		}

		public byte[] Swap(byte[] toswap)
		{
			byte[] array = new byte[toswap.Length];
			for (int i = 0; i < toswap.Length; i++)
			{
				array[toswap.Length - i - 1] = toswap[i];
			}
			return array;
		}

		public void SendIfFull()
		{
			if (this.nextOutBufferIndex == this.outBufferLength)
			{
				this.SendPhysicalPacket(false);
				this.nextOutBufferIndex = TdsComm.headerLength;
			}
		}

		public void SendIfFull(int reserve)
		{
			if (this.nextOutBufferIndex + reserve > this.outBufferLength)
			{
				this.SendPhysicalPacket(false);
				this.nextOutBufferIndex = TdsComm.headerLength;
			}
		}

		public void Append(object o)
		{
			if (o == null || o == DBNull.Value)
			{
				this.Append(0);
				return;
			}
			switch (Type.GetTypeCode(o.GetType()))
			{
			case TypeCode.Object:
				if (o is byte[])
				{
					this.Append((byte[])o);
				}
				return;
			case TypeCode.Boolean:
				if ((bool)o)
				{
					this.Append(1);
				}
				else
				{
					this.Append(0);
				}
				return;
			case TypeCode.Byte:
				this.Append((byte)o);
				return;
			case TypeCode.Int16:
				this.Append((short)o);
				return;
			case TypeCode.Int32:
				this.Append((int)o);
				return;
			case TypeCode.Int64:
				this.Append((long)o);
				return;
			case TypeCode.Single:
				this.Append((float)o);
				return;
			case TypeCode.Double:
				this.Append((double)o);
				return;
			case TypeCode.Decimal:
				this.Append((decimal)o, 17);
				return;
			case TypeCode.DateTime:
				this.Append((DateTime)o, 8);
				return;
			case TypeCode.String:
				this.Append((string)o);
				return;
			}
			throw new InvalidOperationException(string.Format("Object Type :{0} , not being appended", o.GetType()));
		}

		public void Append(byte b)
		{
			this.SendIfFull();
			this.Store(this.nextOutBufferIndex, b);
			this.nextOutBufferIndex++;
		}

		public void Append(DateTime t, int bytes)
		{
			DateTime dateTime = new DateTime(1900, 1, 1);
			TimeSpan timeSpan = t - dateTime;
			int days = timeSpan.Days;
			this.SendIfFull(bytes);
			if (bytes == 8)
			{
				long num = (long)(timeSpan.Hours * 3600 + timeSpan.Minutes * 60 + timeSpan.Seconds) * 1000L + (long)timeSpan.Milliseconds;
				int num2 = (int)(num * 300L / 1000L);
				this.AppendInternal(days);
				this.AppendInternal(num2);
			}
			else
			{
				if (bytes != 4)
				{
					throw new Exception("Invalid No of bytes");
				}
				int num2 = timeSpan.Hours * 60 + timeSpan.Minutes;
				this.AppendInternal((short)days);
				this.AppendInternal((short)num2);
			}
		}

		public void Append(byte[] b)
		{
			this.Append(b, b.Length, 0);
		}

		public void Append(byte[] b, int len, byte pad)
		{
			int i = Math.Min(b.Length, len);
			int j = len - i;
			int num = 0;
			while (i > 0)
			{
				this.SendIfFull();
				int num2 = this.outBufferLength - this.nextOutBufferIndex;
				int num3 = Math.Min(num2, i);
				Buffer.BlockCopy(b, num, this.outBuffer, this.nextOutBufferIndex, num3);
				this.nextOutBufferIndex += num3;
				i -= num3;
				num += num3;
			}
			while (j > 0)
			{
				this.SendIfFull();
				int num4 = this.outBufferLength - this.nextOutBufferIndex;
				int num5 = Math.Min(num4, j);
				for (int k = 0; k < num5; k++)
				{
					this.outBuffer[this.nextOutBufferIndex++] = pad;
				}
				j -= num5;
			}
		}

		private void AppendInternal(short s)
		{
			if (!this.lsb)
			{
				this.outBuffer[this.nextOutBufferIndex++] = (byte)(s >> 8) & byte.MaxValue;
				this.outBuffer[this.nextOutBufferIndex++] = (byte)(s & 255);
			}
			else
			{
				this.outBuffer[this.nextOutBufferIndex++] = (byte)(s & 255);
				this.outBuffer[this.nextOutBufferIndex++] = (byte)(s >> 8) & byte.MaxValue;
			}
		}

		public void Append(short s)
		{
			this.SendIfFull(2);
			this.AppendInternal(s);
		}

		public void Append(ushort s)
		{
			this.SendIfFull(2);
			this.AppendInternal((short)s);
		}

		private void AppendInternal(int i)
		{
			if (!this.lsb)
			{
				this.AppendInternal((short)((int)((short)(i >> 16)) & 65535));
				this.AppendInternal((short)(i & 65535));
			}
			else
			{
				this.AppendInternal((short)(i & 65535));
				this.AppendInternal((short)((int)((short)(i >> 16)) & 65535));
			}
		}

		public void Append(int i)
		{
			this.SendIfFull(4);
			this.AppendInternal(i);
		}

		public void Append(string s)
		{
			if (this.tdsVersion < TdsVersion.tds70)
			{
				this.Append(this.encoder.GetBytes(s));
			}
			else
			{
				int num = s.Length * 2;
				int num2 = num / this.outBufferLength;
				int num3 = 0;
				if (num % this.outBufferLength > 0)
				{
					num2++;
				}
				int num4 = this.outBufferLength - this.nextOutBufferIndex;
				for (int i = 0; i < num2; i++)
				{
					int num5 = Math.Min(num4, num);
					int j = 0;
					while (j < num5)
					{
						this.AppendInternal((short)s[num3]);
						j += 2;
						num3++;
					}
					num -= Math.Min(num4, num);
					this.SendIfFull(num + 2);
				}
			}
		}

		public byte[] Append(string s, int len, byte pad)
		{
			if (s == null)
			{
				return new byte[0];
			}
			byte[] bytes = this.encoder.GetBytes(s);
			this.Append(bytes, len, pad);
			return bytes;
		}

		public void Append(double value)
		{
			if (!this.lsb)
			{
				this.Append(this.Swap(BitConverter.GetBytes(value)), 8, 0);
			}
			else
			{
				this.Append(BitConverter.GetBytes(value), 8, 0);
			}
		}

		public void Append(float value)
		{
			if (!this.lsb)
			{
				this.Append(this.Swap(BitConverter.GetBytes(value)), 4, 0);
			}
			else
			{
				this.Append(BitConverter.GetBytes(value), 4, 0);
			}
		}

		public void Append(long l)
		{
			this.SendIfFull(8);
			if (!this.lsb)
			{
				this.AppendInternal((int)((long)((int)(l >> 32)) & (long)((ulong)(-1))));
				this.AppendInternal((int)(l & (long)((ulong)(-1))));
			}
			else
			{
				this.AppendInternal((int)(l & (long)((ulong)(-1))));
				this.AppendInternal((int)((long)((int)(l >> 32)) & (long)((ulong)(-1))));
			}
		}

		public void Append(decimal d, int bytes)
		{
			int[] bits = decimal.GetBits(d);
			byte b = ((!(d > 0m)) ? 0 : 1);
			this.SendIfFull(bytes);
			this.Append(b);
			this.AppendInternal(bits[0]);
			this.AppendInternal(bits[1]);
			this.AppendInternal(bits[2]);
			this.AppendInternal(0);
		}

		public void Close()
		{
			if (this.stream == null)
			{
				return;
			}
			this.connReset = false;
			this.socket = null;
			try
			{
				this.stream.Close();
			}
			catch
			{
			}
			this.stream = null;
		}

		public bool IsConnected()
		{
			return this.socket != null && this.socket.Connected && (!this.socket.Poll(0, SelectMode.SelectRead) || this.socket.Available != 0);
		}

		public byte GetByte()
		{
			if (this.inBufferIndex >= this.inBufferLength)
			{
				this.GetPhysicalPacket();
			}
			return this.inBuffer[this.inBufferIndex++];
		}

		public byte[] GetBytes(int len, bool exclusiveBuffer)
		{
			byte[] array;
			if (exclusiveBuffer || len > 16384)
			{
				array = new byte[len];
			}
			else
			{
				if (this.resBuffer.Length < len)
				{
					this.resBuffer = new byte[len];
				}
				array = this.resBuffer;
			}
			int i = 0;
			while (i < len)
			{
				if (this.inBufferIndex >= this.inBufferLength)
				{
					this.GetPhysicalPacket();
				}
				int num = this.inBufferLength - this.inBufferIndex;
				num = ((num <= len - i) ? num : (len - i));
				Buffer.BlockCopy(this.inBuffer, this.inBufferIndex, array, i, num);
				i += num;
				this.inBufferIndex += num;
			}
			return array;
		}

		public string GetString(int len, Encoding enc)
		{
			if (this.tdsVersion >= TdsVersion.tds70)
			{
				return this.GetString(len, true, null);
			}
			return this.GetString(len, false, null);
		}

		public string GetString(int len)
		{
			if (this.tdsVersion >= TdsVersion.tds70)
			{
				return this.GetString(len, true);
			}
			return this.GetString(len, false);
		}

		public string GetString(int len, bool wide, Encoding enc)
		{
			if (wide)
			{
				char[] array = new char[len];
				for (int i = 0; i < len; i++)
				{
					int num = (int)(this.GetByte() & byte.MaxValue);
					int num2 = (int)(this.GetByte() & byte.MaxValue);
					array[i] = (char)(num | (num2 << 8));
				}
				return new string(array);
			}
			byte[] array2 = new byte[len];
			Array.Copy(this.GetBytes(len, false), array2, len);
			if (enc != null)
			{
				return enc.GetString(array2);
			}
			return this.encoder.GetString(array2);
		}

		public string GetString(int len, bool wide)
		{
			return this.GetString(len, wide, null);
		}

		public int GetNetShort()
		{
			return TdsComm.Ntohs(new byte[]
			{
				this.GetByte(),
				this.GetByte()
			}, 0);
		}

		public short GetTdsShort()
		{
			byte[] array = new byte[2];
			for (int i = 0; i < 2; i++)
			{
				array[i] = this.GetByte();
			}
			if (!BitConverter.IsLittleEndian)
			{
				return BitConverter.ToInt16(this.Swap(array), 0);
			}
			return BitConverter.ToInt16(array, 0);
		}

		public int GetTdsInt()
		{
			byte[] array = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = this.GetByte();
			}
			if (!BitConverter.IsLittleEndian)
			{
				return BitConverter.ToInt32(this.Swap(array), 0);
			}
			return BitConverter.ToInt32(array, 0);
		}

		public long GetTdsInt64()
		{
			byte[] array = new byte[8];
			for (int i = 0; i < 8; i++)
			{
				array[i] = this.GetByte();
			}
			if (!BitConverter.IsLittleEndian)
			{
				return BitConverter.ToInt64(this.Swap(array), 0);
			}
			return BitConverter.ToInt64(array, 0);
		}

		private void GetPhysicalPacket()
		{
			int physicalPacketHeader = this.GetPhysicalPacketHeader();
			this.GetPhysicalPacketData(physicalPacketHeader);
		}

		private int Read(byte[] buffer, int offset, int count)
		{
			int num;
			try
			{
				num = this.stream.Read(buffer, offset, count);
			}
			catch
			{
				this.socket = null;
				this.stream.Close();
				throw;
			}
			return num;
		}

		private int GetPhysicalPacketHeader()
		{
			int num;
			for (int i = 0; i < 8; i += num)
			{
				num = this.Read(this.tmpBuf, i, 8 - i);
				if (num <= 0)
				{
					this.socket = null;
					this.stream.Close();
					throw new IOException((num != 0) ? "Connection error" : "Connection lost");
				}
			}
			TdsPacketType tdsPacketType = (TdsPacketType)this.tmpBuf[0];
			if (tdsPacketType != TdsPacketType.Logon && tdsPacketType != TdsPacketType.Query && tdsPacketType != TdsPacketType.Reply)
			{
				throw new Exception(string.Format("Unknown packet type {0}", this.tmpBuf[0]));
			}
			int num2 = TdsComm.Ntohs(this.tmpBuf, 2) - 8;
			if (num2 >= this.inBuffer.Length)
			{
				this.inBuffer = new byte[num2];
			}
			if (num2 < 0)
			{
				throw new Exception(string.Format("Confused by a length of {0}", num2));
			}
			return num2;
		}

		private void GetPhysicalPacketData(int length)
		{
			int num;
			for (int i = 0; i < length; i += num)
			{
				num = this.Read(this.inBuffer, i, length - i);
				if (num <= 0)
				{
					this.socket = null;
					this.stream.Close();
					throw new IOException((num != 0) ? "Connection error" : "Connection lost");
				}
			}
			this.packetsReceived++;
			this.inBufferLength = length;
			this.inBufferIndex = 0;
		}

		private static int Ntohs(byte[] buf, int offset)
		{
			int num = (int)(buf[offset + 1] & byte.MaxValue);
			int num2 = (int)(buf[offset] & byte.MaxValue) << 8;
			return num2 | num;
		}

		public byte Peek()
		{
			if (this.inBufferIndex >= this.inBufferLength)
			{
				this.GetPhysicalPacket();
			}
			return this.inBuffer[this.inBufferIndex];
		}

		public bool Poll(int seconds, SelectMode selectMode)
		{
			return this.Poll(this.socket, seconds, selectMode);
		}

		private bool Poll(Socket s, int seconds, SelectMode selectMode)
		{
			long num;
			for (num = (long)(seconds * 1000000); num > 2147483647L; num -= 2147483647L)
			{
				bool flag = s.Poll(int.MaxValue, selectMode);
				if (flag)
				{
					return true;
				}
			}
			return s.Poll((int)num, selectMode);
		}

		internal void ResizeOutBuf(int newSize)
		{
			if (newSize != this.outBufferLength)
			{
				byte[] array = new byte[newSize];
				Buffer.BlockCopy(this.outBuffer, 0, array, 0, newSize);
				this.outBufferLength = newSize;
				this.outBuffer = array;
			}
		}

		public bool ResetConnection
		{
			get
			{
				return this.connReset;
			}
			set
			{
				this.connReset = value;
			}
		}

		public void SendPacket()
		{
			if (this.packetType != TdsPacketType.Query && this.packetType != TdsPacketType.Proc)
			{
				this.connReset = false;
			}
			this.SendPhysicalPacket(true);
			this.nextOutBufferIndex = 0;
			this.packetType = TdsPacketType.None;
			this.connReset = false;
			this.packetsSent = 1;
		}

		private void SendPhysicalPacket(bool isLastSegment)
		{
			if (this.nextOutBufferIndex > TdsComm.headerLength || this.packetType == TdsPacketType.Cancel)
			{
				byte b = (byte)(((!isLastSegment) ? 0 : 1) | ((!this.connReset) ? 0 : 8));
				this.Store(0, (byte)this.packetType);
				this.Store(1, b);
				this.Store(2, (short)this.nextOutBufferIndex);
				this.Store(4, 0);
				this.Store(5, 0);
				if (this.tdsVersion >= TdsVersion.tds70)
				{
					this.Store(6, (byte)this.packetsSent);
				}
				else
				{
					this.Store(6, 0);
				}
				this.Store(7, 0);
				this.stream.Write(this.outBuffer, 0, this.nextOutBufferIndex);
				this.stream.Flush();
				this.packetsSent++;
			}
		}

		public void Skip(long i)
		{
			while (i > 0L)
			{
				this.GetByte();
				i -= 1L;
			}
		}

		public void StartPacket(TdsPacketType type)
		{
			if (type != TdsPacketType.Cancel && this.inBufferIndex != this.inBufferLength)
			{
				this.inBufferIndex = this.inBufferLength;
			}
			this.packetType = type;
			this.nextOutBufferIndex = TdsComm.headerLength;
		}

		private void Store(int index, byte value)
		{
			this.outBuffer[index] = value;
		}

		private void Store(int index, short value)
		{
			this.outBuffer[index] = (byte)(value >> 8) & byte.MaxValue;
			this.outBuffer[index + 1] = (byte)value & byte.MaxValue;
		}

		public IAsyncResult BeginReadPacket(AsyncCallback callback, object stateObject)
		{
			TdsAsyncResult tdsAsyncResult = new TdsAsyncResult(callback, stateObject);
			this.stream.BeginRead(this.tmpBuf, 0, 8, new AsyncCallback(this.OnReadPacketCallback), tdsAsyncResult);
			return tdsAsyncResult;
		}

		public int EndReadPacket(IAsyncResult ar)
		{
			if (!ar.IsCompleted)
			{
				ar.AsyncWaitHandle.WaitOne();
			}
			return (int)((TdsAsyncResult)ar).ReturnValue;
		}

		public void OnReadPacketCallback(IAsyncResult socketAsyncResult)
		{
			TdsAsyncResult tdsAsyncResult = (TdsAsyncResult)socketAsyncResult.AsyncState;
			int num;
			for (int i = this.stream.EndRead(socketAsyncResult); i < 8; i += num)
			{
				num = this.Read(this.tmpBuf, i, 8 - i);
				if (num <= 0)
				{
					this.socket = null;
					this.stream.Close();
					throw new IOException((num != 0) ? "Connection error" : "Connection lost");
				}
			}
			TdsPacketType tdsPacketType = (TdsPacketType)this.tmpBuf[0];
			if (tdsPacketType != TdsPacketType.Logon && tdsPacketType != TdsPacketType.Query && tdsPacketType != TdsPacketType.Reply)
			{
				throw new Exception(string.Format("Unknown packet type {0}", this.tmpBuf[0]));
			}
			int num2 = TdsComm.Ntohs(this.tmpBuf, 2) - 8;
			if (num2 >= this.inBuffer.Length)
			{
				this.inBuffer = new byte[num2];
			}
			if (num2 < 0)
			{
				throw new Exception(string.Format("Confused by a length of {0}", num2));
			}
			this.GetPhysicalPacketData(num2);
			int num3 = num2 + 8;
			tdsAsyncResult.ReturnValue = num3;
			tdsAsyncResult.MarkComplete();
		}

		private NetworkStream stream;

		private int packetSize;

		private TdsPacketType packetType;

		private bool connReset;

		private Encoding encoder;

		private string dataSource;

		private int commandTimeout;

		private byte[] outBuffer;

		private int outBufferLength;

		private int nextOutBufferIndex;

		private bool lsb;

		private byte[] inBuffer;

		private int inBufferLength;

		private int inBufferIndex;

		private static int headerLength = 8;

		private byte[] tmpBuf = new byte[8];

		private byte[] resBuffer = new byte[256];

		private int packetsSent;

		private int packetsReceived;

		private Socket socket;

		private TdsVersion tdsVersion;
	}
}
