using System;

namespace UnityEngine.UIElements
{
	internal static class StyleValueFunctionExtension
	{
		public static StyleValueFunction FromUssString(string ussValue)
		{
			ussValue = ussValue.ToLowerInvariant();
			string text = ussValue;
			string text2 = text;
			uint num = global::<PrivateImplementationDetails>.ComputeStringHash(text2);
			if (num <= 1547034252U)
			{
				if (num <= 713384307U)
				{
					if (num != 352001611U)
					{
						if (num != 630476745U)
						{
							if (num == 713384307U)
							{
								if (text2 == "contrast")
								{
									return StyleValueFunction.FilterContrast;
								}
							}
						}
						else if (text2 == "sepia")
						{
							return StyleValueFunction.FilterSepia;
						}
					}
					else if (text2 == "hue-rotate")
					{
						return StyleValueFunction.FilterHueRotate;
					}
				}
				else if (num != 829202337U)
				{
					if (num != 1302218235U)
					{
						if (num == 1547034252U)
						{
							if (text2 == "tint")
							{
								return StyleValueFunction.FilterTint;
							}
						}
					}
					else if (text2 == "linear-gradient")
					{
						return StyleValueFunction.LinearGradient;
					}
				}
				else if (text2 == "invert")
				{
					return StyleValueFunction.FilterInvert;
				}
			}
			else if (num <= 2317739966U)
			{
				if (num != 1811665304U)
				{
					if (num != 2022607796U)
					{
						if (num == 2317739966U)
						{
							if (text2 == "var")
							{
								return StyleValueFunction.Var;
							}
						}
					}
					else if (text2 == "env")
					{
						return StyleValueFunction.Env;
					}
				}
				else if (text2 == "blur")
				{
					return StyleValueFunction.FilterBlur;
				}
			}
			else if (num <= 3334659430U)
			{
				if (num != 2913447899U)
				{
					if (num == 3334659430U)
					{
						if (text2 == "opacity")
						{
							return StyleValueFunction.FilterOpacity;
						}
					}
				}
				else if (text2 == "none")
				{
					return StyleValueFunction.NoneFilter;
				}
			}
			else if (num != 3552172496U)
			{
				if (num == 3644723704U)
				{
					if (text2 == "prop")
					{
						return StyleValueFunction.MaterialProperty;
					}
				}
			}
			else if (text2 == "grayscale")
			{
				return StyleValueFunction.FilterGrayscale;
			}
			throw new ArgumentOutOfRangeException("ussValue", ussValue, "Unknown function name");
		}

		public static string ToUssString(this StyleValueFunction svf)
		{
			string text;
			switch (svf)
			{
			case StyleValueFunction.Var:
				text = "var";
				break;
			case StyleValueFunction.Env:
				text = "env";
				break;
			case StyleValueFunction.LinearGradient:
				text = "linear-gradient";
				break;
			case StyleValueFunction.NoneFilter:
				text = "none";
				break;
			case StyleValueFunction.CustomFilter:
				text = "filter";
				break;
			case StyleValueFunction.FilterTint:
				text = "tint";
				break;
			case StyleValueFunction.FilterOpacity:
				text = "opacity";
				break;
			case StyleValueFunction.FilterInvert:
				text = "invert";
				break;
			case StyleValueFunction.FilterGrayscale:
				text = "grayscale";
				break;
			case StyleValueFunction.FilterSepia:
				text = "sepia";
				break;
			case StyleValueFunction.FilterBlur:
				text = "blur";
				break;
			case StyleValueFunction.FilterContrast:
				text = "contrast";
				break;
			case StyleValueFunction.FilterHueRotate:
				text = "hue-rotate";
				break;
			case StyleValueFunction.MaterialProperty:
				text = "prop";
				break;
			default:
				throw new ArgumentOutOfRangeException("svf", svf, "Unknown StyleValueFunction");
			}
			return text;
		}

		public const string k_Var = "var";

		public const string k_Env = "env";

		public const string k_LinearGradient = "linear-gradient";

		public const string k_NoneFilter = "none";

		public const string k_CustomFilter = "filter";

		public const string k_FilterTint = "tint";

		public const string k_FilterOpacity = "opacity";

		public const string k_FilterInvert = "invert";

		public const string k_FilterGrayscale = "grayscale";

		public const string k_FilterSepia = "sepia";

		public const string k_FilterBlur = "blur";

		public const string k_FilterContrast = "contrast";

		public const string k_FilterHueRotate = "hue-rotate";

		public const string k_MaterialProperty = "prop";
	}
}
