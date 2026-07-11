using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
		Dictionary<string, float> pointsByTypeID = this.PointsByTypeID;
		pointsByTypeID[researchTypeID] += points;
	}

	public void RemoveResearchPoints(string researchTypeID, float points)
	{
		this.AddResearchPoints(researchTypeID, -points);
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		foreach (ResearchType researchType in Research.Instance.researchTypes.Types)
		{
			if (!this.PointsByTypeID.ContainsKey(researchType.id))
			{
				this.PointsByTypeID.Add(researchType.id, 0f);
			}
		}
	}

	public Dictionary<string, float> PointsByTypeID = new Dictionary<string, float>();
}
