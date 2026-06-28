using System;
using UnityEngine;

public class BuildingLoader : DefLoader
{
	protected override void OnPrefabInit()
	{
		BuildingLoader.Instance = this;
		this.previewTemplate = this.CreatePreviewTemplate();
		this.constructionTemplate = this.CreateConstructionTemplate();
		this.packageTemplate = this.CreatePackageTemplate();
	}

	private GameObject CreateTemplate()
	{
		GameObject gameObject = new GameObject();
		gameObject.SetActive(false);
		gameObject.AddComponent<KPrefabID>();
		gameObject.AddComponent<KSelectable>();
		gameObject.AddComponent<PrimaryElement>();
		gameObject.AddComponent<StateMachineController>();
		return gameObject;
	}

	private GameObject CreatePreviewTemplate()
	{
		GameObject gameObject = this.CreateTemplate();
		gameObject.AddComponent<BuildingPreview>();
		return gameObject;
	}

	private GameObject CreateConstructionTemplate()
	{
		GameObject gameObject = this.CreateTemplate();
		gameObject.AddComponent<BuildingUnderConstruction>();
		gameObject.AddComponent<Constructable>();
		gameObject.AddComponent<Storage>();
		gameObject.AddComponent<Cancellable>();
		gameObject.AddComponent<Approachable>();
		gameObject.AddComponent<UserMenu>();
		gameObject.AddComponent<Notifier>();
		return gameObject;
	}

	private GameObject CreatePackageTemplate()
	{
		return EntityPrefabs.Instance.GenericBuildingPackage;
	}

	public GameObject CreateBuilding(BuildingDef def, GameObject go, GameObject parent = null)
	{
		go = global::UnityEngine.Object.Instantiate<GameObject>(go);
		go.name = def.PrefabID;
		if (parent != null)
		{
			go.transform.parent = parent.transform;
		}
		go.GetComponent<Building>().Def = def;
		return go;
	}

	private static bool Add2DComponents(BuildingDef def, GameObject go, string initialAnimState = null, bool no_collider = false, int layer = -1)
	{
		bool flag = def.AnimFiles != null && def.AnimFiles.Length > 0;
		if (layer == -1)
		{
			layer = LayerMask.NameToLayer("Default");
		}
		go.layer = layer;
		KBatchedAnimController[] components = go.GetComponents<KBatchedAnimController>();
		if (components.Length > 1)
		{
			for (int i = 2; i < components.Length; i++)
			{
				global::UnityEngine.Object.DestroyImmediate(components[i]);
			}
		}
		if (def.BlockTileAtlas == null)
		{
			KBatchedAnimController kbatchedAnimController = BuildingLoader.UpdateComponentRequirement<KBatchedAnimController>(go, flag);
			if (kbatchedAnimController != null)
			{
				kbatchedAnimController.AnimFiles = def.AnimFiles;
				if (def.isKAnimTile)
				{
					kbatchedAnimController.initialAnim = null;
				}
				else
				{
					if (def.isUtility && initialAnimState == null)
					{
						initialAnimState = "idle";
					}
					else if (go.GetComponent<Door>() != null)
					{
						initialAnimState = "closed";
					}
					kbatchedAnimController.initialAnim = ((initialAnimState == null) ? def.DefaultAnimState : initialAnimState);
				}
				kbatchedAnimController.SetFGLayer(def.ForegroundLayer);
				kbatchedAnimController.materialType = KAnimBatchGroup.MaterialType.Default;
			}
		}
		BuildingLoader.UpdateComponentRequirement<KAnimGridTileVisualizer>(go, flag && def.isKAnimTile && !def.isGraphTile);
		KAnimGraphTileVisualizer kanimGraphTileVisualizer = BuildingLoader.UpdateComponentRequirement<KAnimGraphTileVisualizer>(go, flag && def.isKAnimTile && def.isGraphTile);
		if (kanimGraphTileVisualizer != null)
		{
			Wire component = def.BuildingTemplate.GetComponent<Wire>();
			if (component != null)
			{
				kanimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Electrical;
			}
			else
			{
				Vent vent = go.GetComponent<Vent>();
				if (vent == null)
				{
					vent = def.BuildingComplete.GetComponent<Vent>();
				}
				kanimGraphTileVisualizer.connectionSource = ((vent.transferType != Vent.Transfer.Gas) ? KAnimGraphTileVisualizer.ConnectionSource.Liquid : KAnimGraphTileVisualizer.ConnectionSource.Gas);
			}
			BuildingComplete component2 = go.GetComponent<BuildingComplete>();
			kanimGraphTileVisualizer.isPhysicalBuilding = component2 != null;
		}
		BoxCollider2D boxCollider2D = BuildingLoader.UpdateComponentRequirement<BoxCollider2D>(go, flag && !no_collider);
		if (boxCollider2D != null)
		{
			boxCollider2D.offset = new Vector3(0f, 0.5f * (float)def.HeightInCells, 0f);
			boxCollider2D.size = new Vector3((float)def.WidthInCells, (float)def.HeightInCells, 0f);
			boxCollider2D.sharedMaterial = Assets.defaultPhysicsMaterial;
		}
		if (def.AnimFiles == null)
		{
			Debug.LogError(def.Name + " Def missing anim files");
		}
		return flag;
	}

