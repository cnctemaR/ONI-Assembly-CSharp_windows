using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	public class TimelinePlayable : PlayableBehaviour
	{
		public static ScriptPlayable<TimelinePlayable> Create(PlayableGraph graph, IEnumerable<TrackAsset> tracks, GameObject go, bool autoRebalance, bool createOutputs)
		{
			if (tracks == null)
			{
				throw new ArgumentNullException("Tracks list is null", "tracks");
			}
			if (go == null)
			{
				throw new ArgumentNullException("GameObject parameter is null", "go");
			}
			ScriptPlayable<TimelinePlayable> scriptPlayable = ScriptPlayable<TimelinePlayable>.Create(graph, 0);
			scriptPlayable.SetTraversalMode(PlayableTraversalMode.Passthrough);
			scriptPlayable.GetBehaviour().Compile(graph, scriptPlayable, tracks, go, autoRebalance, createOutputs);
			return scriptPlayable;
		}

		public void Compile(PlayableGraph graph, Playable timelinePlayable, IEnumerable<TrackAsset> tracks, GameObject go, bool autoRebalance, bool createOutputs)
		{
			if (tracks == null)
			{
				throw new ArgumentNullException("Tracks list is null", "tracks");
			}
			if (go == null)
			{
				throw new ArgumentNullException("GameObject parameter is null", "go");
			}
			List<TrackAsset> list = new List<TrackAsset>(tracks);
			int num = list.Count * 2 + list.Count;
			this.m_CurrentListOfActiveClips = new List<RuntimeElement>(num);
			this.m_ActiveClips = new List<RuntimeElement>(num);
			this.m_EvaluateCallbacks.Clear();
			this.m_AlwaysEvaluateCallbacks.Clear();
			this.m_PlayableCache.Clear();
			this.CompileTrackList(graph, timelinePlayable, list, go, createOutputs);
		}

		private void CompileTrackList(PlayableGraph graph, Playable timelinePlayable, IEnumerable<TrackAsset> tracks, GameObject go, bool createOutputs)
		{
			foreach (TrackAsset trackAsset in tracks)
			{
				if (trackAsset.IsCompilable() && !this.m_PlayableCache.ContainsKey(trackAsset))
				{
					trackAsset.SortClips();
					trackAsset.ComputeBlendsFromOverlaps(false);
					this.CreateTrackPlayable(graph, timelinePlayable, trackAsset, go, createOutputs);
				}
			}
		}

		private void CreateTrackOutput(PlayableGraph graph, TrackAsset track, GameObject go, Playable playable, int port)
		{
			if (track.isSubTrack)
			{
				return;
			}
			foreach (PlayableBinding playableBinding in track.outputs)
			{
				PlayableOutput playableOutput = playableBinding.CreateOutput(graph);
				playableOutput.SetReferenceObject(playableBinding.sourceObject);
				playableOutput.SetSourcePlayable(playable, port);
				playableOutput.SetWeight(1f);
				AnimationTrack animationTrack = track as AnimationTrack;
				if (animationTrack != null)
				{
					this.AddPlayableOutputCallbacks(animationTrack, playableOutput);
				}
				if (playableOutput.IsPlayableOutputOfType<AudioPlayableOutput>())
				{
					((AudioPlayableOutput)playableOutput).SetEvaluateOnSeek(!TimelinePlayable.muteAudioScrubbing);
				}
				if (track.timelineAsset.markerTrack == track)
				{
					PlayableDirector component = go.GetComponent<PlayableDirector>();
					playableOutput.SetUserData(component);
					foreach (INotificationReceiver notificationReceiver in go.GetComponents<INotificationReceiver>())
					{
						playableOutput.AddNotificationReceiver(notificationReceiver);
					}
				}
			}
		}

		private Playable CreateTrackPlayable(PlayableGraph graph, Playable timelinePlayable, TrackAsset track, GameObject go, bool createOutputs)
		{
			if (!track.IsCompilable())
			{
				return timelinePlayable;
			}
			Playable playable;
			if (this.m_PlayableCache.TryGetValue(track, out playable))
			{
				return playable;
			}
			if (track.name == "root")
			{
				return timelinePlayable;
			}
			TrackAsset trackAsset = track.parent as TrackAsset;
			Playable playable2 = ((trackAsset != null) ? this.CreateTrackPlayable(graph, timelinePlayable, trackAsset, go, createOutputs) : timelinePlayable);
			Playable playable3 = track.CreatePlayableGraph(graph, go, this.m_IntervalTree, timelinePlayable);
			bool flag = false;
			if (!playable3.IsValid<Playable>())
			{
				string name = track.name;
				string text = "(";
				Type type = track.GetType();
				throw new InvalidOperationException(name + text + ((type != null) ? type.ToString() : null) + ") did not produce a valid playable.");
			}
			if (playable2.IsValid<Playable>() && playable3.IsValid<Playable>())
			{
				int inputCount = playable2.GetInputCount<Playable>();
				playable2.SetInputCount(inputCount + 1);
				flag = graph.Connect<Playable, Playable>(playable3, 0, playable2, inputCount);
				playable2.SetInputWeight(inputCount, 1f);
			}
			if (createOutputs && flag)
			{
				this.CreateTrackOutput(graph, track, go, playable2, playable2.GetInputCount<Playable>() - 1);
			}
			this.CacheTrack(track, playable3);
			return playable3;
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
			this.Evaluate(playable, info);
		}

		private void Evaluate(Playable playable, FrameData frameData)
		{
			if (this.m_IntervalTree == null)
			{
				return;
			}
			double time = playable.GetTime<Playable>();
			this.m_ActiveBit = ((this.m_ActiveBit == 0) ? 1 : 0);
			this.m_CurrentListOfActiveClips.Clear();
			this.m_IntervalTree.IntersectsWith(DiscreteTime.GetNearestTick(time), this.m_CurrentListOfActiveClips);
			foreach (RuntimeElement runtimeElement in this.m_CurrentListOfActiveClips)
			{
				runtimeElement.intervalBit = this.m_ActiveBit;
			}
			double num = (double)new DiscreteTime(playable.GetDuration<Playable>());
			foreach (RuntimeElement runtimeElement2 in this.m_ActiveClips)
			{
				if (runtimeElement2.intervalBit != this.m_ActiveBit)
				{
					runtimeElement2.DisableAt(time, num, frameData);
				}
			}
			this.m_ActiveClips.Clear();
			for (int i = 0; i < this.m_CurrentListOfActiveClips.Count; i++)
			{
				this.m_CurrentListOfActiveClips[i].EvaluateAt(time, frameData);
				this.m_ActiveClips.Add(this.m_CurrentListOfActiveClips[i]);
			}
			this.InvokeOutputCallbacks(this.m_CurrentListOfActiveClips);
		}

		private void CacheTrack(TrackAsset track, Playable playable)
		{
			this.m_PlayableCache[track] = playable;
		}

		private static void ForAOTCompilationOnly()
		{
			new List<IntervalTree<RuntimeElement>.Entry>();
		}

		private void AddPlayableOutputCallbacks(AnimationTrack track, PlayableOutput playableOutput)
		{
			this.AddOutputWeightProcessor(track, (AnimationPlayableOutput)playableOutput);
		}

		private void AddOutputWeightProcessor(AnimationTrack track, AnimationPlayableOutput animOutput)
		{
			AnimationOutputWeightProcessor animationOutputWeightProcessor = new AnimationOutputWeightProcessor(animOutput);
			if (track.inClipMode)
			{
				this.AddEvaluateCallback(track, animationOutputWeightProcessor);
			}
			else
			{
				this.m_AlwaysEvaluateCallbacks.Add(animationOutputWeightProcessor);
			}
			this.m_ForceEvaluateNextEvaluate.Add(animationOutputWeightProcessor);
		}

		private void AddEvaluateCallback(AnimationTrack track, ITimelineEvaluateCallback callback)
		{
			List<ITimelineEvaluateCallback> list;
			if (this.m_EvaluateCallbacks.TryGetValue(track, out list))
			{
				list.Add(callback);
				return;
			}
			this.m_EvaluateCallbacks[track] = new List<ITimelineEvaluateCallback> { callback };
		}

		private void InvokeOutputCallbacks(IReadOnlyList<RuntimeElement> activeRuntimeElements)
		{
			foreach (ITimelineEvaluateCallback timelineEvaluateCallback in this.m_ForceEvaluateNextEvaluate)
			{
				timelineEvaluateCallback.Evaluate();
				this.m_InvokedThisFrame.Add(timelineEvaluateCallback);
			}
			this.m_ForceEvaluateNextEvaluate.Clear();
			if (activeRuntimeElements.Count > 0)
			{
				using (TimelinePlayable.TrackCacheManager trackCacheManager = new TimelinePlayable.TrackCacheManager(this.m_ActiveTracksToEvaluateCache, activeRuntimeElements))
				{
					using (HashSet<AnimationTrack>.Enumerator enumerator2 = trackCacheManager.trackCache.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							AnimationTrack animationTrack = enumerator2.Current;
							List<ITimelineEvaluateCallback> list;
							if (this.TryGetCallbackList(animationTrack, out list))
							{
								foreach (ITimelineEvaluateCallback timelineEvaluateCallback2 in list)
								{
									if (!this.m_InvokedThisFrame.Contains(timelineEvaluateCallback2))
									{
										timelineEvaluateCallback2.Evaluate();
										this.m_InvokedThisFrame.Add(timelineEvaluateCallback2);
										this.m_ForceEvaluateNextEvaluate.Add(timelineEvaluateCallback2);
									}
								}
							}
						}
						goto IL_0188;
					}
				}
			}
			foreach (List<ITimelineEvaluateCallback> list2 in this.m_EvaluateCallbacks.Values)
			{
				foreach (ITimelineEvaluateCallback timelineEvaluateCallback3 in list2)
				{
					if (!this.m_InvokedThisFrame.Contains(timelineEvaluateCallback3))
					{
						timelineEvaluateCallback3.Evaluate();
					}
				}
			}
			IL_0188:
			foreach (ITimelineEvaluateCallback timelineEvaluateCallback4 in this.m_AlwaysEvaluateCallbacks)
			{
				timelineEvaluateCallback4.Evaluate();
			}
			this.m_InvokedThisFrame.Clear();
		}

		private bool TryGetCallbackList(AnimationTrack track, out List<ITimelineEvaluateCallback> list)
		{
			if (track == null)
			{
				list = null;
				return false;
			}
			return this.m_EvaluateCallbacks.TryGetValue(track, out list) || this.TryGetCallbackList(track.parent as AnimationTrack, out list);
		}

		private static ProfilerMarker k_CreateTimelineGraphMarker = new ProfilerMarker(ProfilerCategory.Scripts, "Timeline.CreatePlayableGraph");

		private static ProfilerMarker k_CreateTimelineTrackMarker = new ProfilerMarker(ProfilerCategory.Scripts, "Timeline.CreateTrackPlayable");

		private static ProfilerMarker k_CreateTimelineTrackOutputsMarker = new ProfilerMarker(ProfilerCategory.Scripts, "Timeline.CreateTrackPlayableOutputs");

		private static ProfilerMarker m_findActiveClipsMarker = new ProfilerMarker(ProfilerCategory.Scripts, "TimelinePlayable.GetActiveClips");

		private static ProfilerMarker m_SetClipsLocalTimeMarker = new ProfilerMarker(ProfilerCategory.Scripts, "TimelinePlayable.SetActiveClipsTime");

		private IntervalTree<RuntimeElement> m_IntervalTree = new IntervalTree<RuntimeElement>();

		private List<RuntimeElement> m_ActiveClips = new List<RuntimeElement>();

		private List<RuntimeElement> m_CurrentListOfActiveClips;

		private int m_ActiveBit;

		private Dictionary<TrackAsset, Playable> m_PlayableCache = new Dictionary<TrackAsset, Playable>();

		internal static bool muteAudioScrubbing = true;

		private readonly Dictionary<AnimationTrack, List<ITimelineEvaluateCallback>> m_EvaluateCallbacks = new Dictionary<AnimationTrack, List<ITimelineEvaluateCallback>>();

		private readonly List<ITimelineEvaluateCallback> m_AlwaysEvaluateCallbacks = new List<ITimelineEvaluateCallback>();

		private readonly HashSet<ITimelineEvaluateCallback> m_ForceEvaluateNextEvaluate = new HashSet<ITimelineEvaluateCallback>();

		private readonly HashSet<ITimelineEvaluateCallback> m_InvokedThisFrame = new HashSet<ITimelineEvaluateCallback>();

		private readonly HashSet<AnimationTrack> m_ActiveTracksToEvaluateCache = new HashSet<AnimationTrack>();

		private readonly struct TrackCacheManager : IDisposable
		{
			public TrackCacheManager(HashSet<AnimationTrack> cache, IReadOnlyList<RuntimeElement> activeRuntimeElements)
			{
				this.trackCache = cache;
				this.GetTrackAssetsFromRuntimeElements(activeRuntimeElements);
			}

			public void Dispose()
			{
				this.trackCache.Clear();
			}

			private void GetTrackAssetsFromRuntimeElements(IReadOnlyList<RuntimeElement> activeRuntimeElements)
			{
				for (int i = 0; i < activeRuntimeElements.Count; i++)
				{
					RuntimeClip runtimeClip = activeRuntimeElements[i] as RuntimeClip;
					if (runtimeClip != null)
					{
						TimelineClip clip = runtimeClip.clip;
						AnimationTrack animationTrack = ((clip != null) ? clip.GetParentTrack() : null) as AnimationTrack;
						if (animationTrack != null)
						{
							this.trackCache.Add(animationTrack);
						}
					}
				}
			}

			public readonly HashSet<AnimationTrack> trackCache;
		}
	}
}
