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
		}).ToggleBehaviour(GameTags.Creatures.WantsToEat, (SolidConsumerMonitor.Instance smi) => smi.targetEdible != null, null);
		this.satisfied.TagTransition(GameTags.Creatures.Hungry, this.lookingforfood, false);
		this.lookingforfood.TagTransition(GameTags.Creatures.Hungry, this.satisfied, true).Update("FindFood", delegate(SolidConsumerMonitor.Instance smi, float dt)
		{
			smi.FindEdible();
		}, UpdateRate.SIM_200ms, false);
	}

	private GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.State satisfied;

	private GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.State lookingforfood;

	public class Def : StateMachine.BaseDef
	{
		public Diet diet;
	}

	public new class Instance : GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, SolidConsumerMonitor.Def def)
			: base(master, def)
		{
		}

		public GameObject targetEdible { get; private set; }

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
			num2 = Mathf.Min(num2, component.Mass);
			component.Mass -= num2;
			float num3 = dietInfo.ConvertConsumptionMassToCalories(num2);
			CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = new CreatureCalorieMonitor.CaloriesConsumedEvent
			{
				tag = kprefabID.PrefabTag,
				calories = num3
			};
			base.Trigger(-2038961714, caloriesConsumedEvent);
			this.targetEdible = null;
		}

		public void FindEdible()
		{
			int num = 0;
			int num2 = 0;
			Grid.CellToXY(Grid.PosToCell(base.gameObject.transform.GetPosition()), out num, out num2);
			int num3 = 8;
			SolidConsumerMonitor.Instance.EdibleIterator edibleIterator = new SolidConsumerMonitor.Instance.EdibleIterator(base.GetComponent<Navigator>(), base.def.diet);
			foreach (CreatureFeeder creatureFeeder in Components.CreatureFeeders)
			{
				edibleIterator.Iterate(creatureFeeder);
			}
			if (edibleIterator.GetResult() == null)
			{
				GameScenePartitioner.Instance.Iterate<SolidConsumerMonitor.Instance.EdibleIterator>(Grid.PosToCell(base.gameObject.transform.GetPosition()), num3, GameScenePartitioner.Instance.pickupablesLayer, ref edibleIterator);
			}
			edibleIterator.Cleanup();
			this.targetEdible = edibleIterator.GetResult();
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
				this.FindEdibleInFeeder(ref this, kmonoBehaviour);
				GameObject gameObject = kmonoBehaviour.gameObject;
				if (gameObject == null)
				{
					return;
				}
				Pickupable component = gameObject.GetComponent<Pickupable>();
				if (component == null)
				{
					return;
				}
				if (this.diet.GetDietInfo(gameObject.GetComponent<KPrefabID>().GetTagBits()) == null)
				{
					return;
				}
				int num = Grid.PosToCell(gameObject.transform.GetPosition());
				int navigationCost = this.navigator.GetNavigationCost(num);
				if (navigationCost != PathProber.InvalidCost && (navigationCost < this.resultCost || this.resultCost == PathProber.InvalidCost))
				{
					this.resultCost = navigationCost;
					this.result = gameObject;
				}
			}

			public void Cleanup()
			{
			}

			private void FindEdibleInFeeder(ref SolidConsumerMonitor.Instance.EdibleIterator edible_iterator, KMonoBehaviour target)
			{
				if (!target.HasTag(RoomConstraints.ConstraintTags.CreatureFeeder))
				{
					return;
				}
				Storage component = target.GetComponent<Storage>();
				if (component == null)
				{
					return;
				}
				foreach (GameObject gameObject in component.items)
				{
					if (!(gameObject == null))
					{
						edible_iterator.Iterate(gameObject.GetComponent<KMonoBehaviour>());
					}
				}
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
	}
}
