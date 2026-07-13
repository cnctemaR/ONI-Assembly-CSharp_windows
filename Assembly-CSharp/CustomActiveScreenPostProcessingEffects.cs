using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomActiveScreenPostProcessingEffects : MonoBehaviour
{
	public void RegisterEffect(Func<RenderTexture, Material> effectFn)
	{
		this.ActiveEffects.Add(effectFn);
	}

	public void UnregisterEffect(Func<RenderTexture, Material> effectFn)
	{
		this.ActiveEffects.Remove(effectFn);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CheckTemporaryRenderTextureValidity(ref this.previousSource, source);
		this.CheckTemporaryRenderTextureValidity(ref this.previousDestination, source);
		Graphics.Blit(source, this.previousSource);
		foreach (Func<RenderTexture, Material> func in this.ActiveEffects)
		{
			Graphics.Blit(this.previousSource, this.previousDestination, func(source));
			this.previousSource.DiscardContents();
			Graphics.Blit(this.previousDestination, this.previousSource);
		}
		Graphics.Blit(this.previousSource, destination);
		this.previousSource.Release();
		this.previousDestination.Release();
	}

	private void CheckTemporaryRenderTextureValidity(ref RenderTexture temporaryTexture, RenderTexture source)
	{
		if (temporaryTexture == null || temporaryTexture.width != source.width || temporaryTexture.height != source.height || temporaryTexture.depth != source.depth || temporaryTexture.format != source.format)
		{
			if (temporaryTexture != null)
			{
				temporaryTexture.Release();
			}
			temporaryTexture = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);
		}
	}

	private List<Func<RenderTexture, Material>> ActiveEffects = new List<Func<RenderTexture, Material>>();

	private RenderTexture previousSource;

	private RenderTexture previousDestination;
}
