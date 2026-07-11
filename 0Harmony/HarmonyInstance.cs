using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony.Tools;

namespace Harmony
{
	public class HarmonyInstance
	{
		public string Id
		{
			get
			{
				return this.id;
			}
		}

		private HarmonyInstance(string id)
		{
			bool debug = HarmonyInstance.DEBUG;
			if (debug)
			{
				Assembly assembly = typeof(HarmonyInstance).Assembly;
				Version version = assembly.GetName().Version;
				string text = assembly.Location;
				bool flag = text == null || text == "";
				if (flag)
				{
					text = new Uri(assembly.CodeBase).LocalPath;
				}
				FileLog.Log(string.Concat(new object[] { "### Harmony id=", id, ", version=", version, ", location=", text }));
				MethodBase outsideCaller = this.GetOutsideCaller();
				Assembly assembly2 = outsideCaller.DeclaringType.Assembly;
				text = assembly2.Location;
				bool flag2 = text == null || text == "";
				if (flag2)
				{
					text = new Uri(assembly2.CodeBase).LocalPath;
				}
				FileLog.Log("### Started from " + outsideCaller.FullDescription() + ", location " + text);
				FileLog.Log("### At " + DateTime.Now.ToString("yyyy-MM-dd hh.mm.ss"));
			}
			this.id = id;
			bool flag3 = !HarmonyInstance.selfPatchingDone;
			if (flag3)
			{
				HarmonyInstance.selfPatchingDone = true;
				SelfPatching.PatchOldHarmonyMethods();
			}
		}

		public static HarmonyInstance Create(string id)
		{
			bool flag = id == null;
			if (flag)
			{
				throw new Exception("id cannot be null");
			}
			return new HarmonyInstance(id);
		}

		private MethodBase GetOutsideCaller()
		{
			StackTrace stackTrace = new StackTrace(true);
			foreach (StackFrame stackFrame in stackTrace.GetFrames())
			{
				MethodBase method = stackFrame.GetMethod();
				bool flag = method.DeclaringType.Namespace != typeof(HarmonyInstance).Namespace;
				if (flag)
				{
					return method;
				}
			}
			throw new Exception("Unexpected end of stack trace");
		}

		public void PatchAll()
		{
			MethodBase method = new StackTrace().GetFrame(1).GetMethod();
			Assembly assembly = method.ReflectedType.Assembly;
			this.PatchAll(assembly);
		}

		public void PatchAll(Assembly assembly)
		{
			assembly.GetTypes().Do<Type>(delegate(Type type)
			{
				List<HarmonyMethod> harmonyMethods = type.GetHarmonyMethods();
				bool flag = harmonyMethods != null && harmonyMethods.Count<HarmonyMethod>() > 0;
				if (flag)
				{
					HarmonyMethod harmonyMethod = HarmonyMethod.Merge(harmonyMethods);
					PatchProcessor patchProcessor = new PatchProcessor(this, type, harmonyMethod);
					patchProcessor.Patch();
				}
			});
		}

		public DynamicMethod Patch(MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
		{
			PatchProcessor patchProcessor = new PatchProcessor(this, new List<MethodBase> { original }, prefix, postfix, transpiler);
			return patchProcessor.Patch().FirstOrDefault<DynamicMethod>();
		}

		public void UnpatchAll(string harmonyID = null)
		{
			HarmonyInstance.<>c__DisplayClass11_0 CS$<>8__locals1 = new HarmonyInstance.<>c__DisplayClass11_0();
			CS$<>8__locals1.harmonyID = harmonyID;
			CS$<>8__locals1.<>4__this = this;
			List<MethodBase> list = this.GetPatchedMethods().ToList<MethodBase>();
			using (List<MethodBase>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MethodBase original = enumerator.Current;
					Patches patchInfo2 = this.GetPatchInfo(original);
					patchInfo2.Prefixes.DoIf<Patch>(new Func<Patch, bool>(CS$<>8__locals1.<UnpatchAll>g__IDCheck|0), delegate(Patch patchInfo)
					{
						CS$<>8__locals1.<>4__this.Unpatch(original, patchInfo.patch);
					});
					patchInfo2.Postfixes.DoIf<Patch>(new Func<Patch, bool>(CS$<>8__locals1.<UnpatchAll>g__IDCheck|0), delegate(Patch patchInfo)
					{
						CS$<>8__locals1.<>4__this.Unpatch(original, patchInfo.patch);
					});
					patchInfo2.Transpilers.DoIf<Patch>(new Func<Patch, bool>(CS$<>8__locals1.<UnpatchAll>g__IDCheck|0), delegate(Patch patchInfo)
					{
						CS$<>8__locals1.<>4__this.Unpatch(original, patchInfo.patch);
					});
				}
			}
		}

