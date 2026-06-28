using System;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SpeedControlScreen : KScreen
{
	public static SpeedControlScreen Instance { get; private set; }

	public bool IsPaused
	{
		get
		{
			return this.pauseCount > 0;
		}
	}

	protected override void OnPrefabInit()
	{
		SpeedControlScreen.Instance = this;
		this.pauseButton = this.pauseButtonWidget.GetComponent<KToggle>();
		this.slowButton = this.speedButtonWidget_slow.GetComponent<KToggle>();
		this.mediumButton = this.speedButtonWidget_medium.GetComponent<KToggle>();
		this.fastButton = this.speedButtonWidget_fast.GetComponent<KToggle>();
		this.slowButton.onClick += delegate
		{
			this.SetSpeed(0);
			this.PlaySpeedChangeSound((float)(this.speed + 1));
		};
		this.mediumButton.onClick += delegate
		{
			this.SetSpeed(1);
			this.PlaySpeedChangeSound((float)(this.speed + 1));
		};
		this.fastButton.onClick += delegate
		{
			this.SetSpeed(2);
			this.PlaySpeedChangeSound((float)(this.speed + 1));
		};
		this.pauseButton.onClick += delegate
		{
			this.TogglePause();
		};
		this.playButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip("Play " + GameUtil.GetHotkeyString(global::Action.TogglePause), this.TooltipTextStyle);
		this.pauseButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip("Pause " + GameUtil.GetHotkeyString(global::Action.TogglePause), this.TooltipTextStyle);
		this.speedButtonWidget_slow.GetComponent<ToolTip>().AddMultiStringTooltip(string.Format(UI.TOOLTIPS.SPEEDBUTTON_SLOW, GameUtil.GetHotkeyString(global::Action.CycleSpeed)), this.TooltipTextStyle);
		this.speedButtonWidget_medium.GetComponent<ToolTip>().AddMultiStringTooltip(string.Format(UI.TOOLTIPS.SPEEDBUTTON_MEDIUM, GameUtil.GetHotkeyString(global::Action.CycleSpeed)), this.TooltipTextStyle);
		this.speedButtonWidget_fast.GetComponent<ToolTip>().AddMultiStringTooltip(string.Format(UI.TOOLTIPS.SPEEDBUTTON_FAST, GameUtil.GetHotkeyString(global::Action.CycleSpeed)), this.TooltipTextStyle);
		this.playButtonWidget.GetComponent<KButton>().onClick += delegate
		{
			this.TogglePause();
		};
	}

	protected override void OnSpawn()
	{
		if (SaveGame.Instance != null)
		{
			this.speed = SaveGame.Instance.GetSpeed();
			this.SetSpeed(this.speed);
		}
		base.OnSpawn();
		this.OnChanged();
	}

	public int GetSpeed()
	{
		return this.speed;
	}

	public void SetSpeed(int Speed)
	{
		this.speed = Speed % 3;
		switch (this.speed)
		{
		case 0:
			this.slowButton.ActivateFlourish(true, ImageToggleState.State.Active);
			this.mediumButton.ActivateFlourish(false, ImageToggleState.State.Inactive);
			this.fastButton.ActivateFlourish(false, ImageToggleState.State.Inactive);
			break;
		case 1:
			this.slowButton.ActivateFlourish(false, ImageToggleState.State.Inactive);
			this.mediumButton.ActivateFlourish(true, ImageToggleState.State.Active);
			this.fastButton.ActivateFlourish(false, ImageToggleState.State.Inactive);
			break;
		case 2:
			this.slowButton.ActivateFlourish(false, ImageToggleState.State.Inactive);
			this.mediumButton.ActivateFlourish(false, ImageToggleState.State.Inactive);
			this.fastButton.ActivateFlourish(true, ImageToggleState.State.Active);
			break;
		}
		this.OnSpeedChange();
	}

	public void ToggleRidiculousSpeed()
	{
		if (this.ultraSpeed == 3f)
		{
			this.ultraSpeed = 10f;
		}
		else
		{
			this.ultraSpeed = 3f;
		}
		this.speed = 2;
		this.OnChanged();
	}

	public void TogglePause()
	{
		if (this.IsPaused)
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().SpeedPausedMigrated, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.SetDynamicMusicUnpaused();
			this.Unpause(true);
		}
		else
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().SpeedPausedMigrated);
			MusicManager.instance.SetDynamicMusicPaused();
			this.Pause(true);
		}
	}

	public void Pause(bool playSound = true)
	{
		this.pauseButtonWidget.GetComponent<ToolTip>().ClearMultiStringTooltip();
		this.pauseButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip(UI.TOOLTIPS.UNPAUSE + " " + GameUtil.GetHotkeyString(global::Action.TogglePause), this.TooltipTextStyle);
		this.pauseButton.ActivateFlourish(true, ImageToggleState.State.Active);
		this.pauseCount++;
		this.OnPause(playSound);
	}

	public void Unpause(bool playSound = true)
	{
		this.pauseButtonWidget.GetComponent<ToolTip>().ClearMultiStringTooltip();
		this.pauseButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip(UI.TOOLTIPS.PAUSE + " " + GameUtil.GetHotkeyString(global::Action.TogglePause), this.TooltipTextStyle);
		this.pauseButton.ActivateFlourish(false, ImageToggleState.State.Inactive);
		this.pauseCount = Mathf.Max(0, this.pauseCount - 1);
		this.SetSpeed(this.speed);
		this.OnPlay(playSound);
	}

	private void OnPause(bool playSound = true)
	{
		this.OnChanged();
		if (this.pauseCount == 1 && playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Speed_Pause", false));
			if (SoundListenerController.Instance != null)
			{
				SoundListenerController.Instance.SetLoopingVolume(0f);
			}
		}
	}

	private void OnPlay(bool playSound = true)
	{
		this.OnChanged();
		if (this.pauseCount == 0 && playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Speed_Unpause", false));
			if (SoundListenerController.Instance != null)
			{
				SoundListenerController.Instance.SetLoopingVolume(1f);
			}
		}
	}

	public void OnSpeedChange()
	{
		if (Game.IsQuitting())
		{
			return;
		}
		this.OnChanged();
	}

	private void OnChanged()
	{
		if (this.IsPaused)
		{
			Time.timeScale = 0f;
		}
		else if (this.speed == 0)
		{
			Time.timeScale = this.normalSpeed;
		}
		else if (this.speed == 1)
		{
			Time.timeScale = this.fastSpeed;
		}
		else if (this.speed == 2)
		{
			Time.timeScale = this.ultraSpeed;
		}
		if (this.OnGameSpeedChanged != null)
		{
			this.OnGameSpeedChanged();
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.TogglePause))
		{
			this.TogglePause();
		}
		else if (e.TryConsume(global::Action.SpeedUp))
		{
			this.speed++;
			this.speed = Math.Min(this.speed, 2);
			this.SetSpeed(this.speed);
		}
		else if (e.TryConsume(global::Action.SlowDown))
		{
			this.speed--;
			this.speed = Math.Max(this.speed, 0);
			this.SetSpeed(this.speed);
		}
		else if (e.TryConsume(global::Action.CycleSpeed))
		{
			this.SetSpeed(this.speed + 1);
			this.OnSpeedChange();
			this.PlaySpeedChangeSound((float)(this.speed + 1));
		}
	}

	private void PlaySpeedChangeSound(float speed)
	{
		string sound = GlobalAssets.GetSound("Speed_Change", false);
		if (sound != null)
		{
			EventInstance eventInstance = SoundEvent.BeginOneShot(sound, Vector3.zero);
			eventInstance.setParameterValue("Speed", speed);
			SoundEvent.EndOneShot(eventInstance);
		}
	}

	public GameObject playButtonWidget;

	public GameObject pauseButtonWidget;

	public Image playIcon;

	public Image pauseIcon;

	[SerializeField]
	private TextStyleSetting TooltipTextStyle;

	public GameObject speedButtonWidget_slow;

	public GameObject speedButtonWidget_medium;

	public GameObject speedButtonWidget_fast;

	public GameObject mainMenuWidget;

	public float normalSpeed;

	public float fastSpeed;

	public float ultraSpeed;

	public global::System.Action OnGameSpeedChanged;

	private KToggle pauseButton;

	private KToggle slowButton;

	private KToggle mediumButton;

	private KToggle fastButton;

	private int speed;

	private int pauseCount;
}
