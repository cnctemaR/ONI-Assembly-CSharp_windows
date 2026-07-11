using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Events;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement
{
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Export/SceneManager/SceneManager.bindings.h")]
	public class SceneManager
	{
		public static extern int sceneCount
		{
			[NativeHeader("Runtime/SceneManager/SceneManager.h")]
			[StaticAccessor("GetSceneManager()", StaticAccessorType.Dot)]
			[NativeMethod("GetSceneCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int sceneCountInBuildSettings
		{
			[NativeMethod("GetNumScenesInBuildSettings")]
			[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static Scene GetActiveScene()
		{
			Scene scene;
			SceneManager.GetActiveScene_Injected(out scene);
			return scene;
		}

		[NativeThrows]
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static bool SetActiveScene(Scene scene)
		{
			return SceneManager.SetActiveScene_Injected(ref scene);
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static Scene GetSceneByPath(string scenePath)
		{
			Scene scene;
			SceneManager.GetSceneByPath_Injected(scenePath, out scene);
			return scene;
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static Scene GetSceneByName(string name)
		{
			Scene scene;
			SceneManager.GetSceneByName_Injected(name, out scene);
			return scene;
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		public static Scene GetSceneByBuildIndex(int buildIndex)
		{
			Scene scene;
			SceneManager.GetSceneByBuildIndex_Injected(buildIndex, out scene);
			return scene;
		}

		[NativeThrows]
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static Scene GetSceneAt(int index)
		{
			Scene scene;
			SceneManager.GetSceneAt_Injected(index, out scene);
			return scene;
		}

		[NativeThrows]
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static Scene CreateScene([NotNull] string sceneName, CreateSceneParameters parameters)
		{
			Scene scene;
			SceneManager.CreateScene_Injected(sceneName, ref parameters, out scene);
			return scene;
		}

		[NativeThrows]
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		private static bool UnloadSceneInternal(Scene scene, UnloadSceneOptions options)
		{
			return SceneManager.UnloadSceneInternal_Injected(ref scene, options);
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		private static AsyncOperation UnloadSceneAsyncInternal(Scene scene, UnloadSceneOptions options)
		{
			return SceneManager.UnloadSceneAsyncInternal_Injected(ref scene, options);
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		private static AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, LoadSceneParameters parameters, bool mustCompleteNextFrame)
		{
			return SceneManager.LoadSceneAsyncNameIndexInternal_Injected(sceneName, sceneBuildIndex, ref parameters, mustCompleteNextFrame);
		}

		[NativeThrows]
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncOperation UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex, bool immediately, UnloadSceneOptions options, out bool outSuccess);

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		public static void MergeScenes(Scene sourceScene, Scene destinationScene)
		{
			SceneManager.MergeScenes_Injected(ref sourceScene, ref destinationScene);
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		public static void MoveGameObjectToScene([NotNull] GameObject go, Scene scene)
		{
			SceneManager.MoveGameObjectToScene_Injected(go, ref scene);
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event UnityAction<Scene, LoadSceneMode> sceneLoaded;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event UnityAction<Scene> sceneUnloaded;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event UnityAction<Scene, Scene> activeSceneChanged;

		[Obsolete("Use SceneManager.sceneCount and SceneManager.GetSceneAt(int index) to loop the all scenes instead.")]
		public static Scene[] GetAllScenes()
		{
			Scene[] array = new Scene[SceneManager.sceneCount];
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				array[i] = SceneManager.GetSceneAt(i);
			}
			return array;
		}

		public static Scene CreateScene(string sceneName)
		{
			CreateSceneParameters createSceneParameters = new CreateSceneParameters(LocalPhysicsMode.None);
			return SceneManager.CreateScene(sceneName, createSceneParameters);
		}

		public static void LoadScene(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			LoadSceneParameters loadSceneParameters = new LoadSceneParameters(mode);
			SceneManager.LoadScene(sceneName, loadSceneParameters);
		}

		[ExcludeFromDocs]
		public static void LoadScene(string sceneName)
		{
			LoadSceneParameters loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single);
			SceneManager.LoadScene(sceneName, loadSceneParameters);
		}

		public static Scene LoadScene(string sceneName, LoadSceneParameters parameters)
		{
			SceneManager.LoadSceneAsyncNameIndexInternal(sceneName, -1, parameters, true);
			return SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
		}

		public static void LoadScene(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			LoadSceneParameters loadSceneParameters = new LoadSceneParameters(mode);
			SceneManager.LoadScene(sceneBuildIndex, loadSceneParameters);
		}

		[ExcludeFromDocs]
		public static void LoadScene(int sceneBuildIndex)
		{
			LoadSceneParameters loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single);
			SceneManager.LoadScene(sceneBuildIndex, loadSceneParameters);
		}

		public static Scene LoadScene(int sceneBuildIndex, LoadSceneParameters parameters)
		{
			SceneManager.LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, parameters, true);
			return SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
		}

		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			LoadSceneParameters loadSceneParameters = new LoadSceneParameters(mode);
			return SceneManager.LoadSceneAsync(sceneBuildIndex, loadSceneParameters);
		}

		[ExcludeFromDocs]
		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex)
		{
			LoadSceneParameters loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single);
			return SceneManager.LoadSceneAsync(sceneBuildIndex, loadSceneParameters);
		}

		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, LoadSceneParameters parameters)
		{
			return SceneManager.LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, parameters, false);
		}

		public static AsyncOperation LoadSceneAsync(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			LoadSceneParameters loadSceneParameters = new LoadSceneParameters(mode);
			return SceneManager.LoadSceneAsync(sceneName, loadSceneParameters);
		}

		[ExcludeFromDocs]
		public static AsyncOperation LoadSceneAsync(string sceneName)
		{
			LoadSceneParameters loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single);
			return SceneManager.LoadSceneAsync(sceneName, loadSceneParameters);
		}

		public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneParameters parameters)
		{
			return SceneManager.LoadSceneAsyncNameIndexInternal(sceneName, -1, parameters, false);
		}

		[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
		public static bool UnloadScene(Scene scene)
		{
			return SceneManager.UnloadSceneInternal(scene, UnloadSceneOptions.None);
		}

		[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
		public static bool UnloadScene(int sceneBuildIndex)
		{
			bool flag;
			SceneManager.UnloadSceneNameIndexInternal("", sceneBuildIndex, true, UnloadSceneOptions.None, out flag);
			return flag;
		}

		[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
		public static bool UnloadScene(string sceneName)
		{
			bool flag;
			SceneManager.UnloadSceneNameIndexInternal(sceneName, -1, true, UnloadSceneOptions.None, out flag);
			return flag;
		}

		public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal("", sceneBuildIndex, false, UnloadSceneOptions.None, out flag);
		}

		public static AsyncOperation UnloadSceneAsync(string sceneName)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal(sceneName, -1, false, UnloadSceneOptions.None, out flag);
		}

		public static AsyncOperation UnloadSceneAsync(Scene scene)
		{
			return SceneManager.UnloadSceneAsyncInternal(scene, UnloadSceneOptions.None);
		}

		public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex, UnloadSceneOptions options)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal("", sceneBuildIndex, false, options, out flag);
		}

		public static AsyncOperation UnloadSceneAsync(string sceneName, UnloadSceneOptions options)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal(sceneName, -1, false, options, out flag);
		}

		public static AsyncOperation UnloadSceneAsync(Scene scene, UnloadSceneOptions options)
		{
			return SceneManager.UnloadSceneAsyncInternal(scene, options);
		}

		[RequiredByNativeCode]
		private static void Internal_SceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (SceneManager.sceneLoaded != null)
			{
				SceneManager.sceneLoaded(scene, mode);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_SceneUnloaded(Scene scene)
		{
			if (SceneManager.sceneUnloaded != null)
			{
				SceneManager.sceneUnloaded(scene);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_ActiveSceneChanged(Scene previousActiveScene, Scene newActiveScene)
		{
			if (SceneManager.activeSceneChanged != null)
			{
				SceneManager.activeSceneChanged(previousActiveScene, newActiveScene);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActiveScene_Injected(out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetActiveScene_Injected(ref Scene scene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSceneByPath_Injected(string scenePath, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSceneByName_Injected(string name, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSceneByBuildIndex_Injected(int buildIndex, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSceneAt_Injected(int index, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateScene_Injected(string sceneName, ref CreateSceneParameters parameters, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UnloadSceneInternal_Injected(ref Scene scene, UnloadSceneOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncOperation UnloadSceneAsyncInternal_Injected(ref Scene scene, UnloadSceneOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncOperation LoadSceneAsyncNameIndexInternal_Injected(string sceneName, int sceneBuildIndex, ref LoadSceneParameters parameters, bool mustCompleteNextFrame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MergeScenes_Injected(ref Scene sourceScene, ref Scene destinationScene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveGameObjectToScene_Injected(GameObject go, ref Scene scene);
	}
}
