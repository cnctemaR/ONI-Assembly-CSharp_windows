using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Runtime/Dynamics/Public/PhysicsSceneHandle.h")]
	public struct PhysicsScene : IEquatable<PhysicsScene>
	{
		public override string ToString()
		{
			return UnityString.Format("({0})", new object[] { this.m_Handle });
		}

		public static bool operator ==(PhysicsScene lhs, PhysicsScene rhs)
		{
			return lhs.m_Handle == rhs.m_Handle;
		}

		public static bool operator !=(PhysicsScene lhs, PhysicsScene rhs)
		{
			return lhs.m_Handle != rhs.m_Handle;
		}

		public override int GetHashCode()
		{
			return this.m_Handle;
		}

		public override bool Equals(object other)
		{
			bool flag;
			if (!(other is PhysicsScene))
			{
				flag = false;
			}
			else
			{
				PhysicsScene physicsScene = (PhysicsScene)other;
				flag = this.m_Handle == physicsScene.m_Handle;
			}
			return flag;
		}

		public bool Equals(PhysicsScene other)
		{
			return this.m_Handle == other.m_Handle;
		}

		public bool IsValid()
		{
			return PhysicsScene.IsValid_Internal(this);
		}

		[StaticAccessor("GetPhysicsManager()", StaticAccessorType.Dot)]
		[NativeMethod("IsPhysicsSceneValid")]
		private static bool IsValid_Internal(PhysicsScene physicsScene)
		{
			return PhysicsScene.IsValid_Internal_Injected(ref physicsScene);
		}

		public bool IsEmpty()
		{
			if (this.IsValid())
			{
				return PhysicsScene.IsEmpty_Internal(this);
			}
			throw new InvalidOperationException("Cannot check if physics scene is empty as it is invalid.");
		}

		[StaticAccessor("GetPhysicsManager()", StaticAccessorType.Dot)]
		[NativeMethod("IsPhysicsWorldEmpty")]
		private static bool IsEmpty_Internal(PhysicsScene physicsScene)
		{
			return PhysicsScene.IsEmpty_Internal_Injected(ref physicsScene);
		}

		public void Simulate(float step)
		{
			if (this.IsValid())
			{
				if (this == Physics.defaultPhysicsScene && Physics.autoSimulation)
				{
					Debug.LogWarning("PhysicsScene.Simulate(...) was called but auto simulation is active. You should disable auto simulation first before calling this function therefore the simulation was not run.");
				}
				else
				{
					Physics.Simulate_Internal(this, step);
				}
				return;
			}
			throw new InvalidOperationException("Cannot simulate the physics scene as it is invalid.");
		}

		public bool Raycast(Vector3 origin, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance = float.PositiveInfinity, [DefaultValue("Physics.DefaultRaycastLayers")] int layerMask = -5, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			float magnitude = direction.magnitude;
			bool flag;
			if (magnitude > 1E-45f)
			{
				Vector3 vector = direction / magnitude;
				Ray ray = new Ray(origin, vector);
				flag = PhysicsScene.Internal_RaycastTest(this, ray, maxDistance, layerMask, queryTriggerInteraction);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		[NativeName("RaycastTest")]
		[StaticAccessor("GetPhysicsManager().GetPhysicsQuery()", StaticAccessorType.Dot)]
		private static bool Internal_RaycastTest(PhysicsScene physicsScene, Ray ray, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			return PhysicsScene.Internal_RaycastTest_Injected(ref physicsScene, ref ray, maxDistance, layerMask, queryTriggerInteraction);
		}

		public bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance = float.PositiveInfinity, [DefaultValue("Physics.DefaultRaycastLayers")] int layerMask = -5, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			hitInfo = default(RaycastHit);
			float magnitude = direction.magnitude;
			bool flag;
			if (magnitude > 1E-45f)
			{
				Vector3 vector = direction / magnitude;
				Ray ray = new Ray(origin, vector);
				flag = PhysicsScene.Internal_Raycast(this, ray, maxDistance, ref hitInfo, layerMask, queryTriggerInteraction);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		[NativeName("Raycast")]
		[StaticAccessor("GetPhysicsManager().GetPhysicsQuery()", StaticAccessorType.Dot)]
		private static bool Internal_Raycast(PhysicsScene physicsScene, Ray ray, float maxDistance, ref RaycastHit hit, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			return PhysicsScene.Internal_Raycast_Injected(ref physicsScene, ref ray, maxDistance, ref hit, layerMask, queryTriggerInteraction);
		}

		public int Raycast(Vector3 origin, Vector3 direction, RaycastHit[] raycastHits, [DefaultValue("Mathf.Infinity")] float maxDistance = float.PositiveInfinity, [DefaultValue("Physics.DefaultRaycastLayers")] int layerMask = -5, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			float magnitude = direction.magnitude;
			int num;
			if (magnitude > 1E-45f)
			{
				Ray ray = new Ray(origin, direction.normalized);
				num = PhysicsScene.Internal_RaycastNonAlloc(this, ray, raycastHits, maxDistance, layerMask, queryTriggerInteraction);
			}
			else
			{
				num = 0;
			}
			return num;
		}

		[StaticAccessor("GetPhysicsManager().GetPhysicsQuery()")]
		[NativeName("RaycastNonAlloc")]
		private static int Internal_RaycastNonAlloc(PhysicsScene physicsScene, Ray ray, RaycastHit[] raycastHits, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
		{
			return PhysicsScene.Internal_RaycastNonAlloc_Injected(ref physicsScene, ref ray, raycastHits, maxDistance, mask, queryTriggerInteraction);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValid_Internal_Injected(ref PhysicsScene physicsScene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsEmpty_Internal_Injected(ref PhysicsScene physicsScene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_RaycastTest_Injected(ref PhysicsScene physicsScene, ref Ray ray, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Raycast_Injected(ref PhysicsScene physicsScene, ref Ray ray, float maxDistance, ref RaycastHit hit, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_RaycastNonAlloc_Injected(ref PhysicsScene physicsScene, ref Ray ray, RaycastHit[] raycastHits, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction);

		private int m_Handle;
	}
}
