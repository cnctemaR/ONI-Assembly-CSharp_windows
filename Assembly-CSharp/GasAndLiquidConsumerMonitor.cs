using System;
using UnityEngine;

public class GasAndLiquidConsumerMonitor : GameStateMachine<GasAndLiquidConsumerMonitor, GasAndLiquidConsumerMonitor.Instance, IStateMachineTarget, GasAndLiquidConsumerMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.cooldown;
		this.cooldown.Enter("ClearTargetCell", delegate(GasAndLiquidConsumerMonitor.Instance smi)
		{
			smi.ClearTargetCell();
		}).ScheduleGoTo((GasAndLiquidConsumerMonitor.Instance smi) => global::UnityEngine.Random.Range(smi.def.minCooldown, smi.def.maxCooldown), this.satisfied);
		this.satisfied.Enter("ClearTargetCell", delegate(GasAndLiquidConsumerMonitor.Instance smi)
		{
			smi.ClearTargetCell();
		}).TagTransition((GasAndLiquidConsumerMonitor.Instance smi) => smi.def.transitionTag, this.looking, false);
		this.looking.ToggleBehaviour((GasAndLiquidConsumerMonitor.Instance smi) => smi.def.behaviourTag, (GasAndLiquidConsumerMonitor.Instance smi) => smi.targetCell != -1, delegate(GasAndLiquidConsumerMonitor.Instance smi)
		{
			smi.GoTo(this.cooldown);
		}).TagTransition((GasAndLiquidConsumerMonitor.Instance smi) => smi.def.transitionTag, this.satisfied, true).PreBrainUpdate(delegate(GasAndLiquidConsumerMonitor.Instance smi)
		{
			smi.FindElement();
		});
	}

	private GameStateMachine<GasAndLiquidConsumerMonitor, GasAndLiquidConsumerMonitor.Instance, IStateMachineTarget, GasAndLiquidConsumerMonitor.Def>.State cooldown;

	private GameStateMachine<GasAndLiquidConsumerMonitor, GasAndLiquidConsumerMonitor.Instance, IStateMachineTarget, GasAndLiquidConsumerMonitor.Def>.State satisfied;

	private GameStateMachine<GasAndLiquidConsumerMonitor, GasAndLiquidConsumerMonitor.Instance, IStateMachineTarget, GasAndLiquidConsumerMonitor.Def>.State looking;

	public class Def : StateMachine.BaseDef
	{
		public Tag[] transitionTag = new Tag[] { GameTags.Creatures.Hungry };

		public Tag behaviourTag = GameTags.Creatures.WantsToEat;

		public float minCooldown = 5f;

		public float maxCooldown = 5f;

		public Diet diet;

		public float consumptionRate = 0.5f;

		public Tag consumableElementTag = Tag.Invalid;
	}

	public new class Instance : GameStateMachine<GasAndLiquidConsumerMonitor, GasAndLiquidConsumerMonitor.Instance, IStateMachineTarget, GasAndLiquidConsumerMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, GasAndLiquidConsumerMonitor.Def def)
			: base(master, def)
		{
			this.navigator = base.smi.GetComponent<Navigator>();
			DebugUtil.Assert(base.smi.def.diet != null || this.storage != null, "GasAndLiquidConsumerMonitor needs either a diet or a storage");
		}

		public void ClearTargetCell()
		{
			this.targetCell = -1;
			this.massUnavailableFrameCount = 0;
		}

		public void FindElement()
		{
			this.targetCell = -1;
			this.FindTargetCell();
		}

		public Element GetTargetElement()
		{
			return this.targetElement;
		}

		public bool IsConsumableCell(int cell, out Element element)
		{
			element = Grid.Element[cell];
			bool flag = true;
			bool flag2 = true;
			if (base.smi.def.consumableElementTag != Tag.Invalid)
			{
				flag = element.HasTag(base.smi.def.consumableElementTag);
			}
			if (base.smi.def.diet != null)
			{
				flag2 = false;
				Diet.Info[] infos = base.smi.def.diet.infos;
				for (int i = 0; i < infos.Length; i++)
				{
					if (infos[i].IsMatch(element.tag))
					{
						flag2 = true;
						break;
					}
				}
			}
			return flag && flag2;
		}

		public void FindTargetCell()
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
			int index = Game.Instance.massConsumedCallbackManager.Add(GasAndLiquidConsumerMonitor.Instance.OnMassConsumedAction, this, "GasAndLiquidConsumerMonitor").index;
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
			if (mcd.mass > 0f)
			{
				if (base.def.diet != null)
				{
					this.massUnavailableFrameCount = 0;
					Diet.Info dietInfo = base.def.diet.GetDietInfo(this.targetElement.tag);
					if (dietInfo == null)
					{
						return;
					}
					float num = dietInfo.ConvertConsumptionMassToCalories(mcd.mass);
					base.BoxingTrigger<CreatureCalorieMonitor.CaloriesConsumedEvent>(-2038961714, new CreatureCalorieMonitor.CaloriesConsumedEvent
					{
						tag = this.targetElement.tag,
						calories = num
					});
					return;
				}
				else if (this.storage != null)
				{
					this.storage.AddElement(this.targetElement.id, mcd.mass, mcd.temperature, mcd.diseaseIdx, mcd.diseaseCount, false, true);
					return;
				}
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

		public int targetCell = -1;

		private Element targetElement;

		private Navigator navigator;

		private int massUnavailableFrameCount;

		[MyCmpGet]
		private Storage storage;

		private static Action<Sim.MassConsumedCallback, object> OnMassConsumedAction = new Action<Sim.MassConsumedCallback, object>(GasAndLiquidConsumerMonitor.Instance.OnMassConsumedCallback);
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
