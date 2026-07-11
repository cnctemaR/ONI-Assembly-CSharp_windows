using System;
using System.Collections.Generic;
using ProcGen;
using UnityEngine;

namespace ProcGenGame
{
	public class River : River, SymbolicMapElement
	{
		public River(River other)
			: base(other, true)
		{
		}

		public void ConvertToMap(Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			Element element = ElementLoader.FindElementByName(base.backgroundElement);
			Sim.PhysicsData defaultValues = element.defaultValues;
			Element element2 = ElementLoader.FindElementByName(base.element);
			Sim.PhysicsData defaultValues2 = element2.defaultValues;
			defaultValues2.temperature = base.temperature;
			Sim.DiseaseCell invalid = Sim.DiseaseCell.Invalid;
			for (int i = 0; i < this.pathElements.Count; i++)
			{
				Segment segment = this.pathElements[i];
				Vector2 vector = segment.e1 - segment.e0;
				Vector2 vector2 = new Vector2(-vector.y, vector.x);
				Vector2 normalized = vector2.normalized;
				List<Vector2I> line = global::ProcGen.Util.GetLine(segment.e0, segment.e1);
				for (int j = 0; j < line.Count; j++)
				{
					for (float num = 0.5f; num <= base.widthCenter; num += 1f)
					{
						Vector2 vector3 = line[j] + normalized * num;
						int num2 = Grid.XYToCell((int)vector3.x, (int)vector3.y);
						if (Grid.IsValidCell(num2))
						{
							SetValues(num2, element2, defaultValues2, invalid);
						}
						Vector2 vector4 = line[j] - normalized * num;
						num2 = Grid.XYToCell((int)vector4.x, (int)vector4.y);
						if (Grid.IsValidCell(num2))
						{
							SetValues(num2, element2, defaultValues2, invalid);
						}
					}
					for (float num3 = 0.5f; num3 <= base.widthBorder; num3 += 1f)
					{
						Vector2 vector5 = line[j] + normalized * (base.widthCenter + num3);
						int num4 = Grid.XYToCell((int)vector5.x, (int)vector5.y);
						if (Grid.IsValidCell(num4))
						{
							defaultValues.temperature = temperatureMin + world.heatOffset[num4] * temperatureRange;
							SetValues(num4, element, defaultValues, invalid);
						}
						Vector2 vector6 = line[j] - normalized * (base.widthCenter + num3);
						num4 = Grid.XYToCell((int)vector6.x, (int)vector6.y);
						if (Grid.IsValidCell(num4))
						{
							defaultValues.temperature = temperatureMin + world.heatOffset[num4] * temperatureRange;
							SetValues(num4, element, defaultValues, invalid);
						}
					}
				}
			}
		}

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
					global::Debug.LogError(string.Concat(new object[] { "Process::SetValuesFunction Index [", index, "] is not valid. cells.Length [", cells.Length, "]" }));
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
			return new River(rivers.Find((River river) => Grid.PosToCell(river.SourcePosition()) == cell || Grid.PosToCell(river.SinkPosition()) == cell));
		}

		private static void GetRiverLocation(List<River> rivers, ref GameSpawnData gsd)
		{
			for (int i = 0; i < rivers.Count; i++)
			{
				Vector2 vector = rivers[i].SourcePosition();
				Vector2 vector2 = rivers[i].SinkPosition();
				if (vector.y < vector2.y)
				{
				}
			}
		}
	}
}
