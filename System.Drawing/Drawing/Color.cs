using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace System.Drawing
{
	[TypeConverter(typeof(ColorConverter))]
	[Editor("System.Drawing.Design.ColorEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[Serializable]
	public struct Color
	{
		public string Name
		{
			get
			{
				if (this.name == null)
				{
					if (this.IsNamedColor)
					{
						this.name = KnownColors.GetName(this.knownColor);
					}
					else
					{
						this.name = string.Format("{0:x}", this.ToArgb());
					}
				}
				return this.name;
			}
		}

		public bool IsKnownColor
		{
			get
			{
				return (this.state & 1) != 0;
			}
		}

		public bool IsSystemColor
		{
			get
			{
				return (this.state & 8) != 0;
			}
		}

		public bool IsNamedColor
		{
			get
			{
				return (this.state & 5) != 0;
			}
		}

		internal long Value
		{
			get
			{
				if (this.value == 0L && this.IsKnownColor)
				{
					this.value = (long)KnownColors.FromKnownColor((KnownColor)this.knownColor).ToArgb() & (long)((ulong)(-1));
				}
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		public static Color FromArgb(int red, int green, int blue)
		{
			return Color.FromArgb(255, red, green, blue);
		}

		public static Color FromArgb(int alpha, int red, int green, int blue)
		{
			Color.CheckARGBValues(alpha, red, green, blue);
			return new Color
			{
				state = 2,
				Value = (long)((alpha << 24) + (red << 16) + (green << 8) + blue)
			};
		}

		public int ToArgb()
		{
			return (int)this.Value;
		}

		public static Color FromArgb(int alpha, Color baseColor)
		{
			return Color.FromArgb(alpha, (int)baseColor.R, (int)baseColor.G, (int)baseColor.B);
		}

		public static Color FromArgb(int argb)
		{
			return Color.FromArgb((argb >> 24) & 255, (argb >> 16) & 255, (argb >> 8) & 255, argb & 255);
		}

		public static Color FromKnownColor(KnownColor color)
		{
			return KnownColors.FromKnownColor(color);
		}

		public static Color FromName(string name)
		{
			Color color;
			try
			{
				KnownColor knownColor = (KnownColor)((int)Enum.Parse(typeof(KnownColor), name, true));
				color = KnownColors.FromKnownColor(knownColor);
			}
			catch
			{
				Color color2 = Color.FromArgb(0, 0, 0, 0);
				color2.name = name;
				color2.state |= 4;
				color = color2;
			}
			return color;
		}

		public float GetBrightness()
		{
			byte b = Math.Min(this.R, Math.Min(this.G, this.B));
			byte b2 = Math.Max(this.R, Math.Max(this.G, this.B));
			return (float)(b2 + b) / 510f;
		}

		public float GetSaturation()
		{
			byte b = Math.Min(this.R, Math.Min(this.G, this.B));
			byte b2 = Math.Max(this.R, Math.Max(this.G, this.B));
			if (b2 == b)
			{
				return 0f;
			}
			int num = (int)(b2 + b);
			if (num > 255)
			{
				num = 510 - num;
			}
			return (float)(b2 - b) / (float)num;
		}

		public float GetHue()
		{
			int r = (int)this.R;
			int g = (int)this.G;
			int b = (int)this.B;
			byte b2 = (byte)Math.Min(r, Math.Min(g, b));
			byte b3 = (byte)Math.Max(r, Math.Max(g, b));
			if (b3 == b2)
			{
				return 0f;
			}
			float num = (float)(b3 - b2);
			float num2 = (float)((int)b3 - r) / num;
			float num3 = (float)((int)b3 - g) / num;
			float num4 = (float)((int)b3 - b) / num;
			float num5 = 0f;
			if (r == (int)b3)
			{
				num5 = 60f * (6f + num4 - num3);
			}
			if (g == (int)b3)
			{
				num5 = 60f * (2f + num2 - num4);
			}
			if (b == (int)b3)
			{
				num5 = 60f * (4f + num3 - num2);
			}
			if (num5 > 360f)
			{
				num5 -= 360f;
			}
			return num5;
		}

		public KnownColor ToKnownColor()
		{
			return (KnownColor)this.knownColor;
		}

		public bool IsEmpty
		{
			get
			{
				return this.state == 0;
			}
		}

		public byte A
		{
			get
			{
				return (byte)(this.Value >> 24);
			}
		}

		public byte R
		{
			get
			{
				return (byte)(this.Value >> 16);
			}
		}

		public byte G
		{
			get
			{
				return (byte)(this.Value >> 8);
			}
		}

		public byte B
		{
			get
			{
				return (byte)this.Value;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Color))
			{
				return false;
			}
			Color color = (Color)obj;
			return this == color;
		}

		public override int GetHashCode()
		{
			int num = (int)(this.Value ^ (this.Value >> 32) ^ (long)this.state ^ (long)(this.knownColor >> 16));
			if (this.IsNamedColor)
			{
				num ^= this.Name.GetHashCode();
			}
			return num;
		}

		public override string ToString()
		{
			if (this.IsEmpty)
			{
				return "Color [Empty]";
			}
			if (this.IsNamedColor)
			{
				return "Color [" + this.Name + "]";
			}
			return string.Format("Color [A={0}, R={1}, G={2}, B={3}]", new object[] { this.A, this.R, this.G, this.B });
		}

		private static void CheckRGBValues(int red, int green, int blue)
		{
			if (red > 255 || red < 0)
			{
				throw Color.CreateColorArgumentException(red, "red");
			}
			if (green > 255 || green < 0)
			{
				throw Color.CreateColorArgumentException(green, "green");
			}
			if (blue > 255 || blue < 0)
			{
				throw Color.CreateColorArgumentException(blue, "blue");
			}
		}

		private static ArgumentException CreateColorArgumentException(int value, string color)
		{
			return new ArgumentException(string.Format("'{0}' is not a valid value for '{1}'. '{1}' should be greater or equal to 0 and less than or equal to 255.", value, color));
		}

		private static void CheckARGBValues(int alpha, int red, int green, int blue)
		{
			if (alpha > 255 || alpha < 0)
			{
				throw Color.CreateColorArgumentException(alpha, "alpha");
			}
			Color.CheckRGBValues(red, green, blue);
		}

		public static Color Transparent
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Transparent);
			}
		}

		public static Color AliceBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.AliceBlue);
			}
		}

		public static Color AntiqueWhite
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.AntiqueWhite);
			}
		}

		public static Color Aqua
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Aqua);
			}
		}

		public static Color Aquamarine
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Aquamarine);
			}
		}

		public static Color Azure
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Azure);
			}
		}

		public static Color Beige
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Beige);
			}
		}

		public static Color Bisque
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Bisque);
			}
		}

		public static Color Black
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Black);
			}
		}

		public static Color BlanchedAlmond
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.BlanchedAlmond);
			}
		}

		public static Color Blue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Blue);
			}
		}

		public static Color BlueViolet
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.BlueViolet);
			}
		}

		public static Color Brown
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Brown);
			}
		}

		public static Color BurlyWood
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.BurlyWood);
			}
		}

		public static Color CadetBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.CadetBlue);
			}
		}

		public static Color Chartreuse
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Chartreuse);
			}
		}

		public static Color Chocolate
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Chocolate);
			}
		}

		public static Color Coral
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Coral);
			}
		}

		public static Color CornflowerBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.CornflowerBlue);
			}
		}

		public static Color Cornsilk
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Cornsilk);
			}
		}

		public static Color Crimson
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Crimson);
			}
		}

		public static Color Cyan
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Cyan);
			}
		}

		public static Color DarkBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkBlue);
			}
		}

		public static Color DarkCyan
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkCyan);
			}
		}

		public static Color DarkGoldenrod
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkGoldenrod);
			}
		}

		public static Color DarkGray
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkGray);
			}
		}

		public static Color DarkGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkGreen);
			}
		}

		public static Color DarkKhaki
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkKhaki);
			}
		}

		public static Color DarkMagenta
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkMagenta);
			}
		}

		public static Color DarkOliveGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkOliveGreen);
			}
		}

		public static Color DarkOrange
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkOrange);
			}
		}

		public static Color DarkOrchid
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkOrchid);
			}
		}

		public static Color DarkRed
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkRed);
			}
		}

		public static Color DarkSalmon
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkSalmon);
			}
		}

		public static Color DarkSeaGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkSeaGreen);
			}
		}

		public static Color DarkSlateBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkSlateBlue);
			}
		}

		public static Color DarkSlateGray
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkSlateGray);
			}
		}

		public static Color DarkTurquoise
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkTurquoise);
			}
		}

		public static Color DarkViolet
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DarkViolet);
			}
		}

		public static Color DeepPink
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DeepPink);
			}
		}

		public static Color DeepSkyBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
			}
		}

		public static Color DimGray
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DimGray);
			}
		}

		public static Color DodgerBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.DodgerBlue);
			}
		}

		public static Color Firebrick
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Firebrick);
			}
		}

		public static Color FloralWhite
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.FloralWhite);
			}
		}

		public static Color ForestGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.ForestGreen);
			}
		}

		public static Color Fuchsia
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Fuchsia);
			}
		}

		public static Color Gainsboro
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Gainsboro);
			}
		}

		public static Color GhostWhite
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.GhostWhite);
			}
		}

		public static Color Gold
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Gold);
			}
		}

		public static Color Goldenrod
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Goldenrod);
			}
		}

		public static Color Gray
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Gray);
			}
		}

		public static Color Green
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Green);
			}
		}

		public static Color GreenYellow
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.GreenYellow);
			}
		}

		public static Color Honeydew
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Honeydew);
			}
		}

		public static Color HotPink
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.HotPink);
			}
		}

		public static Color IndianRed
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.IndianRed);
			}
		}

		public static Color Indigo
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Indigo);
			}
		}

		public static Color Ivory
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Ivory);
			}
		}

		public static Color Khaki
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Khaki);
			}
		}

		public static Color Lavender
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Lavender);
			}
		}

		public static Color LavenderBlush
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LavenderBlush);
			}
		}

		public static Color LawnGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LawnGreen);
			}
		}

		public static Color LemonChiffon
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LemonChiffon);
			}
		}

		public static Color LightBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightBlue);
			}
		}

		public static Color LightCoral
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightCoral);
			}
		}

		public static Color LightCyan
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightCyan);
			}
		}

		public static Color LightGoldenrodYellow
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightGoldenrodYellow);
			}
		}

		public static Color LightGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightGreen);
			}
		}

		public static Color LightGray
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightGray);
			}
		}

		public static Color LightPink
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightPink);
			}
		}

		public static Color LightSalmon
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightSalmon);
			}
		}

		public static Color LightSeaGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightSeaGreen);
			}
		}

		public static Color LightSkyBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightSkyBlue);
			}
		}

		public static Color LightSlateGray
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightSlateGray);
			}
		}

		public static Color LightSteelBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightSteelBlue);
			}
		}

		public static Color LightYellow
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LightYellow);
			}
		}

		public static Color Lime
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Lime);
			}
		}

		public static Color LimeGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.LimeGreen);
			}
		}

		public static Color Linen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Linen);
			}
		}

		public static Color Magenta
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Magenta);
			}
		}

		public static Color Maroon
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Maroon);
			}
		}

		public static Color MediumAquamarine
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.MediumAquamarine);
			}
		}

		public static Color MediumBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.MediumBlue);
			}
		}

		public static Color MediumOrchid
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.MediumOrchid);
			}
		}

		public static Color MediumPurple
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.MediumPurple);
			}
		}

		public static Color MediumSeaGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.MediumSeaGreen);
			}
		}

		public static Color MediumSlateBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.MediumSlateBlue);
			}
		}

		public static Color MediumSpringGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.MediumSpringGreen);
			}
		}

		public static Color MediumTurquoise
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.MediumTurquoise);
			}
		}

		public static Color MediumVioletRed
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.MediumVioletRed);
			}
		}

		public static Color MidnightBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.MidnightBlue);
			}
		}

		public static Color MintCream
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.MintCream);
			}
		}

		public static Color MistyRose
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.MistyRose);
			}
		}

		public static Color Moccasin
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Moccasin);
			}
		}

		public static Color NavajoWhite
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.NavajoWhite);
			}
		}

		public static Color Navy
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Navy);
			}
		}

		public static Color OldLace
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.OldLace);
			}
		}

		public static Color Olive
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Olive);
			}
		}

		public static Color OliveDrab
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.OliveDrab);
			}
		}

		public static Color Orange
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Orange);
			}
		}

		public static Color OrangeRed
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.OrangeRed);
			}
		}

		public static Color Orchid
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Orchid);
			}
		}

		public static Color PaleGoldenrod
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.PaleGoldenrod);
			}
		}

		public static Color PaleGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.PaleGreen);
			}
		}

		public static Color PaleTurquoise
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.PaleTurquoise);
			}
		}

		public static Color PaleVioletRed
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.PaleVioletRed);
			}
		}

		public static Color PapayaWhip
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.PapayaWhip);
			}
		}

		public static Color PeachPuff
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.PeachPuff);
			}
		}

		public static Color Peru
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Peru);
			}
		}

		public static Color Pink
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Pink);
			}
		}

		public static Color Plum
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Plum);
			}
		}

		public static Color PowderBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.PowderBlue);
			}
		}

		public static Color Purple
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Purple);
			}
		}

		public static Color Red
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Red);
			}
		}

		public static Color RosyBrown
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.RosyBrown);
			}
		}

		public static Color RoyalBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.RoyalBlue);
			}
		}

		public static Color SaddleBrown
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.SaddleBrown);
			}
		}

		public static Color Salmon
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Salmon);
			}
		}

		public static Color SandyBrown
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.SandyBrown);
			}
		}

		public static Color SeaGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.SeaGreen);
			}
		}

		public static Color SeaShell
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.SeaShell);
			}
		}

		public static Color Sienna
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Sienna);
			}
		}

		public static Color Silver
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Silver);
			}
		}

		public static Color SkyBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.SkyBlue);
			}
		}

		public static Color SlateBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.SlateBlue);
			}
		}

		public static Color SlateGray
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.SlateGray);
			}
		}

		public static Color Snow
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Snow);
			}
		}

		public static Color SpringGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.SpringGreen);
			}
		}

		public static Color SteelBlue
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.SteelBlue);
			}
		}

		public static Color Tan
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Tan);
			}
		}

		public static Color Teal
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Teal);
			}
		}

		public static Color Thistle
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Thistle);
			}
		}

		public static Color Tomato
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Tomato);
			}
		}

		public static Color Turquoise
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Turquoise);
			}
		}

		public static Color Violet
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Violet);
			}
		}

		public static Color Wheat
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Wheat);
			}
		}

		public static Color White
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.White);
			}
		}

		public static Color WhiteSmoke
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.WhiteSmoke);
			}
		}

		public static Color Yellow
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.Yellow);
			}
		}

		public static Color YellowGreen
		{
			get
			{
				return KnownColors.FromKnownColor(KnownColor.YellowGreen);
			}
		}

		public static bool operator ==(Color left, Color right)
		{
			return left.Value == right.Value && left.IsNamedColor == right.IsNamedColor && left.IsSystemColor == right.IsSystemColor && left.IsEmpty == right.IsEmpty && (!left.IsNamedColor || !(left.Name != right.Name));
		}

		public static bool operator !=(Color left, Color right)
		{
			return !(left == right);
		}

		private long value;

		internal short state;

		internal short knownColor;

		internal string name;

		public static readonly Color Empty;

		[Flags]
		internal enum ColorType : short
		{
			Empty = 0,
			Known = 1,
			ARGB = 2,
			Named = 4,
			System = 8
		}
	}
}
