using System;

namespace Hjg.Pngcs
{
	[Serializable]
	public class PngjException : Exception
	{
		public PngjException(string message, Exception cause)
			: base(message, cause)
		{
		}

		public PngjException(string message)
			: base(message)
		{
		}

		public PngjException(Exception cause)
			: base(cause.Message, cause)
		{
		}

		private const long serialVersionUID = 1L;
	}
}
