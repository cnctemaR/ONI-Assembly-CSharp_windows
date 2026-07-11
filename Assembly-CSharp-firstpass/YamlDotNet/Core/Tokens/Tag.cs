using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class Tag : Token
	{
		public Tag(string handle, string suffix)
			: this(handle, suffix, Mark.Empty, Mark.Empty)
		{
		}

		public Tag(string handle, string suffix, Mark start, Mark end)
			: base(start, end)
		{
			this.handle = handle;
			this.suffix = suffix;
		}

		public string Handle
		{
			get
			{
				return this.handle;
			}
		}

		public string Suffix
		{
			get
			{
				return this.suffix;
			}
		}

		private readonly string handle;

		private readonly string suffix;
	}
}
