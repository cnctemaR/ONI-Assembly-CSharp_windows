using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Numerics.Hashing;

namespace System.Drawing
{
	[TypeConverter(typeof(ColorConverter))]
	[Editor("System.Drawing.Design.ColorEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[DebuggerDisplay("{NameAndARGBValue}")]
	[Serializable]
	public readonly struct Color : IEquatable<Color>
	{
		public static Color Transparent
		{
			get
			{
				return new Color(KnownColor.Transparent);
			}
		}

		public static Color AliceBlue
		{
			get
			{
				return new Color(KnownColor.AliceBlue);
			}
		}

		public static Color AntiqueWhite
		{
			get
			{
				return new Color(KnownColor.AntiqueWhite);
			}
		}

		public static Color Aqua
		{
			get
			{
				return new Color(KnownColor.Aqua);
			}
		}

		public static Color Aquamarine
		{
			get
			{
				return new Color(KnownColor.Aquamarine);
			}
		}

		public static Color Azure
		{
			get
			{
				return new Color(KnownColor.Azure);
			}
		}

		public static Color Beige
		{
			get
			{
				return new Color(KnownColor.Beige);
			}
		}

		public static Color Bisque
		{
			get
			{
				return new Color(KnownColor.Bisque);
			}
		}

		public static Color Black
		{
			get
			{
				return new Color(KnownColor.Black);
			}
		}

		public static Color BlanchedAlmond
		{
			get
			{
				return new Color(KnownColor.BlanchedAlmond);
			}
		}

		public static Color Blue
		{
			get
			{
				return new Color(KnownColor.Blue);
			}
		}

		public static Color BlueViolet
		{
			get
			{
				return new Color(KnownColor.BlueViolet);
			}
		}

		public static Color Brown
		{
			get
			{
				return new Color(KnownColor.Brown);
			}
		}

		public static Color BurlyWood
		{
			get
			{
				return new Color(KnownColor.BurlyWood);
			}
		}

		public static Color CadetBlue
		{
			get
			{
				return new Color(KnownColor.CadetBlue);
			}
		}

		public static Color Chartreuse
		{
			get
			{
				return new Color(KnownColor.Chartreuse);
			}
		}

		public static Color Chocolate
		{
			get
			{
				return new Color(KnownColor.Chocolate);
			}
		}

		public static Color Coral
		{
			get
			{
				return new Color(KnownColor.Coral);
			}
		}

		public static Color CornflowerBlue
		{
			get
			{
				return new Color(KnownColor.CornflowerBlue);
			}
		}

		public static Color Cornsilk
		{
			get
			{
				return new Color(KnownColor.Cornsilk);
			}
		}

		public static Color Crimson
		{
			get
			{
				return new Color(KnownColor.Crimson);
			}
		}

		public static Color Cyan
		{
			get
			{
				return new Color(KnownColor.Cyan);
			}
		}

		public static Color DarkBlue
		{
			get
			{
				return new Color(KnownColor.DarkBlue);
			}
		}

		public static Color DarkCyan
		{
			get
			{
				return new Color(KnownColor.DarkCyan);
			}
		}

		public static Color DarkGoldenrod
		{
			get
			{
				return new Color(KnownColor.DarkGoldenrod);
			}
		}

		public static Color DarkGray
		{
			get
			{
				return new Color(KnownColor.DarkGray);
			}
		}

		public static Color DarkGreen
		{
			get
			{
				return new Color(KnownColor.DarkGreen);
			}
		}

		public static Color DarkKhaki
		{
			get
			{
				return new Color(KnownColor.DarkKhaki);
			}
		}

		public static Color DarkMagenta
		{
			get
			{
				return new Color(KnownColor.DarkMagenta);
			}
		}

		public static Color DarkOliveGreen
		{
			get
			{
				return new Color(KnownColor.DarkOliveGreen);
			}
		}

		public static Color DarkOrange
		{
			get
			{
				return new Color(KnownColor.DarkOrange);
			}
		}

		public static Color DarkOrchid
		{
			get
			{
				return new Color(KnownColor.DarkOrchid);
			}
		}

		public static Color DarkRed
		{
			get
			{
				return new Color(KnownColor.DarkRed);
			}
		}

		public static Color DarkSalmon
		{
			get
			{
				return new Color(KnownColor.DarkSalmon);
			}
		}

		public static Color DarkSeaGreen
		{
			get
			{
				return new Color(KnownColor.DarkSeaGreen);
			}
		}

		public static Color DarkSlateBlue
		{
			get
			{
				return new Color(KnownColor.DarkSlateBlue);
			}
		}

		public static Color DarkSlateGray
		{
			get
			{
				return new Color(KnownColor.DarkSlateGray);
			}
		}

		public static Color DarkTurquoise
		{
			get
			{
				return new Color(KnownColor.DarkTurquoise);
			}
		}

		public static Color DarkViolet
		{
			get
			{
				return new Color(KnownColor.DarkViolet);
			}
		}

		public static Color DeepPink
		{
			get
			{
				return new Color(KnownColor.DeepPink);
			}
		}

		public static Color DeepSkyBlue
		{
			get
			{
				return new Color(KnownColor.DeepSkyBlue);
			}
		}

		public static Color DimGray
		{
			get
			{
				return new Color(KnownColor.DimGray);
			}
		}

		public static Color DodgerBlue
		{
			get
			{
				return new Color(KnownColor.DodgerBlue);
			}
		}

		public static Color Firebrick
		{
			get
			{
				return new Color(KnownColor.Firebrick);
			}
		}

		public static Color FloralWhite
		{
			get
			{
				return new Color(KnownColor.FloralWhite);
			}
		}

		public static Color ForestGreen
		{
			get
			{
				return new Color(KnownColor.ForestGreen);
			}
		}

		public static Color Fuchsia
		{
			get
			{
				return new Color(KnownColor.Fuchsia);
			}
		}

		public static Color Gainsboro
		{
			get
			{
				return new Color(KnownColor.Gainsboro);
			}
		}

		public static Color GhostWhite
		{
			get
			{
				return new Color(KnownColor.GhostWhite);
			}
		}

		public static Color Gold
		{
			get
			{
				return new Color(KnownColor.Gold);
			}
		}

		public static Color Goldenrod
		{
			get
			{
				return new Color(KnownColor.Goldenrod);
			}
		}

		public static Color Gray
		{
			get
			{
				return new Color(KnownColor.Gray);
			}
		}

		public static Color Green
		{
			get
			{
				return new Color(KnownColor.Green);
			}
		}

		public static Color GreenYellow
		{
			get
			{
				return new Color(KnownColor.GreenYellow);
			}
		}

		public static Color Honeydew
		{
			get
			{
				return new Color(KnownColor.Honeydew);
			}
		}

		public static Color HotPink
		{
			get
			{
				return new Color(KnownColor.HotPink);
			}
		}

		public static Color IndianRed
		{
			get
			{
				return new Color(KnownColor.IndianRed);
			}
		}

		public static Color Indigo
		{
			get
			{
				return new Color(KnownColor.Indigo);
			}
		}

		public static Color Ivory
		{
			get
			{
				return new Color(KnownColor.Ivory);
			}
		}

		public static Color Khaki
		{
			get
			{
				return new Color(KnownColor.Khaki);
			}
		}

		public static Color Lavender
		{
			get
			{
				return new Color(KnownColor.Lavender);
			}
		}

		public static Color LavenderBlush
		{
			get
			{
				return new Color(KnownColor.LavenderBlush);
			}
		}

		public static Color LawnGreen
		{
			get
			{
				return new Color(KnownColor.LawnGreen);
			}
		}

		public static Color LemonChiffon
		{
			get
			{
				return new Color(KnownColor.LemonChiffon);
			}
		}

		public static Color LightBlue
		{
			get
			{
				return new Color(KnownColor.LightBlue);
			}
		}

		public static Color LightCoral
		{
			get
			{
				return new Color(KnownColor.LightCoral);
			}
		}

		public static Color LightCyan
		{
			get
			{
				return new Color(KnownColor.LightCyan);
			}
		}

		public static Color LightGoldenrodYellow
		{
			get
			{
				return new Color(KnownColor.LightGoldenrodYellow);
			}
		}

		public static Color LightGreen
		{
			get
			{
				return new Color(KnownColor.LightGreen);
			}
		}

		public static Color LightGray
		{
			get
			{
				return new Color(KnownColor.LightGray);
			}
		}

		public static Color LightPink
		{
			get
			{
				return new Color(KnownColor.LightPink);
			}
		}

		public static Color LightSalmon
		{
			get
			{
				return new Color(KnownColor.LightSalmon);
			}
		}

		public static Color LightSeaGreen
		{
			get
			{
				return new Color(KnownColor.LightSeaGreen);
			}
		}

		public static Color LightSkyBlue
		{
			get
			{
				return new Color(KnownColor.LightSkyBlue);
			}
		}

		public static Color LightSlateGray
		{
			get
			{
				return new Color(KnownColor.LightSlateGray);
			}
		}

		public static Color LightSteelBlue
		{
			get
			{
				return new Color(KnownColor.LightSteelBlue);
			}
		}

		public static Color LightYellow
		{
			get
			{
				return new Color(KnownColor.LightYellow);
			}
		}

		public static Color Lime
		{
			get
			{
				return new Color(KnownColor.Lime);
			}
		}

		public static Color LimeGreen
		{
			get
			{
				return new Color(KnownColor.LimeGreen);
			}
		}

		public static Color Linen
		{
			get
			{
				return new Color(KnownColor.Linen);
			}
		}

		public static Color Magenta
		{
			get
			{
				return new Color(KnownColor.Magenta);
			}
		}

		public static Color Maroon
		{
			get
			{
				return new Color(KnownColor.Maroon);
			}
		}

		public static Color MediumAquamarine
		{
			get
			{
				return new Color(KnownColor.MediumAquamarine);
			}
		}

		public static Color MediumBlue
		{
			get
			{
				return new Color(KnownColor.MediumBlue);
			}
		}

		public static Color MediumOrchid
		{
			get
			{
				return new Color(KnownColor.MediumOrchid);
			}
		}

		public static Color MediumPurple
		{
			get
			{
				return new Color(KnownColor.MediumPurple);
			}
		}

		public static Color MediumSeaGreen
		{
			get
			{
				return new Color(KnownColor.MediumSeaGreen);
			}
		}

		public static Color MediumSlateBlue
		{
			get
			{
				return new Color(KnownColor.MediumSlateBlue);
			}
		}

		public static Color MediumSpringGreen
		{
			get
			{
				return new Color(KnownColor.MediumSpringGreen);
			}
		}

		public static Color MediumTurquoise
		{
			get
			{
				return new Color(KnownColor.MediumTurquoise);
			}
		}

		public static Color MediumVioletRed
		{
			get
			{
				return new Color(KnownColor.MediumVioletRed);
			}
		}

		public static Color MidnightBlue
		{
			get
			{
				return new Color(KnownColor.MidnightBlue);
			}
		}

		public static Color MintCream
		{
			get
			{
				return new Color(KnownColor.MintCream);
			}
		}

		public static Color MistyRose
		{
			get
			{
				return new Color(KnownColor.MistyRose);
			}
		}

		public static Color Moccasin
		{
			get
			{
				return new Color(KnownColor.Moccasin);
			}
		}

		public static Color NavajoWhite
		{
			get
			{
				return new Color(KnownColor.NavajoWhite);
			}
		}

		public static Color Navy
		{
			get
			{
				return new Color(KnownColor.Navy);
			}
		}

		public static Color OldLace
		{
			get
			{
				return new Color(KnownColor.OldLace);
			}
		}

		public static Color Olive
		{
			get
			{
				return new Color(KnownColor.Olive);
			}
		}

		public static Color OliveDrab
		{
			get
			{
				return new Color(KnownColor.OliveDrab);
			}
		}

		public static Color Orange
		{
			get
			{
				return new Color(KnownColor.Orange);
			}
		}

		public static Color OrangeRed
		{
			get
			{
				return new Color(KnownColor.OrangeRed);
			}
		}

		public static Color Orchid
		{
			get
			{
				return new Color(KnownColor.Orchid);
			}
		}

		public static Color PaleGoldenrod
		{
			get
			{
				return new Color(KnownColor.PaleGoldenrod);
			}
		}

		public static Color PaleGreen
		{
			get
			{
				return new Color(KnownColor.PaleGreen);
			}
		}

		public static Color PaleTurquoise
		{
			get
			{
				return new Color(KnownColor.PaleTurquoise);
			}
		}

		public static Color PaleVioletRed
		{
			get
			{
				return new Color(KnownColor.PaleVioletRed);
			}
		}

		public static Color PapayaWhip
		{
			get
			{
				return new Color(KnownColor.PapayaWhip);
			}
		}

		public static Color PeachPuff
		{
			get
			{
				return new Color(KnownColor.PeachPuff);
			}
		}

		public static Color Peru
		{
			get
			{
				return new Color(KnownColor.Peru);
			}
		}

		public static Color Pink
		{
			get
			{
				return new Color(KnownColor.Pink);
			}
		}

		public static Color Plum
		{
			get
			{
				return new Color(KnownColor.Plum);
			}
		}

		public static Color PowderBlue
		{
			get
			{
				return new Color(KnownColor.PowderBlue);
			}
		}

		public static Color Purple
		{
			get
			{
				return new Color(KnownColor.Purple);
			}
		}

		public static Color Red
		{
			get
			{
				return new Color(KnownColor.Red);
			}
		}

		public static Color RosyBrown
		{
			get
			{
				return new Color(KnownColor.RosyBrown);
			}
		}

		public static Color RoyalBlue
		{
			get
			{
				return new Color(KnownColor.RoyalBlue);
			}
		}

		public static Color SaddleBrown
		{
			get
			{
				return new Color(KnownColor.SaddleBrown);
			}
		}

		public static Color Salmon
		{
			get
			{
				return new Color(KnownColor.Salmon);
			}
		}

		public static Color SandyBrown
		{
			get
			{
				return new Color(KnownColor.SandyBrown);
			}
		}

		public static Color SeaGreen
		{
			get
			{
				return new Color(KnownColor.SeaGreen);
			}
		}

		public static Color SeaShell
		{
			get
			{
				return new Color(KnownColor.SeaShell);
			}
		}

		public static Color Sienna
		{
			get
			{
				return new Color(KnownColor.Sienna);
			}
		}

		public static Color Silver
		{
			get
			{
				return new Color(KnownColor.Silver);
			}
		}

		public static Color SkyBlue
		{
			get
			{
				return new Color(KnownColor.SkyBlue);
			}
		}

		public static Color SlateBlue
		{
			get
			{
				return new Color(KnownColor.SlateBlue);
			}
		}

		public static Color SlateGray
		{
			get
			{
				return new Color(KnownColor.SlateGray);
			}
		}

		public static Color Snow
		{
			get
			{
				return new Color(KnownColor.Snow);
			}
		}

		public static Color SpringGreen
		{
			get
			{
				return new Color(KnownColor.SpringGreen);
			}
		}

		public static Color SteelBlue
		{
			get
			{
				return new Color(KnownColor.SteelBlue);
			}
		}

		public static Color Tan
		{
			get
			{
				return new Color(KnownColor.Tan);
			}
		}

		public static Color Teal
		{
			get
			{
				return new Color(KnownColor.Teal);
			}
		}

		public static Color Thistle
		{
			get
			{
				return new Color(KnownColor.Thistle);
			}
		}

		public static Color Tomato
		{
			get
			{
				return new Color(KnownColor.Tomato);
			}
		}

		public static Color Turquoise
		{
			get
			{
				return new Color(KnownColor.Turquoise);
			}
		}

		public static Color Violet
		{
			get
			{
				return new Color(KnownColor.Violet);
			}
		}

		public static Color Wheat
		{
			get
			{
				return new Color(KnownColor.Wheat);
			}
		}

		public static Color White
		{
			get
			{
				return new Color(KnownColor.White);
			}
		}

		public static Color WhiteSmoke
		{
			get
			{
				return new Color(KnownColor.WhiteSmoke);
			}
		}

		public static Color Yellow
		{
			get
			{
				return new Color(KnownColor.Yellow);
			}
		}

		public static Color YellowGreen
		{
			get
			{
				return new Color(KnownColor.YellowGreen);
			}
		}

		internal Color(KnownColor knownColor)
		{
			this.value = 0L;
			this.state = 1;
			this.name = null;
			this.knownColor = (short)knownColor;
		}

		private Color(long value, short state, string name, KnownColor knownColor)
		{
			this.value = value;
			this.state = state;
			this.name = name;
			this.knownColor = (short)knownColor;
		}

		public byte R
		{
			get
			{
				return (byte)((this.Value >> 16) & 255L);
			}
		}

		public byte G
		{
			get
			{
				return (byte)((this.Value >> 8) & 255L);
			}
		}

		public byte B
		{
			get
			{
				return (byte)(this.Value & 255L);
			}
		}

		public byte A
		{
			get
			{
				return (byte)((this.Value >> 24) & 255L);
			}
		}

		public bool IsKnownColor
		{
			get
			{
				return (this.state & 1) != 0;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.state == 0;
			}
		}

		public bool IsNamedColor
		{
			get
			{
				return (this.state & 8) != 0 || this.IsKnownColor;
			}
		}

		public bool IsSystemColor
		{
			get
			{
				return this.IsKnownColor && (this.knownColor <= 26 || this.knownColor > 167);
			}
		}

		private string NameAndARGBValue
		{
			get
			{
				return string.Format("{{Name={0}, ARGB=({1}, {2}, {3}, {4})}}", new object[] { this.Name, this.A, this.R, this.G, this.B });
			}
		}

		public string Name
		{
			get
			{
				if ((this.state & 8) != 0)
				{
					return this.name;
				}
				if (this.IsKnownColor)
				{
					return KnownColorTable.KnownColorToName((KnownColor)this.knownColor);
				}
				return Convert.ToString(this.value, 16);
			}
		}

		private long Value
		{
			get
			{
				if ((this.state & 2) != 0)
				{
					return this.value;
				}
				if (this.IsKnownColor)
				{
					return (long)KnownColorTable.KnownColorToArgb((KnownColor)this.knownColor);
				}
				return 0L;
			}
		}

		private static void CheckByte(int value, string name)
		{
			if (value < 0 || value > 255)
			{
				throw new ArgumentException(SR.Format("Value of '{1}' is not valid for '{0}'. '{0}' should be greater than or equal to {2} and less than or equal to {3}.", new object[] { name, value, 0, 255 }));
			}
		}

		private static long MakeArgb(byte alpha, byte red, byte green, byte blue)
		{
			return (long)((ulong)(((int)red << 16) | ((int)green << 8) | (int)blue | ((int)alpha << 24)) & (ulong)(-1));
		}

		public static Color FromArgb(int argb)
		{
			return new Color((long)argb & (long)((ulong)(-1)), 2, null, (KnownColor)0);
		}

		public static Color FromArgb(int alpha, int red, int green, int blue)
		{
			Color.CheckByte(alpha, "alpha");
			Color.CheckByte(red, "red");
			Color.CheckByte(green, "green");
			Color.CheckByte(blue, "blue");
			return new Color(Color.MakeArgb((byte)alpha, (byte)red, (byte)green, (byte)blue), 2, null, (KnownColor)0);
		}

		public static Color FromArgb(int alpha, Color baseColor)
		{
			Color.CheckByte(alpha, "alpha");
			return new Color(Color.MakeArgb((byte)alpha, baseColor.R, baseColor.G, baseColor.B), 2, null, (KnownColor)0);
		}

		public static Color FromArgb(int red, int green, int blue)
		{
			return Color.FromArgb(255, red, green, blue);
		}

		public static Color FromKnownColor(KnownColor color)
		{
			if (color > (KnownColor)0 && color <= KnownColor.MenuHighlight)
			{
				return new Color(color);
			}
			return Color.FromName(color.ToString());
		}

		public static Color FromName(string name)
		{
			Color color;
			if (ColorTable.TryGetNamedColor(name, out color))
			{
				return color;
			}
			return new Color(0L, 8, name, (KnownColor)0);
		}

		public float GetBrightness()
		{
			float num = (float)this.R / 255f;
			float num2 = (float)this.G / 255f;
			float num3 = (float)this.B / 255f;
			float num4 = num;
			float num5 = num;
			if (num2 > num4)
			{
				num4 = num2;
			}
			else if (num2 < num5)
			{
				num5 = num2;
			}
			if (num3 > num4)
			{
				num4 = num3;
			}
			else if (num3 < num5)
			{
				num5 = num3;
			}
			return (num4 + num5) / 2f;
		}

		public float GetHue()
		{
			if (this.R == this.G && this.G == this.B)
			{
				return 0f;
			}
			float num = (float)this.R / 255f;
			float num2 = (float)this.G / 255f;
			float num3 = (float)this.B / 255f;
			float num4 = num;
			float num5 = num;
			if (num2 > num4)
			{
				num4 = num2;
			}
			else if (num2 < num5)
			{
				num5 = num2;
			}
			if (num3 > num4)
			{
				num4 = num3;
			}
			else if (num3 < num5)
			{
				num5 = num3;
			}
			float num6 = num4 - num5;
			float num7;
			if (num == num4)
			{
				num7 = (num2 - num3) / num6;
			}
			else if (num2 == num4)
			{
				num7 = 2f + (num3 - num) / num6;
			}
			else
			{
				num7 = 4f + (num - num2) / num6;
			}
			num7 *= 60f;
			if (num7 < 0f)
			{
				num7 += 360f;
			}
			return num7;
		}

		public float GetSaturation()
		{
			float num = (float)this.R / 255f;
			float num2 = (float)this.G / 255f;
			float num3 = (float)this.B / 255f;
			float num4 = 0f;
			float num5 = num;
			float num6 = num;
			if (num2 > num5)
			{
				num5 = num2;
			}
			else if (num2 < num6)
			{
				num6 = num2;
			}
			if (num3 > num5)
			{
				num5 = num3;
			}
			else if (num3 < num6)
			{
				num6 = num3;
			}
			if (num5 != num6)
			{
				if ((double)((num5 + num6) / 2f) <= 0.5)
				{
					num4 = (num5 - num6) / (num5 + num6);
				}
				else
				{
					num4 = (num5 - num6) / (2f - num5 - num6);
				}
			}
			return num4;
		}

		public int ToArgb()
		{
			return (int)this.Value;
		}

		public KnownColor ToKnownColor()
		{
			return (KnownColor)this.knownColor;
		}

		public override string ToString()
		{
			if ((this.state & 8) != 0 || (this.state & 1) != 0)
			{
				return "Color [" + this.Name + "]";
			}
			if ((this.state & 2) != 0)
			{
				return string.Concat(new string[]
				{
					"Color [A=",
					this.A.ToString(),
					", R=",
					this.R.ToString(),
					", G=",
					this.G.ToString(),
					", B=",
					this.B.ToString(),
					"]"
				});
			}
			return "Color [Empty]";
		}

		public static bool operator ==(Color left, Color right)
		{
			return left.value == right.value && left.state == right.state && left.knownColor == right.knownColor && left.name == right.name;
		}

		public static bool operator !=(Color left, Color right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			return obj is Color && this.Equals((Color)obj);
		}

		public bool Equals(Color other)
		{
			return this == other;
		}

		public override int GetHashCode()
		{
			if ((this.name != null) & !this.IsKnownColor)
			{
				return this.name.GetHashCode();
			}
			return HashHelpers.Combine(HashHelpers.Combine(this.value.GetHashCode(), this.state.GetHashCode()), this.knownColor.GetHashCode());
		}

		public static readonly Color Empty;

		private const short StateKnownColorValid = 1;

		private const short StateARGBValueValid = 2;

		private const short StateValueMask = 2;

		private const short StateNameValid = 8;

		private const long NotDefinedValue = 0L;

		private const int ARGBAlphaShift = 24;

		private const int ARGBRedShift = 16;

		private const int ARGBGreenShift = 8;

		private const int ARGBBlueShift = 0;

		private readonly string name;

		private readonly long value;

		private readonly short knownColor;

		private readonly short state;
	}
}
