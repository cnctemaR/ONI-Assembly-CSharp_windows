using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarmonyLib
{
	public static class HarmonyMethodExtensions
	{
		internal static void SetValue(Traverse trv, string name, object val)
		{
			if (val == null)
			{
				return;
			}
			Traverse traverse = trv.Field(name);
			if (name == "methodType" || name == "reversePatchType")
			{
				val = Enum.ToObject(Nullable.GetUnderlyingType(traverse.GetValueType()), (int)val);
			}
			traverse.SetValue(val);
		}

		public static void CopyTo(this HarmonyMethod from, HarmonyMethod to)
		{
			if (to == null)
			{
				return;
			}
			Traverse fromTrv = Traverse.Create(from);
			Traverse toTrv = Traverse.Create(to);
			HarmonyMethod.HarmonyFields().ForEach(delegate(string f)
			{
				object value = fromTrv.Field(f).GetValue();
				if (value != null)
				{
					HarmonyMethodExtensions.SetValue(toTrv, f, value);
				}
			});
		}

		public static HarmonyMethod Clone(this HarmonyMethod original)
		{
			HarmonyMethod harmonyMethod = new HarmonyMethod();
			original.CopyTo(harmonyMethod);
			return harmonyMethod;
		}

		public static HarmonyMethod Merge(this HarmonyMethod master, HarmonyMethod detail)
		{
			if (detail == null)
			{
				return master;
			}
			HarmonyMethod harmonyMethod = new HarmonyMethod();
			Traverse resultTrv = Traverse.Create(harmonyMethod);
			Traverse masterTrv = Traverse.Create(master);
			Traverse detailTrv = Traverse.Create(detail);
			HarmonyMethod.HarmonyFields().ForEach(delegate(string f)
			{
				object value = masterTrv.Field(f).GetValue();
				object value2 = detailTrv.Field(f).GetValue();
				if (f != "priority" || (int)value2 != -1)
				{
					HarmonyMethodExtensions.SetValue(resultTrv, f, value2 ?? value);
				}
			});
			return harmonyMethod;
		}

		private static HarmonyMethod GetHarmonyMethodInfo(object attribute)
		{
			FieldInfo field = attribute.GetType().GetField("info", AccessTools.all);
			if (field == null)
			{
				return null;
			}
			if (field.FieldType.FullName != typeof(HarmonyMethod).FullName)
			{
				return null;
			}
			return AccessTools.MakeDeepCopy<HarmonyMethod>(field.GetValue(attribute));
		}

		public static List<HarmonyMethod> GetFromType(Type type)
		{
			return (from attr in type.GetCustomAttributes(true)
				select HarmonyMethodExtensions.GetHarmonyMethodInfo(attr) into info
				where info != null
				select info).ToList<HarmonyMethod>();
		}

		public static HarmonyMethod GetMergedFromType(Type type)
		{
			return HarmonyMethod.Merge(HarmonyMethodExtensions.GetFromType(type));
		}

		public static List<HarmonyMethod> GetFromMethod(MethodBase method)
		{
			return (from attr in method.GetCustomAttributes(true)
				select HarmonyMethodExtensions.GetHarmonyMethodInfo(attr) into info
				where info != null
				select info).ToList<HarmonyMethod>();
		}

		public static HarmonyMethod GetMergedFromMethod(MethodBase method)
		{
			return HarmonyMethod.Merge(HarmonyMethodExtensions.GetFromMethod(method));
		}
	}
}
