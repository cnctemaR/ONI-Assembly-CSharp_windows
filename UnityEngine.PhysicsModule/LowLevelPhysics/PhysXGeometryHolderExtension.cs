using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.LowLevelPhysics
{
	[NativeHeader("Modules/Physics/PhysicsCollisionGeometry.h")]
	internal static class PhysXGeometryHolderExtension
	{
		[FreeFunction("Physics::PhysXGeometryExtension::GetGeometryHolderFromCollider")]
		public static GeometryHolder GetGeometryHolder(this Collider col)
		{
			GeometryHolder geometryHolder;
			PhysXGeometryHolderExtension.GetGeometryHolder_Injected(Object.MarshalledUnityObject.Marshal<Collider>(col), out geometryHolder);
			return geometryHolder;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGeometryHolder_Injected(IntPtr col, out GeometryHolder ret);
	}
}
