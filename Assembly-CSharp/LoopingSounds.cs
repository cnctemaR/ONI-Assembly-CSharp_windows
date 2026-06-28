using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[SkipSaveFileSerialization]
public class LoopingSounds : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.updatePosition)
		{
			this.AddLoopingSoundUpdater();
		}
	}

	public void AddLoopingSoundUpdater()
	{
		if (!GameComps.LoopingSoundUpdaterComponents.Has(base.gameObject))
		{
			GameComps.LoopingSoundUpdaterComponents.Add(this);
		}
	}

	public void RemoveLoopingSoundUpdater()
	{
		if (GameComps.LoopingSoundUpdaterComponents.Has(base.gameObject))
		{
			GameComps.LoopingSoundUpdaterComponents.Remove(base.gameObject);
		}
	}

	public void DoUpdate()
	{
		Vector3 position = this.transform.position;
		for (int i = 0; i < this.loopingSounds.Count; i++)
		{
			EventInstance ev = this.loopingSounds[i].ev;
			Vector3 vector = new Vector3(position.x, position.y, 0f);
			ev.set3DAttributes(CameraController.Instance.GetVerticallyScaledPosition(vector).To3DAttributes());
			this.UpdateProgressParameter(this.loopingSounds[i]);
		}
	}

	private void OnStartLoopingSound(object data)
	{
		if (data is FMODAsset)
		{
			this.StartSound(GameUtil.MigrateFMOD(data as FMODAsset), this.transform.position);
		}
		else
		{
			this.StartSound(data as string, this.transform.position);
		}
	}

	public bool IsSoundPlaying(string path)
	{
		for (int i = 0; i < this.loopingSounds.Count; i++)
		{
			if (this.loopingSounds[i].asset == path)
			{
				return true;
			}
		}
		return false;
	}

	private void UpdateProgressParameter(LoopingSounds.SoundEvent sound)
	{
		string progressParameterName = sound.progressParameterName;
		if (progressParameterName == null)
		{
			return;
		}
		if (progressParameterName == "percentComplete")
		{
			Worker component = base.GetComponent<Worker>();
			Workable workable = null;
			if (component != null)
			{
				workable = component.workable;
			}
			if (workable != null)
			{
				float percentComplete = workable.GetPercentComplete();
				sound.ev.setParameterValue("percentComplete", percentComplete);
			}
		}
		else if (progressParameterName == "consumedMass")
		{
			ElementConsumer component2 = base.GetComponent<KMonoBehaviour>().GetComponent<ElementConsumer>();
			if (component2 != null)
			{
				sound.ev.setParameterValue("consumedMass", component2.consumedMass);
			}
		}
	}

	public bool StartSound(string asset, Vector3 pos)
	{
		if (asset == null || asset == string.Empty)
		{
			global::Debug.LogWarning("Missing sound", null);
			return false;
		}
		if (!this.IsSoundPlaying(asset))
		{
			EventInstance eventInstance = KFMOD.CreateInstance(asset);
			if (eventInstance == null)
			{
				Output.LogError(new object[] { "StartSound() Couldnt Get FMOD event for asset [" + asset + "]" });
				return false;
			}
			LoopingSounds.SoundEvent soundEvent = new LoopingSounds.SoundEvent
			{
				asset = asset,
				ev = eventInstance,
				progressParameter = null,
				progressParameterName = null
			};
			soundEvent.SetupProgressParameter();
			if (soundEvent.progressParameter != null)
			{
				this.AddLoopingSoundUpdater();
			}
			this.loopingSounds.Add(soundEvent);
			LoopingSoundManager.Get().Add(asset, eventInstance, true);
			Vector3 vector = new Vector3(pos.x, pos.y, 0f);
			eventInstance.set3DAttributes(CameraController.Instance.GetVerticallyScaledPosition(vector).To3DAttributes());
			LoopingSoundManager.UpdateSpeed(eventInstance);
			eventInstance.start();
			if (Time.timeScale == 0f)
			{
				eventInstance.setPaused(true);
			}
		}
		return true;
	}

	public void StopSound(string asset)
	{
		for (int i = 0; i < this.loopingSounds.Count; i++)
		{
			if (this.loopingSounds[i].asset == asset)
			{
				EventInstance ev = this.loopingSounds[i].ev;
				ev.stop(STOP_MODE.ALLOWFADEOUT);
				ev.release();
				this.loopingSounds.RemoveAt(i);
				LoopingSoundManager.Get().Remove(asset, ev);
				break;
			}
		}
	}

	private void OnStopLoopingSound(object data)
	{
		string text;
		if (data is FMODAsset)
		{
			text = GameUtil.MigrateFMOD(data as FMODAsset);
		}
		else
		{
			text = data as string;
		}
		this.StopSound(text);
	}

	public void StopAllSounds()
	{
		while (this.loopingSounds.Count > 0)
		{
			this.StopSound(this.loopingSounds[0].asset);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.RemoveLoopingSoundUpdater();
		this.StopAllSounds();
	}

	public void SetParameter(string path, string parameter, float value)
	{
		foreach (LoopingSounds.SoundEvent soundEvent in this.loopingSounds)
		{
			if (soundEvent.asset == path)
			{
				soundEvent.ev.setParameterValue(parameter, value);
			}
		}
	}

	public void PlayEvent(GameSoundEvents.Event ev)
	{
		if (AudioDebug.Get().debugGameEventSounds)
		{
			global::Debug.Log("GameSoundEvent: " + ev.Name, null);
		}
		List<AnimEvent> events = GameAudioSheets.Get().GetEvents(ev.Name);
		if (events == null)
		{
			return;
		}
		for (int i = 0; i < events.Count; i++)
		{
			AnimEvent animEvent = events[i];
			global::SoundEvent soundEvent = animEvent as global::SoundEvent;
			if (soundEvent == null || soundEvent.sound == null)
			{
				return;
			}
			if (AudioDebug.Get().debugGameEventSounds)
			{
				global::Debug.Log("GameSound: " + soundEvent.sound, null);
			}
			float num = 0f;
			if (this.lastTimePlayed.TryGetValue(soundEvent.soundHash, out num))
			{
				if (Time.time - num > soundEvent.minInterval)
				{
					global::SoundEvent.PlayOneShot(soundEvent.sound, this.transform.position);
				}
			}
			else
			{
				global::SoundEvent.PlayOneShot(soundEvent.sound, this.transform.position);
			}
			this.lastTimePlayed[soundEvent.soundHash] = Time.time;
		}
	}

	private List<LoopingSounds.SoundEvent> loopingSounds = new List<LoopingSounds.SoundEvent>();

	private Dictionary<HashedString, float> lastTimePlayed = new Dictionary<HashedString, float>();

	[SerializeField]
	public bool updatePosition;

	private struct SoundEvent
	{
		public void SetupProgressParameter()
		{
			EventInstance eventInstance = this.ev;
			EventDescription eventDescription;
			eventInstance.getDescription(out eventDescription);
			string text = null;
			USER_PROPERTY user_PROPERTY;
			if (eventDescription.getUserProperty("progressParameter", out user_PROPERTY) == RESULT.OK)
			{
				text = user_PROPERTY.stringValue;
			}
			ParameterInstance parameterInstance = null;
			eventInstance.getParameter(text, out parameterInstance);
			this.progressParameter = parameterInstance;
			this.progressParameterName = text;
		}

		public string asset;

		public EventInstance ev;

		public ParameterInstance progressParameter;

		public string progressParameterName;
	}
}
