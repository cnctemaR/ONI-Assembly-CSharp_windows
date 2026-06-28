using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KToggle : Toggle
{
	public event global::System.Action onClick;

	public event global::System.Action onDoubleClick;

	public event Action<GameObject> onRefresh;

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

	private new void OnEnable()
	{
		base.OnEnable();
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
		this.isOn = !this.isOn;
		if (this.soundPlayer.AcceptClickCondition != null && !this.soundPlayer.AcceptClickCondition())
		{
			this.soundPlayer.Play(3);
		}
		else
		{
			this.soundPlayer.Play((!this.isOn) ? 1 : 0);
		}
		this.onClick.Signal();
		global::EventSystem.Trigger(base.gameObject, 2098165161, null);
	}

	private void OnValueChanged(bool value)
	{
		ImageToggleState[] components = base.GetComponents<ImageToggleState>();
		if (components != null && components.Length > 0)
		{
			foreach (ImageToggleState imageToggleState in components)
			{
				imageToggleState.SetActiveState(value);
			}
		}
		this.ActivateFlourish(value);
		this.onValueChanged.Signal(value);
	}

	public void ForceUpdateVisualState()
	{
		ImageToggleState[] components = base.GetComponents<ImageToggleState>();
		if (components != null && components.Length > 0)
		{
			foreach (ImageToggleState imageToggleState in components)
			{
				imageToggleState.ResetColor();
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
		if (eventData.clickCount == 1 || this.onDoubleClick == null)
		{
			this.Click();
		}
		else if (eventData.clickCount == 2 && this.onDoubleClick != null)
		{
			this.onDoubleClick();
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
