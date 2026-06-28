using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Ownable : Assignable, ISaveLoadableJson, IEffectDescriptor
{
	protected override Assignables GetAssignables()
	{
		return this.assignablesRef.Get();
	}

	protected override void SetAssignables(Assignables assignables)
	{
		this.assignablesRef.Set((Ownables)assignables);
	}

	public override Assignables GetAssignables(GameObject go)
	{
		return go.GetComponent<Ownables>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.UpdateTint();
		base.OnAssign += this.OnNewAssignment;
		this.UpdateStatusString();
	}

	private void OnNewAssignment(Assignables assignables)
	{
		this.UpdateTint();
		this.UpdateStatusString();
	}

	private void UpdateTint()
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		if (component != null)
		{
			component.TintColour = ((!(this.GetAssignables() == null)) ? this.ownedTint : this.unownedTint);
		}
		else
		{
			KBatchedAnimController component2 = base.GetComponent<KBatchedAnimController>();
			if (component2 != null)
			{
				component2.TintColour = ((!(this.GetAssignables() == null)) ? this.ownedTint : this.unownedTint);
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
		StatusItem statusItem = Db.Get().BuildingStatusItems.Unassigned;
		if (this.GetAssignables() != null)
		{
			statusItem = Db.Get().BuildingStatusItems.AssignedTo;
		}
		component.SetStatusItem(Db.Get().StatusItemCategories.Main, statusItem, this);
	}

	public int DescriptionOrder { get; set; }

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.ASSIGNEDDUPLICANT), new object[0]), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ASSIGNEDDUPLICANT, new object[0]));
		list.Add(descriptor);
		return list;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		return null;
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	private Color unownedTint = Color.gray;

	private Color ownedTint = Color.white;

	[Serialize]
	private Ref<Ownables> assignablesRef = new Ref<Ownables>();
}