	private static T UpdateComponentRequirement<T>(GameObject go, bool required) where T : Component
	{
		T t = go.GetComponent(typeof(T)) as T;
		if (!required && t != null)
		{
			global::UnityEngine.Object.DestroyImmediate(t, true);
			t = (T)((object)null);
		}
		else if (required && t == null)
		{
			t = go.AddComponent(typeof(T)) as T;
		}
		return t;
	}

	private static void CopyBuildingCellVisualizer(GameObject src, BuildingCellVisualizer target)
	{
		BuildingCellVisualizer component = src.GetComponent<BuildingCellVisualizer>();
		target.secondOutputOffset = component.secondOutputOffset;
		target.secondOutputColour = component.secondOutputColour;
	}

	public GameObject CreateBuildingUnderConstruction(BuildingDef def, bool isRelocating)
	{
		GameObject gameObject = this.CreateBuilding(def, this.constructionTemplate, SceneOrganizer.Instance.GetFolder(Folder.BuildingUnderConstructions));
		KSelectable component = gameObject.GetComponent<KSelectable>();
		component.SetName(def.Name);
		gameObject.GetComponent<PrimaryElement>().MassPerUnit = def.Mass[0];
		KPrefabID kprefabID = DefLoader.AddID(gameObject, def.PrefabID + ((!isRelocating) ? "UnderConstruction" : "UnderRelocation"));
		BuildingCellVisualizer buildingCellVisualizer = BuildingLoader.UpdateComponentRequirement<BuildingCellVisualizer>(gameObject, BuildingCellVisualizer.CheckRequiresComponent(def));
		if (buildingCellVisualizer != null)
		{
			BuildingLoader.CopyBuildingCellVisualizer(def.BuildingComplete, buildingCellVisualizer);
		}
		Constructable component2 = gameObject.GetComponent<Constructable>();
		component2.isRelocating = isRelocating;
		component2.SetWorkTime((!isRelocating) ? def.ConstructionTime : 4f);
		Rotatable rotatable = BuildingLoader.UpdateComponentRequirement<Rotatable>(gameObject, def.PermittedRotations != Rotatable.PermittedRotations.Unrotatable);
		if (rotatable)
		{
			rotatable.permittedRotations = def.PermittedRotations;
		}
		int num = LayerMask.NameToLayer("Construction");
		kprefabID.defaultLayer = num;
		BuildingLoader.Add2DComponents(def, gameObject, "place", false, num);
		BuildingLoader.UpdateComponentRequirement<Vent>(gameObject, false);
		bool flag = def.BuildingComplete.GetComponent<AnimTileable>() != null;
		BuildingLoader.UpdateComponentRequirement<AnimTileable>(gameObject, flag);
		Assets.AddPrefab(kprefabID);
		gameObject.PreInit();
		return gameObject;
	}

