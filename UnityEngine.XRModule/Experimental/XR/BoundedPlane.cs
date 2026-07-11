using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeHeader("Modules/XR/Subsystems/Planes/XRBoundedPlane.h")]
	[NativeConditional("ENABLE_XR")]
	[UsedByNativeCode]
	[NativeHeader("XRScriptingClasses.h")]
	public struct BoundedPlane
	{
		public TrackableId Id { get; set; }

		public TrackableId SubsumedById { get; set; }

		public Pose Pose { get; set; }

		public Vector3 Center { get; set; }

		public Vector2 Size { get; set; }

		public PlaneAlignment Alignment { get; set; }

		public float Width
		{
			get
			{
				return this.Size.x;
			}
		}

		public float Height
		{
			get
			{
				return this.Size.y;
			}
		}

		public Vector3 Normal
		{
			get
			{
				return this.Pose.up;
			}
		}

		public Plane Plane
		{
			get
			{
				return new Plane(this.Normal, this.Center);
			}
		}

		public void GetCorners(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3)
		{
			Vector3 vector = this.Pose.right * (this.Width * 0.5f);
			Vector3 vector2 = this.Pose.forward * (this.Height * 0.5f);
			p0 = this.Center - vector - vector2;
			p1 = this.Center - vector + vector2;
			p2 = this.Center + vector + vector2;
			p3 = this.Center + vector - vector2;
		}

		public bool TryGetBoundary(List<Vector3> boundaryOut)
		{
			if (boundaryOut == null)
			{
				throw new ArgumentNullException("boundaryOut");
			}
			return BoundedPlane.Internal_GetBoundaryAsList(this.m_InstanceId, this.Id, boundaryOut);
		}

		private static Vector3[] Internal_GetBoundaryAsFixedArray(uint instanceId, TrackableId id)
		{
			return BoundedPlane.Internal_GetBoundaryAsFixedArray_Injected(instanceId, ref id);
		}

		private static bool Internal_GetBoundaryAsList(uint instanceId, TrackableId id, List<Vector3> boundaryOut)
		{
			return BoundedPlane.Internal_GetBoundaryAsList_Injected(instanceId, ref id, boundaryOut);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector3[] Internal_GetBoundaryAsFixedArray_Injected(uint instanceId, ref TrackableId id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_GetBoundaryAsList_Injected(uint instanceId, ref TrackableId id, List<Vector3> boundaryOut);

		private uint m_InstanceId;
	}
}
