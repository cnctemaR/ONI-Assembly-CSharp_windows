using System;
using System.Collections.Generic;
using System.Diagnostics;
using STRINGS;
using UnityEngine;

public class RocketModule : KMonoBehaviour
{
	public RocketLaunchCondition AddLaunchCondition(RocketLaunchCondition condition)
	{
		if (!this.launchConditions.Contains(condition))
		{
			this.launchConditions.Add(condition);
		}
		return condition;
	}

	public RocketFlightCondition AddFlightCondition(RocketFlightCondition condition)
	{
		if (!this.flightConditions.Contains(condition))
		{
			this.flightConditions.Add(condition);
		}
		return condition;
	}

	public void SetBGKAnim(KAnimFile anim_file)
	{
		this.bgAnimFile = anim_file;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.conditionManager = this.FindLaunchConditionManager();
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.conditionManager);
		if (spacecraftFromLaunchConditionManager != null)
		{
			this.SetParentRocketName(spacecraftFromLaunchConditionManager.GetRocketName());
		}
		this.RegisterWithConditionManager();
		KSelectable component = base.GetComponent<KSelectable>();
		if (component != null)
		{
			component.AddStatusItem(Db.Get().BuildingStatusItems.RocketName, this);
		}
		if (this.conditionManager != null && this.conditionManager.GetComponent<KPrefabID>().HasTag(GameTags.RocketNotOnGround))
		{
			this.OnLaunch(null);
		}
		base.Subscribe<RocketModule>(-1056989049, RocketModule.OnLaunchDelegate);
		base.Subscribe<RocketModule>(238242047, RocketModule.OnLandDelegate);
		base.Subscribe<RocketModule>(1502190696, RocketModule.DEBUG_OnDestroyDelegate);
		this.FixSorting();
		AttachableBuilding component2 = base.GetComponent<AttachableBuilding>();
		component2.onAttachmentNetworkChanged = (Action<AttachableBuilding>)Delegate.Combine(component2.onAttachmentNetworkChanged, new Action<AttachableBuilding>(this.OnAttachmentNetworkChanged));
		if (this.bgAnimFile != null)
		{
			this.AddBGGantry();
		}
	}

	public void FixSorting()
	{
		int num = 0;
		AttachableBuilding attachableBuilding = base.GetComponent<AttachableBuilding>();
		while (attachableBuilding != null)
		{
			BuildingAttachPoint attachedTo = attachableBuilding.GetAttachedTo();
			if (!(attachedTo != null))
			{
				break;
			}
			attachableBuilding = attachedTo.GetComponent<AttachableBuilding>();
			num++;
		}
		Vector3 localPosition = base.transform.GetLocalPosition();
		localPosition.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront) - (float)num * 0.01f;
		base.transform.SetLocalPosition(localPosition);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.enabled = false;
		component.enabled = true;
	}

	private void OnAttachmentNetworkChanged(AttachableBuilding ab)
	{
		this.FixSorting();
	}

	private void AddBGGantry()
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		GameObject gameObject = new GameObject();
		gameObject.name = string.Format(this.rocket_module_bg_base_string, base.name, this.rocket_module_bg_affix);
		gameObject.SetActive(false);
		Vector3 position = component.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.InteriorWall);
		gameObject.transform.SetPosition(position);
		gameObject.transform.parent = base.transform;
		KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.AnimFiles = new KAnimFile[] { this.bgAnimFile };
		kbatchedAnimController.initialAnim = this.rocket_module_bg_anim;
		kbatchedAnimController.fgLayer = Grid.SceneLayer.NoLayer;
		kbatchedAnimController.initialMode = KAnim.PlayMode.Paused;
		kbatchedAnimController.FlipX = component.FlipX;
		kbatchedAnimController.FlipY = component.FlipY;
		gameObject.SetActive(true);
	}

	private void DEBUG_OnDestroy(object data)
	{
		if (this.conditionManager != null && !App.IsExiting && !KMonoBehaviour.isLoadingScene)
		{
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.conditionManager);
			this.conditionManager.DEBUG_TraceModuleDestruction(base.name, (spacecraftFromLaunchConditionManager != null) ? spacecraftFromLaunchConditionManager.state.ToString() : "null spacecraft", new StackTrace(true).ToString());
		}
	}

	public void OnConditionManagerTagsChanged(object data)
	{
		KPrefabID component = this.conditionManager.GetComponent<KPrefabID>();
		if (component.HasTag(GameTags.RocketNotOnGround))
		{
			this.OnLaunch(null);
		}
	}

	private void OnLaunch(object data)
	{
		KSelectable component = base.GetComponent<KSelectable>();
		component.IsSelectable = false;
		if (SelectTool.Instance.selected == component)
		{
			SelectTool.Instance.Select(null, false);
		}
		ConduitConsumer component2 = base.GetComponent<ConduitConsumer>();
		if (component2)
		{
			ConduitType conduitType = component2.conduitType;
			if (conduitType == ConduitType.Gas || conduitType == ConduitType.Liquid)
			{
				component2.consumptionRate = 0f;
			}
		}
		Deconstructable component3 = base.GetComponent<Deconstructable>();
		if (component3 != null)
		{
			component3.SetAllowDeconstruction(false);
		}
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		if (handle.IsValid())
		{
			GameComps.StructureTemperatures.Disable(handle);
		}
		ManualDeliveryKG[] components = base.GetComponents<ManualDeliveryKG>();
		foreach (ManualDeliveryKG manualDeliveryKG in components)
		{
			manualDeliveryKG.Pause(true, "Rocket in space");
		}
		this.ToggleComponent(typeof(ElementConsumer), false);
		this.ToggleComponent(typeof(ElementConverter), false);
		this.ToggleComponent(typeof(ConduitDispenser), false);
		this.ToggleComponent(typeof(SolidConduitDispenser), false);
		this.ToggleComponent(typeof(EnergyConsumer), false);
	}

	private void OnLand(object data)
	{
		base.GetComponent<KSelectable>().IsSelectable = true;
		ConduitConsumer component = base.GetComponent<ConduitConsumer>();
		if (component)
		{
			ConduitType conduitType = component.conduitType;
			if (conduitType != ConduitType.Gas)
			{
				if (conduitType == ConduitType.Liquid)
				{
					base.GetComponent<ConduitConsumer>().consumptionRate = 10f;
				}
			}
			else
			{
				base.GetComponent<ConduitConsumer>().consumptionRate = 1f;
			}
		}
		Deconstructable component2 = base.GetComponent<Deconstructable>();
		if (component2 != null)
		{
			component2.SetAllowDeconstruction(true);
		}
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		if (handle.IsValid())
		{
			GameComps.StructureTemperatures.Enable(handle);
		}
		ManualDeliveryKG[] components = base.GetComponents<ManualDeliveryKG>();
		foreach (ManualDeliveryKG manualDeliveryKG in components)
		{
			manualDeliveryKG.Pause(false, "landed");
		}
		this.ToggleComponent(typeof(ElementConsumer), true);
		this.ToggleComponent(typeof(ElementConverter), true);
		this.ToggleComponent(typeof(ConduitDispenser), true);
		this.ToggleComponent(typeof(SolidConduitDispenser), true);
		this.ToggleComponent(typeof(EnergyConsumer), true);
	}

	private void ToggleComponent(Type cmpType, bool enabled)
	{
		MonoBehaviour monoBehaviour = (MonoBehaviour)base.GetComponent(cmpType);
		if (monoBehaviour != null)
		{
			monoBehaviour.enabled = enabled;
		}
	}

	public void RegisterWithConditionManager()
	{
		if (this.conditionManager != null)
		{
			this.conditionManager.RegisterRocketModule(this);
		}
		else
		{
			global::Debug.LogWarning("Module conditionManager is null");
		}
	}

	protected override void OnCleanUp()
	{
		if (this.conditionManager != null)
		{
			this.conditionManager.UnregisterRocketModule(this);
		}
		base.OnCleanUp();
	}

	public virtual void OnSuspend(object data)
	{
		this.isSuspended = true;
	}

	public bool IsSuspended()
	{
		return this.isSuspended;
	}

	public LaunchConditionManager FindLaunchConditionManager()
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>());
		foreach (GameObject gameObject in attachedNetwork)
		{
			LaunchConditionManager component = gameObject.GetComponent<LaunchConditionManager>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public void SetParentRocketName(string newName)
	{
		this.parentRocketName = newName;
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
	}

	public string GetParentRocketName()
	{
		return this.parentRocketName;
	}

	protected bool isSuspended;

	public LaunchConditionManager conditionManager;

	public List<RocketLaunchCondition> launchConditions = new List<RocketLaunchCondition>();

	public List<RocketFlightCondition> flightConditions = new List<RocketFlightCondition>();

	private string rocket_module_bg_base_string = "{0}{1}";

	private string rocket_module_bg_affix = "BG";

	private string rocket_module_bg_anim = "on";

	[SerializeField]
	private KAnimFile bgAnimFile;

	protected string parentRocketName = UI.STARMAP.DEFAULT_NAME;

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnLaunchDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.OnLaunch(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnLandDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.OnLand(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModule> DEBUG_OnDestroyDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.DEBUG_OnDestroy(data);
	});
}
