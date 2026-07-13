using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeClass("Rectf", "template<typename T> class RectT; typedef RectT<float> Rectf;")]
	[NativeHeader("Runtime/Math/Rect.h")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	public struct Rect : IEquatable<Rect>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rect(float x, float y, float width, float height)
		{
			this.m_XMin = x;
			this.m_YMin = y;
			this.m_Width = width;
			this.m_Height = height;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rect(Vector2 position, Vector2 size)
		{
			this.m_XMin = position.x;
			this.m_YMin = position.y;
			this.m_Width = size.x;
			this.m_Height = size.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rect(in Vector2 position, in Vector2 size)
		{
			this.m_XMin = position.x;
			this.m_YMin = position.y;
			this.m_Width = size.x;
			this.m_Height = size.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rect(Rect source)
		{
			this.m_XMin = source.m_XMin;
			this.m_YMin = source.m_YMin;
			this.m_Width = source.m_Width;
			this.m_Height = source.m_Height;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rect(in Rect source)
		{
			this.m_XMin = source.m_XMin;
			this.m_YMin = source.m_YMin;
			this.m_Width = source.m_Width;
			this.m_Height = source.m_Height;
		}

		public static Rect zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Rect.kZero;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Rect MinMaxRect(float xmin, float ymin, float xmax, float ymax)
		{
			return new Rect
			{
				m_XMin = xmin,
				m_YMin = ymin,
				m_Width = xmax - xmin,
				m_Height = ymax - ymin
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(float x, float y, float width, float height)
		{
			this.m_XMin = x;
			this.m_YMin = y;
			this.m_Width = width;
			this.m_Height = height;
		}

		public float x
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_XMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_XMin = value;
			}
		}

		public float y
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_YMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_YMin = value;
			}
		}

		public Vector2 position
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector2
				{
					x = this.m_XMin,
					y = this.m_YMin
				};
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_XMin = value.x;
				this.m_YMin = value.y;
			}
		}

		public Vector2 center
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector2
				{
					x = this.m_XMin + this.m_Width * 0.5f,
					y = this.m_YMin + this.m_Height * 0.5f
				};
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_XMin = value.x - this.m_Width * 0.5f;
				this.m_YMin = value.y - this.m_Height * 0.5f;
			}
		}

		public Vector2 min
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector2
				{
					x = this.m_XMin,
					y = this.m_YMin
				};
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.xMin = value.x;
				this.yMin = value.y;
			}
		}

		public Vector2 max
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector2
				{
					x = this.xMax,
					y = this.yMax
				};
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.xMax = value.x;
				this.yMax = value.y;
			}
		}

		public float width
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Width;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Width = value;
			}
		}

		public float height
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Height;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Height = value;
			}
		}

		public Vector2 size
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector2
				{
					x = this.m_Width,
					y = this.m_Height
				};
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Width = value.x;
				this.m_Height = value.y;
			}
		}

		public float xMin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_XMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				float xMax = this.xMax;
				this.m_XMin = value;
				this.m_Width = xMax - this.m_XMin;
			}
		}

		public float yMin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_YMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				float yMax = this.yMax;
				this.m_YMin = value;
				this.m_Height = yMax - this.m_YMin;
			}
		}

		public float xMax
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Width + this.m_XMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Width = value - this.m_XMin;
			}
		}

		public float yMax
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Height + this.m_YMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Height = value - this.m_YMin;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(Vector2 point)
		{
			return point.x >= this.m_XMin && point.x < this.xMax && point.y >= this.m_YMin && point.y < this.yMax;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(in Vector2 point)
		{
			return point.x >= this.m_XMin && point.x < this.xMax && point.y >= this.m_YMin && point.y < this.yMax;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(Vector3 point)
		{
			return point.x >= this.m_XMin && point.x < this.xMax && point.y >= this.m_YMin && point.y < this.yMax;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(in Vector3 point)
		{
			return point.x >= this.m_XMin && point.x < this.xMax && point.y >= this.m_YMin && point.y < this.yMax;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(Vector3 point, bool allowInverse)
		{
			bool flag = !allowInverse;
			bool flag2;
			if (flag)
			{
				flag2 = this.Contains(in point);
			}
			else
			{
				float xMax = this.xMax;
				float yMax = this.yMax;
				bool flag3 = (this.m_Width < 0f && point.x <= this.m_XMin && point.x > xMax) || (this.m_Width >= 0f && point.x >= this.m_XMin && point.x < xMax);
				bool flag4 = (this.m_Height < 0f && point.y <= this.m_YMin && point.y > yMax) || (this.m_Height >= 0f && point.y >= this.m_YMin && point.y < yMax);
				flag2 = flag3 && flag4;
			}
			return flag2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(in Vector3 point, bool allowInverse)
		{
			bool flag = !allowInverse;
			bool flag2;
			if (flag)
			{
				flag2 = this.Contains(in point);
			}
			else
			{
				float xMax = this.xMax;
				float yMax = this.yMax;
				bool flag3 = (this.m_Width < 0f && point.x <= this.m_XMin && point.x > xMax) || (this.m_Width >= 0f && point.x >= this.m_XMin && point.x < xMax);
				bool flag4 = (this.m_Height < 0f && point.y <= this.m_YMin && point.y > yMax) || (this.m_Height >= 0f && point.y >= this.m_YMin && point.y < yMax);
				flag2 = flag3 && flag4;
			}
			return flag2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Rect OrderMinMax(Rect rect)
		{
			float xMax = rect.xMax;
			float yMax = rect.yMax;
			return Rect.MinMaxRect(Mathf.Min(rect.m_XMin, xMax), Mathf.Min(rect.m_YMin, yMax), Mathf.Max(rect.m_XMin, xMax), Mathf.Max(rect.m_YMin, yMax));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Rect OrderMinMax(in Rect rect)
		{
			float xMax = rect.xMax;
			float yMax = rect.yMax;
			return Rect.MinMaxRect(Mathf.Min(rect.m_XMin, xMax), Mathf.Min(rect.m_YMin, yMax), Mathf.Max(rect.m_XMin, xMax), Mathf.Max(rect.m_YMin, yMax));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Overlaps(Rect other)
		{
			return other.xMax > this.m_XMin && other.m_XMin < this.xMax && other.yMax > this.m_YMin && other.m_YMin < this.yMax;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Overlaps(in Rect other)
		{
			return other.xMax > this.m_XMin && other.m_XMin < this.xMax && other.yMax > this.m_YMin && other.m_YMin < this.yMax;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Overlaps(Rect other, bool allowInverse)
		{
			bool flag;
			if (allowInverse)
			{
				other = Rect.OrderMinMax(in other);
				flag = Rect.OrderMinMax(in this).Overlaps(in other);
			}
			else
			{
				flag = this.Overlaps(in other);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Overlaps(in Rect other, bool allowInverse)
		{
			bool flag;
			if (allowInverse)
			{
				Rect rect = Rect.OrderMinMax(in other);
				flag = Rect.OrderMinMax(in this).Overlaps(in rect);
			}
			else
			{
				flag = this.Overlaps(in other);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 NormalizedToPoint(Rect rectangle, Vector2 normalizedRectCoordinates)
		{
			return new Vector2
			{
				x = Mathf.Lerp(rectangle.m_XMin, rectangle.xMax, normalizedRectCoordinates.x),
				y = Mathf.Lerp(rectangle.m_YMin, rectangle.yMax, normalizedRectCoordinates.y)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 NormalizedToPoint(in Rect rectangle, in Vector2 normalizedRectCoordinates)
		{
			return new Vector2
			{
				x = Mathf.Lerp(rectangle.m_XMin, rectangle.xMax, normalizedRectCoordinates.x),
				y = Mathf.Lerp(rectangle.m_YMin, rectangle.yMax, normalizedRectCoordinates.y)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 PointToNormalized(Rect rectangle, Vector2 point)
		{
			return new Vector2
			{
				x = Mathf.InverseLerp(rectangle.m_XMin, rectangle.xMax, point.x),
				y = Mathf.InverseLerp(rectangle.m_YMin, rectangle.yMax, point.y)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 PointToNormalized(in Rect rectangle, in Vector2 point)
		{
			return new Vector2
			{
				x = Mathf.InverseLerp(rectangle.m_XMin, rectangle.xMax, point.x),
				y = Mathf.InverseLerp(rectangle.m_YMin, rectangle.yMax, point.y)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Rect lhs, Rect rhs)
		{
			return !(lhs == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Rect lhs, Rect rhs)
		{
			return lhs.m_XMin == rhs.m_XMin && lhs.m_YMin == rhs.m_YMin && lhs.m_Width == rhs.m_Width && lhs.m_Height == rhs.m_Height;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return this.m_XMin.GetHashCode() ^ (this.m_Width.GetHashCode() << 2) ^ (this.m_YMin.GetHashCode() >> 2) ^ (this.m_Height.GetHashCode() >> 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object other)
		{
			Rect rect;
			bool flag;
			if (other is Rect)
			{
				rect = (Rect)other;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(in rect);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Rect other)
		{
			return this.m_XMin.Equals(other.m_XMin) && this.m_YMin.Equals(other.m_YMin) && this.m_Width.Equals(other.m_Width) && this.m_Height.Equals(other.m_Height);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Rect other)
		{
			return this.m_XMin.Equals(other.m_XMin) && this.m_YMin.Equals(other.m_YMin) && this.m_Width.Equals(other.m_Width) && this.m_Height.Equals(other.m_Height);
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
			return string.Format("(x:{0}, y:{1}, width:{2}, height:{3})", new object[]
			{
				this.m_XMin.ToString(format, formatProvider),
				this.m_YMin.ToString(format, formatProvider),
				this.m_Width.ToString(format, formatProvider),
				this.m_Height.ToString(format, formatProvider)
			});
		}

		[Obsolete("use xMin")]
		public readonly float left
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_XMin;
			}
		}

		[Obsolete("use xMax")]
		public readonly float right
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_XMin + this.m_Width;
			}
		}

		[Obsolete("use yMin")]
		public readonly float top
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_YMin;
			}
		}

		[Obsolete("use yMax")]
		public readonly float bottom
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_YMin + this.m_Height;
			}
		}

		[NativeName("x")]
		private float m_XMin;

		[NativeName("y")]
		private float m_YMin;

		[NativeName("width")]
		private float m_Width;

		[NativeName("height")]
		private float m_Height;

		private static readonly Rect kZero = new Rect(0f, 0f, 0f, 0f);
	}
}
