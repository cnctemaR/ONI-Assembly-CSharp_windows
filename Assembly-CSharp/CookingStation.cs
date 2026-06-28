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
		Debug.Assert(worker != null, "How did we get a null worker?");
		return false;
	}

	protected override void CompleteOrder(Fabricator.UserOrder completed_order)
	{
		base.CompleteOrder(completed_order);
		int num = Grid.PosToCell(this);
		for (int i = 0; i < 1; i++)
		{
			int num2 = Grid.OffsetCell(num, new CellOffset(0, i));
			GameObject gameObject = completed_order.recipe.Craft(this.inStorage, completed_order.orderTags);
			gameObject.transform.SetPosition(Grid.CellToPosCCC(num2, Grid.SceneLayer.Move));
			gameObject.SetActive(true);
			gameObject.GetComponent<KMonoBehaviour>().Trigger(748399584, null);
		}
		base.GetComponent<Operational>().SetActive(false, false);
	}
}
