using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KButton : KMonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public event global::System.Action onClick;

	public event global::System.Action onDoubleClick;

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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.UpdateColor(this.interactable, false, false);
	}

	public void ClearOnClick()
	{
		this.onClick = null;
		this.onDoubleClick = null;
	}

	public void ClearOnPointerEnter()
	{
		this.onPointerEnter = null;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		this.UpdateColor(this.interactable, false, false);
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
			if ((eventData.clickCount == 1 || this.onDoubleClick == null) && this.onClick != null)
			{
				this.onClick();
			}
			else if (eventData.clickCount == 2 && this.onDoubleClick != null)
			{
				this.onDoubleClick();
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
		if (components != null && components.Length > 0)
		{
			foreach (ImageToggleState imageToggleState in components)
			{
				imageToggleState.OnHoverIn();
			}
		}
		this.UpdateColor(this.interactable, true, false);
		this.soundPlayer.Play(1);
		this.mouseOver = true;
		if (this.onPointerEnter != null)
		{
			this.onPointerEnter();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
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
		this.UpdateColor(this.interactable, false, false);
		this.mouseOver = false;
		this.onPointerExit.Signal();
	}

	private void UpdateColor(bool interactable, bool hover, bool press)
	{
		if (this.bgImage == null)
		{
			this.bgImage = base.GetComponent<KImage>();
			string text = string.Empty;
			Transform transform = this.transform;
			for (int i = 0; i < 5; i++)
			{
				if (!(transform.parent != null))
				{
					break;
				}
				transform = transform.parent;
				string name = transform.name;
				text = string.Format("{0}/{1}", name, text);
			}
			if (this.bgImage == null)
			{
				return;
			}
		}
		if (this.bgImage != null)
		{
			if (interactable)
			{
				if (press)
				{
					this.bgImage.ColorState = KImage.ColorSelector.Active;
				}
				else
				{
					this.bgImage.ColorState = ((!hover) ? KImage.ColorSelector.Inactive : KImage.ColorSelector.Hover);
				}
			}
			else
			{
				this.bgImage.ColorState = ((!hover) ? KImage.ColorSelector.Disabled : KImage.ColorSelector.Disabled);
			}
		}
	}

	private void PlayPointerDownSound()
	{
		if (!this.interactable || (this.soundPlayer.AcceptClickCondition != null && !this.soundPlayer.AcceptClickCondition()))
		{
			this.soundPlayer.Play(2);
		}
		else
		{
			this.soundPlayer.Play(0);
		}
	}

	[SerializeField]
	public ButtonSoundPlayer soundPlayer;

	public ColorStyleSetting colorStyleSetting;

	public KImage bgImage;

	public Image fgImage;

	public global::System.Action onPointerEnter;

	public global::System.Action onPointerExit;

	private bool interactable = true;

	private bool mouseOver;
}
