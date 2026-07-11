using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace KMod
{
	internal static class DLLLoader
	{
		public static bool LoadUserModLoaderDLL()
		{
			try
			{
				string text = Path.Combine(Path.Combine(Application.dataPath, "Managed"), "ModLoader.dll");
				if (!File.Exists(text))
				{
					return false;
				}
				Assembly assembly = Assembly.LoadFile(text);
				if (assembly == null)
				{
					return false;
				}
				Type type = assembly.GetType("ModLoader.ModLoader");
				if (type == null)
				{
					return false;
				}
				MethodInfo method = type.GetMethod("Start");
				if (method == null)
				{
					return false;
				}
				method.Invoke(null, null);
				global::Debug.Log("Successfully started ModLoader.dll");
				return true;
			}
			catch (Exception ex)
			{
				global::Debug.Log(ex.ToString());
			}
			return false;
		}

		public static bool LoadDLLs(string path)
		{
			bool flag;
			try
			{
				if (Testing.dll_loading == Testing.DLLLoading.Fail)
				{
					flag = false;
				}
				else if (Testing.dll_loading == Testing.DLLLoading.UseModLoaderDLLExclusively)
				{
					flag = false;
				}
				else
				{
					global::Debug.LogFormat("Using built-in mod system...looking for DLL mods in {0}", new object[] { Manager.GetDirectory() });
					DirectoryInfo directoryInfo = new DirectoryInfo(path);
					if (!directoryInfo.Exists)
					{
						flag = false;
					}
					else
					{
						List<Assembly> list = new List<Assembly>();
						foreach (FileInfo fileInfo in directoryInfo.GetFiles())
						{
							if (fileInfo.Name.ToLower().EndsWith(".dll"))
							{
								global::Debug.Log(string.Format("Loading MOD dll: {0}", fileInfo.Name));
								Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);
								if (assembly != null)
								{
									list.Add(assembly);
								}
							}
						}
						if (list.Count == 0)
						{
							flag = false;
						}
						else
						{
							HarmonyInstance harmonyInstance = HarmonyInstance.Create(string.Format("OxygenNotIncluded_v{0}.{1}", 0, 1));
							if (harmonyInstance != null)
							{
								foreach (Assembly assembly2 in list)
								{
									harmonyInstance.PatchAll(assembly2);
								}
							}
							Type[] array = new Type[0];
							Type[] array2 = new Type[] { typeof(string) };
							object[] array3 = new object[] { path };
							foreach (Assembly assembly3 in list)
							{
								foreach (Type type in assembly3.GetTypes())
								{
									if (type != null)
									{
										MethodInfo methodInfo = type.GetMethod("OnLoad", array);
										if (methodInfo != null)
										{
											methodInfo.Invoke(null, null);
										}
										methodInfo = type.GetMethod("OnLoad", array2);
										if (methodInfo != null)
										{
											methodInfo.Invoke(null, array3);
										}
									}
								}
							}
							flag = true;
						}
					}
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		private const string managed_path = "Managed";
	}
}
