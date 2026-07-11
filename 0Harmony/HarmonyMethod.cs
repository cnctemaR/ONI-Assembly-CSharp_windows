using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Harmony
{
	public class HarmonyMethod
	{
		public HarmonyMethod()
		{
		}

		private void ImportMethod(MethodInfo theMethod)
		{
			this.method = theMethod;
			bool flag = this.method != null;
			if (flag)
			{
				List<HarmonyMethod> harmonyMethods = this.method.GetHarmonyMethods();
				bool flag2 = harmonyMethods != null;
				if (flag2)
				{
					HarmonyMethod.Merge(harmonyMethods).CopyTo(this);
				}
			}
		}

		public HarmonyMethod(MethodInfo method)
		{
			this.ImportMethod(method);
		}

		public HarmonyMethod(Type type, string name, Type[] parameters = null)
		{
			MethodInfo methodInfo = AccessTools.Method(type, name, parameters, null);
			this.ImportMethod(methodInfo);
		}

		public static List<string> HarmonyFields()
		{
			return (from s in AccessTools.GetFieldNames(typeof(HarmonyMethod))
				where s != "method"
				select s).ToList<string>();
		}

		public static HarmonyMethod Merge(List<HarmonyMethod> attributes)
		{
			HarmonyMethod harmonyMethod = new HarmonyMethod();
			bool flag = attributes == null;
			HarmonyMethod harmonyMethod2;
			if (flag)
			{
				harmonyMethod2 = harmonyMethod;
			}
			else
			{
				Traverse resultTrv = Traverse.Create(harmonyMethod);
				attributes.ForEach(delegate(HarmonyMethod attribute)
				{
					Traverse trv = Traverse.Create(attribute);
					HarmonyMethod.HarmonyFields().ForEach(delegate(string f)
					{
						object value = trv.Field(f).GetValue();
						bool flag2 = value != null;
						if (flag2)
						{
							resultTrv.Field(f).SetValue(value);
						}
					});
				});
				harmonyMethod2 = harmonyMethod;
			}
			return harmonyMethod2;
		}

		public override string ToString()
		{
			string result = "HarmonyMethod[";
			Traverse trv = Traverse.Create(this);
			HarmonyMethod.HarmonyFields().ForEach(delegate(string f)
			{
				result = string.Concat(new object[]
				{
					result,
					f,
					"=",
					trv.Field(f).GetValue()
				});
			});
			return result + "]";
		}

		public MethodInfo method;

		public Type declaringType;

		public string methodName;

		public MethodType? methodType;

		public Type[] argumentTypes;

		public int prioritiy = -1;

		public string[] before;

		public string[] after;
	}
}
