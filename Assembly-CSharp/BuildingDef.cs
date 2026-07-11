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

	public GameObject Create(Vector3 pos, Storage resource_storage, IList<Element> selected_elements, Recipe recipe, float temperature, GameObject obj)
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
		GameObject gameObject = GameUtil.KInstantiate(obj, pos, this.SceneLayer, Folder.Buildings, null, 0);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.ElementID = selected_elements[0].id;
		component.Temperature = temperature;
		component.AddDisease(diseaseInfo.idx, diseaseInfo.count, "BuildingDef.Create");
		gameObject.name = obj.name;
		gameObject.SetActive(true);
		return gameObject;
	}

	public GameObject Build(int cell, Orientation orientation, Storage resource_storage, IList<Element> selected_elements, float temperature, bool relocated, bool playsound = true)
	{
		Vector3 vector = Grid.CellToPosCBC(cell, this.SceneLayer);
		GameObject gameObject;
		if (relocated)
		{
			gameObject = this.Create(vector, resource_storage, selected_elements, this.RelocateRecipe, temperature, this.BuildingComplete);
		}
		else
		{
			gameObject = this.Create(vector, resource_storage, selected_elements, this.CraftRecipe, temperature, this.BuildingComplete);
		}
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
				TileVisualizer.RefreshCell(c, this.TileLayer);
			});
		}
		string sound = GlobalAssets.GetSound("Finish_Building_" + this.AudioSize, false);
		if (playsound && sound != null)
		{
			KMonoBehaviour.PlaySound3DAtLocation(sound, gameObject.transform.GetPosition());
		}
		Game.Instance.Trigger(-1661515756, gameObject);
		return gameObject;
	}

	public GameObject TryPlace(GameObject src_go, Vector3 pos, Orientation orientation, IList<Element> selected_elements, int layer = 0, bool relocated = false)
	{
		GameObject gameObject = null;
		string text;
		if (this.IsValidPlaceLocation(src_go, pos, orientation, out text))
		{
			gameObject = this.Instantiate(pos, orientation, selected_elements, layer, relocated);
		}
		return gameObject;
	}

	public GameObject Instantiate(Vector3 pos, Orientation orientation, IList<Element> selected_elements, int layer = 0, bool relocated = false)
	{
		float depthBias = InterfaceTool.DepthBias;
		pos.z += depthBias;
		GameObject gameObject = ((!relocated) ? this.BuildingUnderConstruction : this.BuildingUnderRelocation);
		Vector3 vector = pos;
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		Folder folder = Folder.Placers;
		GameObject gameObject2 = GameUtil.KInstantiate(gameObject, vector, sceneLayer, folder, null, layer);
		gameObject2.GetComponent<PrimaryElement>().ElementID = selected_elements[0].id;
		gameObject2.GetComponent<Constructable>().SelectedElements = selected_elements;
		gameObject2.SetActive(true);
		return gameObject2;
	}

	private bool IsAreaClear(GameObject source_go, int cell, Orientation orientation, ObjectLayer layer, ObjectLayer tile_layer, out string fail_reason)
	{
		bool flag = true;
		fail_reason = null;
		switch (this.BuildLocationRule)
		{
		case BuildLocationRule.Conduit:
			return this.IsValidConduitLocation(source_go, cell, orientation, out fail_reason);
		case BuildLocationRule.NotInTiles:
		{
			GameObject gameObject = Grid.Objects[cell, 9];
			if (gameObject != null && gameObject != source_go)
			{
				flag = false;
			}
			else
			{
				GameObject gameObject2 = Grid.Objects[cell, (int)this.ObjectLayer];
				if (gameObject2 != null)
				{
					if (this.ReplacementLayer == ObjectLayer.NumLayers)
					{
						if (gameObject2 != source_go)
						{
							flag = false;
						}
					}
					else
					{
						Building component = gameObject2.GetComponent<Building>();
						if (component != null && component.Def.ReplacementLayer != this.ReplacementLayer)
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
			goto IL_0330;
		}
		case BuildLocationRule.LogicBridge:
			return this.AreLogicPortsInValidPositions(source_go, cell, out fail_reason);
		}
		for (int i = 0; i < this.PlacementOffsets.Length; i++)
		{
			CellOffset cellOffset = this.PlacementOffsets[i];
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(cellOffset, orientation);
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
			GameObject gameObject3 = Grid.Objects[num, (int)layer];
			if (gameObject3 != null)
			{
				if (gameObject3.GetComponent<Wire>() == null || this.BuildingComplete.GetComponent<Wire>() == null)
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
					flag = false;
				}
				break;
			}
			if (tile_layer != ObjectLayer.NumLayers && Grid.Objects[num, (int)tile_layer] != null && Grid.Objects[num, (int)tile_layer].GetComponent<BuildingPreview>() == null)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
				flag = false;
				break;
			}
			if (this.BuildLocationRule == BuildLocationRule.Tile)
			{
				GameObject gameObject4 = Grid.Objects[num, 25];
				if (gameObject4 != null && gameObject4 != source_go)
				{
					Building component2 = gameObject4.GetComponent<Building>();
					if (component2.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
					{
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRE_OBSTRUCTION;
						flag = false;
						break;
					}
				}
				gameObject4 = Grid.Objects[cell, 2];
				if (gameObject4 != null && gameObject4 != source_go)
				{
					Building component3 = gameObject4.GetComponent<Building>();
					if (component3 != null && component3.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
					{
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRE_OBSTRUCTION;
						flag = false;
						break;
					}
				}
			}
			else if (this.BuildLocationRule == BuildLocationRule.OnFloorOverSpace && global::World.Instance.zoneRenderData.GetSubWorldZoneType(num) != SubWorld.ZoneType.Space)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_SPACE;
				flag = false;
				break;
			}
		}
		IL_0330:
		flag = flag && this.IsValidConduitLocation(source_go, cell, orientation, out fail_reason);
		return flag && this.AreLogicPortsInValidPositions(source_go, cell, out fail_reason);
	}

	public void RunOnArea(int cell, Orientation orientation, Action<int> callback)
	{
		for (int i = 0; i < this.PlacementOffsets.Length; i++)
		{
			CellOffset cellOffset = this.PlacementOffsets[i];
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(cellOffset, orientation);
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			callback(num);
		}
	}

	public void MarkArea(int cell, Orientation orientation, ObjectLayer layer, GameObject go)
	{
		if (this.BuildLocationRule != BuildLocationRule.Conduit)
		{
			for (int i = 0; i < this.PlacementOffsets.Length; i++)
			{
				CellOffset cellOffset = this.PlacementOffsets[i];
				CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(cellOffset, orientation);
				int num = Grid.OffsetCell(cell, rotatedCellOffset);
				Grid.Objects[num, (int)layer] = go;
			}
		}
		if (this.InputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(this.UtilityInputOffset, orientation);
			int num2 = Grid.OffsetCell(cell, rotatedCellOffset2);
			ObjectLayer objectLayerForConduitType = Grid.GetObjectLayerForConduitType(this.InputConduitType);
			Grid.Objects[num2, (int)objectLayerForConduitType] = go;
		}
		if (this.OutputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(this.UtilityOutputOffset, orientation);
			int num3 = Grid.OffsetCell(cell, rotatedCellOffset3);
			ObjectLayer objectLayerForConduitType2 = Grid.GetObjectLayerForConduitType(this.OutputConduitType);
			Grid.Objects[num3, (int)objectLayerForConduitType2] = go;
		}
	}

	public void UnmarkArea(int cell, Orientation orientation, ObjectLayer layer, GameObject go)
	{
		for (int i = 0; i < this.PlacementOffsets.Length; i++)
		{
			CellOffset cellOffset = this.PlacementOffsets[i];
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(cellOffset, orientation);
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
			Grid.Objects[num2, (int)objectLayerForConduitType] = null;
		}
		if (this.OutputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(this.UtilityOutputOffset, orientation);
			int num3 = Grid.OffsetCell(cell, rotatedCellOffset3);
			ObjectLayer objectLayerForConduitType2 = Grid.GetObjectLayerForConduitType(this.OutputConduitType);
			Grid.Objects[num3, (int)objectLayerForConduitType2] = null;
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
		return this.IsValidPlaceLocation(go, num, orientation, out fail_reason);
	}

	public bool IsValidPlaceLocation(GameObject go, int cell, Orientation orientation, out string fail_reason)
	{
		if (!Grid.IsValidBuildingCell(cell))
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
			return false;
		}
		return this.IsAreaClear(go, cell, orientation, this.ObjectLayer, this.TileLayer, out fail_reason);
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
			CellOffset cellOffset = this.PlacementOffsets[i];
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(cellOffset, orientation);
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
		string empty = string.Empty;
		return this.IsValidBuildLocation(source_go, pos, orientation, out empty);
	}

	public bool IsValidBuildLocation(GameObject source_go, Vector3 pos, Orientation orientation, out string reason)
	{
		int num = Grid.PosToCell(pos);
		if (!Grid.IsValidBuildingCell(num))
		{
			reason = "Invalid cell";
			return false;
		}
		return this.IsValidBuildLocation(source_go, num, orientation, out reason);
	}

	public bool IsValidBuildLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		if (!Grid.IsValidBuildingCell(cell))
		{
			fail_reason = "Invalid cell";
			return false;
		}
		bool flag = true;
		fail_reason = null;
		switch (this.BuildLocationRule)
		{
		case BuildLocationRule.OnFloor:
		case BuildLocationRule.OnCeiling:
			if (!BuildingDef.CheckFoundation(cell, orientation, this.BuildLocationRule, this.WidthInCells, this.HeightInCells))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_FLOOR;
			}
			break;
		case BuildLocationRule.Anywhere:
		case BuildLocationRule.Conduit:
			flag = true;
			break;
		case BuildLocationRule.Tile:
		{
			flag = true;
			GameObject gameObject = Grid.Objects[cell, 25];
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
			GameObject gameObject4 = Grid.Objects[cell, 1];
			if (gameObject4 != null && Grid.PosToCell(gameObject4) == cell)
			{
				BuildingAttachPoint component4 = gameObject4.GetComponent<BuildingAttachPoint>();
				if (component4 != null && component4.AcceptsAttachment(this.AttachableBuildingType))
				{
					flag = true;
				}
			}
			fail_reason = string.Format(UI.TOOLTIPS.HELP_BUILDLOCATION_ATTACHPOINT, this.AttachableBuildingType);
			break;
		}
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
		}
		if (flag)
		{
			flag = this.IsValidConduitLocation(source_go, cell, orientation, out fail_reason);
		}
		return flag;
	}

	private bool IsValidConduitLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		bool flag = true;
		fail_reason = null;
		if (this.InputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.UtilityInputOffset, orientation);
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			flag = this.IsValidConduitConnection(source_go, num, ref fail_reason);
		}
		if (flag && this.OutputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(this.UtilityOutputOffset, orientation);
			int num2 = Grid.OffsetCell(cell, rotatedCellOffset2);
			flag = this.IsValidConduitConnection(source_go, num2, ref fail_reason);
		}
		return flag;
	}

	private bool AreLogicPortsInValidPositions(GameObject source_go, int cell, out string fail_reason)
	{
		fail_reason = null;
		if (source_go == null)
		{
			return true;
		}
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		ReadOnlyCollection<ILogicUIElement> visElements = logicCircuitManager.GetVisElements();
		LogicPorts component = source_go.GetComponent<LogicPorts>();
		if (component != null)
		{
			if (this.DoLogicPortsConflict(component.inputPorts, visElements) || this.DoLogicPortsConflict(component.outputPorts, visElements))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_LOGIC_PORTS_OBSTRUCTED;
				return false;
			}
		}
		else
		{
			LogicGateBase component2 = source_go.GetComponent<LogicGateBase>();
			if (component2 != null && (this.IsLogicPortObstructed(component2.InputCellOne, visElements) || this.IsLogicPortObstructed(component2.OutputCell, visElements) || (component2.RequiresTwoInputs && this.IsLogicPortObstructed(component2.InputCellTwo, visElements))))
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
		foreach (ILogicUIElement logicUIElement in ports)
		{
			if (logicUIElement.GetLogicUICell() == cell)
			{
				num++;
			}
		}
		return num > 0;
	}

	private bool IsValidConduitConnection(GameObject source_go, int utility_cell, ref string fail_reason)
	{
		bool flag = true;
		ConduitType inputConduitType = this.InputConduitType;
		if (inputConduitType != ConduitType.Gas)
		{
			if (inputConduitType != ConduitType.Liquid)
			{
				if (inputConduitType == ConduitType.Solid)
				{
					GameObject gameObject = Grid.Objects[utility_cell, 23];
					if (gameObject != null && gameObject != source_go)
					{
						flag = false;
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_SOLIDPORTS_OBSTRUCTED;
					}
				}
			}
			else
			{
				GameObject gameObject2 = Grid.Objects[utility_cell, 19];
				if (gameObject2 != null && gameObject2 != source_go)
				{
					flag = false;
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_LIQUIDPORTS_OBSTRUCTED;
				}
			}
		}
		else
		{
			GameObject gameObject3 = Grid.Objects[utility_cell, 15];
			if (gameObject3 != null && gameObject3 != source_go)
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_GASPORTS_OBSTRUCTED;
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
		int num = -(width - 1) / 2;
		int num2 = width / 2;
		if (orientation == Orientation.FlipH)
		{
			int num3 = num;
			num = -num2;
			num2 = -num3;
		}
		bool flag = true;
		for (int i = num; i <= num2; i++)
		{
			int num4 = ((location_rule != BuildLocationRule.OnCeiling) ? Grid.OffsetCell(cell, i, -1) : Grid.OffsetCell(cell, i, height));
			if (!Grid.IsValidBuildingCell(num4) || !Grid.Solid[num4])
			{
				flag = false;
				break;
			}
		}
		return flag;
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

	public Sprite GetUISprite(string animName = "ui")
	{
		return Def.GetUISpriteFromMultiObjectAnim(this.AnimFiles[0], animName);
	}

	public void GetExtents(bool is_rotated, Vector3 pos, out Vector2I min, out Vector2I max)
	{
		Grid.PosToXY(pos, out min);
		if (!is_rotated)
		{
			max = min + new Vector2I(this.WidthInCells, this.HeightInCells);
		}
		else
		{
			max = min + new Vector2I(this.HeightInCells, this.WidthInCells);
		}
	}

	public void GenerateOffsets()
	{
		int num = this.WidthInCells / 2;
		int num2 = num;
		int num3 = num2 - this.WidthInCells + 1;
		List<CellOffset> list = new List<CellOffset>();
		int num4 = 0;
		int heightInCells = this.HeightInCells;
		for (int i = num4; i < heightInCells; i++)
		{
			for (int j = num3; j <= num2; j++)
			{
				list.Add(new CellOffset
				{
					x = j,
					y = i
				});
			}
		}
		this.PlacementOffsets = list.ToArray();
	}

	public void PostProcess()
	{
		this.RelocateRecipe = new Recipe();
		this.RelocateRecipe.Ingredients = new List<Recipe.Ingredient>
		{
			new Recipe.Ingredient(this.PrefabID + "Package", 1f)
		};
		string name = this.BuildingComplete.PrefabID().Name;
		string name2 = this.Name;
		this.CraftRecipe = new Recipe(name, 1f, (SimHashes)0, name2, null, 0);
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
			Db.Get().TechItems.AddTechItem(this.PrefabID, this.Name, this.Effect, new Func<string, Sprite>(this.GetUISprite));
		}
	}

	public bool MaterialsAvailable(IList<Element> selected_elements)
	{
		bool flag = true;
		foreach (Recipe.Ingredient ingredient in this.CraftRecipe.GetAllIngredients(selected_elements))
		{
			float amount = WorldInventory.Instance.GetAmount(ingredient.tag);
			if (amount < ingredient.amount)
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

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

	[HashedEnum]
	[NonSerialized]
	public SimViewMode ViewMode;

	[HashedEnum]
	[NonSerialized]
	public SimViewMode SelectMode;

	public BuildLocationRule BuildLocationRule;

	public ObjectLayer ObjectLayer = ObjectLayer.Building;

	public ObjectLayer TileLayer = ObjectLayer.NumLayers;

	public ObjectLayer ReplacementLayer = ObjectLayer.NumLayers;

	public Vector3 placementPivot;

	public string DiseaseCellVisName;

	public string[] MaterialCategory;

	public string AudioCategory;

	public string AudioSize = "medium";

	public float[] Mass;

	public bool Upgradeable;

	public float BaseTimeUntilRepair = 600f;

	public bool ShowInBuildMenu = true;

	public PermittedRotations PermittedRotations;

	public bool Deprecated;

	public CellOffset PowerInputOffset;

	public CellOffset PowerOutputOffset;

	public CellOffset UtilityInputOffset = new CellOffset(0, 1);

	public CellOffset UtilityOutputOffset = new CellOffset(1, 0);

	public Grid.SceneLayer SceneLayer = Grid.SceneLayer.Building;

	public Grid.SceneLayer ForegroundLayer = Grid.SceneLayer.BuildingFront;

	public string RequiredAttribute = string.Empty;

	public int RequiredAttributeLevel;

	public List<Descriptor> EffectDescription;

	public float MassTier;

	public float HeatTier;

	public float ConstructionTimeTier;

	public string PrimaryUse;

	public string SecondaryUse;

	public string PrimarySideEffect;

	public string SecondarySideEffect;

	public GameObject BuildingTemplate;

	public Recipe CraftRecipe;

	public Recipe RelocateRecipe;

	public Sprite UISprite;

	public bool isKAnimTile;

	public bool isUtility;

	public bool isSolidTile;

	public KAnimFile[] AnimFiles;

	public string DefaultAnimState = "off";

	public TextureAtlas BlockTileAtlas;

	public TextureAtlas BlockTilePlaceAtlas;

	public TextureAtlas BlockTileShineAtlas;

	public Material BlockTileMaterial;

	public BlockTileDecorInfo DecorBlockTileInfo;

	public BlockTileDecorInfo DecorPlaceBlockTileInfo;

	public List<global::Klei.AI.Attribute> attributes = new List<global::Klei.AI.Attribute>();

	public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();

	public Tag AttachableBuildingType;

	public bool PreventIdlingInFrontOfBuilding;

	public GameObject BuildingComplete;

	public GameObject BuildingPreview;

	public GameObject BuildingUnderConstruction;

	public GameObject BuildingUnderRelocation;

	public CellOffset[] PlacementOffsets;

	public CellOffset[] ConstructionOffsetFilter;

	public float BaseDecor;

	public float BaseDecorRadius;

	public int BaseNoisePollution;

	public int BaseNoisePollutionRadius;

	public BuildingDef[] Enables;
}
