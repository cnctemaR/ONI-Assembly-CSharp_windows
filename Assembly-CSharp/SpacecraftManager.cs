using System;
using System.Collections.Generic;
using Database;
using KSerialization;
using STRINGS;
using TUNING;

[SerializationConfig(MemberSerialization.OptIn)]
public class SpacecraftManager : KMonoBehaviour, ISim1000ms
{
	public static void DestroyInstance()
	{
		SpacecraftManager.instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SpacecraftManager.instance = this;
		SpaceDestinationTypes spaceDestinationTypes = Db.Get().SpaceDestinationTypes;
		if (this.savedSpacecraftDestinations == null)
		{
			this.savedSpacecraftDestinations = new Dictionary<int, int>();
		}
		if (this.destinations == null)
		{
			this.destinations = new List<SpaceDestination>
			{
				new SpaceDestination(0, spaceDestinationTypes.CarbonaceousAsteroid.Id, 0),
				new SpaceDestination(1, spaceDestinationTypes.CarbonaceousAsteroid.Id, 0),
				new SpaceDestination(2, spaceDestinationTypes.MetallicAsteroid.Id, 1),
				new SpaceDestination(3, spaceDestinationTypes.RockyAsteroid.Id, 2),
				new SpaceDestination(4, spaceDestinationTypes.IcyDwarf.Id, 3),
				new SpaceDestination(5, spaceDestinationTypes.OrganicDwarf.Id, 4)
			};
		}
	}

