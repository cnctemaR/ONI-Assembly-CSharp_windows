using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Animation/HumanDescription.h")]
	[NativeType(CodegenOptions.Custom, "MonoHumanBone")]
	public struct HumanBone
	{
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

		[NativeName("m_Limit")]
		public HumanLimit limit;
	}
}
