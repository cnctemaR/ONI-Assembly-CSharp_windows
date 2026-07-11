using System;
using System.Collections.Generic;
using UnityEngine;

namespace rendering
{
	public class BackWall : MonoBehaviour
	{
		private void Awake()
		{
			DebugUtil.DevAssert(this.backwallMaterial != null, "Expected a backwall material!");
			DebugUtil.DevAssert(this.images.Count > 0, "Expected backwall images (at least one)!");
			Texture2D texture2D = this.images[0];
			int count = this.images.Count;
			int mipmapCount = texture2D.mipmapCount;
			bool flag = mipmapCount > 0;
			this.textureArray = new Texture2DArray(texture2D.width, texture2D.height, count, TextureFormat.RGB24, flag);
			for (int i = 0; i < count; i++)
			{
				Texture2D texture2D2 = this.images[i];
				global::Debug.Log(string.Format("copying image {0} type {1} size {2}x{3}", new object[] { texture2D2.name, texture2D2.format, texture2D2.width, texture2D2.height }));
				for (int j = 0; j < mipmapCount; j++)
				{
					this.textureArray.SetPixels(texture2D2.GetPixels(j), i, j);
				}
			}
			this.textureArray.Apply();
			this.backwallMaterial.SetTexture("images", this.textureArray);
		}

		[SerializeField]
		public Material backwallMaterial;

		[SerializeField]
		public List<Texture2D> images;

		private Texture2DArray textureArray;
	}
}
