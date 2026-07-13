using System;
using UnityEngine;

public class SceneInitializerLoader : MonoBehaviour
{
	private void Awake()
	{
		Camera[] array = global::UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		KMonoBehaviour.isLoadingScene = false;
		Singleton<StateMachineManager>.Instance.Clear();
		Util.KInstantiate(this.sceneInitializer, null, null);
		if (SceneInitializerLoader.ReportDeferredError != null && SceneInitializerLoader.deferred_error.IsValid)
		{
			SceneInitializerLoader.ReportDeferredError(SceneInitializerLoader.deferred_error);
			SceneInitializerLoader.deferred_error = default(SceneInitializerLoader.DeferredError);
		}
	}

	public SceneInitializer sceneInitializer;

	public static SceneInitializerLoader.DeferredError deferred_error;

	public static SceneInitializerLoader.DeferredErrorDelegate ReportDeferredError;

	public struct DeferredError
	{
		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.msg);
			}
		}

		public string msg;

		public string stack_trace;
	}

	public delegate void DeferredErrorDelegate(SceneInitializerLoader.DeferredError deferred_error);
}
