using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour
{
	public static string GetCurrentSceneName()
	{
		return App.currentSceneName;
	}

	private void OnApplicationQuit()
	{
		App.IsExiting = true;
	}

	public static void LoadScene(string scene_name)
	{
		KMonoBehaviour.isLoadingScene = true;
		App.isLoading = true;
		App.loadingSceneName = scene_name;
	}

	public void LateUpdate()
	{
		if (App.isLoading)
		{
			KObjectManager.Instance.Cleanup();
			KGlobalAnimParser.Get().ClearDynamic();
			KAnimBatchManager.Instance().ClearMultiInstances();
			if (App.OnPreLoadScene != null)
			{
				App.OnPreLoadScene();
			}
			SceneManager.LoadScene(App.loadingSceneName);
			if (App.OnPostLoadScene != null)
			{
				App.OnPostLoadScene();
			}
			App.isLoading = false;
			App.currentSceneName = App.loadingSceneName;
			App.loadingSceneName = null;
		}
	}

	public static bool IsExiting;

	public static global::System.Action OnPreLoadScene;

	public static global::System.Action OnPostLoadScene;

	public static bool isLoading;

	public static string loadingSceneName;

	private static string currentSceneName;
}
