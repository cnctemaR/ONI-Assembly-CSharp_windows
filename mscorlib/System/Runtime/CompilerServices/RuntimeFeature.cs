using System;

namespace System.Runtime.CompilerServices
{
	public static class RuntimeFeature
	{
		public static bool IsSupported(string feature)
		{
			if (feature == "PortablePdb" || feature == "DefaultImplementationsOfInterfaces")
			{
				return true;
			}
			if (!(feature == "IsDynamicCodeSupported"))
			{
				return feature == "IsDynamicCodeCompiled" && RuntimeFeature.IsDynamicCodeCompiled;
			}
			return RuntimeFeature.IsDynamicCodeSupported;
		}

		public static bool IsDynamicCodeSupported
		{
			get
			{
				return true;
			}
		}

		public static bool IsDynamicCodeCompiled
		{
			get
			{
				return true;
			}
		}

		public const string PortablePdb = "PortablePdb";

		public const string DefaultImplementationsOfInterfaces = "DefaultImplementationsOfInterfaces";
	}
}
