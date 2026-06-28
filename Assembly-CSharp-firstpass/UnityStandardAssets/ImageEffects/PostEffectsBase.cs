using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class PostEffectsBase : MonoBehaviour
	{
		protected Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
		{
			Material material;
			if (!s)
			{
				global::Debug.Log("Missing shader in " + this.ToString(), null);
				base.enabled = false;
				material = null;
			}
			else if (s.isSupported && m2Create && m2Create.shader == s)
			{
				material = m2Create;
			}
			else if (!s.isSupported)
			{
				this.NotSupported();
				global::Debug.Log(string.Concat(new string[]
				{
					"The shader ",
					s.ToString(),
					" on effect ",
					this.ToString(),
					" is not supported on this platform!"
				}), null);
				material = null;
			}
			else
			{
				m2Create = new Material(s);
				m2Create.hideFlags = HideFlags.DontSave;
				if (m2Create)
				{
					material = m2Create;
				}
				else
				{
					material = null;
				}
			}
			return material;
		}

		protected Material CreateMaterial(Shader s, Material m2Create)
		{
			Material material;
			if (!s)
			{
				global::Debug.Log("Missing shader in " + this.ToString(), null);
				material = null;
			}
			else if (m2Create && m2Create.shader == s && s.isSupported)
			{
				material = m2Create;
			}
			else if (!s.isSupported)
			{
				material = null;
			}
			else
			{
				m2Create = new Material(s);
				m2Create.hideFlags = HideFlags.DontSave;
				if (m2Create)
				{
					material = m2Create;
				}
				else
				{
					material = null;
				}
			}
			return material;
		}

		private void OnEnable()
		{
			this.isSupported = true;
		}

		protected bool CheckSupport()
		{
			return this.CheckSupport(false);
		}

		public virtual bool CheckResources()
		{
			global::Debug.LogWarning("CheckResources () for " + this.ToString() + " should be overwritten.", null);
			return this.isSupported;
		}

		protected void Start()
		{
			this.CheckResources();
		}

		protected bool CheckSupport(bool needDepth)
		{
			this.isSupported = true;
			this.supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
			this.supportDX11 = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
			bool flag;
			if (!SystemInfo.supportsImageEffects)
			{
				this.NotSupported();
				flag = false;
			}
			else if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
			{
				this.NotSupported();
				flag = false;
			}
			else
			{
				if (needDepth)
				{
					base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
				}
				flag = true;
			}
			return flag;
		}

		protected bool CheckSupport(bool needDepth, bool needHdr)
		{
			bool flag;
			if (!this.CheckSupport(needDepth))
			{
				flag = false;
			}
			else if (needHdr && !this.supportHDRTextures)
			{
				this.NotSupported();
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public bool Dx11Support()
		{
			return this.supportDX11;
		}

		protected void ReportAutoDisable()
		{
			global::Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.", null);
		}

		private bool CheckShader(Shader s)
		{
			global::Debug.Log(string.Concat(new string[]
			{
				"The shader ",
				s.ToString(),
				" on effect ",
				this.ToString(),
				" is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package."
			}), null);
			bool flag;
			if (!s.isSupported)
			{
				this.NotSupported();
				flag = false;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		protected void NotSupported()
		{
			base.enabled = false;
			this.isSupported = false;
		}

		protected void DrawBorder(RenderTexture dest, Material material)
		{
			RenderTexture.active = dest;
			bool flag = true;
			GL.PushMatrix();
			GL.LoadOrtho();
			for (int i = 0; i < material.passCount; i++)
			{
				material.SetPass(i);
				float num;
				float num2;
				if (flag)
				{
					num = 1f;
					num2 = 0f;
				}
				else
				{
					num = 0f;
					num2 = 1f;
				}
				float num3 = 0f;
				float num4 = 1f / ((float)dest.width * 1f);
				float num5 = 0f;
				float num6 = 1f;
				GL.Begin(7);
				GL.TexCoord2(0f, num);
				GL.Vertex3(num3, num5, 0.1f);
				GL.TexCoord2(1f, num);
				GL.Vertex3(num4, num5, 0.1f);
				GL.TexCoord2(1f, num2);
				GL.Vertex3(num4, num6, 0.1f);
				GL.TexCoord2(0f, num2);
				GL.Vertex3(num3, num6, 0.1f);
				num3 = 1f - 1f / ((float)dest.width * 1f);
				num4 = 1f;
				num5 = 0f;
				num6 = 1f;
				GL.TexCoord2(0f, num);
				GL.Vertex3(num3, num5, 0.1f);
				GL.TexCoord2(1f, num);
				GL.Vertex3(num4, num5, 0.1f);
				GL.TexCoord2(1f, num2);
				GL.Vertex3(num4, num6, 0.1f);
				GL.TexCoord2(0f, num2);
				GL.Vertex3(num3, num6, 0.1f);
				num3 = 0f;
				num4 = 1f;
				num5 = 0f;
				num6 = 1f / ((float)dest.height * 1f);
				GL.TexCoord2(0f, num);
				GL.Vertex3(num3, num5, 0.1f);
				GL.TexCoord2(1f, num);
				GL.Vertex3(num4, num5, 0.1f);
				GL.TexCoord2(1f, num2);
				GL.Vertex3(num4, num6, 0.1f);
				GL.TexCoord2(0f, num2);
				GL.Vertex3(num3, num6, 0.1f);
				num3 = 0f;
				num4 = 1f;
				num5 = 1f - 1f / ((float)dest.height * 1f);
				num6 = 1f;
				GL.TexCoord2(0f, num);
				GL.Vertex3(num3, num5, 0.1f);
				GL.TexCoord2(1f, num);
				GL.Vertex3(num4, num5, 0.1f);
				GL.TexCoord2(1f, num2);
				GL.Vertex3(num4, num6, 0.1f);
				GL.TexCoord2(0f, num2);
				GL.Vertex3(num3, num6, 0.1f);
				GL.End();
			}
			GL.PopMatrix();
		}

		protected bool supportHDRTextures = true;

		protected bool supportDX11 = false;

		protected bool isSupported = true;
	}
}
