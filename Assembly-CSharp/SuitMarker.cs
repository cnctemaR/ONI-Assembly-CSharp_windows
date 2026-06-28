using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

public class SuitMarker : KMonoBehaviour, Pathfinding.INavigationFeature
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.CreateNewReactable();
		Pathfinding.Instance.AddNavigationFeature(Grid.PosToCell(this), this);
		base.GetComponent<KAnimControllerBase>().Play("no_suit", KAnim.PlayMode.Once, 1f, 0f);
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits);
		this.RefreshTraverseIfUnequipStatusItem();
	}

	private void CreateNewReactable()
	{
		this.reactable = new SuitMarker.SuitMarkerReactable(this);
	}

	public List<SuitLocker> GetAttachedLockers()
	{
		List<SuitLocker> list = new List<SuitLocker>();
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
			SuitLocker component = gameObject.GetComponent<SuitLocker>();
			if (component == null)
			{
				break;
			}
			if (!list.Contains(component))
			{
				list.Add(component);
			}
			num3++;
		}
		return list;
	}

	private KPrefabID GetAvailableSuit()
	{
		List<SuitLocker> attachedLockers = this.GetAttachedLockers();
		foreach (SuitLocker suitLocker in attachedLockers)
		{
			KPrefabID storedOutfit = suitLocker.GetStoredOutfit();
			if (storedOutfit != null)
			{
				return storedOutfit;
			}
		}
		return null;
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
		List<SuitLocker> attachedLockers = this.GetAttachedLockers();
		foreach (SuitLocker suitLocker in attachedLockers)
		{
			if (suitLocker.GetFullyChargedOutfit() != null)
			{
				num++;
			}
		}
		return num;
	}

	public bool IsSuitAvailableForTraversal(SuitWearer.Instance suit_wearer)
	{
		int fullyChargedSuitCount = this.GetFullyChargedSuitCount();
		int count = this.equipReservations.Count;
		return count < fullyChargedSuitCount || (count == fullyChargedSuitCount && this.equipReservations.Contains(suit_wearer));
	}

	public bool IsSuitAvailableForReactable(SuitWearer.Instance suit_wearer)
	{
		if (this.equipReservations.Contains(suit_wearer))
		{
			return true;
		}
		int count = this.equipReservations.Count;
		int fullyChargedSuitCount = this.GetFullyChargedSuitCount();
		return fullyChargedSuitCount > count;
	}

	public bool IsUnequipAvailableForSuitWearer(SuitWearer.Instance suit_wearer)
	{
		int num = 0;
		List<SuitLocker> attachedLockers = this.GetAttachedLockers();
		foreach (SuitLocker suitLocker in attachedLockers)
		{
			if (suitLocker.CanDropOffSuit())
			{
				num++;
			}
		}
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
		SuitWearer.Instance smi = agent.GetSMI<SuitWearer.Instance>();
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
			this.equipReservations.Add(suit_wearer);
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

	private void OnRefreshUserMenu(object data)
	{
		if (!this.onlyTraverseIfUnequipAvailable)
		{
			UserMenu userMenu = this.userMenu;
			string text = UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ONLY_WHEN_ROOM_AVAILABLE.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_clearance", UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ONLY_WHEN_ROOM_AVAILABLE.NAME, new global::System.Action(this.OnEnableTraverseIfUnequipAvailable), global::Action.NumActions, null, null, null, text, true), 1f);
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			string text = UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ALWAYS.TOOLTIP;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("action_clearance", UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ALWAYS.NAME, new global::System.Action(this.OnDisableTraverseIfUnequipAvailable), global::Action.NumActions, null, null, null, text, true), 1f);
		}
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
	}

	[MyCmpAdd]
	private UserMenu userMenu;

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
			: base(suit_marker.gameObject, Db.Get().ChoreTypes.SuitMarker, 1, 1, false)
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
			if (new_reactor.GetComponent<Equipment>().IsSlotOccupied(global::TUNING.EQUIPMENT.SUIT_SLOT))
			{
				return (transition.x >= 0 || !component.IsRotated) && (transition.x <= 0 || component.IsRotated);
			}
			return (transition.x <= 0 || !component.IsRotated) && (transition.x >= 0 || component.IsRotated) && this.suitMarker.IsSuitAvailableForReactable(smi);
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
				bool flag = !reactor.GetComponent<Equipment>().IsSlotOccupied(global::TUNING.EQUIPMENT.SUIT_SLOT);
				reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(Assets.GetAnim("anim_equip_clothing_kanim"));
				if (this.suitMarker != null)
				{
					List<SuitLocker> attachedLockers = this.suitMarker.GetAttachedLockers();
					bool flag2 = false;
					foreach (SuitLocker suitLocker in attachedLockers)
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
					if (!flag2 && !flag)
					{
						Assignable assignable = reactor.GetComponent<Equipment>().GetAssignable(global::TUNING.EQUIPMENT.SUIT_SLOT);
						assignable.Unassign();
						Notification notification = new Notification(MISC.NOTIFICATIONS.SUIT_DROPPED.NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.SUIT_DROPPED.TOOLTIP, null, true, 0f, null, null, null);
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
