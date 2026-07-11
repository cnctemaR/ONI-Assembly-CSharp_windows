using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	/// <summary>
	///   <para>Represents a source for the constraint.</para>
	/// </summary>
	[NativeHeader("Runtime/Animation/Constraints/Constraint.bindings.h")]
	[UsedByNativeCode]
	[NativeType(CodegenOptions = CodegenOptions.Custom, Header = "Runtime/Animation/Constraints/ConstraintSource.h", IntermediateScriptingStructName = "MonoConstraintSource")]
	[Serializable]
	public struct ConstraintSource
	{
		/// <summary>
		///   <para>The transform component of the source object.</para>
		/// </summary>
		public Transform sourceTransform
		{
			get
			{
				return this.m_SourceTransform;
			}
			set
			{
				this.m_SourceTransform = value;
			}
		}

		/// <summary>
		///   <para>The weight of the source in the evaluation of the constraint.</para>
		/// </summary>
		public float weight
		{
			get
			{
				return this.m_Weight;
			}
			set
			{
				this.m_Weight = value;
			}
		}

		[NativeName("sourceTransform")]
		private Transform m_SourceTransform;

		[NativeName("weight")]
		private float m_Weight;
	}
}
