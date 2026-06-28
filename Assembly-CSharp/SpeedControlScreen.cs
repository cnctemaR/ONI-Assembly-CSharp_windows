using System;
using System.Collections;
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
		KToggle[] array = new KToggle[] { this.pauseButton, this.slowButton, this.mediumButton, this.fastButton };
		foreach (KToggle ktoggle in array)
		{
			ktoggle.soundPlayer.Enabled = false;
		}
		this.slowButton.onClick += delegate
		{
			this.PlaySpeedChangeSound(1f);
			this.SetSpeed(0);
		};
		this.mediumButton.onClick += delegate
		{
			this.PlaySpeedChangeSound(2f);
			this.SetSpeed(1);
		};
		this.fastButton.onClick += delegate
		{
			this.PlaySpeedChangeSound(3f);
			this.SetSpeed(2);
		};
		this.pauseButton.onClick += delegate
		{
			this.TogglePause(true);
		};
		this.playButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip("Play " + GameUtil.GetHotkeyString(global::Action.TogglePause), this.TooltipTextStyle);
		this.pauseButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip("Pause " + GameUtil.GetHotkeyString(global::Action.TogglePause), this.TooltipTextStyle);
		this.speedButtonWidget_slow.GetComponent<ToolTip>().AddMultiStringTooltip(string.Format(UI.TOOLTIPS.SPEEDBUTTON_SLOW, GameUtil.GetHotkeyString(global::Action.CycleSpeed)), this.TooltipTextStyle);
		this.speedButtonWidget_medium.GetComponent<ToolTip>().AddMultiStringTooltip(string.Format(UI.TOOLTIPS.SPEEDBUTTON_MEDIUM, GameUtil.GetHotkeyString(global::Action.CycleSpeed)), this.TooltipTextStyle);
		this.speedButtonWidget_fast.GetComponent<ToolTip>().AddMultiStringTooltip(string.Format(UI.TOOLTIPS.SPEEDBUTTON_FAST, GameUtil.GetHotkeyString(global::Action.CycleSpeed)), this.TooltipTextStyle);
		this.playButtonWidget.GetComponent<KButton>().onClick += delegate
		{
			this.TogglePause(true);
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
		int num = this.speed;
		if (num != 0)
		{
			if (num != 1)
			{
				if (num == 2)
				{
					this.fastButton.Select();
					this.slowButton.isOn = false;
					this.mediumButton.isOn = false;
					this.fastButton.isOn = true;
				}
			}
			else
			{
				this.mediumButton.Select();
				this.slowButton.isOn = false;
				this.mediumButton.isOn = true;
				this.fastButton.isOn = false;
			}
		}
		else
		{
			this.slowButton.Select();
			this.slowButton.isOn = true;
			this.mediumButton.isOn = false;
			this.fastButton.isOn = false;
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

	public void TogglePause(bool playsound = true)
	{
		if (this.IsPaused)
		{
			this.Unpause(playsound);
		}
		else
		{
			this.Pause(playsound);
		}
	}

	public void Pause(bool playSound = true)
	{
		this.pauseCount++;
		if (this.pauseCount == 1)
		{
			if (playSound)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Speed_Pause", false));
				if (SoundListenerController.Instance != null)
				{
					SoundListenerController.Instance.SetLoopingVolume(0f);
				}
			}
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().SpeedPausedMigrated);
			MusicManager.instance.SetDynamicMusicPaused();
			this.pauseButtonWidget.GetComponent<ToolTip>().ClearMultiStringTooltip();
			this.pauseButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip(UI.TOOLTIPS.UNPAUSE + " " + GameUtil.GetHotkeyString(global::Action.TogglePause), this.TooltipTextStyle);
			this.pauseButton.isOn = true;
			this.OnPause();
		}
	}

	public void Unpause(bool playSound = true)
	{
		this.pauseCount = Mathf.Max(0, this.pauseCount - 1);
		if (this.pauseCount == 0)
		{
			if (playSound)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Speed_Unpause", false));
				if (SoundListenerController.Instance != null)
				{
					SoundListenerController.Instance.SetLoopingVolume(1f);
				}
			}
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().SpeedPausedMigrated, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.SetDynamicMusicUnpaused();
			this.pauseButtonWidget.GetComponent<ToolTip>().ClearMultiStringTooltip();
			this.pauseButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip(UI.TOOLTIPS.PAUSE + " " + GameUtil.GetHotkeyString(global::Action.TogglePause), this.TooltipTextStyle);
			this.pauseButton.isOn = false;
			this.SetSpeed(this.speed);
			this.OnPlay();
		}
	}

	private void OnPause()
	{
		this.OnChanged();
	}

	private void OnPlay()
	{
		this.OnChanged();
	}

	public void OnSpeedChange()
	{
		if (!Game.IsQuitting())
		{
			this.OnChanged();
		}
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
			this.TogglePause(true);
		}
		else if (e.TryConsume(global::Action.CycleSpeed))
		{
			this.PlaySpeedChangeSound((float)((this.speed + 1) % 3 + 1));
			this.SetSpeed(this.speed + 1);
			this.OnSpeedChange();
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

	public void DebugStepFrame()
	{
		Output.Log(new object[] { "Stepping one frame" });
		this.Unpause(false);
		base.StartCoroutine(this.DebugStepFrameDelay());
	}

	private IEnumerator DebugStepFrameDelay()
	{
		yield return null;
		this.Pause(false);
		yield break;
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

	private int pauseCount = 0;
}
