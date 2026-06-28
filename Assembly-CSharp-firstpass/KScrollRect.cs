using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KScrollRect : ScrollRect
{
	protected override void Awake()
	{
		base.Awake();
		base.elasticity = this.default_elasticity;
		base.inertia = this.default_intertia;
		base.decelerationRate = this.default_decelerationRate;
		base.scrollSensitivity = 1f;
		base.movementType = ScrollRect.MovementType.Elastic;
		foreach (KeyValuePair<KScrollRect.SoundType, string> keyValuePair in KScrollRect.DefaultSounds)
		{
			this.currentSounds[keyValuePair.Key] = keyValuePair.Value;
		}
	}

	public override void OnScroll(PointerEventData data)
	{
		if (base.vertical)
		{
			this.scrollVelocity += data.scrollDelta.y * this.verticalScrollInertiaScale;
		}
		else if (base.horizontal)
		{
			this.scrollVelocity -= data.scrollDelta.y * this.horizontalScrollInertiaScale;
		}
		if (Mathf.Abs(data.scrollDelta.y) > 0.2f)
		{
			EventInstance eventInstance = KFMOD.BeginOneShot(this.currentSounds[KScrollRect.SoundType.OnMouseScroll], Vector3.zero);
			float boundsExceedAmount = this.GetBoundsExceedAmount();
			eventInstance.setParameterValue("scrollbarPosition", boundsExceedAmount);
			KFMOD.EndOneShot(eventInstance);
		}
	}

	private float GetBoundsExceedAmount()
	{
		if (base.vertical && base.verticalScrollbar != null)
		{
			RectTransform rectTransform = ((!(base.viewport == null)) ? base.viewport.rectTransform() : base.gameObject.GetComponent<RectTransform>());
			float num = Mathf.Min(rectTransform.rect.size.y, base.content.sizeDelta.y) / base.content.sizeDelta.y;
			float num2 = Mathf.Abs(base.verticalScrollbar.size - num);
			if (Mathf.Abs(num2) < 0.001f)
			{
				num2 = 0f;
			}
			return num2;
		}
		if (base.horizontal && base.horizontalScrollbar != null)
		{
			RectTransform rectTransform2 = ((!(base.viewport == null)) ? base.viewport.rectTransform() : base.gameObject.GetComponent<RectTransform>());
			float num3 = Mathf.Min(rectTransform2.rect.size.x, base.content.sizeDelta.x) / base.content.sizeDelta.x;
			float num4 = Mathf.Abs(base.horizontalScrollbar.size - num3);
			if (Mathf.Abs(num4) < 0.001f)
			{
				num4 = 0f;
			}
			return num4;
		}
		return 0f;
	}

	private void PlaySound(KScrollRect.SoundType soundType)
	{
		if (this.currentSounds.ContainsKey(soundType))
		{
			KFMOD.PlayOneShot(this.currentSounds[soundType]);
		}
	}

	public void SetSound(KScrollRect.SoundType soundType, string soundPath)
	{
		this.currentSounds[soundType] = soundPath;
	}

	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		this.scrollVelocity = 0f;
	}

	protected override void LateUpdate()
	{
		this.UpdateScrollIntertia();
		base.LateUpdate();
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
			if (base.vertical)
			{
				anchoredPosition.y -= this.scrollVelocity;
			}
			if (base.horizontal)
			{
				anchoredPosition.x -= this.scrollVelocity;
			}
			base.content.anchoredPosition = anchoredPosition;
		}
		if (base.vertical && (base.verticalNormalizedPosition < -0.05f || base.verticalNormalizedPosition > 1.05f))
		{
			this.scrollVelocity *= 0.9f;
		}
		if (base.horizontal && (base.horizontalNormalizedPosition < -0.05f || base.horizontalNormalizedPosition > 1.05f))
		{
			this.scrollVelocity *= 0.9f;
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

	public enum SoundType
	{
		OnMouseScroll
	}
}
