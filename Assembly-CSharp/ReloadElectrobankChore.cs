using System;
using STRINGS;
using UnityEngine;

public class ReloadElectrobankChore : Chore<ReloadElectrobankChore.Instance>
{
	public ReloadElectrobankChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.ReloadElectrobank, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new ReloadElectrobankChore.Instance(this, target.gameObject);
		this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		this.AddPrecondition(ReloadElectrobankChore.ElectrobankIsNotNull, null);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			global::Debug.LogError("ReloadElectrobankChore null context.consumer");
			return;
		}
		BionicBatteryMonitor.Instance smi = context.consumerState.consumer.GetSMI<BionicBatteryMonitor.Instance>();
		if (smi == null)
		{
			global::Debug.LogError("ReloadElectrobankChore null BionicBatteryMonitor.Instance");
			return;
		}
		Electrobank closestElectrobank = smi.GetClosestElectrobank();
		if (closestElectrobank == null)
		{
			global::Debug.LogError("ReloadElectrobankChore null electrobank.gameObject");
			return;
		}
		base.smi.sm.electrobankSource.Set(closestElectrobank.gameObject, base.smi, false);
		base.smi.sm.amountRequested.Set(closestElectrobank.GetComponent<PrimaryElement>().Mass, base.smi, false);
		base.smi.sm.dupe.Set(context.consumerState.consumer, base.smi);
		base.Begin(context);
	}

	public bool IsInstallingAtMessStation()
	{
		return base.smi.IsInsideState(base.smi.sm.installAtMessStation.install);
	}

	public static bool HasAnyDepletedBattery(ReloadElectrobankChore.Instance smi)
	{
		return ReloadElectrobankChore.GetAnyEmptyBattery(smi) != null;
	}

	public static GameObject GetAnyEmptyBattery(ReloadElectrobankChore.Instance smi)
	{
		return smi.batteryMonitor.storage.FindFirst(GameTags.EmptyPortableBattery);
	}

	public static void RemoveDepletedElectrobank(ReloadElectrobankChore.Instance smi)
	{
		GameObject anyEmptyBattery = ReloadElectrobankChore.GetAnyEmptyBattery(smi);
		if (anyEmptyBattery != null)
		{
			smi.batteryMonitor.storage.Drop(anyEmptyBattery, true);
		}
	}

	public static void InstallElectrobank(ReloadElectrobankChore.Instance smi)
	{
		Storage[] components = smi.gameObject.GetComponents<Storage>();
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i] != smi.batteryMonitor.storage && components[i].FindFirst(GameTags.ChargedPortableBattery) != null)
			{
				components[i].Transfer(smi.batteryMonitor.storage, false, false);
				break;
			}
		}
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_BionicBattery, true);
	}

	private static void SetStoredItemVisibility(GameObject item, bool visible)
	{
		if (item == null)
		{
			return;
		}
		KBatchedAnimTracker component = item.GetComponent<KBatchedAnimTracker>();
		if (component != null)
		{
			component.enabled = visible;
		}
		Storage.MakeItemInvisible(item, !visible, false);
	}

	public const float LOOP_LENGTH = 4.333f;

	public static readonly Chore.Precondition ElectrobankIsNotNull = new Chore.Precondition
	{
		id = "ElectrobankIsNotNull",
		description = DUPLICANTS.CHORES.PRECONDITIONS.EDIBLE_IS_NOT_NULL,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return null != context.consumerState.consumer.GetSMI<BionicBatteryMonitor.Instance>().GetClosestElectrobank();
		}
	};

	public class States : GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore>
	{
		private bool IsMessStationInvalid(GameObject messStation)
		{
			return messStation == null || !messStation.GetComponent<Operational>().IsOperational;
		}

		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			this.defaultElectrobankSymbol = Assets.GetPrefab("Electrobank").GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbolByIndex(0U);
			this.depletedElectrobankSymbol = Assets.GetPrefab("EmptyElectrobank").GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbolByIndex(0U);
			default_state = this.fetch;
			base.Target(this.dupe);
			this.root.Enter("SetMessStation", delegate(ReloadElectrobankChore.Instance smi)
			{
				smi.UpdateMessStation();
			}).EventHandler(GameHashes.AssignablesChanged, delegate(ReloadElectrobankChore.Instance smi)
			{
				smi.UpdateMessStation();
			});
			this.fetch.InitializeStates(this.dupe, this.electrobankSource, this.pickedUpElectrobank, this.amountRequested, this.actualunits, this.installAtMessStation, null).OnTargetLost(this.electrobankSource, this.electrobankLost);
			this.installAtMessStation.Enter(delegate(ReloadElectrobankChore.Instance smi)
			{
				EatChore.StatesInstance.SetZ(this.pickedUpElectrobank.Get(smi), Grid.GetLayerZ(Grid.SceneLayer.Ore));
			}).EnterTransition(this.installAtSafeLocation, (ReloadElectrobankChore.Instance smi) => this.IsMessStationInvalid(this.messstation.Get(smi))).DefaultState(this.installAtMessStation.approach)
				.ParamTransition<GameObject>(this.messstation, this.installAtSafeLocation, (ReloadElectrobankChore.Instance _, GameObject messStation) => this.IsMessStationInvalid(messStation));
			this.installAtMessStation.approach.InitializeStates(this.dupe, this.messstation, this.installAtMessStation.removeDepletedBatteries, this.installAtSafeLocation, null, null);
			this.installAtMessStation.removeDepletedBatteries.InitializeStates(this.installAtMessStation.install);
			this.installAtMessStation.install.InitializeStates(this.complete, new ReloadElectrobankChore.States.MessStationInstallBatteryAnim()).Enter(delegate(ReloadElectrobankChore.Instance smi)
			{
				GameObject gameObject = this.dupe.Get(smi);
				EatChore.StatesInstance.SetZ(gameObject, Grid.GetLayerZ(Grid.SceneLayer.BuildingFront));
				EatChore.StatesInstance.SetZ(this.pickedUpElectrobank.Get(smi), Grid.GetLayerZ(Grid.SceneLayer.Ore));
				EatChore.StatesInstance.ApplyRoomAndSaltEffects(this.messstation.Get(smi), gameObject, new float?(1800f));
			}).Exit(delegate(ReloadElectrobankChore.Instance smi)
			{
				EatChore.StatesInstance.SetZ(this.dupe.Get(smi), Grid.GetLayerZ(Grid.SceneLayer.Move));
			});
			this.installAtSafeLocation.Enter("CreateSafeLocation", delegate(ReloadElectrobankChore.Instance smi)
			{
				ValueTuple<GameObject, int> valueTuple = EatChore.StatesInstance.CreateLocator(this.dupe.Get<Sensors>(smi), this.dupe.Get<Transform>(smi), "ReloadElectrobankLocator");
				GameObject item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				this.safeLocation.Set(item, smi, false);
				this.safeCellIndex.Set(item2, smi, false);
			}).Exit("DestroySafeLocation", delegate(ReloadElectrobankChore.Instance smi)
			{
				Grid.Reserved[this.safeCellIndex.Get(smi)] = false;
				ChoreHelpers.DestroyLocator(this.safeLocation.Get(smi));
				this.safeLocation.Set(null, smi);
			}).DefaultState(this.installAtSafeLocation.approach);
			this.installAtSafeLocation.approach.InitializeStates(this.dupe, this.safeLocation, this.installAtSafeLocation.removeDepletedBatteries, this.installAtSafeLocation.removeDepletedBatteries, null, null);
			this.installAtSafeLocation.removeDepletedBatteries.InitializeStates(this.installAtSafeLocation.install);
			this.installAtSafeLocation.install.InitializeStates(this.complete, new ReloadElectrobankChore.States.DefaultInstallBatteryAnim());
			this.complete.Enter(new StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State.Callback(ReloadElectrobankChore.InstallElectrobank)).ReturnSuccess();
			this.electrobankLost.Target(this.dupe).TriggerOnEnter(GameHashes.TargetElectrobankLost, null).ReturnFailure();
		}

		public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.FetchSubState fetch;

		public ReloadElectrobankChore.States.InstallAtMessStation installAtMessStation;

		public ReloadElectrobankChore.States.InstallAtSafeLocation installAtSafeLocation;

		public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State complete;

		public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State electrobankLost;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.TargetParameter dupe;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.TargetParameter electrobankSource;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.TargetParameter lastDepletedElectrobankFound;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.TargetParameter pickedUpElectrobank;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.TargetParameter messstation;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.TargetParameter safeLocation;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.FloatParameter actualunits;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.FloatParameter amountRequested;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.IntParameter safeCellIndex;

		public KAnim.Build.Symbol defaultElectrobankSymbol;

		public KAnim.Build.Symbol depletedElectrobankSymbol;

		private const float ROOM_EFFECT_DURATION = 1800f;

		public class RemoveDepletedBatteries : GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State
		{
			public ReloadElectrobankChore.States.RemoveDepletedBatteries InitializeStates(GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State nextState)
			{
				base.DefaultState(this.animate).EnterTransition(nextState, (ReloadElectrobankChore.Instance smi) => !ReloadElectrobankChore.HasAnyDepletedBattery(smi));
				this.animate.ToggleAnims("anim_bionic_kanim", 0f).PlayAnim("discharge", KAnim.PlayMode.Once).Enter("Add Symbol Override", delegate(ReloadElectrobankChore.Instance smi)
				{
					smi.ShowElectrobankSymbol(true, smi.sm.depletedElectrobankSymbol);
				})
					.Exit("Revert Symbol Override", delegate(ReloadElectrobankChore.Instance smi)
					{
						smi.ShowElectrobankSymbol(false, smi.sm.depletedElectrobankSymbol);
					})
					.OnAnimQueueComplete(this.end);
				this.end.Enter(new StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State.Callback(ReloadElectrobankChore.RemoveDepletedElectrobank)).EnterTransition(this.animate, new StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.Transition.ConditionCallback(ReloadElectrobankChore.HasAnyDepletedBattery)).GoTo(nextState);
				return this;
			}

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State animate;

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State end;
		}

		public interface IInstallBatteryAnim
		{
			string GetBank();

			string GetPrefix(ReloadElectrobankChore.Instance smi, ReloadElectrobankChore.States.IInstallBatteryAnim.Anim anim);

			bool ForceFacing();

			public enum Anim
			{
				Pre,
				Loop,
				Pst
			}
		}

		public class DefaultInstallBatteryAnim : ReloadElectrobankChore.States.IInstallBatteryAnim
		{
			public string GetBank()
			{
				return "anim_bionic_kanim";
			}

			public string GetPrefix(ReloadElectrobankChore.Instance _smi, ReloadElectrobankChore.States.IInstallBatteryAnim.Anim _anim)
			{
				return "consume";
			}

			public bool ForceFacing()
			{
				return false;
			}
		}

		public class MessStationInstallBatteryAnim : ReloadElectrobankChore.States.IInstallBatteryAnim
		{
			public string GetBank()
			{
				return "anim_bionic_eat_table_kanim";
			}

			public string GetPrefix(ReloadElectrobankChore.Instance smi, ReloadElectrobankChore.States.IInstallBatteryAnim.Anim anim)
			{
				MinionResume component = smi.GetComponent<MinionResume>();
				bool flag = component != null && component.CurrentHat != null;
				bool flag2 = false;
				GameObject gameObject = smi.sm.messstation.Get(smi);
				if (gameObject != null)
				{
					MessStation component2 = gameObject.GetComponent<MessStation>();
					if (component2 != null && component2.HasSalt)
					{
						flag2 = true;
					}
				}
				if (flag2 && flag)
				{
					return "salt_hat";
				}
				if (flag2)
				{
					return "salt";
				}
				if (!flag)
				{
					return "working";
				}
				if (anim == ReloadElectrobankChore.States.IInstallBatteryAnim.Anim.Loop)
				{
					return "working";
				}
				return "hat";
			}

			public bool ForceFacing()
			{
				return true;
			}
		}

		public class InstallBattery : GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State
		{
			public ReloadElectrobankChore.States.InstallBattery InitializeStates(GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State nextState, ReloadElectrobankChore.States.IInstallBatteryAnim anim)
			{
				base.DefaultState(this.pre).ToggleAnims(anim.GetBank(), 0f).Enter("Add Symbol Override", delegate(ReloadElectrobankChore.Instance smi)
				{
					smi.StowElectrobank(false);
					if (anim.ForceFacing())
					{
						Facing component = smi.GetComponent<Facing>();
						if (component != null)
						{
							component.SetFacing(false);
						}
					}
				})
					.Exit("Revert Symbol Override", delegate(ReloadElectrobankChore.Instance smi)
					{
						smi.StowElectrobank(true);
					});
				this.pre.PlayAnim((ReloadElectrobankChore.Instance smi) => anim.GetPrefix(smi, ReloadElectrobankChore.States.IInstallBatteryAnim.Anim.Pre) + "_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.loop).ScheduleGoTo(5f, this.loop);
				this.loop.PlayAnim((ReloadElectrobankChore.Instance smi) => anim.GetPrefix(smi, ReloadElectrobankChore.States.IInstallBatteryAnim.Anim.Loop) + "_loop", KAnim.PlayMode.Loop).ScheduleGoTo(4.333f, this.pst);
				this.pst.PlayAnim((ReloadElectrobankChore.Instance smi) => anim.GetPrefix(smi, ReloadElectrobankChore.States.IInstallBatteryAnim.Anim.Pst) + "_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(nextState).ScheduleGoTo(5f, nextState);
				return this;
			}

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State pre;

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State loop;

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State pst;
		}

		public class InstallAtMessStation : GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State
		{
			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.ApproachSubState<MessStation> approach;

			public ReloadElectrobankChore.States.RemoveDepletedBatteries removeDepletedBatteries;

			public ReloadElectrobankChore.States.InstallBattery install;
		}

		public class InstallAtSafeLocation : GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State
		{
			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.ApproachSubState<IApproachable> approach;

			public ReloadElectrobankChore.States.RemoveDepletedBatteries removeDepletedBatteries;

			public ReloadElectrobankChore.States.InstallBattery install;
		}
	}

	public class Instance : GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.GameInstance
	{
		public BionicBatteryMonitor.Instance batteryMonitor
		{
			get
			{
				return base.sm.dupe.Get(this).GetSMI<BionicBatteryMonitor.Instance>();
			}
		}

		public Instance(ReloadElectrobankChore master, GameObject duplicant)
			: base(master)
		{
		}

		public void UpdateMessStation()
		{
			base.sm.messstation.Set(EatChore.StatesInstance.GetPreferredMessStation(base.sm.dupe.Get(this).GetComponent<MinionIdentity>()), this);
		}

		public void ShowElectrobankSymbol(bool show, KAnim.Build.Symbol symbol)
		{
			SymbolOverrideController component = base.GetComponent<SymbolOverrideController>();
			if (show)
			{
				component.AddSymbolOverride(ReloadElectrobankChore.Instance.SYMBOL_NAME, symbol, 0);
			}
			else
			{
				component.RemoveSymbolOverride(ReloadElectrobankChore.Instance.SYMBOL_NAME, 0);
			}
			base.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(ReloadElectrobankChore.Instance.SYMBOL_NAME, show);
		}

		public void StowElectrobank(bool stow)
		{
			GameObject gameObject = base.sm.pickedUpElectrobank.Get(this);
			ReloadElectrobankChore.SetStoredItemVisibility(gameObject, stow);
			KAnim.Build.Symbol symbol = ((gameObject != null) ? gameObject.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbolByIndex(0U) : base.sm.defaultElectrobankSymbol);
			this.ShowElectrobankSymbol(!stow, symbol);
		}

		private static readonly string SYMBOL_NAME = "object";
	}
}
