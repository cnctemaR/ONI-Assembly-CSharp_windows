using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LiquidShaderProperties", menuName = "Klei/Liquid Shader Properties")]
public class LiquidShaderProperties : ScriptableObject
{
	public void ApplyToMaterial(Material material)
	{
		if (this.cachedScrollSpeeds == null)
		{
			this.cachedScrollSpeeds = new float[LiquidShaderProperties.SubstanceTextureCount];
		}
		Array.Clear(this.cachedScrollSpeeds, 0, this.cachedScrollSpeeds.Length);
		for (int i = 0; i < this.TextureScrollSpeed.Length; i++)
		{
			int num = (int)(this.TextureScrollSpeed[i].texture - Substance.SubstanceTexture.Magma);
			if (num >= 0 && num < LiquidShaderProperties.SubstanceTextureCount)
			{
				this.cachedScrollSpeeds[num] = this.TextureScrollSpeed[i].scrollSpeed;
			}
		}
		material.SetFloatArray("_TextureScrollSpeeds", this.cachedScrollSpeeds);
	}

	[SerializeField]
	private LiquidShaderProperties.Entry[] TextureScrollSpeed = new LiquidShaderProperties.Entry[]
	{
		new LiquidShaderProperties.Entry
		{
			texture = Substance.SubstanceTexture.Magma,
			scrollSpeed = 0.02f
		},
		new LiquidShaderProperties.Entry
		{
			texture = Substance.SubstanceTexture.MoltenMetal,
			scrollSpeed = 0.02f
		},
		new LiquidShaderProperties.Entry
		{
			texture = Substance.SubstanceTexture.Polluted,
			scrollSpeed = 0.02f
		},
		new LiquidShaderProperties.Entry
		{
			texture = Substance.SubstanceTexture.Oil,
			scrollSpeed = 0.02f
		},
		new LiquidShaderProperties.Entry
		{
			texture = Substance.SubstanceTexture.Thick,
			scrollSpeed = 0.02f
		},
		new LiquidShaderProperties.Entry
		{
			texture = Substance.SubstanceTexture.Sap,
			scrollSpeed = 0.02f
		},
		new LiquidShaderProperties.Entry
		{
			texture = Substance.SubstanceTexture.CrystalFragments,
			scrollSpeed = 0.01f
		}
	};

	private static readonly int SubstanceTextureCount = Enum.GetValues(typeof(Substance.SubstanceTexture)).Length;

	private float[] cachedScrollSpeeds;

	[Serializable]
	public struct Entry
	{
		public Substance.SubstanceTexture texture;

		public float scrollSpeed;
	}
}
