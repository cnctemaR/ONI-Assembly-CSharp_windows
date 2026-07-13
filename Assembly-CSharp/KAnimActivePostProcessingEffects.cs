using System;
using UnityEngine;

public class KAnimActivePostProcessingEffects : KMonoBehaviour
{
	public void EnableEffect(KAnimConverter.PostProcessingEffects effect_flag)
	{
		this.currentActiveEffects |= effect_flag;
	}

	public void DisableEffect(KAnimConverter.PostProcessingEffects effect_flag)
	{
		if (this.IsEffectActive(effect_flag))
		{
			this.currentActiveEffects ^= effect_flag;
		}
	}

	public bool IsEffectActive(KAnimConverter.PostProcessingEffects effect_flag)
	{
		return (this.currentActiveEffects & effect_flag) > (KAnimConverter.PostProcessingEffects)0;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination);
		if (this.currentActiveEffects != (KAnimConverter.PostProcessingEffects)0)
		{
			KAnimBatchManager.Instance().RenderKAnimPostProcessingEffects(this.currentActiveEffects);
		}
	}

	private KAnimConverter.PostProcessingEffects currentActiveEffects;
}
