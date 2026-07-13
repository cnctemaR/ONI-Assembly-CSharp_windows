using System;
using System.Runtime.CompilerServices;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/Movable")]
public class Movable : Workable
{
	public bool IsMarkedForMove
	{
		get
		{
			return this.isMarkedForMove;
		}
	}

	public Storage StorageProxy
	{
		get
		{
			if (this.storageProxy == null)
			{
				return null;
			}
			return this.storageProxy.Get();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(493375141, Movable.OnRefreshUserMenuDispatcher, this);
		base.Subscribe(1335436905, Movable.OnSplitFromChunkDispatcher, this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.isMarkedForMove)
		{
			if (this.StorageProxy != null)
			{
				if (this.reachableChangedHandle < 0)
				{
					this.reachableChangedHandle = base.Subscribe(-1432940121, Movable.OnReachableChangedDispatcher, this);
				}
				if (this.storageReachableChangedHandle < 0)
				{
					this.storageReachableChangedHandle = this.StorageProxy.Subscribe(-1432940121, Movable.OnReachableChangedDispatcher, this);
				}
				if (this.cancelHandle < 0)
				{
					this.cancelHandle = base.Subscribe(2127324410, Movable.CleanupMoveDispatcher, this);
				}
				if (this.tagsChangedHandle < 0)
				{
					this.tagsChangedHandle = base.Subscribe(-1582839653, Movable.OnTagsChangedDispatcher, this);
				}
				base.gameObject.AddTag(GameTags.MarkedForMove);
			}
			else
			{
				this.isMarkedForMove = false;
			}
		}
		if (Movable.IsCritterPickupable(base.gameObject))
		{
			this.skillsUpdateHandle = Game.Instance.Subscribe(-1523247426, Workable.UpdateStatusItemDispatcher, this);
			this.shouldShowSkillPerkStatusItem = this.isMarkedForMove;
			this.requiredSkillPerk = Db.Get().SkillPerks.CanWrangleCreatures.Id;
			this.UpdateStatusItem();
		}
	}

	private void OnReachableChanged(object _)
	{
		if (this.isMarkedForMove)
		{
			if (this.StorageProxy != null)
			{
				int num = Grid.PosToCell(this.pickupable);
				int num2 = Grid.PosToCell(this.StorageProxy);
				if (num != num2)
				{
					bool flag = MinionGroupProber.Get().IsReachable(num, OffsetGroups.Standard) && MinionGroupProber.Get().IsReachable(num2, OffsetGroups.Standard);
					if (this.pickupable.KPrefabID.HasTag(GameTags.Creatures.Confined))
					{
						flag = false;
					}
					KSelectable component = base.GetComponent<KSelectable>();
					this.pendingMoveGuid = component.ToggleStatusItem(Db.Get().MiscStatusItems.MarkedForMove, this.pendingMoveGuid, flag, this);
					this.storageUnreachableGuid = component.ToggleStatusItem(Db.Get().MiscStatusItems.MoveStorageUnreachable, this.storageUnreachableGuid, !flag, this);
					return;
				}
			}
			else
			{
				this.ClearMove();
			}
		}
	}

	private void OnSplitFromChunk(object data)
	{
		Pickupable pickupable = data as Pickupable;
		if (pickupable != null)
		{
			Movable component = pickupable.GetComponent<Movable>();
			if (component.isMarkedForMove)
			{
				this.storageProxy = new Ref<Storage>(component.StorageProxy);
				this.MarkForMove();
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.isMarkedForMove && this.StorageProxy != null)
		{
			this.StorageProxy.GetComponent<CancellableMove>().RemoveMovable(this);
			this.ClearStorageProxy();
		}
	}

	private void CleanupMove(object _)
	{
		if (this.StorageProxy != null)
		{
			this.StorageProxy.GetComponent<CancellableMove>().OnCancel(this);
		}
	}

	private void OnTagsChanged(object data)
	{
		if (this.isMarkedForMove && !this.HasTagRequiredToMove() && this.StorageProxy != null)
		{
			this.StorageProxy.GetComponent<CancellableMove>().OnCancel(this);
		}
	}

	public void ClearMove()
	{
		if (this.isMarkedForMove)
		{
			this.isMarkedForMove = false;
			KSelectable component = base.GetComponent<KSelectable>();
			this.pendingMoveGuid = component.RemoveStatusItem(this.pendingMoveGuid, false);
			this.storageUnreachableGuid = component.RemoveStatusItem(this.storageUnreachableGuid, false);
			this.ClearStorageProxy();
			base.gameObject.RemoveTag(GameTags.MarkedForMove);
			base.Unsubscribe(ref this.reachableChangedHandle);
			base.Unsubscribe(ref this.cancelHandle);
			base.Unsubscribe(ref this.tagsChangedHandle);
		}
		this.UpdateStatusItem();
	}

	private void ClearStorageProxy()
	{
		this.StorageProxy.Unsubscribe(ref this.storageReachableChangedHandle);
		this.storageProxy = null;
	}

	private void OnClickMove()
	{
		MoveToLocationTool.Instance.Activate(this);
	}

	private void OnClickCancel()
	{
		if (this.StorageProxy != null)
		{
			this.StorageProxy.GetComponent<CancellableMove>().OnCancel(this);
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		if (this.pickupable.KPrefabID.HasTag(GameTags.Stored) || !this.HasTagRequiredToMove())
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo = (this.isMarkedForMove ? new KIconButtonMenu.ButtonInfo("action_control", UI.USERMENUACTIONS.PICKUPABLEMOVE.NAME_OFF, new global::System.Action(this.OnClickCancel), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.PICKUPABLEMOVE.TOOLTIP_OFF, true) : new KIconButtonMenu.ButtonInfo("action_control", UI.USERMENUACTIONS.PICKUPABLEMOVE.NAME, new global::System.Action(this.OnClickMove), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.PICKUPABLEMOVE.TOOLTIP, true));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}

	private bool HasTagRequiredToMove()
	{
		return this.tagRequiredForMove == Tag.Invalid || this.pickupable.KPrefabID.HasTag(this.tagRequiredForMove);
	}

	public void MoveToLocation(int cell)
	{
		this.CreateStorageProxy(cell);
		this.MarkForMove();
		base.gameObject.Trigger(1122777325, base.gameObject);
	}

	private void MarkForMove()
	{
		base.Trigger(2127324410, null);
		this.isMarkedForMove = true;
		this.OnReachableChanged(null);
		this.storageReachableChangedHandle = this.StorageProxy.Subscribe(-1432940121, Movable.OnReachableChangedDispatcher, this);
		this.reachableChangedHandle = base.Subscribe(-1432940121, Movable.OnReachableChangedDispatcher, this);
		this.StorageProxy.GetComponent<CancellableMove>().SetMovable(this);
		base.gameObject.AddTag(GameTags.MarkedForMove);
		this.cancelHandle = base.Subscribe(2127324410, Movable.CleanupMoveDispatcher, this);
		this.tagsChangedHandle = base.Subscribe(-1582839653, Movable.OnTagsChangedDispatcher, this);
		this.UpdateStatusItem();
	}

	private void UpdateStatusItem()
	{
		if (Movable.IsCritterPickupable(base.gameObject))
		{
			this.shouldShowSkillPerkStatusItem = this.isMarkedForMove;
			base.UpdateStatusItem(null);
		}
	}

	public bool CanMoveTo(int cell)
	{
		return !Grid.IsSolidCell(cell) && Grid.IsWorldValidCell(cell) && base.gameObject.IsMyParentWorld(cell);
	}

	private void CreateStorageProxy(int cell)
	{
		if (this.storageProxy == null || this.storageProxy.Get() == null)
		{
			if (Grid.Objects[cell, 44] != null)
			{
				Storage component = Grid.Objects[cell, 44].GetComponent<Storage>();
				this.storageProxy = new Ref<Storage>(component);
				return;
			}
			Vector3 vector = Grid.CellToPosCBC(cell, MoveToLocationTool.Instance.visualizerLayer);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MovePickupablePlacerConfig.ID), vector);
			Storage component2 = gameObject.GetComponent<Storage>();
			gameObject.SetActive(true);
			this.storageProxy = new Ref<Storage>(component2);
		}
	}

	public static bool IsCritterPickupable(GameObject pickupable_go)
	{
		return pickupable_go.GetComponent<Capturable>();
	}

	[MyCmpReq]
	private Pickupable pickupable;

	public Tag tagRequiredForMove = Tag.Invalid;

	[Serialize]
	private bool isMarkedForMove;

	[Serialize]
	private Ref<Storage> storageProxy;

	private int storageReachableChangedHandle = -1;

	private int reachableChangedHandle = -1;

	private int cancelHandle = -1;

	private int tagsChangedHandle = -1;

	private Guid pendingMoveGuid;

	private Guid storageUnreachableGuid;

	public Action<GameObject> onDeliveryComplete;

	public Action<GameObject> onPickupComplete;

	private static Action<object, object> OnReachableChangedDispatcher = delegate(object context, object data)
	{
		Unsafe.As<Movable>(context).OnReachableChanged(data);
	};

	private static Action<object, object> OnSplitFromChunkDispatcher = delegate(object context, object data)
	{
		Unsafe.As<Movable>(context).OnSplitFromChunk(data);
	};

	private static Action<object, object> CleanupMoveDispatcher = delegate(object context, object data)
	{
		Unsafe.As<Movable>(context).CleanupMove(data);
	};

	private static Action<object, object> OnTagsChangedDispatcher = delegate(object context, object data)
	{
		Unsafe.As<Movable>(context).OnTagsChanged(data);
	};

	private static Action<object, object> OnRefreshUserMenuDispatcher = delegate(object context, object data)
	{
		Unsafe.As<Movable>(context).OnRefreshUserMenu(data);
	};
}
