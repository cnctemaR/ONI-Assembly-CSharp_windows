using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Desalinator : StateMachineComponent<Desalinator.StatesInstance>
{
	public float SaltStorageLeft
	{
		get
		{
			return this._storageLeft;
		}
		set
		{
			this._storageLeft = value;
			base.smi.sm.saltStorageLeft.Set(value, base.smi, false);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.deliveryComponents = base.GetComponents<ManualDeliveryKG>();
		this.OnConduitConnectionChanged(base.GetComponent<ConduitConsumer>().IsConnected);
		base.Subscribe<Desalinator>(-2094018600, Desalinator.OnConduitConnectionChangedDelegate);
		base.smi.StartSM();
	}

	private void OnConduitConnectionChanged(object data)
	{
		bool flag = (bool)data;
		foreach (ManualDeliveryKG manualDeliveryKG in this.deliveryComponents)
		{
			Element element = ElementLoader.GetElement(manualDeliveryKG.RequestedItemTag);
			if (element != null && element.IsLiquid)
			{
				manualDeliveryKG.Pause(flag, "pipe connected");
			}
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.smi.GetCurrentState() == base.smi.sm.full || !base.smi.HasSalt || base.smi.emptyChore != null)
		{
			return;
		}
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("status_item_desalinator_needs_emptying", UI.USERMENUACTIONS.EMPTYDESALINATOR.NAME, delegate
		{
			base.smi.GoTo(base.smi.sm.earlyEmpty);
		}, global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.CLEANTOILET.TOOLTIP, true), 1f);
	}

	private bool CheckCanConvert()
	{
		if (this.converters == null)
		{
			this.converters = base.GetComponents<ElementConverter>();
		}
		for (int i = 0; i < this.converters.Length; i++)
		{
			if (this.converters[i].CanConvertAtAll())
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckEnoughMassToConvert()
	{
		if (this.converters == null)
		{
			this.converters = base.GetComponents<ElementConverter>();
		}
		for (int i = 0; i < this.converters.Length; i++)
		{
			if (this.converters[i].HasEnoughMassToStartConverting(false))
			{
				return true;
			}
		}
		return false;
	}

	[MyCmpAdd]
	private ManuallySetRemoteWorkTargetComponent remoteChore;

	[MyCmpGet]
	private Operational operational;

	private ManualDeliveryKG[] deliveryComponents;

	[MyCmpReq]
	private Storage storage;

	[Serialize]
	public float maxSalt = 1000f;

	[Serialize]
	private float _storageLeft = 1000f;

	private ElementConverter[] converters;

	private static readonly EventSystem.IntraObjectHandler<Desalinator> OnConduitConnectionChangedDelegate = new EventSystem.IntraObjectHandler<Desalinator>(delegate(Desalinator component, object data)
	{
		component.OnConduitConnectionChanged(data);
	});

	public class StatesInstance : GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.GameInstance
	{
		public StatesInstance(Desalinator smi)
			: base(smi)
		{
		}

		public bool HasSalt
		{
			get
			{
				return base.master.storage.Has(ElementLoader.FindElementByHash(SimHashes.Salt).tag);
			}
		}

		public bool IsFull()
		{
			return base.master.SaltStorageLeft <= 0f;
		}

		public bool IsSaltRemoved()
		{
			return !this.HasSalt;
		}

		public void CreateEmptyChore()
		{
			if (this.emptyChore != null)
			{
				this.emptyChore.Cancel("dupe");
			}
			DesalinatorWorkableEmpty component = base.master.GetComponent<DesalinatorWorkableEmpty>();
			this.emptyChore = new WorkChore<DesalinatorWorkableEmpty>(Db.Get().ChoreTypes.EmptyDesalinator, component, null, true, new Action<Chore>(this.OnEmptyComplete), null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true, true);
			base.smi.master.remoteChore.SetChore(this.emptyChore);
		}

		public void CancelEmptyChore()
		{
			if (this.emptyChore != null)
			{
				this.emptyChore.Cancel("Cancelled");
				this.emptyChore = null;
				base.smi.master.remoteChore.SetChore(null);
			}
		}

		private void OnEmptyComplete(Chore chore)
		{
			this.emptyChore = null;
			Tag tag = GameTagExtensions.Create(SimHashes.Salt);
			ListPool<GameObject, Desalinator>.PooledList pooledList = ListPool<GameObject, Desalinator>.Allocate();
			base.master.storage.Find(tag, pooledList);
			foreach (GameObject gameObject in pooledList)
			{
				base.master.storage.Drop(gameObject, true);
			}
			pooledList.Recycle();
		}

		public void UpdateStorageLeft()
		{
			Tag tag = GameTagExtensions.Create(SimHashes.Salt);
			base.master.SaltStorageLeft = base.master.maxSalt - base.master.storage.GetMassAvailable(tag);
		}

		public Chore emptyChore;
	}

	public class States : GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (Desalinator.StatesInstance smi) => smi.master.operational.IsOperational);
			this.on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, this.off, (Desalinator.StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(this.on.waiting);
			this.on.waiting.EventTransition(GameHashes.OnStorageChange, this.on.working_pre, (Desalinator.StatesInstance smi) => smi.master.CheckEnoughMassToConvert());
			this.on.working_pre.PlayAnim("working_pre").OnAnimQueueComplete(this.on.working);
			this.on.working.Enter(delegate(Desalinator.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).QueueAnim("working_loop", true, null).EventTransition(GameHashes.OnStorageChange, this.on.working_pst, (Desalinator.StatesInstance smi) => !smi.master.CheckCanConvert())
				.ParamTransition<float>(this.saltStorageLeft, this.full, (Desalinator.StatesInstance smi, float p) => smi.IsFull())
				.EventHandler(GameHashes.OnStorageChange, delegate(Desalinator.StatesInstance smi)
				{
					smi.UpdateStorageLeft();
				})
				.Exit(delegate(Desalinator.StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				});
			this.on.working_pst.PlayAnim("working_pst").OnAnimQueueComplete(this.on.waiting);
			this.earlyEmpty.PlayAnims((Desalinator.StatesInstance smi) => Desalinator.States.FULL_ANIMS, KAnim.PlayMode.Once).OnAnimQueueComplete(this.earlyWaitingForEmpty);
			this.earlyWaitingForEmpty.Enter(delegate(Desalinator.StatesInstance smi)
			{
				smi.CreateEmptyChore();
			}).Exit(delegate(Desalinator.StatesInstance smi)
			{
				smi.CancelEmptyChore();
			}).EventTransition(GameHashes.OnStorageChange, this.empty, (Desalinator.StatesInstance smi) => smi.IsSaltRemoved());
			this.full.PlayAnims((Desalinator.StatesInstance smi) => Desalinator.States.FULL_ANIMS, KAnim.PlayMode.Once).OnAnimQueueComplete(this.fullWaitingForEmpty);
			this.fullWaitingForEmpty.Enter(delegate(Desalinator.StatesInstance smi)
			{
				smi.CreateEmptyChore();
			}).Exit(delegate(Desalinator.StatesInstance smi)
			{
				smi.CancelEmptyChore();
			}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.DesalinatorNeedsEmptying, null)
				.EventTransition(GameHashes.OnStorageChange, this.empty, (Desalinator.StatesInstance smi) => smi.IsSaltRemoved());
			this.empty.PlayAnim("off").Enter("ResetStorage", delegate(Desalinator.StatesInstance smi)
			{
				smi.master.SaltStorageLeft = smi.master.maxSalt;
			}).GoTo(this.on.waiting);
		}

		public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State off;

		public Desalinator.States.OnStates on;

		public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State full;

		public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State fullWaitingForEmpty;

		public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State earlyEmpty;

		public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State earlyWaitingForEmpty;

		public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State empty;

		private static readonly HashedString[] FULL_ANIMS = new HashedString[] { "working_pst", "off" };

		public StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.FloatParameter saltStorageLeft = new StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.FloatParameter(0f);

		public class OnStates : GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State
		{
			public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State waiting;

			public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State working_pre;

			public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State working;

			public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State working_pst;
		}
	}
}
