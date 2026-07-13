using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/SystemTypeMarshallingTests.h")]
	[ExcludeFromDocs]
	internal static class SystemTypeMarshallingTests
	{
		public static string CanMarshallSystemTypeArgumentToScriptingClassPtr(Type param)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				SystemTypeMarshallingTests.CanMarshallSystemTypeArgumentToScriptingClassPtr_Injected(param, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public static string CanMarshallSystemTypeStructField(StructSystemType param)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				SystemTypeMarshallingTests.CanMarshallSystemTypeStructField_Injected(ref param, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public static string[] CanMarshallSystemTypeArrayStructField(StructSystemTypeArray param)
		{
			return SystemTypeMarshallingTests.CanMarshallSystemTypeArrayStructField_Injected(ref param);
		}

		public static StructSystemType CanUnmarshallSystemTypeStructField()
		{
			StructSystemType structSystemType;
			SystemTypeMarshallingTests.CanUnmarshallSystemTypeStructField_Injected(out structSystemType);
			return structSystemType;
		}

		public static StructSystemTypeArray CanUnmarshallSystemTypeArrayStructField()
		{
			StructSystemTypeArray structSystemTypeArray;
			SystemTypeMarshallingTests.CanUnmarshallSystemTypeArrayStructField_Injected(out structSystemTypeArray);
			return structSystemTypeArray;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] CanUnmarshallArrayOfSystemTypeArgumentToVectorOfScriptingSystemTypeObjectPtr(Type[] param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] CanUnmarshallArrayOfSystemTypeArgumentToVectorOfUnityType(Type[] param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] CanUnmarshallArrayOfSystemTypeArgumentToVectorOfScriptingClassPtr(Type[] param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type CanUnmarshallScriptingSystemTypeObjectPtrToSystemType();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type CanUnmarshallUnityTypeToSystemType();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type CanUnmarshallScriptingClassPtrToSystemType();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public static extern Type[] CanUnmarshallScriptingArrayPtrToSystemTypeArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type[] CanUnmarshallArrayOfScriptingSystemTypeObjectPtrToSystemTypeArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type[] CanUnmarshallArrayOfUnityTypeToSystemTypeArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type[] CanUnmarshallArrayOfScriptingClassPtrToSystemTypeArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanMarshallSystemTypeArgumentToScriptingClassPtr_Injected(Type param, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanMarshallSystemTypeStructField_Injected([In] ref StructSystemType param, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] CanMarshallSystemTypeArrayStructField_Injected([In] ref StructSystemTypeArray param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanUnmarshallSystemTypeStructField_Injected(out StructSystemType ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanUnmarshallSystemTypeArrayStructField_Injected(out StructSystemTypeArray ret);
	}
}
