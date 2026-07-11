using System;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	[TrackClipType(typeof(AnimationPlayableAsset), false)]
	[TrackBindingType(typeof(Animator))]
	[SupportsChildTracks(typeof(AnimationTrack), 1)]
	[Serializable]
	public class AnimationTrack : TrackAsset
	{
		public Vector3 position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
			}
		}

		public Quaternion rotation
		{
			get
			{
				return Quaternion.Euler(this.m_EulerAngles);
			}
			set
			{
				this.m_EulerAngles = value.eulerAngles;
			}
		}

		public Vector3 eulerAngles
		{
			get
			{
				return this.m_EulerAngles;
			}
			set
			{
				this.m_EulerAngles = value;
			}
		}

		[Obsolete("applyOffset is deprecated. Use trackOffset instead", true)]
		public bool applyOffsets
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public TrackOffset trackOffset
		{
			get
			{
				return this.m_TrackOffset;
			}
			set
			{
				this.m_TrackOffset = value;
			}
		}

		public MatchTargetFields matchTargetFields
		{
			get
			{
				return this.m_MatchTargetFields;
			}
			set
			{
				this.m_MatchTargetFields = value & MatchTargetFieldConstants.All;
			}
		}

		internal bool openClipRemoveOffset
		{
			get
			{
				return this.m_OpenClipRemoveOffset;
			}
			set
			{
				this.m_OpenClipRemoveOffset = value;
			}
		}

		private bool compilableIsolated
		{
			get
			{
				return !base.muted && (this.m_Clips.Count > 0 || (base.animClip != null && !base.animClip.empty));
			}
		}

		public AvatarMask avatarMask
		{
			get
			{
				return this.m_AvatarMask;
			}
			set
			{
				this.m_AvatarMask = value;
			}
		}

		public bool applyAvatarMask
		{
			get
			{
				return this.m_ApplyAvatarMask;
			}
			set
			{
				this.m_ApplyAvatarMask = value;
			}
		}

		internal override bool compilable
		{
			get
			{
				bool flag;
				if (this.compilableIsolated)
				{
					flag = true;
				}
				else
				{
					foreach (TrackAsset trackAsset in base.GetChildTracks())
					{
						if (trackAsset.compilable)
						{
							return true;
						}
					}
					flag = false;
				}
				return flag;
			}
		}

		public override IEnumerable<PlayableBinding> outputs
		{
			get
			{
				yield return AnimationPlayableBinding.Create(base.name, this);
				yield break;
			}
		}

		public bool inClipMode
		{
			get
			{
				return base.clips != null && base.clips.Length != 0;
			}
		}

		public Vector3 openClipOffsetPosition
		{
			get
			{
				return this.m_OpenClipOffsetPosition;
			}
			set
			{
				this.m_OpenClipOffsetPosition = value;
			}
		}

		public Quaternion openClipOffsetRotation
		{
			get
			{
				return Quaternion.Euler(this.m_OpenClipOffsetEulerAngles);
			}
			set
			{
				this.m_OpenClipOffsetEulerAngles = value.eulerAngles;
			}
		}

		public Vector3 openClipOffsetEulerAngles
		{
			get
			{
				return this.m_OpenClipOffsetEulerAngles;
			}
			set
			{
				this.m_OpenClipOffsetEulerAngles = value;
			}
		}

		internal bool infiniteClipApplyFootIK
		{
			get
			{
				return this.m_InfiniteClipApplyFootIK;
			}
			set
			{
				this.m_InfiniteClipApplyFootIK = value;
			}
		}

		internal double openClipTimeOffset
		{
			get
			{
				return this.m_OpenClipTimeOffset;
			}
			set
			{
				this.m_OpenClipTimeOffset = value;
			}
		}

		public TimelineClip.ClipExtrapolation openClipPreExtrapolation
		{
			get
			{
				return this.m_OpenClipPreExtrapolation;
			}
			set
			{
				this.m_OpenClipPreExtrapolation = value;
			}
		}

		public TimelineClip.ClipExtrapolation openClipPostExtrapolation
		{
			get
			{
				return this.m_OpenClipPostExtrapolation;
			}
			set
			{
				this.m_OpenClipPostExtrapolation = value;
			}
		}

		[ContextMenu("Reset Offsets")]
		private void ResetOffsets()
		{
			this.m_Position = Vector3.zero;
			this.m_EulerAngles = Vector3.zero;
			this.UpdateClipOffsets();
		}

		public TimelineClip CreateClip(AnimationClip clip)
		{
			TimelineClip timelineClip;
			if (clip == null)
			{
				timelineClip = null;
			}
			else
			{
				TimelineClip timelineClip2 = base.CreateClip<AnimationPlayableAsset>();
				this.AssignAnimationClip(timelineClip2, clip);
				timelineClip = timelineClip2;
			}
			return timelineClip;
		}

		protected override void OnCreateClip(TimelineClip clip)
		{
			TimelineClip.ClipExtrapolation clipExtrapolation = TimelineClip.ClipExtrapolation.None;
			if (!base.isSubTrack)
			{
				clipExtrapolation = TimelineClip.ClipExtrapolation.Hold;
			}
			clip.preExtrapolationMode = clipExtrapolation;
			clip.postExtrapolationMode = clipExtrapolation;
		}

		internal void UpdateClipOffsets()
		{
		}

		private Playable CompileTrackPlayable(PlayableGraph graph, TrackAsset track, GameObject go, IntervalTree<RuntimeElement> tree, AppliedOffsetMode mode)
		{
			AnimationMixerPlayable animationMixerPlayable = AnimationMixerPlayable.Create(graph, track.clips.Length, false);
			for (int i = 0; i < track.clips.Length; i++)
			{
				TimelineClip timelineClip = track.clips[i];
				PlayableAsset playableAsset = timelineClip.asset as PlayableAsset;
				if (!(playableAsset == null))
				{
					AnimationPlayableAsset animationPlayableAsset = playableAsset as AnimationPlayableAsset;
					if (animationPlayableAsset != null)
					{
						animationPlayableAsset.appliedOffsetMode = mode;
					}
					Playable playable = playableAsset.CreatePlayable(graph, go);
					if (playable.IsValid<Playable>())
					{
						RuntimeClip runtimeClip = new RuntimeClip(timelineClip, playable, animationMixerPlayable);
						tree.Add(runtimeClip);
						graph.Connect<Playable, AnimationMixerPlayable>(playable, 0, animationMixerPlayable, i);
						animationMixerPlayable.SetInputWeight(i, 0f);
					}
				}
			}
			return this.ApplyTrackOffset(graph, animationMixerPlayable, go, mode);
		}

		internal override Playable OnCreatePlayableGraph(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree)
		{
			if (base.isSubTrack)
			{
				throw new InvalidOperationException("Nested animation tracks should never be asked to create a graph directly");
			}
			List<AnimationTrack> list = new List<AnimationTrack>();
			if (this.compilableIsolated)
			{
				list.Add(this);
			}
			bool flag = this.AnimatesRootTransform();
			foreach (TrackAsset trackAsset in base.GetChildTracks())
			{
				AnimationTrack animationTrack = trackAsset as AnimationTrack;
				if (animationTrack != null && animationTrack.compilable)
				{
					flag |= animationTrack.AnimatesRootTransform();
					list.Add(animationTrack);
				}
			}
			AppliedOffsetMode offsetMode = this.GetOffsetMode(go, flag);
			AnimationLayerMixerPlayable animationLayerMixerPlayable = AnimationTrack.CreateGroupMixer(graph, go, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Playable playable = ((!list[i].inClipMode) ? list[i].CreateInfiniteTrackPlayable(graph, go, tree, offsetMode) : this.CompileTrackPlayable(graph, list[i], go, tree, offsetMode));
				graph.Connect<Playable, AnimationLayerMixerPlayable>(playable, 0, animationLayerMixerPlayable, i);
				animationLayerMixerPlayable.SetInputWeight(i, (float)((!list[i].inClipMode) ? 1 : 0));
				if (list[i].applyAvatarMask && list[i].avatarMask != null)
				{
					animationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint)i, list[i].avatarMask);
				}
			}
			Playable playable2 = animationLayerMixerPlayable;
			Playable playable3;
			if (!this.RequiresMotionXPlayable(offsetMode, go))
			{
				playable3 = playable2;
			}
			else
			{
				AnimationMotionXToDeltaPlayable animationMotionXToDeltaPlayable = AnimationMotionXToDeltaPlayable.Create(graph);
				graph.Connect<Playable, AnimationMotionXToDeltaPlayable>(playable2, 0, animationMotionXToDeltaPlayable, 0);
				animationMotionXToDeltaPlayable.SetInputWeight(0, 1f);
				animationMotionXToDeltaPlayable.SetAbsoluteMotion(AnimationTrack.UsesAbsoluteMotion(offsetMode));
				playable3 = animationMotionXToDeltaPlayable;
			}
			return playable3;
		}

		private bool RequiresMotionXPlayable(AppliedOffsetMode mode, GameObject gameObject)
		{
			bool flag;
			if (mode == AppliedOffsetMode.NoRootTransform)
			{
				flag = false;
			}
			else if (mode == AppliedOffsetMode.SceneOffsetLegacy)
			{
				Animator binding = this.GetBinding((!(gameObject != null)) ? null : gameObject.GetComponent<PlayableDirector>());
				flag = binding != null && binding.hasRootMotion;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		private static bool UsesAbsoluteMotion(AppliedOffsetMode mode)
		{
			return mode != AppliedOffsetMode.SceneOffset && mode != AppliedOffsetMode.SceneOffsetLegacy;
		}

		private bool HasController(GameObject gameObject)
		{
			Animator binding = this.GetBinding((!(gameObject != null)) ? null : gameObject.GetComponent<PlayableDirector>());
			return binding != null && binding.runtimeAnimatorController != null;
		}

		internal Animator GetBinding(PlayableDirector director)
		{
			Animator animator;
			if (director == null)
			{
				animator = null;
			}
			else
			{
				Object @object = this;
				if (base.isSubTrack)
				{
					@object = base.parent;
				}
				Object object2 = null;
				if (director != null)
				{
					object2 = director.GetGenericBinding(@object);
				}
				Animator animator2 = null;
				if (object2 != null)
				{
					animator2 = object2 as Animator;
					GameObject gameObject = object2 as GameObject;
					if (animator2 == null && gameObject != null)
					{
						animator2 = gameObject.GetComponent<Animator>();
					}
				}
				animator = animator2;
			}
			return animator;
		}

		private static AnimationLayerMixerPlayable CreateGroupMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			return AnimationLayerMixerPlayable.Create(graph, inputCount);
		}

		private Playable CreateInfiniteTrackPlayable(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree, AppliedOffsetMode mode)
		{
			Playable playable;
			if (base.animClip == null)
			{
				playable = Playable.Null;
			}
			else
			{
				AnimationMixerPlayable animationMixerPlayable = AnimationMixerPlayable.Create(graph, 1, false);
				Playable playable2 = AnimationPlayableAsset.CreatePlayable(graph, base.animClip, this.m_OpenClipOffsetPosition, this.m_OpenClipOffsetEulerAngles, false, mode, this.infiniteClipApplyFootIK);
				if (playable2.IsValid<Playable>())
				{
					tree.Add(new InfiniteRuntimeClip(playable2));
					graph.Connect<Playable, AnimationMixerPlayable>(playable2, 0, animationMixerPlayable, 0);
					animationMixerPlayable.SetInputWeight(0, 1f);
				}
				playable = this.ApplyTrackOffset(graph, animationMixerPlayable, go, mode);
			}
			return playable;
		}

		private Playable ApplyTrackOffset(PlayableGraph graph, Playable root, GameObject go, AppliedOffsetMode mode)
		{
			Playable playable;
			if (mode == AppliedOffsetMode.SceneOffsetLegacy || mode == AppliedOffsetMode.SceneOffset || mode == AppliedOffsetMode.NoRootTransform || !this.AnimatesRootTransform())
			{
				playable = root;
			}
			else
			{
				Vector3 position = this.position;
				Quaternion rotation = this.rotation;
				AnimationOffsetPlayable animationOffsetPlayable = AnimationOffsetPlayable.Create(graph, position, rotation, 1);
				graph.Connect<Playable, AnimationOffsetPlayable>(root, 0, animationOffsetPlayable, 0);
				animationOffsetPlayable.SetInputWeight(0, 1f);
				playable = animationOffsetPlayable;
			}
			return playable;
		}

		internal override void GetEvaluationTime(out double outStart, out double outDuration)
		{
			if (this.inClipMode)
			{
				base.GetEvaluationTime(out outStart, out outDuration);
			}
			else
			{
				outStart = 0.0;
				outDuration = TimelineClip.kMaxTimeValue;
			}
		}

		internal override void GetSequenceTime(out double outStart, out double outDuration)
		{
			if (this.inClipMode)
			{
				base.GetSequenceTime(out outStart, out outDuration);
			}
			else
			{
				outStart = 0.0;
				outDuration = 0.0;
				if (base.animClip != null)
				{
					outDuration = (double)base.animClip.length;
				}
			}
		}

		private void AssignAnimationClip(TimelineClip clip, AnimationClip animClip)
		{
			if (clip != null && !(animClip == null))
			{
				if (animClip.legacy)
				{
					throw new InvalidOperationException("Legacy Animation Clips are not supported");
				}
				AnimationPlayableAsset animationPlayableAsset = clip.asset as AnimationPlayableAsset;
				if (animationPlayableAsset != null)
				{
					animationPlayableAsset.clip = animClip;
					animationPlayableAsset.name = animClip.name;
					double duration = animationPlayableAsset.duration;
					if (!double.IsInfinity(duration) && duration >= TimelineClip.kMinDuration && duration < TimelineClip.kMaxTimeValue)
					{
						clip.duration = duration;
					}
				}
				clip.displayName = animClip.name;
			}
		}

		public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
		{
		}

		private void GetAnimationClips(List<AnimationClip> animClips)
		{
			foreach (TimelineClip timelineClip in base.clips)
			{
				AnimationPlayableAsset animationPlayableAsset = timelineClip.asset as AnimationPlayableAsset;
				if (animationPlayableAsset != null && animationPlayableAsset.clip != null)
				{
					animClips.Add(animationPlayableAsset.clip);
				}
			}
			if (base.animClip != null)
			{
				animClips.Add(base.animClip);
			}
			foreach (TrackAsset trackAsset in base.GetChildTracks())
			{
				AnimationTrack animationTrack = trackAsset as AnimationTrack;
				if (animationTrack != null)
				{
					animationTrack.GetAnimationClips(animClips);
				}
			}
		}

		private AppliedOffsetMode GetOffsetMode(GameObject go, bool animatesRootTransform)
		{
			AppliedOffsetMode appliedOffsetMode;
			if (!animatesRootTransform)
			{
				appliedOffsetMode = AppliedOffsetMode.NoRootTransform;
			}
			else if (this.m_TrackOffset == TrackOffset.ApplyTransformOffsets)
			{
				appliedOffsetMode = AppliedOffsetMode.TransformOffset;
			}
			else if (this.m_TrackOffset == TrackOffset.ApplySceneOffsets)
			{
				appliedOffsetMode = ((!Application.isPlaying) ? AppliedOffsetMode.SceneOffsetEditor : AppliedOffsetMode.SceneOffset);
			}
			else if (this.HasController(go))
			{
				if (!Application.isPlaying)
				{
					appliedOffsetMode = AppliedOffsetMode.SceneOffsetLegacyEditor;
				}
				else
				{
					appliedOffsetMode = AppliedOffsetMode.SceneOffsetLegacy;
				}
			}
			else
			{
				appliedOffsetMode = AppliedOffsetMode.TransformOffsetLegacy;
			}
			return appliedOffsetMode;
		}

		internal bool AnimatesRootTransform()
		{
			bool flag;
			if (AnimationPlayableAsset.HasRootTransforms(base.animClip))
			{
				flag = true;
			}
			else
			{
				foreach (TimelineClip timelineClip in base.GetClips())
				{
					AnimationPlayableAsset animationPlayableAsset = timelineClip.asset as AnimationPlayableAsset;
					if (animationPlayableAsset != null && animationPlayableAsset.hasRootTransforms)
					{
						return true;
					}
				}
				flag = false;
			}
			return flag;
		}

		protected internal override void OnUpgradeFromVersion(int oldVersion)
		{
			if (oldVersion < 1)
			{
				AnimationTrack.AnimationTrackUpgrade.ConvertRotationsToEuler(this);
			}
			if (oldVersion < 2)
			{
				AnimationTrack.AnimationTrackUpgrade.ConvertRootMotion(this);
			}
		}

		[SerializeField]
		private TimelineClip.ClipExtrapolation m_OpenClipPreExtrapolation = TimelineClip.ClipExtrapolation.None;

		[SerializeField]
		private TimelineClip.ClipExtrapolation m_OpenClipPostExtrapolation = TimelineClip.ClipExtrapolation.None;

		[SerializeField]
		private Vector3 m_OpenClipOffsetPosition = Vector3.zero;

		[SerializeField]
		private Vector3 m_OpenClipOffsetEulerAngles = Vector3.zero;

		[SerializeField]
		private double m_OpenClipTimeOffset;

		[SerializeField]
		private bool m_OpenClipRemoveOffset;

		[SerializeField]
		private bool m_InfiniteClipApplyFootIK = true;

		[SerializeField]
		private MatchTargetFields m_MatchTargetFields = MatchTargetFieldConstants.All;

		[SerializeField]
		private Vector3 m_Position = Vector3.zero;

		[SerializeField]
		private Vector3 m_EulerAngles = Vector3.zero;

		[SerializeField]
		private AvatarMask m_AvatarMask;

		[SerializeField]
		private bool m_ApplyAvatarMask = true;

		[SerializeField]
		private TrackOffset m_TrackOffset = TrackOffset.ApplyTransformOffsets;

		[SerializeField]
		[Obsolete("Use m_OpenClipOffsetEuler Instead", false)]
		[HideInInspector]
		private Quaternion m_OpenClipOffsetRotation = Quaternion.identity;

		[SerializeField]
		[Obsolete("Use m_RotationEuler Instead", false)]
		[HideInInspector]
		private Quaternion m_Rotation = Quaternion.identity;

		[SerializeField]
		[Obsolete("Use m_RootTransformOffsetMode", false)]
		[HideInInspector]
		private bool m_ApplyOffsets;

		private static class AnimationTrackUpgrade
		{
			public static void ConvertRotationsToEuler(AnimationTrack track)
			{
				track.m_EulerAngles = track.m_Rotation.eulerAngles;
				track.m_OpenClipOffsetEulerAngles = track.m_OpenClipOffsetRotation.eulerAngles;
			}

			public static void ConvertRootMotion(AnimationTrack track)
			{
				track.m_TrackOffset = TrackOffset.Auto;
				if (!track.m_ApplyOffsets)
				{
					track.m_Position = Vector3.zero;
					track.m_EulerAngles = Vector3.zero;
				}
			}
		}
	}
}
