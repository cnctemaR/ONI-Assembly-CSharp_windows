using System;
using System.IO;
using Mono.Security.X509;

namespace Mono.Btls
{
	internal static class MonoBtlsX509StoreManager
	{
		private static void Initialize()
		{
			if (MonoBtlsX509StoreManager.initialized)
			{
				return;
			}
			try
			{
				MonoBtlsX509StoreManager.DoInitialize();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("MonoBtlsX509StoreManager.Initialize() threw exception: {0}", ex);
			}
			finally
			{
				MonoBtlsX509StoreManager.initialized = true;
			}
		}

		private static void DoInitialize()
		{
			string newCurrentUserPath = X509StoreManager.NewCurrentUserPath;
			MonoBtlsX509StoreManager.userTrustedRootPath = Path.Combine(newCurrentUserPath, "Trust");
			MonoBtlsX509StoreManager.userIntermediateCAPath = Path.Combine(newCurrentUserPath, "CA");
			MonoBtlsX509StoreManager.userUntrustedPath = Path.Combine(newCurrentUserPath, "Disallowed");
			string newLocalMachinePath = X509StoreManager.NewLocalMachinePath;
			MonoBtlsX509StoreManager.machineTrustedRootPath = Path.Combine(newLocalMachinePath, "Trust");
			MonoBtlsX509StoreManager.machineIntermediateCAPath = Path.Combine(newLocalMachinePath, "CA");
			MonoBtlsX509StoreManager.machineUntrustedPath = Path.Combine(newLocalMachinePath, "Disallowed");
		}

		public static bool HasStore(MonoBtlsX509StoreType type)
		{
			string storePath = MonoBtlsX509StoreManager.GetStorePath(type);
			return storePath != null && Directory.Exists(storePath);
		}

		public static string GetStorePath(MonoBtlsX509StoreType type)
		{
			MonoBtlsX509StoreManager.Initialize();
			switch (type)
			{
			case MonoBtlsX509StoreType.MachineTrustedRoots:
				return MonoBtlsX509StoreManager.machineTrustedRootPath;
			case MonoBtlsX509StoreType.MachineIntermediateCA:
				return MonoBtlsX509StoreManager.machineIntermediateCAPath;
			case MonoBtlsX509StoreType.MachineUntrusted:
				return MonoBtlsX509StoreManager.machineUntrustedPath;
			case MonoBtlsX509StoreType.UserTrustedRoots:
				return MonoBtlsX509StoreManager.userTrustedRootPath;
			case MonoBtlsX509StoreType.UserIntermediateCA:
				return MonoBtlsX509StoreManager.userIntermediateCAPath;
			case MonoBtlsX509StoreType.UserUntrusted:
				return MonoBtlsX509StoreManager.userUntrustedPath;
			default:
				throw new NotSupportedException();
			}
		}

		private static bool initialized;

		private static string machineTrustedRootPath;

		private static string machineIntermediateCAPath;

		private static string machineUntrustedPath;

		private static string userTrustedRootPath;

		private static string userIntermediateCAPath;

		private static string userUntrustedPath;
	}
}
