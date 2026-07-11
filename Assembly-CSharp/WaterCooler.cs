using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class WaterCooler : StateMachineComponent<WaterCooler.StatesInstance>, IApproachable
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule);
		}, null, null);
		this.workables = new SocialGatheringPointWorkable[this.socializeOffsets.Length];
		for (int i = 0; i < this.workables.Length; i++)
		{
			int num = Grid.OffsetCell(Grid.PosToCell(this), this.socializeOffsets[i]);
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			GameObject gameObject = ChoreHelpers.CreateLocator("WaterCoolerWorkable", vector);
			SocialGatheringPointWorkable socialGatheringPointWorkable = gameObject.AddOrGet<SocialGatheringPointWorkable>();
			socialGatheringPointWorkable.specificEffect = "Socialized";
			socialGatheringPointWorkable.SetWorkTime(this.workTime);
			this.workables[i] = socialGatheringPointWorkable;
		}
		this.chores = new Chore[this.socializeOffsets.Length];
		Extents extents = new Extents(Grid.PosToCell(this), this.socializeOffsets);
		this.validNavCellChangedPartitionerEntry = GameScenePartitioner.Instance.Add("WaterCooler", this, extents, GameScenePartitioner.Instance.validNavCellChangedLayer, new Action<object>(this.OnCellChanged));
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		if (this.validNavCellChangedPartitionerEntry != null)
		{
			this.validNavCellChangedPartitionerEntry.Release();
			this.validNavCellChangedPartitionerEntry = null;
		}
		this.CancelDrinkChores();
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

	public void UpdateDrinkChores(bool force = true)
	{
		if (!force && !this.choresDirty)
		{
			return;
		}
		float num = this.storage.GetMassAvailable(GameTags.Water);
		int num2 = 0;
		for (int i = 0; i < this.socializeOffsets.Length; i++)
		{
			CellOffset cellOffset = this.socializeOffsets[i];
			Chore chore = this.chores[i];
			bool flag = num2 < this.choreCount && this.IsOffsetValid(cellOffset) && num >= 1f;
			if (flag)
			{
				num2++;
				num -= 1f;
				if (chore == null || chore.isComplete)
				{
					this.chores[i] = new WaterCoolerChore(this, this.workables[i], null, null, new Action<Chore>(this.OnChoreEnd));
				}
			}
			else if (chore != null)
			{
				chore.Cancel("invalid");
				this.chores[i] = null;
			}
		}
		this.choresDirty = false;
	}

	public void CancelDrinkChores()
	{
		for (int i = 0; i < this.socializeOffsets.Length; i++)
		{
			Chore chore = this.chores[i];
			if (chore != null)
			{
				chore.Cancel("cancelled");
				this.chores[i] = null;
			}
		}
	}

	private bool IsOffsetValid(CellOffset offset)
	{
		int num = Grid.PosToCell(this);
		int num2 = Grid.OffsetCell(num, offset);
		int num3 = Grid.CellBelow(num2);
		return GameNavGrids.FloorValidator.IsWalkableCell(num2, num3, Grid.BitFields, false, false);
	}

	private void OnChoreEnd(Chore chore)
	{
		this.choresDirty = true;
	}

	private void OnCellChanged(object data)
	{
		this.choresDirty = true;
	}

	private void OnStorageChange(object data)
	{
		this.choresDirty = true;
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

	public const float DRINK_MASS = 1f;

	public CellOffset[] socializeOffsets = new CellOffset[]
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

	private Chore[] chores;

	private GameScenePartitionerEntry validNavCellChangedPartitionerEntry;

	private SocialGatheringPointWorkable[] workables;

	[MyCmpGet]
	private Storage storage;

	public bool choresDirty;

	public class States : GameStateMachine<WaterCooler.States, WaterCooler.StatesInstance, WaterCooler>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.unoperational.TagTransition(GameTags.Operational, this.waitingfordelivery, false).PlayAnim("off");
			this.waitingfordelivery.TagTransition(GameTags.Operational, this.unoperational, true).Transition(this.dispensing, (WaterCooler.StatesInstance smi) => smi.HasMinimumMass(), UpdateRate.SIM_200ms).EventTransition(GameHashes.OnStorageChange, this.dispensing, (WaterCooler.StatesInstance smi) => smi.HasMinimumMass())
				.PlayAnim("off");
			this.dispensing.Enter("StartMeter", delegate(WaterCooler.StatesInstance smi)
			{
				smi.StartMeter();
			}).Enter("UpdateDrinkChores.force", delegate(WaterCooler.StatesInstance smi)
			{
				smi.master.UpdateDrinkChores(true);
			}).Update("UpdateDrinkChores", delegate(WaterCooler.StatesInstance smi, float dt)
			{
				smi.master.UpdateDrinkChores(true);
			}, UpdateRate.SIM_200ms, false)
				.Exit("CancelDrinkChores", delegate(WaterCooler.StatesInstance smi)
				{
					smi.master.CancelDrinkChores();
				})
				.TagTransition(GameTags.Operational, this.unoperational, true)
				.EventTransition(GameHashes.OnStorageChange, this.waitingfordelivery, (WaterCooler.StatesInstance smi) => !smi.HasMinimumMass())
				.PlayAnim("working");
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

		public bool HasMinimumMass()
		{
			return this.storage.GetMassAvailable(GameTags.Water) >= 1f;
		}

		private Storage storage;

		private MeterController meter;
	}
}
