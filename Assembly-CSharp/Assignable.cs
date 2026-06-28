using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

public abstract class Assignable : Workable
{
	public AssignableSlot slot { get; set; }

	public RequiresRegion RequiresRegion
	{
		get
		{
			return this.requiresRegion;
		}
	}

	public bool CanBeAssigned
	{
		get
		{
			return this.canBeAssigned;
		}
	}

	protected abstract Assignables GetAssignables();

	protected abstract void SetAssignables(Assignables assignables);

	public abstract Assignables GetAssignables(GameObject go);

	protected virtual void OnClickAssign(IAssignableIdentity new_assignee)
	{
		this.Assign(new_assignee);
	}

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<IAssignableIdentity> OnAssign;

	[OnDeserialized]
	internal void OnDeserialized()
	{
		IAssignableIdentity savedAssignee = this.GetSavedAssignee();
		if (savedAssignee != null)
		{
			this.Assign(savedAssignee);
		}
	}

	private IAssignableIdentity GetSavedAssignee()
	{
		IAssignableIdentity assignableIdentity;
		if (this.assignee_identityRef.Get() != null)
		{
			assignableIdentity = this.assignee_identityRef.Get().GetComponent<IAssignableIdentity>();
		}
		else if (this.assignee_groupID != "")
		{
			assignableIdentity = Game.Instance.assignmentManager.assignment_groups[this.assignee_groupID];
		}
		else
		{
			assignableIdentity = null;
		}
		return assignableIdentity;
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
		Game.Instance.assignmentManager.Add(this);
		if (this.assignee == null && this.canBePublic)
		{
			this.Assign(Game.Instance.assignmentManager.assignment_groups["public"]);
		}
	}

	protected override void OnCleanUp()
	{
		this.Unassign();
		Game.Instance.assignmentManager.Remove(this);
		base.OnCleanUp();
	}

	public virtual bool CanAutoAssignTo(KMonoBehaviour worker)
	{
		return true;
	}

	public bool IsAssigned()
	{
		return this.assignee != null;
	}

	public virtual void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee != this.assignee)
		{
			if (new_assignee is KMonoBehaviour)
			{
				this.assignee_identityRef.Set(new_assignee as KMonoBehaviour);
				this.assignee_groupID = "";
			}
			else if (new_assignee is AssignmentGroup)
			{
				this.assignee_identityRef.Set(null);
				this.assignee_groupID = (new_assignee as AssignmentGroup).id;
			}
			base.GetComponent<KPrefabID>().AddTag(GameTags.Assigned);
			this.assignee = new_assignee;
			if (this.slot != null)
			{
				if (new_assignee is MinionIdentity)
				{
					Assignables component = (new_assignee as MinionIdentity).GetComponent<Ownables>();
					AssignableSlotInstance slot = component.GetSlot(this.slot);
					if (slot != null)
					{
						component.GetSlot(this.slot).Assign(this);
					}
				}
			}
			if (this.OnAssign != null)
			{
				this.OnAssign(new_assignee);
				base.Trigger(684616645, new_assignee);
			}
		}
	}

	public virtual void Unassign()
	{
		if (this.assignee != null)
		{
			base.GetComponent<KPrefabID>().RemoveTag(GameTags.Assigned);
			if (this.slot != null)
			{
				if (this.assignee is MinionIdentity)
				{
					Assignables component = (this.assignee as MinionIdentity).GetComponent<Ownables>();
					AssignableSlotInstance slot = component.GetSlot(this.slot);
					if (slot != null)
					{
						slot.Unassign();
					}
				}
			}
			this.assignee = null;
			if (this.canBePublic)
			{
				this.Assign(Game.Instance.assignmentManager.assignment_groups["public"]);
				base.Trigger(2070884250, null);
			}
			this.assignee_identityRef.Set(null);
			this.assignee_groupID = "";
			if (this.OnAssign != null)
			{
				this.OnAssign(null);
			}
		}
	}

	public void SetCanBeAssigned(bool state)
	{
		this.canBeAssigned = state;
	}

	[MyCmpGet]
	private RequiresRegion requiresRegion;

	public IAssignableIdentity assignee;

	[Serialize]
	private Ref<KMonoBehaviour> assignee_identityRef = new Ref<KMonoBehaviour>();

	[Serialize]
	private string assignee_groupID = "";

	public AssignableSlot[] subSlots;

	public bool canBePublic = false;

	[Serialize]
	private bool canBeAssigned = true;
}
