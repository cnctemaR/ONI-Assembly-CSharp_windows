using System;
using System.Collections.Generic;
using ProcGen;
using UnityEngine;

namespace ProcGenGame
{
	public class Border : Path, SymbolicMapElement
	{
		public Border(Neighbors neighbors, Vector2 e0, Vector2 e1)
		{
			this.neighbors = neighbors;
			base.AddSegment(e0, e1);
		}

		public Border(TerrainCell a, TerrainCell b, Vector2 e0, Vector2 e1)
		{
			global::Debug.Assert(a != null && b != null, "NULL neighbor for Border");
			this.neighbors.n0 = a;
			this.neighbors.n1 = b;
			base.AddSegment(e0, e1);
		}

		public void ConvertToMap(Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			Sim.DiseaseCell invalid = Sim.DiseaseCell.Invalid;
			for (int i = 0; i < this.pathElements.Count; i++)
			{
				Vector2 vector = this.pathElements[i].e1 - this.pathElements[i].e0;
				Vector2 normalized = new Vector2(-vector.y, vector.x).normalized;
				List<Vector2I> line = global::ProcGen.Util.GetLine(this.pathElements[i].e0, this.pathElements[i].e1);
				for (int j = 0; j < line.Count; j++)
				{
					int num = Grid.XYToCell(line[j].x, line[j].y);
					if (Grid.IsValidCell(num))
					{
						Element element = ElementLoader.FindElementByName(WeightedRandom.Choose<WeightedSimHash>(this.element, rnd).element);
						Sim.PhysicsData defaultValues = element.defaultValues;
						defaultValues.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
						SetValues(num, element, defaultValues, invalid);
					}
					for (float num2 = 0.5f; num2 <= this.width; num2 += 1f)
					{
						Vector2 vector2 = line[j] + normalized * num2;
						num = Grid.XYToCell((int)vector2.x, (int)vector2.y);
						if (Grid.IsValidCell(num))
						{
							Element element2 = ElementLoader.FindElementByName(WeightedRandom.Choose<WeightedSimHash>(this.element, rnd).element);
							Sim.PhysicsData defaultValues2 = element2.defaultValues;
							defaultValues2.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
							SetValues(num, element2, defaultValues2, invalid);
						}
						Vector2 vector3 = line[j] - normalized * num2;
						num = Grid.XYToCell((int)vector3.x, (int)vector3.y);
						if (Grid.IsValidCell(num))
						{
							Element element3 = ElementLoader.FindElementByName(WeightedRandom.Choose<WeightedSimHash>(this.element, rnd).element);
							Sim.PhysicsData defaultValues3 = element3.defaultValues;
							defaultValues3.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
							SetValues(num, element3, defaultValues3, invalid);
						}
					}
				}
			}
		}

		public Neighbors neighbors;

		public List<WeightedSimHash> element;

		public float width;
	}
}
