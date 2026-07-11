using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixServerTransportSink : IServerChannelSink, IChannelSinkBase
	{
		public UnixServerTransportSink(IServerChannelSink next)
		{
			this.next_sink = next;
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
				if (this.next_sink != null)
				{
					return this.next_sink.Properties;
				}
				return null;
			}
		}

		public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, Stream responseStream)
		{
			ClientConnection clientConnection = (ClientConnection)state;
			NetworkStream networkStream = new NetworkStream(clientConnection.Client);
			UnixMessageIO.SendMessageStream(networkStream, responseStream, headers, clientConnection.Buffer);
			networkStream.Flush();
			networkStream.Close();
		}

		public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
		{
			return null;
		}

		public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			throw new NotSupportedException();
		}

		internal void InternalProcessMessage(ClientConnection connection, Stream stream)
		{
			ITransportHeaders transportHeaders;
			Stream stream2 = UnixMessageIO.ReceiveMessageStream(stream, out transportHeaders, connection.Buffer);
			ServerChannelSinkStack serverChannelSinkStack = new ServerChannelSinkStack();
			serverChannelSinkStack.Push(this, connection);
			IMessage message;
			ITransportHeaders transportHeaders2;
			Stream stream3;
			ServerProcessing serverProcessing = this.next_sink.ProcessMessage(serverChannelSinkStack, null, transportHeaders, stream2, out message, out transportHeaders2, out stream3);
			if (serverProcessing != ServerProcessing.Complete)
			{
				int num = serverProcessing - ServerProcessing.OneWay;
				return;
			}
			UnixMessageIO.SendMessageStream(stream, stream3, transportHeaders2, connection.Buffer);
			stream.Flush();
		}

		private IServerChannelSink next_sink;
	}
}
