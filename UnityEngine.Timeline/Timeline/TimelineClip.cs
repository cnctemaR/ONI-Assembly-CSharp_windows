using System;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace UnityEngine.Timeline
{
	[Serializable]
	public class TimelineClip : ISerializationCallbackReceiver
	{
		internal TimelineClip(TrackAsset parent)
		{
			this.parentTrack = parent;
		}

		internal int dirtyHash { get; set; }

		public bool hasPreExtrapolation
		{
			get
			{
				return this.m_PreExtrapolationMode != TimelineClip.ClipExtrapolation.None && this.m_PreExtrapolationTime > 0.0;
			}
		}

		public bool hasPostExtrapolation
		{
			get
			{
				return this.m_PostExtrapolationMode != TimelineClip.ClipExtrapolation.None && this.m_PostExtrapolationTime > 0.0;
			}
		}

		public double timeScale
		{
			get
			{
				return (!this.clipCaps.HasAny(ClipCaps.SpeedMultiplier)) ? 1.0 : Math.Max(TimelineClip.kTimeScaleMin, Math.Min(this.m_TimeScale, TimelineClip.kTimeScaleMax));
			}
			set
			{
				this.m_TimeScale = ((!this.clipCaps.HasAny(ClipCaps.SpeedMultiplier)) ? 1.0 : Math.Max(TimelineClip.kTimeScaleMin, Math.Min(value, TimelineClip.kTimeScaleMax)));
			}
		}

		public double start
		{
			get
			{
				return this.m_Start;
			}
			set
			{
				double num = Math.Max(TimelineClip.SanitizeTimeValue(value, this.m_Start), 0.0);
				if (this.m_ParentTrack != null && this.m_Start != num)
				{
					this.m_ParentTrack.OnClipMove();
				}
				this.m_Start = num;
			}
		}

		public double duration
		{
			get
			{
				return this.m_Duration;
			}
			set
			{
				this.m_Duration = Math.Max(TimelineClip.SanitizeTimeValue(value, this.m_Duration), double.Epsilon);
			}
		}

		public double end
		{
			get
			{
				return this.m_Start + this.m_Duration;
			}
		}

		public double clipIn
		{
			get
			{
				return (!this.clipCaps.HasAny(ClipCaps.ClipIn)) ? 0.0 : this.m_ClipIn;
			}
			set
			{
				this.m_ClipIn = ((!this.clipCaps.HasAny(ClipCaps.ClipIn)) ? 0.0 : Math.Max(Math.Min(TimelineClip.SanitizeTimeValue(value, this.m_ClipIn), TimelineClip.kMaxTimeValue), 0.0));
			}
		}

		public string displayName
		{
			get
			{
				return this.m_DisplayName;
			}
			set
			{
				this.m_DisplayName = value;
			}
		}

		public double clipAssetDuration
		{
			get
			{
				IPlayableAsset playableAsset = this.m_Asset as IPlayableAsset;
				return (playableAsset == null) ? double.MaxValue : playableAsset.duration;
			}
		}

		public AnimationClip curves
		{
			get
			{
				return this.m_AnimationCurves;
			}
		}

		public Object asset
		{
			get
			{
				return this.m_Asset;
			}
			set
			{
				this.m_Asset = value;
			}
		}

		[Obsolete("underlyingAsset property is obsolete. Use asset property instead", true)]
		public Object underlyingAsset
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public TrackAsset parentTrack
		{
			get
			{
				return this.m_ParentTrack;
			}
			set
			{
				if (!(this.m_ParentTrack == value))
				{
					if (this.m_ParentTrack != null)
					{
						this.m_ParentTrack.RemoveClip(this);
					}
					this.m_ParentTrack = value;
					if (this.m_ParentTrack != null)
					{
						this.m_ParentTrack.AddClip(this);
					}
				}
			}
		}

		public double easeInDuration
		{
			get
			{
				return (!this.clipCaps.HasAny(ClipCaps.Blending)) ? 0.0 : Math.Min(Math.Max(this.m_EaseInDuration, 0.0), this.duration * 0.49);
			}
			set
			{
				this.m_EaseInDuration = ((!this.clipCaps.HasAny(ClipCaps.Blending)) ? 0.0 : Math.Max(0.0, Math.Min(TimelineClip.SanitizeTimeValue(value, this.m_EaseInDuration), this.duration * 0.49)));
			}
		}

		public double easeOutDuration
		{
			get
			{
				return (!this.clipCaps.HasAny(ClipCaps.Blending)) ? 0.0 : Math.Min(Math.Max(this.m_EaseOutDuration, 0.0), this.duration * 0.49);
			}
			set
			{
				this.m_EaseOutDuration = ((!this.clipCaps.HasAny(ClipCaps.Blending)) ? 0.0 : Math.Max(0.0, Math.Min(TimelineClip.SanitizeTimeValue(value, this.m_EaseOutDuration), this.duration * 0.49)));
			}
		}

		[Obsolete("Use easeOutTime instead (UnityUpgradable) -> easeOutTime", true)]
		public double eastOutTime
		{
			get
			{
				return this.duration - this.easeOutDuration + this.m_Start;
			}
		}

		public double easeOutTime
		{
			get
			{
				return this.duration - this.easeOutDuration + this.m_Start;
			}
		}

		public double blendInDuration
		{
			get
			{
				return (!this.clipCaps.HasAny(ClipCaps.Blending)) ? 0.0 : this.m_BlendInDuration;
			}
			set
			{
				this.m_BlendInDuration = ((!this.clipCaps.HasAny(ClipCaps.Blending)) ? 0.0 : TimelineClip.SanitizeTimeValue(value, this.m_BlendInDuration));
			}
		}

		public double blendOutDuration
		{
			get
			{
				return (!this.clipCaps.HasAny(ClipCaps.Blending)) ? 0.0 : this.m_BlendOutDuration;
			}
			set
			{
				this.m_BlendOutDuration = ((!this.clipCaps.HasAny(ClipCaps.Blending)) ? 0.0 : TimelineClip.SanitizeTimeValue(value, this.m_BlendOutDuration));
			}
		}

		public TimelineClip.BlendCurveMode blendInCurveMode
		{
			get
			{
				return this.m_BlendInCurveMode;
			}
			set
			{
				this.m_BlendInCurveMode = value;
			}
		}

		public TimelineClip.BlendCurveMode blendOutCurveMode
		{
			get
			{
				return this.m_BlendOutCurveMode;
			}
			set
			{
				this.m_BlendOutCurveMode = value;
			}
		}

		public bool hasBlendIn
		{
			get
			{
				return this.clipCaps.HasAny(ClipCaps.Blending) && this.m_BlendInDuration > 0.0;
			}
		}

		public bool hasBlendOut
		{
			get
			{
				return this.clipCaps.HasAny(ClipCaps.Blending) && this.m_BlendOutDuration > 0.0;
			}
		}

		public AnimationCurve mixInCurve
		{
			get
			{
				if (this.m_MixInCurve == null || this.m_MixInCurve.length < 2)
				{
					this.m_MixInCurve = TimelineClip.GetDefaultMixInCurve();
				}
				return this.m_MixInCurve;
			}
			set
			{
				this.m_MixInCurve = value;
			}
		}

		public float mixInPercentage
		{
			get
			{
				return (float)(this.mixInDuration / this.duration);
			}
		}

		public double mixInDuration
		{
			get
			{
				return (!this.hasBlendIn) ? this.easeInDuration : this.blendInDuration;
			}
		}

		public AnimationCurve mixOutCurve
		{
			get
			{
				if (this.m_MixOutCurve == null || this.m_MixOutCurve.length < 2)
				{
					this.m_MixOutCurve = TimelineClip.GetDefaultMixOutCurve();
				}
				return this.m_MixOutCurve;
			}
			set
			{
				this.m_MixOutCurve = value;
			}
		}

		public double mixOutTime
		{
			get
			{
				return this.duration - this.mixOutDuration + this.m_Start;
			}
		}

		public double mixOutDuration
		{
			get
			{
				return (!this.hasBlendOut) ? this.easeOutDuration : this.blendOutDuration;
			}
		}

		public float mixOutPercentage
		{
			get
			{
				return (float)(this.mixOutDuration / this.duration);
			}
		}

		public bool recordable
		{
			get
			{
				return this.m_Recordable;
			}
			internal set
			{
				this.m_Recordable = value;
			}
		}

		[Obsolete("exposedParameter is deprecated and will be removed in a future release")]
		public List<string> exposedParameters
		{
			get
			{
				List<string> list;
				if ((list = this.m_ExposedParameterNames) == null)
				{
					list = (this.m_ExposedParameterNames = new List<string>());
				}
				return list;
			}
		}

		public ClipCaps clipCaps
		{
			get
			{
				ITimelineClipAsset timelineClipAsset = this.asset as ITimelineClipAsset;
				return (timelineClipAsset == null) ? TimelineClip.kDefaultClipCaps : timelineClipAsset.clipCaps;
			}
		}

		internal int Hash()
		{
			int hashCode = this.m_Start.GetHashCode();
			int hashCode2 = this.m_Duration.GetHashCode();
			int hashCode3 = this.m_TimeScale.GetHashCode();
			int hashCode4 = this.m_ClipIn.GetHashCode();
			int preExtrapolationMode = (int)this.m_PreExtrapolationMode;
			int hashCode5 = preExtrapolationMode.GetHashCode();
			int postExtrapolationMode = (int)this.m_PostExtrapolationMode;
			return HashUtility.CombineHash(hashCode, hashCode2, hashCode3, hashCode4, hashCode5, postExtrapolationMode.GetHashCode());
		}

		public float EvaluateMixOut(double time)
		{
			float num;
			if (!this.clipCaps.HasAny(ClipCaps.Blending))
			{
				num = 1f;
			}
			else if (this.mixOutDuration > (double)Mathf.Epsilon)
			{
				float num2 = (float)(time - this.mixOutTime) / (float)this.mixOutDuration;
				num2 = Mathf.Clamp01(this.mixOutCurve.Evaluate(num2));
				num = num2;
			}
			else
			{
				num = 1f;
			}
			return num;
		}

		public float EvaluateMixIn(double time)
		{
			float num;
			if (!this.clipCaps.HasAny(ClipCaps.Blending))
			{
				num = 1f;
			}
			else if (this.mixInDuration > (double)Mathf.Epsilon)
			{
				float num2 = (float)(time - this.m_Start) / (float)this.mixInDuration;
				num2 = Mathf.Clamp01(this.mixInCurve.Evaluate(num2));
				num = num2;
			}
			else
			{
				num = 1f;
			}
			return num;
		}

		private static AnimationCurve GetDefaultMixInCurve()
		{
			return AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		}

		private static AnimationCurve GetDefaultMixOutCurve()
		{
			return AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
		}

		public double ToLocalTime(double time)
		{
			double num;
			if (time < 0.0)
			{
				num = time;
			}
			else
			{
				if (this.IsPreExtrapolatedTime(time))
				{
					time = TimelineClip.GetExtrapolatedTime(time - this.m_Start, this.m_PreExtrapolationMode, this.m_Duration);
				}
				else if (this.IsPostExtrapolatedTime(time))
				{
					time = TimelineClip.GetExtrapolatedTime(time - this.m_Start, this.m_PostExtrapolationMode, this.m_Duration);
				}
				else
				{
					time -= this.m_Start;
				}
				time *= this.timeScale;
				time += this.clipIn;
				num = time;
			}
			return num;
		}

		public double ToLocalTimeUnbound(double time)
		{
			return (time - this.m_Start) * this.timeScale + this.clipIn;
		}

		internal double FromLocalTimeUnbound(double time)
		{
			return (time - this.clipIn) / this.timeScale + this.m_Start;
		}

		public AnimationClip animationClip
		{
			get
			{
				AnimationClip animationClip;
				if (this.m_Asset == null)
				{
					animationClip = null;
				}
				else
				{
					AnimationPlayableAsset animationPlayableAsset = this.m_Asset as AnimationPlayableAsset;
					animationClip = ((!(animationPlayableAsset != null)) ? null : animationPlayableAsset.clip);
				}
				return animationClip;
			}
		}

		private static double SanitizeTimeValue(double value, double defaultValue)
		{
			double num;
			if (double.IsInfinity(value) || double.IsNaN(value))
			{
				Debug.LogError("Invalid time value assigned");
				num = defaultValue;
			}
			else
			{
				num = Math.Max(-TimelineClip.kMaxTimeValue, Math.Min(TimelineClip.kMaxTimeValue, value));
			}
			return num;
		}

		public TimelineClip.ClipExtrapolation postExtrapolationMode
		{
			get
			{
				return (!this.clipCaps.HasAny(ClipCaps.Extrapolation)) ? TimelineClip.ClipExtrapolation.None : this.m_PostExtrapolationMode;
			}
			internal set
			{
				this.m_PostExtrapolationMode = ((!this.clipCaps.HasAny(ClipCaps.Extrapolation)) ? TimelineClip.ClipExtrapolation.None : value);
			}
		}

		public TimelineClip.ClipExtrapolation preExtrapolationMode
		{
			get
			{
				return (!this.clipCaps.HasAny(ClipCaps.Extrapolation)) ? TimelineClip.ClipExtrapolation.None : this.m_PreExtrapolationMode;
			}
			internal set
			{
				this.m_PreExtrapolationMode = ((!this.clipCaps.HasAny(ClipCaps.Extrapolation)) ? TimelineClip.ClipExtrapolation.None : value);
			}
		}

		internal void SetPostExtrapolationTime(double time)
		{
			this.m_PostExtrapolationTime = time;
		}

		internal void SetPreExtrapolationTime(double time)
		{
			this.m_PreExtrapolationTime = time;
		}

		public bool IsExtrapolatedTime(double sequenceTime)
		{
			return this.IsPreExtrapolatedTime(sequenceTime) || this.IsPostExtrapolatedTime(sequenceTime);
		}

		public bool IsPreExtrapolatedTime(double sequenceTime)
		{
			return this.preExtrapolationMode != TimelineClip.ClipExtrapolation.None && sequenceTime < this.m_Start && sequenceTime >= this.m_Start - this.m_PreExtrapolationTime;
		}

		public bool IsPostExtrapolatedTime(double sequenceTime)
		{
			return this.postExtrapolationMode != TimelineClip.ClipExtrapolation.None && sequenceTime > this.end && sequenceTime - this.end < this.m_PostExtrapolationTime;
		}

		public double extrapolatedStart
		{
			get
			{
				double num;
				if (this.m_PreExtrapolationMode != TimelineClip.ClipExtrapolation.None)
				{
					num = this.m_Start - this.m_PreExtrapolationTime;
				}
				else
				{
					num = this.m_Start;
				}
				return num;
			}
		}

		public double extrapolatedDuration
		{
			get
			{
				double num = this.m_Duration;
				if (this.m_PostExtrapolationMode != TimelineClip.ClipExtrapolation.None)
				{
					num += Math.Min(this.m_PostExtrapolationTime, TimelineClip.kMaxTimeValue);
				}
				if (this.m_PreExtrapolationMode != TimelineClip.ClipExtrapolation.None)
				{
					num += this.m_PreExtrapolationTime;
				}
				return num;
			}
		}

		private static double GetExtrapolatedTime(double time, TimelineClip.ClipExtrapolation mode, double duration)
		{
			double num;
			if (duration == 0.0)
			{
				num = 0.0;
			}
			else
			{
				switch (mode)
				{
				case TimelineClip.ClipExtrapolation.Hold:
					if (time < 0.0)
					{
						return 0.0;
					}
					if (time > duration)
					{
						return duration;
					}
					break;
				case TimelineClip.ClipExtrapolation.Loop:
					if (time < 0.0)
					{
						time = duration - -time % duration;
					}
					else if (time > duration)
					{
						time %= duration;
					}
					break;
				case TimelineClip.ClipExtrapolation.PingPong:
					if (time < 0.0)
					{
						time = duration * 2.0 - -time % (duration * 2.0);
						time = duration - Math.Abs(time - duration);
					}
					else
					{
						time %= duration * 2.0;
						time = duration - Math.Abs(time - duration);
					}
					break;
				}
				num = time;
			}
			return num;
		}

		internal void AllocateAnimatedParameterCurves()
		{
			if (this.m_AnimationCurves == null)
			{
				this.m_AnimationCurves = new AnimationClip
				{
					legacy = true
				};
			}
		}

		internal void ClearAnimatedParameterCurves()
		{
			this.m_AnimationCurves = null;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_Version = 1;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this.m_Version < 1)
			{
				this.UpgradeToLatestVersion();
			}
		}

		public override string ToString()
		{
			return UnityString.Format("{0} ({1:F2}, {2:F2}):{3:F2} | {4}", new object[] { this.displayName, this.start, this.end, this.clipIn, this.parentTrack });
		}

		private void UpgradeToLatestVersion()
		{
			if (this.m_Version < 1)
			{
				TimelineClip.TimelineClipUpgrade.UpgradeClipInFromGlobalToLocal(this);
			}
		}

		public static readonly ClipCaps kDefaultClipCaps = ClipCaps.Blending;

		public static readonly float kDefaultClipDurationInSeconds = 5f;

		public static readonly double kTimeScaleMin = 0.001;

		public static readonly double kTimeScaleMax = 1000.0;

		internal static readonly double kMinDuration = 0.016666666666666666;

		internal static readonly double kMaxTimeValue = 1000000.0;

		[SerializeField]
		private double m_Start;

		[SerializeField]
		private double m_ClipIn;

		[SerializeField]
		private Object m_Asset;

		[SerializeField]
		[FormerlySerializedAs("m_HackDuration")]
		private double m_Duration;

		[SerializeField]
		private double m_TimeScale = 1.0;

		[SerializeField]
		private TrackAsset m_ParentTrack;

		[SerializeField]
		private double m_EaseInDuration;

		[SerializeField]
		private double m_EaseOutDuration;

		[SerializeField]
		private double m_BlendInDuration = -1.0;

		[SerializeField]
		private double m_BlendOutDuration = -1.0;

		[SerializeField]
		private AnimationCurve m_MixInCurve;

		[SerializeField]
		private AnimationCurve m_MixOutCurve;

		[SerializeField]
		private TimelineClip.BlendCurveMode m_BlendInCurveMode = TimelineClip.BlendCurveMode.Auto;

		[SerializeField]
		private TimelineClip.BlendCurveMode m_BlendOutCurveMode = TimelineClip.BlendCurveMode.Auto;

		[SerializeField]
		private List<string> m_ExposedParameterNames;

		[SerializeField]
		private AnimationClip m_AnimationCurves;

		[SerializeField]
		private bool m_Recordable;

		[SerializeField]
		private TimelineClip.ClipExtrapolation m_PostExtrapolationMode;

		[SerializeField]
		private TimelineClip.ClipExtrapolation m_PreExtrapolationMode;

		[SerializeField]
		private double m_PostExtrapolationTime;

		[SerializeField]
		private double m_PreExtrapolationTime;

		[SerializeField]
		private string m_DisplayName;

		private const int k_LatestVersion = 1;

		[SerializeField]
		[HideInInspector]
		private int m_Version;

		public enum ClipExtrapolation
		{
			None,
			Hold,
			Loop,
			PingPong,
			Continue
		}

		public enum BlendCurveMode
		{
			Auto,
			Manual
		}

		private enum Versions
		{
			Initial,
			ClipInFromGlobalToLocal
		}

		private static class TimelineClipUpgrade
		{
			public static void UpgradeClipInFromGlobalToLocal(TimelineClip clip)
			{
				if (clip.m_ClipIn > 0.0 && clip.m_TimeScale > 1.401298464324817E-45)
				{
					clip.m_ClipIn *= clip.m_TimeScale;
				}
			}
		}
	}
}
