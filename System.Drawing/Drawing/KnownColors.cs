using System;

namespace System.Drawing
{
	internal static class KnownColors
	{
		static KnownColors()
		{
			if (GDIPlus.RunningOnWindows())
			{
				KnownColors.RetrieveWindowsSystemColors();
			}
		}

		private static uint GetSysColor(GetSysColorIndex index)
		{
			uint num = GDIPlus.Win32GetSysColor(index);
			return 4278190080U | ((num & 255U) << 16) | (num & 65280U) | (num >> 16);
		}

		private static void RetrieveWindowsSystemColors()
		{
			KnownColors.ArgbValues[1] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_ACTIVEBORDER);
			KnownColors.ArgbValues[2] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_ACTIVECAPTION);
			KnownColors.ArgbValues[3] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_CAPTIONTEXT);
			KnownColors.ArgbValues[4] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_APPWORKSPACE);
			KnownColors.ArgbValues[5] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_BTNFACE);
			KnownColors.ArgbValues[6] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_BTNSHADOW);
			KnownColors.ArgbValues[7] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_3DDKSHADOW);
			KnownColors.ArgbValues[8] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_3DLIGHT);
			KnownColors.ArgbValues[9] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_BTNHIGHLIGHT);
			KnownColors.ArgbValues[10] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_BTNTEXT);
			KnownColors.ArgbValues[11] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_BACKGROUND);
			KnownColors.ArgbValues[12] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_GRAYTEXT);
			KnownColors.ArgbValues[13] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_HIGHLIGHT);
			KnownColors.ArgbValues[14] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_HIGHLIGHTTEXT);
			KnownColors.ArgbValues[15] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_HOTLIGHT);
			KnownColors.ArgbValues[16] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_INACTIVEBORDER);
			KnownColors.ArgbValues[17] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_INACTIVECAPTION);
			KnownColors.ArgbValues[18] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_INACTIVECAPTIONTEXT);
			KnownColors.ArgbValues[19] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_INFOBK);
			KnownColors.ArgbValues[20] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_INFOTEXT);
			KnownColors.ArgbValues[21] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_MENU);
			KnownColors.ArgbValues[22] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_MENUTEXT);
			KnownColors.ArgbValues[23] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_SCROLLBAR);
			KnownColors.ArgbValues[24] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_WINDOW);
			KnownColors.ArgbValues[25] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_WINDOWFRAME);
			KnownColors.ArgbValues[26] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_WINDOWTEXT);
			KnownColors.ArgbValues[168] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_BTNFACE);
			KnownColors.ArgbValues[169] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_BTNHIGHLIGHT);
			KnownColors.ArgbValues[170] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_BTNSHADOW);
			KnownColors.ArgbValues[171] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_GRADIENTACTIVECAPTION);
			KnownColors.ArgbValues[172] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_GRADIENTINACTIVECAPTION);
			KnownColors.ArgbValues[173] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_MENUBAR);
			KnownColors.ArgbValues[174] = KnownColors.GetSysColor(GetSysColorIndex.COLOR_MENUHIGHLIGHT);
		}

		public static Color FromKnownColor(KnownColor kc)
		{
			short num = (short)kc;
			Color color;
			if (num <= 0 || (int)num >= KnownColors.ArgbValues.Length)
			{
				color = Color.FromArgb(0, 0, 0, 0);
				color.state |= 4;
			}
			else
			{
				color = default(Color);
				color.state = 7;
				if (num < 27 || num > 169)
				{
					color.state |= 8;
				}
				color.Value = (long)((ulong)KnownColors.ArgbValues[(int)num]);
			}
			color.knownColor = num;
			return color;
		}

		public static string GetName(short kc)
		{
			switch (kc)
			{
			case 1:
				return "ActiveBorder";
			case 2:
				return "ActiveCaption";
			case 3:
				return "ActiveCaptionText";
			case 4:
				return "AppWorkspace";
			case 5:
				return "Control";
			case 6:
				return "ControlDark";
			case 7:
				return "ControlDarkDark";
			case 8:
				return "ControlLight";
			case 9:
				return "ControlLightLight";
			case 10:
				return "ControlText";
			case 11:
				return "Desktop";
			case 12:
				return "GrayText";
			case 13:
				return "Highlight";
			case 14:
				return "HighlightText";
			case 15:
				return "HotTrack";
			case 16:
				return "InactiveBorder";
			case 17:
				return "InactiveCaption";
			case 18:
				return "InactiveCaptionText";
			case 19:
				return "Info";
			case 20:
				return "InfoText";
			case 21:
				return "Menu";
			case 22:
				return "MenuText";
			case 23:
				return "ScrollBar";
			case 24:
				return "Window";
			case 25:
				return "WindowFrame";
			case 26:
				return "WindowText";
			case 27:
				return "Transparent";
			case 28:
				return "AliceBlue";
			case 29:
				return "AntiqueWhite";
			case 30:
				return "Aqua";
			case 31:
				return "Aquamarine";
			case 32:
				return "Azure";
			case 33:
				return "Beige";
			case 34:
				return "Bisque";
			case 35:
				return "Black";
			case 36:
				return "BlanchedAlmond";
			case 37:
				return "Blue";
			case 38:
				return "BlueViolet";
			case 39:
				return "Brown";
			case 40:
				return "BurlyWood";
			case 41:
				return "CadetBlue";
			case 42:
				return "Chartreuse";
			case 43:
				return "Chocolate";
			case 44:
				return "Coral";
			case 45:
				return "CornflowerBlue";
			case 46:
				return "Cornsilk";
			case 47:
				return "Crimson";
			case 48:
				return "Cyan";
			case 49:
				return "DarkBlue";
			case 50:
				return "DarkCyan";
			case 51:
				return "DarkGoldenrod";
			case 52:
				return "DarkGray";
			case 53:
				return "DarkGreen";
			case 54:
				return "DarkKhaki";
			case 55:
				return "DarkMagenta";
			case 56:
				return "DarkOliveGreen";
			case 57:
				return "DarkOrange";
			case 58:
				return "DarkOrchid";
			case 59:
				return "DarkRed";
			case 60:
				return "DarkSalmon";
			case 61:
				return "DarkSeaGreen";
			case 62:
				return "DarkSlateBlue";
			case 63:
				return "DarkSlateGray";
			case 64:
				return "DarkTurquoise";
			case 65:
				return "DarkViolet";
			case 66:
				return "DeepPink";
			case 67:
				return "DeepSkyBlue";
			case 68:
				return "DimGray";
			case 69:
				return "DodgerBlue";
			case 70:
				return "Firebrick";
			case 71:
				return "FloralWhite";
			case 72:
				return "ForestGreen";
			case 73:
				return "Fuchsia";
			case 74:
				return "Gainsboro";
			case 75:
				return "GhostWhite";
			case 76:
				return "Gold";
			case 77:
				return "Goldenrod";
			case 78:
				return "Gray";
			case 79:
				return "Green";
			case 80:
				return "GreenYellow";
			case 81:
				return "Honeydew";
			case 82:
				return "HotPink";
			case 83:
				return "IndianRed";
			case 84:
				return "Indigo";
			case 85:
				return "Ivory";
			case 86:
				return "Khaki";
			case 87:
				return "Lavender";
			case 88:
				return "LavenderBlush";
			case 89:
				return "LawnGreen";
			case 90:
				return "LemonChiffon";
			case 91:
				return "LightBlue";
			case 92:
				return "LightCoral";
			case 93:
				return "LightCyan";
			case 94:
				return "LightGoldenrodYellow";
			case 95:
				return "LightGray";
			case 96:
				return "LightGreen";
			case 97:
				return "LightPink";
			case 98:
				return "LightSalmon";
			case 99:
				return "LightSeaGreen";
			case 100:
				return "LightSkyBlue";
			case 101:
				return "LightSlateGray";
			case 102:
				return "LightSteelBlue";
			case 103:
				return "LightYellow";
			case 104:
				return "Lime";
			case 105:
				return "LimeGreen";
			case 106:
				return "Linen";
			case 107:
				return "Magenta";
			case 108:
				return "Maroon";
			case 109:
				return "MediumAquamarine";
			case 110:
				return "MediumBlue";
			case 111:
				return "MediumOrchid";
			case 112:
				return "MediumPurple";
			case 113:
				return "MediumSeaGreen";
			case 114:
				return "MediumSlateBlue";
			case 115:
				return "MediumSpringGreen";
			case 116:
				return "MediumTurquoise";
			case 117:
				return "MediumVioletRed";
			case 118:
				return "MidnightBlue";
			case 119:
				return "MintCream";
			case 120:
				return "MistyRose";
			case 121:
				return "Moccasin";
			case 122:
				return "NavajoWhite";
			case 123:
				return "Navy";
			case 124:
				return "OldLace";
			case 125:
				return "Olive";
			case 126:
				return "OliveDrab";
			case 127:
				return "Orange";
			case 128:
				return "OrangeRed";
			case 129:
				return "Orchid";
			case 130:
				return "PaleGoldenrod";
			case 131:
				return "PaleGreen";
			case 132:
				return "PaleTurquoise";
			case 133:
				return "PaleVioletRed";
			case 134:
				return "PapayaWhip";
			case 135:
				return "PeachPuff";
			case 136:
				return "Peru";
			case 137:
				return "Pink";
			case 138:
				return "Plum";
			case 139:
				return "PowderBlue";
			case 140:
				return "Purple";
			case 141:
				return "Red";
			case 142:
				return "RosyBrown";
			case 143:
				return "RoyalBlue";
			case 144:
				return "SaddleBrown";
			case 145:
				return "Salmon";
			case 146:
				return "SandyBrown";
			case 147:
				return "SeaGreen";
			case 148:
				return "SeaShell";
			case 149:
				return "Sienna";
			case 150:
				return "Silver";
			case 151:
				return "SkyBlue";
			case 152:
				return "SlateBlue";
			case 153:
				return "SlateGray";
			case 154:
				return "Snow";
			case 155:
				return "SpringGreen";
			case 156:
				return "SteelBlue";
			case 157:
				return "Tan";
			case 158:
				return "Teal";
			case 159:
				return "Thistle";
			case 160:
				return "Tomato";
			case 161:
				return "Turquoise";
			case 162:
				return "Violet";
			case 163:
				return "Wheat";
			case 164:
				return "White";
			case 165:
				return "WhiteSmoke";
			case 166:
				return "Yellow";
			case 167:
				return "YellowGreen";
			case 168:
				return "ButtonFace";
			case 169:
				return "ButtonHighlight";
			case 170:
				return "ButtonShadow";
			case 171:
				return "GradientActiveCaption";
			case 172:
				return "GradientInactiveCaption";
			case 173:
				return "MenuBar";
			case 174:
				return "MenuHighlight";
			default:
				return string.Empty;
			}
		}

		public static string GetName(KnownColor kc)
		{
			return KnownColors.GetName((short)kc);
		}

		public static Color FindColorMatch(Color c)
		{
			uint num = (uint)c.ToArgb();
			for (int i = 27; i < 167; i++)
			{
				if (num == KnownColors.ArgbValues[i])
				{
					return KnownColors.FromKnownColor((KnownColor)i);
				}
			}
			return Color.Empty;
		}

		public static void Update(int knownColor, int color)
		{
			KnownColors.ArgbValues[knownColor] = (uint)color;
		}

		internal static uint[] ArgbValues = new uint[]
		{
			0U, 4292137160U, 4278211811U, uint.MaxValue, 4286611584U, 4293716440U, 4289505433U, 4285624164U, 4294045666U, uint.MaxValue,
			4278190080U, 4278210200U, 4289505433U, 4281428677U, uint.MaxValue, 4278190208U, 4292137160U, 4286224095U, 4292404472U, 4294967265U,
			4278190080U, uint.MaxValue, 4278190080U, 4292137160U, uint.MaxValue, 4278190080U, 4278190080U, 16777215U, 4293982463U, 4294634455U,
			4278255615U, 4286578644U, 4293984255U, 4294309340U, 4294960324U, 4278190080U, 4294962125U, 4278190335U, 4287245282U, 4289014314U,
			4292786311U, 4284456608U, 4286578432U, 4291979550U, 4294934352U, 4284782061U, 4294965468U, 4292613180U, 4278255615U, 4278190219U,
			4278225803U, 4290283019U, 4289309097U, 4278215680U, 4290623339U, 4287299723U, 4283788079U, 4294937600U, 4288230092U, 4287299584U,
			4293498490U, 4287609995U, 4282924427U, 4281290575U, 4278243025U, 4287889619U, 4294907027U, 4278239231U, 4285098345U, 4280193279U,
			4289864226U, 4294966000U, 4280453922U, 4294902015U, 4292664540U, 4294506751U, 4294956800U, 4292519200U, 4286611584U, 4278222848U,
			4289593135U, 4293984240U, 4294928820U, 4291648604U, 4283105410U, 4294967280U, 4293977740U, 4293322490U, 4294963445U, 4286381056U,
			4294965965U, 4289583334U, 4293951616U, 4292935679U, 4294638290U, 4292072403U, 4287688336U, 4294948545U, 4294942842U, 4280332970U,
			4287090426U, 4286023833U, 4289774814U, 4294967264U, 4278255360U, 4281519410U, 4294635750U, 4294902015U, 4286578688U, 4284927402U,
			4278190285U, 4290401747U, 4287852763U, 4282168177U, 4286277870U, 4278254234U, 4282962380U, 4291237253U, 4279834992U, 4294311930U,
			4294960353U, 4294960309U, 4294958765U, 4278190208U, 4294833638U, 4286611456U, 4285238819U, 4294944000U, 4294919424U, 4292505814U,
			4293847210U, 4288215960U, 4289720046U, 4292571283U, 4294963157U, 4294957753U, 4291659071U, 4294951115U, 4292714717U, 4289781990U,
			4286578816U, 4294901760U, 4290547599U, 4282477025U, 4287317267U, 4294606962U, 4294222944U, 4281240407U, 4294964718U, 4288696877U,
			4290822336U, 4287090411U, 4285160141U, 4285563024U, 4294966010U, 4278255487U, 4282811060U, 4291998860U, 4278222976U, 4292394968U,
			4294927175U, 4282441936U, 4293821166U, 4294303411U, uint.MaxValue, 4294309365U, 4294967040U, 4288335154U, 4293716440U, uint.MaxValue,
			4289505433U, 4282226175U, 4288526827U, 4293716440U, 4281428677U
		};
	}
}
