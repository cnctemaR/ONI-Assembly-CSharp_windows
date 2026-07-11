using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Net
{
	internal class WebConnectionGroup
	{
		public WebConnectionGroup(ServicePoint sPoint, string name)
		{
			this.sPoint = sPoint;
			this.name = name;
			this.connections = new LinkedList<WebConnectionGroup.ConnectionState>();
			this.queue = new Queue();
		}

		public event EventHandler ConnectionClosed;

		private void OnConnectionClosed()
		{
			if (this.ConnectionClosed != null)
			{
				this.ConnectionClosed(this, null);
			}
		}

		public void Close()
		{
			List<WebConnection> list = null;
			ServicePoint servicePoint = this.sPoint;
			lock (servicePoint)
			{
				this.closing = true;
				LinkedListNode<WebConnectionGroup.ConnectionState> linkedListNode = this.connections.First;
				while (linkedListNode != null)
				{
					WebConnection connection = linkedListNode.Value.Connection;
					LinkedListNode<WebConnectionGroup.ConnectionState> linkedListNode2 = linkedListNode;
					linkedListNode = linkedListNode.Next;
					if (list == null)
					{
						list = new List<WebConnection>();
					}
					list.Add(connection);
					this.connections.Remove(linkedListNode2);
				}
			}
			if (list != null)
			{
				foreach (WebConnection webConnection in list)
				{
					webConnection.Close(false);
					this.OnConnectionClosed();
				}
			}
		}

		public WebConnection GetConnection(HttpWebRequest request, out bool created)
		{
			ServicePoint servicePoint = this.sPoint;
			WebConnection webConnection;
			lock (servicePoint)
			{
				webConnection = this.CreateOrReuseConnection(request, out created);
			}
			return webConnection;
		}

		private static void PrepareSharingNtlm(WebConnection cnc, HttpWebRequest request)
		{
			if (!cnc.NtlmAuthenticated)
			{
				return;
			}
			bool flag = false;
			NetworkCredential ntlmCredential = cnc.NtlmCredential;
			ICredentials credentials = ((request.Proxy == null || request.Proxy.IsBypassed(request.RequestUri)) ? request.Credentials : request.Proxy.Credentials);
			NetworkCredential networkCredential = ((credentials != null) ? credentials.GetCredential(request.RequestUri, "NTLM") : null);
			if (ntlmCredential == null || networkCredential == null || ntlmCredential.Domain != networkCredential.Domain || ntlmCredential.UserName != networkCredential.UserName || ntlmCredential.Password != networkCredential.Password)
			{
				flag = true;
			}
			if (!flag)
			{
				bool unsafeAuthenticatedConnectionSharing = request.UnsafeAuthenticatedConnectionSharing;
				bool unsafeAuthenticatedConnectionSharing2 = cnc.UnsafeAuthenticatedConnectionSharing;
				flag = !unsafeAuthenticatedConnectionSharing || unsafeAuthenticatedConnectionSharing != unsafeAuthenticatedConnectionSharing2;
			}
			if (flag)
			{
				cnc.Close(false);
				cnc.ResetNtlm();
			}
		}

		private WebConnectionGroup.ConnectionState FindIdleConnection()
		{
			foreach (WebConnectionGroup.ConnectionState connectionState in this.connections)
			{
				if (!connectionState.Busy)
				{
					this.connections.Remove(connectionState);
					this.connections.AddFirst(connectionState);
					return connectionState;
				}
			}
			return null;
		}

		private WebConnection CreateOrReuseConnection(HttpWebRequest request, out bool created)
		{
			WebConnectionGroup.ConnectionState connectionState = this.FindIdleConnection();
			if (connectionState != null)
			{
				created = false;
				WebConnectionGroup.PrepareSharingNtlm(connectionState.Connection, request);
				return connectionState.Connection;
			}
			if (this.sPoint.ConnectionLimit > this.connections.Count || this.connections.Count == 0)
			{
				created = true;
				connectionState = new WebConnectionGroup.ConnectionState(this);
				this.connections.AddFirst(connectionState);
				return connectionState.Connection;
			}
			created = false;
			connectionState = this.connections.Last.Value;
			this.connections.Remove(connectionState);
			this.connections.AddFirst(connectionState);
			return connectionState.Connection;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		internal Queue Queue
		{
			get
			{
				return this.queue;
			}
		}

		internal bool TryRecycle(TimeSpan maxIdleTime, ref DateTime idleSince)
		{
			DateTime utcNow = DateTime.UtcNow;
			bool flag2;
			for (;;)
			{
				List<WebConnection> list = null;
				ServicePoint servicePoint = this.sPoint;
				lock (servicePoint)
				{
					if (this.closing)
					{
						idleSince = DateTime.MinValue;
						return true;
					}
					int num = 0;
					LinkedListNode<WebConnectionGroup.ConnectionState> linkedListNode = this.connections.First;
					while (linkedListNode != null)
					{
						WebConnectionGroup.ConnectionState value = linkedListNode.Value;
						LinkedListNode<WebConnectionGroup.ConnectionState> linkedListNode2 = linkedListNode;
						linkedListNode = linkedListNode.Next;
						num++;
						if (!value.Busy)
						{
							if (num <= this.sPoint.ConnectionLimit && utcNow - value.IdleSince < maxIdleTime)
							{
								if (value.IdleSince > idleSince)
								{
									idleSince = value.IdleSince;
								}
							}
							else
							{
								if (list == null)
								{
									list = new List<WebConnection>();
								}
								list.Add(value.Connection);
								this.connections.Remove(linkedListNode2);
							}
						}
					}
					flag2 = this.connections.Count == 0;
				}
				if (list == null)
				{
					break;
				}
				using (List<WebConnection>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						WebConnection webConnection = enumerator.Current;
						webConnection.Close(false);
					}
					continue;
				}
				bool flag3;
				return flag3;
			}
			return flag2;
		}

		private ServicePoint sPoint;

		private string name;

		private LinkedList<WebConnectionGroup.ConnectionState> connections;

		private Queue queue;

		private bool closing;

		private class ConnectionState : IWebConnectionState
		{
			public WebConnection Connection { get; private set; }

			public WebConnectionGroup Group { get; private set; }

			public ServicePoint ServicePoint
			{
				get
				{
					return this.Group.sPoint;
				}
			}

			public bool Busy
			{
				get
				{
					return this.busy;
				}
			}

			public DateTime IdleSince
			{
				get
				{
					return this.idleSince;
				}
			}

			public bool TrySetBusy()
			{
				ServicePoint servicePoint = this.ServicePoint;
				bool flag2;
				lock (servicePoint)
				{
					if (this.busy)
					{
						flag2 = false;
					}
					else
					{
						this.busy = true;
						this.idleSince = DateTime.UtcNow + TimeSpan.FromDays(3650.0);
						flag2 = true;
					}
				}
				return flag2;
			}

			public void SetIdle()
			{
				ServicePoint servicePoint = this.ServicePoint;
				lock (servicePoint)
				{
					this.busy = false;
					this.idleSince = DateTime.UtcNow;
				}
			}

			public ConnectionState(WebConnectionGroup group)
			{
				this.Group = group;
				this.idleSince = DateTime.UtcNow;
				this.Connection = new WebConnection(this, group.sPoint);
			}

			private bool busy;

			private DateTime idleSince;
		}
	}
}
