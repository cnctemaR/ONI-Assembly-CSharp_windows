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
[AddComponentMenu("KMonoBehaviour/Workable/Repairable")]
public class Repairable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
		base.Subscribe<Repairable>(493375141, Repairable.OnRefreshUserMenuDelegate);
		this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		this.showProgressBar = false;
		this.faceTargetWhenWorking = true;
		this.multitoolContext = "build";
		this.multitoolHitEffectTag = EffectConfigs.BuildSplashId;
		this.workingPstComplete = null;
		this.workingPstFailed = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.smi = new Repairable.SMInstance(this);
		this.smi.StartSM();
		this.workTime = float.PositiveInfinity;
		this.workTimeRemaining = float.PositiveInfinity;
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
			if (this.smi.GetCurrentState() == this.smi.sm.forbidden)
			{
				Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_repair", global::STRINGS.BUILDINGS.REPAIRABLE.ENABLE_AUTOREPAIR.NAME, new global::System.Action(this.AllowRepair), global::Action.NumActions, null, null, null, global::STRINGS.BUILDINGS.REPAIRABLE.ENABLE_AUTOREPAIR.TOOLTIP, true), 0.5f);
				return;
			}
			Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_repair", global::STRINGS.BUILDINGS.REPAIRABLE.DISABLE_AUTOREPAIR.NAME, new global::System.Action(this.CancelRepair), global::Action.NumActions, null, null, null, global::STRINGS.BUILDINGS.REPAIRABLE.DISABLE_AUTOREPAIR.TOOLTIP, true), 0.5f);
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
		this.smi.sm.worker.Set(worker, this.smi);
		this.timeSpentRepairing = 0f;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		float num = Mathf.Sqrt(base.GetComponent<PrimaryElement>().Mass);
		float num2 = ((this.expectedRepairTime < 0f) ? num : this.expectedRepairTime) * 0.1f;
		if (this.timeSpentRepairing >= num2)
		{
			this.timeSpentRepairing -= num2;
			int num3 = 0;
			if (worker != null)
			{
				num3 = (int)Db.Get().Attributes.Machinery.Lookup(worker).GetTotalValue();
			}
			int num4 = Mathf.CeilToInt((float)(10 + Math.Max(0, num3 * 10)) * 0.1f);
			this.hp.Repair(num4);
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
			PrimaryElement primaryElement = base.smi.master.storageProxy.FindPrimaryElement(component.ElementID);
			return primaryElement != null && primaryElement.Mass >= num;
		}

		public KeyValuePair<Tag, float> GetRequiredMass()
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			float num = component.Mass * 0.1f;
			PrimaryElement primaryElement = base.smi.master.storageProxy.FindPrimaryElement(component.ElementID);
			float num2 = ((primaryElement != null) ? Math.Max(0f, num - primaryElement.Mass) : num);
			return new KeyValuePair<Tag, float>(component.Element.tag, num2);
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
				List<GameObject> list = new List<GameObject>();
				base.smi.master.storageProxy.DropAll(false, false, default(Vector3), true, list);
				GameObject gameObject = base.smi.sm.worker.Get(base.smi);
				if (gameObject != null)
				{
					foreach (GameObject gameObject2 in list)
					{
						gameObject2.Trigger(580035959, gameObject.GetComponent<Worker>());
					}
				}
				base.smi.sm.worker.Set(null, base.smi);
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
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
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
			PrimaryElement primaryElement = smi.master.storageProxy.FindPrimaryElement(component.ElementID);
			float num = component.Mass * 0.1f - ((primaryElement != null) ? primaryElement.Mass : 0f);
			HashSet<Tag> hashSet = new HashSet<Tag> { GameTagExtensions.Create(component.ElementID) };
			return new FetchChore(Db.Get().ChoreTypes.RepairFetch, smi.master.storageProxy, num, hashSet, FetchChore.MatchCriteria.MatchID, Tag.Invalid, null, null, true, null, null, null, Operational.State.None, 0);
		}

		private Chore CreateRepairChore(Repairable.SMInstance smi)
		{
			WorkChore<Repairable> workChore = new WorkChore<Repairable>(Db.Get().ChoreTypes.Repair, smi.master, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true, true);
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

		public StateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.TargetParameter worker;

		public static readonly Chore.Precondition IsNotBeingAttacked = new Chore.Precondition
		{
			id = "IsNotBeingAttacked",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_BEING_ATTACKED,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				bool flag = true;
				if (data != null)
				{
					flag = ((Breakable)data).worker == null;
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
