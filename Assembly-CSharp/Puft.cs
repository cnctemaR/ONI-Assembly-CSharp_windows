using System;
using Klei;
using UnityEngine;

public class Puft : StateMachineComponent<Puft.StatesInstance>
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
		this.inhaleSound = GlobalAssets.GetSound(this.inhaleSound, false);
	}

	private int GetBreathMoveTarget()
	{
		return this.breathTargetCell;
	}

	private bool HasConsumedEnough()
	{
		return this.storage.MassStored() > 1f;
	}

	private int FindLure()
	{
		int num = -1;
		int num2 = int.MaxValue;
		foreach (CreatureLure creatureLure in Components.Lures)
		{
			if (creatureLure.GetComponent<Operational>().IsOperational)
			{
				if (!(creatureLure.activeBaitSetting != GameTags.SlimeMold))
				{
					int num3 = global::UnityEngine.Random.Range(0, creatureLure.lurePoints.Length);
					if (this.nav.CanReach(Grid.OffsetCell(Grid.PosToCell(creatureLure), creatureLure.lurePoints[num3])))
					{
						int navigationCost = this.nav.GetNavigationCost(Grid.OffsetCell(Grid.PosToCell(creatureLure), creatureLure.lurePoints[num3]));
						if (navigationCost < num2 && navigationCost < this.MAX_LURE_RANGE)
						{
							num2 = navigationCost;
							num = Grid.OffsetCell(Grid.PosToCell(creatureLure), creatureLure.lurePoints[num3]);
						}
					}
				}
			}
		}
		return num;
	}

	private int FindTargetGasCell()
	{
		return GameUtil.FloodFillFind(new Func<int, bool>(this.isTargetElement), Grid.PosToCell(base.gameObject), 8, true, true);
	}

	private bool StandingOnFood()
	{
		int num = Grid.PosToCell(this);
		return this.isTargetElement(num);
	}

	private bool isTargetElement(int cell)
	{
		return ElementLoader.elements[(int)Grid.Cell[cell].elementIdx] == ElementLoader.FindElementByHash(this.consumedElement) && Grid.Cell[cell].mass > this.minimumApproachMass && !Grid.Solid[Grid.CellAbove(cell)] && this.nav.CanReach(cell);
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

	private void UpdateInhaleSound()
	{
		if (this.playingInhaleSound)
		{
			LoopingSounds component = base.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.SetParameter(this.inhaleSound, "consumedMass", this.storage.MassStored() / 1f);
			}
		}
	}

	[NonSerialized]
	public string inhaleSound = "Puft_air_intake";

	[MyCmpReq]
	private ElementEmitter emitter;

	[MyCmpReq]
	private KBatchedAnimController anim;

	[MyCmpReq]
	private Navigator nav;

	[MyCmpReq]
	private Storage storage;

	private int breathTargetCell = -1;

	public SimHashes consumedElement;

	public float consumptionRate;

	public float minimumApproachMass;

	public byte emitDiseaseIdx = byte.MaxValue;

	public int emitDiseasePerKg;

	private int MAX_LURE_RANGE = 50;

	private bool playingInhaleSound;

	public class StatesInstance : GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.GameInstance
	{
		public StatesInstance(Puft smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<Puft.States, Puft.StatesInstance, Puft>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.alive.idle.idle;
			this.root.Enter(delegate(Puft.StatesInstance smi)
			{
				this.mover.Set(smi.master.gameObject, smi);
			});
			this.alive.ToggleStateMachine((Puft.StatesInstance smi) => new ThreatMonitor.Instance(smi.master)).EventTransition(GameHashes.TooColdFatal, this.death, null).EventTransition(GameHashes.TooHotFatal, this.death, null)
				.EventTransition(GameHashes.Drowning, this.alive.distressed.Drowning, null)
				.EventTransition(GameHashes.Drowned, this.death, null)
				.TagTransition(GameTags.Entombed, this.death, false)
				.TagTransition(GameTags.Dead, this.death, false)
				.EventTransition(GameHashes.Died, this.death, null)
				.Enter(delegate(Puft.StatesInstance smi)
				{
					smi.Subscribe(-787691065, new Action<object>(smi.master.OnAttacked));
				});
			this.alive.flee.InitializeStates(this.mover, this.alive.idle.idle);
			this.alive.distressed.Drowning.PlayAnim("harvest", KAnim.PlayMode.Loop).EventTransition(GameHashes.EnteredBreathableArea, this.alive.idle.move, null);
			this.alive.idle.idle.Enter(delegate(Puft.StatesInstance smi)
			{
				smi.Play("idle_loop", KAnim.PlayMode.Loop);
				if (smi.master.StandingOnFood())
				{
					smi.GoTo(this.alive.inhale.pre);
				}
				else
				{
					smi.Schedule(2f, delegate(object d)
					{
						int num = smi.master.FindLure();
						if (num == -1)
						{
							num = smi.master.FindTargetGasCell();
						}
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
			this.alive.idle.move.InitializeStates(this.alive.idle.idle);
			this.alive.moveToBreathable.MoveTo((Puft.StatesInstance smi) => smi.master.GetBreathMoveTarget(), this.alive.idle.idle, this.alive.idle.idle, false);
			this.alive.full.alt.GoTo(this.alive.full.full).PlayAnim("idle_loop_full", KAnim.PlayMode.Loop);
			this.alive.full.full.MoveTo((Puft.StatesInstance smi) => Grid.CellAbove(Grid.PosToCell(smi.master.gameObject)), this.alive.full.alt, this.alive.full.fart, false).PlayAnim("idle_loop_full", KAnim.PlayMode.Loop);
			this.alive.full.fart.PlayAnim("fart").Enter(delegate(Puft.StatesInstance smi)
			{
				smi.Schedule(1f, delegate(object obj)
				{
					smi.master.ConvertFoodToPoop();
				}, null);
			}).OnAnimQueueComplete(this.alive.idle.idle);
			this.alive.inhale.DefaultState(this.alive.inhale.pre).Update(delegate(Puft.StatesInstance smi, float dt)
			{
				smi.master.ConsumeFood(dt);
			});
			this.alive.inhale.pre.PlayAnim("inhale_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.alive.inhale.loop);
			this.alive.inhale.loop.PlayAnim("inhale_loop", KAnim.PlayMode.Loop).Enter(delegate(Puft.StatesInstance smi)
			{
				smi.master.StartInhaleSound();
			}).Update(delegate(Puft.StatesInstance smi, float dt)
			{
				smi.master.UpdateInhaleSound();
			})
				.Exit(delegate(Puft.StatesInstance smi)
				{
					smi.master.StopInhaleSound();
				})
				.ScheduleGoTo(2f, this.alive.inhale.pst)
				.OnSignal(this.noFood, this.alive.inhale.pst);
			this.alive.inhale.pst.Enter(delegate(Puft.StatesInstance smi)
			{
				if (smi.master.HasConsumedEnough())
				{
					smi.GoTo(this.alive.inhale.pst_full);
				}
				else
				{
					smi.GoTo(this.alive.idle.move);
				}
			});
			this.alive.inhale.pst_full.PlayAnim("inhale_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.alive.full.alt);
			this.death.ToggleGravity().PlayAnim("death").EventHandler(GameHashes.AnimQueueComplete, delegate(Puft.StatesInstance smi)
			{
				Util.KDestroyGameObject(smi.gameObject);
			})
				.Enter(delegate(Puft.StatesInstance smi)
				{
					smi.Schedule(2f, delegate(object d)
					{
						Util.KDestroyGameObject(smi.master.gameObject);
					}, null);
				});
		}

		public StateMachine<Puft.States, Puft.StatesInstance, Puft, object>.TargetParameter breathMoveTarget;

		public StateMachine<Puft.States, Puft.StatesInstance, Puft, object>.TargetParameter mover;

		public StateMachine<Puft.States, Puft.StatesInstance, Puft, object>.Signal noFood;

		public Puft.States.AliveStates alive;

		public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State death;

		public class AliveStates : GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State
		{
			public Puft.States.IdleStates idle;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.ApproachSubState<IApproachable> moveToBreathable;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.CreatureFleeSubState<IApproachable> flee;

			public Puft.States.InhaleStates inhale;

			public Puft.States.FullStates full;

			public Puft.States.DistressStates distressed;
		}

		public class FullStates : GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State
		{
			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.ApproachSubState<IApproachable> full;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State alt;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State fart;
		}

		public class IdleStates : GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State
		{
			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State idle;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.IdleMoveSubState move;
		}

		public class InhaleStates : GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State
		{
			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State pre;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State loop;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State pst;

			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State pst_full;
		}

		public class DistressStates : GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State
		{
			public GameStateMachine<Puft.States, Puft.StatesInstance, Puft, object>.State Drowning;
		}
	}
}
