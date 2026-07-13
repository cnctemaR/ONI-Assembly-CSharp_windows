using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	public class HarmonyMethod
	{
		public HarmonyMethod()
		{
		}

		private void ImportMethod(MethodInfo theMethod)
		{
			this.method = theMethod;
			if (this.method != null)
			{
				List<HarmonyMethod> fromMethod = HarmonyMethodExtensions.GetFromMethod(this.method);
				if (fromMethod != null)
				{
					HarmonyMethod.Merge(fromMethod).CopyTo(this);
				}
			}
		}

		public HarmonyMethod(MethodInfo method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			this.ImportMethod(method);
		}

		public HarmonyMethod(Delegate @delegate)
			: this(@delegate.Method)
		{
		}

		public HarmonyMethod(MethodInfo method, int priority = -1, string[] before = null, string[] after = null, bool? debug = null)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			this.ImportMethod(method);
			this.priority = priority;
			this.before = before;
			this.after = after;
			this.debug = debug;
		}

		public HarmonyMethod(Delegate @delegate, int priority = -1, string[] before = null, string[] after = null, bool? debug = null)
			: this(@delegate.Method, priority, before, after, debug)
		{
		}

		public HarmonyMethod(Type methodType, string methodName, Type[] argumentTypes = null)
		{
			MethodInfo methodInfo = AccessTools.Method(methodType, methodName, argumentTypes, null);
			if (methodInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(58, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Cannot not find method for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(methodType);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(methodName);
				defaultInterpolatedStringHandler.AppendLiteral(" and parameters ");
				defaultInterpolatedStringHandler.AppendFormatted((argumentTypes != null) ? argumentTypes.Description() : null);
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
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
			if (attributes == null || attributes.Count == 0)
			{
				return harmonyMethod;
			}
			Traverse resultTrv = Traverse.Create(harmonyMethod);
			attributes.ForEach(delegate(HarmonyMethod attribute)
			{
				Traverse trv = Traverse.Create(attribute);
				HarmonyMethod.HarmonyFields().ForEach(delegate(string f)
				{
					object value = trv.Field(f).GetValue();
					if (value != null && (f != "priority" || (int)value != -1))
					{
						HarmonyMethodExtensions.SetValue(resultTrv, f, value);
					}
				});
			});
			return harmonyMethod;
		}

		public override string ToString()
		{
			string result = "";
			Traverse trv = Traverse.Create(this);
			HarmonyMethod.HarmonyFields().ForEach(delegate(string f)
			{
				if (result.Length > 0)
				{
					result += ", ";
				}
				string result2 = result;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted(f);
				defaultInterpolatedStringHandler.AppendLiteral("=");
				defaultInterpolatedStringHandler.AppendFormatted<object>(trv.Field(f).GetValue());
				result = result2 + defaultInterpolatedStringHandler.ToStringAndClear();
			});
			return "HarmonyMethod[" + result + "]";
		}

		internal string Description()
		{
			string text = ((this.declaringType != null) ? this.declaringType.FullName : "undefined");
			string text2 = this.methodName ?? "undefined";
			string text3 = ((this.methodType != null) ? this.methodType.Value.ToString() : "undefined");
			string text4 = ((this.argumentTypes != null) ? this.argumentTypes.Description() : "undefined");
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 4);
			defaultInterpolatedStringHandler.AppendLiteral("(class=");
			defaultInterpolatedStringHandler.AppendFormatted(text);
			defaultInterpolatedStringHandler.AppendLiteral(", methodname=");
			defaultInterpolatedStringHandler.AppendFormatted(text2);
			defaultInterpolatedStringHandler.AppendLiteral(", type=");
			defaultInterpolatedStringHandler.AppendFormatted(text3);
			defaultInterpolatedStringHandler.AppendLiteral(", args=");
			defaultInterpolatedStringHandler.AppendFormatted(text4);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		public static implicit operator HarmonyMethod(MethodInfo method)
		{
			return new HarmonyMethod(method);
		}

		public static implicit operator HarmonyMethod(Delegate @delegate)
		{
			return new HarmonyMethod(@delegate);
		}

		public MethodInfo method;

		public string category;

		public Type declaringType;

		public string methodName;

		public MethodType? methodType;

		public Type[] argumentTypes;

		public int priority = -1;

		public string[] before;

		public string[] after;

		public HarmonyReversePatchType? reversePatchType;

		public bool? debug;

		public bool nonVirtualDelegate;
	}
}
