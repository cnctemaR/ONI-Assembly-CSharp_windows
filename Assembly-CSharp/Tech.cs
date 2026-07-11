using System;
using System.Collections.Generic;
using UnityEngine;

public class Tech : Resource
{
	public Vector2 center
	{
		get
		{
			return this.node.center;
		}
	}

	public float width
	{
		get
		{
			return this.node.width;
		}
	}

	public float height
	{
		get
		{
			return this.node.height;
		}
	}

	public List<ResourceTreeNode.Edge> edges
	{
		get
		{
			return this.node.edges;
		}
	}

	public Tech(string id, ResourceSet parent, string name, string desc, ResourceTreeNode node)
		: base(id, parent, name)
	{
		this.desc = desc;
		this.node = node;
	}

	public bool CanAfford(ResearchPointInventory pointInventory)
	{
		foreach (KeyValuePair<string, float> keyValuePair in this.costsByResearchTypeID)
		{
			if (pointInventory.PointsByTypeID[keyValuePair.Key] < keyValuePair.Value)
			{
				return false;
			}
		}
		return true;
	}

	public string CostString(ResearchTypes types)
	{
		string text = "";
		foreach (KeyValuePair<string, float> keyValuePair in this.costsByResearchTypeID)
		{
			text += string.Format("{0}:{1}", types.GetResearchType(keyValuePair.Key).name.ToString(), keyValuePair.Value.ToString());
			text += "\n";
		}
		return text;
	}

	public bool IsComplete()
	{
		if (Research.Instance != null)
		{
			TechInstance techInstance = Research.Instance.Get(this);
			return techInstance != null && techInstance.IsComplete();
		}
		return false;
	}

	public bool ArePrerequisitesComplete()
	{
		using (List<Tech>.Enumerator enumerator = this.requiredTech.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.IsComplete())
				{
					return false;
				}
			}
		}
		return true;
	}

	public List<Tech> requiredTech = new List<Tech>();

	public List<Tech> unlockedTech = new List<Tech>();

	public List<TechItem> unlockedItems = new List<TechItem>();

	public int tier;

	public Dictionary<string, float> costsByResearchTypeID = new Dictionary<string, float>();

	public string desc;

	private ResourceTreeNode node;
}
