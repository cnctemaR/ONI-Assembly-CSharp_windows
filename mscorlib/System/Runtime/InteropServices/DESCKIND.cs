using System;

namespace System.Runtime.InteropServices
{
	[Obsolete]
	[Serializable]
	public enum DESCKIND
	{
		DESCKIND_NONE,
		DESCKIND_FUNCDESC,
		DESCKIND_VARDESC,
		DESCKIND_TYPECOMP,
		DESCKIND_IMPLICITAPPOBJ,
		DESCKIND_MAX
	}
}
