using System;

public class SimpleVent : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.Subscribe(-592767678, new Action<object>(this.OnChanged));
		base.Subscribe(-111137758, new Action<object>(this.OnChanged));
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
		}
		else
		{
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, null, null);
		}
	}

	[MyCmpGet]
	private Operational operational;
}
