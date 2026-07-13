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

	private static void SetZ(GameObject go, float z)
	{
		Vector3 position = go.transform.GetPosition();
		position.z = z;
		go.transform.SetPosition(position);
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
		Storage[] storages = smi.Storages;
		for (int i = 0; i < storages.Length; i++)
		{
			if (storages[i] != smi.batteryMonitor.storage && storages[i].FindFirst(GameTags.ChargedPortableBattery) != null)
			{
				storages[i].Transfer(smi.batteryMonitor.storage, false, false);
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
		KBatchedAnimTracker kbatchedAnimTracker;
		if (item.TryGetComponent<KBatchedAnimTracker>(out kbatchedAnimTracker))
		{
			kbatchedAnimTracker.enabled = visible;
		}
		Storage.MakeItemInvisible(item, !visible, false);
	}

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
			return EatChore.IsMessStationNonOperational(messStation);
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
			}).Exit(delegate(ReloadElectrobankChore.Instance smi)
			{
				smi.ClearMessStation();
			});
			this.fetch.InitializeStates(this.dupe, this.electrobankSource, this.pickedUpElectrobank, this.amountRequested, this.actualunits, this.installAtMessStation, null).OnTargetLost(this.electrobankSource, this.electrobankLost);
			this.installAtMessStation.EnterTransition(this.installAtSafeLocation, (ReloadElectrobankChore.Instance smi) => this.IsMessStationInvalid(this.messstation.Get(smi))).DefaultState(this.installAtMessStation.approach).ParamTransition<GameObject>(this.messstation, this.installAtSafeLocation, (ReloadElectrobankChore.Instance _, GameObject messStation) => this.IsMessStationInvalid(messStation));
			this.installAtMessStation.approach.InitializeStates(this.dupe, this.messstation, this.installAtMessStation.removeDepletedBatteries, this.installAtSafeLocation, null, null);
			this.installAtMessStation.removeDepletedBatteries.InitializeStates(this.installAtMessStation.install);
			this.installAtMessStation.install.InitializeStates(this.complete, new ReloadElectrobankChore.States.MessStationInstallBatteryAnim()).Enter(delegate(ReloadElectrobankChore.Instance smi)
			{
				GameObject gameObject = this.dupe.Get(smi);
				smi.eatAnim = EatChore.StatesInstance.OnEnterMessStation(this.messstation.Get(smi), gameObject, this.pickedUpElectrobank.Get(smi), true, new float?(1800f));
				ReloadElectrobankChore.SetZ(gameObject, Grid.GetLayerZ(Grid.SceneLayer.BuildingFront));
			}).Exit(delegate(ReloadElectrobankChore.Instance smi)
			{
				GameObject gameObject2 = this.dupe.Get(smi);
				EatChore.StatesInstance.OnExitMessStation(this.messstation.Get(smi), gameObject2, smi.eatAnim);
				ReloadElectrobankChore.SetZ(gameObject2, Grid.GetLayerZ(Grid.SceneLayer.Move));
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

		public struct WorkerSnapshot
		{
			public bool hasHat;

			public bool hasSalt;
		}

		public interface IInstallBatteryAnim
		{
			HashedString GetBank(ReloadElectrobankChore.Instance smi);

			string GetPrefix(ReloadElectrobankChore.Instance smi, ReloadElectrobankChore.States.IInstallBatteryAnim.Anim anim);

			bool ForceFacing();

			public enum Anim
			{
				Pre,
				Idle,
				Convo,
				Pst
			}
		}

		public class DefaultInstallBatteryAnim : ReloadElectrobankChore.States.IInstallBatteryAnim
		{
			public HashedString GetBank(ReloadElectrobankChore.Instance _)
			{
				return ReloadElectrobankChore.States.DefaultInstallBatteryAnim.bank;
			}

			public string GetPrefix(ReloadElectrobankChore.Instance _smi, ReloadElectrobankChore.States.IInstallBatteryAnim.Anim _anim)
			{
				return "consume";
			}

			public bool ForceFacing()
			{
				return false;
			}

			private static readonly HashedString bank = "anim_bionic_kanim";
		}

		public class MessStationInstallBatteryAnim : ReloadElectrobankChore.States.IInstallBatteryAnim
		{
			public HashedString GetBank(ReloadElectrobankChore.Instance smi)
			{
				IDiningSeat diningSeat = EatChore.ResolveDiningSeat(smi.sm.messstation.Get(smi));
				if (diningSeat == null)
				{
					return MessStation.reloadElectrobankAnim;
				}
				return diningSeat.ReloadElectrobankAnim;
			}

			public string GetPrefix(ReloadElectrobankChore.Instance smi, ReloadElectrobankChore.States.IInstallBatteryAnim.Anim anim)
			{
				bool hasHat = smi.workerSnapshot.hasHat;
				bool hasSalt = smi.workerSnapshot.hasSalt;
				if (hasSalt && hasHat)
				{
					return "salt_hat";
				}
				if (hasSalt)
				{
					return "salt";
				}
				if (!hasHat)
				{
					return "working";
				}
				if (anim == ReloadElectrobankChore.States.IInstallBatteryAnim.Anim.Idle)
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
			private static ReloadElectrobankChore.States.WorkerSnapshot Snapshot(ReloadElectrobankChore.Instance smi)
			{
				bool flag = smi.Resume != null && smi.Resume.CurrentHat != null;
				bool flag2 = EatChore.StatesInstance.UseSalt(smi.sm.messstation.Get(smi));
				return new ReloadElectrobankChore.States.WorkerSnapshot
				{
					hasHat = flag,
					hasSalt = flag2
				};
			}

			public ReloadElectrobankChore.States.InstallBattery InitializeStates(GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State nextState, ReloadElectrobankChore.States.IInstallBatteryAnim anim)
			{
				base.DefaultState(this.pre).Enter("Install Battery", delegate(ReloadElectrobankChore.Instance smi)
				{
					KAnimFile anim2 = Assets.GetAnim(anim.GetBank(smi));
					smi.AnimController.AddAnims(anim2);
					smi.AnimController.AddAnimOverrides(anim2, 0f);
					smi.StowElectrobank(false);
					if (anim.ForceFacing() && smi.Facing != null)
					{
						smi.Facing.SetFacing(false);
					}
					smi.workerSnapshot = ReloadElectrobankChore.States.InstallBattery.Snapshot(smi);
					smi.diningTimedOut = false;
				}).ScheduleAction("Dining Timeout", 15f, delegate(ReloadElectrobankChore.Instance smi)
				{
					smi.diningTimedOut = true;
				})
					.Exit("Exit Install Battery", delegate(ReloadElectrobankChore.Instance smi)
					{
						smi.StowElectrobank(true);
						KAnimFile anim3 = Assets.GetAnim(anim.GetBank(smi));
						smi.AnimController.RemoveAnimOverrides(anim3);
						smi.workerSnapshot = default(ReloadElectrobankChore.States.WorkerSnapshot);
						smi.Kpid.RemoveTag(GameTags.DoNotInterruptMe);
					});
				this.pre.PlayAnim((ReloadElectrobankChore.Instance smi) => anim.GetPrefix(smi, ReloadElectrobankChore.States.IInstallBatteryAnim.Anim.Pre) + "_pre", KAnim.PlayMode.Once).ToggleTag(GameTags.SuppressConversation).OnAnimQueueComplete(this.idle)
					.ScheduleGoTo(15f, this.idle);
				this.idle.PlayAnim((ReloadElectrobankChore.Instance smi) => anim.GetPrefix(smi, ReloadElectrobankChore.States.IInstallBatteryAnim.Anim.Idle) + "_loop", KAnim.PlayMode.Once).OnAnimQueueComplete(this.idleOrConvo).ScheduleGoTo(15f, this.idleOrConvo);
				this.idleOrConvo.Enter("IdleOrConvo", delegate(ReloadElectrobankChore.Instance smi)
				{
					if (!smi.Kpid.HasTag(GameTags.CommunalDining) || smi.diningTimedOut)
					{
						smi.GoTo(this.pst);
						return;
					}
					if (smi.Kpid.HasTag(GameTags.WantsToTalk))
					{
						smi.GoTo(this.convo);
						return;
					}
					smi.GoTo(this.idle);
				});
				this.convo.Enter("Convo", delegate(ReloadElectrobankChore.Instance smi)
				{
					smi.Kpid.RemoveTag(GameTags.WantsToTalk);
					smi.AnimController.SetSymbolVisiblity(Edible.SALT_SYMBOL, smi.workerSnapshot.hasSalt);
					smi.AnimController.SetSymbolVisiblity(Edible.HAT_SYMBOL, smi.workerSnapshot.hasHat);
				}).PlayAnim((ReloadElectrobankChore.Instance _) => Edible.convoAnims[global::UnityEngine.Random.Range(0, Edible.convoAnims.Length)], KAnim.PlayMode.Once).OnAnimQueueComplete(this.idleOrConvo)
					.ScheduleGoTo(15f, this.idleOrConvo)
					.Exit("Exit Convo", delegate(ReloadElectrobankChore.Instance smi)
					{
						smi.Kpid.RemoveTag(GameTags.DoNotInterruptMe);
						smi.AnimController.SetSymbolVisiblity(Edible.SALT_SYMBOL, true);
						smi.AnimController.SetSymbolVisiblity(Edible.HAT_SYMBOL, true);
					});
				this.pst.PlayAnim((ReloadElectrobankChore.Instance smi) => anim.GetPrefix(smi, ReloadElectrobankChore.States.IInstallBatteryAnim.Anim.Pst) + "_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(nextState).ScheduleGoTo(15f, nextState);
				return this;
			}

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State pre;

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State idle;

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State idleOrConvo;

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State convo;

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State pst;

			private const float ANIMATION_TIMEOUT = 15f;

			private const float DINING_DURATION_MAXIMUM = 15f;
		}

		public class InstallAtMessStation : GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State
		{
			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.ApproachSubState<IApproachable> approach;

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

		public KPrefabID Kpid { get; private set; }

		public KBatchedAnimController AnimController { get; private set; }

		public SymbolOverrideController SymbolOverrideController { get; private set; }

		public Facing Facing { get; private set; }

		public Storage[] Storages { get; private set; }

		public MinionResume Resume { get; private set; }

		public Instance(ReloadElectrobankChore master, GameObject duplicant)
			: base(master)
		{
			this.Kpid = master.GetComponent<KPrefabID>();
			this.AnimController = master.GetComponent<KBatchedAnimController>();
			this.SymbolOverrideController = master.GetComponent<SymbolOverrideController>();
			this.Facing = master.GetComponent<Facing>();
			this.Storages = master.gameObject.GetComponents<Storage>();
			this.Resume = master.GetComponent<MinionResume>();
		}

		public void UpdateMessStation()
		{
			Assignable assignable = EatChore.StatesInstance.ReserveMessStation(base.sm.messstation.Get(base.smi), base.sm.dupe.Get(base.smi));
			base.sm.messstation.Set(assignable, base.smi);
		}

		public void ClearMessStation()
		{
			GameObject gameObject = base.smi.sm.messstation.Get(base.smi);
			if (gameObject != null)
			{
				gameObject.GetComponent<Reservable>().ClearReservation();
			}
			base.sm.messstation.Set(null, base.smi);
		}

		public void ShowElectrobankSymbol(bool show, KAnim.Build.Symbol symbol)
		{
			if (show)
			{
				this.SymbolOverrideController.AddSymbolOverride(ReloadElectrobankChore.Instance.SYMBOL_NAME, symbol, 0);
			}
			else
			{
				this.SymbolOverrideController.RemoveSymbolOverride(ReloadElectrobankChore.Instance.SYMBOL_NAME, 0);
			}
			this.AnimController.SetSymbolVisiblity(ReloadElectrobankChore.Instance.SYMBOL_NAME, show);
		}

		public void StowElectrobank(bool stow)
		{
			GameObject gameObject = base.sm.pickedUpElectrobank.Get(this);
			ReloadElectrobankChore.SetStoredItemVisibility(gameObject, stow);
			KAnim.Build.Symbol symbol = ((gameObject != null) ? gameObject.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbolByIndex(0U) : base.sm.defaultElectrobankSymbol);
			this.ShowElectrobankSymbol(!stow, symbol);
		}

		public ReloadElectrobankChore.States.WorkerSnapshot workerSnapshot;

		public bool diningTimedOut;

		public KAnimFile eatAnim;

		private static readonly HashedString SYMBOL_NAME = "object";
	}
}
