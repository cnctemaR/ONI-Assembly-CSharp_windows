using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/Plugins/ImageToggleState")]
public class ImageToggleState : KMonoBehaviour
{
	public bool IsDisabled
	{
		get
		{
			return this.currentState == ImageToggleState.State.Disabled || this.currentState == ImageToggleState.State.DisabledActive;
		}
	}

	public new void Awake()
	{
		base.Awake();
		this.RefreshColorStyle();
		if (this.useStartingState)
		{
			this.SetState(this.startingState);
		}
	}

	[ContextMenu("Refresh Colour Style")]
	public void RefreshColorStyle()
	{
		if (this.colorStyleSetting != null)
		{
			this.ActiveColour = this.colorStyleSetting.activeColor;
			this.InactiveColour = this.colorStyleSetting.inactiveColor;
			this.DisabledColour = this.colorStyleSetting.disabledColor;
			this.DisabledActiveColour = this.colorStyleSetting.disabledActiveColor;
			this.HoverColour = this.colorStyleSetting.hoverColor;
			this.DisabledHoverColor = this.colorStyleSetting.disabledhoverColor;
		}
	}

	public void SetSprites(Sprite disabled, Sprite inactive, Sprite active, Sprite disabledActive)
	{
		if (disabled != null)
		{
			this.DisabledSprite = disabled;
		}
		if (inactive != null)
		{
			this.InactiveSprite = inactive;
		}
		if (active != null)
		{
			this.ActiveSprite = active;
		}
		if (disabledActive != null)
		{
			this.DisabledActiveSprite = disabledActive;
		}
		this.useSprites = true;
	}

	public bool GetIsActive()
	{
		return this.isActive;
	}

	private void SetTargetImageColor(Color color)
	{
		this.TargetImage.color = color;
	}

	public void SetState(ImageToggleState.State newState)
	{
		if (this.currentState == newState)
		{
			return;
		}
		switch (newState)
		{
		case ImageToggleState.State.Disabled:
			this.SetDisabled();
			return;
		case ImageToggleState.State.Inactive:
			this.SetInactive();
			return;
		case ImageToggleState.State.Active:
			this.SetActive();
			return;
		case ImageToggleState.State.DisabledActive:
			this.SetDisabledActive();
			return;
		default:
			return;
		}
	}

	public void SetActiveState(bool active)
	{
		if (active)
		{
			this.SetActive();
			return;
		}
		this.SetInactive();
	}

	public virtual void SetActive()
	{
		if (this.currentState == ImageToggleState.State.Active)
		{
			return;
		}
		this.isActive = true;
		this.currentState = ImageToggleState.State.Active;
		if (this.TargetImage == null)
		{
			return;
		}
		this.SetTargetImageColor(this.ActiveColour);
		if (this.useSprites)
		{
			if (this.ActiveSprite != null && this.TargetImage.sprite != this.ActiveSprite)
			{
				this.TargetImage.sprite = this.ActiveSprite;
				return;
			}
			if (this.ActiveSprite == null)
			{
				this.TargetImage.sprite = null;
			}
		}
	}

	public void SetColorStyle(ColorStyleSetting style)
	{
		this.colorStyleSetting = style;
		this.RefreshColorStyle();
		this.ResetColor();
	}

	public void ResetColor()
	{
		switch (this.currentState)
		{
		case ImageToggleState.State.Disabled:
			this.SetTargetImageColor(this.DisabledColour);
			return;
		case ImageToggleState.State.Inactive:
			this.SetTargetImageColor(this.InactiveColour);
			return;
		case ImageToggleState.State.Active:
			this.SetTargetImageColor(this.ActiveColour);
			return;
		case ImageToggleState.State.DisabledActive:
			this.SetTargetImageColor(this.DisabledActiveColour);
			return;
		default:
			return;
		}
	}

	public void OnHoverIn()
	{
		this.SetTargetImageColor((this.currentState == ImageToggleState.State.Disabled || this.currentState == ImageToggleState.State.DisabledActive) ? this.DisabledHoverColor : this.HoverColour);
	}

	public void OnHoverOut()
	{
		this.ResetColor();
	}

	public virtual void SetInactive()
	{
		if (this.currentState == ImageToggleState.State.Inactive)
		{
			return;
		}
		this.isActive = false;
		this.currentState = ImageToggleState.State.Inactive;
		this.SetTargetImageColor(this.InactiveColour);
		if (this.TargetImage == null)
		{
			return;
		}
		if (this.useSprites)
		{
			if (this.InactiveSprite != null && this.TargetImage.sprite != this.InactiveSprite)
			{
				this.TargetImage.sprite = this.InactiveSprite;
				return;
			}
			if (this.InactiveSprite == null)
			{
				this.TargetImage.sprite = null;
			}
		}
	}

	public virtual void SetDisabled()
	{
		if (this.currentState == ImageToggleState.State.Disabled)
		{
			this.SetTargetImageColor(this.DisabledColour);
			return;
		}
		this.isActive = false;
		this.currentState = ImageToggleState.State.Disabled;
		this.SetTargetImageColor(this.DisabledColour);
		if (this.TargetImage == null)
		{
			return;
		}
		if (this.useSprites)
		{
			if (this.DisabledSprite != null && this.TargetImage.sprite != this.DisabledSprite)
			{
				this.TargetImage.sprite = this.DisabledSprite;
				return;
			}
			if (this.DisabledSprite == null)
			{
				this.TargetImage.sprite = null;
			}
		}
	}

	public virtual void SetDisabledActive()
	{
		this.isActive = false;
		this.currentState = ImageToggleState.State.DisabledActive;
		if (this.TargetImage == null)
		{
			return;
		}
		this.SetTargetImageColor(this.DisabledActiveColour);
		if (this.useSprites)
		{
			if (this.DisabledActiveSprite != null && this.TargetImage.sprite != this.DisabledActiveSprite)
			{
				this.TargetImage.sprite = this.DisabledActiveSprite;
				return;
			}
			if (this.DisabledActiveSprite == null)
			{
				this.TargetImage.sprite = null;
			}
		}
	}

	public Image TargetImage;

	public Sprite ActiveSprite;

	public Sprite InactiveSprite;

	public Sprite DisabledSprite;

	public Sprite DisabledActiveSprite;

	public bool useSprites;

	public Color ActiveColour = Color.white;

	public Color InactiveColour = Color.white;

	public Color DisabledColour = Color.white;

	public Color DisabledActiveColour = Color.white;

	public Color HoverColour = Color.white;

	public Color DisabledHoverColor = Color.white;

	public ColorStyleSetting colorStyleSetting;

	private bool isActive;

	private ImageToggleState.State currentState = ImageToggleState.State.Inactive;

	public bool useStartingState;

	public ImageToggleState.State startingState = ImageToggleState.State.Inactive;

	public enum State
	{
		Disabled,
		Inactive,
		Active,
		DisabledActive
	}
}