	private void GenerateRandomDestinations()
	{
		Random random = new Random(SaveLoader.Instance.worldDetailSave.globalWorldSeed);
		SpaceDestinationTypes spaceDestinationTypes = Db.Get().SpaceDestinationTypes;
		List<List<string>> list = new List<List<string>>
		{
			new List<string>(),
			new List<string>(),
			new List<string> { spaceDestinationTypes.Satellite.Id },
			new List<string>
			{
				spaceDestinationTypes.Satellite.Id,
				spaceDestinationTypes.MetallicAsteroid.Id,
				spaceDestinationTypes.RockyAsteroid.Id,
				spaceDestinationTypes.CarbonaceousAsteroid.Id
			},
			new List<string>
			{
				spaceDestinationTypes.MetallicAsteroid.Id,
				spaceDestinationTypes.RockyAsteroid.Id,
				spaceDestinationTypes.CarbonaceousAsteroid.Id
			},
			new List<string>
			{
				spaceDestinationTypes.MetallicAsteroid.Id,
				spaceDestinationTypes.RockyAsteroid.Id,
				spaceDestinationTypes.CarbonaceousAsteroid.Id,
				spaceDestinationTypes.IcyDwarf.Id,
				spaceDestinationTypes.OrganicDwarf.Id
			},
			new List<string>
			{
				spaceDestinationTypes.IcyDwarf.Id,
				spaceDestinationTypes.OrganicDwarf.Id,
				spaceDestinationTypes.DustyMoon.Id
			},
			new List<string>
			{
				spaceDestinationTypes.IcyDwarf.Id,
				spaceDestinationTypes.OrganicDwarf.Id,
				spaceDestinationTypes.DustyMoon.Id
			},
			new List<string>
			{
				spaceDestinationTypes.DustyMoon.Id,
				spaceDestinationTypes.TerraPlanet.Id,
				spaceDestinationTypes.VolcanoPlanet.Id
			},
			new List<string>
			{
				spaceDestinationTypes.TerraPlanet.Id,
				spaceDestinationTypes.VolcanoPlanet.Id,
				spaceDestinationTypes.GasGiant.Id,
				spaceDestinationTypes.IceGiant.Id
			},
			new List<string>
			{
				spaceDestinationTypes.GasGiant.Id,
				spaceDestinationTypes.IceGiant.Id
			}
		};
		List<int> list2 = new List<int>();
		int num = 3;
		int num2 = 10;
		int num3 = 20;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Count != 0)
			{
				for (int j = 0; j < num; j++)
				{
					list2.Add(i);
				}
			}
		}
		int num4 = random.Next(num2, num3);
		for (int k = 0; k < num4; k++)
		{
			int num5 = random.Next(0, list2.Count - 1);
			int num6 = list2[num5];
			list2.RemoveAt(num5);
			List<string> list3 = list[num6];
			string text = list3[random.Next(0, list3.Count)];
			SpaceDestination spaceDestination = new SpaceDestination(this.destinations.Count, text, num6);
			this.destinations.Add(spaceDestination);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.spacecraftManager = this;
		if (!this.destinationsGenerated)
		{
			this.GenerateRandomDestinations();
			this.destinations.Sort((SpaceDestination a, SpaceDestination b) => a.distance.CompareTo(b.distance));
			List<float> list = new List<float>();
			for (int i = 0; i < 10; i++)
			{
				list.Add((float)i / 10f);
			}
			for (int j = 0; j < 20; j++)
			{
				list.Shuffle<float>();
				int num = 0;
				foreach (SpaceDestination spaceDestination in this.destinations)
				{
					if (spaceDestination.distance == j)
					{
						num++;
						spaceDestination.startingOrbitPercentage = list[num];
					}
				}
			}
			this.destinationsGenerated = true;
		}
	}

	public SpaceDestination GetSpacecraftDestination(LaunchConditionManager lcm)
	{
		Spacecraft spacecraftFromLaunchConditionManager = this.GetSpacecraftFromLaunchConditionManager(lcm);
		return this.GetSpacecraftDestination(spacecraftFromLaunchConditionManager.id);
	}

	public SpaceDestination GetSpacecraftDestination(int spacecraftID)
	{
		this.CleanSavedSpacecraftDestinations();
		if (this.savedSpacecraftDestinations.ContainsKey(spacecraftID))
		{
			return this.GetDestination(this.savedSpacecraftDestinations[spacecraftID]);
		}
		return null;
	}

	public List<int> GetSpacecraftsForDestination(SpaceDestination destination)
	{
		this.CleanSavedSpacecraftDestinations();
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, int> keyValuePair in this.savedSpacecraftDestinations)
		{
			if (keyValuePair.Value == destination.id)
			{
				list.Add(keyValuePair.Key);
			}
		}
		return list;
	}

	private void CleanSavedSpacecraftDestinations()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, int> keyValuePair in this.savedSpacecraftDestinations)
		{
			bool flag = false;
			foreach (Spacecraft spacecraft in this.spacecraft)
			{
				if (spacecraft.id == keyValuePair.Key)
				{
					flag = true;
					break;
				}
			}
			bool flag2 = false;
			foreach (SpaceDestination spaceDestination in this.destinations)
			{
				if (spaceDestination.id == keyValuePair.Value)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag || !flag2)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (int num in list)
		{
			this.savedSpacecraftDestinations.Remove(num);
		}
	}

	public void SetSpacecraftDestination(LaunchConditionManager lcm, SpaceDestination destination)
	{
		Spacecraft spacecraftFromLaunchConditionManager = this.GetSpacecraftFromLaunchConditionManager(lcm);
		this.savedSpacecraftDestinations[spacecraftFromLaunchConditionManager.id] = destination.id;
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
		Debug.LogErrorFormat("No space destination with ID {0}", new object[] { destinationID });
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
		Spacecraft spacecraftFromLaunchConditionManager = this.GetSpacecraftFromLaunchConditionManager(conditionManager);
		spacecraftFromLaunchConditionManager.SetState(Spacecraft.MissionState.Destroyed);
		this.spacecraft.Remove(spacecraftFromLaunchConditionManager);
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

	public void Sim1000ms(float dt)
	{
		foreach (Spacecraft spacecraft in this.spacecraft)
		{
			spacecraft.ProgressMission(dt);
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

	public void EarnDestinationAnalysisPoints(int destinationID, float points)
	{
		if (!this.destinationAnalysisScores.ContainsKey(destinationID))
		{
			this.destinationAnalysisScores.Add(destinationID, 0f);
		}
		SpaceDestination destination = this.GetDestination(destinationID);
		SpacecraftManager.DestinationAnalysisState destinationAnalysisState = this.GetDestinationAnalysisState(destination);
		Dictionary<int, float> dictionary;
		(dictionary = this.destinationAnalysisScores)[destinationID] = dictionary[destinationID] + points;
		SpacecraftManager.DestinationAnalysisState destinationAnalysisState2 = this.GetDestinationAnalysisState(destination);
		if (destinationAnalysisState != destinationAnalysisState2)
		{
			int starmapAnalysisDestinationID = SpacecraftManager.instance.GetStarmapAnalysisDestinationID();
			if (starmapAnalysisDestinationID == destinationID)
			{
				if (destinationAnalysisState2 == SpacecraftManager.DestinationAnalysisState.Complete)
				{
					SpacecraftManager.instance.SetStarmapAnalysisDestinationID(-1);
				}
				base.Trigger(532901469, null);
			}
		}
	}

	public SpacecraftManager.DestinationAnalysisState GetDestinationAnalysisState(SpaceDestination destination)
	{
		if (destination.startAnalyzed)
		{
			return SpacecraftManager.DestinationAnalysisState.Complete;
		}
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

	public void SetStarmapAnalysisDestinationID(int id)
	{
		this.analyzeDestinationID = id;
		base.Trigger(532901469, id);
	}

	public int GetStarmapAnalysisDestinationID()
	{
		return this.analyzeDestinationID;
	}

	public bool HasAnalysisTarget()
	{
		return this.analyzeDestinationID != -1;
	}

	public static SpacecraftManager instance;

	[Serialize]
	private List<Spacecraft> spacecraft = new List<Spacecraft>();

	[Serialize]
	public List<SpaceDestination> destinations;

	[Serialize]
	public Dictionary<int, int> savedSpacecraftDestinations;

	[Serialize]
	private int nextSpacecraftID;

	[Serialize]
	public bool destinationsGenerated;

	public const int INVALID_DESTINATION_ID = -1;

	[Serialize]
	private int analyzeDestinationID = -1;

	[Serialize]
	public Dictionary<int, float> destinationAnalysisScores = new Dictionary<int, float>();

	public enum DestinationAnalysisState
	{
		Hidden,
		Discovered,
		Complete
	}
}
