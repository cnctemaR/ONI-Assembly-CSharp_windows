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
		this.lookingforfood.TagTransition(GameTags.Creatures.Hungry, this.satisfied, true).PreBrainUpdate(new Action<SolidConsumerMonitor.Instance>(SolidConsumerMonitor.FindFood));
	}

	private static void FindFood(SolidConsumerMonitor.Instance smi)
	{
		if (smi.IsTargetEdibleValid())
		{
			return;
		}
		smi.ClearTargetEdible();
		Diet diet = smi.diet;
		int num = 0;
		int num2 = 0;
		Grid.PosToXY(smi.gameObject.transform.GetPosition(), out num, out num2);
		num -= 8;
		num2 -= 8;
		bool flag = false;
		if (diet.CanEatPreyCritter)
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi));
			KPrefabID kprefabID = null;
			int num3 = int.MaxValue;
			if (cavityForCell != null)
			{
				foreach (KPrefabID kprefabID2 in cavityForCell.creatures)
				{
					if (!kprefabID2.HasTag(GameTags.Creatures.ReservedByCreature) && diet.GetDietInfo(kprefabID2.PrefabTag) != null)
					{
						int cost = smi.GetCost(kprefabID2.gameObject);
						if (cost != -1 && (cost < num3 || num3 == -1))
						{
							kprefabID = kprefabID2;
							num3 = cost;
						}
					}
					if (kprefabID != null && num3 < 3)
					{
						break;
					}
				}
			}
			if (kprefabID != null)
			{
				smi.SetTargetEdible(kprefabID.gameObject, num3);
				smi.targetEdibleOffset = smi.GetBestEdibleOffset(kprefabID.gameObject);
				flag = true;
			}
		}
		bool flag2 = false;
		if (!flag && diet.CanEatAnySolid)
		{
			ListPool<Storage, SolidConsumerMonitor>.PooledList pooledList = ListPool<Storage, SolidConsumerMonitor>.Allocate();
			int num4 = 32;
			foreach (CreatureFeeder creatureFeeder in Components.CreatureFeeders.GetItems(smi.GetMyWorldId()))
			{
				Vector2I targetFeederCell = creatureFeeder.GetTargetFeederCell();
				if (targetFeederCell.x >= num && targetFeederCell.x <= num + num4 && targetFeederCell.y >= num2 && targetFeederCell.y <= num2 + num4 && !creatureFeeder.StoragesAreEmpty())
				{
					int cost2 = smi.GetCost(Grid.XYToCell(targetFeederCell.x, targetFeederCell.y));
					if (smi.IsCloserThanTargetEdible(cost2))
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
											smi.SetTargetEdible(gameObject, cost2);
											smi.targetEdibleOffset = Vector3.zero;
											flag2 = true;
											break;
										}
									}
								}
								if (flag2)
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
		bool flag3 = false;
		if (!flag && !flag2 && diet.CanEatAnyPlantDirectly)
		{
			ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList2 = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(num, num2, 16, 16, GameScenePartitioner.Instance.plants, pooledList2);
			foreach (ScenePartitionerEntry scenePartitionerEntry in pooledList2)
			{
				KPrefabID kprefabID3 = (KPrefabID)scenePartitionerEntry.obj;
				Diet.Info dietInfo = diet.GetDietInfo(kprefabID3.PrefabTag);
				Vector3 vector = kprefabID3.transform.GetPosition();
				bool flag4 = kprefabID3.HasTag(GameTags.PlantedOnFloorVessel);
				if (flag4)
				{
					vector += SolidConsumerMonitor.PLANT_ON_FLOOR_VESSEL_OFFSET;
				}
				int num5 = smi.GetCost(Grid.PosToCell(vector));
				Vector3 vector2 = Vector3.zero;
				if (smi.IsCloserThanTargetEdible(num5) && !kprefabID3.HasAnyTags(SolidConsumerMonitor.creatureTags) && dietInfo != null)
				{
					if (kprefabID3.HasTag(GameTags.Plant))
					{
						IPlantConsumptionInstructions[] plantConsumptionInstructions = GameUtil.GetPlantConsumptionInstructions(kprefabID3.gameObject);
						if (plantConsumptionInstructions == null || plantConsumptionInstructions.Length == 0)
						{
							continue;
						}
						bool flag5 = false;
						foreach (IPlantConsumptionInstructions plantConsumptionInstructions2 in plantConsumptionInstructions)
						{
							if (plantConsumptionInstructions2.CanPlantBeEaten() && dietInfo.foodType == plantConsumptionInstructions2.GetDietFoodType())
							{
								CellOffset[] allowedOffsets = plantConsumptionInstructions2.GetAllowedOffsets();
								if (allowedOffsets != null)
								{
									num5 = -1;
									foreach (CellOffset cellOffset in allowedOffsets)
									{
										int cost3 = smi.GetCost(Grid.OffsetCell(Grid.PosToCell(vector), cellOffset));
										if (cost3 != -1 && (num5 == -1 || cost3 < num5))
										{
											num5 = cost3;
											vector2 = cellOffset.ToVector3();
										}
									}
									if (num5 != -1)
									{
										flag5 = true;
										break;
									}
								}
								else
								{
									flag5 = true;
								}
							}
						}
						if (!flag5)
						{
							continue;
						}
					}
					smi.SetTargetEdible(kprefabID3.gameObject, num5);
					smi.targetEdibleOffset = vector2 + (flag4 ? SolidConsumerMonitor.PLANT_ON_FLOOR_VESSEL_OFFSET : Vector3.zero);
					flag3 = true;
				}
			}
			pooledList2.Recycle();
		}
		if (!flag && !flag2 && !flag3 && diet.CanEatAnySolid)
		{
			bool flag6 = false;
			ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList3 = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(num, num2, 16, 16, GameScenePartitioner.Instance.pickupablesLayer, pooledList3);
			foreach (ScenePartitionerEntry scenePartitionerEntry2 in pooledList3)
			{
				Pickupable pickupable = (Pickupable)scenePartitionerEntry2.obj;
				KPrefabID kprefabID4 = pickupable.KPrefabID;
				if (!kprefabID4.HasAnyTags(SolidConsumerMonitor.creatureTags) && diet.GetDietInfo(kprefabID4.PrefabTag) != null)
				{
					bool flag7;
					smi.ProcessEdible(pickupable.gameObject, out flag7);
					smi.targetEdibleOffset = Vector3.zero;
					flag6 = flag6 || flag7;
				}
			}
			pooledList3.Recycle();
		}
	}

	public static Vector3 PLANT_ON_FLOOR_VESSEL_OFFSET = Vector3.down;

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

		public Vector3[] possibleEatPositionOffsets = new Vector3[] { Vector3.zero };

		public Vector2 navigatorSize = Vector2.one;
	}

	public new class Instance : GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, SolidConsumerMonitor.Def def)
			: base(master, def)
		{
			this.diet = DietManager.Instance.GetPrefabDiet(base.gameObject);
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
			if (this.targetEdible == null)
			{
				return false;
			}
			int cost = this.GetCost(Grid.PosToCell(this.targetEdible.transform.GetPosition() + this.targetEdibleOffset));
			return cost != -1 && cost <= this.targetEdibleCost + 4;
		}

		public void ClearTargetEdible()
		{
			this.targetEdibleCost = -1;
			this.targetEdible = null;
			this.targetEdibleOffset = Vector3.zero;
		}

		public bool ProcessEdible(GameObject edible, out bool isReachable)
		{
			int cost = this.GetCost(edible);
			isReachable = cost != -1;
			if (cost != -1 && (cost < this.targetEdibleCost || this.targetEdibleCost == -1))
			{
				this.SetTargetEdible(edible, cost);
				return true;
			}
			return false;
		}

		public void SetTargetEdible(GameObject gameObject, int cost)
		{
			if (this.targetEdible == gameObject)
			{
				return;
			}
			this.targetEdibleCost = cost;
			this.targetEdible = gameObject;
		}

		public int GetCost(GameObject edible)
		{
			return this.GetCost(Grid.PosToCell(edible.transform.GetPosition() + base.smi.GetBestEdibleOffset(edible)));
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
			Diet.Info dietInfo = this.diet.GetDietInfo(kprefabID.PrefabTag);
			if (dietInfo == null)
			{
				return;
			}
			AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(base.smi.gameObject);
			string properName = kprefabID.GetProperName();
			PopFXManager.Instance.SpawnFX(global::Def.GetUISprite(kprefabID.gameObject, "ui", false).first, PopFXManager.Instance.sprite_Negative, properName, kprefabID.transform, Vector3.zero, 1.5f, true, false, false);
			float num = amountInstance.GetMax() - amountInstance.value;
			float num2 = dietInfo.ConvertCaloriesToConsumptionMass(num);
			IPlantConsumptionInstructions plantConsumptionInstructions = null;
			foreach (IPlantConsumptionInstructions plantConsumptionInstructions3 in GameUtil.GetPlantConsumptionInstructions(kprefabID.gameObject))
			{
				if (dietInfo.foodType == plantConsumptionInstructions3.GetDietFoodType())
				{
					plantConsumptionInstructions = plantConsumptionInstructions3;
				}
			}
			float num3;
			if (plantConsumptionInstructions != null)
			{
				num2 = plantConsumptionInstructions.ConsumePlant(num2);
				num3 = dietInfo.ConvertConsumptionMassToCalories(num2);
			}
			else if (dietInfo.foodType == Diet.Info.FoodType.EatPrey || dietInfo.foodType == Diet.Info.FoodType.EatButcheredPrey)
			{
				float num4 = this.diet.AvailableCaloriesInPrey(kprefabID.PrefabTag);
				float num5 = Mathf.Clamp(1f - num / num4, 0f, 1f);
				if (num5 > 0f)
				{
					Butcherable component2 = kprefabID.GetComponent<Butcherable>();
					if (component2 != null)
					{
						component2.CreateDrops(num5);
					}
				}
				component.Mass = 0f;
				num3 = Mathf.Min(num, num4);
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
				num3 = dietInfo.ConvertConsumptionMassToCalories(num2);
			}
			Boxed<CreatureCalorieMonitor.CaloriesConsumedEvent> boxed = Boxed<CreatureCalorieMonitor.CaloriesConsumedEvent>.Get(new CreatureCalorieMonitor.CaloriesConsumedEvent
			{
				tag = kprefabID.PrefabTag,
				calories = num3
			});
			base.Trigger(-2038961714, boxed);
			boxed.Release();
			this.targetEdible = null;
		}

		public string[] GetTargetEdibleEatAnims()
		{
			return this.diet.GetDietInfo(this.targetEdible.PrefabID()).eatAnims;
		}

		public Vector3 GetBestEdibleOffset(GameObject edible)
		{
			int num = int.MaxValue;
			Vector3 vector = Vector3.zero;
			foreach (Vector3 vector2 in base.def.possibleEatPositionOffsets)
			{
				Vector3 vector3 = edible.transform.position + vector2;
				if (vector2.x > 0f)
				{
					vector3 += new Vector3(base.def.navigatorSize.x / 2f, 0f, 0f);
				}
				else if (vector2.x < 0f)
				{
					vector3 -= new Vector3(base.def.navigatorSize.x / 2f, 0f, 0f);
				}
				if (vector2.y > 0f)
				{
					vector3 += new Vector3(0f, base.def.navigatorSize.y / 2f, 0f);
				}
				else if (vector2.y < 0f)
				{
					vector3 -= new Vector3(0f, base.def.navigatorSize.y / 2f, 0f);
				}
				int navigationCost = this.navigator.GetNavigationCost(Grid.PosToCell(vector3));
				if (navigationCost != -1 && navigationCost < num)
				{
					num = navigationCost;
					vector = vector2;
				}
			}
			return vector;
		}

		private const int RECALC_THRESHOLD = 4;

		public GameObject targetEdible;

		public Vector3 targetEdibleOffset;

		private int targetEdibleCost;

		[MyCmpGet]
		private Navigator navigator;

		[MyCmpGet]
		private DrowningMonitor drowningMonitor;

		public Diet diet;
	}
}
