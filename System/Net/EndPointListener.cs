using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Authenticode;

namespace System.Net
{
	internal sealed class EndPointListener
	{
		public EndPointListener(IPAddress addr, int port, bool secure)
		{
			if (secure)
			{
				this.secure = secure;
				this.LoadCertificateAndKey(addr, port);
			}
			this.endpoint = new IPEndPoint(addr, port);
			this.sock = new global::System.Net.Sockets.Socket(addr.AddressFamily, global::System.Net.Sockets.SocketType.Stream, global::System.Net.Sockets.ProtocolType.Tcp);
			this.sock.Bind(this.endpoint);
			this.sock.Listen(500);
			this.sock.BeginAccept(new AsyncCallback(EndPointListener.OnAccept), this);
			this.prefixes = new Hashtable();
		}

		private void LoadCertificateAndKey(IPAddress addr, int port)
		{
			try
			{
				string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string text = Path.Combine(folderPath, ".mono");
				text = Path.Combine(text, "httplistener");
				string text2 = Path.Combine(text, string.Format("{0}.cer", port));
				string text3 = Path.Combine(text, string.Format("{0}.pvk", port));
				this.cert = new global::System.Security.Cryptography.X509Certificates.X509Certificate2(text2);
				this.key = PrivateKey.CreateFromFile(text3).RSA;
			}
			catch
			{
			}
		}

		private static void OnAccept(IAsyncResult ares)
		{
			EndPointListener endPointListener = (EndPointListener)ares.AsyncState;
			global::System.Net.Sockets.Socket socket = null;
			try
			{
				socket = endPointListener.sock.EndAccept(ares);
			}
			catch
			{
			}
			finally
			{
				try
				{
					endPointListener.sock.BeginAccept(new AsyncCallback(EndPointListener.OnAccept), endPointListener);
				}
				catch
				{
					if (socket != null)
					{
						try
						{
							socket.Close();
						}
						catch
						{
						}
						socket = null;
					}
				}
			}
			if (socket == null)
			{
				return;
			}
			if (endPointListener.secure && (endPointListener.cert == null || endPointListener.key == null))
			{
				socket.Close();
				return;
			}
			HttpConnection httpConnection = new HttpConnection(socket, endPointListener, endPointListener.secure, endPointListener.cert, endPointListener.key);
			httpConnection.BeginReadRequest();
		}

		public bool BindContext(HttpListenerContext context)
		{
			HttpListenerRequest request = context.Request;
			ListenerPrefix listenerPrefix;
			HttpListener httpListener = this.SearchListener(request.UserHostName, request.Url, out listenerPrefix);
			if (httpListener == null)
			{
				return false;
			}
			context.Listener = httpListener;
			context.Connection.Prefix = listenerPrefix;
			httpListener.RegisterContext(context);
			return true;
		}

		public void UnbindContext(HttpListenerContext context)
		{
			if (context == null || context.Request == null)
			{
				return;
			}
			HttpListenerRequest request = context.Request;
			ListenerPrefix listenerPrefix;
			HttpListener httpListener = this.SearchListener(request.UserHostName, request.Url, out listenerPrefix);
			if (httpListener != null)
			{
				httpListener.UnregisterContext(context);
			}
		}

