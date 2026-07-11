using System;

namespace UnityEngine.UIElements
{
	internal static class StyleValueFunctionExtension
	{
		public static StyleValueFunction FromUssString(string ussValue)
		{
			ussValue = ussValue.ToLower();
			string text = ussValue;
			StyleValueFunction styleValueFunction;
			if (!(text == "var"))
			{
				if (!(text == "env"))
				{
					if (!(text == "linear-gradient"))
					{
						throw new ArgumentOutOfRangeException("ussValue", ussValue, "Unknown function name");
					}
					styleValueFunction = StyleValueFunction.LinearGradient;
				}
				else
				{
					styleValueFunction = StyleValueFunction.Env;
				}
			}
			else
			{
				styleValueFunction = StyleValueFunction.Var;
			}
			return styleValueFunction;
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
			default:
				throw new ArgumentOutOfRangeException("svf", svf, "Unknown StyleValueFunction");
			}
			return text;
		}

		public const string k_Var = "var";

		public const string k_Env = "env";

		public const string k_LinearGradient = "linear-gradient";
	}
}
