using System;
using System.Collections.Generic;
using UnityEngine;

public class MilkSeparator : GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.noOperational;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.root.EventHandler(GameHashes.OnStorageChange, new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State.Callback(MilkSeparator.RefreshMeters));
		this.noOperational.TagTransition(GameTags.Operational, this.operational, false).PlayAnim("off");
		this.operational.TagTransition(GameTags.Operational, this.noOperational, true).PlayAnim("on").DefaultState(this.operational.idle);
		this.operational.idle.EventTransition(GameHashes.OnStorageChange, this.operational.working.pre, new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.Transition.ConditionCallback(MilkSeparator.CanBeginSeparate)).EnterTransition(this.operational.full, new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.Transition.ConditionCallback(MilkSeparator.RequiresEmptying));
		this.operational.working.pre.QueueAnim("separating_pre", false, null).OnAnimQueueComplete(this.operational.working.work);
		this.operational.working.work.Enter(new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State.Callback(MilkSeparator.BeginSeparation)).PlayAnim("separating_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OnStorageChange, this.operational.working.post, new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.Transition.ConditionCallback(MilkSeparator.CanNOTKeepSeparating))
			.Exit(new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State.Callback(MilkSeparator.EndSeparation));
		this.operational.working.post.QueueAnim("separating_pst", false, null).OnAnimQueueComplete(this.operational.idle);
		this.operational.full.PlayAnim("ready").ToggleRecurringChore(new Func<MilkSeparator.Instance, Chore>(MilkSeparator.CreateEmptyChore), null).WorkableCompleteTransition((MilkSeparator.Instance smi) => smi.workable, this.operational.emptyComplete)
			.ToggleStatusItem(Db.Get().BuildingStatusItems.MilkSeparatorNeedsEmptying, null);
		this.operational.emptyComplete.Enter(new StateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State.Callback(MilkSeparator.DropMilkFat)).ScheduleActionNextFrame("AfterMilkFatDrop", delegate(MilkSeparator.Instance smi)
		{
			smi.GoTo(this.operational.idle);
		});
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
		return !smi.MilkFatLimitReached && smi.elementConverter.HasEnoughMassToStartConverting(false);
	}

	public static bool CanKeepSeparating(MilkSeparator.Instance smi)
	{
		return !smi.MilkFatLimitReached && smi.elementConverter.CanConvertAtAll();
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

	public const string WORK_PRE_ANIM_NAME = "separating_pre";

	public const string WORK_ANIM_NAME = "separating_loop";

	public const string WORK_POST_ANIM_NAME = "separating_pst";

	public GameStateMachine<MilkSeparator, MilkSeparator.Instance, IStateMachineTarget, MilkSeparator.Def>.State noOperational;

	public MilkSeparator.OperationalStates operational;

	public class Def : StateMachine.BaseDef
	{
		public Def()
		{
			this.MILK_FAT_TAG = ElementLoader.FindElementByHash(SimHashes.MilkFat).tag;
			this.MILK_TAG = ElementLoader.FindElementByHash(SimHashes.Milk).tag;
		}

		public float MILK_FAT_CAPACITY = 100f;

		public Tag MILK_TAG;

		public Tag MILK_FAT_TAG;
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
		public float MilkFatStored
		{
			get
			{
				return this.storage.GetAmountAvailable(base.def.MILK_FAT_TAG);
			}
		}

		public float MilkFatStoragePercentage
		{
			get
			{
				return Mathf.Clamp(this.MilkFatStored / base.def.MILK_FAT_CAPACITY, 0f, 1f);
			}
		}

		public bool MilkFatLimitReached
		{
			get
			{
				return this.MilkFatStored >= base.def.MILK_FAT_CAPACITY;
			}
		}

		public Instance(IStateMachineTarget master, MilkSeparator.Def def)
			: base(master, def)
		{
			KAnimControllerBase component = base.GetComponent<KBatchedAnimController>();
			this.fatMeter = new MeterController(component, "meter_target_1", "meter_fat", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target_1" });
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

		public void DropMilkFat()
		{
			List<GameObject> list = new List<GameObject>();
			this.storage.Drop(base.def.MILK_FAT_TAG, list);
			Vector3 dropSpawnLocation = this.GetDropSpawnLocation();
			foreach (GameObject gameObject in list)
			{
				gameObject.transform.position = dropSpawnLocation;
			}
		}

		private Vector3 GetDropSpawnLocation()
		{
			bool flag;
			Vector3 vector = base.GetComponent<KBatchedAnimController>().GetSymbolTransform(new HashedString("milkfat"), out flag).GetColumn(3);
			vector.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			int num = Grid.PosToCell(vector);
			if (Grid.IsValidCell(num) && !Grid.Solid[num])
			{
				return vector;
			}
			return base.transform.GetPosition();
		}

		public void RefreshMeters()
		{
			if (this.fatMeter.meterController.currentAnim != "meter_fat")
			{
				this.fatMeter.meterController.Play("meter_fat", KAnim.PlayMode.Paused, 1f, 0f);
			}
			this.fatMeter.SetPositionPercent(this.MilkFatStoragePercentage);
		}

		[MyCmpGet]
		public EmptyMilkSeparatorWorkable workable;

		[MyCmpGet]
		public Operational operational;

		[MyCmpGet]
		public ElementConverter elementConverter;

		[MyCmpGet]
		private Storage storage;

		private MeterController fatMeter;
	}
}
