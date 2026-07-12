using System;

namespace UnityEngine.Search
{
	[Flags]
	public enum SearchViewFlags
	{
		None = 0,
		Debug = 16,
		NoIndexing = 32,
		Packages = 256,
		OpenLeftSidePanel = 2048,
		OpenInspectorPreview = 4096,
		Centered = 8192,
		HideSearchBar = 16384,
		CompactView = 32768,
		ListView = 65536,
		GridView = 131072,
		TableView = 262144,
		EnableSearchQuery = 524288,
		DisableInspectorPreview = 1048576,
		DisableSavedSearchQuery = 2097152,
		OpenInBuilderMode = 4194304,
		OpenInTextMode = 8388608,
		DisableBuilderModeToggle = 16777216,
		Borderless = 33554432,
		DisableQueryHelpers = 67108864,
		DisableNoResultTips = 134217728,
		IgnoreSavedSearches = 268435456,
		ObjectPicker = 536870912,
		ObjectPickerAdvancedUI = 1073741824,
		ContextSwitchPreservedMask = 33560576
	}
}
