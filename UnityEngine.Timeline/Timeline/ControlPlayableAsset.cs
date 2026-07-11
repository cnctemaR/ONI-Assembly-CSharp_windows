using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	[NotKeyable]
	[Serializable]
	public class ControlPlayableAsset : PlayableAsset, IPropertyPreview, ITimelineClipAsset, IClipInitializer, IDirectorDriver
	{
		public void OnEnable()
		{
			if (this.particleRandomSeed == 0U)
			{
				this.particleRandomSeed = (uint)Random.Range(1, 10000);
			}
		}

		public override double duration
		{
			get
			{
				return this.m_Duration;
			}
		}

		public ClipCaps clipCaps
		{
			get
			{
				return ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | ((!this.m_SupportLoop) ? ClipCaps.None : ClipCaps.Looping);
			}
		}

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			if (this.prefabGameObject != null)
			{
				if (ControlPlayableAsset.s_CreatedPrefabs.Contains(this.prefabGameObject))
				{
					Debug.LogWarningFormat("Control Track Clip ({0}) is causing a prefab to instantiate itself recursively. Aborting further instances.", new object[] { base.name });
					return Playable.Create(graph, 0);
				}
				ControlPlayableAsset.s_CreatedPrefabs.Add(this.prefabGameObject);
			}
			Playable playable = Playable.Null;
			List<Playable> list = new List<Playable>();
			GameObject gameObject = this.sourceGameObject.Resolve(graph.GetResolver());
			if (this.prefabGameObject != null)
			{
				Transform transform = ((!(gameObject != null)) ? null : gameObject.transform);
				ScriptPlayable<PrefabControlPlayable> scriptPlayable = PrefabControlPlayable.Create(graph, this.prefabGameObject, transform);
				gameObject = scriptPlayable.GetBehaviour().prefabInstance;
				list.Add(scriptPlayable);
			}
			this.m_Duration = PlayableBinding.DefaultDuration;
			this.m_SupportLoop = false;
			if (gameObject != null)
			{
				IList<PlayableDirector> list2 = ((!this.updateDirector) ? ControlPlayableAsset.k_EmptyDirectorsList : this.GetComponent<PlayableDirector>(gameObject));
				IList<ParticleSystem> list3 = ((!this.updateParticle) ? ControlPlayableAsset.k_EmptyParticlesList : this.GetParticleSystemRoots(gameObject));
				this.UpdateDurationAndLoopFlag(list2, list3);
				PlayableDirector component = go.GetComponent<PlayableDirector>();
				if (component != null)
				{
					this.m_ControlDirectorAsset = component.playableAsset;
				}
				if (go == gameObject && this.prefabGameObject == null)
				{
					Debug.LogWarningFormat("Control Playable ({0}) is referencing the same PlayableDirector component than the one in which it is playing.", new object[] { base.name });
					this.active = false;
					if (!this.searchHierarchy)
					{
						this.updateDirector = false;
					}
				}
				if (this.active)
				{
					this.CreateActivationPlayable(gameObject, graph, list);
				}
				if (this.updateDirector)
				{
					this.SearchHierarchyAndConnectDirector(list2, graph, list, this.prefabGameObject != null);
				}
				if (this.updateParticle)
				{
					this.SearchHiearchyAndConnectParticleSystem(list3, graph, list);
				}
				if (this.updateITimeControl)
				{
					ControlPlayableAsset.SearchHierarchyAndConnectControlableScripts(ControlPlayableAsset.GetControlableScripts(gameObject), graph, list);
				}
				playable = ControlPlayableAsset.ConnectPlayablesToMixer(graph, list);
			}
			if (this.prefabGameObject != null)
			{
				ControlPlayableAsset.s_CreatedPrefabs.Remove(this.prefabGameObject);
			}
			if (!playable.IsValid<Playable>())
			{
				playable = Playable.Create(graph, 0);
			}
			return playable;
		}

		private static Playable ConnectPlayablesToMixer(PlayableGraph graph, List<Playable> playables)
		{
			Playable playable = Playable.Create(graph, playables.Count);
			for (int num = 0; num != playables.Count; num++)
			{
				ControlPlayableAsset.ConnectMixerAndPlayable(graph, playable, playables[num], num);
			}
			playable.SetPropagateSetTime(true);
			return playable;
		}

		private void CreateActivationPlayable(GameObject root, PlayableGraph graph, List<Playable> outplayables)
		{
			ScriptPlayable<ActivationControlPlayable> scriptPlayable = ActivationControlPlayable.Create(graph, root, this.postPlayback);
			if (scriptPlayable.IsValid<ScriptPlayable<ActivationControlPlayable>>())
			{
				outplayables.Add(scriptPlayable);
			}
		}

		private void SearchHiearchyAndConnectParticleSystem(IEnumerable<ParticleSystem> particleSystems, PlayableGraph graph, List<Playable> outplayables)
		{
			foreach (ParticleSystem particleSystem in particleSystems)
			{
				if (particleSystem != null)
				{
					outplayables.Add(ParticleControlPlayable.Create(graph, particleSystem, this.particleRandomSeed));
				}
			}
		}

		private void SearchHierarchyAndConnectDirector(IEnumerable<PlayableDirector> directors, PlayableGraph graph, List<Playable> outplayables, bool disableSelfReferences)
		{
			foreach (PlayableDirector playableDirector in directors)
			{
				if (playableDirector != null)
				{
					if (playableDirector.playableAsset != this.m_ControlDirectorAsset)
					{
						outplayables.Add(DirectorControlPlayable.Create(graph, playableDirector));
					}
					else if (disableSelfReferences)
					{
						playableDirector.enabled = false;
					}
				}
			}
		}

		private static void SearchHierarchyAndConnectControlableScripts(IEnumerable<MonoBehaviour> controlableScripts, PlayableGraph graph, List<Playable> outplayables)
		{
			foreach (MonoBehaviour monoBehaviour in controlableScripts)
			{
				outplayables.Add(TimeControlPlayable.Create(graph, (ITimeControl)monoBehaviour));
			}
		}

		private static void ConnectMixerAndPlayable(PlayableGraph graph, Playable mixer, Playable playable, int portIndex)
		{
			graph.Connect<Playable, Playable>(playable, 0, mixer, portIndex);
			mixer.SetInputWeight(playable, 1f);
		}

		internal IList<T> GetComponent<T>(GameObject gameObject)
		{
			List<T> list = new List<T>();
			if (gameObject != null)
			{
				if (this.searchHierarchy)
				{
					gameObject.GetComponentsInChildren<T>(true, list);
				}
				else
				{
					gameObject.GetComponents<T>(list);
				}
			}
			return list;
		}

		private static IEnumerable<MonoBehaviour> GetControlableScripts(GameObject root)
		{
			if (root == null)
			{
				yield break;
			}
			foreach (MonoBehaviour script in root.GetComponentsInChildren<MonoBehaviour>())
			{
				if (script is ITimeControl)
				{
					yield return script;
				}
			}
			yield break;
		}

		private void UpdateDurationAndLoopFlag(IList<PlayableDirector> directors, IList<ParticleSystem> particleSystems)
		{
			if (directors.Count != 0 || particleSystems.Count != 0)
			{
				double num = double.NegativeInfinity;
				bool flag = false;
				foreach (PlayableDirector playableDirector in directors)
				{
					if (playableDirector.playableAsset != null)
					{
						double num2 = playableDirector.playableAsset.duration;
						if (playableDirector.playableAsset is TimelineAsset && num2 > 0.0)
						{
							num2 = (double)((DiscreteTime)num2).OneTickAfter();
						}
						num = Math.Max(num, num2);
						flag = flag || playableDirector.extrapolationMode == DirectorWrapMode.Loop;
					}
				}
				foreach (ParticleSystem particleSystem in particleSystems)
				{
					num = Math.Max(num, (double)particleSystem.main.duration);
					flag = flag || particleSystem.main.loop;
				}
				this.m_Duration = ((!double.IsNegativeInfinity(num)) ? num : PlayableBinding.DefaultDuration);
				this.m_SupportLoop = flag;
			}
		}

		private IList<ParticleSystem> GetParticleSystemRoots(GameObject go)
		{
			IList<ParticleSystem> list2;
			if (this.searchHierarchy)
			{
				List<ParticleSystem> list = new List<ParticleSystem>();
				ControlPlayableAsset.GetParticleSystemRoots(go.transform, list);
				list2 = list;
			}
			else
			{
				list2 = this.GetComponent<ParticleSystem>(go);
			}
			return list2;
		}

		private static void GetParticleSystemRoots(Transform t, ICollection<ParticleSystem> roots)
		{
			ParticleSystem component = t.GetComponent<ParticleSystem>();
			if (component != null)
			{
				roots.Add(component);
			}
			else
			{
				for (int i = 0; i < t.childCount; i++)
				{
					ControlPlayableAsset.GetParticleSystemRoots(t.GetChild(i), roots);
				}
			}
		}

		public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
		{
			if (!(director == null))
			{
				if (!ControlPlayableAsset.s_ProcessedDirectors.Contains(director))
				{
					ControlPlayableAsset.s_ProcessedDirectors.Add(director);
					GameObject gameObject = this.sourceGameObject.Resolve(director);
					if (gameObject != null)
					{
						if (this.updateParticle)
						{
							foreach (ParticleSystem particleSystem in gameObject.GetComponentsInChildren<ParticleSystem>(true))
							{
								driver.AddFromName<ParticleSystem>(particleSystem.gameObject, "randomSeed");
								driver.AddFromName<ParticleSystem>(particleSystem.gameObject, "autoRandomSeed");
							}
						}
						if (this.active)
						{
							driver.AddFromName(gameObject, "m_IsActive");
						}
						if (this.updateITimeControl)
						{
							foreach (MonoBehaviour monoBehaviour in ControlPlayableAsset.GetControlableScripts(gameObject))
							{
								IPropertyPreview propertyPreview = monoBehaviour as IPropertyPreview;
								if (propertyPreview != null)
								{
									propertyPreview.GatherProperties(director, driver);
								}
								else
								{
									driver.AddFromComponent(monoBehaviour.gameObject, monoBehaviour);
								}
							}
						}
						if (this.updateDirector)
						{
							foreach (PlayableDirector playableDirector in this.GetComponent<PlayableDirector>(gameObject))
							{
								if (!(playableDirector == null))
								{
									TimelineAsset timelineAsset = playableDirector.playableAsset as TimelineAsset;
									if (!(timelineAsset == null))
									{
										timelineAsset.GatherProperties(playableDirector, driver);
									}
								}
							}
						}
					}
					ControlPlayableAsset.s_ProcessedDirectors.Remove(director);
				}
			}
		}

		void IClipInitializer.OnCreate(TimelineClip newClip, TrackAsset track, IExposedPropertyTable table)
		{
			GameObject gameObject = null;
			if (table != null)
			{
				gameObject = this.sourceGameObject.Resolve(table);
			}
			if (gameObject == null && this.prefabGameObject != null)
			{
				gameObject = this.prefabGameObject;
			}
			if (gameObject)
			{
				IList<PlayableDirector> component = this.GetComponent<PlayableDirector>(gameObject);
				IList<ParticleSystem> component2 = this.GetComponent<ParticleSystem>(gameObject);
				this.UpdateDurationAndLoopFlag(component, component2);
				newClip.displayName = gameObject.name;
			}
		}

		IList<PlayableDirector> IDirectorDriver.GetDrivenDirectors(IExposedPropertyTable resolver)
		{
			IList<PlayableDirector> list;
			if (!this.updateDirector || this.prefabGameObject != null || resolver == null)
			{
				list = new List<PlayableDirector>(0);
			}
			else
			{
				GameObject gameObject = this.sourceGameObject.Resolve(resolver);
				if (gameObject == null)
				{
					list = new List<PlayableDirector>(0);
				}
				else
				{
					List<PlayableDirector> list2 = new List<PlayableDirector>();
					foreach (PlayableDirector playableDirector in this.GetComponent<PlayableDirector>(gameObject))
					{
						if (playableDirector.playableAsset is TimelineAsset)
						{
							list2.Add(playableDirector);
						}
					}
					list = list2;
				}
			}
			return list;
		}

		private const int k_MaxRandInt = 10000;

		private static readonly List<PlayableDirector> k_EmptyDirectorsList = new List<PlayableDirector>(0);

		private static readonly List<ParticleSystem> k_EmptyParticlesList = new List<ParticleSystem>(0);

		[SerializeField]
		public ExposedReference<GameObject> sourceGameObject;

		[SerializeField]
		public GameObject prefabGameObject;

		[SerializeField]
		public bool updateParticle = true;

		[SerializeField]
		public uint particleRandomSeed;

		[SerializeField]
		public bool updateDirector = true;

		[SerializeField]
		public bool updateITimeControl = true;

		[SerializeField]
		public bool searchHierarchy = true;

		[SerializeField]
		public bool active = true;

		[SerializeField]
		public ActivationControlPlayable.PostPlaybackState postPlayback = ActivationControlPlayable.PostPlaybackState.Revert;

		private PlayableAsset m_ControlDirectorAsset;

		private double m_Duration = PlayableBinding.DefaultDuration;

		private bool m_SupportLoop;

		private static HashSet<PlayableDirector> s_ProcessedDirectors = new HashSet<PlayableDirector>();

		private static HashSet<GameObject> s_CreatedPrefabs = new HashSet<GameObject>();
	}
}
