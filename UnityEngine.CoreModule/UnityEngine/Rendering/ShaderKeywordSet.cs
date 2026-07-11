using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	public struct ShaderKeywordSet
	{
		private void ComputeSliceAndMask(ShaderKeyword keyword, out uint slice, out uint mask)
		{
			int keywordIndex = keyword.GetKeywordIndex();
			slice = (uint)(keywordIndex / 32);
			mask = 1U << keywordIndex % 32;
		}

		public unsafe bool IsEnabled(ShaderKeyword keyword)
		{
			bool flag;
			if (!keyword.IsValid())
			{
				flag = false;
			}
			else
			{
				uint num;
				uint num2;
				this.ComputeSliceAndMask(keyword, out num, out num2);
				fixed (uint* ptr = &this.m_Bits.FixedElementField)
				{
					flag = (ptr[(UIntPtr)num * 4] & num2) != 0U;
				}
			}
			return flag;
		}

		public unsafe void Enable(ShaderKeyword keyword)
		{
			if (keyword.IsValid())
			{
				uint num;
				uint num2;
				this.ComputeSliceAndMask(keyword, out num, out num2);
				fixed (uint* ptr = &this.m_Bits.FixedElementField)
				{
					ptr[(UIntPtr)num * 4] |= num2;
				}
			}
		}

		public unsafe void Disable(ShaderKeyword keyword)
		{
			if (keyword.IsValid())
			{
				uint num;
				uint num2;
				this.ComputeSliceAndMask(keyword, out num, out num2);
				fixed (uint* ptr = &this.m_Bits.FixedElementField)
				{
					ptr[(UIntPtr)num * 4] &= ~num2;
				}
			}
		}

		public ShaderKeyword[] GetShaderKeywords()
		{
			ShaderKeyword[] array = new ShaderKeyword[256];
			int num = 0;
			for (int i = 0; i < 256; i++)
			{
				ShaderKeyword shaderKeyword = new ShaderKeyword(i);
				if (this.IsEnabled(shaderKeyword))
				{
					array[num] = shaderKeyword;
					num++;
				}
			}
			Array.Resize<ShaderKeyword>(ref array, num);
			return array;
		}

		private const int k_SizeInBits = 32;

		[FixedBuffer(typeof(uint), 8)]
		internal ShaderKeywordSet.<m_Bits>__FixedBuffer0 m_Bits;

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 32)]
		public struct <m_Bits>__FixedBuffer0
		{
			public uint FixedElementField;
		}
	}
}
