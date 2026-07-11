using System;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Spacecraft
{
	public Spacecraft(LaunchConditionManager launchConditions)
	{
		this.launchConditions = launchConditions;
	}

	public LaunchConditionManager launchConditions
	{
		get
		{
			return this.refLaunchConditions.Get();
		}
		set
		{
			this.refLaunchConditions.Set(value);
		}
	}

	public void SetRocketName(string newName)
	{
		this.rocketName = newName;
		this.UpdateNameOnRocketModules();
	}

	public string GetRocketName()
	{
		return this.rocketName;
	}

	public void UpdateNameOnRocketModules()
	{
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.launchConditions.GetComponent<AttachableBuilding>()))
		{
			RocketModule component = gameObject.GetComponent<RocketModule>();
			if (component != null)
			{
				component.SetParentRocketName(this.rocketName);
			}
		}
	}

	public bool HasInvalidID()
	{
		return this.id == -1;
	}

	public void SetID(int id)
	{
		this.id = id;
	}

	public void SetState(Spacecraft.MissionState state)
	{
		this.state = state;
	}

	public void ForceComplete()
	{
		this.missionElapsed = this.missionDuration;
	}

	public void ProgressMission(float deltaTime)
	{
		if (this.state == Spacecraft.MissionState.Underway)
		{
			this.missionElapsed += deltaTime;
			if (this.missionElapsed > this.missionDuration)
			{
				this.CompleteMission();
			}
		}
	}

	public float GetTimeLeft()
	{
		return this.missionDuration - this.missionElapsed;
	}

	public float GetDuration()
	{
		return this.missionDuration;
	}

	public void SetMission(SpaceDestination destination)
	{
		if (!SpacecraftManager.instance.savedSpacecraftDestinations.ContainsKey(this.id))
		{
			SpacecraftManager.instance.savedSpacecraftDestinations.Add(this.id, destination.id);
		}
		else
		{
			SpacecraftManager.instance.savedSpacecraftDestinations[this.id] = destination.id;
		}
		this.missionElapsed = 0f;
		this.missionDuration = (float)destination.OneBasedDistance * ROCKETRY.MISSION_DURATION_SCALE;
	}

	private void CompleteMission()
	{
		SpacecraftManager.instance.PushReadyToLandNotification(this);
		this.state = Spacecraft.MissionState.WaitingToLand;
		this.Land();
	}

	private void ClearMission()
	{
		SpacecraftManager.instance.savedSpacecraftDestinations[this.id] = -1;
		this.missionElapsed = 0f;
		this.missionDuration = 0f;
	}

	private void Land()
	{
		this.launchConditions.Trigger(1366341636, SpacecraftManager.instance.GetActiveMission(this.id));
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.launchConditions.GetComponent<AttachableBuilding>()))
		{
			if (gameObject != this.launchConditions.gameObject)
			{
				gameObject.Trigger(1366341636, SpacecraftManager.instance.GetActiveMission(this.id));
			}
		}
	}

	[Serialize]
	public int id = -1;

	[Serialize]
	public string rocketName = UI.STARMAP.DEFAULT_NAME;

	[Serialize]
	public int moduleCount;

	[Serialize]
	public Ref<LaunchConditionManager> refLaunchConditions = new Ref<LaunchConditionManager>();

	[Serialize]
	public Spacecraft.MissionState state;

	[Serialize]
	private float missionElapsed;

	[Serialize]
	private float missionDuration;

	public enum MissionState
	{
		Grounded,
		Launching,
		Underway,
		WaitingToLand,
		Destroyed
	}
}
