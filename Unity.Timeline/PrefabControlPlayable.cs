using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	public class PrefabControlPlayable : PlayableBehaviour
	{
		public static ScriptPlayable<PrefabControlPlayable> Create(PlayableGraph graph, GameObject prefabGameObject, Transform parentTransform)
		{
			if (prefabGameObject == null)
			{
				return ScriptPlayable<PrefabControlPlayable>.Null;
			}
			ScriptPlayable<PrefabControlPlayable> scriptPlayable = ScriptPlayable<PrefabControlPlayable>.Create(graph, 0);
			scriptPlayable.GetBehaviour().Initialize(prefabGameObject, parentTransform);
			return scriptPlayable;
		}

		public GameObject prefabInstance
		{
			get
			{
				return this.m_Instance;
			}
		}

		public GameObject Initialize(GameObject prefabGameObject, Transform parentTransform)
		{
			if (prefabGameObject == null)
			{
				throw new ArgumentNullException("Prefab cannot be null");
			}
			if (this.m_Instance != null)
			{
				Debug.LogWarningFormat("Prefab Control Playable ({0}) has already been initialized with a Prefab ({1}).", new object[]
				{
					prefabGameObject.name,
					this.m_Instance.name
				});
			}
			else
			{
				this.m_Instance = Object.Instantiate<GameObject>(prefabGameObject, parentTransform, false);
				this.m_Instance.name = prefabGameObject.name + " [Timeline]";
				this.m_Instance.SetActive(false);
				PrefabControlPlayable.SetHideFlagsRecursive(this.m_Instance);
			}
			return this.m_Instance;
		}

		public override void OnPlayableDestroy(Playable playable)
		{
			if (this.m_Instance)
			{
				if (Application.isPlaying)
				{
					Object.Destroy(this.m_Instance);
					return;
				}
				Object.DestroyImmediate(this.m_Instance);
			}
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (this.m_Instance == null)
			{
				return;
			}
			this.m_Instance.SetActive(true);
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (this.m_Instance != null && info.effectivePlayState == PlayState.Paused)
			{
				this.m_Instance.SetActive(false);
			}
		}

		private static void SetHideFlagsRecursive(GameObject gameObject)
		{
			if (gameObject == null)
			{
				return;
			}
			gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
			if (!Application.isPlaying)
			{
				gameObject.hideFlags |= HideFlags.HideInHierarchy;
			}
			foreach (object obj in gameObject.transform)
			{
				PrefabControlPlayable.SetHideFlagsRecursive(((Transform)obj).gameObject);
			}
		}

		private GameObject m_Instance;
	}
}
