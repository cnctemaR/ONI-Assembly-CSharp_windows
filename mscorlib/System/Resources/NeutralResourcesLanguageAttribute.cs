using System;
using System.Runtime.InteropServices;

namespace System.Resources
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	[ComVisible(true)]
	public sealed class NeutralResourcesLanguageAttribute : Attribute
	{
		public NeutralResourcesLanguageAttribute(string cultureName)
		{
			if (cultureName == null)
			{
				throw new ArgumentNullException("cultureName");
			}
			this._culture = cultureName;
			this._fallbackLoc = UltimateResourceFallbackLocation.MainAssembly;
		}

		public NeutralResourcesLanguageAttribute(string cultureName, UltimateResourceFallbackLocation location)
		{
			if (cultureName == null)
			{
				throw new ArgumentNullException("cultureName");
			}
			if (!Enum.IsDefined(typeof(UltimateResourceFallbackLocation), location))
			{
				throw new ArgumentException(Environment.GetResourceString("The NeutralResourcesLanguageAttribute specifies an invalid or unrecognized ultimate resource fallback location: \"{0}\".", new object[] { location }));
			}
			this._culture = cultureName;
			this._fallbackLoc = location;
		}

		public string CultureName
		{
			get
			{
				return this._culture;
			}
		}

		public UltimateResourceFallbackLocation Location
		{
			get
			{
				return this._fallbackLoc;
			}
		}

		private string _culture;

		private UltimateResourceFallbackLocation _fallbackLoc;
	}
}
