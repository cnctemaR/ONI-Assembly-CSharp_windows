using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

public abstract class Assignable : KMonoBehaviour, ISaveLoadable
{
	public AssignableSlot slot
	{
		get
		{
			if (this._slot == null)
			{
				this._slot = Db.Get().AssignableSlots.Get(this.slotID);
			}
			return this._slot;
		}
	}

	public bool CanBeAssigned
	{
		get
		{
			return this.canBeAssigned;
		}
	}

	public event Action<IAssignableIdentity> OnAssign;

	[OnDeserialized]
	internal void OnDeserialized()
	{
	}

	private void RestoreAssignee()
	{
		IAssignableIdentity savedAssignee = this.GetSavedAssignee();
		if (savedAssignee != null)
		{
			this.Assign(savedAssignee);
		}
	}

	private IAssignableIdentity GetSavedAssignee()
	{
		if (this.assignee_identityRef.Get() != null)
		{
			return this.assignee_identityRef.Get().GetComponent<IAssignableIdentity>();
		}
		if (!string.IsNullOrEmpty(this.assignee_groupID))
		{
			return Game.Instance.assignmentManager.assignment_groups[this.assignee_groupID];
		}
		return null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.RestoreAssignee();
		Game.Instance.assignmentManager.Add(this);
		if (this.assignee == null && this.canBePublic)
		{
			this.Assign(Game.Instance.assignmentManager.assignment_groups["public"]);
		}
		this.assignmentPreconditions.Add(delegate(MinionAssignablesProxy proxy)
		{
			GameObject targetGameObject = proxy.GetTargetGameObject();
			return targetGameObject.GetComponent<KMonoBehaviour>().GetMyWorldId() == this.GetMyWorldId() || targetGameObject.IsMyParentWorld(base.gameObject);
		});
	}

	protected override void OnCleanUp()
	{
		this.Unassign();
		Game.Instance.assignmentManager.Remove(this);
		base.OnCleanUp();
	}

