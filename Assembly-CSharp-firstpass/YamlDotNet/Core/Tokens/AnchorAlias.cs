using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class AnchorAlias : Token
	{
		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public AnchorAlias(string value)
			: this(value, Mark.Empty, Mark.Empty)
		{
		}

		public AnchorAlias(string value, Mark start, Mark end)
			: base(start, end)
		{
			this.value = value;
		}

		private readonly string value;
	}
}
