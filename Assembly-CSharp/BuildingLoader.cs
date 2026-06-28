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
		gameObject.AddComponent<StateMachineController>();
		PrimaryElement primaryElement = gameObject.AddComponent<PrimaryElement>();
		primaryElement.Mass = 1f;
		primaryElement.Temperature = 293f;
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
		Storage storage = gameObject.AddComponent<Storage>();
		storage.doDiseaseTransfer = false;
		gameObject.AddComponent<Cancellable>();
		gameObject.AddComponent<Approachable>();
		gameObject.AddComponent<UserMenu>();
		gameObject.AddComponent<Notifier>();
		gameObject.AddComponent<Prioritizable>();
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
		BoxCollider2D boxCollider2D = BuildingLoader.UpdateComponentRequirement<BoxCollider2D>(go, flag && !no_collider);
		if (boxCollider2D != null)
		{
			boxCollider2D.offset = new Vector3(0f, 0.5f * (float)def.HeightInCells, 0f);
			boxCollider2D.size = new Vector3((float)def.WidthInCells, (float)def.HeightInCells, 0f);
			boxCollider2D.sharedMaterial = Assets.defaultPhysicsMaterial;
		}
		if (def.AnimFiles == null)
		{
			global::Debug.LogError(def.Name + " Def missing anim files", null);
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
	}

	public GameObject CreateBuildingUnderConstruction(BuildingDef def, bool isRelocating)
	{
		GameObject gameObject = this.CreateBuilding(def, this.constructionTemplate, SceneOrganizer.Instance.GetFolder(Folder.GlobalDoNotDestroy));
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
		Rotatable rotatable = BuildingLoader.UpdateComponentRequirement<Rotatable>(gameObject, def.PermittedRotations != PermittedRotations.Unrotatable);
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
			gameObject.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.GlobalDoNotDestroy).transform;
			gameObject.transform.position = new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer));
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.MassPerUnit = def.Mass[0];
			BuildingHP buildingHP = BuildingLoader.UpdateComponentRequirement<BuildingHP>(gameObject, true);
			if (def.Invincible)
			{
				buildingHP.invincible = true;
			}
			buildingHP.SetHitPoints(def.HitPoints);
			if (def.Repairable)
			{
				BuildingLoader.UpdateComponentRequirement<Repairable>(gameObject, true);
			}
			int num = LayerMask.NameToLayer("Default");
			gameObject.layer = num;
			Building component2 = gameObject.GetComponent<BuildingComplete>();
			component2.Def = def;
			if (def.InputConduitType != ConduitType.None || def.OutputConduitType != ConduitType.None)
			{
				gameObject.AddComponent<BuildingConduitEndpoints>();
			}
			if (!BuildingLoader.Add2DComponents(def, gameObject, null, false, -1))
			{
				global::Debug.Log(def.Name + " is not yet a 2d building!", null);
			}
			BuildingLoader.UpdateComponentRequirement<EnergyConsumer>(gameObject, def.RequiresPowerInput);
			Rotatable rotatable = BuildingLoader.UpdateComponentRequirement<Rotatable>(gameObject, def.PermittedRotations != PermittedRotations.Unrotatable);
			if (rotatable)
			{
				rotatable.permittedRotations = def.PermittedRotations;
			}
			if (def.Breakable)
			{
				gameObject.AddComponent<Breakable>();
			}
			ConduitConsumer conduitConsumer = BuildingLoader.UpdateComponentRequirement<ConduitConsumer>(gameObject, def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid);
			if (conduitConsumer != null)
			{
				conduitConsumer.SetConduitData(def.InputConduitType);
			}
			bool flag = def.RequiresPowerInput || def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid;
			RequireInputs requireInputs = BuildingLoader.UpdateComponentRequirement<RequireInputs>(gameObject, flag);
			if (requireInputs != null)
			{
				requireInputs.SetRequirements(def.RequiresPowerInput, def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid);
			}
			BuildingLoader.UpdateComponentRequirement<RequireOutputs>(gameObject, def.OutputConduitType != ConduitType.None);
			BuildingLoader.UpdateComponentRequirement<Operational>(gameObject, !def.isUtility);
			if (def.Floodable)
			{
				gameObject.AddComponent<Floodable>();
			}
			if (def.Disinfectable)
			{
				gameObject.AddOrGet<AutoDisinfectable>();
				gameObject.AddOrGet<Disinfectable>();
			}
			if (def.Overheatable)
			{
				Overheatable overheatable = gameObject.AddComponent<Overheatable>();
				overheatable.baseOverheatTemp = def.OverheatTemperature;
				overheatable.baseFatalTemp = def.FatalHot;
			}
			if (def.Entombable)
			{
				gameObject.AddComponent<Structure>();
			}
			if (def.Relocatable)
			{
				gameObject.AddComponent<Relocatable>();
			}
			BuildingLoader.UpdateComponentRequirement<BuildingCellVisualizer>(gameObject, BuildingCellVisualizer.CheckRequiresComponent(def));
			LoopingSounds component3 = gameObject.GetComponent<LoopingSounds>();
			if (component3 == null)
			{
				gameObject.AddComponent<LoopingSounds>();
			}
			BuildingLoader.UpdateComponentRequirement<Upgradable>(gameObject, def.Upgradeable);
			DecorProvider decorProvider = BuildingLoader.UpdateComponentRequirement<DecorProvider>(gameObject, true);
			decorProvider.baseDecor = def.BaseDecor;
			decorProvider.baseRadius = def.BaseDecorRadius;
			if (def.AttachableBuildingType != Tag.Invalid)
			{
				AttachableBuilding attachableBuilding = BuildingLoader.UpdateComponentRequirement<AttachableBuilding>(gameObject, true);
				attachableBuilding.attachableTag = def.AttachableBuildingType;
			}
			KPrefabID kprefabID = DefLoader.AddID(gameObject, def.PrefabID);
			kprefabID.defaultLayer = num;
			Assets.AddPrefab(kprefabID);
			gameObject.PreInit();
		}
		return gameObject;
	}

	public GameObject CreateBuildingPreview(BuildingDef def)
	{
		GameObject gameObject = this.CreateBuilding(def, this.previewTemplate, SceneOrganizer.Instance.GetFolder(Folder.GlobalDoNotDestroy));
		int num = LayerMask.NameToLayer("Place");
		gameObject.transform.position = new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer));
		BuildingLoader.Add2DComponents(def, gameObject, "place", true, num);
		KAnimControllerBase component = gameObject.GetComponent<KAnimControllerBase>();
		if (component != null)
		{
			component.fgLayer = Grid.SceneLayer.NoLayer;
		}
		Rotatable rotatable = BuildingLoader.UpdateComponentRequirement<Rotatable>(gameObject, def.PermittedRotations != PermittedRotations.Unrotatable);
		if (rotatable)
		{
			rotatable.permittedRotations = def.PermittedRotations;
		}
		KPrefabID kprefabID = DefLoader.AddID(gameObject, def.PrefabID + "Preview");
		kprefabID.defaultLayer = num;
		KSelectable component2 = gameObject.GetComponent<KSelectable>();
		component2.SetName(def.Name);
		BuildingCellVisualizer buildingCellVisualizer = BuildingLoader.UpdateComponentRequirement<BuildingCellVisualizer>(gameObject, BuildingCellVisualizer.CheckRequiresComponent(def));
		if (buildingCellVisualizer != null)
		{
			BuildingLoader.CopyBuildingCellVisualizer(def.BuildingComplete, buildingCellVisualizer);
		}
		KAnimGraphTileVisualizer component3 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
		if (component3 != null)
		{
			global::UnityEngine.Object.DestroyImmediate(component3);
		}
		gameObject.PreInit();
		return gameObject;
	}

	public GameObject CreateBuildingPackage(BuildingDef def)
	{
		GameObject gameObject = this.CreateBuilding(def, this.packageTemplate, SceneOrganizer.Instance.GetFolder(Folder.GlobalDoNotDestroy));
		int num = LayerMask.NameToLayer("Loot");
		gameObject.transform.position = new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer));
		Relocatable relocatable = BuildingLoader.UpdateComponentRequirement<Relocatable>(gameObject, true);
		relocatable.deconstruct = false;
		KPrefabID kprefabID = DefLoader.AddID(gameObject, def.PrefabID + "Package");
		kprefabID.defaultLayer = num;
		KSelectable component = gameObject.GetComponent<KSelectable>();
		component.SetName(def.Name);
		Assets.AddPrefab(kprefabID);
		gameObject.PreInit();
		return gameObject;
	}

	private GameObject previewTemplate;

	private GameObject constructionTemplate;

	private GameObject packageTemplate;

	public static BuildingLoader Instance;
}
