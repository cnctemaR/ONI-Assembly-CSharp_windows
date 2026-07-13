using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/SystemReflectionFieldInfoMarshallingTests.h")]
	[ExcludeFromDocs]
	internal static class SystemReflectionFieldInfoMarshallingTests
	{
		public static string CanMarshallFieldInfoArgumentToScriptingFieldPtr(FieldInfo param)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				SystemReflectionFieldInfoMarshallingTests.CanMarshallFieldInfoArgumentToScriptingFieldPtr_Injected(param, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public static string CanMarshallSystemReflectionFieldInfoStructField(StructSystemReflectionFieldInfo param)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				SystemReflectionFieldInfoMarshallingTests.CanMarshallSystemReflectionFieldInfoStructField_Injected(ref param, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public static string[] CanMarshallSystemReflectionFieldInfoArrayStructField(StructSystemReflectionFieldInfoArray param)
		{
			return SystemReflectionFieldInfoMarshallingTests.CanMarshallSystemReflectionFieldInfoArrayStructField_Injected(ref param);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] CanMarshallArrayOfFieldInfoArgumentToVectorOfScriptingFieldInfoObjectPtr(FieldInfo[] param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] CanMarshallArrayOfFieldInfoArgumentToVectorOfScriptingFieldPtr(FieldInfo[] param);

		public static StructSystemReflectionFieldInfo CanUnmarshallSystemReflectionFieldInfoStructField()
		{
			StructSystemReflectionFieldInfo structSystemReflectionFieldInfo;
			SystemReflectionFieldInfoMarshallingTests.CanUnmarshallSystemReflectionFieldInfoStructField_Injected(out structSystemReflectionFieldInfo);
			return structSystemReflectionFieldInfo;
		}

		public static StructSystemReflectionFieldInfoArray CanUnmarshallSystemReflectionFieldInfoArrayStructField()
		{
			StructSystemReflectionFieldInfoArray structSystemReflectionFieldInfoArray;
			SystemReflectionFieldInfoMarshallingTests.CanUnmarshallSystemReflectionFieldInfoArrayStructField_Injected(out structSystemReflectionFieldInfoArray);
			return structSystemReflectionFieldInfoArray;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FieldInfo CanUnmarshallScriptingFieldInfoObjectPtrToFieldInfo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FieldInfo CanUnmarshallScriptingFieldPtrToFieldInfo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public static extern FieldInfo[] CanUnmarshallScriptingArrayPtrToFieldInfoArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FieldInfo[] CanUnmarshallArrayOfScriptingFieldInfoObjectPtrToFieldInfoArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FieldInfo[] CanUnmarshallArrayOfScriptingFieldPtrToFieldInfoArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanMarshallFieldInfoArgumentToScriptingFieldPtr_Injected(FieldInfo param, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanMarshallSystemReflectionFieldInfoStructField_Injected([In] ref StructSystemReflectionFieldInfo param, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] CanMarshallSystemReflectionFieldInfoArrayStructField_Injected([In] ref StructSystemReflectionFieldInfoArray param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanUnmarshallSystemReflectionFieldInfoStructField_Injected(out StructSystemReflectionFieldInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanUnmarshallSystemReflectionFieldInfoArrayStructField_Injected(out StructSystemReflectionFieldInfoArray ret);
	}
}
