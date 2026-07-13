using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Explicit)]
	public struct Color32 : IEquatable<Color32>, IFormattable
	{
		public Color32(byte r, byte g, byte b, byte a)
		{
			this.rgba = 0;
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public static implicit operator Color32(Color c)
		{
			return new Color32
			{
				r = (byte)Mathf.Round(Mathf.Clamp01(c.r) * 255f),
				g = (byte)Mathf.Round(Mathf.Clamp01(c.g) * 255f),
				b = (byte)Mathf.Round(Mathf.Clamp01(c.b) * 255f),
				a = (byte)Mathf.Round(Mathf.Clamp01(c.a) * 255f)
			};
		}

		public static implicit operator Color(Color32 c)
		{
			return new Color
			{
				r = (float)c.r / 255f,
				g = (float)c.g / 255f,
				b = (float)c.b / 255f,
				a = (float)c.a / 255f
			};
		}

		public static Color32 Lerp(Color32 a, Color32 b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Color32
			{
				r = (byte)((float)a.r + (float)(b.r - a.r) * t),
				g = (byte)((float)a.g + (float)(b.g - a.g) * t),
				b = (byte)((float)a.b + (float)(b.b - a.b) * t),
				a = (byte)((float)a.a + (float)(b.a - a.a) * t)
			};
		}

		public static Color32 Lerp(in Color32 a, in Color32 b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Color32
			{
				r = (byte)((float)a.r + (float)(b.r - a.r) * t),
				g = (byte)((float)a.g + (float)(b.g - a.g) * t),
				b = (byte)((float)a.b + (float)(b.b - a.b) * t),
				a = (byte)((float)a.a + (float)(b.a - a.a) * t)
			};
		}

		public static Color32 LerpUnclamped(Color32 a, Color32 b, float t)
		{
			return new Color32
			{
				r = (byte)((float)a.r + (float)(b.r - a.r) * t),
				g = (byte)((float)a.g + (float)(b.g - a.g) * t),
				b = (byte)((float)a.b + (float)(b.b - a.b) * t),
				a = (byte)((float)a.a + (float)(b.a - a.a) * t)
			};
		}

		public static Color32 LerpUnclamped(in Color32 a, in Color32 b, float t)
		{
			return new Color32
			{
				r = (byte)((float)a.r + (float)(b.r - a.r) * t),
				g = (byte)((float)a.g + (float)(b.g - a.g) * t),
				b = (byte)((float)a.b + (float)(b.b - a.b) * t),
				a = (byte)((float)a.a + (float)(b.a - a.a) * t)
			};
		}

		public byte this[int index]
		{
			readonly get
			{
				byte b;
				switch (index)
				{
				case 0:
					b = this.r;
					break;
				case 1:
					b = this.g;
					break;
				case 2:
					b = this.b;
					break;
				case 3:
					b = this.a;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Color32 index(" + index.ToString() + ")!");
				}
				return b;
			}
			set
			{
				switch (index)
				{
				case 0:
					this.r = value;
					break;
				case 1:
					this.g = value;
					break;
				case 2:
					this.b = value;
					break;
				case 3:
					this.a = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Color32 index(" + index.ToString() + ")!");
				}
			}
		}

		public override readonly int GetHashCode()
		{
			return this.rgba.GetHashCode();
		}

		public override readonly bool Equals(object other)
		{
			Color32 color;
			bool flag;
			if (other is Color32)
			{
				color = (Color32)other;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(in color);
		}

		public readonly bool Equals(Color32 other)
		{
			return this.rgba == other.rgba;
		}

		public readonly bool Equals(in Color32 other)
		{
			return this.rgba == other.rgba;
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
			bool flag = formatProvider == null;
			if (flag)
			{
				formatProvider = CultureInfo.InvariantCulture.NumberFormat;
			}
			return string.Format("RGBA({0}, {1}, {2}, {3})", new object[]
			{
				this.r.ToString(format, formatProvider),
				this.g.ToString(format, formatProvider),
				this.b.ToString(format, formatProvider),
				this.a.ToString(format, formatProvider)
			});
		}

		[Ignore(DoesNotContributeToSize = true)]
		[FieldOffset(0)]
		private int rgba;

		[FieldOffset(0)]
		public byte r;

		[FieldOffset(1)]
		public byte g;

		[FieldOffset(2)]
		public byte b;

		[FieldOffset(3)]
		public byte a;
	}
}
