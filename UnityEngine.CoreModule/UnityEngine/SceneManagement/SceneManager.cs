using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Events;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement
{
	/// <summary>
	///   <para>Scene management at run-time.</para>
	/// </summary>
	[NativeHeader("Runtime/Export/SceneManager/SceneManager.bindings.h")]
	[RequiredByNativeCode]
	public class SceneManager
	{
		/// <summary>
		///   <para>The total number of currently loaded Scenes.</para>
		/// </summary>
		public static extern int sceneCount
		{
			[NativeHeader("Runtime/SceneManager/SceneManager.h")]
			[StaticAccessor("GetSceneManager()", StaticAccessorType.Dot)]
			[NativeMethod("GetSceneCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Number of Scenes in Build Settings.</para>
		/// </summary>
		public static extern int sceneCountInBuildSettings
		{
			[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
			[NativeMethod("GetNumScenesInBuildSettings")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Gets the currently active Scene.</para>
		/// </summary>
		/// <returns>
		///   <para>The active Scene.</para>
		/// </returns>
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static Scene GetActiveScene()
		{
			Scene scene;
			SceneManager.GetActiveScene_Injected(out scene);
			return scene;
		}

		/// <summary>
		///   <para>Set the Scene to be active.</para>
		/// </summary>
		/// <param name="scene">The Scene to be set.</param>
		/// <returns>
		///   <para>Returns false if the Scene is not loaded yet.</para>
		/// </returns>
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		public static bool SetActiveScene(Scene scene)
		{
			return SceneManager.SetActiveScene_Injected(ref scene);
		}

		/// <summary>
		///   <para>Searches all Scenes loaded for a Scene that has the given asset path.</para>
		/// </summary>
		/// <param name="scenePath">Path of the Scene. Should be relative to the project folder. Like: "AssetsMyScenesMyScene.unity".</param>
		/// <returns>
		///   <para>A reference to the Scene, if valid. If not, an invalid Scene is returned.</para>
		/// </returns>
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static Scene GetSceneByPath(string scenePath)
		{
			Scene scene;
			SceneManager.GetSceneByPath_Injected(scenePath, out scene);
			return scene;
		}

		/// <summary>
		///   <para>Searches through the Scenes loaded for a Scene with the given name.</para>
		/// </summary>
		/// <param name="name">Name of Scene to find.</param>
		/// <returns>
		///   <para>A reference to the Scene, if valid. If not, an invalid Scene is returned.</para>
		/// </returns>
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static Scene GetSceneByName(string name)
		{
			Scene scene;
			SceneManager.GetSceneByName_Injected(name, out scene);
			return scene;
		}

		/// <summary>
		///   <para>Get a Scene struct from a build index.</para>
		/// </summary>
		/// <param name="buildIndex">Build index as shown in the Build Settings window.</param>
		/// <returns>
		///   <para>A reference to the Scene, if valid. If not, an invalid Scene is returned.</para>
		/// </returns>
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		public static Scene GetSceneByBuildIndex(int buildIndex)
		{
			Scene scene;
			SceneManager.GetSceneByBuildIndex_Injected(buildIndex, out scene);
			return scene;
		}

		/// <summary>
		///   <para>Get the Scene at index in the SceneManager's list of loaded Scenes.</para>
		/// </summary>
		/// <param name="index">Index of the Scene to get. Index must be greater than or equal to 0 and less than SceneManager.sceneCount.</param>
		/// <returns>
		///   <para>A reference to the Scene at the index specified.</para>
		/// </returns>
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		public static Scene GetSceneAt(int index)
		{
			Scene scene;
			SceneManager.GetSceneAt_Injected(index, out scene);
			return scene;
		}

		/// <summary>
		///   <para>Create an empty new Scene at runtime with the given name.</para>
		/// </summary>
		/// <param name="sceneName">The name of the new Scene. It cannot be empty or null, or same as the name of the existing Scenes.</param>
		/// <returns>
		///   <para>A reference to the new Scene that was created, or an invalid Scene if creation failed.</para>
		/// </returns>
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		public static Scene CreateScene([NotNull] string sceneName)
		{
			Scene scene;
			SceneManager.CreateScene_Injected(sceneName, out scene);
			return scene;
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		private static bool UnloadSceneInternal(Scene scene)
		{
			return SceneManager.UnloadSceneInternal_Injected(ref scene);
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		private static AsyncOperation UnloadSceneAsyncInternal(Scene scene)
		{
			return SceneManager.UnloadSceneAsyncInternal_Injected(ref scene);
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, bool isAdditive, bool mustCompleteNextFrame);

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncOperation UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex, bool immediately, out bool outSuccess);

		/// <summary>
		///   <para>This will merge the source Scene into the destinationScene.</para>
		/// </summary>
		/// <param name="sourceScene">The Scene that will be merged into the destination Scene.</param>
		/// <param name="destinationScene">Existing Scene to merge the source Scene into.</param>
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		public static void MergeScenes(Scene sourceScene, Scene destinationScene)
		{
			SceneManager.MergeScenes_Injected(ref sourceScene, ref destinationScene);
		}

		/// <summary>
		///   <para>Move a GameObject from its current Scene to a new Scene.</para>
		/// </summary>
		/// <param name="go">GameObject to move.</param>
		/// <param name="scene">Scene to move into.</param>
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

		/// <summary>
		///   <para>Returns an array of all the Scenes currently open in the hierarchy.</para>
		/// </summary>
		/// <returns>
		///   <para>Array of Scenes in the Hierarchy.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Loads the Scene by its name or index in Build Settings.</para>
		/// </summary>
		/// <param name="sceneName">Name or path of the Scene to load.</param>
		/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to load.</param>
		/// <param name="mode">Allows you to specify whether or not to load the Scene additively. See SceneManagement.LoadSceneMode for more information about the options.</param>
		public static void LoadScene(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			SceneManager.LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, true);
		}

		[ExcludeFromDocs]
		public static void LoadScene(string sceneName)
		{
			LoadSceneMode loadSceneMode = LoadSceneMode.Single;
			SceneManager.LoadScene(sceneName, loadSceneMode);
		}

		/// <summary>
		///   <para>Loads the Scene by its name or index in Build Settings.</para>
		/// </summary>
		/// <param name="sceneName">Name or path of the Scene to load.</param>
		/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to load.</param>
		/// <param name="mode">Allows you to specify whether or not to load the Scene additively. See SceneManagement.LoadSceneMode for more information about the options.</param>
		public static void LoadScene(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			SceneManager.LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, mode == LoadSceneMode.Additive, true);
		}

		[ExcludeFromDocs]
		public static void LoadScene(int sceneBuildIndex)
		{
			LoadSceneMode loadSceneMode = LoadSceneMode.Single;
			SceneManager.LoadScene(sceneBuildIndex, loadSceneMode);
		}

		/// <summary>
		///   <para>Loads the Scene asynchronously in the background.</para>
		/// </summary>
		/// <param name="sceneName">Name or path of the Scene to load.</param>
		/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to load.</param>
		/// <param name="mode">If LoadSceneMode.Single then all current Scenes will be unloaded before loading.</param>
		/// <returns>
		///   <para>Use the AsyncOperation to determine if the operation has completed.</para>
		/// </returns>
		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			return SceneManager.LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, mode == LoadSceneMode.Additive, false);
		}

		[ExcludeFromDocs]
		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex)
		{
			LoadSceneMode loadSceneMode = LoadSceneMode.Single;
			return SceneManager.LoadSceneAsync(sceneBuildIndex, loadSceneMode);
		}

		/// <summary>
		///   <para>Loads the Scene asynchronously in the background.</para>
		/// </summary>
		/// <param name="sceneName">Name or path of the Scene to load.</param>
		/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to load.</param>
		/// <param name="mode">If LoadSceneMode.Single then all current Scenes will be unloaded before loading.</param>
		/// <returns>
		///   <para>Use the AsyncOperation to determine if the operation has completed.</para>
		/// </returns>
		public static AsyncOperation LoadSceneAsync(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			return SceneManager.LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, false);
		}

		[ExcludeFromDocs]
		public static AsyncOperation LoadSceneAsync(string sceneName)
		{
			LoadSceneMode loadSceneMode = LoadSceneMode.Single;
			return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
		}

		/// <summary>
		///   <para>Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager.</para>
		/// </summary>
		/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to unload.</param>
		/// <param name="sceneName">Name or path of the Scene to unload.</param>
		/// <param name="scene">Scene to unload.</param>
		/// <returns>
		///   <para>Returns true if the Scene is unloaded.</para>
		/// </returns>
		[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
		public static bool UnloadScene(Scene scene)
		{
			return SceneManager.UnloadSceneInternal(scene);
		}

		/// <summary>
		///   <para>Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager.</para>
		/// </summary>
		/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to unload.</param>
		/// <param name="sceneName">Name or path of the Scene to unload.</param>
		/// <param name="scene">Scene to unload.</param>
		/// <returns>
		///   <para>Returns true if the Scene is unloaded.</para>
		/// </returns>
		[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
		public static bool UnloadScene(int sceneBuildIndex)
		{
			bool flag;
			SceneManager.UnloadSceneNameIndexInternal("", sceneBuildIndex, true, out flag);
			return flag;
		}

		/// <summary>
		///   <para>Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager.</para>
		/// </summary>
		/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to unload.</param>
		/// <param name="sceneName">Name or path of the Scene to unload.</param>
		/// <param name="scene">Scene to unload.</param>
		/// <returns>
		///   <para>Returns true if the Scene is unloaded.</para>
		/// </returns>
		[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
		public static bool UnloadScene(string sceneName)
		{
			bool flag;
			SceneManager.UnloadSceneNameIndexInternal(sceneName, -1, true, out flag);
			return flag;
		}

		/// <summary>
		///   <para>Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager.</para>
		/// </summary>
		/// <param name="sceneBuildIndex">Index of the Scene in BuildSettings.</param>
		/// <param name="sceneName">Name or path of the Scene to unload.</param>
		/// <param name="scene">Scene to unload.</param>
		/// <returns>
		///   <para>Use the AsyncOperation to determine if the operation has completed.</para>
		/// </returns>
		public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal("", sceneBuildIndex, false, out flag);
		}

		/// <summary>
		///   <para>Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager.</para>
		/// </summary>
		/// <param name="sceneBuildIndex">Index of the Scene in BuildSettings.</param>
		/// <param name="sceneName">Name or path of the Scene to unload.</param>
		/// <param name="scene">Scene to unload.</param>
		/// <returns>
		///   <para>Use the AsyncOperation to determine if the operation has completed.</para>
		/// </returns>
		public static AsyncOperation UnloadSceneAsync(string sceneName)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal(sceneName, -1, false, out flag);
		}

		/// <summary>
		///   <para>Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager.</para>
		/// </summary>
		/// <param name="sceneBuildIndex">Index of the Scene in BuildSettings.</param>
		/// <param name="sceneName">Name or path of the Scene to unload.</param>
		/// <param name="scene">Scene to unload.</param>
		/// <returns>
		///   <para>Use the AsyncOperation to determine if the operation has completed.</para>
		/// </returns>
		public static AsyncOperation UnloadSceneAsync(Scene scene)
		{
			return SceneManager.UnloadSceneAsyncInternal(scene);
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
		private static extern void CreateScene_Injected(string sceneName, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UnloadSceneInternal_Injected(ref Scene scene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncOperation UnloadSceneAsyncInternal_Injected(ref Scene scene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MergeScenes_Injected(ref Scene sourceScene, ref Scene destinationScene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveGameObjectToScene_Injected(GameObject go, ref Scene scene);
	}
}
