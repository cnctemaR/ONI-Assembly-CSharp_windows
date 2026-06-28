using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiToggle : KMonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public int CurrentState
	{
		get
		{
			return this.state;
		}
	}

	public void NextState()
	{
		this.ChangeState((this.state + 1) % this.states.Length);
	}

	public void ChangeState(int new_state_index)
	{
		this.state = new_state_index;
		this.toggle_image.sprite = this.states[new_state_index].sprite;
		this.toggle_image.color = this.states[new_state_index].color;
		foreach (StatePresentationSetting statePresentationSetting in this.states[this.state].additional_display_settings)
		{
			if (!(statePresentationSetting.image_target == null))
			{
				statePresentationSetting.image_target.sprite = statePresentationSetting.sprite;
				statePresentationSetting.image_target.color = statePresentationSetting.color;
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.play_sound_on_click)
		{
			if (this.states[this.state].on_click_override_sound_path == "")
			{
				KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Click", false));
			}
			else
			{
				KFMOD.PlayOneShot(GlobalAssets.GetSound(this.states[this.state].on_click_override_sound_path, false));
			}
		}
		if (this.onClick != null)
		{
			this.onClick();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (KInputManager.isFocused)
		{
			KInputManager.SetUserActive();
			if (this.states.Length != 0)
			{
				if (this.states[this.state].use_color_on_hover && this.states[this.state].color_on_hover != this.states[this.state].color)
				{
					this.toggle_image.color = this.states[this.state].color_on_hover;
				}
				foreach (StatePresentationSetting statePresentationSetting in this.states[this.state].additional_display_settings)
				{
					if (!(statePresentationSetting.image_target == null))
					{
						if (statePresentationSetting.use_color_on_hover)
						{
							statePresentationSetting.image_target.color = statePresentationSetting.color_on_hover;
						}
					}
				}
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (KInputManager.isFocused)
		{
			KInputManager.SetUserActive();
			if (this.states.Length != 0)
			{
				if (this.states[this.state].use_color_on_hover && this.states[this.state].color_on_hover != this.states[this.state].color)
				{
					this.toggle_image.color = this.states[this.state].color;
				}
				foreach (StatePresentationSetting statePresentationSetting in this.states[this.state].additional_display_settings)
				{
					if (!(statePresentationSetting.image_target == null))
					{
						if (statePresentationSetting.use_color_on_hover)
						{
							statePresentationSetting.image_target.color = statePresentationSetting.color;
						}
					}
				}
			}
		}
	}

	[Header("Settings")]
	[SerializeField]
	public ToggleState[] states;

	public bool play_sound_on_click = true;

	public Image toggle_image;

	protected int state;

	public global::System.Action onClick;
}
