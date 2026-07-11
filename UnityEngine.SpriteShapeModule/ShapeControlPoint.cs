using System;

namespace UnityEngine.Experimental.U2D
{
	/// <summary>
	///   <para>Data that describes the important points of the shape.</para>
	/// </summary>
	public struct ShapeControlPoint
	{
		/// <summary>
		///   <para>The position of this point in the object's local space.</para>
		/// </summary>
		public Vector3 position;

		/// <summary>
		///   <para>The position of the left tangent in local space.</para>
		/// </summary>
		public Vector3 leftTangent;

		/// <summary>
		///   <para>The position of the right tangent point in the local space.</para>
		/// </summary>
		public Vector3 rightTangent;

		/// <summary>
		///   <para>The various modes of the tangent handles. They could be continuous or broken.</para>
		/// </summary>
		public int mode;
	}
}
