using System;

namespace System.Net.WebSockets
{
	public class WebSocketReceiveResult
	{
		public WebSocketReceiveResult(int count, WebSocketMessageType messageType, bool endOfMessage)
			: this(count, messageType, endOfMessage, null, null)
		{
		}

		public WebSocketReceiveResult(int count, WebSocketMessageType messageType, bool endOfMessage, WebSocketCloseStatus? closeStatus, string closeStatusDescription)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.Count = count;
			this.EndOfMessage = endOfMessage;
			this.MessageType = messageType;
			this.CloseStatus = closeStatus;
			this.CloseStatusDescription = closeStatusDescription;
		}

		public int Count { get; }

		public bool EndOfMessage { get; }

		public WebSocketMessageType MessageType { get; }

		public WebSocketCloseStatus? CloseStatus { get; }

		public string CloseStatusDescription { get; }
	}
}