		public void Unpatch(MethodBase original, HarmonyPatchType type, string harmonyID = null)
		{
			PatchProcessor patchProcessor = new PatchProcessor(this, new List<MethodBase> { original }, null, null, null);
			patchProcessor.Unpatch(type, harmonyID);
		}

		public void Unpatch(MethodBase original, MethodInfo patch)
		{
			PatchProcessor patchProcessor = new PatchProcessor(this, new List<MethodBase> { original }, null, null, null);
			patchProcessor.Unpatch(patch);
		}

		public bool HasAnyPatches(string harmonyID)
		{
			return (from original in this.GetPatchedMethods()
				select this.GetPatchInfo(original)).Any<Patches>((Patches info) => info.Owners.Contains(harmonyID));
		}

		public Patches GetPatchInfo(MethodBase method)
		{
			return PatchProcessor.GetPatchInfo(method);
		}

		public IEnumerable<MethodBase> GetPatchedMethods()
		{
			return HarmonySharedState.GetPatchedMethods();
		}

		public Dictionary<string, Version> VersionInfo(out Version currentVersion)
		{
			currentVersion = typeof(HarmonyInstance).Assembly.GetName().Version;
			Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
			Action<Patch> <>9__2;
			Action<Patch> <>9__3;
			Action<Patch> <>9__4;
			this.GetPatchedMethods().Do<MethodBase>(delegate(MethodBase method)
			{
				PatchInfo patchInfo = HarmonySharedState.GetPatchInfo(method);
				IEnumerable<Patch> prefixes = patchInfo.prefixes;
				Action<Patch> action;
				if ((action = <>9__2) == null)
				{
					action = (<>9__2 = delegate(Patch fix)
					{
						assemblies[fix.owner] = fix.patch.DeclaringType.Assembly;
					});
				}
				prefixes.Do<Patch>(action);
				IEnumerable<Patch> postfixes = patchInfo.postfixes;
				Action<Patch> action2;
				if ((action2 = <>9__3) == null)
				{
					action2 = (<>9__3 = delegate(Patch fix)
					{
						assemblies[fix.owner] = fix.patch.DeclaringType.Assembly;
					});
				}
				postfixes.Do<Patch>(action2);
				IEnumerable<Patch> transpilers = patchInfo.transpilers;
				Action<Patch> action3;
				if ((action3 = <>9__4) == null)
				{
					action3 = (<>9__4 = delegate(Patch fix)
					{
						assemblies[fix.owner] = fix.patch.DeclaringType.Assembly;
					});
				}
				transpilers.Do<Patch>(action3);
			});
			Dictionary<string, Version> result = new Dictionary<string, Version>();
			assemblies.Do<KeyValuePair<string, Assembly>>(delegate(KeyValuePair<string, Assembly> info)
			{
				AssemblyName assemblyName = info.Value.GetReferencedAssemblies().FirstOrDefault<AssemblyName>((AssemblyName a) => a.FullName.StartsWith("0Harmony, Version"));
				bool flag = assemblyName != null;
				if (flag)
				{
					result[info.Key] = assemblyName.Version;
				}
			});
			return result;
		}

		private readonly string id;

		public static bool DEBUG = false;

		private static bool selfPatchingDone = false;
	}
}
