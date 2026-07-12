using System;

namespace System.Net.Http.Headers
{
	internal struct Token
	{
		public Token(Token.Type type, int startPosition, int endPosition)
		{
			this = default(Token);
			this.type = type;
			this.StartPosition = startPosition;
			this.EndPosition = endPosition;
		}

		public int StartPosition { readonly get; private set; }

		public int EndPosition { readonly get; private set; }

		public Token.Type Kind
		{
			get
			{
				return this.type;
			}
		}

		public static implicit operator Token.Type(Token token)
		{
			return token.type;
		}

		public override string ToString()
		{
			return this.type.ToString();
		}

		public static readonly Token Empty = new Token(Token.Type.Token, 0, 0);

		private readonly Token.Type type;

		public enum Type
		{
			Error,
			End,
			Token,
			QuotedString,
			SeparatorEqual,
			SeparatorSemicolon,
			SeparatorSlash,
			SeparatorDash,
			SeparatorComma,
			OpenParens
		}
	}
}
