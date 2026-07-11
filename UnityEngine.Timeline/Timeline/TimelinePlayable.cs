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
			TimelinePlayable behaviour = scriptPlayable.GetBehaviour();
			behaviour.Compile(graph, scriptPlayable, tracks, go, autoRebalance, createOutputs);
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
			this.AllocateDefaultTracks(graph, timelinePlayable, list, go);
			this.CompileTrackList(graph, timelinePlayable, list, go, createOutputs);
		}

		private void AllocateDefaultTracks(PlayableGraph graph, Playable timelinePlayable, IList<TrackAsset> tracks, GameObject go)
		{
		}

		private void CompileTrackList(PlayableGraph graph, Playable timelinePlayable, IEnumerable<TrackAsset> tracks, GameObject go, bool createOutputs)
		{
			foreach (TrackAsset trackAsset in tracks)
			{
				if (trackAsset.compilable)
				{
					if (!this.m_PlayableCache.ContainsKey(trackAsset))
					{
						trackAsset.SortClips();
						this.CreateTrackPlayable(graph, timelinePlayable, trackAsset, go, createOutputs);
					}
				}
			}
		}

		private void CreateTrackOutput(PlayableGraph graph, TrackAsset track, Playable playable, int port)
		{
			if (!track.isSubTrack)
			{
				IEnumerable<PlayableBinding> outputs = track.outputs;
				foreach (PlayableBinding playableBinding in outputs)
				{
					PlayableOutput playableOutput = playableBinding.CreateOutput(graph);
					playableOutput.SetReferenceObject(playableBinding.sourceObject);
					playableOutput.SetSourcePlayable(playable);
					playableOutput.SetSourceOutputPort(port);
					playableOutput.SetWeight(1f);
					if (track as AnimationTrack != null)
					{
						this.EvaluateWeightsForAnimationPlayableOutput(track, (AnimationPlayableOutput)playableOutput);
					}
					if (playableOutput.IsPlayableOutputOfType<AudioPlayableOutput>())
					{
						((AudioPlayableOutput)playableOutput).SetEvaluateOnSeek(!TimelinePlayable.muteAudioScrubbing);
					}
				}
			}
		}

		private void EvaluateWeightsForAnimationPlayableOutput(TrackAsset track, AnimationPlayableOutput animOutput)
		{
			this.m_EvaluateCallbacks.Add(new AnimationOutputWeightProcessor(animOutput));
		}

		private static Playable CreatePlayableGraph(PlayableGraph graph, TrackAsset asset, GameObject go, IntervalTree<RuntimeElement> tree)
		{
			return asset.CreatePlayableGraph(graph, go, tree);
		}

		private Playable CreateTrackPlayable(PlayableGraph graph, Playable timelinePlayable, TrackAsset track, GameObject go, bool createOutputs)
		{
			Playable playable;
			Playable playable2;
			if (!track.compilable)
			{
				playable = timelinePlayable;
			}
			else if (this.m_PlayableCache.TryGetValue(track, out playable2))
			{
				playable = playable2;
			}
			else if (track.name == "root")
			{
				playable = timelinePlayable;
			}
			else
			{
				TrackAsset trackAsset = track.parent as TrackAsset;
				Playable playable3 = ((!(trackAsset != null)) ? timelinePlayable : this.CreateTrackPlayable(graph, timelinePlayable, trackAsset, go, createOutputs));
				Playable playable4 = TimelinePlayable.CreatePlayableGraph(graph, track, go, this.m_IntervalTree);
				bool flag = false;
				if (!playable4.IsValid<Playable>())
				{
					throw new InvalidOperationException(string.Concat(new object[]
					{
						track.name,
						"(",
						track.GetType(),
						") did not produce a valid playable. Use the compilable property to indicate whether the track is valid for processing"
					}));
				}
				if (playable3.IsValid<Playable>() && playable4.IsValid<Playable>())
				{
					int inputCount = playable3.GetInputCount<Playable>();
					playable3.SetInputCount(inputCount + 1);
					flag = graph.Connect<Playable, Playable>(playable4, 0, playable3, inputCount);
					playable3.SetInputWeight(inputCount, 1f);
				}
				if (createOutputs && flag)
				{
					this.CreateTrackOutput(graph, track, playable3, playable3.GetInputCount<Playable>() - 1);
				}
				this.CacheTrack(track, playable4, (!flag) ? (-1) : (playable3.GetInputCount<Playable>() - 1), playable3);
				playable = playable4;
			}
			return playable;
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
			this.Evaluate(playable, info);
		}

		private void Evaluate(Playable playable, FrameData frameData)
		{
			if (this.m_IntervalTree != null)
			{
				double time = playable.GetTime<Playable>();
				this.m_ActiveBit = ((this.m_ActiveBit != 0) ? 0 : 1);
				this.m_CurrentListOfActiveClips.Clear();
				this.m_IntervalTree.IntersectsWith(DiscreteTime.GetNearestTick(time), this.m_ActiveBit, ref this.m_CurrentListOfActiveClips);
				for (int i = 0; i < this.m_ActiveClips.Count; i++)
				{
					RuntimeElement runtimeElement = this.m_ActiveClips[i];
					if (runtimeElement.intervalBit != this.m_ActiveBit)
					{
						runtimeElement.enable = false;
					}
				}
				this.m_ActiveClips.Clear();
				for (int j = 0; j < this.m_CurrentListOfActiveClips.Count; j++)
				{
					this.m_CurrentListOfActiveClips[j].EvaluateAt(time, frameData);
					this.m_ActiveClips.Add(this.m_CurrentListOfActiveClips[j]);
				}
				int count = this.m_EvaluateCallbacks.Count;
				for (int k = 0; k < count; k++)
				{
					this.m_EvaluateCallbacks[k].Evaluate();
				}
			}
		}

		private void CacheTrack(TrackAsset track, Playable playable, int port, Playable parent)
		{
			this.m_PlayableCache[track] = playable;
		}

		private IntervalTree<RuntimeElement> m_IntervalTree = new IntervalTree<RuntimeElement>();

		private List<RuntimeElement> m_ActiveClips = new List<RuntimeElement>();

		private List<RuntimeElement> m_CurrentListOfActiveClips;

		private int m_ActiveBit = 0;

		private List<ITimelineEvaluateCallback> m_EvaluateCallbacks = new List<ITimelineEvaluateCallback>();

		private Dictionary<TrackAsset, Playable> m_PlayableCache = new Dictionary<TrackAsset, Playable>();

		internal static bool muteAudioScrubbing = true;
	}
}
