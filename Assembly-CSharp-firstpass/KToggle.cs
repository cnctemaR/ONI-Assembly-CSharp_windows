using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KToggle : Toggle
{
	public event global::System.Action onClick;

	public event global::System.Action onDoubleClick;

	public event Func<bool> onValidate;

	public event Action<GameObject> onRefresh;

	public event KToggle.PointerEvent onPointerEnter;

	public event KToggle.PointerEvent onPointerExit;

	public bool GetMouseOver
	{
		get
		{
			return this.mouseOver;
		}
	}

	private new void Awake()
	{
		foreach (KeyValuePair<KToggle.SoundType, string> keyValuePair in KToggle.DefaultSounds)
		{
			this.currentSounds[keyValuePair.Key] = keyValuePair.Value;
		}
	}

	private new void OnEnable()
	{
		base.OnEnable();
		if (this.artExtension.animator != null && this.artExtension.animator.isInitialized)
		{
			this.artExtension.animator.Play("Released", 0, 1f);
		}
	}

	public void ClearOnClick()
	{
		this.onClick = null;
	}

	public void ClearPointerCallbacks()
	{
		this.onPointerEnter = null;
		this.onPointerExit = null;
	}

	public void ClearAllCallbacks()
	{
		this.ClearOnClick();
		this.ClearPointerCallbacks();
		this.onDoubleClick = null;
		this.onValidate = null;
		this.onRefresh = null;
	}

	public void Click()
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		if (global::UnityEngine.EventSystems.EventSystem.current == null || !global::UnityEngine.EventSystems.EventSystem.current.enabled)
		{
			return;
		}
		this.Select();
		global::EventSystem.Trigger(base.gameObject, 2098165161, null);
		this.onClick.Signal();
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		if (this.Validate())
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				return;
			}
			if (this.onClick != null && (eventData.clickCount == 1 || this.onDoubleClick == null))
			{
				this.Click();
			}
			else if (eventData.clickCount == 2 && this.onDoubleClick != null)
			{
				this.onDoubleClick();
			}
		}
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		ToggleGroup parentToggleGroup = this.GetParentToggleGroup(eventData);
		if (parentToggleGroup == base.group)
		{
			base.OnDeselect(eventData);
		}
	}

	public void Deselect()
	{
		base.OnDeselect(null);
	}

	private bool Validate()
	{
		return this.onValidate == null || this.onValidate();
	}

	public override void OnSelect(BaseEventData eventData)
	{
		if (base.group != null)
		{
			foreach (Toggle toggle in base.group.ActiveToggles())
			{
				KToggle ktoggle = (KToggle)toggle;
				ktoggle.Deselect();
			}
			base.group.SetAllTogglesOff();
		}
		base.OnSelect(eventData);
	}

	private void OnChanged(bool state)
	{
	}

	public void ActivateFlourish(bool state)
	{
		if (this.artExtension.animator != null && this.artExtension.animator.isInitialized)
		{
			this.artExtension.animator.SetBool("Toggled", state);
		}
		if (this.artExtension.SelectedFlourish != null)
		{
			this.artExtension.SelectedFlourish.enabled = state;
		}
	}

	public void ActivateFlourish(bool state, ImageToggleState.State ImageState)
	{
		ImageToggleState[] components = base.GetComponents<ImageToggleState>();
		if (components != null && components.Length > 0)
		{
			foreach (ImageToggleState imageToggleState in components)
			{
				imageToggleState.SetState(ImageState);
			}
		}
		this.ActivateFlourish(state);
	}

	private ToggleGroup GetParentToggleGroup(BaseEventData eventData)
	{
		PointerEventData pointerEventData = eventData as PointerEventData;
		if (pointerEventData == null)
		{
			return null;
		}
		GameObject gameObject = pointerEventData.pointerPressRaycast.gameObject;
		if (gameObject == null)
		{
			return null;
		}
		Toggle componentInParent = gameObject.GetComponentInParent<Toggle>();
		if (componentInParent == null || componentInParent.group == null)
		{
			return null;
		}
		return componentInParent.group;
	}

	private void Update()
	{
		bool flag = this.Validate();
		if (flag != base.interactable)
		{
			base.interactable = flag;
		}
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
		this.PlaySound(KToggle.SoundType.OnMouseOver);
		this.mouseOver = true;
		if (this.onPointerEnter != null)
		{
			this.onPointerEnter();
		}
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
		if (this.onPointerExit != null)
		{
			this.onPointerExit();
		}
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

	private void PlaySound(KToggle.SoundType soundType)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		if (this.currentSounds.ContainsKey(soundType))
		{
			KFMOD.PlayOneShot(this.currentSounds[soundType]);
		}
	}

	public void SetSound(KToggle.SoundType soundType, string soundPath)
	{
		this.currentSounds[soundType] = soundPath;
	}

	public Image bgImage;

	public Image fgImage;

	public KToggleArtExtensions artExtension;

	protected bool mouseOver;

	public static Dictionary<KToggle.SoundType, string> DefaultSounds = new Dictionary<KToggle.SoundType, string>();

	private Dictionary<KToggle.SoundType, string> currentSounds = new Dictionary<KToggle.SoundType, string>();

	public enum SoundType
	{
		OnMouseOver
	}

	public delegate void PointerEvent();
}
