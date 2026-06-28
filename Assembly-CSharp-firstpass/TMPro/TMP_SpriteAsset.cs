using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	public class TMP_SpriteAsset : TMP_Asset
	{
		public static TMP_SpriteAsset defaultSpriteAsset
		{
			get
			{
				if (TMP_SpriteAsset.m_defaultSpriteAsset == null)
				{
					TMP_SpriteAsset.m_defaultSpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/Default Sprite Asset");
				}
				return TMP_SpriteAsset.m_defaultSpriteAsset;
			}
		}

		private void OnEnable()
		{
		}

		private Material GetDefaultSpriteMaterial()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			Shader shader = Shader.Find("TMPro/Sprite");
			Material material = new Material(shader);
			material.SetTexture(ShaderUtilities.ID_MainTex, this.spriteSheet);
			material.hideFlags = HideFlags.HideInHierarchy;
			return material;
		}

		public int GetSpriteIndex(int hashCode)
		{
			for (int i = 0; i < this.spriteInfoList.Count; i++)
			{
				if (this.spriteInfoList[i].hashCode == hashCode)
				{
					return i;
				}
			}
			return -1;
		}

		public static TMP_SpriteAsset m_defaultSpriteAsset;

		public Texture spriteSheet;

		public List<TMP_Sprite> spriteInfoList;

		private List<Sprite> m_sprites;
	}
}
