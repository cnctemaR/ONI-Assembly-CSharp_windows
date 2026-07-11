using System;
using System.Collections.Generic;
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
				if (track as AnimationTrack != null)
				{
					this.EvaluateWeightsForAnimationPlayableOutput(track, (AnimationPlayableOutput)playableOutput);
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

		private void EvaluateWeightsForAnimationPlayableOutput(TrackAsset track, AnimationPlayableOutput animOutput)
		{
			this.m_EvaluateCallbacks.Add(new AnimationOutputWeightProcessor(animOutput));
		}

		private void EvaluateAnimationPreviewUpdateCallback(TrackAsset track, AnimationPlayableOutput animOutput)
		{
			this.m_EvaluateCallbacks.Add(new AnimationPreviewUpdateCallback(animOutput));
		}

		private static Playable CreatePlayableGraph(PlayableGraph graph, TrackAsset asset, GameObject go, IntervalTree<RuntimeElement> tree, Playable timelinePlayable)
		{
			return asset.CreatePlayableGraph(graph, go, tree, timelinePlayable);
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
			Playable playable3 = TimelinePlayable.CreatePlayableGraph(graph, track, go, this.m_IntervalTree, timelinePlayable);
			bool flag = false;
			if (!playable3.IsValid<Playable>())
			{
				throw new InvalidOperationException(string.Concat(new object[]
				{
					track.name,
					"(",
					track.GetType(),
					") did not produce a valid playable. Use the compilable property to indicate whether the track is valid for processing"
				}));
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
			this.CacheTrack(track, playable3, flag ? (playable2.GetInputCount<Playable>() - 1) : (-1), playable2);
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
				if (frameData.timeLooped)
				{
					runtimeElement.Reset();
				}
			}
			double duration = playable.GetDuration<Playable>();
			foreach (RuntimeElement runtimeElement2 in this.m_ActiveClips)
			{
				if (runtimeElement2.intervalBit != this.m_ActiveBit)
				{
					double num = (double)DiscreteTime.FromTicks(runtimeElement2.intervalEnd);
					double num2 = (frameData.timeLooped ? Math.Min(num, duration) : Math.Min(time, num));
					runtimeElement2.EvaluateAt(num2, frameData);
					runtimeElement2.enable = false;
				}
			}
			this.m_ActiveClips.Clear();
			for (int i = 0; i < this.m_CurrentListOfActiveClips.Count; i++)
			{
				this.m_CurrentListOfActiveClips[i].EvaluateAt(time, frameData);
				this.m_ActiveClips.Add(this.m_CurrentListOfActiveClips[i]);
			}
			int count = this.m_EvaluateCallbacks.Count;
			for (int j = 0; j < count; j++)
			{
				this.m_EvaluateCallbacks[j].Evaluate();
			}
		}

		private void CacheTrack(TrackAsset track, Playable playable, int port, Playable parent)
		{
			this.m_PlayableCache[track] = playable;
		}

		private static void ForAOTCompilationOnly()
		{
			new List<IntervalTree<RuntimeElement>.Entry>();
		}

		private IntervalTree<RuntimeElement> m_IntervalTree = new IntervalTree<RuntimeElement>();

		private List<RuntimeElement> m_ActiveClips = new List<RuntimeElement>();

		private List<RuntimeElement> m_CurrentListOfActiveClips;

		private int m_ActiveBit;

		private List<ITimelineEvaluateCallback> m_EvaluateCallbacks = new List<ITimelineEvaluateCallback>();

		private Dictionary<TrackAsset, Playable> m_PlayableCache = new Dictionary<TrackAsset, Playable>();

		internal static bool muteAudioScrubbing = true;
	}
}
