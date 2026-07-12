using System;

namespace Klei
{
	public class SimSaveFileStructure
	{
		public SimSaveFileStructure()
		{
			this.worldDetail = new WorldDetailSave();
		}

		public int WidthInCells;

		public int HeightInCells;

		public int x;

		public int y;

		public byte[] Sim;

		public WorldDetailSave worldDetail;
	}
}
