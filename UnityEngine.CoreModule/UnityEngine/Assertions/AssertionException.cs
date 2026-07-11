using System;

namespace UnityEngine.Assertions
{
	public class AssertionException : Exception
	{
		public AssertionException(string message, string userMessage)
			: base(message)
		{
			this.m_UserMessage = userMessage;
		}

		public override string Message
		{
			get
			{
				string text = base.Message;
				bool flag = this.m_UserMessage != null;
				if (flag)
				{
					text = text + "\n" + this.m_UserMessage;
				}
				return text;
			}
		}

		private string m_UserMessage;
	}
}
