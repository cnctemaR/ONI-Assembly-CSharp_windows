using System;
using UnityEngine;

public class LightBufferCompositor : MonoBehaviour
{
	private void Start()
	{
		this.material = new Material(Shader.Find("Klei/PostFX/LightBufferCompositor"));
		this.material.SetTexture("_InvalidTex", Assets.instance.invalidAreaTex);
		this.blurMaterial = new Material(Shader.Find("Klei/PostFX/Blur"));
		this.OnShadersReloaded();
		ShaderReloader.Register(new global::System.Action(this.OnShadersReloaded));
	}

	private void OnEnable()
	{
		this.OnShadersReloaded();
	}

	private void DownSample4x(Texture source, RenderTexture dest)
	{
		float num = 1f;
		Graphics.BlitMultiTap(source, dest, this.blurMaterial, new Vector2[]
		{
			new Vector2(-num, -num),
			new Vector2(-num, num),
			new Vector2(num, num),
			new Vector2(num, -num)
		});
	}

	[ContextMenu("ToggleParticles")]
	private void ToggleParticles()
	{
		this.particlesEnabled = !this.particlesEnabled;
		this.UpdateMaterialState();
	}

	public void SetParticlesEnabled(bool enabled)
	{
		this.particlesEnabled = enabled;
		this.UpdateMaterialState();
	}

	private void UpdateMaterialState()
	{
		if (this.particlesEnabled)
		{
			this.material.DisableKeyword("DISABLE_TEMPERATURE_PARTICLES");
			return;
		}
		this.material.EnableKeyword("DISABLE_TEMPERATURE_PARTICLES");
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		RenderTexture renderTexture = null;
		if (PropertyTextures.instance != null)
		{
			Texture texture = PropertyTextures.instance.GetTexture(PropertyTextures.Property.Temperature);
			texture.name = "temperature_tex";
			renderTexture = RenderTexture.GetTemporary(Screen.width / 8, Screen.height / 8);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(texture, renderTexture, this.blurMaterial);
			Shader.SetGlobalTexture("_BlurredTemperature", renderTexture);
		}
		this.material.SetTexture("_LightBufferTex", LightBuffer.Instance.Texture);
		Graphics.Blit(src, dest, this.material);
		if (renderTexture != null)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	private void OnShadersReloaded()
	{
		if (this.material != null && Lighting.Instance != null)
		{
			this.material.SetTexture("_EmberTex", Lighting.Instance.Settings.EmberTex);
			this.material.SetTexture("_FrostTex", Lighting.Instance.Settings.FrostTex);
			this.material.SetTexture("_Thermal1Tex", Lighting.Instance.Settings.Thermal1Tex);
			this.material.SetTexture("_Thermal2Tex", Lighting.Instance.Settings.Thermal2Tex);
			this.material.SetTexture("_RadHaze1Tex", Lighting.Instance.Settings.Radiation1Tex);
			this.material.SetTexture("_RadHaze2Tex", Lighting.Instance.Settings.Radiation2Tex);
			this.material.SetTexture("_RadHaze3Tex", Lighting.Instance.Settings.Radiation3Tex);
			this.material.SetTexture("_NoiseTex", Lighting.Instance.Settings.NoiseTex);
		}
	}

	[SerializeField]
	private Material material;

	[SerializeField]
	private Material blurMaterial;

	private bool particlesEnabled = true;
}
