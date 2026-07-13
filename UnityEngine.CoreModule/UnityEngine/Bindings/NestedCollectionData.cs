using System;

namespace UnityEngine.Bindings
{
	[NativeType(CodegenOptions = CodegenOptions.Custom, IntermediateScriptingStructName = "Marshalling::NestedCollectionData", Header = "Runtime/Scripting/Marshalling/BlittableNestedCollectionMarshaller.h")]
	internal struct NestedCollectionData
	{
		public unsafe void* Data;

		public int Length;
	}
}
