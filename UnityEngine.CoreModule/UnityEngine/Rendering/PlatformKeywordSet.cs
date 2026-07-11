using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>A collection of Rendering.ShaderKeyword that represents a specific platform variant.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct PlatformKeywordSet
	{
		private uint ComputeKeywordMask(BuiltinShaderDefine define)
		{
			return 1U << (int)(define % (BuiltinShaderDefine)32);
		}

		/// <summary>
		///   <para>Check whether a specific shader keyword is enabled.</para>
		/// </summary>
		/// <param name="define"></param>
		public bool IsEnabled(BuiltinShaderDefine define)
		{
			return (this.m_Bits & this.ComputeKeywordMask(define)) != 0U;
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
