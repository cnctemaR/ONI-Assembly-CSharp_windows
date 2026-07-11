using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitElementSensor : ConduitSensor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.filterable.onFilterChanged += this.OnFilterChanged;
		this.OnFilterChanged(this.filterable.SelectedTag);
	}

	private void OnFilterChanged(Tag tag)
	{
		if (!tag.IsValid)
		{
			return;
		}
		bool flag = tag == GameTags.Void;
		base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, flag, null);
	}

	protected override void ConduitUpdate(float dt)
	{
		Tag tag;
		bool flag;
		this.GetContentsElement(out tag, out flag);
		if (!base.IsSwitchedOn)
		{
			if (tag == this.filterable.SelectedTag && flag)
			{
				this.Toggle();
				return;
			}
		}
		else if (tag != this.filterable.SelectedTag || !flag)
		{
			this.Toggle();
		}
	}

	private void GetContentsElement(out Tag element, out bool hasMass)
	{
		int num = Grid.PosToCell(this);
		if (this.conduitType == ConduitType.Liquid || this.conduitType == ConduitType.Gas)
		{
			ConduitFlow.ConduitContents contents = Conduit.GetFlowManager(this.conduitType).GetContents(num);
			element = contents.element.CreateTag();
			hasMass = contents.mass > 0f;
			return;
		}
		SolidConduitFlow flowManager = SolidConduit.GetFlowManager();
		SolidConduitFlow.ConduitContents contents2 = flowManager.GetContents(num);
		Pickupable pickupable = flowManager.GetPickupable(contents2.pickupableHandle);
		KPrefabID kprefabID = ((pickupable != null) ? pickupable.GetComponent<KPrefabID>() : null);
		if (kprefabID != null && pickupable.PrimaryElement.Mass > 0f)
		{
			element = kprefabID.PrefabTag;
			hasMass = true;
			return;
		}
		element = GameTags.Void;
		hasMass = false;
	}

	[MyCmpGet]
	private Filterable filterable;
}
