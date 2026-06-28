using System;
using System.Collections.Generic;
using UnityEngine;

public class ResearchPointInventory
{
	public ResearchPointInventory()
	{
		foreach (ResearchType researchType in Research.Instance.researchTypes.Types)
		{
			this.PointsByTypeID.Add(researchType.id, 0f);
		}
	}

	public void AddResearchPoints(string researchTypeID, float points)
	{
		if (!this.PointsByTypeID.ContainsKey(researchTypeID))
		{
			Debug.LogWarning("Research inventory is missing research point key " + researchTypeID);
			return;
		}
		Dictionary<string, float> pointsByTypeID;
		Dictionary<string, float> dictionary = (pointsByTypeID = this.PointsByTypeID);
		float num = pointsByTypeID[researchTypeID];
		dictionary[researchTypeID] = num + points;
	}

	public void RemoveResearchPoints(string researchTypeID, float points)
	{
		this.AddResearchPoints(researchTypeID, -points);
	}

	public Dictionary<string, float> PointsByTypeID = new Dictionary<string, float>();
}
