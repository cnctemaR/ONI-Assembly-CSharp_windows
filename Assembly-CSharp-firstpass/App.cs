using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Klei;
using UnityEngine;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour
{
	static App()
	{
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			try
			{
				foreach (Type type in assembly.GetTypes())
				{
					App.types.Add(type);
				}
			}
			catch (Exception)
			{
			}
		}
	}

	public static string GetCurrentSceneName()
	{
		return App.currentSceneName;
	}

	private void OnApplicationQuit()
	{
		App.IsExiting = true;
	}

	private void Awake()
	{
		App.instance = this;
	}

	public static void LoadScene(string scene_name)
	{
		KMonoBehaviour.isLoadingScene = true;
		App.isLoading = true;
		App.loadingSceneName = scene_name;
	}

	private void OnApplicationFocus(bool focus)
	{
		App.hasFocus = focus;
		this.lastSuspendTime = Time.realtimeSinceStartup;
	}

	public void LateUpdate()
	{
		if (App.isLoading)
		{
			KObjectManager.Instance.Cleanup();
			KMonoBehaviour.lastGameObject = null;
			KMonoBehaviour.lastObj = null;
			if (SimAndRenderScheduler.instance != null)
			{
				SimAndRenderScheduler.instance.Reset();
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
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
		if (!App.hasFocus && GenericGameSettings.instance.sleepWhenOutOfFocus)
		{
			float num = (Time.realtimeSinceStartup - this.lastSuspendTime) * 1000f;
			float num2 = 0f;
			for (int i = 0; i < App.sleepIntervals.Length; i++)
			{
				num2 = App.sleepIntervals[i];
				if (num2 > num)
				{
					break;
				}
			}
			float num3 = num2 - num;
			num3 = Mathf.Max(0f, num3);
			Thread.Sleep((int)num3);
			this.lastSuspendTime = Time.realtimeSinceStartup;
		}
	}

	private void OnDestroy()
	{
		GlobalJobManager.Cleanup();
	}

	public static List<Type> GetCurrentDomainTypes()
	{
		return App.types;
	}

	public static App instance;

	public static bool IsExiting = false;

	public static global::System.Action OnPreLoadScene;

	public static global::System.Action OnPostLoadScene;

	public static bool isLoading = false;

	public static bool hasFocus = true;

	public static string loadingSceneName = null;

	private static string currentSceneName = null;

	private float lastSuspendTime;

	private static List<Type> types = new List<Type>();

	private static float[] sleepIntervals = new float[] { 8.333333f, 16.666666f, 33.333332f };
}
