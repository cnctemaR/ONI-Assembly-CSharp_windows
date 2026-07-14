using System;
using System.Runtime.CompilerServices;

namespace VYaml.Parser
{
	[NullableContext(2)]
	[Nullable(0)]
	internal readonly struct Token
	{
		public Token(TokenType type, ITokenContent content = null)
		{
			this.Type = type;
			this.Content = content;
		}

		[NullableContext(1)]
		public override string ToString()
		{
			return string.Format("{0} \"{1}\"", this.Type, this.Content);
		}

		public readonly TokenType Type;

		public readonly ITokenContent Content;
	}
}
