using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixBinaryClientFormatterSink : IClientFormatterSink, IMessageSink, IClientChannelSink, IChannelSinkBase
	{
		public UnixBinaryClientFormatterSink(IClientChannelSink nextSink)
		{
			this._nextInChain = nextSink;
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

		public IClientChannelSink NextChannelSink
		{
			get
			{
				return this._nextInChain;
			}
		}

		public IMessageSink NextSink
		{
			get
			{
				return null;
			}
		}

		public IDictionary Properties
		{
			get
			{
				return null;
			}
		}

		public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			throw new NotSupportedException("UnixBinaryClientFormatterSink must be the first sink in the IClientChannelSink chain");
		}

		public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream)
		{
			IMessage message = (IMessage)this._binaryCore.Deserializer.DeserializeMethodResponse(stream, null, (IMethodCallMessage)state);
			sinkStack.DispatchReplyMessage(message);
		}

		public Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
		{
			throw new NotSupportedException();
		}

		public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			throw new NotSupportedException();
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			ITransportHeaders transportHeaders = new TransportHeaders();
			Stream stream = this._nextInChain.GetRequestStream(msg, transportHeaders);
			if (stream == null)
			{
				stream = new MemoryStream();
			}
			this._binaryCore.Serializer.Serialize(stream, msg, null);
			if (stream is MemoryStream)
			{
				stream.Position = 0L;
			}
			ClientChannelSinkStack clientChannelSinkStack = new ClientChannelSinkStack(replySink);
			clientChannelSinkStack.Push(this, msg);
			this._nextInChain.AsyncProcessRequest(clientChannelSinkStack, msg, transportHeaders, stream);
			return null;
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			IMessage message;
			try
			{
				ITransportHeaders transportHeaders = new TransportHeaders();
				transportHeaders["__RequestUri"] = ((IMethodCallMessage)msg).Uri;
				transportHeaders["Content-Type"] = "application/octet-stream";
				Stream stream = this._nextInChain.GetRequestStream(msg, transportHeaders);
				if (stream == null)
				{
					stream = new MemoryStream();
				}
				this._binaryCore.Serializer.Serialize(stream, msg, null);
				if (stream is MemoryStream)
				{
					stream.Position = 0L;
				}
				ITransportHeaders transportHeaders2;
				Stream stream2;
				this._nextInChain.ProcessMessage(msg, transportHeaders, stream, out transportHeaders2, out stream2);
				message = (IMessage)this._binaryCore.Deserializer.DeserializeMethodResponse(stream2, null, (IMethodCallMessage)msg);
			}
			catch (Exception ex)
			{
				message = new ReturnMessage(ex, (IMethodCallMessage)msg);
			}
			return message;
		}

		private UnixBinaryCore _binaryCore = UnixBinaryCore.DefaultInstance;

		private IClientChannelSink _nextInChain;
	}
}
