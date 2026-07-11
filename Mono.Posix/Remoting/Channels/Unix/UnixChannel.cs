using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace Mono.Remoting.Channels.Unix
{
	public class UnixChannel : IChannelReceiver, IChannel, IChannelSender
	{
		public UnixChannel()
			: this(null)
		{
		}

		public UnixChannel(string path)
		{
			this._name = "unix";
			this._priority = 1;
			base..ctor();
			Hashtable hashtable = new Hashtable();
			hashtable["path"] = path;
			this.Init(hashtable, null, null);
		}

		private void Init(IDictionary properties, IClientChannelSinkProvider clientSink, IServerChannelSinkProvider serverSink)
		{
			this._clientChannel = new UnixClientChannel(properties, clientSink);
			if (properties["path"] != null)
			{
				this._serverChannel = new UnixServerChannel(properties, serverSink);
			}
			object obj = properties["name"];
			if (obj != null)
			{
				this._name = obj as string;
			}
			obj = properties["priority"];
			if (obj != null)
			{
				this._priority = Convert.ToInt32(obj);
			}
		}

		public UnixChannel(IDictionary properties, IClientChannelSinkProvider clientSinkProvider, IServerChannelSinkProvider serverSinkProvider)
		{
			this._name = "unix";
			this._priority = 1;
			base..ctor();
			this.Init(properties, clientSinkProvider, serverSinkProvider);
		}

		public IMessageSink CreateMessageSink(string url, object remoteChannelData, out string objectURI)
		{
			return this._clientChannel.CreateMessageSink(url, remoteChannelData, out objectURI);
		}

		public string ChannelName
		{
			get
			{
				return this._name;
			}
		}

		public int ChannelPriority
		{
			get
			{
				return this._priority;
			}
		}

		public void StartListening(object data)
		{
			if (this._serverChannel != null)
			{
				this._serverChannel.StartListening(data);
			}
		}

		public void StopListening(object data)
		{
			if (this._serverChannel != null)
			{
				this._serverChannel.StopListening(data);
			}
		}

		public string[] GetUrlsForUri(string uri)
		{
			if (this._serverChannel != null)
			{
				return this._serverChannel.GetUrlsForUri(uri);
			}
			return null;
		}

		public object ChannelData
		{
			get
			{
				if (this._serverChannel != null)
				{
					return this._serverChannel.ChannelData;
				}
				return null;
			}
		}

		public string Parse(string url, out string objectURI)
		{
			return UnixChannel.ParseUnixURL(url, out objectURI);
		}

		internal static string ParseUnixURL(string url, out string objectURI)
		{
			objectURI = null;
			if (!url.StartsWith("unix://"))
			{
				return null;
			}
			int num = url.IndexOf('?');
			if (num == -1)
			{
				return url.Substring(7);
			}
			objectURI = url.Substring(num + 1);
			if (objectURI.Length == 0)
			{
				objectURI = null;
			}
			return url.Substring(7, num - 7);
		}

		private UnixClientChannel _clientChannel;

		private UnixServerChannel _serverChannel;

		private string _name;

		private int _priority;
	}
}
