using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KToggleSlider : Toggle
{
	protected override void Start()
	{
		base.Start();
		this.hasSpawned = true;
		if (base.enabled)
		{
			this.timer = (base.isOn ? this.offEffectDuration : 0f);
			this.InitializeAnimationCoroutine();
		}
	}

	private IEnumerator Animations()
	{
		float previousTime = -1f;
		Vector2 dotPosition = this.dot.rectTransform.anchoredPosition;
		for (;;)
		{
			this.timer += Time.unscaledDeltaTime * (float)(base.isOn ? 1 : (-1));
			this.timer = Mathf.Clamp(this.timer, 0f, Mathf.Max(this.onEffectDuration, this.offEffectDuration));
			if (previousTime != this.timer)
			{
				float num = Mathf.Clamp(this.timer / (base.isOn ? this.onEffectDuration : this.offEffectDuration), 0f, 1f);
				this.fill.fillAmount = num;
				this.fill.color = Color.Lerp(this.color_Fill_OFF, this.color_Fill_ON, num);
				this.dot.color = Color.Lerp(this.color_Dot_OFF, this.color_Dot_ON, num);
				this.glow.color = Color.Lerp(this.color_Glow_OFF, this.color_Glow_ON, Mathf.Sin(num * 3.1415927f));
				dotPosition.x = Mathf.Lerp(0f, this.dot.transform.parent.rectTransform().rect.width - this.dot.rectTransform.rect.width, num);
				this.dot.rectTransform.anchoredPosition = dotPosition;
			}
			previousTime = this.timer;
			yield return null;
		}
		yield break;
	}

	private void AbortAnimationCoroutine()
	{
		if (this.animationCoroutine != null)
		{
			base.StopCoroutine(this.animationCoroutine);
		}
		this.animationCoroutine = null;
	}

	private void InitializeAnimationCoroutine()
	{
		if (Application.isPlaying)
		{
			this.AbortAnimationCoroutine();
			this.animationCoroutine = base.StartCoroutine(this.Animations());
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.hasSpawned)
		{
			this.InitializeAnimationCoroutine();
		}
	}

	protected override void OnDisable()
	{
		this.AbortAnimationCoroutine();
		base.OnDisable();
	}

	[SerializeField]
	public Color color_Dot_ON;

	[SerializeField]
	public Color color_Fill_ON;

	[SerializeField]
	public Color color_Glow_ON;

	[SerializeField]
	public Color color_Dot_OFF;

	[SerializeField]
	public Color color_Fill_OFF;

	[SerializeField]
	public Color color_Glow_OFF;

	[SerializeField]
	public Image fill;

	[SerializeField]
	public Image dot;

	[SerializeField]
	public Image glow;

	public float onEffectDuration = 0.3f;

	public float offEffectDuration = 0.3f;

	private Coroutine animationCoroutine;

	private bool hasSpawned;

	private float timer;
}
