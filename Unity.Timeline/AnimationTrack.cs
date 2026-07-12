using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace UnityEngine.Timeline
{
	[TrackClipType(typeof(AnimationPlayableAsset), false)]
	[TrackBindingType(typeof(Animator))]
	[ExcludeFromPreset]
	[Serializable]
	public class AnimationTrack : TrackAsset, ILayerable
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

		public AnimationClip infiniteClip
		{
			get
			{
				return this.m_InfiniteClip;
			}
			internal set
			{
				this.m_InfiniteClip = value;
			}
		}

		internal bool infiniteClipRemoveOffset
		{
			get
			{
				return this.m_InfiniteClipRemoveOffset;
			}
			set
			{
				this.m_InfiniteClipRemoveOffset = value;
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

		internal override bool CanCompileClips()
		{
			return !base.muted && (this.m_Clips.Count > 0 || (this.m_InfiniteClip != null && !this.m_InfiniteClip.empty));
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

		public Vector3 infiniteClipOffsetPosition
		{
			get
			{
				return this.m_InfiniteClipOffsetPosition;
			}
			set
			{
				this.m_InfiniteClipOffsetPosition = value;
			}
		}

		public Quaternion infiniteClipOffsetRotation
		{
			get
			{
				return Quaternion.Euler(this.m_InfiniteClipOffsetEulerAngles);
			}
			set
			{
				this.m_InfiniteClipOffsetEulerAngles = value.eulerAngles;
			}
		}

		public Vector3 infiniteClipOffsetEulerAngles
		{
			get
			{
				return this.m_InfiniteClipOffsetEulerAngles;
			}
			set
			{
				this.m_InfiniteClipOffsetEulerAngles = value;
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

		internal double infiniteClipTimeOffset
		{
			get
			{
				return this.m_InfiniteClipTimeOffset;
			}
			set
			{
				this.m_InfiniteClipTimeOffset = value;
			}
		}

		public TimelineClip.ClipExtrapolation infiniteClipPreExtrapolation
		{
			get
			{
				return this.m_InfiniteClipPreExtrapolation;
			}
			set
			{
				this.m_InfiniteClipPreExtrapolation = value;
			}
		}

		public TimelineClip.ClipExtrapolation infiniteClipPostExtrapolation
		{
			get
			{
				return this.m_InfiniteClipPostExtrapolation;
			}
			set
			{
				this.m_InfiniteClipPostExtrapolation = value;
			}
		}

		internal AnimationPlayableAsset.LoopMode infiniteClipLoop
		{
			get
			{
				return this.mInfiniteClipLoop;
			}
			set
			{
				this.mInfiniteClipLoop = value;
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
			if (clip == null)
			{
				return null;
			}
			TimelineClip timelineClip = base.CreateClip<AnimationPlayableAsset>();
			this.AssignAnimationClip(timelineClip, clip);
			return timelineClip;
		}

		public void CreateInfiniteClip(string infiniteClipName)
		{
			if (this.inClipMode)
			{
				Debug.LogWarning("CreateInfiniteClip cannot create an infinite clip for an AnimationTrack that contains one or more Timeline Clips.");
				return;
			}
			if (this.m_InfiniteClip != null)
			{
				return;
			}
			this.m_InfiniteClip = TimelineCreateUtilities.CreateAnimationClipForTrack(string.IsNullOrEmpty(infiniteClipName) ? "Recorded" : infiniteClipName, this, false);
		}

		public TimelineClip CreateRecordableClip(string animClipName)
		{
			AnimationClip animationClip = TimelineCreateUtilities.CreateAnimationClipForTrack(string.IsNullOrEmpty(animClipName) ? "Recorded" : animClipName, this, false);
			TimelineClip timelineClip = this.CreateClip(animationClip);
			timelineClip.displayName = animClipName;
			timelineClip.recordable = true;
			timelineClip.start = 0.0;
			timelineClip.duration = 1.0;
			AnimationPlayableAsset animationPlayableAsset = timelineClip.asset as AnimationPlayableAsset;
			if (animationPlayableAsset != null)
			{
				animationPlayableAsset.removeStartOffset = false;
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

		protected internal override int CalculateItemsHash()
		{
			return TrackAsset.GetAnimationClipHash(this.m_InfiniteClip).CombineHash(base.CalculateItemsHash());
		}

		internal void UpdateClipOffsets()
		{
		}

		private Playable CompileTrackPlayable(PlayableGraph graph, AnimationTrack track, GameObject go, IntervalTree<RuntimeElement> tree, AppliedOffsetMode mode)
		{
			AnimationMixerPlayable animationMixerPlayable = AnimationMixerPlayable.Create(graph, track.clips.Length);
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
			if (!track.AnimatesRootTransform())
			{
				return animationMixerPlayable;
			}
			return this.ApplyTrackOffset(graph, animationMixerPlayable, go, mode);
		}

		Playable ILayerable.CreateLayerMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			return Playable.Null;
		}

		internal override Playable CreateMixerPlayableGraph(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree)
		{
			if (base.isSubTrack)
			{
				throw new InvalidOperationException("Nested animation tracks should never be asked to create a graph directly");
			}
			List<AnimationTrack> list = new List<AnimationTrack>();
			if (this.CanCompileClips())
			{
				list.Add(this);
			}
			Transform genericRootNode = this.GetGenericRootNode(go);
			bool flag = this.AnimatesRootTransform();
			bool flag2 = flag && !this.IsRootTransformDisabledByMask(go, genericRootNode);
			foreach (TrackAsset trackAsset in base.GetChildTracks())
			{
				AnimationTrack animationTrack = trackAsset as AnimationTrack;
				if (animationTrack != null && animationTrack.CanCompileClips())
				{
					bool flag3 = animationTrack.AnimatesRootTransform();
					flag |= animationTrack.AnimatesRootTransform();
					flag2 |= flag3 && !animationTrack.IsRootTransformDisabledByMask(go, genericRootNode);
					list.Add(animationTrack);
				}
			}
			AppliedOffsetMode offsetMode = this.GetOffsetMode(go, flag2);
			int defaultBlendCount = this.GetDefaultBlendCount();
			AnimationLayerMixerPlayable animationLayerMixerPlayable = AnimationTrack.CreateGroupMixer(graph, go, list.Count + defaultBlendCount);
			for (int i = 0; i < list.Count; i++)
			{
				int num = i + defaultBlendCount;
				AppliedOffsetMode appliedOffsetMode = offsetMode;
				if (offsetMode != AppliedOffsetMode.NoRootTransform && list[i].IsRootTransformDisabledByMask(go, genericRootNode))
				{
					appliedOffsetMode = AppliedOffsetMode.NoRootTransform;
				}
				Playable playable = (list[i].inClipMode ? this.CompileTrackPlayable(graph, list[i], go, tree, appliedOffsetMode) : list[i].CreateInfiniteTrackPlayable(graph, go, tree, appliedOffsetMode));
				graph.Connect<Playable, AnimationLayerMixerPlayable>(playable, 0, animationLayerMixerPlayable, num);
				animationLayerMixerPlayable.SetInputWeight(num, (float)(list[i].inClipMode ? 0 : 1));
				if (list[i].applyAvatarMask && list[i].avatarMask != null)
				{
					animationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint)num, list[i].avatarMask);
				}
			}
			bool flag4 = this.RequiresMotionXPlayable(offsetMode, go);
			flag4 |= defaultBlendCount > 0 && this.RequiresMotionXPlayable(this.GetOffsetMode(go, flag), go);
			this.AttachDefaultBlend(graph, animationLayerMixerPlayable, flag4);
			Playable playable2 = animationLayerMixerPlayable;
			if (flag4)
			{
				AnimationMotionXToDeltaPlayable animationMotionXToDeltaPlayable = AnimationMotionXToDeltaPlayable.Create(graph);
				graph.Connect<Playable, AnimationMotionXToDeltaPlayable>(playable2, 0, animationMotionXToDeltaPlayable, 0);
				animationMotionXToDeltaPlayable.SetInputWeight(0, 1f);
				animationMotionXToDeltaPlayable.SetAbsoluteMotion(AnimationTrack.UsesAbsoluteMotion(offsetMode));
				playable2 = animationMotionXToDeltaPlayable;
			}
			return playable2;
		}

		private int GetDefaultBlendCount()
		{
			return 0;
		}

		private void AttachDefaultBlend(PlayableGraph graph, AnimationLayerMixerPlayable mixer, bool requireOffset)
		{
		}

		private Playable AttachOffsetPlayable(PlayableGraph graph, Playable playable, Vector3 pos, Quaternion rot)
		{
			AnimationOffsetPlayable animationOffsetPlayable = AnimationOffsetPlayable.Create(graph, pos, rot, 1);
			animationOffsetPlayable.SetInputWeight(0, 1f);
			graph.Connect<Playable, AnimationOffsetPlayable>(playable, 0, animationOffsetPlayable, 0);
			return animationOffsetPlayable;
		}

		private bool RequiresMotionXPlayable(AppliedOffsetMode mode, GameObject gameObject)
		{
			if (mode == AppliedOffsetMode.NoRootTransform)
			{
				return false;
			}
			if (mode == AppliedOffsetMode.SceneOffsetLegacy)
			{
				Animator binding = this.GetBinding((gameObject != null) ? gameObject.GetComponent<PlayableDirector>() : null);
				return binding != null && binding.hasRootMotion;
			}
			return true;
		}

		private static bool UsesAbsoluteMotion(AppliedOffsetMode mode)
		{
			return mode != AppliedOffsetMode.SceneOffset && mode != AppliedOffsetMode.SceneOffsetLegacy;
		}

		private bool HasController(GameObject gameObject)
		{
			Animator binding = this.GetBinding((gameObject != null) ? gameObject.GetComponent<PlayableDirector>() : null);
			return binding != null && binding.runtimeAnimatorController != null;
		}

		internal Animator GetBinding(PlayableDirector director)
		{
			if (director == null)
			{
				return null;
			}
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
			Animator animator = null;
			if (object2 != null)
			{
				animator = object2 as Animator;
				GameObject gameObject = object2 as GameObject;
				if (animator == null && gameObject != null)
				{
					animator = gameObject.GetComponent<Animator>();
				}
			}
			return animator;
		}

		private static AnimationLayerMixerPlayable CreateGroupMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			return AnimationLayerMixerPlayable.Create(graph, inputCount, false);
		}

		private Playable CreateInfiniteTrackPlayable(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree, AppliedOffsetMode mode)
		{
			if (this.m_InfiniteClip == null)
			{
				return Playable.Null;
			}
			AnimationMixerPlayable animationMixerPlayable = AnimationMixerPlayable.Create(graph, 1);
			Playable playable = AnimationPlayableAsset.CreatePlayable(graph, this.m_InfiniteClip, this.m_InfiniteClipOffsetPosition, this.m_InfiniteClipOffsetEulerAngles, false, mode, this.infiniteClipApplyFootIK, AnimationPlayableAsset.LoopMode.Off);
			if (playable.IsValid<Playable>())
			{
				tree.Add(new InfiniteRuntimeClip(playable));
				graph.Connect<Playable, AnimationMixerPlayable>(playable, 0, animationMixerPlayable, 0);
				animationMixerPlayable.SetInputWeight(0, 1f);
			}
			if (!this.AnimatesRootTransform())
			{
				return animationMixerPlayable;
			}
			return (base.isSubTrack ? ((AnimationTrack)base.parent) : this).ApplyTrackOffset(graph, animationMixerPlayable, go, mode);
		}

		private Playable ApplyTrackOffset(PlayableGraph graph, Playable root, GameObject go, AppliedOffsetMode mode)
		{
			if (mode == AppliedOffsetMode.SceneOffsetLegacy || mode == AppliedOffsetMode.SceneOffset || mode == AppliedOffsetMode.NoRootTransform)
			{
				return root;
			}
			Vector3 position = this.position;
			Quaternion rotation = this.rotation;
			AnimationOffsetPlayable animationOffsetPlayable = AnimationOffsetPlayable.Create(graph, position, rotation, 1);
			graph.Connect<Playable, AnimationOffsetPlayable>(root, 0, animationOffsetPlayable, 0);
			animationOffsetPlayable.SetInputWeight(0, 1f);
			return animationOffsetPlayable;
		}

		internal override void GetEvaluationTime(out double outStart, out double outDuration)
		{
			if (this.inClipMode)
			{
				base.GetEvaluationTime(out outStart, out outDuration);
				return;
			}
			outStart = 0.0;
			outDuration = TimelineClip.kMaxTimeValue;
		}

		internal override void GetSequenceTime(out double outStart, out double outDuration)
		{
			if (this.inClipMode)
			{
				base.GetSequenceTime(out outStart, out outDuration);
				return;
			}
			outStart = 0.0;
			outDuration = Math.Max(base.GetNotificationDuration(), TimeUtility.GetAnimationClipLength(this.m_InfiniteClip));
		}

		private void AssignAnimationClip(TimelineClip clip, AnimationClip animClip)
		{
			if (clip == null || animClip == null)
			{
				return;
			}
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

		public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
		{
		}

		private void GetAnimationClips(List<AnimationClip> animClips)
		{
			TimelineClip[] clips = base.clips;
			for (int i = 0; i < clips.Length; i++)
			{
				AnimationPlayableAsset animationPlayableAsset = clips[i].asset as AnimationPlayableAsset;
				if (animationPlayableAsset != null && animationPlayableAsset.clip != null)
				{
					animClips.Add(animationPlayableAsset.clip);
				}
			}
			if (this.m_InfiniteClip != null)
			{
				animClips.Add(this.m_InfiniteClip);
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
			if (!animatesRootTransform)
			{
				return AppliedOffsetMode.NoRootTransform;
			}
			if (this.m_TrackOffset == TrackOffset.ApplyTransformOffsets)
			{
				return AppliedOffsetMode.TransformOffset;
			}
			if (this.m_TrackOffset == TrackOffset.ApplySceneOffsets)
			{
				if (!Application.isPlaying)
				{
					return AppliedOffsetMode.SceneOffsetEditor;
				}
				return AppliedOffsetMode.SceneOffset;
			}
			else
			{
				if (!this.HasController(go))
				{
					return AppliedOffsetMode.TransformOffsetLegacy;
				}
				if (!Application.isPlaying)
				{
					return AppliedOffsetMode.SceneOffsetLegacyEditor;
				}
				return AppliedOffsetMode.SceneOffsetLegacy;
			}
		}

		private bool IsRootTransformDisabledByMask(GameObject gameObject, Transform genericRootNode)
		{
			if (this.avatarMask == null || !this.applyAvatarMask)
			{
				return false;
			}
			Animator binding = this.GetBinding((gameObject != null) ? gameObject.GetComponent<PlayableDirector>() : null);
			if (binding == null)
			{
				return false;
			}
			if (binding.isHuman)
			{
				return !this.avatarMask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.Root);
			}
			if (this.avatarMask.transformCount == 0)
			{
				return false;
			}
			if (genericRootNode == null)
			{
				return string.IsNullOrEmpty(this.avatarMask.GetTransformPath(0)) && !this.avatarMask.GetTransformActive(0);
			}
			for (int i = 0; i < this.avatarMask.transformCount; i++)
			{
				if (genericRootNode == binding.transform.Find(this.avatarMask.GetTransformPath(i)))
				{
					return !this.avatarMask.GetTransformActive(i);
				}
			}
			return false;
		}

		private Transform GetGenericRootNode(GameObject gameObject)
		{
			Animator binding = this.GetBinding((gameObject != null) ? gameObject.GetComponent<PlayableDirector>() : null);
			if (binding == null)
			{
				return null;
			}
			if (binding.isHuman)
			{
				return null;
			}
			if (binding.avatar == null)
			{
				return null;
			}
			string rootMotionBoneName = binding.avatar.humanDescription.m_RootMotionBoneName;
			if (rootMotionBoneName == binding.name || string.IsNullOrEmpty(rootMotionBoneName))
			{
				return null;
			}
			return AnimationTrack.FindInHierarchyBreadthFirst(binding.transform, rootMotionBoneName);
		}

		internal bool AnimatesRootTransform()
		{
			if (AnimationPlayableAsset.HasRootTransforms(this.m_InfiniteClip))
			{
				return true;
			}
			foreach (TimelineClip timelineClip in base.GetClips())
			{
				AnimationPlayableAsset animationPlayableAsset = timelineClip.asset as AnimationPlayableAsset;
				if (animationPlayableAsset != null && animationPlayableAsset.hasRootTransforms)
				{
					return true;
				}
			}
			return false;
		}

		private static Transform FindInHierarchyBreadthFirst(Transform t, string name)
		{
			AnimationTrack.s_CachedQueue.Clear();
			AnimationTrack.s_CachedQueue.Enqueue(t);
			while (AnimationTrack.s_CachedQueue.Count > 0)
			{
				Transform transform = AnimationTrack.s_CachedQueue.Dequeue();
				if (transform.name == name)
				{
					return transform;
				}
				for (int i = 0; i < transform.childCount; i++)
				{
					AnimationTrack.s_CachedQueue.Enqueue(transform.GetChild(i));
				}
			}
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("openClipOffsetPosition has been deprecated. Use infiniteClipOffsetPosition instead. (UnityUpgradable) -> infiniteClipOffsetPosition", true)]
		public Vector3 openClipOffsetPosition
		{
			get
			{
				return this.infiniteClipOffsetPosition;
			}
			set
			{
				this.infiniteClipOffsetPosition = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("openClipOffsetRotation has been deprecated. Use infiniteClipOffsetRotation instead. (UnityUpgradable) -> infiniteClipOffsetRotation", true)]
		public Quaternion openClipOffsetRotation
		{
			get
			{
				return this.infiniteClipOffsetRotation;
			}
			set
			{
				this.infiniteClipOffsetRotation = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("openClipOffsetEulerAngles has been deprecated. Use infiniteClipOffsetEulerAngles instead. (UnityUpgradable) -> infiniteClipOffsetEulerAngles", true)]
		public Vector3 openClipOffsetEulerAngles
		{
			get
			{
				return this.infiniteClipOffsetEulerAngles;
			}
			set
			{
				this.infiniteClipOffsetEulerAngles = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("openClipPreExtrapolation has been deprecated. Use infiniteClipPreExtrapolation instead. (UnityUpgradable) -> infiniteClipPreExtrapolation", true)]
		public TimelineClip.ClipExtrapolation openClipPreExtrapolation
		{
			get
			{
				return this.infiniteClipPreExtrapolation;
			}
			set
			{
				this.infiniteClipPreExtrapolation = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("openClipPostExtrapolation has been deprecated. Use infiniteClipPostExtrapolation instead. (UnityUpgradable) -> infiniteClipPostExtrapolation", true)]
		public TimelineClip.ClipExtrapolation openClipPostExtrapolation
		{
			get
			{
				return this.infiniteClipPostExtrapolation;
			}
			set
			{
				this.infiniteClipPostExtrapolation = value;
			}
		}

		internal override void OnUpgradeFromVersion(int oldVersion)
		{
			if (oldVersion < 1)
			{
				AnimationTrack.AnimationTrackUpgrade.ConvertRotationsToEuler(this);
			}
			if (oldVersion < 2)
			{
				AnimationTrack.AnimationTrackUpgrade.ConvertRootMotion(this);
			}
			if (oldVersion < 3)
			{
				AnimationTrack.AnimationTrackUpgrade.ConvertInfiniteTrack(this);
			}
		}

		private const string k_DefaultInfiniteClipName = "Recorded";

		private const string k_DefaultRecordableClipName = "Recorded";

		[SerializeField]
		[FormerlySerializedAs("m_OpenClipPreExtrapolation")]
		private TimelineClip.ClipExtrapolation m_InfiniteClipPreExtrapolation;

		[SerializeField]
		[FormerlySerializedAs("m_OpenClipPostExtrapolation")]
		private TimelineClip.ClipExtrapolation m_InfiniteClipPostExtrapolation;

		[SerializeField]
		[FormerlySerializedAs("m_OpenClipOffsetPosition")]
		private Vector3 m_InfiniteClipOffsetPosition = Vector3.zero;

		[SerializeField]
		[FormerlySerializedAs("m_OpenClipOffsetEulerAngles")]
		private Vector3 m_InfiniteClipOffsetEulerAngles = Vector3.zero;

		[SerializeField]
		[FormerlySerializedAs("m_OpenClipTimeOffset")]
		private double m_InfiniteClipTimeOffset;

		[SerializeField]
		[FormerlySerializedAs("m_OpenClipRemoveOffset")]
		private bool m_InfiniteClipRemoveOffset;

		[SerializeField]
		private bool m_InfiniteClipApplyFootIK = true;

		[SerializeField]
		[HideInInspector]
		private AnimationPlayableAsset.LoopMode mInfiniteClipLoop;

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
		private TrackOffset m_TrackOffset;

		[SerializeField]
		[HideInInspector]
		private AnimationClip m_InfiniteClip;

		private static readonly Queue<Transform> s_CachedQueue = new Queue<Transform>(100);

		[SerializeField]
		[Obsolete("Use m_InfiniteClipOffsetEulerAngles Instead", false)]
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
				track.m_InfiniteClipOffsetEulerAngles = track.m_OpenClipOffsetRotation.eulerAngles;
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

			public static void ConvertInfiniteTrack(AnimationTrack track)
			{
				track.m_InfiniteClip = track.m_AnimClip;
				track.m_AnimClip = null;
			}
		}
	}
}
