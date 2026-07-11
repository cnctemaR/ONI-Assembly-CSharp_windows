using System;
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
		this.lookingforfood.TagTransition(GameTags.Creatures.Hungry, this.satisfied, true).Update(new Action<SolidConsumerMonitor.Instance, float>(SolidConsumerMonitor.FindFood), UpdateRate.SIM_1000ms, true);
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
		ListPool<KMonoBehaviour, SolidConsumerMonitor>.PooledList pooledList = ListPool<KMonoBehaviour, SolidConsumerMonitor>.Allocate();
		ListPool<Storage, SolidConsumerMonitor>.PooledList pooledList2 = ListPool<Storage, SolidConsumerMonitor>.Allocate();
		foreach (CreatureFeeder creatureFeeder in Components.CreatureFeeders.Items)
		{
			creatureFeeder.GetComponents<Storage>(pooledList2);
			foreach (Storage storage in pooledList2)
			{
				if (!(storage == null))
				{
					foreach (GameObject gameObject in storage.items)
					{
						pooledList.Add((!(gameObject != null)) ? null : gameObject.GetComponent<KMonoBehaviour>());
					}
				}
			}
		}
		pooledList2.Recycle();
		int num = 0;
		int num2 = 0;
		Grid.PosToXY(smi.gameObject.transform.GetPosition(), out num, out num2);
		num -= 8;
		num2 -= 8;
		ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList3 = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(num, num2, 16, 16, GameScenePartitioner.Instance.pickupablesLayer, pooledList3);
		GameScenePartitioner.Instance.GatherEntries(num, num2, 16, 16, GameScenePartitioner.Instance.plants, pooledList3);
		foreach (ScenePartitionerEntry scenePartitionerEntry in pooledList3)
		{
			pooledList.Add(scenePartitionerEntry.obj as KMonoBehaviour);
		}
		pooledList3.Recycle();
		Diet diet = smi.def.diet;
		for (int num3 = 0; num3 != pooledList.Count; num3++)
		{
			KMonoBehaviour kmonoBehaviour = pooledList[num3];
			if (!(kmonoBehaviour == null))
			{
				KPrefabID component = kmonoBehaviour.GetComponent<KPrefabID>();
				component.UpdateTagBits();
				if (component.HasAnyTags_AssumeLaundered(ref SolidConsumerMonitor.creatureMask) || diet.GetDietInfo(component.PrefabTag) == null)
				{
					pooledList[num3] = null;
				}
				else if (component.HasAnyTags_AssumeLaundered(ref SolidConsumerMonitor.plantMask))
				{
					float num4 = 0.25f;
					float num5 = 0f;
					BuddingTrunk component2 = component.GetComponent<BuddingTrunk>();
					if (component2)
					{
						num5 = component2.GetMaxBranchMaturity();
					}
					else
					{
						AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(component);
						if (amountInstance != null)
						{
							num5 = amountInstance.value / amountInstance.GetMax();
						}
					}
					if (num5 < num4)
					{
						pooledList[num3] = null;
					}
				}
			}
		}
		Navigator component3 = smi.GetComponent<Navigator>();
		smi.targetEdible = null;
		int num6 = -1;
		foreach (KMonoBehaviour kmonoBehaviour2 in pooledList)
		{
			if (!(kmonoBehaviour2 == null))
			{
				int navigationCost = component3.GetNavigationCost(Grid.PosToCell(kmonoBehaviour2.gameObject.transform.GetPosition()));
				if (navigationCost != -1)
				{
					if (navigationCost < num6 || num6 == -1)
					{
						num6 = navigationCost;
						smi.targetEdible = kmonoBehaviour2.gameObject;
					}
				}
			}
		}
		pooledList.Recycle();
	}

	private GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.State satisfied;

	private GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.State lookingforfood;

	private static TagBits plantMask = new TagBits(GameTags.GrowingPlant);

	private static TagBits creatureMask = new TagBits(new Tag[]
	{
		GameTags.Creatures.ReservedByCreature,
		GameTags.CreatureBrain
	});

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
					component3.ConsumeMass(num2);
				}
				else
				{
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
