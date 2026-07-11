using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Color Adjustments/Color Correction (3D Lookup Texture)")]
	public class ColorCorrectionLookup : PostEffectsBase
	{
		private void Awake()
		{
			this.supports3dTextures = SystemInfo.supports3DTextures;
			base.CheckSupport(false);
		}

		public override bool CheckResources()
		{
			this.material = base.CheckShaderAndCreateMaterial(this.shader, this.material);
			if (!this.isSupported || !this.supports3dTextures)
			{
				base.ReportAutoDisable();
			}
			return this.isSupported;
		}

		private void OnDisable()
		{
			if (this.material)
			{
				global::UnityEngine.Object.DestroyImmediate(this.material);
				this.material = null;
			}
		}

		private void OnDestroy()
		{
			if (this.converted3DLut)
			{
				global::UnityEngine.Object.DestroyImmediate(this.converted3DLut);
			}
			if (this.converted3DLut2)
			{
				global::UnityEngine.Object.DestroyImmediate(this.converted3DLut2);
			}
			this.converted3DLut = null;
			this.converted3DLut2 = null;
		}

		public void SetIdentityLut()
		{
			this.SetIdentityLut(ref this.converted3DLut);
		}

		public void SetIdentityLut2()
		{
			this.SetIdentityLut(ref this.converted3DLut);
		}

		private void SetIdentityLut(ref Texture3D target)
		{
			int num = 16;
			Color[] array = new Color[num * num * num];
			float num2 = 1f / (1f * (float)num - 1f);
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num; j++)
				{
					for (int k = 0; k < num; k++)
					{
						array[i + j * num + k * num * num] = new Color((float)i * 1f * num2, (float)j * 1f * num2, (float)k * 1f * num2, 1f);
					}
				}
			}
			if (target)
			{
				global::UnityEngine.Object.DestroyImmediate(target);
			}
			target = new Texture3D(num, num, num, TextureFormat.ARGB32, false);
			target.SetPixels(array);
			target.Apply();
			this.basedOnTempTex = string.Empty;
		}

		public bool ValidDimensions(Texture2D tex2d)
		{
			if (!tex2d)
			{
				return false;
			}
			int height = tex2d.height;
			return height == Mathf.FloorToInt(Mathf.Sqrt((float)tex2d.width));
		}

		public void Convert(Texture2D temp2DTex, string path)
		{
			this.Convert(temp2DTex, path, ref this.converted3DLut);
		}

		public void Convert2(Texture2D temp2DTex, string path)
		{
			this.Convert(temp2DTex, path, ref this.converted3DLut2);
		}

		private void Convert(Texture2D temp2DTex, string path, ref Texture3D target)
		{
			if (temp2DTex)
			{
				int num = temp2DTex.width * temp2DTex.height;
				num = temp2DTex.height;
				if (!this.ValidDimensions(temp2DTex))
				{
					global::Debug.LogWarning("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT.");
					this.basedOnTempTex = string.Empty;
					return;
				}
				Color[] pixels = temp2DTex.GetPixels();
				Color[] array = new Color[pixels.Length];
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < num; j++)
					{
						for (int k = 0; k < num; k++)
						{
							int num2 = num - j - 1;
							array[i + j * num + k * num * num] = pixels[k * num + i + num2 * num * num];
						}
					}
				}
				if (target)
				{
					global::UnityEngine.Object.DestroyImmediate(target);
				}
				target = new Texture3D(num, num, num, TextureFormat.ARGB32, false);
				target.SetPixels(array);
				target.Apply();
				this.basedOnTempTex = path;
			}
			else
			{
				global::Debug.LogError("Couldn't color correct with 3D LUT texture. Image Effect will be disabled.");
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.CheckResources() || !this.supports3dTextures)
			{
				Graphics.Blit(source, destination);
				return;
			}
			if (this.converted3DLut == null)
			{
				this.SetIdentityLut();
			}
			if (this.converted3DLut2 == null)
			{
				this.SetIdentityLut2();
			}
			int width = this.converted3DLut.width;
			this.converted3DLut.wrapMode = TextureWrapMode.Clamp;
			this.material.SetFloat("_Scale", (float)(width - 1) / (1f * (float)width));
			this.material.SetFloat("_Offset", 1f / (2f * (float)width));
			this.material.SetTexture("_ClutTex", this.converted3DLut);
			this.material.SetTexture("_ClutTex2", this.converted3DLut2);
			Graphics.Blit(source, destination, this.material, (QualitySettings.activeColorSpace != ColorSpace.Linear) ? 0 : 1);
		}

		public Shader shader;

		private Material material;

		public Texture3D converted3DLut;

		public Texture3D converted3DLut2;

		public string basedOnTempTex = string.Empty;

		private bool supports3dTextures;
	}
}
