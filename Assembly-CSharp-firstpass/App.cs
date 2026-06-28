using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour
{
	private void OnApplicationQuit()
	{
		App.IsExiting = true;
	}

	public static void LoadScene(string scene_name)
	{
		KMonoBehaviour.isLoadingScene = true;
		App.isLoading = true;
		App.sceneName = scene_name;
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
			SceneManager.LoadScene(App.sceneName);
			if (App.OnPostLoadScene != null)
			{
				App.OnPostLoadScene();
			}
			App.isLoading = false;
			App.sceneName = null;
		}
	}

	public static bool IsExiting;

	public static global::System.Action OnPreLoadScene;

	public static global::System.Action OnPostLoadScene;

	public static bool isLoading;

	public static string sceneName;
}
