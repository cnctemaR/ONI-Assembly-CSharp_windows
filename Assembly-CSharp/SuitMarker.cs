using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class SuitMarker : KMonoBehaviour, Pathfinding.INavigationFeature
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.CreateNewReactable();
		Pathfinding.Instance.AddNavigationFeature(Grid.PosToCell(this), this);
		base.GetComponent<KAnimControllerBase>().Play("no_suit", KAnim.PlayMode.Once, 1f, 0f);
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits);
		this.RefreshTraverseIfUnequipStatusItem();
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), base.gameObject);
	}

	private void CreateNewReactable()
	{
		this.reactable = new SuitMarker.SuitMarkerReactable(this);
	}

	public void GetAttachedLockers(List<SuitLocker> suit_lockers)
	{
		int num = -1;
		if (base.GetComponent<Rotatable>().IsRotated)
		{
			num = 1;
		}
		int num2 = Grid.PosToCell(this);
		int num3 = 1;
		for (;;)
		{
			int num4 = Grid.OffsetCell(num2, num3 * num, 0);
			GameObject gameObject = Grid.Objects[num4, 1];
			if (gameObject == null)
			{
				break;
			}
			if (!gameObject.HasTag("SuitLocker"))
			{
				break;
			}
			SuitLocker component = gameObject.GetComponent<SuitLocker>();
			if (component == null)
			{
				break;
			}
			if (!suit_lockers.Contains(component))
			{
				suit_lockers.Add(component);
			}
			num3++;
		}
	}

	private KPrefabID GetAvailableSuit()
	{
		ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
		this.GetAttachedLockers(pooledList);
		KPrefabID kprefabID = null;
		foreach (SuitLocker suitLocker in pooledList)
		{
			kprefabID = suitLocker.GetStoredOutfit();
			if (kprefabID != null)
			{
				break;
			}
		}
		pooledList.Recycle();
		return kprefabID;
	}

	public bool DoesTraversalDirectionRequireSuit(int source_cell, int dest_cell)
	{
		int num;
		int num2;
		Grid.CellToXY(source_cell, out num, out num2);
		int num3;
		int num4;
		Grid.CellToXY(dest_cell, out num3, out num4);
		bool flag = num3 > num;
		bool isRotated = base.GetComponent<Rotatable>().IsRotated;
		return (flag && !isRotated) || (!flag && isRotated);
	}

	private int GetFullyChargedSuitCount()
	{
		int num = 0;
		ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
		this.GetAttachedLockers(pooledList);
		foreach (SuitLocker suitLocker in pooledList)
		{
			if (suitLocker.GetFullyChargedOutfit() != null)
			{
				num++;
			}
		}
		pooledList.Recycle();
		return num;
	}

	public bool IsSuitAvailableForTraversal(SuitWearer.Instance suit_wearer)
	{
		int fullyChargedSuitCount = this.GetFullyChargedSuitCount();
		int count = this.equipReservations.Count;
		return count < fullyChargedSuitCount || (count == fullyChargedSuitCount && this.equipReservations.Contains(suit_wearer));
	}

	public bool IsUnequipAvailableForSuitWearer(SuitWearer.Instance suit_wearer)
	{
		int num = 0;
		ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
		this.GetAttachedLockers(pooledList);
		foreach (SuitLocker suitLocker in pooledList)
		{
			if (suitLocker.CanDropOffSuit())
			{
				num++;
			}
		}
		pooledList.Recycle();
		return num > this.unequipReservations.Count || (num == this.unequipReservations.Count && this.unequipReservations.Contains(suit_wearer));
	}

	public bool IsTraversable(Navigator agent, PathFinder.PotentialPath path, int from_cell, int cost, PathFinderAbilities abilities)
	{
		if (!base.GetComponent<Operational>().IsOperational)
		{
			return true;
		}
		if (!path.HasFlag(PathFinder.PotentialPath.Flags.PerformSuitChecks))
		{
			return true;
		}
		SuitWearer.Instance smi = agent.GetSMI<SuitWearer.Instance>();
		bool flag = this.DoesTraversalDirectionRequireSuit(from_cell, path.cell);
		bool flag2 = path.HasFlag(PathFinder.PotentialPath.Flags.HasSuit);
		if (flag)
		{
			bool flag3 = this.IsSuitAvailableForTraversal(smi);
			return flag2 || flag3;
		}
		return !flag2 || !this.onlyTraverseIfUnequipAvailable || this.IsUnequipAvailableForSuitWearer(smi);
	}

	public void ApplyTraversalToPath(Navigator agent, ref PathFinder.PotentialPath path, int from_cell)
	{
		if (!path.HasFlag(PathFinder.PotentialPath.Flags.PerformSuitChecks))
		{
			return;
		}
		if (!base.GetComponent<Operational>().IsOperational)
		{
			return;
		}
		bool flag = this.DoesTraversalDirectionRequireSuit(from_cell, path.cell);
		if (flag)
		{
			path.SetFlags(PathFinder.PotentialPath.Flags.HasSuit);
		}
		else
		{
			path.ClearFlags(PathFinder.PotentialPath.Flags.HasSuit);
		}
	}

	public void Reserve(SuitWearer.Instance suit_wearer, bool reserve_for_equipping)
	{
		if (reserve_for_equipping)
		{
			if (this.equipReservations.Contains(suit_wearer))
			{
				Output.LogWarningWithObj(base.gameObject, new object[] { "Reserve called more than once for same suit wearer: " + suit_wearer.gameObject });
			}
			else if (!this.IsSuitAvailableForTraversal(suit_wearer))
			{
				Output.LogWarningWithObj(base.gameObject, new object[] { "Reserve called with no suit available: " + suit_wearer.gameObject });
			}
			else
			{
				this.equipReservations.Add(suit_wearer);
			}
		}
		else
		{
			this.unequipReservations.Add(suit_wearer);
		}
	}

	public void Unreserve(SuitWearer.Instance suit_wearer, bool unreserve_for_equipping)
	{
		if (unreserve_for_equipping)
		{
			this.equipReservations.Remove(suit_wearer);
		}
		else
		{
			this.unequipReservations.Remove(suit_wearer);
		}
	}

	private void Update()
	{
		bool flag = this.GetAvailableSuit() != null;
		if (flag != this.hasAvailableSuit)
		{
			if (flag)
			{
				base.GetComponent<KAnimControllerBase>().Play("off", KAnim.PlayMode.Once, 1f, 0f);
			}
			else
			{
				base.GetComponent<KAnimControllerBase>().Play("no_suit", KAnim.PlayMode.Once, 1f, 0f);
			}
			this.hasAvailableSuit = flag;
		}
	}

	private void RefreshTraverseIfUnequipStatusItem()
	{
		if (this.onlyTraverseIfUnequipAvailable)
		{
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalOnlyWhenRoomAvailable, null);
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalAnytime, false);
		}
		else
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalOnlyWhenRoomAvailable, false);
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalAnytime, null);
		}
	}

	private void OnEnableTraverseIfUnequipAvailable()
	{
		this.onlyTraverseIfUnequipAvailable = true;
		this.RefreshTraverseIfUnequipStatusItem();
	}

	private void OnDisableTraverseIfUnequipAvailable()
	{
		this.onlyTraverseIfUnequipAvailable = false;
		this.RefreshTraverseIfUnequipStatusItem();
	}

	private void OnOperationalChanged(object data)
	{
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), base.gameObject);
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo buttonInfo;
		if (!this.onlyTraverseIfUnequipAvailable)
		{
			string text = "action_clearance";
			string text2 = UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ONLY_WHEN_ROOM_AVAILABLE.NAME;
			global::System.Action action = new global::System.Action(this.OnEnableTraverseIfUnequipAvailable);
			string text3 = UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ONLY_WHEN_ROOM_AVAILABLE.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true);
		}
		else
		{
			string text3 = "action_clearance";
			string text2 = UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ALWAYS.NAME;
			global::System.Action action = new global::System.Action(this.OnDisableTraverseIfUnequipAvailable);
			string text = UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ALWAYS.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true);
		}
		KIconButtonMenu.ButtonInfo buttonInfo2 = buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo2, 1f);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (base.isSpawned)
		{
			Pathfinding.Instance.RemoveNavigationFeature(Grid.PosToCell(this), this);
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

	private List<SuitWearer.Instance> equipReservations = new List<SuitWearer.Instance>();

	private List<SuitWearer.Instance> unequipReservations = new List<SuitWearer.Instance>();

	[Serialize]
	private bool onlyTraverseIfUnequipAvailable;

	private class SuitMarkerReactable : Reactable
	{
		public SuitMarkerReactable(SuitMarker suit_marker)
			: base(suit_marker.gameObject, "SuitMarkerReactable", Db.Get().ChoreTypes.SuitMarker, 1, 1, false, 0f, 0f, float.PositiveInfinity)
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
			if (!this.suitMarker.GetComponent<Operational>().IsOperational)
			{
				return false;
			}
			Rotatable component = this.gameObject.GetComponent<Rotatable>();
			SuitWearer.Instance smi = new_reactor.GetSMI<SuitWearer.Instance>();
			int x = transition.navGridTransition.x;
			if (x == 0)
			{
				return false;
			}
			if (new_reactor.GetComponent<Equipment>().IsSlotOccupied(Db.Get().AssignableSlots.Suit))
			{
				return (x >= 0 || !component.IsRotated) && (x <= 0 || component.IsRotated);
			}
			return (x <= 0 || !component.IsRotated) && (x >= 0 || component.IsRotated) && this.suitMarker.IsSuitAvailableForTraversal(smi);
		}

		protected override void InternalBegin()
		{
			this.reactor.GetComponent<KBatchedAnimController>().AddAnimOverrides(Assets.GetAnim("anim_equip_clothing_kanim"), 1f);
			this.reactor.GetComponent<KBatchedAnimController>().Play("working_pre", KAnim.PlayMode.Once, 1f, 0f);
			this.reactor.GetComponent<KBatchedAnimController>().Queue("working_loop", KAnim.PlayMode.Once, 1f, 0f);
			this.reactor.GetComponent<KBatchedAnimController>().Queue("working_pst", KAnim.PlayMode.Once, 1f, 0f);
			this.startTime = Time.time;
			this.suitMarker.CreateNewReactable();
		}

		public override void Update(float dt)
		{
			if (Time.time - this.startTime > 2.8f)
			{
				this.Run();
				base.Cleanup();
			}
		}

		private void Run()
		{
			if (this.reactor != null)
			{
				GameObject reactor = this.reactor;
				bool flag = !reactor.GetComponent<Equipment>().IsSlotOccupied(Db.Get().AssignableSlots.Suit);
				reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(Assets.GetAnim("anim_equip_clothing_kanim"));
				if (this.suitMarker != null)
				{
					ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
					this.suitMarker.GetAttachedLockers(pooledList);
					bool flag2 = false;
					foreach (SuitLocker suitLocker in pooledList)
					{
						KPrefabID fullyChargedOutfit = suitLocker.GetFullyChargedOutfit();
						if (fullyChargedOutfit != null && flag)
						{
							suitLocker.EquipTo(reactor.GetComponent<Equipment>());
							flag2 = true;
							break;
						}
						if (!flag && suitLocker.CanDropOffSuit())
						{
							suitLocker.UnequipFrom(reactor.GetComponent<Equipment>());
							flag2 = true;
							break;
						}
					}
					pooledList.Recycle();
					if (!flag2 && !flag)
					{
						Assignable assignable = reactor.GetComponent<Equipment>().GetAssignable(Db.Get().AssignableSlots.Suit);
						assignable.Unassign();
						Notification notification = new Notification(MISC.NOTIFICATIONS.SUIT_DROPPED.NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.SUIT_DROPPED.TOOLTIP, null, true, 0f, null, null);
						assignable.GetComponent<Notifier>().Add(notification, string.Empty);
					}
				}
			}
		}

		protected override void InternalEnd()
		{
			if (this.reactor != null)
			{
				this.reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(Assets.GetAnim("anim_equip_clothing_kanim"));
			}
		}

		protected override void InternalCleanup()
		{
		}

		private SuitMarker suitMarker;

		private float startTime;
	}
}
