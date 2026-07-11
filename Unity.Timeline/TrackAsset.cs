using System;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace UnityEngine.Timeline
{
	[IgnoreOnPlayableTrack]
	[Serializable]
	public abstract class TrackAsset : PlayableAsset, ISerializationCallbackReceiver, IPropertyPreview, ICurvesOwner
	{
		protected virtual void OnBeforeTrackSerialize()
		{
		}

		protected virtual void OnAfterTrackDeserialize()
		{
		}

		internal virtual void OnUpgradeFromVersion(int oldVersion)
		{
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_Version = 3;
			if (this.m_Children != null)
			{
				for (int i = this.m_Children.Count - 1; i >= 0; i--)
				{
					TrackAsset trackAsset = this.m_Children[i] as TrackAsset;
					if (trackAsset != null && trackAsset.parent != this)
					{
						trackAsset.parent = this;
					}
				}
			}
			this.OnBeforeTrackSerialize();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.m_ClipsCache = null;
			this.Invalidate();
			if (this.m_Version < 3)
			{
				this.UpgradeToLatestVersion();
				this.OnUpgradeFromVersion(this.m_Version);
			}
			foreach (IMarker marker in this.GetMarkers())
			{
				marker.Initialize(this);
			}
			this.OnAfterTrackDeserialize();
		}

		private void UpgradeToLatestVersion()
		{
		}

		internal static event Action<TimelineClip, GameObject, Playable> OnClipPlayableCreate;

		internal static event Action<TrackAsset, GameObject, Playable> OnTrackAnimationPlayableCreate;

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

		public bool mutedInHierarchy
		{
			get
			{
				if (this.muted)
				{
					return true;
				}
				TrackAsset trackAsset = this;
				while (trackAsset.parent as TrackAsset != null)
				{
					trackAsset = (TrackAsset)trackAsset.parent;
					if (trackAsset as GroupTrack != null)
					{
						return trackAsset.mutedInHierarchy;
					}
				}
				return false;
			}
		}

		public TimelineAsset timelineAsset
		{
			get
			{
				TrackAsset trackAsset = this;
				while (trackAsset != null)
				{
					if (trackAsset.parent == null)
					{
						return null;
					}
					TimelineAsset timelineAsset = trackAsset.parent as TimelineAsset;
					if (timelineAsset != null)
					{
						return timelineAsset;
					}
					trackAsset = trackAsset.parent as TrackAsset;
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
				return !this.hasClips && !this.hasCurves && this.GetMarkerCount() == 0;
			}
		}

		public bool hasClips
		{
			get
			{
				return this.m_Clips != null && this.m_Clips.Count != 0;
			}
		}

		public bool hasCurves
		{
			get
			{
				return this.m_Curves != null && !this.m_Curves.empty;
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
				TrackBindingTypeAttribute trackBindingTypeAttribute;
				if (!TrackAsset.s_TrackBindingTypeAttributeCache.TryGetValue(base.GetType(), out trackBindingTypeAttribute))
				{
					trackBindingTypeAttribute = (TrackBindingTypeAttribute)Attribute.GetCustomAttribute(base.GetType(), typeof(TrackBindingTypeAttribute));
					TrackAsset.s_TrackBindingTypeAttributeCache.Add(base.GetType(), trackBindingTypeAttribute);
				}
				Type type = ((trackBindingTypeAttribute != null) ? trackBindingTypeAttribute.type : null);
				yield return ScriptPlayableBinding.Create(base.name, this, type);
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

		public AnimationClip curves
		{
			get
			{
				return this.m_Curves;
			}
			internal set
			{
				this.m_Curves = value;
			}
		}

		string ICurvesOwner.defaultCurvesName
		{
			get
			{
				return "Track Parameters";
			}
		}

		Object ICurvesOwner.asset
		{
			get
			{
				return this;
			}
		}

		Object ICurvesOwner.assetOwner
		{
			get
			{
				return this.timelineAsset;
			}
		}

		TrackAsset ICurvesOwner.targetTrack
		{
			get
			{
				return this;
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
				if (this.locked)
				{
					return true;
				}
				TrackAsset trackAsset = this;
				while (trackAsset.parent as TrackAsset != null)
				{
					trackAsset = (TrackAsset)trackAsset.parent;
					if (trackAsset as GroupTrack != null)
					{
						return trackAsset.lockedInHierarchy;
					}
				}
				return false;
			}
		}

		public bool supportsNotifications
		{
			get
			{
				if (this.m_SupportsNotifications == null)
				{
					this.m_SupportsNotifications = new bool?(NotificationUtilities.TrackTypeSupportsNotifications(base.GetType()));
				}
				return this.m_SupportsNotifications.Value;
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
		}

		public void CreateCurves(string curvesClipName)
		{
			if (this.m_Curves != null)
			{
				return;
			}
			this.m_Curves = TimelineCreateUtilities.CreateAnimationClipForTrack(string.IsNullOrEmpty(curvesClipName) ? "Track Parameters" : curvesClipName, this, true);
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
			object[] array = customAttributes;
			for (int i = 0; i < array.Length; i++)
			{
				TrackClipTypeAttribute trackClipTypeAttribute = array[i] as TrackClipTypeAttribute;
				if (trackClipTypeAttribute != null && typeof(IPlayableAsset).IsAssignableFrom(trackClipTypeAttribute.inspectedType) && typeof(ScriptableObject).IsAssignableFrom(trackClipTypeAttribute.inspectedType))
				{
					type = trackClipTypeAttribute.inspectedType;
					break;
				}
			}
			if (type == null)
			{
				Debug.LogWarning("Cannot create a default clip for type " + base.GetType());
				return null;
			}
			return this.CreateAndAddNewClipOfType(type);
		}

		public TimelineClip CreateClip<T>() where T : ScriptableObject, IPlayableAsset
		{
			return this.CreateClip(typeof(T));
		}

		public IMarker CreateMarker(Type type, double time)
		{
			return this.m_Markers.CreateMarker(type, time, this);
		}

		public T CreateMarker<T>(double time) where T : ScriptableObject, IMarker
		{
			return (T)((object)this.CreateMarker(typeof(T), time));
		}

		public bool DeleteMarker(IMarker marker)
		{
			return this.m_Markers.Remove(marker);
		}

		public IEnumerable<IMarker> GetMarkers()
		{
			return this.m_Markers.GetMarkers();
		}

		public int GetMarkerCount()
		{
			return this.m_Markers.Count;
		}

		public IMarker GetMarker(int idx)
		{
			return this.m_Markers[idx];
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
			return this.CreateClipFromAsset(scriptableObject);
		}

		internal TimelineClip CreateClipFromPlayableAsset(IPlayableAsset asset)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (asset as ScriptableObject == null)
			{
				throw new ArgumentException("CreateClipFromPlayableAsset  only supports ScriptableObject-derived Types");
			}
			if (!this.ValidateClipType(asset.GetType()))
			{
				throw new InvalidOperationException(string.Concat(new object[]
				{
					"Clips of type ",
					asset.GetType(),
					" are not permitted on tracks of type ",
					base.GetType()
				}));
			}
			return this.CreateClipFromAsset(asset as ScriptableObject);
		}

		private TimelineClip CreateClipFromAsset(ScriptableObject playableAsset)
		{
			TimelineClip timelineClip = this.CreateNewClipContainerInternal();
			timelineClip.displayName = playableAsset.name;
			timelineClip.asset = playableAsset;
			IPlayableAsset playableAsset2 = playableAsset as IPlayableAsset;
			if (playableAsset2 != null)
			{
				double duration = playableAsset2.duration;
				if (!double.IsInfinity(duration) && duration > 0.0)
				{
					timelineClip.duration = Math.Min(Math.Max(duration, TimelineClip.kMinDuration), TimelineClip.kMaxTimeValue);
				}
			}
			try
			{
				this.OnCreateClip(timelineClip);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message, playableAsset);
				return null;
			}
			return timelineClip;
		}

		internal IEnumerable<ScriptableObject> GetMarkersRaw()
		{
			return this.m_Markers.GetRawMarkerList();
		}

		internal void ClearMarkers()
		{
			this.m_Markers.Clear();
		}

		internal void AddMarker(ScriptableObject e)
		{
			this.m_Markers.Add(e);
		}

		internal bool DeleteMarkerRaw(ScriptableObject marker)
		{
			return this.m_Markers.Remove(marker, this.timelineAsset, this);
		}

		private int GetTimeRangeHash()
		{
			double num = double.MaxValue;
			double num2 = double.MinValue;
			foreach (IMarker marker in this.GetMarkers())
			{
				if (marker is INotification)
				{
					if (marker.time < num)
					{
						num = marker.time;
					}
					if (marker.time > num2)
					{
						num2 = marker.time;
					}
				}
			}
			return num.GetHashCode().CombineHash(num2.GetHashCode());
		}

		internal void AddClip(TimelineClip newClip)
		{
			if (!this.m_Clips.Contains(newClip))
			{
				this.m_Clips.Add(newClip);
				this.m_ClipsCache = null;
			}
		}

		private Playable CreateNotificationsPlayable(PlayableGraph graph, Playable mixerPlayable, GameObject go, Playable timelinePlayable)
		{
			TrackAsset.s_BuildData.markerList.Clear();
			this.GatherNotificiations(TrackAsset.s_BuildData.markerList);
			ScriptPlayable<TimeNotificationBehaviour> scriptPlayable = NotificationUtilities.CreateNotificationsPlayable(graph, TrackAsset.s_BuildData.markerList, go);
			if (scriptPlayable.IsValid<ScriptPlayable<TimeNotificationBehaviour>>())
			{
				scriptPlayable.GetBehaviour().timeSource = timelinePlayable;
				if (mixerPlayable.IsValid<Playable>())
				{
					scriptPlayable.SetInputCount(1);
					graph.Connect<Playable, ScriptPlayable<TimeNotificationBehaviour>>(mixerPlayable, 0, scriptPlayable, 0);
					scriptPlayable.SetInputWeight(mixerPlayable, 1f);
				}
			}
			return scriptPlayable;
		}

		internal Playable CreatePlayableGraph(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree, Playable timelinePlayable)
		{
			this.UpdateDuration();
			Playable playable = Playable.Null;
			if (this.CanCompileClipsRecursive())
			{
				playable = this.OnCreateClipPlayableGraph(graph, go, tree);
			}
			Playable playable2 = this.CreateNotificationsPlayable(graph, playable, go, timelinePlayable);
			if (!playable2.IsValid<Playable>() && !playable.IsValid<Playable>())
			{
				Debug.LogErrorFormat("Track {0} of type {1} has no notifications and returns an invalid mixer Playable", new object[]
				{
					base.name,
					base.GetType().FullName
				});
				return Playable.Create(graph, 0);
			}
			if (!playable2.IsValid<Playable>())
			{
				return playable;
			}
			return playable2;
		}

		internal virtual Playable CompileClips(PlayableGraph graph, GameObject go, IList<TimelineClip> timelineClips, IntervalTree<RuntimeElement> tree)
		{
			Playable playable = this.CreateTrackMixer(graph, go, timelineClips.Count);
			for (int i = 0; i < timelineClips.Count; i++)
			{
				Playable playable2 = this.CreatePlayable(graph, go, timelineClips[i]);
				if (playable2.IsValid<Playable>())
				{
					playable2.SetDuration(timelineClips[i].duration);
					RuntimeClip runtimeClip = new RuntimeClip(timelineClips[i], playable2, playable);
					tree.Add(runtimeClip);
					graph.Connect<Playable, Playable>(playable2, 0, playable, i);
					playable.SetInputWeight(i, 0f);
				}
			}
			this.ConfigureTrackAnimation(tree, go, playable);
			return playable;
		}

		private void GatherCompilableTracks(IList<TrackAsset> tracks)
		{
			if (!this.muted && this.CanCompileClips())
			{
				tracks.Add(this);
			}
			foreach (TrackAsset trackAsset in this.GetChildTracks())
			{
				if (trackAsset != null)
				{
					trackAsset.GatherCompilableTracks(tracks);
				}
			}
		}

		private void GatherNotificiations(List<IMarker> markers)
		{
			if (!this.muted && this.CanCompileNotifications())
			{
				markers.AddRange(this.GetMarkers());
			}
			foreach (TrackAsset trackAsset in this.GetChildTracks())
			{
				if (trackAsset != null)
				{
					trackAsset.GatherNotificiations(markers);
				}
			}
		}

		internal virtual Playable OnCreateClipPlayableGraph(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree)
		{
			if (tree == null)
			{
				throw new ArgumentException("IntervalTree argument cannot be null", "tree");
			}
			if (go == null)
			{
				throw new ArgumentException("GameObject argument cannot be null", "go");
			}
			TrackAsset.s_BuildData.Clear();
			this.GatherCompilableTracks(TrackAsset.s_BuildData.trackList);
			if (TrackAsset.s_BuildData.trackList.Count == 0)
			{
				return Playable.Null;
			}
			Playable playable = Playable.Null;
			ILayerable layerable = this as ILayerable;
			if (layerable != null)
			{
				playable = layerable.CreateLayerMixer(graph, go, TrackAsset.s_BuildData.trackList.Count);
			}
			if (playable.IsValid<Playable>())
			{
				for (int i = 0; i < TrackAsset.s_BuildData.trackList.Count; i++)
				{
					Playable playable2 = TrackAsset.s_BuildData.trackList[i].CompileClips(graph, go, TrackAsset.s_BuildData.trackList[i].clips, tree);
					if (playable2.IsValid<Playable>())
					{
						graph.Connect<Playable, Playable>(playable2, 0, playable, i);
						playable.SetInputWeight(i, 1f);
					}
				}
				return playable;
			}
			if (TrackAsset.s_BuildData.trackList.Count == 1)
			{
				return TrackAsset.s_BuildData.trackList[0].CompileClips(graph, go, TrackAsset.s_BuildData.trackList[0].clips, tree);
			}
			for (int j = 0; j < TrackAsset.s_BuildData.trackList.Count; j++)
			{
				TrackAsset.s_BuildData.clipList.AddRange(TrackAsset.s_BuildData.trackList[j].clips);
			}
			return this.CompileClips(graph, go, TrackAsset.s_BuildData.clipList, tree);
		}

		internal void ConfigureTrackAnimation(IntervalTree<RuntimeElement> tree, GameObject go, Playable blend)
		{
			if (!this.hasCurves)
			{
				return;
			}
			blend.SetAnimatedProperties(this.m_Curves);
			tree.Add(new InfiniteRuntimeClip(blend));
			if (TrackAsset.OnTrackAnimationPlayableCreate != null)
			{
				TrackAsset.OnTrackAnimationPlayableCreate(this, go, blend);
			}
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
			if (child == null)
			{
				return;
			}
			this.m_Children.Add(child);
			child.parent = this;
			this.Invalidate();
		}

		internal void MoveLastTrackBefore(TrackAsset asset)
		{
			if (this.m_Children == null || this.m_Children.Count < 2 || asset == null)
			{
				return;
			}
			ScriptableObject scriptableObject = this.m_Children[this.m_Children.Count - 1];
			if (scriptableObject == asset)
			{
				return;
			}
			for (int i = 0; i < this.m_Children.Count - 1; i++)
			{
				if (this.m_Children[i] == asset)
				{
					for (int j = this.m_Children.Count - 1; j > i; j--)
					{
						this.m_Children[j] = this.m_Children[j - 1];
					}
					this.m_Children[i] = scriptableObject;
					this.Invalidate();
					return;
				}
			}
		}

		internal bool RemoveSubTrack(TrackAsset child)
		{
			if (this.m_Children.Remove(child))
			{
				this.Invalidate();
				child.parent = null;
				return true;
			}
			return false;
		}

		internal void RemoveClip(TimelineClip clip)
		{
			this.m_Clips.Remove(clip);
			this.m_ClipsCache = null;
		}

		internal virtual void GetEvaluationTime(out double outStart, out double outDuration)
		{
			outStart = double.PositiveInfinity;
			double num = double.NegativeInfinity;
			if (this.hasCurves)
			{
				outStart = 0.0;
				num = TimeUtility.GetAnimationClipLength(this.curves);
			}
			foreach (TimelineClip timelineClip in this.clips)
			{
				outStart = Math.Min(timelineClip.start, outStart);
				num = Math.Max(timelineClip.end, num);
			}
			if (this.HasNotifications())
			{
				double notificationDuration = this.GetNotificationDuration();
				outStart = Math.Min(notificationDuration, outStart);
				num = Math.Max(notificationDuration, num);
			}
			if (double.IsInfinity(outStart) || double.IsInfinity(num))
			{
				outStart = (outDuration = 0.0);
				return;
			}
			outDuration = num - outStart;
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
			if (this.hasCurves)
			{
				driver.AddObjectProperties(this, this.m_Curves);
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
			if (director == null)
			{
				return null;
			}
			Object genericBinding = director.GetGenericBinding(this);
			GameObject gameObject = genericBinding as GameObject;
			if (gameObject != null)
			{
				return gameObject;
			}
			Component component = genericBinding as Component;
			if (component != null)
			{
				return component.gameObject;
			}
			return null;
		}

		internal bool ValidateClipType(Type clipType)
		{
			object[] customAttributes = base.GetType().GetCustomAttributes(typeof(TrackClipTypeAttribute), true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				if (((TrackClipTypeAttribute)customAttributes[i]).inspectedType.IsAssignableFrom(clipType))
				{
					return true;
				}
			}
			return typeof(PlayableTrack).IsAssignableFrom(base.GetType()) && typeof(IPlayableAsset).IsAssignableFrom(clipType) && typeof(ScriptableObject).IsAssignableFrom(clipType);
		}

		protected virtual void OnCreateClip(TimelineClip clip)
		{
		}

		private void UpdateDuration()
		{
			int num = this.CalculateItemsHash();
			if (num == this.m_ItemsHash)
			{
				return;
			}
			this.m_ItemsHash = num;
			double num2;
			double num3;
			this.GetSequenceTime(out num2, out num3);
			this.m_Start = (DiscreteTime)num2;
			this.m_End = (DiscreteTime)(num2 + num3);
			this.CalculateExtrapolationTimes();
		}

		protected internal virtual int CalculateItemsHash()
		{
			return HashUtility.CombineHash(this.GetClipsHash(), TrackAsset.GetAnimationClipHash(this.m_Curves), this.GetTimeRangeHash());
		}

		protected virtual Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
		{
			if (!graph.IsValid())
			{
				throw new ArgumentException("graph must be a valid PlayableGraph");
			}
			if (clip == null)
			{
				throw new ArgumentNullException("clip");
			}
			IPlayableAsset playableAsset = clip.asset as IPlayableAsset;
			if (playableAsset != null)
			{
				Playable playable = playableAsset.CreatePlayable(graph, gameObject);
				if (playable.IsValid<Playable>())
				{
					playable.SetAnimatedProperties(clip.curves);
					playable.SetSpeed(clip.timeScale);
					if (TrackAsset.OnClipPlayableCreate != null)
					{
						TrackAsset.OnClipPlayableCreate(clip, gameObject, playable);
					}
				}
				return playable;
			}
			return Playable.Null;
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

		internal double GetNotificationDuration()
		{
			if (!this.supportsNotifications)
			{
				return 0.0;
			}
			double num = 0.0;
			foreach (IMarker marker in this.GetMarkers())
			{
				if (marker is INotification)
				{
					num = Math.Max(num, marker.time);
				}
			}
			return num;
		}

		internal virtual bool CanCompileClips()
		{
			return this.hasClips || this.hasCurves;
		}

		internal bool IsCompilable()
		{
			if (typeof(GroupTrack).IsAssignableFrom(base.GetType()))
			{
				return false;
			}
			bool flag = !this.mutedInHierarchy && (this.CanCompileClips() || this.CanCompileNotifications());
			if (!flag)
			{
				using (IEnumerator<TrackAsset> enumerator = this.GetChildTracks().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsCompilable())
						{
							return true;
						}
					}
				}
				return flag;
			}
			return flag;
		}

		private void UpdateChildTrackCache()
		{
			if (this.m_ChildTrackCache == null)
			{
				if (this.m_Children == null || this.m_Children.Count == 0)
				{
					this.m_ChildTrackCache = TrackAsset.s_EmptyCache;
					return;
				}
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

		internal virtual int Hash()
		{
			return this.clips.Length + (this.m_Markers.Count << 16);
		}

		private int GetClipsHash()
		{
			int num = 0;
			foreach (TimelineClip timelineClip in this.m_Clips)
			{
				num = num.CombineHash(timelineClip.Hash());
			}
			return num;
		}

		protected static int GetAnimationClipHash(AnimationClip clip)
		{
			int num = 0;
			if (clip != null && !clip.empty)
			{
				num = num.CombineHash(clip.frameRate.GetHashCode()).CombineHash(clip.length.GetHashCode());
			}
			return num;
		}

		private bool HasNotifications()
		{
			return this.m_Markers.HasNotifications();
		}

		private bool CanCompileNotifications()
		{
			return this.supportsNotifications && this.m_Markers.HasNotifications();
		}

		private bool CanCompileClipsRecursive()
		{
			if (this.CanCompileClips())
			{
				return true;
			}
			using (IEnumerator<TrackAsset> enumerator = this.GetChildTracks().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CanCompileClipsRecursive())
					{
						return true;
					}
				}
			}
			return false;
		}

		private const int k_LatestVersion = 3;

		[SerializeField]
		[HideInInspector]
		private int m_Version;

		[Obsolete("Please use m_InfiniteClip (on AnimationTrack) instead.", false)]
		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("m_animClip")]
		internal AnimationClip m_AnimClip;

		private static TrackAsset.TransientBuildData s_BuildData = TrackAsset.TransientBuildData.Create();

		internal const string kDefaultCurvesName = "Track Parameters";

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
		[HideInInspector]
		private AnimationClip m_Curves;

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

		private bool? m_SupportsNotifications;

		private static TrackAsset[] s_EmptyCache = new TrackAsset[0];

		private IEnumerable<TrackAsset> m_ChildTrackCache;

		private static Dictionary<Type, TrackBindingTypeAttribute> s_TrackBindingTypeAttributeCache = new Dictionary<Type, TrackBindingTypeAttribute>();

		[SerializeField]
		[HideInInspector]
		protected internal List<TimelineClip> m_Clips = new List<TimelineClip>();

		[SerializeField]
		[HideInInspector]
		private MarkerList m_Markers = new MarkerList(0);

		internal enum Versions
		{
			Initial,
			RotationAsEuler,
			RootMotionUpgrade,
			AnimatedTrackProperties
		}

		private static class TrackAssetUpgrade
		{
		}

		private struct TransientBuildData
		{
			public static TrackAsset.TransientBuildData Create()
			{
				return new TrackAsset.TransientBuildData
				{
					trackList = new List<TrackAsset>(20),
					clipList = new List<TimelineClip>(500),
					markerList = new List<IMarker>(100)
				};
			}

			public void Clear()
			{
				this.trackList.Clear();
				this.clipList.Clear();
				this.markerList.Clear();
			}

			public List<TrackAsset> trackList;

			public List<TimelineClip> clipList;

			public List<IMarker> markerList;
		}
	}
}
