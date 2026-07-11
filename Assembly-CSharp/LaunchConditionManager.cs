using System;
using System.Collections.Generic;
using UnityEngine;

public class LaunchConditionManager : KMonoBehaviour, ISim4000ms, ISim1000ms
{
	public List<RocketModule> rocketModules { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.rocketModules = new List<RocketModule>();
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
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void Sim1000ms(float dt)
	{
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
		if (spacecraftFromLaunchConditionManager == null)
		{
			return;
		}
		SpaceDestination activeMission = SpacecraftManager.instance.GetActiveMission(spacecraftFromLaunchConditionManager.id);
		LogicPorts component = base.gameObject.GetComponent<LogicPorts>();
		if (component.GetInputValue(this.triggerPort) == 1 && activeMission != null && activeMission.id != -1)
		{
			this.Launch(activeMission);
		}
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
				component.RegisterWithConditionManager();
			}
		}
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
		if (spacecraftFromLaunchConditionManager != null)
		{
			spacecraftFromLaunchConditionManager.moduleCount = attachedNetwork.Count;
		}
	}

	public void RegisterRocketModule(RocketModule module)
	{
		if (!this.rocketModules.Contains(module))
		{
			this.rocketModules.Add(module);
		}
	}

	public void UnregisterRocketModule(RocketModule module)
	{
		this.rocketModules.Remove(module);
	}

	public List<RocketLaunchCondition> GetLaunchConditionList()
	{
		List<RocketLaunchCondition> list = new List<RocketLaunchCondition>();
		foreach (RocketModule rocketModule in this.rocketModules)
		{
			foreach (RocketLaunchCondition rocketLaunchCondition in rocketModule.launchConditions)
			{
				list.Add(rocketLaunchCondition);
			}
		}
		return list;
	}

	public void Launch(SpaceDestination destination)
	{
		if (destination == null)
		{
			global::Debug.LogError("Null destination passed to launch", null);
		}
		if (this.CheckReadyToLaunch() && this.CheckAbleToFly())
		{
			this.launchable.Trigger(-1056989049, null);
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
			spacecraftFromLaunchConditionManager.SetState(Spacecraft.MissionState.Underway);
			SpacecraftManager.instance.savedSpacecraftDestinations[spacecraftFromLaunchConditionManager.id] = destination.id;
		}
	}

	public bool CheckReadyToLaunch()
	{
		foreach (RocketModule rocketModule in this.rocketModules)
		{
			foreach (RocketLaunchCondition rocketLaunchCondition in rocketModule.launchConditions)
			{
				if (!rocketLaunchCondition.EvaluateLaunchCondition())
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool CheckAbleToFly()
	{
		foreach (RocketModule rocketModule in this.rocketModules)
		{
			foreach (RocketFlightCondition rocketFlightCondition in rocketModule.flightConditions)
			{
				if (!rocketFlightCondition.EvaluateFlightCondition())
				{
					return false;
				}
			}
		}
		return true;
	}

	private void ClearFlightStatuses()
	{
		KSelectable component = base.GetComponent<KSelectable>();
		foreach (KeyValuePair<RocketFlightCondition, Guid> keyValuePair in this.conditionStatuses)
		{
			component.RemoveStatusItem(keyValuePair.Value, false);
		}
		this.conditionStatuses.Clear();
	}

	public void Sim4000ms(float dt)
	{
		bool flag = this.CheckReadyToLaunch();
		LogicPorts component = base.gameObject.GetComponent<LogicPorts>();
		component.SendSignal(this.statusPort, (!flag) ? 0 : 1);
		if (flag)
		{
			KSelectable component2 = base.GetComponent<KSelectable>();
			foreach (RocketModule rocketModule in this.rocketModules)
			{
				foreach (RocketFlightCondition rocketFlightCondition in rocketModule.flightConditions)
				{
					if (!rocketFlightCondition.EvaluateFlightCondition())
					{
						if (!this.conditionStatuses.ContainsKey(rocketFlightCondition))
						{
							StatusItem failureStatusItem = rocketFlightCondition.GetFailureStatusItem();
							this.conditionStatuses[rocketFlightCondition] = component2.AddStatusItem(failureStatusItem, rocketFlightCondition);
						}
					}
					else if (this.conditionStatuses.ContainsKey(rocketFlightCondition))
					{
						component2.RemoveStatusItem(this.conditionStatuses[rocketFlightCondition], false);
						this.conditionStatuses.Remove(rocketFlightCondition);
					}
				}
			}
		}
		else
		{
			this.ClearFlightStatuses();
		}
	}

	public HashedString triggerPort;

	public HashedString statusPort;

	private LaunchableRocket launchable;

	private Dictionary<RocketFlightCondition, Guid> conditionStatuses = new Dictionary<RocketFlightCondition, Guid>();
}
