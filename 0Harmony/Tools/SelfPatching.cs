using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Harmony.ILCopying;

namespace Harmony.Tools
{
	internal class SelfPatching
	{
		[UpgradeToLatestVersion(1)]
		private static int GetVersion(MethodBase method)
		{
			object obj = (from attr in method.GetCustomAttributes(false)
				where attr.GetType().FullName.GetHashCode() == SelfPatching.upgradeToLatestVersionFullNameHash
				select attr).FirstOrDefault<object>();
			bool flag = obj == null;
			int num;
			if (flag)
			{
				num = -1;
			}
			else
			{
				num = Traverse.Create(obj).Field("version").GetValue<int>();
			}
			return num;
		}

		[UpgradeToLatestVersion(1)]
		private static string MethodKey(MethodBase method)
		{
			return method.FullDescription();
		}

		[UpgradeToLatestVersion(1)]
		private static bool IsHarmonyAssembly(Assembly assembly)
		{
			bool flag;
			try
			{
				flag = !assembly.ReflectionOnly && assembly.GetType(typeof(HarmonyInstance).FullName) != null;
			}
			catch (Exception)
			{
				flag = false;
			}
			return flag;
		}

		private static List<MethodBase> GetAllMethods(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			return (from method in types.SelectMany<Type, MethodBase>((Type type) => type.GetMethods(AccessTools.all).Cast<MethodBase>()).Concat<MethodBase>(types.SelectMany<Type, ConstructorInfo>((Type type) => type.GetConstructors(AccessTools.all)).Cast<MethodBase>()).Concat<MethodBase>((from prop in types.SelectMany<Type, PropertyInfo>((Type type) => type.GetProperties(AccessTools.all))
					select prop.GetGetMethod()).Cast<MethodBase>())
					.Concat<MethodBase>((from prop in types.SelectMany<Type, PropertyInfo>((Type type) => type.GetProperties(AccessTools.all))
						select prop.GetSetMethod()).Cast<MethodBase>())
				where method != null && method.DeclaringType.Assembly == assembly
				orderby method.FullDescription()
				select method).ToList<MethodBase>();
		}

		private static string AssemblyInfo(Assembly assembly)
		{
			Version version = assembly.GetName().Version;
			string text = assembly.Location;
			bool flag = text == null || text == "";
			if (flag)
			{
				text = new Uri(assembly.CodeBase).LocalPath;
			}
			return string.Concat(new object[]
			{
				text,
				"(v",
				version,
				assembly.GlobalAssemblyCache ? ", cached" : "",
				")"
			});
		}

		[UpgradeToLatestVersion(1)]
		public static void PatchOldHarmonyMethods()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Assembly ourAssembly = new StackTrace(true).GetFrame(1).GetMethod().DeclaringType.Assembly;
			bool debug = HarmonyInstance.DEBUG;
			if (debug)
			{
				Version version = ourAssembly.GetName().Version;
				Version version2 = typeof(SelfPatching).Assembly.GetName().Version;
				bool flag = version2 > version;
				if (flag)
				{
					FileLog.Log("### Harmony v" + version + " started");
					FileLog.Log("### Self-patching unnecessary because we are already patched by v" + version2);
					FileLog.Log("### At " + DateTime.Now.ToString("yyyy-MM-dd hh.mm.ss"));
					return;
				}
				FileLog.Log("Self-patching started (v" + version + ")");
			}
			Dictionary<string, MethodBase> potentialMethodsToUpgrade = new Dictionary<string, MethodBase>();
			SelfPatching.GetAllMethods(ourAssembly).Where<MethodBase>(delegate(MethodBase method)
			{
				bool flag4;
				if (method != null)
				{
					flag4 = method.GetCustomAttributes(false).Any<object>((object attr) => attr is UpgradeToLatestVersion);
				}
				else
				{
					flag4 = false;
				}
				return flag4;
			}).Do<MethodBase>(delegate(MethodBase method)
			{
				potentialMethodsToUpgrade.Add(SelfPatching.MethodKey(method), method);
			});
			List<Assembly> list = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
				where SelfPatching.IsHarmonyAssembly(assembly) && assembly != ourAssembly
				select assembly).ToList<Assembly>();
			bool debug2 = HarmonyInstance.DEBUG;
			if (debug2)
			{
				list.Do<Assembly>(delegate(Assembly assembly)
				{
					FileLog.Log("Found Harmony " + SelfPatching.AssemblyInfo(assembly));
				});
				FileLog.Log("Potential methods to upgrade:");
				potentialMethodsToUpgrade.Values.OrderBy<MethodBase, string>((MethodBase method) => method.FullDescription()).Do<MethodBase>(delegate(MethodBase method)
				{
					FileLog.Log("- " + method.FullDescription());
				});
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (Assembly assembly2 in list)
			{
				foreach (MethodBase methodBase in SelfPatching.GetAllMethods(assembly2))
				{
					num++;
					MethodBase methodBase2;
					bool flag2 = potentialMethodsToUpgrade.TryGetValue(SelfPatching.MethodKey(methodBase), out methodBase2);
					if (flag2)
					{
						int version3 = SelfPatching.GetVersion(methodBase2);
						num2++;
						int version4 = SelfPatching.GetVersion(methodBase);
						bool flag3 = version4 < version3;
						if (flag3)
						{
							bool debug3 = HarmonyInstance.DEBUG;
							if (debug3)
							{
								FileLog.Log("Self-patching " + methodBase.FullDescription() + " in " + SelfPatching.AssemblyInfo(assembly2));
							}
							num3++;
							Memory.DetourMethod(methodBase, methodBase2);
						}
					}
				}
			}
			bool debug4 = HarmonyInstance.DEBUG;
			if (debug4)
			{
				FileLog.Log(string.Concat(new object[]
				{
					"Self-patched ",
					num3,
					" out of ",
					num,
					" methods (",
					num2 - num3,
					" skipped) in ",
					stopwatch.ElapsedMilliseconds,
					"ms"
				}));
			}
		}

		private static readonly int upgradeToLatestVersionFullNameHash = typeof(UpgradeToLatestVersion).FullName.GetHashCode();
	}
}
