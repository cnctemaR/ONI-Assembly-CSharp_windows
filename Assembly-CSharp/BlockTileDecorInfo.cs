using System;
using Rendering;
using UnityEngine;

public class BlockTileDecorInfo : ScriptableObject
{
	public void PostProcess()
	{
		if (this.decor != null && this.atlas != null && this.atlas.items != null)
		{
			for (int i = 0; i < this.decor.Length; i++)
			{
				if (this.decor[i].variants != null && this.decor[i].variants.Length != 0)
				{
					for (int j = 0; j < this.decor[i].variants.Length; j++)
					{
						bool flag = false;
						foreach (TextureAtlas.Item item in this.atlas.items)
						{
							if (this.decor[i].variants[j].name == item.name)
							{
								this.decor[i].variants[j].atlasItem = item;
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							Output.LogError(new object[]
							{
								base.name,
								"/",
								this.decor[i].name,
								"could not find ",
								this.decor[i].variants[j].name,
								"in",
								this.atlas.name
							});
						}
					}
				}
			}
		}
	}

	public TextureAtlas atlas;

	public int sortOrder;

	public BlockTileDecorInfo.Decor[] decor;

	[Serializable]
	public struct ImageInfo
	{
		public string name;

		public Vector3 offset;

		[NonSerialized]
		public TextureAtlas.Item atlasItem;
	}

	[Serializable]
	public struct Decor
	{
		public string name;

		[EnumFlags]
		public BlockTileRenderer.Bits requiredConnections;

		[EnumFlags]
		public BlockTileRenderer.Bits forbiddenConnections;

		public float probabilityCutoff;

		public BlockTileDecorInfo.ImageInfo[] variants;

		public int sortOrder;
	}
}
