using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public struct Ray2D : IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Ray2D(Vector2 origin, Vector2 direction)
		{
			this.m_Origin = origin;
			this.m_Direction = direction;
			this.m_Direction.Normalize();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Ray2D(in Vector2 origin, in Vector2 direction)
		{
			this.m_Origin = origin;
			this.m_Direction = direction;
			this.m_Direction.Normalize();
		}

		public Vector2 origin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Origin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Origin = value;
			}
		}

		public Vector2 direction
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Direction;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Direction = value;
				this.m_Direction.Normalize();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2 GetPoint(float distance)
		{
			return new Vector2
			{
				x = this.m_Origin.x + this.m_Direction.x * distance,
				y = this.m_Origin.y + this.m_Direction.y * distance
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly string ToString()
		{
			return this.ToString(null, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly string ToString(string format)
		{
			return this.ToString(format, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly string ToString(string format, IFormatProvider formatProvider)
		{
			bool flag = string.IsNullOrEmpty(format);
			if (flag)
			{
				format = "F2";
			}
			bool flag2 = formatProvider == null;
			if (flag2)
			{
				formatProvider = CultureInfo.InvariantCulture.NumberFormat;
			}
			return string.Format("Origin: {0}, Dir: {1}", this.m_Origin.ToString(format, formatProvider), this.m_Direction.ToString(format, formatProvider));
		}

		private Vector2 m_Origin;

		private Vector2 m_Direction;
	}
}
