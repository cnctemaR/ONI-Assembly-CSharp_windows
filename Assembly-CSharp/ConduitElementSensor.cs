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
		if (element == null)
		{
			return;
		}
		this.desiredElement = element.id;
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
