using System;
using System.Collections.Generic;
using UnityEngine;

public class SpaceDestination
{
	public SpaceDestination(int id, int distance, float startPosition, int thrustCost)
	{
		this.id = id;
		this.distance = distance;
		this.startingOrbitPercentage = startPosition;
		this.GenerateMissions();
	}

	public float GetCurrentOrbitPercentage()
	{
		float num = 0.1f * Mathf.Pow((float)this.distance, 2f);
		float num2 = (float)GameClock.Instance.GetCycle() + GameClock.Instance.GetCurrentCycleAsPercentage();
		return (num2 + this.startingOrbitPercentage * num) % num / num;
	}

	public void GenerateSurfaceElements()
	{
		foreach (KeyValuePair<SimHashes, Tuple<float, float>> keyValuePair in this.elementTable)
		{
			this.recoverableElements.Add(keyValuePair.Key, global::UnityEngine.Random.Range(keyValuePair.Value.first, keyValuePair.Value.second));
		}
	}

	public void GenerateMissions()
	{
		bool flag = true;
		foreach (SpaceMission spaceMission in this.missions)
		{
			if (spaceMission.craft == null)
			{
				flag = false;
			}
		}
		if (flag)
		{
			this.missions.Add(new SpaceMission(this));
		}
	}

	public Dictionary<SimHashes, float> GetMissionResourceResult(float totalCargoSpace, bool solids = true, bool liquids = true, bool gasses = true)
	{
		Dictionary<SimHashes, float> dictionary = new Dictionary<SimHashes, float>();
		float num = 0f;
		foreach (KeyValuePair<SimHashes, float> keyValuePair in this.recoverableElements)
		{
			if ((ElementLoader.FindElementByHash(keyValuePair.Key).IsSolid && solids) || (ElementLoader.FindElementByHash(keyValuePair.Key).IsLiquid && liquids) || (ElementLoader.FindElementByHash(keyValuePair.Key).IsGas && gasses))
			{
				num += keyValuePair.Value;
			}
		}
		foreach (KeyValuePair<SimHashes, float> keyValuePair2 in this.recoverableElements)
		{
			if ((ElementLoader.FindElementByHash(keyValuePair2.Key).IsSolid && solids) || (ElementLoader.FindElementByHash(keyValuePair2.Key).IsLiquid && liquids) || (ElementLoader.FindElementByHash(keyValuePair2.Key).IsGas && gasses))
			{
				dictionary.Add(keyValuePair2.Key, totalCargoSpace * (keyValuePair2.Value / num));
			}
		}
		return dictionary;
	}

	public Dictionary<Tag, int> GetMissionEntityResult()
	{
		Dictionary<Tag, int> dictionary = new Dictionary<Tag, int>();
		foreach (KeyValuePair<string, int> keyValuePair in this.recoverableEntities)
		{
			dictionary.Add(keyValuePair.Key, keyValuePair.Value);
		}
		return dictionary;
	}

	public string name;

	public string typeName;

	public string description;

	public int id;

	public string spriteName;

	public int distance;

	public float activePeriod = 20f;

	public float inactivePeriod = 10f;

	public float startingOrbitPercentage = 0.5f;

	public int iconSize = 128;

	public Dictionary<SimHashes, float> recoverableElements = new Dictionary<SimHashes, float>();

	public Dictionary<string, int> recoverableEntities = new Dictionary<string, int>();

	public Dictionary<SimHashes, Tuple<float, float>> elementTable = new Dictionary<SimHashes, Tuple<float, float>>();

	public List<SpaceMission> missions = new List<SpaceMission>();
}
