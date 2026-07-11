using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Shaders/ShaderKeywordSet.h")]
	public enum ShaderKeywordType
	{
		None,
		BuiltinDefault = 2,
		BuiltinExtra = 6,
		BuiltinAutoStripped = 10,
		UserDefined = 16
	}
}
