using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

internal class LogicEventHandler : ILogicEventReceiver, ILogicUIElement, ILogicNetworkConnection, IUniformGridObject
{
	public LogicEventHandler(int cell, Action<int> on_value_changed, Action<int, bool> on_connection_changed, LogicPortSpriteType sprite_type)
	{
		this.cell = cell;
		this.onValueChanged = on_value_changed;
		this.onConnectionChanged = on_connection_changed;
		this.spriteType = sprite_type;
	}

	public void ReceiveLogicEvent(int value)
	{
		this.TriggerAudio(value);
		this.value = value;
		this.onValueChanged(value);
	}

	public int Value
	{
		get
		{
			return this.value;
		}
	}

	public int GetLogicUICell()
	{
		return this.cell;
	}

	public LogicPortSpriteType GetLogicPortSpriteType()
	{
		return this.spriteType;
	}

	public Vector2 PosMin()
	{
		return Grid.CellToPos2D(this.cell);
	}

	public Vector2 PosMax()
	{
		return Grid.CellToPos2D(this.cell);
	}

	public int GetLogicCell()
	{
		return this.cell;
	}

	private void TriggerAudio(int new_value)
	{
		LogicCircuitNetwork networkForCell = Game.Instance.logicCircuitManager.GetNetworkForCell(this.cell);
		SpeedControlScreen instance = SpeedControlScreen.Instance;
		if (networkForCell != null && new_value != this.value && instance != null && !instance.IsPaused)
		{
			if (KPlayerPrefs.HasKey(AudioOptionsScreen.AlwaysPlayAutomation) && KPlayerPrefs.GetInt(AudioOptionsScreen.AlwaysPlayAutomation) != 1 && OverlayScreen.Instance.GetMode() != OverlayModes.Logic.ID)
			{
				return;
			}
			string text = "Logic_Building_Toggle";
			if (!CameraController.Instance.IsAudibleSound(Grid.CellToPosCCC(this.cell, Grid.SceneLayer.BuildingFront)))
			{
				return;
			}
			LogicCircuitNetwork.LogicSoundPair logicSoundPair = new LogicCircuitNetwork.LogicSoundPair();
			Dictionary<int, LogicCircuitNetwork.LogicSoundPair> logicSoundRegister = LogicCircuitNetwork.logicSoundRegister;
			int id = networkForCell.id;
			if (!logicSoundRegister.ContainsKey(id))
			{
				logicSoundRegister.Add(id, logicSoundPair);
			}
			else
			{
				logicSoundPair.playedIndex = logicSoundRegister[id].playedIndex;
				logicSoundPair.lastPlayed = logicSoundRegister[id].lastPlayed;
			}
			if (logicSoundPair.playedIndex < 2)
			{
				logicSoundRegister[id].playedIndex = logicSoundPair.playedIndex + 1;
			}
			else
			{
				logicSoundRegister[id].playedIndex = 0;
				logicSoundRegister[id].lastPlayed = Time.time;
			}
			float num = (Time.time - logicSoundPair.lastPlayed) / 5f;
			EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound(text, false), Grid.CellToPos(this.cell), 1f);
			eventInstance.setParameterValue("logic_volumeModifer", num);
			eventInstance.setParameterValue("wireCount", (float)(networkForCell.Wires.Count % 24));
			eventInstance.setParameterValue("enabled", (float)new_value);
			KFMOD.EndOneShot(eventInstance);
		}
	}

	public void OnLogicNetworkConnectionChanged(bool connected)
	{
		if (this.onConnectionChanged != null)
		{
			this.onConnectionChanged(this.cell, connected);
		}
	}

	private int cell;

	private int value;

	private Action<int> onValueChanged;

	private Action<int, bool> onConnectionChanged;

	private LogicPortSpriteType spriteType;
}
