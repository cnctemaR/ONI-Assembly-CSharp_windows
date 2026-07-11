using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Details of the Transform name mapped to the skeleton bone of a model and its default position and rotation in the T-pose.</para>
	/// </summary>
	[NativeHeader("Runtime/Animation/HumanDescription.h")]
	[RequiredByNativeCode]
	[NativeType(CodegenOptions.Custom, "MonoSkeletonBone")]
	public struct SkeletonBone
	{
		[Obsolete("transformModified is no longer used and has been deprecated.", true)]
		public int transformModified
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		/// <summary>
		///   <para>The name of the Transform mapped to the bone.</para>
		/// </summary>
		[NativeName("m_Name")]
		public string name;

		[NativeName("m_ParentName")]
		internal string parentName;

		/// <summary>
		///   <para>The T-pose position of the bone in local space.</para>
		/// </summary>
		[NativeName("m_Position")]
		public Vector3 position;

		/// <summary>
		///   <para>The T-pose rotation of the bone in local space.</para>
		/// </summary>
		[NativeName("m_Rotation")]
		public Quaternion rotation;

		/// <summary>
		///   <para>The T-pose scaling of the bone in local space.</para>
		/// </summary>
		[NativeName("m_Scale")]
		public Vector3 scale;
	}
}
