using System;

namespace Mono.Cecil
{
	[Flags]
	internal enum TypeAttributes : uint
	{
		VisibilityMask = 7U,
		NotPublic = 0U,
		Public = 1U,
		NestedPublic = 2U,
		NestedPrivate = 3U,
		NestedFamily = 4U,
		NestedAssembly = 5U,
		NestedFamANDAssem = 6U,
		NestedFamORAssem = 7U,
		LayoutMask = 24U,
		AutoLayout = 0U,
		SequentialLayout = 8U,
		ExplicitLayout = 16U,
		ClassSemanticMask = 32U,
		Class = 0U,
		Interface = 32U,
		Abstract = 128U,
		Sealed = 256U,
		SpecialName = 1024U,
		Import = 4096U,
		Serializable = 8192U,
		WindowsRuntime = 16384U,
		StringFormatMask = 196608U,
		AnsiClass = 0U,
		UnicodeClass = 65536U,
		AutoClass = 131072U,
		BeforeFieldInit = 1048576U,
		RTSpecialName = 2048U,
		HasSecurity = 262144U,
		Forwarder = 2097152U
	}
}
