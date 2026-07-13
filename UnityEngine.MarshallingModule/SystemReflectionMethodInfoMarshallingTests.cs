using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/SystemReflectionMethodInfoMarshallingTests.h")]
	[ExcludeFromDocs]
	internal static class SystemReflectionMethodInfoMarshallingTests
	{
		public static string CanMarshallMethodInfoArgumentToScriptingMethodPtr(MethodInfo param)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				SystemReflectionMethodInfoMarshallingTests.CanMarshallMethodInfoArgumentToScriptingMethodPtr_Injected(param, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public static string CanMarshallSystemReflectionMethodInfoStructField(StructSystemReflectionMethodInfo param)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				SystemReflectionMethodInfoMarshallingTests.CanMarshallSystemReflectionMethodInfoStructField_Injected(ref param, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public static string[] CanMarshallSystemReflectionMethodInfoArrayStructField(StructSystemReflectionMethodInfoArray param)
		{
			return SystemReflectionMethodInfoMarshallingTests.CanMarshallSystemReflectionMethodInfoArrayStructField_Injected(ref param);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] CanMarshallArrayOfMethodInfoArgumentToVectorOfScriptingMethodInfoObjectPtr(MethodInfo[] param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] CanMarshallArrayOfMethodInfoArgumentToVectorOfScriptingMethodPtr(MethodInfo[] param);

		public static StructSystemReflectionMethodInfo CanUnmarshallSystemReflectionMethodInfoStructField()
		{
			StructSystemReflectionMethodInfo structSystemReflectionMethodInfo;
			SystemReflectionMethodInfoMarshallingTests.CanUnmarshallSystemReflectionMethodInfoStructField_Injected(out structSystemReflectionMethodInfo);
			return structSystemReflectionMethodInfo;
		}

		public static StructSystemReflectionMethodInfoArray CanUnmarshallSystemReflectionMethodInfoArrayStructField()
		{
			StructSystemReflectionMethodInfoArray structSystemReflectionMethodInfoArray;
			SystemReflectionMethodInfoMarshallingTests.CanUnmarshallSystemReflectionMethodInfoArrayStructField_Injected(out structSystemReflectionMethodInfoArray);
			return structSystemReflectionMethodInfoArray;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MethodInfo CanUnmarshallScriptingMethodInfoObjectPtrToMethodInfo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MethodInfo CanUnmarshallScriptingMethodPtrToMethodInfo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public static extern MethodInfo[] CanUnmarshallScriptingArrayPtrToMethodInfoArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MethodInfo[] CanUnmarshallArrayOfScriptingMethodInfoObjectPtrToMethodInfoArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MethodInfo[] CanUnmarshallArrayOfScriptingMethodPtrToMethodInfoArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanMarshallMethodInfoArgumentToScriptingMethodPtr_Injected(MethodInfo param, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanMarshallSystemReflectionMethodInfoStructField_Injected([In] ref StructSystemReflectionMethodInfo param, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] CanMarshallSystemReflectionMethodInfoArrayStructField_Injected([In] ref StructSystemReflectionMethodInfoArray param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanUnmarshallSystemReflectionMethodInfoStructField_Injected(out StructSystemReflectionMethodInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanUnmarshallSystemReflectionMethodInfoArrayStructField_Injected(out StructSystemReflectionMethodInfoArray ret);
	}
}
