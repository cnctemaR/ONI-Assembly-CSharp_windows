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

		public byte[] Sim;

		public WorldDetailSave worldDetail;
	}
}
