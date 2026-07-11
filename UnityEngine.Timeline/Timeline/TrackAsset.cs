using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace UnityEngine.Timeline
{
	[IgnoreOnPlayableTrack]
	[Serializable]
	public abstract class TrackAsset : PlayableAsset, IPropertyPreview, ISerializationCallbackReceiver
	{
		public double start
		{
			get
			{
				this.UpdateDuration();
				return (double)this.m_Start;
			}
		}

		public double end
		{
			get
			{
				this.UpdateDuration();
				return (double)this.m_End;
			}
		}

		public sealed override double duration
		{
			get
			{
				this.UpdateDuration();
				return (double)(this.m_End - this.m_Start);
			}
		}

		public bool muted
		{
			get
			{
				return this.m_Muted;
			}
			set
			{
				this.m_Muted = value;
			}
		}

		public TimelineAsset timelineAsset
		{
			get
			{
				TrackAsset trackAsset = this;
				while (trackAsset != null)
				{
					TimelineAsset timelineAsset;
					if (trackAsset.parent == null)
					{
						timelineAsset = null;
					}
					else
					{
						TimelineAsset timelineAsset2 = trackAsset.parent as TimelineAsset;
						if (!(timelineAsset2 != null))
						{
							trackAsset = trackAsset.parent as TrackAsset;
							continue;
						}
						timelineAsset = timelineAsset2;
					}
					return timelineAsset;
				}
				return null;
			}
		}

		public PlayableAsset parent
		{
			get
			{
				return this.m_Parent;
			}
			internal set
			{
				this.m_Parent = value;
			}
		}

		public IEnumerable<TimelineClip> GetClips()
		{
			return this.clips;
		}

		internal TimelineClip[] clips
		{
			get
			{
				if (this.m_Clips == null)
				{
					this.m_Clips = new List<TimelineClip>();
				}
				if (this.m_ClipsCache == null)
				{
					this.m_CacheSorted = false;
					this.m_ClipsCache = this.m_Clips.ToArray();
				}
				return this.m_ClipsCache;
			}
		}

		public virtual bool isEmpty
		{
			get
			{
				return !this.hasClips;
			}
		}

		internal bool hasClips
		{
			get
			{
				return this.m_Clips != null && this.m_Clips.Count != 0;
			}
		}

		public bool isSubTrack
		{
			get
			{
				TrackAsset trackAsset = this.parent as TrackAsset;
				return trackAsset != null && trackAsset.GetType() == base.GetType();
			}
		}

		public override IEnumerable<PlayableBinding> outputs
		{
			get
			{
				TrackBindingTypeAttribute attribute;
				if (!TrackAsset.s_TrackBindingTypeAttributeCache.TryGetValue(base.GetType(), out attribute))
				{
					attribute = (TrackBindingTypeAttribute)Attribute.GetCustomAttribute(base.GetType(), typeof(TrackBindingTypeAttribute));
					TrackAsset.s_TrackBindingTypeAttributeCache.Add(base.GetType(), attribute);
				}
				Type trackBindingType = ((attribute == null) ? null : attribute.type);
				yield return ScriptPlayableBinding.Create(base.name, this, trackBindingType);
				yield break;
			}
		}

		public IEnumerable<TrackAsset> GetChildTracks()
		{
			this.UpdateChildTrackCache();
			return this.m_ChildTrackCache;
		}

		internal string customPlayableTypename
		{
			get
			{
				return this.m_CustomPlayableFullTypename;
			}
			set
			{
				this.m_CustomPlayableFullTypename = value;
			}
		}

		internal AnimationClip animClip
		{
			get
			{
				return this.m_AnimClip;
			}
			set
			{
				this.m_AnimClip = value;
			}
		}

		internal List<ScriptableObject> subTracksObjects
		{
			get
			{
				return this.m_Children;
			}
		}

		public bool locked
		{
			get
			{
				return this.m_Locked;
			}
			set
			{
				this.m_Locked = value;
			}
		}

		public bool lockedInHierarchy
		{
			get
			{
				return this.locked || this.parentLocked;
			}
		}

		internal bool parentLocked
		{
			get
			{
				TrackAsset trackAsset = this.parent as TrackAsset;
				return trackAsset != null && trackAsset.lockedInHierarchy;
			}
		}

		internal virtual bool compilable
		{
			get
			{
				return !this.muted && !this.isEmpty;
			}
		}

		private void __internalAwake()
		{
			if (this.m_Clips == null)
			{
				this.m_Clips = new List<TimelineClip>();
			}
			this.m_ChildTrackCache = null;
			if (this.m_Children == null)
			{
				this.m_Children = new List<ScriptableObject>();
			}
			for (int i = this.m_Children.Count - 1; i >= 0; i--)
			{
			}
		}

		public virtual Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			return Playable.Create(graph, inputCount);
		}

		public sealed override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			return Playable.Null;
		}

		public TimelineClip CreateDefaultClip()
		{
			object[] customAttributes = base.GetType().GetCustomAttributes(typeof(TrackClipTypeAttribute), true);
			Type type = null;
			foreach (object obj in customAttributes)
			{
				TrackClipTypeAttribute trackClipTypeAttribute = obj as TrackClipTypeAttribute;
				if (trackClipTypeAttribute != null && typeof(IPlayableAsset).IsAssignableFrom(trackClipTypeAttribute.inspectedType) && typeof(ScriptableObject).IsAssignableFrom(trackClipTypeAttribute.inspectedType))
				{
					type = trackClipTypeAttribute.inspectedType;
					break;
				}
			}
			TimelineClip timelineClip;
			if (type == null)
			{
				Debug.LogWarning("Cannot create a default clip for type " + base.GetType());
				timelineClip = null;
			}
			else
			{
				timelineClip = this.CreateAndAddNewClipOfType(type);
			}
			return timelineClip;
		}

		public TimelineClip CreateClip<T>() where T : ScriptableObject, IPlayableAsset
		{
			return this.CreateClip(typeof(T));
		}

		internal TimelineClip CreateClip(Type requestedType)
		{
			if (this.ValidateClipType(requestedType))
			{
				return this.CreateAndAddNewClipOfType(requestedType);
			}
			throw new InvalidOperationException(string.Concat(new object[]
			{
				"Clips of type ",
				requestedType,
				" are not permitted on tracks of type ",
				base.GetType()
			}));
		}

		internal TimelineClip CreateAndAddNewClipOfType(Type requestedType)
		{
			TimelineClip timelineClip = this.CreateClipOfType(requestedType);
			this.AddClip(timelineClip);
			return timelineClip;
		}

		internal TimelineClip CreateClipOfType(Type requestedType)
		{
			if (!this.ValidateClipType(requestedType))
			{
				throw new InvalidOperationException(string.Concat(new object[]
				{
					"Clips of type ",
					requestedType,
					" are not permitted on tracks of type ",
					base.GetType()
				}));
			}
			ScriptableObject scriptableObject = ScriptableObject.CreateInstance(requestedType);
			if (scriptableObject == null)
			{
				throw new InvalidOperationException("Could not create an instance of the ScriptableObject type " + requestedType.Name);
			}
			scriptableObject.name = requestedType.Name;
			TimelineCreateUtilities.SaveAssetIntoObject(scriptableObject, this);
			TimelineClip timelineClip = this.CreateNewClipContainerInternal();
			timelineClip.displayName = scriptableObject.name;
			timelineClip.asset = scriptableObject;
			IPlayableAsset playableAsset = scriptableObject as IPlayableAsset;
			if (playableAsset != null)
			{
				double duration = playableAsset.duration;
				if (!double.IsInfinity(duration) && duration >= TimelineClip.kMinDuration && duration < TimelineClip.kMaxTimeValue)
				{
					timelineClip.duration = duration;
				}
			}
			try
			{
				this.OnCreateClip(timelineClip);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message, scriptableObject);
				return null;
			}
			return timelineClip;
		}

		internal void AddClip(TimelineClip newClip)
		{
			if (!this.m_Clips.Contains(newClip))
			{
				this.m_Clips.Add(newClip);
				this.m_ClipsCache = null;
			}
		}

		internal Playable CreatePlayableGraph(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree)
		{
			this.UpdateDuration();
			return this.OnCreatePlayableGraph(graph, go, tree);
		}

		internal virtual Playable OnCreatePlayableGraph(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree)
		{
			if (tree == null)
			{
				throw new ArgumentException("IntervalTree argument cannot be null", "tree");
			}
			if (go == null)
			{
				throw new ArgumentException("GameObject argument cannot be null", "go");
			}
			Playable playable = this.CreateTrackMixer(graph, go, this.clips.Length);
			for (int i = 0; i < this.clips.Length; i++)
			{
				Playable playable2 = this.CreatePlayable(graph, go, this.clips[i]);
				if (playable2.IsValid<Playable>())
				{
					playable2.SetDuration(this.clips[i].duration);
					RuntimeClip runtimeClip = new RuntimeClip(this.clips[i], playable2, playable);
					tree.Add(runtimeClip);
					graph.Connect<Playable, Playable>(playable2, 0, playable, i);
					playable.SetInputWeight(i, 0f);
				}
			}
			return playable;
		}

		internal void SortClips()
		{
			TimelineClip[] clips = this.clips;
			if (!this.m_CacheSorted)
			{
				Array.Sort<TimelineClip>(this.clips, (TimelineClip clip1, TimelineClip clip2) => clip1.start.CompareTo(clip2.start));
				this.m_CacheSorted = true;
			}
		}

		internal void ClearClipsInternal()
		{
			this.m_Clips = new List<TimelineClip>();
			this.m_ClipsCache = null;
		}

		internal void ClearSubTracksInternal()
		{
			this.m_Children = new List<ScriptableObject>();
			this.Invalidate();
		}

		internal void OnClipMove()
		{
			this.m_CacheSorted = false;
		}

		internal TimelineClip CreateNewClipContainerInternal()
		{
			TimelineClip timelineClip = new TimelineClip(this);
			timelineClip.asset = null;
			double num = 0.0;
			for (int i = 0; i < this.m_Clips.Count - 1; i++)
			{
				double num2 = this.m_Clips[i].duration;
				if (double.IsInfinity(num2))
				{
					num2 = (double)TimelineClip.kDefaultClipDurationInSeconds;
				}
				num = Math.Max(num, this.m_Clips[i].start + num2);
			}
			timelineClip.mixInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
			timelineClip.mixOutCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
			timelineClip.start = num;
			timelineClip.duration = (double)TimelineClip.kDefaultClipDurationInSeconds;
			timelineClip.displayName = "untitled";
			return timelineClip;
		}

		internal void AddChild(TrackAsset child)
		{
			if (!(child == null))
			{
				this.m_Children.Add(child);
				child.parent = this;
				this.Invalidate();
			}
		}

		internal bool AddChildAfter(TrackAsset child, TrackAsset other)
		{
			bool flag;
			if (child == null)
			{
				flag = false;
			}
			else
			{
				int num = this.m_Children.IndexOf(other);
				if (num >= 0 && num != this.m_Children.Count - 1)
				{
					this.m_Children.Insert(num + 1, child);
				}
				else
				{
					this.m_Children.Add(child);
				}
				this.Invalidate();
				child.parent = this;
				flag = true;
			}
			return flag;
		}

		internal bool RemoveSubTrack(TrackAsset child)
		{
			bool flag;
			if (this.m_Children.Remove(child))
			{
				this.Invalidate();
				child.parent = null;
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		internal void RemoveClip(TimelineClip clip)
		{
			this.m_Clips.Remove(clip);
			this.m_ClipsCache = null;
		}

		internal virtual void GetEvaluationTime(out double outStart, out double outDuration)
		{
			if (this.clips.Length == 0)
			{
				outStart = 0.0;
				outDuration = this.GetMarkerDuration();
			}
			else
			{
				outStart = double.MaxValue;
				double num = 0.0;
				for (int i = 0; i < this.clips.Length; i++)
				{
					outStart = Math.Min(this.clips[i].start, outStart);
					num = Math.Max(this.clips[i].start + this.clips[i].duration, num);
				}
				outStart = Math.Max(outStart, 0.0);
				outDuration = Math.Max(this.GetMarkerDuration(), num - outStart);
			}
		}

		internal virtual void GetSequenceTime(out double outStart, out double outDuration)
		{
			this.GetEvaluationTime(out outStart, out outDuration);
		}

		public virtual void GatherProperties(PlayableDirector director, IPropertyCollector driver)
		{
			GameObject gameObjectBinding = this.GetGameObjectBinding(director);
			if (gameObjectBinding != null)
			{
				driver.PushActiveGameObject(gameObjectBinding);
			}
			if (this.animClip != null)
			{
				driver.AddFromClip(this.animClip);
			}
			foreach (TimelineClip timelineClip in this.clips)
			{
				if (timelineClip.curves != null && timelineClip.asset != null)
				{
					driver.AddObjectProperties(timelineClip.asset, timelineClip.curves);
				}
				IPropertyPreview propertyPreview = timelineClip.asset as IPropertyPreview;
				if (propertyPreview != null)
				{
					propertyPreview.GatherProperties(director, driver);
				}
			}
			foreach (TrackAsset trackAsset in this.GetChildTracks())
			{
				if (trackAsset != null)
				{
					trackAsset.GatherProperties(director, driver);
				}
			}
			if (gameObjectBinding != null)
			{
				driver.PopActiveGameObject();
			}
		}

		internal GameObject GetGameObjectBinding(PlayableDirector director)
		{
			GameObject gameObject;
			if (director == null)
			{
				gameObject = null;
			}
			else
			{
				Object genericBinding = director.GetGenericBinding(this);
				GameObject gameObject2 = genericBinding as GameObject;
				if (gameObject2 != null)
				{
					gameObject = gameObject2;
				}
				else
				{
					Component component = genericBinding as Component;
					if (component != null)
					{
						gameObject = component.gameObject;
					}
					else
					{
						gameObject = null;
					}
				}
			}
			return gameObject;
		}

		internal bool ValidateClipType(Type clipType)
		{
			foreach (TrackClipTypeAttribute trackClipTypeAttribute in base.GetType().GetCustomAttributes(typeof(TrackClipTypeAttribute), true))
			{
				if (trackClipTypeAttribute.inspectedType.IsAssignableFrom(clipType))
				{
					return true;
				}
			}
			return typeof(PlayableTrack).IsAssignableFrom(base.GetType()) && typeof(IPlayableAsset).IsAssignableFrom(clipType) && typeof(ScriptableObject).IsAssignableFrom(clipType);
		}

		protected virtual void OnCreateClip(TimelineClip clip)
		{
		}

		protected internal virtual void UpdateDuration()
		{
			int num = ((!(this.m_AnimClip != null)) ? 0 : ((int)(this.m_AnimClip.frameRate * this.m_AnimClip.length)));
			int num2 = HashUtility.CombineHash(this.GetClipsHash(), this.GetMarkerHash(), num);
			if (num2 != this.m_ItemsHash)
			{
				this.m_ItemsHash = num2;
				double num3;
				double num4;
				this.GetSequenceTime(out num3, out num4);
				this.m_Start = (DiscreteTime)num3;
				this.m_End = (DiscreteTime)(num3 + num4);
				this.CalculateExtrapolationTimes();
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal static event Action<TimelineClip, GameObject, Playable> OnPlayableCreate;

		protected internal Playable CreatePlayable(PlayableGraph graph, GameObject go, TimelineClip clip)
		{
			IPlayableAsset playableAsset = clip.asset as IPlayableAsset;
			Playable playable2;
			if (playableAsset != null)
			{
				Playable playable = playableAsset.CreatePlayable(graph, go);
				if (playable.IsValid<Playable>())
				{
					playable.SetAnimatedProperties(clip.curves);
					playable.SetSpeed(clip.timeScale);
					if (TrackAsset.OnPlayableCreate != null)
					{
						TrackAsset.OnPlayableCreate(clip, go, playable);
					}
				}
				playable2 = playable;
			}
			else
			{
				playable2 = Playable.Null;
			}
			return playable2;
		}

		internal void Invalidate()
		{
			this.m_ChildTrackCache = null;
			TimelineAsset timelineAsset = this.timelineAsset;
			if (timelineAsset != null)
			{
				timelineAsset.Invalidate();
			}
		}

		private void UpdateChildTrackCache()
		{
			if (this.m_ChildTrackCache == null)
			{
				if (this.m_Children == null || this.m_Children.Count == 0)
				{
					this.m_ChildTrackCache = TrackAsset.s_EmptyCache;
				}
				else
				{
					List<TrackAsset> list = new List<TrackAsset>(this.m_Children.Count);
					for (int i = 0; i < this.m_Children.Count; i++)
					{
						TrackAsset trackAsset = this.m_Children[i] as TrackAsset;
						if (trackAsset != null)
						{
							list.Add(trackAsset);
						}
					}
					this.m_ChildTrackCache = list;
				}
			}
		}

		protected internal virtual int Hash()
		{
			return this.clips.Length + (this.GetMarkerContainerHash() << 16);
		}

		private int GetClipsHash()
		{
			int num = 0;
			foreach (ITimelineItem timelineItem in this.m_Clips)
			{
				num = num.CombineHash(timelineItem.Hash());
			}
			return num;
		}

		private int GetMarkerContainerHash()
		{
			ITimelineMarkerContainer timelineMarkerContainer = this as ITimelineMarkerContainer;
			int num;
			if (timelineMarkerContainer == null)
			{
				num = 0;
			}
			else
			{
				TimelineMarker[] markers = timelineMarkerContainer.GetMarkers();
				num = ((markers != null) ? markers.Length : 0);
			}
			return num;
		}

		private int GetMarkerHash()
		{
			ITimelineMarkerContainer timelineMarkerContainer = this as ITimelineMarkerContainer;
			int num = 0;
			if (timelineMarkerContainer != null)
			{
				TimelineMarker[] markers = timelineMarkerContainer.GetMarkers();
				if (markers != null)
				{
					for (int i = 0; i < markers.Length; i++)
					{
						num = num.CombineHash(((ITimelineItem)markers[i]).Hash());
					}
				}
			}
			return num;
		}

		private double GetMarkerDuration()
		{
			ITimelineMarkerContainer timelineMarkerContainer = this as ITimelineMarkerContainer;
			double num = 0.0;
			if (timelineMarkerContainer != null)
			{
				TimelineMarker[] markers = timelineMarkerContainer.GetMarkers();
				if (markers != null)
				{
					for (int i = 0; i < markers.Length; i++)
					{
						num = Math.Max(num, markers[i].time);
					}
				}
			}
			return num;
		}

		protected virtual void OnBeforeTrackSerialize()
		{
		}

		protected virtual void OnAfterTrackDeserialize()
		{
		}

		protected internal virtual void OnUpgradeFromVersion(int oldVersion)
		{
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_Version = 1;
			for (int i = this.m_Children.Count - 1; i >= 0; i--)
			{
				TrackAsset trackAsset = this.m_Children[i] as TrackAsset;
				if (trackAsset != null && trackAsset.parent != this)
				{
					trackAsset.parent = this;
				}
			}
			this.OnBeforeTrackSerialize();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.m_ClipsCache = null;
			this.Invalidate();
			if (this.m_Version < 1)
			{
				this.UpgradeToLatestVersion();
				this.OnUpgradeFromVersion(this.m_Version);
			}
			this.OnAfterTrackDeserialize();
		}

		private void UpgradeToLatestVersion()
		{
		}

		[SerializeField]
		[HideInInspector]
		private bool m_Locked;

		[SerializeField]
		[HideInInspector]
		private bool m_Muted;

		[SerializeField]
		[HideInInspector]
		private string m_CustomPlayableFullTypename = string.Empty;

		[SerializeField]
		[FormerlySerializedAs("m_animClip")]
		[HideInInspector]
		private AnimationClip m_AnimClip;

		[SerializeField]
		[HideInInspector]
		private PlayableAsset m_Parent;

		[SerializeField]
		[HideInInspector]
		private List<ScriptableObject> m_Children;

		[NonSerialized]
		private int m_ItemsHash;

		[NonSerialized]
		private TimelineClip[] m_ClipsCache;

		private DiscreteTime m_Start;

		private DiscreteTime m_End;

		private bool m_CacheSorted;

		private static TrackAsset[] s_EmptyCache = new TrackAsset[0];

		private IEnumerable<TrackAsset> m_ChildTrackCache;

		private static Dictionary<Type, TrackBindingTypeAttribute> s_TrackBindingTypeAttributeCache = new Dictionary<Type, TrackBindingTypeAttribute>();

		[SerializeField]
		[HideInInspector]
		protected internal List<TimelineClip> m_Clips = new List<TimelineClip>();

		protected internal const int k_LatestVersion = 1;

		[SerializeField]
		[HideInInspector]
		private int m_Version;

		protected internal enum Versions
		{
			Initial,
			RotationAsEuler
		}

		private static class TrackAssetUpgrade
		{
		}
	}
}
