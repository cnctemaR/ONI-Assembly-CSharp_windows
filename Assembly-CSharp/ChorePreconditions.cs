using System;
using Klei.AI;
using UnityEngine;

public static class ChorePreconditions
{
	public static Chore.Precondition ChoreDriverIsNull = new Chore.Precondition
	{
		id = "ChoreDriverIsNull",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.isAttemptingOverride || context.chore.CanPreempt(context) || context.chore.driver == null;
		}
	};

	public static Chore.Precondition HasUrge = new Chore.Precondition
	{
		id = "HasUrge",
		fn = delegate(ref Chore.Precondition.Context context, object data)
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
		}
	};

	public static Chore.Precondition IsValid = new Chore.Precondition
	{
		id = "IsValid",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.chore.IsValid();
		}
	};

	public static Chore.Precondition IsPermitted = new Chore.Precondition
	{
		id = "IsPermitted",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.consumer.IsPermittedOrEnabled(context.chore);
		}
	};

	public static Chore.Precondition IsAssignedtoMe = new Chore.Precondition
	{
		id = "IsAssignedToMe",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Assignable assignable = (Assignable)data;
			if (assignable.assignee != null)
			{
				foreach (Ownables ownables in assignable.assignee.GetOwners())
				{
					if (ownables.gameObject == context.consumer.gameObject)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
	};

	public static Chore.Precondition IsPreferredAssignable = new Chore.Precondition
	{
		id = "IsPreferredAssignable",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return Game.Instance.assignmentManager.GetPreferredAssignables(context.consumer.gameObject.GetComponent<Navigator>(), (data as Assignable).slot).Contains(data as Assignable);
		}
	};

	public static Chore.Precondition IsPreferredAssignableOrUrgentBladder = new Chore.Precondition
	{
		id = "IsPreferredAssignableOrUrgent",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (Game.Instance.assignmentManager.GetPreferredAssignables(context.consumer.gameObject.GetComponent<Navigator>(), (data as Assignable).slot).Contains(data as Assignable))
			{
				return true;
			}
			PeeChoreMonitor.Instance smi = context.consumer.gameObject.GetSMI<PeeChoreMonitor.Instance>();
			return smi.IsInsideState(smi.sm.critical);
		}
	};

	public static Chore.Precondition IsRegionValid = new Chore.Precondition
	{
		id = "IsCorrectRegion",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Tag tag = (Tag)data;
			RequiresRegion component = context.chore.gameObject.GetComponent<RequiresRegion>();
			return component == null || (component.OwnerRegion != null && component.OwnerRegion.RegionTag == tag);
		}
	};

	public static Chore.Precondition IsMoreSatisfying = new Chore.Precondition
	{
		id = "IsMoreSatisfying",
		fn = delegate(ref Chore.Precondition.Context context, object data)
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
			if (context.masterPriority.priority_class != currentChore.masterPriority.priority_class)
			{
				return context.masterPriority.priority_class > currentChore.masterPriority.priority_class;
			}
			if (context.masterPriority.priority_value != currentChore.masterPriority.priority_value)
			{
				return context.masterPriority.priority_value > currentChore.masterPriority.priority_value;
			}
			return context.priority > currentChore.choreType.priority;
		}
	};

	public static Chore.Precondition IsChattable = new Chore.Precondition
	{
		id = "CanChat",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			KMonoBehaviour kmonoBehaviour = (KMonoBehaviour)data;
			return !(context.consumer == null) && !(kmonoBehaviour == null) && context.consumer.navigator.CanReach(kmonoBehaviour.GetComponent<Chattable>());
		}
	};

	public static Chore.Precondition IsNotRedAlert = new Chore.Precondition
	{
		id = "IsNotRedAlert",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !RedAlertManager.Instance.Get().IsOn();
		}
	};

	public static Chore.Precondition IsScheduledTime = new Chore.Precondition
	{
		id = "IsScheduledTime",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			ScheduleBlockType scheduleBlockType = (ScheduleBlockType)data;
			Schedulable component2 = context.consumer.GetComponent<Schedulable>();
			return false || component2.IsAllowed(scheduleBlockType);
		}
	};

	public static Chore.Precondition CanMoveTo = new Chore.Precondition
	{
		id = "CanMoveTo",
		fn = delegate(ref Chore.Precondition.Context context, object data)
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
		}
	};

	public static Chore.Precondition CanPickup = new Chore.Precondition
	{
		id = "CanPickup",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Pickupable pickupable = (Pickupable)data;
			return !(pickupable == null) && !(context.consumer == null) && pickupable.CouldBePickedUp(context.consumer.gameObject) && context.consumer.navigator.CanReach(pickupable);
		}
	};

	public static Chore.Precondition IsAwake = new Chore.Precondition
	{
		id = "IsAwake",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.consumer == null)
			{
				return false;
			}
			StaminaMonitor.Instance smi2 = context.consumer.GetSMI<StaminaMonitor.Instance>();
			return !context.consumer.GetSMI<StaminaMonitor.Instance>().IsInsideState(smi2.sm.sleepy.sleeping);
		}
	};

	public static Chore.Precondition IsStanding = new Chore.Precondition
	{
		id = "IsStanding",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !(context.consumer == null) && context.consumer.navigator.CurrentNavType == NavType.Floor;
		}
	};

	public static Chore.Precondition IsMoving = new Chore.Precondition
	{
		id = "IsMoving",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !(context.consumer == null) && context.consumer.navigator.IsMoving();
		}
	};

	public static Chore.Precondition IsOffLadder = new Chore.Precondition
	{
		id = "IsOffLadder",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !(context.consumer == null) && context.consumer.navigator.CurrentNavType != NavType.Ladder && context.consumer.navigator.CurrentNavType != NavType.Pole;
		}
	};

	public static Chore.Precondition ConsumerHasTrait = new Chore.Precondition
	{
		id = "ConsumerHasTrait",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			string text = (string)data;
			Traits component3 = context.consumer.GetComponent<Traits>();
			return !(component3 == null) && component3.HasTrait(text);
		}
	};

	public static Chore.Precondition IsOperational = new Chore.Precondition
	{
		id = "IsOperational",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject = (GameObject)data;
			Operational component4 = gameObject.GetComponent<Operational>();
			return component4.IsOperational;
		}
	};

	public static Chore.Precondition IsMarkedForDeconstruction = new Chore.Precondition
	{
		id = "IsMarkedForDeconstruction",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject2 = (GameObject)data;
			Deconstructable component5 = gameObject2.GetComponent<Deconstructable>();
			return component5 == null || !component5.IsMarkedForDeconstruction();
		}
	};

	public static Chore.Precondition IsMarkedForDisable = new Chore.Precondition
	{
		id = "IsMarkedForDisable",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject3 = (GameObject)data;
			BuildingEnabledButton component6 = gameObject3.GetComponent<BuildingEnabledButton>();
			return component6 == null || (component6.IsEnabled && !component6.WaitingForDisable);
		}
	};

	public static Chore.Precondition IsFunctional = new Chore.Precondition
	{
		id = "IsFunctional",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject4 = (GameObject)data;
			Operational component7 = gameObject4.GetComponent<Operational>();
			return component7.IsFunctional;
		}
	};

	public static Chore.Precondition IsOverrideTargetNullOrMe = new Chore.Precondition
	{
		id = "IsOverrideTargetNullOrMe",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.isAttemptingOverride || context.chore.overrideTarget == null || context.chore.overrideTarget == context.consumer;
		}
	};

	public static Chore.Precondition NotChoreCreator = new Chore.Precondition
	{
		id = "NotChoreCreator",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject5 = (GameObject)data;
			return !(context.consumer == null) && !(context.consumer.gameObject == gameObject5);
		}
	};

	public static Chore.Precondition IsGettingMoreStressed = new Chore.Precondition
	{
		id = "IsGettingMoreStressed",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(context.consumer.gameObject);
			return amountInstance.GetDelta() > 0f;
		}
	};
}
