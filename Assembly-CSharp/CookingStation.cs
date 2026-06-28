using System;
using UnityEngine;

public class CookingStation : Fabricator
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Cook;
		this.inStorage.choreType = Db.Get().ChoreTypes.CookFetch;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Cooking;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_cookstation_kanim") };
		this.attributeConverter = Db.Get().AttributeConverters.CookingSpeed;
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
		gameObject.SetActive(true);
		base.GetComponent<Operational>().SetActive(false, false);
		return gameObject;
	}
}
