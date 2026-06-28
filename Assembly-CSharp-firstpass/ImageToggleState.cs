using System;
using UnityEngine;
using UnityEngine.UI;

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
		if (this.currentState != newState)
		{
			switch (newState)
			{
			case ImageToggleState.State.Disabled:
				this.SetDisabled();
				break;
			case ImageToggleState.State.Inactive:
				this.SetInactive();
				break;
			case ImageToggleState.State.Active:
				this.SetActive();
				break;
			case ImageToggleState.State.DisabledActive:
				this.SetDisabledActive();
				break;
			}
		}
	}

	public void SetActiveState(bool state)
	{
		if (state)
		{
			this.SetActive();
		}
		else
		{
			this.SetInactive();
		}
	}

	public void SetActive()
	{
		if (this.currentState != ImageToggleState.State.Active)
		{
			this.isActive = true;
			this.currentState = ImageToggleState.State.Active;
			if (!(this.TargetImage == null))
			{
				this.SetTargetImageColor(this.ActiveColour);
				if (this.useSprites)
				{
					if (this.ActiveSprite != null && this.TargetImage.sprite != this.ActiveSprite)
					{
						this.TargetImage.sprite = this.ActiveSprite;
					}
					else if (this.ActiveSprite == null)
					{
						this.TargetImage.sprite = null;
					}
				}
			}
		}
	}

	public void ResetColor()
	{
		switch (this.currentState)
		{
		case ImageToggleState.State.Disabled:
			this.SetTargetImageColor(this.DisabledColour);
			break;
		case ImageToggleState.State.Inactive:
			this.SetTargetImageColor(this.InactiveColour);
			break;
		case ImageToggleState.State.Active:
			this.SetTargetImageColor(this.ActiveColour);
			break;
		case ImageToggleState.State.DisabledActive:
			this.SetTargetImageColor(this.DisabledActiveColour);
			break;
		}
	}

	public void OnHoverIn()
	{
		this.SetTargetImageColor((this.currentState != ImageToggleState.State.Disabled && this.currentState != ImageToggleState.State.DisabledActive) ? this.HoverColour : this.DisabledHoverColor);
	}

	public void OnHoverOut()
	{
		this.ResetColor();
	}

	public void SetInactive()
	{
		if (this.currentState != ImageToggleState.State.Inactive)
		{
			this.isActive = false;
			this.currentState = ImageToggleState.State.Inactive;
			this.SetTargetImageColor(this.InactiveColour);
			if (!(this.TargetImage == null))
			{
				if (this.useSprites)
				{
					if (this.InactiveSprite != null && this.TargetImage.sprite != this.InactiveSprite)
					{
						this.TargetImage.sprite = this.InactiveSprite;
					}
					else if (this.InactiveSprite == null)
					{
						this.TargetImage.sprite = null;
					}
				}
			}
		}
	}

	public void SetDisabled()
	{
		if (this.currentState == ImageToggleState.State.Disabled)
		{
			this.SetTargetImageColor(this.DisabledColour);
		}
		else
		{
			this.isActive = false;
			this.currentState = ImageToggleState.State.Disabled;
			this.SetTargetImageColor(this.DisabledColour);
			if (!(this.TargetImage == null))
			{
				if (this.useSprites)
				{
					if (this.DisabledSprite != null && this.TargetImage.sprite != this.DisabledSprite)
					{
						this.TargetImage.sprite = this.DisabledSprite;
					}
					else if (this.DisabledSprite == null)
					{
						this.TargetImage.sprite = null;
					}
				}
			}
		}
	}

	public void SetDisabledActive()
	{
		this.isActive = false;
		this.currentState = ImageToggleState.State.DisabledActive;
		if (!(this.TargetImage == null))
		{
			this.SetTargetImageColor(this.DisabledActiveColour);
			if (this.useSprites)
			{
				if (this.DisabledActiveSprite != null && this.TargetImage.sprite != this.DisabledActiveSprite)
				{
					this.TargetImage.sprite = this.DisabledActiveSprite;
				}
				else if (this.DisabledActiveSprite == null)
				{
					this.TargetImage.sprite = null;
				}
			}
		}
	}

	public Image TargetImage;

	public Sprite ActiveSprite;

	public Sprite InactiveSprite;

	public Sprite DisabledSprite;

	public Sprite DisabledActiveSprite;

	public bool useSprites = false;

	public Color ActiveColour = Color.white;

	public Color InactiveColour = Color.white;

	public Color DisabledColour = Color.white;

	public Color DisabledActiveColour = Color.white;

	public Color HoverColour = Color.white;

	public Color DisabledHoverColor = Color.white;

	public ColorStyleSetting colorStyleSetting;

	private bool isActive = false;

	private ImageToggleState.State currentState = ImageToggleState.State.Inactive;

	public bool useStartingState = false;

	public ImageToggleState.State startingState = ImageToggleState.State.Inactive;

	public enum State
	{
		Disabled,
		Inactive,
		Active,
		DisabledActive
	}
}
