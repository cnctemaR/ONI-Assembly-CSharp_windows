using System;
using System.Collections;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Threading;
using Mono.Unix;
using Mono.Unix.Native;

namespace Mono.Remoting.Channels.Unix
{
	public class UnixServerChannel : IChannel, IChannelReceiver
	{
		public UnixServerChannel(string path)
		{
			this.path = path;
			this.Init(null);
		}

		public UnixServerChannel(IDictionary properties, IServerChannelSinkProvider serverSinkProvider)
		{
			foreach (object obj in properties)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				string text = (string)dictionaryEntry.Key;
				switch (text)
				{
				case "path":
					this.path = dictionaryEntry.Value as string;
					break;
				case "priority":
					this.priority = Convert.ToInt32(dictionaryEntry.Value);
					break;
				case "supressChannelData":
					this.supressChannelData = Convert.ToBoolean(dictionaryEntry.Value);
					break;
				}
			}
			this.Init(serverSinkProvider);
		}

		public UnixServerChannel(string name, string path, IServerChannelSinkProvider serverSinkProvider)
		{
			this.name = name;
			this.path = path;
			this.Init(serverSinkProvider);
		}

		public UnixServerChannel(string name, string path)
		{
			this.name = name;
			this.path = path;
			this.Init(null);
		}

		private void Init(IServerChannelSinkProvider serverSinkProvider)
		{
			if (serverSinkProvider == null)
			{
				serverSinkProvider = new UnixBinaryServerFormatterSinkProvider();
			}
			this.channel_data = new ChannelDataStore(null);
			for (IServerChannelSinkProvider serverChannelSinkProvider = serverSinkProvider; serverChannelSinkProvider != null; serverChannelSinkProvider = serverChannelSinkProvider.Next)
			{
				serverChannelSinkProvider.GetChannelData(this.channel_data);
			}
			IServerChannelSink serverChannelSink = ChannelServices.CreateServerChannelSinkChain(serverSinkProvider, this);
			this.sink = new UnixServerTransportSink(serverChannelSink);
			this.StartListening(null);
		}

		public object ChannelData
		{
			get
			{
				if (this.supressChannelData)
				{
					return null;
				}
				return this.channel_data;
			}
		}

		public string ChannelName
		{
			get
			{
				return this.name;
			}
		}

		public int ChannelPriority
		{
			get
			{
				return this.priority;
			}
		}

		public string GetChannelUri()
		{
			return "unix://" + this.path;
		}

		public string[] GetUrlsForUri(string uri)
		{
			if (!uri.StartsWith("/"))
			{
				uri = "/" + uri;
			}
			string[] channelUris = this.channel_data.ChannelUris;
			string[] array = new string[channelUris.Length];
			for (int i = 0; i < channelUris.Length; i++)
			{
				array[i] = channelUris[i] + "?" + uri;
			}
			return array;
		}

		public string Parse(string url, out string objectURI)
		{
			return UnixChannel.ParseUnixURL(url, out objectURI);
		}

		private void WaitForConnections()
		{
			try
			{
				for (;;)
				{
					Socket socket = this.listener.AcceptSocket();
					this.CreateListenerConnection(socket);
				}
			}
			catch
			{
			}
		}

		internal void CreateListenerConnection(Socket client)
		{
			ArrayList activeConnections = this._activeConnections;
			lock (activeConnections)
			{
				if (this._activeConnections.Count >= this._maxConcurrentConnections)
				{
					Monitor.Wait(this._activeConnections);
				}
				if (this.server_thread != null)
				{
					ClientConnection clientConnection = new ClientConnection(this, client, this.sink);
					Thread thread = new Thread(new ThreadStart(clientConnection.ProcessMessages));
					thread.Start();
					thread.IsBackground = true;
					this._activeConnections.Add(thread);
				}
			}
		}

		internal void ReleaseConnection(Thread thread)
		{
			ArrayList activeConnections = this._activeConnections;
			lock (activeConnections)
			{
				this._activeConnections.Remove(thread);
				Monitor.Pulse(this._activeConnections);
			}
		}

		public void StartListening(object data)
		{
			this.listener = new UnixListener(this.path);
			Syscall.chmod(this.path, FilePermissions.DEFFILEMODE);
			if (this.server_thread == null)
			{
				this.listener.Start();
				string[] array = new string[1];
				array = new string[] { this.GetChannelUri() };
				this.channel_data.ChannelUris = array;
				this.server_thread = new Thread(new ThreadStart(this.WaitForConnections));
				this.server_thread.IsBackground = true;
				this.server_thread.Start();
			}
		}

		public void StopListening(object data)
		{
			if (this.server_thread == null)
			{
				return;
			}
			ArrayList activeConnections = this._activeConnections;
			lock (activeConnections)
			{
				this.server_thread.Abort();
				this.server_thread = null;
				this.listener.Stop();
				foreach (object obj in this._activeConnections)
				{
					Thread thread = (Thread)obj;
					thread.Abort();
				}
				this._activeConnections.Clear();
				Monitor.PulseAll(this._activeConnections);
			}
		}

		private string path;

		private string name = "unix";

		private int priority = 1;

		private bool supressChannelData;

		private Thread server_thread;

		private UnixListener listener;

		private UnixServerTransportSink sink;

		private ChannelDataStore channel_data;

		private int _maxConcurrentConnections = 100;

		private ArrayList _activeConnections = new ArrayList();
	}
}
