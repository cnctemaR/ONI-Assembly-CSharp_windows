using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony
{
	public static class HarmonySharedState
	{
		private static Dictionary<MethodBase, byte[]> GetState()
		{
			string text = HarmonySharedState.name;
			Dictionary<MethodBase, byte[]> dictionary;
			lock (text)
			{
				Assembly assembly = HarmonySharedState.SharedStateAssembly();
				bool flag = assembly == null;
				if (flag)
				{
					AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(HarmonySharedState.name), AssemblyBuilderAccess.Run);
					ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(HarmonySharedState.name);
					TypeAttributes typeAttributes = TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed;
					TypeBuilder typeBuilder = moduleBuilder.DefineType(HarmonySharedState.name, typeAttributes);
					typeBuilder.DefineField("state", typeof(Dictionary<MethodBase, byte[]>), FieldAttributes.FamANDAssem | FieldAttributes.Family | FieldAttributes.Static);
					typeBuilder.DefineField("version", typeof(int), FieldAttributes.FamANDAssem | FieldAttributes.Family | FieldAttributes.Static).SetConstant(HarmonySharedState.internalVersion);
					typeBuilder.CreateType();
					assembly = HarmonySharedState.SharedStateAssembly();
					bool flag2 = assembly == null;
					if (flag2)
					{
						throw new Exception("Cannot find or create harmony shared state");
					}
				}
				FieldInfo field = assembly.GetType(HarmonySharedState.name).GetField("version");
				bool flag3 = field == null;
				if (flag3)
				{
					throw new Exception("Cannot find harmony state version field");
				}
				HarmonySharedState.actualVersion = (int)field.GetValue(null);
				FieldInfo field2 = assembly.GetType(HarmonySharedState.name).GetField("state");
				bool flag4 = field2 == null;
				if (flag4)
				{
					throw new Exception("Cannot find harmony state field");
				}
				bool flag5 = field2.GetValue(null) == null;
				if (flag5)
				{
					field2.SetValue(null, new Dictionary<MethodBase, byte[]>());
				}
				dictionary = (Dictionary<MethodBase, byte[]>)field2.GetValue(null);
			}
			return dictionary;
		}

		private static Assembly SharedStateAssembly()
		{
			return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault<Assembly>((Assembly a) => a.GetName().Name.Contains(HarmonySharedState.name));
		}

		internal static PatchInfo GetPatchInfo(MethodBase method)
		{
			byte[] valueSafe = HarmonySharedState.GetState().GetValueSafe(method);
			bool flag = valueSafe == null;
			PatchInfo patchInfo;
			if (flag)
			{
				patchInfo = null;
			}
			else
			{
				patchInfo = PatchInfoSerialization.Deserialize(valueSafe);
			}
			return patchInfo;
		}

		internal static IEnumerable<MethodBase> GetPatchedMethods()
		{
			return HarmonySharedState.GetState().Keys.AsEnumerable<MethodBase>();
		}

		internal static void UpdatePatchInfo(MethodBase method, PatchInfo patchInfo)
		{
			HarmonySharedState.GetState()[method] = patchInfo.Serialize();
		}

		private static readonly string name = "HarmonySharedState";

		internal static readonly int internalVersion = 100;

		internal static int actualVersion = -1;
	}
}
