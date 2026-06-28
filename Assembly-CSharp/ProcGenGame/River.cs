using System;
using System.Collections.Generic;
using KSerialization;
using ProcGen;
using UnityEngine;

namespace ProcGenGame
{
	[SerializationConfig(MemberSerialization.OptOut)]
	public class River : Path, SymbolicMapElement
	{
		public River()
		{
			this.pathElements = new List<Segment>();
		}

		public River(Node t0, Node t1, SimHashes element = SimHashes.Water, SimHashes backgroundElement = SimHashes.Granite, float temperature = 373f, float maxMass = 2000f, float flowIn = 1000f, float flowOut = 100f, float widthCenter = 1.5f, float widthBorder = 1.5f)
		{
			this.pathElements = new List<Segment>();
			this.AddSection(t0, t1);
			this.element = element;
			this.backgroundElement = backgroundElement;
			this.temperature = temperature;
			this.maxMass = maxMass;
			this.flowIn = flowIn;
			this.flowOut = flowOut;
			this.widthCenter = widthCenter;
			this.widthBorder = widthBorder;
		}

		public River(River other, bool copySections = true)
		{
			if (copySections)
			{
				this.pathElements = new List<Segment>(other.pathElements);
			}
			this.element = other.element;
			this.backgroundElement = other.backgroundElement;
			this.temperature = other.temperature;
			this.maxMass = other.maxMass;
			this.flowIn = other.flowIn;
			this.flowOut = other.flowOut;
			this.widthCenter = other.widthCenter;
			this.widthBorder = other.widthBorder;
		}

		public SimHashes element { get; set; }

		public SimHashes backgroundElement { get; set; }

		public float widthCenter { get; set; }

		public float widthBorder { get; set; }

		public float temperature { get; set; }

		public float maxMass { get; set; }

		public float flowIn { get; set; }

		public float flowOut { get; set; }

		public void AddSection(Node t0, Node t1)
		{
			this.pathElements.Add(new Segment(t0.position, t1.position));
		}

		public Vector2 SourcePosition()
		{
			return this.pathElements[0].e0;
		}

		public Vector2 SinkPosition()
		{
			return this.pathElements[this.pathElements.Count - 1].e1;
		}

		public void ConvertToMap(Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			Element element = ElementLoader.FindElementByHash(this.backgroundElement);
			Sim.PhysicsData defaultValues = element.defaultValues;
			Element element2 = ElementLoader.FindElementByHash(this.element);
			Sim.PhysicsData defaultValues2 = element2.defaultValues;
			defaultValues2.temperature = this.temperature;
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
					for (float num = 0.5f; num <= this.widthCenter; num += 1f)
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
					for (float num3 = 0.5f; num3 <= this.widthBorder; num3 += 1f)
					{
						Vector2 vector5 = line[j] + normalized * (this.widthCenter + num3);
						int num4 = Grid.XYToCell((int)vector5.x, (int)vector5.y);
						if (Grid.IsValidCell(num4))
						{
							defaultValues.temperature = temperatureMin + world.heatOffset[num4] * temperatureRange;
							SetValues(num4, element, defaultValues, invalid);
						}
						Vector2 vector6 = line[j] - normalized * (this.widthCenter + num3);
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
	}
}
