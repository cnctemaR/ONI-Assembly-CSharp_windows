using System;

namespace System.Net.WebSockets
{
	public readonly struct ValueWebSocketReceiveResult
	{
		public ValueWebSocketReceiveResult(int count, WebSocketMessageType messageType, bool endOfMessage)
		{
			if (count < 0)
			{
				ValueWebSocketReceiveResult.ThrowCountOutOfRange();
			}
			if (messageType > WebSocketMessageType.Close)
			{
				ValueWebSocketReceiveResult.ThrowMessageTypeOutOfRange();
			}
			this._countAndEndOfMessage = (uint)(count | (endOfMessage ? int.MinValue : 0));
			this._messageType = messageType;
		}

		public int Count
		{
			get
			{
				return (int)(this._countAndEndOfMessage & 2147483647U);
			}
		}

		public bool EndOfMessage
		{
			get
			{
				return (this._countAndEndOfMessage & 2147483648U) == 2147483648U;
			}
		}

		public WebSocketMessageType MessageType
		{
			get
			{
				return this._messageType;
			}
		}

		private static void ThrowCountOutOfRange()
		{
			throw new ArgumentOutOfRangeException("count");
		}

		private static void ThrowMessageTypeOutOfRange()
		{
			throw new ArgumentOutOfRangeException("messageType");
		}

		private readonly uint _countAndEndOfMessage;

		private readonly WebSocketMessageType _messageType;
	}
}
