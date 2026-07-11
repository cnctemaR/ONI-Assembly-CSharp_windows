using System;
using System.ComponentModel;

namespace UnityEngine
{
	[Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public enum ProceduralPropertyType
	{
		Boolean,
		Float,
		Vector2,
		Vector3,
		Vector4,
		Color3,
		Color4,
		Enum,
		Texture,
		String
	}
}
