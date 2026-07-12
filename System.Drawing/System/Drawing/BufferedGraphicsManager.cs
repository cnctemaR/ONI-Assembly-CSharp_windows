using System;

namespace System.Drawing
{
	public sealed class BufferedGraphicsManager
	{
		private BufferedGraphicsManager()
		{
		}

		public static BufferedGraphicsContext Current
		{
			get
			{
				return BufferedGraphicsManager.graphics_context;
			}
		}

		private static BufferedGraphicsContext graphics_context = new BufferedGraphicsContext();
	}
}
