using System;
using System.Collections.Generic;
using UnityEngine;

public class TextureAtlas : ScriptableObject
{
	public List<string> sourceReference = new List<string>();

	public Texture2D texture;

	public float scaleFactor = 1f;

	public int mipPaddingPixels = 64;

	public int tileDimension = 256;

	public bool onlyRebuildSelfNamedFolder = true;

	public TextureAtlas.Item[] items;

	[Serializable]
	public struct Item
	{
		public string name;

		public Vector4 uvBox;

		public Vector3[] vertices;

		public Vector2[] uvs;

		public int[] indices;
	}

	public class AtlasData
	{
		public List<TextureAtlas.AtlasData.Frame> frames;

		public TextureAtlas.AtlasData.Meta meta;

		public class Frame
		{
			public string filename;

			public Dictionary<string, int> frame;

			public List<int[]> vertices;

			public List<int[]> verticesUV;

			public List<int[]> triangles;
		}

		public struct Meta
		{
			public Dictionary<string, int> size;
		}
	}
}
