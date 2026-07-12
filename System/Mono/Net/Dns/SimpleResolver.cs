using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Mono.Net.Dns
{
	internal sealed class SimpleResolver : IDisposable
	{
		public SimpleResolver()
		{
			this.queries = new Dictionary<int, SimpleResolverEventArgs>();
			this.receive_cb = new AsyncCallback(this.OnReceive);
			this.timeout_cb = new TimerCallback(this.OnTimeout);
			this.InitFromSystem();
			this.InitSocket();
		}

		void IDisposable.Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				if (this.client != null)
				{
					this.client.Close();
					this.client = null;
				}
			}
		}

		public void Close()
		{
			((IDisposable)this).Dispose();
		}

		private void GetLocalHost(SimpleResolverEventArgs args)
		{
			IPHostEntry iphostEntry = new IPHostEntry();
			iphostEntry.HostName = "localhost";
			iphostEntry.AddressList = new IPAddress[] { IPAddress.Loopback };
			iphostEntry.Aliases = SimpleResolver.EmptyStrings;
			args.ResolverError = ResolverError.NoError;
			args.HostEntry = iphostEntry;
		}

		public bool GetHostAddressesAsync(SimpleResolverEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (args.HostName == null)
			{
				throw new ArgumentNullException("args.HostName is null");
			}
			if (args.HostName.Length > 255)
			{
				throw new ArgumentException("args.HostName is too long");
			}
			args.Reset(ResolverAsyncOperation.GetHostAddresses);
			string hostName = args.HostName;
			if (hostName == "")
			{
				this.GetLocalHost(args);
				return false;
			}
			IPAddress ipaddress;
			if (IPAddress.TryParse(hostName, out ipaddress))
			{
				args.HostEntry = new IPHostEntry
				{
					HostName = hostName,
					Aliases = SimpleResolver.EmptyStrings,
					AddressList = new IPAddress[] { ipaddress }
				};
				return false;
			}
			this.SendAQuery(args, true);
			return true;
		}

		public bool GetHostEntryAsync(SimpleResolverEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (args.HostName == null)
			{
				throw new ArgumentNullException("args.HostName is null");
			}
			if (args.HostName.Length > 255)
			{
				throw new ArgumentException("args.HostName is too long");
			}
			args.Reset(ResolverAsyncOperation.GetHostEntry);
			string hostName = args.HostName;
			if (hostName == "")
			{
				this.GetLocalHost(args);
				return false;
			}
			IPAddress ipaddress;
			if (IPAddress.TryParse(hostName, out ipaddress))
			{
				args.HostEntry = new IPHostEntry
				{
					HostName = hostName,
					Aliases = SimpleResolver.EmptyStrings,
					AddressList = new IPAddress[] { ipaddress }
				};
				args.PTRAddress = ipaddress;
				this.SendPTRQuery(args, true);
				return true;
			}
			this.SendAQuery(args, true);
			return true;
		}

		private bool AddQuery(DnsQuery query, SimpleResolverEventArgs args)
		{
			Dictionary<int, SimpleResolverEventArgs> dictionary = this.queries;
			lock (dictionary)
			{
				if (this.queries.ContainsKey((int)query.Header.ID))
				{
					return false;
				}
				this.queries[(int)query.Header.ID] = args;
			}
			return true;
		}

		private static DnsQuery GetQuery(string host, DnsQType q, DnsQClass c)
		{
			return new DnsQuery(host, q, c);
		}

		private void SendAQuery(SimpleResolverEventArgs args, bool add_it)
		{
			this.SendAQuery(args, args.HostName, add_it);
		}

		private void SendAQuery(SimpleResolverEventArgs args, string host, bool add_it)
		{
			DnsQuery query = SimpleResolver.GetQuery(host, DnsQType.A, DnsQClass.Internet);
			this.SendQuery(args, query, add_it);
		}

		private static string GetPTRName(IPAddress address)
		{
			byte[] addressBytes = address.GetAddressBytes();
			StringBuilder stringBuilder = new StringBuilder(28);
			for (int i = addressBytes.Length - 1; i >= 0; i--)
			{
				stringBuilder.AppendFormat("{0}.", addressBytes[i]);
			}
			stringBuilder.Append("in-addr.arpa");
			return stringBuilder.ToString();
		}

		private void SendPTRQuery(SimpleResolverEventArgs args, bool add_it)
		{
			DnsQuery query = SimpleResolver.GetQuery(SimpleResolver.GetPTRName(args.PTRAddress), DnsQType.PTR, DnsQClass.Internet);
			this.SendQuery(args, query, add_it);
		}

		private void SendQuery(SimpleResolverEventArgs args, DnsQuery query, bool add_it)
		{
			int num = 0;
			if (add_it)
			{
				for (;;)
				{
					query.Header.ID = (ushort)new Random().Next(1, 65534);
					if (num > 500)
					{
						break;
					}
					if (this.AddQuery(query, args))
					{
						goto Block_2;
					}
				}
				throw new InvalidOperationException("Too many pending queries (or really bad luck)");
				Block_2:
				args.QueryID = query.Header.ID;
			}
			else
			{
				query.Header.ID = args.QueryID;
			}
			if (args.Timer == null)
			{
				args.Timer = new Timer(this.timeout_cb, args, 5000, -1);
			}
			else
			{
				args.Timer.Change(5000, -1);
			}
			this.client.BeginSend(query.Packet, 0, query.Length, SocketFlags.None, null, null);
		}

		private byte[] GetFreshBuffer()
		{
			return new byte[512];
		}

		private void FreeBuffer(byte[] buffer)
		{
		}

		private void InitSocket()
		{
			this.client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			this.client.Blocking = true;
			this.client.Bind(new IPEndPoint(IPAddress.Any, 0));
			this.client.Connect(this.endpoints[0]);
			this.BeginReceive();
		}

		private void BeginReceive()
		{
			byte[] freshBuffer = this.GetFreshBuffer();
			this.client.BeginReceive(freshBuffer, 0, freshBuffer.Length, SocketFlags.None, this.receive_cb, freshBuffer);
		}

		private void OnTimeout(object obj)
		{
			SimpleResolverEventArgs e = (SimpleResolverEventArgs)obj;
			Dictionary<int, SimpleResolverEventArgs> dictionary = this.queries;
			lock (dictionary)
			{
				SimpleResolverEventArgs e2;
				if (this.queries.TryGetValue((int)e.QueryID, out e2))
				{
					if (e != e2)
					{
						throw new Exception("Should not happen: args != args2");
					}
					SimpleResolverEventArgs e3 = e;
					e3.Retries += 1;
					if (e.Retries > 1)
					{
						e.ResolverError = ResolverError.Timeout;
						e.OnCompleted(this);
					}
					else
					{
						this.SendAQuery(e, false);
					}
				}
			}
		}

		private void OnReceive(IAsyncResult ares)
		{
			if (this.disposed)
			{
				return;
			}
			int num = 0;
			EndPoint remoteEndPoint = this.client.RemoteEndPoint;
			try
			{
				num = this.client.EndReceive(ares);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
			}
			this.BeginReceive();
			byte[] array = (byte[])ares.AsyncState;
			if (num > 12)
			{
				DnsResponse dnsResponse = new DnsResponse(array, num);
				int id = (int)dnsResponse.Header.ID;
				SimpleResolverEventArgs e = null;
				Dictionary<int, SimpleResolverEventArgs> dictionary = this.queries;
				lock (dictionary)
				{
					if (this.queries.TryGetValue(id, out e))
					{
						this.queries.Remove(id);
					}
				}
				if (e != null)
				{
					Timer timer = e.Timer;
					if (timer != null)
					{
						timer.Change(-1, -1);
					}
					try
					{
						this.ProcessResponse(e, dnsResponse, remoteEndPoint);
					}
					catch (Exception ex2)
					{
						e.ResolverError = (ResolverError)(-1);
						e.ErrorMessage = ex2.Message;
					}
					IPHostEntry hostEntry = e.HostEntry;
					if (e.ResolverError != ResolverError.NoError && e.PTRAddress != null && hostEntry != null && hostEntry.HostName != null)
					{
						e.PTRAddress = null;
						this.SendAQuery(e, hostEntry.HostName, true);
						e.Timer.Change(5000, -1);
					}
					else
					{
						e.OnCompleted(this);
					}
				}
			}
			this.FreeBuffer(array);
		}

		private void ProcessResponse(SimpleResolverEventArgs args, DnsResponse response, EndPoint server_ep)
		{
			DnsRCode rcode = response.Header.RCode;
			if (rcode != DnsRCode.NoError)
			{
				if (args.PTRAddress != null)
				{
					return;
				}
				args.ResolverError = (ResolverError)rcode;
				return;
			}
			else
			{
				if (((IPEndPoint)server_ep).Port != 53)
				{
					args.ResolverError = ResolverError.ResponseHeaderError;
					args.ErrorMessage = "Port";
					return;
				}
				DnsHeader header = response.Header;
				if (!header.IsQuery)
				{
					args.ResolverError = ResolverError.ResponseHeaderError;
					args.ErrorMessage = "IsQuery";
					return;
				}
				if (header.QuestionCount > 1)
				{
					args.ResolverError = ResolverError.ResponseHeaderError;
					args.ErrorMessage = "QuestionCount";
					return;
				}
				ReadOnlyCollection<DnsQuestion> questions = response.GetQuestions();
				if (questions.Count != 1)
				{
					args.ResolverError = ResolverError.ResponseHeaderError;
					args.ErrorMessage = "QuestionCount 2";
					return;
				}
				DnsQuestion dnsQuestion = questions[0];
				DnsQType type = dnsQuestion.Type;
				if (type != DnsQType.A && type != DnsQType.AAAA && type != DnsQType.PTR)
				{
					args.ResolverError = ResolverError.ResponseHeaderError;
					args.ErrorMessage = "QType " + dnsQuestion.Type.ToString();
					return;
				}
				if (dnsQuestion.Class != DnsQClass.Internet)
				{
					args.ResolverError = ResolverError.ResponseHeaderError;
					args.ErrorMessage = "QClass " + dnsQuestion.Class.ToString();
					return;
				}
				ReadOnlyCollection<DnsResourceRecord> answers = response.GetAnswers();
				if (answers.Count != 0)
				{
					List<string> list = null;
					List<IPAddress> list2 = null;
					foreach (DnsResourceRecord dnsResourceRecord in answers)
					{
						if (dnsResourceRecord.Class == DnsClass.Internet)
						{
							if (dnsResourceRecord.Type == DnsType.A || dnsResourceRecord.Type == DnsType.AAAA)
							{
								if (list2 == null)
								{
									list2 = new List<IPAddress>();
								}
								list2.Add(((DnsResourceRecordIPAddress)dnsResourceRecord).Address);
							}
							else if (dnsResourceRecord.Type == DnsType.CNAME)
							{
								if (list == null)
								{
									list = new List<string>();
								}
								list.Add(((DnsResourceRecordCName)dnsResourceRecord).CName);
							}
							else if (dnsResourceRecord.Type == DnsType.PTR)
							{
								args.HostEntry.HostName = ((DnsResourceRecordPTR)dnsResourceRecord).DName;
								args.HostEntry.Aliases = ((list == null) ? SimpleResolver.EmptyStrings : list.ToArray());
								args.HostEntry.AddressList = SimpleResolver.EmptyAddresses;
								return;
							}
						}
					}
					IPHostEntry iphostEntry = args.HostEntry ?? new IPHostEntry();
					if (iphostEntry.HostName == null && list != null && list.Count > 0)
					{
						iphostEntry.HostName = list[0];
						list.RemoveAt(0);
					}
					iphostEntry.Aliases = ((list == null) ? SimpleResolver.EmptyStrings : list.ToArray());
					iphostEntry.AddressList = ((list2 == null) ? SimpleResolver.EmptyAddresses : list2.ToArray());
					args.HostEntry = iphostEntry;
					if ((dnsQuestion.Type == DnsQType.A || dnsQuestion.Type == DnsQType.AAAA) && iphostEntry.AddressList == SimpleResolver.EmptyAddresses)
					{
						args.ResolverError = ResolverError.NameError;
						args.ErrorMessage = "No addresses in response";
						return;
					}
					if (dnsQuestion.Type == DnsQType.PTR && iphostEntry.HostName == null)
					{
						args.ResolverError = ResolverError.NameError;
						args.ErrorMessage = "No PTR in response";
					}
					return;
				}
				if (args.PTRAddress != null)
				{
					return;
				}
				args.ResolverError = ResolverError.NameError;
				args.ErrorMessage = "NoAnswers";
				return;
			}
		}

		private void InitFromSystem()
		{
			List<IPEndPoint> list = new List<IPEndPoint>();
			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (NetworkInterfaceType.Loopback != networkInterface.NetworkInterfaceType)
				{
					foreach (IPAddress ipaddress in networkInterface.GetIPProperties().DnsAddresses)
					{
						if (AddressFamily.InterNetworkV6 != ipaddress.AddressFamily)
						{
							IPEndPoint ipendPoint = new IPEndPoint(ipaddress, 53);
							if (!list.Contains(ipendPoint))
							{
								list.Add(ipendPoint);
							}
						}
					}
				}
			}
			this.endpoints = list.ToArray();
		}

		private static string[] EmptyStrings = new string[0];

		private static IPAddress[] EmptyAddresses = new IPAddress[0];

		private IPEndPoint[] endpoints;

		private Socket client;

		private Dictionary<int, SimpleResolverEventArgs> queries;

		private AsyncCallback receive_cb;

		private TimerCallback timeout_cb;

		private bool disposed;
	}
}
