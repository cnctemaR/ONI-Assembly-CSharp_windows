using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LaunchConditionManager")]
public class LaunchConditionManager : KMonoBehaviour, ISim4000ms, ISim1000ms
{
	public List<RocketModule> rocketModules { get; private set; }

	public void DEBUG_TraceModuleDestruction(string moduleName, string state, string stackTrace)
	{
		if (this.DEBUG_ModuleDestructions == null)
		{
			this.DEBUG_ModuleDestructions = new List<global::Tuple<string, string, string>>();
		}
		this.DEBUG_ModuleDestructions.Add(new global::Tuple<string, string, string>(moduleName, state, stackTrace));
	}

	[ContextMenu("Dump Module Destructions")]
	private void DEBUG_DumpModuleDestructions()
	{
		if (this.DEBUG_ModuleDestructions == null || this.DEBUG_ModuleDestructions.Count == 0)
		{
			DebugUtil.LogArgs(new object[] { "Sorry, no logged module destructions. :(" });
			return;
		}
		foreach (global::Tuple<string, string, string> tuple in this.DEBUG_ModuleDestructions)
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
		this.launchable = base.GetComponent<ILaunchableRocket>();
		this.FindModules();
		base.GetComponent<AttachableBuilding>().onAttachmentNetworkChanged = delegate(object data)
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
		global::Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(spacecraftFromLaunchConditionManager.id);
		if (base.gameObject.GetComponent<LogicPorts>().GetInputValue(this.triggerPort) == 1 && spacecraftDestination != null && spacecraftDestination.id != -1)
		{
			this.Launch(spacecraftDestination);
		}
	}

	public void FindModules()
	{
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>()))
		{
			RocketModule component = gameObject.GetComponent<RocketModule>();
			if (component != null && component.conditionManager == null)
			{
				component.conditionManager = this;
				component.RegisterWithConditionManager();
			}
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

	public List<ProcessCondition> GetLaunchConditionList()
	{
		List<ProcessCondition> list = new List<ProcessCondition>();
		foreach (RocketModule rocketModule in this.rocketModules)
		{
			foreach (ProcessCondition processCondition in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketPrep))
			{
				list.Add(processCondition);
			}
			foreach (ProcessCondition processCondition2 in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketStorage))
			{
				list.Add(processCondition2);
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
		if (SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this).state != Spacecraft.MissionState.Grounded)
		{
			return;
		}
		if (DebugHandler.InstantBuildMode || (this.CheckReadyToLaunch() && this.CheckAbleToFly()))
		{
			this.launchable.LaunchableGameObject.Trigger(705820818, null);
			SpacecraftManager.instance.SetSpacecraftDestination(this, destination);
			SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this).BeginMission(destination);
		}
	}

	public bool CheckReadyToLaunch()
	{
		foreach (RocketModule rocketModule in this.rocketModules)
		{
			using (List<ProcessCondition>.Enumerator enumerator2 = rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketPrep).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.EvaluateCondition() == ProcessCondition.Status.Failure)
					{
						return false;
					}
				}
			}
			using (List<ProcessCondition>.Enumerator enumerator2 = rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketStorage).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.EvaluateCondition() == ProcessCondition.Status.Failure)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public bool CheckAbleToFly()
	{
		foreach (RocketModule rocketModule in this.rocketModules)
		{
			using (List<ProcessCondition>.Enumerator enumerator2 = rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketFlight).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.EvaluateCondition() == ProcessCondition.Status.Failure)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private void ClearFlightStatuses()
	{
		KSelectable component = base.GetComponent<KSelectable>();
		foreach (KeyValuePair<ProcessCondition, Guid> keyValuePair in this.conditionStatuses)
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
			using (List<RocketModule>.Enumerator enumerator = this.rocketModules.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					RocketModule rocketModule = enumerator.Current;
					foreach (ProcessCondition processCondition in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketFlight))
					{
						if (processCondition.EvaluateCondition() == ProcessCondition.Status.Failure)
						{
							if (!this.conditionStatuses.ContainsKey(processCondition))
							{
								StatusItem statusItem = processCondition.GetStatusItem(ProcessCondition.Status.Failure);
								this.conditionStatuses[processCondition] = component2.AddStatusItem(statusItem, processCondition);
							}
						}
						else if (this.conditionStatuses.ContainsKey(processCondition))
						{
							component2.RemoveStatusItem(this.conditionStatuses[processCondition], false);
							this.conditionStatuses.Remove(processCondition);
						}
					}
				}
				return;
			}
		}
		this.ClearFlightStatuses();
		component.SendSignal(this.statusPort, 0);
	}

	public HashedString triggerPort;

	public HashedString statusPort;

	private ILaunchableRocket launchable;

	[Serialize]
	private List<global::Tuple<string, string, string>> DEBUG_ModuleDestructions;

	private Dictionary<ProcessCondition, Guid> conditionStatuses = new Dictionary<ProcessCondition, Guid>();

	public enum ConditionType
	{
		Launch,
		Flight
	}
}
