using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Android
{
	[NativeType(Header = "Modules/AndroidJNI/Public/AndroidConfiguration.bindings.h")]
	[RequiredByNativeCode]
	[NativeAsStruct]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AndroidConfiguration
	{
		private int colorMode { get; set; }

		public int densityDpi { get; private set; }

		public float fontScale { get; private set; }

		public int fontWeightAdjustment { get; private set; }

		public AndroidKeyboard keyboard { get; private set; }

		public AndroidHardwareKeyboardHidden hardKeyboardHidden { get; private set; }

		public AndroidKeyboardHidden keyboardHidden { get; private set; }

		public int mobileCountryCode { get; private set; }

		public int mobileNetworkCode { get; private set; }

		public AndroidNavigation navigation { get; private set; }

		public AndroidNavigationHidden navigationHidden { get; private set; }

		public AndroidOrientation orientation { get; private set; }

		public int screenHeightDp { get; private set; }

		public int screenWidthDp { get; private set; }

		public int smallestScreenWidthDp { get; private set; }

		private int screenLayout { get; set; }

		public AndroidTouchScreen touchScreen { get; private set; }

		private int uiMode { get; set; }

		private string primaryLocaleCountry { get; set; }

		private string primaryLocaleLanguage { get; set; }

		public AndroidLocale[] locales
		{
			get
			{
				bool flag = this.primaryLocaleCountry == null && this.primaryLocaleLanguage == null;
				AndroidLocale[] array;
				if (flag)
				{
					array = new AndroidLocale[0];
				}
				else
				{
					array = new AndroidLocale[]
					{
						new AndroidLocale(this.primaryLocaleCountry, this.primaryLocaleLanguage)
					};
				}
				return array;
			}
		}

		public AndroidColorModeHdr colorModeHdr
		{
			get
			{
				return (AndroidColorModeHdr)(this.colorMode & 12);
			}
		}

		public AndroidColorModeWideColorGamut colorModeWideColorGamut
		{
			get
			{
				return (AndroidColorModeWideColorGamut)(this.colorMode & 3);
			}
		}

		public AndroidScreenLayoutDirection screenLayoutDirection
		{
			get
			{
				return (AndroidScreenLayoutDirection)(this.screenLayout & 192);
			}
		}

		public AndroidScreenLayoutLong screenLayoutLong
		{
			get
			{
				return (AndroidScreenLayoutLong)(this.screenLayout & 48);
			}
		}

		public AndroidScreenLayoutRound screenLayoutRound
		{
			get
			{
				return (AndroidScreenLayoutRound)(this.screenLayout & 768);
			}
		}

		public AndroidScreenLayoutSize screenLayoutSize
		{
			get
			{
				return (AndroidScreenLayoutSize)(this.screenLayout & 15);
			}
		}

		public AndroidUIModeNight uiModeNight
		{
			get
			{
				return (AndroidUIModeNight)(this.uiMode & 48);
			}
		}

		public AndroidUIModeType uiModeType
		{
			get
			{
				return (AndroidUIModeType)(this.uiMode & 15);
			}
		}

		public AndroidConfiguration()
		{
		}

		public AndroidConfiguration(AndroidConfiguration otherConfiguration)
		{
			this.CopyFrom(otherConfiguration);
		}

		public void CopyFrom(AndroidConfiguration otherConfiguration)
		{
			this.colorMode = otherConfiguration.colorMode;
			this.densityDpi = otherConfiguration.densityDpi;
			this.fontScale = otherConfiguration.fontScale;
			this.fontWeightAdjustment = otherConfiguration.fontWeightAdjustment;
			this.keyboard = otherConfiguration.keyboard;
			this.hardKeyboardHidden = otherConfiguration.hardKeyboardHidden;
			this.keyboardHidden = otherConfiguration.keyboardHidden;
			this.mobileCountryCode = otherConfiguration.mobileCountryCode;
			this.mobileNetworkCode = otherConfiguration.mobileNetworkCode;
			this.navigation = otherConfiguration.navigation;
			this.navigationHidden = otherConfiguration.navigationHidden;
			this.orientation = otherConfiguration.orientation;
			this.screenHeightDp = otherConfiguration.screenHeightDp;
			this.screenWidthDp = otherConfiguration.screenWidthDp;
			this.smallestScreenWidthDp = otherConfiguration.smallestScreenWidthDp;
			this.screenLayout = otherConfiguration.screenLayout;
			this.touchScreen = otherConfiguration.touchScreen;
			this.uiMode = otherConfiguration.uiMode;
			this.primaryLocaleCountry = otherConfiguration.primaryLocaleCountry;
			this.primaryLocaleLanguage = otherConfiguration.primaryLocaleLanguage;
		}

		[Preserve]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("* ColorMode, Hdr: {0}", this.colorModeHdr));
			stringBuilder.AppendLine(string.Format("* ColorMode, Gamut: {0}", this.colorModeWideColorGamut));
			stringBuilder.AppendLine(string.Format("* DensityDpi: {0}", this.densityDpi));
			stringBuilder.AppendLine(string.Format("* FontScale: {0}", this.fontScale));
			stringBuilder.AppendLine(string.Format("* FontWeightAdj: {0}", this.fontWeightAdjustment));
			stringBuilder.AppendLine(string.Format("* Keyboard: {0}", this.keyboard));
			stringBuilder.AppendLine(string.Format("* Keyboard Hidden, Hard: {0}", this.hardKeyboardHidden));
			stringBuilder.AppendLine(string.Format("* Keyboard Hidden, Normal: {0}", this.keyboardHidden));
			stringBuilder.AppendLine(string.Format("* Mcc: {0}", this.mobileCountryCode));
			stringBuilder.AppendLine(string.Format("* Mnc: {0}", this.mobileNetworkCode));
			stringBuilder.AppendLine(string.Format("* Navigation: {0}", this.navigation));
			stringBuilder.AppendLine(string.Format("* NavigationHidden: {0}", this.navigationHidden));
			stringBuilder.AppendLine(string.Format("* Orientation: {0}", this.orientation));
			stringBuilder.AppendLine(string.Format("* ScreenHeightDp: {0}", this.screenHeightDp));
			stringBuilder.AppendLine(string.Format("* ScreenWidthDp: {0}", this.screenWidthDp));
			stringBuilder.AppendLine(string.Format("* SmallestScreenWidthDp: {0}", this.smallestScreenWidthDp));
			stringBuilder.AppendLine(string.Format("* ScreenLayout, Direction: {0}", this.screenLayoutDirection));
			stringBuilder.AppendLine(string.Format("* ScreenLayout, Size: {0}", this.screenLayoutSize));
			stringBuilder.AppendLine(string.Format("* ScreenLayout, Long: {0}", this.screenLayoutLong));
			stringBuilder.AppendLine(string.Format("* ScreenLayout, Round: {0}", this.screenLayoutRound));
			stringBuilder.AppendLine(string.Format("* TouchScreen: {0}", this.touchScreen));
			stringBuilder.AppendLine(string.Format("* UiMode, Night: {0}", this.uiModeNight));
			stringBuilder.AppendLine(string.Format("* UiMode, Type: {0}", this.uiModeType));
			stringBuilder.AppendLine(string.Format("* Locales ({0}):", this.locales.Length));
			for (int i = 0; i < this.locales.Length; i++)
			{
				AndroidLocale androidLocale = this.locales[i];
				stringBuilder.AppendLine(string.Format("* Locale[{0}] {1}-{2}", i, androidLocale.country, androidLocale.language));
			}
			return stringBuilder.ToString();
		}

		private const int UiModeNightMask = 48;

		private const int UiModeTypeMask = 15;

		private const int ScreenLayoutDirectionMask = 192;

		private const int ScreenLayoutLongMask = 48;

		private const int ScreenLayoutRoundMask = 768;

		private const int ScreenLayoutSizeMask = 15;

		private const int ColorModeHdrMask = 12;

		private const int ColorModeWideColorGamutMask = 3;
	}
}
