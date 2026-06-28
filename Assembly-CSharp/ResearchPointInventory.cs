using System;
using System.Collections.Generic;

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
			Debug.LogWarning("Research inventory is missing research point key " + researchTypeID, null);
			return;
		}
		Dictionary<string, float> pointsByTypeID;
		(pointsByTypeID = this.PointsByTypeID)[researchTypeID] = pointsByTypeID[researchTypeID] + points;
	}

	public void RemoveResearchPoints(string researchTypeID, float points)
	{
		this.AddResearchPoints(researchTypeID, -points);
	}

	public Dictionary<string, float> PointsByTypeID = new Dictionary<string, float>();
}
