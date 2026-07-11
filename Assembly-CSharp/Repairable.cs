using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Repairable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
		base.Subscribe<Repairable>(493375141, Repairable.OnRefreshUserMenuDelegate);
		this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.showProgressBar = false;
		this.faceTargetWhenWorking = true;
		this.multitoolContext = "build";
		this.multitoolHitEffectTag = EffectConfigs.BuildSplashId;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.smi = new Repairable.SMInstance(this);
		this.smi.StartSM();
		this.workTime = float.PositiveInfinity;
		this.workTimeRemaining = float.PositiveInfinity;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(Handyman.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	private void OnProxyStorageChanged(object data)
	{
		base.Trigger(-1697596308, data);
	}

	protected override void OnLoadLevel()
	{
		this.smi = null;
		base.OnLoadLevel();
	}

	protected override void OnCleanUp()
	{
		if (this.smi != null)
		{
			this.smi.StopSM("Destroy Repairable");
		}
		base.OnCleanUp();
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.gameObject != null && this.smi != null)
		{
			StateMachine.BaseState currentState = this.smi.GetCurrentState();
			if (currentState == this.smi.sm.forbidden)
			{
				UserMenu userMenu = Game.Instance.userMenu;
				GameObject gameObject = base.gameObject;
				string text = "action_repair";
				string text2 = global::STRINGS.BUILDINGS.REPAIRABLE.ENABLE_AUTOREPAIR.NAME;
				global::System.Action action = new global::System.Action(this.AllowRepair);
				string text3 = global::STRINGS.BUILDINGS.REPAIRABLE.ENABLE_AUTOREPAIR.TOOLTIP;
				userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
			}
			else
			{
				UserMenu userMenu2 = Game.Instance.userMenu;
				GameObject gameObject2 = base.gameObject;
				string text3 = "action_repair";
				string text2 = global::STRINGS.BUILDINGS.REPAIRABLE.DISABLE_AUTOREPAIR.NAME;
				global::System.Action action = new global::System.Action(this.CancelRepair);
				string text = global::STRINGS.BUILDINGS.REPAIRABLE.DISABLE_AUTOREPAIR.TOOLTIP;
				userMenu2.AddButton(gameObject2, new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true), 1f);
			}
		}
	}

	private void AllowRepair()
	{
		if (DebugHandler.InstantBuildMode)
		{
			this.hp.Repair(this.hp.MaxHitPoints);
			this.OnCompleteWork(null);
		}
		this.smi.sm.allow.Trigger(this.smi);
		this.OnRefreshUserMenu(null);
	}

	public void CancelRepair()
	{
		if (this.smi != null)
		{
			this.smi.sm.forbid.Trigger(this.smi);
		}
		this.OnRefreshUserMenu(null);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		Operational component = base.GetComponent<Operational>();
		if (component != null)
		{
			component.SetFlag(Repairable.repairedFlag, false);
		}
		this.timeSpentRepairing = 0f;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		float num = Mathf.Sqrt(component.Mass);
		float num2 = ((this.expectedRepairTime >= 0f) ? this.expectedRepairTime : num);
		float num3 = num2 * 0.1f;
		if (this.timeSpentRepairing >= num3)
		{
			this.timeSpentRepairing -= num3;
			int num4 = 0;
			if (worker != null)
			{
				AttributeInstance attributeInstance = Db.Get().Attributes.Machinery.Lookup(worker);
				num4 = (int)attributeInstance.GetTotalValue();
			}
			int num5 = 10 + Math.Max(0, num4 * 10);
			int num6 = Mathf.CeilToInt((float)num5 * 0.1f);
			this.hp.Repair(num6);
			if (this.hp.HitPoints >= this.hp.MaxHitPoints)
			{
				return true;
			}
		}
		this.timeSpentRepairing += dt;
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		Operational component = base.GetComponent<Operational>();
		if (component != null)
		{
			component.SetFlag(Repairable.repairedFlag, true);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Operational component = base.GetComponent<Operational>();
		if (component != null)
		{
			component.SetFlag(Repairable.repairedFlag, true);
		}
	}

	public void CreateStorageProxy()
	{
		if (this.storageProxy == null)
		{
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(RepairableStorageProxy.ID), base.transform.gameObject, null);
			gameObject.transform.SetLocalPosition(Vector3.zero);
			this.storageProxy = gameObject.GetComponent<Storage>();
			this.storageProxy.prioritizable = base.transform.GetComponent<Prioritizable>();
			this.storageProxy.prioritizable.AddRef();
			gameObject.SetActive(true);
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		this.storedData = null;
		if (this.storageProxy != null && !this.storageProxy.IsEmpty())
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					this.storageProxy.Serialize(binaryWriter);
				}
				this.storedData = memoryStream.ToArray();
			}
		}
	}

	[OnSerialized]
	private void OnSerialized()
	{
		this.storedData = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.storedData != null)
		{
			FastReader fastReader = new FastReader(this.storedData);
			this.CreateStorageProxy();
			this.storageProxy.Deserialize(fastReader);
			this.storedData = null;
		}
	}

	public float expectedRepairTime = -1f;

	[MyCmpGet]
	private BuildingHP hp;

	private Repairable.SMInstance smi;

	private Storage storageProxy;

	[Serialize]
	private byte[] storedData;

	private float timeSpentRepairing;

	private static readonly Operational.Flag repairedFlag = new Operational.Flag("repaired", Operational.Flag.Type.Functional);

	private static readonly EventSystem.IntraObjectHandler<Repairable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Repairable>(delegate(Repairable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public class SMInstance : GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.GameInstance
	{
		public SMInstance(Repairable smi)
			: base(smi)
		{
		}

		public bool HasRequiredMass()
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			float num = component.Mass * 0.1f;
			Storage storageProxy = base.smi.master.storageProxy;
			PrimaryElement primaryElement = storageProxy.FindPrimaryElement(component.ElementID);
			return primaryElement != null && primaryElement.Mass >= num;
		}

		public KeyValuePair<Tag, float> GetRequiredMass()
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			float num = component.Mass * 0.1f;
			Storage storageProxy = base.smi.master.storageProxy;
			PrimaryElement primaryElement = storageProxy.FindPrimaryElement(component.ElementID);
			float num2 = ((!(primaryElement != null)) ? num : Math.Max(0f, num - primaryElement.Mass));
			KeyValuePair<Tag, float> keyValuePair = new KeyValuePair<Tag, float>(component.Element.tag, num2);
			return keyValuePair;
		}

		public void ConsumeRepairMaterials()
		{
			base.smi.master.storageProxy.ConsumeAllIgnoringDisease();
		}

		public void DestroyStorageProxy()
		{
			if (base.smi.master.storageProxy != null)
			{
				base.smi.master.transform.GetComponent<Prioritizable>().RemoveRef();
				base.smi.master.storageProxy.DropAll(false);
				Util.KDestroyGameObject(base.smi.master.storageProxy.gameObject);
			}
		}

		public bool NeedsRepairs()
		{
			return base.smi.master.GetComponent<BuildingHP>().NeedsRepairs;
		}

		private const float REQUIRED_MASS_SCALE = 0.1f;
	}

	public class States : GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.repaired;
			base.serializable = true;
			this.forbidden.OnSignal(this.allow, this.repaired);
			this.allowed.Enter(delegate(Repairable.SMInstance smi)
			{
				smi.master.CreateStorageProxy();
			}).DefaultState(this.allowed.needMass).EventHandler(GameHashes.BuildingFullyRepaired, delegate(Repairable.SMInstance smi)
			{
				smi.ConsumeRepairMaterials();
			})
				.EventTransition(GameHashes.BuildingFullyRepaired, this.repaired, null)
				.OnSignal(this.forbid, this.forbidden)
				.Exit(delegate(Repairable.SMInstance smi)
				{
					smi.DestroyStorageProxy();
				});
			this.allowed.needMass.Enter(delegate(Repairable.SMInstance smi)
			{
				Prioritizable.AddRef(smi.master.storageProxy.transform.parent.gameObject);
			}).Exit(delegate(Repairable.SMInstance smi)
			{
				if (!smi.isMasterNull && smi.master.storageProxy != null)
				{
					Prioritizable.RemoveRef(smi.master.storageProxy.transform.parent.gameObject);
				}
			}).EventTransition(GameHashes.OnStorageChange, this.allowed.repairable, (Repairable.SMInstance smi) => smi.HasRequiredMass())
				.ToggleChore(new Func<Repairable.SMInstance, Chore>(this.CreateFetchChore), this.allowed.repairable, this.allowed.needMass)
				.ToggleStatusItem(Db.Get().BuildingStatusItems.WaitingForRepairMaterials, (Repairable.SMInstance smi) => smi.GetRequiredMass());
			this.allowed.repairable.ToggleRecurringChore(new Func<Repairable.SMInstance, Chore>(this.CreateRepairChore), null).ToggleStatusItem(Db.Get().BuildingStatusItems.PendingRepair, null);
			this.repaired.EventTransition(GameHashes.BuildingReceivedDamage, this.allowed, (Repairable.SMInstance smi) => smi.NeedsRepairs()).OnSignal(this.allow, this.allowed).OnSignal(this.forbid, this.forbidden);
		}

		private Chore CreateFetchChore(Repairable.SMInstance smi)
		{
			PrimaryElement component = smi.master.GetComponent<PrimaryElement>();
			Storage storageProxy = smi.master.storageProxy;
			PrimaryElement primaryElement = storageProxy.FindPrimaryElement(component.ElementID);
			float num = component.Mass * 0.1f - ((!(primaryElement != null)) ? 0f : primaryElement.Mass);
			Tag[] array = new Tag[] { GameTagExtensions.Create(component.ElementID) };
			return new FetchChore(Db.Get().ChoreTypes.Fetch, smi.master.storageProxy, num, array, null, null, null, true, null, null, null, FetchOrder2.OperationalRequirement.None, 0, null);
		}

		private Chore CreateRepairChore(Repairable.SMInstance smi)
		{
			WorkChore<Repairable> workChore = new WorkChore<Repairable>(Db.Get().ChoreTypes.Repair, smi.master, null, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true);
			Deconstructable component = smi.master.GetComponent<Deconstructable>();
			if (component != null)
			{
				workChore.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component);
			}
			Breakable component2 = smi.master.GetComponent<Breakable>();
			if (component2 != null)
			{
				workChore.AddPrecondition(Repairable.States.IsNotBeingAttacked, component2);
			}
			workChore.AddPrecondition(Repairable.States.IsNotAngry, null);
			return workChore;
		}

		public StateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.Signal allow;

		public StateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.Signal forbid;

		public GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State forbidden;

		public Repairable.States.AllowedState allowed;

		public GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State repaired;

		public static readonly Chore.Precondition IsNotBeingAttacked = new Chore.Precondition
		{
			id = "IsNotBeingAttacked",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_BEING_ATTACKED,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				bool flag = true;
				if (data != null)
				{
					Breakable breakable = (Breakable)data;
					flag = breakable.worker == null;
				}
				return flag;
			}
		};

		public static readonly Chore.Precondition IsNotAngry = new Chore.Precondition
		{
			id = "IsNotAngry",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_ANGRY,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Traits traits = context.consumerState.traits;
				AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(context.consumerState.gameObject);
				return !(traits != null) || amountInstance == null || amountInstance.value < STRESS.ACTING_OUT_RESET || !traits.HasTrait("Aggressive");
			}
		};

		public class AllowedState : GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State
		{
			public GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State needMass;

			public GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State repairable;
		}
	}
}
