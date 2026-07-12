using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements
{
	internal struct TextureId
	{
		public TextureId(int index)
		{
			this.m_Index = index + 1;
		}

		public int index
		{
			get
			{
				return this.m_Index - 1;
			}
		}

		public bool IsValid()
		{
			return this.m_Index > 0;
		}

		public float ConvertToGpu()
		{
			return (float)this.index;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is TextureId);
			return !flag && (TextureId)obj == this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(TextureId other)
		{
			return this.m_Index == other.m_Index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return this.m_Index.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(TextureId left, TextureId right)
		{
			return left.m_Index == right.m_Index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(TextureId left, TextureId right)
		{
			return !(left == right);
		}

		private readonly int m_Index;

		public static readonly TextureId invalid = new TextureId(-1);
	}
}
