using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SpacecraftManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SpacecraftManager.instance = this;
		if (this.savedSpacecraftDestinations == null)
		{
			this.savedSpacecraftDestinations = new Dictionary<int, int>();
		}
		this.destinations = new List<SpaceDestination>
		{
			new CarbonaceousAsteroid(0, 1, 0.2f, ROCKETRY.DESTINATION_THRUST_COSTS.LOW),
			new MetallicAsteroid(1, 2, 0.4f, ROCKETRY.DESTINATION_THRUST_COSTS.LOW),
			new RockyAsteroid(2, 2, 0.7f, ROCKETRY.DESTINATION_THRUST_COSTS.LOW),
			new IcyDwarf(3, 3, 0.3f, ROCKETRY.DESTINATION_THRUST_COSTS.MID),
			new OrganicDwarf(4, 4, 0.1f, ROCKETRY.DESTINATION_THRUST_COSTS.HIGH)
		};
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.spacecraftManager = this;
	}

	public SpaceDestination GetActiveMission(int spacecraftID)
	{
		if (this.savedSpacecraftDestinations.ContainsKey(spacecraftID))
		{
			return this.GetDestination(this.savedSpacecraftDestinations[spacecraftID]);
		}
		return null;
	}

	public int GetSpacecraftID(LaunchableRocket rocket)
	{
		foreach (Spacecraft spacecraft in this.spacecraft)
		{
			if (spacecraft.launchConditions.gameObject == rocket.gameObject)
			{
				return spacecraft.id;
			}
		}
		return -1;
	}

	public SpaceDestination GetDestination(int destinationID)
	{
		foreach (SpaceDestination spaceDestination in this.destinations)
		{
			if (spaceDestination.id == destinationID)
			{
				return spaceDestination;
			}
		}
		global::Debug.LogErrorFormat("No space destination with ID {0}", new object[] { destinationID });
		return null;
	}

	public void RegisterSpacecraft(Spacecraft craft)
	{
		if (this.spacecraft.Contains(craft))
		{
			return;
		}
		if (craft.HasInvalidID())
		{
			craft.SetID(this.nextSpacecraftID);
			this.nextSpacecraftID++;
		}
		this.spacecraft.Add(craft);
	}

	public void UnregisterSpacecraft(LaunchConditionManager conditionManager)
	{
		this.spacecraft.Remove(this.GetSpacecraftFromLaunchConditionManager(conditionManager));
	}

	public List<Spacecraft> GetSpacecraft()
	{
		return this.spacecraft;
	}

	public Spacecraft GetSpacecraftFromLaunchConditionManager(LaunchConditionManager lcm)
	{
		foreach (Spacecraft spacecraft in this.spacecraft)
		{
			if (spacecraft.launchConditions == lcm)
			{
				return spacecraft;
			}
		}
		return null;
	}

	public void Update()
	{
		foreach (Spacecraft spacecraft in this.spacecraft)
		{
			spacecraft.ProgressMission(Time.deltaTime);
		}
	}

	public void PushReadyToLandNotification(Spacecraft spacecraft)
	{
		Notification notification = new Notification(BUILDING.STATUSITEMS.SPACECRAFTREADYTOLAND.NOTIFICATION, NotificationType.Good, HashedString.Invalid, (List<Notification> notificationList, object data) => BUILDING.STATUSITEMS.SPACECRAFTREADYTOLAND.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), spacecraft.launchConditions.GetProperName(), false, 0f, null, null);
		base.gameObject.AddOrGet<Notifier>().Add(notification, string.Empty);
	}

	private void SpawnMissionResults(Dictionary<SimHashes, float> results)
	{
		foreach (KeyValuePair<SimHashes, float> keyValuePair in results)
		{
			ElementLoader.FindElementByHash(keyValuePair.Key).substance.SpawnResource(PlayerController.GetCursorPos(KInputManager.GetMousePos()), keyValuePair.Value, 300f, 0, 0, false, false);
		}
	}

	public float GetDestinationAnalysisScore(SpaceDestination destination)
	{
		return this.GetDestinationAnalysisScore(destination.id);
	}

	public float GetDestinationAnalysisScore(int destinationID)
	{
		if (this.destinationAnalysisScores.ContainsKey(destinationID))
		{
			return this.destinationAnalysisScores[destinationID];
		}
		return 0f;
	}

	public void EarnDestinationAnalysisPoints(SpaceDestination destination, float points)
	{
		this.EarnDestinationAnalysisPoints(destination.id, points);
	}

	public void EarnDestinationAnalysisPoints(int destinationID, float points)
	{
		if (!this.destinationAnalysisScores.ContainsKey(destinationID))
		{
			this.destinationAnalysisScores.Add(destinationID, 0f);
		}
		Dictionary<int, float> dictionary;
		(dictionary = this.destinationAnalysisScores)[destinationID] = dictionary[destinationID] + points;
	}

	public SpacecraftManager.DestinationAnalysisState GetDestinationAnalysisState(SpaceDestination destination)
	{
		float destinationAnalysisScore = this.GetDestinationAnalysisScore(destination);
		if (destinationAnalysisScore >= (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE)
		{
			return SpacecraftManager.DestinationAnalysisState.Complete;
		}
		if (destinationAnalysisScore >= (float)ROCKETRY.DESTINATION_ANALYSIS.DISCOVERED)
		{
			return SpacecraftManager.DestinationAnalysisState.Discovered;
		}
		return SpacecraftManager.DestinationAnalysisState.Hidden;
	}

	public static SpacecraftManager instance;

	[Serialize]
	private List<Spacecraft> spacecraft = new List<Spacecraft>();

	public List<SpaceDestination> destinations = new List<SpaceDestination>();

	[Serialize]
	public Dictionary<int, int> savedSpacecraftDestinations;

	[Serialize]
	private int nextSpacecraftID;

	[Serialize]
	public Dictionary<int, float> destinationAnalysisScores = new Dictionary<int, float>();

	public enum DestinationAnalysisState
	{
		Hidden,
		Discovered,
		Complete
	}
}
