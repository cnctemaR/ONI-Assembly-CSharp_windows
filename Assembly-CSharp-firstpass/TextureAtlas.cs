using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TextureAtlas : ScriptableObject
{
	public void Import(TextAsset data, Texture2D texture)
	{
		this.texture = texture;
		float num = (float)texture.width;
		float num2 = (float)texture.height;
		TextureAtlas.AtlasData atlasData = JsonConvert.DeserializeObject<TextureAtlas.AtlasData>(data.text);
		float num3 = (float)atlasData.meta.size["w"];
		float num4 = (float)atlasData.meta.size["h"];
		this.items = new TextureAtlas.Item[atlasData.frames.Count];
		for (int i = 0; i < atlasData.frames.Count; i++)
		{
			TextureAtlas.AtlasData.Frame frame = atlasData.frames[i];
			this.items[i].name = frame.filename;
			this.items[i].uvBox.x = (float)frame.frame["x"] / num3;
			this.items[i].uvBox.y = 1f - (float)frame.frame["y"] / num4;
			this.items[i].uvBox.z = this.items[i].uvBox.x + (float)frame.frame["w"] / num3;
			this.items[i].uvBox.w = this.items[i].uvBox.y - (float)frame.frame["h"] / num4;
			if (frame.vertices != null)
			{
				Vector3[] array = new Vector3[frame.vertices.Count];
				Vector2[] array2 = new Vector2[frame.verticesUV.Count];
				int[] array3 = new int[frame.triangles.Count * 3];
				for (int j = 0; j < frame.vertices.Count; j++)
				{
					array[j] = new Vector3((float)frame.vertices[j][0], (float)(frame.frame["h"] - frame.vertices[j][1]), 0f) / this.vertexScale;
				}
				for (int k = 0; k < frame.verticesUV.Count; k++)
				{
					array2[k] = new Vector2((float)frame.verticesUV[k][0] / num, 1f - (float)frame.verticesUV[k][1] / num2);
				}
				for (int l = 0; l < frame.triangles.Count; l++)
				{
					array3[l * 3] = frame.triangles[l][0];
					array3[l * 3 + 1] = frame.triangles[l][1];
					array3[l * 3 + 2] = frame.triangles[l][2];
				}
				this.items[i].vertices = array;
				this.items[i].uvs = array2;
				this.items[i].indices = array3;
			}
		}
	}

	public Texture2D texture;

	public float vertexScale = 1f;

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
