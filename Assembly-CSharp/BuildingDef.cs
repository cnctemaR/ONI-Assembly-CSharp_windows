using System;
using System.Collections.Generic;
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

	public static float GetEnergyEfficiency(Element element, float scale = 0.0125f)
	{
		return scale;
	}

	public static string GetEfficiencyString(Element element)
	{
		string text = UIConstants.ColorPrefixRed + "Horrible";
		if (element != null && element.id == SimHashes.Copper)
		{
			text = UIConstants.ColorPrefixGreen + "Very Good";
		}
		else
		{
			float energyEfficiency = BuildingDef.GetEnergyEfficiency(ElementLoader.FindElementByHash(SimHashes.Copper), 0.0125f);
			float energyEfficiency2 = BuildingDef.GetEnergyEfficiency(element, 0.0125f);
			float num = energyEfficiency2 / energyEfficiency;
			if ((double)num > 0.65)
			{
				text = UIConstants.ColorPrefixGreen + "Good";
			}
			else if ((double)num > 0.15)
			{
				text = UIConstants.ColorPrefixWhite + "Fair";
			}
			else if ((double)num > 0.05)
			{
				text = UIConstants.ColorPrefixYellow + "Poor";
			}
		}
		return text + UIConstants.ColorSuffix;
	}

	public GameObject Create(Vector3 pos, Storage resource_storage, Recipe.Ingredient[] tags, GameObject obj)
	{
		if (tags != null)
		{
			foreach (Recipe.Ingredient ingredient in tags)
			{
				if (resource_storage != null)
				{
					resource_storage.Consume(ingredient.tag, ingredient.amount);
				}
			}
		}
		GameObject gameObject = GameUtil.KInstantiate(obj, pos, Grid.SceneLayer.Building, Folder.Buildings, null, 0);
		gameObject.name = obj.name;
		gameObject.SetActive(true);
		return gameObject;
	}

	public GameObject Build(int cell, Orientation orientation, Storage resource_storage, IList<Element> selected_elements, bool relocated)
	{
		Vector3 vector = Grid.CellToPosCBC(cell, this.SceneLayer);
		GameObject gameObject;
		if (relocated)
		{
			gameObject = this.Create(vector, resource_storage, this.RelocateRecipe.GetAllIngredients(selected_elements), this.BuildingComplete);
		}
		else
		{
			gameObject = this.Create(vector, resource_storage, this.CraftRecipe.GetAllIngredients(selected_elements), this.BuildingComplete);
		}
		Rotatable component = gameObject.GetComponent<Rotatable>();
		if (component != null)
		{
			component.SetOrientation(orientation);
		}
		PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
		component2.ElementID = selected_elements[0].id;
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
		if (sound != null)
		{
			KMonoBehaviour.PlaySound3DAtLocation(sound, gameObject.transform.position);
		}
		Game.Instance.Trigger(-1661515756, gameObject);
		return gameObject;
	}

	public GameObject TryPlace(Vector3 pos, Orientation orientation, IList<Element> selected_elements, int layer = 0, bool relocated = false)
	{
		GameObject gameObject = null;
		if (this.IsValidPlaceLocation(pos, orientation))
		{
			gameObject = this.Instantiate(pos, orientation, selected_elements, layer, relocated);
		}
		return gameObject;
	}

	public GameObject Instantiate(Vector3 pos, Orientation orientation, IList<Element> selected_elements, int layer = 0, bool relocated = false)
	{
		float depthBias = InterfaceTool.DepthBias;
		pos.z += depthBias;
		GameObject gameObject = GameUtil.KInstantiate((!relocated) ? this.BuildingUnderConstruction : this.BuildingUnderRelocation, pos, Grid.SceneLayer.Front, Folder.Placers, null, layer);
		gameObject.SetActive(true);
		gameObject.GetComponent<PrimaryElement>().ElementID = selected_elements[0].id;
		gameObject.GetComponent<Constructable>().SelectedElements = selected_elements;
		return gameObject;
	}

	private bool IsAreaClear(int cell, Orientation orientation, ObjectLayer layer, ObjectLayer tile_layer)
	{
		BuildLocationRule buildLocationRule = this.BuildLocationRule;
		if (buildLocationRule == BuildLocationRule.Conduit)
		{
			return this.IsValidConduitLocation(cell, orientation);
		}
		if (buildLocationRule != BuildLocationRule.NotInTiles)
		{
			for (int i = 0; i < this.PlacementOffsets.Length; i++)
			{
				CellOffset cellOffset = this.PlacementOffsets[i];
				CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(cellOffset, orientation);
				int num = Grid.OffsetCell(cell, rotatedCellOffset);
				if (Grid.Element[num].id == SimHashes.Unobtanium)
				{
					return false;
				}
				if (!Grid.IsValidCell(num) || Grid.Objects[num, (int)layer] != null)
				{
					return false;
				}
				if (tile_layer != ObjectLayer.NumLayers && Grid.Objects[num, (int)tile_layer] != null && Grid.Objects[num, (int)tile_layer].GetComponent<BuildingPreview>() == null)
				{
					return false;
				}
			}
			if (this.BuildLocationRule == BuildLocationRule.Tile)
			{
				GameObject gameObject = Grid.Objects[cell, 20];
				if (gameObject != null)
				{
					Building component = gameObject.GetComponent<Building>();
					if (component.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
					{
						return false;
					}
				}
			}
			return this.IsValidConduitLocation(cell, orientation);
		}
		return Grid.Objects[cell, 9] == null;
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
			ConduitType conduitType = this.InputConduitType;
			if (conduitType != ConduitType.Gas)
			{
				if (conduitType == ConduitType.Liquid)
				{
					Grid.Objects[num2, 18] = go;
				}
			}
			else
			{
				Grid.Objects[num2, 14] = go;
			}
		}
		if (this.OutputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(this.UtilityOutputOffset, orientation);
			int num3 = Grid.OffsetCell(cell, rotatedCellOffset3);
			ConduitType conduitType = this.OutputConduitType;
			if (conduitType != ConduitType.Gas)
			{
				if (conduitType == ConduitType.Liquid)
				{
					Grid.Objects[num3, 18] = go;
				}
			}
			else
			{
				Grid.Objects[num3, 14] = go;
			}
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
			ConduitType conduitType = this.InputConduitType;
			if (conduitType != ConduitType.Gas)
			{
				if (conduitType == ConduitType.Liquid)
				{
					Grid.Objects[num2, 18] = null;
				}
			}
			else
			{
				Grid.Objects[num2, 14] = null;
			}
		}
		if (this.OutputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(this.UtilityOutputOffset, orientation);
			int num3 = Grid.OffsetCell(cell, rotatedCellOffset3);
			ConduitType conduitType = this.OutputConduitType;
			if (conduitType != ConduitType.Gas)
			{
				if (conduitType == ConduitType.Liquid)
				{
					Grid.Objects[num3, 18] = null;
				}
			}
			else
			{
				Grid.Objects[num3, 14] = null;
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

	public bool IsValidPlaceLocation(Vector3 pos, Orientation orientation)
	{
		int num = Grid.PosToCell(pos);
		return Grid.IsValidCell(num) && this.IsAreaClear(num, orientation, this.ObjectLayer, this.TileLayer);
	}

	public bool IsValidBuildLocation(Vector3 pos, Orientation orientation)
	{
		string empty = string.Empty;
		return this.IsValidBuildLocation(pos, orientation, out empty);
	}

	public bool IsValidBuildLocation(Vector3 pos, Orientation orientation, out string reason)
	{
		int num = Grid.PosToCell(pos);
		if (!Grid.IsValidCell(num))
		{
			reason = "Invalid cell";
			return false;
		}
		return this.IsValidBuildLocation(num, orientation, out reason);
	}

	public bool IsValidBuildLocation(int cell, Orientation orientation, out string reason)
	{
		if (!Grid.IsValidCell(cell))
		{
			reason = "Invalid cell";
			return false;
		}
		reason = string.Empty;
		bool flag = false;
		switch (this.BuildLocationRule)
		{
		case BuildLocationRule.OnFloor:
		{
			int num = -(this.WidthInCells - 1) / 2;
			int num2 = this.WidthInCells / 2;
			flag = true;
			reason = string.Empty;
			for (int i = num; i <= num2; i++)
			{
				int num3 = Grid.OffsetCell(cell, i, -1);
				if (!Grid.IsValidCell(num3) || !Grid.Solid[num3])
				{
					reason = UI.TOOLTIPS.HELP_BUILDLOCATION_FLOOR;
					flag = false;
				}
			}
			break;
		}
		case BuildLocationRule.Anywhere:
		case BuildLocationRule.Conduit:
			flag = true;
			break;
		case BuildLocationRule.OnCeiling:
		{
			int num4 = -(this.WidthInCells - 1) / 2;
			int num5 = this.WidthInCells / 2;
			flag = true;
			for (int j = num4; j <= num5; j++)
			{
				int num6 = Grid.OffsetCell(cell, j, 1 * this.HeightInCells);
				if (!Grid.IsValidCell(num6) || !Grid.Solid[num6])
				{
					reason = UI.TOOLTIPS.HELP_BUILDLOCATION_CEILING;
					flag = false;
				}
			}
			break;
		}
		case BuildLocationRule.Tile:
		{
			flag = true;
			GameObject gameObject = Grid.Objects[cell, 20];
			if (gameObject != null)
			{
				Building component = gameObject.GetComponent<Building>();
				if (component.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
				{
					flag = false;
				}
			}
			break;
		}
		case BuildLocationRule.NotInTiles:
			flag = Grid.Objects[cell, 9] == null;
			break;
		}
		return flag;
	}

	private bool IsValidConduitLocation(int cell, Orientation orientation)
	{
		bool flag = true;
		if (this.InputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.UtilityInputOffset, orientation);
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			ConduitType conduitType = this.InputConduitType;
			if (conduitType != ConduitType.Gas)
			{
				if (conduitType == ConduitType.Liquid)
				{
					flag = flag && Grid.Objects[num, 18] == null;
				}
			}
			else
			{
				flag = flag && Grid.Objects[num, 14] == null;
			}
		}
		if (this.OutputConduitType != ConduitType.None)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(this.UtilityOutputOffset, orientation);
			int num2 = Grid.OffsetCell(cell, rotatedCellOffset2);
			ConduitType conduitType = this.OutputConduitType;
			if (conduitType != ConduitType.Gas)
			{
				if (conduitType == ConduitType.Liquid)
				{
					flag = flag && Grid.Objects[num2, 18] == null;
				}
			}
			else
			{
				flag = flag && Grid.Objects[num2, 14] == null;
			}
		}
		return flag;
	}

	public Sprite GetUISprite(string animName = "ui")
	{
		if (this.AnimFiles == null || this.AnimFiles.Length == 0)
		{
			Output.LogWarning(new object[] { "Building", base.name, "missing Anim Files" });
			return this.UISprite;
		}
		KAnimFile kanimFile = this.AnimFiles[0];
		if (this.AnimFiles[0] == null)
		{
			Debug.LogError("Missing anim file for building: " + base.name);
		}
		KAnimFileData data = kanimFile.GetData();
		if (!data.batchTag.isValid || data.batchTag == KAnimBatchManager.NO_BATCH)
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
		float x = symbolFrame.uv0.x;
		float x2 = symbolFrame.uv1.x;
		float y = symbolFrame.uv2.y;
		float y2 = symbolFrame.uv0.y;
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
		Sprite sprite = Sprite.Create(texture, rect, new Vector2(0f, 0f), num4, 0U, SpriteMeshType.FullRect);
		sprite.name = base.name + ":" + frameElement.frame.ToString();
		return sprite;
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
		string name = this.Name;
		this.CraftRecipe = new Recipe(this.BuildingComplete.PrefabID().Name, 1f, (SimHashes)0, name, null, 0);
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
		if (!string.IsNullOrEmpty(this.RequiredTechName))
		{
			Tech tech = Db.Get().Techs.Get(this.RequiredTechName);
			if (tech != null && !this.Deprecated)
			{
				BuildingDef buildingDef = tech.unlockedBuildings.Find((BuildingDef ub) => ub.PrefabID == this.PrefabID);
				if (buildingDef != null)
				{
					tech.unlockedBuildings.Remove(buildingDef);
				}
				this.RequiredTech = tech;
				tech.unlockedBuildings.Add(this);
			}
		}
	}

	public float EnergyConsumptionWhenActive;

	public float GeneratorWattageRating;

	public float GeneratorBaseCapacity;

	public float MassForTemperatureModification;

	public float ExhaustKilowattsWhenActive;

	public float OperatingKilowatts;

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

	public bool Entombable = true;

	public bool Relocatable = true;

	public bool Overheatable = true;

	public float OverheatTemperature = 348.15f;

	public float FatalHot = 533.15f;

	public bool Breakable;

	public bool ContinuouslyCheckFoundation;

	public bool IsFoundation;

	public bool DragBuild;

	public bool UseStructureTemperature = true;

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

	public string[] MaterialCategory;

	public string AudioCategory;

	public string AudioSize = "medium";

	public float[] Mass;

	public bool Upgradeable;

	public float BaseTimeUntilRepair = 600f;

	public float PlanOrder;

	public string PlanCategory;

	public bool ShowInBuildMenu = true;

	public PermittedRotations PermittedRotations;

	public bool Deprecated;

	public CellOffset PowerInputOffset;

	public CellOffset PowerOutputOffset;

	public CellOffset UtilityInputOffset = new CellOffset(0, 1);

	public CellOffset UtilityOutputOffset = new CellOffset(1, 0);

	public string MinionEffect = string.Empty;

	public Grid.SceneLayer SceneLayer = Grid.SceneLayer.Building;

	public Grid.SceneLayer ForegroundLayer = Grid.SceneLayer.BuildingFront;

	public string RequiredAttribute = string.Empty;

	public int RequiredAttributeLevel;

	public string RequiredTechName = string.Empty;

	public string Slot = string.Empty;

	public Tech RequiredTech;

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

	[Tooltip("The visualizer used when this objects overlay is active")]
	public KAnimFile OverlayAnim;

	public string DefaultAnimState = "off";

	public TextureAtlas BlockTileAtlas;

	public TextureAtlas BlockTilePlaceAtlas;

	public Material BlockTileMaterial;

	public BlockTileDecorInfo DecorBlockTileInfo;

	public BlockTileDecorInfo DecorPlaceBlockTileInfo;

	public List<Klei.AI.Attribute> attributes = new List<Klei.AI.Attribute>();

	public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();

	public GameObject BuildingComplete;

	public GameObject BuildingPreview;

	public GameObject BuildingUnderConstruction;

	public GameObject BuildingUnderRelocation;

	public GameObject BuildingPackage;

	public CellOffset[] PlacementOffsets;

	public CellOffset[] ConstructionOffsetFilter;

	public float BaseDecor;

	public float BaseDecorRadius;

	public BuildingDef[] Enables;
}
