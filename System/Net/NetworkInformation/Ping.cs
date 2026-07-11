using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace System.Net.NetworkInformation
{
	[global::System.MonoTODO("IPv6 support is missing")]
	public class Ping : global::System.ComponentModel.Component, IDisposable
	{
		static Ping()
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				Ping.CheckLinuxCapabilities();
				if (!Ping.canSendPrivileged && WindowsIdentity.GetCurrent().Name == "root")
				{
					Ping.canSendPrivileged = true;
				}
				foreach (string text in Ping.PingBinPaths)
				{
					if (File.Exists(text))
					{
						Ping.PingBinPath = text;
						break;
					}
				}
			}
			else
			{
				Ping.canSendPrivileged = true;
			}
			if (Ping.PingBinPath == null)
			{
				Ping.PingBinPath = "/bin/ping";
			}
		}

		public event PingCompletedEventHandler PingCompleted;

		void IDisposable.Dispose()
		{
		}

		[DllImport("libc")]
		private static extern int capget(ref Ping.cap_user_header_t header, ref Ping.cap_user_data_t data);

		private static void CheckLinuxCapabilities()
		{
			try
			{
				Ping.cap_user_header_t cap_user_header_t = default(Ping.cap_user_header_t);
				Ping.cap_user_data_t cap_user_data_t = default(Ping.cap_user_data_t);
				cap_user_header_t.version = 537333798U;
				int num = -1;
				try
				{
					num = Ping.capget(ref cap_user_header_t, ref cap_user_data_t);
				}
				catch (Exception)
				{
				}
				if (num != -1)
				{
					Ping.canSendPrivileged = (cap_user_data_t.effective & 8192U) != 0U;
				}
			}
			catch
			{
				Ping.canSendPrivileged = false;
			}
		}

		protected void OnPingCompleted(PingCompletedEventArgs e)
		{
			if (this.PingCompleted != null)
			{
				this.PingCompleted(this, e);
			}
			this.user_async_state = null;
			this.worker = null;
		}

		public PingReply Send(IPAddress address)
		{
			return this.Send(address, 4000);
		}

		public PingReply Send(IPAddress address, int timeout)
		{
			return this.Send(address, timeout, Ping.default_buffer);
		}

		public PingReply Send(IPAddress address, int timeout, byte[] buffer)
		{
			return this.Send(address, timeout, buffer, new PingOptions());
		}

		public PingReply Send(string hostNameOrAddress)
		{
			return this.Send(hostNameOrAddress, 4000);
		}

		public PingReply Send(string hostNameOrAddress, int timeout)
		{
			return this.Send(hostNameOrAddress, timeout, Ping.default_buffer);
		}

		public PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer)
		{
			return this.Send(hostNameOrAddress, timeout, buffer, new PingOptions());
		}

		public PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options)
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostNameOrAddress);
			return this.Send(hostAddresses[0], timeout, buffer, options);
		}

		private static IPAddress GetNonLoopbackIP()
		{
			foreach (IPAddress ipaddress in Dns.GetHostByName(Dns.GetHostName()).AddressList)
			{
				if (!IPAddress.IsLoopback(ipaddress))
				{
					return ipaddress;
				}
			}
			throw new InvalidOperationException("Could not resolve non-loopback IP address for localhost");
		}

		public PingReply Send(IPAddress address, int timeout, byte[] buffer, PingOptions options)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (timeout < 0)
			{
				throw new ArgumentOutOfRangeException("timeout", "timeout must be non-negative integer");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (buffer.Length > 65500)
			{
				throw new ArgumentException("buffer");
			}
			if (Ping.canSendPrivileged)
			{
				return this.SendPrivileged(address, timeout, buffer, options);
			}
			return this.SendUnprivileged(address, timeout, buffer, options);
		}

		private PingReply SendPrivileged(IPAddress address, int timeout, byte[] buffer, PingOptions options)
		{
			IPEndPoint ipendPoint = new IPEndPoint(address, 0);
			IPEndPoint ipendPoint2 = new IPEndPoint(Ping.GetNonLoopbackIP(), 0);
			PingReply pingReply;
			using (global::System.Net.Sockets.Socket socket = new global::System.Net.Sockets.Socket(global::System.Net.Sockets.AddressFamily.InterNetwork, global::System.Net.Sockets.SocketType.Raw, global::System.Net.Sockets.ProtocolType.Icmp))
			{
				if (options != null)
				{
					socket.DontFragment = options.DontFragment;
					socket.Ttl = (short)options.Ttl;
				}
				socket.SendTimeout = timeout;
				socket.ReceiveTimeout = timeout;
				Ping.IcmpMessage icmpMessage = new Ping.IcmpMessage(8, 0, 1, 0, buffer);
				byte[] array = icmpMessage.GetBytes();
				socket.SendBufferSize = array.Length;
				socket.SendTo(array, array.Length, global::System.Net.Sockets.SocketFlags.None, ipendPoint);
				DateTime now = DateTime.Now;
				array = new byte[100];
				int num;
				long num3;
				Ping.IcmpMessage icmpMessage2;
				for (;;)
				{
					EndPoint endPoint = ipendPoint2;
					num = 0;
					int num2 = socket.ReceiveFrom_nochecks_exc(array, 0, 100, global::System.Net.Sockets.SocketFlags.None, ref endPoint, false, out num);
					if (num != 0)
					{
						break;
					}
					num3 = (long)(DateTime.Now - now).TotalMilliseconds;
					int num4 = (int)(array[0] & 15) << 2;
					int num5 = num2 - num4;
					if (!((IPEndPoint)endPoint).Address.Equals(ipendPoint.Address))
					{
						long num6 = (long)timeout - num3;
						if (num6 <= 0L)
						{
							goto Block_7;
						}
						socket.ReceiveTimeout = (int)num6;
					}
					else
					{
						icmpMessage2 = new Ping.IcmpMessage(array, num4, num5);
						if (icmpMessage2.Identifier == 1 && icmpMessage2.Type != 8)
						{
							goto IL_01C9;
						}
						long num7 = (long)timeout - num3;
						if (num7 <= 0L)
						{
							goto Block_9;
						}
						socket.ReceiveTimeout = (int)num7;
					}
				}
				if (num == 10060)
				{
					return new PingReply(null, new byte[0], options, 0L, IPStatus.TimedOut);
				}
				throw new NotSupportedException(string.Format("Unexpected socket error during ping request: {0}", num));
				Block_7:
				return new PingReply(null, new byte[0], options, 0L, IPStatus.TimedOut);
				Block_9:
				return new PingReply(null, new byte[0], options, 0L, IPStatus.TimedOut);
				IL_01C9:
				pingReply = new PingReply(address, icmpMessage2.Data, options, num3, icmpMessage2.IPStatus);
			}
			return pingReply;
		}

		private PingReply SendUnprivileged(IPAddress address, int timeout, byte[] buffer, PingOptions options)
		{
			DateTime now = DateTime.Now;
			global::System.Diagnostics.Process process = new global::System.Diagnostics.Process();
			string text = this.BuildPingArgs(address, timeout, options);
			long num = 0L;
			process.StartInfo.FileName = Ping.PingBinPath;
			process.StartInfo.Arguments = text;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			DateTime utcNow = DateTime.UtcNow;
			try
			{
				process.Start();
				num = (long)(DateTime.Now - now).TotalMilliseconds;
				if (!process.WaitForExit(timeout) || (process.HasExited && process.ExitCode == 2))
				{
					return new PingReply(address, buffer, options, num, IPStatus.TimedOut);
				}
				if (process.ExitCode == 1)
				{
					return new PingReply(address, buffer, options, num, IPStatus.TtlExpired);
				}
			}
			catch (Exception)
			{
				return new PingReply(address, buffer, options, num, IPStatus.Unknown);
			}
			finally
			{
				if (process != null)
				{
					if (!process.HasExited)
					{
						process.Kill();
					}
					process.Dispose();
				}
			}
			return new PingReply(address, buffer, options, num, IPStatus.Success);
		}

		public void SendAsync(IPAddress address, int timeout, byte[] buffer, object userToken)
		{
			this.SendAsync(address, 4000, Ping.default_buffer, new PingOptions(), userToken);
		}

		public void SendAsync(IPAddress address, int timeout, object userToken)
		{
			this.SendAsync(address, 4000, Ping.default_buffer, userToken);
		}

		public void SendAsync(IPAddress address, object userToken)
		{
			this.SendAsync(address, 4000, userToken);
		}

		public void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, object userToken)
		{
			this.SendAsync(hostNameOrAddress, timeout, buffer, new PingOptions(), userToken);
		}

		public void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options, object userToken)
		{
			IPAddress ipaddress = Dns.GetHostEntry(hostNameOrAddress).AddressList[0];
			this.SendAsync(ipaddress, timeout, buffer, options, userToken);
		}

		public void SendAsync(string hostNameOrAddress, int timeout, object userToken)
		{
			this.SendAsync(hostNameOrAddress, timeout, Ping.default_buffer, userToken);
		}

		public void SendAsync(string hostNameOrAddress, object userToken)
		{
			this.SendAsync(hostNameOrAddress, 4000, userToken);
		}

		public void SendAsync(IPAddress address, int timeout, byte[] buffer, PingOptions options, object userToken)
		{
			if (this.worker != null)
			{
				throw new InvalidOperationException("Another SendAsync operation is in progress");
			}
			this.worker = new global::System.ComponentModel.BackgroundWorker();
			this.worker.DoWork += delegate(object o, global::System.ComponentModel.DoWorkEventArgs ea)
			{
				try
				{
					this.user_async_state = ea.Argument;
					ea.Result = this.Send(address, timeout, buffer, options);
				}
				catch (Exception ex)
				{
					ea.Result = ex;
				}
			};
			this.worker.WorkerSupportsCancellation = true;
			this.worker.RunWorkerCompleted += delegate(object o, global::System.ComponentModel.RunWorkerCompletedEventArgs ea)
			{
				this.OnPingCompleted(new PingCompletedEventArgs(ea.Error, ea.Cancelled, this.user_async_state, ea.Result as PingReply));
			};
			this.worker.RunWorkerAsync(userToken);
		}

		public void SendAsyncCancel()
		{
			if (this.worker == null)
			{
				throw new InvalidOperationException("SendAsync operation is not in progress");
			}
			this.worker.CancelAsync();
		}

		private string BuildPingArgs(IPAddress address, int timeout, PingOptions options)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			StringBuilder stringBuilder = new StringBuilder();
			uint num = Convert.ToUInt32(Math.Floor((double)(timeout + 1000) / 1000.0));
			bool flag = Environment.OSVersion.Platform == PlatformID.MacOSX;
			if (!flag)
			{
				stringBuilder.AppendFormat(invariantCulture, "-q -n -c {0} -w {1} -t {2} -M ", new object[] { 1, num, options.Ttl });
			}
			else
			{
				stringBuilder.AppendFormat(invariantCulture, "-q -n -c {0} -t {1} -o -m {2} ", new object[] { 1, num, options.Ttl });
			}
			if (!flag)
			{
				stringBuilder.Append((!options.DontFragment) ? "dont " : "do ");
			}
			else if (options.DontFragment)
			{
				stringBuilder.Append("-D ");
			}
			stringBuilder.Append(address.ToString());
			return stringBuilder.ToString();
		}

		private const int DefaultCount = 1;

		private const int default_timeout = 4000;

		private const int identifier = 1;

		private const uint linux_cap_version = 537333798U;

		private static readonly string[] PingBinPaths = new string[] { "/bin/ping", "/sbin/ping", "/usr/sbin/ping", "/system/bin/ping" };

		private static readonly string PingBinPath;

		private static readonly byte[] default_buffer = new byte[0];

		private static bool canSendPrivileged;

		private global::System.ComponentModel.BackgroundWorker worker;

		private object user_async_state;

		private struct cap_user_header_t
		{
			public uint version;

			public int pid;
		}

		private struct cap_user_data_t
		{
			public uint effective;

			public uint permitted;

			public uint inheritable;
		}

		private class IcmpMessage
		{
			public IcmpMessage(byte[] bytes, int offset, int size)
			{
				this.bytes = new byte[size];
				Buffer.BlockCopy(bytes, offset, this.bytes, 0, size);
			}

			public IcmpMessage(byte type, byte code, short identifier, short sequence, byte[] data)
			{
				this.bytes = new byte[data.Length + 8];
				this.bytes[0] = type;
				this.bytes[1] = code;
				this.bytes[4] = (byte)(identifier & 255);
				this.bytes[5] = (byte)(identifier >> 8);
				this.bytes[6] = (byte)(sequence & 255);
				this.bytes[7] = (byte)(sequence >> 8);
				Buffer.BlockCopy(data, 0, this.bytes, 8, data.Length);
				ushort num = Ping.IcmpMessage.ComputeChecksum(this.bytes);
				this.bytes[2] = (byte)(num & 255);
				this.bytes[3] = (byte)(num >> 8);
			}

			public byte Type
			{
				get
				{
					return this.bytes[0];
				}
			}

			public byte Code
			{
				get
				{
					return this.bytes[1];
				}
			}

			public byte Identifier
			{
				get
				{
					return (byte)((int)this.bytes[4] + ((int)this.bytes[5] << 8));
				}
			}

			public byte Sequence
			{
				get
				{
					return (byte)((int)this.bytes[6] + ((int)this.bytes[7] << 8));
				}
			}

			public byte[] Data
			{
				get
				{
					byte[] array = new byte[this.bytes.Length - 8];
					Buffer.BlockCopy(this.bytes, 0, array, 0, array.Length);
					return array;
				}
			}

			public byte[] GetBytes()
			{
				return this.bytes;
			}

			private static ushort ComputeChecksum(byte[] data)
			{
				uint num = 0U;
				for (int i = 0; i < data.Length; i += 2)
				{
					ushort num2 = (ushort)((i + 1 >= data.Length) ? 0 : data[i + 1]);
					num2 = (ushort)(num2 << 8);
					num2 += (ushort)data[i];
					num += (uint)num2;
				}
				num = (num >> 16) + (num & 65535U);
				return (ushort)(~(ushort)num);
			}

			public IPStatus IPStatus
			{
				get
				{
					byte type = this.Type;
					switch (type)
					{
					case 0:
						return IPStatus.Success;
					default:
						switch (type)
						{
						case 8:
							return IPStatus.Success;
						case 11:
						{
							byte code = this.Code;
							if (code == 0)
							{
								return IPStatus.TimeExceeded;
							}
							if (code == 1)
							{
								return IPStatus.TtlReassemblyTimeExceeded;
							}
							break;
						}
						case 12:
							return IPStatus.ParameterProblem;
						}
						break;
					case 3:
						switch (this.Code)
						{
						case 0:
							return IPStatus.DestinationNetworkUnreachable;
						case 1:
							return IPStatus.DestinationHostUnreachable;
						case 2:
							return IPStatus.DestinationProhibited;
						case 3:
							return IPStatus.DestinationPortUnreachable;
						case 4:
							return IPStatus.BadOption;
						case 5:
							return IPStatus.BadRoute;
						}
						break;
					case 4:
						return IPStatus.SourceQuench;
					}
					return IPStatus.Unknown;
				}
			}

			private byte[] bytes;
		}
	}
}
