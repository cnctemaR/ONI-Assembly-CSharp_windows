using System;
using UnityEngine;

public class Rottable : GameStateMachine<Rottable, Rottable.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.preserved;
		base.serializable = true;
		this.refrigerated.ToggleStatusItem(Db.Get().CreatureStatusItems.Refrigerated, null).EventTransition(GameHashes.Unrefrigerated, this.preserved, null);
		this.preserved.InitializeStates(this).EventTransition(GameHashes.Refrigerated, this.refrigerated, null).ToggleStatusItem(Db.Get().CreatureStatusItems.Preserved, null)
			.ToggleSchedulePeriodic("TryRotting", 0.5f, delegate(Rottable.Instance smi)
			{
				if (smi.IsRotting())
				{
					smi.GoTo(this.rotting);
				}
			});
		this.rotting.InitializeStates(this).EventTransition(GameHashes.Refrigerated, this.refrigerated, null).ParamTransition<float>(this.rotAmount, this.spoiled, (Rottable.Instance smi, float p) => p > smi.foodInfo.SpoilTime)
			.ToggleStatusItem(Db.Get().CreatureStatusItems.Rotting, null)
			.ToggleSchedulePeriodic("TryPreserving", 0.5f, delegate(Rottable.Instance smi)
			{
				if (!smi.IsRotting())
				{
					smi.GoTo(this.preserved);
				}
				else
				{
					this.rotAmount.Set(this.rotAmount.Get(smi) + 0.5f, smi);
				}
			});
		this.spoiled.ToggleStatusItem(Db.Get().CreatureStatusItems.Spoiled, null).ScheduleGoTo(20f, this.dead);
		this.dead.Enter(delegate(Rottable.Instance smi)
		{
			Util.KDestroyGameObject(smi.gameObject);
		});
	}

	private const float ROT_INTERVAL = 0.5f;

	public StateMachine<Rottable, Rottable.Instance, IStateMachineTarget>.FloatParameter rotAmount;

	public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget>.State refrigerated;

	public Rottable.RotState preserved;

	public Rottable.RotState rotting;

	public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget>.State spoiled;

	public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget>.State dead;

	public class RotState : GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget>.State
	{
		public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget>.State InitializeStates(Rottable parent)
		{
			base.DefaultState(this.fresh);
			this.fresh.ParamTransition<float>(parent.rotAmount, this.stale, (Rottable.Instance smi, float p) => p > smi.foodInfo.StaleTime).ToggleStatusItem(Db.Get().CreatureStatusItems.Fresh, null);
			this.stale.ParamTransition<float>(parent.rotAmount, this.fresh, (Rottable.Instance smi, float p) => p < smi.foodInfo.StaleTime).ToggleStatusItem(Db.Get().CreatureStatusItems.Stale, null);
			return this;
		}

		public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget>.State fresh;

		public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget>.State stale;
	}

	public new class Instance : GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master, EdiblesManager.FoodInfo foodInfo)
			: base(master)
		{
			this.foodInfo = foodInfo;
			this.pickupable = base.gameObject.RequireComponent<Pickupable>();
			base.master.Subscribe(-2064133523, new EventSystem.EventHandler(this.OnAbsorb));
			base.master.Subscribe(1335436905, new EventSystem.EventHandler(this.OnSplitFromChunk));
		}

		public float RotAmount
		{
			get
			{
				return base.sm.rotAmount.Get(this);
			}
			set
			{
				base.sm.rotAmount.Set(value, this);
			}
		}

		public bool IsRotting()
		{
			if (this.foodInfo == null)
			{
				return false;
			}
			if (!Grid.IsValidCell(Grid.PosToCell(base.gameObject)))
			{
				return false;
			}
			if (Grid.Temperature[Grid.PosToCell(base.gameObject)] < this.foodInfo.RotTemperature)
			{
				return false;
			}
			if (this.pickupable.storage != null)
			{
				StructureTemperature component = this.pickupable.storage.GetComponent<StructureTemperature>();
				if (component != null && component.Temperature < this.foodInfo.RotTemperature)
				{
					return false;
				}
			}
			return true;
		}

		private void OnAbsorb(object data)
		{
			GameObject gameObject = (GameObject)data;
			if (gameObject != null)
			{
				PrimaryElement component = base.gameObject.GetComponent<PrimaryElement>();
				PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
				Rottable.Instance smi = gameObject.GetSMI<Rottable.Instance>();
				if (component != null && component2 != null && smi != null)
				{
					float num = component.Units * base.sm.rotAmount.Get(base.smi);
					float num2 = component2.Units * base.sm.rotAmount.Get(smi);
					float num3 = (num + num2) / (component.Units + component2.Units);
					base.sm.rotAmount.Set(num3, base.smi);
				}
			}
		}

		private void OnSplitFromChunk(object data)
		{
			Pickupable pickupable = (Pickupable)data;
			if (pickupable != null)
			{
				Rottable.Instance smi = pickupable.GetSMI<Rottable.Instance>();
				if (smi != null)
				{
					this.RotAmount = smi.RotAmount;
				}
			}
		}

		public EdiblesManager.FoodInfo foodInfo;

		private Pickupable pickupable;
	}
}
