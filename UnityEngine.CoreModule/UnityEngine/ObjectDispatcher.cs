using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[StaticAccessor("GetObjectDispatcher()", StaticAccessorType.Dot)]
	[NativeHeader("Runtime/Misc/ObjectDispatcher.h")]
	[RequiredByNativeCode]
	internal sealed class ObjectDispatcher : IDisposable
	{
		public bool valid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		public int maxDispatchHistoryFramesCount
		{
			get
			{
				this.ValidateSystemHandleAndThrow();
				return ObjectDispatcher.GetMaxDispatchHistoryFramesCount(this.m_Ptr);
			}
			set
			{
				this.ValidateSystemHandleAndThrow();
				ObjectDispatcher.SetMaxDispatchHistoryFramesCount(this.m_Ptr, value);
			}
		}

		public ObjectDispatcher()
		{
			this.m_Ptr = ObjectDispatcher.CreateDispatchSystemHandle();
			this.m_TypeDataCallback = new Action<TypeDispatchData>(this.DispatchCallback);
			this.m_TransformDataCallback = new Action<TransformDispatchData>(this.DispatchCallback);
			this.m_TransformComponentCallback = new Action<Component[]>(this.DispatchCallback);
		}

		~ObjectDispatcher()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				ObjectDispatcher.DestroyDispatchSystemHandle(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		private void ValidateSystemHandleAndThrow()
		{
			bool flag = !this.valid;
			if (flag)
			{
				throw new Exception("The ObjectDispatcher is invalid or has been disposed.");
			}
		}

		private void ValidateTypeAndThrow(Type type)
		{
			bool flag = !type.IsSubclassOf(typeof(Object));
			if (flag)
			{
				throw new Exception("Only types inherited from UnityEngine.Object are supported.");
			}
		}

		private void ValidateComponentTypeAndThrow(Type type)
		{
			bool flag = !type.IsSubclassOf(typeof(Component));
			if (flag)
			{
				throw new Exception("Only types inherited from UnityEngine.Component are supported.");
			}
		}

		private void DispatchCallback(TypeDispatchData data)
		{
			this.m_TypeDispatchData = default(TypeDispatchData);
			this.m_TypeDispatchData.changed = data.changed;
			this.m_TypeDispatchData.changedID = new NativeArray<EntityId>(data.changedID, this.m_DispatchAllocator);
			this.m_TypeDispatchData.destroyedID = new NativeArray<EntityId>(data.destroyedID, this.m_DispatchAllocator);
		}

		private void DispatchCallback(TransformDispatchData data)
		{
			this.m_TransformDispatchData = default(TransformDispatchData);
			this.m_TransformDispatchData.transformedID = new NativeArray<EntityId>(data.transformedID, this.m_DispatchAllocator);
			this.m_TransformDispatchData.parentID = new NativeArray<EntityId>(data.parentID, this.m_DispatchAllocator);
			this.m_TransformDispatchData.localToWorldMatrices = new NativeArray<Matrix4x4>(data.localToWorldMatrices, this.m_DispatchAllocator);
			this.m_TransformDispatchData.positions = new NativeArray<Vector3>(data.positions, this.m_DispatchAllocator);
			this.m_TransformDispatchData.rotations = new NativeArray<Quaternion>(data.rotations, this.m_DispatchAllocator);
			this.m_TransformDispatchData.scales = new NativeArray<Vector3>(data.scales, this.m_DispatchAllocator);
		}

		private void DispatchCallback(Component[] components)
		{
			this.m_TransformedComponents = components;
		}

		public void DispatchTypeChangesAndClear(Type type, Action<TypeDispatchData> callback, bool sortByInstanceID = false, bool noScriptingArray = false)
		{
			this.ValidateSystemHandleAndThrow();
			this.ValidateTypeAndThrow(type);
			ObjectDispatcher.DispatchTypeChangesAndClear(this.m_Ptr, type, ObjectDispatcher.s_TypeDispatch, sortByInstanceID, noScriptingArray, callback);
		}

		public void DispatchTransformChangesAndClear(Type type, ObjectDispatcher.TransformTrackingType trackingType, Action<Component[]> callback, bool sortByInstanceID = false)
		{
			this.ValidateSystemHandleAndThrow();
			this.ValidateComponentTypeAndThrow(type);
			ObjectDispatcher.DispatchTransformChangesAndClear(this.m_Ptr, type, trackingType, callback, sortByInstanceID);
		}

		public void DispatchTransformChangesAndClear(Type type, ObjectDispatcher.TransformTrackingType trackingType, Action<TransformDispatchData> callback)
		{
			this.ValidateSystemHandleAndThrow();
			this.ValidateComponentTypeAndThrow(type);
			ObjectDispatcher.DispatchTransformDataChangesAndClear(this.m_Ptr, type, trackingType, ObjectDispatcher.s_TransformDispatch, callback);
		}

		public void ClearTypeChanges(Type type)
		{
			this.ValidateSystemHandleAndThrow();
			this.ValidateTypeAndThrow(type);
			ObjectDispatcher.DispatchTypeChangesAndClear(this.m_Ptr, type, null, false, false, null);
		}

		public TypeDispatchData GetTypeChangesAndClear(Type type, Allocator allocator, bool sortByInstanceID = false, bool noScriptingArray = false)
		{
			this.m_DispatchAllocator = allocator;
			this.DispatchTypeChangesAndClear(type, this.m_TypeDataCallback, sortByInstanceID, noScriptingArray);
			return this.m_TypeDispatchData;
		}

		public void GetTypeChangesAndClear(Type type, List<Object> changed, out NativeArray<EntityId> changedID, out NativeArray<EntityId> destroyedID, Allocator allocator, bool sortByInstanceID = false)
		{
			this.m_DispatchAllocator = allocator;
			this.DispatchTypeChangesAndClear(type, this.m_TypeDataCallback, sortByInstanceID, true);
			changedID = this.m_TypeDispatchData.changedID;
			destroyedID = this.m_TypeDispatchData.destroyedID;
			Resources.EntityIdsToObjectList(this.m_TypeDispatchData.changedID, changed);
		}

		public Component[] GetTransformChangesAndClear(Type type, ObjectDispatcher.TransformTrackingType trackingType, bool sortByInstanceID = false)
		{
			this.DispatchTransformChangesAndClear(type, trackingType, this.m_TransformComponentCallback, sortByInstanceID);
			return this.m_TransformedComponents;
		}

		public TransformDispatchData GetTransformChangesAndClear(Type type, ObjectDispatcher.TransformTrackingType trackingType, Allocator allocator)
		{
			this.m_DispatchAllocator = allocator;
			this.DispatchTransformChangesAndClear(type, trackingType, this.m_TransformDataCallback);
			return this.m_TransformDispatchData;
		}

		public void EnableTypeTracking(ObjectDispatcher.TypeTrackingFlags typeTrackingMask, params Type[] types)
		{
			this.ValidateSystemHandleAndThrow();
			foreach (Type type in types)
			{
				this.ValidateTypeAndThrow(type);
				ObjectDispatcher.EnableTypeTracking(this.m_Ptr, type, typeTrackingMask);
			}
		}

		public void EnableTypeTracking(params Type[] types)
		{
			this.EnableTypeTracking(ObjectDispatcher.TypeTrackingFlags.Default, types);
		}

		[Obsolete("EnableTypeTrackingIncludingAssets is deprecated, please use EnableTypeTracking and provide the flag that specifies whether you need assets or not.", false)]
		public void EnableTypeTrackingIncludingAssets(params Type[] types)
		{
			this.EnableTypeTracking(ObjectDispatcher.TypeTrackingFlags.Default, types);
		}

		public void DisableTypeTracking(params Type[] types)
		{
			this.ValidateSystemHandleAndThrow();
			foreach (Type type in types)
			{
				this.ValidateTypeAndThrow(type);
				ObjectDispatcher.DisableTypeTracking(this.m_Ptr, type);
			}
		}

		public void EnableTransformTracking(ObjectDispatcher.TransformTrackingType trackingType, params Type[] types)
		{
			this.ValidateSystemHandleAndThrow();
			foreach (Type type in types)
			{
				this.ValidateComponentTypeAndThrow(type);
				ObjectDispatcher.EnableTransformTracking(this.m_Ptr, type, trackingType);
			}
		}

		public void DisableTransformTracking(ObjectDispatcher.TransformTrackingType trackingType, params Type[] types)
		{
			this.ValidateSystemHandleAndThrow();
			foreach (Type type in types)
			{
				this.ValidateComponentTypeAndThrow(type);
				ObjectDispatcher.DisableTransformTracking(this.m_Ptr, type, trackingType);
			}
		}

		public void DispatchTypeChangesAndClear<T>(Action<TypeDispatchData> callback, bool sortByInstanceID = false, bool noScriptingArray = false) where T : Object
		{
			this.DispatchTypeChangesAndClear(typeof(T), callback, sortByInstanceID, noScriptingArray);
		}

		public void DispatchTransformChangesAndClear<T>(ObjectDispatcher.TransformTrackingType trackingType, Action<Component[]> callback, bool sortByInstanceID = false) where T : Object
		{
			this.DispatchTransformChangesAndClear(typeof(T), trackingType, callback, sortByInstanceID);
		}

		public void DispatchTransformChangesAndClear<T>(ObjectDispatcher.TransformTrackingType trackingType, Action<TransformDispatchData> callback) where T : Object
		{
			this.DispatchTransformChangesAndClear(typeof(T), trackingType, callback);
		}

		public void ClearTypeChanges<T>() where T : Object
		{
			this.ClearTypeChanges(typeof(T));
		}

		public TypeDispatchData GetTypeChangesAndClear<T>(Allocator allocator, bool sortByInstanceID = false, bool noScriptingArray = false) where T : Object
		{
			return this.GetTypeChangesAndClear(typeof(T), allocator, sortByInstanceID, noScriptingArray);
		}

		public void GetTypeChangesAndClear<T>(List<Object> changed, out NativeArray<EntityId> changedID, out NativeArray<EntityId> destroyedID, Allocator allocator, bool sortByInstanceID = false) where T : Object
		{
			this.GetTypeChangesAndClear(typeof(T), changed, out changedID, out destroyedID, allocator, sortByInstanceID);
		}

		public Component[] GetTransformChangesAndClear<T>(ObjectDispatcher.TransformTrackingType trackingType, bool sortByInstanceID = false) where T : Object
		{
			return this.GetTransformChangesAndClear(typeof(T), trackingType, sortByInstanceID);
		}

		public TransformDispatchData GetTransformChangesAndClear<T>(ObjectDispatcher.TransformTrackingType trackingType, Allocator allocator) where T : Object
		{
			return this.GetTransformChangesAndClear(typeof(T), trackingType, allocator);
		}

		public void EnableTypeTracking<T>(ObjectDispatcher.TypeTrackingFlags typeTrackingMask = ObjectDispatcher.TypeTrackingFlags.Default) where T : Object
		{
			this.EnableTypeTracking(typeTrackingMask, new Type[] { typeof(T) });
		}

		public void DisableTypeTracking<T>() where T : Object
		{
			this.DisableTypeTracking(new Type[] { typeof(T) });
		}

		public void EnableTransformTracking<T>(ObjectDispatcher.TransformTrackingType trackingType) where T : Object
		{
			this.EnableTransformTracking(trackingType, new Type[] { typeof(T) });
		}

		public void DisableTransformTracking<T>(ObjectDispatcher.TransformTrackingType trackingType) where T : Object
		{
			this.DisableTransformTracking(trackingType, new Type[] { typeof(T) });
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateDispatchSystemHandle();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyDispatchSystemHandle(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMaxDispatchHistoryFramesCount(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMaxDispatchHistoryFramesCount(IntPtr ptr, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableTypeTracking(IntPtr ptr, Type type, ObjectDispatcher.TypeTrackingFlags typeTrackingMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableTypeTracking(IntPtr ptr, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableTransformTracking(IntPtr ptr, Type type, ObjectDispatcher.TransformTrackingType trackingType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableTransformTracking(IntPtr ptr, Type type, ObjectDispatcher.TransformTrackingType trackingType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DispatchTypeChangesAndClear(IntPtr ptr, Type type, Action<Object[], IntPtr, IntPtr, int, int, Action<TypeDispatchData>> callback, bool sortByInstanceID, bool noScriptingArray, Action<TypeDispatchData> param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DispatchTransformDataChangesAndClear(IntPtr ptr, Type type, ObjectDispatcher.TransformTrackingType trackingType, Action<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, int, Action<TransformDispatchData>> callback, Action<TransformDispatchData> param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DispatchTransformChangesAndClear(IntPtr ptr, Type type, ObjectDispatcher.TransformTrackingType trackingType, Action<Component[]> callback, bool sortByInstanceID);

		private IntPtr m_Ptr = IntPtr.Zero;

		private Allocator m_DispatchAllocator;

		private TypeDispatchData m_TypeDispatchData;

		private TransformDispatchData m_TransformDispatchData;

		private Component[] m_TransformedComponents;

		private Action<TypeDispatchData> m_TypeDataCallback;

		private Action<TransformDispatchData> m_TransformDataCallback;

		private Action<Component[]> m_TransformComponentCallback;

		private static Action<Object[], IntPtr, IntPtr, int, int, Action<TypeDispatchData>> s_TypeDispatch = delegate(Object[] changed, IntPtr changedID, IntPtr destroyedID, int changedCount, int destroyedCount, Action<TypeDispatchData> callback)
		{
			NativeArray<EntityId> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<EntityId>(changedID.ToPointer(), changedCount, Allocator.Invalid);
			NativeArray<EntityId> nativeArray2 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<EntityId>(destroyedID.ToPointer(), destroyedCount, Allocator.Invalid);
			TypeDispatchData typeDispatchData = new TypeDispatchData
			{
				changed = changed,
				changedID = nativeArray,
				destroyedID = nativeArray2
			};
			callback(typeDispatchData);
		};

		private static Action<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, int, Action<TransformDispatchData>> s_TransformDispatch = delegate(IntPtr transformed, IntPtr parents, IntPtr localToWorldMatrices, IntPtr positions, IntPtr rotations, IntPtr scales, int count, Action<TransformDispatchData> callback)
		{
			NativeArray<EntityId> nativeArray3 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<EntityId>(transformed.ToPointer(), count, Allocator.Invalid);
			NativeArray<EntityId> nativeArray4 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<EntityId>(parents.ToPointer(), (parents != IntPtr.Zero) ? count : 0, Allocator.Invalid);
			NativeArray<Matrix4x4> nativeArray5 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(localToWorldMatrices.ToPointer(), (localToWorldMatrices != IntPtr.Zero) ? count : 0, Allocator.Invalid);
			NativeArray<Vector3> nativeArray6 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(positions.ToPointer(), (positions != IntPtr.Zero) ? count : 0, Allocator.Invalid);
			NativeArray<Quaternion> nativeArray7 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Quaternion>(rotations.ToPointer(), (rotations != IntPtr.Zero) ? count : 0, Allocator.Invalid);
			NativeArray<Vector3> nativeArray8 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(scales.ToPointer(), (scales != IntPtr.Zero) ? count : 0, Allocator.Invalid);
			TransformDispatchData transformDispatchData = new TransformDispatchData
			{
				transformedID = nativeArray3,
				parentID = nativeArray4,
				localToWorldMatrices = nativeArray5,
				positions = nativeArray6,
				rotations = nativeArray7,
				scales = nativeArray8
			};
			callback(transformDispatchData);
		};

		public enum TransformTrackingType
		{
			GlobalTRS,
			LocalTRS,
			Hierarchy
		}

		[Flags]
		public enum TypeTrackingFlags
		{
			SceneObjects = 1,
			Assets = 2,
			EditorOnlyObjects = 4,
			Default = 3,
			All = 7
		}
	}
}
