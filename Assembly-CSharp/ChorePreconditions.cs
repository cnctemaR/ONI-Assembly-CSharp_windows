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
			return context.isAttemptingOverride || context.chore.IsPreemptable || context.chore.driver == null;
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
		precondition3.id = "CanPreempt";
		precondition3.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.chore.IsPreemptable;
		};
		ChorePreconditions.CanPreempt = precondition3;
		Chore.Precondition precondition4 = default(Chore.Precondition);
		precondition4.id = "IsValid";
		precondition4.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.chore.IsValid();
		};
		ChorePreconditions.IsValid = precondition4;
		Chore.Precondition precondition5 = default(Chore.Precondition);
		precondition5.id = "IsPermitted";
		precondition5.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.consumer.IsPermittedOrEnabled(context.chore);
		};
		ChorePreconditions.IsPermitted = precondition5;
		Chore.Precondition precondition6 = default(Chore.Precondition);
		precondition6.id = "IsAssignedToMe";
		precondition6.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			IAssignable assignable = (IAssignable)data;
			return assignable.Assignable.assignee != null && assignable.Assignable.assignee.gameObject == context.consumer.gameObject;
		};
		ChorePreconditions.IsAssignedtoMe = precondition6;
		Chore.Precondition precondition7 = default(Chore.Precondition);
		precondition7.id = "IsCorrectRegion";
		precondition7.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Tag tag = (Tag)data;
			RequiresRegion component = context.chore.gameObject.GetComponent<RequiresRegion>();
			return component == null || (component.OwnerRegion != null && component.OwnerRegion.RegionTag == tag);
		};
		ChorePreconditions.IsRegionValid = precondition7;
		Chore.Precondition precondition8 = default(Chore.Precondition);
		precondition8.id = "IsMoreSatisfying";
		precondition8.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.isAttemptingOverride)
			{
				return true;
			}
			Chore currentChore = context.consumer.GetComponent<ChoreDriver>().GetCurrentChore();
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
		ChorePreconditions.IsMoreSatisfying = precondition8;
		Chore.Precondition precondition9 = default(Chore.Precondition);
		precondition9.id = "CanChat";
		precondition9.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			KMonoBehaviour kmonoBehaviour = (KMonoBehaviour)data;
			return !(context.consumer == null) && !(kmonoBehaviour == null) && context.consumer.GetComponent<Navigator>().CanReach(kmonoBehaviour.GetComponent<Chattable>());
		};
		ChorePreconditions.IsChattable = precondition9;
		Chore.Precondition precondition10 = default(Chore.Precondition);
		precondition10.id = "IsNotRedAlert";
		precondition10.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !RedAlertManager.Instance.Get().IsOn();
		};
		ChorePreconditions.IsNotRedAlert = precondition10;
		Chore.Precondition precondition11 = default(Chore.Precondition);
		precondition11.id = "IsScheduledTime";
		precondition11.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			ScheduleBlockType scheduleBlockType = (ScheduleBlockType)data;
			Schedulable component2 = context.consumer.GetComponent<Schedulable>();
			return false || component2.IsAllowed(scheduleBlockType);
		};
		ChorePreconditions.IsScheduledTime = precondition11;
		Chore.Precondition precondition12 = default(Chore.Precondition);
		precondition12.id = "CanMoveTo";
		precondition12.fn = delegate(ref Chore.Precondition.Context context, object data)
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
			int navigationCost = context.consumer.GetComponent<Navigator>().GetNavigationCost(workable);
			if (navigationCost != PathProber.InvalidCost)
			{
				context.cost += navigationCost;
				return true;
			}
			return false;
		};
		ChorePreconditions.CanMoveTo = precondition12;
		Chore.Precondition precondition13 = default(Chore.Precondition);
		precondition13.id = "CanPickup";
		precondition13.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Pickupable pickupable = (Pickupable)data;
			return !(pickupable == null) && !(context.consumer == null) && pickupable.CouldBePickedUp(context.consumer.gameObject) && context.consumer.GetComponent<Navigator>().CanReach(pickupable);
		};
		ChorePreconditions.CanPickup = precondition13;
		Chore.Precondition precondition14 = default(Chore.Precondition);
		precondition14.id = "ConsumerHasTrait";
		precondition14.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			string text = (string)data;
			Traits component3 = context.consumer.GetComponent<Traits>();
			return !(component3 == null) && component3.HasTrait(text);
		};
		ChorePreconditions.ConsumerHasTrait = precondition14;
		Chore.Precondition precondition15 = default(Chore.Precondition);
		precondition15.id = "IsOperational";
		precondition15.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject = (GameObject)data;
			Operational component4 = gameObject.GetComponent<Operational>();
			return component4.IsOperational;
		};
		ChorePreconditions.IsOperational = precondition15;
		Chore.Precondition precondition16 = default(Chore.Precondition);
		precondition16.id = "IsMarkedForDeconstruction";
		precondition16.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject2 = (GameObject)data;
			Deconstructable component5 = gameObject2.GetComponent<Deconstructable>();
			return component5 == null || !component5.IsMarkedForDeconstruction();
		};
		ChorePreconditions.IsMarkedForDeconstruction = precondition16;
		Chore.Precondition precondition17 = default(Chore.Precondition);
		precondition17.id = "IsMarkedForDisable";
		precondition17.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject3 = (GameObject)data;
			BuildingEnabledButton component6 = gameObject3.GetComponent<BuildingEnabledButton>();
			return component6 == null || (component6.IsEnabled && !component6.WaitingForDisable);
		};
		ChorePreconditions.IsMarkedForDisable = precondition17;
		Chore.Precondition precondition18 = default(Chore.Precondition);
		precondition18.id = "IsFunctional";
		precondition18.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject4 = (GameObject)data;
			Operational component7 = gameObject4.GetComponent<Operational>();
			return component7.IsFunctional;
		};
		ChorePreconditions.IsFunctional = precondition18;
		Chore.Precondition precondition19 = default(Chore.Precondition);
		precondition19.id = "IsOverrideTargetNullOrMe";
		precondition19.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.isAttemptingOverride || context.chore.overrideTarget == null || context.chore.overrideTarget == context.consumer;
		};
		ChorePreconditions.IsOverrideTargetNullOrMe = precondition19;
		Chore.Precondition precondition20 = default(Chore.Precondition);
		precondition20.id = "NotChoreCreator";
		precondition20.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject5 = (GameObject)data;
			return !(context.consumer == null) && !(context.consumer.gameObject == gameObject5);
		};
		ChorePreconditions.NotChoreCreator = precondition20;
	}

	public static Chore.Precondition ChoreDriverIsNull;

	public static Chore.Precondition HasUrge;

	public static Chore.Precondition CanPreempt;

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

	public static Chore.Precondition ConsumerHasTrait;

	public static Chore.Precondition IsOperational;

	public static Chore.Precondition IsMarkedForDeconstruction;

	public static Chore.Precondition IsMarkedForDisable;

	public static Chore.Precondition IsFunctional;

	public static Chore.Precondition IsOverrideTargetNullOrMe;

	public static Chore.Precondition NotChoreCreator;
}
