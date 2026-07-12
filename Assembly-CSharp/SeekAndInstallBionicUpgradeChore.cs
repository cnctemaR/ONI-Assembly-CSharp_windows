using System;
using STRINGS;
using UnityEngine;

public class SeekAndInstallBionicUpgradeChore : Chore<SeekAndInstallBionicUpgradeChore.Instance>
{
	public SeekAndInstallBionicUpgradeChore(IStateMachineTarget target)
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "CanPickupAnyAssignedUpgrade";
		precondition.description = DUPLICANTS.CHORES.PRECONDITIONS.CANPICKUPANYASSIGNEDUPGRADE;
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return ((BionicUpgradesMonitor.Instance)data).GetAnyReachableAssignedSlot() != null;
		};
		precondition.canExecuteOnAnyThread = false;
		this.CanPickupAnyAssignedUpgrade = precondition;
		base..ctor(Db.Get().ChoreTypes.SeekAndInstallUpgrade, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.WorkTime);
		base.smi = new SeekAndInstallBionicUpgradeChore.Instance(this, target.gameObject);
		BionicUpgradesMonitor.Instance smi = target.gameObject.GetSMI<BionicUpgradesMonitor.Instance>();
		this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		this.AddPrecondition(this.CanPickupAnyAssignedUpgrade, smi);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			global::Debug.LogError("SeekAndInstallBionicUpgradeChore null context.consumer");
			return;
		}
		BionicUpgradesMonitor.Instance smi = context.consumerState.consumer.GetSMI<BionicUpgradesMonitor.Instance>();
		if (smi == null)
		{
			global::Debug.LogError("SeekAndInstallBionicUpgradeChore null BionicUpgradesMonitor.Instance");
			return;
		}
		BionicUpgradesMonitor.UpgradeComponentSlot anyReachableAssignedSlot = smi.GetAnyReachableAssignedSlot();
		BionicUpgradeComponent bionicUpgradeComponent = ((anyReachableAssignedSlot == null) ? null : anyReachableAssignedSlot.assignedUpgradeComponent);
		if (bionicUpgradeComponent == null)
		{
			global::Debug.LogError("SeekAndInstallBionicUpgradeChore null upgradeComponent.gameObject");
			return;
		}
		base.smi.sm.initialUpgradeComponent.Set(bionicUpgradeComponent.gameObject, base.smi, false);
		base.smi.sm.dupe.Set(context.consumerState.consumer, base.smi);
		base.Begin(context);
	}

	public static void SetOverrideAnimSymbol(SeekAndInstallBionicUpgradeChore.Instance smi, bool overriding)
	{
		string text = "booster";
		KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
		SymbolOverrideController component2 = smi.gameObject.GetComponent<SymbolOverrideController>();
		GameObject gameObject = smi.sm.pickedUpgrade.Get(smi);
		if (gameObject != null)
		{
			gameObject.GetComponent<KBatchedAnimTracker>().enabled = !overriding;
			Storage.MakeItemInvisible(gameObject, overriding, false);
		}
		if (!overriding)
		{
			component2.RemoveSymbolOverride(text, 0);
			component.SetSymbolVisiblity(text, false);
			return;
		}
		string animStateName = BionicUpgradeComponentConfig.UpgradesData[gameObject.PrefabID()].animStateName;
		KAnim.Build.Symbol symbol = gameObject.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol(animStateName);
		component2.AddSymbolOverride(text, symbol, 0);
		component.SetSymbolVisiblity(text, true);
	}

	public static bool IsBionicUpgradeAssignedTo(GameObject bionicUpgradeGameObject, GameObject ownerInQuestion)
	{
		if (bionicUpgradeGameObject == null)
		{
			return false;
		}
		Assignable component = bionicUpgradeGameObject.GetComponent<BionicUpgradeComponent>();
		IAssignableIdentity component2 = ownerInQuestion.GetComponent<IAssignableIdentity>();
		return component.IsAssignedTo(component2);
	}

	public static void InstallUpgrade(SeekAndInstallBionicUpgradeChore.Instance smi)
	{
		Storage storage = smi.gameObject.GetComponents<Storage>().FindFirst<Storage>((Storage s) => s.storageID == GameTags.StoragesIds.DefaultStorage);
		GameObject gameObject = storage.FindFirst(GameTags.BionicUpgrade);
		if (gameObject != null)
		{
			BionicUpgradeComponent component = gameObject.GetComponent<BionicUpgradeComponent>();
			storage.Remove(component.gameObject, true);
			smi.upgradeMonitor.InstallUpgrade(component);
			if (PopFXManager.Instance != null)
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, component.GetProperName(), smi.gameObject.transform, Vector3.up, 1.5f, true, false);
			}
		}
	}

	private Chore.Precondition CanPickupAnyAssignedUpgrade;

	public class States : GameStateMachine<SeekAndInstallBionicUpgradeChore.States, SeekAndInstallBionicUpgradeChore.Instance, SeekAndInstallBionicUpgradeChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.fetch;
			base.Target(this.dupe);
			this.fetch.InitializeStates(this.dupe, this.initialUpgradeComponent, this.pickedUpgrade, this.amountRequested, this.actualunits, this.install, null).Target(this.initialUpgradeComponent).EventHandlerTransition(GameHashes.AssigneeChanged, null, (SeekAndInstallBionicUpgradeChore.Instance smi, object obj) => !SeekAndInstallBionicUpgradeChore.IsBionicUpgradeAssignedTo(smi.sm.initialUpgradeComponent.Get(smi), smi.gameObject));
			this.install.Target(this.dupe).ToggleAnims("anim_bionic_booster_installation_kanim", 0f).PlayAnim("installation", KAnim.PlayMode.Once)
				.Enter(delegate(SeekAndInstallBionicUpgradeChore.Instance smi)
				{
					SeekAndInstallBionicUpgradeChore.SetOverrideAnimSymbol(smi, true);
				})
				.Exit(delegate(SeekAndInstallBionicUpgradeChore.Instance smi)
				{
					SeekAndInstallBionicUpgradeChore.SetOverrideAnimSymbol(smi, false);
				})
				.OnAnimQueueComplete(this.complete)
				.ScheduleGoTo(10f, this.complete)
				.Target(this.pickedUpgrade)
				.EventHandlerTransition(GameHashes.AssigneeChanged, null, (SeekAndInstallBionicUpgradeChore.Instance smi, object obj) => !SeekAndInstallBionicUpgradeChore.IsBionicUpgradeAssignedTo(smi.sm.pickedUpgrade.Get(smi), smi.gameObject));
			this.complete.Target(this.dupe).Enter(new StateMachine<SeekAndInstallBionicUpgradeChore.States, SeekAndInstallBionicUpgradeChore.Instance, SeekAndInstallBionicUpgradeChore, object>.State.Callback(SeekAndInstallBionicUpgradeChore.InstallUpgrade)).ReturnSuccess();
		}

		public GameStateMachine<SeekAndInstallBionicUpgradeChore.States, SeekAndInstallBionicUpgradeChore.Instance, SeekAndInstallBionicUpgradeChore, object>.FetchSubState fetch;

		public GameStateMachine<SeekAndInstallBionicUpgradeChore.States, SeekAndInstallBionicUpgradeChore.Instance, SeekAndInstallBionicUpgradeChore, object>.State install;

		public GameStateMachine<SeekAndInstallBionicUpgradeChore.States, SeekAndInstallBionicUpgradeChore.Instance, SeekAndInstallBionicUpgradeChore, object>.State complete;

		public StateMachine<SeekAndInstallBionicUpgradeChore.States, SeekAndInstallBionicUpgradeChore.Instance, SeekAndInstallBionicUpgradeChore, object>.TargetParameter dupe;

		public StateMachine<SeekAndInstallBionicUpgradeChore.States, SeekAndInstallBionicUpgradeChore.Instance, SeekAndInstallBionicUpgradeChore, object>.TargetParameter initialUpgradeComponent;

		public StateMachine<SeekAndInstallBionicUpgradeChore.States, SeekAndInstallBionicUpgradeChore.Instance, SeekAndInstallBionicUpgradeChore, object>.TargetParameter pickedUpgrade;

		public StateMachine<SeekAndInstallBionicUpgradeChore.States, SeekAndInstallBionicUpgradeChore.Instance, SeekAndInstallBionicUpgradeChore, object>.FloatParameter actualunits;

		public StateMachine<SeekAndInstallBionicUpgradeChore.States, SeekAndInstallBionicUpgradeChore.Instance, SeekAndInstallBionicUpgradeChore, object>.FloatParameter amountRequested = new StateMachine<SeekAndInstallBionicUpgradeChore.States, SeekAndInstallBionicUpgradeChore.Instance, SeekAndInstallBionicUpgradeChore, object>.FloatParameter(1f);
	}

	public class Instance : GameStateMachine<SeekAndInstallBionicUpgradeChore.States, SeekAndInstallBionicUpgradeChore.Instance, SeekAndInstallBionicUpgradeChore, object>.GameInstance
	{
		public BionicUpgradesMonitor.Instance upgradeMonitor
		{
			get
			{
				return base.sm.dupe.Get(this).GetSMI<BionicUpgradesMonitor.Instance>();
			}
		}

		public Instance(SeekAndInstallBionicUpgradeChore master, GameObject duplicant)
			: base(master)
		{
		}
	}
}
