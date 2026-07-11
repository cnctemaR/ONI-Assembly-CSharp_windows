using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiToggle : KMonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
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

	protected virtual void Update()
	{
		if (this.clickHeldDown)
		{
			this.totalHeldTime += Time.unscaledDeltaTime;
			if (this.totalHeldTime > this.heldTimeThreshold && this.onHold != null)
			{
				this.onHold();
			}
		}
	}

	public void ChangeState(int new_state_index)
	{
		this.state = new_state_index;
		try
		{
			this.toggle_image.sprite = this.states[new_state_index].sprite;
			this.toggle_image.color = this.states[new_state_index].color;
			if (this.states[new_state_index].use_rect_margins)
			{
				this.toggle_image.rectTransform().sizeDelta = this.states[new_state_index].rect_margins;
			}
		}
		catch
		{
			string text = base.gameObject.name;
			Transform transform = base.transform;
			while (transform.parent != null)
			{
				text = text.Insert(0, transform.name + ">");
				transform = transform.parent;
			}
			global::Debug.LogError(string.Concat(new object[] { "Multi Toggle state index out of range: ", text, " idx:", new_state_index }), base.gameObject);
		}
		foreach (StatePresentationSetting statePresentationSetting in this.states[this.state].additional_display_settings)
		{
			if (!(statePresentationSetting.image_target == null))
			{
				statePresentationSetting.image_target.sprite = statePresentationSetting.sprite;
				statePresentationSetting.image_target.color = statePresentationSetting.color;
			}
		}
		this.RefreshHoverColor();
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (this.states.Length - 1 < this.state)
		{
			global::Debug.LogWarning("Multi toggle has too few / no states");
		}
		if (this.onClick != null)
		{
			this.onClick();
		}
		this.RefreshHoverColor();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		this.pointerOver = true;
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		if (this.states.Length == 0)
		{
			return;
		}
		if (this.states[this.state].use_color_on_hover && this.states[this.state].color_on_hover != this.states[this.state].color)
		{
			this.toggle_image.color = this.states[this.state].color_on_hover;
		}
		if (this.states[this.state].use_rect_margins)
		{
			this.toggle_image.rectTransform().sizeDelta = this.states[this.state].rect_margins;
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
		if (this.onEnter != null)
		{
			this.onEnter();
		}
	}

	protected void RefreshHoverColor()
	{
		if (this.pointerOver)
		{
			if (this.states[this.state].use_color_on_hover && this.states[this.state].color_on_hover != this.states[this.state].color)
			{
				this.toggle_image.color = this.states[this.state].color_on_hover;
			}
			foreach (StatePresentationSetting statePresentationSetting in this.states[this.state].additional_display_settings)
			{
				if (!(statePresentationSetting.image_target == null))
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
		this.pointerOver = false;
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		if (this.states.Length == 0)
		{
			return;
		}
		if (this.states[this.state].use_color_on_hover && this.states[this.state].color_on_hover != this.states[this.state].color)
		{
			this.toggle_image.color = this.states[this.state].color;
		}
		if (this.states[this.state].use_rect_margins)
		{
			this.toggle_image.rectTransform().sizeDelta = this.states[this.state].rect_margins;
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
		if (this.onExit != null)
		{
			this.onExit();
		}
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		this.clickHeldDown = true;
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
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		if (this.clickHeldDown)
		{
			if (this.play_sound_on_release && this.states[this.state].on_release_override_sound_path != string.Empty)
			{
				KFMOD.PlayOneShot(GlobalAssets.GetSound(this.states[this.state].on_release_override_sound_path, false));
			}
			this.clickHeldDown = false;
			if (this.onStopHold != null)
			{
				this.onStopHold();
			}
		}
		this.totalHeldTime = 0f;
	}

	[Header("Settings")]
	[SerializeField]
	public ToggleState[] states;

	public bool play_sound_on_click = true;

	public bool play_sound_on_release;

	public Image toggle_image;

	protected int state;

	public global::System.Action onClick;

	public global::System.Action onEnter;

	public global::System.Action onExit;

	public global::System.Action onHold;

	public global::System.Action onStopHold;

	protected bool clickHeldDown;

	protected float totalHeldTime;

	protected float heldTimeThreshold = 0.4f;

	private bool pointerOver;
}
