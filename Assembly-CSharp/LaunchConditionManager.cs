using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LaunchConditionManager")]
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
			rocketModule.PopulateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep, list);
			rocketModule.PopulateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage, list);
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
		List<ProcessCondition> list;
		using (ProcessCondition.ListPool.Get(out list))
		{
			foreach (RocketModule rocketModule in this.rocketModules)
			{
				rocketModule.PopulateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep, list);
				using (List<ProcessCondition>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.EvaluateCondition() == ProcessCondition.Status.Failure)
						{
							return false;
						}
					}
				}
				list.Clear();
				rocketModule.PopulateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage, list);
				using (List<ProcessCondition>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.EvaluateCondition() == ProcessCondition.Status.Failure)
						{
							return false;
						}
					}
				}
				list.Clear();
			}
		}
		return true;
	}

	public bool CheckAbleToFly()
	{
		List<ProcessCondition> list;
		using (ProcessCondition.ListPool.Get(out list))
		{
			foreach (RocketModule rocketModule in this.rocketModules)
			{
				rocketModule.PopulateConditionSet(ProcessCondition.ProcessConditionType.RocketFlight, list);
				using (List<ProcessCondition>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.EvaluateCondition() == ProcessCondition.Status.Failure)
						{
							return false;
						}
					}
				}
				list.Clear();
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
			List<ProcessCondition> list;
			using (ProcessCondition.ListPool.Get(out list))
			{
				using (List<RocketModule>.Enumerator enumerator = this.rocketModules.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RocketModule rocketModule = enumerator.Current;
						rocketModule.PopulateConditionSet(ProcessCondition.ProcessConditionType.RocketFlight, list);
						foreach (ProcessCondition processCondition in list)
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
						list.Clear();
					}
					return;
				}
			}
		}
		this.ClearFlightStatuses();
		component.SendSignal(this.statusPort, 0);
	}

	public HashedString triggerPort;

	public HashedString statusPort;

	private ILaunchableRocket launchable;

	private Dictionary<ProcessCondition, Guid> conditionStatuses = new Dictionary<ProcessCondition, Guid>();

	public enum ConditionType
	{
		Launch,
		Flight
	}
}
