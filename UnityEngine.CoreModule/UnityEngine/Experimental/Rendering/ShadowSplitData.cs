using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Describes the culling information for a given shadow split (e.g. directional cascade).</para>
	/// </summary>
	[UsedByNativeCode]
	public struct ShadowSplitData
	{
		/// <summary>
		///   <para>Gets a culling plane.</para>
		/// </summary>
		/// <param name="index">The culling plane index.</param>
		/// <returns>
		///   <para>The culling plane.</para>
		/// </returns>
		public unsafe Plane GetCullingPlane(int index)
		{
			if (index < 0 || index >= this.cullingPlaneCount || index >= 10)
			{
				throw new IndexOutOfRangeException("Invalid plane index");
			}
			fixed (float* ptr = &this._cullingPlanes.FixedElementField)
			{
				return new Plane(new Vector3(ptr[(IntPtr)(index * 4) * 4], ptr[(IntPtr)(index * 4 + 1) * 4], ptr[(IntPtr)(index * 4 + 2) * 4]), ptr[(IntPtr)(index * 4 + 3) * 4]);
			}
		}

		/// <summary>
		///   <para>Sets a culling plane.</para>
		/// </summary>
		/// <param name="index">The index of the culling plane to set.</param>
		/// <param name="plane">The culling plane.</param>
		public unsafe void SetCullingPlane(int index, Plane plane)
		{
			if (index < 0 || index >= this.cullingPlaneCount || index >= 10)
			{
				throw new IndexOutOfRangeException("Invalid plane index");
			}
			fixed (float* ptr = &this._cullingPlanes.FixedElementField)
			{
				ptr[(IntPtr)(index * 4) * 4] = plane.normal.x;
				ptr[(IntPtr)(index * 4 + 1) * 4] = plane.normal.y;
				ptr[(IntPtr)(index * 4 + 2) * 4] = plane.normal.z;
				ptr[(IntPtr)(index * 4 + 3) * 4] = plane.distance;
			}
		}

		/// <summary>
		///   <para>The number of culling planes.</para>
		/// </summary>
		public int cullingPlaneCount;

		private ShadowSplitData.<_cullingPlanes>__FixedBuffer7 _cullingPlanes;

		/// <summary>
		///   <para>The culling sphere.  The first three components of the vector describe the sphere center, and the last component specifies the radius.</para>
		/// </summary>
		public Vector4 cullingSphere;

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 160)]
		public struct <_cullingPlanes>__FixedBuffer7
		{
			public float FixedElementField;
		}
	}
}