	public bool CanAutoAssignTo(IAssignableIdentity identity)
	{
		MinionAssignablesProxy minionAssignablesProxy = identity as MinionAssignablesProxy;
		if (minionAssignablesProxy == null)
		{
			return true;
		}
		if (!this.CanAssignTo(minionAssignablesProxy))
		{
			return false;
		}
		using (List<Func<MinionAssignablesProxy, bool>>.Enumerator enumerator = this.autoassignmentPreconditions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current(minionAssignablesProxy))
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool CanAssignTo(IAssignableIdentity identity)
	{
		MinionAssignablesProxy minionAssignablesProxy = identity as MinionAssignablesProxy;
		if (minionAssignablesProxy == null)
		{
			return true;
		}
		using (List<Func<MinionAssignablesProxy, bool>>.Enumerator enumerator = this.assignmentPreconditions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current(minionAssignablesProxy))
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool IsAssigned()
	{
		return this.assignee != null;
	}

	public bool IsAssignedTo(IAssignableIdentity identity)
	{
		global::Debug.Assert(identity != null, "IsAssignedTo identity is null");
		Ownables soleOwner = identity.GetSoleOwner();
		global::Debug.Assert(soleOwner != null, "IsAssignedTo identity sole owner is null");
		if (this.assignee != null)
		{
			foreach (Ownables ownables in this.assignee.GetOwners())
			{
				global::Debug.Assert(ownables, "Assignable owners list contained null");
				if (ownables.gameObject == soleOwner.gameObject)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public virtual void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee == this.assignee)
		{
			return;
		}
		if (new_assignee is KMonoBehaviour)
		{
			if (!this.CanAssignTo(new_assignee))
			{
				return;
			}
			this.assignee_identityRef.Set((KMonoBehaviour)new_assignee);
			this.assignee_groupID = "";
		}
		else if (new_assignee is AssignmentGroup)
		{
			this.assignee_identityRef.Set(null);
			this.assignee_groupID = ((AssignmentGroup)new_assignee).id;
		}
		base.GetComponent<KPrefabID>().AddTag(GameTags.Assigned, false);
		this.assignee = new_assignee;
		if (this.slot != null && (new_assignee is MinionIdentity || new_assignee is StoredMinionIdentity || new_assignee is MinionAssignablesProxy))
		{
			Ownables soleOwner = new_assignee.GetSoleOwner();
			if (soleOwner != null)
			{
				AssignableSlotInstance slot = soleOwner.GetSlot(this.slot);
				if (slot != null)
				{
					slot.Assign(this);
				}
			}
			Equipment component = soleOwner.GetComponent<Equipment>();
			if (component != null)
			{
				AssignableSlotInstance slot2 = component.GetSlot(this.slot);
				if (slot2 != null)
				{
					slot2.Assign(this);
				}
			}
		}
		if (this.OnAssign != null)
		{
			this.OnAssign(new_assignee);
		}
		base.Trigger(684616645, new_assignee);
	}

	public virtual void Unassign()
	{
		if (this.assignee == null)
		{
			return;
		}
		base.GetComponent<KPrefabID>().RemoveTag(GameTags.Assigned);
		if (this.slot != null)
		{
			Ownables soleOwner = this.assignee.GetSoleOwner();
			if (soleOwner)
			{
				AssignableSlotInstance assignableSlotInstance = soleOwner.GetSlot(this.slot);
				if (assignableSlotInstance != null)
				{
					assignableSlotInstance.Unassign(true);
				}
				Equipment component = soleOwner.GetComponent<Equipment>();
				if (component != null)
				{
					assignableSlotInstance = component.GetSlot(this.slot);
					if (assignableSlotInstance != null)
					{
						assignableSlotInstance.Unassign(true);
					}
				}
			}
		}
		this.assignee = null;
		if (this.canBePublic)
		{
			this.Assign(Game.Instance.assignmentManager.assignment_groups["public"]);
		}
		this.assignee_identityRef.Set(null);
		this.assignee_groupID = "";
		if (this.OnAssign != null)
		{
			this.OnAssign(null);
		}
		base.Trigger(684616645, null);
	}

	public void SetCanBeAssigned(bool state)
	{
		this.canBeAssigned = state;
	}

	public void AddAssignPrecondition(Func<MinionAssignablesProxy, bool> precondition)
	{
		this.assignmentPreconditions.Add(precondition);
	}

	public void AddAutoassignPrecondition(Func<MinionAssignablesProxy, bool> precondition)
	{
		this.autoassignmentPreconditions.Add(precondition);
	}

	public int GetNavigationCost(Navigator navigator)
	{
		int num = -1;
		int num2 = Grid.PosToCell(this);
		IApproachable component = base.GetComponent<IApproachable>();
		CellOffset[] array = ((component != null) ? component.GetOffsets() : new CellOffset[1]);
		DebugUtil.DevAssert(navigator != null, "Navigator is mysteriously null", null);
		if (navigator == null)
		{
			return -1;
		}
		foreach (CellOffset cellOffset in array)
		{
			int num3 = Grid.OffsetCell(num2, cellOffset);
			int navigationCost = navigator.GetNavigationCost(num3);
			if (navigationCost != -1 && (num == -1 || navigationCost < num))
			{
				num = navigationCost;
			}
		}
		return num;
	}

	public string slotID;

	private AssignableSlot _slot;

	public IAssignableIdentity assignee;

	[Serialize]
	protected Ref<KMonoBehaviour> assignee_identityRef = new Ref<KMonoBehaviour>();

	[Serialize]
	private string assignee_groupID = "";

	public AssignableSlot[] subSlots;

	public bool canBePublic;

	[Serialize]
	private bool canBeAssigned = true;

	private List<Func<MinionAssignablesProxy, bool>> autoassignmentPreconditions = new List<Func<MinionAssignablesProxy, bool>>();

	private List<Func<MinionAssignablesProxy, bool>> assignmentPreconditions = new List<Func<MinionAssignablesProxy, bool>>();
}
