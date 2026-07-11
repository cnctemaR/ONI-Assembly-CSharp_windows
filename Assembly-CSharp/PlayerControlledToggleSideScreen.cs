using System;
using UnityEngine;

public class PlayerControlledToggleSideScreen : SideScreenContent, IRenderEveryTick, ISim200ms
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.toggleButton.onClick += this.RequestToggle;
		this.togglePendingStatusItem = new StatusItem("PlayerControlledToggleSideScreen", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<IPlayerControlledToggle>() != null;
	}

	public void RenderEveryTick(float dt)
	{
		if (base.isActiveAndEnabled)
		{
			if (!this.keyDown && (Input.GetKeyDown(KeyCode.Return) & (Time.unscaledTime - this.lastKeyboardShortcutTime > 0.1f)))
			{
				if (SpeedControlScreen.Instance.IsPaused)
				{
					this.RequestToggle();
				}
				else
				{
					this.Toggle();
				}
				this.lastKeyboardShortcutTime = Time.unscaledTime;
				this.keyDown = true;
			}
			if (this.keyDown && Input.GetKeyUp(KeyCode.Return))
			{
				this.keyDown = false;
			}
		}
	}

	public void Sim200ms(float dt)
	{
		if (this.toggleRequested)
		{
			this.Toggle();
		}
	}

	private void RequestToggle()
	{
		this.toggleRequested = !this.toggleRequested;
		if (this.toggleRequested && SpeedControlScreen.Instance.IsPaused)
		{
			this.target.GetSelectable().SetStatusItem(Db.Get().StatusItemCategories.Main, this.togglePendingStatusItem, this);
		}
		else
		{
			this.target.GetSelectable().SetStatusItem(Db.Get().StatusItemCategories.Main, null, null);
		}
		this.UpdateVisuals(this.toggleRequested, true);
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			global::Debug.LogError("Invalid gameObject received");
			return;
		}
		this.target = new_target.GetComponent<IPlayerControlledToggle>();
		if (this.target == null)
		{
			global::Debug.LogError("The gameObject received is not an IPlayerControlledToggle");
			return;
		}
		this.UpdateVisuals(this.target.ToggledOn(), false);
		this.titleKey = this.target.SideScreenTitleKey;
	}

	private void Toggle()
	{
		this.target.ToggledByPlayer();
		this.UpdateVisuals(this.target.ToggledOn(), true);
		this.toggleRequested = false;
		this.target.GetSelectable().RemoveStatusItem(this.togglePendingStatusItem, false);
	}

	private void UpdateVisuals(bool state, bool smooth)
	{
		if (state != this.currentState)
		{
			if (smooth)
			{
				this.kbac.Play(state ? PlayerControlledToggleSideScreen.ON_ANIMS : PlayerControlledToggleSideScreen.OFF_ANIMS, KAnim.PlayMode.Once);
			}
			else
			{
				this.kbac.Play(state ? PlayerControlledToggleSideScreen.ON_ANIMS[1] : PlayerControlledToggleSideScreen.OFF_ANIMS[1], KAnim.PlayMode.Once, 1f, 0f);
			}
		}
		this.currentState = state;
	}

	public IPlayerControlledToggle target;

	public KButton toggleButton;

	protected static readonly HashedString[] ON_ANIMS = new HashedString[] { "on_pre", "on" };

	protected static readonly HashedString[] OFF_ANIMS = new HashedString[] { "off_pre", "off" };

	private StatusItem togglePendingStatusItem;

	[SerializeField]
	private KBatchedAnimController kbac;

	private float lastKeyboardShortcutTime;

	private const float KEYBOARD_COOLDOWN = 0.1f;

	private bool keyDown;

	private bool toggleRequested;

	private bool currentState;
}
