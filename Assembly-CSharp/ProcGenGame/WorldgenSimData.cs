using System;

namespace ProcGenGame
{
	public struct WorldgenSimData
	{
		public void Init(int cellCount)
		{
			this.cells = new Sim.Cell[cellCount];
			this.diseaseCells = new Sim.DiseaseCell[cellCount];
			this.backwallCells = new Sim.SimBackwall[cellCount];
			ushort elementIndex = ElementLoader.GetElementIndex(SimHashes.Vacuum);
			for (int i = 0; i < cellCount; i++)
			{
				this.backwallCells[i].elementIdx = elementIndex;
			}
		}

		public Sim.Cell[] cells;

		public Sim.DiseaseCell[] diseaseCells;

		public Sim.SimBackwall[] backwallCells;
	}
}
