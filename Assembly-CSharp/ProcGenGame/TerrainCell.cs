using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay.Geo;
using KSerialization;
using ProcGen;
using ProcGen.Map;
using UnityEngine;
using VoronoiTree;

namespace ProcGenGame
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class TerrainCell
	{
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

		protected TerrainCell()
		{
		}

		protected TerrainCell(global::ProcGen.Node node, Diagram.Site site)
		{
			this.node = node;
			this.site = site;
			this.node.SetPosition(site.position);
		}

		public virtual void LogInfo(string evt, string param, float value)
		{
			global::Debug.Log(string.Concat(new object[] { evt, ":", param, "=", value }));
		}

		public static HashSet<int> GetClaimedCells()
		{
			return TerrainCell.claimedCells;
		}

		public static HashSet<int> GetHighPriorityClaimCells()
		{
			return TerrainCell.highPriorityClaims;
		}

		public static void ClearClaimedCells()
		{
			TerrainCell.claimedCells.Clear();
			TerrainCell.highPriorityClaims.Clear();
		}

		public void InitializeCells()
		{
			if (this.allCells != null)
			{
				return;
			}
			this.allCells = new List<int>();
			this.availableTerrainPoints = new HashSet<Vector2I>();
			this.availableSpawnPoints = new HashSet<Vector2I>();
			for (int i = 0; i < Grid.HeightInCells; i++)
			{
				for (int j = 0; j < Grid.WidthInCells; j++)
				{
					if (this.poly.Contains(new Vector2((float)j, (float)i)))
					{
						int num = Grid.XYToCell(j, i);
						this.availableTerrainPoints.Add(Grid.CellToXY(num));
						this.availableSpawnPoints.Add(Grid.CellToXY(num));
						if (TerrainCell.claimedCells.Add(num))
						{
							this.allCells.Add(num);
						}
					}
				}
			}
			this.LogInfo("Initialise cells", "", (float)this.allCells.Count);
		}

		public List<int> GetAllCells()
		{
			return new List<int>(this.allCells);
		}

		public List<int> GetAvailableSpawnCellsAll()
		{
			List<int> list = new List<int>();
			foreach (Vector2I vector2I in this.availableSpawnPoints)
			{
				list.Add(Grid.XYToCell(vector2I.x, vector2I.y));
			}
			return list;
		}

		public List<int> GetAvailableSpawnCellsFeature()
		{
			List<int> list = new List<int>();
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>(this.availableSpawnPoints);
			hashSet.ExceptWith(this.availableTerrainPoints);
			foreach (Vector2I vector2I in hashSet)
			{
				list.Add(Grid.XYToCell(vector2I.x, vector2I.y));
			}
			return list;
		}

		public List<int> GetAvailableSpawnCellsBiome()
		{
			List<int> list = new List<int>();
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>(this.availableSpawnPoints);
			hashSet.ExceptWith(this.featureSpawnPoints);
			foreach (Vector2I vector2I in hashSet)
			{
				list.Add(Grid.XYToCell(vector2I.x, vector2I.y));
			}
			return list;
		}

		public List<int> GetAvailableTerrainCells()
		{
			List<int> list = new List<int>();
			foreach (Vector2I vector2I in this.availableTerrainPoints)
			{
				list.Add(Grid.XYToCell(vector2I.x, vector2I.y));
			}
			return list;
		}

		private void AddHighPriorityCells(HashSet<Vector2I> cells)
		{
			foreach (Vector2I vector2I in cells)
			{
				int num = Grid.XYToCell(vector2I.x, vector2I.y);
				TerrainCell.highPriorityClaims.Add(num);
			}
		}

		private bool RemoveFromAvailableSpawnCells(int cell)
		{
			int num;
			int num2;
			Grid.CellToXY(cell, out num, out num2);
			Vector2I vector2I = new Vector2I(num, num2);
			return this.availableSpawnPoints.Remove(vector2I);
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
			bool flag = this.RemoveFromAvailableSpawnCells(mob.Key);
			this.LogInfo("\t\t\tRemoveFromAvailableCells", mob.Value.Name + ": " + (flag ? "success" : "failed"), (float)mob.Key);
			if (!flag)
			{
				if (!this.allCells.Contains(mob.Key))
				{
					global::Debug.Assert(false, string.Concat(new object[]
					{
						"Couldnt find cell [",
						mob.Key,
						"] we dont own, to remove for mob [",
						mob.Value.Name,
						"]"
					}));
					return;
				}
				global::Debug.Assert(false, string.Concat(new object[]
				{
					"Couldnt find cell [",
					mob.Key,
					"] to remove for mob [",
					mob.Value.Name,
					"]"
				}));
			}
		}

		protected string GetSubWorldType(WorldGen worldGen)
		{
			Vector2I vector2I = new Vector2I((int)this.site.poly.Centroid().x, (int)this.site.poly.Centroid().y);
			return worldGen.GetSubWorldType(vector2I);
		}

		protected Temperature.Range GetTemperatureRange(WorldGen worldGen)
		{
			string subWorldType = this.GetSubWorldType(worldGen);
			if (subWorldType == null)
			{
				return Temperature.Range.Mild;
			}
			if (!worldGen.Settings.HasSubworld(subWorldType))
			{
				return Temperature.Range.Mild;
			}
			return worldGen.Settings.GetSubWorld(subWorldType).temperatureRange;
		}

		protected void GetTemperatureRange(WorldGen worldGen, ref float min, ref float range)
		{
			Temperature.Range temperatureRange = this.GetTemperatureRange(worldGen);
			min = SettingsCache.temperatures[temperatureRange].min;
			range = SettingsCache.temperatures[temperatureRange].max - min;
		}

		protected float GetDensityMassForCell(Chunk world, int cellIdx, float mass)
		{
			if (!Grid.IsValidCell(cellIdx))
			{
				return 0f;
			}
			global::Debug.Assert(world.density[cellIdx] >= 0f && world.density[cellIdx] <= 1f, "Density [" + world.density[cellIdx] + "] out of range [0-1]");
			float num = world.density[cellIdx] - 0.5f;
			float num2 = mass + mass * num;
			if (num2 > 10000f)
			{
				num2 = 10000f;
			}
			return num2;
		}

		private void HandleSprinkleOfElement(WorldGenSettings settings, Tag targetTag, Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			Element element = ElementLoader.FindElementByName(settings.GetFeature(targetTag.Name).GetOneWeightedSimHash("SprinkleOfElementChoices", rnd).element);
			global::ProcGen.Room room = null;
			SettingsCache.rooms.TryGetValue(targetTag.Name, out room);
			SampleDescriber sampleDescriber = room;
			Sim.PhysicsData defaultValues = element.defaultValues;
			Sim.DiseaseCell invalid = Sim.DiseaseCell.Invalid;
			for (int i = 0; i < this.terrainPositions.Count; i++)
			{
				if (!(this.terrainPositions[i].Value != targetTag))
				{
					float num = rnd.RandomRange(sampleDescriber.blobSize.min, sampleDescriber.blobSize.max);
					List<Vector2I> filledCircle = global::ProcGen.Util.GetFilledCircle(Grid.CellToPos2D(this.terrainPositions[i].Key), num);
					for (int j = 0; j < filledCircle.Count; j++)
					{
						int num2 = Grid.XYToCell(filledCircle[j].x, filledCircle[j].y);
						if (Grid.IsValidCell(num2))
						{
							defaultValues.mass = this.GetDensityMassForCell(world, num2, element.defaultValues.mass);
							defaultValues.temperature = temperatureMin + world.heatOffset[num2] * temperatureRange;
							SetValues(num2, element, defaultValues, invalid);
						}
					}
				}
			}
		}

		private HashSet<Vector2I> DigFeature(global::ProcGen.Room.Shape shape, float size, List<int> bordersWidths, SeededRandom rnd, out List<Vector2I> featureCenterPoints, out List<List<Vector2I>> featureBorders)
		{
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			featureCenterPoints = new List<Vector2I>();
			featureBorders = new List<List<Vector2I>>();
			if (size < 1f)
			{
				return hashSet;
			}
			Vector2 vector = this.site.poly.Centroid();
			this.finalSize = size;
			switch (shape)
			{
			case global::ProcGen.Room.Shape.Circle:
				featureCenterPoints = global::ProcGen.Util.GetFilledCircle(vector, this.finalSize);
				break;
			case global::ProcGen.Room.Shape.Blob:
				featureCenterPoints = global::ProcGen.Util.GetBlob(vector, this.finalSize, rnd.RandomSource());
				break;
			case global::ProcGen.Room.Shape.Square:
				featureCenterPoints = global::ProcGen.Util.GetFilledRectangle(vector, this.finalSize, this.finalSize, rnd, 2f, 2f);
				break;
			case global::ProcGen.Room.Shape.TallThin:
				featureCenterPoints = global::ProcGen.Util.GetFilledRectangle(vector, this.finalSize / 4f, this.finalSize, rnd, 2f, 2f);
				break;
			case global::ProcGen.Room.Shape.ShortWide:
				featureCenterPoints = global::ProcGen.Util.GetFilledRectangle(vector, this.finalSize, this.finalSize / 4f, rnd, 2f, 2f);
				break;
			case global::ProcGen.Room.Shape.Splat:
				featureCenterPoints = global::ProcGen.Util.GetSplat(vector, this.finalSize, rnd.RandomSource());
				break;
			}
			hashSet.UnionWith(featureCenterPoints);
			if (featureCenterPoints.Count == 0)
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
				}));
			}
			else if (bordersWidths != null && bordersWidths.Count > 0 && bordersWidths[0] > 0)
			{
				int num = 0;
				while (num < bordersWidths.Count && bordersWidths[num] > 0)
				{
					featureBorders.Add(global::ProcGen.Util.GetBorder(hashSet, bordersWidths[num]));
					hashSet.UnionWith(featureBorders[num]);
					num++;
				}
			}
			return hashSet;
		}

		public static TerrainCell.ElementOverride GetElementOverride(string element, SampleDescriber.Override overrides)
		{
			global::Debug.Assert(element != null && element.Length > 0);
			TerrainCell.ElementOverride elementOverride = new TerrainCell.ElementOverride
			{
				element = ElementLoader.FindElementByName(element)
			};
			global::Debug.Assert(elementOverride.element != null, "Couldn't find an element called " + element);
			elementOverride.pdelement = elementOverride.element.defaultValues;
			elementOverride.dc = Sim.DiseaseCell.Invalid;
			elementOverride.mass = elementOverride.element.defaultValues.mass;
			elementOverride.temperature = elementOverride.element.defaultValues.temperature;
			if (overrides == null)
			{
				return elementOverride;
			}
			elementOverride.overrideMass = false;
			elementOverride.overrideTemperature = false;
			elementOverride.overrideDiseaseIdx = false;
			elementOverride.overrideDiseaseAmount = false;
			if (overrides.massOverride != null)
			{
				elementOverride.mass = overrides.massOverride.Value;
				elementOverride.overrideMass = true;
			}
			if (overrides.massMultiplier != null)
			{
				elementOverride.mass *= overrides.massMultiplier.Value;
				elementOverride.overrideMass = true;
			}
			if (overrides.temperatureOverride != null)
			{
				elementOverride.temperature = overrides.temperatureOverride.Value;
				elementOverride.overrideTemperature = true;
			}
			if (overrides.temperatureMultiplier != null)
			{
				elementOverride.temperature *= overrides.temperatureMultiplier.Value;
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
			return elementOverride;
		}

		private void ApplyPlaceElementForRoom(FeatureSettings feature, string group, List<Vector2I> cells, Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
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
			case global::ProcGen.Room.Selection.Weighted:
			case global::ProcGen.Room.Selection.WeightedResample:
			{
				for (int i = 0; i < cells.Count; i++)
				{
					int num = Grid.XYToCell(cells[i].x, cells[i].y);
					if (Grid.IsValidCell(num) && !TerrainCell.highPriorityClaims.Contains(num))
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
				return;
			}
			}
			WeightedSimHash oneWeightedSimHash2 = feature.GetOneWeightedSimHash(group, rnd);
			DebugUtil.LogArgs(new object[] { "Picked one: ", oneWeightedSimHash2.element });
			for (int j = 0; j < cells.Count; j++)
			{
				int num2 = Grid.XYToCell(cells[j].x, cells[j].y);
				if (Grid.IsValidCell(num2) && !TerrainCell.highPriorityClaims.Contains(num2))
				{
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

		private int GetIndexForLocation(List<Vector2I> points, Mob.Location location, SeededRandom rnd)
		{
			int num = -1;
			if (points == null || points.Count == 0)
			{
				return num;
			}
			if (location == Mob.Location.Air || location == Mob.Location.Solid)
			{
				return rnd.RandomRange(0, points.Count);
			}
			for (int i = 0; i < points.Count; i++)
			{
				if (Grid.IsValidCell(Grid.XYToCell(points[i].x, points[i].y)))
				{
					if (num == -1)
					{
						num = i;
					}
					else if (location != Mob.Location.Floor)
					{
						if (location == Mob.Location.Ceiling && points[i].y > points[num].y)
						{
							num = i;
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

		private void PlaceMobsInRoom(WorldGenSettings settings, List<MobReference> mobTags, List<Vector2I> points, SeededRandom rnd)
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
				if (!settings.HasMob(mobTags[i].type))
				{
					global::Debug.LogError("Missing sample description for tag [" + mobTags[i].type + "]");
				}
				else
				{
					Mob mob = settings.GetMob(mobTags[i].type);
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

		private int[] ConvertNoiseToPoints(float[] basenoise, float minThreshold = 0.9f, float maxThreshold = 1f)
		{
			if (basenoise == null)
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
						float num4 = (float)((int)basenoise[num3]);
						if (num4 >= minThreshold && num4 <= maxThreshold && !list.Contains(num3))
						{
							list.Add(Grid.PosToCell(new Vector2(num2, num)));
						}
					}
				}
			}
			return list.ToArray();
		}

		private void ApplyForeground(WorldGenSettings settings, Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			this.LogInfo("Apply foregreound", (this.node.tags != null).ToString(), (float)((this.node.tags != null) ? this.node.tags.Count : 0));
			if (this.node.tags != null)
			{
				FeatureSettings featureSettings = settings.TryGetFeature(this.node.type);
				this.LogInfo("\tFeature?", (featureSettings != null).ToString(), 0f);
				if (featureSettings == null && this.node.tags != null)
				{
					List<Tag> list = new List<Tag>();
					foreach (Tag tag in this.node.tags)
					{
						if (settings.HasFeature(tag.Name))
						{
							list.Add(tag);
						}
					}
					this.LogInfo("\tNo feature, checking possible feature tags, found", "", (float)list.Count);
					if (list.Count > 0)
					{
						Tag tag2 = list[rnd.RandomSource().Next(list.Count)];
						featureSettings = settings.GetFeature(tag2.Name);
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
							}));
						}
						num = num2 - 6f;
					}
					if (num <= 0f)
					{
						return;
					}
					List<Vector2I> list2;
					List<List<Vector2I>> list3;
					this.featureSpawnPoints = this.DigFeature(featureSettings.shape, num, featureSettings.borders, rnd, out list2, out list3);
					this.LogInfo("\t\t", "claimed points", (float)this.featureSpawnPoints.Count);
					this.availableTerrainPoints.ExceptWith(this.featureSpawnPoints);
					this.ApplyPlaceElementForRoom(featureSettings, "RoomCenterElements", list2, world, SetValues, temperatureMin, temperatureRange, rnd);
					if (list3 != null)
					{
						for (int i = 0; i < list3.Count; i++)
						{
							this.ApplyPlaceElementForRoom(featureSettings, "RoomBorderChoices" + i, list3[i], world, SetValues, temperatureMin, temperatureRange, rnd);
						}
					}
					if (featureSettings.tags.Contains(WorldGenTags.HighPriorityFeature.Name))
					{
						this.AddHighPriorityCells(this.featureSpawnPoints);
					}
				}
			}
		}

		private void ApplyBackground(WorldGen worldGen, Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			this.LogInfo("Apply Background", this.node.type, 0f);
			float floatSetting = worldGen.Settings.GetFloatSetting("CaveOverrideMaxValue");
			float floatSetting2 = worldGen.Settings.GetFloatSetting("CaveOverrideSliverValue");
			Leaf leafForTerrainCell = worldGen.GetLeafForTerrainCell(this);
			bool flag = leafForTerrainCell.tags.Contains(WorldGenTags.IgnoreCaveOverride);
			bool flag2 = leafForTerrainCell.tags.Contains(WorldGenTags.CaveVoidSliver);
			bool flag3 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToCentroid);
			bool flag4 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToCentroidInv);
			bool flag5 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToEdge);
			bool flag6 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToEdgeInv);
			bool flag7 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToBorder);
			bool flag8 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToBorderInv);
			bool flag9 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToWorldTop);
			bool flag10 = leafForTerrainCell.tags.Contains(WorldGenTags.DistFunctionPointCentroid);
			bool flag11 = leafForTerrainCell.tags.Contains(WorldGenTags.DistFunctionPointEdge);
			Sim.DiseaseCell diseaseCell = default(Sim.DiseaseCell);
			diseaseCell.diseaseIdx = byte.MaxValue;
			if (this.node.tags.Contains(WorldGenTags.Infected))
			{
				diseaseCell.diseaseIdx = (byte)rnd.RandomRange(0, WorldGen.diseaseIds.Count);
				this.node.tags.Add(new Tag("Infected:" + WorldGen.diseaseIds[(int)diseaseCell.diseaseIdx]));
				diseaseCell.elementCount = rnd.RandomRange(10000, 1000000);
			}
			this.LogInfo("Getting Element Bands", this.node.type, 0f);
			ElementBandConfiguration elementBandConfiguration = worldGen.Settings.GetElementBandForBiome(this.node.type);
			if (elementBandConfiguration == null && this.node.biomeSpecificTags != null)
			{
				this.LogInfo("\tType is not a biome, checking tags", "", (float)this.node.tags.Count);
				List<ElementBandConfiguration> list = new List<ElementBandConfiguration>();
				foreach (Tag tag in this.node.biomeSpecificTags)
				{
					ElementBandConfiguration elementBandForBiome = worldGen.Settings.GetElementBandForBiome(tag.Name);
					if (elementBandForBiome != null)
					{
						list.Add(elementBandForBiome);
						this.LogInfo("\tFound biome", tag.Name, 0f);
					}
				}
				if (list.Count > 0)
				{
					int num = rnd.RandomSource().Next(list.Count);
					elementBandConfiguration = list[num];
					this.LogInfo("\tPicked biome", "", (float)num);
				}
			}
			DebugUtil.Assert(elementBandConfiguration != null, "A node didn't get assigned a biome! ", this.node.type);
			foreach (Vector2I vector2I in this.availableTerrainPoints)
			{
				int num2 = Grid.XYToCell(vector2I.x, vector2I.y);
				if (!TerrainCell.highPriorityClaims.Contains(num2))
				{
					float num3 = world.overrides[num2];
					if (!flag && num3 >= 100f)
					{
						if (num3 >= 300f)
						{
							SetValues(num2, WorldGen.voidElement, WorldGen.voidElement.defaultValues, Sim.DiseaseCell.Invalid);
						}
						else if (num3 >= 200f)
						{
							SetValues(num2, WorldGen.unobtaniumElement, WorldGen.unobtaniumElement.defaultValues, Sim.DiseaseCell.Invalid);
						}
						else
						{
							SetValues(num2, WorldGen.katairiteElement, WorldGen.katairiteElement.defaultValues, Sim.DiseaseCell.Invalid);
						}
					}
					else
					{
						float num4 = 1f;
						Vector2 vector = new Vector2((float)vector2I.x, (float)vector2I.y);
						if (flag3 || flag4)
						{
							float num5 = 15f;
							if (flag11)
							{
								float num6 = 0f;
								MathUtil.Pair<Vector2, Vector2> closestEdge = this.poly.GetClosestEdge(vector, ref num6);
								num5 = Vector2.Distance(closestEdge.First + (closestEdge.Second - closestEdge.First) * num6, vector);
							}
							num4 = Vector2.Distance(this.poly.Centroid(), vector) / num5;
							num4 = Mathf.Max(0f, Mathf.Min(1f, num4));
							if (flag4)
							{
								num4 = 1f - num4;
							}
						}
						if (flag6 || flag5)
						{
							float num7 = 0f;
							MathUtil.Pair<Vector2, Vector2> closestEdge2 = this.poly.GetClosestEdge(vector, ref num7);
							Vector2 vector2 = closestEdge2.First + (closestEdge2.Second - closestEdge2.First) * num7;
							float num8 = 15f;
							if (flag10)
							{
								num8 = Vector2.Distance(this.poly.Centroid(), vector);
							}
							num4 = Vector2.Distance(vector2, vector) / num8;
							num4 = Mathf.Max(0f, Mathf.Min(1f, num4));
							if (flag6)
							{
								num4 = 1f - num4;
							}
						}
						if (flag8 || flag7)
						{
							List<Edge> edgesWithTag = worldGen.WorldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeClosed);
							float num9 = float.MaxValue;
							foreach (Edge edge in edgesWithTag)
							{
								MathUtil.Pair<Vector2, Vector2> pair = new MathUtil.Pair<Vector2, Vector2>(edge.corner0.position, edge.corner1.position);
								float num10 = 0f;
								num9 = Mathf.Min(Mathf.Abs(MathUtil.GetClosestPointBetweenPointAndLineSegment(pair, vector, ref num10)), num9);
							}
							float num11 = 7f;
							if (flag10)
							{
								num11 = Vector2.Distance(this.poly.Centroid(), vector);
							}
							num4 = num9 / num11;
							num4 = Mathf.Max(0f, Mathf.Min(1f, num4));
							if (flag8)
							{
								num4 = 1f - num4;
							}
						}
						if (flag9)
						{
							float y = (float)worldGen.WorldSize.y;
							float num12 = 38f;
							float num13 = 58f;
							float num14 = y - vector.y;
							if (num14 < num12)
							{
								num4 = 0f;
							}
							else if (num14 < num13)
							{
								num4 = Mathf.Clamp01((num14 - num12) / (num13 - num12));
							}
							else
							{
								num4 = 1f;
							}
						}
						Element element;
						Sim.PhysicsData defaultValues;
						Sim.DiseaseCell diseaseCell2;
						worldGen.GetElementForBiomePoint(world, elementBandConfiguration, vector2I, out element, out defaultValues, out diseaseCell2, num4);
						if (!element.IsVacuum && element.id != SimHashes.Katairite && element.id != SimHashes.Unobtanium)
						{
							if (element.lowTempTransition != null && temperatureMin < element.lowTemp)
							{
								temperatureMin = element.lowTemp + 20f;
							}
							defaultValues.temperature = temperatureMin + world.heatOffset[num2] * temperatureRange;
						}
						if (element.IsSolid && !flag && num3 > floatSetting && num3 < 100f)
						{
							if (flag2 && num3 > floatSetting2)
							{
								element = WorldGen.voidElement;
							}
							else
							{
								element = WorldGen.vacuumElement;
							}
							defaultValues = element.defaultValues;
						}
						if (diseaseCell2.diseaseIdx == 255)
						{
							diseaseCell2 = diseaseCell;
						}
						SetValues(num2, element, defaultValues, diseaseCell2);
					}
				}
			}
			if (this.node.tags.Contains(WorldGenTags.SprinkleOfOxyRock))
			{
				this.HandleSprinkleOfElement(worldGen.Settings, WorldGenTags.SprinkleOfOxyRock, world, SetValues, temperatureMin, temperatureRange, rnd);
			}
			if (this.node.tags.Contains(WorldGenTags.SprinkleOfMetal))
			{
				this.HandleSprinkleOfElement(worldGen.Settings, WorldGenTags.SprinkleOfMetal, world, SetValues, temperatureMin, temperatureRange, rnd);
			}
		}

		private void GenerateActionCells(WorldGenSettings settings, Tag tag, HashSet<Vector2I> possiblePoints, SeededRandom rnd)
		{
			global::ProcGen.Room room = null;
			SettingsCache.rooms.TryGetValue(tag.Name, out room);
			SampleDescriber sampleDescriber = room;
			if (sampleDescriber == null && settings.HasMob(tag.Name))
			{
				sampleDescriber = settings.GetMob(tag.Name);
			}
			if (sampleDescriber == null)
			{
				return;
			}
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
			if (room != null && room.mobselection == global::ProcGen.Room.Selection.None)
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

		private void DoProcess(WorldGen worldGen, Chunk world, TerrainCell.SetValuesFunction SetValues, SeededRandom rnd)
		{
			float num = 265f;
			float num2 = 30f;
			this.InitializeCells();
			this.GetTemperatureRange(worldGen, ref num, ref num2);
			this.ApplyForeground(worldGen.Settings, world, SetValues, num, num2, rnd);
			for (int i = 0; i < this.node.tags.Count; i++)
			{
				this.GenerateActionCells(worldGen.Settings, this.node.tags[i], this.availableTerrainPoints, rnd);
			}
			this.ApplyBackground(worldGen, world, SetValues, num, num2, rnd);
		}

		public void Process(WorldGen worldGen, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dcs, Chunk world, SeededRandom rnd)
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
					return;
				}
				global::Debug.LogError(string.Concat(new object[] { "Process::SetValuesFunction Index [", index, "] is not valid. cells.Length [", cells.Length, "]" }));
			};
			this.DoProcess(worldGen, world, setValuesFunction, rnd);
		}

		public void Process(WorldGen worldGen, Chunk world, SeededRandom rnd)
		{
			TerrainCell.SetValuesFunction setValuesFunction = delegate(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc)
			{
				SimMessages.ModifyCell(index, ElementLoader.GetElementIndex((elem as Element).id), pd.temperature, pd.mass, dc.diseaseIdx, dc.elementCount, SimMessages.ReplaceType.Replace, false, -1);
			};
			this.DoProcess(worldGen, world, setValuesFunction, rnd);
		}

		[OnDeserializing]
		internal void OnDeserializingMethod()
		{
			this.node = new global::ProcGen.Node();
			this.site = new Diagram.Site();
		}

		public bool IsSafeToSpawnFeatureTemplate(Tag additionalTag)
		{
			return !this.node.tags.Contains(additionalTag) && !this.node.tags.ContainsOne(TerrainCell.noFeatureSpawnTagSet);
		}

		public bool IsSafeToSpawnFeatureTemplate()
		{
			return !this.node.tags.ContainsOne(TerrainCell.noFeatureSpawnTagSet);
		}

		public bool IsSafeToSpawnPOI(List<TerrainCell> allCells)
		{
			using (List<uint>.Enumerator enumerator = this.terrain_neighbors_idx.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					uint neighbor_idx = enumerator.Current;
					if (allCells.Find((TerrainCell cell) => cell.site.id == neighbor_idx).node.tags.ContainsOne(TerrainCell.noPOINeighborSpawnTagSet))
					{
						return false;
					}
				}
			}
			return !this.node.tags.ContainsOne(TerrainCell.noPOISpawnTagSet);
		}

		public List<KeyValuePair<int, Tag>> terrainPositions;

		public List<KeyValuePair<int, Tag>> poi;

		public List<uint> terrain_neighbors_idx = new List<uint>();

		private float finalSize;

		private bool debugMode;

		private List<int> allCells;

		private HashSet<Vector2I> availableTerrainPoints;

		private HashSet<Vector2I> featureSpawnPoints;

		private HashSet<Vector2I> availableSpawnPoints;

		private static HashSet<int> claimedCells = new HashSet<int>();

		private static HashSet<int> highPriorityClaims = new HashSet<int>();

		public const int DONT_SET_TEMPERATURE_DEFAULTS = -1;

		private static readonly Tag[] noFeatureSpawnTags = new Tag[]
		{
			WorldGenTags.StartLocation,
			WorldGenTags.AtStart,
			WorldGenTags.NearStartLocation,
			WorldGenTags.POI,
			WorldGenTags.Feature
		};

		private static readonly TagSet noFeatureSpawnTagSet = new TagSet(TerrainCell.noFeatureSpawnTags);

		private static readonly Tag[] noPOISpawnTags = new Tag[]
		{
			WorldGenTags.StartLocation,
			WorldGenTags.AtStart,
			WorldGenTags.NearStartLocation,
			WorldGenTags.POI,
			WorldGenTags.AtEdge,
			WorldGenTags.AtDepths
		};

		private static readonly TagSet noPOISpawnTagSet = new TagSet(TerrainCell.noPOISpawnTags);

		private static readonly Tag[] noPOINeighborSpawnTags = new Tag[] { WorldGenTags.POI };

		private static readonly TagSet noPOINeighborSpawnTagSet = new TagSet(TerrainCell.noPOINeighborSpawnTags);

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
