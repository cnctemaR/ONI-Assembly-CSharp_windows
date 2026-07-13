using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Terrain/Public/TerrainData.h")]
	[NativeHeader("Modules/TerrainPhysics/TerrainCollider.h")]
	public class TerrainCollider : Collider
	{
		public TerrainData terrainData
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<TerrainData>(TerrainCollider.get_terrainData_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TerrainCollider.set_terrainData_Injected(intPtr, Object.MarshalledUnityObject.Marshal<TerrainData>(value));
			}
		}

		private RaycastHit Raycast(Ray ray, float maxDistance, bool hitHoles, ref bool hasHit)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<TerrainCollider>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RaycastHit raycastHit;
			TerrainCollider.Raycast_Injected(intPtr, ref ray, maxDistance, hitHoles, ref hasHit, out raycastHit);
			return raycastHit;
		}

		internal bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, bool hitHoles)
		{
			bool flag = false;
			hitInfo = this.Raycast(ray, maxDistance, hitHoles, ref flag);
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_terrainData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_terrainData_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Raycast_Injected(IntPtr _unity_self, [In] ref Ray ray, float maxDistance, bool hitHoles, ref bool hasHit, out RaycastHit ret);
	}
}
