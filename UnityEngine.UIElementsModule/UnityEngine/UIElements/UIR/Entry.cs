using System;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR
{
	internal class Entry
	{
		public void Reset()
		{
			this.nextSibling = null;
			this.firstChild = null;
			this.lastChild = null;
			this.texture = null;
			this.material = null;
			this.userProps = null;
			this.gradientsOwner = null;
			this.flags = (EntryFlags)0;
			this.immediateCallback = null;
		}

		public EntryType type;

		public EntryFlags flags;

		public NativeSlice<Vertex> vertices;

		public NativeSlice<ushort> indices;

		public Texture texture;

		public float textScale;

		public float fontSharpness;

		public VectorImage gradientsOwner;

		public Material material;

		public MaterialPropertyBlock userProps;

		public Action immediateCallback;

		public TextureId textureId;

		public Entry nextSibling;

		public Entry firstChild;

		public Entry lastChild;
	}
}
