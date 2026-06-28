using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[SkipSaveFileSerialization]
public class LoopingSounds : KMonoBehaviour, IRenderEveryTick
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.updatePosition)
		{
			this.AddLoopingSoundUpdater();
		}
		else
		{
			this.RemoveLoopingSoundUpdater();
		}
	}

	public void AddLoopingSoundUpdater()
	{
		SimAndRenderScheduler.instance.Add(this, false);
	}

	public void RemoveLoopingSoundUpdater()
	{
		SimAndRenderScheduler.instance.Remove(this);
	}

	public void RenderEveryTick(float dt)
	{
		Vector3 position = base.transform.GetPosition();
		for (int i = 0; i < this.loopingSounds.Count; i++)
		{
			EventInstance ev = this.loopingSounds[i].ev;
			Vector3 vector = new Vector3(position.x, position.y, 0f);
			ev.set3DAttributes(SoundEvent.GetCameraScaledPosition(vector).To3DAttributes());
			this.UpdateProgressParameter(this.loopingSounds[i]);
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

	private void UpdateProgressParameter(LoopingSounds.LoopingSoundEvent sound)
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

	public bool StartSound(string asset, AnimEventManager.EventPlayerData behaviour, bool playAtTarget, EffectorValues noiseValues)
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
			LoopingSounds.LoopingSoundEvent loopingSoundEvent = new LoopingSounds.LoopingSoundEvent
			{
				asset = asset,
				ev = eventInstance,
				progressParameter = null,
				progressParameterName = null,
				splat = null
			};
			loopingSoundEvent.SetupProgressParameter();
			if (loopingSoundEvent.progressParameter != null)
			{
				this.AddLoopingSoundUpdater();
			}
			LoopingSoundManager.Get().Add(asset, eventInstance, true);
			Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
			Vector3 position2 = behaviour.position;
			Vector3 vector = ((!playAtTarget) ? position : position2);
			Vector3 vector2 = new Vector3(vector.x, vector.y, 0f);
			eventInstance.set3DAttributes(SoundEvent.GetCameraScaledPosition(vector2).To3DAttributes());
			LoopingSoundManager.UpdateSpeed(eventInstance);
			eventInstance.start();
			if (Time.timeScale == 0f)
			{
				eventInstance.setPaused(true);
			}
			this.loopingSounds.Add(loopingSoundEvent);
		}
		return true;
	}

	public bool StartSound(string asset, Vector3 sound_pos)
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
			LoopingSounds.LoopingSoundEvent loopingSoundEvent = new LoopingSounds.LoopingSoundEvent
			{
				asset = asset,
				ev = eventInstance,
				progressParameter = null,
				progressParameterName = null,
				splat = null
			};
			loopingSoundEvent.SetupProgressParameter();
			if (!this.updatePosition && loopingSoundEvent.progressParameter != null)
			{
				this.updatePosition = true;
			}
			this.loopingSounds.Add(loopingSoundEvent);
			LoopingSoundManager.Get().Add(asset, eventInstance, true);
			Vector3 vector = new Vector3(sound_pos.x, sound_pos.y, 0f);
			eventInstance.set3DAttributes(SoundEvent.GetCameraScaledPosition(vector).To3DAttributes());
			LoopingSoundManager.UpdateSpeed(eventInstance);
			eventInstance.start();
			if (Time.timeScale == 0f)
			{
				eventInstance.setPaused(true);
			}
		}
		return true;
	}

	private void StopSoundAtIndex(int i)
	{
		EventInstance ev = this.loopingSounds[i].ev;
		ev.stop(STOP_MODE.ALLOWFADEOUT);
		ev.release();
		LoopingSoundManager.Get().Remove(this.loopingSounds[i].asset, ev);
	}

	public void StopSound(string asset)
	{
		for (int i = 0; i < this.loopingSounds.Count; i++)
		{
			if (this.loopingSounds[i].asset == asset)
			{
				this.StopSoundAtIndex(i);
				this.loopingSounds.RemoveAt(i);
				break;
			}
		}
	}

	public void StopAllSounds()
	{
		for (int i = 0; i < this.loopingSounds.Count; i++)
		{
			this.StopSoundAtIndex(i);
		}
		this.loopingSounds.Clear();
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

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.RemoveLoopingSoundUpdater();
		this.StopAllSounds();
	}

	public void SetParameter(string path, string parameter, float value)
	{
		foreach (LoopingSounds.LoopingSoundEvent loopingSoundEvent in this.loopingSounds)
		{
			if (loopingSoundEvent.asset == path)
			{
				loopingSoundEvent.ev.setParameterValue(parameter, value);
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
			SoundEvent soundEvent = animEvent as SoundEvent;
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
					SoundEvent.PlayOneShot(soundEvent.sound, base.transform.GetPosition());
				}
			}
			else
			{
				SoundEvent.PlayOneShot(soundEvent.sound, base.transform.GetPosition());
			}
			this.lastTimePlayed[soundEvent.soundHash] = Time.time;
		}
	}

	private List<LoopingSounds.LoopingSoundEvent> loopingSounds = new List<LoopingSounds.LoopingSoundEvent>();

	private Dictionary<HashedString, float> lastTimePlayed = new Dictionary<HashedString, float>();

	[SerializeField]
	public bool updatePosition;

	private struct LoopingSoundEvent
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

		public NoiseSplat splat;

		public ParameterInstance progressParameter;

		public string progressParameterName;
	}
}
