using System;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	[NotKeyable]
	[Serializable]
	public class AnimationPlayableAsset : PlayableAsset, ITimelineClipAsset, IPropertyPreview, IClipInitializer, ISerializationCallbackReceiver
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

		public bool useTrackMatchFields
		{
			get
			{
				return this.m_UseTrackMatchFields;
			}
			set
			{
				this.m_UseTrackMatchFields = value;
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
				this.m_MatchTargetFields = value;
			}
		}

		public bool removeStartOffset
		{
			get
			{
				return this.m_RemoveStartOffset;
			}
			set
			{
				this.m_RemoveStartOffset = value;
			}
		}

		public bool applyFootIK
		{
			get
			{
				return this.m_ApplyFootIK;
			}
			set
			{
				this.m_ApplyFootIK = value;
			}
		}

		internal bool hasRootTransforms
		{
			get
			{
				return this.m_Clip != null && AnimationPlayableAsset.HasRootTransforms(this.m_Clip);
			}
		}

		internal AppliedOffsetMode appliedOffsetMode { get; set; }

		public AnimationClip clip
		{
			get
			{
				return this.m_Clip;
			}
			set
			{
				if (value != null)
				{
					base.name = "AnimationPlayableAsset of " + value.name;
				}
				this.m_Clip = value;
			}
		}

		public override double duration
		{
			get
			{
				double num;
				if (this.clip == null || this.clip.empty)
				{
					num = base.duration;
				}
				else
				{
					double num2 = (double)this.clip.length;
					if (num2 < 1.401298464324817E-45)
					{
						num = base.duration;
					}
					else
					{
						if (this.clip.frameRate > 0f)
						{
							double num3 = (double)Mathf.Round(this.clip.length * this.clip.frameRate);
							num2 = num3 / (double)this.clip.frameRate;
						}
						num = num2;
					}
				}
				return num;
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

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			return AnimationPlayableAsset.CreatePlayable(graph, this.m_Clip, this.position, this.eulerAngles, this.removeStartOffset, this.appliedOffsetMode, this.applyFootIK);
		}

		internal static Playable CreatePlayable(PlayableGraph graph, AnimationClip clip, Vector3 positionOffset, Vector3 eulerOffset, bool removeStartOffset, AppliedOffsetMode mode, bool applyFootIK)
		{
			Playable playable;
			if (clip == null || clip.legacy)
			{
				playable = Playable.Null;
			}
			else
			{
				AnimationClipPlayable animationClipPlayable = AnimationClipPlayable.Create(graph, clip);
				animationClipPlayable.SetRemoveStartOffset(removeStartOffset);
				animationClipPlayable.SetApplyFootIK(applyFootIK);
				Playable playable2 = animationClipPlayable;
				if (AnimationPlayableAsset.ShouldApplyScaleRemove(mode))
				{
					AnimationRemoveScalePlayable animationRemoveScalePlayable = AnimationRemoveScalePlayable.Create(graph, 1);
					graph.Connect<Playable, AnimationRemoveScalePlayable>(playable2, 0, animationRemoveScalePlayable, 0);
					animationRemoveScalePlayable.SetInputWeight(0, 1f);
					playable2 = animationRemoveScalePlayable;
				}
				if (AnimationPlayableAsset.ShouldApplyOffset(mode, clip))
				{
					AnimationOffsetPlayable animationOffsetPlayable = AnimationOffsetPlayable.Create(graph, positionOffset, Quaternion.Euler(eulerOffset), 1);
					graph.Connect<Playable, AnimationOffsetPlayable>(playable2, 0, animationOffsetPlayable, 0);
					animationOffsetPlayable.SetInputWeight(0, 1f);
					playable2 = animationOffsetPlayable;
				}
				playable = playable2;
			}
			return playable;
		}

		private static bool ShouldApplyOffset(AppliedOffsetMode mode, AnimationClip clip)
		{
			return mode != AppliedOffsetMode.NoRootTransform && mode != AppliedOffsetMode.SceneOffsetLegacy && AnimationPlayableAsset.HasRootTransforms(clip);
		}

		private static bool ShouldApplyScaleRemove(AppliedOffsetMode mode)
		{
			return mode == AppliedOffsetMode.SceneOffsetLegacyEditor || mode == AppliedOffsetMode.SceneOffsetLegacy || mode == AppliedOffsetMode.TransformOffsetLegacy;
		}

		public ClipCaps clipCaps
		{
			get
			{
				ClipCaps clipCaps = ClipCaps.All;
				if (this.m_Clip == null || !this.m_Clip.isLooping)
				{
					clipCaps &= ~ClipCaps.Looping;
				}
				if (this.m_Clip == null || this.m_Clip.empty)
				{
					clipCaps &= ~ClipCaps.ClipIn;
				}
				return clipCaps;
			}
		}

		public void ResetOffsets()
		{
			this.position = Vector3.zero;
			this.eulerAngles = Vector3.zero;
		}

		public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
		{
			driver.AddFromClip(this.m_Clip);
		}

		void IClipInitializer.OnCreate(TimelineClip newClip, TrackAsset owner, IExposedPropertyTable resolver)
		{
			if (this.clip != null && this.clip.legacy)
			{
				this.clip = null;
				Debug.LogError("Legacy Animation Clips are not supported");
			}
		}

		internal static bool HasRootTransforms(AnimationClip clip)
		{
			return !(clip == null) && !clip.empty && (clip.hasRootMotion || clip.hasGenericRootTransform || clip.hasMotionCurves || clip.hasRootCurves);
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_Version = AnimationPlayableAsset.k_LatestVersion;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this.m_Version < AnimationPlayableAsset.k_LatestVersion)
			{
				this.OnUpgradeFromVersion(this.m_Version);
			}
		}

		private void OnUpgradeFromVersion(int oldVersion)
		{
			if (oldVersion < 1)
			{
				AnimationPlayableAsset.AnimationPlayableAssetUpgrade.ConvertRotationToEuler(this);
			}
		}

		[SerializeField]
		private AnimationClip m_Clip;

		[SerializeField]
		private Vector3 m_Position = Vector3.zero;

		[SerializeField]
		private Vector3 m_EulerAngles = Vector3.zero;

		[SerializeField]
		private bool m_UseTrackMatchFields = true;

		[SerializeField]
		private MatchTargetFields m_MatchTargetFields = MatchTargetFieldConstants.All;

		[SerializeField]
		private bool m_RemoveStartOffset = true;

		[SerializeField]
		private bool m_ApplyFootIK = true;

		private static readonly int k_LatestVersion = 1;

		[SerializeField]
		[HideInInspector]
		private int m_Version;

		[SerializeField]
		[Obsolete("Use m_RotationEuler Instead", false)]
		[HideInInspector]
		private Quaternion m_Rotation = Quaternion.identity;

		private enum Versions
		{
			Initial,
			RotationAsEuler
		}

		private static class AnimationPlayableAssetUpgrade
		{
			public static void ConvertRotationToEuler(AnimationPlayableAsset asset)
			{
				asset.m_EulerAngles = asset.m_Rotation.eulerAngles;
			}
		}
	}
}
