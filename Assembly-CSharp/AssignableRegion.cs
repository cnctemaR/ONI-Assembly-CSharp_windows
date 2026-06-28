using System;
using System.Collections.Generic;
using UnityEngine;

public class AssignableRegion : KMonoBehaviour
{
	public Region Region
	{
		get
		{
			return this.region;
		}
	}

	protected override void OnPrefabInit()
	{
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.region.OnBuildingAdded += this.OnBuildingAddedToRegion;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (Game.IsQuitting())
		{
			return;
		}
		this.region.OnBuildingAdded -= this.OnBuildingAddedToRegion;
	}

	private void OnBuildingAddedToRegion(BuildingComplete buildingComplete)
	{
		if (buildingComplete == null)
		{
			Debug.LogError("Tried adding a null building to the region.");
			return;
		}
		Assignable component = buildingComplete.GetComponent<Assignable>();
		KPrefabID component2 = buildingComplete.GetComponent<KPrefabID>();
		if (component == null || component2 == null)
		{
			return;
		}
		this.assignablesInRegion.Add(component);
		this.AddToAssignablesMapByPrefabTag(component2, component);
		component.Assign(null);
	}

	public GameObject[] GetRequiredAssignables()
	{
		TagSet requiredBuildingsPrefabTags = this.region.GetRequiredBuildingsPrefabTags();
		List<GameObject> list = new List<GameObject>();
		foreach (Tag tag in requiredBuildingsPrefabTags)
		{
			GameObject prefab = Assets.GetPrefab(new Tag(tag));
			if (prefab != null && prefab.GetComponent<Assignable>() != null)
			{
				list.Add(prefab);
			}
		}
		return list.ToArray();
	}

	public string GetWarningMessage()
	{
		return this.region.GetMissingRequirementsString();
	}

	public void RemoveDuplicant(Assignables duplicant)
	{
		if (!this.assignedDuplicants.Contains(duplicant))
		{
			return;
		}
		this.assignedDuplicants.Remove(duplicant);
		this.PopulateAssignablesByPrefabTag();
		foreach (KeyValuePair<string, List<Assignable>> keyValuePair in this.assignablesByPrefabTag)
		{
			Assignable assignable = keyValuePair.Value.Find((Assignable assign) => assign.assignee == duplicant);
			if (assignable != null)
			{
				assignable.Assign(null);
			}
		}
	}

	public void AssignDuplicant(Assignables duplicant)
	{
		if (this.assignedDuplicants.Contains(duplicant))
		{
			Debug.LogError("Duplicant was already assigned to this region");
			return;
		}
		this.PopulateAssignablesByPrefabTag();
		foreach (KeyValuePair<string, List<Assignable>> keyValuePair in this.assignablesByPrefabTag)
		{
			Assignable assignable = keyValuePair.Value.Find((Assignable assign) => assign.assignee == null);
			if (assignable != null)
			{
				assignable.Assign(duplicant);
			}
		}
		this.assignedDuplicants.Add(duplicant);
	}

	private void PopulateAssignablesByPrefabTag()
	{
		this.assignablesByPrefabTag = new Dictionary<string, List<Assignable>>();
		this.assignablesInRegion = this.region.GetComponentsInOwnedBuildings<Assignable>(false);
		for (int i = 0; i < this.assignablesInRegion.Count; i++)
		{
			KPrefabID component = this.assignablesInRegion[i].GetComponent<KPrefabID>();
			if (!(component == null))
			{
				this.AddToAssignablesMapByPrefabTag(component, this.assignablesInRegion[i]);
			}
		}
	}

	private void RemoveFromAssignablesMapByPrefabTag(KPrefabID prefabID, Assignable asg)
	{
		if (!this.assignablesByPrefabTag.ContainsKey(prefabID.PrefabTag.Name))
		{
			Debug.LogError("The building removed was not contained in the region. That shouldn't happen.");
			return;
		}
		this.assignablesByPrefabTag[prefabID.PrefabTag.Name].Remove(asg);
	}

	private void AddToAssignablesMapByPrefabTag(KPrefabID prefabID, Assignable asg)
	{
		if (!this.assignablesByPrefabTag.ContainsKey(prefabID.PrefabTag.Name))
		{
			this.assignablesByPrefabTag.Add(prefabID.PrefabTag.Name, new List<Assignable>());
		}
		this.assignablesByPrefabTag[prefabID.PrefabTag.Name].Add(asg);
	}

	public bool ContainsDuplicant(Assignables dup)
	{
		return this.assignedDuplicants.Contains(dup);
	}

	public Assignable[] GetAssignables()
	{
		return this.region.GetComponentsInOwnedBuildings<Assignable>(true).ToArray();
	}

	[MyCmpReq]
	private Region region;

	private List<Assignables> assignedDuplicants = new List<Assignables>();

	public string slotID;

	private Dictionary<string, List<Assignable>> assignablesByPrefabTag = new Dictionary<string, List<Assignable>>();

	private List<Assignable> assignablesInRegion = new List<Assignable>();
}
