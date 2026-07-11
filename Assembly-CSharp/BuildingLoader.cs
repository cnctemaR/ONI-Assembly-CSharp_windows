using System;
using UnityEngine;

public class BuildingLoader : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		BuildingLoader.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		BuildingLoader.Instance = this;
		this.previewTemplate = this.CreatePreviewTemplate();
		this.constructionTemplate = this.CreateConstructionTemplate();
	}

	private GameObject CreateTemplate()
	{
		GameObject gameObject = new GameObject();
		gameObject.SetActive(false);
		gameObject.AddOrGet<KPrefabID>();
		gameObject.AddOrGet<KSelectable>();
		gameObject.AddOrGet<StateMachineController>();
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
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
		gameObject.AddOrGet<BuildingUnderConstruction>();
		gameObject.AddOrGet<Constructable>();
		Storage storage = gameObject.AddComponent<Storage>();
		storage.doDiseaseTransfer = false;
		gameObject.AddOrGet<Cancellable>();
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<Notifier>();
		gameObject.AddOrGet<SaveLoadRoot>();
		return gameObject;
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
		KBoxCollider2D kboxCollider2D = BuildingLoader.UpdateComponentRequirement<KBoxCollider2D>(go, flag && !no_collider);
		if (kboxCollider2D != null)
		{
			kboxCollider2D.offset = new Vector3(0f, 0.5f * (float)def.HeightInCells, 0f);
			kboxCollider2D.size = new Vector3((float)def.WidthInCells, (float)def.HeightInCells, 0f);
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

	public static KPrefabID AddID(GameObject go, string str)
	{
		KPrefabID kprefabID = go.GetComponent<KPrefabID>();
		if (kprefabID == null)
		{
			kprefabID = go.AddComponent<KPrefabID>();
		}
		kprefabID.PrefabTag = new Tag(str);
		kprefabID.SaveLoadTag = kprefabID.PrefabTag;
		return kprefabID;
	}

	public GameObject CreateBuildingUnderConstruction(BuildingDef def)
	{
		GameObject gameObject = this.CreateBuilding(def, this.constructionTemplate, null);
		global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
		KSelectable component = gameObject.GetComponent<KSelectable>();
		component.SetName(def.Name);
		gameObject.GetComponent<PrimaryElement>().MassPerUnit = def.Mass[0];
		KPrefabID kprefabID = BuildingLoader.AddID(gameObject, def.PrefabID + "UnderConstruction");
		BuildingLoader.UpdateComponentRequirement<BuildingCellVisualizer>(gameObject, BuildingCellVisualizer.CheckRequiresComponent(def));
		Constructable component2 = gameObject.GetComponent<Constructable>();
		component2.SetWorkTime(def.ConstructionTime);
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
		if (def.RequiresPowerInput)
		{
			GeneratedBuildings.RegisterLogicPorts(gameObject, LogicOperationalController.INPUT_PORTS);
		}
		Assets.AddPrefab(kprefabID);
		gameObject.PreInit();
		return gameObject;
	}

	public GameObject CreateBuildingComplete(GameObject go, BuildingDef def)
	{
		go.name = def.PrefabID + "Complete";
		go.transform.SetPosition(new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer)));
		KSelectable component = go.GetComponent<KSelectable>();
		component.SetName(def.Name);
		PrimaryElement component2 = go.GetComponent<PrimaryElement>();
		component2.MassPerUnit = def.Mass[0];
		BuildingHP buildingHP = go.AddOrGet<BuildingHP>();
		if (def.Invincible)
		{
			buildingHP.invincible = true;
		}
		buildingHP.SetHitPoints(def.HitPoints);
		if (def.Repairable)
		{
			BuildingLoader.UpdateComponentRequirement<Repairable>(go, true);
		}
		int num = LayerMask.NameToLayer("Default");
		go.layer = num;
		Building component3 = go.GetComponent<BuildingComplete>();
		component3.Def = def;
		if (def.InputConduitType != ConduitType.None || def.OutputConduitType != ConduitType.None)
		{
			go.AddComponent<BuildingConduitEndpoints>();
		}
		if (!BuildingLoader.Add2DComponents(def, go, null, false, -1))
		{
			global::Debug.Log(def.Name + " is not yet a 2d building!", null);
		}
		BuildingLoader.UpdateComponentRequirement<EnergyConsumer>(go, def.RequiresPowerInput);
		Rotatable rotatable = BuildingLoader.UpdateComponentRequirement<Rotatable>(go, def.PermittedRotations != PermittedRotations.Unrotatable);
		if (rotatable)
		{
			rotatable.permittedRotations = def.PermittedRotations;
		}
		if (def.Breakable)
		{
			go.AddComponent<Breakable>();
		}
		ConduitConsumer conduitConsumer = BuildingLoader.UpdateComponentRequirement<ConduitConsumer>(go, def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid);
		if (conduitConsumer != null)
		{
			conduitConsumer.SetConduitData(def.InputConduitType);
		}
		bool flag = def.RequiresPowerInput || def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid;
		RequireInputs requireInputs = BuildingLoader.UpdateComponentRequirement<RequireInputs>(go, flag);
		if (requireInputs != null)
		{
			requireInputs.SetRequirements(def.RequiresPowerInput, def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid);
		}
		BuildingLoader.UpdateComponentRequirement<RequireOutputs>(go, def.OutputConduitType != ConduitType.None);
		BuildingLoader.UpdateComponentRequirement<Operational>(go, !def.isUtility);
		if (def.Floodable)
		{
			go.AddComponent<Floodable>();
		}
		if (def.Disinfectable)
		{
			go.AddOrGet<AutoDisinfectable>();
			go.AddOrGet<Disinfectable>();
		}
		if (def.Overheatable)
		{
			Overheatable overheatable = go.AddComponent<Overheatable>();
			overheatable.baseOverheatTemp = def.OverheatTemperature;
			overheatable.baseFatalTemp = def.FatalHot;
		}
		if (def.Entombable)
		{
			go.AddComponent<Structure>();
		}
		if (def.RequiresPowerInput)
		{
			GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS);
			go.AddOrGet<LogicOperationalController>();
		}
		BuildingLoader.UpdateComponentRequirement<BuildingCellVisualizer>(go, BuildingCellVisualizer.CheckRequiresComponent(def));
		if (def.BaseDecor != 0f)
		{
			DecorProvider decorProvider = BuildingLoader.UpdateComponentRequirement<DecorProvider>(go, true);
			decorProvider.baseDecor = def.BaseDecor;
			decorProvider.baseRadius = def.BaseDecorRadius;
		}
		if (def.AttachmentSlotTag != Tag.Invalid)
		{
			AttachableBuilding attachableBuilding = BuildingLoader.UpdateComponentRequirement<AttachableBuilding>(go, true);
			attachableBuilding.attachableToTag = def.AttachmentSlotTag;
		}
		KPrefabID kprefabID = BuildingLoader.AddID(go, def.PrefabID);
		kprefabID.defaultLayer = num;
		Assets.AddPrefab(kprefabID);
		go.PreInit();
		return go;
	}

	public GameObject CreateBuildingPreview(BuildingDef def)
	{
		GameObject gameObject = this.CreateBuilding(def, this.previewTemplate, null);
		global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
		int num = LayerMask.NameToLayer("Place");
		gameObject.transform.SetPosition(new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer)));
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
		KPrefabID kprefabID = BuildingLoader.AddID(gameObject, def.PrefabID + "Preview");
		kprefabID.defaultLayer = num;
		KSelectable component2 = gameObject.GetComponent<KSelectable>();
		component2.SetName(def.Name);
		BuildingLoader.UpdateComponentRequirement<BuildingCellVisualizer>(gameObject, BuildingCellVisualizer.CheckRequiresComponent(def));
		KAnimGraphTileVisualizer component3 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
		if (component3 != null)
		{
			global::UnityEngine.Object.DestroyImmediate(component3);
		}
		if (def.RequiresPowerInput)
		{
			GeneratedBuildings.RegisterLogicPorts(gameObject, LogicOperationalController.INPUT_PORTS);
		}
		gameObject.PreInit();
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		return gameObject;
	}

	private GameObject previewTemplate;

	private GameObject constructionTemplate;

	public static BuildingLoader Instance;
}
