using System;

namespace Hjg.Pngcs
{
	[Serializable]
	public class PngjBadCrcException : PngjException
	{
		public PngjBadCrcException(string message, Exception cause)
			: base(message, cause)
		{
		}

		public PngjBadCrcException(string message)
			: base(message)
		{
		}

		public PngjBadCrcException(Exception cause)
			: base(cause)
		{
		}

		private const long serialVersionUID = 1L;
	}
}
