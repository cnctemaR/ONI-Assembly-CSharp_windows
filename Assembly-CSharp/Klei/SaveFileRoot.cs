using System;
using System.Collections.Generic;

namespace Klei
{
	internal class SaveFileRoot
	{
		public SaveFileRoot()
		{
			this.streamed = new Dictionary<string, byte[]>();
		}

		public int WidthInCells;

		public int HeightInCells;

		public Dictionary<string, byte[]> streamed;
	}
}
