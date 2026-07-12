using System;

namespace ProcGenGame
{
	public class TemplateSpawningException : Exception
	{
		public TemplateSpawningException(string message, string userMessage)
			: base(message)
		{
			this.userMessage = userMessage;
		}

		public readonly string userMessage;
	}
}
