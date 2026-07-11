using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class WaterCooler : StateMachineComponent<WaterCooler.StatesInstance>, IApproachable
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.workables = new SocialGatheringPointWorkable[this.choreOffsets.Length];
		for (int i = 0; i < this.workables.Length; i++)
		{
			int num = Grid.OffsetCell(Grid.PosToCell(this), this.choreOffsets[i]);
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			GameObject gameObject = ChoreHelpers.CreateLocator("WaterCoolerWorkable", vector);
			SocialGatheringPointWorkable socialGatheringPointWorkable = gameObject.AddOrGet<SocialGatheringPointWorkable>();
			socialGatheringPointWorkable.specificEffect = "Socialized";
			socialGatheringPointWorkable.SetWorkTime(this.workTime);
			this.workables[i] = socialGatheringPointWorkable;
		}
		this.tracker = new SocialChoreTracker(base.gameObject, this.choreOffsets);
		this.tracker.choreCount = this.choreCount;
		this.tracker.CreateChoreCB = new Func<int, Chore>(this.CreateChore);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		if (this.tracker != null)
		{
			this.tracker.Clear();
			this.tracker = null;
		}
		for (int i = 0; i < this.workables.Length; i++)
		{
			if (this.workables[i])
			{
				Util.KDestroyGameObject(this.workables[i]);
				this.workables[i] = null;
			}
		}
		base.OnCleanUp();
	}

	private Chore CreateChore(int i)
	{
		Workable workable = this.workables[i];
		return new WaterCoolerChore(this, workable, null, null, new Action<Chore>(this.OnChoreEnd));
	}

	private void OnChoreEnd(Chore chore)
	{
		if (base.gameObject.HasTag(GameTags.Operational))
		{
			this.tracker.Update(true);
		}
	}

	public CellOffset[] GetOffsets()
	{
		return this.drinkOffsets;
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}

	public bool ShouldPreferPrimaryCell()
	{
		throw new NotImplementedException();
	}

	public bool ShouldPreferUnreservedCell()
	{
		throw new NotImplementedException();
	}

	Transform IApproachable.get_transform()
	{
		return base.transform;
	}

	public CellOffset[] choreOffsets = new CellOffset[]
	{
		new CellOffset(-1, 0),
		new CellOffset(2, 0),
		new CellOffset(0, 0),
		new CellOffset(1, 0)
	};

	public int choreCount = 2;

	public float workTime = 5f;

	private CellOffset[] drinkOffsets = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0)
	};

	private SocialChoreTracker tracker;

	private SocialGatheringPointWorkable[] workables;

	public class States : GameStateMachine<WaterCooler.States, WaterCooler.StatesInstance, WaterCooler>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.root.DoNothing();
			this.unoperational.TagTransition(GameTags.Operational, this.waitingfordelivery, false).PlayAnim("off");
			this.waitingfordelivery.TagTransition(GameTags.Operational, this.unoperational, true).EventTransition(GameHashes.OnStorageChange, this.dispensing, (WaterCooler.StatesInstance smi) => !smi.storage.IsEmpty()).Enter("CreateChore", delegate(WaterCooler.StatesInstance smi)
			{
				smi.CreateFetchChore();
			})
				.Exit("CancelChore", delegate(WaterCooler.StatesInstance smi)
				{
					smi.CancelFetchChore();
				})
				.PlayAnim("on");
			this.dispensing.TagTransition(GameTags.Operational, this.unoperational, true).EventTransition(GameHashes.OnStorageChange, this.waitingfordelivery, (WaterCooler.StatesInstance smi) => smi.storage.IsEmpty()).Enter("StartMeter", delegate(WaterCooler.StatesInstance smi)
			{
				smi.StartMeter();
			})
				.Enter("CreateChore", delegate(WaterCooler.StatesInstance smi)
				{
					smi.master.tracker.Update(true);
				})
				.Exit("CancelChore", delegate(WaterCooler.StatesInstance smi)
				{
					smi.master.tracker.Update(false);
				})
				.PlayAnim("on");
		}

		public GameStateMachine<WaterCooler.States, WaterCooler.StatesInstance, WaterCooler, object>.State unoperational;

		public GameStateMachine<WaterCooler.States, WaterCooler.StatesInstance, WaterCooler, object>.State waitingfordelivery;

		public GameStateMachine<WaterCooler.States, WaterCooler.StatesInstance, WaterCooler, object>.State dispensing;
	}

	public class StatesInstance : GameStateMachine<WaterCooler.States, WaterCooler.StatesInstance, WaterCooler, object>.GameInstance
	{
		public StatesInstance(WaterCooler smi)
			: base(smi)
		{
			this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_bottle", "meter", Meter.Offset.Behind, new string[] { "meter_bottle" });
			this.storage = base.master.GetComponent<Storage>();
			base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		}

		public void CreateFetchChore()
		{
			Tag[] array = new Tag[] { GameTags.Water };
			this.chore = new FetchChore(Db.Get().ChoreTypes.Fetch, this.storage, this.storage.Capacity(), array, null, null, null, true, null, null, null, FetchOrder2.OperationalRequirement.Operational, 0, null);
		}

		public void CancelFetchChore()
		{
			if (this.chore != null)
			{
				this.chore.Cancel("Storage Changed");
				this.chore = null;
			}
		}

		private void OnStorageChange(object data)
		{
			float num = Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg);
			this.meter.SetPositionPercent(num);
		}

		public void StartMeter()
		{
			PrimaryElement primaryElement = this.storage.FindFirstWithMass(GameTags.Water);
			if (primaryElement == null)
			{
				return;
			}
			this.meter.SetSymbolTint(new KAnimHashedString("meter_water"), primaryElement.Element.substance.colour);
			this.OnStorageChange(null);
		}

		public Storage storage;

		private FetchChore chore;

		private MeterController meter;
	}
}
