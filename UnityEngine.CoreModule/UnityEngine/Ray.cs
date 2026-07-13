using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public struct Ray : IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Ray(Vector3 origin, Vector3 direction)
		{
			this.m_Origin = origin;
			this.m_Direction = direction;
			this.m_Direction.Normalize();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Ray(in Vector3 origin, in Vector3 direction)
		{
			this.m_Origin = origin;
			this.m_Direction = direction;
			this.m_Direction.Normalize();
		}

		public Vector3 origin
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

		public Vector3 direction
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
		public readonly Vector3 GetPoint(float distance)
		{
			return new Vector3
			{
				x = this.m_Origin.x + this.m_Direction.x * distance,
				y = this.m_Origin.y + this.m_Direction.y * distance,
				z = this.m_Origin.z + this.m_Direction.z * distance
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

		private Vector3 m_Origin;

		private Vector3 m_Direction;
	}
}
