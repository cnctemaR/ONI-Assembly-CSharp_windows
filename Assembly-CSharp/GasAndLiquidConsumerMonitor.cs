using System;

public class GasAndLiquidConsumerMonitor : GameStateMachine<GasAndLiquidConsumerMonitor, GasAndLiquidConsumerMonitor.Instance, IStateMachineTarget, GasAndLiquidConsumerMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.cooldown;
		this.cooldown.Enter("ClearTargetCell", delegate(GasAndLiquidConsumerMonitor.Instance smi)
		{
			smi.ClearTargetCell();
		}).ScheduleGoTo((GasAndLiquidConsumerMonitor.Instance smi) => smi.def.mininmumTimeBetweenMeals, this.satisfied);
		this.satisfied.Enter("ClearTargetCell", delegate(GasAndLiquidConsumerMonitor.Instance smi)
		{
			smi.ClearTargetCell();
		}).TagTransition(GameTags.Creatures.Hungry, this.lookingforfood, false);
		this.lookingforfood.ToggleBehaviour(GameTags.Creatures.WantsToEat, (GasAndLiquidConsumerMonitor.Instance smi) => smi.targetCell != -1, delegate(GasAndLiquidConsumerMonitor.Instance smi)
		{
			smi.GoTo(this.cooldown);
		}).TagTransition(GameTags.Creatures.Hungry, this.satisfied, true).Update("FindFood", delegate(GasAndLiquidConsumerMonitor.Instance smi, float dt)
		{
			smi.FindFood();
		}, UpdateRate.SIM_1000ms, false);
	}

	private GameStateMachine<GasAndLiquidConsumerMonitor, GasAndLiquidConsumerMonitor.Instance, IStateMachineTarget, GasAndLiquidConsumerMonitor.Def>.State cooldown;

	private GameStateMachine<GasAndLiquidConsumerMonitor, GasAndLiquidConsumerMonitor.Instance, IStateMachineTarget, GasAndLiquidConsumerMonitor.Def>.State satisfied;

	private GameStateMachine<GasAndLiquidConsumerMonitor, GasAndLiquidConsumerMonitor.Instance, IStateMachineTarget, GasAndLiquidConsumerMonitor.Def>.State lookingforfood;

	public class Def : StateMachine.BaseDef
	{
		public Diet diet;

		public float consumptionRate = 0.5f;

		public float mininmumTimeBetweenMeals = 5f;
	}

	public new class Instance : GameStateMachine<GasAndLiquidConsumerMonitor, GasAndLiquidConsumerMonitor.Instance, IStateMachineTarget, GasAndLiquidConsumerMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, GasAndLiquidConsumerMonitor.Def def)
			: base(master, def)
		{
			this.navigator = base.smi.GetComponent<Navigator>();
		}

		public void ClearTargetCell()
		{
			this.targetCell = -1;
			this.massUnavailableFrameCount = 0;
		}

		public void FindFood()
		{
			this.targetCell = -1;
			this.FindTargetGasCell();
		}

		public bool IsConsumableCell(int cell, out Element element)
		{
			element = Grid.Element[cell];
			Diet.Info[] infos = base.smi.def.diet.infos;
			for (int i = 0; i < infos.Length; i++)
			{
				if (infos[i].IsMatch(element.tag))
				{
					return true;
				}
			}
			return false;
		}

		public void FindTargetGasCell()
		{
			GasAndLiquidConsumerMonitor.ConsumableCellQuery consumableCellQuery = new GasAndLiquidConsumerMonitor.ConsumableCellQuery(base.smi, 25);
			this.navigator.RunQuery(consumableCellQuery);
			if (consumableCellQuery.success)
			{
				this.targetCell = consumableCellQuery.GetResultCell();
				this.targetElement = consumableCellQuery.targetElement;
			}
		}

		public void Consume(float dt)
		{
			int index = Game.Instance.massConsumedCallbackManager.Add(new Action<Sim.MassConsumedCallback, object>(GasAndLiquidConsumerMonitor.Instance.OnMassConsumedCallback), this, "GasAndLiquidConsumerMonitor").index;
			SimMessages.ConsumeMass(Grid.PosToCell(this), this.targetElement.id, base.def.consumptionRate * dt, 3, index);
		}

		private static void OnMassConsumedCallback(Sim.MassConsumedCallback mcd, object data)
		{
			((GasAndLiquidConsumerMonitor.Instance)data).OnMassConsumed(mcd);
		}

		private void OnMassConsumed(Sim.MassConsumedCallback mcd)
		{
			if (!base.IsRunning())
			{
				return;
			}
			if (mcd.mass <= 0f)
			{
				this.massUnavailableFrameCount++;
				if (this.massUnavailableFrameCount >= 2)
				{
					base.Trigger(801383139, null);
				}
				return;
			}
			this.massUnavailableFrameCount = 0;
			Diet.Info dietInfo = base.def.diet.GetDietInfo(this.targetElement.tag);
			if (dietInfo == null)
			{
				return;
			}
			float num = dietInfo.ConvertConsumptionMassToCalories(mcd.mass);
			CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = new CreatureCalorieMonitor.CaloriesConsumedEvent
			{
				tag = this.targetElement.tag,
				calories = num
			};
			base.Trigger(-2038961714, caloriesConsumedEvent);
		}

		public int targetCell = -1;

		private Element targetElement;

		private Navigator navigator;

		private int massUnavailableFrameCount;
	}

	public class ConsumableCellQuery : PathFinderQuery
	{
		public ConsumableCellQuery(GasAndLiquidConsumerMonitor.Instance smi, int maxIterations)
		{
			this.smi = smi;
			this.maxIterations = maxIterations;
		}

		public override bool IsMatch(int cell, int parent_cell, int cost)
		{
			int num = Grid.CellAbove(cell);
			this.success = this.smi.IsConsumableCell(cell, out this.targetElement) || (Grid.IsValidCell(num) && this.smi.IsConsumableCell(num, out this.targetElement));
			if (!this.success)
			{
				int num2 = this.maxIterations - 1;
				this.maxIterations = num2;
				return num2 <= 0;
			}
			return true;
		}

		public bool success;

		public Element targetElement;

		private GasAndLiquidConsumerMonitor.Instance smi;

		private int maxIterations;
	}
}
