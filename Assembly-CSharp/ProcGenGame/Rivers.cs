using System;
using System.Collections.Generic;
using Klei;

namespace ProcGenGame
{
	public class Rivers : YamlIO<Rivers>
	{
		public Rivers()
		{
			this.rivers = new Dictionary<string, River>();
		}

		public Dictionary<string, River> rivers { get; private set; }

		public static void ProcessRivers(Chunk world, List<River> rivers, Sim.Cell[] cells, Sim.DiseaseCell[] dcs)
		{
			TerrainCell.SetValuesFunction setValuesFunction = delegate(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc)
			{
				if (Grid.IsValidCell(index))
				{
					cells[index].SetValues(elem as Element, pd, ElementLoader.elements);
					dcs[index] = dc;
				}
				else
				{
					Debug.LogError(string.Concat(new object[] { "Process::SetValuesFunction Index [", index, "] is not valid. cells.Length [", cells.Length, "]" }), null);
				}
			};
			float num = 265f;
			float num2 = 30f;
			for (int i = 0; i < rivers.Count; i++)
			{
				rivers[i].ConvertToMap(world, setValuesFunction, num, num2, null);
			}
		}

		public static River GetRiverForCell(List<River> rivers, int cell)
		{
			return rivers.Find((River river) => Grid.PosToCell(river.SourcePosition()) == cell || Grid.PosToCell(river.SinkPosition()) == cell);
		}

		private static void GetRiverLocation(List<River> rivers, ref GameSpawnData gsd)
		{
		}
	}
}
