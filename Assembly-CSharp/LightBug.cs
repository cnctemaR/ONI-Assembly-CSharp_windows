using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LightBug : StateMachineComponent<LightBug.StatesInstance>, ISaveLoadable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Vector3 position = base.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
		base.transform.SetPosition(position);
		base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	private int FindLure()
	{
		int num = -1;
		int num2 = int.MaxValue;
		Navigator component = base.GetComponent<Navigator>();
		foreach (CreatureLure creatureLure in Components.Lures)
		{
			if (creatureLure.GetComponent<Operational>().IsOperational)
			{
				if (!(creatureLure.activeBaitSetting != GameTags.Phosphorite))
				{
					int num3 = global::UnityEngine.Random.Range(0, creatureLure.lurePoints.Length);
					if (component.CanReach(Grid.OffsetCell(Grid.PosToCell(creatureLure), creatureLure.lurePoints[num3])))
					{
						int navigationCost = component.GetNavigationCost(Grid.OffsetCell(Grid.PosToCell(creatureLure), creatureLure.lurePoints[num3]));
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

	[MyCmpAdd]
	private KBatchedAnimController anim;

	[MyCmpGet]
	public LoopingSounds loopingSounds;

	[NonSerialized]
	public string wingSound = "ShineBug_wings_LP";

	private int MAX_LURE_RANGE = 50;

	public class StatesInstance : GameStateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.GameInstance
	{
		public StatesInstance(LightBug smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<LightBug.States, LightBug.StatesInstance, LightBug>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.alive.idleStates.idle;
			this.root.ToggleSchedulePeriodic("RefreshLightbugLight", 0.5f, delegate(LightBug.StatesInstance smi)
			{
				smi.master.GetComponent<Light2D>().Refresh();
			});
			this.alive.EventTransition(GameHashes.TooColdFatal, this.death, null).EventTransition(GameHashes.TooHotFatal, this.death, null).EventTransition(GameHashes.Drowned, this.death, null)
				.EventTransition(GameHashes.Drowning, this.alive.distressed.Drowning, null)
				.TagTransition(GameTags.Entombed, this.death, false)
				.TagTransition(GameTags.Dead, this.death, false)
				.EventTransition(GameHashes.DebugGoTo, (LightBug.StatesInstance smi) => Game.Instance, this.alive.idleStates.debug_go_to, null)
				.ToggleStateMachine((LightBug.StatesInstance smi) => new ThreatMonitor.Instance(smi.master))
				.Enter(delegate(LightBug.StatesInstance smi)
				{
					this.mover.Set(smi.master, smi);
					if (smi.master.loopingSounds != null)
					{
						smi.master.loopingSounds.AddLoopingSoundUpdater();
						smi.master.loopingSounds.StartSound(GlobalAssets.GetSound(smi.master.wingSound, false), smi.master.transform.position);
					}
				})
				.Exit(delegate(LightBug.StatesInstance smi)
				{
					smi.master.loopingSounds.StopSound(GlobalAssets.GetSound(smi.master.wingSound, false));
					smi.master.loopingSounds.RemoveLoopingSoundUpdater();
				});
			this.alive.distressed.Drowning.PlayAnim("hit", KAnim.PlayMode.Loop).EventTransition(GameHashes.EnteredBreathableArea, this.alive.idleStates.move, null);
			this.alive.idleStates.idle.Enter(delegate(LightBug.StatesInstance smi)
			{
				smi.Play("idle", KAnim.PlayMode.Once);
			}).EventHandler(GameHashes.AnimQueueComplete, delegate(LightBug.StatesInstance smi)
			{
				smi.GoTo(this.alive.idleStates.lured);
			}).EventTransition(GameHashes.TooColdFatal, this.death, null)
				.EventTransition(GameHashes.TooHotFatal, this.death, null);
			this.alive.idleStates.lured.Enter(delegate(LightBug.StatesInstance smi)
			{
				int num = smi.master.FindLure();
				if (num == -1)
				{
					smi.GoTo(this.alive.idleStates.move);
				}
				else
				{
					smi.GetComponent<Navigator>().GoTo(num, Grid.DefaultOffset);
				}
			}).EventTransition(GameHashes.TooColdFatal, this.death, null).EventTransition(GameHashes.TooHotFatal, this.death, null)
				.EventTransition(GameHashes.NavigationFailed, this.alive.idleStates.idle, null)
				.EventTransition(GameHashes.DestinationReached, this.alive.idleStates.idle, null);
			this.alive.idleStates.move.InitializeStates(this.alive.idleStates.idle);
			this.alive.idleStates.debug_go_to.InitializeStates(this.alive.idleStates.idle);
			this.death.ToggleGravity().PlayAnim("death").EventHandler(GameHashes.AnimQueueComplete, delegate(LightBug.StatesInstance smi)
			{
				Util.KDestroyGameObject(smi.gameObject);
			})
				.Enter(delegate(LightBug.StatesInstance smi)
				{
					Butcherable component = smi.master.GetComponent<Butcherable>();
					if (component)
					{
						component.OnButcherComplete();
					}
				});
		}

		public StateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.TargetParameter threatMoveTarget;

		public StateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.TargetParameter mover;

		public StateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.ObjectParameter<ThreatMonitor> threatMonitor;

		public LightBug.States.AliveStates alive;

		public GameStateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.State death;

		public class AliveStates : GameStateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.State
		{
			public LightBug.States.IdleStates idleStates;

			public LightBug.States.DistressStates distressed;
		}

		public class IdleStates : GameStateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.State
		{
			public GameStateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.State idle;

			public GameStateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.IdleMoveSubState move;

			public GameStateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.State lured;

			public GameStateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.DebugGoToSubState debug_go_to;
		}

		public class DistressStates : GameStateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.State
		{
			public GameStateMachine<LightBug.States, LightBug.StatesInstance, LightBug, object>.State Drowning;
		}
	}
}
