using System;
using Klei;
using KSerialization;
using UnityEngine;

public class OilFloater : StateMachineComponent<OilFloater.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Vector3 position = base.transform.GetPosition();
		base.transform.SetPosition(position);
		base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		this.moveSound = GlobalAssets.GetSound(this.moveSound, false);
		this.inhaleSound = GlobalAssets.GetSound(this.inhaleSound, false);
	}

	private int GetBreathMoveTarget()
	{
		return this.breathTargetCell;
	}

	private int GetLastPoopCell()
	{
		if (this.lastPoopCell == -1)
		{
			this.SetLastPoopCell();
		}
		return this.lastPoopCell;
	}

	private void SetLastPoopCell()
	{
		this.lastPoopCell = Grid.PosToCell(this);
	}

	private bool StandingOnFood()
	{
		int num = Grid.PosToCell(this);
		return this.isTargetElement(num);
	}

	private bool HasConsumedEnough()
	{
		return this.storage.MassStored() > this.consumptionRate * 5f;
	}

	private int FindTargetGasCell()
	{
		return GameUtil.FloodFillFind(new Func<int, bool>(this.isTargetElement), Grid.PosToCell(base.gameObject), 8, true, true);
	}

	private bool isTargetElement(int cell)
	{
		return Grid.Element[cell].id == this.consumedElement && Grid.Cell[cell].mass > this.minimumApproachMass && this.nav.CanReach(cell);
	}

	private int FindAbovewaterCell()
	{
		int num = Grid.PosToCell(base.gameObject);
		int num2 = GameUtil.FloodFillFind(new Func<int, bool>(this.isAboveWater), num, 8, true, false);
		if (num2 == -1)
		{
			CellOffset[] array = new CellOffset[]
			{
				new CellOffset(0, 0),
				new CellOffset(-1, 0),
				new CellOffset(1, 0),
				new CellOffset(-1, -1),
				new CellOffset(1, -1)
			};
			num2 = Grid.OffsetCell(num, array[global::UnityEngine.Random.Range(0, array.Length)]);
			int num3 = Grid.CellAbove(num2);
			while (Grid.IsSubstantialLiquid(num3, 0.35f))
			{
				num2 = num3;
				num3 = Grid.CellAbove(num2);
			}
		}
		return num2;
	}

	private bool isAboveWater(int cell)
	{
		return !Grid.IsSubstantialLiquid(cell, 0.35f);
	}

	private void CheckForUnderwater()
	{
		int num = Grid.PosToCell(this);
		if (!this.isAboveWater(num))
		{
			base.smi.GoTo(base.smi.sm.underwater);
		}
	}

	private void CheckForAbovewater()
	{
		int num = Grid.PosToCell(this);
		if (this.isAboveWater(num))
		{
			base.smi.GoTo(base.smi.sm.alive.idle);
		}
	}

	public void OnAttacked(object data)
	{
		if (!base.smi.IsInsideState(base.smi.sm.alive.flee))
		{
			base.smi.GoTo(base.smi.sm.alive.flee);
		}
	}

	private void ConsumeFood(float dt)
	{
		int index = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(new Action<object>(this.OnMassConsumed))).index;
		SimMessages.ConsumeMass(Grid.PosToCell(this), this.consumedElement, this.consumptionRate * dt, 3, index);
	}

	private void OnMassConsumed(object data)
	{
		if (this == null)
		{
			return;
		}
		Sim.MassConsumedCallback massConsumedCallback = (Sim.MassConsumedCallback)data;
		if (massConsumedCallback.mass > 0f)
		{
			this.storage.AddGasChunk(ElementLoader.elements[(int)massConsumedCallback.elemIdx].id, massConsumedCallback.mass, massConsumedCallback.temperature, massConsumedCallback.diseaseIdx, massConsumedCallback.diseaseCount, true, true);
			if (this.HasConsumedEnough())
			{
				base.smi.sm.noFood.Trigger(base.smi);
			}
		}
		else
		{
			base.smi.sm.noFood.Trigger(base.smi);
		}
	}

	private void ConvertFoodToPoop()
	{
		float massAvailable = this.storage.GetMassAvailable(this.consumedElement);
		SimUtil.DiseaseInfo diseaseInfo;
		float num;
		this.storage.ConsumeAndGetDisease(this.consumedElement.CreateTag(), massAvailable, out diseaseInfo, out num);
		diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo, new SimUtil.DiseaseInfo
		{
			idx = this.emitDiseaseIdx,
			count = Mathf.RoundToInt((float)this.emitDiseasePerKg * massAvailable)
		});
		base.smi.master.emitter.ForceEmit(massAvailable, diseaseInfo.idx, diseaseInfo.count, num);
	}

	private void StartMoveSound()
	{
		if (!this.playingMoveSound)
		{
			LoopingSounds component = base.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.AddLoopingSoundUpdater();
				component.StartSound(this.moveSound, base.transform.GetPosition());
				this.playingMoveSound = true;
			}
		}
	}

	private void StopMoveSound()
	{
		if (this.playingMoveSound)
		{
			LoopingSounds component = base.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.StopSound(this.moveSound);
				component.RemoveLoopingSoundUpdater();
				this.playingMoveSound = false;
			}
		}
	}

	private void StartInhaleSound()
	{
		if (!this.playingInhaleSound)
		{
			LoopingSounds component = base.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.AddLoopingSoundUpdater();
				component.StartSound(this.inhaleSound, base.transform.GetPosition());
				this.playingInhaleSound = true;
			}
		}
	}

	private void UpdateInhaleSound()
	{
		if (this.playingInhaleSound)
		{
			LoopingSounds component = base.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.SetParameter(this.inhaleSound, "consumedMass", this.storage.MassStored() / (this.consumptionRate * 5f));
			}
		}
	}

	private void StopInhaleSound()
	{
		if (this.playingInhaleSound)
		{
			LoopingSounds component = base.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.StopSound(this.inhaleSound);
				component.RemoveLoopingSoundUpdater();
				this.playingInhaleSound = false;
			}
		}
	}

	[NonSerialized]
	public string moveSound = "OilFloater_move_LP";

	[NonSerialized]
	public string inhaleSound = "OilFloater_intake_air";

	public const float EATS_BEFORE_POOP = 5f;

	[MyCmpReq]
	private ElementEmitter emitter;

	[MyCmpReq]
	private KBatchedAnimController anim;

	[MyCmpReq]
	private Navigator nav;

	[MyCmpReq]
	private Storage storage;

	private int breathTargetCell = -1;

	[Serialize]
	private int lastPoopCell = -1;

	public SimHashes consumedElement;

	public float consumptionRate;

	public float minimumApproachMass;

	public byte emitDiseaseIdx = byte.MaxValue;

	public int emitDiseasePerKg;

	private bool playingMoveSound;

	private bool playingInhaleSound;

	public class StatesInstance : GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.GameInstance
	{
		public StatesInstance(OilFloater master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.alive.idle;
			this.root.Enter(delegate(OilFloater.StatesInstance smi)
			{
				this.mover.Set(smi.master.gameObject, smi);
			});
			this.alive.ToggleStateMachine((OilFloater.StatesInstance smi) => new ThreatMonitor.Instance(smi.master)).EventTransition(GameHashes.TooColdFatal, this.death, null).EventTransition(GameHashes.TooHotFatal, this.death, null)
				.TagTransition(GameTags.Entombed, this.death, false)
				.TagTransition(GameTags.Dead, this.death, false)
				.TagTransition(GameTags.Trapped, this.trapped, false)
				.EventTransition(GameHashes.Died, this.death, null)
				.Enter(delegate(OilFloater.StatesInstance smi)
				{
					smi.Subscribe(-787691065, new Action<object>(smi.master.OnAttacked));
					Navigator component = smi.GetComponent<Navigator>();
					component.SetCurrentNavType(NavType.Hover);
					smi.master.CheckForUnderwater();
				})
				.Update("floater drown check", delegate(OilFloater.StatesInstance smi, float dt)
				{
					smi.master.CheckForUnderwater();
				}, UpdateRate.SIM_1000ms, false);
			this.alive.flee.InitializeStates(this.mover, this.alive.idle);
			this.alive.idle.DefaultState(this.alive.idle.idle);
			this.alive.idle.idle.PlayAnim("idle_loop", KAnim.PlayMode.Loop).Enter(delegate(OilFloater.StatesInstance smi)
			{
				if (smi.master.HasConsumedEnough())
				{
					smi.GoTo(this.alive.full);
				}
				else
				{
					smi.Schedule(2f, delegate(object data)
					{
						int num = smi.master.FindTargetGasCell();
						if (num != -1)
						{
							smi.master.breathTargetCell = num;
							smi.GoTo(this.alive.moveToBreathable);
						}
						else
						{
							smi.ScheduleGoTo(2f, this.alive.idle.move);
						}
					}, null);
				}
			});
			this.alive.idle.move.InitializeStates(this.alive.idle.post_move).Enter(delegate(OilFloater.StatesInstance smi)
			{
				smi.master.StartMoveSound();
			}).Exit(delegate(OilFloater.StatesInstance smi)
			{
				smi.master.StopMoveSound();
			});
			this.alive.idle.post_move.PlayAnim("idle_loop", KAnim.PlayMode.Loop).Enter(delegate(OilFloater.StatesInstance smi)
			{
				if (smi.master.StandingOnFood())
				{
					smi.GoTo(this.alive.inhale);
				}
				else
				{
					smi.GoTo(this.alive.idle);
				}
			});
			this.alive.moveToBreathable.MoveTo((OilFloater.StatesInstance smi) => smi.master.GetBreathMoveTarget(), this.alive.idle.post_move, this.alive.idle.post_move, false).Enter(delegate(OilFloater.StatesInstance smi)
			{
				smi.master.StartMoveSound();
			}).Exit(delegate(OilFloater.StatesInstance smi)
			{
				smi.master.StopMoveSound();
			});
			this.alive.full.DefaultState(this.alive.full.full);
			this.alive.full.full.PlayAnim("idle_loop_full", KAnim.PlayMode.Loop).MoveTo((OilFloater.StatesInstance smi) => smi.master.GetLastPoopCell(), this.alive.full.poop, this.alive.full.poop, false).Enter(delegate(OilFloater.StatesInstance smi)
			{
				smi.master.StartMoveSound();
			})
				.Exit(delegate(OilFloater.StatesInstance smi)
				{
					smi.master.StopMoveSound();
				});
			this.alive.full.poop.PlayAnim("poop").Enter(delegate(OilFloater.StatesInstance smi)
			{
				smi.Schedule(1f, delegate(object obj)
				{
					smi.master.ConvertFoodToPoop();
					smi.master.SetLastPoopCell();
				}, null);
			}).OnAnimQueueComplete(this.alive.idle);
			this.alive.inhale.DefaultState(this.alive.inhale.pre).Update(delegate(OilFloater.StatesInstance smi, float dt)
			{
				smi.master.ConsumeFood(dt);
			}).Enter(delegate(OilFloater.StatesInstance smi)
			{
				smi.master.StartInhaleSound();
			})
				.Update(delegate(OilFloater.StatesInstance smi, float dt)
				{
					smi.master.UpdateInhaleSound();
				});
			this.alive.inhale.pre.PlayAnim("eat_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.alive.inhale.loop);
			this.alive.inhale.loop.PlayAnim("eat_loop", KAnim.PlayMode.Loop).ScheduleGoTo(5f, this.alive.inhale.pst).OnSignal(this.noFood, this.alive.inhale.pst)
				.Exit(delegate(OilFloater.StatesInstance smi)
				{
					smi.master.StopInhaleSound();
				});
			this.alive.inhale.pst.PlayAnim("eat_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.alive.idle.move);
			this.underwater.DefaultState(this.underwater.move).Enter(delegate(OilFloater.StatesInstance smi)
			{
				Navigator component2 = smi.GetComponent<Navigator>();
				component2.SetCurrentNavType(NavType.Swim);
				smi.master.CheckForAbovewater();
			}).Update("floater surface check", delegate(OilFloater.StatesInstance smi, float dt)
			{
				smi.master.CheckForAbovewater();
			}, UpdateRate.SIM_1000ms, false);
			this.underwater.idle.PlayAnim("swim_idle_loop", KAnim.PlayMode.Loop).ScheduleGoTo(2f, this.underwater.move);
			this.underwater.move.MoveTo((OilFloater.StatesInstance smi) => smi.master.FindAbovewaterCell(), this.alive.idle, this.underwater.idle, false);
			this.trapped.InitializeStates(this.masterTarget, this.alive.idle);
			this.death.ToggleGravity().PlayAnim("death").EventHandler(GameHashes.AnimQueueComplete, delegate(OilFloater.StatesInstance smi)
			{
				Util.KDestroyGameObject(smi.gameObject);
			})
				.Enter(delegate(OilFloater.StatesInstance smi)
				{
					smi.Schedule(2f, delegate(object d)
					{
						Util.KDestroyGameObject(smi.master.gameObject);
					}, null);
				});
		}

		public StateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.TargetParameter breathMoveTarget;

		public StateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.TargetParameter mover;

		public StateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.Signal noFood;

		public OilFloater.States.AliveStates alive;

		public OilFloater.States.UnderwaterStates underwater;

		public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.TrappedSubState trapped;

		public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State death;

		public class AliveStates : GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State
		{
			public OilFloater.States.IdleStates idle;

			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.ApproachSubState<IApproachable> moveToBreathable;

			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.CreatureFleeSubState<IApproachable> flee;

			public OilFloater.States.InhaleStates inhale;

			public OilFloater.States.FullStates full;
		}

		public class FullStates : GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State
		{
			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.ApproachSubState<IApproachable> full;

			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State poop;
		}

		public class IdleStates : GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State
		{
			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State idle;

			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.IdleMoveSubState move;

			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State post_move;
		}

		public class UnderwaterStates : GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State
		{
			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State idle;

			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.IdleMoveSubState move;

			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State post_move;
		}

		public class InhaleStates : GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State
		{
			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State pre;

			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State loop;

			public GameStateMachine<OilFloater.States, OilFloater.StatesInstance, OilFloater, object>.State pst;
		}
	}
}
