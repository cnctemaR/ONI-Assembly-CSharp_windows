using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.SceneManagement
{
	[NativeHeader("Runtime/Export/SceneManager/Scene.bindings.h")]
	[Serializable]
	public struct Scene
	{
		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static bool IsValidInternal(SceneHandle sceneHandle)
		{
			return Scene.IsValidInternal_Injected(ref sceneHandle);
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static string GetPathInternal(SceneHandle sceneHandle)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				Scene.GetPathInternal_Injected(ref sceneHandle, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private unsafe static void SetPathAndGUIDInternal(SceneHandle sceneHandle, string path, string guid)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(guid, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = guid.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				Scene.SetPathAndGUIDInternal_Injected(ref sceneHandle, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static string GetNameInternal(SceneHandle sceneHandle)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				Scene.GetNameInternal_Injected(ref sceneHandle, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		private unsafe static void SetNameInternal(SceneHandle sceneHandle, string name)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Scene.SetNameInternal_Injected(ref sceneHandle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static string GetGUIDInternal(SceneHandle sceneHandle)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				Scene.GetGUIDInternal_Injected(ref sceneHandle, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static bool IsSubScene(SceneHandle sceneHandle)
		{
			return Scene.IsSubScene_Injected(ref sceneHandle);
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static void SetIsSubScene(SceneHandle sceneHandle, bool value)
		{
			Scene.SetIsSubScene_Injected(ref sceneHandle, value);
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static bool GetIsLoadedInternal(SceneHandle sceneHandle)
		{
			return Scene.GetIsLoadedInternal_Injected(ref sceneHandle);
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static Scene.LoadingState GetLoadingStateInternal(SceneHandle sceneHandle)
		{
			return Scene.GetLoadingStateInternal_Injected(ref sceneHandle);
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static bool GetIsDirtyInternal(SceneHandle sceneHandle)
		{
			return Scene.GetIsDirtyInternal_Injected(ref sceneHandle);
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static int GetDirtyID(SceneHandle sceneHandle)
		{
			return Scene.GetDirtyID_Injected(ref sceneHandle);
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static int GetBuildIndexInternal(SceneHandle sceneHandle)
		{
			return Scene.GetBuildIndexInternal_Injected(ref sceneHandle);
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static int GetRootCountInternal(SceneHandle sceneHandle)
		{
			return Scene.GetRootCountInternal_Injected(ref sceneHandle);
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static void GetRootGameObjectsInternal(SceneHandle sceneHandle, object resultRootList)
		{
			Scene.GetRootGameObjectsInternal_Injected(ref sceneHandle, resultRootList);
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static EntityId GetDefaultParent(SceneHandle sceneHandle)
		{
			EntityId entityId;
			Scene.GetDefaultParent_Injected(ref sceneHandle, out entityId);
			return entityId;
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static void SetDefaultParent(SceneHandle sceneHandle, EntityId value)
		{
			Scene.SetDefaultParent_Injected(ref sceneHandle, ref value);
		}

		internal Scene(SceneHandle handle)
		{
			this.m_Handle = handle;
		}

		public SceneHandle handle
		{
			get
			{
				return this.m_Handle;
			}
		}

		internal Scene.LoadingState loadingState
		{
			get
			{
				return Scene.GetLoadingStateInternal(this.handle);
			}
		}

		internal string guid
		{
			get
			{
				return Scene.GetGUIDInternal(this.handle);
			}
		}

		public bool IsValid()
		{
			return Scene.IsValidInternal(this.handle);
		}

		public string path
		{
			get
			{
				return Scene.GetPathInternal(this.handle);
			}
		}

		public string name
		{
			get
			{
				return Scene.GetNameInternal(this.handle);
			}
			set
			{
				Scene.SetNameInternal(this.handle, value);
			}
		}

		public bool isLoaded
		{
			get
			{
				return Scene.GetIsLoadedInternal(this.handle);
			}
		}

		public int buildIndex
		{
			get
			{
				return Scene.GetBuildIndexInternal(this.handle);
			}
		}

		public bool isDirty
		{
			get
			{
				return Scene.GetIsDirtyInternal(this.handle);
			}
		}

		internal int dirtyID
		{
			get
			{
				return Scene.GetDirtyID(this.handle);
			}
		}

		public int rootCount
		{
			get
			{
				return Scene.GetRootCountInternal(this.handle);
			}
		}

		public bool isSubScene
		{
			get
			{
				return Scene.IsSubScene(this.handle);
			}
			set
			{
				Scene.SetIsSubScene(this.handle, value);
			}
		}

		internal EntityId defaultParent
		{
			get
			{
				return Scene.GetDefaultParent(this.handle);
			}
			set
			{
				Scene.SetDefaultParent(this.handle, value);
			}
		}

		public GameObject[] GetRootGameObjects()
		{
			List<GameObject> list = new List<GameObject>(this.rootCount);
			this.GetRootGameObjects(list);
			return list.ToArray();
		}

		public void GetRootGameObjects(List<GameObject> rootGameObjects)
		{
			bool flag = rootGameObjects.Capacity < this.rootCount;
			if (flag)
			{
				rootGameObjects.Capacity = this.rootCount;
			}
			rootGameObjects.Clear();
			bool flag2 = !this.IsValid();
			if (flag2)
			{
				throw new ArgumentException("The scene is invalid.");
			}
			bool flag3 = !Application.isPlaying && !this.isLoaded;
			if (flag3)
			{
				throw new ArgumentException("The scene is not loaded.");
			}
			bool flag4 = this.rootCount == 0;
			if (!flag4)
			{
				Scene.GetRootGameObjectsInternal(this.handle, rootGameObjects);
			}
		}

		public static bool operator ==(Scene lhs, Scene rhs)
		{
			return lhs.handle == rhs.handle;
		}

		public static bool operator !=(Scene lhs, Scene rhs)
		{
			return lhs.handle != rhs.handle;
		}

		public override int GetHashCode()
		{
			return this.m_Handle.GetHashCode();
		}

		public override bool Equals(object other)
		{
			bool flag = !(other is Scene);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				Scene scene = (Scene)other;
				flag2 = this.handle == scene.handle;
			}
			return flag2;
		}

		internal void SetPathAndGuid(string path, string guid)
		{
			Scene.SetPathAndGUIDInternal(this.m_Handle, path, guid);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValidInternal_Injected([In] ref SceneHandle sceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPathInternal_Injected([In] ref SceneHandle sceneHandle, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPathAndGUIDInternal_Injected([In] ref SceneHandle sceneHandle, ref ManagedSpanWrapper path, ref ManagedSpanWrapper guid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNameInternal_Injected([In] ref SceneHandle sceneHandle, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetNameInternal_Injected([In] ref SceneHandle sceneHandle, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGUIDInternal_Injected([In] ref SceneHandle sceneHandle, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsSubScene_Injected([In] ref SceneHandle sceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIsSubScene_Injected([In] ref SceneHandle sceneHandle, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIsLoadedInternal_Injected([In] ref SceneHandle sceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Scene.LoadingState GetLoadingStateInternal_Injected([In] ref SceneHandle sceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIsDirtyInternal_Injected([In] ref SceneHandle sceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDirtyID_Injected([In] ref SceneHandle sceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetBuildIndexInternal_Injected([In] ref SceneHandle sceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRootCountInternal_Injected([In] ref SceneHandle sceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRootGameObjectsInternal_Injected([In] ref SceneHandle sceneHandle, object resultRootList);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDefaultParent_Injected([In] ref SceneHandle sceneHandle, out EntityId ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDefaultParent_Injected([In] ref SceneHandle sceneHandle, [In] ref EntityId value);

		[HideInInspector]
		[SerializeField]
		private SceneHandle m_Handle;

		internal enum LoadingState
		{
			NotLoaded,
			Loading,
			Loaded,
			Unloading
		}
	}
}
