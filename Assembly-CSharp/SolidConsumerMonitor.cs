using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		this.lookingforfood.TagTransition(GameTags.Creatures.Hungry, this.satisfied, true).Update(new Action<SolidConsumerMonitor.Instance, float>(SolidConsumerMonitor.FindFood), UpdateRate.SIM_4000ms, true);
	}

	[Conditional("DETAILED_SOLID_CONSUMER_MONITOR_PROFILE")]
	private static void BeginDetailedSample(string region_name)
	{
	}

	[Conditional("DETAILED_SOLID_CONSUMER_MONITOR_PROFILE")]
	private static void EndDetailedSample(string region_name)
	{
	}

	private static void FindFood(SolidConsumerMonitor.Instance smi, float dt)
	{
		ListPool<GameObject, SolidConsumerMonitor>.PooledList pooledList = ListPool<GameObject, SolidConsumerMonitor>.Allocate();
		Diet diet = smi.def.diet;
		int num = 0;
		int num2 = 0;
		Grid.PosToXY(smi.gameObject.transform.GetPosition(), out num, out num2);
		num -= 8;
		num2 -= 8;
		ListPool<Storage, SolidConsumerMonitor>.PooledList pooledList2 = ListPool<Storage, SolidConsumerMonitor>.Allocate();
		if (!diet.eatsPlantsDirectly)
		{
			foreach (CreatureFeeder creatureFeeder in Components.CreatureFeeders.GetItems(smi.GetMyWorldId()))
			{
				int num3;
				int num4;
				Grid.PosToXY(creatureFeeder.transform.GetPosition(), out num3, out num4);
				if (num3 >= num && num3 <= num + 16 && num4 >= num2 && num4 <= num2 + 16)
				{
					creatureFeeder.GetComponents<Storage>(pooledList2);
					foreach (Storage storage in pooledList2)
					{
						if (!(storage == null))
						{
							foreach (GameObject gameObject in storage.items)
							{
								if (!(gameObject == null))
								{
									KPrefabID component = gameObject.GetComponent<KPrefabID>();
									if (!component.HasAnyTags(SolidConsumerMonitor.creatureTags) && diet.GetDietInfo(component.PrefabTag) != null)
									{
										pooledList.Add(gameObject);
									}
								}
							}
						}
					}
				}
			}
		}
		pooledList2.Recycle();
		ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList3 = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
		if (diet.eatsPlantsDirectly)
		{
			GameScenePartitioner.Instance.GatherEntries(num, num2, 16, 16, GameScenePartitioner.Instance.plants, pooledList3);
			using (List<ScenePartitionerEntry>.Enumerator enumerator4 = pooledList3.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					ScenePartitionerEntry scenePartitionerEntry = enumerator4.Current;
					KPrefabID kprefabID = (KPrefabID)scenePartitionerEntry.obj;
					if (!kprefabID.HasAnyTags(SolidConsumerMonitor.creatureTags) && diet.GetDietInfo(kprefabID.PrefabTag) != null)
					{
						if (kprefabID.HasTag(GameTags.Plant))
						{
							float num5 = 0.25f;
							float num6 = 0f;
							BuddingTrunk component2 = kprefabID.GetComponent<BuddingTrunk>();
							if (component2)
							{
								num6 = component2.GetMaxBranchMaturity();
							}
							else
							{
								AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(kprefabID);
								if (amountInstance != null)
								{
									num6 = amountInstance.value / amountInstance.GetMax();
								}
							}
							if (num6 < num5)
							{
								continue;
							}
						}
						pooledList.Add(kprefabID.gameObject);
					}
				}
				goto IL_0306;
			}
		}
		GameScenePartitioner.Instance.GatherEntries(num, num2, 16, 16, GameScenePartitioner.Instance.pickupablesLayer, pooledList3);
		foreach (ScenePartitionerEntry scenePartitionerEntry2 in pooledList3)
		{
			Pickupable pickupable = (Pickupable)scenePartitionerEntry2.obj;
			KPrefabID component3 = pickupable.GetComponent<KPrefabID>();
			if (!component3.HasAnyTags(SolidConsumerMonitor.creatureTags) && diet.GetDietInfo(component3.PrefabTag) != null)
			{
				pooledList.Add(pickupable.gameObject);
			}
		}
		IL_0306:
		pooledList3.Recycle();
		Navigator component4 = smi.GetComponent<Navigator>();
		DrowningMonitor component5 = smi.GetComponent<DrowningMonitor>();
		bool flag = component5 != null && component5.canDrownToDeath && !component5.livesUnderWater;
		smi.targetEdible = null;
		int num7 = -1;
		foreach (GameObject gameObject2 in pooledList)
		{
			int num8 = Grid.PosToCell(gameObject2.transform.GetPosition());
			if (!flag || component5.IsCellSafe(num8))
			{
				int navigationCost = component4.GetNavigationCost(num8);
				if (navigationCost != -1 && (navigationCost < num7 || num7 == -1))
				{
					num7 = navigationCost;
					smi.targetEdible = gameObject2.gameObject;
				}
			}
		}
		pooledList.Recycle();
	}

	private GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.State satisfied;

	private GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.State lookingforfood;

	private static Tag[] creatureTags = new Tag[]
	{
		GameTags.Creatures.ReservedByCreature,
		GameTags.CreatureBrain
	};

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
			Diet.Info dietInfo = base.def.diet.GetDietInfo(kprefabID.PrefabTag);
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
				BuddingTrunk component3 = kprefabID.GetComponent<BuddingTrunk>();
				if (component3)
				{
					float maxBranchMaturity = component3.GetMaxBranchMaturity();
					num2 = Mathf.Min(num2, maxBranchMaturity);
					component3.ConsumeMass(num2);
				}
				else
				{
					float value = Db.Get().Amounts.Maturity.Lookup(component2.gameObject).value;
					num2 = Mathf.Min(num2, value);
					component2.ConsumeMass(num2);
				}
			}
			else
			{
				num2 = Mathf.Min(num2, component.Mass);
				component.Mass -= num2;
				Pickupable component4 = component.GetComponent<Pickupable>();
				if (component4.storage != null)
				{
					component4.storage.Trigger(-1452790913, base.gameObject);
					component4.storage.Trigger(-1697596308, base.gameObject);
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
