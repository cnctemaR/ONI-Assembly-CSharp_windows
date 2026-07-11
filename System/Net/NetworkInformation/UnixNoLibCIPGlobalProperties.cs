using System;

namespace System.Net.NetworkInformation
{
	internal sealed class UnixNoLibCIPGlobalProperties : UnixIPGlobalProperties
	{
		public override string DomainName
		{
			get
			{
				return string.Empty;
			}
		}
	}
}
