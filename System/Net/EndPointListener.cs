using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace System.Net
{
	internal sealed class EndPointListener
	{
		public EndPointListener(HttpListener listener, IPAddress addr, int port, bool secure)
		{
			this.listener = listener;
			if (secure)
			{
				this.secure = secure;
				this.cert = listener.LoadCertificateAndKey(addr, port);
			}
			this.endpoint = new IPEndPoint(addr, port);
			this.sock = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			this.sock.Bind(this.endpoint);
			this.sock.Listen(500);
			SocketAsyncEventArgs e = new SocketAsyncEventArgs();
			e.UserToken = this;
			e.Completed += EndPointListener.OnAccept;
			Socket socket = null;
			EndPointListener.Accept(this.sock, e, ref socket);
			this.prefixes = new Hashtable();
			this.unregistered = new Dictionary<HttpConnection, HttpConnection>();
		}

		internal HttpListener Listener
		{
			get
			{
				return this.listener;
			}
		}

		private static void Accept(Socket socket, SocketAsyncEventArgs e, ref Socket accepted)
		{
			e.AcceptSocket = null;
			bool flag;
			try
			{
				flag = socket.AcceptAsync(e);
			}
			catch
			{
				if (accepted != null)
				{
					try
					{
						accepted.Close();
					}
					catch
					{
					}
					accepted = null;
				}
				return;
			}
			if (!flag)
			{
				EndPointListener.ProcessAccept(e);
			}
		}

		private static void ProcessAccept(SocketAsyncEventArgs args)
		{
			Socket socket = null;
			if (args.SocketError == SocketError.Success)
			{
				socket = args.AcceptSocket;
			}
			EndPointListener endPointListener = (EndPointListener)args.UserToken;
			EndPointListener.Accept(endPointListener.sock, args, ref socket);
			if (socket == null)
			{
				return;
			}
			if (endPointListener.secure && endPointListener.cert == null)
			{
				socket.Close();
				return;
			}
			HttpConnection httpConnection;
			try
			{
				httpConnection = new HttpConnection(socket, endPointListener, endPointListener.secure, endPointListener.cert);
			}
			catch
			{
				socket.Close();
				return;
			}
			Dictionary<HttpConnection, HttpConnection> dictionary = endPointListener.unregistered;
			lock (dictionary)
			{
				endPointListener.unregistered[httpConnection] = httpConnection;
			}
			httpConnection.BeginReadRequest();
		}

		private static void OnAccept(object sender, SocketAsyncEventArgs e)
		{
			EndPointListener.ProcessAccept(e);
		}

		internal void RemoveConnection(HttpConnection conn)
		{
			Dictionary<HttpConnection, HttpConnection> dictionary = this.unregistered;
			lock (dictionary)
			{
				this.unregistered.Remove(conn);
			}
		}

		public bool BindContext(HttpListenerContext context)
		{
			HttpListenerRequest request = context.Request;
			ListenerPrefix listenerPrefix;
			HttpListener httpListener = this.SearchListener(request.Url, out listenerPrefix);
			if (httpListener == null)
			{
				return false;
			}
			context.Listener = httpListener;
			context.Connection.Prefix = listenerPrefix;
			return true;
		}

		public void UnbindContext(HttpListenerContext context)
		{
			if (context == null || context.Request == null)
			{
				return;
			}
			context.Listener.UnregisterContext(context);
		}

		private HttpListener SearchListener(Uri uri, out ListenerPrefix prefix)
		{
			prefix = null;
			if (uri == null)
			{
				return null;
			}
			string host = uri.Host;
			int port = uri.Port;
			string text = WebUtility.UrlDecode(uri.AbsolutePath);
			string text2 = ((text[text.Length - 1] == '/') ? text : (text + "/"));
			HttpListener httpListener = null;
			int num = -1;
			if (host != null && host != "")
			{
				Hashtable hashtable = this.prefixes;
				foreach (object obj in hashtable.Keys)
				{
					ListenerPrefix listenerPrefix = (ListenerPrefix)obj;
					string path = listenerPrefix.Path;
					if (path.Length >= num && !(listenerPrefix.Host != host) && listenerPrefix.Port == port && (text.StartsWith(path) || text2.StartsWith(path)))
					{
						num = path.Length;
						httpListener = (HttpListener)hashtable[listenerPrefix];
						prefix = listenerPrefix;
					}
				}
				if (num != -1)
				{
					return httpListener;
				}
			}
			ArrayList arrayList = this.unhandled;
			httpListener = this.MatchFromList(host, text, arrayList, out prefix);
			if (text != text2 && httpListener == null)
			{
				httpListener = this.MatchFromList(host, text2, arrayList, out prefix);
			}
			if (httpListener != null)
			{
				return httpListener;
			}
			arrayList = this.all;
			httpListener = this.MatchFromList(host, text, arrayList, out prefix);
			if (text != text2 && httpListener == null)
			{
				httpListener = this.MatchFromList(host, text2, arrayList, out prefix);
			}
			if (httpListener != null)
			{
				return httpListener;
			}
			return null;
		}

		private HttpListener MatchFromList(string host, string path, ArrayList list, out ListenerPrefix prefix)
		{
			prefix = null;
			if (list == null)
			{
				return null;
			}
			HttpListener httpListener = null;
			int num = -1;
			foreach (object obj in list)
			{
				ListenerPrefix listenerPrefix = (ListenerPrefix)obj;
				string path2 = listenerPrefix.Path;
				if (path2.Length >= num && path.StartsWith(path2))
				{
					num = path2.Length;
					httpListener = listenerPrefix.Listener;
					prefix = listenerPrefix;
				}
			}
			return httpListener;
		}

		private void AddSpecial(ArrayList coll, ListenerPrefix prefix)
		{
			if (coll == null)
			{
				return;
			}
			using (IEnumerator enumerator = coll.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((ListenerPrefix)enumerator.Current).Path == prefix.Path)
					{
						throw new HttpListenerException(400, "Prefix already in use.");
					}
				}
			}
			coll.Add(prefix);
		}

		private bool RemoveSpecial(ArrayList coll, ListenerPrefix prefix)
		{
			if (coll == null)
			{
				return false;
			}
			int count = coll.Count;
			for (int i = 0; i < count; i++)
			{
				if (((ListenerPrefix)coll[i]).Path == prefix.Path)
				{
					coll.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		private void CheckIfRemove()
		{
			if (this.prefixes.Count > 0)
			{
				return;
			}
			ArrayList arrayList = this.unhandled;
			if (arrayList != null && arrayList.Count > 0)
			{
				return;
			}
			arrayList = this.all;
			if (arrayList != null && arrayList.Count > 0)
			{
				return;
			}
			EndPointManager.RemoveEndPoint(this, this.endpoint);
		}

		public void Close()
		{
			this.sock.Close();
			Dictionary<HttpConnection, HttpConnection> dictionary = this.unregistered;
			lock (dictionary)
			{
				foreach (HttpConnection httpConnection in new List<HttpConnection>(this.unregistered.Keys))
				{
					httpConnection.Close(true);
				}
				this.unregistered.Clear();
			}
		}

		public void AddPrefix(ListenerPrefix prefix, HttpListener listener)
		{
			if (prefix.Host == "*")
			{
				ArrayList arrayList;
				ArrayList arrayList2;
				do
				{
					arrayList = this.unhandled;
					arrayList2 = ((arrayList != null) ? ((ArrayList)arrayList.Clone()) : new ArrayList());
					prefix.Listener = listener;
					this.AddSpecial(arrayList2, prefix);
				}
				while (Interlocked.CompareExchange<ArrayList>(ref this.unhandled, arrayList2, arrayList) != arrayList);
				return;
			}
			if (prefix.Host == "+")
			{
				ArrayList arrayList;
				ArrayList arrayList2;
				do
				{
					arrayList = this.all;
					arrayList2 = ((arrayList != null) ? ((ArrayList)arrayList.Clone()) : new ArrayList());
					prefix.Listener = listener;
					this.AddSpecial(arrayList2, prefix);
				}
				while (Interlocked.CompareExchange<ArrayList>(ref this.all, arrayList2, arrayList) != arrayList);
				return;
			}
			Hashtable hashtable;
			for (;;)
			{
				hashtable = this.prefixes;
				if (hashtable.ContainsKey(prefix))
				{
					break;
				}
				Hashtable hashtable2 = (Hashtable)hashtable.Clone();
				hashtable2[prefix] = listener;
				if (Interlocked.CompareExchange<Hashtable>(ref this.prefixes, hashtable2, hashtable) == hashtable)
				{
					return;
				}
			}
			if ((HttpListener)hashtable[prefix] != listener)
			{
				throw new HttpListenerException(400, "There's another listener for " + prefix);
			}
			return;
		}

		public void RemovePrefix(ListenerPrefix prefix, HttpListener listener)
		{
			if (prefix.Host == "*")
			{
				ArrayList arrayList;
				ArrayList arrayList2;
				do
				{
					arrayList = this.unhandled;
					arrayList2 = ((arrayList != null) ? ((ArrayList)arrayList.Clone()) : new ArrayList());
				}
				while (this.RemoveSpecial(arrayList2, prefix) && Interlocked.CompareExchange<ArrayList>(ref this.unhandled, arrayList2, arrayList) != arrayList);
				this.CheckIfRemove();
				return;
			}
			if (prefix.Host == "+")
			{
				ArrayList arrayList;
				ArrayList arrayList2;
				do
				{
					arrayList = this.all;
					arrayList2 = ((arrayList != null) ? ((ArrayList)arrayList.Clone()) : new ArrayList());
				}
				while (this.RemoveSpecial(arrayList2, prefix) && Interlocked.CompareExchange<ArrayList>(ref this.all, arrayList2, arrayList) != arrayList);
				this.CheckIfRemove();
				return;
			}
			Hashtable hashtable;
			Hashtable hashtable2;
			do
			{
				hashtable = this.prefixes;
				if (!hashtable.ContainsKey(prefix))
				{
					break;
				}
				hashtable2 = (Hashtable)hashtable.Clone();
				hashtable2.Remove(prefix);
			}
			while (Interlocked.CompareExchange<Hashtable>(ref this.prefixes, hashtable2, hashtable) != hashtable);
			this.CheckIfRemove();
		}

		private HttpListener listener;

		private IPEndPoint endpoint;

		private Socket sock;

		private Hashtable prefixes;

		private ArrayList unhandled;

		private ArrayList all;

		private X509Certificate cert;

		private bool secure;

		private Dictionary<HttpConnection, HttpConnection> unregistered;
	}
}
