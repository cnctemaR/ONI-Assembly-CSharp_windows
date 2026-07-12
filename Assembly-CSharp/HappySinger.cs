using System;
using Klei.AI;
using UnityEngine;

public class HappySinger : GameStateMachine<HappySinger, HappySinger.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.neutral;
		this.root.TagTransition(GameTags.Dead, null, false);
		this.neutral.TagTransition(GameTags.Overjoyed, this.overjoyed, false);
		this.overjoyed.DefaultState(this.overjoyed.idle).TagTransition(GameTags.Overjoyed, this.neutral, true).ToggleEffect("IsJoySinger")
			.ToggleLoopingSound(this.soundPath, null, true, true, true)
			.ToggleAnims("anim_loco_singer_kanim", 0f)
			.ToggleAnims("anim_idle_singer_kanim", 0f)
			.EventHandler(GameHashes.TagsChanged, delegate(HappySinger.Instance smi, object obj)
			{
				smi.musicParticleFX.SetActive(!smi.HasTag(GameTags.Asleep));
			})
			.Enter(delegate(HappySinger.Instance smi)
			{
				smi.musicParticleFX = Util.KInstantiate(EffectPrefabs.Instance.HappySingerFX, smi.master.transform.GetPosition() + this.offset);
				smi.musicParticleFX.transform.SetParent(smi.master.transform);
				smi.CreatePasserbyReactable();
				smi.musicParticleFX.SetActive(!smi.HasTag(GameTags.Asleep));
			})
			.Update(delegate(HappySinger.Instance smi, float dt)
			{
				if (!smi.GetSpeechMonitor().IsPlayingSpeech() && SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject))
				{
					smi.GetSpeechMonitor().PlaySpeech(Db.Get().Thoughts.CatchyTune.speechPrefix, Db.Get().Thoughts.CatchyTune.sound);
				}
			}, UpdateRate.SIM_1000ms, false)
			.Exit(delegate(HappySinger.Instance smi)
			{
				Util.KDestroyGameObject(smi.musicParticleFX);
				smi.ClearPasserbyReactable();
				smi.musicParticleFX.SetActive(false);
			});
	}

	private Vector3 offset = new Vector3(0f, 0f, 0.1f);

	public GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.State neutral;

	public HappySinger.OverjoyedStates overjoyed;

	public string soundPath = GlobalAssets.GetSound("DupeSinging_NotesFX_LP", false);

	public class OverjoyedStates : GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.State idle;

		public GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.State moving;
	}

	public new class Instance : GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void CreatePasserbyReactable()
		{
			if (this.passerbyReactable == null)
			{
				EmoteReactable emoteReactable = new EmoteReactable(base.gameObject, "WorkPasserbyAcknowledgement", Db.Get().ChoreTypes.Emote, 5, 5, 0f, 600f, float.PositiveInfinity, 0f);
				Emote sing = Db.Get().Emotes.Minion.Sing;
				emoteReactable.SetEmote(sing).SetThought(Db.Get().Thoughts.CatchyTune).AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsOnFloor));
				emoteReactable.RegisterEmoteStepCallbacks("react", new Action<GameObject>(this.AddReactionEffect), null);
				this.passerbyReactable = emoteReactable;
			}
		}

		public SpeechMonitor.Instance GetSpeechMonitor()
		{
			if (this.speechMonitor == null)
			{
				this.speechMonitor = base.master.gameObject.GetSMI<SpeechMonitor.Instance>();
			}
			return this.speechMonitor;
		}

		private void AddReactionEffect(GameObject reactor)
		{
			reactor.Trigger(-1278274506, null);
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

		private Reactable passerbyReactable;

		public GameObject musicParticleFX;

		public SpeechMonitor.Instance speechMonitor;
	}
}
