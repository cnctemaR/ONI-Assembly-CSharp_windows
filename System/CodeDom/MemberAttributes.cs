using System;

namespace System.CodeDom
{
	public enum MemberAttributes
	{
		Abstract = 1,
		Final,
		Static,
		Override,
		Const,
		New = 16,
		Overloaded = 256,
		Assembly = 4096,
		FamilyAndAssembly = 8192,
		Family = 12288,
		FamilyOrAssembly = 16384,
		Private = 20480,
		Public = 24576,
		AccessMask = 61440,
		ScopeMask = 15,
		VTableMask = 240
	}
}
