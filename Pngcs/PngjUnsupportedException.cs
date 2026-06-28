using System;

namespace Hjg.Pngcs
{
	[Serializable]
	public class PngjUnsupportedException : Exception
	{
		public PngjUnsupportedException()
		{
		}

		public PngjUnsupportedException(string message, Exception cause)
			: base(message, cause)
		{
		}

		public PngjUnsupportedException(string message)
			: base(message)
		{
		}

		public PngjUnsupportedException(Exception cause)
			: base(cause.Message, cause)
		{
		}

		private const long serialVersionUID = 1L;
	}
}
