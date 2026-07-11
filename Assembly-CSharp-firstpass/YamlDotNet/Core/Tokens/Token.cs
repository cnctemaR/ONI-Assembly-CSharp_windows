using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public abstract class Token
	{
		protected Token(Mark start, Mark end)
		{
			this.start = start;
			this.end = end;
		}

		public Mark Start
		{
			get
			{
				return this.start;
			}
		}

		public Mark End
		{
			get
			{
				return this.end;
			}
		}

		private readonly Mark start;

		private readonly Mark end;
	}
}
