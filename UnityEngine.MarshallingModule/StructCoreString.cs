using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[RequiredByNativeCode(GenerateProxy = true, Name = "StructCoreStringManaged", Optional = true)]
	[NativeClass("StructCoreString", "struct StructCoreString;")]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal struct StructCoreString
	{
		public string GetField()
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				StructCoreString.GetField_Injected(ref this, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public unsafe void SetField(string value)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = value.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				StructCoreString.SetField_Injected(ref this, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetField_Injected(ref StructCoreString _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetField_Injected(ref StructCoreString _unity_self, ref ManagedSpanWrapper value);

		public string field;
	}
}
