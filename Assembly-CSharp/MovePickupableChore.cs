using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class MovePickupableChore : Chore<MovePickupableChore.StatesInstance>
{
	public MovePickupableChore(IStateMachineTarget target, GameObject pickupable, Action<Chore> onEnd)
		: base((!Movable.IsCritterPickupable(pickupable)) ? Db.Get().ChoreTypes.Fetch : Db.Get().ChoreTypes.Ranch, target, target.GetComponent<ChoreProvider>(), false, null, null, onEnd, PriorityScreen.PriorityClass.basic, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new MovePickupableChore.StatesInstance(this);
		Pickupable component = pickupable.GetComponent<Pickupable>();
		this.AddPrecondition(ChorePreconditions.instance.CanMoveTo, target.GetComponent<Storage>());
		this.AddPrecondition(ChorePreconditions.instance.IsNotARobot, "FetchDrone");
		this.AddPrecondition(ChorePreconditions.instance.IsNotTransferArm, this);
		if (Movable.IsCritterPickupable(pickupable))
		{
			this.AddPrecondition(MovePickupableChore.CanReachCritter, pickupable);
			this.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanWrangleCreatures);
			this.AddPrecondition(ChorePreconditions.instance.CanMoveTo, pickupable.GetComponent<Capturable>());
		}
		else
		{
			this.AddPrecondition(ChorePreconditions.instance.CanPickup, component);
		}
		PrimaryElement primaryElement = component.PrimaryElement;
		base.smi.sm.requestedamount.Set(primaryElement.Mass, base.smi, false);
		base.smi.sm.pickupablesource.Set(pickupable.gameObject, base.smi, false);
		base.smi.sm.deliverypoint.Set(target.gameObject, base.smi, false);
		this.movePlacer = target.gameObject;
		bool flag = MinionGroupProber.Get().IsReachable(Grid.PosToCell(pickupable), OffsetGroups.Standard) && MinionGroupProber.Get().IsReachable(Grid.PosToCell(target.gameObject), OffsetGroups.Standard);
		this.OnReachableChanged(flag);
		pickupable.Subscribe(-1432940121, new Action<object>(this.OnReachableChanged));
		target.Subscribe(-1432940121, new Action<object>(this.OnReachableChanged));
		Prioritizable component2 = target.GetComponent<Prioritizable>();
		if (!component2.IsPrioritizable())
		{
			component2.AddRef();
		}
		base.SetPrioritizable(target.GetComponent<Prioritizable>());
	}

	private void OnReachableChanged(object data)
	{
		Color color = (((bool)data) ? Color.white : new Color(0.91f, 0.21f, 0.2f));
		this.SetColor(this.movePlacer, color);
	}

	private void SetColor(GameObject visualizer, Color color)
	{
		if (visualizer != null)
		{
			visualizer.GetComponentInChildren<MeshRenderer>().material.color = color;
		}
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			global::Debug.LogError("MovePickupable null context.consumer");
			return;
		}
		if (base.smi == null)
		{
			global::Debug.LogError("MovePickupable null smi");
			return;
		}
		if (base.smi.sm == null)
		{
			global::Debug.LogError("MovePickupable null smi.sm");
			return;
		}
		if (base.smi.sm.pickupablesource == null)
		{
			global::Debug.LogError("MovePickupable null smi.sm.pickupablesource");
			return;
		}
		base.smi.sm.deliverer.Set(context.consumerState.gameObject, base.smi, false);
		base.Begin(context);
	}

	public GameObject movePlacer;

	public static Chore.Precondition CanReachCritter = new Chore.Precondition
	{
		id = "CanReachCritter",
		description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject = (GameObject)data;
			return !(gameObject == null) && gameObject.HasTag(GameTags.Reachable);
		}
	};

	public class StatesInstance : GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.GameInstance
	{
		public StatesInstance(MovePickupableChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.fetch;
			base.Target(this.deliverypoint);
			this.fetch.Target(this.deliverer).DefaultState(this.fetch.approach).Enter(delegate(MovePickupableChore.StatesInstance smi)
			{
				this.pickupablesource.Get<Pickupable>(smi).ClearReservations();
			})
				.ToggleReserve(this.deliverer, this.pickupablesource, this.requestedamount, this.actualamount)
				.EnterTransition(this.fetch.approachCritter, (MovePickupableChore.StatesInstance smi) => this.IsCritter(smi))
				.OnTargetLost(this.pickupablesource, null);
			this.fetch.approachCritter.Enter(delegate(MovePickupableChore.StatesInstance smi)
			{
				GameObject gameObject = this.pickupablesource.Get(smi);
				if (!gameObject.HasTag(GameTags.Creatures.Bagged))
				{
					IdleStates.Instance smi2 = gameObject.GetSMI<IdleStates.Instance>();
					if (!smi2.IsNullOrStopped())
					{
						smi2.GoTo(smi2.sm.root);
					}
					FlopStates.Instance smi3 = gameObject.GetSMI<FlopStates.Instance>();
					if (!smi3.IsNullOrStopped())
					{
						smi3.GoTo(smi3.sm.root);
					}
					gameObject.GetComponent<Navigator>().Stop(false, true);
				}
			}).MoveTo<Capturable>(this.pickupablesource, this.fetch.wrangle, null, null, null);
			this.fetch.wrangle.EnterTransition(this.fetch.approach, (MovePickupableChore.StatesInstance smi) => this.pickupablesource.Get(smi).HasTag(GameTags.Creatures.Bagged)).ToggleWork<Capturable>(this.pickupablesource, this.fetch.approach, null, null);
			this.fetch.approach.MoveTo<IApproachable>(this.pickupablesource, this.fetch.pickup, new Func<MovePickupableChore.StatesInstance, CellOffset[]>(this.GetFetcherOffset), null, null);
			this.fetch.pickup.DoPickup(this.pickupablesource, this.pickup, this.actualamount, this.approachstorage, this.delivering.deliverfail).Exit(delegate(MovePickupableChore.StatesInstance smi)
			{
				GameObject gameObject2 = this.pickup.Get(smi);
				Movable movable = ((gameObject2 != null) ? gameObject2.GetComponent<Movable>() : null);
				if (movable != null && movable.onPickupComplete != null)
				{
					movable.onPickupComplete(gameObject2);
				}
			});
			this.approachstorage.DefaultState(this.approachstorage.deliveryStorage);
			this.approachstorage.deliveryStorage.InitializeStates(this.deliverer, this.deliverypoint, new Func<MovePickupableChore.StatesInstance, CellOffset[]>(this.GetFetcherOffset), this.delivering.storing, this.delivering.deliverfail, NavigationTactics.ReduceTravelDistance);
			this.delivering.storing.Target(this.deliverer).DoDelivery(this.deliverer, this.deliverypoint, this.success, this.delivering.deliverfail);
			this.delivering.deliverfail.ReturnFailure();
			this.success.Enter(delegate(MovePickupableChore.StatesInstance smi)
			{
				Storage component = this.deliverypoint.Get(smi).GetComponent<Storage>();
				Storage component2 = this.deliverer.Get(smi).GetComponent<Storage>();
				float num = this.actualamount.Get(smi);
				GameObject gameObject3 = this.pickup.Get(smi);
				num += gameObject3.GetComponent<PrimaryElement>().Mass;
				this.actualamount.Set(num, smi, false);
				component2.Transfer(this.pickup.Get(smi), component, false, false);
				this.DropPickupable(component, gameObject3);
				CancellableMove component3 = component.GetComponent<CancellableMove>();
				Movable component4 = gameObject3.GetComponent<Movable>();
				component3.RemoveMovable(component4);
				component4.ClearMove();
				if (!this.IsDeliveryComplete(smi))
				{
					GameObject gameObject4 = this.pickupablesource.Get(smi);
					int num2 = Grid.PosToCell(this.deliverypoint.Get(smi));
					if (this.pickupablesource.Get(smi) == null || Grid.PosToCell(gameObject4) == num2)
					{
						GameObject nextTarget = component3.GetNextTarget();
						this.pickupablesource.Set(nextTarget, smi, false);
						PrimaryElement component5 = nextTarget.GetComponent<PrimaryElement>();
						smi.sm.requestedamount.Set(component5.Mass, smi, false);
					}
					smi.GoTo(this.fetch);
				}
			}).ReturnSuccess();
		}

		private CellOffset[] GetFetcherOffset(MovePickupableChore.StatesInstance smi)
		{
			return this.deliverer.Get(smi).GetComponent<WorkerBase>().GetFetchCellOffsets();
		}

		private void DropPickupable(Storage storage, GameObject delivered)
		{
			if (delivered.GetComponent<Capturable>() != null)
			{
				List<GameObject> items = storage.items;
				int count = items.Count;
				Vector3 vector = Grid.CellToPosCBC(Grid.PosToCell(storage), Grid.SceneLayer.Creatures);
				for (int i = count - 1; i >= 0; i--)
				{
					GameObject gameObject = items[i];
					storage.Drop(gameObject, true);
					gameObject.transform.SetPosition(vector);
					gameObject.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
				}
			}
			else
			{
				storage.DropAll(false, false, default(Vector3), true, null);
			}
			Movable component = delivered.GetComponent<Movable>();
			if (component.onDeliveryComplete != null)
			{
				component.onDeliveryComplete(delivered);
			}
		}

		private bool IsDeliveryComplete(MovePickupableChore.StatesInstance smi)
		{
			GameObject gameObject = smi.sm.deliverypoint.Get(smi);
			return !(gameObject != null) || gameObject.GetComponent<CancellableMove>().IsDeliveryComplete();
		}

		private bool IsCritter(MovePickupableChore.StatesInstance smi)
		{
			GameObject gameObject = this.pickupablesource.Get(smi);
			return gameObject != null && gameObject.GetComponent<Capturable>() != null;
		}

		public static CellOffset[] critterCellOffsets = new CellOffset[]
		{
			new CellOffset(0, 0)
		};

		public static HashedString[] critterReleaseWorkAnims = new HashedString[] { "place", "release" };

		public static KAnimFile[] critterReleaseAnim = new KAnimFile[] { Assets.GetAnim("anim_restrain_creature_kanim") };

		public StateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.TargetParameter deliverer;

		public StateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.TargetParameter pickupablesource;

		public StateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.TargetParameter pickup;

		public StateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.TargetParameter deliverypoint;

		public StateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.FloatParameter requestedamount;

		public StateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.FloatParameter actualamount;

		public MovePickupableChore.States.FetchState fetch;

		public MovePickupableChore.States.ApproachStorage approachstorage;

		public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State success;

		public MovePickupableChore.States.DeliveryState delivering;

		public class ApproachStorage : GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State
		{
			public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.ApproachSubState<Storage> deliveryStorage;

			public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.ApproachSubState<Storage> unbagCritter;
		}

		public class DeliveryState : GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State
		{
			public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State storing;

			public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State deliverfail;
		}

		public class FetchState : GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State
		{
			public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.ApproachSubState<Pickupable> approach;

			public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State pickup;

			public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State approachCritter;

			public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State wrangle;
		}
	}
}
