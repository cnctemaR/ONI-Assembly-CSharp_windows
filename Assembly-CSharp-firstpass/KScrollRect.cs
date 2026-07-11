using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KScrollRect : ScrollRect
{
	public bool isDragging { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		base.elasticity = this.default_elasticity;
		base.inertia = this.default_intertia;
		base.decelerationRate = this.default_decelerationRate;
		base.scrollSensitivity = 1f;
		foreach (KeyValuePair<KScrollRect.SoundType, string> keyValuePair in KScrollRect.DefaultSounds)
		{
			this.currentSounds[keyValuePair.Key] = keyValuePair.Value;
		}
	}

	public override void OnScroll(PointerEventData data)
	{
		if (base.vertical && this.allowVerticalScrollWheel)
		{
			this.scrollVelocity += data.scrollDelta.y * this.verticalScrollInertiaScale;
		}
		else if (base.horizontal && this.allowHorizontalScrollWheel)
		{
			this.scrollVelocity -= data.scrollDelta.y * this.horizontalScrollInertiaScale;
		}
		if (Mathf.Abs(data.scrollDelta.y) > 0.2f)
		{
			EventInstance eventInstance = KFMOD.BeginOneShot(this.currentSounds[KScrollRect.SoundType.OnMouseScroll], Vector3.zero, 1f);
			float boundsExceedAmount = this.GetBoundsExceedAmount();
			eventInstance.setParameterValue("scrollbarPosition", boundsExceedAmount);
			KFMOD.EndOneShot(eventInstance);
		}
	}

	private float GetBoundsExceedAmount()
	{
		if (base.vertical && base.verticalScrollbar != null)
		{
			float num = Mathf.Min(((base.viewport == null) ? base.gameObject.GetComponent<RectTransform>() : base.viewport.rectTransform()).rect.size.y, base.content.sizeDelta.y) / base.content.sizeDelta.y;
			float num2 = Mathf.Abs(base.verticalScrollbar.size - num);
			if (Mathf.Abs(num2) < 0.001f)
			{
				num2 = 0f;
			}
			return num2;
		}
		if (base.horizontal && base.horizontalScrollbar != null)
		{
			float num3 = Mathf.Min(((base.viewport == null) ? base.gameObject.GetComponent<RectTransform>() : base.viewport.rectTransform()).rect.size.x, base.content.sizeDelta.x) / base.content.sizeDelta.x;
			float num4 = Mathf.Abs(base.horizontalScrollbar.size - num3);
			if (Mathf.Abs(num4) < 0.001f)
			{
				num4 = 0f;
			}
			return num4;
		}
		return 0f;
	}

	public void SetSmoothAutoScrollTarget(float normalizedVerticalPos)
	{
		this.autoScrollTargetVerticalPos = normalizedVerticalPos;
		this.autoScrolling = true;
	}

	private void PlaySound(KScrollRect.SoundType soundType)
	{
		if (this.currentSounds.ContainsKey(soundType))
		{
			KFMOD.PlayUISound(this.currentSounds[soundType]);
		}
	}

	public void SetSound(KScrollRect.SoundType soundType, string soundPath)
	{
		this.currentSounds[soundType] = soundPath;
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		this.startDrag = true;
		base.OnBeginDrag(eventData);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		this.stopDrag = true;
		base.OnEndDrag(eventData);
	}

	public override void OnDrag(PointerEventData eventData)
	{
		if (this.allowRightMouseScroll && (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Middle))
		{
			base.content.localPosition = base.content.localPosition + new Vector3(eventData.delta.x, eventData.delta.y);
			base.normalizedPosition = new Vector2(Mathf.Clamp(base.normalizedPosition.x, 0f, 1f), Mathf.Clamp(base.normalizedPosition.y, 0f, 1f));
		}
		base.OnDrag(eventData);
		this.scrollVelocity = 0f;
	}

	protected override void LateUpdate()
	{
		this.UpdateScrollIntertia();
		if (this.allowRightMouseScroll)
		{
			if (this.panUp)
			{
				this.keyboardScrollDelta.y = this.keyboardScrollDelta.y - this.keyboardScrollSpeed;
			}
			if (this.panDown)
			{
				this.keyboardScrollDelta.y = this.keyboardScrollDelta.y + this.keyboardScrollSpeed;
			}
			if (this.panLeft)
			{
				this.keyboardScrollDelta.x = this.keyboardScrollDelta.x + this.keyboardScrollSpeed;
			}
			if (this.panRight)
			{
				this.keyboardScrollDelta.x = this.keyboardScrollDelta.x - this.keyboardScrollSpeed;
			}
			if (this.panUp || this.panDown || this.panLeft || this.panRight)
			{
				base.content.localPosition = base.content.localPosition + this.keyboardScrollDelta;
				base.normalizedPosition = new Vector2(Mathf.Clamp(base.normalizedPosition.x, 0f, 1f), Mathf.Clamp(base.normalizedPosition.y, 0f, 1f));
			}
		}
		if (this.startDrag)
		{
			this.startDrag = false;
			this.isDragging = true;
		}
		else if (this.stopDrag)
		{
			this.stopDrag = false;
			this.isDragging = false;
		}
		if (this.autoScrolling)
		{
			base.normalizedPosition = new Vector2(base.normalizedPosition.x, Mathf.Lerp(base.normalizedPosition.y, this.autoScrollTargetVerticalPos, Time.unscaledDeltaTime * 3f));
			if (Mathf.Abs(this.autoScrollTargetVerticalPos - base.normalizedPosition.y) < 0.01f)
			{
				this.autoScrolling = false;
			}
		}
		base.LateUpdate();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		if (this.forceContentMatchWidth)
		{
			Vector2 sizeDelta = base.content.GetComponent<RectTransform>().sizeDelta;
			sizeDelta.x = base.viewport.rectTransform().sizeDelta.x;
			base.content.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		}
		if (this.forceContentMatchHeight)
		{
			Vector2 sizeDelta2 = base.content.GetComponent<RectTransform>().sizeDelta;
			sizeDelta2.y = base.viewport.rectTransform().sizeDelta.y;
			base.content.GetComponent<RectTransform>().sizeDelta = sizeDelta2;
		}
	}

	private void UpdateScrollIntertia()
	{
		this.scrollVelocity *= 1f - Mathf.Clamp(this.scrollDeceleration, 0f, 1f);
		if (Mathf.Abs(this.scrollVelocity) < 0.001f)
		{
			this.scrollVelocity = 0f;
		}
		else
		{
			Vector2 anchoredPosition = base.content.anchoredPosition;
			if (base.vertical && this.allowVerticalScrollWheel)
			{
				anchoredPosition.y -= this.scrollVelocity;
			}
			if (base.horizontal && this.allowHorizontalScrollWheel)
			{
				anchoredPosition.x -= this.scrollVelocity;
			}
			if (base.content.anchoredPosition != anchoredPosition)
			{
				base.content.anchoredPosition = anchoredPosition;
			}
		}
		if (base.vertical && this.allowVerticalScrollWheel && (base.verticalNormalizedPosition < -0.05f || base.verticalNormalizedPosition > 1.05f))
		{
			this.scrollVelocity *= 0.9f;
		}
		if (base.horizontal && this.allowHorizontalScrollWheel && (base.horizontalNormalizedPosition < -0.05f || base.horizontalNormalizedPosition > 1.05f))
		{
			this.scrollVelocity *= 0.9f;
		}
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (!this.allowRightMouseScroll)
		{
			return;
		}
		if (e.TryConsume(global::Action.PanLeft))
		{
			this.panLeft = true;
			return;
		}
		if (e.TryConsume(global::Action.PanRight))
		{
			this.panRight = true;
			return;
		}
		if (e.TryConsume(global::Action.PanUp))
		{
			this.panUp = true;
			return;
		}
		if (e.TryConsume(global::Action.PanDown))
		{
			this.panDown = true;
		}
	}

	public void OnKeyUp(KButtonEvent e)
	{
		if (!this.allowRightMouseScroll)
		{
			return;
		}
		if (this.panUp && e.TryConsume(global::Action.PanUp))
		{
			this.panUp = false;
			this.keyboardScrollDelta.y = 0f;
			return;
		}
		if (this.panDown && e.TryConsume(global::Action.PanDown))
		{
			this.panDown = false;
			this.keyboardScrollDelta.y = 0f;
			return;
		}
		if (this.panRight && e.TryConsume(global::Action.PanRight))
		{
			this.panRight = false;
			this.keyboardScrollDelta.x = 0f;
			return;
		}
		if (this.panLeft && e.TryConsume(global::Action.PanLeft))
		{
			this.panLeft = false;
			this.keyboardScrollDelta.x = 0f;
		}
	}

	public static Dictionary<KScrollRect.SoundType, string> DefaultSounds = new Dictionary<KScrollRect.SoundType, string>();

	private Dictionary<KScrollRect.SoundType, string> currentSounds = new Dictionary<KScrollRect.SoundType, string>();

	private float scrollVelocity;

	private bool default_intertia = true;

	private float default_elasticity = 0.2f;

	private float default_decelerationRate = 0.02f;

	private float verticalScrollInertiaScale = 10f;

	private float horizontalScrollInertiaScale = 5f;

	private float scrollDeceleration = 0.25f;

	[SerializeField]
	public bool forceContentMatchWidth;

	[SerializeField]
	public bool forceContentMatchHeight;

	[SerializeField]
	public bool allowHorizontalScrollWheel = true;

	[SerializeField]
	public bool allowVerticalScrollWheel = true;

	[SerializeField]
	public bool allowRightMouseScroll;

	private bool panUp;

	private bool panDown;

	private bool panRight;

	private bool panLeft;

	private Vector3 keyboardScrollDelta;

	private float keyboardScrollSpeed = 1f;

	private bool startDrag;

	private bool stopDrag;

	private bool autoScrolling;

	private float autoScrollTargetVerticalPos;

	public enum SoundType
	{
		OnMouseScroll
	}
}
