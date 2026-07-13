using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
	[NativeHeader("Physics2DScriptingClasses.h")]
	[NativeHeader("Modules/Physics2D/PhysicsManager2D.h")]
	public class Physics2D
	{
		public static PhysicsScene2D defaultPhysicsScene
		{
			get
			{
				return default(PhysicsScene2D);
			}
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern int velocityIterations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern int positionIterations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static Vector2 gravity
		{
			get
			{
				Vector2 vector;
				Physics2D.get_gravity_Injected(out vector);
				return vector;
			}
			set
			{
				Physics2D.set_gravity_Injected(ref value);
			}
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern bool queriesHitTriggers
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern bool queriesStartInColliders
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern bool callbacksOnDisable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern bool reuseCollisionCallbacks
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern SimulationMode2D simulationMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static LayerMask simulationLayers
		{
			get
			{
				LayerMask layerMask;
				Physics2D.get_simulationLayers_Injected(out layerMask);
				return layerMask;
			}
			set
			{
				Physics2D.set_simulationLayers_Injected(ref value);
			}
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern bool useSubStepping
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern bool useSubStepContacts
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float minSubStepFPS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern int maxSubStepCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static PhysicsJobOptions2D jobOptions
		{
			get
			{
				PhysicsJobOptions2D physicsJobOptions2D;
				Physics2D.get_jobOptions_Injected(out physicsJobOptions2D);
				return physicsJobOptions2D;
			}
			set
			{
				Physics2D.set_jobOptions_Injected(ref value);
			}
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float bounceThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float contactThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float maxLinearCorrection
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float maxAngularCorrection
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float maxTranslationSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float maxRotationSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float defaultContactOffset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float baumgarteScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float baumgarteTOIScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float timeToSleep
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float linearSleepTolerance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float angularSleepTolerance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[ExcludeFromDocs]
		public static bool Simulate(float deltaTime)
		{
			return Physics2D.Simulate_Internal(Physics2D.defaultPhysicsScene, deltaTime, -1);
		}

		public static bool Simulate(float deltaTime, [DefaultValue("Physics2D.AllLayers")] int simulationLayers = -1)
		{
			return Physics2D.Simulate_Internal(Physics2D.defaultPhysicsScene, deltaTime, simulationLayers);
		}

		[NativeMethod("Simulate_Binding")]
		internal static bool Simulate_Internal(PhysicsScene2D physicsScene, float deltaTime, int simulationLayers)
		{
			return Physics2D.Simulate_Internal_Injected(ref physicsScene, deltaTime, simulationLayers);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SyncTransforms();

		[ExcludeFromDocs]
		public static void IgnoreCollision(Collider2D collider1, Collider2D collider2)
		{
			Physics2D.IgnoreCollision(collider1, collider2, true);
		}

		[NativeMethod("IgnoreCollision_Binding")]
		[StaticAccessor("PhysicsScene2D", StaticAccessorType.DoubleColon)]
		public static void IgnoreCollision([NotNull] Collider2D collider1, [NotNull] Collider2D collider2, [DefaultValue("true")] bool ignore)
		{
			if (collider1 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			if (collider2 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider1);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider2);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			Physics2D.IgnoreCollision_Injected(intPtr, intPtr2, ignore);
		}

		[NativeMethod("GetIgnoreCollision_Binding")]
		[StaticAccessor("PhysicsScene2D", StaticAccessorType.DoubleColon)]
		public static bool GetIgnoreCollision([NotNull] Collider2D collider1, [NotNull] Collider2D collider2)
		{
			if (collider1 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			if (collider2 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider1);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider2);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			return Physics2D.GetIgnoreCollision_Injected(intPtr, intPtr2);
		}

		[ExcludeFromDocs]
		public static void IgnoreLayerCollision(int layer1, int layer2)
		{
			Physics2D.IgnoreLayerCollision(layer1, layer2, true);
		}

		public static void IgnoreLayerCollision(int layer1, int layer2, bool ignore)
		{
			bool flag = layer1 < 0 || layer1 > 31;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
			}
			bool flag2 = layer2 < 0 || layer2 > 31;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("layer2 is out of range. Layer numbers must be in the range 0 to 31.");
			}
			Physics2D.IgnoreLayerCollision_Internal(layer1, layer2, ignore);
		}

		[NativeMethod("IgnoreLayerCollision")]
		[StaticAccessor("GetPhysics2DSettings()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IgnoreLayerCollision_Internal(int layer1, int layer2, bool ignore);

		public static bool GetIgnoreLayerCollision(int layer1, int layer2)
		{
			bool flag = layer1 < 0 || layer1 > 31;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
			}
			bool flag2 = layer2 < 0 || layer2 > 31;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("layer2 is out of range. Layer numbers must be in the range 0 to 31.");
			}
			return Physics2D.GetIgnoreLayerCollision_Internal(layer1, layer2);
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		[NativeMethod("GetIgnoreLayerCollision")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIgnoreLayerCollision_Internal(int layer1, int layer2);

		public static void SetLayerCollisionMask(int layer, int layerMask)
		{
			bool flag = layer < 0 || layer > 31;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
			}
			Physics2D.SetLayerCollisionMask_Internal(layer, layerMask);
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		[NativeMethod("SetLayerCollisionMask")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLayerCollisionMask_Internal(int layer, int layerMask);

		public static int GetLayerCollisionMask(int layer)
		{
			bool flag = layer < 0 || layer > 31;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
			}
			return Physics2D.GetLayerCollisionMask_Internal(layer);
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		[NativeMethod("GetLayerCollisionMask")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLayerCollisionMask_Internal(int layer);

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		public static bool IsTouching([NotNull] Collider2D collider1, [NotNull] Collider2D collider2)
		{
			if (collider1 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			if (collider2 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider1);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider2);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			return Physics2D.IsTouching_Injected(intPtr, intPtr2);
		}

		public static bool IsTouching(Collider2D collider1, Collider2D collider2, ContactFilter2D contactFilter)
		{
			return Physics2D.IsTouching_TwoCollidersWithFilter(collider1, collider2, contactFilter);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("IsTouching")]
		private static bool IsTouching_TwoCollidersWithFilter([NotNull] Collider2D collider1, [NotNull] Collider2D collider2, ContactFilter2D contactFilter)
		{
			if (collider1 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			if (collider2 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider1);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider2);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			return Physics2D.IsTouching_TwoCollidersWithFilter_Injected(intPtr, intPtr2, ref contactFilter);
		}

		public static bool IsTouching(Collider2D collider, ContactFilter2D contactFilter)
		{
			return Physics2D.IsTouching_SingleColliderWithFilter(collider, contactFilter);
		}

		[NativeMethod("IsTouching")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static bool IsTouching_SingleColliderWithFilter([NotNull] Collider2D collider, ContactFilter2D contactFilter)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			return Physics2D.IsTouching_SingleColliderWithFilter_Injected(intPtr, ref contactFilter);
		}

		[ExcludeFromDocs]
		public static bool IsTouchingLayers(Collider2D collider)
		{
			return Physics2D.IsTouchingLayers(collider, -1);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		public static bool IsTouchingLayers([NotNull] Collider2D collider, [DefaultValue("Physics2D.AllLayers")] int layerMask)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			return Physics2D.IsTouchingLayers_Injected(intPtr, layerMask);
		}

		public static ColliderDistance2D Distance(Collider2D colliderA, Collider2D colliderB)
		{
			bool flag = colliderA == colliderB;
			if (flag)
			{
				throw new ArgumentException("Cannot calculate the distance between the same collider.");
			}
			return Physics2D.Distance_Internal(colliderA, colliderB);
		}

		public static ColliderDistance2D Distance(Collider2D colliderA, Vector2 positionA, float angleA, Collider2D colliderB, Vector2 positionB, float angleB)
		{
			bool flag = colliderA == colliderB;
			if (flag)
			{
				throw new ArgumentException("Cannot calculate the distance between the same collider.");
			}
			bool flag2 = !colliderA.attachedRigidbody || !colliderB.attachedRigidbody;
			if (flag2)
			{
				throw new InvalidOperationException("Cannot perform a Collider Distance at a specific position and angle if the Collider is not attached to a Rigidbody2D.");
			}
			return Physics2D.DistanceFrom_Internal(colliderA, positionA, angleA, colliderB, positionB, angleB);
		}

		[NativeMethod("Distance")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static ColliderDistance2D Distance_Internal([NotNull] Collider2D colliderA, [NotNull] Collider2D colliderB)
		{
			if (colliderA == null)
			{
				ThrowHelper.ThrowArgumentNullException(colliderA, "colliderA");
			}
			if (colliderB == null)
			{
				ThrowHelper.ThrowArgumentNullException(colliderB, "colliderB");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(colliderA);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(colliderA, "colliderA");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(colliderB);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(colliderB, "colliderB");
			}
			ColliderDistance2D colliderDistance2D;
			Physics2D.Distance_Internal_Injected(intPtr, intPtr2, out colliderDistance2D);
			return colliderDistance2D;
		}

		[NativeMethod("DistanceFrom")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static ColliderDistance2D DistanceFrom_Internal([NotNull] Collider2D colliderA, Vector2 positionA, float angleA, [NotNull] Collider2D colliderB, Vector2 positionB, float angleB)
		{
			if (colliderA == null)
			{
				ThrowHelper.ThrowArgumentNullException(colliderA, "colliderA");
			}
			if (colliderB == null)
			{
				ThrowHelper.ThrowArgumentNullException(colliderB, "colliderB");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(colliderA);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(colliderA, "colliderA");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(colliderB);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(colliderB, "colliderB");
			}
			ColliderDistance2D colliderDistance2D;
			Physics2D.DistanceFrom_Internal_Injected(intPtr, ref positionA, angleA, intPtr2, ref positionB, angleB, out colliderDistance2D);
			return colliderDistance2D;
		}

		public static Vector2 ClosestPoint(Vector2 position, Collider2D collider)
		{
			bool flag = collider == null;
			if (flag)
			{
				throw new ArgumentNullException("Collider cannot be NULL.");
			}
			return Physics2D.ClosestPoint_Collider(position, collider);
		}

		public static Vector2 ClosestPoint(Vector2 position, Rigidbody2D rigidbody)
		{
			bool flag = rigidbody == null;
			if (flag)
			{
				throw new ArgumentNullException("Rigidbody cannot be NULL.");
			}
			return Physics2D.ClosestPoint_Rigidbody(position, rigidbody);
		}

		[NativeMethod("ClosestPoint")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static Vector2 ClosestPoint_Collider(Vector2 position, [NotNull] Collider2D collider)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			Vector2 vector;
			Physics2D.ClosestPoint_Collider_Injected(ref position, intPtr, out vector);
			return vector;
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("ClosestPoint")]
		private static Vector2 ClosestPoint_Rigidbody(Vector2 position, [NotNull] Rigidbody2D rigidbody)
		{
			if (rigidbody == null)
			{
				ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(rigidbody);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
			}
			Vector2 vector;
			Physics2D.ClosestPoint_Rigidbody_Injected(ref position, intPtr, out vector);
			return vector;
		}

		[ExcludeFromDocs]
		public static RaycastHit2D Linecast(Vector2 start, Vector2 end)
		{
			return Physics2D.defaultPhysicsScene.Linecast(start, end, -5);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.Linecast(start, end, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.Linecast(start, end, contactFilter2D);
		}

		public static RaycastHit2D Linecast(Vector2 start, Vector2 end, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.Linecast(start, end, contactFilter2D);
		}

		public static int Linecast(Vector2 start, Vector2 end, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.Linecast(start, end, contactFilter, results);
		}

		public static int Linecast(Vector2 start, Vector2 end, ContactFilter2D contactFilter, List<RaycastHit2D> results)
		{
			return Physics2D.defaultPhysicsScene.Linecast(start, end, contactFilter, results);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.LinecastAll_Internal(Physics2D.defaultPhysicsScene, start, end, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.LinecastAll_Internal(Physics2D.defaultPhysicsScene, start, end, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.LinecastAll_Internal(Physics2D.defaultPhysicsScene, start, end, contactFilter2D);
		}

		public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.LinecastAll_Internal(Physics2D.defaultPhysicsScene, start, end, contactFilter2D);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("LinecastAll_Binding")]
		private static RaycastHit2D[] LinecastAll_Internal(PhysicsScene2D physicsScene, Vector2 start, Vector2 end, ContactFilter2D contactFilter)
		{
			RaycastHit2D[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Physics2D.LinecastAll_Internal_Injected(ref physicsScene, ref start, ref end, ref contactFilter, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RaycastHit2D[] array;
				blittableArrayWrapper.Unmarshal<RaycastHit2D>(ref array);
				array2 = array;
			}
			return array2;
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction)
		{
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, float.PositiveInfinity, -5);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance)
		{
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, -5);
		}

		[RequiredByNativeCode]
		[ExcludeFromDocs]
		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter2D);
		}

		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static int Raycast(Vector2 origin, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, float.PositiveInfinity, contactFilter, results);
		}

		public static int Raycast(Vector2 origin, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
		{
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter, results);
		}

		public static int Raycast(Vector2 origin, Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
		{
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter, results);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.RaycastAll_Internal(Physics2D.defaultPhysicsScene, origin, direction, float.PositiveInfinity, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.RaycastAll_Internal(Physics2D.defaultPhysicsScene, origin, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.RaycastAll_Internal(Physics2D.defaultPhysicsScene, origin, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.RaycastAll_Internal(Physics2D.defaultPhysicsScene, origin, direction, distance, contactFilter2D);
		}

		public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.RaycastAll_Internal(Physics2D.defaultPhysicsScene, origin, direction, distance, contactFilter2D);
		}

		[NativeMethod("RaycastAll_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static RaycastHit2D[] RaycastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			RaycastHit2D[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Physics2D.RaycastAll_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, ref contactFilter, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RaycastHit2D[] array;
				blittableArrayWrapper.Unmarshal<RaycastHit2D>(ref array);
				array2 = array;
			}
			return array2;
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction)
		{
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, float.PositiveInfinity, -5);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance)
		{
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, -5);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter2D);
		}

		public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static int CircleCast(Vector2 origin, float radius, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, float.PositiveInfinity, contactFilter, results);
		}

		public static int CircleCast(Vector2 origin, float radius, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
		{
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter, results);
		}

		public static int CircleCast(Vector2 origin, float radius, Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
		{
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter, results);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.CircleCastAll_Internal(Physics2D.defaultPhysicsScene, origin, radius, direction, float.PositiveInfinity, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.CircleCastAll_Internal(Physics2D.defaultPhysicsScene, origin, radius, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.CircleCastAll_Internal(Physics2D.defaultPhysicsScene, origin, radius, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.CircleCastAll_Internal(Physics2D.defaultPhysicsScene, origin, radius, direction, distance, contactFilter2D);
		}

		public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.CircleCastAll_Internal(Physics2D.defaultPhysicsScene, origin, radius, direction, distance, contactFilter2D);
		}

		[NativeMethod("CircleCastAll_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static RaycastHit2D[] CircleCastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			RaycastHit2D[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Physics2D.CircleCastAll_Internal_Injected(ref physicsScene, ref origin, radius, ref direction, distance, ref contactFilter, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RaycastHit2D[] array;
				blittableArrayWrapper.Unmarshal<RaycastHit2D>(ref array);
				array2 = array;
			}
			return array2;
		}

		[ExcludeFromDocs]
		public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction)
		{
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, float.PositiveInfinity, -5);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
		{
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, -5);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter2D);
		}

		public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("Physics2D.AllLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, float.PositiveInfinity, contactFilter, results);
		}

		public static int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
		{
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter, results);
		}

		public static int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
		{
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter, results);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.BoxCastAll_Internal(Physics2D.defaultPhysicsScene, origin, size, angle, direction, float.PositiveInfinity, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.BoxCastAll_Internal(Physics2D.defaultPhysicsScene, origin, size, angle, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.BoxCastAll_Internal(Physics2D.defaultPhysicsScene, origin, size, angle, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.BoxCastAll_Internal(Physics2D.defaultPhysicsScene, origin, size, angle, direction, distance, contactFilter2D);
		}

		public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.BoxCastAll_Internal(Physics2D.defaultPhysicsScene, origin, size, angle, direction, distance, contactFilter2D);
		}

		[NativeMethod("BoxCastAll_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static RaycastHit2D[] BoxCastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			RaycastHit2D[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Physics2D.BoxCastAll_Internal_Injected(ref physicsScene, ref origin, ref size, angle, ref direction, distance, ref contactFilter, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RaycastHit2D[] array;
				blittableArrayWrapper.Unmarshal<RaycastHit2D>(ref array);
				array2 = array;
			}
			return array2;
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction)
		{
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, float.PositiveInfinity, -5);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
		{
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, -5);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter2D);
		}

		public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, float.PositiveInfinity, contactFilter, results);
		}

		public static int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
		{
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
		}

		public static int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
		{
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.CapsuleCastAll_Internal(Physics2D.defaultPhysicsScene, origin, size, capsuleDirection, angle, direction, float.PositiveInfinity, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.CapsuleCastAll_Internal(Physics2D.defaultPhysicsScene, origin, size, capsuleDirection, angle, direction, distance, contactFilter2D);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("CapsuleCastAll_Binding")]
		private static RaycastHit2D[] CapsuleCastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			RaycastHit2D[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Physics2D.CapsuleCastAll_Internal_Injected(ref physicsScene, ref origin, ref size, capsuleDirection, angle, ref direction, distance, ref contactFilter, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RaycastHit2D[] array;
				blittableArrayWrapper.Unmarshal<RaycastHit2D>(ref array);
				array2 = array;
			}
			return array2;
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.CapsuleCastAll_Internal(Physics2D.defaultPhysicsScene, origin, size, capsuleDirection, angle, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.CapsuleCastAll_Internal(Physics2D.defaultPhysicsScene, origin, size, capsuleDirection, angle, direction, distance, contactFilter2D);
		}

		public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.CapsuleCastAll_Internal(Physics2D.defaultPhysicsScene, origin, size, capsuleDirection, angle, direction, distance, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D GetRayIntersection(Ray ray)
		{
			return Physics2D.defaultPhysicsScene.GetRayIntersection(ray, float.PositiveInfinity, -5);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D GetRayIntersection(Ray ray, float distance)
		{
			return Physics2D.defaultPhysicsScene.GetRayIntersection(ray, distance, -5);
		}

		public static RaycastHit2D GetRayIntersection(Ray ray, float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask = -5)
		{
			return Physics2D.defaultPhysicsScene.GetRayIntersection(ray, distance, layerMask);
		}

		public static int GetRayIntersection(Ray ray, float distance, List<RaycastHit2D> results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			return Physics2D.defaultPhysicsScene.GetRayIntersection(ray, distance, results, layerMask);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] GetRayIntersectionAll(Ray ray)
		{
			return Physics2D.GetRayIntersectionAll_Internal(Physics2D.defaultPhysicsScene, ray.origin, ray.direction, float.PositiveInfinity, -5);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, float distance)
		{
			return Physics2D.GetRayIntersectionAll_Internal(Physics2D.defaultPhysicsScene, ray.origin, ray.direction, distance, -5);
		}

		[RequiredByNativeCode]
		public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask = -5)
		{
			return Physics2D.GetRayIntersectionAll_Internal(Physics2D.defaultPhysicsScene, ray.origin, ray.direction, distance, layerMask);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("GetRayIntersectionAll_Binding")]
		private static RaycastHit2D[] GetRayIntersectionAll_Internal(PhysicsScene2D physicsScene, Vector3 origin, Vector3 direction, float distance, int layerMask)
		{
			RaycastHit2D[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Physics2D.GetRayIntersectionAll_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, layerMask, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RaycastHit2D[] array;
				blittableArrayWrapper.Unmarshal<RaycastHit2D>(ref array);
				array2 = array;
			}
			return array2;
		}

		[ExcludeFromDocs]
		[RequiredByNativeCode]
		public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask = -5)
		{
			return Physics2D.defaultPhysicsScene.GetRayIntersection(ray, distance, results, layerMask);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapPoint(Vector2 point)
		{
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, -5);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapPoint(Vector2 point, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapPoint(Vector2 point, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, contactFilter2D);
		}

		public static Collider2D OverlapPoint(Vector2 point, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, contactFilter2D);
		}

		public static int OverlapPoint(Vector2 point, ContactFilter2D contactFilter, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, contactFilter, results);
		}

		public static int OverlapPoint(Vector2 point, ContactFilter2D contactFilter, List<Collider2D> results)
		{
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, contactFilter, results);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapPointAll(Vector2 point)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.OverlapPointAll_Internal(Physics2D.defaultPhysicsScene, point, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.OverlapPointAll_Internal(Physics2D.defaultPhysicsScene, point, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.OverlapPointAll_Internal(Physics2D.defaultPhysicsScene, point, contactFilter2D);
		}

		public static Collider2D[] OverlapPointAll(Vector2 point, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.OverlapPointAll_Internal(Physics2D.defaultPhysicsScene, point, contactFilter2D);
		}

		[NativeMethod("OverlapPointAll_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static Collider2D[] OverlapPointAll_Internal(PhysicsScene2D physicsScene, Vector2 point, ContactFilter2D contactFilter)
		{
			return Physics2D.OverlapPointAll_Internal_Injected(ref physicsScene, ref point, ref contactFilter);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapCircle(Vector2 point, float radius)
		{
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, -5);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, contactFilter2D);
		}

		public static Collider2D OverlapCircle(Vector2 point, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, contactFilter2D);
		}

		public static int OverlapCircle(Vector2 point, float radius, ContactFilter2D contactFilter, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, contactFilter, results);
		}

		public static int OverlapCircle(Vector2 point, float radius, ContactFilter2D contactFilter, List<Collider2D> results)
		{
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, contactFilter, results);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapCircleAll(Vector2 point, float radius)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.OverlapCircleAll_Internal(Physics2D.defaultPhysicsScene, point, radius, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.OverlapCircleAll_Internal(Physics2D.defaultPhysicsScene, point, radius, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.OverlapCircleAll_Internal(Physics2D.defaultPhysicsScene, point, radius, contactFilter2D);
		}

		public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.OverlapCircleAll_Internal(Physics2D.defaultPhysicsScene, point, radius, contactFilter2D);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("OverlapCircleAll_Binding")]
		private static Collider2D[] OverlapCircleAll_Internal(PhysicsScene2D physicsScene, Vector2 point, float radius, ContactFilter2D contactFilter)
		{
			return Physics2D.OverlapCircleAll_Internal_Injected(ref physicsScene, ref point, radius, ref contactFilter);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle)
		{
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, -5);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter2D);
		}

		public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter2D);
		}

		public static int OverlapBox(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter, results);
		}

		public static int OverlapBox(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, List<Collider2D> results)
		{
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter, results);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.OverlapBoxAll_Internal(Physics2D.defaultPhysicsScene, point, size, angle, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.OverlapBoxAll_Internal(Physics2D.defaultPhysicsScene, point, size, angle, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.OverlapBoxAll_Internal(Physics2D.defaultPhysicsScene, point, size, angle, contactFilter2D);
		}

		public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.OverlapBoxAll_Internal(Physics2D.defaultPhysicsScene, point, size, angle, contactFilter2D);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("OverlapBoxAll_Binding")]
		private static Collider2D[] OverlapBoxAll_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter)
		{
			return Physics2D.OverlapBoxAll_Internal_Injected(ref physicsScene, ref point, ref size, angle, ref contactFilter);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB)
		{
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, -5);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter2D);
		}

		public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter2D);
		}

		public static int OverlapArea(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter, results);
		}

		public static int OverlapArea(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter, List<Collider2D> results)
		{
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter, results);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB)
		{
			return Physics2D.OverlapAreaAllToBox_Internal(pointA, pointB, -5, float.NegativeInfinity, float.PositiveInfinity);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask)
		{
			return Physics2D.OverlapAreaAllToBox_Internal(pointA, pointB, layerMask, float.NegativeInfinity, float.PositiveInfinity);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth)
		{
			return Physics2D.OverlapAreaAllToBox_Internal(pointA, pointB, layerMask, minDepth, float.PositiveInfinity);
		}

		public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.OverlapAreaAllToBox_Internal(pointA, pointB, layerMask, minDepth, maxDepth);
		}

		private static Collider2D[] OverlapAreaAllToBox_Internal(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth, float maxDepth)
		{
			Vector2 vector = (pointA + pointB) * 0.5f;
			Vector2 vector2 = new Vector2(Mathf.Abs(pointA.x - pointB.x), Math.Abs(pointA.y - pointB.y));
			return Physics2D.OverlapBoxAll(vector, vector2, 0f, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
		{
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, -5);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter2D);
		}

		public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter2D);
		}

		public static int OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter, results);
		}

		public static int OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, List<Collider2D> results)
		{
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter, results);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.OverlapCapsuleAll_Internal(Physics2D.defaultPhysicsScene, point, size, direction, angle, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.OverlapCapsuleAll_Internal(Physics2D.defaultPhysicsScene, point, size, direction, angle, contactFilter2D);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.OverlapCapsuleAll_Internal(Physics2D.defaultPhysicsScene, point, size, direction, angle, contactFilter2D);
		}

		public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.OverlapCapsuleAll_Internal(Physics2D.defaultPhysicsScene, point, size, direction, angle, contactFilter2D);
		}

		[NativeMethod("OverlapCapsuleAll_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static Collider2D[] OverlapCapsuleAll_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter)
		{
			return Physics2D.OverlapCapsuleAll_Internal_Injected(ref physicsScene, ref point, ref size, direction, angle, ref contactFilter);
		}

		public static int OverlapCollider(Collider2D collider, ContactFilter2D contactFilter, Collider2D[] results)
		{
			return PhysicsScene2D.OverlapCollider(collider, contactFilter, results);
		}

		public static int OverlapCollider(Collider2D collider, ContactFilter2D contactFilter, List<Collider2D> results)
		{
			return PhysicsScene2D.OverlapCollider(collider, contactFilter, results);
		}

		public static int OverlapCollider(Collider2D collider, List<Collider2D> results)
		{
			return PhysicsScene2D.OverlapCollider(collider, results);
		}

		public static int GetContacts(Collider2D collider1, Collider2D collider2, ContactFilter2D contactFilter, ContactPoint2D[] contacts)
		{
			return Physics2D.GetColliderColliderContactsArray(collider1, collider2, contactFilter, contacts);
		}

		public static int GetContacts(Collider2D collider, ContactPoint2D[] contacts)
		{
			return Physics2D.GetColliderContactsArray(collider, ContactFilter2D.noFilter, contacts);
		}

		public static int GetContacts(Collider2D collider, ContactFilter2D contactFilter, ContactPoint2D[] contacts)
		{
			return Physics2D.GetColliderContactsArray(collider, contactFilter, contacts);
		}

		public static int GetContacts(Collider2D collider, Collider2D[] colliders)
		{
			return Physics2D.GetColliderContactsCollidersOnlyArray(collider, ContactFilter2D.noFilter, colliders);
		}

		public static int GetContacts(Collider2D collider, ContactFilter2D contactFilter, Collider2D[] colliders)
		{
			return Physics2D.GetColliderContactsCollidersOnlyArray(collider, contactFilter, colliders);
		}

		public static int GetContacts(Rigidbody2D rigidbody, ContactPoint2D[] contacts)
		{
			return Physics2D.GetRigidbodyContactsArray(rigidbody, ContactFilter2D.noFilter, contacts);
		}

		public static int GetContacts(Rigidbody2D rigidbody, ContactFilter2D contactFilter, ContactPoint2D[] contacts)
		{
			return Physics2D.GetRigidbodyContactsArray(rigidbody, contactFilter, contacts);
		}

		public static int GetContacts(Rigidbody2D rigidbody, Collider2D[] colliders)
		{
			return Physics2D.GetRigidbodyContactsCollidersOnlyArray(rigidbody, ContactFilter2D.noFilter, colliders);
		}

		public static int GetContacts(Rigidbody2D rigidbody, ContactFilter2D contactFilter, Collider2D[] colliders)
		{
			return Physics2D.GetRigidbodyContactsCollidersOnlyArray(rigidbody, contactFilter, colliders);
		}

		[NativeMethod("GetColliderContactsArray_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private unsafe static int GetColliderContactsArray([NotNull] Collider2D collider, ContactFilter2D contactFilter, [NotNull] ContactPoint2D[] results)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			Span<ContactPoint2D> span = new Span<ContactPoint2D>(results);
			int colliderContactsArray_Injected;
			fixed (ContactPoint2D* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				colliderContactsArray_Injected = Physics2D.GetColliderContactsArray_Injected(intPtr, ref contactFilter, ref managedSpanWrapper);
			}
			return colliderContactsArray_Injected;
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("GetColliderColliderContactsArray_Binding")]
		private unsafe static int GetColliderColliderContactsArray([NotNull] Collider2D collider1, [NotNull] Collider2D collider2, ContactFilter2D contactFilter, [NotNull] ContactPoint2D[] results)
		{
			if (collider1 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			if (collider2 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider1);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider2);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			Span<ContactPoint2D> span = new Span<ContactPoint2D>(results);
			int colliderColliderContactsArray_Injected;
			fixed (ContactPoint2D* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				colliderColliderContactsArray_Injected = Physics2D.GetColliderColliderContactsArray_Injected(intPtr, intPtr2, ref contactFilter, ref managedSpanWrapper);
			}
			return colliderColliderContactsArray_Injected;
		}

		[NativeMethod("GetRigidbodyContactsArray_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private unsafe static int GetRigidbodyContactsArray([NotNull] Rigidbody2D rigidbody, ContactFilter2D contactFilter, [NotNull] ContactPoint2D[] results)
		{
			if (rigidbody == null)
			{
				ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
			}
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(rigidbody);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
			}
			Span<ContactPoint2D> span = new Span<ContactPoint2D>(results);
			int rigidbodyContactsArray_Injected;
			fixed (ContactPoint2D* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				rigidbodyContactsArray_Injected = Physics2D.GetRigidbodyContactsArray_Injected(intPtr, ref contactFilter, ref managedSpanWrapper);
			}
			return rigidbodyContactsArray_Injected;
		}

		[NativeMethod("GetColliderContactsCollidersOnlyArray_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static int GetColliderContactsCollidersOnlyArray([NotNull] Collider2D collider, ContactFilter2D contactFilter, [NotNull] [UnityMarshalAs(NativeType.ScriptingObjectPtr)] Collider2D[] results)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			return Physics2D.GetColliderContactsCollidersOnlyArray_Injected(intPtr, ref contactFilter, results);
		}

		[NativeMethod("GetRigidbodyContactsCollidersOnlyArray_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static int GetRigidbodyContactsCollidersOnlyArray([NotNull] Rigidbody2D rigidbody, ContactFilter2D contactFilter, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] [NotNull] Collider2D[] results)
		{
			if (rigidbody == null)
			{
				ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
			}
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(rigidbody);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
			}
			return Physics2D.GetRigidbodyContactsCollidersOnlyArray_Injected(intPtr, ref contactFilter, results);
		}

		public static int GetContacts(Collider2D collider1, Collider2D collider2, ContactFilter2D contactFilter, List<ContactPoint2D> contacts)
		{
			return Physics2D.GetColliderColliderContactsList(collider1, collider2, contactFilter, contacts);
		}

		public static int GetContacts(Collider2D collider, List<ContactPoint2D> contacts)
		{
			return Physics2D.GetColliderContactsList(collider, ContactFilter2D.noFilter, contacts);
		}

		public static int GetContacts(Collider2D collider, ContactFilter2D contactFilter, List<ContactPoint2D> contacts)
		{
			return Physics2D.GetColliderContactsList(collider, contactFilter, contacts);
		}

		public static int GetContacts(Collider2D collider, List<Collider2D> colliders)
		{
			return Physics2D.GetColliderContactsCollidersOnlyList(collider, ContactFilter2D.noFilter, colliders);
		}

		public static int GetContacts(Collider2D collider, ContactFilter2D contactFilter, List<Collider2D> colliders)
		{
			return Physics2D.GetColliderContactsCollidersOnlyList(collider, contactFilter, colliders);
		}

		public static int GetContacts(Rigidbody2D rigidbody, List<ContactPoint2D> contacts)
		{
			return Physics2D.GetRigidbodyContactsList(rigidbody, ContactFilter2D.noFilter, contacts);
		}

		public static int GetContacts(Rigidbody2D rigidbody, ContactFilter2D contactFilter, List<ContactPoint2D> contacts)
		{
			return Physics2D.GetRigidbodyContactsList(rigidbody, contactFilter, contacts);
		}

		public static int GetContacts(Rigidbody2D rigidbody, List<Collider2D> colliders)
		{
			return Physics2D.GetRigidbodyContactsCollidersOnlyList(rigidbody, ContactFilter2D.noFilter, colliders);
		}

		public static int GetContacts(Rigidbody2D rigidbody, ContactFilter2D contactFilter, List<Collider2D> colliders)
		{
			return Physics2D.GetRigidbodyContactsCollidersOnlyList(rigidbody, contactFilter, colliders);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("GetColliderContactsList_Binding")]
		private unsafe static int GetColliderContactsList([NotNull] Collider2D collider, ContactFilter2D contactFilter, [NotNull] List<ContactPoint2D> results)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			int colliderContactsList_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(collider, "collider");
				}
				fixed (ContactPoint2D[] array = NoAllocHelpers.ExtractArrayFromList<ContactPoint2D>(results))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, results.Count);
					colliderContactsList_Injected = Physics2D.GetColliderContactsList_Injected(intPtr, ref contactFilter, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<ContactPoint2D>(results);
			}
			return colliderContactsList_Injected;
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("GetColliderColliderContactsList_Binding")]
		private unsafe static int GetColliderColliderContactsList([NotNull] Collider2D collider1, [NotNull] Collider2D collider2, ContactFilter2D contactFilter, [NotNull] List<ContactPoint2D> results)
		{
			if (collider1 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			if (collider2 == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			int colliderColliderContactsList_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider1);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider2);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
				}
				fixed (ContactPoint2D[] array = NoAllocHelpers.ExtractArrayFromList<ContactPoint2D>(results))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, results.Count);
					colliderColliderContactsList_Injected = Physics2D.GetColliderColliderContactsList_Injected(intPtr, intPtr2, ref contactFilter, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<ContactPoint2D>(results);
			}
			return colliderColliderContactsList_Injected;
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("GetRigidbodyContactsList_Binding")]
		private unsafe static int GetRigidbodyContactsList([NotNull] Rigidbody2D rigidbody, ContactFilter2D contactFilter, [NotNull] List<ContactPoint2D> results)
		{
			if (rigidbody == null)
			{
				ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
			}
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			int rigidbodyContactsList_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(rigidbody);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
				}
				fixed (ContactPoint2D[] array = NoAllocHelpers.ExtractArrayFromList<ContactPoint2D>(results))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, results.Count);
					rigidbodyContactsList_Injected = Physics2D.GetRigidbodyContactsList_Injected(intPtr, ref contactFilter, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<ContactPoint2D>(results);
			}
			return rigidbodyContactsList_Injected;
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("GetColliderContactsCollidersOnlyList_Binding")]
		private static int GetColliderContactsCollidersOnlyList([NotNull] Collider2D collider, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			return Physics2D.GetColliderContactsCollidersOnlyList_Injected(intPtr, ref contactFilter, results);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("GetRigidbodyContactsCollidersOnlyList_Binding")]
		private static int GetRigidbodyContactsCollidersOnlyList([NotNull] Rigidbody2D rigidbody, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
		{
			if (rigidbody == null)
			{
				ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
			}
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(rigidbody);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
			}
			return Physics2D.GetRigidbodyContactsCollidersOnlyList_Injected(intPtr, ref contactFilter, results);
		}

		internal static void SetEditorDragMovement(bool dragging, GameObject[] objs)
		{
			foreach (Rigidbody2D rigidbody2D in Physics2D.m_LastDisabledRigidbody2D)
			{
				bool flag = rigidbody2D != null;
				if (flag)
				{
					rigidbody2D.SetDragBehaviour(false);
				}
			}
			Physics2D.m_LastDisabledRigidbody2D.Clear();
			bool flag2 = !dragging;
			if (!flag2)
			{
				foreach (GameObject gameObject in objs)
				{
					Rigidbody2D[] componentsInChildren = gameObject.GetComponentsInChildren<Rigidbody2D>(false);
					foreach (Rigidbody2D rigidbody2D2 in componentsInChildren)
					{
						Physics2D.m_LastDisabledRigidbody2D.Add(rigidbody2D2);
						rigidbody2D2.SetDragBehaviour(true);
					}
				}
			}
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[StaticAccessor("GetPhysics2DSettings()")]
		[Obsolete("Physics2D.autoSyncTransforms has been deprecated please use Physics2D.SyncTransforms instead to manually sync physics transforms when required.", false)]
		public static extern bool autoSyncTransforms
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Physics2D.raycastsHitTriggers is obsolete. Use Physics2D.queriesHitTriggers instead. (UnityUpgradable) -> queriesHitTriggers", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool raycastsHitTriggers
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Physics2D.raycastsStartInColliders is obsolete. Use Physics2D.queriesStartInColliders instead. (UnityUpgradable) -> queriesStartInColliders", true)]
		public static bool raycastsStartInColliders
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[Obsolete("Physics2D.deleteStopsCallbacks is obsolete.(UnityUpgradable) -> changeStopsCallbacks", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool deleteStopsCallbacks
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[Obsolete("Physics2D.changeStopsCallbacks is obsolete and will always return false.", true)]
		public static bool changeStopsCallbacks
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[Obsolete("Physics2D.minPenetrationForPenalty is obsolete. Use Physics2D.defaultContactOffset instead. (UnityUpgradable) -> defaultContactOffset", true)]
		public static float minPenetrationForPenalty
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[ExcludeFromDocs]
		[Obsolete("Physics2D.velocityThreshold is obsolete. Use Physics2D.bounceThreshold instead. (UnityUpgradable) -> bounceThreshold", true)]
		public static float velocityThreshold
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Physics2D.autoSimulation is obsolete. Use Physics2D.simulationMode instead.", true)]
		public static bool autoSimulation
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Physics2D.colliderAwakeColor is obsolete. This options has been moved to 2D Preferences.", true)]
		[ExcludeFromDocs]
		public static Color colliderAwakeColor
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Physics2D.colliderAsleepColor is obsolete. This options has been moved to 2D Preferences.", true)]
		public static Color colliderAsleepColor
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Physics2D.colliderContactColor is obsolete. This options has been moved to 2D Preferences.", true)]
		public static Color colliderContactColor
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		[Obsolete("Physics2D.colliderAABBColor is obsolete. All Physics 2D colors moved to Preferences. This is now known as 'Collider Bounds Color'.", true)]
		public static Color colliderAABBColor
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[ExcludeFromDocs]
		[Obsolete("Physics2D.contactArrowScale is obsolete. This options has been moved to 2D Preferences.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static float contactArrowScale
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[Obsolete("Physics2D.alwaysShowColliders is obsolete. It is no longer available in the Editor or Builds.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		public static bool alwaysShowColliders
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Physics2D.showCollidersFilled is obsolete. It is no longer available in the Editor or Builds.", true)]
		public static bool showCollidersFilled
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		[Obsolete("Physics2D.showColliderSleep is obsolete. It is no longer available in the Editor or Builds.", true)]
		public static bool showColliderSleep
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		[Obsolete("Physics2D.showColliderContacts is obsolete. It is no longer available in the Editor or Builds.", true)]
		public static bool showColliderContacts
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[Obsolete("Physics2D.showColliderAABB is obsolete. It is no longer available in the Editor or Builds.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		public static bool showColliderAABB
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[Obsolete("LinecastNonAlloc has neen deprecated. Please use Linecast.", false)]
		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.Linecast(start, end, results, -5);
		}

		[Obsolete("LinecastNonAlloc has been deprecated. Please use Linecast.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.Linecast(start, end, contactFilter2D, results);
		}

		[Obsolete("LinecastNonAlloc has been deprecated. Please use Linecast.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.Linecast(start, end, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("LinecastNonAlloc has been deprecated. Please use Linecast.", false)]
		public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.Linecast(start, end, contactFilter2D, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("RaycastNonAlloc has been deprecated. Please use Raycast.", false)]
		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, float.PositiveInfinity, results, -5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("RaycastNonAlloc has been deprecated. Please use Raycast.", false)]
		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance)
		{
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, results, -5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		[Obsolete("RaycastNonAlloc has been deprecated. Please use Raycast.", false)]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("RaycastNonAlloc has been deprecated. Please use Raycast.", false)]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		[Obsolete("CircleCastNonAlloc has been deprecated. Please use CircleCast instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, float.PositiveInfinity, results, -5);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("CircleCastNonAlloc has been deprecated. Please use CircleCast instead.", false)]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance)
		{
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, results, -5);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("CircleCastNonAlloc has been deprecated. Please use CircleCast instead.", false)]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter2D, results);
		}

		[Obsolete("CircleCastNonAlloc has been deprecated. Please use CircleCast instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("CircleCastNonAlloc has been deprecated. Please use CircleCast instead.", false)]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("BoxCastNonAlloc has been deprecated. Please use BoxCast.", false)]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, float.PositiveInfinity, results, -5);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("BoxCastNonAlloc has been deprecated. Please use BoxCast.", false)]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
		{
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, results, -5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		[Obsolete("BoxCastNonAlloc has been deprecated. Please use BoxCast.", false)]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("BoxCastNonAlloc has been deprecated. Please use BoxCast.", false)]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter2D, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("BoxCastNonAlloc has been deprecated. Please use BoxCast.", false)]
		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter2D, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		[Obsolete("CapsuleCastNonAlloc has been deprecated. Please use CapsuleCast.", false)]
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, float.PositiveInfinity, results, -5);
		}

		[ExcludeFromDocs]
		[Obsolete("CapsuleCastNonAlloc has been deprecated. Please use CapsuleCast.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
		{
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, results, -5);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("CapsuleCastNonAlloc has been deprecated. Please use CapsuleCast.", false)]
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		[Obsolete("CapsuleCastNonAlloc has been deprecated. Please use CapsuleCast.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter2D, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("CapsuleCastNonAlloc has been deprecated. Please use CapsuleCast.", false)]
		[ExcludeFromDocs]
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("GetRayIntersectionNonAlloc is deprecated. Please use GetRayIntersection.", false)]
		public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.GetRayIntersection(ray, float.PositiveInfinity, results, -5);
		}

		[ExcludeFromDocs]
		[Obsolete("GetRayIntersectionNonAlloc is deprecated. Please use GetRayIntersection.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, float distance)
		{
			return Physics2D.defaultPhysicsScene.GetRayIntersection(ray, distance, results, -5);
		}

		[Obsolete("OverlapPointNonAlloc has been deprecated. Please use OverlapPoint.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, results, -5);
		}

		[Obsolete("OverlapPointNonAlloc has been deprecated. Please use OverlapPoint.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, contactFilter2D, results);
		}

		[Obsolete("OverlapPointNonAlloc has been deprecated. Please use OverlapPoint.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("OverlapPointNonAlloc has been deprecated. Please use OverlapPoint.", false)]
		public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, contactFilter2D, results);
		}

		[Obsolete("OverlapCircleNonAlloc has been deprecated. Please use OverlapCircle.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, results, -5);
		}

		[ExcludeFromDocs]
		[Obsolete("OverlapCircleNonAlloc has been deprecated. Please use OverlapCircle.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		[Obsolete("OverlapCircleNonAlloc has been deprecated. Please use OverlapCircle.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, contactFilter2D, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		[Obsolete("OverlapCircleNonAlloc has been deprecated. Please use OverlapCircle.", false)]
		public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, contactFilter2D, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("OverlapBoxNonAlloc has been deprecated. Please use OverlapBox.", false)]
		[ExcludeFromDocs]
		public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, results, -5);
		}

		[Obsolete("OverlapBoxNonAlloc has been deprecated. Please use OverlapBox.", false)]
		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter2D, results);
		}

		[Obsolete("OverlapBoxNonAlloc has been deprecated. Please use OverlapBox.", false)]
		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("OverlapBoxNonAlloc has been deprecated. Please use OverlapBox.", false)]
		public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter2D, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("OverlapAreaNonAlloc has been deprecated. Please use OverlapArea.", false)]
		[ExcludeFromDocs]
		public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, results, -5);
		}

		[Obsolete("OverlapAreaNonAlloc has been deprecated. Please use OverlapArea.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter2D, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		[Obsolete("OverlapAreaNonAlloc has been deprecated. Please use OverlapArea.", false)]
		public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter2D, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("OverlapAreaNonAlloc has been deprecated. Please use OverlapArea.", false)]
		[ExcludeFromDocs]
		public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter2D, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromDocs]
		[Obsolete("OverlapCapsuleNonAlloc has been deprecated. Please use OverlapCapsule.", false)]
		public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, results, -5);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("OverlapCapsuleNonAlloc has been deprecated. Please use OverlapCapsule.", false)]
		public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		[Obsolete("OverlapCapsuleNonAlloc has been deprecated. Please use OverlapCapsule.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter2D, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("OverlapCapsuleNonAlloc has been deprecated. Please use OverlapCapsule.", false)]
		[ExcludeFromDocs]
		public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter2D, results);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_gravity_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_gravity_Injected([In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_simulationLayers_Injected(out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_simulationLayers_Injected([In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_jobOptions_Injected(out PhysicsJobOptions2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_jobOptions_Injected([In] ref PhysicsJobOptions2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Simulate_Internal_Injected([In] ref PhysicsScene2D physicsScene, float deltaTime, int simulationLayers);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IgnoreCollision_Injected(IntPtr collider1, IntPtr collider2, [DefaultValue("true")] bool ignore);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIgnoreCollision_Injected(IntPtr collider1, IntPtr collider2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouching_Injected(IntPtr collider1, IntPtr collider2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouching_TwoCollidersWithFilter_Injected(IntPtr collider1, IntPtr collider2, [In] ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouching_SingleColliderWithFilter_Injected(IntPtr collider, [In] ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouchingLayers_Injected(IntPtr collider, [DefaultValue("Physics2D.AllLayers")] int layerMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Distance_Internal_Injected(IntPtr colliderA, IntPtr colliderB, out ColliderDistance2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DistanceFrom_Internal_Injected(IntPtr colliderA, [In] ref Vector2 positionA, float angleA, IntPtr colliderB, [In] ref Vector2 positionB, float angleB, out ColliderDistance2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClosestPoint_Collider_Injected([In] ref Vector2 position, IntPtr collider, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClosestPoint_Rigidbody_Injected([In] ref Vector2 position, IntPtr rigidbody, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LinecastAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 start, [In] ref Vector2 end, [In] ref ContactFilter2D contactFilter, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RaycastAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CircleCastAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, float radius, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BoxCastAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 size, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CapsuleCastAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRayIntersectionAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector3 origin, [In] ref Vector3 direction, float distance, int layerMask, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] OverlapPointAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] OverlapCircleAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, float radius, [In] ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] OverlapBoxAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref Vector2 size, float angle, [In] ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] OverlapCapsuleAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref Vector2 size, CapsuleDirection2D direction, float angle, [In] ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetColliderContactsArray_Injected(IntPtr collider, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetColliderColliderContactsArray_Injected(IntPtr collider1, IntPtr collider2, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRigidbodyContactsArray_Injected(IntPtr rigidbody, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetColliderContactsCollidersOnlyArray_Injected(IntPtr collider, [In] ref ContactFilter2D contactFilter, Collider2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRigidbodyContactsCollidersOnlyArray_Injected(IntPtr rigidbody, [In] ref ContactFilter2D contactFilter, Collider2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetColliderContactsList_Injected(IntPtr collider, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetColliderColliderContactsList_Injected(IntPtr collider1, IntPtr collider2, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRigidbodyContactsList_Injected(IntPtr rigidbody, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetColliderContactsCollidersOnlyList_Injected(IntPtr collider, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRigidbodyContactsCollidersOnlyList_Injected(IntPtr rigidbody, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);

		public const int IgnoreRaycastLayer = 4;

		public const int DefaultRaycastLayers = -5;

		public const int AllLayers = -1;

		public const int MaxPolygonShapeVertices = 8;

		private static List<Rigidbody2D> m_LastDisabledRigidbody2D = new List<Rigidbody2D>();

		[Flags]
		internal enum GizmoOptions
		{
			AllColliders = 1,
			CollidersOutlined = 2,
			CollidersFilled = 4,
			CollidersSleeping = 8,
			ColliderContacts = 16,
			ColliderBounds = 32
		}
	}
}
