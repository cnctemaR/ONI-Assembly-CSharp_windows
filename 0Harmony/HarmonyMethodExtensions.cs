using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony
{
	public static class HarmonyMethodExtensions
	{
		public static void CopyTo(this HarmonyMethod from, HarmonyMethod to)
		{
			bool flag = to == null;
			if (!flag)
			{
				Traverse fromTrv = Traverse.Create(from);
				Traverse toTrv = Traverse.Create(to);
				HarmonyMethod.HarmonyFields().ForEach(delegate(string f)
				{
					object value = fromTrv.Field(f).GetValue();
					bool flag2 = value != null;
					if (flag2)
					{
						toTrv.Field(f).SetValue(value);
					}
				});
			}
		}

		public static HarmonyMethod Clone(this HarmonyMethod original)
		{
			HarmonyMethod harmonyMethod = new HarmonyMethod();
			original.CopyTo(harmonyMethod);
			return harmonyMethod;
		}

		public static HarmonyMethod Merge(this HarmonyMethod master, HarmonyMethod detail)
		{
			bool flag = detail == null;
			HarmonyMethod harmonyMethod;
			if (flag)
			{
				harmonyMethod = master;
			}
			else
			{
				HarmonyMethod harmonyMethod2 = new HarmonyMethod();
				Traverse resultTrv = Traverse.Create(harmonyMethod2);
				Traverse masterTrv = Traverse.Create(master);
				Traverse detailTrv = Traverse.Create(detail);
				HarmonyMethod.HarmonyFields().ForEach(delegate(string f)
				{
					object value = masterTrv.Field(f).GetValue();
					object value2 = detailTrv.Field(f).GetValue();
					resultTrv.Field(f).SetValue(value2 ?? value);
				});
				harmonyMethod = harmonyMethod2;
			}
			return harmonyMethod;
		}

		public static List<HarmonyMethod> GetHarmonyMethods(this Type type)
		{
			return (from HarmonyAttribute attr in from attr in type.GetCustomAttributes(true)
					where attr is HarmonyAttribute
					select attr
				select attr.info).ToList<HarmonyMethod>();
		}

		public static List<HarmonyMethod> GetHarmonyMethods(this MethodBase method)
		{
			bool flag = method is DynamicMethod;
			List<HarmonyMethod> list;
			if (flag)
			{
				list = new List<HarmonyMethod>();
			}
			else
			{
				list = (from HarmonyAttribute attr in from attr in method.GetCustomAttributes(true)
						where attr is HarmonyAttribute
						select attr
					select attr.info).ToList<HarmonyMethod>();
			}
			return list;
		}
	}
}
