using System;
using Klei.AI;
using UnityEngine;

public class SparkleStreaker : GameStateMachine<SparkleStreaker, SparkleStreaker.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.neutral;
		this.root.EventTransition(GameHashes.Died, null, null);
		this.neutral.TagTransition(GameTags.Overjoyed, this.overjoyed, false);
		this.overjoyed.DefaultState(this.overjoyed.idle).TagTransition(GameTags.Overjoyed, this.neutral, true).ToggleEffect("IsSparkleStreaker")
			.ToggleLoopingSound(this.soundPath, null, true, true, true)
			.Enter(delegate(SparkleStreaker.Instance smi)
			{
				smi.sparkleStreakFX = Util.KInstantiate(EffectPrefabs.Instance.SparkleStreakFX, smi.master.transform.GetPosition() + this.offset);
				smi.sparkleStreakFX.transform.SetParent(smi.master.transform);
				smi.sparkleStreakFX.SetActive(true);
				smi.CreatePasserbyReactable();
			})
			.Exit(delegate(SparkleStreaker.Instance smi)
			{
				Util.KDestroyGameObject(smi.sparkleStreakFX);
				smi.ClearPasserbyReactable();
			});
		this.overjoyed.idle.Enter(delegate(SparkleStreaker.Instance smi)
		{
			smi.SetSparkleSoundParam(0f);
		}).EventTransition(GameHashes.ObjectMovementStateChanged, this.overjoyed.moving, (SparkleStreaker.Instance smi) => smi.IsMoving());
		this.overjoyed.moving.Enter(delegate(SparkleStreaker.Instance smi)
		{
			smi.SetSparkleSoundParam(1f);
		}).EventTransition(GameHashes.ObjectMovementStateChanged, this.overjoyed.idle, (SparkleStreaker.Instance smi) => !smi.IsMoving());
	}

	private Vector3 offset = new Vector3(0f, 0f, 0.1f);

	public GameStateMachine<SparkleStreaker, SparkleStreaker.Instance, IStateMachineTarget, object>.State neutral;

	public SparkleStreaker.OverjoyedStates overjoyed;

	public string soundPath = GlobalAssets.GetSound("SparkleStreaker_lp", false);

	public HashedString SPARKLE_STREAKER_MOVING_PARAMETER = "sparkleStreaker_moving";

	public class OverjoyedStates : GameStateMachine<SparkleStreaker, SparkleStreaker.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<SparkleStreaker, SparkleStreaker.Instance, IStateMachineTarget, object>.State idle;

		public GameStateMachine<SparkleStreaker, SparkleStreaker.Instance, IStateMachineTarget, object>.State moving;
	}

	public new class Instance : GameStateMachine<SparkleStreaker, SparkleStreaker.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void CreatePasserbyReactable()
		{
			if (this.passerbyReactable == null)
			{
				this.passerbyReactable = new EmoteReactable(base.gameObject, "WorkPasserbyAcknowledgement", Db.Get().ChoreTypes.Emote, "anim_cheer_kanim", 5, 5, 0f, 600f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
				{
					anim = "cheer_pre",
					startcb = new Action<GameObject>(this.AddReactionEffect)
				}).AddStep(new EmoteReactable.EmoteStep
				{
					anim = "cheer_loop"
				}).AddStep(new EmoteReactable.EmoteStep
				{
					anim = "cheer_pst"
				})
					.AddThought(Db.Get().Thoughts.Happy)
					.AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsOnFloor));
			}
		}

		private void AddReactionEffect(GameObject reactor)
		{
			reactor.GetComponent<Effects>().Add("SawSparkleStreaker", true);
		}

		private bool ReactorIsOnFloor(GameObject reactor, Navigator.ActiveTransition transition)
		{
			return transition.end == NavType.Floor;
		}

		public void ClearPasserbyReactable()
		{
			if (this.passerbyReactable != null)
			{
				this.passerbyReactable.Cleanup();
				this.passerbyReactable = null;
			}
		}

		public bool IsMoving()
		{
			return base.smi.master.GetComponent<Navigator>().IsMoving();
		}

		public void SetSparkleSoundParam(float val)
		{
			base.GetComponent<LoopingSounds>().SetParameter(GlobalAssets.GetSound("SparkleStreaker_lp", false), "sparkleStreaker_moving", val);
		}

		private Reactable passerbyReactable;

		public GameObject sparkleStreakFX;
	}
}
