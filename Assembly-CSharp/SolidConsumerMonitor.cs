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
		this.lookingforfood.TagTransition(GameTags.Creatures.Hungry, this.satisfied, true).PreBrainUpdate(new Action<SolidConsumerMonitor.Instance>(SolidConsumerMonitor.FindFood));
	}

	[Conditional("DETAILED_SOLID_CONSUMER_MONITOR_PROFILE")]
	private static void BeginDetailedSample(string region_name)
	{
	}

	[Conditional("DETAILED_SOLID_CONSUMER_MONITOR_PROFILE")]
	private static void EndDetailedSample(string region_name)
	{
	}

	private static void FindFood(SolidConsumerMonitor.Instance smi)
	{
		if (smi.IsTargetEdibleValid())
		{
			return;
		}
		smi.ClearTargetEdible();
		Diet diet = smi.def.diet;
		int num = 0;
		int num2 = 0;
		Grid.PosToXY(smi.gameObject.transform.GetPosition(), out num, out num2);
		num -= 8;
		num2 -= 8;
		bool flag = false;
		if (!diet.eatsPlantsDirectly)
		{
			ListPool<Storage, SolidConsumerMonitor>.PooledList pooledList = ListPool<Storage, SolidConsumerMonitor>.Allocate();
			int num3 = 32;
			foreach (CreatureFeeder creatureFeeder in Components.CreatureFeeders.GetItems(smi.GetMyWorldId()))
			{
				Vector2I targetFeederCell = creatureFeeder.GetTargetFeederCell();
				if (targetFeederCell.x >= num && targetFeederCell.x <= num + num3 && targetFeederCell.y >= num2 && targetFeederCell.y <= num2 + num3 && !creatureFeeder.StoragesAreEmpty())
				{
					int cost = smi.GetCost(Grid.XYToCell(targetFeederCell.x, targetFeederCell.y));
					if (smi.IsCloserThanTargetEdible(cost))
					{
						foreach (Storage storage in creatureFeeder.storages)
						{
							if (!(storage == null) && !storage.IsEmpty() && smi.GetCost(Grid.PosToCell(storage.items[0])) != -1)
							{
								foreach (GameObject gameObject in storage.items)
								{
									if (!(gameObject == null))
									{
										KPrefabID component = gameObject.GetComponent<KPrefabID>();
										if (!component.HasAnyTags(SolidConsumerMonitor.creatureTags) && diet.GetDietInfo(component.PrefabTag) != null)
										{
											smi.SetTargetEdible(gameObject, cost);
											flag = true;
											break;
										}
									}
								}
								if (flag)
								{
									break;
								}
							}
						}
					}
				}
			}
			pooledList.Recycle();
		}
		if (diet.eatsPlantsDirectly)
		{
			ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList2 = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(num, num2, 16, 16, GameScenePartitioner.Instance.plants, pooledList2);
			foreach (ScenePartitionerEntry scenePartitionerEntry in pooledList2)
			{
				KPrefabID kprefabID = (KPrefabID)scenePartitionerEntry.obj;
				int cost2 = smi.GetCost(kprefabID.gameObject);
				if (smi.IsCloserThanTargetEdible(cost2) && !kprefabID.HasAnyTags(SolidConsumerMonitor.creatureTags) && diet.GetDietInfo(kprefabID.PrefabTag) != null)
				{
					if (kprefabID.HasTag(GameTags.Plant))
					{
						float num4 = 0.25f;
						float num5 = 0f;
						BuddingTrunk component2 = kprefabID.GetComponent<BuddingTrunk>();
						if (component2)
						{
							num5 = component2.GetMaxBranchMaturity();
						}
						else
						{
							AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(kprefabID);
							if (amountInstance != null)
							{
								num5 = amountInstance.value / amountInstance.GetMax();
							}
						}
						if (num5 < num4)
						{
							continue;
						}
					}
					smi.SetTargetEdible(kprefabID.gameObject, cost2);
				}
			}
			pooledList2.Recycle();
			return;
		}
		if (smi.CanSearchForPickupables(flag))
		{
			bool flag2 = false;
			ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList3 = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(num, num2, 16, 16, GameScenePartitioner.Instance.pickupablesLayer, pooledList3);
			foreach (ScenePartitionerEntry scenePartitionerEntry2 in pooledList3)
			{
				Pickupable pickupable = (Pickupable)scenePartitionerEntry2.obj;
				KPrefabID kprefabID2 = pickupable.KPrefabID;
				if (!kprefabID2.HasAnyTags(SolidConsumerMonitor.creatureTags) && diet.GetDietInfo(kprefabID2.PrefabTag) != null)
				{
					bool flag3;
					smi.ProcessEdible(pickupable.gameObject, out flag3);
					flag2 = flag2 || flag3;
				}
			}
			pooledList3.Recycle();
		}
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

		public bool CanSearchForPickupables(bool foodAtFeeder)
		{
			return !foodAtFeeder;
		}

		public bool IsCloserThanTargetEdible(int cost)
		{
			return cost != -1 && (cost < this.targetEdibleCost || this.targetEdibleCost == -1);
		}

		public bool IsTargetEdibleValid()
		{
			if (this.targetEdible == null || this.targetEdible.HasTag(GameTags.Creatures.ReservedByCreature))
			{
				return false;
			}
			int cost = this.GetCost(this.targetEdible);
			return cost != -1 && this.targetEdibleCost <= cost + 4;
		}

		public void ClearTargetEdible()
		{
			this.targetEdibleCost = -1;
			this.targetEdible = null;
		}

		public bool ProcessEdible(GameObject edible, out bool isReachable)
		{
			int cost = this.GetCost(edible);
			isReachable = cost != -1;
			if (cost != -1 && (cost < this.targetEdibleCost || this.targetEdibleCost == -1))
			{
				this.targetEdibleCost = cost;
				this.targetEdible = edible.gameObject;
				return true;
			}
			return false;
		}

		public void SetTargetEdible(GameObject gameObject, int cost)
		{
			this.targetEdibleCost = cost;
			this.targetEdible = gameObject;
		}

		public int GetCost(GameObject edible)
		{
			return this.GetCost(Grid.PosToCell(edible.transform.GetPosition()));
		}

		public int GetCost(int cell)
		{
			if (this.drowningMonitor != null && this.drowningMonitor.canDrownToDeath && !this.drowningMonitor.livesUnderWater && !this.drowningMonitor.IsCellSafe(cell))
			{
				return -1;
			}
			return this.navigator.GetNavigationCost(cell);
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
					AmountInstance amountInstance2 = Db.Get().Amounts.Maturity.Lookup(component2.gameObject);
					float growthUnitToMaturityRatio = this.GetGrowthUnitToMaturityRatio(amountInstance2.GetMax(), kprefabID);
					float num3 = amountInstance2.value * growthUnitToMaturityRatio;
					num2 = Mathf.Min(num2, num3);
					component2.ConsumeGrowthUnits(num2, growthUnitToMaturityRatio);
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
			float num4 = dietInfo.ConvertConsumptionMassToCalories(num2);
			CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = new CreatureCalorieMonitor.CaloriesConsumedEvent
			{
				tag = kprefabID.PrefabTag,
				calories = num4
			};
			base.Trigger(-2038961714, caloriesConsumedEvent);
			this.targetEdible = null;
		}

		private float GetGrowthUnitToMaturityRatio(float maturityMax, KPrefabID prefab_id)
		{
			ResourceSet<Trait> traits = Db.Get().traits;
			Tag prefabTag = prefab_id.PrefabTag;
			Trait trait = traits.Get(prefabTag.ToString() + "Original");
			if (trait != null)
			{
				AttributeModifier attributeModifier = trait.SelfModifiers.Find((AttributeModifier match) => match.AttributeId == "MaturityMax");
				if (attributeModifier != null)
				{
					return attributeModifier.Value / maturityMax;
				}
			}
			return 1f;
		}

		private const int RECALC_THRESHOLD = 4;

		public GameObject targetEdible;

		private int targetEdibleCost;

		[MyCmpGet]
		private Navigator navigator;

		[MyCmpGet]
		private DrowningMonitor drowningMonitor;
	}
}
