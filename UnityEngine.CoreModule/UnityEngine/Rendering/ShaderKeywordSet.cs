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
			int index = keyword.index;
			slice = (uint)(index / 32);
			mask = 1U << index % 32;
		}

		public unsafe bool IsEnabled(ShaderKeyword keyword)
		{
			bool flag = !keyword.IsValid();
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				uint num;
				uint num2;
				this.ComputeSliceAndMask(keyword, out num, out num2);
				fixed (uint* ptr = &this.m_Bits.FixedElementField)
				{
					uint* ptr2 = ptr;
					flag2 = (ptr2[(ulong)num * 4UL / 4UL] & num2) > 0U;
				}
			}
			return flag2;
		}

		public unsafe void Enable(ShaderKeyword keyword)
		{
			bool flag = !keyword.IsValid();
			if (!flag)
			{
				uint num;
				uint num2;
				this.ComputeSliceAndMask(keyword, out num, out num2);
				fixed (uint* ptr = &this.m_Bits.FixedElementField)
				{
					uint* ptr2 = ptr;
					ptr2[(ulong)num * 4UL / 4UL] |= num2;
				}
			}
		}

		public unsafe void Disable(ShaderKeyword keyword)
		{
			bool flag = !keyword.IsValid();
			if (!flag)
			{
				uint num;
				uint num2;
				this.ComputeSliceAndMask(keyword, out num, out num2);
				fixed (uint* ptr = &this.m_Bits.FixedElementField)
				{
					uint* ptr2 = ptr;
					ptr2[(ulong)num * 4UL / 4UL] &= ~num2;
				}
			}
		}

		public ShaderKeyword[] GetShaderKeywords()
		{
			ShaderKeyword[] array = new ShaderKeyword[448];
			int num = 0;
			for (int i = 0; i < 448; i++)
			{
				ShaderKeyword shaderKeyword = new ShaderKeyword(i);
				bool flag = this.IsEnabled(shaderKeyword);
				if (flag)
				{
					array[num] = shaderKeyword;
					num++;
				}
			}
			Array.Resize<ShaderKeyword>(ref array, num);
			return array;
		}

		private const int k_SizeInBits = 32;

		[FixedBuffer(typeof(uint), 14)]
		internal ShaderKeywordSet.<m_Bits>e__FixedBuffer m_Bits;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 56)]
		public struct <m_Bits>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
