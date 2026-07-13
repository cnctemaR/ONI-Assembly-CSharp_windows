using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class IncrementorToggle : MultiToggle
{
	protected override void Update()
	{
		if (this.clickHeldDown)
		{
			this.totalHeldTime += Time.unscaledDeltaTime;
			if (this.timeToNextIncrement <= 0f)
			{
				this.PlayClickSound();
				this.onClick();
				this.timeToNextIncrement = Mathf.Lerp(this.timeBetweenIncrementsMax, this.timeBetweenIncrementsMin, this.totalHeldTime / 2.5f);
				return;
			}
			this.timeToNextIncrement -= Time.unscaledDeltaTime;
		}
	}

	private void PlayClickSound()
	{
		if (!this.play_sound_on_click)
		{
			return;
		}
		string text = this.states[this.state].on_click_override_sound_path;
		if (text == "")
		{
			text = "HUD_Click";
		}
		KFMOD.PlayUISound(GlobalAssets.GetSound(text, false));
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		if (base.AllowClick(eventData))
		{
			this.timeToNextIncrement = this.timeBetweenIncrementsMax;
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (!base.AllowClick(eventData))
		{
			return;
		}
		if (!this.clickHeldDown)
		{
			this.clickHeldDown = true;
			this.PlayClickSound();
			if (this.onClick != null)
			{
				this.onClick();
			}
		}
		if (this.states.Length - 1 < this.state)
		{
			global::Debug.LogWarning("Multi toggle has too few / no states");
		}
		base.RefreshHoverColor();
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.RefreshHoverColor();
	}

	private float timeBetweenIncrementsMin = 0.033f;

	private float timeBetweenIncrementsMax = 0.25f;

	private const float incrementAccelerationScale = 2.5f;

	private float timeToNextIncrement;
}
