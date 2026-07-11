using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Klei;
using Klei.AI;
using ProcGen;
using STRINGS;
using UnityEngine;

[Serializable]
public class BuildingDef : Def
{
	public override string Name
	{
		get
		{
			return Strings.Get("STRINGS.BUILDINGS.PREFABS." + this.PrefabID.ToUpper() + ".NAME");
		}
	}

	public string Desc
	{
		get
		{
			return Strings.Get("STRINGS.BUILDINGS.PREFABS." + this.PrefabID.ToUpper() + ".DESC");
		}
	}

	public string Flavor
	{
		get
		{
			return "\"" + Strings.Get("STRINGS.BUILDINGS.PREFABS." + this.PrefabID.ToUpper() + ".FLAVOR") + "\"";
		}
	}

	public string Effect
	{
		get
		{
			return Strings.Get("STRINGS.BUILDINGS.PREFABS." + this.PrefabID.ToUpper() + ".EFFECT");
		}
	}

	public bool IsTilePiece
	{
		get
		{
			return this.TileLayer != ObjectLayer.NumLayers;
		}
	}

	public bool CanReplace(GameObject go)
	{
		if (this.ReplacementTags == null)
		{
			return false;
		}
		foreach (Tag tag in this.ReplacementTags)
		{
			if (go.GetComponent<KPrefabID>().HasTag(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsReplacementLayerOccupied(int cell)
	{
		if (Grid.Objects[cell, (int)this.ReplacementLayer] != null)
		{
			return true;
		}
		if (this.EquivalentReplacementLayers != null)
		{
			foreach (ObjectLayer objectLayer in this.EquivalentReplacementLayers)
			{
				if (Grid.Objects[cell, (int)objectLayer] != null)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public GameObject GetReplacementCandidate(int cell)
	{
		if (this.ReplacementCandidateLayers != null)
		{
			using (List<ObjectLayer>.Enumerator enumerator = this.ReplacementCandidateLayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ObjectLayer objectLayer = enumerator.Current;
					if (Grid.ObjectLayers[(int)objectLayer].ContainsKey(cell))
					{
						GameObject gameObject = Grid.ObjectLayers[(int)objectLayer][cell];
						if (gameObject != null && gameObject.GetComponent<BuildingComplete>() != null)
						{
							return gameObject;
						}
					}
				}
				goto IL_0096;
			}
		}
		if (Grid.ObjectLayers[(int)this.TileLayer].ContainsKey(cell))
		{
			return Grid.ObjectLayers[(int)this.TileLayer][cell];
		}
		IL_0096:
		return null;
	}

	public GameObject Create(Vector3 pos, Storage resource_storage, IList<Tag> selected_elements, Recipe recipe, float temperature, GameObject obj)
	{
		SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;
		if (resource_storage != null)
		{
			Recipe.Ingredient[] allIngredients = recipe.GetAllIngredients(selected_elements);
			if (allIngredients != null)
			{
				foreach (Recipe.Ingredient ingredient in allIngredients)
				{
					SimUtil.DiseaseInfo diseaseInfo2;
					float num;
					resource_storage.ConsumeAndGetDisease(ingredient.tag, ingredient.amount, out diseaseInfo2, out num);
					diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo, diseaseInfo2);
				}
			}
		}
		GameObject gameObject = GameUtil.KInstantiate(obj, pos, this.SceneLayer, null, 0);
		Element element = ElementLoader.GetElement(selected_elements[0]);
		global::Debug.Assert(element != null);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.ElementID = element.id;
		component.Temperature = temperature;
		component.AddDisease(diseaseInfo.idx, diseaseInfo.count, "BuildingDef.Create");
		gameObject.name = obj.name;
		gameObject.SetActive(true);
		return gameObject;
	}

	public List<Tag> DefaultElements()
	{
		List<Tag> list = new List<Tag>();
		foreach (string text in this.MaterialCategory)
		{
			foreach (Element element in ElementLoader.elements)
			{
				if (element.IsSolid && (element.tag.Name == text || element.HasTag(text)))
				{
					list.Add(element.tag);
					break;
				}
			}
		}
		return list;
	}

	public GameObject Build(int cell, Orientation orientation, Storage resource_storage, IList<Tag> selected_elements, float temperature, bool playsound = true, float timeBuilt = -1f)
	{
		Vector3 vector = Grid.CellToPosCBC(cell, this.SceneLayer);
		GameObject gameObject = this.Create(vector, resource_storage, selected_elements, this.CraftRecipe, temperature, this.BuildingComplete);
		Rotatable component = gameObject.GetComponent<Rotatable>();
		if (component != null)
		{
			component.SetOrientation(orientation);
		}
		this.MarkArea(cell, orientation, this.ObjectLayer, gameObject);
		if (this.IsTilePiece)
		{
			this.MarkArea(cell, orientation, this.TileLayer, gameObject);
			this.RunOnArea(cell, orientation, delegate(int c)
			{
				TileVisualizer.RefreshCell(c, this.TileLayer, this.ReplacementLayer);
			});
		}
		string sound = GlobalAssets.GetSound("Finish_Building_" + this.AudioSize, false);
		if (playsound && sound != null)
		{
			Vector3 position = gameObject.transform.GetPosition();
			position.z = 0f;
			KFMOD.PlayOneShot(sound, position, 1f);
		}
		Deconstructable component2 = gameObject.GetComponent<Deconstructable>();
		if (component2 != null)
		{
			component2.constructionElements = new Tag[selected_elements.Count];
			for (int i = 0; i < selected_elements.Count; i++)
			{
				component2.constructionElements[i] = selected_elements[i];
			}
		}
		BuildingComplete component3 = gameObject.GetComponent<BuildingComplete>();
		if (component3)
		{
			component3.SetCreationTime(timeBuilt);
		}
		Game.Instance.Trigger(-1661515756, gameObject);
		gameObject.Trigger(-1661515756, gameObject);
		return gameObject;
	}

	public GameObject TryPlace(GameObject src_go, Vector3 pos, Orientation orientation, IList<Tag> selected_elements, int layer = 0)
	{
		GameObject gameObject = null;
		string text;
		if (this.IsValidPlaceLocation(src_go, pos, orientation, false, out text))
		{
			gameObject = this.Instantiate(pos, orientation, selected_elements, layer);
		}
		return gameObject;
	}

	public GameObject TryReplaceTile(GameObject src_go, Vector3 pos, Orientation orientation, IList<Tag> selected_elements, int layer = 0)
	{
		GameObject gameObject = null;
		string text;
		if (this.IsValidPlaceLocation(src_go, pos, orientation, true, out text))
		{
			Constructable component = this.BuildingUnderConstruction.GetComponent<Constructable>();
			component.IsReplacementTile = true;
			gameObject = this.Instantiate(pos, orientation, selected_elements, layer);
			component.IsReplacementTile = false;
		}
		return gameObject;
	}

	public GameObject Instantiate(Vector3 pos, Orientation orientation, IList<Tag> selected_elements, int layer = 0)
	{
		float num = -0.15f;
		pos.z += num;
		GameObject gameObject = GameUtil.KInstantiate(this.BuildingUnderConstruction, pos, Grid.SceneLayer.Front, null, layer);
		Element element = ElementLoader.GetElement(selected_elements[0]);
		global::Debug.Assert(element != null, "Missing primary element for BuildingDef");
		gameObject.GetComponent<PrimaryElement>().ElementID = element.id;
		gameObject.GetComponent<Constructable>().SelectedElementsTags = selected_elements;
		gameObject.SetActive(true);
		return gameObject;
	}

	private bool IsAreaClear(GameObject source_go, int cell, Orientation orientation, ObjectLayer layer, ObjectLayer tile_layer, bool replace_tile, out string fail_reason)
	{
		bool flag = true;
		fail_reason = null;
		int i = 0;
		while (i < this.PlacementOffsets.Length)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.PlacementOffsets[i], orientation);
			if (!Grid.IsCellOffsetValid(cell, rotatedCellOffset))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				flag = false;
				break;
			}
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			if (!Grid.IsValidBuildingCell(num))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				flag = false;
				break;
			}
			if (Grid.Element[num].id == SimHashes.Unobtanium)
			{
				fail_reason = null;
				flag = false;
				break;
			}
			bool flag2 = this.BuildLocationRule == BuildLocationRule.LogicBridge || this.BuildLocationRule == BuildLocationRule.Conduit || this.BuildLocationRule == BuildLocationRule.WireBridge;
			if (!replace_tile && !flag2)
			{
				GameObject gameObject = Grid.Objects[num, (int)layer];
				if (gameObject != null)
				{
					if (gameObject.GetComponent<Wire>() == null || this.BuildingComplete.GetComponent<Wire>() == null)
					{
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
						flag = false;
						break;
					}
					break;
				}
				else if (tile_layer != ObjectLayer.NumLayers && Grid.Objects[num, (int)tile_layer] != null && Grid.Objects[num, (int)tile_layer].GetComponent<BuildingPreview>() == null)
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
					flag = false;
					break;
				}
			}
			if (layer == ObjectLayer.Building && this.AttachmentSlotTag != GameTags.Rocket && Grid.Objects[num, 39] != null)
			{
				if (this.BuildingComplete.GetComponent<Wire>() == null)
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
					flag = false;
					break;
				}
				break;
			}
			else
			{
				if (layer == ObjectLayer.Gantry)
				{
					bool flag3 = false;
					for (int j = 0; j < Gantry.TileOffsets.Length; j++)
					{
						CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(Gantry.TileOffsets[j], orientation);
						flag3 |= rotatedCellOffset2 == rotatedCellOffset;
					}
					if (flag3 && !this.IsValidTileLocation(source_go, num, ref fail_reason))
					{
						flag = false;
						break;
					}
					GameObject gameObject2 = Grid.Objects[num, 1];
					if (gameObject2 != null && gameObject2.GetComponent<BuildingPreview>() == null)
					{
						Building component = gameObject2.GetComponent<Building>();
						if (flag3 || component == null || component.Def.AttachmentSlotTag != GameTags.Rocket)
						{
							fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
							flag = false;
							break;
						}
					}
				}
				if (this.BuildLocationRule == BuildLocationRule.Tile)
				{
					if (!this.IsValidTileLocation(source_go, num, ref fail_reason))
					{
						flag = false;
						break;
					}
				}
				else if (this.BuildLocationRule == BuildLocationRule.OnFloorOverSpace && global::World.Instance.zoneRenderData.GetSubWorldZoneType(num) != SubWorld.ZoneType.Space)
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_SPACE;
					flag = false;
					break;
				}
				i++;
			}
		}
		if (!flag)
		{
			return false;
		}
		switch (this.BuildLocationRule)
		{
		case BuildLocationRule.NotInTiles:
		{
			GameObject gameObject3 = Grid.Objects[cell, 9];
			if (gameObject3 != null && gameObject3 != source_go)
			{
				flag = false;
			}
			else if (Grid.HasDoor[cell])
			{
				flag = false;
			}
			else
			{
				GameObject gameObject4 = Grid.Objects[cell, (int)this.ObjectLayer];
				if (gameObject4 != null)
				{
					if (this.ReplacementLayer == ObjectLayer.NumLayers)
					{
						if (gameObject4 != source_go)
						{
							flag = false;
						}
					}
					else
					{
						Building component2 = gameObject4.GetComponent<Building>();
						if (component2 != null && component2.Def.ReplacementLayer != this.ReplacementLayer)
						{
							flag = false;
						}
					}
				}
			}
			if (!flag)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_NOT_IN_TILES;
			}
			break;
		}
		case BuildLocationRule.WireBridge:
			return this.IsValidWireBridgeLocation(source_go, cell, orientation, out fail_reason);
		case BuildLocationRule.HighWattBridgeTile:
			flag = this.IsValidTileLocation(source_go, cell, ref fail_reason) && this.IsValidHighWattBridgeLocation(source_go, cell, orientation, out fail_reason);
			break;
		case BuildLocationRule.BuildingAttachPoint:
		{
			flag = false;
			int num2 = 0;
			while (num2 < Components.BuildingAttachPoints.Count && !flag)
			{
				for (int k = 0; k < Components.BuildingAttachPoints[num2].points.Length; k++)
				{
					if (Components.BuildingAttachPoints[num2].AcceptsAttachment(this.AttachmentSlotTag, Grid.OffsetCell(cell, this.attachablePosition)))
					{
						flag = true;
						break;
					}
				}
				num2++;
			}
			if (!flag)
			{
				fail_reason = string.Format(UI.TOOLTIPS.HELP_BUILDLOCATION_ATTACHPOINT, this.AttachmentSlotTag);
			}
			break;
		}
		}
		flag = flag && this.ArePowerPortsInValidPositions(source_go, cell, orientation, out fail_reason);
		flag = flag && this.AreConduitPortsInValidPositions(source_go, cell, orientation, out fail_reason);
		return flag && this.AreLogicPortsInValidPositions(source_go, cell, out fail_reason);
	}

	private bool IsValidTileLocation(GameObject source_go, int cell, ref string fail_reason)
	{
		GameObject gameObject = Grid.Objects[cell, 27];
		if (gameObject != null && gameObject != source_go && gameObject.GetComponent<Building>().Def.BuildLocationRule == BuildLocationRule.NotInTiles)
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRE_OBSTRUCTION;
			return false;
		}
		gameObject = Grid.Objects[cell, 29];
		if (gameObject != null && gameObject != source_go && gameObject.GetComponent<Building>().Def.BuildLocationRule == BuildLocationRule.HighWattBridgeTile)
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRE_OBSTRUCTION;
			return false;
		}
		gameObject = Grid.Objects[cell, 2];
		if (gameObject != null && gameObject != source_go)
		{
			Building component = gameObject.GetComponent<Building>();
			if (component != null && component.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_BACK_WALL;
				return false;
			}
		}
		return true;
	}

	public void RunOnArea(int cell, Orientation orientation, Action<int> callback)
	{
		for (int i = 0; i < this.PlacementOffsets.Length; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.PlacementOffsets[i], orientation);
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			callback(num);
		}
	}

	public void MarkArea(int cell, Orientation orientation, ObjectLayer layer, GameObject go)
	{
		if (this.BuildLocationRule != BuildLocationRule.Conduit && this.BuildLocationRule != BuildLocationRule.WireBridge && this.BuildLocationRule != BuildLocationRule.LogicBridge)
		{
			for (int i = 0; i < this.PlacementOffsets.Length; i++)
			{
				CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.PlacementOffsets[i], orientation);
				int num = Grid.OffsetCell(cell, rotatedCellOffset);
				Grid.Objects[num, (int)layer] = go;
			}
		}
		if (this.InputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(this.UtilityInputOffset, orientation);
			int num2 = Grid.OffsetCell(cell, rotatedCellOffset2);
			ObjectLayer objectLayerForConduitType = Grid.GetObjectLayerForConduitType(this.InputConduitType);
			this.MarkOverlappingPorts(Grid.Objects[num2, (int)objectLayerForConduitType], go);
			Grid.Objects[num2, (int)objectLayerForConduitType] = go;
		}
		if (this.OutputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(this.UtilityOutputOffset, orientation);
			int num3 = Grid.OffsetCell(cell, rotatedCellOffset3);
			ObjectLayer objectLayerForConduitType2 = Grid.GetObjectLayerForConduitType(this.OutputConduitType);
			this.MarkOverlappingPorts(Grid.Objects[num3, (int)objectLayerForConduitType2], go);
			Grid.Objects[num3, (int)objectLayerForConduitType2] = go;
		}
		if (this.RequiresPowerInput)
		{
			CellOffset rotatedCellOffset4 = Rotatable.GetRotatedCellOffset(this.PowerInputOffset, orientation);
			int num4 = Grid.OffsetCell(cell, rotatedCellOffset4);
			this.MarkOverlappingPorts(Grid.Objects[num4, 29], go);
			Grid.Objects[num4, 29] = go;
		}
		if (this.RequiresPowerOutput || this.GeneratorWattageRating > 0f)
		{
			CellOffset rotatedCellOffset5 = Rotatable.GetRotatedCellOffset(this.PowerOutputOffset, orientation);
			int num5 = Grid.OffsetCell(cell, rotatedCellOffset5);
			this.MarkOverlappingPorts(Grid.Objects[num5, 29], go);
			Grid.Objects[num5, 29] = go;
		}
		if (this.BuildLocationRule == BuildLocationRule.WireBridge || this.BuildLocationRule == BuildLocationRule.HighWattBridgeTile)
		{
			int num6;
			int num7;
			go.GetComponent<UtilityNetworkLink>().GetCells(cell, orientation, out num6, out num7);
			this.MarkOverlappingPorts(Grid.Objects[num6, 29], go);
			this.MarkOverlappingPorts(Grid.Objects[num7, 29], go);
			Grid.Objects[num6, 29] = go;
			Grid.Objects[num7, 29] = go;
		}
		if (this.BuildLocationRule == BuildLocationRule.LogicBridge)
		{
			LogicPorts component = go.GetComponent<LogicPorts>();
			if (component != null && component.inputPortInfo != null)
			{
				LogicPorts.Port[] inputPortInfo = component.inputPortInfo;
				for (int j = 0; j < inputPortInfo.Length; j++)
				{
					CellOffset rotatedCellOffset6 = Rotatable.GetRotatedCellOffset(inputPortInfo[j].cellOffset, orientation);
					int num8 = Grid.OffsetCell(cell, rotatedCellOffset6);
					this.MarkOverlappingPorts(Grid.Objects[num8, (int)layer], go);
					Grid.Objects[num8, (int)layer] = go;
				}
			}
		}
		ISecondaryInput component2 = this.BuildingComplete.GetComponent<ISecondaryInput>();
		if (component2 != null)
		{
			ObjectLayer objectLayerForConduitType3 = Grid.GetObjectLayerForConduitType(component2.GetSecondaryConduitType());
			CellOffset rotatedCellOffset7 = Rotatable.GetRotatedCellOffset(component2.GetSecondaryConduitOffset(), orientation);
			int num9 = Grid.OffsetCell(cell, rotatedCellOffset7);
			this.MarkOverlappingPorts(Grid.Objects[num9, (int)objectLayerForConduitType3], go);
			Grid.Objects[num9, (int)objectLayerForConduitType3] = go;
		}
		ISecondaryOutput component3 = this.BuildingComplete.GetComponent<ISecondaryOutput>();
		if (component3 != null)
		{
			ObjectLayer objectLayerForConduitType4 = Grid.GetObjectLayerForConduitType(component3.GetSecondaryConduitType());
			CellOffset rotatedCellOffset8 = Rotatable.GetRotatedCellOffset(component3.GetSecondaryConduitOffset(), orientation);
			int num10 = Grid.OffsetCell(cell, rotatedCellOffset8);
			this.MarkOverlappingPorts(Grid.Objects[num10, (int)objectLayerForConduitType4], go);
			Grid.Objects[num10, (int)objectLayerForConduitType4] = go;
		}
	}

	public void MarkOverlappingPorts(GameObject existing, GameObject replaced)
	{
		if (existing == null)
		{
			return;
		}
		if (existing != replaced)
		{
			existing.AddTag(GameTags.HasInvalidPorts);
		}
	}

	public void UnmarkArea(int cell, Orientation orientation, ObjectLayer layer, GameObject go)
	{
		for (int i = 0; i < this.PlacementOffsets.Length; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.PlacementOffsets[i], orientation);
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			if (Grid.Objects[num, (int)layer] == go)
			{
				Grid.Objects[num, (int)layer] = null;
			}
		}
		if (this.InputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(this.UtilityInputOffset, orientation);
			int num2 = Grid.OffsetCell(cell, rotatedCellOffset2);
			ObjectLayer objectLayerForConduitType = Grid.GetObjectLayerForConduitType(this.InputConduitType);
			if (Grid.Objects[num2, (int)objectLayerForConduitType] == go)
			{
				Grid.Objects[num2, (int)objectLayerForConduitType] = null;
			}
		}
		if (this.OutputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(this.UtilityOutputOffset, orientation);
			int num3 = Grid.OffsetCell(cell, rotatedCellOffset3);
			ObjectLayer objectLayerForConduitType2 = Grid.GetObjectLayerForConduitType(this.OutputConduitType);
			if (Grid.Objects[num3, (int)objectLayerForConduitType2] == go)
			{
				Grid.Objects[num3, (int)objectLayerForConduitType2] = null;
			}
		}
		if (this.RequiresPowerInput)
		{
			CellOffset rotatedCellOffset4 = Rotatable.GetRotatedCellOffset(this.PowerInputOffset, orientation);
			int num4 = Grid.OffsetCell(cell, rotatedCellOffset4);
			if (Grid.Objects[num4, 29] == go)
			{
				Grid.Objects[num4, 29] = null;
			}
		}
		if (this.RequiresPowerOutput || this.GeneratorWattageRating > 0f)
		{
			CellOffset rotatedCellOffset5 = Rotatable.GetRotatedCellOffset(this.PowerOutputOffset, orientation);
			int num5 = Grid.OffsetCell(cell, rotatedCellOffset5);
			if (Grid.Objects[num5, 29] == go)
			{
				Grid.Objects[num5, 29] = null;
			}
		}
		if (this.BuildLocationRule == BuildLocationRule.HighWattBridgeTile)
		{
			int num6;
			int num7;
			go.GetComponent<UtilityNetworkLink>().GetCells(cell, orientation, out num6, out num7);
			if (Grid.Objects[num6, 29] == go)
			{
				Grid.Objects[num6, 29] = null;
			}
			if (Grid.Objects[num7, 29] == go)
			{
				Grid.Objects[num7, 29] = null;
			}
		}
		ISecondaryInput component = this.BuildingComplete.GetComponent<ISecondaryInput>();
		if (component != null)
		{
			ObjectLayer objectLayerForConduitType3 = Grid.GetObjectLayerForConduitType(component.GetSecondaryConduitType());
			CellOffset rotatedCellOffset6 = Rotatable.GetRotatedCellOffset(component.GetSecondaryConduitOffset(), orientation);
			int num8 = Grid.OffsetCell(cell, rotatedCellOffset6);
			if (Grid.Objects[num8, (int)objectLayerForConduitType3] == go)
			{
				Grid.Objects[num8, (int)objectLayerForConduitType3] = null;
			}
		}
		ISecondaryOutput component2 = this.BuildingComplete.GetComponent<ISecondaryOutput>();
		if (component2 != null)
		{
			ObjectLayer objectLayerForConduitType4 = Grid.GetObjectLayerForConduitType(component2.GetSecondaryConduitType());
			CellOffset rotatedCellOffset7 = Rotatable.GetRotatedCellOffset(component2.GetSecondaryConduitOffset(), orientation);
			int num9 = Grid.OffsetCell(cell, rotatedCellOffset7);
			if (Grid.Objects[num9, (int)objectLayerForConduitType4] == go)
			{
				Grid.Objects[num9, (int)objectLayerForConduitType4] = null;
			}
		}
	}

	public int GetBuildingCell(int cell)
	{
		return cell + (this.WidthInCells - 1) / 2;
	}

	public Vector3 GetVisualizerOffset()
	{
		return Vector3.right * (0.5f * (float)((this.WidthInCells + 1) % 2));
	}

	public bool IsValidPlaceLocation(GameObject go, Vector3 pos, Orientation orientation, out string fail_reason)
	{
		int num = Grid.PosToCell(pos);
		return this.IsValidPlaceLocation(go, num, orientation, false, out fail_reason);
	}

	public bool IsValidPlaceLocation(GameObject go, Vector3 pos, Orientation orientation, bool replace_tile, out string fail_reason)
	{
		int num = Grid.PosToCell(pos);
		return this.IsValidPlaceLocation(go, num, orientation, replace_tile, out fail_reason);
	}

	public bool IsValidPlaceLocation(GameObject go, int cell, Orientation orientation, out string fail_reason)
	{
		if (!Grid.IsValidBuildingCell(cell))
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
			return false;
		}
		if (this.BuildLocationRule == BuildLocationRule.OnWall)
		{
			if (!BuildingDef.CheckFoundation(cell, orientation, this.BuildLocationRule, this.WidthInCells, this.HeightInCells))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WALL;
				return false;
			}
		}
		else if (this.BuildLocationRule == BuildLocationRule.InCorner && !BuildingDef.CheckFoundation(cell, orientation, this.BuildLocationRule, this.WidthInCells, this.HeightInCells))
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER;
			return false;
		}
		return this.IsAreaClear(go, cell, orientation, this.ObjectLayer, this.TileLayer, false, out fail_reason);
	}

	public bool IsValidPlaceLocation(GameObject go, int cell, Orientation orientation, bool replace_tile, out string fail_reason)
	{
		if (!Grid.IsValidBuildingCell(cell))
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
			return false;
		}
		if (this.BuildLocationRule == BuildLocationRule.OnWall)
		{
			if (!BuildingDef.CheckFoundation(cell, orientation, this.BuildLocationRule, this.WidthInCells, this.HeightInCells))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WALL;
				return false;
			}
		}
		else if (this.BuildLocationRule == BuildLocationRule.InCorner && !BuildingDef.CheckFoundation(cell, orientation, this.BuildLocationRule, this.WidthInCells, this.HeightInCells))
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER;
			return false;
		}
		return this.IsAreaClear(go, cell, orientation, this.ObjectLayer, this.TileLayer, replace_tile, out fail_reason);
	}

	public bool IsValidReplaceLocation(Vector3 pos, Orientation orientation, ObjectLayer replace_layer, ObjectLayer obj_layer)
	{
		if (replace_layer == ObjectLayer.NumLayers)
		{
			return false;
		}
		bool flag = true;
		int num = Grid.PosToCell(pos);
		for (int i = 0; i < this.PlacementOffsets.Length; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.PlacementOffsets[i], orientation);
			int num2 = Grid.OffsetCell(num, rotatedCellOffset);
			if (!Grid.IsValidBuildingCell(num2))
			{
				return false;
			}
			if (Grid.Objects[num2, (int)obj_layer] == null || Grid.Objects[num2, (int)replace_layer] != null)
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	public bool IsValidBuildLocation(GameObject source_go, Vector3 pos, Orientation orientation)
	{
		string text = "";
		return this.IsValidBuildLocation(source_go, pos, orientation, out text);
	}

	public bool IsValidBuildLocation(GameObject source_go, Vector3 pos, Orientation orientation, out string reason)
	{
		int num = Grid.PosToCell(pos);
		return this.IsValidBuildLocation(source_go, num, orientation, out reason);
	}

	public bool IsValidBuildLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		if (!Grid.IsValidBuildingCell(cell))
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
			return false;
		}
		if (!this.IsAreaValid(cell, orientation, out fail_reason))
		{
			return false;
		}
		bool flag = true;
		fail_reason = null;
		switch (this.BuildLocationRule)
		{
		case BuildLocationRule.Anywhere:
		case BuildLocationRule.Conduit:
			flag = true;
			break;
		case BuildLocationRule.OnFloor:
		case BuildLocationRule.OnCeiling:
		case BuildLocationRule.OnFoundationRotatable:
			if (!BuildingDef.CheckFoundation(cell, orientation, this.BuildLocationRule, this.WidthInCells, this.HeightInCells))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_FLOOR;
			}
			break;
		case BuildLocationRule.OnFloorOverSpace:
			if (!BuildingDef.CheckFoundation(cell, orientation, this.BuildLocationRule, this.WidthInCells, this.HeightInCells))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_FLOOR;
			}
			else if (!BuildingDef.AreAllCellsValid(cell, orientation, this.WidthInCells, this.HeightInCells, (int check_cell) => global::World.Instance.zoneRenderData.GetSubWorldZoneType(check_cell) == SubWorld.ZoneType.Space))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_SPACE;
			}
			break;
		case BuildLocationRule.OnWall:
			if (!BuildingDef.CheckFoundation(cell, orientation, this.BuildLocationRule, this.WidthInCells, this.HeightInCells))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WALL;
			}
			break;
		case BuildLocationRule.InCorner:
			if (!BuildingDef.CheckFoundation(cell, orientation, this.BuildLocationRule, this.WidthInCells, this.HeightInCells))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER;
			}
			break;
		case BuildLocationRule.Tile:
		{
			flag = true;
			GameObject gameObject = Grid.Objects[cell, 27];
			if (gameObject != null)
			{
				Building component = gameObject.GetComponent<Building>();
				if (component != null && component.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
				{
					flag = false;
				}
			}
			gameObject = Grid.Objects[cell, 2];
			if (gameObject != null)
			{
				Building component2 = gameObject.GetComponent<Building>();
				if (component2 != null && component2.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
				{
					flag = false;
				}
			}
			break;
		}
		case BuildLocationRule.NotInTiles:
		{
			GameObject gameObject2 = Grid.Objects[cell, 9];
			flag = gameObject2 == null || gameObject2 == source_go;
			flag = flag && !Grid.HasDoor[cell];
			if (flag)
			{
				GameObject gameObject3 = Grid.Objects[cell, (int)this.ObjectLayer];
				if (gameObject3 != null)
				{
					if (this.ReplacementLayer == ObjectLayer.NumLayers)
					{
						flag = flag && (gameObject3 == null || gameObject3 == source_go);
					}
					else
					{
						Building component3 = gameObject3.GetComponent<Building>();
						flag = component3 == null || component3.Def.ReplacementLayer == this.ReplacementLayer;
					}
				}
			}
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_NOT_IN_TILES;
			break;
		}
		case BuildLocationRule.BuildingAttachPoint:
		{
			flag = false;
			int num = 0;
			while (num < Components.BuildingAttachPoints.Count && !flag)
			{
				for (int i = 0; i < Components.BuildingAttachPoints[num].points.Length; i++)
				{
					if (Components.BuildingAttachPoints[num].AcceptsAttachment(this.AttachmentSlotTag, Grid.OffsetCell(cell, this.attachablePosition)))
					{
						flag = true;
						break;
					}
				}
				num++;
			}
			fail_reason = string.Format(UI.TOOLTIPS.HELP_BUILDLOCATION_ATTACHPOINT, this.AttachmentSlotTag);
			break;
		}
		case BuildLocationRule.OnFloorOrBuildingAttachPoint:
			if (!BuildingDef.CheckFoundation(cell, orientation, BuildLocationRule.OnFloor, this.WidthInCells, this.HeightInCells))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_FLOOR_OR_ATTACHPOINT;
				if (!flag)
				{
					int num2 = 0;
					while (num2 < Components.BuildingAttachPoints.Count && !flag)
					{
						for (int j = 0; j < Components.BuildingAttachPoints[num2].points.Length; j++)
						{
							if (Components.BuildingAttachPoints[num2].AcceptsAttachment(this.AttachmentSlotTag, Grid.OffsetCell(cell, this.attachablePosition)))
							{
								flag = true;
								break;
							}
						}
						num2++;
					}
					fail_reason = string.Format(UI.TOOLTIPS.HELP_BUILDLOCATION_FLOOR_OR_ATTACHPOINT, this.AttachmentSlotTag);
				}
			}
			else
			{
				flag = true;
			}
			break;
		}
		flag = flag && this.ArePowerPortsInValidPositions(source_go, cell, orientation, out fail_reason);
		return flag && this.AreConduitPortsInValidPositions(source_go, cell, orientation, out fail_reason);
	}

	private bool IsAreaValid(int cell, Orientation orientation, out string fail_reason)
	{
		bool flag = true;
		fail_reason = null;
		for (int i = 0; i < this.PlacementOffsets.Length; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.PlacementOffsets[i], orientation);
			if (!Grid.IsCellOffsetValid(cell, rotatedCellOffset))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				flag = false;
				break;
			}
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			if (!Grid.IsValidBuildingCell(num))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				flag = false;
				break;
			}
			if (Grid.Element[num].id == SimHashes.Unobtanium)
			{
				fail_reason = null;
				flag = false;
				break;
			}
		}
		return flag;
	}

	private bool ArePowerPortsInValidPositions(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		fail_reason = null;
		if (source_go == null)
		{
			return true;
		}
		if (this.RequiresPowerInput)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.PowerInputOffset, orientation);
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			GameObject gameObject = Grid.Objects[num, 29];
			if (gameObject != null && gameObject != source_go)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP;
				return false;
			}
		}
		if (this.RequiresPowerOutput || this.GeneratorWattageRating > 0f)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(this.PowerOutputOffset, orientation);
			int num2 = Grid.OffsetCell(cell, rotatedCellOffset2);
			GameObject gameObject2 = Grid.Objects[num2, 29];
			if (gameObject2 != null && gameObject2 != source_go)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP;
				return false;
			}
		}
		return true;
	}

	private bool AreConduitPortsInValidPositions(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		fail_reason = null;
		if (source_go == null)
		{
			return true;
		}
		bool flag = true;
		if (this.InputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.UtilityInputOffset, orientation);
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			flag = this.IsValidConduitConnection(source_go, this.InputConduitType, num, ref fail_reason);
		}
		if (flag && this.OutputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(this.UtilityOutputOffset, orientation);
			int num2 = Grid.OffsetCell(cell, rotatedCellOffset2);
			flag = this.IsValidConduitConnection(source_go, this.OutputConduitType, num2, ref fail_reason);
		}
		Building component = source_go.GetComponent<Building>();
		if (flag && component)
		{
			ISecondaryInput component2 = component.Def.BuildingComplete.GetComponent<ISecondaryInput>();
			if (component2 != null)
			{
				ConduitType secondaryConduitType = component2.GetSecondaryConduitType();
				CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(component2.GetSecondaryConduitOffset(), orientation);
				int num3 = Grid.OffsetCell(cell, rotatedCellOffset3);
				flag = this.IsValidConduitConnection(source_go, secondaryConduitType, num3, ref fail_reason);
			}
		}
		if (flag)
		{
			ISecondaryOutput component3 = component.Def.BuildingComplete.GetComponent<ISecondaryOutput>();
			if (component3 != null)
			{
				ConduitType secondaryConduitType2 = component3.GetSecondaryConduitType();
				CellOffset rotatedCellOffset4 = Rotatable.GetRotatedCellOffset(component3.GetSecondaryConduitOffset(), orientation);
				int num4 = Grid.OffsetCell(cell, rotatedCellOffset4);
				flag = this.IsValidConduitConnection(source_go, secondaryConduitType2, num4, ref fail_reason);
			}
		}
		return flag;
	}

	private bool IsValidWireBridgeLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		UtilityNetworkLink component = source_go.GetComponent<UtilityNetworkLink>();
		if (component != null)
		{
			int num;
			int num2;
			component.GetCells(out num, out num2);
			if (Grid.Objects[num, 29] != null || Grid.Objects[num2, 29] != null)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP;
				return false;
			}
		}
		fail_reason = null;
		return true;
	}

	private bool IsValidHighWattBridgeLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		UtilityNetworkLink component = source_go.GetComponent<UtilityNetworkLink>();
		if (component != null)
		{
			if (!component.AreCellsValid(cell, orientation))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				return false;
			}
			int num;
			int num2;
			component.GetCells(out num, out num2);
			if (Grid.Objects[num, 29] != null || Grid.Objects[num2, 29] != null)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP;
				return false;
			}
			if (Grid.Objects[num, 9] != null || Grid.Objects[num2, 9] != null)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_HIGHWATT_NOT_IN_TILE;
				return false;
			}
			if (Grid.HasDoor[num] || Grid.HasDoor[num2])
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_HIGHWATT_NOT_IN_TILE;
				return false;
			}
			GameObject gameObject = Grid.Objects[num, 1];
			GameObject gameObject2 = Grid.Objects[num2, 1];
			if (gameObject != null || gameObject2 != null)
			{
				BuildingUnderConstruction buildingUnderConstruction = (gameObject ? gameObject.GetComponent<BuildingUnderConstruction>() : null);
				BuildingUnderConstruction buildingUnderConstruction2 = (gameObject2 ? gameObject2.GetComponent<BuildingUnderConstruction>() : null);
				if ((buildingUnderConstruction && buildingUnderConstruction.Def.BuildingComplete.GetComponent<Door>()) || (buildingUnderConstruction2 && buildingUnderConstruction2.Def.BuildingComplete.GetComponent<Door>()))
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_HIGHWATT_NOT_IN_TILE;
					return false;
				}
			}
		}
		fail_reason = null;
		return true;
	}

	private bool AreLogicPortsInValidPositions(GameObject source_go, int cell, out string fail_reason)
	{
		fail_reason = null;
		if (source_go == null)
		{
			return true;
		}
		ReadOnlyCollection<ILogicUIElement> visElements = Game.Instance.logicCircuitManager.GetVisElements();
		LogicPorts component = source_go.GetComponent<LogicPorts>();
		if (component != null)
		{
			component.HackRefreshVisualizers();
			if (this.DoLogicPortsConflict(component.inputPorts, visElements) || this.DoLogicPortsConflict(component.outputPorts, visElements))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_LOGIC_PORTS_OBSTRUCTED;
				return false;
			}
		}
		else
		{
			LogicGateBase component2 = source_go.GetComponent<LogicGateBase>();
			if (component2 != null && (this.IsLogicPortObstructed(component2.InputCellOne, visElements) || this.IsLogicPortObstructed(component2.OutputCellOne, visElements) || ((component2.RequiresTwoInputs || component2.RequiresFourInputs) && this.IsLogicPortObstructed(component2.InputCellTwo, visElements)) || (component2.RequiresFourInputs && (this.IsLogicPortObstructed(component2.InputCellThree, visElements) || this.IsLogicPortObstructed(component2.InputCellFour, visElements))) || (component2.RequiresFourOutputs && (this.IsLogicPortObstructed(component2.OutputCellTwo, visElements) || this.IsLogicPortObstructed(component2.OutputCellThree, visElements) || this.IsLogicPortObstructed(component2.OutputCellFour, visElements)))))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_LOGIC_PORTS_OBSTRUCTED;
				return false;
			}
		}
		return true;
	}

	private bool DoLogicPortsConflict(IList<ILogicUIElement> ports_a, IList<ILogicUIElement> ports_b)
	{
		if (ports_a == null || ports_b == null)
		{
			return false;
		}
		foreach (ILogicUIElement logicUIElement in ports_a)
		{
			int logicUICell = logicUIElement.GetLogicUICell();
			foreach (ILogicUIElement logicUIElement2 in ports_b)
			{
				if (logicUIElement != logicUIElement2 && logicUICell == logicUIElement2.GetLogicUICell())
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsLogicPortObstructed(int cell, IList<ILogicUIElement> ports)
	{
		int num = 0;
		using (IEnumerator<ILogicUIElement> enumerator = ports.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.GetLogicUICell() == cell)
				{
					num++;
				}
			}
		}
		return num > 0;
	}

	private bool IsValidConduitConnection(GameObject source_go, ConduitType conduit_type, int utility_cell, ref string fail_reason)
	{
		bool flag = true;
		switch (conduit_type)
		{
		case ConduitType.Gas:
		{
			GameObject gameObject = Grid.Objects[utility_cell, 15];
			if (gameObject != null && gameObject != source_go)
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_GASPORTS_OVERLAP;
			}
			break;
		}
		case ConduitType.Liquid:
		{
			GameObject gameObject2 = Grid.Objects[utility_cell, 19];
			if (gameObject2 != null && gameObject2 != source_go)
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_LIQUIDPORTS_OVERLAP;
			}
			break;
		}
		case ConduitType.Solid:
		{
			GameObject gameObject3 = Grid.Objects[utility_cell, 23];
			if (gameObject3 != null && gameObject3 != source_go)
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_SOLIDPORTS_OVERLAP;
			}
			break;
		}
		}
		return flag;
	}

	public static int GetXOffset(int width)
	{
		return -(width - 1) / 2;
	}

	public static bool CheckFoundation(int cell, Orientation orientation, BuildLocationRule location_rule, int width, int height)
	{
		if (location_rule == BuildLocationRule.OnWall)
		{
			return BuildingDef.CheckWallFoundation(cell, width, height, orientation != Orientation.FlipH);
		}
		if (location_rule == BuildLocationRule.InCorner)
		{
			return BuildingDef.CheckBaseFoundation(cell, orientation, BuildLocationRule.OnCeiling, width, height) && BuildingDef.CheckWallFoundation(cell, width, height, orientation != Orientation.FlipH);
		}
		return BuildingDef.CheckBaseFoundation(cell, orientation, location_rule, width, height);
	}

	public static bool CheckBaseFoundation(int cell, Orientation orientation, BuildLocationRule location_rule, int width, int height)
	{
		int num = -(width - 1) / 2;
		int num2 = width / 2;
		for (int i = num; i <= num2; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset((location_rule == BuildLocationRule.OnCeiling) ? new CellOffset(i, height) : new CellOffset(i, -1), orientation);
			int num3 = Grid.OffsetCell(cell, rotatedCellOffset);
			if (!Grid.IsValidBuildingCell(num3) || !Grid.Solid[num3])
			{
				return false;
			}
		}
		return true;
	}

	public static bool CheckWallFoundation(int cell, int width, int height, bool leftWall)
	{
		for (int i = 0; i <= height; i++)
		{
			CellOffset cellOffset = new CellOffset(leftWall ? (-(width - 1) / 2 - 1) : (width / 2 + 1), i);
			int num = Grid.OffsetCell(cell, cellOffset);
			if (!Grid.IsValidBuildingCell(num) || !Grid.Solid[num])
			{
				return false;
			}
		}
		return true;
	}

	public static bool AreAllCellsValid(int base_cell, Orientation orientation, int width, int height, Func<int, bool> valid_cell_check)
	{
		int num = -(width - 1) / 2;
		int num2 = width / 2;
		if (orientation == Orientation.FlipH)
		{
			int num3 = num;
			num = -num2;
			num2 = -num3;
		}
		for (int i = 0; i < height; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				int num4 = Grid.OffsetCell(base_cell, j, i);
				if (!valid_cell_check(num4))
				{
					return false;
				}
			}
		}
		return true;
	}

	public Sprite GetUISprite(string animName = "ui", bool centered = false)
	{
		return Def.GetUISpriteFromMultiObjectAnim(this.AnimFiles[0], animName, centered, "");
	}

	public void GenerateOffsets()
	{
		this.GenerateOffsets(this.WidthInCells, this.HeightInCells);
	}

	public void GenerateOffsets(int width, int height)
	{
		if (!BuildingDef.placementOffsetsCache.TryGetValue(new CellOffset(width, height), out this.PlacementOffsets))
		{
			int num = width / 2 - width + 1;
			this.PlacementOffsets = new CellOffset[width * height];
			for (int num2 = 0; num2 != height; num2++)
			{
				int num3 = num2 * width;
				for (int num4 = 0; num4 != width; num4++)
				{
					int num5 = num3 + num4;
					this.PlacementOffsets[num5].x = num4 + num;
					this.PlacementOffsets[num5].y = num2;
				}
			}
			BuildingDef.placementOffsetsCache.Add(new CellOffset(width, height), this.PlacementOffsets);
		}
	}

	public void PostProcess()
	{
		this.CraftRecipe = new Recipe(this.BuildingComplete.PrefabID().Name, 1f, (SimHashes)0, this.Name, null, 0);
		this.CraftRecipe.Icon = this.UISprite;
		for (int i = 0; i < this.MaterialCategory.Length; i++)
		{
			Recipe.Ingredient ingredient = new Recipe.Ingredient(this.MaterialCategory[i], (float)((int)this.Mass[i]));
			this.CraftRecipe.Ingredients.Add(ingredient);
		}
		if (this.DecorBlockTileInfo != null)
		{
			this.DecorBlockTileInfo.PostProcess();
		}
		if (this.DecorPlaceBlockTileInfo != null)
		{
			this.DecorPlaceBlockTileInfo.PostProcess();
		}
		if (!this.Deprecated)
		{
			Db.Get().TechItems.AddTechItem(this.PrefabID, this.Name, this.Effect, new Func<string, bool, Sprite>(this.GetUISprite));
		}
	}

	public bool MaterialsAvailable(IList<Tag> selected_elements)
	{
		bool flag = true;
		foreach (Recipe.Ingredient ingredient in this.CraftRecipe.GetAllIngredients(selected_elements))
		{
			if (WorldInventory.Instance.GetAmount(ingredient.tag) < ingredient.amount)
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	public bool CheckRequiresBuildingCellVisualizer()
	{
		return this.CheckRequiresPowerInput() || this.CheckRequiresPowerOutput() || this.CheckRequiresGasInput() || this.CheckRequiresGasOutput() || this.CheckRequiresLiquidInput() || this.CheckRequiresLiquidOutput() || this.CheckRequiresSolidInput() || this.CheckRequiresSolidOutput() || this.DiseaseCellVisName != null;
	}

	public bool CheckRequiresPowerInput()
	{
		return this.RequiresPowerInput;
	}

	public bool CheckRequiresPowerOutput()
	{
		return this.GeneratorWattageRating > 0f || this.RequiresPowerOutput;
	}

	public bool CheckRequiresGasInput()
	{
		return this.InputConduitType == ConduitType.Gas;
	}

	public bool CheckRequiresGasOutput()
	{
		return this.OutputConduitType == ConduitType.Gas;
	}

	public bool CheckRequiresLiquidInput()
	{
		return this.InputConduitType == ConduitType.Liquid;
	}

	public bool CheckRequiresLiquidOutput()
	{
		return this.OutputConduitType == ConduitType.Liquid;
	}

	public bool CheckRequiresSolidInput()
	{
		return this.InputConduitType == ConduitType.Solid;
	}

	public bool CheckRequiresSolidOutput()
	{
		return this.OutputConduitType == ConduitType.Solid;
	}

	public string DlcId;

	public float EnergyConsumptionWhenActive;

	public float GeneratorWattageRating;

	public float GeneratorBaseCapacity;

	public float MassForTemperatureModification;

	public float ExhaustKilowattsWhenActive;

	public float SelfHeatKilowattsWhenActive;

	public float BaseMeltingPoint;

	public float ConstructionTime;

	public float WorkTime;

	public float ThermalConductivity = 1f;

	public int WidthInCells;

	public int HeightInCells;

	public int HitPoints;

	public bool RequiresPowerInput;

	public bool RequiresPowerOutput;

	public bool UseWhitePowerOutputConnectorColour;

	public CellOffset ElectricalArrowOffset;

	public ConduitType InputConduitType;

	public ConduitType OutputConduitType;

	public bool ModifiesTemperature;

	public bool Floodable = true;

	public bool Disinfectable = true;

	public bool Entombable = true;

	public bool Replaceable = true;

	public bool Invincible;

	public bool Overheatable = true;

	public bool Repairable = true;

	public float OverheatTemperature = 348.15f;

	public float FatalHot = 533.15f;

	public bool Breakable;

	public bool ContinuouslyCheckFoundation;

	public bool IsFoundation;

	public bool DragBuild;

	public bool UseStructureTemperature = true;

	public global::Action HotKey = global::Action.NumActions;

	public CellOffset attachablePosition = new CellOffset(0, 0);

	public bool CanMove;

	public List<Tag> ReplacementTags;

	public List<ObjectLayer> ReplacementCandidateLayers;

	public List<ObjectLayer> EquivalentReplacementLayers;

	[HashedEnum]
	[NonSerialized]
	public HashedString ViewMode = OverlayModes.None.ID;

	public BuildLocationRule BuildLocationRule;

	public ObjectLayer ObjectLayer = ObjectLayer.Building;

	public ObjectLayer TileLayer = ObjectLayer.NumLayers;

	public ObjectLayer ReplacementLayer = ObjectLayer.NumLayers;

	public Vector3 placementPivot;

	public string DiseaseCellVisName;

	public string[] MaterialCategory;

	public string AudioCategory = "Metal";

	public string AudioSize = "medium";

	public float[] Mass;

	public bool AlwaysOperational;

	public List<LogicPorts.Port> LogicInputPorts;

	public List<LogicPorts.Port> LogicOutputPorts;

	public bool Upgradeable;

	public float BaseTimeUntilRepair = 600f;

	public bool ShowInBuildMenu = true;

	public PermittedRotations PermittedRotations;

	public Orientation InitialOrientation;

	public bool Deprecated;

	public CellOffset PowerInputOffset;

	public CellOffset PowerOutputOffset;

	public CellOffset UtilityInputOffset = new CellOffset(0, 1);

	public CellOffset UtilityOutputOffset = new CellOffset(1, 0);

	public Grid.SceneLayer SceneLayer = Grid.SceneLayer.Building;

	public Grid.SceneLayer ForegroundLayer = Grid.SceneLayer.BuildingFront;

	public string RequiredAttribute = "";

	public int RequiredAttributeLevel;

	public List<Descriptor> EffectDescription;

	public float MassTier;

	public float HeatTier;

	public float ConstructionTimeTier;

	public string PrimaryUse;

	public string SecondaryUse;

	public string PrimarySideEffect;

	public string SecondarySideEffect;

	public Recipe CraftRecipe;

	public Sprite UISprite;

	public bool isKAnimTile;

	public bool isUtility;

	public bool isSolidTile;

	public KAnimFile[] AnimFiles;

	public string DefaultAnimState = "off";

	public bool BlockTileIsTransparent;

	public TextureAtlas BlockTileAtlas;

	public TextureAtlas BlockTilePlaceAtlas;

	public TextureAtlas BlockTileShineAtlas;

	public Material BlockTileMaterial;

	public BlockTileDecorInfo DecorBlockTileInfo;

	public BlockTileDecorInfo DecorPlaceBlockTileInfo;

	public List<global::Klei.AI.Attribute> attributes = new List<global::Klei.AI.Attribute>();

	public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();

	public Tag AttachmentSlotTag;

	public bool PreventIdleTraversalPastBuilding;

	public GameObject BuildingComplete;

	public GameObject BuildingPreview;

	public GameObject BuildingUnderConstruction;

	public CellOffset[] PlacementOffsets;

	public CellOffset[] ConstructionOffsetFilter;

	public static CellOffset[] ConstructionOffsetFilter_OneDown = new CellOffset[]
	{
		new CellOffset(0, -1)
	};

	public float BaseDecor;

	public float BaseDecorRadius;

	public int BaseNoisePollution;

	public int BaseNoisePollutionRadius;

	private static Dictionary<CellOffset, CellOffset[]> placementOffsetsCache = new Dictionary<CellOffset, CellOffset[]>();
}
