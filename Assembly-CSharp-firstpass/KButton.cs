using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/Plugins/KButton")]
public class KButton : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerClickHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public event global::System.Action onClick;

	public event global::System.Action onDoubleClick;

	public event Action<KKeyCode> onBtnClick;

	public event global::System.Action onPointerEnter;

	public event global::System.Action onPointerExit;

	public event global::System.Action onPointerDown;

	public event global::System.Action onPointerUp;

	public bool isInteractable
	{
		get
		{
			return this.interactable;
		}
		set
		{
			this.interactable = value;
			this.UpdateColor(this.interactable, this.mouseOver, false);
		}
	}

	public bool GetMouseOver
	{
		get
		{
			return this.mouseOver;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.UpdateColor(this.interactable, false, false);
	}

	public void ClearOnClick()
	{
		this.onClick = null;
		this.onBtnClick = null;
		this.onDoubleClick = null;
	}

	public void ClearOnPointerEvents()
	{
		this.onPointerEnter = null;
		this.onPointerExit = null;
		this.onPointerDown = null;
		this.onPointerUp = null;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		this.UpdateColor(this.interactable, false, false);
		this.onPointerUp.Signal();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		this.UpdateColor(this.interactable, true, true);
		this.PlayPointerDownSound();
		this.onPointerDown.Signal();
	}

	public void SignalClick(KKeyCode btn)
	{
		if (this.interactable)
		{
			if (this.onClick != null)
			{
				this.onClick();
			}
			if (this.onBtnClick != null)
			{
				this.onBtnClick(btn);
			}
		}
	}

	public void SignalDoubleClick(KKeyCode btn)
	{
		if (this.interactable && this.onDoubleClick != null)
		{
			this.onDoubleClick();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		if (this.interactable)
		{
			KKeyCode kkeyCode = KKeyCode.None;
			switch (eventData.button)
			{
			case PointerEventData.InputButton.Left:
				kkeyCode = KKeyCode.Mouse0;
				break;
			case PointerEventData.InputButton.Right:
				kkeyCode = KKeyCode.Mouse1;
				break;
			case PointerEventData.InputButton.Middle:
				kkeyCode = KKeyCode.Mouse2;
				break;
			}
			if ((eventData.clickCount == 1 || this.onDoubleClick == null) && (this.onClick != null || this.onBtnClick != null))
			{
				this.SignalClick(kkeyCode);
				return;
			}
			if (eventData.clickCount == 2 && this.onDoubleClick != null)
			{
				this.SignalDoubleClick(kkeyCode);
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
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
		this.UpdateColor(this.interactable, true, false);
		this.soundPlayer.Play(1);
		this.mouseOver = true;
		this.onPointerEnter.Signal();
	}

	public void OnPointerExit(PointerEventData eventData)
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
		this.UpdateColor(this.interactable, false, false);
		this.mouseOver = false;
		this.onPointerExit.Signal();
	}

	private void UpdateColor(bool interactable, bool hover, bool press)
	{
		if (this.bgImage == null)
		{
			this.bgImage = base.GetComponent<KImage>();
			string text = "";
			Transform transform = base.transform;
			int num = 0;
			while (num < 5 && transform.parent != null)
			{
				transform = transform.parent;
				string name = transform.name;
				text = string.Format("{0}/{1}", name, text);
				num++;
			}
			if (this.bgImage == null)
			{
				return;
			}
		}
		this.UpdateKImageColor(this.bgImage, interactable, hover, press);
		for (int i = 0; i < this.additionalKImages.Length; i++)
		{
			this.UpdateKImageColor(this.additionalKImages[i], interactable, hover, press);
		}
	}

	private void UpdateKImageColor(KImage image, bool interactable, bool hover, bool press)
	{
		if (image != null)
		{
			if (interactable)
			{
				if (press)
				{
					image.ColorState = KImage.ColorSelector.Active;
					return;
				}
				image.ColorState = (hover ? KImage.ColorSelector.Hover : KImage.ColorSelector.Inactive);
				return;
			}
			else
			{
				image.ColorState = (hover ? KImage.ColorSelector.Disabled : KImage.ColorSelector.Disabled);
			}
		}
	}

	public void PlayPointerDownSound()
	{
		if (!this.interactable || (this.soundPlayer.AcceptClickCondition != null && !this.soundPlayer.AcceptClickCondition()))
		{
			this.soundPlayer.Play(2);
			return;
		}
		this.soundPlayer.Play(0);
	}

	[SerializeField]
	public ButtonSoundPlayer soundPlayer;

	[HideInInspector]
	[Tooltip("Don't use this field it is misleading, you need to specify the color style setting on the associate bg image")]
	public ColorStyleSetting colorStyleSetting;

	public KImage bgImage;

	public Image fgImage;

	public KImage[] additionalKImages;

	private bool interactable = true;

	private bool mouseOver;
}
