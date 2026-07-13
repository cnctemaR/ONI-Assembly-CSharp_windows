using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Math/Color.h")]
	[NativeClass("ColorRGBAf")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	public struct Color : IEquatable<Color>, IFormattable
	{
		public Color(float r, float g, float b, float a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public Color(float r, float g, float b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 1f;
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
				format = "F3";
			}
			if (formatProvider == null)
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

		public override readonly int GetHashCode()
		{
			return this.r.GetHashCode() ^ (this.g.GetHashCode() << 2) ^ (this.b.GetHashCode() >> 2) ^ (this.a.GetHashCode() >> 1);
		}

		public override readonly bool Equals(object other)
		{
			Color color;
			bool flag;
			if (other is Color)
			{
				color = (Color)other;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(in color);
		}

		public readonly bool Equals(Color other)
		{
			return this.r.Equals(other.r) && this.g.Equals(other.g) && this.b.Equals(other.b) && this.a.Equals(other.a);
		}

		public readonly bool Equals(in Color other)
		{
			return this.r.Equals(other.r) && this.g.Equals(other.g) && this.b.Equals(other.b) && this.a.Equals(other.a);
		}

		public static Color operator +(Color a, Color b)
		{
			return new Color
			{
				r = a.r + b.r,
				g = a.g + b.g,
				b = a.b + b.b,
				a = a.a + b.a
			};
		}

		public static Color operator -(Color a, Color b)
		{
			return new Color
			{
				r = a.r - b.r,
				g = a.g - b.g,
				b = a.b - b.b,
				a = a.a - b.a
			};
		}

		public static Color operator *(Color a, Color b)
		{
			return new Color
			{
				r = a.r * b.r,
				g = a.g * b.g,
				b = a.b * b.b,
				a = a.a * b.a
			};
		}

		public static Color operator *(Color a, Vector4 b)
		{
			return new Color
			{
				r = a.r * b.x,
				g = a.g * b.y,
				b = a.b * b.z,
				a = a.a * b.w
			};
		}

		public static Color operator *(Color a, float b)
		{
			return new Color
			{
				r = a.r * b,
				g = a.g * b,
				b = a.b * b,
				a = a.a * b
			};
		}

		public static Color operator *(float b, Color a)
		{
			return new Color
			{
				r = a.r * b,
				g = a.g * b,
				b = a.b * b,
				a = a.a * b
			};
		}

		public static Color operator /(Color a, float b)
		{
			return new Color
			{
				r = a.r / b,
				g = a.g / b,
				b = a.b / b,
				a = a.a / b
			};
		}

		public static bool operator ==(Color lhs, Color rhs)
		{
			float num = lhs.r - rhs.r;
			float num2 = lhs.g - rhs.g;
			float num3 = lhs.b - rhs.b;
			float num4 = lhs.a - rhs.a;
			float num5 = num * num + num2 * num2 + num3 * num3 + num4 * num4;
			return num5 < 9.9999994E-11f;
		}

		public static bool operator !=(Color lhs, Color rhs)
		{
			return !(lhs == rhs);
		}

		public static Color Lerp(Color a, Color b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Color
			{
				r = a.r + (b.r - a.r) * t,
				g = a.g + (b.g - a.g) * t,
				b = a.b + (b.b - a.b) * t,
				a = a.a + (b.a - a.a) * t
			};
		}

		public static Color Lerp(in Color a, in Color b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Color
			{
				r = a.r + (b.r - a.r) * t,
				g = a.g + (b.g - a.g) * t,
				b = a.b + (b.b - a.b) * t,
				a = a.a + (b.a - a.a) * t
			};
		}

		public static Color LerpUnclamped(Color a, Color b, float t)
		{
			return new Color
			{
				r = a.r + (b.r - a.r) * t,
				g = a.g + (b.g - a.g) * t,
				b = a.b + (b.b - a.b) * t,
				a = a.a + (b.a - a.a) * t
			};
		}

		public static Color LerpUnclamped(in Color a, in Color b, float t)
		{
			return new Color
			{
				r = a.r + (b.r - a.r) * t,
				g = a.g + (b.g - a.g) * t,
				b = a.b + (b.b - a.b) * t,
				a = a.a + (b.a - a.a) * t
			};
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal readonly Color RGBMultiplied(float multiplier)
		{
			return new Color
			{
				r = this.r * multiplier,
				g = this.g * multiplier,
				b = this.b * multiplier,
				a = this.a
			};
		}

		internal readonly Color AlphaMultiplied(float multiplier)
		{
			return new Color
			{
				r = this.r,
				g = this.g,
				b = this.b,
				a = this.a * multiplier
			};
		}

		internal readonly Color RGBMultiplied(Color multiplier)
		{
			return new Color
			{
				r = this.r * multiplier.r,
				g = this.g * multiplier.g,
				b = this.b * multiplier.b,
				a = this.a
			};
		}

		internal readonly Color RGBMultiplied(in Color multiplier)
		{
			return new Color
			{
				r = this.r * multiplier.r,
				g = this.g * multiplier.g,
				b = this.b * multiplier.b,
				a = this.a
			};
		}

		public readonly float grayscale
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return 0.299f * this.r + 0.587f * this.g + 0.114f * this.b;
			}
		}

		public readonly Color linear
		{
			get
			{
				return new Color
				{
					r = Mathf.GammaToLinearSpace(this.r),
					g = Mathf.GammaToLinearSpace(this.g),
					b = Mathf.GammaToLinearSpace(this.b),
					a = this.a
				};
			}
		}

		public readonly Color gamma
		{
			get
			{
				return new Color
				{
					r = Mathf.LinearToGammaSpace(this.r),
					g = Mathf.LinearToGammaSpace(this.g),
					b = Mathf.LinearToGammaSpace(this.b),
					a = this.a
				};
			}
		}

		public readonly float maxColorComponent
		{
			get
			{
				return Mathf.Max(Mathf.Max(this.r, this.g), this.b);
			}
		}

		public static implicit operator Vector4(Color c)
		{
			return new Vector4
			{
				x = c.r,
				y = c.g,
				z = c.b,
				w = c.a
			};
		}

		public static implicit operator Color(Vector4 v)
		{
			return new Color
			{
				r = v.x,
				g = v.y,
				b = v.z,
				a = v.w
			};
		}

		public float this[int index]
		{
			readonly get
			{
				float num;
				switch (index)
				{
				case 0:
					num = this.r;
					break;
				case 1:
					num = this.g;
					break;
				case 2:
					num = this.b;
					break;
				case 3:
					num = this.a;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Color index(" + index.ToString() + ")!");
				}
				return num;
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
					throw new IndexOutOfRangeException("Invalid Color index(" + index.ToString() + ")!");
				}
			}
		}

		public static void RGBToHSV(Color rgbColor, out float H, out float S, out float V)
		{
			bool flag = rgbColor.b > rgbColor.g && rgbColor.b > rgbColor.r;
			if (flag)
			{
				Color.RGBToHSVHelper(4f, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
			}
			else
			{
				bool flag2 = rgbColor.g > rgbColor.r;
				if (flag2)
				{
					Color.RGBToHSVHelper(2f, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
				}
				else
				{
					Color.RGBToHSVHelper(0f, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
				}
			}
		}

		public static void RGBToHSV(in Color rgbColor, out float H, out float S, out float V)
		{
			bool flag = rgbColor.b > rgbColor.g && rgbColor.b > rgbColor.r;
			if (flag)
			{
				Color.RGBToHSVHelper(4f, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
			}
			else
			{
				bool flag2 = rgbColor.g > rgbColor.r;
				if (flag2)
				{
					Color.RGBToHSVHelper(2f, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
				}
				else
				{
					Color.RGBToHSVHelper(0f, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
				}
			}
		}

		private static void RGBToHSVHelper(float offset, float dominantcolor, float colorone, float colortwo, out float H, out float S, out float V)
		{
			V = dominantcolor;
			bool flag = V != 0f;
			if (flag)
			{
				bool flag2 = colorone > colortwo;
				float num;
				if (flag2)
				{
					num = colortwo;
				}
				else
				{
					num = colorone;
				}
				float num2 = V - num;
				bool flag3 = num2 != 0f;
				if (flag3)
				{
					S = num2 / V;
					H = offset + (colorone - colortwo) / num2;
				}
				else
				{
					S = 0f;
					H = offset + (colorone - colortwo);
				}
				H /= 6f;
				bool flag4 = H < 0f;
				if (flag4)
				{
					H += 1f;
				}
			}
			else
			{
				S = 0f;
				H = 0f;
			}
		}

		public static Color HSVToRGB(float H, float S, float V)
		{
			return Color.HSVToRGB(H, S, V, true);
		}

		public static Color HSVToRGB(float H, float S, float V, bool hdr)
		{
			Color white = Color.white;
			bool flag = S == 0f;
			if (flag)
			{
				white.r = V;
				white.g = V;
				white.b = V;
			}
			else
			{
				bool flag2 = V == 0f;
				if (flag2)
				{
					white.r = 0f;
					white.g = 0f;
					white.b = 0f;
				}
				else
				{
					white.r = 0f;
					white.g = 0f;
					white.b = 0f;
					float num = H * 6f;
					int num2 = Mathf.FloorToInt(num);
					float num3 = num - (float)num2;
					float num4 = V * (1f - S);
					float num5 = V * (1f - S * num3);
					float num6 = V * (1f - S * (1f - num3));
					switch (num2)
					{
					case -1:
						white.r = V;
						white.g = num4;
						white.b = num5;
						break;
					case 0:
						white.r = V;
						white.g = num6;
						white.b = num4;
						break;
					case 1:
						white.r = num5;
						white.g = V;
						white.b = num4;
						break;
					case 2:
						white.r = num4;
						white.g = V;
						white.b = num6;
						break;
					case 3:
						white.r = num4;
						white.g = num5;
						white.b = V;
						break;
					case 4:
						white.r = num6;
						white.g = num4;
						white.b = V;
						break;
					case 5:
						white.r = V;
						white.g = num4;
						white.b = num5;
						break;
					case 6:
						white.r = V;
						white.g = num6;
						white.b = num4;
						break;
					}
					bool flag3 = !hdr;
					if (flag3)
					{
						white.r = Mathf.Clamp01(white.r);
						white.g = Mathf.Clamp01(white.g);
						white.b = Mathf.Clamp01(white.b);
					}
				}
			}
			return white;
		}

		public static Color aliceBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9411765f, 0.9725491f, 1f, 1f);
			}
		}

		public static Color antiqueWhite
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9803922f, 0.9215687f, 0.8431373f, 1f);
			}
		}

		public static Color aquamarine
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.4980392f, 1f, 0.8313726f, 1f);
			}
		}

		public static Color azure
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9411765f, 1f, 1f, 1f);
			}
		}

		public static Color beige
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9607844f, 0.9607844f, 0.8627452f, 1f);
			}
		}

		public static Color bisque
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.8941177f, 0.7686275f, 1f);
			}
		}

		public static Color black
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0f, 0f, 1f);
			}
		}

		public static Color blanchedAlmond
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.9215687f, 0.8039216f, 1f);
			}
		}

		public static Color blue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0f, 1f, 1f);
			}
		}

		public static Color blueViolet
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5411765f, 0.1686275f, 0.8862746f, 1f);
			}
		}

		public static Color brown
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.6470588f, 0.1647059f, 0.1647059f, 1f);
			}
		}

		public static Color burlywood
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8705883f, 0.7215686f, 0.5294118f, 1f);
			}
		}

		public static Color cadetBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.372549f, 0.6196079f, 0.627451f, 1f);
			}
		}

		public static Color chartreuse
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.4980392f, 1f, 0f, 1f);
			}
		}

		public static Color chocolate
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8235295f, 0.4117647f, 0.1176471f, 1f);
			}
		}

		public static Color clear
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0f, 0f, 0f);
			}
		}

		public static Color coral
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.4980392f, 0.3137255f, 1f);
			}
		}

		public static Color cornflowerBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.3921569f, 0.5843138f, 0.9294118f, 1f);
			}
		}

		public static Color cornsilk
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.9725491f, 0.8627452f, 1f);
			}
		}

		public static Color crimson
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8627452f, 0.07843138f, 0.2352941f, 1f);
			}
		}

		public static Color cyan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 1f, 1f, 1f);
			}
		}

		public static Color darkBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0f, 0.5450981f, 1f);
			}
		}

		public static Color darkCyan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0.5450981f, 0.5450981f, 1f);
			}
		}

		public static Color darkGoldenRod
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.7215686f, 0.5254902f, 0.04313726f, 1f);
			}
		}

		public static Color darkGray
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.6627451f, 0.6627451f, 0.6627451f, 1f);
			}
		}

		public static Color darkGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0.3921569f, 0f, 1f);
			}
		}

		public static Color darkKhaki
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.7411765f, 0.7176471f, 0.4196079f, 1f);
			}
		}

		public static Color darkMagenta
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5450981f, 0f, 0.5450981f, 1f);
			}
		}

		public static Color darkOliveGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.3333333f, 0.4196079f, 0.1843137f, 1f);
			}
		}

		public static Color darkOrange
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.5490196f, 0f, 1f);
			}
		}

		public static Color darkOrchid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.6f, 0.1960784f, 0.8000001f, 1f);
			}
		}

		public static Color darkRed
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5450981f, 0f, 0f, 1f);
			}
		}

		public static Color darkSalmon
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9137256f, 0.5882353f, 0.4784314f, 1f);
			}
		}

		public static Color darkSeaGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5607843f, 0.7372549f, 0.5607843f, 1f);
			}
		}

		public static Color darkSlateBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.282353f, 0.2392157f, 0.5450981f, 1f);
			}
		}

		public static Color darkSlateGray
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.1843137f, 0.3098039f, 0.3098039f, 1f);
			}
		}

		public static Color darkTurquoise
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0.8078432f, 0.8196079f, 1f);
			}
		}

		public static Color darkViolet
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5803922f, 0f, 0.8274511f, 1f);
			}
		}

		public static Color deepPink
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.07843138f, 0.5764706f, 1f);
			}
		}

		public static Color deepSkyBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0.7490196f, 1f, 1f);
			}
		}

		public static Color dimGray
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.4117647f, 0.4117647f, 0.4117647f, 1f);
			}
		}

		public static Color dodgerBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.1176471f, 0.5647059f, 1f, 1f);
			}
		}

		public static Color firebrick
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.6980392f, 0.1333333f, 0.1333333f, 1f);
			}
		}

		public static Color floralWhite
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.9803922f, 0.9411765f, 1f);
			}
		}

		public static Color forestGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.1333333f, 0.5450981f, 0.1333333f, 1f);
			}
		}

		public static Color gainsboro
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8627452f, 0.8627452f, 0.8627452f, 1f);
			}
		}

		public static Color ghostWhite
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9725491f, 0.9725491f, 1f, 1f);
			}
		}

		public static Color gold
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.8431373f, 0f, 1f);
			}
		}

		public static Color goldenRod
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.854902f, 0.6470588f, 0.1254902f, 1f);
			}
		}

		public static Color gray
		{
			get
			{
				return Color.gray5;
			}
		}

		public static Color grey
		{
			get
			{
				return Color.gray5;
			}
		}

		public static Color gray1
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.1f, 0.1f, 0.1f, 1f);
			}
		}

		public static Color gray2
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.2f, 0.2f, 0.2f, 1f);
			}
		}

		public static Color gray3
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.3f, 0.3f, 0.3f, 1f);
			}
		}

		public static Color gray4
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.4f, 0.4f, 0.4f, 1f);
			}
		}

		public static Color gray5
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5f, 0.5f, 0.5f, 1f);
			}
		}

		public static Color gray6
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.6f, 0.6f, 0.6f, 1f);
			}
		}

		public static Color gray7
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.7f, 0.7f, 0.7f, 1f);
			}
		}

		public static Color gray8
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8f, 0.8f, 0.8f, 1f);
			}
		}

		public static Color gray9
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9f, 0.9f, 0.9f, 1f);
			}
		}

		public static Color green
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 1f, 0f, 1f);
			}
		}

		public static Color greenYellow
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.6784314f, 1f, 0.1843137f, 1f);
			}
		}

		public static Color honeydew
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9411765f, 1f, 0.9411765f, 1f);
			}
		}

		public static Color hotPink
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.4117647f, 0.7058824f, 1f);
			}
		}

		public static Color indianRed
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8039216f, 0.3607843f, 0.3607843f, 1f);
			}
		}

		public static Color indigo
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.2941177f, 0f, 0.509804f, 1f);
			}
		}

		public static Color ivory
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 1f, 0.9411765f, 1f);
			}
		}

		public static Color khaki
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9411765f, 0.9019608f, 0.5490196f, 1f);
			}
		}

		public static Color lavender
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9019608f, 0.9019608f, 0.9803922f, 1f);
			}
		}

		public static Color lavenderBlush
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.9411765f, 0.9607844f, 1f);
			}
		}

		public static Color lawnGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.4862745f, 0.9882354f, 0f, 1f);
			}
		}

		public static Color lemonChiffon
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.9803922f, 0.8039216f, 1f);
			}
		}

		public static Color lightBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.6784314f, 0.8470589f, 0.9019608f, 1f);
			}
		}

		public static Color lightCoral
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9411765f, 0.5019608f, 0.5019608f, 1f);
			}
		}

		public static Color lightCyan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8784314f, 1f, 1f, 1f);
			}
		}

		public static Color lightGoldenRod
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9333334f, 0.8666667f, 0.509804f, 1f);
			}
		}

		public static Color lightGoldenRodYellow
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9803922f, 0.9803922f, 0.8235295f, 1f);
			}
		}

		public static Color lightGray
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8274511f, 0.8274511f, 0.8274511f, 1f);
			}
		}

		public static Color lightGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5647059f, 0.9333334f, 0.5647059f, 1f);
			}
		}

		public static Color lightPink
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.7137255f, 0.7568628f, 1f);
			}
		}

		public static Color lightSalmon
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.627451f, 0.4784314f, 1f);
			}
		}

		public static Color lightSeaGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.1254902f, 0.6980392f, 0.6666667f, 1f);
			}
		}

		public static Color lightSkyBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5294118f, 0.8078432f, 0.9803922f, 1f);
			}
		}

		public static Color lightSlateBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5176471f, 0.4392157f, 1f, 1f);
			}
		}

		public static Color lightSlateGray
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.4666667f, 0.5333334f, 0.6f, 1f);
			}
		}

		public static Color lightSteelBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.6901961f, 0.7686275f, 0.8705883f, 1f);
			}
		}

		public static Color lightYellow
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 1f, 0.8784314f, 1f);
			}
		}

		public static Color limeGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.1960784f, 0.8039216f, 0.1960784f, 1f);
			}
		}

		public static Color linen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9803922f, 0.9411765f, 0.9019608f, 1f);
			}
		}

		public static Color magenta
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0f, 1f, 1f);
			}
		}

		public static Color maroon
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.6901961f, 0.1882353f, 0.3764706f, 1f);
			}
		}

		public static Color mediumAquamarine
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.4f, 0.8039216f, 0.6666667f, 1f);
			}
		}

		public static Color mediumBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0f, 0.8039216f, 1f);
			}
		}

		public static Color mediumOrchid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.7294118f, 0.3333333f, 0.8274511f, 1f);
			}
		}

		public static Color mediumPurple
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5764706f, 0.4392157f, 0.8588236f, 1f);
			}
		}

		public static Color mediumSeaGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.2352941f, 0.7019608f, 0.4431373f, 1f);
			}
		}

		public static Color mediumSlateBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.482353f, 0.4078432f, 0.9333334f, 1f);
			}
		}

		public static Color mediumSpringGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0.9803922f, 0.6039216f, 1f);
			}
		}

		public static Color mediumTurquoise
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.282353f, 0.8196079f, 0.8000001f, 1f);
			}
		}

		public static Color mediumVioletRed
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.7803922f, 0.08235294f, 0.5215687f, 1f);
			}
		}

		public static Color midnightBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.09803922f, 0.09803922f, 0.4392157f, 1f);
			}
		}

		public static Color mintCream
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9607844f, 1f, 0.9803922f, 1f);
			}
		}

		public static Color mistyRose
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.8941177f, 0.882353f, 1f);
			}
		}

		public static Color moccasin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.8941177f, 0.7098039f, 1f);
			}
		}

		public static Color navajoWhite
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.8705883f, 0.6784314f, 1f);
			}
		}

		public static Color navyBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0f, 0.5019608f, 1f);
			}
		}

		public static Color oldLace
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9921569f, 0.9607844f, 0.9019608f, 1f);
			}
		}

		public static Color olive
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5019608f, 0.5019608f, 0f, 1f);
			}
		}

		public static Color oliveDrab
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.4196079f, 0.5568628f, 0.1372549f, 1f);
			}
		}

		public static Color orange
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.6470588f, 0f, 1f);
			}
		}

		public static Color orangeRed
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.2705882f, 0f, 1f);
			}
		}

		public static Color orchid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.854902f, 0.4392157f, 0.8392158f, 1f);
			}
		}

		public static Color paleGoldenRod
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9333334f, 0.909804f, 0.6666667f, 1f);
			}
		}

		public static Color paleGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5960785f, 0.9843138f, 0.5960785f, 1f);
			}
		}

		public static Color paleTurquoise
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.6862745f, 0.9333334f, 0.9333334f, 1f);
			}
		}

		public static Color paleVioletRed
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8588236f, 0.4392157f, 0.5764706f, 1f);
			}
		}

		public static Color papayaWhip
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.937255f, 0.8352942f, 1f);
			}
		}

		public static Color peachPuff
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.854902f, 0.7254902f, 1f);
			}
		}

		public static Color peru
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8039216f, 0.5215687f, 0.2470588f, 1f);
			}
		}

		public static Color pink
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.7529413f, 0.7960785f, 1f);
			}
		}

		public static Color plum
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8666667f, 0.627451f, 0.8666667f, 1f);
			}
		}

		public static Color powderBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.6901961f, 0.8784314f, 0.9019608f, 1f);
			}
		}

		public static Color purple
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.627451f, 0.1254902f, 0.9411765f, 1f);
			}
		}

		public static Color rebeccaPurple
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.4f, 0.2f, 0.6f, 1f);
			}
		}

		public static Color red
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0f, 0f, 1f);
			}
		}

		public static Color rosyBrown
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.7372549f, 0.5607843f, 0.5607843f, 1f);
			}
		}

		public static Color royalBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.254902f, 0.4117647f, 0.882353f, 1f);
			}
		}

		public static Color saddleBrown
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5450981f, 0.2705882f, 0.07450981f, 1f);
			}
		}

		public static Color salmon
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9803922f, 0.5019608f, 0.4470589f, 1f);
			}
		}

		public static Color sandyBrown
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9568628f, 0.6431373f, 0.3764706f, 1f);
			}
		}

		public static Color seaGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.1803922f, 0.5450981f, 0.3411765f, 1f);
			}
		}

		public static Color seashell
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.9607844f, 0.9333334f, 1f);
			}
		}

		public static Color sienna
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.627451f, 0.3215686f, 0.1764706f, 1f);
			}
		}

		public static Color silver
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.7529413f, 0.7529413f, 0.7529413f, 1f);
			}
		}

		public static Color skyBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5294118f, 0.8078432f, 0.9215687f, 1f);
			}
		}

		public static Color slateBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.4156863f, 0.3529412f, 0.8039216f, 1f);
			}
		}

		public static Color slateGray
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.4392157f, 0.5019608f, 0.5647059f, 1f);
			}
		}

		public static Color snow
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.9803922f, 0.9803922f, 1f);
			}
		}

		public static Color softRed
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8627452f, 0.1921569f, 0.1960784f, 1f);
			}
		}

		public static Color softBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.1882353f, 0.682353f, 0.7490196f, 1f);
			}
		}

		public static Color softGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5490196f, 0.7882354f, 0.1411765f, 1f);
			}
		}

		public static Color softYellow
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.9333334f, 0.5490196f, 1f);
			}
		}

		public static Color springGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 1f, 0.4980392f, 1f);
			}
		}

		public static Color steelBlue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.2745098f, 0.509804f, 0.7058824f, 1f);
			}
		}

		public static Color tan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8235295f, 0.7058824f, 0.5490196f, 1f);
			}
		}

		public static Color teal
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0.5019608f, 0.5019608f, 1f);
			}
		}

		public static Color thistle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8470589f, 0.7490196f, 0.8470589f, 1f);
			}
		}

		public static Color tomato
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.3882353f, 0.2784314f, 1f);
			}
		}

		public static Color turquoise
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.2509804f, 0.8784314f, 0.8156863f, 1f);
			}
		}

		public static Color violet
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9333334f, 0.509804f, 0.9333334f, 1f);
			}
		}

		public static Color violetRed
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.8156863f, 0.1254902f, 0.5647059f, 1f);
			}
		}

		public static Color wheat
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9607844f, 0.8705883f, 0.7019608f, 1f);
			}
		}

		public static Color white
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 1f, 1f, 1f);
			}
		}

		public static Color whiteSmoke
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.9607844f, 0.9607844f, 0.9607844f, 1f);
			}
		}

		public static Color yellow
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.92156863f, 0.015686275f, 1f);
			}
		}

		public static Color yellowGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.6039216f, 0.8039216f, 0.1960784f, 1f);
			}
		}

		public static Color yellowNice
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.92156863f, 0.015686275f, 1f);
			}
		}

		public float r;

		public float g;

		public float b;

		public float a;
	}
}
