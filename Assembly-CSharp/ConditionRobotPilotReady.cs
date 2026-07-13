using System;
using STRINGS;
using UnityEngine;

public class ConditionRobotPilotReady : ProcessCondition
{
	public ConditionRobotPilotReady(RoboPilotModule module)
	{
		this.module = module;
		this.craftRegisterType = module.GetComponent<ILaunchableRocket>().registerType;
		if (this.craftRegisterType == LaunchableRocketRegisterType.Clustercraft)
		{
			this.craftInterface = module.GetComponent<RocketModuleCluster>().CraftInterface;
		}
	}

	public override ProcessCondition.Status EvaluateCondition()
	{
		ProcessCondition.Status status = ProcessCondition.Status.Failure;
		LaunchableRocketRegisterType launchableRocketRegisterType = this.craftRegisterType;
		if (launchableRocketRegisterType != LaunchableRocketRegisterType.Spacecraft)
		{
			if (launchableRocketRegisterType == LaunchableRocketRegisterType.Clustercraft && this.HasDestination())
			{
				global::UnityEngine.Object component = this.craftInterface.GetComponent<Clustercraft>();
				ClusterTraveler component2 = this.craftInterface.GetComponent<ClusterTraveler>();
				if (component == null || component2 == null || component2.CurrentPath == null)
				{
					return ProcessCondition.Status.Failure;
				}
				int num = component2.RemainingTravelNodes();
				bool flag = this.module.HasResourcesToMove(num * 2);
				bool flag2 = this.module.HasResourcesToMove(num);
				if (flag)
				{
					status = ProcessCondition.Status.Ready;
				}
				else if (flag2 || this.RocketHasDupeControlStation())
				{
					status = ProcessCondition.Status.Warning;
				}
			}
		}
		else
		{
			int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.module.GetComponent<LaunchConditionManager>()).id;
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
			if (spacecraftDestination == null)
			{
				status = ProcessCondition.Status.Failure;
			}
			else if (this.module.HasResourcesToMove(spacecraftDestination.OneBasedDistance * 2))
			{
				status = ProcessCondition.Status.Ready;
			}
			else
			{
				status = ProcessCondition.Status.Failure;
			}
		}
		return status;
	}

	private bool HasDestination()
	{
		if (this.craftRegisterType == LaunchableRocketRegisterType.Clustercraft)
		{
			return !this.module.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<RocketClusterDestinationSelector>().IsAtDestination();
		}
		if (this.craftRegisterType == LaunchableRocketRegisterType.Spacecraft)
		{
			int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.module.GetComponent<LaunchConditionManager>()).id;
			return SpacecraftManager.instance.GetSpacecraftDestination(id) != null;
		}
		return false;
	}

	private bool RocketHasDupeControlStation()
	{
		if (this.craftInterface != null)
		{
			PassengerRocketModule passengerModule = this.craftInterface.GetPassengerModule();
			if (passengerModule != null)
			{
				return passengerModule.CheckPilotBoarded();
			}
		}
		return false;
	}

	public override string GetStatusMessage(ProcessCondition.Status status)
	{
		if (status == ProcessCondition.Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.STATUS.READY;
		}
		if (status != ProcessCondition.Status.Warning)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.STATUS.FAILURE;
		}
		if (this.RocketHasDupeControlStation())
		{
			return UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.STATUS.WARNING_NO_DATA_BANKS_HUMAN_PILOT;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.STATUS.WARNING;
	}

	public override string GetStatusTooltip(ProcessCondition.Status status)
	{
		LaunchableRocketRegisterType launchableRocketRegisterType = this.craftRegisterType;
		if (launchableRocketRegisterType != LaunchableRocketRegisterType.Spacecraft)
		{
			if (launchableRocketRegisterType == LaunchableRocketRegisterType.Clustercraft)
			{
				ClusterTraveler component = this.craftInterface.GetComponent<ClusterTraveler>();
				if (status == ProcessCondition.Status.Ready)
				{
					if (this.craftInterface.GetClusterDestinationSelector().IsAtDestination())
					{
						return string.Format(UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.READY_NO_DESTINATION, this.module.GetDataBanksStored());
					}
					int num = component.RemainingTravelNodes() * 2 * this.module.dataBankConsumption;
					return string.Format(UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.READY, this.module.GetDataBanksStored(), num);
				}
				else if (status == ProcessCondition.Status.Warning)
				{
					if (this.RocketHasDupeControlStation())
					{
						return UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.WARNING_NO_DATA_BANKS_HUMAN_PILOT;
					}
					if (component == null || component.CurrentPath == null)
					{
						return UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.FAILURE_NO_DESTINATION;
					}
					int num2 = component.RemainingTravelNodes() * 2 * this.module.dataBankConsumption;
					return string.Format(UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.WARNING, this.module.GetDataBanksStored(), num2);
				}
				else
				{
					if (this.HasDestination() && !(component == null) && component.CurrentPath != null)
					{
						int num3 = component.RemainingTravelNodes();
						return string.Format(UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.FAILURE, num3 * this.module.dataBankConsumption, this.module.GetDataBanksStored());
					}
					if (this.module.IsFull())
					{
						return string.Format(UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.READY_NO_DESTINATION, this.module.GetDataBanksStored());
					}
					return UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.FAILURE_NO_DESTINATION;
				}
			}
		}
		else
		{
			int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.module.GetComponent<LaunchConditionManager>()).id;
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
			if (status == ProcessCondition.Status.Ready)
			{
				int num4 = spacecraftDestination.OneBasedDistance * 2;
				return string.Format(UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.READY, this.module.GetDataBanksStored(), num4);
			}
			if (status == ProcessCondition.Status.Warning)
			{
				if (spacecraftDestination != null)
				{
					int num5 = spacecraftDestination.OneBasedDistance * 2;
					return string.Format(UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.WARNING, this.module.GetDataBanksStored(), num5);
				}
			}
			else
			{
				if (spacecraftDestination != null)
				{
					return string.Format(UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.FAILURE, spacecraftDestination.OneBasedDistance * 2, this.module.GetDataBanksStored());
				}
				if (this.module.IsFull())
				{
					return string.Format(UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.READY_NO_DESTINATION, this.module.GetDataBanksStored());
				}
				return UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.FAILURE_NO_DESTINATION;
			}
		}
		DebugUtil.DevAssert(false, "Rocket type " + this.craftRegisterType.ToString() + " does not have a status tooltip for " + status.ToString(), null);
		return UI.STARMAP.LAUNCHCHECKLIST.ROBOT_PILOT_DATA_REQUIREMENTS.TOOLTIP.FAILURE_NO_DESTINATION;
	}

	public override bool ShowInUI()
	{
		return true;
	}

	private LaunchableRocketRegisterType craftRegisterType;

	private RoboPilotModule module;

	private CraftModuleInterface craftInterface;
}
