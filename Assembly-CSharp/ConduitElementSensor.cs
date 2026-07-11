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
		this.desiredElement = SimHashes.Void;
		if (!tag.IsValid)
		{
			return;
		}
		Element element = ElementLoader.GetElement(tag);
		bool flag = true;
		if (element != null)
		{
			this.desiredElement = element.id;
			flag = this.desiredElement == SimHashes.Void || this.desiredElement == SimHashes.Vacuum;
		}
		base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, flag, null);
	}

	protected override void ConduitUpdate(float dt)
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
		int num = Grid.PosToCell(base.transform.GetPosition());
		ConduitFlow.ConduitContents contents = flowManager.GetContents(num);
		if (base.IsSwitchedOn)
		{
			if (contents.element != this.desiredElement)
			{
				this.Toggle();
				return;
			}
		}
		else if (contents.element == this.desiredElement)
		{
			this.Toggle();
		}
	}

	[MyCmpGet]
	private Filterable filterable;

	private SimHashes desiredElement = SimHashes.Void;
}
