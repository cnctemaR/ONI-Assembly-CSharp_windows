using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundMasks : ScriptableObject
{
	public void Initialize()
	{
		if (this.maskAtlas == null || this.maskAtlas.items == null)
		{
			return;
		}
		this.biomeMasks = new Dictionary<string, GroundMasks.BiomeMaskData>();
		foreach (TextureAtlas.Item item in this.maskAtlas.items)
		{
			string name = item.name;
			int num = name.IndexOf('/');
			string text = name.Substring(0, num);
			string text2 = name.Substring(num + 1, 4);
			text = text.ToLower();
			for (int num2 = text.IndexOf('_'); num2 != -1; num2 = text.IndexOf('_'))
			{
				text = text.Remove(num2, 1);
			}
			GroundMasks.BiomeMaskData biomeMaskData = null;
			if (!this.biomeMasks.TryGetValue(text, out biomeMaskData))
			{
				biomeMaskData = new GroundMasks.BiomeMaskData(text);
				this.biomeMasks[text] = biomeMaskData;
			}
			int num3 = Convert.ToInt32(text2, 2);
			GroundMasks.Tile tile = biomeMaskData.tiles[num3];
			if (tile.variationUVs == null)
			{
				tile.isSource = true;
				tile.variationUVs = new GroundMasks.UVData[1];
			}
			else
			{
				GroundMasks.UVData[] array = new GroundMasks.UVData[tile.variationUVs.Length + 1];
				Array.Copy(tile.variationUVs, array, tile.variationUVs.Length);
				tile.variationUVs = array;
			}
			Vector4 vector = new Vector4(item.uvBox.x, item.uvBox.w, item.uvBox.z, item.uvBox.y);
			Vector2 vector2 = new Vector2(vector.x, vector.y);
			Vector2 vector3 = new Vector2(vector.z, vector.y);
			Vector2 vector4 = new Vector2(vector.x, vector.w);
			Vector2 vector5 = new Vector2(vector.z, vector.w);
			GroundMasks.UVData uvdata = new GroundMasks.UVData(vector2, vector3, vector4, vector5);
			tile.variationUVs[tile.variationUVs.Length - 1] = uvdata;
			biomeMaskData.tiles[num3] = tile;
		}
		foreach (KeyValuePair<string, GroundMasks.BiomeMaskData> keyValuePair in this.biomeMasks)
		{
			keyValuePair.Value.GenerateRotations();
			keyValuePair.Value.Validate();
		}
	}

	[ContextMenu("Regenerate")]
	private void Regenerate()
	{
		this.Initialize();
		foreach (KeyValuePair<string, GroundMasks.BiomeMaskData> keyValuePair in this.biomeMasks)
		{
			GroundMasks.BiomeMaskData value = keyValuePair.Value;
			Output.Log(new object[] { value.name });
			for (int i = 1; i < value.tiles.Length; i++)
			{
				GroundMasks.Tile tile = value.tiles[i];
				Output.Log(new object[]
				{
					"Tile",
					i,
					"has",
					tile.variationUVs.Length,
					"variations"
				});
			}
		}
	}

	public TextureAtlas maskAtlas;

	[NonSerialized]
	public Dictionary<string, GroundMasks.BiomeMaskData> biomeMasks;

	public struct UVData
	{
		public UVData(Vector2 bl, Vector2 br, Vector2 tl, Vector2 tr)
		{
			this.bl = bl;
			this.br = br;
			this.tl = tl;
			this.tr = tr;
		}

		public Vector2 bl;

		public Vector2 br;

		public Vector2 tl;

		public Vector2 tr;
	}

	public struct Tile
	{
		public bool isSource;

		public GroundMasks.UVData[] variationUVs;
	}

	public class BiomeMaskData
	{
		public BiomeMaskData(string name)
		{
			this.name = name;
			this.tiles = new GroundMasks.Tile[16];
		}

		public void GenerateRotations()
		{
			for (int i = 1; i < 15; i++)
			{
				if (!this.tiles[i].isSource)
				{
					GroundMasks.Tile tile = this.tiles[i];
					tile.variationUVs = this.GetNonNullRotationUVs(i);
					this.tiles[i] = tile;
				}
			}
		}

		public GroundMasks.UVData[] GetNonNullRotationUVs(int dest_mask)
		{
			GroundMasks.UVData[] array = null;
			int num = dest_mask;
			for (int i = 0; i < 3; i++)
			{
				int num2 = (num & 1) >> 0;
				int num3 = (num & 2) >> 1;
				int num4 = (num & 4) >> 2;
				int num5 = (num & 8) >> 3;
				int num6 = (num5 << 2) | (num4 << 0) | (num3 << 3) | (num2 << 1);
				if (this.tiles[num6].isSource)
				{
					array = new GroundMasks.UVData[this.tiles[num6].variationUVs.Length];
					for (int j = 0; j < this.tiles[num6].variationUVs.Length; j++)
					{
						GroundMasks.UVData uvdata = this.tiles[num6].variationUVs[j];
						GroundMasks.UVData uvdata2 = uvdata;
						switch (i)
						{
						case 0:
							uvdata2 = new GroundMasks.UVData(uvdata.tl, uvdata.bl, uvdata.tr, uvdata.br);
							break;
						case 1:
							uvdata2 = new GroundMasks.UVData(uvdata.tr, uvdata.tl, uvdata.br, uvdata.bl);
							break;
						case 2:
							uvdata2 = new GroundMasks.UVData(uvdata.br, uvdata.tr, uvdata.bl, uvdata.tl);
							break;
						default:
							Output.LogError(new object[] { "Unhandled rotation case" });
							break;
						}
						array[j] = uvdata2;
					}
					break;
				}
				num = num6;
			}
			return array;
		}

		public void Validate()
		{
			for (int i = 1; i < this.tiles.Length; i++)
			{
				GroundMasks.Tile tile = this.tiles[i];
				if (tile.variationUVs == null)
				{
					Output.LogError(new object[] { this.name, "has invalid tile at index", i });
				}
			}
		}

		public string name;

		public GroundMasks.Tile[] tiles;
	}
}
