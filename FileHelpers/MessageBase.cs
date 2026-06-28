using System;

namespace FileHelpers
{
	internal abstract class MessageBase
	{
		protected MessageBase(string text)
		{
			this.SourceText = text;
		}

		private protected string SourceText { protected get; private set; }

		public string Text
		{
			get
			{
				return this.GenerateText();
			}
		}

		protected abstract string GenerateText();

		public sealed override string ToString()
		{
			return this.Text;
		}
	}
}
