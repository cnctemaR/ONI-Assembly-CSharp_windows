using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[StaticAccessor("GetPhysicsManager()", StaticAccessorType.Dot)]
	[NativeHeader("Modules/Physics/PhysicsManager.h")]
	[NativeHeader("Modules/Physics/PhysicsQuery.h")]
	public class Physics
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<PhysicsScene, NativeArray<ModifiableContactPair>> ContactModifyEvent;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<PhysicsScene, NativeArray<ModifiableContactPair>> ContactModifyEventCCD;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal static event Action<PhysicsScene, IntPtr, int, bool> GenericContactModifyEvent;

		[RequiredByNativeCode]
		private static void OnSceneContactModify(PhysicsScene scene, IntPtr buffer, int count, bool isCCD)
		{
			Action<PhysicsScene, IntPtr, int, bool> genericContactModifyEvent = Physics.GenericContactModifyEvent;
			if (genericContactModifyEvent != null)
			{
				genericContactModifyEvent(scene, buffer, count, isCCD);
			}
		}

		private static void PhysXOnSceneContactModify(PhysicsScene scene, IntPtr buffer, int count, bool isCCD)
		{
			NativeArray<ModifiableContactPair> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ModifiableContactPair>(buffer.ToPointer(), count, Allocator.None);
			bool flag = !isCCD;
			if (flag)
			{
				Action<PhysicsScene, NativeArray<ModifiableContactPair>> contactModifyEvent = Physics.ContactModifyEvent;
				if (contactModifyEvent != null)
				{
					contactModifyEvent(scene, nativeArray);
				}
			}
			else
			{
				Action<PhysicsScene, NativeArray<ModifiableContactPair>> contactModifyEventCCD = Physics.ContactModifyEventCCD;
				if (contactModifyEventCCD != null)
				{
					contactModifyEventCCD(scene, nativeArray);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetIntegrationInfos(out IntPtr integrations, out ulong integrationCount);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCurrentIntegrationInfo(out IntPtr integration);

		internal static ReadOnlySpan<IntegrationInfo> GetIntegrationInfos()
		{
			IntPtr intPtr;
			ulong num;
			Physics.GetIntegrationInfos(out intPtr, out num);
			return new ReadOnlySpan<IntegrationInfo>(intPtr.ToPointer(), (int)num);
		}

		public unsafe static IntegrationInfo GetCurrentIntegrationInfo()
		{
			IntPtr intPtr;
			Physics.GetCurrentIntegrationInfo(out intPtr);
			return *(IntegrationInfo*)intPtr.ToPointer();
		}

		public static Vector3 gravity
		{
			[ThreadSafe]
			get
			{
				Vector3 vector;
				Physics.get_gravity_Injected(out vector);
				return vector;
			}
			set
			{
				Physics.set_gravity_Injected(ref value);
			}
		}

		public static extern float defaultContactOffset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float sleepThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool queriesHitTriggers
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool queriesHitBackfaces
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float bounceThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float defaultMaxDepenetrationVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultSolverIterations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultSolverVelocityIterations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern SimulationMode simulationMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float defaultMaxAngularSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool improvedPatchFriction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool invokeCollisionCallbacks
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static PhysicsScene defaultPhysicsScene
		{
			get
			{
				return PhysicsScene.GetDefaultScene();
			}
		}

		public static void IgnoreCollision([NotNull] Collider collider1, [NotNull] Collider collider2, [DefaultValue("true")] bool ignore)
		{
			if (collider1 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			if (collider2 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider>(collider1);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider>(collider2);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			Physics.IgnoreCollision_Injected(intPtr, intPtr2, ignore);
		}

		[ExcludeFromDocs]
		public static void IgnoreCollision(Collider collider1, Collider collider2)
		{
			Physics.IgnoreCollision(collider1, collider2, true);
		}

		[NativeName("IgnoreCollision")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void IgnoreLayerCollision(int layer1, int layer2, [DefaultValue("true")] bool ignore);

		[ExcludeFromDocs]
		public static void IgnoreLayerCollision(int layer1, int layer2)
		{
			Physics.IgnoreLayerCollision(layer1, layer2, true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetIgnoreLayerCollision(int layer1, int layer2);

		public static bool GetIgnoreCollision([NotNull] Collider collider1, [NotNull] Collider collider2)
		{
			if (collider1 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			if (collider2 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider>(collider1);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider>(collider2);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			return Physics.GetIgnoreCollision_Injected(intPtr, intPtr2);
		}

		public static bool Raycast(Vector3 origin, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.Raycast(origin, direction, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask)
		{
			return Physics.defaultPhysicsScene.Raycast(origin, direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance)
		{
			return Physics.defaultPhysicsScene.Raycast(origin, direction, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction)
		{
			return Physics.defaultPhysicsScene.Raycast(origin, direction, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.Raycast(origin, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
		}

		[RequiredByNativeCode]
		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
		{
			return Physics.defaultPhysicsScene.Raycast(origin, direction, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
		{
			return Physics.defaultPhysicsScene.Raycast(origin, direction, out hitInfo, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
		{
			return Physics.defaultPhysicsScene.Raycast(origin, direction, out hitInfo, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static bool Raycast(Ray ray, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.Raycast(ray.origin, ray.direction, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Ray ray, float maxDistance, int layerMask)
		{
			return Physics.defaultPhysicsScene.Raycast(ray.origin, ray.direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Ray ray, float maxDistance)
		{
			return Physics.defaultPhysicsScene.Raycast(ray.origin, ray.direction, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Ray ray)
		{
			return Physics.defaultPhysicsScene.Raycast(ray.origin, ray.direction, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static bool Raycast(Ray ray, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.Raycast(ray.origin, ray.direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask)
		{
			return Physics.Raycast(ray.origin, ray.direction, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
		{
			return Physics.defaultPhysicsScene.Raycast(ray.origin, ray.direction, out hitInfo, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Ray ray, out RaycastHit hitInfo)
		{
			return Physics.defaultPhysicsScene.Raycast(ray.origin, ray.direction, out hitInfo, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static bool Linecast(Vector3 start, Vector3 end, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			Vector3 vector = end - start;
			return Physics.defaultPhysicsScene.Raycast(start, vector, vector.magnitude, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool Linecast(Vector3 start, Vector3 end, int layerMask)
		{
			return Physics.Linecast(start, end, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool Linecast(Vector3 start, Vector3 end)
		{
			return Physics.Linecast(start, end, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			Vector3 vector = end - start;
			return Physics.defaultPhysicsScene.Raycast(start, vector, out hitInfo, vector.magnitude, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, int layerMask)
		{
			return Physics.Linecast(start, end, out hitInfo, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo)
		{
			return Physics.Linecast(start, end, out hitInfo, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			RaycastHit raycastHit;
			return Physics.defaultPhysicsScene.CapsuleCast(point1, point2, radius, direction, out raycastHit, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int layerMask)
		{
			return Physics.CapsuleCast(point1, point2, radius, direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance)
		{
			return Physics.CapsuleCast(point1, point2, radius, direction, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
		{
			return Physics.CapsuleCast(point1, point2, radius, direction, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
		{
			return Physics.CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
		{
			return Physics.CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo)
		{
			return Physics.CapsuleCast(point1, point2, radius, direction, out hitInfo, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.SphereCast(origin, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
		{
			return Physics.SphereCast(origin, radius, direction, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
		{
			return Physics.SphereCast(origin, radius, direction, out hitInfo, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo)
		{
			return Physics.SphereCast(origin, radius, direction, out hitInfo, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static bool SphereCast(Ray ray, float radius, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			RaycastHit raycastHit;
			return Physics.SphereCast(ray.origin, radius, ray.direction, out raycastHit, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius, float maxDistance, int layerMask)
		{
			return Physics.SphereCast(ray, radius, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius, float maxDistance)
		{
			return Physics.SphereCast(ray, radius, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius)
		{
			return Physics.SphereCast(ray, radius, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.SphereCast(ray.origin, radius, ray.direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance, int layerMask)
		{
			return Physics.SphereCast(ray, radius, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance)
		{
			return Physics.SphereCast(ray, radius, out hitInfo, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo)
		{
			return Physics.SphereCast(ray, radius, out hitInfo, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			RaycastHit raycastHit;
			return Physics.defaultPhysicsScene.BoxCast(center, halfExtents, direction, out raycastHit, orientation, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, int layerMask)
		{
			return Physics.BoxCast(center, halfExtents, direction, orientation, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance)
		{
			return Physics.BoxCast(center, halfExtents, direction, orientation, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation)
		{
			return Physics.BoxCast(center, halfExtents, direction, orientation, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction)
		{
			return Physics.BoxCast(center, halfExtents, direction, Quaternion.identity, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.BoxCast(center, halfExtents, direction, out hitInfo, orientation, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, Quaternion orientation, float maxDistance, int layerMask)
		{
			return Physics.BoxCast(center, halfExtents, direction, out hitInfo, orientation, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, Quaternion orientation, float maxDistance)
		{
			return Physics.BoxCast(center, halfExtents, direction, out hitInfo, orientation, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, Quaternion orientation)
		{
			return Physics.BoxCast(center, halfExtents, direction, out hitInfo, orientation, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo)
		{
			return Physics.BoxCast(center, halfExtents, direction, out hitInfo, Quaternion.identity, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		[FreeFunction("Physics::RaycastAll")]
		private static RaycastHit[] Internal_RaycastAll(PhysicsScene physicsScene, Ray ray, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
		{
			RaycastHit[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Physics.Internal_RaycastAll_Injected(ref physicsScene, ref ray, maxDistance, mask, queryTriggerInteraction, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RaycastHit[] array;
				blittableArrayWrapper.Unmarshal<RaycastHit>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			float magnitude = direction.magnitude;
			bool flag = magnitude > float.Epsilon;
			RaycastHit[] array;
			if (flag)
			{
				Vector3 vector = direction / magnitude;
				Ray ray = new Ray(origin, vector);
				array = Physics.Internal_RaycastAll(Physics.defaultPhysicsScene, ray, maxDistance, layerMask, queryTriggerInteraction);
			}
			else
			{
				array = new RaycastHit[0];
			}
			return array;
		}

		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance, int layerMask)
		{
			return Physics.RaycastAll(origin, direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance)
		{
			return Physics.RaycastAll(origin, direction, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction)
		{
			return Physics.RaycastAll(origin, direction, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static RaycastHit[] RaycastAll(Ray ray, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.RaycastAll(ray.origin, ray.direction, maxDistance, layerMask, queryTriggerInteraction);
		}

		[RequiredByNativeCode]
		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Ray ray, float maxDistance, int layerMask)
		{
			return Physics.RaycastAll(ray.origin, ray.direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Ray ray, float maxDistance)
		{
			return Physics.RaycastAll(ray.origin, ray.direction, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Ray ray)
		{
			return Physics.RaycastAll(ray.origin, ray.direction, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.Raycast(ray.origin, ray.direction, results, maxDistance, layerMask, queryTriggerInteraction);
		}

		[RequiredByNativeCode]
		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, float maxDistance, int layerMask)
		{
			return Physics.defaultPhysicsScene.Raycast(ray.origin, ray.direction, results, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, float maxDistance)
		{
			return Physics.defaultPhysicsScene.Raycast(ray.origin, ray.direction, results, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Ray ray, RaycastHit[] results)
		{
			return Physics.defaultPhysicsScene.Raycast(ray.origin, ray.direction, results, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.Raycast(origin, direction, results, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, float maxDistance, int layerMask)
		{
			return Physics.defaultPhysicsScene.Raycast(origin, direction, results, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, float maxDistance)
		{
			return Physics.defaultPhysicsScene.Raycast(origin, direction, results, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results)
		{
			return Physics.defaultPhysicsScene.Raycast(origin, direction, results, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		[FreeFunction("Physics::CapsuleCastAll")]
		private static RaycastHit[] Query_CapsuleCastAll(PhysicsScene physicsScene, Vector3 p0, Vector3 p1, float radius, Vector3 direction, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
		{
			RaycastHit[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Physics.Query_CapsuleCastAll_Injected(ref physicsScene, ref p0, ref p1, radius, ref direction, maxDistance, mask, queryTriggerInteraction, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RaycastHit[] array;
				blittableArrayWrapper.Unmarshal<RaycastHit>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			float magnitude = direction.magnitude;
			bool flag = magnitude > float.Epsilon;
			RaycastHit[] array;
			if (flag)
			{
				Vector3 vector = direction / magnitude;
				array = Physics.Query_CapsuleCastAll(Physics.defaultPhysicsScene, point1, point2, radius, vector, maxDistance, layerMask, queryTriggerInteraction);
			}
			else
			{
				array = new RaycastHit[0];
			}
			return array;
		}

		[ExcludeFromDocs]
		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int layerMask)
		{
			return Physics.CapsuleCastAll(point1, point2, radius, direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance)
		{
			return Physics.CapsuleCastAll(point1, point2, radius, direction, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
		{
			return Physics.CapsuleCastAll(point1, point2, radius, direction, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		[FreeFunction("Physics::SphereCastAll")]
		private static RaycastHit[] Query_SphereCastAll(PhysicsScene physicsScene, Vector3 origin, float radius, Vector3 direction, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
		{
			RaycastHit[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Physics.Query_SphereCastAll_Injected(ref physicsScene, ref origin, radius, ref direction, maxDistance, mask, queryTriggerInteraction, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RaycastHit[] array;
				blittableArrayWrapper.Unmarshal<RaycastHit>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			float magnitude = direction.magnitude;
			bool flag = magnitude > float.Epsilon;
			RaycastHit[] array;
			if (flag)
			{
				Vector3 vector = direction / magnitude;
				array = Physics.Query_SphereCastAll(Physics.defaultPhysicsScene, origin, radius, vector, maxDistance, layerMask, queryTriggerInteraction);
			}
			else
			{
				array = new RaycastHit[0];
			}
			return array;
		}

		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance, int layerMask)
		{
			return Physics.SphereCastAll(origin, radius, direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance)
		{
			return Physics.SphereCastAll(origin, radius, direction, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction)
		{
			return Physics.SphereCastAll(origin, radius, direction, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static RaycastHit[] SphereCastAll(Ray ray, float radius, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.SphereCastAll(ray.origin, radius, ray.direction, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Ray ray, float radius, float maxDistance, int layerMask)
		{
			return Physics.SphereCastAll(ray, radius, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Ray ray, float radius, float maxDistance)
		{
			return Physics.SphereCastAll(ray, radius, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Ray ray, float radius)
		{
			return Physics.SphereCastAll(ray, radius, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		[FreeFunction("Physics::OverlapCapsule")]
		private static Collider[] OverlapCapsule_Internal(PhysicsScene physicsScene, Vector3 point0, Vector3 point1, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.OverlapCapsule_Internal_Injected(ref physicsScene, ref point0, ref point1, radius, layerMask, queryTriggerInteraction);
		}

		public static Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, [DefaultValue("AllLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.OverlapCapsule_Internal(Physics.defaultPhysicsScene, point0, point1, radius, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, int layerMask)
		{
			return Physics.OverlapCapsule(point0, point1, radius, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius)
		{
			return Physics.OverlapCapsule(point0, point1, radius, -1, QueryTriggerInteraction.UseGlobal);
		}

		[FreeFunction("Physics::OverlapSphere")]
		private static Collider[] OverlapSphere_Internal(PhysicsScene physicsScene, Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.OverlapSphere_Internal_Injected(ref physicsScene, ref position, radius, layerMask, queryTriggerInteraction);
		}

		public static Collider[] OverlapSphere(Vector3 position, float radius, [DefaultValue("AllLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.OverlapSphere_Internal(Physics.defaultPhysicsScene, position, radius, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static Collider[] OverlapSphere(Vector3 position, float radius, int layerMask)
		{
			return Physics.OverlapSphere(position, radius, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static Collider[] OverlapSphere(Vector3 position, float radius)
		{
			return Physics.OverlapSphere(position, radius, -1, QueryTriggerInteraction.UseGlobal);
		}

		[NativeName("Simulate")]
		internal static void Simulate_Internal(PhysicsScene physicsScene, float step, SimulationStage stages, SimulationOption options)
		{
			Physics.Simulate_Internal_Injected(ref physicsScene, step, stages, options);
		}

		public static void Simulate(float step)
		{
			bool flag = Physics.simulationMode != SimulationMode.Script;
			if (flag)
			{
				Debug.LogWarning("Physics.Simulate(...) was called but simulation mode is not set to Script. You should set simulation mode to Script first before calling this function therefore the simulation was not run.");
			}
			else
			{
				Physics.Simulate_Internal(Physics.defaultPhysicsScene, step, SimulationStage.All, SimulationOption.All);
			}
		}

		[NativeName("InterpolateBodies")]
		internal static void InterpolateBodies_Internal(PhysicsScene physicsScene)
		{
			Physics.InterpolateBodies_Internal_Injected(ref physicsScene);
		}

		[NativeName("ResetInterpolatedTransformPosition")]
		internal static void ResetInterpolationPoses_Internal(PhysicsScene physicsScene)
		{
			Physics.ResetInterpolationPoses_Internal_Injected(ref physicsScene);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SyncTransforms();

		public static extern bool reuseCollisionCallbacks
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction("Physics::ComputePenetration")]
		private static bool Query_ComputePenetration([NotNull] Collider colliderA, Vector3 positionA, Quaternion rotationA, [NotNull] Collider colliderB, Vector3 positionB, Quaternion rotationB, ref Vector3 direction, ref float distance)
		{
			if (colliderA == null)
			{
				ThrowHelper.ThrowArgumentNullException(colliderA, "colliderA");
			}
			if (colliderB == null)
			{
				ThrowHelper.ThrowArgumentNullException(colliderB, "colliderB");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider>(colliderA);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(colliderA, "colliderA");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider>(colliderB);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(colliderB, "colliderB");
			}
			return Physics.Query_ComputePenetration_Injected(intPtr, ref positionA, ref rotationA, intPtr2, ref positionB, ref rotationB, ref direction, ref distance);
		}

		public static bool ComputePenetration(Collider colliderA, Vector3 positionA, Quaternion rotationA, Collider colliderB, Vector3 positionB, Quaternion rotationB, out Vector3 direction, out float distance)
		{
			direction = Vector3.zero;
			distance = 0f;
			return Physics.Query_ComputePenetration(colliderA, positionA, rotationA, colliderB, positionB, rotationB, ref direction, ref distance);
		}

		[FreeFunction("Physics::ClosestPoint")]
		private static Vector3 Query_ClosestPoint([NotNull] Collider collider, Vector3 position, Quaternion rotation, Vector3 point)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider>(collider);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			Vector3 vector;
			Physics.Query_ClosestPoint_Injected(intPtr, ref position, ref rotation, ref point, out vector);
			return vector;
		}

		public static Vector3 ClosestPoint(Vector3 point, Collider collider, Vector3 position, Quaternion rotation)
		{
			return Physics.Query_ClosestPoint(collider, position, rotation, point);
		}

		[StaticAccessor("GetPhysicsManager()")]
		public static extern float interCollisionDistance
		{
			[NativeName("GetClothInterCollisionDistance")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetClothInterCollisionDistance")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysicsManager()")]
		public static extern float interCollisionStiffness
		{
			[NativeName("GetClothInterCollisionStiffness")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetClothInterCollisionStiffness")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysicsManager()")]
		public static extern bool interCollisionSettingsToggle
		{
			[NativeName("GetClothInterCollisionSettingsToggle")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetClothInterCollisionSettingsToggle")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Vector3 clothGravity
		{
			[ThreadSafe]
			get
			{
				Vector3 vector;
				Physics.get_clothGravity_Injected(out vector);
				return vector;
			}
			set
			{
				Physics.set_clothGravity_Injected(ref value);
			}
		}

		public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results, [DefaultValue("AllLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.OverlapSphere(position, radius, results, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results, int layerMask)
		{
			return Physics.OverlapSphereNonAlloc(position, radius, results, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results)
		{
			return Physics.OverlapSphereNonAlloc(position, radius, results, -1, QueryTriggerInteraction.UseGlobal);
		}

		[FreeFunction("Physics::SphereTest")]
		private static bool CheckSphere_Internal(PhysicsScene physicsScene, Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.CheckSphere_Internal_Injected(ref physicsScene, ref position, radius, layerMask, queryTriggerInteraction);
		}

		public static bool CheckSphere(Vector3 position, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.CheckSphere_Internal(Physics.defaultPhysicsScene, position, radius, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool CheckSphere(Vector3 position, float radius, int layerMask)
		{
			return Physics.CheckSphere(position, radius, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool CheckSphere(Vector3 position, float radius)
		{
			return Physics.CheckSphere(position, radius, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.CapsuleCast(point1, point2, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, float maxDistance, int layerMask)
		{
			return Physics.CapsuleCastNonAlloc(point1, point2, radius, direction, results, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, float maxDistance)
		{
			return Physics.CapsuleCastNonAlloc(point1, point2, radius, direction, results, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results)
		{
			return Physics.CapsuleCastNonAlloc(point1, point2, radius, direction, results, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.SphereCast(origin, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, float maxDistance, int layerMask)
		{
			return Physics.SphereCastNonAlloc(origin, radius, direction, results, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, float maxDistance)
		{
			return Physics.SphereCastNonAlloc(origin, radius, direction, results, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results)
		{
			return Physics.SphereCastNonAlloc(origin, radius, direction, results, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.SphereCastNonAlloc(ray.origin, radius, ray.direction, results, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, float maxDistance, int layerMask)
		{
			return Physics.SphereCastNonAlloc(ray, radius, results, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, float maxDistance)
		{
			return Physics.SphereCastNonAlloc(ray, radius, results, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results)
		{
			return Physics.SphereCastNonAlloc(ray, radius, results, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		[FreeFunction("Physics::CapsuleTest")]
		private static bool CheckCapsule_Internal(PhysicsScene physicsScene, Vector3 start, Vector3 end, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.CheckCapsule_Internal_Injected(ref physicsScene, ref start, ref end, radius, layerMask, queryTriggerInteraction);
		}

		public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.CheckCapsule_Internal(Physics.defaultPhysicsScene, start, end, radius, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask)
		{
			return Physics.CheckCapsule(start, end, radius, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool CheckCapsule(Vector3 start, Vector3 end, float radius)
		{
			return Physics.CheckCapsule(start, end, radius, -5, QueryTriggerInteraction.UseGlobal);
		}

		[FreeFunction("Physics::BoxTest")]
		private static bool CheckBox_Internal(PhysicsScene physicsScene, Vector3 center, Vector3 halfExtents, Quaternion orientation, int layermask, QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.CheckBox_Internal_Injected(ref physicsScene, ref center, ref halfExtents, ref orientation, layermask, queryTriggerInteraction);
		}

		public static bool CheckBox(Vector3 center, Vector3 halfExtents, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("DefaultRaycastLayers")] int layermask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.CheckBox_Internal(Physics.defaultPhysicsScene, center, halfExtents, orientation, layermask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask)
		{
			return Physics.CheckBox(center, halfExtents, orientation, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation)
		{
			return Physics.CheckBox(center, halfExtents, orientation, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static bool CheckBox(Vector3 center, Vector3 halfExtents)
		{
			return Physics.CheckBox(center, halfExtents, Quaternion.identity, -5, QueryTriggerInteraction.UseGlobal);
		}

		[FreeFunction("Physics::OverlapBox")]
		private static Collider[] OverlapBox_Internal(PhysicsScene physicsScene, Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.OverlapBox_Internal_Injected(ref physicsScene, ref center, ref halfExtents, ref orientation, layerMask, queryTriggerInteraction);
		}

		public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("AllLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.OverlapBox_Internal(Physics.defaultPhysicsScene, center, halfExtents, orientation, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask)
		{
			return Physics.OverlapBox(center, halfExtents, orientation, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation)
		{
			return Physics.OverlapBox(center, halfExtents, orientation, -1, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents)
		{
			return Physics.OverlapBox(center, halfExtents, Quaternion.identity, -1, QueryTriggerInteraction.UseGlobal);
		}

		public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("AllLayers")] int mask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.OverlapBox(center, halfExtents, results, orientation, mask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation, int mask)
		{
			return Physics.OverlapBoxNonAlloc(center, halfExtents, results, orientation, mask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation)
		{
			return Physics.OverlapBoxNonAlloc(center, halfExtents, results, orientation, -1, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results)
		{
			return Physics.OverlapBoxNonAlloc(center, halfExtents, results, Quaternion.identity, -1, QueryTriggerInteraction.UseGlobal);
		}

		public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.BoxCast(center, halfExtents, direction, results, orientation, maxDistance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation)
		{
			return Physics.BoxCastNonAlloc(center, halfExtents, direction, results, orientation, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation, float maxDistance)
		{
			return Physics.BoxCastNonAlloc(center, halfExtents, direction, results, orientation, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation, float maxDistance, int layerMask)
		{
			return Physics.BoxCastNonAlloc(center, halfExtents, direction, results, orientation, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results)
		{
			return Physics.BoxCastNonAlloc(center, halfExtents, direction, results, Quaternion.identity, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		[FreeFunction("Physics::BoxCastAll")]
		private static RaycastHit[] Internal_BoxCastAll(PhysicsScene physicsScene, Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			RaycastHit[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Physics.Internal_BoxCastAll_Injected(ref physicsScene, ref center, ref halfExtents, ref direction, ref orientation, maxDistance, layerMask, queryTriggerInteraction, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RaycastHit[] array;
				blittableArrayWrapper.Unmarshal<RaycastHit>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			float magnitude = direction.magnitude;
			bool flag = magnitude > float.Epsilon;
			RaycastHit[] array;
			if (flag)
			{
				Vector3 vector = direction / magnitude;
				array = Physics.Internal_BoxCastAll(Physics.defaultPhysicsScene, center, halfExtents, vector, orientation, maxDistance, layerMask, queryTriggerInteraction);
			}
			else
			{
				array = new RaycastHit[0];
			}
			return array;
		}

		[ExcludeFromDocs]
		public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, int layerMask)
		{
			return Physics.BoxCastAll(center, halfExtents, direction, orientation, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance)
		{
			return Physics.BoxCastAll(center, halfExtents, direction, orientation, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation)
		{
			return Physics.BoxCastAll(center, halfExtents, direction, orientation, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction)
		{
			return Physics.BoxCastAll(center, halfExtents, direction, Quaternion.identity, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
		}

		public static int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results, [DefaultValue("AllLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.defaultPhysicsScene.OverlapCapsule(point0, point1, radius, results, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		public static int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results, int layerMask)
		{
			return Physics.OverlapCapsuleNonAlloc(point0, point1, radius, results, layerMask, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public static int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results)
		{
			return Physics.OverlapCapsuleNonAlloc(point0, point1, radius, results, -1, QueryTriggerInteraction.UseGlobal);
		}

		[StaticAccessor("GetPhysicsManager()")]
		public static void RebuildBroadphaseRegions(Bounds worldBounds, int subdivisions)
		{
			Physics.RebuildBroadphaseRegions_Injected(ref worldBounds, subdivisions);
		}

		[StaticAccessor("GetPhysicsManager()")]
		[ThreadSafe]
		public static void BakeMesh(EntityId meshEntityId, bool convex, MeshColliderCookingOptions cookingOptions)
		{
			Physics.BakeMesh_Injected(ref meshEntityId, convex, cookingOptions);
		}

		[Obsolete("BakeMesh(int, bool, MeshColliderCookingOptions) is obsolete. Use BakeMesh(EntityId, bool, MeshColliderCookingOptions) instead.")]
		public static void BakeMesh(int meshID, bool convex, MeshColliderCookingOptions cookingOptions)
		{
			Physics.BakeMesh(meshID, convex, cookingOptions);
		}

		[Obsolete("BakeMesh(int, bool) is obsolete. Use BakeMesh(EntityId, bool) instead.")]
		public static void BakeMesh(int meshID, bool convex)
		{
			Physics.BakeMesh(meshID, convex, MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase);
		}

		public static void BakeMesh(EntityId meshEntityId, bool convex)
		{
			Physics.BakeMesh(meshEntityId, convex, MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase);
		}

		[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool ConnectPhysicsSDKVisualDebugger();

		[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DisconnectPhysicsSDKVisualDebugger();

		[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
		internal static Collider GetColliderByInstanceID(EntityId entityId)
		{
			return Unmarshal.UnmarshalUnityObject<Collider>(Physics.GetColliderByInstanceID_Injected(ref entityId));
		}

		[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
		internal static Component GetBodyByInstanceID(EntityId entityId)
		{
			return Unmarshal.UnmarshalUnityObject<Component>(Physics.GetBodyByInstanceID_Injected(ref entityId));
		}

		[ThreadSafe]
		[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
		internal static uint TranslateTriangleIndexFromID(EntityId instanceID, uint faceIndex)
		{
			return Physics.TranslateTriangleIndexFromID_Injected(ref instanceID, faceIndex);
		}

		[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
		private static void SendOnCollisionEnter(Component component, Collision collision)
		{
			Physics.SendOnCollisionEnter_Injected(Object.MarshalledUnityObject.Marshal<Component>(component), collision);
		}

		[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
		private static void SendOnCollisionStay(Component component, Collision collision)
		{
			Physics.SendOnCollisionStay_Injected(Object.MarshalledUnityObject.Marshal<Component>(component), collision);
		}

		[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
		private static void SendOnCollisionExit(Component component, Collision collision)
		{
			Physics.SendOnCollisionExit_Injected(Object.MarshalledUnityObject.Marshal<Component>(component), collision);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Physics.autoSimulation has been replaced by Physics.simulationMode", false)]
		[ExcludeFromDocs]
		public static bool autoSimulation
		{
			get
			{
				return Physics.simulationMode != SimulationMode.Script;
			}
			set
			{
				Physics.simulationMode = (value ? SimulationMode.FixedUpdate : SimulationMode.Script);
			}
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Physics.autoSyncTransforms has been deprecated please use Physics.SyncTransforms instead to manually sync physics transforms when required.", false)]
		public static extern bool autoSyncTransforms
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Physics.ContactEventDelegate ContactEvent;

		[RequiredByNativeCode]
		private static void OnSceneContact(PhysicsScene scene, IntPtr buffer, int count)
		{
			bool flag = count == 0;
			if (!flag)
			{
				NativeArray<ContactPairHeader> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ContactPairHeader>(buffer.ToPointer(), count, Allocator.None);
				try
				{
					Physics.ContactEventDelegate contactEvent = Physics.ContactEvent;
					if (contactEvent != null)
					{
						contactEvent(scene, nativeArray.AsReadOnly());
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}
				finally
				{
					Physics.ReportContacts(nativeArray.AsReadOnly());
				}
			}
		}

		private static void ReportContacts(NativeArray<ContactPairHeader>.ReadOnly array)
		{
			bool flag = !Physics.invokeCollisionCallbacks;
			if (!flag)
			{
				for (int i = 0; i < array.Length; i++)
				{
					ContactPairHeader contactPairHeader = array[i];
					bool hasRemovedBody = contactPairHeader.hasRemovedBody;
					if (!hasRemovedBody)
					{
						int num = 0;
						while ((long)num < (long)((ulong)contactPairHeader.m_NbPairs))
						{
							readonly ref ContactPair contactPair = ref contactPairHeader.GetContactPair(num);
							bool hasRemovedCollider = contactPair.hasRemovedCollider;
							if (!hasRemovedCollider)
							{
								Component body = contactPairHeader.body;
								Component otherBody = contactPairHeader.otherBody;
								Component component = ((body != null) ? body : contactPair.collider);
								Component component2 = ((otherBody != null) ? otherBody : contactPair.otherCollider);
								bool flag2 = !component || !component2;
								if (!flag2)
								{
									bool isCollisionEnter = contactPair.isCollisionEnter;
									if (isCollisionEnter)
									{
										Physics.SendOnCollisionEnter(component, Physics.GetCollisionToReport(in contactPairHeader, in contactPair, false));
										Physics.SendOnCollisionEnter(component2, Physics.GetCollisionToReport(in contactPairHeader, in contactPair, true));
									}
									bool isCollisionStay = contactPair.isCollisionStay;
									if (isCollisionStay)
									{
										Physics.SendOnCollisionStay(component, Physics.GetCollisionToReport(in contactPairHeader, in contactPair, false));
										Physics.SendOnCollisionStay(component2, Physics.GetCollisionToReport(in contactPairHeader, in contactPair, true));
									}
									bool isCollisionExit = contactPair.isCollisionExit;
									if (isCollisionExit)
									{
										Physics.SendOnCollisionExit(component, Physics.GetCollisionToReport(in contactPairHeader, in contactPair, false));
										Physics.SendOnCollisionExit(component2, Physics.GetCollisionToReport(in contactPairHeader, in contactPair, true));
									}
								}
							}
							num++;
						}
					}
				}
			}
		}

		private static Collision GetCollisionToReport(in ContactPairHeader header, in ContactPair pair, bool flipped)
		{
			bool reuseCollisionCallbacks = Physics.reuseCollisionCallbacks;
			Collision collision;
			if (reuseCollisionCallbacks)
			{
				Physics.s_ReusableCollision.Reuse(in header, in pair);
				Physics.s_ReusableCollision.Flipped = flipped;
				collision = Physics.s_ReusableCollision;
			}
			else
			{
				collision = new Collision(in header, in pair, flipped);
			}
			return collision;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Physics()
		{
			Physics.GenericContactModifyEvent = new Action<PhysicsScene, IntPtr, int, bool>(Physics.PhysXOnSceneContactModify);
			Physics.s_ReusableCollision = new Collision();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_gravity_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_gravity_Injected([In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IgnoreCollision_Injected(IntPtr collider1, IntPtr collider2, [DefaultValue("true")] bool ignore);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIgnoreCollision_Injected(IntPtr collider1, IntPtr collider2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_RaycastAll_Injected([In] ref PhysicsScene physicsScene, [In] ref Ray ray, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Query_CapsuleCastAll_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 p0, [In] ref Vector3 p1, float radius, [In] ref Vector3 direction, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Query_SphereCastAll_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 origin, float radius, [In] ref Vector3 direction, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider[] OverlapCapsule_Internal_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 point0, [In] ref Vector3 point1, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider[] OverlapSphere_Internal_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Simulate_Internal_Injected([In] ref PhysicsScene physicsScene, float step, SimulationStage stages, SimulationOption options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InterpolateBodies_Internal_Injected([In] ref PhysicsScene physicsScene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetInterpolationPoses_Internal_Injected([In] ref PhysicsScene physicsScene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Query_ComputePenetration_Injected(IntPtr colliderA, [In] ref Vector3 positionA, [In] ref Quaternion rotationA, IntPtr colliderB, [In] ref Vector3 positionB, [In] ref Quaternion rotationB, ref Vector3 direction, ref float distance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Query_ClosestPoint_Injected(IntPtr collider, [In] ref Vector3 position, [In] ref Quaternion rotation, [In] ref Vector3 point, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_clothGravity_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_clothGravity_Injected([In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CheckSphere_Internal_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CheckCapsule_Internal_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 start, [In] ref Vector3 end, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CheckBox_Internal_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 center, [In] ref Vector3 halfExtents, [In] ref Quaternion orientation, int layermask, QueryTriggerInteraction queryTriggerInteraction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider[] OverlapBox_Internal_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 center, [In] ref Vector3 halfExtents, [In] ref Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BoxCastAll_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 center, [In] ref Vector3 halfExtents, [In] ref Vector3 direction, [In] ref Quaternion orientation, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RebuildBroadphaseRegions_Injected([In] ref Bounds worldBounds, int subdivisions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BakeMesh_Injected([In] ref EntityId meshEntityId, bool convex, MeshColliderCookingOptions cookingOptions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetColliderByInstanceID_Injected([In] ref EntityId entityId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetBodyByInstanceID_Injected([In] ref EntityId entityId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint TranslateTriangleIndexFromID_Injected([In] ref EntityId instanceID, uint faceIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendOnCollisionEnter_Injected(IntPtr component, Collision collision);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendOnCollisionStay_Injected(IntPtr component, Collision collision);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendOnCollisionExit_Injected(IntPtr component, Collision collision);

		internal const float k_MaxFloatMinusEpsilon = 3.4028233E+38f;

		public const int IgnoreRaycastLayer = 4;

		public const int DefaultRaycastLayers = -5;

		public const int AllLayers = -1;

		private static readonly Collision s_ReusableCollision;

		public delegate void ContactEventDelegate(PhysicsScene scene, NativeArray<ContactPairHeader>.ReadOnly headerArray);
	}
}
