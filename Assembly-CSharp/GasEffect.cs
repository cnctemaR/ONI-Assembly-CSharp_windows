using System;
using UnityEngine;

public class GasEffect : KMonoBehaviour
{
	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		this.Material.SetTexture("_LightBufferTex", LightBuffer.Instance.Texture);
		Graphics.Blit(source, dest, this.Material);
	}

	public Material Material;
}
