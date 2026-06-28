using System;
using Klei.AI;
using UnityEngine;

public static class ChorePreconditions
{
	// Note: this type is marked as 'beforefieldinit'.
	static ChorePreconditions()
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "ChoreDriverIsNull";
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.isAttemptingOverride || context.chore.CanPreempt(context) || context.chore.driver == null;
		};
		ChorePreconditions.ChoreDriverIsNull = precondition;
		Chore.Precondition precondition2 = default(Chore.Precondition);
		precondition2.id = "HasUrge";
		precondition2.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.chore.choreType.urge == null)
			{
				return true;
			}
			foreach (Urge urge in context.consumer.GetUrges())
			{
				if (context.chore.SatisfiesUrge(urge))
				{
					return true;
				}
			}
			return false;
		};
		ChorePreconditions.HasUrge = precondition2;
		Chore.Precondition precondition3 = default(Chore.Precondition);
		precondition3.id = "IsValid";
		precondition3.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.chore.IsValid();
		};
		ChorePreconditions.IsValid = precondition3;
		Chore.Precondition precondition4 = default(Chore.Precondition);
		precondition4.id = "IsPermitted";
		precondition4.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.consumer.IsPermittedOrEnabled(context.chore);
		};
		ChorePreconditions.IsPermitted = precondition4;
		Chore.Precondition precondition5 = default(Chore.Precondition);
		precondition5.id = "IsAssignedToMe";
		precondition5.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Assignable assignable = (Assignable)data;
			return assignable.assignee != null && assignable.assignee.gameObject == context.consumer.gameObject;
		};
		ChorePreconditions.IsAssignedtoMe = precondition5;
		Chore.Precondition precondition6 = default(Chore.Precondition);
		precondition6.id = "IsCorrectRegion";
		precondition6.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Tag tag = (Tag)data;
			RequiresRegion component = context.chore.gameObject.GetComponent<RequiresRegion>();
			return component == null || (component.OwnerRegion != null && component.OwnerRegion.RegionTag == tag);
		};
		ChorePreconditions.IsRegionValid = precondition6;
		Chore.Precondition precondition7 = default(Chore.Precondition);
		precondition7.id = "IsMoreSatisfying";
		precondition7.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.isAttemptingOverride)
			{
				return true;
			}
			Chore currentChore = context.consumer.choreDriver.GetCurrentChore();
			if (currentChore == null)
			{
				return true;
			}
			if (context.masterPriority == currentChore.masterPriority)
			{
				return context.priority > currentChore.choreType.priority;
			}
			return context.masterPriority > currentChore.masterPriority;
		};
		ChorePreconditions.IsMoreSatisfying = precondition7;
		Chore.Precondition precondition8 = default(Chore.Precondition);
		precondition8.id = "CanChat";
		precondition8.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			KMonoBehaviour kmonoBehaviour = (KMonoBehaviour)data;
			return !(context.consumer == null) && !(kmonoBehaviour == null) && context.consumer.navigator.CanReach(kmonoBehaviour.GetComponent<Chattable>());
		};
		ChorePreconditions.IsChattable = precondition8;
		Chore.Precondition precondition9 = default(Chore.Precondition);
		precondition9.id = "IsNotRedAlert";
		precondition9.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !RedAlertManager.Instance.Get().IsOn();
		};
		ChorePreconditions.IsNotRedAlert = precondition9;
		Chore.Precondition precondition10 = default(Chore.Precondition);
		precondition10.id = "IsScheduledTime";
		precondition10.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			ScheduleBlockType scheduleBlockType = (ScheduleBlockType)data;
			Schedulable component2 = context.consumer.GetComponent<Schedulable>();
			return false || component2.IsAllowed(scheduleBlockType);
		};
		ChorePreconditions.IsScheduledTime = precondition10;
		Chore.Precondition precondition11 = default(Chore.Precondition);
		precondition11.id = "CanMoveTo";
		precondition11.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Workable workable = (Workable)data;
			if (context.consumer == null)
			{
				return false;
			}
			if (workable == null)
			{
				return false;
			}
			int navigationCost = context.consumer.navigator.GetNavigationCost(workable);
			if (navigationCost != PathProber.InvalidCost)
			{
				context.cost += navigationCost;
				return true;
			}
			return false;
		};
		ChorePreconditions.CanMoveTo = precondition11;
		Chore.Precondition precondition12 = default(Chore.Precondition);
		precondition12.id = "CanPickup";
		precondition12.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Pickupable pickupable = (Pickupable)data;
			return !(pickupable == null) && !(context.consumer == null) && pickupable.CouldBePickedUp(context.consumer.gameObject) && context.consumer.navigator.CanReach(pickupable);
		};
		ChorePreconditions.CanPickup = precondition12;
		Chore.Precondition precondition13 = default(Chore.Precondition);
		precondition13.id = "IsAwake";
		precondition13.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.consumer == null)
			{
				return false;
			}
			StaminaMonitor.Instance smi = context.consumer.GetSMI<StaminaMonitor.Instance>();
			return !context.consumer.GetSMI<StaminaMonitor.Instance>().IsInsideState(smi.sm.sleepy.sleeping);
		};
		ChorePreconditions.IsAwake = precondition13;
		Chore.Precondition precondition14 = default(Chore.Precondition);
		precondition14.id = "IsStanding";
		precondition14.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !(context.consumer == null) && context.consumer.navigator.CurrentNavType == NavType.Floor;
		};
		ChorePreconditions.IsStanding = precondition14;
		Chore.Precondition precondition15 = default(Chore.Precondition);
		precondition15.id = "IsMoving";
		precondition15.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !(context.consumer == null) && context.consumer.navigator.IsMoving();
		};
		ChorePreconditions.IsMoving = precondition15;
		Chore.Precondition precondition16 = default(Chore.Precondition);
		precondition16.id = "IsOffLadder";
		precondition16.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !(context.consumer == null) && context.consumer.navigator.CurrentNavType != NavType.Ladder;
		};
		ChorePreconditions.IsOffLadder = precondition16;
		Chore.Precondition precondition17 = default(Chore.Precondition);
		precondition17.id = "ConsumerHasTrait";
		precondition17.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			string text = (string)data;
			Traits component3 = context.consumer.GetComponent<Traits>();
			return !(component3 == null) && component3.HasTrait(text);
		};
		ChorePreconditions.ConsumerHasTrait = precondition17;
		Chore.Precondition precondition18 = default(Chore.Precondition);
		precondition18.id = "IsOperational";
		precondition18.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject = (GameObject)data;
			Operational component4 = gameObject.GetComponent<Operational>();
			return component4.IsOperational;
		};
		ChorePreconditions.IsOperational = precondition18;
		Chore.Precondition precondition19 = default(Chore.Precondition);
		precondition19.id = "IsMarkedForDeconstruction";
		precondition19.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject2 = (GameObject)data;
			Deconstructable component5 = gameObject2.GetComponent<Deconstructable>();
			return component5 == null || !component5.IsMarkedForDeconstruction();
		};
		ChorePreconditions.IsMarkedForDeconstruction = precondition19;
		Chore.Precondition precondition20 = default(Chore.Precondition);
		precondition20.id = "IsMarkedForDisable";
		precondition20.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject3 = (GameObject)data;
			BuildingEnabledButton component6 = gameObject3.GetComponent<BuildingEnabledButton>();
			return component6 == null || (component6.IsEnabled && !component6.WaitingForDisable);
		};
		ChorePreconditions.IsMarkedForDisable = precondition20;
		Chore.Precondition precondition21 = default(Chore.Precondition);
		precondition21.id = "IsFunctional";
		precondition21.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject4 = (GameObject)data;
			Operational component7 = gameObject4.GetComponent<Operational>();
			return component7.IsFunctional;
		};
		ChorePreconditions.IsFunctional = precondition21;
		Chore.Precondition precondition22 = default(Chore.Precondition);
		precondition22.id = "IsOverrideTargetNullOrMe";
		precondition22.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.isAttemptingOverride || context.chore.overrideTarget == null || context.chore.overrideTarget == context.consumer;
		};
		ChorePreconditions.IsOverrideTargetNullOrMe = precondition22;
		Chore.Precondition precondition23 = default(Chore.Precondition);
		precondition23.id = "NotChoreCreator";
		precondition23.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject5 = (GameObject)data;
			return !(context.consumer == null) && !(context.consumer.gameObject == gameObject5);
		};
		ChorePreconditions.NotChoreCreator = precondition23;
		Chore.Precondition precondition24 = default(Chore.Precondition);
		precondition24.id = "IsGettingMoreStressed";
		precondition24.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(context.consumer.gameObject);
			return amountInstance.GetDelta() > 0f;
		};
		ChorePreconditions.IsGettingMoreStressed = precondition24;
	}

	public static Chore.Precondition ChoreDriverIsNull;

	public static Chore.Precondition HasUrge;

	public static Chore.Precondition IsValid;

	public static Chore.Precondition IsPermitted;

	public static Chore.Precondition IsAssignedtoMe;

	public static Chore.Precondition IsRegionValid;

	public static Chore.Precondition IsMoreSatisfying;

	public static Chore.Precondition IsChattable;

	public static Chore.Precondition IsNotRedAlert;

	public static Chore.Precondition IsScheduledTime;

	public static Chore.Precondition CanMoveTo;

	public static Chore.Precondition CanPickup;

	public static Chore.Precondition IsAwake;

	public static Chore.Precondition IsStanding;

	public static Chore.Precondition IsMoving;

	public static Chore.Precondition IsOffLadder;

	public static Chore.Precondition ConsumerHasTrait;

	public static Chore.Precondition IsOperational;

	public static Chore.Precondition IsMarkedForDeconstruction;

	public static Chore.Precondition IsMarkedForDisable;

	public static Chore.Precondition IsFunctional;

	public static Chore.Precondition IsOverrideTargetNullOrMe;

	public static Chore.Precondition NotChoreCreator;

	public static Chore.Precondition IsGettingMoreStressed;
}
