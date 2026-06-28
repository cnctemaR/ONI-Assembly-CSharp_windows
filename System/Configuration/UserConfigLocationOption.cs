using System;

namespace System.Configuration
{
	internal enum UserConfigLocationOption : uint
	{
		Product = 32U,
		Product_VersionMajor,
		Product_VersionMinor,
		Product_VersionBuild = 36U,
		Product_VersionRevision = 40U,
		Company_Product = 48U,
		Company_Product_VersionMajor,
		Company_Product_VersionMinor,
		Company_Product_VersionBuild = 52U,
		Company_Product_VersionRevision = 56U,
		Evidence = 64U,
		Other = 32768U
	}
}
