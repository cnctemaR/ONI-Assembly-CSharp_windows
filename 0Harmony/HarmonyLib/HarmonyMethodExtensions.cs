using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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
				Type underlyingType = Nullable.GetUnderlyingType(traverse.GetValueType());
				val = Enum.ToObject(underlyingType, (int)val);
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
				if (f != "priority")
				{
					HarmonyMethodExtensions.SetValue(resultTrv, f, value2 ?? value);
					return;
				}
				int num = (int)value;
				int num2 = (int)value2;
				int num3 = Math.Max(num, num2);
				if (num == -1 && num2 != -1)
				{
					num3 = num2;
				}
				if (num != -1 && num2 == -1)
				{
					num3 = num;
				}
				HarmonyMethodExtensions.SetValue(resultTrv, f, num3);
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
			if (field.FieldType.FullName != PatchTools.harmonyMethodFullName)
			{
				return null;
			}
			object value = field.GetValue(attribute);
			return AccessTools.MakeDeepCopy<HarmonyMethod>(value);
		}

		public static List<HarmonyMethod> GetFromType(Type type)
		{
			IEnumerable<object> customAttributes = type.GetCustomAttributes(true);
			Func<object, HarmonyMethod> func;
			if ((func = HarmonyMethodExtensions.<>O.<0>__GetHarmonyMethodInfo) == null)
			{
				func = (HarmonyMethodExtensions.<>O.<0>__GetHarmonyMethodInfo = new Func<object, HarmonyMethod>(HarmonyMethodExtensions.GetHarmonyMethodInfo));
			}
			return (from info in customAttributes.Select<object, HarmonyMethod>(func)
				where info != null
				select info).ToList<HarmonyMethod>();
		}

		public static HarmonyMethod GetMergedFromType(Type type)
		{
			return HarmonyMethod.Merge(HarmonyMethodExtensions.GetFromType(type));
		}

		public static List<HarmonyMethod> GetFromMethod(MethodBase method)
		{
			IEnumerable<object> customAttributes = method.GetCustomAttributes(true);
			Func<object, HarmonyMethod> func;
			if ((func = HarmonyMethodExtensions.<>O.<0>__GetHarmonyMethodInfo) == null)
			{
				func = (HarmonyMethodExtensions.<>O.<0>__GetHarmonyMethodInfo = new Func<object, HarmonyMethod>(HarmonyMethodExtensions.GetHarmonyMethodInfo));
			}
			return (from info in customAttributes.Select<object, HarmonyMethod>(func)
				where info != null
				select info).ToList<HarmonyMethod>();
		}

		public static HarmonyMethod GetMergedFromMethod(MethodBase method)
		{
			return HarmonyMethod.Merge(HarmonyMethodExtensions.GetFromMethod(method));
		}

		[CompilerGenerated]
		private static class <>O
		{
			public static Func<object, HarmonyMethod> <0>__GetHarmonyMethodInfo;
		}
	}
}
