using System;
using System.Globalization;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[VisibleToOtherModules]
	internal sealed class UnityString
	{
		[Obsolete("UnityString.Format is redundant and will be removed in a future version. Please move to using modern C# string interpolation or string.Format. (UnityUpgradable) -> [netstandard] System.String.Format(*)")]
		public static string Format(string fmt, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture.NumberFormat, fmt, args);
		}
	}
}
