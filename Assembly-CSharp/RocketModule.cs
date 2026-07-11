using System;
using System.Collections.Generic;
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
	}

	public void RegisterWithConditionManager()
	{
		if (this.conditionManager != null)
		{
			this.conditionManager.RegisterRocketModule(this);
		}
		else
		{
			global::Debug.LogWarning("Module conditionManager is null", null);
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

	protected string parentRocketName = UI.STARMAP.DEFAULT_NAME;

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnLaunchDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.OnLaunch(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnLandDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.OnLand(data);
	});
}
