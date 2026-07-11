using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	[UsedByNativeCode]
	public struct VisibleReflectionProbe
	{
		public Texture texture
		{
			get
			{
				return VisibleReflectionProbe.GetTextureObject(this.textureId);
			}
		}

		[FreeFunction("(Texture*)Object::IDToPointer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture GetTextureObject(int textureId);

		public ReflectionProbe probe
		{
			get
			{
				return VisibleReflectionProbe.GetReflectionProbeObject(this.instanceId);
			}
		}

		[FreeFunction("(ReflectionProbe*)Object::IDToPointer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ReflectionProbe GetReflectionProbeObject(int instanceId);

		public Bounds bounds;

		public Matrix4x4 localToWorld;

		public Vector4 hdr;

		public Vector3 center;

		public float blendDistance;

		public int importance;

		public int boxProjection;

		private int instanceId;

		private int textureId;
	}
}