	public GameObject CreateBuildingComplete(BuildingDef def)
	{
		GameObject gameObject = def.BuildingTemplate;
		gameObject.SetActive(false);
		if (gameObject != null)
		{
			gameObject = global::UnityEngine.Object.Instantiate<GameObject>(gameObject);
			gameObject.name = def.PrefabID + "Complete";
			gameObject.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.BuildingCompletes).transform;
			gameObject.transform.position = new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer));
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.MassPerUnit = def.Mass[0];
			int num = LayerMask.NameToLayer("Default");
			gameObject.layer = num;
			Building component2 = gameObject.GetComponent<BuildingComplete>();
			component2.Def = def;
			BuildingLoader.AddVentComponents(def, gameObject);
			if (!BuildingLoader.Add2DComponents(def, gameObject, null, false, -1))
			{
				Debug.Log(def.Name + " is not yet a 2d building!");
			}
			BuildingLoader.UpdateComponentRequirement<EnergyConsumer>(gameObject, def.RequiresPower);
			Rotatable rotatable = BuildingLoader.UpdateComponentRequirement<Rotatable>(gameObject, def.PermittedRotations != Rotatable.PermittedRotations.Unrotatable);
			if (rotatable)
			{
				rotatable.permittedRotations = def.PermittedRotations;
			}
			if (def.Breakable)
			{
				gameObject.AddComponent<Breakable>();
				gameObject.AddComponent<Repairable>();
			}
			ConduitConsumer conduitConsumer = BuildingLoader.UpdateComponentRequirement<ConduitConsumer>(gameObject, def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid);
			if (conduitConsumer != null)
			{
				conduitConsumer.SetConduitData(def.InputConduitType);
			}
			bool flag = def.RequiresPower || def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid;
			RequireInputs requireInputs = BuildingLoader.UpdateComponentRequirement<RequireInputs>(gameObject, flag);
			if (requireInputs != null)
			{
				requireInputs.SetRequirements(def.RequiresPower, def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid);
			}
			BuildingLoader.UpdateComponentRequirement<RequireOutputs>(gameObject, def.OutputConduitType != ConduitType.None);
			bool flag2 = gameObject.GetComponent<SimCellOccupier>();
			BuildingLoader.UpdateComponentRequirement<Operational>(gameObject, !def.isUtility && !flag2);
			if (def.Floodable)
			{
				gameObject.AddComponent<Floodable>();
			}
			if (def.Entombable)
			{
				gameObject.AddComponent<Structure>();
			}
			if (def.Relocatable)
			{
				gameObject.AddComponent<Relocatable>();
			}
			if (def.Slot != null && def.Slot != string.Empty)
			{
				Ownable ownable = gameObject.AddComponent<Ownable>();
				ownable.slot = Db.Get().OwnableSlots.Get(def.Slot);
			}
			BuildingLoader.UpdateComponentRequirement<BuildingCellVisualizer>(gameObject, BuildingCellVisualizer.CheckRequiresComponent(def));
			LoopingSounds component3 = gameObject.GetComponent<LoopingSounds>();
			if (def.IsTilePiece && def.isKAnimTile)
			{
				if (component3 != null)
				{
					global::UnityEngine.Object.DestroyImmediate(component3);
				}
			}
			else if (component3 == null)
			{
				gameObject.AddComponent<LoopingSounds>();
			}
			BuildingLoader.UpdateComponentRequirement<StructureTemperature>(gameObject, !def.isSolidTile && gameObject.GetComponent<Door>() == null);
			BuildingLoader.UpdateComponentRequirement<InfraredVisualizer>(gameObject, !def.isSolidTile);
			BuildingLoader.UpdateComponentRequirement<Upgradable>(gameObject, def.Upgradeable);
			DecorProvider decorProvider = BuildingLoader.UpdateComponentRequirement<DecorProvider>(gameObject, true);
			decorProvider.baseDecor = def.BaseDecor;
			decorProvider.baseRadius = def.BaseDecorRadius;
			KPrefabID kprefabID = DefLoader.AddID(gameObject, def.PrefabID);
			kprefabID.defaultLayer = num;
			Assets.AddPrefab(kprefabID);
			gameObject.PreInit();
		}
		return gameObject;
	}

	private static void AddVentComponents(BuildingDef def, GameObject go)
	{
		Vent[] array = go.GetComponents<Vent>();
		bool flag = def.InputConduitType != ConduitType.None;
		bool flag2 = def.OutputConduitType == ConduitType.Gas || def.OutputConduitType == ConduitType.Liquid;
		int num = 0;
		num += ((!flag) ? 0 : 1);
		num += ((!flag2) ? 0 : 1);
		if (array == null || array.Length == 0)
		{
			for (int i = 0; i < num; i++)
			{
				go.AddComponent<Vent>();
			}
		}
		else if (array != null)
		{
			if (array.Length < num)
			{
				for (int j = array.Length; j < num; j++)
				{
					go.AddComponent<Vent>();
				}
			}
			else
			{
				for (int k = num; k < array.Length; k++)
				{
					global::UnityEngine.Object.DestroyImmediate(array[k]);
				}
			}
		}
		array = go.GetComponents<Vent>();
		int num2 = 0;
		Vent vent = null;
		if (flag)
		{
			vent = array[num2];
			num2++;
		}
		Vent vent2 = null;
		if (flag2)
		{
			vent2 = array[num2];
			num2++;
		}
		if (vent != null)
		{
			switch (def.InputConduitType)
			{
			case ConduitType.Gas:
				vent.transferType = Vent.Transfer.Gas;
				vent.endpointType = Vent.Endpoint.Sink;
				break;
			case ConduitType.Liquid:
				vent.transferType = Vent.Transfer.Liquid;
				vent.endpointType = Vent.Endpoint.Sink;
				break;
			case ConduitType.GasConduit:
				vent.transferType = Vent.Transfer.Gas;
				vent.endpointType = Vent.Endpoint.Conduit;
				break;
			case ConduitType.LiquidConduit:
				vent.transferType = Vent.Transfer.Liquid;
				vent.endpointType = Vent.Endpoint.Conduit;
				break;
			}
		}
		if (vent2 != null)
		{
			ConduitType outputConduitType = def.OutputConduitType;
			if (outputConduitType != ConduitType.Gas)
			{
				if (outputConduitType == ConduitType.Liquid)
				{
					vent2.transferType = Vent.Transfer.Liquid;
					vent2.endpointType = Vent.Endpoint.Source;
				}
			}
			else
			{
				vent2.transferType = Vent.Transfer.Gas;
				vent2.endpointType = Vent.Endpoint.Source;
			}
		}
	}

	public GameObject CreateBuildingPreview(BuildingDef def)
	{
		GameObject gameObject = this.CreateBuilding(def, this.previewTemplate, SceneOrganizer.Instance.GetFolder(Folder.BuildingPreviews));
		int num = LayerMask.NameToLayer("Place");
		gameObject.transform.position = new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer));
		BuildingLoader.Add2DComponents(def, gameObject, "place", true, num);
		Rotatable rotatable = BuildingLoader.UpdateComponentRequirement<Rotatable>(gameObject, def.PermittedRotations != Rotatable.PermittedRotations.Unrotatable);
		if (rotatable)
		{
			rotatable.permittedRotations = def.PermittedRotations;
		}
		KPrefabID kprefabID = DefLoader.AddID(gameObject, def.PrefabID + "Preview");
		kprefabID.defaultLayer = num;
		BuildingCellVisualizer buildingCellVisualizer = BuildingLoader.UpdateComponentRequirement<BuildingCellVisualizer>(gameObject, BuildingCellVisualizer.CheckRequiresComponent(def));
		if (buildingCellVisualizer != null)
		{
			BuildingLoader.CopyBuildingCellVisualizer(def.BuildingComplete, buildingCellVisualizer);
		}
		KAnimGraphTileVisualizer component = gameObject.GetComponent<KAnimGraphTileVisualizer>();
		if (component != null)
		{
			global::UnityEngine.Object.DestroyImmediate(component);
		}
		gameObject.PreInit();
		return gameObject;
	}

	public GameObject CreateBuildingPackage(BuildingDef def)
	{
		GameObject gameObject = this.CreateBuilding(def, this.packageTemplate, SceneOrganizer.Instance.GetFolder(Folder.Loot));
		int num = LayerMask.NameToLayer("Loot");
		gameObject.transform.position = new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer));
		Relocatable relocatable = BuildingLoader.UpdateComponentRequirement<Relocatable>(gameObject, true);
		relocatable.deconstruct = false;
		KPrefabID kprefabID = DefLoader.AddID(gameObject, def.PrefabID + "Package");
		kprefabID.defaultLayer = num;
		Assets.AddPrefab(kprefabID);
		gameObject.PreInit();
		return gameObject;
	}

	private GameObject previewTemplate;

	private GameObject constructionTemplate;

	private GameObject packageTemplate;

	public static BuildingLoader Instance;
}
