using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro.SpriteAssetUtilities
{
	public class TexturePacker_JsonArray
	{
		[Serializable]
		public struct SpriteFrame
		{
			public override string ToString()
			{
				return string.Concat(new string[]
				{
					"x: ",
					this.x.ToString("f2"),
					" y: ",
					this.y.ToString("f2"),
					" h: ",
					this.h.ToString("f2"),
					" w: ",
					this.w.ToString("f2")
				});
			}

			public float x;

			public float y;

			public float w;

			public float h;
		}

		[Serializable]
		public struct SpriteSize
		{
			public override string ToString()
			{
				return "w: " + this.w.ToString("f2") + " h: " + this.h.ToString("f2");
			}

			public float w;

			public float h;
		}

		[Serializable]
		public struct Frame
		{
			public string filename;

			public TexturePacker_JsonArray.SpriteFrame frame;

			public bool rotated;

			public bool trimmed;

			public TexturePacker_JsonArray.SpriteFrame spriteSourceSize;

			public TexturePacker_JsonArray.SpriteSize sourceSize;

			public Vector2 pivot;
		}

		[Serializable]
		public struct Meta
		{
			public string app;

			public string version;

			public string image;

			public string format;

			public TexturePacker_JsonArray.SpriteSize size;

			public float scale;

			public string smartupdate;
		}

		[Serializable]
		public class SpriteDataObject
		{
			public List<TexturePacker_JsonArray.Frame> frames;

			public TexturePacker_JsonArray.Meta meta;
		}
	}
}
