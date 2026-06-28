using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixBinaryServerFormatterSink : IServerChannelSink, IChannelSinkBase
	{
		public UnixBinaryServerFormatterSink(IServerChannelSink nextSink, IChannelReceiver receiver)
		{
			this.next_sink = nextSink;
			this.receiver = receiver;
		}

		internal UnixBinaryCore BinaryCore
		{
			get
			{
				return this._binaryCore;
			}
			set
			{
				this._binaryCore = value;
			}
		}

		public IServerChannelSink NextChannelSink
		{
			get
			{
				return this.next_sink;
			}
		}

		public IDictionary Properties
		{
			get
			{
				return null;
			}
		}

		public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage message, ITransportHeaders headers, Stream stream)
		{
			ITransportHeaders transportHeaders = new TransportHeaders();
			if (sinkStack != null)
			{
				stream = sinkStack.GetResponseStream(message, transportHeaders);
			}
			if (stream == null)
			{
				stream = new MemoryStream();
			}
			this._binaryCore.Serializer.Serialize(stream, message, null);
			if (stream is MemoryStream)
			{
				stream.Position = 0L;
			}
			sinkStack.AsyncProcessResponse(message, transportHeaders, stream);
		}

		public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
		{
			return null;
		}

		public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			sinkStack.Push(this, null);
			ServerProcessing serverProcessing;
			try
			{
				string text = (string)requestHeaders["__RequestUri"];
				string text2;
				this.receiver.Parse(text, out text2);
				if (text2 == null)
				{
					text2 = text;
				}
				MethodCallHeaderHandler methodCallHeaderHandler = new MethodCallHeaderHandler(text2);
				requestMsg = (IMessage)this._binaryCore.Deserializer.Deserialize(requestStream, new HeaderHandler(methodCallHeaderHandler.HandleHeaders));
				serverProcessing = this.next_sink.ProcessMessage(sinkStack, requestMsg, requestHeaders, null, out responseMsg, out responseHeaders, out responseStream);
			}
			catch (Exception ex)
			{
				responseMsg = new ReturnMessage(ex, (IMethodCallMessage)requestMsg);
				serverProcessing = ServerProcessing.Complete;
				responseHeaders = null;
				responseStream = null;
			}
			if (serverProcessing == ServerProcessing.Complete)
			{
				for (int i = 0; i < 3; i++)
				{
					responseStream = null;
					responseHeaders = new TransportHeaders();
					if (sinkStack != null)
					{
						responseStream = sinkStack.GetResponseStream(responseMsg, responseHeaders);
					}
					if (responseStream == null)
					{
						responseStream = new MemoryStream();
					}
					try
					{
						this._binaryCore.Serializer.Serialize(responseStream, responseMsg);
						break;
					}
					catch (Exception ex2)
					{
						if (i == 2)
						{
							throw ex2;
						}
						responseMsg = new ReturnMessage(ex2, (IMethodCallMessage)requestMsg);
					}
				}
				if (responseStream is MemoryStream)
				{
					responseStream.Position = 0L;
				}
				sinkStack.Pop(this);
			}
			return serverProcessing;
		}

		private UnixBinaryCore _binaryCore = UnixBinaryCore.DefaultInstance;

		private IServerChannelSink next_sink;

		private IChannelReceiver receiver;
	}
}
