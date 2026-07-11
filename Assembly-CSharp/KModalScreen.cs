using System;
using UnityEngine;
using UnityEngine.UI;

public class KModalScreen : KScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ConsumeMouseScroll = true;
		this.activateOnSpawn = true;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (CameraController.Instance != null)
		{
			CameraController.Instance.DisableUserCameraControl = true;
		}
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (CameraController.Instance != null)
		{
			CameraController.Instance.DisableUserCameraControl = false;
		}
		base.Trigger(476357528, null);
	}

	public override bool IsModal()
	{
		return true;
	}

	public override float GetSortKey()
	{
		return 100f;
	}

	protected override void OnActivate()
	{
		this.OnShow(true);
	}

	protected override void OnDeactivate()
	{
		this.OnShow(false);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (this.pause && SpeedControlScreen.Instance != null)
		{
			if (show && !this.shown)
			{
				SpeedControlScreen.Instance.Pause(false);
			}
			else if (!show && this.shown)
			{
				SpeedControlScreen.Instance.Unpause(false);
			}
			this.shown = show;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (Game.Instance != null && (e.TryConsume(global::Action.TogglePause) || e.TryConsume(global::Action.CycleSpeed)))
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		}
		if (!e.Consumed && e.TryConsume(global::Action.Escape))
		{
			this.Deactivate();
		}
		if (!e.Consumed)
		{
			KScrollRect componentInChildren = base.GetComponentInChildren<KScrollRect>();
			if (componentInChildren != null)
			{
				componentInChildren.OnKeyDown(e);
			}
		}
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			KScrollRect componentInChildren = base.GetComponentInChildren<KScrollRect>();
			if (componentInChildren != null)
			{
				componentInChildren.OnKeyUp(e);
			}
		}
		e.Consumed = true;
	}

	public void SetBackgroundActive(bool active)
	{
		int num = ((!active) ? 0 : 70);
		base.GetComponent<Image>().color = new Color32(0, 0, 0, (byte)num);
	}

	private bool shown;

	public bool pause = true;

	public const float SCREEN_SORT_KEY = 100f;
}
