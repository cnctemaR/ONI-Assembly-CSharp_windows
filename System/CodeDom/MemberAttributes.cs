using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[Serializable]
	public enum MemberAttributes
	{
		Abstract = 1,
		Final,
		Static,
		Override,
		Const,
		ScopeMask = 15,
		New,
		VTableMask = 240,
		Overloaded = 256,
		Assembly = 4096,
		FamilyAndAssembly = 8192,
		Family = 12288,
		FamilyOrAssembly = 16384,
		Private = 20480,
		Public = 24576,
		AccessMask = 61440
	}
}
