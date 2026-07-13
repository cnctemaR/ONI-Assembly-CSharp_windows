using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.DedicatedServer
{
	[StaticAccessor("DedicatedServerBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Export/DedicatedServer/Arguments.bindings.h")]
	public static class Arguments
	{
		[FreeFunction("DedicatedServerBindings::GetBoolArgument")]
		[NativeConditional("PLATFORM_SERVER")]
		internal unsafe static bool GetBoolArgument(string arg)
		{
			bool boolArgument_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(arg, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = arg.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				boolArgument_Injected = Arguments.GetBoolArgument_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return boolArgument_Injected;
		}

		[FreeFunction("DedicatedServerBindings::GetIntArgument")]
		[NativeConditional("PLATFORM_SERVER")]
		internal unsafe static bool GetIntArgument(string arg, out int intValue)
		{
			bool intArgument_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(arg, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = arg.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				intArgument_Injected = Arguments.GetIntArgument_Injected(ref managedSpanWrapper, out intValue);
			}
			finally
			{
				char* ptr = null;
			}
			return intArgument_Injected;
		}

		[FreeFunction("DedicatedServerBindings::GetStringArgument")]
		[NativeConditional("PLATFORM_SERVER")]
		internal unsafe static bool GetStringArgument(string arg, out string stringValue)
		{
			bool stringArgument_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(arg, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = arg.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				stringArgument_Injected = Arguments.GetStringArgument_Injected(ref managedSpanWrapper, out managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				ManagedSpanWrapper managedSpanWrapper2;
				stringValue = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper2);
			}
			return stringArgument_Injected;
		}

		[FreeFunction("DedicatedServerBindings::SetBoolArgument")]
		[NativeConditional("PLATFORM_SERVER")]
		internal unsafe static void SetBoolArgument(string arg)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(arg, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = arg.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Arguments.SetBoolArgument_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeConditional("PLATFORM_SERVER")]
		[FreeFunction("DedicatedServerBindings::SetIntArgument")]
		internal unsafe static void SetIntArgument(string arg, int intValue)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(arg, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = arg.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Arguments.SetIntArgument_Injected(ref managedSpanWrapper, intValue);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeConditional("PLATFORM_SERVER")]
		[FreeFunction("DedicatedServerBindings::SetStringArgument")]
		internal unsafe static void SetStringArgument(string arg, string stringValue)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(arg, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = arg.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(stringValue, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = stringValue.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				Arguments.SetStringArgument_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		public static int? Port
		{
			get
			{
				int num;
				bool intArgument = Arguments.GetIntArgument("port", out num);
				int? num2;
				if (intArgument)
				{
					num2 = new int?(num);
				}
				else
				{
					num2 = null;
				}
				return num2;
			}
			set
			{
				int valueOrDefault = value.GetValueOrDefault();
				Arguments.SetIntArgument("port", valueOrDefault);
			}
		}

		public static int? TargetFramerate
		{
			get
			{
				int num;
				bool intArgument = Arguments.GetIntArgument("framerate", out num);
				int? num2;
				if (intArgument)
				{
					num2 = new int?(num);
				}
				else
				{
					num2 = null;
				}
				return num2;
			}
			set
			{
				int valueOrDefault = value.GetValueOrDefault();
				Arguments.SetIntArgument("framerate", valueOrDefault);
			}
		}

		public static int? LogLevel
		{
			get
			{
				int num;
				bool intArgument = Arguments.GetIntArgument("loglevel", out num);
				int? num2;
				if (intArgument)
				{
					num2 = new int?(num);
				}
				else
				{
					num2 = null;
				}
				return num2;
			}
			set
			{
				int valueOrDefault = value.GetValueOrDefault();
				Arguments.SetIntArgument("loglevel", valueOrDefault);
			}
		}

		public static string LogPath
		{
			get
			{
				string text;
				bool stringArgument = Arguments.GetStringArgument("logpath", out text);
				string text2;
				if (stringArgument)
				{
					text2 = text;
				}
				else
				{
					bool stringArgument2 = Arguments.GetStringArgument("logfile", out text);
					if (stringArgument2)
					{
						text2 = Path.GetDirectoryName(text);
					}
					else
					{
						text2 = null;
					}
				}
				return text2;
			}
			set
			{
				Arguments.SetStringArgument("logpath", value);
			}
		}

		public static int? QueryPort
		{
			get
			{
				int num;
				bool intArgument = Arguments.GetIntArgument("queryport", out num);
				int? num2;
				if (intArgument)
				{
					num2 = new int?(num);
				}
				else
				{
					num2 = null;
				}
				return num2;
			}
			set
			{
				int valueOrDefault = value.GetValueOrDefault();
				Arguments.SetIntArgument("queryport", valueOrDefault);
			}
		}

		public static string QueryType
		{
			get
			{
				string text;
				bool stringArgument = Arguments.GetStringArgument("querytype", out text);
				string text2;
				if (stringArgument)
				{
					text2 = text;
				}
				else
				{
					text2 = null;
				}
				return text2;
			}
			set
			{
				Arguments.SetStringArgument("querytype", value);
			}
		}

		[FreeFunction("DedicatedServerBindings::SetArgumentErrorPolicy")]
		[NativeConditional("PLATFORM_SERVER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetArgumentErrorPolicy(Arguments.ArgumentErrorPolicy policy);

		[NativeConditional("PLATFORM_SERVER")]
		[FreeFunction("DedicatedServerBindings::GetArgumentErrorPolicy")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Arguments.ArgumentErrorPolicy GetArgumentErrorPolicy();

		public static Arguments.ArgumentErrorPolicy ErrorPolicy
		{
			get
			{
				return Arguments.GetArgumentErrorPolicy();
			}
			set
			{
				Arguments.SetArgumentErrorPolicy(value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBoolArgument_Injected(ref ManagedSpanWrapper arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIntArgument_Injected(ref ManagedSpanWrapper arg, out int intValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetStringArgument_Injected(ref ManagedSpanWrapper arg, out ManagedSpanWrapper stringValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoolArgument_Injected(ref ManagedSpanWrapper arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntArgument_Injected(ref ManagedSpanWrapper arg, int intValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetStringArgument_Injected(ref ManagedSpanWrapper arg, ref ManagedSpanWrapper stringValue);

		public enum ArgumentErrorPolicy
		{
			Ignore,
			Warn,
			Fatal
		}
	}
}
