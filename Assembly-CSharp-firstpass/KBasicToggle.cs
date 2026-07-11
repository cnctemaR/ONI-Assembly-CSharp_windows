using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class KBasicToggle : KMonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public event global::System.Action onClick;

	public event global::System.Action onDoubleClick;

	public event global::System.Action onPointerEnter;

	public event global::System.Action onPointerExit;

	public event Action<bool> onValueChanged;

	public bool isOn
	{
		get
		{
			return this._isOn;
		}
		set
		{
			this._isOn = value;
			if (this.onValueChanged != null)
			{
				this.onValueChanged(value);
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.doubleClickCoroutine != null && this.onDoubleClick != null)
		{
			this.onDoubleClick();
			this.didDoubleClick = true;
			return;
		}
		this.doubleClickCoroutine = this.DoubleClickTimer(eventData);
		base.StartCoroutine(this.doubleClickCoroutine);
	}

	private IEnumerator DoubleClickTimer(PointerEventData eventData)
	{
		float startTime = Time.unscaledTime;
		while (Time.unscaledTime - startTime < 0.15f && !this.didDoubleClick)
		{
			yield return null;
		}
		if (!this.didDoubleClick && this.onClick != null)
		{
			this.isOn = !this.isOn;
			this.onClick();
			this.onValueChanged(this.isOn);
		}
		this.doubleClickCoroutine = null;
		this.didDoubleClick = false;
		yield break;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.onPointerEnter != null)
		{
			this.onPointerEnter();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (this.onPointerExit != null)
		{
			this.onPointerExit();
		}
	}

	private const float DoubleClickTime = 0.15f;

	private bool _isOn;

	private bool didDoubleClick;

	private IEnumerator doubleClickCoroutine;
}
