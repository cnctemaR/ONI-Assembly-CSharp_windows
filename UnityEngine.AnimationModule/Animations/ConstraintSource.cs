using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[NativeHeader("Runtime/Animation/Constraints/Constraint.bindings.h")]
	[NativeType(CodegenOptions = CodegenOptions.Custom, Header = "Runtime/Animation/Constraints/ConstraintSource.h", IntermediateScriptingStructName = "MonoConstraintSource")]
	[UsedByNativeCode]
	[Serializable]
	public struct ConstraintSource
	{
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
