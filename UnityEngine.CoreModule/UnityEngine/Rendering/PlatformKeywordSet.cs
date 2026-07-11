using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	public struct PlatformKeywordSet
	{
		private uint ComputeKeywordMask(BuiltinShaderDefine define)
		{
			return 1U << (int)(define % (BuiltinShaderDefine)32);
		}

		public bool IsEnabled(BuiltinShaderDefine define)
		{
			return (this.m_Bits & this.ComputeKeywordMask(define)) > 0U;
		}

		public void Enable(BuiltinShaderDefine define)
		{
			this.m_Bits |= this.ComputeKeywordMask(define);
		}

		public void Disable(BuiltinShaderDefine define)
		{
			this.m_Bits &= ~this.ComputeKeywordMask(define);
		}

		private const int k_SizeInBits = 32;

		internal uint m_Bits;
	}
}
