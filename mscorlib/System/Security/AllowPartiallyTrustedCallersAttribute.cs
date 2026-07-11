using System;
using System.Runtime.InteropServices;

namespace System.Security
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	public sealed class AllowPartiallyTrustedCallersAttribute : Attribute
	{
		public PartialTrustVisibilityLevel PartialTrustVisibilityLevel
		{
			get
			{
				return this._visibilityLevel;
			}
			set
			{
				this._visibilityLevel = value;
			}
		}

		private PartialTrustVisibilityLevel _visibilityLevel;
	}
}
