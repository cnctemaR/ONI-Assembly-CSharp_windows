using System;
using UnityEngine;

public class SceneInitializerLoader : MonoBehaviour
{
	private void Awake()
	{
		foreach (Camera camera in global::UnityEngine.Object.FindObjectsOfType<Camera>())
		{
			camera.enabled = false;
		}
		KMonoBehaviour.isLoadingScene = false;
		StateMachineManager.Destroy();
		Util.KInstantiate(this.sceneInitializer, null, null);
	}

	public SceneInitializer sceneInitializer;
}
