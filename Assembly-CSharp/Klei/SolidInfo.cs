using System;

namespace Klei
{
	public struct SolidInfo
	{
		public SolidInfo(int cellIdx, bool isSolid)
		{
			this.cellIdx = cellIdx;
			this.isSolid = isSolid;
		}

		public int cellIdx;

		public bool isSolid;
	}
}
