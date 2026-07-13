using System;

namespace UnityEngine.Rendering
{
	public enum OpenGLESVersion
	{
		None,
		[Obsolete("OpenGL ES 2.0 is no longer supported in Unity 2023.1")]
		OpenGLES20,
		OpenGLES30,
		OpenGLES31,
		OpenGLES31AEP,
		OpenGLES32
	}
}
