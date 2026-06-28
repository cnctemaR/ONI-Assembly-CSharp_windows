using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Ownable : Assignable, ISaveLoadable, IEffectDescriptor
{
	public override void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee == this.assignee)
		{
			return;
		}
		if (new_assignee is MinionIdentity && base.slot != null && new_assignee.GetSoleOwner().GetComponent<Ownables>().GetSlot(base.slot)
			.assignable != null)
		{
			new_assignee.GetSoleOwner().GetComponent<Ownables>().GetSlot(base.slot)
				.assignable.Unassign();
		}
		base.Assign(new_assignee);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.UpdateTint();
		this.UpdateStatusString();
		base.OnAssign += this.OnNewAssignment;
	}

	private void OnNewAssignment(IAssignableIdentity assignables)
	{
		this.UpdateTint();
		this.UpdateStatusString();
	}

	private void UpdateTint()
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		if (component != null && component.HasBatchInstanceData)
		{
			component.TintColour = ((this.assignee != null) ? this.ownedTint : this.unownedTint);
		}
		else
		{
			KBatchedAnimController component2 = base.GetComponent<KBatchedAnimController>();
			if (component2 != null && component2.HasBatchInstanceData)
			{
				component2.TintColour = ((this.assignee != null) ? this.ownedTint : this.unownedTint);
			}
		}
	}

	private void UpdateStatusString()
	{
		KSelectable component = base.GetComponent<KSelectable>();
		if (component == null)
		{
			return;
		}
		StatusItem statusItem;
		if (this.assignee != null)
		{
			if (this.assignee is MinionIdentity)
			{
				statusItem = Db.Get().BuildingStatusItems.AssignedTo;
			}
			else if (this.assignee is Room)
			{
				statusItem = Db.Get().BuildingStatusItems.AssignedTo;
			}
			else
			{
				statusItem = Db.Get().BuildingStatusItems.AssignedTo;
			}
		}
		else
		{
			statusItem = Db.Get().BuildingStatusItems.Unassigned;
		}
		component.SetStatusItem(Db.Get().StatusItemCategories.Main, statusItem, this);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.ASSIGNEDDUPLICANT, UI.BUILDINGEFFECTS.TOOLTIPS.ASSIGNEDDUPLICANT, Descriptor.DescriptorType.Requirement);
		list.Add(descriptor);
		return list;
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	private Color unownedTint = Color.gray;

	private Color ownedTint = Color.white;
}
