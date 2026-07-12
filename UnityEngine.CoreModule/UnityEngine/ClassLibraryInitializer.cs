using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32.SafeHandles;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal static class ClassLibraryInitializer
	{
		[RequiredByNativeCode]
		private static void Init()
		{
			UnityLogWriter.Init();
		}

		[RequiredByNativeCode]
		private static void InitStdErrWithHandle(IntPtr fileHandle)
		{
			SafeFileHandle safeFileHandle = new SafeFileHandle(fileHandle, false);
			bool flag = !safeFileHandle.IsInvalid;
			if (flag)
			{
				StreamWriter streamWriter = new StreamWriter(new FileStream(safeFileHandle, FileAccess.Write))
				{
					AutoFlush = true
				};
				Console.SetError(streamWriter);
			}
		}

		[RequiredByNativeCode]
		private static void InitAssemblyRedirections()
		{
			AppDomain.CurrentDomain.AssemblyResolve += delegate(object _, ResolveEventArgs args)
			{
				AssemblyName assemblyName = new AssemblyName(args.Name);
				Assembly assembly2;
				try
				{
					Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName.Name);
					assembly2 = assembly;
				}
				catch
				{
					assembly2 = null;
				}
				return assembly2;
			};
		}
	}
}
