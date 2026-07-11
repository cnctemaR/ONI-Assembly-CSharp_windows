using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Rendering
{
	[NativeHeader("Runtime/Export/ScriptableRenderLoop/ScriptableRenderLoop.bindings.h")]
	public struct ShaderPassName
	{
		public ShaderPassName(string name)
		{
			this.m_NameIndex = ShaderPassName.Init(name);
		}

		[FreeFunction("ScriptableRenderLoop_Bindings::InitShaderPassName")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Init(string name);

		internal int nameIndex
		{
			get
			{
				return this.m_NameIndex;
			}
		}

		private int m_NameIndex;
	}
}
