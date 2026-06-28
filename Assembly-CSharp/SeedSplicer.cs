using System;
using System.Collections.Generic;
using UnityEngine;

public class SeedSplicer : Fabricator, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Cook;
		this.inStorage.choreType = Db.Get().ChoreTypes.CookFetch;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Cooking;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_cookstation_kanim") };
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return false;
	}

	protected override GameObject CompleteOrder(Fabricator.UserOrder completed_order)
	{
		GameObject gameObject = base.CompleteOrder(completed_order);
		base.GetComponent<Operational>().SetActive(false, false);
		return gameObject;
	}

	public override List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return base.GetDescriptors(def);
	}
}
