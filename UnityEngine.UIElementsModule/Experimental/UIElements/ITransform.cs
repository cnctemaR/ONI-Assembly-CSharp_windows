using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface ITransform
	{
		/// <summary>
		///   <para>The position of the VisualElement's transform.</para>
		/// </summary>
		Vector3 position { get; set; }

		/// <summary>
		///   <para>The rotation of the VisualElement's transform stored as a Quaternion.</para>
		/// </summary>
		Quaternion rotation { get; set; }

		/// <summary>
		///   <para>The scale of the VisualElement's transform.</para>
		/// </summary>
		Vector3 scale { get; set; }

		/// <summary>
		///   <para>Transformation matrix calculated from the position, rotation and scale of the transform (Read Only).</para>
		/// </summary>
		Matrix4x4 matrix { get; }
	}
}
