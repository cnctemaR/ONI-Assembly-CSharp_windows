using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class SpeechMonitor : GameStateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.root.Enter(new StateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State.Callback(SpeechMonitor.CreateMouth)).Exit(new StateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State.Callback(SpeechMonitor.DestroyMouth));
		this.satisfied.DoNothing();
		this.talking.Enter(delegate(SpeechMonitor.Instance smi)
		{
			SpeechMonitor.StartAudio(smi);
			smi.mouth.Play(SpeechMonitor.GetRandomSpeechAnim(smi), KAnim.PlayMode.Once, 1f, 0f);
			if (smi.Kpid.HasTag(GameTags.DoNotInterruptMe))
			{
				smi.GoTo(this.talking.animGoverned);
				return;
			}
			if (smi.ev.isValid())
			{
				smi.GoTo(this.talking.audioGoverned);
				return;
			}
			smi.GoTo(this.talking.fallback);
		}).Exit(delegate(SpeechMonitor.Instance smi)
		{
			smi.SymbolOverrideController.RemoveSymbolOverride(SpeechMonitor.HASH_SNAPTO_MOUTH, 3);
		});
		this.talking.audioGoverned.Transition(this.satisfied, new StateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.Transition.ConditionCallback(SpeechMonitor.IsAudioStopped), UpdateRate.SIM_200ms).Update(new Action<SpeechMonitor.Instance, float>(SpeechMonitor.LipFlap), UpdateRate.RENDER_EVERY_TICK, false);
		this.talking.animGoverned.TagTransition(GameTags.DoNotInterruptMe, this.satisfied, true).Update(new Action<SpeechMonitor.Instance, float>(SpeechMonitor.LipFlap), UpdateRate.RENDER_EVERY_TICK, false).Update(delegate(SpeechMonitor.Instance smi, float dt)
		{
			if (SpeechMonitor.IsAudioStopped(smi))
			{
				SpeechMonitor.StartAudio(smi);
			}
		}, UpdateRate.RENDER_EVERY_TICK, false);
		this.talking.fallback.Enter(delegate(SpeechMonitor.Instance smi)
		{
			smi.mouth.Queue(SpeechMonitor.GetRandomSpeechAnim(smi), KAnim.PlayMode.Once, 1f, 0f);
		}).Target(this.mouth).OnAnimQueueComplete(this.satisfied);
	}

	private static void CreateMouth(SpeechMonitor.Instance smi)
	{
		smi.mouth = global::Util.KInstantiate(Assets.GetPrefab(MouthAnimation.ID), null, null).GetComponent<KBatchedAnimController>();
		smi.mouth.gameObject.SetActive(true);
		smi.sm.mouth.Set(smi.mouth.gameObject, smi, false);
		smi.SetMouthId();
	}

	private static void DestroyMouth(SpeechMonitor.Instance smi)
	{
		if (smi.mouth != null)
		{
			global::Util.KDestroyGameObject(smi.mouth);
			smi.mouth = null;
		}
	}

	private static string GetRandomSpeechAnim(SpeechMonitor.Instance smi)
	{
		return smi.speechPrefix + global::UnityEngine.Random.Range(1, TuningData<SpeechMonitor.Tuning>.Get().speechCount).ToString() + smi.mouthId;
	}

	public static bool IsAllowedToPlaySpeech(KPrefabID prefabID, KBatchedAnimController controller)
	{
		if (prefabID.HasTag(GameTags.Dead))
		{
			return false;
		}
		if (prefabID.HasTag(GameTags.Incapacitated))
		{
			return false;
		}
		KAnim.Anim currentAnim = controller.GetCurrentAnim();
		return currentAnim == null || (GameAudioSheets.Get().IsAnimAllowedToPlaySpeech(currentAnim) && SpeechMonitor.CanOverrideHead(controller));
	}

	private static bool CanOverrideHead(KBatchedAnimController kbac)
	{
		bool flag = true;
		KAnim.Anim currentAnim = kbac.GetCurrentAnim();
		if (currentAnim == null)
		{
			flag = false;
		}
		else if (currentAnim.animFile.name != SpeechMonitor.GENERIC_CONVO_ANIM_NAME)
		{
			int currentFrameIndex = kbac.GetCurrentFrameIndex();
			KAnim.Anim.Frame frame;
			if (currentFrameIndex <= 0)
			{
				flag = false;
			}
			else if (KAnimBatchManager.Instance().GetBatchGroupData(currentAnim.animFile.animBatchTag).TryGetFrame(currentFrameIndex, out frame) && frame.hasHead)
			{
				flag = false;
			}
		}
		return flag;
	}

	private static KAnim.Anim.FrameElement GetFirstFrameElement(KBatchedAnimController controller)
	{
		int currentFrameIndex = controller.GetCurrentFrameIndex();
		if (currentFrameIndex == -1)
		{
			return SpeechMonitor.INVALID_FRAME_ELEMENT;
		}
		KAnimBatch batch = controller.GetBatch();
		if (batch == null)
		{
			return SpeechMonitor.INVALID_FRAME_ELEMENT;
		}
		KAnim.Anim.Frame frame;
		if (!batch.group.data.TryGetFrame(currentFrameIndex, out frame))
		{
			return SpeechMonitor.INVALID_FRAME_ELEMENT;
		}
		List<KAnim.Anim.FrameElement> frameElements = batch.group.data.frameElements;
		for (int i = 0; i < frame.numElements; i++)
		{
			int num = frame.firstElementIdx + i;
			bool flag = num < frameElements.Count;
			DebugUtil.DevAssert(flag, "Frame element index out of range", null);
			if (flag)
			{
				KAnim.Anim.FrameElement frameElement = frameElements[num];
				if (!(frameElement.symbol == HashedString.Invalid))
				{
					return frameElement;
				}
			}
		}
		return SpeechMonitor.INVALID_FRAME_ELEMENT;
	}

	private static void StartAudio(SpeechMonitor.Instance smi)
	{
		smi.ev.clearHandle();
		if (smi.voiceEvent != null)
		{
			smi.ev = VoiceSoundEvent.PlayVoice(smi.voiceEvent, smi.AnimController, 0f, false, false);
		}
	}

	private static bool IsAudioStopped(SpeechMonitor.Instance smi)
	{
		if (!smi.ev.isValid())
		{
			return true;
		}
		PLAYBACK_STATE playback_STATE;
		smi.ev.getPlaybackState(out playback_STATE);
		if (playback_STATE == PLAYBACK_STATE.STOPPING || playback_STATE == PLAYBACK_STATE.STOPPED)
		{
			smi.ev.clearHandle();
			return true;
		}
		return false;
	}

	private static void LipFlap(SpeechMonitor.Instance smi, float dt)
	{
		if (smi.mouth.IsStopped())
		{
			smi.mouth.Play(SpeechMonitor.GetRandomSpeechAnim(smi), KAnim.PlayMode.Once, 1f, 0f);
			DebugUtil.DevAssert(!smi.mouth.IsStopped(), "Mouth animation should be playing", null);
		}
	}

	public GameStateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State satisfied;

	public SpeechMonitor.Playing talking;

	public static string PREFIX_SAD = "sad";

	public static string PREFIX_HAPPY = "happy";

	public static string PREFIX_SINGER = "sing";

	public StateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.TargetParameter mouth;

	private static HashedString HASH_SNAPTO_MOUTH = "snapto_mouth";

	private static HashedString GENERIC_CONVO_ANIM_NAME = new HashedString("anim_generic_convo_kanim");

	private static KAnim.Anim.FrameElement INVALID_FRAME_ELEMENT = new KAnim.Anim.FrameElement
	{
		symbol = HashedString.Invalid
	};

	public class Playing : GameStateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State
	{
		public GameStateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State audioGoverned;

		public GameStateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State animGoverned;

		public GameStateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State fallback;
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public class Tuning : TuningData<SpeechMonitor.Tuning>
	{
		public float randomSpeechIntervalMin;

		public float randomSpeechIntervalMax;

		public int speechCount;
	}

	public new class Instance : GameStateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.GameInstance
	{
		public KBatchedAnimController AnimController { get; private set; }

		public SymbolOverrideController SymbolOverrideController { get; private set; }

		public MinionIdentity MinionIdentity { get; private set; }

		public KPrefabID Kpid { get; private set; }

		public Instance(IStateMachineTarget master, SpeechMonitor.Def def)
			: base(master, def)
		{
			this.AnimController = master.GetComponent<KBatchedAnimController>();
			this.SymbolOverrideController = master.GetComponent<SymbolOverrideController>();
			this.MinionIdentity = master.GetComponent<MinionIdentity>();
			this.Kpid = master.GetComponent<KPrefabID>();
		}

		public bool IsPlayingSpeech()
		{
			return base.IsInsideState(base.sm.talking);
		}

		public void PlaySpeech(string speech_prefix, string voice_event)
		{
			this.speechPrefix = speech_prefix;
			this.voiceEvent = voice_event;
			this.GoTo(base.sm.talking);
		}

		public void DrawMouth()
		{
			KAnim.Anim.FrameElement firstFrameElement = SpeechMonitor.GetFirstFrameElement(base.smi.mouth);
			bool flag = firstFrameElement.symbol != HashedString.Invalid;
			DebugUtil.DevAssert(flag, "Mouth frame element invalid", null);
			if (!flag)
			{
				return;
			}
			KAnim.Build build = base.smi.mouth.AnimFiles[0].GetData().build;
			KAnim.Build.Symbol symbol = build.GetSymbol(firstFrameElement.symbol);
			this.SymbolOverrideController.AddSymbolOverride(SpeechMonitor.HASH_SNAPTO_MOUTH, symbol, 3);
			KAnim.Build.Symbol symbol2 = KAnimBatchManager.Instance().GetBatchGroupData(this.AnimController.batchGroupID).GetSymbol(SpeechMonitor.HASH_SNAPTO_MOUTH);
			DebugUtil.DevAssert(build == symbol.build, "Mouth build mismatch", null);
			KAnim.Build.SymbolFrameInstance symbolFrameInstance = KAnimBatchManager.Instance().GetBatchGroupData(build.batchTag).symbolFrameInstances[symbol.firstFrameIdx + firstFrameElement.frame];
			symbolFrameInstance.buildImageIdx = this.SymbolOverrideController.GetAtlasIdx(build.GetTexture(0));
			this.AnimController.SetSymbolOverride(symbol2.firstFrameIdx, ref symbolFrameInstance);
		}

		public void SetMouthId()
		{
			Personality personality = Db.Get().Personalities.Get(this.MinionIdentity.personalityResourceId);
			if (personality.speech_mouth > 0)
			{
				base.smi.mouthId = string.Format("_{0:000}", personality.speech_mouth);
			}
		}

		public KBatchedAnimController mouth;

		public string speechPrefix = "happy";

		public string voiceEvent;

		public EventInstance ev;

		public string mouthId;
	}
}
