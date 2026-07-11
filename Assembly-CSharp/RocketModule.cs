using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class RocketModule : KMonoBehaviour
{
	public void AddCondition(RocketLaunchCondition condition)
	{
		if (!this.launchConditions.Contains(condition))
		{
			this.launchConditions.Add(condition);
			this.RegisterConditions();
		}
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
		this.RegisterConditions();
		KSelectable component = base.GetComponent<KSelectable>();
		if (component != null)
		{
			component.AddStatusItem(Db.Get().BuildingStatusItems.RocketName, this);
		}
	}

	public void RegisterConditions()
	{
		if (this.conditionManager != null)
		{
			foreach (RocketLaunchCondition rocketLaunchCondition in this.launchConditions)
			{
				this.conditionManager.RegisterCondition(rocketLaunchCondition);
			}
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
			foreach (RocketLaunchCondition rocketLaunchCondition in this.launchConditions)
			{
				this.conditionManager.UnregisterCondition(rocketLaunchCondition);
			}
		}
		base.OnCleanUp();
	}

	public virtual void OnSuspend(object data)
	{
		this.isSuspended = true;
	}

	public virtual void OnResume(object data)
	{
		this.isSuspended = false;
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

	protected string parentRocketName = UI.STARMAP.DEFAULT_NAME;
}
