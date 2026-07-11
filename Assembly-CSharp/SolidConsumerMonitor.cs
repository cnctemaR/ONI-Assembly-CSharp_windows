using System;
using Klei.AI;
using UnityEngine;

public class SolidConsumerMonitor : GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.root.EventHandler(GameHashes.EatSolidComplete, delegate(SolidConsumerMonitor.Instance smi, object data)
		{
			smi.OnEatSolidComplete(data);
		}).ToggleBehaviour(GameTags.Creatures.WantsToEat, (SolidConsumerMonitor.Instance smi) => smi.targetEdible != null && !smi.targetEdible.HasTag(GameTags.Creatures.ReservedByCreature), null);
		this.satisfied.TagTransition(GameTags.Creatures.Hungry, this.lookingforfood, false);
		this.lookingforfood.TagTransition(GameTags.Creatures.Hungry, this.satisfied, true).Update(new Action<SolidConsumerMonitor.Instance, float>(SolidConsumerMonitor.FindFood), UpdateRate.SIM_200ms, false);
	}

	private static void FindFood(SolidConsumerMonitor.Instance smi, float dt)
	{
		int num = 0;
		int num2 = 0;
		int num3 = Grid.PosToCell(smi.gameObject.transform.GetPosition());
		Grid.CellToXY(num3, out num, out num2);
		int num4 = 8;
		SolidConsumerMonitor.EdibleIterator edibleIterator = new SolidConsumerMonitor.EdibleIterator(smi.GetComponent<Navigator>(), smi.def.diet);
		foreach (CreatureFeeder creatureFeeder in Components.CreatureFeeders.Items)
		{
			edibleIterator.Iterate(creatureFeeder);
		}
		if (edibleIterator.GetResult() == null)
		{
			GameScenePartitioner.Instance.Iterate<SolidConsumerMonitor.EdibleIterator>(num3, num4, GameScenePartitioner.Instance.pickupablesLayer, ref edibleIterator);
			GameScenePartitioner.Instance.Iterate<SolidConsumerMonitor.EdibleIterator>(num3, num4, GameScenePartitioner.Instance.plants, ref edibleIterator);
		}
		edibleIterator.Cleanup();
		smi.targetEdible = edibleIterator.GetResult();
	}

	private GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.State satisfied;

	private GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.State lookingforfood;

	public class Def : StateMachine.BaseDef
	{
		public Diet diet;
	}

	private struct EdibleIterator : GameScenePartitioner.Iterator
	{
		public EdibleIterator(Navigator navigator, Diet diet)
		{
			this.navigator = navigator;
			this.diet = diet;
			this.result = null;
			this.resultCost = PathProber.InvalidCost;
		}

		public void Iterate(object target_obj)
		{
			KMonoBehaviour kmonoBehaviour = target_obj as KMonoBehaviour;
			if (kmonoBehaviour == null)
			{
				return;
			}
			if (kmonoBehaviour.HasTag(GameTags.Creatures.ReservedByCreature))
			{
				return;
			}
			this.FindEdibleInFeeder(ref this, kmonoBehaviour);
			GameObject gameObject = kmonoBehaviour.gameObject;
			if (gameObject == null)
			{
				return;
			}
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (this.diet.GetDietInfo(component.GetTagBits()) == null)
			{
				return;
			}
			if (component.HasTag(GameTags.Plant))
			{
				AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(component);
				if (amountInstance != null)
				{
					float num = 0.25f;
					if (amountInstance.value / amountInstance.GetMax() < num)
					{
						return;
					}
				}
			}
			int num2 = Grid.PosToCell(gameObject.transform.GetPosition());
			int navigationCost = this.navigator.GetNavigationCost(num2);
			if (navigationCost != PathProber.InvalidCost && (navigationCost < this.resultCost || this.resultCost == PathProber.InvalidCost))
			{
				this.resultCost = navigationCost;
				this.result = gameObject;
			}
		}

		public void Cleanup()
		{
		}

		private void FindEdibleInFeeder(ref SolidConsumerMonitor.EdibleIterator edible_iterator, KMonoBehaviour target)
		{
			if (!target.HasTag(RoomConstraints.ConstraintTags.CreatureFeeder))
			{
				return;
			}
			ListPool<Storage, SolidConsumerMonitor>.PooledList pooledList = ListPool<Storage, SolidConsumerMonitor>.Allocate();
			target.GetComponents<Storage>(pooledList);
			foreach (Storage storage in pooledList)
			{
				if (!(storage == null))
				{
					foreach (GameObject gameObject in storage.items)
					{
						if (!(gameObject == null))
						{
							edible_iterator.Iterate(gameObject.GetComponent<KMonoBehaviour>());
						}
					}
				}
			}
			pooledList.Recycle();
		}

		public GameObject GetResult()
		{
			return this.result;
		}

		private Navigator navigator;

		private Diet diet;

		private GameObject result;

		private int resultCost;
	}

	public new class Instance : GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, SolidConsumerMonitor.Def def)
			: base(master, def)
		{
		}

		public void OnEatSolidComplete(object data)
		{
			KPrefabID kprefabID = data as KPrefabID;
			if (kprefabID == null)
			{
				return;
			}
			PrimaryElement component = kprefabID.GetComponent<PrimaryElement>();
			if (component == null)
			{
				return;
			}
			Diet.Info dietInfo = base.def.diet.GetDietInfo(kprefabID.GetTagBits());
			if (dietInfo == null)
			{
				return;
			}
			AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(base.smi.gameObject);
			string properName = kprefabID.GetProperName();
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, properName, kprefabID.transform, 1.5f, false);
			float num = amountInstance.GetMax() - amountInstance.value;
			float num2 = dietInfo.ConvertCaloriesToConsumptionMass(num);
			Growing component2 = kprefabID.GetComponent<Growing>();
			if (component2 != null)
			{
				AmountInstance amountInstance2 = Db.Get().Amounts.Maturity.Lookup(component2.gameObject);
				float value = amountInstance2.value;
				num2 = Mathf.Min(num2, value);
				amountInstance2.value -= num2;
				kprefabID.Trigger(-1793167409, null);
			}
			else
			{
				num2 = Mathf.Min(num2, component.Mass);
				component.Mass -= num2;
				Pickupable component3 = component.GetComponent<Pickupable>();
				if (component3.storage != null)
				{
					component3.storage.Trigger(-1452790913, base.gameObject);
					component3.storage.Trigger(-1697596308, base.gameObject);
				}
			}
			float num3 = dietInfo.ConvertConsumptionMassToCalories(num2);
			CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = new CreatureCalorieMonitor.CaloriesConsumedEvent
			{
				tag = kprefabID.PrefabTag,
				calories = num3
			};
			base.Trigger(-2038961714, caloriesConsumedEvent);
			this.targetEdible = null;
		}

		public GameObject targetEdible;
	}
}
