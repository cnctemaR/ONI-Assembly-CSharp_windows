using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using KSerialization;

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
		if (this.assignee_identityRef.Get() != null)
		{
			return this.assignee_identityRef.Get().GetComponent<IAssignableIdentity>();
		}
		if (this.assignee_groupID != string.Empty)
		{
			return Game.Instance.assignmentManager.assignment_groups[this.assignee_groupID];
		}
		return null;
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

	public bool CanAutoAssignTo(IAssignableIdentity identity)
	{
		MinionIdentity minionIdentity = identity as MinionIdentity;
		if (minionIdentity == null)
		{
			return true;
		}
		if (!this.CanAssignTo(minionIdentity))
		{
			return false;
		}
		foreach (Func<MinionIdentity, bool> func in this.autoassignmentPreconditions)
		{
			if (!func(minionIdentity))
			{
				return false;
			}
		}
		return true;
	}

	public bool CanAssignTo(IAssignableIdentity identity)
	{
		MinionIdentity minionIdentity = identity as MinionIdentity;
		if (minionIdentity == null)
		{
			return true;
		}
		foreach (Func<MinionIdentity, bool> func in this.assignmentPreconditions)
		{
			if (!func(minionIdentity))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsAssigned()
	{
		return this.assignee != null;
	}

	public virtual void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee == this.assignee)
		{
			return;
		}
		if (new_assignee is KMonoBehaviour)
		{
			KMonoBehaviour kmonoBehaviour = new_assignee as KMonoBehaviour;
			if (!this.CanAssignTo(new_assignee))
			{
				return;
			}
			this.assignee_identityRef.Set(kmonoBehaviour);
			this.assignee_groupID = string.Empty;
		}
		else if (new_assignee is AssignmentGroup)
		{
			this.assignee_identityRef.Set(null);
			this.assignee_groupID = (new_assignee as AssignmentGroup).id;
		}
		base.GetComponent<KPrefabID>().AddTag(GameTags.Assigned);
		this.assignee = new_assignee;
		if (this.slot != null && (new_assignee is MinionIdentity || new_assignee is StoredMinionIdentity))
		{
			KMonoBehaviour kmonoBehaviour2 = new_assignee as KMonoBehaviour;
			Ownables component = kmonoBehaviour2.GetComponent<Ownables>();
			if (component != null)
			{
				AssignableSlotInstance slot = component.GetSlot(this.slot);
				if (slot != null)
				{
					slot.Assign(this);
				}
			}
			Equipment component2 = kmonoBehaviour2.GetComponent<Equipment>();
			if (component2 != null)
			{
				AssignableSlotInstance slot2 = component2.GetSlot(this.slot);
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
		if (this.slot != null && (this.assignee is MinionIdentity || this.assignee is StoredMinionIdentity))
		{
			Assignables component = (this.assignee as KMonoBehaviour).GetComponent<Ownables>();
			AssignableSlotInstance slot = component.GetSlot(this.slot);
			if (slot != null)
			{
				slot.Unassign(true);
			}
		}
		this.assignee = null;
		if (this.canBePublic)
		{
			this.Assign(Game.Instance.assignmentManager.assignment_groups["public"]);
		}
		this.assignee_identityRef.Set(null);
		this.assignee_groupID = string.Empty;
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

	public void AddAssignPrecondition(Func<MinionIdentity, bool> precondition)
	{
		this.assignmentPreconditions.Add(precondition);
	}

	public void AddAutoassignPrecondition(Func<MinionIdentity, bool> precondition)
	{
		this.autoassignmentPreconditions.Add(precondition);
	}

	public int GetNavigationCost(Navigator navigator)
	{
		int num = -1;
		int num2 = Grid.PosToCell(this);
		IApproachable component = base.GetComponent<IApproachable>();
		CellOffset[] array;
		if (component != null)
		{
			array = component.GetOffsets();
		}
		else
		{
			(array = new CellOffset[1])[0] = default(CellOffset);
		}
		CellOffset[] array2 = array;
		foreach (CellOffset cellOffset in array2)
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
	private Ref<KMonoBehaviour> assignee_identityRef = new Ref<KMonoBehaviour>();

	[Serialize]
	private string assignee_groupID = string.Empty;

	public AssignableSlot[] subSlots;

	public bool canBePublic;

	[Serialize]
	private bool canBeAssigned = true;

	private List<Func<MinionIdentity, bool>> autoassignmentPreconditions = new List<Func<MinionIdentity, bool>>();

	private List<Func<MinionIdentity, bool>> assignmentPreconditions = new List<Func<MinionIdentity, bool>>();

	public Func<MinionIdentity, bool> eligibleFilter;
}
