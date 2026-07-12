using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class CancellableMove : Cancellable
{
	public List<Ref<Movable>> movingObjects
	{
		get
		{
			return this.movables;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable component = base.GetComponent<Prioritizable>();
		if (!component.IsPrioritizable())
		{
			component.AddRef();
		}
		if (this.movables.Count <= 0)
		{
			global::Debug.LogWarning("MovePickupable spawned with no objects to move. Destroying placer.");
			Util.KDestroyGameObject(base.gameObject);
			return;
		}
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(2127324410, new Action<object>(this.OnCancel));
		if (this.fetchChore == null && this.movables[0].Get() != null && !this.movables[0].Get().gameObject.IsNullOrDestroyed())
		{
			this.fetchChore = new MovePickupableChore(this, this.movables[0].Get().gameObject, new Action<Chore>(this.OnChoreEnd));
		}
		base.GetComponent<KPrefabID>().AddTag(GameTags.HasChores, false);
		int num = Grid.PosToCell(this);
		Grid.Objects[num, 44] = base.gameObject;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		int num = Grid.PosToCell(this);
		Grid.Objects[num, 44] = null;
		Prioritizable.RemoveRef(base.gameObject);
	}

	public void CancelAll()
	{
		this.OnCancel(null);
	}

	public void OnCancel(Movable cancel_movable = null)
	{
		for (int i = this.movables.Count - 1; i >= 0; i--)
		{
			Ref<Movable> @ref = this.movables[i];
			if (@ref != null)
			{
				Movable movable = @ref.Get();
				if (cancel_movable == null || movable == cancel_movable)
				{
					movable.ClearMove();
					this.movables.RemoveAt(i);
				}
			}
		}
		if (this.fetchChore != null)
		{
			this.fetchChore.Cancel("CancelMove");
			if (this.fetchChore.driver == null && this.movables.Count <= 0)
			{
				Util.KDestroyGameObject(base.gameObject);
			}
		}
	}

	protected override void OnCancel(object data)
	{
		this.OnCancel(null);
	}

	private void OnRefreshUserMenu(object data)
	{
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_control", UI.USERMENUACTIONS.PICKUPABLEMOVE.NAME_OFF, new global::System.Action(this.CancelAll), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.PICKUPABLEMOVE.TOOLTIP_OFF, true), 1f);
	}

	public void SetMovable(Movable movable)
	{
		if (this.fetchChore == null)
		{
			this.fetchChore = new MovePickupableChore(this, movable.gameObject, new Action<Chore>(this.OnChoreEnd));
		}
		if (this.movables.Find((Ref<Movable> move) => move.Get() == movable) == null)
		{
			this.movables.Add(new Ref<Movable>(movable));
		}
	}

	public void OnChoreEnd(Chore chore)
	{
		if (this.IsDeliveryComplete())
		{
			Util.KDestroyGameObject(base.gameObject);
			return;
		}
		this.fetchChore = new MovePickupableChore(this, this.movables[0].Get().gameObject, new Action<Chore>(this.OnChoreEnd));
	}

	public bool IsDeliveryComplete()
	{
		return this.movables.Count <= 0;
	}

	public void RemoveMovable(Movable moved)
	{
		for (int i = this.movables.Count - 1; i >= 0; i--)
		{
			if (this.movables[i].Get() == null || this.movables[i].Get() == moved)
			{
				this.movables.RemoveAt(i);
			}
		}
		if (this.movables.Count <= 0)
		{
			global::Debug.LogWarning("Pickupable " + moved.name + " has been destroyed and there are no more pickups to move. Cancel the chore");
			this.OnCancel(null);
		}
	}

	public GameObject GetNextTarget()
	{
		if (this.movables.Count >= 0)
		{
			return this.movables[0].Get().gameObject;
		}
		return null;
	}

	[Serialize]
	private List<Ref<Movable>> movables = new List<Ref<Movable>>();

	private MovePickupableChore fetchChore;
}
