using System;
using System.Runtime.InteropServices;

namespace System.Resources
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class SatelliteContractVersionAttribute : Attribute
	{
		public SatelliteContractVersionAttribute(string version)
		{
			this.ver = new Version(version);
		}

		public string Version
		{
			get
			{
				return this.ver.ToString();
			}
		}

		private Version ver;
	}
}
