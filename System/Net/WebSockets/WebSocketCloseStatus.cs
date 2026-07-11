using System;

namespace System.Net.WebSockets
{
	public enum WebSocketCloseStatus
	{
		NormalClosure = 1000,
		EndpointUnavailable,
		ProtocolError,
		InvalidMessageType,
		Empty = 1005,
		InvalidPayloadData = 1007,
		PolicyViolation,
		MessageTooBig,
		MandatoryExtension,
		InternalServerError
	}
}
