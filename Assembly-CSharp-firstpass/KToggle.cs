using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KToggle : Toggle
{
	public event global::System.Action onClick;

	public event global::System.Action onDoubleClick;

	public new event Action<bool> onValueChanged;

	public event KToggle.PointerEvent onPointerEnter;

	public event KToggle.PointerEvent onPointerExit;

	public bool GetMouseOver
	{
		get
		{
			return this.mouseOver;
		}
	}

	public void ClearOnValueChanged()
	{
		this.onValueChanged = null;
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
	}

	public void Click()
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		if (!this.IsInteractable())
		{
			return;
		}
		if (global::UnityEngine.EventSystems.EventSystem.current == null || !global::UnityEngine.EventSystems.EventSystem.current.enabled)
		{
			return;
		}
		if (this.isOn)
		{
			this.Deselect();
			this.isOn = false;
		}
		else
		{
			this.Select();
			this.isOn = true;
		}
		if (this.soundPlayer.AcceptClickCondition != null && !this.soundPlayer.AcceptClickCondition())
		{
			this.soundPlayer.Play(3);
		}
		else
		{
			this.soundPlayer.Play(this.isOn ? 0 : 1);
		}
		base.gameObject.Trigger(2098165161, null);
		this.onClick.Signal();
	}

	private void OnValueChanged(bool value)
	{
		if (!this.IsInteractable())
		{
			return;
		}
		ImageToggleState[] components = base.GetComponents<ImageToggleState>();
		if (components != null && components.Length != 0)
		{
			ImageToggleState[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActiveState(value);
			}
		}
		this.ActivateFlourish(value);
		this.onValueChanged.Signal(value);
	}

	public void ForceUpdateVisualState()
	{
		ImageToggleState[] components = base.GetComponents<ImageToggleState>();
		if (components != null && components.Length != 0)
		{
			ImageToggleState[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResetColor();
			}
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			return;
		}
		if (!this.IsInteractable())
		{
			return;
		}
		if (eventData.clickCount == 1 || this.onDoubleClick == null)
		{
			this.Click();
			return;
		}
		if (eventData.clickCount == 2 && this.onDoubleClick != null)
		{
			this.onDoubleClick();
		}
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		if (this.GetParentToggleGroup(eventData) == base.group)
		{
			base.OnDeselect(eventData);
		}
	}

	public void Deselect()
	{
		base.OnDeselect(null);
	}

	public void ClearAnimState()
	{
		if (this.artExtension.animator != null && this.artExtension.animator.isInitialized)
		{
			Animator animator = this.artExtension.animator;
			animator.SetBool("Toggled", false);
			animator.Play("idle", 0);
		}
	}

	public override void OnSelect(BaseEventData eventData)
	{
		if (base.group != null)
		{
			foreach (Toggle toggle in base.group.ActiveToggles())
			{
				((KToggle)toggle).Deselect();
			}
			base.group.SetAllTogglesOff(true);
		}
		base.OnSelect(eventData);
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
		if (components != null && components.Length != 0)
		{
			ImageToggleState[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetState(ImageState);
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

	public void OnPointerEnter()
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		ImageToggleState[] components = base.GetComponents<ImageToggleState>();
		if (components != null && components.Length != 0)
		{
			ImageToggleState[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnHoverIn();
			}
		}
		this.soundPlayer.Play(2);
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
		if (components != null && components.Length != 0)
		{
			ImageToggleState[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnHoverOut();
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

	public new bool isOn
	{
		get
		{
			return base.isOn;
		}
		set
		{
			base.isOn = value;
			this.OnValueChanged(base.isOn);
		}
	}

	[SerializeField]
	public ToggleSoundPlayer soundPlayer;

	public Image bgImage;

	public Image fgImage;

	public KToggleArtExtensions artExtension;

	protected bool mouseOver;

	public delegate void PointerEvent();
}
