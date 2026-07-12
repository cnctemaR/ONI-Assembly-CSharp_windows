using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SuitMarker")]
public class SuitMarker : KMonoBehaviour
{
	private bool OnlyTraverseIfUnequipAvailable
	{
		get
		{
			DebugUtil.Assert(this.onlyTraverseIfUnequipAvailable == (this.gridFlags & Grid.SuitMarker.Flags.OnlyTraverseIfUnequipAvailable) > (Grid.SuitMarker.Flags)0);
			return this.onlyTraverseIfUnequipAvailable;
		}
		set
		{
			this.onlyTraverseIfUnequipAvailable = value;
			this.UpdateGridFlag(Grid.SuitMarker.Flags.OnlyTraverseIfUnequipAvailable, this.onlyTraverseIfUnequipAvailable);
		}
	}

	private bool isRotated
	{
		get
		{
			return (this.gridFlags & Grid.SuitMarker.Flags.Rotated) > (Grid.SuitMarker.Flags)0;
		}
		set
		{
			this.UpdateGridFlag(Grid.SuitMarker.Flags.Rotated, value);
		}
	}

	private bool isOperational
	{
		get
		{
			return (this.gridFlags & Grid.SuitMarker.Flags.Operational) > (Grid.SuitMarker.Flags)0;
		}
		set
		{
			this.UpdateGridFlag(Grid.SuitMarker.Flags.Operational, value);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.OnlyTraverseIfUnequipAvailable = this.onlyTraverseIfUnequipAvailable;
		global::Debug.Assert(this.interactAnim != null, "interactAnim is null");
		base.Subscribe<SuitMarker>(493375141, SuitMarker.OnRefreshUserMenuDelegate);
		this.isOperational = base.GetComponent<Operational>().IsOperational;
		base.Subscribe<SuitMarker>(-592767678, SuitMarker.OnOperationalChangedDelegate);
		this.isRotated = base.GetComponent<Rotatable>().IsRotated;
		base.Subscribe<SuitMarker>(-1643076535, SuitMarker.OnRotatedDelegate);
		this.CreateNewReactable();
		this.cell = Grid.PosToCell(this);
		Grid.RegisterSuitMarker(this.cell);
		base.GetComponent<KAnimControllerBase>().Play("no_suit", KAnim.PlayMode.Once, 1f, 0f);
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits, true);
		this.RefreshTraverseIfUnequipStatusItem();
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), base.gameObject);
	}

	private void CreateNewReactable()
	{
		this.reactable = new SuitMarker.SuitMarkerReactable(this);
	}

	public void GetAttachedLockers(List<SuitLocker> suit_lockers)
	{
		int num = (this.isRotated ? 1 : (-1));
		int num2 = 1;
		for (;;)
		{
			int num3 = Grid.OffsetCell(this.cell, num2 * num, 0);
			GameObject gameObject = Grid.Objects[num3, 1];
			if (gameObject == null)
			{
				break;
			}
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (!(component == null))
			{
				if (!component.IsAnyPrefabID(this.LockerTags))
				{
					break;
				}
				SuitLocker component2 = gameObject.GetComponent<SuitLocker>();
				if (component2 == null)
				{
					break;
				}
				if (!suit_lockers.Contains(component2))
				{
					suit_lockers.Add(component2);
				}
			}
			num2++;
		}
	}

	public static bool DoesTraversalDirectionRequireSuit(int source_cell, int dest_cell, Grid.SuitMarker.Flags flags)
	{
		return Grid.CellColumn(dest_cell) > Grid.CellColumn(source_cell) == ((flags & Grid.SuitMarker.Flags.Rotated) == (Grid.SuitMarker.Flags)0);
	}

	public bool DoesTraversalDirectionRequireSuit(int source_cell, int dest_cell)
	{
		return SuitMarker.DoesTraversalDirectionRequireSuit(source_cell, dest_cell, this.gridFlags);
	}

	private void Update()
	{
		ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
		this.GetAttachedLockers(pooledList);
		int num = 0;
		int num2 = 0;
		KPrefabID kprefabID = null;
		foreach (SuitLocker suitLocker in pooledList)
		{
			if (suitLocker.CanDropOffSuit())
			{
				num++;
			}
			if (suitLocker.GetPartiallyChargedOutfit() != null)
			{
				num2++;
			}
			if (kprefabID == null)
			{
				kprefabID = suitLocker.GetStoredOutfit();
			}
		}
		pooledList.Recycle();
		bool flag = kprefabID != null;
		if (flag != this.hasAvailableSuit)
		{
			base.GetComponent<KAnimControllerBase>().Play(flag ? "off" : "no_suit", KAnim.PlayMode.Once, 1f, 0f);
			this.hasAvailableSuit = flag;
		}
		Grid.UpdateSuitMarker(this.cell, num2, num, this.gridFlags, this.PathFlag);
	}

	private void RefreshTraverseIfUnequipStatusItem()
	{
		if (this.OnlyTraverseIfUnequipAvailable)
		{
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalOnlyWhenRoomAvailable, null);
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalAnytime, false);
			return;
		}
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalOnlyWhenRoomAvailable, false);
		base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalAnytime, null);
	}

	private void OnEnableTraverseIfUnequipAvailable()
	{
		this.OnlyTraverseIfUnequipAvailable = true;
		this.RefreshTraverseIfUnequipStatusItem();
	}

	private void OnDisableTraverseIfUnequipAvailable()
	{
		this.OnlyTraverseIfUnequipAvailable = false;
		this.RefreshTraverseIfUnequipStatusItem();
	}

	private void UpdateGridFlag(Grid.SuitMarker.Flags flag, bool state)
	{
		if (state)
		{
			this.gridFlags |= flag;
			return;
		}
		this.gridFlags &= ~flag;
	}

	private void OnOperationalChanged(bool isOperational)
	{
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), base.gameObject);
		this.isOperational = isOperational;
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo buttonInfo = ((!this.OnlyTraverseIfUnequipAvailable) ? new KIconButtonMenu.ButtonInfo("action_clearance", UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ONLY_WHEN_ROOM_AVAILABLE.NAME, new global::System.Action(this.OnEnableTraverseIfUnequipAvailable), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ONLY_WHEN_ROOM_AVAILABLE.TOOLTIP, true) : new KIconButtonMenu.ButtonInfo("action_clearance", UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ALWAYS.NAME, new global::System.Action(this.OnDisableTraverseIfUnequipAvailable), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ALWAYS.TOOLTIP, true));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (base.isSpawned)
		{
			Grid.UnregisterSuitMarker(this.cell);
		}
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
		if (this.reactable != null)
		{
			this.reactable.Cleanup();
		}
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), null);
	}

	[MyCmpGet]
	private Building building;

	private ScenePartitionerEntry partitionerEntry;

	private SuitMarker.SuitMarkerReactable reactable;

	private bool hasAvailableSuit;

	[Serialize]
	private bool onlyTraverseIfUnequipAvailable;

	private Grid.SuitMarker.Flags gridFlags;

	private int cell;

	public Tag[] LockerTags;

	public PathFinder.PotentialPath.Flags PathFlag;

	public KAnimFile interactAnim = Assets.GetAnim("anim_equip_clothing_kanim");

	private static readonly EventSystem.IntraObjectHandler<SuitMarker> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<SuitMarker>(delegate(SuitMarker component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SuitMarker> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SuitMarker>(delegate(SuitMarker component, object data)
	{
		component.OnOperationalChanged((bool)data);
	});

	private static readonly EventSystem.IntraObjectHandler<SuitMarker> OnRotatedDelegate = new EventSystem.IntraObjectHandler<SuitMarker>(delegate(SuitMarker component, object data)
	{
		component.isRotated = ((Rotatable)data).IsRotated;
	});

	private class SuitMarkerReactable : Reactable
	{
		public SuitMarkerReactable(SuitMarker suit_marker)
			: base(suit_marker.gameObject, "SuitMarkerReactable", Db.Get().ChoreTypes.SuitMarker, 1, 1, false, 0f, 0f, float.PositiveInfinity, 0f, ObjectLayer.NumLayers)
		{
			this.suitMarker = suit_marker;
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (this.reactor != null)
			{
				return false;
			}
			if (this.suitMarker == null)
			{
				base.Cleanup();
				return false;
			}
			if (!this.suitMarker.isOperational)
			{
				return false;
			}
			int x = transition.navGridTransition.x;
			if (x == 0)
			{
				return this.IsRocketDoorExitEquip(new_reactor, transition);
			}
			if (new_reactor.GetComponent<MinionIdentity>().GetEquipment().IsSlotOccupied(Db.Get().AssignableSlots.Suit))
			{
				return (x >= 0 || !this.suitMarker.isRotated) && (x <= 0 || this.suitMarker.isRotated);
			}
			return (x <= 0 || !this.suitMarker.isRotated) && (x >= 0 || this.suitMarker.isRotated) && Grid.HasSuit(Grid.PosToCell(this.suitMarker), new_reactor.GetComponent<KPrefabID>().InstanceID);
		}

		private bool IsRocketDoorExitEquip(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			return new_reactor.GetMyWorld().IsModuleInterior && !new_reactor.GetComponent<MinionIdentity>().GetEquipment().IsSlotOccupied(Db.Get().AssignableSlots.Suit) && (transition.end == NavType.Teleport || transition.start == NavType.Teleport) && Grid.HasSuit(Grid.PosToCell(this.suitMarker), new_reactor.GetComponent<KPrefabID>().InstanceID);
		}

		protected override void InternalBegin()
		{
			this.startTime = Time.time;
			KBatchedAnimController component = this.reactor.GetComponent<KBatchedAnimController>();
			component.AddAnimOverrides(this.suitMarker.interactAnim, 1f);
			component.Play("working_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue("working_loop", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue("working_pst", KAnim.PlayMode.Once, 1f, 0f);
			if (this.suitMarker.HasTag(GameTags.JetSuitBlocker))
			{
				KBatchedAnimController component2 = this.suitMarker.GetComponent<KBatchedAnimController>();
				component2.Play("working_pre", KAnim.PlayMode.Once, 1f, 0f);
				component2.Queue("working_loop", KAnim.PlayMode.Once, 1f, 0f);
				component2.Queue("working_pst", KAnim.PlayMode.Once, 1f, 0f);
			}
			this.suitMarker.CreateNewReactable();
		}

		public override void Update(float dt)
		{
			Facing facing = (this.reactor ? this.reactor.GetComponent<Facing>() : null);
			if (facing && this.suitMarker)
			{
				facing.SetFacing(this.suitMarker.GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH);
			}
			if (Time.time - this.startTime > 2.8f)
			{
				this.Run();
				base.Cleanup();
			}
		}

		private void Run()
		{
			if (this.reactor == null)
			{
				return;
			}
			if (this.suitMarker == null)
			{
				return;
			}
			GameObject reactor = this.reactor;
			Equipment equipment = reactor.GetComponent<MinionIdentity>().GetEquipment();
			bool flag = !equipment.IsSlotOccupied(Db.Get().AssignableSlots.Suit);
			reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(this.suitMarker.interactAnim);
			bool flag2 = false;
			Navigator component = reactor.GetComponent<Navigator>();
			bool flag3 = component != null && (component.flags & this.suitMarker.PathFlag) > PathFinder.PotentialPath.Flags.None;
			if (flag || flag3)
			{
				ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
				this.suitMarker.GetAttachedLockers(pooledList);
				foreach (SuitLocker suitLocker in pooledList)
				{
					if (suitLocker.GetFullyChargedOutfit() != null && flag)
					{
						suitLocker.EquipTo(equipment);
						flag2 = true;
						break;
					}
					if (!flag && suitLocker.CanDropOffSuit())
					{
						suitLocker.UnequipFrom(equipment);
						flag2 = true;
						break;
					}
				}
				if (flag && !flag2)
				{
					SuitLocker suitLocker2 = null;
					float num = 0f;
					foreach (SuitLocker suitLocker3 in pooledList)
					{
						if (suitLocker3.GetSuitScore() > num)
						{
							suitLocker2 = suitLocker3;
							num = suitLocker3.GetSuitScore();
						}
					}
					if (suitLocker2 != null)
					{
						suitLocker2.EquipTo(equipment);
						flag2 = true;
					}
				}
				pooledList.Recycle();
			}
			if (!flag2 && !flag)
			{
				Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Suit);
				assignable.Unassign();
				Notification notification = new Notification(MISC.NOTIFICATIONS.SUIT_DROPPED.NAME, NotificationType.BadMinor, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.SUIT_DROPPED.TOOLTIP, null, true, 0f, null, null, null, true, false);
				assignable.GetComponent<Notifier>().Add(notification, "");
			}
		}

		protected override void InternalEnd()
		{
			if (this.reactor != null)
			{
				this.reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(this.suitMarker.interactAnim);
			}
		}

		protected override void InternalCleanup()
		{
		}

		private SuitMarker suitMarker;

		private float startTime;
	}
}
