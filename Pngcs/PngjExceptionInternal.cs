using System;

namespace Hjg.Pngcs
{
	[Serializable]
	public class PngjExceptionInternal : Exception
	{
		public PngjExceptionInternal()
		{
		}

		public PngjExceptionInternal(string message, Exception cause)
			: base(message, cause)
		{
		}

		public PngjExceptionInternal(string message)
			: base(message)
		{
		}

		public PngjExceptionInternal(Exception cause)
			: base(cause.Message, cause)
		{
		}

		private const long serialVersionUID = 1L;
	}
}
