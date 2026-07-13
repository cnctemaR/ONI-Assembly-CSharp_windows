using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class Switches
	{
		static Switches()
		{
			Type type = Switches.tAppContext;
			Switches.miTryGetSwitch = ((type != null) ? type.GetMethod("TryGetSwitch", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
			{
				typeof(string),
				typeof(bool).MakeByRefType()
			}, null) : null);
			MethodInfo methodInfo = Switches.miTryGetSwitch;
			Switches.dTryGetSwitch = ((methodInfo != null) ? methodInfo.TryCreateDelegate<Switches.TryGetSwitchFunc>() : null);
			foreach (object obj in Environment.GetEnvironmentVariables())
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				string text = (string)dictionaryEntry.Key;
				if (text.StartsWith("MONOMOD_", StringComparison.Ordinal) && dictionaryEntry.Value != null)
				{
					string text2 = text.Substring("MONOMOD_".Length);
					Switches.switchValues.TryAdd(text2, Switches.BestEffortParseEnvVar((string)dictionaryEntry.Value));
				}
			}
		}

		[return: Nullable(2)]
		private static object BestEffortParseEnvVar(string value)
		{
			if (value.Length == 0)
			{
				return null;
			}
			int num;
			if (int.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out num))
			{
				return num;
			}
			long num2;
			if (long.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out num2))
			{
				return num2;
			}
			if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
			{
				return num;
			}
			if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
			{
				return num2;
			}
			char c = value[0];
			if (c <= 'Y')
			{
				if (c <= 'N')
				{
					if (c != 'F' && c != 'N')
					{
						goto IL_00B7;
					}
				}
				else if (c != 'T' && c != 'Y')
				{
					goto IL_00B7;
				}
			}
			else if (c <= 'n')
			{
				if (c != 'f' && c != 'n')
				{
					goto IL_00B7;
				}
			}
			else if (c != 't' && c != 'y')
			{
				goto IL_00B7;
			}
			bool flag = true;
			goto IL_00B9;
			IL_00B7:
			flag = false;
			IL_00B9:
			if (flag)
			{
				bool flag2;
				if (bool.TryParse(value, out flag2))
				{
					return flag2;
				}
				if (value.Equals("yes", StringComparison.OrdinalIgnoreCase) || value.Equals("y", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if (value.Equals("no", StringComparison.OrdinalIgnoreCase) || value.Equals("n", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
			return value;
		}

		public static void SetSwitchValue(string @switch, [Nullable(2)] object value)
		{
			Switches.switchValues[@switch] = value;
		}

		public static void ClearSwitchValue(string @switch)
		{
			object obj;
			Switches.switchValues.TryRemove(@switch, out obj);
		}

		[return: Nullable(new byte[] { 1, 1, 2 })]
		private static Func<string, object> MakeGetDataDelegate()
		{
			Type type = Switches.tAppContext;
			MethodInfo methodInfo = ((type != null) ? type.GetMethod("GetData", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(string) }, null) : null);
			Func<string, object> func = ((methodInfo != null) ? methodInfo.TryCreateDelegate<Func<string, object>>() : null);
			if (func != null)
			{
				try
				{
					func("MonoMod.LogToFile");
				}
				catch
				{
					func = null;
				}
			}
			if (func == null)
			{
				func = new Func<string, object>(AppDomain.CurrentDomain.GetData);
			}
			return func;
		}

		public static bool TryGetSwitchValue(string @switch, [Nullable(2)] out object value)
		{
			if (Switches.switchValues.TryGetValue(@switch, out value))
			{
				return true;
			}
			if (Switches.dGetData != null || Switches.dTryGetSwitch != null)
			{
				string text = "MonoMod." + @switch;
				Func<string, object> func = Switches.dGetData;
				object obj = ((func != null) ? func(text) : null);
				if (obj != null)
				{
					value = obj;
					return true;
				}
				Switches.TryGetSwitchFunc tryGetSwitchFunc = Switches.dTryGetSwitch;
				bool flag;
				if (tryGetSwitchFunc != null && tryGetSwitchFunc(text, out flag))
				{
					value = flag;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool TryGetSwitchEnabled(string @switch, out bool isEnabled)
		{
			object obj;
			if (Switches.switchValues.TryGetValue(@switch, out obj) && obj != null && Switches.TryProcessBoolData(obj, out isEnabled))
			{
				return true;
			}
			if (Switches.dGetData != null || Switches.dTryGetSwitch != null)
			{
				string text = "MonoMod." + @switch;
				Switches.TryGetSwitchFunc tryGetSwitchFunc = Switches.dTryGetSwitch;
				if (tryGetSwitchFunc != null && tryGetSwitchFunc(text, out isEnabled))
				{
					return true;
				}
				Func<string, object> func = Switches.dGetData;
				object obj2 = ((func != null) ? func(text) : null);
				if (obj2 != null && Switches.TryProcessBoolData(obj2, out isEnabled))
				{
					return true;
				}
			}
			isEnabled = false;
			return false;
		}

		private static bool TryProcessBoolData(object data, out bool boolVal)
		{
			if (data is bool)
			{
				bool flag = (bool)data;
				bool flag2 = flag;
				boolVal = flag2;
				return true;
			}
			if (data is int)
			{
				int num = (int)data;
				int num2 = num;
				boolVal = num2 != 0;
				return true;
			}
			if (data is long)
			{
				long num3 = (long)data;
				long num4 = num3;
				boolVal = num4 != 0L;
				return true;
			}
			string text = data as string;
			IConvertible convertible;
			if (text == null)
			{
				convertible = data as IConvertible;
				if (convertible == null)
				{
					boolVal = false;
					return false;
				}
			}
			else
			{
				if (bool.TryParse(text, out boolVal))
				{
					return true;
				}
				convertible = (IConvertible)data;
			}
			IConvertible convertible2 = convertible;
			boolVal = convertible2.ToBoolean(CultureInfo.CurrentCulture);
			return true;
		}

		[Nullable(new byte[] { 1, 1, 2 })]
		private static readonly ConcurrentDictionary<string, object> switchValues = new ConcurrentDictionary<string, object>();

		private const string Prefix = "MONOMOD_";

		public const string RunningOnWine = "RunningOnWine";

		public const string DebugClr = "DebugClr";

		public const string JitPath = "JitPath";

		public const string HelperDropPath = "HelperDropPath";

		public const string LogRecordHoles = "LogRecordHoles";

		public const string LogInMemory = "LogInMemory";

		public const string LogSpam = "LogSpam";

		public const string LogReplayQueueLength = "LogReplayQueueLength";

		public const string LogToFile = "LogToFile";

		public const string LogToFileFilter = "LogToFileFilter";

		public const string DMDType = "DMDType";

		public const string DMDDebug = "DMDDebug";

		public const string DMDDumpTo = "DMDDumpTo";

		[Nullable(2)]
		private static readonly Type tAppContext = typeof(AppDomain).Assembly.GetType("System.AppContext");

		[Nullable(new byte[] { 1, 1, 2 })]
		private static readonly Func<string, object> dGetData = Switches.MakeGetDataDelegate();

		[Nullable(2)]
		private static readonly MethodInfo miTryGetSwitch;

		[Nullable(2)]
		private static readonly Switches.TryGetSwitchFunc dTryGetSwitch;

		[NullableContext(0)]
		private delegate bool TryGetSwitchFunc(string @switch, out bool isEnabled);
	}
}
