using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>The mapping between a bone in the model and the conceptual bone in the Mecanim human anatomy.</para>
	/// </summary>
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Animation/HumanDescription.h")]
	[NativeType(CodegenOptions.Custom, "MonoHumanBone")]
	public struct HumanBone
	{
		/// <summary>
		///   <para>The name of the bone to which the Mecanim human bone is mapped.</para>
		/// </summary>
		public string boneName
		{
			get
			{
				return this.m_BoneName;
			}
			set
			{
				this.m_BoneName = value;
			}
		}

		/// <summary>
		///   <para>The name of the Mecanim human bone to which the bone from the model is mapped.</para>
		/// </summary>
		public string humanName
		{
			get
			{
				return this.m_HumanName;
			}
			set
			{
				this.m_HumanName = value;
			}
		}

		private string m_BoneName;

		private string m_HumanName;

		/// <summary>
		///   <para>The rotation limits that define the muscle for this bone.</para>
		/// </summary>
		[NativeName("m_Limit")]
		public HumanLimit limit;
	}
}
