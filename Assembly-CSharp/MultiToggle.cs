using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiToggle : KMonoBehaviour, IEventSystemHandler, IPointerClickHandler
{
	protected void NextState()
	{
		this.ChangeState((this.state + 1) % this.states.Length);
	}

	public void ChangeState(int new_state_index)
	{
		this.state = new_state_index;
		this.toggle_image.sprite = this.states[new_state_index].sprite;
		this.toggle_image.color = this.states[new_state_index].color;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.play_sound_on_click)
		{
			if (this.states[this.state].on_click_override_sound_path == string.Empty)
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

	public bool play_sound_on_click = true;

	protected int state;

	public global::System.Action onClick;

	[SerializeField]
	public ToggleState[] states;

	public Image toggle_image;
}
