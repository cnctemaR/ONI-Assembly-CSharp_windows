using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>A collection of Rendering.ShaderKeyword that represents a specific shader variant.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct ShaderKeywordSet
	{
		private void ComputeSliceAndMask(ShaderKeyword keyword, out uint slice, out uint mask)
		{
			int index = keyword.GetIndex();
			slice = (uint)(index / 32);
			mask = 1U << index % 32;
		}

		/// <summary>
		///   <para>Check whether a specific shader keyword is enabled.</para>
		/// </summary>
		/// <param name="keyword"></param>
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

		/// <summary>
		///   <para>Enable a specific shader keyword.</para>
		/// </summary>
		/// <param name="keyword"></param>
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

		/// <summary>
		///   <para>Disable a specific shader keyword.</para>
		/// </summary>
		/// <param name="keyword"></param>
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

		/// <summary>
		///   <para>Return an array with all the enabled keywords in the ShaderKeywordSet.</para>
		/// </summary>
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
		internal ShaderKeywordSet.<m_Bits>__FixedBuffer1 m_Bits;

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 32)]
		public struct <m_Bits>__FixedBuffer1
		{
			public uint FixedElementField;
		}
	}
}
