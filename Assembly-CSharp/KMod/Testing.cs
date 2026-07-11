using System;

namespace KMod
{
	public static class Testing
	{
		public static Testing.DLLLoading dll_loading;

		public const Testing.SaveLoad SAVE_LOAD = Testing.SaveLoad.NoTesting;

		public const Testing.Install INSTALL = Testing.Install.NoTesting;

		public const Testing.Boot BOOT = Testing.Boot.NoTesting;

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
