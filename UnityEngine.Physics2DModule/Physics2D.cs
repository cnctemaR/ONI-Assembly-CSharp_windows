using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Physics2DScriptingClasses.h")]
	[NativeHeader("Physics2DScriptingClasses.h")]
	[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
	[NativeHeader("Modules/Physics2D/PhysicsManager2D.h")]
	public class Physics2D
	{
		[NativeProperty("DefaultPhysicsSceneHandle")]
		[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
		public static PhysicsScene2D defaultPhysicsScene
		{
			get
			{
				PhysicsScene2D physicsScene2D;
				Physics2D.get_defaultPhysicsScene_Injected(out physicsScene2D);
				return physicsScene2D;
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
		public static extern bool autoSyncTransforms
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern bool autoSimulation
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
		public static extern float velocityThreshold
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

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern bool alwaysShowColliders
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern bool showColliderSleep
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern bool showColliderContacts
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern bool showColliderAABB
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static extern float contactArrowScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static Color colliderAwakeColor
		{
			get
			{
				Color color;
				Physics2D.get_colliderAwakeColor_Injected(out color);
				return color;
			}
			set
			{
				Physics2D.set_colliderAwakeColor_Injected(ref value);
			}
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static Color colliderAsleepColor
		{
			get
			{
				Color color;
				Physics2D.get_colliderAsleepColor_Injected(out color);
				return color;
			}
			set
			{
				Physics2D.set_colliderAsleepColor_Injected(ref value);
			}
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static Color colliderContactColor
		{
			get
			{
				Color color;
				Physics2D.get_colliderContactColor_Injected(out color);
				return color;
			}
			set
			{
				Physics2D.set_colliderContactColor_Injected(ref value);
			}
		}

		[StaticAccessor("GetPhysics2DSettings()")]
		public static Color colliderAABBColor
		{
			get
			{
				Color color;
				Physics2D.get_colliderAABBColor_Injected(out color);
				return color;
			}
			set
			{
				Physics2D.set_colliderAABBColor_Injected(ref value);
			}
		}

		public static bool Simulate(float step)
		{
			return Physics2D.Simulate_Internal(Physics2D.defaultPhysicsScene, step);
		}

		[NativeMethod("Simulate_Binding")]
		internal static bool Simulate_Internal(PhysicsScene2D physicsScene, float step)
		{
			return Physics2D.Simulate_Internal_Injected(ref physicsScene, step);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SyncTransforms();

		[ExcludeFromDocs]
		public static void IgnoreCollision([Writable] Collider2D collider1, [Writable] Collider2D collider2)
		{
			Physics2D.IgnoreCollision(collider1, collider2, true);
		}

		[StaticAccessor("PhysicsScene2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("IgnoreCollision_Binding")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void IgnoreCollision([Writable] [NotNull] Collider2D collider1, [Writable] [NotNull] Collider2D collider2, [DefaultValue("true")] bool ignore);

		[StaticAccessor("PhysicsScene2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("GetIgnoreCollision_Binding")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetIgnoreCollision([Writable] Collider2D collider1, [Writable] Collider2D collider2);

		[ExcludeFromDocs]
		public static void IgnoreLayerCollision(int layer1, int layer2)
		{
			Physics2D.IgnoreLayerCollision(layer1, layer2, true);
		}

		public static void IgnoreLayerCollision(int layer1, int layer2, bool ignore)
		{
			if (layer1 < 0 || layer1 > 31)
			{
				throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
			}
			if (layer2 < 0 || layer2 > 31)
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
			if (layer1 < 0 || layer1 > 31)
			{
				throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
			}
			if (layer2 < 0 || layer2 > 31)
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
			if (layer < 0 || layer > 31)
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
			if (layer < 0 || layer > 31)
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
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsTouching([Writable] [NotNull] Collider2D collider1, [Writable] [NotNull] Collider2D collider2);

		public static bool IsTouching([Writable] Collider2D collider1, [Writable] Collider2D collider2, ContactFilter2D contactFilter)
		{
			return Physics2D.IsTouching_TwoCollidersWithFilter(collider1, collider2, contactFilter);
		}

		[NativeMethod("IsTouching")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static bool IsTouching_TwoCollidersWithFilter([NotNull] [Writable] Collider2D collider1, [NotNull] [Writable] Collider2D collider2, ContactFilter2D contactFilter)
		{
			return Physics2D.IsTouching_TwoCollidersWithFilter_Injected(collider1, collider2, ref contactFilter);
		}

		public static bool IsTouching([Writable] Collider2D collider, ContactFilter2D contactFilter)
		{
			return Physics2D.IsTouching_SingleColliderWithFilter(collider, contactFilter);
		}

		[NativeMethod("IsTouching")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static bool IsTouching_SingleColliderWithFilter([NotNull] [Writable] Collider2D collider, ContactFilter2D contactFilter)
		{
			return Physics2D.IsTouching_SingleColliderWithFilter_Injected(collider, ref contactFilter);
		}

		[ExcludeFromDocs]
		public static bool IsTouchingLayers([Writable] Collider2D collider)
		{
			return Physics2D.IsTouchingLayers(collider, -1);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsTouchingLayers([Writable] [NotNull] Collider2D collider, [DefaultValue("Physics2D.AllLayers")] int layerMask);

		public static ColliderDistance2D Distance([Writable] Collider2D colliderA, [Writable] Collider2D colliderB)
		{
			if (colliderA == null)
			{
				throw new ArgumentNullException("ColliderA cannot be NULL.");
			}
			if (colliderB == null)
			{
				throw new ArgumentNullException("ColliderB cannot be NULL.");
			}
			if (colliderA == colliderB)
			{
				throw new ArgumentException("Cannot calculate the distance between the same collider.");
			}
			return Physics2D.Distance_Internal(colliderA, colliderB);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("Distance")]
		private static ColliderDistance2D Distance_Internal([NotNull] [Writable] Collider2D colliderA, [Writable] [NotNull] Collider2D colliderB)
		{
			ColliderDistance2D colliderDistance2D;
			Physics2D.Distance_Internal_Injected(colliderA, colliderB, out colliderDistance2D);
			return colliderDistance2D;
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

		[NativeMethod("LinecastAll_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static RaycastHit2D[] LinecastAll_Internal(PhysicsScene2D physicsScene, Vector2 start, Vector2 end, ContactFilter2D contactFilter)
		{
			return Physics2D.LinecastAll_Internal_Injected(ref physicsScene, ref start, ref end, ref contactFilter);
		}

		[ExcludeFromDocs]
		public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.Linecast(start, end, results, -5);
		}

		[ExcludeFromDocs]
		public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.Linecast(start, end, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.Linecast(start, end, contactFilter2D, results);
		}

		public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.Linecast(start, end, contactFilter2D, results);
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

		[ExcludeFromDocs]
		[RequiredByNativeCode]
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

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, float.PositiveInfinity, results, -5);
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance)
		{
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, results, -5);
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter2D, results);
		}

		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter2D, results);
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
			return Physics2D.RaycastAll_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, ref contactFilter);
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

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("CircleCastAll_Binding")]
		private static RaycastHit2D[] CircleCastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			return Physics2D.CircleCastAll_Internal_Injected(ref physicsScene, ref origin, radius, ref direction, distance, ref contactFilter);
		}

		[ExcludeFromDocs]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, float.PositiveInfinity, results, -5);
		}

		[ExcludeFromDocs]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance)
		{
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, results, -5);
		}

		[ExcludeFromDocs]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter2D, results);
		}

		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter2D, results);
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

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("BoxCastAll_Binding")]
		private static RaycastHit2D[] BoxCastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			return Physics2D.BoxCastAll_Internal_Injected(ref physicsScene, ref origin, ref size, angle, ref direction, distance, ref contactFilter);
		}

		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, float.PositiveInfinity, results, -5);
		}

		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
		{
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, results, -5);
		}

		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter2D, results);
		}

		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter2D, results);
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
			return Physics2D.CapsuleCastAll_Internal_Injected(ref physicsScene, ref origin, ref size, capsuleDirection, angle, ref direction, distance, ref contactFilter);
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
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, float.PositiveInfinity, results, -5);
		}

		[ExcludeFromDocs]
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
		{
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, results, -5);
		}

		[ExcludeFromDocs]
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter2D, results);
		}

		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter2D, results);
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

		public static RaycastHit2D GetRayIntersection(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics2D.defaultPhysicsScene.GetRayIntersection(ray, distance, layerMask);
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
		public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics2D.GetRayIntersectionAll_Internal(Physics2D.defaultPhysicsScene, ray.origin, ray.direction, distance, layerMask);
		}

		[NativeMethod("GetRayIntersectionAll_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static RaycastHit2D[] GetRayIntersectionAll_Internal(PhysicsScene2D physicsScene, Vector3 origin, Vector3 direction, float distance, int layerMask)
		{
			return Physics2D.GetRayIntersectionAll_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results)
		{
			return Physics2D.defaultPhysicsScene.GetRayIntersection(ray, float.PositiveInfinity, results, -5);
		}

		[ExcludeFromDocs]
		public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, float distance)
		{
			return Physics2D.defaultPhysicsScene.GetRayIntersection(ray, distance, results, -5);
		}

		[RequiredByNativeCode]
		public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
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

		public static int OverlapPoint(Vector2 point, ContactFilter2D contactFilter, Collider2D[] results)
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
		public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, results, -5);
		}

		[ExcludeFromDocs]
		public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, contactFilter2D, results);
		}

		public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapPoint(point, contactFilter2D, results);
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

		[NativeMethod("OverlapCircleAll_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static Collider2D[] OverlapCircleAll_Internal(PhysicsScene2D physicsScene, Vector2 point, float radius, ContactFilter2D contactFilter)
		{
			return Physics2D.OverlapCircleAll_Internal_Injected(ref physicsScene, ref point, radius, ref contactFilter);
		}

		[ExcludeFromDocs]
		public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, results, -5);
		}

		[ExcludeFromDocs]
		public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, contactFilter2D, results);
		}

		public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapCircle(point, radius, contactFilter2D, results);
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
		public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, results, -5);
		}

		[ExcludeFromDocs]
		public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter2D, results);
		}

		public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter2D, results);
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
		public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, results, -5);
		}

		[ExcludeFromDocs]
		public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter2D, results);
		}

		public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter2D, results);
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

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("OverlapCapsuleAll_Binding")]
		private static Collider2D[] OverlapCapsuleAll_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter)
		{
			return Physics2D.OverlapCapsuleAll_Internal_Injected(ref physicsScene, ref point, ref size, direction, angle, ref contactFilter);
		}

		[ExcludeFromDocs]
		public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results)
		{
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, results, -5);
		}

		[ExcludeFromDocs]
		public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter2D, results);
		}

		public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return Physics2D.defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter2D, results);
		}

		public static int OverlapCollider(Collider2D collider, ContactFilter2D contactFilter, Collider2D[] results)
		{
			return PhysicsScene2D.OverlapCollider(collider, contactFilter, results);
		}

		public static int GetContacts(Collider2D collider1, Collider2D collider2, ContactFilter2D contactFilter, ContactPoint2D[] contacts)
		{
			return Physics2D.GetColliderColliderContacts(collider1, collider2, contactFilter, contacts);
		}

		public static int GetContacts(Collider2D collider, ContactPoint2D[] contacts)
		{
			return Physics2D.GetColliderContacts(collider, default(ContactFilter2D).NoFilter(), contacts);
		}

		public static int GetContacts(Collider2D collider, ContactFilter2D contactFilter, ContactPoint2D[] contacts)
		{
			return Physics2D.GetColliderContacts(collider, contactFilter, contacts);
		}

		public static int GetContacts(Collider2D collider, Collider2D[] colliders)
		{
			return Physics2D.GetColliderContactsCollidersOnly(collider, default(ContactFilter2D).NoFilter(), colliders);
		}

		public static int GetContacts(Collider2D collider, ContactFilter2D contactFilter, Collider2D[] colliders)
		{
			return Physics2D.GetColliderContactsCollidersOnly(collider, contactFilter, colliders);
		}

		public static int GetContacts(Rigidbody2D rigidbody, ContactPoint2D[] contacts)
		{
			return Physics2D.GetRigidbodyContacts(rigidbody, default(ContactFilter2D).NoFilter(), contacts);
		}

		public static int GetContacts(Rigidbody2D rigidbody, ContactFilter2D contactFilter, ContactPoint2D[] contacts)
		{
			return Physics2D.GetRigidbodyContacts(rigidbody, contactFilter, contacts);
		}

		public static int GetContacts(Rigidbody2D rigidbody, Collider2D[] colliders)
		{
			return Physics2D.GetRigidbodyContactsCollidersOnly(rigidbody, default(ContactFilter2D).NoFilter(), colliders);
		}

		public static int GetContacts(Rigidbody2D rigidbody, ContactFilter2D contactFilter, Collider2D[] colliders)
		{
			return Physics2D.GetRigidbodyContactsCollidersOnly(rigidbody, contactFilter, colliders);
		}

		[NativeMethod("GetColliderContacts_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static int GetColliderContacts([NotNull] Collider2D collider, ContactFilter2D contactFilter, [Out] ContactPoint2D[] results)
		{
			return Physics2D.GetColliderContacts_Injected(collider, ref contactFilter, results);
		}

		[NativeMethod("GetColliderColliderContacts_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static int GetColliderColliderContacts([NotNull] Collider2D collider1, [NotNull] Collider2D collider2, ContactFilter2D contactFilter, [Out] ContactPoint2D[] results)
		{
			return Physics2D.GetColliderColliderContacts_Injected(collider1, collider2, ref contactFilter, results);
		}

		[NativeMethod("GetRigidbodyContacts_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static int GetRigidbodyContacts([NotNull] Rigidbody2D rigidbody, ContactFilter2D contactFilter, [Out] ContactPoint2D[] results)
		{
			return Physics2D.GetRigidbodyContacts_Injected(rigidbody, ref contactFilter, results);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("GetColliderContactsCollidersOnly_Binding")]
		private static int GetColliderContactsCollidersOnly([NotNull] Collider2D collider, ContactFilter2D contactFilter, [Out] Collider2D[] results)
		{
			return Physics2D.GetColliderContactsCollidersOnly_Injected(collider, ref contactFilter, results);
		}

		[NativeMethod("GetRigidbodyContactsCollidersOnly_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static int GetRigidbodyContactsCollidersOnly([NotNull] Rigidbody2D rigidbody, ContactFilter2D contactFilter, [Out] Collider2D[] results)
		{
			return Physics2D.GetRigidbodyContactsCollidersOnly_Injected(rigidbody, ref contactFilter, results);
		}

		internal static void SetEditorDragMovement(bool dragging, GameObject[] objs)
		{
			foreach (Rigidbody2D rigidbody2D in Physics2D.m_LastDisabledRigidbody2D)
			{
				if (rigidbody2D != null)
				{
					rigidbody2D.SetDragBehaviour(false);
				}
			}
			Physics2D.m_LastDisabledRigidbody2D.Clear();
			if (dragging)
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_defaultPhysicsScene_Injected(out PhysicsScene2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_gravity_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_gravity_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_jobOptions_Injected(out PhysicsJobOptions2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_jobOptions_Injected(ref PhysicsJobOptions2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_colliderAwakeColor_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_colliderAwakeColor_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_colliderAsleepColor_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_colliderAsleepColor_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_colliderContactColor_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_colliderContactColor_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_colliderAABBColor_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_colliderAABBColor_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Simulate_Internal_Injected(ref PhysicsScene2D physicsScene, float step);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouching_TwoCollidersWithFilter_Injected([Writable] Collider2D collider1, [Writable] Collider2D collider2, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouching_SingleColliderWithFilter_Injected([Writable] Collider2D collider, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Distance_Internal_Injected([Writable] Collider2D colliderA, [Writable] Collider2D colliderB, out ColliderDistance2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit2D[] LinecastAll_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 start, ref Vector2 end, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit2D[] RaycastAll_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 origin, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit2D[] CircleCastAll_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 origin, float radius, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit2D[] BoxCastAll_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit2D[] CapsuleCastAll_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 origin, ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit2D[] GetRayIntersectionAll_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector3 origin, ref Vector3 direction, float distance, int layerMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] OverlapPointAll_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 point, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] OverlapCircleAll_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 point, float radius, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] OverlapBoxAll_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 point, ref Vector2 size, float angle, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] OverlapCapsuleAll_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 point, ref Vector2 size, CapsuleDirection2D direction, float angle, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetColliderContacts_Injected(Collider2D collider, ref ContactFilter2D contactFilter, [Out] ContactPoint2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetColliderColliderContacts_Injected(Collider2D collider1, Collider2D collider2, ref ContactFilter2D contactFilter, [Out] ContactPoint2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRigidbodyContacts_Injected(Rigidbody2D rigidbody, ref ContactFilter2D contactFilter, [Out] ContactPoint2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetColliderContactsCollidersOnly_Injected(Collider2D collider, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRigidbodyContactsCollidersOnly_Injected(Rigidbody2D rigidbody, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

		public const int IgnoreRaycastLayer = 4;

		public const int DefaultRaycastLayers = -5;

		public const int AllLayers = -1;

		private static List<Rigidbody2D> m_LastDisabledRigidbody2D = new List<Rigidbody2D>();
	}
}
