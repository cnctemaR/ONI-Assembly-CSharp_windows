using System;
using UnityEngine;

public class FillRenderTargetEffect : MonoBehaviour
{
	public void SetFillTexture(Texture tex)
	{
		this.fillTexture = tex;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(this.fillTexture, null);
	}

	private Texture fillTexture;
}
