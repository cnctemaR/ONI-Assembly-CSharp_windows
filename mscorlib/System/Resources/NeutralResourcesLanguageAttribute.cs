using System;
using System.Runtime.InteropServices;

namespace System.Resources
{
	[AttributeUsage(AttributeTargets.Assembly)]
	[ComVisible(true)]
	public sealed class NeutralResourcesLanguageAttribute : Attribute
	{
		public NeutralResourcesLanguageAttribute(string cultureName)
		{
			if (cultureName == null)
			{
				throw new ArgumentNullException("culture is null");
			}
			this.culture = cultureName;
		}

		public NeutralResourcesLanguageAttribute(string cultureName, UltimateResourceFallbackLocation location)
		{
			if (cultureName == null)
			{
				throw new ArgumentNullException("culture is null");
			}
			this.culture = cultureName;
			this.loc = location;
		}

		public string CultureName
		{
			get
			{
				return this.culture;
			}
		}

		public UltimateResourceFallbackLocation Location
		{
			get
			{
				return this.loc;
			}
		}

		private string culture;

		private UltimateResourceFallbackLocation loc;
	}
}
