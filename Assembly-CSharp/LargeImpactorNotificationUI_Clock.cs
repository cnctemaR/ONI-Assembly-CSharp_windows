using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LargeImpactorNotificationUI_Clock : KMonoBehaviour, ISim4000ms
{
	protected override void OnSpawn()
	{
		this.color_circleFillOriginalColor = this.TimerOutCircleFill.color;
		this.color_needleTrailBgOriginalColor = this.NeedleTrailBg.color;
		this.color_timerOutCircleBGOriginalColor = this.TimerOutCircleBG.color;
		this.softRed = new Color(1f, 0f, 0f, this.color_needleTrailBgOriginalColor.a);
		GameClock.Instance.Subscribe(631075836, new Action<object>(this.OnNewCycleReached));
		this.UpdateSmallNeedlePosition();
		this.InitializeAnimationCoroutine();
		this.hasSpawned = true;
	}

	private void OnNewCycleReached(object data)
	{
		this.PlayReminderAnimation();
	}

	public void SetLargeImpactorTime(float normalizedValue)
	{
		this.lastLargeImpactorTime = normalizedValue;
		this.SetNeedleRotation(this.LargeNeedle.rectTransform, 1f - this.lastLargeImpactorTime);
		if (!this.hasPlayedEntryAnimation)
		{
			return;
		}
		this.TimerOutCircleFill.fillAmount = this.lastLargeImpactorTime;
	}

	private void SetNeedleRotation(RectTransform needle, float normalizedTime)
	{
		needle.localRotation = Quaternion.Euler(0f, 0f, -360f * normalizedTime);
		if (needle.gameObject == this.LargeNeedle.gameObject)
		{
			this.NeedleTrailBg.fillAmount = normalizedTime;
		}
	}

	public void Sim4000ms(float dt)
	{
		this.UpdateSmallNeedlePosition();
	}

	private void UpdateSmallNeedlePosition()
	{
		float currentCycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
		this.SetNeedleRotation(this.SmallNeedlePivot, currentCycleAsPercentage);
	}

	private void InitializeAnimationCoroutine()
	{
		this.AbortCoroutine();
		this.animationCoroutine = base.StartCoroutine(this.AnimationCoroutineLogic());
	}

	private void AbortCoroutine()
	{
		if (this.animationCoroutine != null)
		{
			base.StopAllCoroutines();
		}
		this.animationCoroutine = null;
	}

	public void PlayReminderAnimation()
	{
		this.reminderAnimationTimer = 0f;
	}

	private IEnumerator AnimationCoroutineLogic()
	{
		if (!this.hasPlayedEntryAnimation)
		{
			GameClock.Instance.GetCurrentCycleAsPercentage();
			float num = this.entryAnimationDuration / 600f;
			yield return this.Interpolate(delegate(float n)
			{
				this.TimerOutCircleFill.fillAmount = n * this.lastLargeImpactorTime;
			}, this.entryAnimationDuration, null);
			this.hasPlayedEntryAnimation = true;
		}
		for (;;)
		{
			if (this.reminderAnimationTimer < 0f)
			{
				yield return null;
			}
			if (this.reminderAnimationTimer >= 0f && this.reminderAnimationTimer < this.reminderAnimationDuration)
			{
				float num2 = Mathf.Abs(Mathf.Sin(this.reminderAnimationTimer / this.reminderAnimationDuration * 3.1415927f * 16f));
				this.TimerOutCircleBG.color = Color.Lerp(this.color_timerOutCircleBGOriginalColor, Color.red, num2);
				this.NeedleTrailBg.color = Color.Lerp(this.color_needleTrailBgOriginalColor, this.softRed, num2);
				this.reminderAnimationTimer += Time.deltaTime;
				yield return null;
			}
			if (this.reminderAnimationTimer >= this.reminderAnimationDuration)
			{
				this.TimerOutCircleBG.color = this.color_timerOutCircleBGOriginalColor;
				this.NeedleTrailBg.color = this.color_needleTrailBgOriginalColor;
				this.reminderAnimationTimer = -1f;
				yield return null;
			}
		}
		yield break;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (this.hasSpawned)
		{
			this.InitializeAnimationCoroutine();
		}
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		this.AbortCoroutine();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameClock.Instance.Unsubscribe(631075836, new Action<object>(this.OnNewCycleReached));
	}

	public KImage LargeNeedle;

	public RectTransform SmallNeedlePivot;

	public KImage NeedleTrailBg;

	public Image TimerOutCircleFill;

	public Image TimerOutCircleBG;

	private Color color_circleFillOriginalColor;

	private Color color_needleTrailBgOriginalColor;

	private Color color_timerOutCircleBGOriginalColor;

	private Color softRed;

	private Coroutine animationCoroutine;

	private bool hasSpawned;

	private float entryAnimationDuration = 1f;

	private float reminderAnimationDuration = 16f;

	private bool hasPlayedEntryAnimation;

	private float reminderAnimationTimer = -1f;

	private float lastLargeImpactorTime = -1f;

	private const int reminderSetting_BlinkTimes = 16;
}
