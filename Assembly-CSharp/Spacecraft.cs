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

	public void BeginMission(SpaceDestination destination)
	{
		this.missionElapsed = 0f;
		this.missionDuration = (float)destination.OneBasedDistance * ROCKETRY.MISSION_DURATION_SCALE;
		this.SetState(Spacecraft.MissionState.Launching);
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

	private void CompleteMission()
	{
		SpacecraftManager.instance.PushReadyToLandNotification(this);
		this.SetState(Spacecraft.MissionState.WaitingToLand);
		this.Land();
	}

	private void Land()
	{
		this.launchConditions.Trigger(1366341636, SpacecraftManager.instance.GetSpacecraftDestination(this.id));
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.launchConditions.GetComponent<AttachableBuilding>()))
		{
			if (gameObject != this.launchConditions.gameObject)
			{
				gameObject.Trigger(1366341636, SpacecraftManager.instance.GetSpacecraftDestination(this.id));
			}
		}
	}

	public void GenerateName()
	{
		this.SetRocketName(GameUtil.GenerateRandomRocketName());
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
		Landing,
		Destroyed
	}
}
