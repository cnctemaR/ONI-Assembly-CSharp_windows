using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SimpleVent")]
public class SimpleVent : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.Subscribe<SimpleVent>(-592767678, SimpleVent.OnChangedDelegate);
		base.Subscribe<SimpleVent>(-111137758, SimpleVent.OnChangedDelegate);
	}

	protected override void OnSpawn()
	{
		this.OnChanged(null);
	}

	private void OnChanged(object data)
	{
		if (this.operational.IsFunctional)
		{
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, this);
			return;
		}
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, null, null);
	}

	[MyCmpGet]
	private Operational operational;

	private static readonly EventSystem.IntraObjectHandler<SimpleVent> OnChangedDelegate = new EventSystem.IntraObjectHandler<SimpleVent>(delegate(SimpleVent component, object data)
	{
		component.OnChanged(data);
	});
}
