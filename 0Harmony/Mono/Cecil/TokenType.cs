using System;

namespace Mono.Cecil
{
	internal enum TokenType : uint
	{
		Module,
		TypeRef = 16777216U,
		TypeDef = 33554432U,
		Field = 67108864U,
		Method = 100663296U,
		Param = 134217728U,
		InterfaceImpl = 150994944U,
		MemberRef = 167772160U,
		CustomAttribute = 201326592U,
		Permission = 234881024U,
		Signature = 285212672U,
		Event = 335544320U,
		Property = 385875968U,
		ModuleRef = 436207616U,
		TypeSpec = 452984832U,
		Assembly = 536870912U,
		AssemblyRef = 587202560U,
		File = 637534208U,
		ExportedType = 654311424U,
		ManifestResource = 671088640U,
		GenericParam = 704643072U,
		MethodSpec = 721420288U,
		GenericParamConstraint = 738197504U,
		Document = 805306368U,
		MethodDebugInformation = 822083584U,
		LocalScope = 838860800U,
		LocalVariable = 855638016U,
		LocalConstant = 872415232U,
		ImportScope = 889192448U,
		StateMachineMethod = 905969664U,
		CustomDebugInformation = 922746880U,
		String = 1879048192U
	}
}
