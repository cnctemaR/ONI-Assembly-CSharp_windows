using System;
using UnityEngine;

public class BloomEffect : MonoBehaviour
{
	protected Material material
	{
		get
		{
			if (BloomEffect.m_Material == null)
			{
				BloomEffect.m_Material = new Material(this.blurShader);
				BloomEffect.m_Material.hideFlags = HideFlags.DontSave;
			}
			return BloomEffect.m_Material;
		}
	}

	protected void OnDisable()
	{
		if (BloomEffect.m_Material)
		{
			global::UnityEngine.Object.DestroyImmediate(BloomEffect.m_Material);
		}
	}

	protected void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
		}
		else if (!this.blurShader || !this.material.shader.isSupported)
		{
			base.enabled = false;
		}
		else
		{
			this.BloomMaskMaterial = new Material(Shader.Find("Klei/PostFX/BloomMask"));
			this.BloomCompositeMaterial = new Material(Shader.Find("Klei/PostFX/BloomComposite"));
		}
	}

	public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
	{
		float num = 0.5f + (float)iteration * this.blurSpread;
		Graphics.BlitMultiTap(source, dest, this.material, new Vector2[]
		{
			new Vector2(-num, -num),
			new Vector2(-num, num),
			new Vector2(num, num),
			new Vector2(num, -num)
		});
	}

	private void DownSample4x(RenderTexture source, RenderTexture dest)
	{
		float num = 1f;
		Graphics.BlitMultiTap(source, dest, this.material, new Vector2[]
		{
			new Vector2(-num, -num),
			new Vector2(-num, num),
			new Vector2(num, num),
			new Vector2(num, -num)
		});
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
		temporary.name = "bloom_source";
		Graphics.Blit(source, temporary, this.BloomMaskMaterial);
		int num = source.width / 4;
		int num2 = source.height / 4;
		RenderTexture renderTexture = RenderTexture.GetTemporary(num, num2, 0);
		renderTexture.name = "bloom_downsampled";
		this.DownSample4x(temporary, renderTexture);
		RenderTexture.ReleaseTemporary(temporary);
		for (int i = 0; i < this.iterations; i++)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(num, num2, 0);
			temporary2.name = "bloom_blurred";
			this.FourTapCone(renderTexture, temporary2, i);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary2;
		}
		this.BloomCompositeMaterial.SetTexture("_BloomTex", renderTexture);
		Graphics.Blit(source, destination, this.BloomCompositeMaterial);
		RenderTexture.ReleaseTemporary(renderTexture);
	}

	private Material BloomMaskMaterial;

	private Material BloomCompositeMaterial;

	public int iterations = 3;

	public float blurSpread = 0.6f;

	public Shader blurShader = null;

	private static Material m_Material = null;
}
