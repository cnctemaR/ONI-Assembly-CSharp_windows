using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Color : IEquatable<Color>, IColor
	{
		public byte Red
		{
			get
			{
				return this._red;
			}
			set
			{
				this._red = value;
			}
		}

		public byte Green
		{
			get
			{
				return this._green;
			}
			set
			{
				this._green = value;
			}
		}

		public byte Blue
		{
			get
			{
				return this._blue;
			}
			set
			{
				this._blue = value;
			}
		}

		public byte Alpha
		{
			get
			{
				return this._alpha;
			}
			set
			{
				this._alpha = value;
			}
		}

		public static Color BLACK
		{
			get
			{
				return new Color(0, 0, 0, byte.MaxValue);
			}
		}

		public static Color WHITE
		{
			get
			{
				return new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
		}

		public static Color RED
		{
			get
			{
				return new Color(byte.MaxValue, 0, 0, byte.MaxValue);
			}
		}

		public static Color GREEN
		{
			get
			{
				return new Color(0, byte.MaxValue, 0, byte.MaxValue);
			}
		}

		public static Color BLUE
		{
			get
			{
				return new Color(0, 0, byte.MaxValue, byte.MaxValue);
			}
		}

		public static Color TRANSPARENT
		{
			get
			{
				return new Color(0, 0, 0, 0);
			}
		}

		public Color()
		{
			this._hashcode = (int)(this._red + this._green + this._blue) ^ Color._rnd.Next();
		}

		public Color(byte r, byte g, byte b, byte a)
			: this()
		{
			this._red = r;
			this._green = g;
			this._blue = b;
			this._alpha = a;
		}

		public Color(byte r, byte g, byte b)
			: this()
		{
			this._red = r;
			this._green = g;
			this._blue = b;
			this._alpha = byte.MaxValue;
		}

		public bool Equals(Color other)
		{
			return this._red == other.Red && this._green == other.Green && this._blue == other.Blue && this._alpha == other.Alpha;
		}

		public static IColor Lerp(IColor color0, IColor color1, float t, bool withAlphaChannel)
		{
			IColor color2 = (IColor)Activator.CreateInstance(color0.GetType());
			color2.Red = Libnoise.Lerp(color0.Red, color1.Red, t);
			color2.Green = Libnoise.Lerp(color0.Green, color1.Green, t);
			color2.Blue = Libnoise.Lerp(color0.Blue, color1.Blue, t);
			color2.Alpha = (withAlphaChannel ? Libnoise.Lerp(color0.Alpha, color1.Alpha, t) : byte.MaxValue);
			return color2;
		}

		public static IColor Lerp(IColor color0, IColor color1, float t)
		{
			return Color.Lerp(color0, color1, t, true);
		}

		public static IColor Lerp32(IColor color0, IColor color1, float t)
		{
			return Color.Lerp(color0, color1, t, true);
		}

		public static IColor Lerp24(IColor color0, IColor color1, float t)
		{
			return Color.Lerp(color0, color1, t, false);
		}

		public static IColor Grayscale(IColor color)
		{
			IColor color2 = (IColor)Activator.CreateInstance(color.GetType());
			color2.Red = (color2.Green = (color2.Blue = Color.GrayscaleLuminosityStrategy(color)));
			color2.Alpha = byte.MaxValue;
			return color2;
		}

		public static IColor Grayscale(IColor color, Color.GrayscaleStrategy Strategy)
		{
			IColor color2 = (IColor)Activator.CreateInstance(color.GetType());
			byte b = Strategy(color);
			color2.Red = (color2.Green = (color2.Blue = b));
			color2.Alpha = byte.MaxValue;
			return color2;
		}

		public static byte GrayscaleLightnessStrategy(IColor color)
		{
			return (Math.Max(color.Red, Math.Max(color.Green, color.Blue)) + Math.Min(color.Red, Math.Max(color.Green, color.Blue))) / 2;
		}

		public static byte GrayscaleAverageStrategy(IColor color)
		{
			return (color.Red + color.Green + color.Blue) / 3;
		}

		public static byte GrayscaleLuminosityStrategy(IColor color)
		{
			return (byte)(0.21f * (float)color.Red + 0.71f * (float)color.Green + 0.07f * (float)color.Blue);
		}

		public override string ToString()
		{
			return string.Format("Color({0},{1},{2},{3})", new object[] { this.Red, this.Green, this.Blue, this.Alpha });
		}

		public override bool Equals(object other)
		{
			return other is IColor && (this._red == ((IColor)other).Red && this._green == ((IColor)other).Green && this._blue == ((IColor)other).Blue) && this._alpha == ((IColor)other).Alpha;
		}

		public override int GetHashCode()
		{
			return this._hashcode;
		}

		public static bool operator ==(Color a, IColor b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Color a, IColor b)
		{
			return !a.Equals(b);
		}

		public static bool operator >(Color a, IColor b)
		{
			return a._red > b.Red && a._green > b.Green && a._blue > b.Blue && a._alpha > b.Alpha;
		}

		public static bool operator <(Color a, IColor b)
		{
			return a._red < b.Red && a._green < b.Green && a._blue < b.Blue && a._alpha < b.Alpha;
		}

		public static bool operator >=(Color a, IColor b)
		{
			return a > b || a == b;
		}

		public static bool operator <=(Color a, IColor b)
		{
			return a < b || a == b;
		}

		protected byte _red;

		protected byte _green;

		protected byte _blue;

		protected byte _alpha = byte.MaxValue;

		protected int _hashcode;

		private static Random _rnd = new Random(666);

		public delegate byte GrayscaleStrategy(IColor color);
	}
}
