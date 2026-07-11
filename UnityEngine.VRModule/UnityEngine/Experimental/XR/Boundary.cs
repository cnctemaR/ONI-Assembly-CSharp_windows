using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.Experimental.XR
{
	[NativeConditional("ENABLE_VR")]
	public static class Boundary
	{
		public static bool TryGetDimensions(out Vector3 dimensionsOut)
		{
			return Boundary.TryGetDimensions(out dimensionsOut, Boundary.Type.PlayArea);
		}

		public static bool TryGetDimensions(out Vector3 dimensionsOut, [DefaultValue("Type.PlayArea")] Boundary.Type boundaryType)
		{
			return Boundary.TryGetDimensionsInternal(out dimensionsOut, boundaryType);
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[NativeName("TryGetBoundaryDimensions")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetDimensionsInternal(out Vector3 dimensionsOut, Boundary.Type boundaryType);

		[NativeName("BoundaryVisible")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern bool visible
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[NativeName("BoundaryConfigured")]
		public static extern bool configured
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static bool TryGetGeometry(List<Vector3> geometry)
		{
			return Boundary.TryGetGeometry(geometry, Boundary.Type.PlayArea);
		}

		public static bool TryGetGeometry(List<Vector3> geometry, [DefaultValue("Type.PlayArea")] Boundary.Type boundaryType)
		{
			bool flag = geometry == null;
			if (flag)
			{
				throw new ArgumentNullException("geometry");
			}
			geometry.Clear();
			return Boundary.TryGetGeometryScriptingInternal(geometry, boundaryType);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetGeometryScriptingInternal(List<Vector3> geometry, Boundary.Type boundaryType);

		public enum Type
		{
			PlayArea,
			TrackedArea
		}
	}
}
