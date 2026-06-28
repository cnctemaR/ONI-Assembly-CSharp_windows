using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class Assignable : Workable, ISaveLoadableJson
{
	public event Action<Assignables> OnAssign;

	public AssignableSlot slot { get; set; }

	public Assignables assignee
	{
		get
		{
			return this.GetAssignables();
		}
		set
		{
			this.SetAssignables(value);
		}
	}

	public RequiresRegion RequiresRegion
	{
		get
		{
			return this.requiresRegion;
		}
	}

	protected abstract Assignables GetAssignables();

	protected abstract void SetAssignables(Assignables assignables);

	public abstract Assignables GetAssignables(GameObject go);

	protected virtual void OnClickAssign(Assignables new_assignee)
	{
		this.Assign(new_assignee);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		KPrefabID originalPrefab = base.GetComponent<KPrefabID>().GetOriginalPrefab();
		Assignable component = originalPrefab.GetComponent<Assignable>();
		this.slot = component.slot;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		AssignmentManager.Instance.Add(this);
	}

	protected override void OnCleanUp()
	{
		AssignmentManager.Instance.Remove(this);
	}

	public bool IsAssigned()
	{
		return this.assignee != null;
	}

	public void Assign(Assignables new_assignee)
	{
		if (new_assignee == this.assignee)
		{
			return;
		}
		Assignables assignee = this.assignee;
		this.assignee = new_assignee;
		if (this.assignee != null)
		{
			this.assignee.Assign(this);
		}
		if (assignee != null)
		{
			assignee.Unassign(this);
		}
		if (this.OnAssign != null)
		{
			this.OnAssign(new_assignee);
			this.Trigger(684616645, new_assignee);
		}
	}

	public void Unassign()
	{
		if (this.assignee == null)
		{
			return;
		}
		Assignables assignee = this.assignee;
		this.assignee = null;
		if (assignee != null)
		{
			assignee.Unassign(this);
		}
		if (this.OnAssign != null)
		{
			this.OnAssign(null);
		}
	}

	[MyCmpGet]
	private RequiresRegion requiresRegion;
}
