using System;

namespace TMPro
{
	public struct CaretInfo
	{
		public CaretInfo(int index, CaretPosition position)
		{
			this.index = index;
			this.position = position;
		}

		public int index;

		public CaretPosition position;
	}
}
