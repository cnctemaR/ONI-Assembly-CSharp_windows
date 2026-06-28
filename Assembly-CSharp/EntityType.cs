using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class EntityType : Resource
{
	public EntityType(string id, string name)
		: base(id, name)
	{
	}

	public void SetupPrefab(GameObject prefab)
	{
		this.prefab = prefab;
		prefab.SetActive(false);
		KPrefabID kprefabID = prefab.UpdateComponentRequirement<KPrefabID>(true);
		kprefabID.PrefabTag = new Tag(this.Id);
		kprefabID.name = this.Name;
		prefab.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.EntityPrefabs).transform;
		EntityTypeSet.Instance.RegisterPrefab(kprefabID);
	}

	public void Apply(Modifiers modifiers, GameObject game_object)
	{
		Amounts amounts = modifiers.GetAmounts();
		foreach (Amount amount in this.amounts)
		{
			amounts.Add(new AmountInstance(amount, game_object)
			{
				value = global::UnityEngine.Random.Range(amount.startingMin, amount.startingMax)
			});
		}
		Traits component = modifiers.GetComponent<Traits>();
		foreach (Trait trait in this.baseTraits)
		{
			component.Add(trait);
		}
	}

	public GameObject CreateInstance()
	{
		return Util.KInstantiate(this.prefab, null, null);
	}

	public List<Amount> amounts = new List<Amount>();

	public List<Klei.AI.Attribute> attributes = new List<Klei.AI.Attribute>();

	public List<Trait> baseTraits = new List<Trait>();

	public GameObject prefab;
}
