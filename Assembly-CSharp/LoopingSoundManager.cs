using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class LoopingSoundManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		LoopingSoundManager.instance = this;
	}

	protected override void OnSpawn()
	{
		if (SpeedControlScreen.Instance != null && Game.Instance != null)
		{
			SpeedControlScreen speedControlScreen = SpeedControlScreen.Instance;
			speedControlScreen.OnGameSpeedChanged = (global::System.Action)Delegate.Combine(speedControlScreen.OnGameSpeedChanged, new global::System.Action(LoopingSoundManager.instance.OnGameSpeedChanged));
			Game.Instance.Subscribe(-1788536802, new Action<object>(LoopingSoundManager.instance.OnPauseChanged));
		}
	}

	protected override void OnCleanUp()
	{
		if (SpeedControlScreen.Instance != null)
		{
			SpeedControlScreen speedControlScreen = SpeedControlScreen.Instance;
			speedControlScreen.OnGameSpeedChanged = (global::System.Action)Delegate.Remove(speedControlScreen.OnGameSpeedChanged, new global::System.Action(LoopingSoundManager.instance.OnGameSpeedChanged));
		}
	}

	private void Update()
	{
		this.RefreshAllSounds();
	}

	public static LoopingSoundManager Get()
	{
		return LoopingSoundManager.instance;
	}

	public void RefreshAllSounds()
	{
		if (this.entries.Count > 0)
		{
			foreach (LoopingSoundManager.Entry entry in this.entries.Values)
			{
				entry.Refresh();
			}
		}
	}

	public void StopAllSounds()
	{
		foreach (LoopingSoundManager.Entry entry in this.entries.Values)
		{
			entry.StopAll(STOP_MODE.IMMEDIATE);
		}
	}

	public void Add(string path, EventInstance ev, bool pauseOnGamePause = true)
	{
		LoopingSoundManager.Entry entry = null;
		if (!this.entries.TryGetValue(path, out entry))
		{
			entry = new LoopingSoundManager.Entry(path, ev, pauseOnGamePause);
			this.entries[path] = entry;
		}
		entry.Add(ev);
	}

	public void Remove(string path, EventInstance ev)
	{
		if (this.entries.ContainsKey(path))
		{
			this.entries[path].Remove(ev);
		}
	}

	public static EventInstance PrepareSound(string path, Vector3 pos, bool pauseOnGamePause = true)
	{
		EventInstance eventInstance;
		if (path == null)
		{
			global::Debug.LogWarning("Missing sound", null);
			eventInstance = null;
		}
		else
		{
			EventInstance eventInstance2 = KFMOD.CreateInstance(path);
			if (eventInstance2 == null)
			{
				Output.LogError(new object[] { "StartSound() Couldnt Get FMOD event for asset [" + path + "]" });
				eventInstance = null;
			}
			else
			{
				LoopingSoundManager.Get().Add(path, eventInstance2, pauseOnGamePause);
				Vector3 vector = new Vector3(pos.x, pos.y, 0f);
				eventInstance2.set3DAttributes(SoundEvent.GetCameraScaledPosition(vector).To3DAttributes());
				LoopingSoundManager.UpdateSpeed(eventInstance2);
				if (Time.timeScale == 0f)
				{
					eventInstance2.setPaused(true);
				}
				eventInstance = eventInstance2;
			}
		}
		return eventInstance;
	}

	public static EventInstance StartSound(string path, Vector3 pos, bool pauseOnGamePause = true)
	{
		EventInstance eventInstance;
		if (path == null)
		{
			global::Debug.LogWarning("Missing sound", null);
			eventInstance = null;
		}
		else
		{
			EventInstance eventInstance2 = KFMOD.CreateInstance(path);
			if (eventInstance2 == null)
			{
				Output.LogError(new object[] { "StartSound() Couldnt Get FMOD event for asset [" + path + "]" });
				eventInstance = null;
			}
			else
			{
				LoopingSoundManager.Get().Add(path, eventInstance2, pauseOnGamePause);
				Vector3 vector = new Vector3(pos.x, pos.y, 0f);
				eventInstance2.set3DAttributes(SoundEvent.GetCameraScaledPosition(vector).To3DAttributes());
				LoopingSoundManager.UpdateSpeed(eventInstance2);
				eventInstance2.start();
				if (Time.timeScale == 0f && pauseOnGamePause)
				{
					eventInstance2.setPaused(true);
				}
				eventInstance = eventInstance2;
			}
		}
		return eventInstance;
	}

	public static RESULT StopSound(string path, EventInstance ev)
	{
		RESULT result = ev.stop(STOP_MODE.ALLOWFADEOUT);
		result = ev.release();
		LoopingSoundManager.Get().Remove(path, ev);
		return result;
	}

	private void OnGameSpeedChanged()
	{
		float num = Time.timeScale * 1f;
		foreach (KeyValuePair<HashedString, LoopingSoundManager.Entry> keyValuePair in this.entries)
		{
			keyValuePair.Value.UpdateSpeed(num);
		}
	}

	public static void UpdateSpeed(EventInstance ev)
	{
		if (ev != null)
		{
			ev.setParameterValue(LoopingSoundManager.SPEED_ID, Time.timeScale * 1f);
		}
	}

	private void OnPauseChanged(object data)
	{
		bool flag = (bool)data;
		foreach (KeyValuePair<HashedString, LoopingSoundManager.Entry> keyValuePair in this.entries)
		{
			if (keyValuePair.Value.pauseOnGamePaused)
			{
				keyValuePair.Value.SetPaused(flag);
			}
		}
	}

	private static LoopingSoundManager instance;

	private Dictionary<HashedString, LoopingSoundManager.Entry> entries = new Dictionary<HashedString, LoopingSoundManager.Entry>();

	private static ParameterID OBJECT_COUNT_ID = new ParameterID("objectCount");

	private static ParameterID SPEED_ID = new ParameterID("Speed");

	private class Entry
	{
		public Entry(string path, EventInstance ev, bool pauseOnGamePaused = true)
		{
			this.pauseOnGamePaused = pauseOnGamePaused;
			EventDescription eventDescription;
			ev.getDescription(out eventDescription);
			USER_PROPERTY user_PROPERTY;
			if (eventDescription.getUserProperty("minObj", out user_PROPERTY) == RESULT.OK)
			{
				this.minObjects = user_PROPERTY.floatValue;
			}
			else
			{
				this.minObjects = 1f;
			}
			USER_PROPERTY user_PROPERTY2;
			if (eventDescription.getUserProperty("maxObj", out user_PROPERTY2) == RESULT.OK)
			{
				this.maxObjects = user_PROPERTY2.floatValue;
			}
			else
			{
				this.maxObjects = 0f;
			}
			USER_PROPERTY user_PROPERTY3;
			if (eventDescription.getUserProperty("curveType", out user_PROPERTY3) == RESULT.OK)
			{
				this.curveType = user_PROPERTY2.stringValue;
			}
			else
			{
				this.curveType = "";
			}
		}

		public void Add(EventInstance ev)
		{
			this.events.Add(ev);
		}

		public void Remove(EventInstance ev)
		{
			this.events.Remove(ev);
		}

		public void Refresh()
		{
			float num = 0f;
			foreach (EventInstance eventInstance in this.events)
			{
				if (CameraController.Instance == null || CameraController.Instance.IsAudibleSound(KFMOD.GetInstancePosition(eventInstance), 0f))
				{
					num += 1f;
				}
			}
			if (this.events.Count > 0)
			{
				float num2;
				if (this.maxObjects == this.minObjects)
				{
					num2 = 0f;
				}
				else
				{
					num2 = (num - this.minObjects) / (this.maxObjects - this.minObjects);
					num2 = Mathf.Clamp01(num2);
				}
				if (this.curveType == "exp")
				{
					num2 *= num2;
				}
				foreach (EventInstance eventInstance2 in this.events)
				{
					eventInstance2.setParameterValue(LoopingSoundManager.OBJECT_COUNT_ID, num2);
				}
			}
		}

		public void UpdateSpeed(float speed)
		{
			foreach (EventInstance eventInstance in this.events)
			{
				if (eventInstance != null)
				{
					eventInstance.setParameterValue(LoopingSoundManager.SPEED_ID, speed);
				}
			}
		}

		public void SetPaused(bool paused)
		{
			foreach (EventInstance eventInstance in this.events)
			{
				if (eventInstance != null)
				{
					eventInstance.setPaused(paused);
				}
			}
		}

		public void StopAll(STOP_MODE stop_mode = STOP_MODE.IMMEDIATE)
		{
			foreach (EventInstance eventInstance in this.events)
			{
				if (eventInstance != null)
				{
					eventInstance.stop(stop_mode);
				}
			}
			this.events.Clear();
		}

		private List<EventInstance> events = new List<EventInstance>();

		private float minObjects;

		private float maxObjects;

		private string curveType;

		public bool pauseOnGamePaused;
	}
}
