using System;

namespace YamlDotNet.Core
{
	[Serializable]
	public class YamlException : Exception
	{
		public YamlException()
		{
		}

		public YamlException(string message)
			: base(message)
		{
		}

		public YamlException(Mark start, Mark end, string message)
			: this(start, end, message, null)
		{
		}

		public YamlException(Mark start, Mark end, string message, Exception innerException)
			: base(string.Format("({0}) - ({1}): {2}", start, end, message), innerException)
		{
			this.Start = start;
			this.End = end;
		}

		public YamlException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public Mark Start { get; private set; }

		public Mark End { get; private set; }
	}
}
