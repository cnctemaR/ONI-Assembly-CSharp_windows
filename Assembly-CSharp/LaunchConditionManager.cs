using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class LaunchConditionManager : KMonoBehaviour
{
	public List<RocketLaunchCondition> conditions { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.conditions = new List<RocketLaunchCondition>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.launchable = base.GetComponent<LaunchableRocket>();
		this.FindModules();
		base.GetComponent<AttachableBuilding>().onAttachmentNetworkChanged = delegate(AttachableBuilding data)
		{
			this.FindModules();
		};
		this.ScheduleUpdate();
	}

	protected override void OnCleanUp()
	{
		UIScheduler.Instance.GetScheduler().Clear(this.updateHandle);
		base.OnCleanUp();
	}

	private void ScheduleUpdate()
	{
		this.updateHandle = UIScheduler.Instance.Schedule("LaunchConditionManagerEvaluateConditions", 1f, delegate(object o)
		{
			this.EvaluateConditions();
			this.ScheduleUpdate();
		}, null, null);
	}

	public void FindModules()
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>());
		foreach (GameObject gameObject in attachedNetwork)
		{
			RocketModule component = gameObject.GetComponent<RocketModule>();
			if (component != null && component.conditionManager == null)
			{
				component.conditionManager = this;
				component.RegisterConditions();
			}
		}
	}

	public void RegisterCondition(RocketLaunchCondition condition)
	{
		foreach (RocketLaunchCondition rocketLaunchCondition in this.conditions)
		{
			if (rocketLaunchCondition == condition)
			{
				return;
			}
		}
		this.conditions.Add(condition);
	}

	public void UnregisterCondition(RocketLaunchCondition condition)
	{
		for (int i = this.conditions.Count - 1; i >= 0; i--)
		{
			if (this.conditions[i] == condition)
			{
				this.conditions.RemoveAt(i);
				break;
			}
		}
	}

	public int NumConditions
	{
		get
		{
			return this.conditions.Count;
		}
	}

	public void Launch(SpaceDestination destination)
	{
		if (destination == null)
		{
			global::Debug.LogError("Null destination passed to launch", null);
		}
		if (this.CheckReadyToLaunch())
		{
			this.launchable.Trigger(-1056989049, null);
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
			spacecraftFromLaunchConditionManager.SetState(Spacecraft.MissionState.Underway);
			SpacecraftManager.instance.savedSpacecraftDestinations[spacecraftFromLaunchConditionManager.id] = destination.id;
		}
	}

	public bool CheckReadyToLaunch()
	{
		foreach (RocketLaunchCondition rocketLaunchCondition in this.conditions)
		{
			if (!rocketLaunchCondition.EvaluateLaunchCondition())
			{
				return false;
			}
		}
		return true;
	}

	public string GetStatusReport()
	{
		if (this.CheckReadyToLaunch())
		{
			return UI.STARMAP.MISSION_STATUS.GO;
		}
		return this.FailedConditionStatusReport();
	}

	private string AllConditionStatusReport()
	{
		string text = string.Empty;
		foreach (RocketLaunchCondition rocketLaunchCondition in this.conditions)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text += "\n";
			}
			text += rocketLaunchCondition.GetLaunchStatusMessage(rocketLaunchCondition.EvaluateLaunchCondition());
		}
		return text;
	}

	private string FailedConditionStatusReport()
	{
		string text = string.Empty;
		foreach (RocketLaunchCondition rocketLaunchCondition in this.conditions)
		{
			if (!rocketLaunchCondition.EvaluateLaunchCondition())
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "\n";
				}
				text += rocketLaunchCondition.GetLaunchStatusMessage(rocketLaunchCondition.EvaluateLaunchCondition());
			}
		}
		return text;
	}

	public void EvaluateConditions()
	{
	}

	private LaunchableRocket launchable;

	private SchedulerHandle updateHandle;
}
