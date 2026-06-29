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
		this.lookingforfood.ToggleBehaviour(GameTags.Creatures.WantsToEat, (GasAndLiquidConsumerMonitor.Instance smi) => smi.targetCell != PathProber.InvalidCell, delegate(GasAndLiquidConsumerMonitor.Instance smi)
		{
			smi.GoTo(this.cooldown);
		}).TagTransition(GameTags.Creatures.Hungry, this.satisfied, true).Update("FindFood", delegate(GasAndLiquidConsumerMonitor.Instance smi, float dt)
		{
			smi.FindFood();
		}, UpdateRate.SIM_200ms, false);
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
		}

		public void ClearTargetCell()
		{
			this.targetCell = PathProber.InvalidCell;
			this.massUnavailableFrameCount = 0;
		}

		public void FindFood()
		{
			this.targetCell = PathProber.InvalidCell;
			this.FindTargetGasCell();
		}

		public void FindTargetGasCell()
		{
			GameUtil.FloodFillFind(delegate(int test_cell)
			{
				Element element = Grid.Element[test_cell];
				TagBits tagBits = new TagBits(element.tag);
				foreach (Diet.Info info in base.def.diet.infos)
				{
					if (info.IsMatch(tagBits))
					{
						this.targetCell = test_cell;
						this.targetElement = element;
						return true;
					}
				}
				return false;
			}, Grid.PosToCell(base.gameObject), 5, true, true);
		}

		public void Consume(float dt)
		{
			int index = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(new Action<object>(this.OnMassConsumed), " GasAndLiquidConsumerMonitor")).index;
			SimMessages.ConsumeMass(Grid.PosToCell(this), this.targetElement.id, base.def.consumptionRate * dt, 3, index);
		}

		public void OnMassConsumed(object data)
		{
			if (!base.IsRunning())
			{
				return;
			}
			Sim.MassConsumedCallback massConsumedCallback = (Sim.MassConsumedCallback)data;
			if (massConsumedCallback.mass > 0f)
			{
				this.massUnavailableFrameCount = 0;
				Diet.Info dietInfo = base.def.diet.GetDietInfo(this.targetElement.tag);
				if (dietInfo == null)
				{
					return;
				}
				float num = dietInfo.ConvertConsumptionMassToCalories(massConsumedCallback.mass);
				CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = new CreatureCalorieMonitor.CaloriesConsumedEvent
				{
					tag = this.targetElement.tag,
					calories = num
				};
				base.Trigger(-2038961714, caloriesConsumedEvent);
			}
			else
			{
				this.massUnavailableFrameCount++;
				if (this.massUnavailableFrameCount >= 2)
				{
					base.Trigger(801383139, null);
				}
			}
		}

		public int targetCell = PathProber.InvalidCell;

		private Element targetElement;

		private int massUnavailableFrameCount;
	}
}
