using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace HarmonyLib
{
	internal static class HarmonySharedState
	{
		static HarmonySharedState()
		{
			Type orCreateSharedStateType = HarmonySharedState.GetOrCreateSharedStateType();
			FieldInfo field = orCreateSharedStateType.GetField("version");
			if ((int)field.GetValue(null) == 0)
			{
				field.SetValue(null, 102);
			}
			HarmonySharedState.actualVersion = (int)field.GetValue(null);
			FieldInfo field2 = orCreateSharedStateType.GetField("state");
			if (field2.GetValue(null) == null)
			{
				field2.SetValue(null, new Dictionary<MethodBase, byte[]>());
			}
			FieldInfo field3 = orCreateSharedStateType.GetField("originals");
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
			HarmonySharedState.methodStarts = new Dictionary<long, MethodInfo>();
			HarmonySharedState.RefreshMethodStarts();
			DetourHelper.Runtime.OnMethodCompiled += delegate(MethodBase method, IntPtr codeStart, ulong codeLen)
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
				HarmonySharedState.methodStartsInvalidated = true;
			};
		}

		private static void RefreshMethodStarts()
		{
			Dictionary<MethodInfo, MethodBase> dictionary = HarmonySharedState.originals;
			lock (dictionary)
			{
				HarmonySharedState.methodStarts.Clear();
				foreach (MethodInfo methodInfo in HarmonySharedState.originals.Keys)
				{
					HarmonySharedState.methodStarts.Add(methodInfo.GetNativeStart().ToInt64(), methodInfo);
				}
			}
			HarmonySharedState.methodStartsInvalidated = false;
		}

		private static Type GetOrCreateSharedStateType()
		{
			Type type = Type.GetType("HarmonySharedState", false);
			if (type != null)
			{
				return type;
			}
			Type type2;
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
				type2 = ReflectionHelper.Load(moduleDefinition).GetType("HarmonySharedState");
			}
			return type2;
		}

		internal static PatchInfo GetPatchInfo(MethodBase method)
		{
			Dictionary<MethodBase, byte[]> dictionary = HarmonySharedState.state;
			byte[] valueSafe;
			lock (dictionary)
			{
				valueSafe = HarmonySharedState.state.GetValueSafe(method);
			}
			if (valueSafe == null)
			{
				return null;
			}
			return PatchInfoSerialization.Deserialize(valueSafe);
		}

		internal static IEnumerable<MethodBase> GetPatchedMethods()
		{
			Dictionary<MethodBase, byte[]> dictionary = HarmonySharedState.state;
			IEnumerable<MethodBase> enumerable;
			lock (dictionary)
			{
				enumerable = HarmonySharedState.state.Keys.ToArray<MethodBase>();
			}
			return enumerable;
		}

		internal static void UpdatePatchInfo(MethodBase original, MethodInfo replacement, PatchInfo patchInfo)
		{
			byte[] array = patchInfo.Serialize();
			Dictionary<MethodBase, byte[]> dictionary = HarmonySharedState.state;
			lock (dictionary)
			{
				HarmonySharedState.state[original] = array;
			}
			Dictionary<MethodInfo, MethodBase> dictionary2 = HarmonySharedState.originals;
			lock (dictionary2)
			{
				HarmonySharedState.originals[replacement] = original;
			}
			Dictionary<long, MethodInfo> dictionary3 = HarmonySharedState.methodStarts;
			lock (dictionary3)
			{
				HarmonySharedState.methodStarts[replacement.GetNativeStart().ToInt64()] = replacement;
			}
		}

		internal static MethodBase GetOriginal(MethodInfo replacement)
		{
			Dictionary<MethodInfo, MethodBase> dictionary = HarmonySharedState.originals;
			MethodBase valueSafe;
			lock (dictionary)
			{
				valueSafe = HarmonySharedState.originals.GetValueSafe(replacement);
			}
			return valueSafe;
		}

		internal static MethodBase FindReplacement(StackFrame frame)
		{
			MethodBase method = frame.GetMethod();
			long num;
			if (method == null || method.IsGenericMethod)
			{
				if (HarmonySharedState.methodAddress == null)
				{
					return null;
				}
				num = (long)HarmonySharedState.methodAddress.GetValue(frame);
			}
			else
			{
				num = DetourHelper.Runtime.GetIdentifiable(method).GetNativeStart().ToInt64();
			}
			if (num == 0L)
			{
				return method;
			}
			Dictionary<long, MethodInfo> dictionary = HarmonySharedState.methodStarts;
			MethodBase methodBase;
			lock (dictionary)
			{
				if (HarmonySharedState.methodStartsInvalidated)
				{
					HarmonySharedState.RefreshMethodStarts();
				}
				MethodInfo methodInfo;
				methodBase = (HarmonySharedState.methodStarts.TryGetValue(num, out methodInfo) ? methodInfo : method);
			}
			return methodBase;
		}

		private const string name = "HarmonySharedState";

		internal const int internalVersion = 102;

		private static readonly Dictionary<MethodBase, byte[]> state;

		private static readonly Dictionary<MethodInfo, MethodBase> originals;

		private static readonly Dictionary<long, MethodInfo> methodStarts;

		private static bool methodStartsInvalidated;

		internal static readonly int actualVersion;

		private static readonly FieldInfo methodAddress = typeof(StackFrame).GetField("methodAddress", BindingFlags.Instance | BindingFlags.NonPublic);
	}
}
