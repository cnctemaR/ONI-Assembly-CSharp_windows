using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class LaunchConditionManager : KMonoBehaviour, ISim4000ms, ISim1000ms
{
	public List<RocketModule> rocketModules { get; private set; }

	public void DEBUG_TraceModuleDestruction(string moduleName, string state, string stackTrace)
	{
		if (this.DEBUG_ModuleDestructions == null)
		{
			this.DEBUG_ModuleDestructions = new List<Tuple<string, string, string>>();
		}
		this.DEBUG_ModuleDestructions.Add(new Tuple<string, string, string>(moduleName, state, stackTrace));
	}

	[ContextMenu("Dump Module Destructions")]
	private void DEBUG_DumpModuleDestructions()
	{
		if (this.DEBUG_ModuleDestructions == null || this.DEBUG_ModuleDestructions.Count == 0)
		{
			DebugUtil.LogArgs(new object[] { "Sorry, no logged module destructions. :(" });
			return;
		}
		foreach (Tuple<string, string, string> tuple in this.DEBUG_ModuleDestructions)
		{
			DebugUtil.LogArgs(new object[] { tuple.first, ">", tuple.second, "\n", tuple.third, "\nEND MODULE DUMP\n\n" });
		}
	}

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
		base.Subscribe(-1582839653, new Action<object>(this.OnTagsChanged));
	}

	private void OnTagsChanged(object data)
	{
		foreach (RocketModule rocketModule in this.rocketModules)
		{
			rocketModule.OnConditionManagerTagsChanged(data);
		}
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
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(spacecraftFromLaunchConditionManager.id);
		LogicPorts component = base.gameObject.GetComponent<LogicPorts>();
		if (component.GetInputValue(this.triggerPort) == 1 && spacecraftDestination != null && spacecraftDestination.id != -1)
		{
			this.Launch(spacecraftDestination);
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
			global::Debug.LogError("Null destination passed to launch");
		}
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
		if (spacecraftFromLaunchConditionManager.state != Spacecraft.MissionState.Grounded)
		{
			return;
		}
		if (DebugHandler.InstantBuildMode || (this.CheckReadyToLaunch() && this.CheckAbleToFly()))
		{
			this.launchable.Trigger(-1056989049, null);
			SpacecraftManager.instance.SetSpacecraftDestination(this, destination);
			Spacecraft spacecraftFromLaunchConditionManager2 = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
			spacecraftFromLaunchConditionManager2.BeginMission(destination);
		}
	}

	public bool CheckReadyToLaunch()
	{
		foreach (RocketModule rocketModule in this.rocketModules)
		{
			foreach (RocketLaunchCondition rocketLaunchCondition in rocketModule.launchConditions)
			{
				if (rocketLaunchCondition.EvaluateLaunchCondition() == RocketLaunchCondition.LaunchStatus.Failure)
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
		if (flag)
		{
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
			if (spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Grounded || spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Launching)
			{
				component.SendSignal(this.statusPort, 1);
			}
			else
			{
				component.SendSignal(this.statusPort, 0);
			}
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
			component.SendSignal(this.statusPort, 0);
		}
	}

	public HashedString triggerPort;

	public HashedString statusPort;

	private LaunchableRocket launchable;

	[Serialize]
	private List<Tuple<string, string, string>> DEBUG_ModuleDestructions;

	private Dictionary<RocketFlightCondition, Guid> conditionStatuses = new Dictionary<RocketFlightCondition, Guid>();
}
