using System;

namespace ProcGenGame
{
	public class WorldgenException : Exception
	{
		public WorldgenException(string message, string userMessage)
			: base(message)
		{
			this.userMessage = userMessage;
		}

		public readonly string userMessage;
	}
}
