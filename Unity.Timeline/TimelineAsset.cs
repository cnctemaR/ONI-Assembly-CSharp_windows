using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	[Serializable]
	public class TimelineAsset : PlayableAsset, ISerializationCallbackReceiver, ITimelineClipAsset, IPropertyPreview
	{
		private void UpgradeToLatestVersion()
		{
		}

		public TimelineAsset.EditorSettings editorSettings
		{
			get
			{
				return this.m_EditorSettings;
			}
		}

		public override double duration
		{
			get
			{
				if (this.m_DurationMode == TimelineAsset.DurationMode.BasedOnClips)
				{
					return this.CalculateDuration();
				}
				return this.m_FixedDuration;
			}
		}

		public double fixedDuration
		{
			get
			{
				DiscreteTime discreteTime = (DiscreteTime)this.m_FixedDuration;
				if (discreteTime <= 0)
				{
					return 0.0;
				}
				return (double)discreteTime.OneTickBefore();
			}
			set
			{
				this.m_FixedDuration = Math.Max(0.0, value);
			}
		}

		public TimelineAsset.DurationMode durationMode
		{
			get
			{
				return this.m_DurationMode;
			}
			set
			{
				this.m_DurationMode = value;
			}
		}

		public override IEnumerable<PlayableBinding> outputs
		{
			get
			{
				foreach (TrackAsset trackAsset in this.GetOutputTracks())
				{
					foreach (PlayableBinding playableBinding in trackAsset.outputs)
					{
						yield return playableBinding;
					}
					IEnumerator<PlayableBinding> enumerator2 = null;
				}
				IEnumerator<TrackAsset> enumerator = null;
				yield break;
				yield break;
			}
		}

		public ClipCaps clipCaps
		{
			get
			{
				ClipCaps clipCaps = ClipCaps.All;
				foreach (TrackAsset trackAsset in this.GetRootTracks())
				{
					foreach (TimelineClip timelineClip in trackAsset.clips)
					{
						clipCaps &= timelineClip.clipCaps;
					}
				}
				return clipCaps;
			}
		}

		public int outputTrackCount
		{
			get
			{
				this.UpdateOutputTrackCache();
				return this.m_CacheOutputTracks.Length;
			}
		}

		public int rootTrackCount
		{
			get
			{
				this.UpdateRootTrackCache();
				return this.m_CacheRootTracks.Count;
			}
		}

		private void OnValidate()
		{
			this.editorSettings.fps = TimelineAsset.GetValidFramerate(this.editorSettings.fps);
		}

		private static float GetValidFramerate(float framerate)
		{
			return Mathf.Clamp(framerate, TimelineAsset.EditorSettings.kMinFps, TimelineAsset.EditorSettings.kMaxFps);
		}

		public TrackAsset GetRootTrack(int index)
		{
			this.UpdateRootTrackCache();
			return this.m_CacheRootTracks[index];
		}

		public IEnumerable<TrackAsset> GetRootTracks()
		{
			this.UpdateRootTrackCache();
			return this.m_CacheRootTracks;
		}

		public TrackAsset GetOutputTrack(int index)
		{
			this.UpdateOutputTrackCache();
			return this.m_CacheOutputTracks[index];
		}

		public IEnumerable<TrackAsset> GetOutputTracks()
		{
			this.UpdateOutputTrackCache();
			return this.m_CacheOutputTracks;
		}

		private void UpdateRootTrackCache()
		{
			if (this.m_CacheRootTracks == null)
			{
				if (this.m_Tracks == null)
				{
					this.m_CacheRootTracks = new List<TrackAsset>();
					return;
				}
				this.m_CacheRootTracks = new List<TrackAsset>(this.m_Tracks.Count);
				if (this.markerTrack != null)
				{
					this.m_CacheRootTracks.Add(this.markerTrack);
				}
				foreach (ScriptableObject scriptableObject in this.m_Tracks)
				{
					TrackAsset trackAsset = scriptableObject as TrackAsset;
					if (trackAsset != null)
					{
						this.m_CacheRootTracks.Add(trackAsset);
					}
				}
			}
		}

		private void UpdateOutputTrackCache()
		{
			if (this.m_CacheOutputTracks == null)
			{
				List<TrackAsset> list = new List<TrackAsset>();
				foreach (TrackAsset trackAsset in this.flattenedTracks)
				{
					if (trackAsset != null && trackAsset.GetType() != typeof(GroupTrack) && !trackAsset.isSubTrack)
					{
						list.Add(trackAsset);
					}
				}
				this.m_CacheOutputTracks = list.ToArray();
			}
		}

		internal IEnumerable<TrackAsset> flattenedTracks
		{
			get
			{
				if (this.m_CacheFlattenedTracks == null)
				{
					this.m_CacheFlattenedTracks = new List<TrackAsset>(this.m_Tracks.Count * 2);
					this.UpdateRootTrackCache();
					this.m_CacheFlattenedTracks.AddRange(this.m_CacheRootTracks);
					for (int i = 0; i < this.m_CacheRootTracks.Count; i++)
					{
						TimelineAsset.AddSubTracksRecursive(this.m_CacheRootTracks[i], ref this.m_CacheFlattenedTracks);
					}
				}
				return this.m_CacheFlattenedTracks;
			}
		}

		public MarkerTrack markerTrack
		{
			get
			{
				return this.m_MarkerTrack;
			}
		}

		internal List<ScriptableObject> trackObjects
		{
			get
			{
				return this.m_Tracks;
			}
		}

		internal void AddTrackInternal(TrackAsset track)
		{
			this.m_Tracks.Add(track);
			track.parent = this;
			this.Invalidate();
		}

		internal void RemoveTrack(TrackAsset track)
		{
			this.m_Tracks.Remove(track);
			this.Invalidate();
			TrackAsset trackAsset = track.parent as TrackAsset;
			if (trackAsset != null)
			{
				trackAsset.RemoveSubTrack(track);
			}
		}

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			bool flag = false;
			bool flag2 = graph.GetPlayableCount() == 0;
			ScriptPlayable<TimelinePlayable> scriptPlayable = TimelinePlayable.Create(graph, this.GetOutputTracks(), go, flag, flag2);
			scriptPlayable.SetPropagateSetTime(true);
			if (!scriptPlayable.IsValid<ScriptPlayable<TimelinePlayable>>())
			{
				return Playable.Null;
			}
			return scriptPlayable;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_Version = 0;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.Invalidate();
			if (this.m_Version < 0)
			{
				this.UpgradeToLatestVersion();
			}
		}

		private void __internalAwake()
		{
			if (this.m_Tracks == null)
			{
				this.m_Tracks = new List<ScriptableObject>();
			}
			for (int i = this.m_Tracks.Count - 1; i >= 0; i--)
			{
				TrackAsset trackAsset = this.m_Tracks[i] as TrackAsset;
				if (trackAsset != null)
				{
					trackAsset.parent = this;
				}
			}
		}

		public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
		{
			foreach (TrackAsset trackAsset in this.GetOutputTracks())
			{
				trackAsset.GatherProperties(director, driver);
			}
		}

		public void CreateMarkerTrack()
		{
			if (this.m_MarkerTrack == null)
			{
				this.m_MarkerTrack = ScriptableObject.CreateInstance<MarkerTrack>();
				TimelineCreateUtilities.SaveAssetIntoObject(this.m_MarkerTrack, this);
				this.m_MarkerTrack.parent = this;
				this.m_MarkerTrack.name = "Markers";
				this.Invalidate();
			}
		}

		internal void Invalidate()
		{
			this.m_CacheRootTracks = null;
			this.m_CacheOutputTracks = null;
			this.m_CacheFlattenedTracks = null;
		}

		private double CalculateDuration()
		{
			DiscreteTime discreteTime = new DiscreteTime(0);
			foreach (TrackAsset trackAsset in this.flattenedTracks)
			{
				if (!trackAsset.muted)
				{
					discreteTime = DiscreteTime.Max(discreteTime, (DiscreteTime)trackAsset.end);
				}
			}
			if (discreteTime <= 0)
			{
				return 0.0;
			}
			return (double)discreteTime.OneTickBefore();
		}

		private static void AddSubTracksRecursive(TrackAsset track, ref List<TrackAsset> allTracks)
		{
			if (track == null)
			{
				return;
			}
			allTracks.AddRange(track.GetChildTracks());
			foreach (TrackAsset trackAsset in track.GetChildTracks())
			{
				TimelineAsset.AddSubTracksRecursive(trackAsset, ref allTracks);
			}
		}

		public TrackAsset CreateTrack(Type type, TrackAsset parent, string name)
		{
			if (parent != null && parent.timelineAsset != this)
			{
				throw new InvalidOperationException("Addtrack cannot parent to a track not in the Timeline");
			}
			if (!typeof(TrackAsset).IsAssignableFrom(type))
			{
				throw new InvalidOperationException("Supplied type must be a track asset");
			}
			if (parent != null && !TimelineCreateUtilities.ValidateParentTrack(parent, type))
			{
				throw new InvalidOperationException("Cannot assign a child of type " + type.Name + " to a parent of type " + parent.GetType().Name);
			}
			PlayableAsset playableAsset = ((parent != null) ? parent : this);
			string text = name;
			if (string.IsNullOrEmpty(text))
			{
				text = type.Name;
			}
			string text2;
			if (parent != null)
			{
				text2 = TimelineCreateUtilities.GenerateUniqueActorName(parent.subTracksObjects, text);
			}
			else
			{
				text2 = TimelineCreateUtilities.GenerateUniqueActorName(this.trackObjects, text);
			}
			TrackAsset trackAsset = this.AllocateTrack(parent, text2, type);
			if (trackAsset != null)
			{
				trackAsset.name = text2;
				TimelineCreateUtilities.SaveAssetIntoObject(trackAsset, playableAsset);
			}
			return trackAsset;
		}

		public T CreateTrack<T>(TrackAsset parent, string trackName) where T : TrackAsset, new()
		{
			return (T)((object)this.CreateTrack(typeof(T), parent, trackName));
		}

		public T CreateTrack<T>(string trackName) where T : TrackAsset, new()
		{
			return (T)((object)this.CreateTrack(typeof(T), null, trackName));
		}

		public T CreateTrack<T>() where T : TrackAsset, new()
		{
			return (T)((object)this.CreateTrack(typeof(T), null, null));
		}

		public bool DeleteClip(TimelineClip clip)
		{
			if (clip == null || clip.parentTrack == null)
			{
				return false;
			}
			if (this != clip.parentTrack.timelineAsset)
			{
				Debug.LogError("Cannot delete a clip from this timeline");
				return false;
			}
			if (clip.curves != null)
			{
				TimelineUndo.PushDestroyUndo(this, clip.parentTrack, clip.curves, "Delete Curves");
			}
			if (clip.asset != null)
			{
				this.DeleteRecordedAnimation(clip);
				TimelineUndo.PushDestroyUndo(this, clip.parentTrack, clip.asset, "Delete Clip Asset");
			}
			TrackAsset parentTrack = clip.parentTrack;
			parentTrack.RemoveClip(clip);
			parentTrack.CalculateExtrapolationTimes();
			return true;
		}

		public bool DeleteTrack(TrackAsset track)
		{
			if (track.timelineAsset != this)
			{
				return false;
			}
			track.parent as TrackAsset != null;
			foreach (TrackAsset trackAsset in track.GetChildTracks())
			{
				this.DeleteTrack(trackAsset);
			}
			this.DeleteRecordedAnimation(track);
			foreach (TimelineClip timelineClip in new List<TimelineClip>(track.clips))
			{
				this.DeleteClip(timelineClip);
			}
			this.RemoveTrack(track);
			TimelineUndo.PushDestroyUndo(this, this, track, "Delete Track");
			return true;
		}

		internal void MoveLastTrackBefore(TrackAsset asset)
		{
			if (this.m_Tracks == null || this.m_Tracks.Count < 2 || asset == null)
			{
				return;
			}
			ScriptableObject scriptableObject = this.m_Tracks[this.m_Tracks.Count - 1];
			if (scriptableObject == asset)
			{
				return;
			}
			for (int i = 0; i < this.m_Tracks.Count - 1; i++)
			{
				if (this.m_Tracks[i] == asset)
				{
					for (int j = this.m_Tracks.Count - 1; j > i; j--)
					{
						this.m_Tracks[j] = this.m_Tracks[j - 1];
					}
					this.m_Tracks[i] = scriptableObject;
					this.Invalidate();
					return;
				}
			}
		}

		internal TrackAsset AllocateTrack(TrackAsset trackAssetParent, string trackName, Type trackType)
		{
			if (trackAssetParent != null && trackAssetParent.timelineAsset != this)
			{
				throw new InvalidOperationException("Addtrack cannot parent to a track not in the Timeline");
			}
			if (!typeof(TrackAsset).IsAssignableFrom(trackType))
			{
				throw new InvalidOperationException("Supplied type must be a track asset");
			}
			TrackAsset trackAsset = (TrackAsset)ScriptableObject.CreateInstance(trackType);
			trackAsset.name = trackName;
			if (trackAssetParent != null)
			{
				trackAssetParent.AddChild(trackAsset);
			}
			else
			{
				this.AddTrackInternal(trackAsset);
			}
			return trackAsset;
		}

		private void DeleteRecordedAnimation(TrackAsset track)
		{
			AnimationTrack animationTrack = track as AnimationTrack;
			if (animationTrack != null && animationTrack.infiniteClip != null)
			{
				TimelineUndo.PushDestroyUndo(this, track, animationTrack.infiniteClip, "Delete Track");
			}
			if (track.curves != null)
			{
				TimelineUndo.PushDestroyUndo(this, track, track.curves, "Delete Track Parameters");
			}
		}

		private void DeleteRecordedAnimation(TimelineClip clip)
		{
			if (clip == null)
			{
				return;
			}
			if (clip.curves != null)
			{
				TimelineUndo.PushDestroyUndo(this, clip.parentTrack, clip.curves, "Delete Clip Parameters");
			}
			if (!clip.recordable)
			{
				return;
			}
			AnimationPlayableAsset animationPlayableAsset = clip.asset as AnimationPlayableAsset;
			if (animationPlayableAsset == null || animationPlayableAsset.clip == null)
			{
				return;
			}
			TimelineUndo.PushDestroyUndo(this, animationPlayableAsset, animationPlayableAsset.clip, "Delete Recording");
		}

		private const int k_LatestVersion = 0;

		[SerializeField]
		[HideInInspector]
		private int m_Version;

		[HideInInspector]
		[SerializeField]
		private List<ScriptableObject> m_Tracks;

		[HideInInspector]
		[SerializeField]
		private double m_FixedDuration;

		[HideInInspector]
		[NonSerialized]
		private TrackAsset[] m_CacheOutputTracks;

		[HideInInspector]
		[NonSerialized]
		private List<TrackAsset> m_CacheRootTracks;

		[HideInInspector]
		[NonSerialized]
		private List<TrackAsset> m_CacheFlattenedTracks;

		[HideInInspector]
		[SerializeField]
		private TimelineAsset.EditorSettings m_EditorSettings = new TimelineAsset.EditorSettings();

		[SerializeField]
		private TimelineAsset.DurationMode m_DurationMode;

		[HideInInspector]
		[SerializeField]
		private MarkerTrack m_MarkerTrack;

		private enum Versions
		{
			Initial
		}

		private static class TimelineAssetUpgrade
		{
		}

		[Obsolete("MediaType has been deprecated. It is no longer required, and will be removed in a future release.", false)]
		public enum MediaType
		{
			Animation,
			Audio,
			Texture,
			[Obsolete("Use Texture MediaType instead. (UnityUpgradable) -> UnityEngine.Timeline.TimelineAsset/MediaType.Texture", false)]
			Video = 2,
			Script,
			Hybrid,
			Group
		}

		public enum DurationMode
		{
			BasedOnClips,
			FixedLength
		}

		[Serializable]
		public class EditorSettings
		{
			public float fps
			{
				get
				{
					return this.m_Framerate;
				}
				set
				{
					this.m_Framerate = TimelineAsset.GetValidFramerate(value);
				}
			}

			internal static readonly float kMinFps = (float)TimeUtility.kFrameRateEpsilon;

			internal static readonly float kMaxFps = 1000f;

			internal static readonly float kDefaultFps = 60f;

			[HideInInspector]
			[SerializeField]
			private float m_Framerate = TimelineAsset.EditorSettings.kDefaultFps;
		}
	}
}