		private HttpListener SearchListener(string host, global::System.Uri uri, out ListenerPrefix prefix)
		{
			prefix = null;
			if (uri == null)
			{
				return null;
			}
			if (host != null)
			{
				int num = host.IndexOf(':');
				if (num >= 0)
				{
					host = host.Substring(0, num);
				}
			}
			string text = HttpUtility.UrlDecode(uri.AbsolutePath);
			string text2 = ((text[text.Length - 1] != '/') ? (text + "/") : text);
			HttpListener httpListener = null;
			int num2 = -1;
			Hashtable hashtable = this.prefixes;
			lock (hashtable)
			{
				if (host != null && host != string.Empty)
				{
					foreach (object obj in this.prefixes.Keys)
					{
						ListenerPrefix listenerPrefix = (ListenerPrefix)obj;
						string path = listenerPrefix.Path;
						if (path.Length >= num2)
						{
							if (listenerPrefix.Host == host && (text.StartsWith(path) || text2.StartsWith(path)))
							{
								num2 = path.Length;
								httpListener = (HttpListener)this.prefixes[listenerPrefix];
								prefix = listenerPrefix;
							}
						}
					}
					if (num2 != -1)
					{
						return httpListener;
					}
				}
				httpListener = this.MatchFromList(host, text, this.unhandled, out prefix);
				if (httpListener != null)
				{
					return httpListener;
				}
				httpListener = this.MatchFromList(host, text, this.all, out prefix);
				if (httpListener != null)
				{
					return httpListener;
				}
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
				if (path2.Length >= num)
				{
					if (path.StartsWith(path2))
					{
						num = path2.Length;
						httpListener = listenerPrefix.Listener;
						prefix = listenerPrefix;
					}
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
			foreach (object obj in coll)
			{
				ListenerPrefix listenerPrefix = (ListenerPrefix)obj;
				if (listenerPrefix.Path == prefix.Path)
				{
					throw new HttpListenerException(400, "Prefix already in use.");
				}
			}
			coll.Add(prefix);
		}

		private void RemoveSpecial(ArrayList coll, ListenerPrefix prefix)
		{
			if (coll == null)
			{
				return;
			}
			int count = coll.Count;
			for (int i = 0; i < count; i++)
			{
				ListenerPrefix listenerPrefix = (ListenerPrefix)coll[i];
				if (listenerPrefix.Path == prefix.Path)
				{
					coll.RemoveAt(i);
					this.CheckIfRemove();
					return;
				}
			}
		}

		private void CheckIfRemove()
		{
			if (this.prefixes.Count > 0)
			{
				return;
			}
			if (this.unhandled != null && this.unhandled.Count > 0)
			{
				return;
			}
			if (this.all != null && this.all.Count > 0)
			{
				return;
			}
			EndPointManager.RemoveEndPoint(this, this.endpoint);
		}

		public void Close()
		{
			this.sock.Close();
		}

		public void AddPrefix(ListenerPrefix prefix, HttpListener listener)
		{
			Hashtable hashtable = this.prefixes;
			lock (hashtable)
			{
				if (prefix.Host == "*")
				{
					if (this.unhandled == null)
					{
						this.unhandled = new ArrayList();
					}
					prefix.Listener = listener;
					this.AddSpecial(this.unhandled, prefix);
				}
				else if (prefix.Host == "+")
				{
					if (this.all == null)
					{
						this.all = new ArrayList();
					}
					prefix.Listener = listener;
					this.AddSpecial(this.all, prefix);
				}
				else if (this.prefixes.ContainsKey(prefix))
				{
					HttpListener httpListener = (HttpListener)this.prefixes[prefix];
					if (httpListener != listener)
					{
						throw new HttpListenerException(400, "There's another listener for " + prefix);
					}
				}
				else
				{
					this.prefixes[prefix] = listener;
				}
			}
		}

		public void RemovePrefix(ListenerPrefix prefix, HttpListener listener)
		{
			Hashtable hashtable = this.prefixes;
			lock (hashtable)
			{
				if (prefix.Host == "*")
				{
					this.RemoveSpecial(this.unhandled, prefix);
				}
				else if (prefix.Host == "+")
				{
					this.RemoveSpecial(this.all, prefix);
				}
				else if (this.prefixes.ContainsKey(prefix))
				{
					this.prefixes.Remove(prefix);
					this.CheckIfRemove();
				}
			}
		}

		private IPEndPoint endpoint;

		private global::System.Net.Sockets.Socket sock;

		private Hashtable prefixes;

		private ArrayList unhandled;

		private ArrayList all;

		private global::System.Security.Cryptography.X509Certificates.X509Certificate2 cert;

		private AsymmetricAlgorithm key;

		private bool secure;
	}
}
