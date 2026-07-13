using System;
using UnityEngine;
using UnityEngine.UI;

public class LargeImpactorUINotificationHitEffects : KMonoBehaviour, IRenderEveryTick
{
	private float Intensity
	{
		get
		{
			return this.timer / this.duration;
		}
	}

	public void PlayHitEffect()
	{
		this.timer = this.duration;
	}

	public void RenderEveryTick(float dt)
	{
		if (this.lastTimerValue != this.timer)
		{
			this.lastTimerValue = this.timer;
			this.hitBackgorund.Opacity(this.Intensity);
			this.heartIcon.color = Color.Lerp(this.heartIconOriginalColor, this.HighlightedColor, this.Intensity);
			this.healthBarFill.color = Color.Lerp(this.healthBarOriginalColor, this.HighlightedColor, this.Intensity);
			this.shake.SetIntensity(this.Intensity);
		}
		this.timer = Mathf.Clamp(this.timer - dt, 0f, this.duration);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.heartIconOriginalColor = this.heartIcon.color;
		this.healthBarOriginalColor = this.healthBarFill.color;
	}

	public Image hitBackgorund;

	public Image heartIcon;

	public Image healthBarFill;

	public UIShake shake;

	public Color HighlightedColor = Color.yellow;

	private Color heartIconOriginalColor;

	private Color healthBarOriginalColor;

	private float duration = 0.4f;

	private float lastTimerValue;

	private float timer;
}
