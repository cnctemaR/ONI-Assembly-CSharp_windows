using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class ChorePreconditions
{
	public ChorePreconditions()
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "IsPreemptable";
		precondition.sortOrder = 1;
		precondition.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_PREEMPTABLE;
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.isAttemptingOverride || context.chore.CanPreempt(context) || context.chore.driver == null;
		};
		this.IsPreemptable = precondition;
		Chore.Precondition precondition2 = default(Chore.Precondition);
		precondition2.id = "HasUrge";
		precondition2.description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_URGE;
		precondition2.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.chore.choreType.urge == null)
			{
				return true;
			}
			foreach (Urge urge in context.consumerState.consumer.GetUrges())
			{
				if (context.chore.SatisfiesUrge(urge))
				{
					return true;
				}
			}
			return false;
		};
		this.HasUrge = precondition2;
		Chore.Precondition precondition3 = default(Chore.Precondition);
		precondition3.id = "IsValid";
		precondition3.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_VALID;
		precondition3.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.chore.IsValid();
		};
		this.IsValid = precondition3;
		Chore.Precondition precondition4 = default(Chore.Precondition);
		precondition4.id = "IsPermitted";
		precondition4.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_PERMITTED;
		precondition4.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.consumerState.consumer.IsPermittedOrEnabled(context.choreTypeForPermission, context.chore);
		};
		this.IsPermitted = precondition4;
		Chore.Precondition precondition5 = default(Chore.Precondition);
		precondition5.id = "IsAssignedToMe";
		precondition5.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_ASSIGNED_TO_ME;
		precondition5.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Assignable assignable = (Assignable)data;
			IAssignableIdentity component = context.consumerState.gameObject.GetComponent<IAssignableIdentity>();
			return assignable.IsAssignedTo(component);
		};
		this.IsAssignedtoMe = precondition5;
		Chore.Precondition precondition6 = default(Chore.Precondition);
		precondition6.id = "IsInMyRoom";
		precondition6.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_IN_MY_ROOM;
		precondition6.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Room room = (Room)data;
			if (room != null)
			{
				if (context.consumerState.ownable != null)
				{
					foreach (Ownables ownables in room.GetOwners())
					{
						if (ownables.gameObject == context.consumerState.gameObject)
						{
							return true;
						}
					}
					return false;
				}
				Room room2 = null;
				FetchChore fetchChore = context.chore as FetchChore;
				if (fetchChore != null && fetchChore.destination != null)
				{
					CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(fetchChore.destination));
					if (cavityForCell != null)
					{
						room2 = cavityForCell.room;
					}
					return room2 != null && room2 == room;
				}
				if (context.chore is WorkChore<Tinkerable>)
				{
					CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell((context.chore as WorkChore<Tinkerable>).gameObject));
					if (cavityForCell2 != null)
					{
						room2 = cavityForCell2.room;
					}
					return room2 != null && room2 == room;
				}
				return false;
			}
			return false;
		};
		this.IsInMyRoom = precondition6;
		Chore.Precondition precondition7 = default(Chore.Precondition);
		precondition7.id = "IsPreferredAssignable";
		precondition7.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_PREFERRED_ASSIGNABLE;
		precondition7.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Assignable assignable2 = (Assignable)data;
			return Game.Instance.assignmentManager.GetPreferredAssignables(context.consumerState.assignables, assignable2.slot).Contains(assignable2);
		};
		this.IsPreferredAssignable = precondition7;
		Chore.Precondition precondition8 = default(Chore.Precondition);
		precondition8.id = "IsPreferredAssignableOrUrgent";
		precondition8.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_PREFERRED_ASSIGNABLE_OR_URGENT_BLADDER;
		precondition8.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Assignable assignable3 = (Assignable)data;
			if (Game.Instance.assignmentManager.GetPreferredAssignables(context.consumerState.assignables, assignable3.slot).Contains(assignable3))
			{
				return true;
			}
			PeeChoreMonitor.Instance smi = context.consumerState.gameObject.GetSMI<PeeChoreMonitor.Instance>();
			return smi != null && smi.IsInsideState(smi.sm.critical);
		};
		this.IsPreferredAssignableOrUrgentBladder = precondition8;
		Chore.Precondition precondition9 = default(Chore.Precondition);
		precondition9.id = "IsNotTransferArm";
		precondition9.description = string.Empty;
		precondition9.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !context.consumerState.hasSolidTransferArm;
		};
		this.IsNotTransferArm = precondition9;
		Chore.Precondition precondition10 = default(Chore.Precondition);
		precondition10.id = "HasSkillPerk";
		precondition10.description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_SKILL_PERK;
		precondition10.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			MinionResume resume = context.consumerState.resume;
			if (!resume)
			{
				return false;
			}
			if (data is SkillPerk)
			{
				SkillPerk skillPerk = data as SkillPerk;
				return resume.HasPerk(skillPerk);
			}
			if (data is HashedString)
			{
				HashedString hashedString = (HashedString)data;
				return resume.HasPerk(hashedString);
			}
			if (data is string)
			{
				HashedString hashedString2 = (string)data;
				return resume.HasPerk(hashedString2);
			}
			return false;
		};
		this.HasSkillPerk = precondition10;
		Chore.Precondition precondition11 = default(Chore.Precondition);
		precondition11.id = "IsMoreSatisfyingEarly";
		precondition11.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MORE_SATISFYING;
		precondition11.sortOrder = -1;
		precondition11.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.isAttemptingOverride)
			{
				return true;
			}
			if (context.consumerState.selectable.IsSelected)
			{
				return true;
			}
			Chore currentChore = context.consumerState.choreDriver.GetCurrentChore();
			if (currentChore == null)
			{
				return true;
			}
			if (context.masterPriority.priority_class != currentChore.masterPriority.priority_class)
			{
				return context.masterPriority.priority_class > currentChore.masterPriority.priority_class;
			}
			if (context.consumerState.consumer != null && context.personalPriority != context.consumerState.consumer.GetPersonalPriority(currentChore.choreType))
			{
				return context.personalPriority > context.consumerState.consumer.GetPersonalPriority(currentChore.choreType);
			}
			if (context.masterPriority.priority_value != currentChore.masterPriority.priority_value)
			{
				return context.masterPriority.priority_value > currentChore.masterPriority.priority_value;
			}
			return context.priority > currentChore.choreType.priority;
		};
		this.IsMoreSatisfyingEarly = precondition11;
		Chore.Precondition precondition12 = default(Chore.Precondition);
		precondition12.id = "IsMoreSatisfyingLate";
		precondition12.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MORE_SATISFYING;
		precondition12.sortOrder = 10000;
		precondition12.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.isAttemptingOverride)
			{
				return true;
			}
			if (!context.consumerState.selectable.IsSelected)
			{
				return true;
			}
			Chore currentChore2 = context.consumerState.choreDriver.GetCurrentChore();
			if (currentChore2 == null)
			{
				return true;
			}
			if (context.masterPriority.priority_class != currentChore2.masterPriority.priority_class)
			{
				return context.masterPriority.priority_class > currentChore2.masterPriority.priority_class;
			}
			if (context.consumerState.consumer != null && context.personalPriority != context.consumerState.consumer.GetPersonalPriority(currentChore2.choreType))
			{
				return context.personalPriority > context.consumerState.consumer.GetPersonalPriority(currentChore2.choreType);
			}
			if (context.masterPriority.priority_value != currentChore2.masterPriority.priority_value)
			{
				return context.masterPriority.priority_value > currentChore2.masterPriority.priority_value;
			}
			return context.priority > currentChore2.choreType.priority;
		};
		this.IsMoreSatisfyingLate = precondition12;
		Chore.Precondition precondition13 = default(Chore.Precondition);
		precondition13.id = "CanChat";
		precondition13.description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_CHAT;
		precondition13.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			KMonoBehaviour kmonoBehaviour = (KMonoBehaviour)data;
			return !(context.consumerState.consumer == null) && !(context.consumerState.navigator == null) && !(kmonoBehaviour == null) && context.consumerState.navigator.CanReach(kmonoBehaviour.GetComponent<Chattable>());
		};
		this.IsChattable = precondition13;
		Chore.Precondition precondition14 = default(Chore.Precondition);
		precondition14.id = "IsNotRedAlert";
		precondition14.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_RED_ALERT;
		precondition14.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.topPriority || !VignetteManager.Instance.Get().IsRedAlert();
		};
		this.IsNotRedAlert = precondition14;
		Chore.Precondition precondition15 = default(Chore.Precondition);
		precondition15.id = "IsScheduledTime";
		precondition15.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_SCHEDULED_TIME;
		precondition15.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (VignetteManager.Instance.Get().IsRedAlert())
			{
				return true;
			}
			ScheduleBlockType scheduleBlockType = (ScheduleBlockType)data;
			ScheduleBlock scheduleBlock = context.consumerState.scheduleBlock;
			return scheduleBlock == null || scheduleBlock.IsAllowed(scheduleBlockType);
		};
		this.IsScheduledTime = precondition15;
		Chore.Precondition precondition16 = default(Chore.Precondition);
		precondition16.id = "CanMoveTo";
		precondition16.description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO;
		precondition16.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.consumerState.consumer == null)
			{
				return false;
			}
			KMonoBehaviour kmonoBehaviour2 = (KMonoBehaviour)data;
			if (kmonoBehaviour2 == null)
			{
				return false;
			}
			IApproachable approachable = (IApproachable)kmonoBehaviour2;
			int num;
			if (context.consumerState.consumer.GetNavigationCost(approachable, out num))
			{
				context.cost += num;
				return true;
			}
			return false;
		};
		this.CanMoveTo = precondition16;
		Chore.Precondition precondition17 = default(Chore.Precondition);
		precondition17.id = "CanPickup";
		precondition17.description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_PICKUP;
		precondition17.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Pickupable pickupable = (Pickupable)data;
			return !(pickupable == null) && !(context.consumerState.consumer == null) && !pickupable.HasTag(GameTags.StoredPrivate) && pickupable.CouldBePickedUpByMinion(context.consumerState.gameObject) && context.consumerState.consumer.CanReach(pickupable);
		};
		this.CanPickup = precondition17;
		Chore.Precondition precondition18 = default(Chore.Precondition);
		precondition18.id = "IsAwake";
		precondition18.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_AWAKE;
		precondition18.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.consumerState.consumer == null)
			{
				return false;
			}
			StaminaMonitor.Instance smi2 = context.consumerState.consumer.GetSMI<StaminaMonitor.Instance>();
			return !smi2.IsInsideState(smi2.sm.sleepy.sleeping);
		};
		this.IsAwake = precondition18;
		Chore.Precondition precondition19 = default(Chore.Precondition);
		precondition19.id = "IsStanding";
		precondition19.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_STANDING;
		precondition19.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !(context.consumerState.consumer == null) && !(context.consumerState.navigator == null) && context.consumerState.navigator.CurrentNavType == NavType.Floor;
		};
		this.IsStanding = precondition19;
		Chore.Precondition precondition20 = default(Chore.Precondition);
		precondition20.id = "IsMoving";
		precondition20.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MOVING;
		precondition20.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !(context.consumerState.consumer == null) && !(context.consumerState.navigator == null) && context.consumerState.navigator.IsMoving();
		};
		this.IsMoving = precondition20;
		Chore.Precondition precondition21 = default(Chore.Precondition);
		precondition21.id = "IsOffLadder";
		precondition21.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_OFF_LADDER;
		precondition21.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !(context.consumerState.consumer == null) && !(context.consumerState.navigator == null) && context.consumerState.navigator.CurrentNavType != NavType.Ladder && context.consumerState.navigator.CurrentNavType != NavType.Pole;
		};
		this.IsOffLadder = precondition21;
		Chore.Precondition precondition22 = default(Chore.Precondition);
		precondition22.id = "NotInTube";
		precondition22.description = DUPLICANTS.CHORES.PRECONDITIONS.NOT_IN_TUBE;
		precondition22.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !(context.consumerState.consumer == null) && !(context.consumerState.navigator == null) && context.consumerState.navigator.CurrentNavType != NavType.Tube;
		};
		this.NotInTube = precondition22;
		Chore.Precondition precondition23 = default(Chore.Precondition);
		precondition23.id = "ConsumerHasTrait";
		precondition23.description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_TRAIT;
		precondition23.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			string text = (string)data;
			Traits traits = context.consumerState.traits;
			return !(traits == null) && traits.HasTrait(text);
		};
		this.ConsumerHasTrait = precondition23;
		Chore.Precondition precondition24 = default(Chore.Precondition);
		precondition24.id = "IsOperational";
		precondition24.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_OPERATIONAL;
		precondition24.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Operational operational = data as Operational;
			return operational.IsOperational;
		};
		this.IsOperational = precondition24;
		Chore.Precondition precondition25 = default(Chore.Precondition);
		precondition25.id = "IsNotMarkedForDeconstruction";
		precondition25.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DECONSTRUCTION;
		precondition25.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Deconstructable deconstructable = data as Deconstructable;
			return deconstructable == null || !deconstructable.IsMarkedForDeconstruction();
		};
		this.IsNotMarkedForDeconstruction = precondition25;
		Chore.Precondition precondition26 = default(Chore.Precondition);
		precondition26.id = "IsNotMarkedForDisable";
		precondition26.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DISABLE;
		precondition26.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			BuildingEnabledButton buildingEnabledButton = data as BuildingEnabledButton;
			return buildingEnabledButton == null || (buildingEnabledButton.IsEnabled && !buildingEnabledButton.WaitingForDisable);
		};
		this.IsNotMarkedForDisable = precondition26;
		Chore.Precondition precondition27 = default(Chore.Precondition);
		precondition27.id = "IsFunctional";
		precondition27.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_FUNCTIONAL;
		precondition27.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Operational operational2 = data as Operational;
			return operational2.IsFunctional;
		};
		this.IsFunctional = precondition27;
		Chore.Precondition precondition28 = default(Chore.Precondition);
		precondition28.id = "IsOverrideTargetNullOrMe";
		precondition28.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_OVERRIDE_TARGET_NULL_OR_ME;
		precondition28.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.isAttemptingOverride || context.chore.overrideTarget == null || context.chore.overrideTarget == context.consumerState.consumer;
		};
		this.IsOverrideTargetNullOrMe = precondition28;
		Chore.Precondition precondition29 = default(Chore.Precondition);
		precondition29.id = "NotChoreCreator";
		precondition29.description = DUPLICANTS.CHORES.PRECONDITIONS.NOT_CHORE_CREATOR;
		precondition29.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject = (GameObject)data;
			return !(context.consumerState.consumer == null) && !(context.consumerState.gameObject == gameObject);
		};
		this.NotChoreCreator = precondition29;
		Chore.Precondition precondition30 = default(Chore.Precondition);
		precondition30.id = "IsGettingMoreStressed";
		precondition30.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_GETTING_MORE_STRESSED;
		precondition30.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(context.consumerState.gameObject);
			return amountInstance.GetDelta() > 0f;
		};
		this.IsGettingMoreStressed = precondition30;
		Chore.Precondition precondition31 = default(Chore.Precondition);
		precondition31.id = "IsAllowedByAutomation";
		precondition31.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_ALLOWED_BY_AUTOMATION;
		precondition31.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Automatable automatable = (Automatable)data;
			return automatable.AllowedByAutomation(context.consumerState.hasSolidTransferArm);
		};
		this.IsAllowedByAutomation = precondition31;
		Chore.Precondition precondition32 = default(Chore.Precondition);
		precondition32.id = "HasTag";
		precondition32.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Tag tag = (Tag)data;
			return context.consumerState.prefabid.HasTag(tag);
		};
		this.HasTag = precondition32;
		Chore.Precondition precondition33 = default(Chore.Precondition);
		precondition33.id = "CheckBehaviourPrecondition";
		precondition33.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Tag tag2 = (Tag)data;
			return context.consumerState.consumer.RunBehaviourPrecondition(tag2);
		};
		this.CheckBehaviourPrecondition = precondition33;
		Chore.Precondition precondition34 = default(Chore.Precondition);
		precondition34.id = "CanDoWorkerPrioritizable";
		precondition34.description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_DO_RECREATION;
		precondition34.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.consumerState.consumer == null)
			{
				return false;
			}
			IWorkerPrioritizable workerPrioritizable = data as IWorkerPrioritizable;
			if (workerPrioritizable == null)
			{
				return false;
			}
			int num2 = 0;
			if (workerPrioritizable.GetWorkerPriority(context.consumerState.worker, out num2))
			{
				context.consumerPriority += num2;
				return true;
			}
			return false;
		};
		this.CanDoWorkerPrioritizable = precondition34;
		Chore.Precondition precondition35 = default(Chore.Precondition);
		precondition35.id = "IsExclusivelyAvailableWithOtherChores";
		precondition35.description = DUPLICANTS.CHORES.PRECONDITIONS.EXCLUSIVELY_AVAILABLE;
		precondition35.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			List<Chore> list = (List<Chore>)data;
			foreach (Chore chore in list)
			{
				if (chore != context.chore && chore.driver != null)
				{
					return false;
				}
			}
			return true;
		};
		this.IsExclusivelyAvailableWithOtherChores = precondition35;
		Chore.Precondition precondition36 = default(Chore.Precondition);
		precondition36.id = "IsBladderFull";
		precondition36.description = DUPLICANTS.CHORES.PRECONDITIONS.BLADDER_FULL;
		precondition36.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			BladderMonitor.Instance smi3 = context.consumerState.gameObject.GetSMI<BladderMonitor.Instance>();
			return smi3 != null && smi3.NeedsToPee();
		};
		this.IsBladderFull = precondition36;
		Chore.Precondition precondition37 = default(Chore.Precondition);
		precondition37.id = "IsBladderNotFull";
		precondition37.description = DUPLICANTS.CHORES.PRECONDITIONS.BLADDER_NOT_FULL;
		precondition37.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			BladderMonitor.Instance smi4 = context.consumerState.gameObject.GetSMI<BladderMonitor.Instance>();
			return smi4 == null || !smi4.NeedsToPee();
		};
		this.IsBladderNotFull = precondition37;
		Chore.Precondition precondition38 = default(Chore.Precondition);
		precondition38.id = "NoDeadBodies";
		precondition38.description = DUPLICANTS.CHORES.PRECONDITIONS.NO_DEAD_BODIES;
		precondition38.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return Components.LiveMinionIdentities.Count == Components.MinionIdentities.Count;
		};
		this.NoDeadBodies = precondition38;
		Chore.Precondition precondition39 = default(Chore.Precondition);
		precondition39.id = "NotCurrentlyPeeing";
		precondition39.description = DUPLICANTS.CHORES.PRECONDITIONS.CURRENTLY_PEEING;
		precondition39.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			bool flag = true;
			Chore currentChore3 = context.consumerState.choreDriver.GetCurrentChore();
			if (currentChore3 != null)
			{
				string id = currentChore3.choreType.Id;
				flag = id != Db.Get().ChoreTypes.BreakPee.Id && id != Db.Get().ChoreTypes.Pee.Id;
			}
			return flag;
		};
		this.NotCurrentlyPeeing = precondition39;
		base..ctor();
	}

	public static ChorePreconditions instance
	{
		get
		{
			if (ChorePreconditions._instance == null)
			{
				ChorePreconditions._instance = new ChorePreconditions();
			}
			return ChorePreconditions._instance;
		}
	}

	public static void DestroyInstance()
	{
		ChorePreconditions._instance = null;
	}

	private static ChorePreconditions _instance;

	public Chore.Precondition IsPreemptable;

	public Chore.Precondition HasUrge;

	public Chore.Precondition IsValid;

	public Chore.Precondition IsPermitted;

	public Chore.Precondition IsAssignedtoMe;

	public Chore.Precondition IsInMyRoom;

	public Chore.Precondition IsPreferredAssignable;

	public Chore.Precondition IsPreferredAssignableOrUrgentBladder;

	public Chore.Precondition IsNotTransferArm;

	public Chore.Precondition HasSkillPerk;

	public Chore.Precondition IsMoreSatisfyingEarly;

	public Chore.Precondition IsMoreSatisfyingLate;

	public Chore.Precondition IsChattable;

	public Chore.Precondition IsNotRedAlert;

	public Chore.Precondition IsScheduledTime;

	public Chore.Precondition CanMoveTo;

	public Chore.Precondition CanPickup;

	public Chore.Precondition IsAwake;

	public Chore.Precondition IsStanding;

	public Chore.Precondition IsMoving;

	public Chore.Precondition IsOffLadder;

	public Chore.Precondition NotInTube;

	public Chore.Precondition ConsumerHasTrait;

	public Chore.Precondition IsOperational;

	public Chore.Precondition IsNotMarkedForDeconstruction;

	public Chore.Precondition IsNotMarkedForDisable;

	public Chore.Precondition IsFunctional;

	public Chore.Precondition IsOverrideTargetNullOrMe;

	public Chore.Precondition NotChoreCreator;

	public Chore.Precondition IsGettingMoreStressed;

	public Chore.Precondition IsAllowedByAutomation;

	public Chore.Precondition HasTag;

	public Chore.Precondition CheckBehaviourPrecondition;

	public Chore.Precondition CanDoWorkerPrioritizable;

	public Chore.Precondition IsExclusivelyAvailableWithOtherChores;

	public Chore.Precondition IsBladderFull;

	public Chore.Precondition IsBladderNotFull;

	public Chore.Precondition NoDeadBodies;

	public Chore.Precondition NotCurrentlyPeeing;
}
