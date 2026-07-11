using System;

namespace KMod
{
	public static class Testing
	{
		public static Testing.DLLLoading dll_loading;

		public static Testing.SaveLoad save_load;

		public static Testing.Install install;

		public static Testing.Boot boot;

		public enum DLLLoading
		{
			NoTesting,
			Fail,
			UseModLoaderDLLExclusively
		}

		public enum SaveLoad
		{
			NoTesting,
			FailSave,
			FailLoad
		}

		public enum Install
		{
			NoTesting,
			ForceUninstall,
			ForceReinstall,
			ForceUpdate
		}

		public enum Boot
		{
			NoTesting,
			Crash
		}
	}
}
