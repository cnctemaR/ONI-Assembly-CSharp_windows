using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Mono.Cecil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace HarmonyLib
{
	internal static class HarmonySharedState
	{
		private static T WithState<T>(Func<T> action)
		{
			T t = default(T);
			bool flag = false;
			try
			{
				HarmonySharedState.mutex.WaitOne();
				flag = true;
				if (HarmonySharedState.state == null)
				{
					DetourHelper.Runtime.OnMethodCompiled += HarmonySharedState.OnCompileMethod;
					Type type = HarmonySharedState.CreateSharedStateType();
					FieldInfo field = type.GetField("version");
					if ((int)field.GetValue(null) == 0)
					{
						field.SetValue(null, 101);
					}
					HarmonySharedState.actualVersion = (int)field.GetValue(null);
					FieldInfo field2 = type.GetField("state");
					if (field2.GetValue(null) == null)
					{
						field2.SetValue(null, new Dictionary<MethodBase, byte[]>());
					}
					FieldInfo field3 = type.GetField("originals");
					if (field3 != null && field3.GetValue(null) == null)
					{
						field3.SetValue(null, new Dictionary<MethodInfo, MethodBase>());
					}
					HarmonySharedState.state = (Dictionary<MethodBase, byte[]>)field2.GetValue(null);
					HarmonySharedState.originals = new Dictionary<MethodInfo, MethodBase>();
					if (field3 != null)
					{
						HarmonySharedState.originals = (Dictionary<MethodInfo, MethodBase>)field3.GetValue(null);
					}
				}
				t = action();
			}
			finally
			{
				if (flag)
				{
					HarmonySharedState.mutex.ReleaseMutex();
				}
			}
			return t;
		}

		private static void OnCompileMethod(MethodBase method, IntPtr codeStart, ulong codeLen)
		{
			if (method == null)
			{
				return;
			}
			PatchInfo patchInfo = HarmonySharedState.GetPatchInfo(method);
			if (patchInfo == null)
			{
				return;
			}
			PatchFunctions.UpdateRecompiledMethod(method, codeStart, patchInfo);
		}

		private static Type CreateSharedStateType()
		{
			Type type;
			using (ModuleDefinition moduleDefinition = ModuleDefinition.CreateModule("HarmonySharedState", new ModuleParameters
			{
				Kind = ModuleKind.Dll,
				ReflectionImporterProvider = MMReflectionImporter.Provider
			}))
			{
				Mono.Cecil.TypeAttributes typeAttributes = Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Abstract | Mono.Cecil.TypeAttributes.Sealed;
				TypeDefinition typeDefinition = new TypeDefinition("", "HarmonySharedState", typeAttributes)
				{
					BaseType = moduleDefinition.TypeSystem.Object
				};
				moduleDefinition.Types.Add(typeDefinition);
				typeDefinition.Fields.Add(new FieldDefinition("state", Mono.Cecil.FieldAttributes.FamANDAssem | Mono.Cecil.FieldAttributes.Family | Mono.Cecil.FieldAttributes.Static, moduleDefinition.ImportReference(typeof(Dictionary<MethodBase, byte[]>))));
				typeDefinition.Fields.Add(new FieldDefinition("originals", Mono.Cecil.FieldAttributes.FamANDAssem | Mono.Cecil.FieldAttributes.Family | Mono.Cecil.FieldAttributes.Static, moduleDefinition.ImportReference(typeof(Dictionary<MethodInfo, MethodBase>))));
				typeDefinition.Fields.Add(new FieldDefinition("version", Mono.Cecil.FieldAttributes.FamANDAssem | Mono.Cecil.FieldAttributes.Family | Mono.Cecil.FieldAttributes.Static, moduleDefinition.ImportReference(typeof(int))));
				type = ReflectionHelper.Load(moduleDefinition).GetType("HarmonySharedState");
			}
			return type;
		}

		internal static PatchInfo GetPatchInfo(MethodBase method)
		{
			return HarmonySharedState.WithState<PatchInfo>(delegate
			{
				byte[] valueSafe = HarmonySharedState.state.GetValueSafe(method);
				if (valueSafe == null)
				{
					return null;
				}
				return PatchInfoSerialization.Deserialize(valueSafe);
			});
		}

		internal static IEnumerable<MethodBase> GetPatchedMethods()
		{
			return HarmonySharedState.WithState<MethodBase[]>(() => HarmonySharedState.state.Keys.ToArray<MethodBase>());
		}

		internal static void UpdatePatchInfo(MethodBase original, MethodInfo replacement, PatchInfo patchInfo)
		{
			byte[] bytes = patchInfo.Serialize();
			HarmonySharedState.WithState<object>(delegate
			{
				HarmonySharedState.state[original] = bytes;
				HarmonySharedState.originals[replacement] = original;
				return null;
			});
		}

		internal static MethodBase GetOriginal(MethodInfo replacement)
		{
			return HarmonySharedState.WithState<MethodBase>(() => HarmonySharedState.originals.GetValueSafe(replacement));
		}

		internal static MethodInfo FindReplacement(StackFrame frame)
		{
			FieldInfo fieldInfo = AccessTools.Field(typeof(StackFrame), "methodAddress");
			if (fieldInfo == null)
			{
				return null;
			}
			long framePtr = (long)fieldInfo.GetValue(frame);
			Func<MethodInfo, bool> <>9__1;
			return HarmonySharedState.WithState<MethodInfo>(delegate
			{
				IEnumerable<MethodInfo> keys = HarmonySharedState.originals.Keys;
				Func<MethodInfo, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (MethodInfo replacement) => replacement.GetNativeStart().ToInt64() == framePtr);
				}
				return keys.FirstOrDefault<MethodInfo>(func);
			});
		}

		private const string name = "HarmonySharedState";

		private static readonly Mutex mutex = new Mutex(false, "HarmonySharedState");

		private static Dictionary<MethodBase, byte[]> state = null;

		private static Dictionary<MethodInfo, MethodBase> originals = null;

		internal const int internalVersion = 101;

		internal static int actualVersion = -1;
	}
}
