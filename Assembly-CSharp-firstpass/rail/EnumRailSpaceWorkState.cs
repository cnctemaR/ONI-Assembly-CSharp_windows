using System;

namespace rail
{
	public enum EnumRailSpaceWorkState
	{
		kRailSpaceWorkStateNone,
		kRailSpaceWorkStateDownloaded,
		kRailSpaceWorkStateNeedsSync,
		kRailSpaceWorkStateDownloading = 4,
		kRailSpaceWorkStateUploading = 8
	}
}
