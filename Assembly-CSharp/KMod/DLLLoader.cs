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

		public static bool LoadDLLs(string harmonyId, string path)
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
							ListPool<MethodInfo, Manager>.PooledList pooledList = ListPool<MethodInfo, Manager>.Allocate();
							ListPool<MethodInfo, Manager>.PooledList pooledList2 = ListPool<MethodInfo, Manager>.Allocate();
							ListPool<MethodInfo, Manager>.PooledList pooledList3 = ListPool<MethodInfo, Manager>.Allocate();
							ListPool<MethodInfo, Manager>.PooledList pooledList4 = ListPool<MethodInfo, Manager>.Allocate();
							Type[] array = new Type[0];
							Type[] array2 = new Type[] { typeof(string) };
							Type[] array3 = new Type[] { typeof(HarmonyInstance) };
							foreach (Assembly assembly2 in list)
							{
								foreach (Type type in assembly2.GetTypes())
								{
									if (!(type == null))
									{
										MethodInfo methodInfo = type.GetMethod("OnLoad", array);
										if (methodInfo != null)
										{
											pooledList3.Add(methodInfo);
										}
										methodInfo = type.GetMethod("OnLoad", array2);
										if (methodInfo != null)
										{
											pooledList4.Add(methodInfo);
										}
										methodInfo = type.GetMethod("PrePatch", array3);
										if (methodInfo != null)
										{
											pooledList.Add(methodInfo);
										}
										methodInfo = type.GetMethod("PostPatch", array3);
										if (methodInfo != null)
										{
											pooledList2.Add(methodInfo);
										}
									}
								}
							}
							HarmonyInstance harmonyInstance = HarmonyInstance.Create(harmonyId);
							if (harmonyInstance != null)
							{
								object[] array4 = new object[] { harmonyInstance };
								foreach (MethodInfo methodInfo2 in pooledList)
								{
									methodInfo2.Invoke(null, array4);
								}
								foreach (Assembly assembly3 in list)
								{
									harmonyInstance.PatchAll(assembly3);
								}
								foreach (MethodInfo methodInfo3 in pooledList2)
								{
									methodInfo3.Invoke(null, array4);
								}
							}
							pooledList.Recycle();
							pooledList2.Recycle();
							foreach (MethodInfo methodInfo4 in pooledList3)
							{
								methodInfo4.Invoke(null, null);
							}
							object[] array5 = new object[] { path };
							foreach (MethodInfo methodInfo5 in pooledList4)
							{
								methodInfo5.Invoke(null, array5);
							}
							pooledList3.Recycle();
							pooledList4.Recycle();
							flag = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				DebugUtil.LogException(null, string.Concat(new string[] { "Exception while loading mod ", harmonyId, " at ", path, "." }), ex);
				flag = false;
			}
			return flag;
		}

		private const string managed_path = "Managed";
	}
}
