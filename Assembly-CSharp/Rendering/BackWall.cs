using System;
using System.Collections.Generic;
using UnityEngine;

namespace rendering
{
	public class BackWall : MonoBehaviour
	{
		private void Awake()
		{
			DebugUtil.DevAssert(this.backwallMaterial != null, "Expected a backwall material!", null);
			DebugUtil.DevAssert(this.images.Count > 0, "Expected backwall images (at least one)!", null);
			BackWallImage backWallImage = this.images[0];
			int count = this.images.Count;
			int mipmapCount = backWallImage.image.mipmapCount;
			bool flag = mipmapCount > 0;
			this.textureArray = new Texture2DArray(backWallImage.image.width, backWallImage.image.height, count, TextureFormat.RGB24, flag);
			for (int i = 0; i < count; i++)
			{
				BackWallImage backWallImage2 = this.images[i];
				for (int j = 0; j < mipmapCount; j++)
				{
					this.textureArray.SetPixels(backWallImage2.image.GetPixels(j), i, j);
				}
			}
			this.textureArray.Apply();
			this.backwallMaterial.SetTexture("images", this.textureArray);
		}

		[SerializeField]
		public Material backwallMaterial;

		[SerializeField]
		public List<BackWallImage> images;

		private Texture2DArray textureArray;
	}
}
