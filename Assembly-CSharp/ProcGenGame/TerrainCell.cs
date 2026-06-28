using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay.Geo;
using KSerialization;
using ProcGen;
using UnityEngine;
using VoronoiTree;

namespace ProcGenGame
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class TerrainCell
	{
		protected TerrainCell()
		{
		}

		protected TerrainCell(global::ProcGen.Node node, Diagram.Site site)
		{
			this.node = node;
			this.site = site;
			this.node.SetPosition(site.position);
		}

		public Polygon poly
		{
			get
			{
				return this.site.poly;
			}
		}

		[Serialize]
		public global::ProcGen.Node node { get; private set; }

		public void SetNode(global::ProcGen.Node newNode)
		{
			this.node = newNode;
		}

		[Serialize]
		public Diagram.Site site { get; private set; }

		public bool HasMobs
		{
			get
			{
				return this.mobs != null && this.mobs.Count > 0;
			}
		}

		public List<KeyValuePair<int, Tag>> mobs { get; private set; }

		public virtual void LogInfo(string evt, string param, float value)
		{
			global::Debug.Log(string.Concat(new object[] { evt, ":", param, "=", value }), null);
		}

		public static void ClearClaimedCells()
		{
			TerrainCell.claimedCells.Clear();
		}

		public List<int> GetAllCells()
		{
			if (this.allCells == null)
			{
				this.allCells = new List<int>();
				this.availablePoints = new HashSet<Vector2I>();
				for (int i = 0; i < Grid.HeightInCells; i++)
				{
					for (int j = 0; j < Grid.WidthInCells; j++)
					{
						if (this.poly.Contains(new Vector2((float)j, (float)i)))
						{
							int num = Grid.XYToCell(j, i);
							this.availablePoints.Add(Grid.CellToXY(num));
							if (TerrainCell.claimedCells.Add(num))
							{
								this.allCells.Add(num);
							}
						}
					}
				}
				this.LogInfo("Initialise cells", "", (float)this.allCells.Count);
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

		private bool RemoveFromAvailableCells(int cell)
		{
			int num;
			int num2;
			Grid.CellToXY(cell, out num, out num2);
			Vector2I vector2I = new Vector2I(num, num2);
			return this.availablePoints.Remove(vector2I);
		}

		public void AddMobs(IEnumerable<KeyValuePair<int, Tag>> newMobs)
		{
			foreach (KeyValuePair<int, Tag> keyValuePair in newMobs)
			{
				this.AddMob(keyValuePair);
			}
		}

		private void AddMob(int cellIdx, string tag)
		{
			this.AddMob(new KeyValuePair<int, Tag>(cellIdx, new Tag(tag)));
		}

		public void AddMob(KeyValuePair<int, Tag> mob)
		{
			if (this.mobs == null)
			{
				this.mobs = new List<KeyValuePair<int, Tag>>();
			}
			this.mobs.Add(mob);
			bool flag = this.RemoveFromAvailableCells(mob.Key);
			this.LogInfo("\t\tRemoveFromAvailableCells", mob.Value.Name + ": " + ((!flag) ? "failed" : "success"), (float)mob.Key);
			if (!flag)
			{
				if (!this.allCells.Contains(mob.Key))
				{
				}
			}
		}

		protected string GetSubWorldType()
		{
			Vector2I vector2I = new Vector2I((int)this.site.poly.Centroid().x, (int)this.site.poly.Centroid().y);
			return WorldGen.GetSubWorldType(vector2I);
		}

		protected Temperature.Range GetTeperatureRange()
		{
			string subWorldType = this.GetSubWorldType();
			Temperature.Range range;
			if (subWorldType == null)
			{
				range = Temperature.Range.Mild;
			}
			else if (!WorldGen.Settings.GetSubWorlds().ContainsKey(subWorldType))
			{
				range = Temperature.Range.Mild;
			}
			else
			{
				range = WorldGen.Settings.GetSubWorld(subWorldType).temperatureRange;
			}
			return range;
		}

		protected void GetTemperatureRange(ref float min, ref float range)
		{
			Temperature.Range teperatureRange = this.GetTeperatureRange();
			min = WorldGen.Settings.temperatures.ranges[teperatureRange].min;
			range = WorldGen.Settings.temperatures.ranges[teperatureRange].max - min;
		}

		protected float GetDensityMassForCell(Chunk world, int cellIdx, float mass)
		{
			float num;
			if (!Grid.IsValidCell(cellIdx))
			{
				num = 0f;
			}
			else
			{
				float num2 = world.density[cellIdx] - 0.5f;
				float num3 = mass + mass * num2;
				if (num3 > 10000f)
				{
					num3 = 10000f;
				}
				num = num3;
			}
			return num;
		}

		private void HandleSprinkleOfElement(Tag targetTag, Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			FeatureSettings feature = WorldGen.Settings.GetFeature(targetTag.Name);
			string element = feature.GetOneWeightedSimHash("SprinkleOfElementChoices", rnd).element;
			Element element2 = ElementLoader.FindElementByName(element);
			SampleDescriber desription = WorldGen.Settings.rooms.GetDesription(targetTag);
			Sim.PhysicsData defaultValues = element2.defaultValues;
			Sim.DiseaseCell invalid = Sim.DiseaseCell.Invalid;
			for (int i = 0; i < this.terrainPositions.Count; i++)
			{
				if (!(this.terrainPositions[i].Value != targetTag))
				{
					float num = rnd.RandomRange(desription.blobSize.min, desription.blobSize.max);
					Vector2 vector = Grid.CellToPos2D(this.terrainPositions[i].Key);
					List<Vector2I> filledCircle = global::ProcGen.Util.GetFilledCircle(vector, num);
					for (int j = 0; j < filledCircle.Count; j++)
					{
						int num2 = Grid.XYToCell(filledCircle[j].x, filledCircle[j].y);
						if (Grid.IsValidCell(num2))
						{
							defaultValues.mass = this.GetDensityMassForCell(world, num2, element2.defaultValues.mass);
							defaultValues.temperature = temperatureMin + world.heatOffset[num2] * temperatureRange;
							SetValues(num2, element2, defaultValues, invalid);
						}
					}
				}
			}
		}

		private HashSet<Vector2I> DigFeature(global::ProcGen.Room.Shape shape, float size, List<int> bordersWidths, SeededRandom rnd)
		{
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			HashSet<Vector2I> hashSet2;
			if (size < 1f)
			{
				hashSet2 = hashSet;
			}
			else
			{
				Vector2 vector = this.site.poly.Centroid();
				this.finalSize = size;
				switch (shape)
				{
				case global::ProcGen.Room.Shape.Circle:
					this.centerPoints = global::ProcGen.Util.GetFilledCircle(vector, this.finalSize);
					break;
				case global::ProcGen.Room.Shape.Blob:
					this.centerPoints = global::ProcGen.Util.GetBlob(vector, this.finalSize, rnd.RandomSource());
					break;
				case global::ProcGen.Room.Shape.Square:
					this.centerPoints = global::ProcGen.Util.GetFilledRectangle(vector, this.finalSize, this.finalSize, rnd, 2f, 2f);
					break;
				case global::ProcGen.Room.Shape.TallThin:
					this.centerPoints = global::ProcGen.Util.GetFilledRectangle(vector, this.finalSize / 4f, this.finalSize, rnd, 2f, 2f);
					break;
				case global::ProcGen.Room.Shape.ShortWide:
					this.centerPoints = global::ProcGen.Util.GetFilledRectangle(vector, this.finalSize, this.finalSize / 4f, rnd, 2f, 2f);
					break;
				}
				if (this.centerPoints.Count == 0)
				{
					global::Debug.LogWarning(string.Concat(new object[]
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
					}), null);
				}
				else if (bordersWidths != null && bordersWidths.Count > 0 && bordersWidths[0] > 0)
				{
					this.borders = new List<List<Vector2I>>();
					hashSet.UnionWith(new HashSet<Vector2I>(this.centerPoints));
					int num = 0;
					while (num < bordersWidths.Count && bordersWidths[num] > 0)
					{
						this.borders.Add(global::ProcGen.Util.GetBorder(hashSet, bordersWidths[num]));
						hashSet.UnionWith(this.borders[num]);
						num++;
					}
				}
				hashSet2 = hashSet;
			}
			return hashSet2;
		}

		public static TerrainCell.ElementOverride GetElementOverride(string element, SampleDescriber.Override overrides)
		{
			TerrainCell.ElementOverride elementOverride = default(TerrainCell.ElementOverride);
			elementOverride.element = ElementLoader.FindElementByName(element);
			elementOverride.pdelement = elementOverride.element.defaultValues;
			elementOverride.dc = Sim.DiseaseCell.Invalid;
			elementOverride.mass = elementOverride.element.defaultValues.mass;
			elementOverride.temperature = elementOverride.element.defaultValues.temperature;
			TerrainCell.ElementOverride elementOverride2;
			if (overrides == null)
			{
				elementOverride2 = elementOverride;
			}
			else
			{
				elementOverride.overrideMass = false;
				elementOverride.overrideTemperature = false;
				elementOverride.overrideDiseaseIdx = false;
				elementOverride.overrideDiseaseAmount = false;
				if (overrides.massMultiplier != null)
				{
					elementOverride.mass *= overrides.massMultiplier.Value;
					elementOverride.overrideMass = true;
				}
				if (overrides.massOverride != null)
				{
					elementOverride.mass = overrides.massOverride.Value;
					elementOverride.overrideMass = true;
				}
				if (overrides.temperatureMultiplier != null)
				{
					elementOverride.temperature *= overrides.temperatureMultiplier.Value;
					elementOverride.overrideTemperature = true;
				}
				if (overrides.temperatureOverride != null)
				{
					elementOverride.temperature = overrides.temperatureOverride.Value;
					elementOverride.overrideTemperature = true;
				}
				if (overrides.diseaseOverride != null)
				{
					elementOverride.diseaseIdx = (byte)WorldGen.GetDiseaseIdx(overrides.diseaseOverride);
					elementOverride.overrideDiseaseIdx = true;
				}
				if (overrides.diseaseAmountOverride != null)
				{
					elementOverride.diseaseAmount = overrides.diseaseAmountOverride.Value;
					elementOverride.overrideDiseaseAmount = true;
				}
				if (elementOverride.overrideTemperature)
				{
					elementOverride.pdelement.temperature = elementOverride.temperature;
				}
				if (elementOverride.overrideMass)
				{
					elementOverride.pdelement.mass = elementOverride.mass;
				}
				if (elementOverride.overrideDiseaseIdx)
				{
					elementOverride.dc.diseaseIdx = elementOverride.diseaseIdx;
				}
				if (elementOverride.overrideDiseaseAmount)
				{
					elementOverride.dc.elementCount = elementOverride.diseaseAmount;
				}
				elementOverride2 = elementOverride;
			}
			return elementOverride2;
		}

		private void ApplyPlaceElementForRoom(FeatureSettings feature, string group, List<Vector2I> cells, Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			if (cells != null && cells.Count != 0)
			{
				if (feature.HasGroup(group))
				{
					global::ProcGen.Room.Selection selectionMethod = feature.ElementChoiceGroups[group].selectionMethod;
					if (selectionMethod != global::ProcGen.Room.Selection.WeightedResample)
					{
						if (selectionMethod != global::ProcGen.Room.Selection.Weighted)
						{
						}
						for (int i = 0; i < cells.Count; i++)
						{
							int num = Grid.XYToCell(cells[i].x, cells[i].y);
							if (Grid.IsValidCell(num))
							{
								WeightedSimHash oneWeightedSimHash = feature.GetOneWeightedSimHash(group, rnd);
								TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(oneWeightedSimHash.element, oneWeightedSimHash.overrides);
								if (!elementOverride.overrideTemperature)
								{
									elementOverride.pdelement.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
								}
								if (!elementOverride.overrideMass)
								{
									elementOverride.pdelement.mass = this.GetDensityMassForCell(world, num, elementOverride.mass);
								}
								SetValues(num, elementOverride.element, elementOverride.pdelement, elementOverride.dc);
							}
						}
					}
					else
					{
						for (int j = 0; j < cells.Count; j++)
						{
							int num2 = Grid.XYToCell(cells[j].x, cells[j].y);
							if (Grid.IsValidCell(num2))
							{
								WeightedSimHash oneWeightedSimHash2 = feature.GetOneWeightedSimHash(group, rnd);
								TerrainCell.ElementOverride elementOverride2 = TerrainCell.GetElementOverride(oneWeightedSimHash2.element, oneWeightedSimHash2.overrides);
								if (!elementOverride2.overrideTemperature)
								{
									elementOverride2.pdelement.temperature = temperatureMin + world.heatOffset[num2] * temperatureRange;
								}
								if (!elementOverride2.overrideMass)
								{
									elementOverride2.pdelement.mass = this.GetDensityMassForCell(world, num2, elementOverride2.mass);
								}
								SetValues(num2, elementOverride2.element, elementOverride2.pdelement, elementOverride2.dc);
							}
						}
					}
				}
			}
		}

		private int GetIndexForLocation(List<Vector2I> points, Mob.Location location, SeededRandom rnd)
		{
			int num = -1;
			int num2;
			if (points == null || points.Count == 0)
			{
				num2 = num;
			}
			else if (location == Mob.Location.Air || location == Mob.Location.Solid)
			{
				num2 = rnd.RandomRange(0, points.Count);
			}
			else
			{
				for (int i = 0; i < points.Count; i++)
				{
					int num3 = Grid.XYToCell(points[i].x, points[i].y);
					if (Grid.IsValidCell(num3))
					{
						if (num == -1)
						{
							num = i;
						}
						else if (location != Mob.Location.Ceiling)
						{
							if (location == Mob.Location.Floor)
							{
								if (points[i].y < points[num].y)
								{
									num = i;
								}
							}
						}
						else if (points[i].y > points[num].y)
						{
							num = i;
						}
					}
				}
				num2 = num;
			}
			return num2;
		}

		private void PlaceMobInRoom(List<MobReference> mobTags, List<Vector2I> points, SeededRandom rnd)
		{
			if (points != null)
			{
				if (this.mobs == null)
				{
					this.mobs = new List<KeyValuePair<int, Tag>>();
				}
				for (int i = 0; i < mobTags.Count; i++)
				{
					if (!WorldGen.Settings.mobs.HasMob(mobTags[i].type))
					{
						global::Debug.LogError("Missing sample description for tag [" + mobTags[i].type + "]", null);
					}
					else
					{
						Mob mob = WorldGen.Settings.mobs.GetMob(mobTags[i].type);
						int num = Mathf.RoundToInt(mobTags[i].count.GetRandomValueWithinRange(rnd));
						for (int j = 0; j < num; j++)
						{
							int indexForLocation = this.GetIndexForLocation(points, mob.location, rnd);
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
							this.AddMob(num2, mobTags[i].type);
						}
					}
				}
			}
		}

		private int[] ConvertNoiseToPoints(float minThreshold = 0.9f, float maxThreshold = 1f)
		{
			float[] baseNoiseMap = WorldGen.BaseNoiseMap;
			int[] array;
			if (baseNoiseMap == null)
			{
				array = null;
			}
			else
			{
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
				array = list.ToArray();
			}
			return array;
		}

		private void ApplyForeground(Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			this.LogInfo("Apply foregreound", (this.node.tags != null).ToString(), (float)((this.node.tags == null) ? 0 : this.node.tags.Count));
			if (this.node.tags != null)
			{
				FeatureSettings featureSettings = WorldGen.Settings.GetFeature(this.node.type);
				this.LogInfo("\tFeature?", (featureSettings != null).ToString(), 0f);
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
					this.LogInfo("\tNo feature, checking possible feature tags, found", "", (float)list.Count);
					if (list.Count > 0)
					{
						Tag tag2 = list[rnd.RandomSource().Next(list.Count)];
						featureSettings = WorldGen.Settings.GetFeature(tag2.Name);
						this.LogInfo("\tPicked feature", tag2.Name, 0f);
					}
				}
				if (featureSettings != null)
				{
					this.LogInfo("APPLY FOREGROUND", this.node.type, 0f);
					float num = featureSettings.blobSize.GetRandomValueWithinRange(rnd);
					float num2 = this.poly.DistanceToClosestEdge(null);
					if (!this.node.tags.Contains(WorldGenTags.AllowExceedNodeBorders) && num2 < num)
					{
						if (this.debugMode)
						{
							global::Debug.LogWarning(string.Concat(new object[]
							{
								this.node.type,
								" ",
								featureSettings.shape,
								"  blob size too large to fit in node. Size reduced. ",
								num,
								"->",
								(num2 - 6f).ToString()
							}), null);
						}
						num = num2 - 6f;
					}
					if (num > 0f)
					{
						HashSet<Vector2I> hashSet = this.DigFeature(featureSettings.shape, num, featureSettings.borders, rnd);
						this.availablePoints.ExceptWith(hashSet);
						this.LogInfo("\t\t", "claimed points", (float)hashSet.Count);
						this.ApplyPlaceElementForRoom(featureSettings, "RoomCenterElements", this.centerPoints, world, SetValues, temperatureMin, temperatureRange, rnd);
						if (this.borders != null)
						{
							for (int i = 0; i < this.borders.Count; i++)
							{
								this.ApplyPlaceElementForRoom(featureSettings, "RoomBorderChoices" + i, this.borders[i], world, SetValues, temperatureMin, temperatureRange, rnd);
							}
						}
						if (featureSettings.internalMobs != null && featureSettings.internalMobs.Count > 0)
						{
							this.LogInfo("\t\t", "internal mobs", (float)featureSettings.internalMobs.Count);
							if (!this.doneMobs)
							{
								if (!this.disableRoomMobs)
								{
								}
								this.doneMobs = true;
							}
						}
					}
				}
			}
		}

		private void ApplyBackground(Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			float defaultFloat = WorldGen.Settings.GetDefaultFloat("CaveOverrideMaxValue");
			float defaultFloat2 = WorldGen.Settings.GetDefaultFloat("CaveOverrideSliverValue");
			Leaf leafForTerrainCell = WorldGen.GetLeafForTerrainCell(this);
			bool flag = leafForTerrainCell.tags.Contains(WorldGenTags.IgnoreCaveOverride);
			bool flag2 = leafForTerrainCell.tags.Contains(WorldGenTags.CaveVoidSliver);
			bool flag3 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToCentroid);
			bool flag4 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToCentroidInv);
			bool flag5 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToEdge);
			bool flag6 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToEdgeInv);
			bool flag7 = leafForTerrainCell.tags.Contains(WorldGenTags.DistFunctionPointCentroid);
			bool flag8 = leafForTerrainCell.tags.Contains(WorldGenTags.DistFunctionPointEdge);
			Sim.DiseaseCell diseaseCell = default(Sim.DiseaseCell);
			diseaseCell.diseaseIdx = byte.MaxValue;
			if (this.node.tags.Contains(WorldGenTags.Infected))
			{
				diseaseCell.diseaseIdx = (byte)rnd.RandomRange(0, WorldGen.diseaseIds.Count);
				this.node.tags.Add(new Tag("Infected:" + WorldGen.diseaseIds[(int)diseaseCell.diseaseIdx]));
				diseaseCell.elementCount = rnd.RandomRange(10000, 1000000);
			}
			foreach (Vector2I vector2I in this.availablePoints)
			{
				int num = Grid.XYToCell(vector2I.x, vector2I.y);
				float num2 = world.overrides[num];
				if (!flag && num2 >= 100f)
				{
					if (num2 >= 300f)
					{
						SetValues(num, WorldGen.voidElement, WorldGen.voidElement.defaultValues, Sim.DiseaseCell.Invalid);
					}
					else if (num2 >= 200f)
					{
						SetValues(num, WorldGen.unobtaniumElement, WorldGen.unobtaniumElement.defaultValues, Sim.DiseaseCell.Invalid);
					}
					else
					{
						SetValues(num, WorldGen.katairiteElement, WorldGen.katairiteElement.defaultValues, Sim.DiseaseCell.Invalid);
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
					Sim.DiseaseCell diseaseCell2;
					WorldGen.GetElementForBiome(world, this.node.type, vector2I, out element, out defaultValues, out diseaseCell2, num3);
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
						if (!flag && num2 > defaultFloat && num2 < 100f)
						{
							if (flag2 && num2 > defaultFloat2)
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
					if (diseaseCell2.diseaseIdx == 255)
					{
						diseaseCell2 = diseaseCell;
					}
					SetValues(num, element, defaultValues, diseaseCell2);
				}
			}
			if (this.node.tags.Contains(WorldGenTags.SprinkleOfOxyRock))
			{
				this.HandleSprinkleOfElement(WorldGenTags.SprinkleOfOxyRock, world, SetValues, temperatureMin, temperatureRange, rnd);
			}
			if (this.node.tags.Contains(WorldGenTags.SprinkleOfMetal))
			{
				this.HandleSprinkleOfElement(WorldGenTags.SprinkleOfMetal, world, SetValues, temperatureMin, temperatureRange, rnd);
			}
		}

		private void GenerateActionCells(Tag tag, HashSet<Vector2I> possiblePoints, SeededRandom rnd)
		{
			global::ProcGen.Room desription = WorldGen.Settings.rooms.GetDesription(tag);
			SampleDescriber sampleDescriber = desription;
			if (sampleDescriber == null && WorldGen.Settings.mobs.GetMobTags().Contains(tag))
			{
				sampleDescriber = WorldGen.Settings.mobs.GetMob(tag.Name);
			}
			if (sampleDescriber != null)
			{
				HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
				float randomValueWithinRange = sampleDescriber.density.GetRandomValueWithinRange(rnd);
				SampleDescriber.PointSelectionMethod selectMethod = sampleDescriber.selectMethod;
				List<Vector2> list;
				if (selectMethod != SampleDescriber.PointSelectionMethod.RandomPoints)
				{
					if (selectMethod != SampleDescriber.PointSelectionMethod.Centroid)
					{
					}
					list = new List<Vector2>();
					list.Add(this.node.position);
				}
				else
				{
					list = PointGenerator.GetRandomPoints(this.poly, randomValueWithinRange, 0f, null, sampleDescriber.sampleBehaviour, true, rnd, true, true);
				}
				foreach (Vector2 vector in list)
				{
					Vector2I vector2I = new Vector2I((int)vector.x, (int)vector.y);
					if (possiblePoints.Contains(vector2I))
					{
						hashSet.Add(vector2I);
					}
				}
				if (desription != null && desription.mobselection == global::ProcGen.Room.Selection.None)
				{
					if (this.terrainPositions == null)
					{
						this.terrainPositions = new List<KeyValuePair<int, Tag>>();
					}
					foreach (Vector2I vector2I2 in hashSet)
					{
						int num = Grid.XYToCell(vector2I2.x, vector2I2.y);
						if (Grid.IsValidCell(num))
						{
							this.terrainPositions.Add(new KeyValuePair<int, Tag>(num, tag));
						}
					}
				}
			}
		}

		private void DoProcess(Chunk world, TerrainCell.SetValuesFunction SetValues, SeededRandom rnd)
		{
			float num = 265f;
			float num2 = 30f;
			this.GetAllCells();
			this.GetTemperatureRange(ref num, ref num2);
			this.ApplyForeground(world, SetValues, num, num2, rnd);
			for (int i = 0; i < this.node.tags.Count; i++)
			{
				this.GenerateActionCells(this.node.tags[i], this.availablePoints, rnd);
			}
			this.ApplyBackground(world, SetValues, num, num2, rnd);
		}

		public void Process(Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dcs, Chunk world, SeededRandom rnd)
		{
			TerrainCell.SetValuesFunction setValuesFunction = delegate(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc)
			{
				if (Grid.IsValidCell(index))
				{
					if (pd.temperature == 0f || (elem as Element).HasTag(GameTags.Special))
					{
						bgTemp[index] = -1f;
					}
					cells[index].SetValues(elem as Element, pd, ElementLoader.elements);
					dcs[index] = dc;
				}
				else
				{
					global::Debug.LogError(string.Concat(new object[] { "Process::SetValuesFunction Index [", index, "] is not valid. cells.Length [", cells.Length, "]" }), null);
				}
			};
			this.DoProcess(world, setValuesFunction, rnd);
		}

		public void Process(Chunk world, SeededRandom rnd)
		{
			TerrainCell.SetValuesFunction setValuesFunction = delegate(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc)
			{
				SimMessages.ModifyCell(index, ElementLoader.GetElementIndex((elem as Element).id), pd.temperature, pd.mass, dc.diseaseIdx, dc.elementCount, SimMessages.ReplaceType.Replace, -1);
			};
			this.DoProcess(world, setValuesFunction, rnd);
		}

		[OnDeserializing]
		internal void OnDeserializingMethod()
		{
			this.node = new global::ProcGen.Node();
			this.site = new Diagram.Site();
		}

		public List<KeyValuePair<int, Tag>> terrainPositions = null;

		public List<KeyValuePair<int, Tag>> poi = null;

		public List<uint> terrain_neighbors_idx = new List<uint>();

		private float finalSize = 0f;

		private bool doneMobs = false;

		private bool debugMode = false;

		private bool disableRoomMobs = false;

		private List<int> allCells = null;

		private HashSet<Vector2I> availablePoints = null;

		private List<Vector2I> centerPoints = null;

		private List<List<Vector2I>> borders = null;

		private static HashSet<int> claimedCells = new HashSet<int>();

		public const int DONT_SET_TEMPERATURE_DEFAULTS = -1;

		public delegate void SetValuesFunction(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc);

		public struct ElementOverride
		{
			public Element element;

			public Sim.PhysicsData pdelement;

			public Sim.DiseaseCell dc;

			public float mass;

			public float temperature;

			public byte diseaseIdx;

			public int diseaseAmount;

			public bool overrideMass;

			public bool overrideTemperature;

			public bool overrideDiseaseIdx;

			public bool overrideDiseaseAmount;
		}
	}
}
