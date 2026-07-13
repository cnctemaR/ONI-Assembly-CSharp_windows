using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TemporaryActionRow : KMonoBehaviour, IRender200ms
{
	public float MaxHeight { get; private set; }

	public bool IsVisible { get; private set; }

	public bool ShouldProgressBarBeEnabled
	{
		get
		{
			return this.ShowTimeout && this.Lifetime > 0f && this.lastSpecifiedLifetime > 0f;
		}
	}

	public float Lifetime { get; private set; } = -1f;

	public bool ShowTimeout { get; set; } = true;

	public bool ShowOnSpawn { get; set; } = true;

	public bool HideOnClick { get; set; } = true;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.layoutElement = base.GetComponent<LayoutElement>();
		this.button = base.GetComponent<Button>();
		this.button.onClick.AddListener(new UnityAction(this._OnRowClicked));
		this.MaxHeight = this.layoutElement.minHeight;
		this.HideImmediatly();
	}

	private void Update()
	{
		if (!this.HasBeenShown && this.ShowOnSpawn)
		{
			this.RefreshContentWidth();
			if (this.Content.sizeDelta.x > 0f)
			{
				this.Show();
			}
		}
	}

	private void _OnRowClicked()
	{
		Action<TemporaryActionRow> onRowClicked = this.OnRowClicked;
		if (onRowClicked != null)
		{
			onRowClicked(this);
		}
		if (this.HideOnClick)
		{
			this.Hide();
		}
	}

	private void _OnRowHidden()
	{
		Action<TemporaryActionRow> onRowHidden = this.OnRowHidden;
		if (onRowHidden == null)
		{
			return;
		}
		onRowHidden(this);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (base.isSpawned)
		{
			this.RefreshContentWidth();
		}
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		this.HideImmediatly();
		this._OnRowHidden();
	}

	public void SetLifetime(float lifetime)
	{
		this.Lifetime = lifetime;
		this.lastSpecifiedLifetime = lifetime;
		this.UpdateTimeout();
	}

	private void UpdateTimeout()
	{
		bool shouldProgressBarBeEnabled = this.ShouldProgressBarBeEnabled;
		if (shouldProgressBarBeEnabled != this.TimeoutBarSection.gameObject.activeInHierarchy)
		{
			this.TimeoutBarSection.gameObject.SetActive(shouldProgressBarBeEnabled);
		}
		if (shouldProgressBarBeEnabled)
		{
			this.TimeoutImage.fillAmount = Mathf.Clamp(this.Lifetime / this.lastSpecifiedLifetime, 0f, 1f);
		}
	}

	public void Render200ms(float dt)
	{
		if (this.HasBeenShown && this.Lifetime > 0f && this.IsVisible)
		{
			this.Lifetime -= dt;
			if (this.Lifetime <= 0f)
			{
				this.Hide();
			}
			this.UpdateTimeout();
		}
	}

	public void Setup(string text, string tooltip, Sprite icon = null)
	{
		this.Label.SetText(text);
		this.Tooltip.SetSimpleTooltip(tooltip);
		this.Image.sprite = icon;
		this.IconSection.gameObject.SetActive(icon != null);
	}

	public void Show()
	{
		this.AbortCoroutine();
		this.IsVisible = true;
		this.HasBeenShown = true;
		this.button.interactable = true;
		if (base.gameObject.activeInHierarchy)
		{
			this.SetContentToHiddenPosition();
			this.layoutCoroutine = this.RunEnterHeightAnimation(delegate
			{
				this.layoutCoroutine = this.RunEnterSlideAnimation(null);
			});
		}
	}

	public void HideImmediatly()
	{
		this.AbortCoroutine();
		this.IsVisible = false;
		this.Content.localPosition = new Vector3(-(base.transform as RectTransform).sizeDelta.x, this.Content.localPosition.y, this.Content.localPosition.z);
		this.layoutElement.minHeight = 0f;
		this.button.interactable = false;
	}

	public void Hide()
	{
		this.AbortCoroutine();
		this.IsVisible = false;
		this.button.interactable = false;
		if (base.gameObject.activeInHierarchy)
		{
			this.layoutCoroutine = this.RunExitSlideAnimation(delegate
			{
				this.layoutCoroutine = this.RunExitHeightAnimation(new global::System.Action(this._OnRowHidden));
			});
		}
	}

	private void AbortCoroutine()
	{
		if (this.layoutCoroutine != null)
		{
			base.StopCoroutine(this.layoutCoroutine);
			this.layoutCoroutine = null;
		}
	}

	private void RefreshContentWidth()
	{
		RectTransform rectTransform = base.transform as RectTransform;
		if (rectTransform.sizeDelta.x != this.Content.sizeDelta.x)
		{
			this.Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x);
		}
	}

	private void SetContentToHiddenPosition()
	{
		this.RefreshContentWidth();
		Vector3 vector = this.Content.anchoredPosition;
		vector.x = -(base.transform as RectTransform).sizeDelta.x;
		this.Content.anchoredPosition = vector;
	}

	private Coroutine RunEnterSlideAnimation(global::System.Action onAnimationEnds = null)
	{
		return base.StartCoroutine(this.SlideTransitionAnimation(0.4f, true, (float n) => Mathf.Sqrt(n), onAnimationEnds));
	}

	private Coroutine RunExitSlideAnimation(global::System.Action onAnimationEnds = null)
	{
		return base.StartCoroutine(this.SlideTransitionAnimation(0.4f, false, (float n) => Mathf.Pow(n, 2f), onAnimationEnds));
	}

	private Coroutine RunEnterHeightAnimation(global::System.Action onAnimationEnds = null)
	{
		return base.StartCoroutine(this.HeightTransitionAnimation(0.5f, true, (float n) => Mathf.Sqrt(n), onAnimationEnds));
	}

	private Coroutine RunExitHeightAnimation(global::System.Action onAnimationEnds = null)
	{
		return base.StartCoroutine(this.HeightTransitionAnimation(0.3f, false, (float n) => Mathf.Pow(n, 2f), onAnimationEnds));
	}

	private IEnumerator SlideTransitionAnimation(float duration, bool show, Func<float, float> curveModifier = null, global::System.Action onAnimationEnds = null)
	{
		float num = -(base.transform as RectTransform).sizeDelta.x;
		float num2 = 0f;
		float contentInitialXPosition = (show ? num : this.Content.anchoredPosition.x);
		float targetPosition = (show ? num2 : num);
		float timePassed = 0f;
		Vector3 vector = this.Content.anchoredPosition;
		while (timePassed < duration)
		{
			this.RefreshContentWidth();
			float num3 = timePassed / duration;
			if (curveModifier != null)
			{
				num3 = curveModifier(num3);
			}
			vector = this.Content.anchoredPosition;
			vector.x = Mathf.Lerp(contentInitialXPosition, targetPosition, num3);
			this.Content.anchoredPosition = vector;
			timePassed += Time.unscaledDeltaTime;
			yield return null;
		}
		this.RefreshContentWidth();
		vector = this.Content.anchoredPosition;
		vector.x = targetPosition;
		this.Content.anchoredPosition = vector;
		yield return null;
		if (onAnimationEnds != null)
		{
			onAnimationEnds();
		}
		yield break;
	}

	private IEnumerator HeightTransitionAnimation(float duration, bool show, Func<float, float> curveModifier = null, global::System.Action onAnimationEnds = null)
	{
		Transform transform = base.transform;
		float initialHeight = this.layoutElement.minHeight;
		float targetHeight = (show ? this.MaxHeight : 0f);
		float timePassed = 0f;
		float num = this.layoutElement.minHeight;
		while (timePassed < duration)
		{
			this.RefreshContentWidth();
			float num2 = timePassed / duration;
			if (curveModifier != null)
			{
				num2 = curveModifier(num2);
			}
			num = Mathf.Lerp(initialHeight, targetHeight, num2);
			this.layoutElement.minHeight = num;
			timePassed += Time.unscaledDeltaTime;
			yield return null;
		}
		this.RefreshContentWidth();
		num = targetHeight;
		this.layoutElement.minHeight = num;
		yield return null;
		if (onAnimationEnds != null)
		{
			onAnimationEnds();
		}
		yield break;
	}

	public const float ROW_HEIGHT_ANIM_ENTRY_DURATION = 0.5f;

	public const float ROW_HEIGHT_ANIM_EXIT_DURATION = 0.3f;

	public const float SLIDE_ENTER_ANIM_DURATION = 0.4f;

	public const float SLIDE_EXIT_ANIM_DURATION = 0.4f;

	public RectTransform Content;

	public RectTransform IconSection;

	public RectTransform TimeoutBarSection;

	public KImage Image;

	public Image TimeoutImage;

	public LocText Label;

	public ToolTip Tooltip;

	public Action<TemporaryActionRow> OnRowClicked;

	public Action<TemporaryActionRow> OnRowHidden;

	private LayoutElement layoutElement;

	private Coroutine layoutCoroutine;

	private Button button;

	private bool HasBeenShown;

	private float lastSpecifiedLifetime = -1f;
}
