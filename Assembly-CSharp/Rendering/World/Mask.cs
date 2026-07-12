using System;
using UnityEngine;

namespace Rendering.World
{
	public struct Mask
	{
		public Vector2 UV0 { readonly get; private set; }

		public Vector2 UV1 { readonly get; private set; }

		public Vector2 UV2 { readonly get; private set; }

		public Vector2 UV3 { readonly get; private set; }

		public bool IsOpaque { readonly get; private set; }

		public Mask(TextureAtlas atlas, int texture_idx, bool transpose, bool flip_x, bool flip_y, bool is_opaque)
		{
			this = default(Mask);
			this.atlas = atlas;
			this.texture_idx = texture_idx;
			this.transpose = transpose;
			this.flip_x = flip_x;
			this.flip_y = flip_y;
			this.atlas_offset = 0;
			this.IsOpaque = is_opaque;
			this.Refresh();
		}

		public void SetOffset(int offset)
		{
			this.atlas_offset = offset;
			this.Refresh();
		}

		public void Refresh()
		{
			int num = this.atlas_offset * 4 + this.atlas_offset;
			if (num + this.texture_idx >= this.atlas.items.Length)
			{
				num = 0;
			}
			Vector4 uvBox = this.atlas.items[num + this.texture_idx].uvBox;
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			Vector2 zero3 = Vector2.zero;
			Vector2 zero4 = Vector2.zero;
			if (this.transpose)
			{
				float num2 = uvBox.x;
				float num3 = uvBox.z;
				if (this.flip_x)
				{
					num2 = uvBox.z;
					num3 = uvBox.x;
				}
				zero.x = num2;
				zero2.x = num2;
				zero3.x = num3;
				zero4.x = num3;
				float num4 = uvBox.y;
				float num5 = uvBox.w;
				if (this.flip_y)
				{
					num4 = uvBox.w;
					num5 = uvBox.y;
				}
				zero.y = num4;
				zero2.y = num5;
				zero3.y = num4;
				zero4.y = num5;
			}
			else
			{
				float num6 = uvBox.x;
				float num7 = uvBox.z;
				if (this.flip_x)
				{
					num6 = uvBox.z;
					num7 = uvBox.x;
				}
				zero.x = num6;
				zero2.x = num7;
				zero3.x = num6;
				zero4.x = num7;
				float num8 = uvBox.y;
				float num9 = uvBox.w;
				if (this.flip_y)
				{
					num8 = uvBox.w;
					num9 = uvBox.y;
				}
				zero.y = num9;
				zero2.y = num9;
				zero3.y = num8;
				zero4.y = num8;
			}
			this.UV0 = zero;
			this.UV1 = zero2;
			this.UV2 = zero3;
			this.UV3 = zero4;
		}

		private TextureAtlas atlas;

		private int texture_idx;

		private bool transpose;

		private bool flip_x;

		private bool flip_y;

		private int atlas_offset;

		private const int TILES_PER_SET = 4;
	}
}
