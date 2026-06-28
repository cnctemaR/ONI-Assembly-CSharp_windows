using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class SimpleVent : KMonoBehaviour, ISaveLoadableJson
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(-592767678, new EventSystem.EventHandler(this.OnChanged));
		this.Subscribe(-111137758, new EventSystem.EventHandler(this.OnChanged));
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
