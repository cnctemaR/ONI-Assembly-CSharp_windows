using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	[NotKeyable]
	[Serializable]
	public class ControlPlayableAsset : PlayableAsset, IPropertyPreview, ITimelineClipAsset
	{
		internal bool controllingDirectors { get; private set; }

		internal bool controllingParticles { get; private set; }

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
				return ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | (this.m_SupportLoop ? ClipCaps.Looping : ClipCaps.None);
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
				Transform transform = ((gameObject != null) ? gameObject.transform : null);
				ScriptPlayable<PrefabControlPlayable> scriptPlayable = PrefabControlPlayable.Create(graph, this.prefabGameObject, transform);
				gameObject = scriptPlayable.GetBehaviour().prefabInstance;
				list.Add(scriptPlayable);
			}
			this.m_Duration = PlayableBinding.DefaultDuration;
			this.m_SupportLoop = false;
			this.controllingParticles = false;
			this.controllingDirectors = false;
			if (gameObject != null)
			{
				IList<PlayableDirector> list3;
				if (!this.updateDirector)
				{
					IList<PlayableDirector> list2 = ControlPlayableAsset.k_EmptyDirectorsList;
					list3 = list2;
				}
				else
				{
					list3 = this.GetComponent<PlayableDirector>(gameObject);
				}
				IList<PlayableDirector> list4 = list3;
				IList<ParticleSystem> list6;
				if (!this.updateParticle)
				{
					IList<ParticleSystem> list5 = ControlPlayableAsset.k_EmptyParticlesList;
					list6 = list5;
				}
				else
				{
					list6 = this.GetControllableParticleSystems(gameObject);
				}
				IList<ParticleSystem> list7 = list6;
				this.UpdateDurationAndLoopFlag(list4, list7);
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
					this.SearchHierarchyAndConnectDirector(list4, graph, list, this.prefabGameObject != null);
				}
				if (this.updateParticle)
				{
					this.SearchHierarchyAndConnectParticleSystem(list7, graph, list);
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

		private void SearchHierarchyAndConnectParticleSystem(IEnumerable<ParticleSystem> particleSystems, PlayableGraph graph, List<Playable> outplayables)
		{
			foreach (ParticleSystem particleSystem in particleSystems)
			{
				if (particleSystem != null)
				{
					this.controllingParticles = true;
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
						this.controllingDirectors = true;
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

		internal static IEnumerable<MonoBehaviour> GetControlableScripts(GameObject root)
		{
			if (root == null)
			{
				yield break;
			}
			foreach (MonoBehaviour monoBehaviour in root.GetComponentsInChildren<MonoBehaviour>())
			{
				if (monoBehaviour is ITimeControl)
				{
					yield return monoBehaviour;
				}
			}
			MonoBehaviour[] array = null;
			yield break;
		}

		internal void UpdateDurationAndLoopFlag(IList<PlayableDirector> directors, IList<ParticleSystem> particleSystems)
		{
			if (directors.Count == 0 && particleSystems.Count == 0)
			{
				return;
			}
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
			this.m_Duration = (double.IsNegativeInfinity(num) ? PlayableBinding.DefaultDuration : num);
			this.m_SupportLoop = flag;
		}

		private IList<ParticleSystem> GetControllableParticleSystems(GameObject go)
		{
			List<ParticleSystem> list = new List<ParticleSystem>();
			if (this.searchHierarchy || go.GetComponent<ParticleSystem>() != null)
			{
				ControlPlayableAsset.GetControllableParticleSystems(go.transform, list, ControlPlayableAsset.s_SubEmitterCollector);
				ControlPlayableAsset.s_SubEmitterCollector.Clear();
			}
			return list;
		}

		private static void GetControllableParticleSystems(Transform t, ICollection<ParticleSystem> roots, HashSet<ParticleSystem> subEmitters)
		{
			ParticleSystem component = t.GetComponent<ParticleSystem>();
			if (component != null && !subEmitters.Contains(component))
			{
				roots.Add(component);
				ControlPlayableAsset.CacheSubEmitters(component, subEmitters);
			}
			for (int i = 0; i < t.childCount; i++)
			{
				ControlPlayableAsset.GetControllableParticleSystems(t.GetChild(i), roots, subEmitters);
			}
		}

		private static void CacheSubEmitters(ParticleSystem ps, HashSet<ParticleSystem> subEmitters)
		{
			if (ps == null)
			{
				return;
			}
			for (int i = 0; i < ps.subEmitters.subEmittersCount; i++)
			{
				subEmitters.Add(ps.subEmitters.GetSubEmitterSystem(i));
			}
		}

		public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
		{
			if (director == null)
			{
				return;
			}
			if (ControlPlayableAsset.s_ProcessedDirectors.Contains(director))
			{
				return;
			}
			ControlPlayableAsset.s_ProcessedDirectors.Add(director);
			GameObject gameObject = this.sourceGameObject.Resolve(director);
			if (gameObject != null)
			{
				if (this.updateParticle)
				{
					ControlPlayableAsset.PreviewParticles(driver, gameObject.GetComponentsInChildren<ParticleSystem>(true));
				}
				if (this.active)
				{
					ControlPlayableAsset.PreviewActivation(driver, new GameObject[] { gameObject });
				}
				if (this.updateITimeControl)
				{
					ControlPlayableAsset.PreviewTimeControl(driver, director, ControlPlayableAsset.GetControlableScripts(gameObject));
				}
				if (this.updateDirector)
				{
					ControlPlayableAsset.PreviewDirectors(driver, this.GetComponent<PlayableDirector>(gameObject));
				}
			}
			ControlPlayableAsset.s_ProcessedDirectors.Remove(director);
		}

		internal static void PreviewParticles(IPropertyCollector driver, IEnumerable<ParticleSystem> particles)
		{
			foreach (ParticleSystem particleSystem in particles)
			{
				driver.AddFromName<ParticleSystem>(particleSystem.gameObject, "randomSeed");
				driver.AddFromName<ParticleSystem>(particleSystem.gameObject, "autoRandomSeed");
			}
		}

		internal static void PreviewActivation(IPropertyCollector driver, IEnumerable<GameObject> objects)
		{
			foreach (GameObject gameObject in objects)
			{
				driver.AddFromName(gameObject, "m_IsActive");
			}
		}

		internal static void PreviewTimeControl(IPropertyCollector driver, PlayableDirector director, IEnumerable<MonoBehaviour> scripts)
		{
			foreach (MonoBehaviour monoBehaviour in scripts)
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

		internal static void PreviewDirectors(IPropertyCollector driver, IEnumerable<PlayableDirector> directors)
		{
			foreach (PlayableDirector playableDirector in directors)
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

		private const int k_MaxRandInt = 10000;

		private static readonly List<PlayableDirector> k_EmptyDirectorsList = new List<PlayableDirector>(0);

		private static readonly List<ParticleSystem> k_EmptyParticlesList = new List<ParticleSystem>(0);

		private static readonly HashSet<ParticleSystem> s_SubEmitterCollector = new HashSet<ParticleSystem>();

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
		public bool searchHierarchy;

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
