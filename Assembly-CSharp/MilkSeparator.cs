using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class MilkSeparator : GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.noOperational;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.root.EventHandler(GameHashes.OnConduitObjectDispensed, new GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.GameEvent.Callback(MilkSeparator.RefreshLastObjectDispensed)).EventHandler(GameHashes.OnStorageChange, new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State.Callback(MilkSeparator.RefreshMeters));
		this.noOperational.TagTransition(GameTags.Operational, this.operational, false).PlayAnim("off");
		this.operational.TagTransition(GameTags.Operational, this.noOperational, true).PlayAnim("on").DefaultState(this.operational.idle);
		this.operational.idle.EventTransition(GameHashes.OnStorageChange, this.operational.working.pre, new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.Transition.ConditionCallback(MilkSeparator.CanBeginSeparate)).EnterTransition(this.operational.full, new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.Transition.ConditionCallback(MilkSeparator.RequiresEmptying));
		this.operational.working.pre.QueueAnim("separating_pre", false, null).OnAnimQueueComplete(this.operational.working.work);
		this.operational.working.work.Enter(new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State.Callback(MilkSeparator.BeginSeparation)).PlayAnim("separating_loop", KAnim.PlayMode.Loop).Update(new Action<MilkSeparator.Instance, float>(MilkSeparator.SpawnCaviar), UpdateRate.SIM_200ms, false)
			.ToggleStatusItem(delegate(MilkSeparator.Instance smi)
			{
				if (!smi.IsProducingCaviar())
				{
					return null;
				}
				return Db.Get().BuildingStatusItems.MilkSeparatorProducingCaviar;
			}, (MilkSeparator.Instance smi) => smi)
			.EventTransition(GameHashes.OnStorageChange, this.operational.working.post, new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.Transition.ConditionCallback(MilkSeparator.CanNOTKeepSeparating))
			.Exit(new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State.Callback(MilkSeparator.EndSeparation));
		this.operational.working.post.QueueAnim("separating_pst", false, null).OnAnimQueueComplete(this.operational.idle);
		this.operational.full.PlayAnim("ready").ToggleRecurringChore(new Func<MilkSeparator.Instance, Chore>(MilkSeparator.CreateEmptyChore), null).WorkableCompleteTransition((MilkSeparator.Instance smi) => smi.workable, this.operational.emptyComplete)
			.ToggleStatusItem(Db.Get().BuildingStatusItems.MilkSeparatorNeedsEmptying, null);
		this.operational.emptyComplete.Enter(new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State.Callback(MilkSeparator.DropMilkFat)).ScheduleActionNextFrame("AfterMilkFatDrop", delegate(MilkSeparator.Instance smi)
		{
			smi.GoTo(this.operational.idle);
		});
	}

	public static void SpawnCaviar(MilkSeparator.Instance smi, float dt)
	{
		smi.SpawnCaviar(dt);
	}

	public static void BeginSeparation(MilkSeparator.Instance smi)
	{
		smi.operational.SetActive(true, false);
	}

	public static void EndSeparation(MilkSeparator.Instance smi)
	{
		smi.operational.SetActive(false, false);
	}

	public static bool CanBeginSeparate(MilkSeparator.Instance smi)
	{
		return !smi.MilkFatLimitReached && smi.HasEnoughMassToStartConverting();
	}

	public static bool CanKeepSeparating(MilkSeparator.Instance smi)
	{
		return !smi.MilkFatLimitReached && smi.CanConvertAtAll();
	}

	public static bool CanNOTKeepSeparating(MilkSeparator.Instance smi)
	{
		return !MilkSeparator.CanKeepSeparating(smi);
	}

	public static bool RequiresEmptying(MilkSeparator.Instance smi)
	{
		return smi.MilkFatLimitReached;
	}

	public static bool ThereIsCapacityForMilkFat(MilkSeparator.Instance smi)
	{
		return !smi.MilkFatLimitReached;
	}

	public static void DropMilkFat(MilkSeparator.Instance smi)
	{
		smi.DropMilkFat();
	}

	public static void RefreshLastObjectDispensed(MilkSeparator.Instance smi, object o)
	{
		smi.RefreshLastObjectDispensed(o);
	}

	public static void RefreshMeters(MilkSeparator.Instance smi)
	{
		smi.RefreshMeters();
	}

	private static Chore CreateEmptyChore(MilkSeparator.Instance smi)
	{
		WorkChore<EmptyMilkSeparatorWorkable> workChore = new WorkChore<EmptyMilkSeparatorWorkable>(Db.Get().ChoreTypes.EmptyStorage, smi.workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		workChore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, null);
		return workChore;
	}

	public const string FAT_IN_METER_COLOR_SYMBOL_NAME = "meter_fat";

	public const string FAT_COLOR_SYMBOL_NAME = "fat";

	public const string INPUT_LIQUID_SYMBOL_NAME = "liquid_reservoir";

	public const string OUTPUT_LIQUID_SYMBOL_NAME = "meter_liquid_cycle";

	public const string WORK_PRE_ANIM_NAME = "separating_pre";

	public const string WORK_ANIM_NAME = "separating_loop";

	public const string WORK_POST_ANIM_NAME = "separating_pst";

	public GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State noOperational;

	public MilkSeparator.OperationalStates operational;

	public class Def : StateMachine.BaseDef, IConverterByproduct
	{
		public Tag ByproductAssociatedInputTag
		{
			get
			{
				return this.FISHMILK_TAG;
			}
		}

		public Tag ByproductTag
		{
			get
			{
				return this.CAVIAR_TAG;
			}
		}

		public float ByproductRate
		{
			get
			{
				return this.CAVIAR_PRODUCTION_RATE;
			}
		}

		public bool ByproductIsContinuous
		{
			get
			{
				return true;
			}
		}

		public Def()
		{
			this.MILK_FAT_TAG = ElementLoader.FindElementByHash(SimHashes.MilkFat).tag;
			this.MILK_TAG = ElementLoader.FindElementByHash(SimHashes.Milk).tag;
			this.FISHMILK_TAG = ElementLoader.FindElementByHash(SimHashes.FishMilk).tag;
			this.MILK_SEPARATED_LIQUID_OUTPUT_TAG = ElementLoader.FindElementByHash(SimHashes.Brine).tag;
			this.FISHMILK_SEPARATED_LIQUID_OUTPUT_TAG = ElementLoader.FindElementByHash(SimHashes.Mucus).tag;
			this.CAVIAR_TAG = new Tag("Caviar");
		}

		public void GetByproductDescriptors(GameObject obj, List<Descriptor> descriptors)
		{
			if (this.CAVIAR_PRODUCTION_RATE <= 0f)
			{
				return;
			}
			string text = this.CAVIAR_TAG.ProperName();
			string formattedMass = GameUtil.GetFormattedMass(this.CAVIAR_PRODUCTION_RATE, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}");
			descriptors.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED_INPUTTEMP, text, formattedMass), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_INPUTTEMP, text, formattedMass), Descriptor.DescriptorType.Effect, false));
		}

		public float MILK_FAT_CAPACITY = 100f;

		public float CAVIAR_PRODUCTION_RATE;

		public Tag MILK_TAG;

		public Tag MILK_FAT_TAG;

		public Tag CAVIAR_TAG;

		public Tag FISHMILK_TAG;

		public Tag MILK_SEPARATED_LIQUID_OUTPUT_TAG;

		public Tag FISHMILK_SEPARATED_LIQUID_OUTPUT_TAG;
	}

	public class WorkingStates : GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State
	{
		public GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State pre;

		public GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State work;

		public GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State post;
	}

	public class OperationalStates : GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State
	{
		public GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State idle;

		public MilkSeparator.WorkingStates working;

		public GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State full;

		public GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State emptyComplete;
	}

	public new class Instance : GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.GameInstance
	{
		public float SolidOutputStored
		{
			get
			{
				return this.MilkFatStored + this.CaviarStored;
			}
		}

		public float CaviarStored
		{
			get
			{
				return this.storage.GetMassAvailable(base.def.CAVIAR_TAG);
			}
		}

		public float MilkFatStored
		{
			get
			{
				return this.storage.GetMassAvailable(base.def.MILK_FAT_TAG);
			}
		}

		public float MilkStored
		{
			get
			{
				return this.storage.GetMassAvailable(base.def.MILK_TAG);
			}
		}

		public float FishMilkStored
		{
			get
			{
				return this.storage.GetMassAvailable(base.def.FISHMILK_TAG);
			}
		}

		public float SolidOutputStoragePercentage
		{
			get
			{
				return Mathf.Clamp(this.SolidOutputStored / base.def.MILK_FAT_CAPACITY, 0f, 1f);
			}
		}

		public bool MilkFatLimitReached
		{
			get
			{
				return this.SolidOutputStored >= base.def.MILK_FAT_CAPACITY;
			}
		}

		public Instance(IStateMachineTarget master, MilkSeparator.Def def)
			: base(master, def)
		{
			this.animController = base.GetComponent<KBatchedAnimController>();
			this.fatMeter = new MeterController(this.animController, "meter_target_1", "meter_fat", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target_1" });
			this.elementConverters = master.gameObject.GetComponents<ElementConverter>();
		}

		public override void StartSM()
		{
			base.StartSM();
			this.workable.OnWork_PST_Begins = new global::System.Action(this.Play_Empty_MeterAnimation);
			this.RefreshMeters();
		}

		private void Play_Empty_MeterAnimation()
		{
			this.fatMeter.SetPositionPercent(0f);
			this.fatMeter.meterController.Play("meter_fat_empty", KAnim.PlayMode.Once, 1f, 0f);
		}

		public bool HasEnoughMassToStartConverting()
		{
			for (int i = 0; i < this.elementConverters.Length; i++)
			{
				if (this.elementConverters[i].HasEnoughMassToStartConverting(false))
				{
					return true;
				}
			}
			return false;
		}

		public bool CanConvertAtAll()
		{
			for (int i = 0; i < this.elementConverters.Length; i++)
			{
				if (this.elementConverters[i].CanConvertAtAll())
				{
					return true;
				}
			}
			return false;
		}

		public bool IsProducingCaviar()
		{
			return base.def.CAVIAR_PRODUCTION_RATE > 0f && this.storage.GetAmountAvailable(base.def.FISHMILK_TAG) > 0f;
		}

		public void SpawnCaviar(float dt)
		{
			if (base.def.CAVIAR_PRODUCTION_RATE <= 0f)
			{
				return;
			}
			if (this.storage.GetAmountAvailable(base.def.FISHMILK_TAG) <= 0f)
			{
				return;
			}
			this.caviarMassAccumulated += base.def.CAVIAR_PRODUCTION_RATE * dt;
			if (this.caviarMassAccumulated >= 1f)
			{
				GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(base.def.CAVIAR_TAG), Grid.SceneLayer.Ore, null, 0);
				gameObject.GetComponent<PrimaryElement>().Units = (float)Mathf.FloorToInt(this.caviarMassAccumulated / 1f);
				gameObject.SetActive(true);
				this.storage.Store(gameObject, true, false, true, false);
				this.caviarMassAccumulated %= 1f;
			}
		}

		public void DropMilkFat()
		{
			List<GameObject> list = new List<GameObject>();
			this.storage.Drop(base.def.MILK_FAT_TAG, list);
			this.storage.Drop(base.def.CAVIAR_TAG, list);
			Vector3 dropSpawnLocation = this.GetDropSpawnLocation();
			foreach (GameObject gameObject in list)
			{
				gameObject.transform.position = dropSpawnLocation;
			}
		}

		private Vector3 GetDropSpawnLocation()
		{
			bool flag;
			Vector3 vector = this.animController.GetSymbolTransform(new HashedString("object"), out flag).GetColumn(3);
			vector.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			int num = Grid.PosToCell(vector);
			if (Grid.IsValidCell(num) && !Grid.Solid[num])
			{
				return vector;
			}
			return base.transform.GetPosition();
		}

		public void RefreshLastObjectDispensed(object o)
		{
			if (o == null)
			{
				return;
			}
			PrimaryElement primaryElement = o as PrimaryElement;
			if (primaryElement == null)
			{
				return;
			}
			this.lastObjectDispensedTag = primaryElement.Element.tag;
		}

		public Color GetFatColor()
		{
			bool flag = this.MilkFatStored >= this.CaviarStored;
			bool flag2 = this.MilkStored >= this.FishMilkStored;
			if (!((this.MilkFatStored <= 0f && this.CaviarStored <= 0f) ? flag2 : flag))
			{
				return CaviarTuning.COLOR;
			}
			return ElementLoader.FindElementByTag(base.def.MILK_FAT_TAG).substance.colour;
		}

		public void RefreshMeters()
		{
			if (this.fatMeter.meterController.currentAnim != "meter_fat")
			{
				this.fatMeter.meterController.Play("meter_fat", KAnim.PlayMode.Paused, 1f, 0f);
			}
			this.fatMeter.SetPositionPercent(this.SolidOutputStoragePercentage);
			float milkFatStored = this.MilkFatStored;
			float caviarStored = this.CaviarStored;
			Tag tag = ((this.MilkStored >= this.FishMilkStored) ? base.def.MILK_TAG : base.def.FISHMILK_TAG);
			Tag tag2 = ((this.lastObjectDispensedTag != Tag.Invalid) ? this.lastObjectDispensedTag : ((tag == base.def.FISHMILK_TAG) ? base.def.FISHMILK_SEPARATED_LIQUID_OUTPUT_TAG : base.def.MILK_SEPARATED_LIQUID_OUTPUT_TAG));
			Color fatColor = this.GetFatColor();
			this.fatMeter.meterController.SetSymbolTint(new KAnimHashedString("meter_fat"), fatColor);
			this.animController.SetSymbolTint("fat", fatColor);
			GameUtil.TintLiquidSymbolOnBuilding("liquid_reservoir", this.animController, ElementLoader.FindElementByTag(tag));
			GameUtil.TintLiquidSymbolOnBuilding("meter_liquid_cycle", this.animController, ElementLoader.FindElementByTag(tag2));
		}

		[MyCmpGet]
		public EmptyMilkSeparatorWorkable workable;

		[MyCmpGet]
		public Operational operational;

		[MyCmpGet]
		private Storage storage;

		private ElementConverter[] elementConverters;

		private SymbolOverrideController symbolOverrideController;

		private float caviarMassAccumulated;

		private KBatchedAnimController animController;

		private MeterController fatMeter;

		private Tag lastObjectDispensedTag = Tag.Invalid;
	}
}
