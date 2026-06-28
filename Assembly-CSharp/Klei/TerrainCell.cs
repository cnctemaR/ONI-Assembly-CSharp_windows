using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay.Geo;
using Generated;
using KSerialization;
using UnityEngine;

namespace Klei
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class TerrainCell
	{
		public TerrainCell()
		{
			this.log = new LoggerSSF("TerrainCell");
		}

		public TerrainCell(Node node, VoronoiDiagram.Site site)
		{
			this.node = node;
			this.node.SetPosition(site.position);
			this.site = site;
			this.log = new LoggerSSF("TerrainCell " + node.type);
		}

		public Polygon poly
		{
			get
			{
				return this.site.poly;
			}
		}

		[Serialize]
		public Node node { get; private set; }

		public void SetNode(Node newNode)
		{
			this.node = newNode;
		}

		[Serialize]
		public VoronoiDiagram.Site site { get; private set; }

		public bool HasMobs
		{
			get
			{
				return this.mobs != null && this.mobs.Count > 0;
			}
		}

		public List<int> GetAllCells()
		{
			if (this.allCells == null)
			{
				this.allCells = new List<int>();
				for (int i = 0; i < Grid.HeightInCells; i++)
				{
					for (int j = 0; j < Grid.WidthInCells; j++)
					{
						if (this.poly.Contains(new Vector2((float)j, (float)i)))
						{
							this.allCells.Add(Grid.XYToCell(j, i));
						}
					}
				}
			}
			return this.allCells;
		}

		public List<int> GetAvailableCells()
		{
			List<int> list = new List<int>();
			foreach (Vector2I vector2I in this.availablePoints)
			{
				list.Add(Grid.XYToCell(vector2I.x, vector2I.y));
			}
			return list;
		}

		private string GetSubWorldType()
		{
			Vector2I vector2I = new Vector2I((int)this.site.poly.Centroid().x, (int)this.site.poly.Centroid().y);
			return WorldGen.GetSubWorldType(vector2I);
		}

		private Temperature.Range GetTeperatureRange()
		{
			string subWorldType = this.GetSubWorldType();
			if (subWorldType == null)
			{
				return Temperature.Range.Mild;
			}
			if (!WorldGen.Settings.subworlds.zones.ContainsKey(subWorldType))
			{
				return Temperature.Range.Mild;
			}
			return WorldGen.Settings.subworlds.zones[subWorldType].temperatureRange;
		}

		private void GetTemperatureRange(ref float min, ref float range)
		{
			Temperature.Range teperatureRange = this.GetTeperatureRange();
			min = WorldGen.Settings.temperatures.ranges[teperatureRange].min;
			range = WorldGen.Settings.temperatures.ranges[teperatureRange].max - min;
		}

		private float GetDensityMassForCell(Chunk world, int cellIdx, float mass)
		{
			if (!Grid.IsValidCell(cellIdx))
			{
				return 0f;
			}
			Debug.Assert(world.density[cellIdx] >= 0f && world.density[cellIdx] <= 1f, "Density [" + world.density[cellIdx] + "] out of range [0-1]");
			float num = world.density[cellIdx] - 0.5f;
			float num2 = mass + mass * num;
			if (num2 > 10000f)
			{
				num2 = 10000f;
			}
			return num2;
		}

		private void HandleSprinkleOfElement(Tag targetTag, Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange)
		{
			FeatureSettings feature = WorldGen.Settings.GetFeature(targetTag.Name);
			SimHashes element = feature.GetOneWeightedSimHash("SprinkleOfElementChoices").element;
			Element element2 = ElementLoader.FindElementByHash(element);
			SampleDescriber desription = WorldGen.Settings.rooms.GetDesription(targetTag);
			Sim.PhysicsData defaultValues = element2.defaultValues;
			for (int i = 0; i < this.terrainPositions.Count; i++)
			{
				if (!(this.terrainPositions[i].Value != targetTag))
				{
					float num = WorldGen.RandomRange(desription.blobSize.min, desription.blobSize.max);
					Vector2 vector = Grid.CellToPos2D(this.terrainPositions[i].Key);
					List<Vector2I> filledCircle = global::Generated.Util.GetFilledCircle(vector, num);
					for (int j = 0; j < filledCircle.Count; j++)
					{
						int num2 = Grid.XYToCell(filledCircle[j].x, filledCircle[j].y);
						if (Grid.IsValidCell(num2))
						{
							defaultValues.mass = this.GetDensityMassForCell(world, num2, element2.defaultValues.mass);
							defaultValues.temperature = temperatureMin + world.heatOffset[num2] * temperatureRange;
							SetValues(num2, element2, defaultValues);
						}
					}
				}
			}
		}

		private HashSet<Vector2I> DigFeature(Room.Shape shape, float size, List<int> bordersWidths)
		{
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			if (size < 1f)
			{
				return hashSet;
			}
			Vector2 vector = this.site.poly.Centroid();
			this.finalSize = size;
			switch (shape)
			{
			case Room.Shape.Circle:
				this.centerPoints = global::Generated.Util.GetFilledCircle(vector, this.finalSize);
				break;
			case Room.Shape.Blob:
				this.centerPoints = global::Generated.Util.GetBlob(vector, this.finalSize, WorldGen.RandomSource());
				break;
			case Room.Shape.Square:
				this.centerPoints = global::Generated.Util.GetFilledRectangle(vector, this.finalSize, this.finalSize, 2f, 2f);
				break;
			case Room.Shape.TallThin:
				this.centerPoints = global::Generated.Util.GetFilledRectangle(vector, this.finalSize / 4f, this.finalSize, 2f, 2f);
				break;
			case Room.Shape.ShortWide:
				this.centerPoints = global::Generated.Util.GetFilledRectangle(vector, this.finalSize, this.finalSize / 4f, 2f, 2f);
				break;
			}
			if (this.centerPoints.Count == 0)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Room has no centerpoints. Terrain Cell [ shape: ",
					shape.ToString(),
					" size: ",
					this.finalSize,
					"] [",
					this.node.node.Id,
					" ",
					this.node.type,
					" ",
					this.node.position,
					"]"
				}));
			}
			else if (bordersWidths != null && bordersWidths.Count > 0 && bordersWidths[0] > 0)
			{
				this.borders = new List<List<Vector2I>>();
				hashSet.UnionWith(new HashSet<Vector2I>(this.centerPoints));
				int num = 0;
				while (num < bordersWidths.Count && bordersWidths[num] > 0)
				{
					this.borders.Add(global::Generated.Util.GetBorder(hashSet, bordersWidths[num]));
					hashSet.UnionWith(this.borders[num]);
					num++;
				}
			}
			return hashSet;
		}

		private TerrainCell.ElementOverride GetElementOverride(WeightedSimHash weightedElement)
		{
			TerrainCell.ElementOverride elementOverride = default(TerrainCell.ElementOverride);
			elementOverride.element = ElementLoader.FindElementByHash(weightedElement.element);
			elementOverride.pdelement = elementOverride.element.defaultValues;
			elementOverride.mass = elementOverride.element.defaultValues.mass;
			elementOverride.temperature = elementOverride.element.defaultValues.temperature;
			elementOverride.overrideMass = false;
			elementOverride.overrideTemperature = false;
			if (weightedElement.overrides != null)
			{
				for (int i = 0; i < weightedElement.overrides.Count; i++)
				{
					switch (weightedElement.overrides[i].type)
					{
					case SampleDescriber.Override.Type.MassMultiplier:
						elementOverride.mass *= weightedElement.overrides[i].value;
						break;
					case SampleDescriber.Override.Type.MassOverride:
						elementOverride.mass = weightedElement.overrides[i].value;
						elementOverride.overrideMass = true;
						break;
					case SampleDescriber.Override.Type.TemperatureMultiplier:
						elementOverride.temperature *= weightedElement.overrides[i].value;
						break;
					case SampleDescriber.Override.Type.TemperatureOverride:
						elementOverride.temperature = weightedElement.overrides[i].value;
						elementOverride.overrideTemperature = true;
						break;
					}
				}
			}
			if (elementOverride.overrideTemperature)
			{
				elementOverride.pdelement.temperature = elementOverride.temperature;
			}
			if (elementOverride.overrideMass)
			{
				elementOverride.pdelement.mass = elementOverride.mass;
			}
			return elementOverride;
		}

		private void ApplyPlaceElementForRoom(FeatureSettings feature, string group, List<Vector2I> cells, Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange)
		{
			if (cells == null || cells.Count == 0)
			{
				return;
			}
			if (!feature.HasGroup(group))
			{
				return;
			}
			switch (feature.ElementChoiceGroups[group].selectionMethod)
			{
			case Room.Selection.WeightedResample:
			{
				for (int i = 0; i < cells.Count; i++)
				{
					int num = Grid.XYToCell(cells[i].x, cells[i].y);
					if (Grid.IsValidCell(num))
					{
						WeightedSimHash oneWeightedSimHash = feature.GetOneWeightedSimHash(group);
						TerrainCell.ElementOverride elementOverride = this.GetElementOverride(oneWeightedSimHash);
						if (!elementOverride.overrideTemperature)
						{
							elementOverride.pdelement.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
						}
						if (!elementOverride.overrideMass)
						{
							elementOverride.pdelement.mass = this.GetDensityMassForCell(world, num, elementOverride.mass);
						}
						SetValues(num, elementOverride.element, elementOverride.pdelement);
					}
				}
				return;
			}
			}
			for (int j = 0; j < cells.Count; j++)
			{
				int num2 = Grid.XYToCell(cells[j].x, cells[j].y);
				if (Grid.IsValidCell(num2))
				{
					WeightedSimHash oneWeightedSimHash2 = feature.GetOneWeightedSimHash(group);
					TerrainCell.ElementOverride elementOverride2 = this.GetElementOverride(oneWeightedSimHash2);
					if (!elementOverride2.overrideTemperature)
					{
						elementOverride2.pdelement.temperature = temperatureMin + world.heatOffset[num2] * temperatureRange;
					}
					if (!elementOverride2.overrideMass)
					{
						elementOverride2.pdelement.mass = this.GetDensityMassForCell(world, num2, elementOverride2.mass);
					}
					SetValues(num2, elementOverride2.element, elementOverride2.pdelement);
				}
			}
		}

		private int GetIndexForLocation(List<Vector2I> points, Mob.Location location)
		{
			int num = -1;
			if (points == null || points.Count == 0)
			{
				return num;
			}
			if (location == Mob.Location.Air || location == Mob.Location.Solid)
			{
				return (int)WorldGen.RandomRange(0f, (float)points.Count);
			}
			for (int i = 0; i < points.Count; i++)
			{
				int num2 = Grid.XYToCell(points[i].x, points[i].y);
				if (Grid.IsValidCell(num2))
				{
					if (num == -1)
					{
						num = i;
					}
					else if (location != Mob.Location.Floor)
					{
						if (location == Mob.Location.Ceiling)
						{
							if (points[i].y > points[num].y)
							{
								num = i;
							}
						}
					}
					else if (points[i].y < points[num].y)
					{
						num = i;
					}
				}
			}
			return num;
		}

		private void PlaceMobInRoom(List<MobReference> mobTags, List<Vector2I> points)
		{
			if (points == null)
			{
				return;
			}
			if (this.mobs == null)
			{
				this.mobs = new List<KeyValuePair<int, Tag>>();
			}
			for (int i = 0; i < mobTags.Count; i++)
			{
				if (!WorldGen.Settings.mobs.MobLookupTable.ContainsKey(mobTags[i].type))
				{
					Debug.LogError("Missing sample description for tag [" + mobTags[i].type + "]");
				}
				else
				{
					Mob mob = WorldGen.Settings.mobs.MobLookupTable[mobTags[i].type];
					Tag tag = new Tag(mobTags[i].type);
					int num = Mathf.RoundToInt(mobTags[i].count.GetValue());
					for (int j = 0; j < num; j++)
					{
						int indexForLocation = this.GetIndexForLocation(points, mob.location);
						if (indexForLocation == -1)
						{
							break;
						}
						if (points.Count <= indexForLocation)
						{
							return;
						}
						int num2 = Grid.XYToCell(points[indexForLocation].x, points[indexForLocation].y);
						points.RemoveAt(indexForLocation);
						this.mobs.Add(new KeyValuePair<int, Tag>(num2, tag));
					}
				}
			}
		}

		private int[] ConvertNoiseToPoints(float minThreshold = 0.9f, float maxThreshold = 1f)
		{
			float[] baseNoiseMap = WorldGen.BaseNoiseMap;
			if (baseNoiseMap == null)
			{
				return null;
			}
			List<int> list = new List<int>();
			float width = this.site.poly.bounds.width;
			float height = this.site.poly.bounds.height;
			for (float num = this.site.position.y - height / 2f; num < this.site.position.y + height / 2f; num += 1f)
			{
				for (float num2 = this.site.position.x - width / 2f; num2 < this.site.position.x + width / 2f; num2 += 1f)
				{
					int num3 = Grid.PosToCell(new Vector2(num2, num));
					if (this.site.poly.Contains(new Vector2(num2, num)))
					{
						float num4 = (float)((int)baseNoiseMap[num3]);
						if (num4 >= minThreshold && num4 <= maxThreshold)
						{
							if (!list.Contains(num3))
							{
								list.Add(Grid.PosToCell(new Vector2(num2, num)));
							}
						}
					}
				}
			}
			return list.ToArray();
		}

		private void ApplyForeground(Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange)
		{
			if (this.node.tags != null)
			{
				FeatureSettings featureSettings = WorldGen.Settings.GetFeature(this.node.type);
				if (featureSettings == null && this.node.tags != null)
				{
					List<Tag> list = new List<Tag>();
					foreach (Tag tag in this.node.tags)
					{
						FeatureSettings feature = WorldGen.Settings.GetFeature(tag.Name);
						if (feature != null)
						{
							list.Add(tag);
						}
					}
					if (list.Count > 0)
					{
						Tag tag2 = list[WorldGen.RandomSource().Next(list.Count)];
						featureSettings = WorldGen.Settings.GetFeature(tag2.Name);
					}
				}
				if (featureSettings != null)
				{
					float num = featureSettings.blobSize.GetValue();
					float num2 = this.poly.DistanceToClosestEdge(null);
					if (!this.node.tags.Contains(WorldGenTags.AllowExceedNodeBorders) && num2 < num)
					{
						if (this.debugMode)
						{
							Debug.LogWarning(string.Concat(new object[]
							{
								this.node.type,
								" ",
								featureSettings.shape,
								"  blob size too large to fit in node. Size reduced. ",
								num,
								"->",
								(num2 - 6f).ToString()
							}));
						}
						num = num2 - 6f;
					}
					if (num <= 0f)
					{
						return;
					}
					HashSet<Vector2I> hashSet = this.DigFeature(featureSettings.shape, num, featureSettings.borders);
					this.availablePoints.ExceptWith(hashSet);
					this.ApplyPlaceElementForRoom(featureSettings, "RoomCenterElements", this.centerPoints, world, SetValues, temperatureMin, temperatureRange);
					if (this.borders != null)
					{
						for (int i = 0; i < this.borders.Count; i++)
						{
							this.ApplyPlaceElementForRoom(featureSettings, "RoomBorderChoices" + i, this.borders[i], world, SetValues, temperatureMin, temperatureRange);
						}
					}
					if (featureSettings.internalMobs != null && featureSettings.internalMobs.Count > 0 && !this.doneMobs)
					{
						if (!this.disableRoomMobs)
						{
							this.PlaceMobInRoom(featureSettings.internalMobs, this.centerPoints);
						}
						this.doneMobs = true;
					}
				}
			}
		}

		private void ApplyBackground(Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange)
		{
			float @float = WorldGen.Settings.defaults.GetFloat("CaveOverrideMaxValue");
			float float2 = WorldGen.Settings.defaults.GetFloat("CaveOverrideSliverValue");
			VoronoiLeaf leafForTerrainCell = WorldGen.GetLeafForTerrainCell(this);
			bool flag = leafForTerrainCell.tags.Contains(WorldGenTags.IgnoreCaveOverride);
			bool flag2 = leafForTerrainCell.tags.Contains(WorldGenTags.CaveVoidSliver);
			bool flag3 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToCentroid);
			bool flag4 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToCentroidInv);
			bool flag5 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToEdge);
			bool flag6 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToEdgeInv);
			bool flag7 = leafForTerrainCell.tags.Contains(WorldGenTags.DistFunctionPointCentroid);
			bool flag8 = leafForTerrainCell.tags.Contains(WorldGenTags.DistFunctionPointEdge);
			foreach (Vector2I vector2I in this.availablePoints)
			{
				int num = Grid.XYToCell(vector2I.x, vector2I.y);
				float num2 = world.overrides[num];
				if (!flag && num2 >= 100f)
				{
					if (num2 >= 300f)
					{
						SetValues(num, WorldGen.voidElement, WorldGen.voidElement.defaultValues);
					}
					else if (num2 >= 200f)
					{
						SetValues(num, WorldGen.unobtaniumElement, WorldGen.unobtaniumElement.defaultValues);
					}
					else
					{
						SetValues(num, WorldGen.katairiteElement, WorldGen.katairiteElement.defaultValues);
					}
				}
				else
				{
					float num3 = 1f;
					Vector2 vector = new Vector2((float)vector2I.x, (float)vector2I.y);
					if (flag3 || flag4)
					{
						float num4 = 15f;
						if (flag8)
						{
							float num5 = 0f;
							MathUtil.Pair<Vector2, Vector2> closestEdge = this.poly.GetClosestEdge(vector, ref num5);
							Vector2 vector2 = closestEdge.First + (closestEdge.Second - closestEdge.First) * num5;
							num4 = Vector2.Distance(vector2, vector);
						}
						num3 = Vector2.Distance(this.poly.Centroid(), vector) / num4;
						num3 = Mathf.Max(0f, Mathf.Min(1f, num3));
						if (flag4)
						{
							num3 = 1f - num3;
						}
					}
					if (flag6 || flag5)
					{
						float num6 = 0f;
						MathUtil.Pair<Vector2, Vector2> closestEdge2 = this.poly.GetClosestEdge(vector, ref num6);
						Vector2 vector3 = closestEdge2.First + (closestEdge2.Second - closestEdge2.First) * num6;
						float num7 = 15f;
						if (flag7)
						{
							num7 = Vector2.Distance(this.poly.Centroid(), vector);
						}
						num3 = Vector2.Distance(vector3, vector) / num7;
						num3 = Mathf.Max(0f, Mathf.Min(1f, num3));
						if (flag6)
						{
							num3 = 1f - num3;
						}
					}
					Element element;
					Sim.PhysicsData defaultValues;
					WorldGen.GetElementForBiome(world, this.node.type, vector2I, out element, out defaultValues, num3);
					if (!element.IsVacuum && element.id != SimHashes.Katairite && element.id != SimHashes.Unobtanium)
					{
						if (element.lowTempTransition != null && temperatureMin < element.lowTemp)
						{
							temperatureMin = element.lowTemp + 20f;
						}
						defaultValues.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
					}
					if (element.IsSolid)
					{
						defaultValues.mass = this.GetDensityMassForCell(world, num, defaultValues.mass);
						if (!flag && num2 > @float && num2 < 100f)
						{
							if (flag2 && num2 > float2)
							{
								element = WorldGen.voidElement;
							}
							else
							{
								element = WorldGen.vacuumElement;
							}
							defaultValues = element.defaultValues;
						}
					}
					SetValues(num, element, defaultValues);
				}
			}
			if (this.node.tags.Contains(WorldGenTags.SprinkleOfOxyRock))
			{
				this.HandleSprinkleOfElement(WorldGenTags.SprinkleOfOxyRock, world, SetValues, temperatureMin, temperatureRange);
			}
			if (this.node.tags.Contains(WorldGenTags.SprinkleOfMetal))
			{
				this.HandleSprinkleOfElement(WorldGenTags.SprinkleOfMetal, world, SetValues, temperatureMin, temperatureRange);
			}
		}

		private void GenerateActionCells(Tag tag, HashSet<Vector2I> possiblePoints)
		{
			Room desription = WorldGen.Settings.rooms.GetDesription(tag);
			SampleDescriber sampleDescriber = desription;
			if (sampleDescriber == null && WorldGen.Settings.mobs.GetMobTags().Contains(tag))
			{
				sampleDescriber = WorldGen.Settings.mobs.MobLookupTable[tag.Name];
			}
			if (sampleDescriber == null)
			{
				return;
			}
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			float value = sampleDescriber.density.GetValue();
			SampleDescriber.PointSelectionMethod selectMethod = sampleDescriber.selectMethod;
			SampleDescriber.PointSelectionMethod pointSelectionMethod = selectMethod;
			List<Vector2> list;
			if (pointSelectionMethod != SampleDescriber.PointSelectionMethod.RandomPoints)
			{
				if (pointSelectionMethod != SampleDescriber.PointSelectionMethod.Centroid)
				{
				}
				list = new List<Vector2>();
				list.Add(this.node.position);
			}
			else
			{
				list = PointGenerator.GetRandomPoints(this.poly, value, 0f, null, sampleDescriber.sampleBehaviour, true, true, true);
			}
			foreach (Vector2 vector in list)
			{
				Vector2I vector2I = new Vector2I((int)vector.x, (int)vector.y);
				if (possiblePoints.Contains(vector2I))
				{
					hashSet.Add(vector2I);
				}
			}
			if (desription == null)
			{
				if (this.mobs == null)
				{
					this.mobs = new List<KeyValuePair<int, Tag>>();
				}
				foreach (Vector2I vector2I2 in hashSet)
				{
					int num = Grid.XYToCell(vector2I2.x, vector2I2.y);
					if (!Grid.IsValidCell(num) || !this.disableRoomMobs)
					{
					}
				}
			}
			else if (desription.mobselection == Room.Selection.None)
			{
				if (this.terrainPositions == null)
				{
					this.terrainPositions = new List<KeyValuePair<int, Tag>>();
				}
				foreach (Vector2I vector2I3 in hashSet)
				{
					int num2 = Grid.XYToCell(vector2I3.x, vector2I3.y);
					if (Grid.IsValidCell(num2))
					{
						this.terrainPositions.Add(new KeyValuePair<int, Tag>(num2, tag));
					}
				}
			}
			else
			{
				if (this.mobs == null)
				{
					this.mobs = new List<KeyValuePair<int, Tag>>();
				}
				desription.ResetMobs();
				foreach (Vector2I vector2I4 in hashSet)
				{
					int num3 = Grid.XYToCell(vector2I4.x, vector2I4.y);
					if (Grid.IsValidCell(num3))
					{
						WeightedMob nextMob = desription.GetNextMob();
						if (nextMob != null && !this.disableRoomMobs)
						{
							this.mobs.Add(new KeyValuePair<int, Tag>(num3, new Tag(nextMob.tag)));
						}
					}
				}
			}
		}

		private void DoProcess(Chunk world, TerrainCell.SetValuesFunction SetValues)
		{
			float num = 265f;
			float num2 = 30f;
			this.availablePoints = new HashSet<Vector2I>();
			foreach (int num3 in this.GetAllCells())
			{
				this.availablePoints.Add(Grid.CellToXY(num3));
			}
			this.GetTemperatureRange(ref num, ref num2);
			this.ApplyForeground(world, SetValues, num, num2);
			for (int i = 0; i < this.node.tags.Count; i++)
			{
				this.GenerateActionCells(this.node.tags[i], this.availablePoints);
			}
			this.ApplyBackground(world, SetValues, num, num2);
		}

		public void Process(Sim.Cell[] cells, float[] bgTemp, Chunk world)
		{
			TerrainCell.SetValuesFunction setValuesFunction = delegate(int index, Element elem, Sim.PhysicsData pd)
			{
				if (Grid.IsValidCell(index))
				{
					if (pd.temperature == 0f || elem.HasTag(GameTags.Special))
					{
						bgTemp[index] = -1f;
					}
					cells[index].SetValues(elem, pd, ElementLoader.elements);
				}
				else
				{
					Debug.LogError(string.Concat(new object[] { "Process::SetValuesFunction Index [", index, "] is not valid. cells.Length [", cells.Length, "]" }));
				}
			};
			this.DoProcess(world, setValuesFunction);
		}

		public void Process(Chunk world)
		{
			TerrainCell.SetValuesFunction setValuesFunction = delegate(int index, Element elem, Sim.PhysicsData pd)
			{
				SimMessages.ModifyCell(index, ElementLoader.GetElementIndex(elem.id), pd.temperature, pd.mass, SimMessages.ReplaceType.Replace, -1);
			};
			this.DoProcess(world, setValuesFunction);
		}

		[OnDeserializing]
		internal void OnDeserializingMethod()
		{
			this.node = new Node();
			this.site = new VoronoiDiagram.Site();
		}

		public void DebugDraw()
		{
			if (this.poly != null)
			{
				this.poly.DebugDraw(Color.green, (TerrainCell.drawOptions & TerrainCell.DebugFlags.Centroid) != (TerrainCell.DebugFlags)0, 15f, 0f);
			}
			if (this.node != null)
			{
			}
		}

		public const int DONT_SET_TEMPERATURE_DEFAULTS = -1;

		public List<KeyValuePair<int, Tag>> mobs;

		public List<KeyValuePair<int, Tag>> terrainPositions;

		public LoggerSSF log;

		private List<int> allCells;

		private HashSet<Vector2I> availablePoints;

		private List<Vector2I> centerPoints;

		private List<List<Vector2I>> borders;

		private bool doneMobs;

		private bool debugMode;

		private bool disableRoomMobs;

		private float finalSize;

		[EnumFlags]
		public static TerrainCell.DebugFlags drawOptions;

		private struct ElementOverride
		{
			public Element element;

			public Sim.PhysicsData pdelement;

			public float mass;

			public float temperature;

			public bool overrideMass;

			public bool overrideTemperature;
		}

		[Flags]
		public enum DebugFlags
		{
			Site = 1,
			Centroid = 2,
			SitePoly = 4
		}

		public delegate void SetValuesFunction(int index, Element elem, Sim.PhysicsData pd);
	}
}
