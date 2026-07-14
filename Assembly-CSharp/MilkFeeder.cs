using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class MilkFeeder : GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.root.Enter(delegate(MilkFeeder.Instance smi)
		{
			smi.UpdateStorageMeter();
		}).EventHandler(GameHashes.OnStorageChange, delegate(MilkFeeder.Instance smi)
		{
			smi.UpdateStorageMeter();
		}).EventHandler(GameHashes.OnStorageChange, new StateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State.Callback(MilkFeeder.RefreshLiquidColor));
		this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, new StateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.Transition.ConditionCallback(MilkFeeder.ShouldBeOn)).EventTransition(GameHashes.BuildingStrawChange, this.on, new StateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.Transition.ConditionCallback(MilkFeeder.ShouldBeOn))
			.Enter(new StateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State.Callback(MilkFeeder.RefreshLiquidColor))
			.DefaultState(this.off.noOperational);
		this.off.noOperational.EventTransition(GameHashes.OperationalChanged, this.off.strawBlocked, (MilkFeeder.Instance smi) => MilkFeeder.IsOperational(smi) && MilkFeeder.IsStrawBlocked(smi)).EventTransition(GameHashes.OperationalChanged, this.off.noLiquidOnStraw, (MilkFeeder.Instance smi) => MilkFeeder.IsOperational(smi) && MilkFeeder.IsStrawOutsideLiquid(smi));
		this.off.strawBlocked.ToggleStatusItem(Db.Get().BuildingStatusItems.OutputTileBlocked, null).EventTransition(GameHashes.OperationalChanged, this.off.noOperational, GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.Not(new StateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.Transition.ConditionCallback(MilkFeeder.IsOperational))).EventTransition(GameHashes.BuildingStrawChange, this.off.noLiquidOnStraw, (MilkFeeder.Instance smi) => !MilkFeeder.IsStrawBlocked(smi) && MilkFeeder.IsStrawOutsideLiquid(smi));
		this.off.noLiquidOnStraw.ToggleStatusItem(Db.Get().BuildingStatusItems.NotSubmerged, null).EventTransition(GameHashes.OperationalChanged, this.off.noOperational, GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.Not(new StateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.Transition.ConditionCallback(MilkFeeder.IsOperational))).EventTransition(GameHashes.BuildingStrawChange, this.off.strawBlocked, new StateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.Transition.ConditionCallback(MilkFeeder.IsStrawBlocked));
		this.on.DefaultState(this.on.pre).EventTransition(GameHashes.BuildingStrawChange, this.on.pst, (MilkFeeder.Instance smi) => !MilkFeeder.ShouldBeOn(smi) && smi.GetCurrentState() != this.on.pre).EventTransition(GameHashes.BuildingStrawChange, this.off, (MilkFeeder.Instance smi) => !MilkFeeder.ShouldBeOn(smi) && smi.GetCurrentState() == this.on.pre)
			.EventTransition(GameHashes.OperationalChanged, this.on.pst, (MilkFeeder.Instance smi) => !MilkFeeder.ShouldBeOn(smi) && smi.GetCurrentState() != this.on.pre)
			.EventTransition(GameHashes.OperationalChanged, this.off, (MilkFeeder.Instance smi) => !MilkFeeder.ShouldBeOn(smi) && smi.GetCurrentState() == this.on.pre)
			.Enter(new StateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State.Callback(MilkFeeder.RefreshLiquidColor));
		this.on.pre.PlayAnim("working_pre").OnAnimQueueComplete(this.on.working);
		this.on.working.PlayAnim("on").DefaultState(this.on.working.empty);
		this.on.working.empty.PlayAnim("empty").EnterTransition(this.on.working.refilling, (MilkFeeder.Instance smi) => smi.HasEnoughMilkForOneFeeding()).EventHandler(GameHashes.OnStorageChange, delegate(MilkFeeder.Instance smi)
		{
			if (smi.HasEnoughMilkForOneFeeding())
			{
				smi.GoTo(this.on.working.refilling);
			}
		});
		this.on.working.refilling.Enter(new StateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State.Callback(MilkFeeder.RefreshLiquidColor)).PlayAnim("fill").OnAnimQueueComplete(this.on.working.full);
		this.on.working.full.PlayAnim("full").Enter(delegate(MilkFeeder.Instance smi)
		{
			this.isReadyToStartFeeding.Set(true, smi, false);
		}).Exit(delegate(MilkFeeder.Instance smi)
		{
			this.isReadyToStartFeeding.Set(false, smi, false);
		})
			.ParamTransition<DrinkMilkStates.Instance>(this.currentFeedingCritter, this.on.working.emptying, (MilkFeeder.Instance smi, DrinkMilkStates.Instance val) => val != null);
		this.on.working.emptying.EnterTransition(this.on.working.full, delegate(MilkFeeder.Instance smi)
		{
			DrinkMilkMonitor.Instance smi2 = this.currentFeedingCritter.Get(smi).GetSMI<DrinkMilkMonitor.Instance>();
			return smi2 != null && !smi2.def.consumesMilk;
		}).PlayAnim("emptying").OnAnimQueueComplete(this.on.working.empty)
			.Exit(delegate(MilkFeeder.Instance smi)
			{
				smi.StopFeeding();
			});
		this.on.pst.PlayAnim("working_pst").OnAnimQueueComplete(this.off);
	}

	public static bool ShouldBeOn(MilkFeeder.Instance smi)
	{
		return MilkFeeder.IsOperational(smi) && !MilkFeeder.IsStrawBlocked(smi) && !MilkFeeder.IsStrawOutsideLiquid(smi);
	}

	public static bool IsOperational(MilkFeeder.Instance smi)
	{
		return smi.IsOperational;
	}

	public static bool IsStrawBlocked(MilkFeeder.Instance smi)
	{
		return smi.IsStrawBlocked;
	}

	public static bool IsStrawOutsideLiquid(MilkFeeder.Instance smi)
	{
		return smi.IsStrawOutsideLiquid;
	}

	public static void RefreshLiquidColor(MilkFeeder.Instance smi)
	{
		smi.RefreshLiquidColor();
	}

	private const string TINT_METER_SYMBOL_NAME = "meter_fill";

	private const string TINT_SYMBOL_NAME = "Milk_fg";

	private const string TINT_SYMBOL2_NAME = "Milk_fill_fg";

	private MilkFeeder.OffState off;

	private MilkFeeder.OnState on;

	public StateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.BoolParameter isReadyToStartFeeding;

	public StateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.ObjectParameter<DrinkMilkStates.Instance> currentFeedingCritter;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> list = new List<Descriptor>();
			go.GetSMI<MilkFeeder.Instance>();
			for (int i = 0; i < MilkFeederConfig.EffectsPerDrinkableLiquid.Length; i++)
			{
				Tag first = MilkFeederConfig.EffectsPerDrinkableLiquid[i].first;
				string second = MilkFeederConfig.EffectsPerDrinkableLiquid[i].second;
				Descriptor descriptor = default(Descriptor);
				descriptor.SetupDescriptor(Strings.Get("STRINGS.CREATURES.MODIFIERS." + second.ToUpper() + ".NAME"), "", Descriptor.DescriptorType.Effect);
				list.Add(descriptor);
				Effect.AddModifierDescriptions(list, second, true, "STRINGS.CREATURES.STATS.");
			}
			return list;
		}

		public CellOffset drinkCellOffset;

		public Tag elementProducedTag;

		public float unitsProducedPerFeeding;

		public bool tintMeter;
	}

	public class OffState : GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State
	{
		public GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State noOperational;

		public GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State strawBlocked;

		public GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State noLiquidOnStraw;
	}

	public class OnState : GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State
	{
		public GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State pre;

		public MilkFeeder.OnState.WorkingState working;

		public GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State pst;

		public class WorkingState : GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State
		{
			public GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State empty;

			public GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State refilling;

			public GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State full;

			public GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.State emptying;
		}
	}

	public new class Instance : GameStateMachine<MilkFeeder, MilkFeeder.Instance, IStateMachineTarget, MilkFeeder.Def>.GameInstance
	{
		public bool IsOperational
		{
			get
			{
				return this.operational != null && this.operational.IsOperational;
			}
		}

		public bool IsStrawInstalled
		{
			get
			{
				return this.straw != null;
			}
		}

		public bool IsStrawOutsideLiquid
		{
			get
			{
				return this.IsStrawInstalled && !this.straw.isInLiquid;
			}
		}

		public bool IsStrawBlocked
		{
			get
			{
				return this.IsStrawInstalled && this.straw.currentDepth <= 0;
			}
		}

		public Instance(IStateMachineTarget master, MilkFeeder.Def def)
			: base(master, def)
		{
			this.milkStorage = base.GetComponent<Storage>();
			this.operational = base.GetComponent<Operational>();
			this.straw = base.GetComponent<BuildingPointStraw>();
			this.storageMeter = new MeterController(base.smi.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
			base.Subscribe(360192579, new Action<object>(this.OnStrawChanged));
		}

		private void OnStrawChanged(object o)
		{
			CellOffset bottomCellOffset = ((BuildingPointStraw)o).GetBottomCellOffset();
			this.strawCellOffset = bottomCellOffset;
		}

		public override void StartSM()
		{
			base.StartSM();
			Components.MilkFeeders.Add(base.smi.GetMyWorldId(), this);
			this.RefreshLiquidColor();
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			Components.MilkFeeders.Remove(base.smi.GetMyWorldId(), this);
		}

		public CellOffset GetDrinkCellOffset()
		{
			return base.def.drinkCellOffset + this.strawCellOffset;
		}

		public void UpdateStorageMeter()
		{
			this.storageMeter.SetPositionPercent(1f - Mathf.Clamp01(this.milkStorage.RemainingCapacity() / this.milkStorage.capacityKg));
		}

		public bool IsReserved()
		{
			return base.HasTag(GameTags.Creatures.ReservedByCreature);
		}

		public void SetReserved(bool isReserved)
		{
			if (isReserved)
			{
				global::Debug.Assert(!base.HasTag(GameTags.Creatures.ReservedByCreature));
				base.GetComponent<KPrefabID>().SetTag(GameTags.Creatures.ReservedByCreature, true);
				return;
			}
			if (base.HasTag(GameTags.Creatures.ReservedByCreature))
			{
				base.GetComponent<KPrefabID>().RemoveTag(GameTags.Creatures.ReservedByCreature);
				return;
			}
			global::Debug.LogWarningFormat(base.smi.gameObject, "Tried to unreserve a MilkFeeder that wasn't reserved", Array.Empty<object>());
		}

		public void RefreshLiquidColor()
		{
			PrimaryElement primaryElement = this.milkStorage.FindFirstWithMass(base.def.elementProducedTag, base.def.unitsProducedPerFeeding);
			if (primaryElement == null)
			{
				return;
			}
			Element element = ElementLoader.FindElementByTag(primaryElement.PrefabID());
			if (base.def.tintMeter && this.milkStorage != null)
			{
				KBatchedAnimController meterController = this.storageMeter.meterController;
				if (meterController != null)
				{
					GameUtil.TintLiquidSymbolOnBuilding("meter_fill", meterController, element);
				}
			}
			KBatchedAnimController[] componentsInChildren = base.gameObject.GetComponentsInChildren<KBatchedAnimController>();
			if (componentsInChildren == null || componentsInChildren.Length == 0)
			{
				return;
			}
			foreach (KBatchedAnimController kbatchedAnimController in componentsInChildren)
			{
				GameUtil.TintLiquidSymbolOnBuilding("Milk_fg", kbatchedAnimController, element);
				GameUtil.TintLiquidSymbolOnBuilding("Milk_fill_fg", kbatchedAnimController, element);
			}
		}

		public bool IsReadyToStartFeeding()
		{
			return base.sm.isReadyToStartFeeding.Get(base.smi);
		}

		public void RequestToStartFeeding(DrinkMilkStates.Instance feedingCritter)
		{
			base.sm.currentFeedingCritter.Set(feedingCritter, base.smi, false);
		}

		public void StopFeeding()
		{
			DrinkMilkStates.Instance instance = base.sm.currentFeedingCritter.Get(base.smi);
			if (instance != null)
			{
				instance.RequestToStopFeeding();
			}
			base.sm.currentFeedingCritter.Set(null, base.smi, false);
		}

		public bool HasEnoughMilkForOneFeeding()
		{
			return this.milkStorage.FindFirstWithMass(base.def.elementProducedTag, base.def.unitsProducedPerFeeding) != null;
		}

		public Tag ConsumeMilkForOneFeeding()
		{
			Tag tag = this.milkStorage.FindFirstWithMass(base.def.elementProducedTag, base.def.unitsProducedPerFeeding).PrefabID();
			this.milkStorage.ConsumeIgnoringDisease(tag, base.def.unitsProducedPerFeeding);
			return tag;
		}

		public bool IsInCreaturePenRoom()
		{
			Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
			return roomOfGameObject != null && roomOfGameObject.roomType == Db.Get().RoomTypes.CreaturePen;
		}

		public Storage milkStorage;

		public MeterController storageMeter;

		private CellOffset strawCellOffset = new CellOffset(0, 0);

		private Operational operational;

		private BuildingPointStraw straw;
	}
}
