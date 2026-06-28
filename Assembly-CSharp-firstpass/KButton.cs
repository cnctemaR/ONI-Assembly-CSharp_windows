using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KButton : Button
{
	public new event global::System.Action onClick;

	public event global::System.Action onClickUp;

	public event global::System.Action onClickDown;

	public event global::System.Action onDoubleClick;

	public bool GetMouseOver
	{
		get
		{
			return this.mouseOver;
		}
	}

	[ContextMenu("Initialize Colors")]
	private void InitializeColors()
	{
		if (this.colorStyleSetting != null)
		{
			if (base.targetGraphic == null)
			{
				Debug.LogError("The KButton needs a target graphic in order to use a colorStyleSetting, please add one in the inspector.");
				return;
			}
			base.targetGraphic.color = Color.white;
			ColorBlock colors = base.colors;
			colors.normalColor = this.colorStyleSetting.inactiveColor;
			colors.highlightedColor = this.colorStyleSetting.hoverColor;
			colors.disabledColor = this.colorStyleSetting.disabledColor;
			colors.pressedColor = this.colorStyleSetting.activeColor;
			base.colors = colors;
		}
	}

	private new void Awake()
	{
		this.InitializeColors();
		foreach (KeyValuePair<KButton.SoundType, string> keyValuePair in KButton.DefaultSounds)
		{
			this.currentSounds[keyValuePair.Key] = keyValuePair.Value;
		}
	}

	public void ClearOnClick()
	{
		this.onClick = null;
	}

	public void ClearOnClickDown()
	{
		this.onClickDown = null;
	}

	public void ClearOnPointerEnter()
	{
		this.onPointerEnter = null;
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		base.OnPointerUp(eventData);
		if (base.interactable && this.onClickUp != null)
		{
			this.onClickUp();
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		base.OnPointerUp(eventData);
		if (base.interactable && this.onClickDown != null)
		{
			this.onClickDown();
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		base.OnPointerClick(eventData);
		if (base.interactable)
		{
			if ((eventData.clickCount == 1 || this.onDoubleClick == null) && this.onClick != null)
			{
				this.onClick();
			}
			else if (eventData.clickCount == 2 && this.onDoubleClick != null)
			{
				this.onDoubleClick();
			}
			this.PlaySound(KButton.SoundType.OnMouseClick);
		}
		else
		{
			this.PlaySound(KButton.SoundType.OnMouseClickNegative);
		}
	}

	public void Deselect()
	{
		this.OnDeselect(null);
	}

	public void OnPointerEnter()
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		ImageToggleState[] components = base.GetComponents<ImageToggleState>();
		if (components != null && components.Length > 0)
		{
			foreach (ImageToggleState imageToggleState in components)
			{
				imageToggleState.OnHoverIn();
			}
		}
		this.PlaySound(KButton.SoundType.OnMouseOver);
		this.mouseOver = true;
		this.onPointerEnter.Signal();
	}

	public void OnPointerExit()
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		ImageToggleState[] components = base.GetComponents<ImageToggleState>();
		if (components != null && components.Length > 0)
		{
			foreach (ImageToggleState imageToggleState in components)
			{
				imageToggleState.OnHoverOut();
			}
		}
		this.mouseOver = false;
		this.onPointerExit.Signal();
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		this.OnPointerEnter();
		base.OnPointerEnter(eventData);
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		this.OnPointerExit();
		base.OnPointerExit(eventData);
	}

	private void PlaySound(KButton.SoundType soundType)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		if (!this.playSounds)
		{
			return;
		}
		if (this.currentSounds.ContainsKey(soundType))
		{
			KFMOD.PlayOneShot(this.currentSounds[soundType]);
		}
	}

	public void SetSound(KButton.SoundType soundType, string soundPath)
	{
		this.currentSounds[soundType] = soundPath;
	}

	public static Dictionary<KButton.SoundType, string> DefaultSounds = new Dictionary<KButton.SoundType, string>();

	private Dictionary<KButton.SoundType, string> currentSounds = new Dictionary<KButton.SoundType, string>();

	public global::System.Action onPointerEnter;

	public global::System.Action onPointerExit;

	public Image bgImage;

	public Image fgImage;

	public ColorStyleSetting colorStyleSetting;

	private bool mouseOver;

	public bool playSounds = true;

	public enum SoundType
	{
		OnMouseOver,
		OnMouseClick,
		OnMouseClickNegative
	}
}
