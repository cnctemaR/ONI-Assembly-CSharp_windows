using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

[Serializable]
public class BuildingDef : Def
{
	public bool IsInsulated
	{
		get
		{
			return this.Insulation < 1f;
		}
	}

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

	public GameObject TryPlace(Vector3 pos, Orientation orientation, IList<Element> selected_elements, int layer = 0, bool relocated = false)
	{
		GameObject gameObject = null;
		string text;
		if (this.IsValidPlaceLocation(null, pos, orientation, out text))
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
		gameObject2.SetActive(true);
		gameObject2.GetComponent<PrimaryElement>().ElementID = selected_elements[0].id;
		gameObject2.GetComponent<Constructable>().SelectedElements = selected_elements;
		return gameObject2;
	}

	private bool IsAreaClear(GameObject source_go, int cell, Orientation orientation, ObjectLayer layer, ObjectLayer tile_layer, out string fail_reason)
	{
		bool flag = true;
		fail_reason = null;
		BuildLocationRule buildLocationRule = this.BuildLocationRule;
		if (buildLocationRule != BuildLocationRule.Conduit)
		{
			if (buildLocationRule != BuildLocationRule.NotInTiles)
			{
				for (int i = 0; i < this.PlacementOffsets.Length; i++)
				{
					CellOffset cellOffset = this.PlacementOffsets[i];
					CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(cellOffset, orientation);
					int num = Grid.OffsetCell(cell, rotatedCellOffset);
					if (!Grid.IsValidCell(num))
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
					GameObject gameObject = Grid.Objects[num, (int)layer];
					if (gameObject != null)
					{
						if (gameObject.GetComponent<Wire>() == null || this.BuildingComplete.GetComponent<Wire>() == null)
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
						GameObject gameObject2 = Grid.Objects[num, 25];
						if (gameObject2 != null && gameObject2 != source_go)
						{
							Building component = gameObject2.GetComponent<Building>();
							if (component.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
							{
								fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRE_OBSTRUCTION;
								flag = false;
							}
						}
						gameObject2 = Grid.Objects[cell, 2];
						if (gameObject2 != null && gameObject2 != source_go)
						{
							Building component2 = gameObject2.GetComponent<Building>();
							if (component2 != null && component2.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
							{
								fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRE_OBSTRUCTION;
								flag = false;
							}
						}
					}
				}
			}
			else
			{
				GameObject gameObject3 = Grid.Objects[cell, 9];
				GameObject gameObject4 = Grid.Objects[cell, (int)this.ObjectLayer];
				if ((gameObject3 != null && gameObject3 != source_go) || (gameObject4 != null && gameObject4 != source_go))
				{
					flag = false;
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_NOT_IN_TILES;
				}
			}
			if (flag)
			{
				flag = this.IsValidConduitLocation(source_go, cell, orientation, out fail_reason);
			}
			return flag;
		}
		return this.IsValidConduitLocation(source_go, cell, orientation, out fail_reason);
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
		if (!Grid.IsValidCell(cell))
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
			if (!Grid.IsValidCell(num2))
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
		if (!Grid.IsValidCell(num))
		{
			reason = "Invalid cell";
			return false;
		}
		return this.IsValidBuildLocation(source_go, num, orientation, out reason);
	}

	public bool IsValidBuildLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		if (!Grid.IsValidCell(cell))
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
				if (component.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
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
			gameObject2 = Grid.Objects[cell, (int)this.ObjectLayer];
			flag = flag && (gameObject2 == null || gameObject2 == source_go);
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_NOT_IN_TILES;
			break;
		}
		case BuildLocationRule.BuildingAttachPoint:
		{
			flag = false;
			GameObject gameObject3 = Grid.Objects[cell, 1];
			if (gameObject3 != null && Grid.PosToCell(gameObject3) == cell)
			{
				BuildingAttachPoint component3 = gameObject3.GetComponent<BuildingAttachPoint>();
				if (component3 != null && component3.AcceptsAttachment(this.AttachableBuildingType))
				{
					flag = true;
				}
			}
			fail_reason = string.Format(UI.TOOLTIPS.HELP_BUILDLOCATION_ATTACHPOINT, this.AttachableBuildingType);
			break;
		}
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

	private bool IsValidConduitConnection(GameObject source_go, int utility_cell, ref string fail_reason)
	{
		bool flag = true;
		ConduitType inputConduitType = this.InputConduitType;
		if (inputConduitType != ConduitType.Gas)
		{
			if (inputConduitType == ConduitType.Liquid)
			{
				GameObject gameObject = Grid.Objects[utility_cell, 19];
				if (gameObject != null && gameObject != source_go)
				{
					flag = false;
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_LIQUIDPORTS_OBSTRUCTED;
				}
			}
		}
		else
		{
			GameObject gameObject2 = Grid.Objects[utility_cell, 15];
			if (gameObject2 != null && gameObject2 != source_go)
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
			if (!Grid.IsValidCell(num4) || !Grid.Solid[num4])
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	public Sprite GetUISprite(string animName = "ui")
	{
		if (this.UISprite != null)
		{
			return this.UISprite;
		}
		if (this.AnimFiles == null || this.AnimFiles.Length == 0)
		{
			Output.LogWarning(new object[] { "Building", base.name, "missing Anim Files" });
			return this.UISprite;
		}
		KAnimFile kanimFile = this.AnimFiles[0];
		if (this.AnimFiles[0] == null)
		{
			global::Debug.LogError("Missing anim file for building: " + base.name, null);
		}
		KAnimFileData data = kanimFile.GetData();
		if (!data.batchTag.IsValid || data.batchTag == KAnimBatchManager.NO_BATCH)
		{
			Output.LogWarning(new object[] { "Building", base.name, "missing Anim Files" });
			return this.UISprite;
		}
		KAnim.Build build = data.build;
		if (build == null)
		{
			Output.LogWarning(new object[] { "Building", base.name, "build is null" });
			return this.UISprite;
		}
		KAnim.Anim.Frame frame = KAnim.Anim.Frame.InvalidFrame;
		for (int i = 0; i < data.animCount; i++)
		{
			KAnim.Anim anim = data.GetAnim(i);
			if (anim.name == animName)
			{
				frame = anim.GetFrame(data.batchTag, 0);
			}
		}
		if (!frame.IsValid())
		{
			Output.LogWarning(new object[]
			{
				"Building",
				base.name,
				"missing '" + animName + "' anim"
			});
			return this.UISprite;
		}
		KAnim.Anim.FrameElement frameElement = data.GetAnimFrameElement(data.elementCount - 1);
		KAnimHashedString kanimHashedString = new KAnimHashedString(animName);
		int elementCount = data.elementCount;
		for (int j = 0; j < elementCount; j++)
		{
			frameElement = data.GetAnimFrameElement(j);
			if (frameElement.symbol == kanimHashedString)
			{
				break;
			}
		}
		KAnim.Build.Symbol symbol = build.GetSymbol(frameElement.symbol);
		if (symbol == null)
		{
			Output.LogWarning(new object[] { "Building", base.name, "placeSymbol [", frameElement.symbol, "] is missing" });
			return this.UISprite;
		}
		if (!symbol.HasFrame(frameElement.frame))
		{
			Output.LogWarning(new object[] { "Building", base.name, "SymbolFrame [", frameElement.frame, "] is missing" });
			return this.UISprite;
		}
		KAnim.Build.SymbolFrame symbolFrame = symbol.GetFrame(frameElement.frame).symbolFrame;
		Texture2D texture = build.GetTexture(0);
		if (texture == null)
		{
			global::Debug.LogError("Missing build texture for:" + base.name, null);
		}
		float x = symbolFrame.uvMin.x;
		float x2 = symbolFrame.uvMax.x;
		float y = symbolFrame.uvMax.y;
		float y2 = symbolFrame.uvMin.y;
		int num = (int)((float)texture.width * Mathf.Abs(x2 - x));
		int num2 = (int)((float)texture.height * Mathf.Abs(y2 - y));
		float num3 = Mathf.Abs(symbolFrame.bboxMax.x - symbolFrame.bboxMin.x);
		Rect rect = default(Rect);
		rect.width = (float)num;
		rect.height = (float)num2;
		rect.x = (float)((int)((float)texture.width * x));
		rect.y = (float)((int)((float)texture.height * y));
		float num4 = 100f;
		if (num != 0)
		{
			num4 = 100f / (num3 / (float)num);
		}
		this.UISprite = Sprite.Create(texture, rect, new Vector2(0f, 0f), num4, 0U, SpriteMeshType.FullRect);
		this.UISprite.name = base.name + ":" + frameElement.frame.ToString();
		return this.UISprite;
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

	public float EnergyConsumptionWhenActive;

	public float GeneratorWattageRating;

	public float GeneratorBaseCapacity;

	public float MassForTemperatureModification;

	public float ExhaustKilowattsWhenActive;

	public float SelfHeatKilowattsWhenActive;

	public float BaseMeltingPoint;

	public float ConstructionTime;

	public float WorkTime;

	public float Insulation = 1f;

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

	public bool Relocatable = true;

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

	public GameObject BuildingPackage;

	public CellOffset[] PlacementOffsets;

	public CellOffset[] ConstructionOffsetFilter;

	public float BaseDecor;

	public float BaseDecorRadius;

	public int BaseNoisePollution;

	public int BaseNoisePollutionRadius;

	public BuildingDef[] Enables;
}
