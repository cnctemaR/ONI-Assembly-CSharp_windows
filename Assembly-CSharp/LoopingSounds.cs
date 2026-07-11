using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class LoopingSounds : KMonoBehaviour
{
	public bool IsSoundPlaying(string path)
	{
		foreach (LoopingSounds.LoopingSoundEvent loopingSoundEvent in this.loopingSounds)
		{
			if (loopingSoundEvent.asset == path)
			{
				return true;
			}
		}
		return false;
	}

	public bool StartSound(string asset, AnimEventManager.EventPlayerData behaviour, EffectorValues noiseValues, bool ignore_pause = false)
	{
		if (asset == null || asset == string.Empty)
		{
			global::Debug.LogWarning("Missing sound", null);
			return false;
		}
		if (!this.IsSoundPlaying(asset))
		{
			LoopingSounds.LoopingSoundEvent loopingSoundEvent = new LoopingSounds.LoopingSoundEvent
			{
				asset = asset
			};
			Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
			loopingSoundEvent.handle = LoopingSoundManager.Get().Add(asset, position, base.transform, !ignore_pause, true);
			this.loopingSounds.Add(loopingSoundEvent);
		}
		return true;
	}

	public bool StartSound(string asset)
	{
		if (asset == null || asset == string.Empty)
		{
			global::Debug.LogWarning("Missing sound", null);
			return false;
		}
		if (!this.IsSoundPlaying(asset))
		{
			LoopingSounds.LoopingSoundEvent loopingSoundEvent = new LoopingSounds.LoopingSoundEvent
			{
				asset = asset
			};
			loopingSoundEvent.handle = LoopingSoundManager.Get().Add(asset, base.transform.GetPosition(), base.transform, true, true);
			this.loopingSounds.Add(loopingSoundEvent);
		}
		return true;
	}

	public void UpdateVelocity(string asset, Vector2 value)
	{
		foreach (LoopingSounds.LoopingSoundEvent loopingSoundEvent in this.loopingSounds)
		{
			if (loopingSoundEvent.asset == asset)
			{
				LoopingSoundManager.Get().UpdateVelocity(loopingSoundEvent.handle, value);
				break;
			}
		}
	}

	public void UpdateFirstParameter(string asset, HashedString parameter, float value)
	{
		foreach (LoopingSounds.LoopingSoundEvent loopingSoundEvent in this.loopingSounds)
		{
			if (loopingSoundEvent.asset == asset)
			{
				LoopingSoundManager.Get().UpdateFirstParameter(loopingSoundEvent.handle, parameter, value);
				break;
			}
		}
	}

	public void UpdateSecondParameter(string asset, HashedString parameter, float value)
	{
		foreach (LoopingSounds.LoopingSoundEvent loopingSoundEvent in this.loopingSounds)
		{
			if (loopingSoundEvent.asset == asset)
			{
				LoopingSoundManager.Get().UpdateSecondParameter(loopingSoundEvent.handle, parameter, value);
				break;
			}
		}
	}

	private void StopSoundAtIndex(int i)
	{
		LoopingSoundManager.StopSound(this.loopingSounds[i].handle);
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

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.StopAllSounds();
	}

	public void SetParameter(string path, HashedString parameter, float value)
	{
		foreach (LoopingSounds.LoopingSoundEvent loopingSoundEvent in this.loopingSounds)
		{
			if (loopingSoundEvent.asset == path)
			{
				LoopingSoundManager.Get().UpdateFirstParameter(loopingSoundEvent.handle, parameter, value);
				break;
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
		Vector2 vector = base.transform.GetPosition();
		for (int i = 0; i < events.Count; i++)
		{
			AnimEvent animEvent = events[i];
			SoundEvent soundEvent = animEvent as SoundEvent;
			if (soundEvent == null || soundEvent.sound == null)
			{
				return;
			}
			if (CameraController.Instance.IsAudibleSound(vector, soundEvent.sound))
			{
				if (AudioDebug.Get().debugGameEventSounds)
				{
					global::Debug.Log("GameSound: " + soundEvent.sound, null);
				}
				float num = 0f;
				if (this.lastTimePlayed.TryGetValue(soundEvent.soundHash, out num))
				{
					if (Time.time - num > soundEvent.minInterval)
					{
						SoundEvent.PlayOneShot(soundEvent.sound, vector);
					}
				}
				else
				{
					SoundEvent.PlayOneShot(soundEvent.sound, vector);
				}
				this.lastTimePlayed[soundEvent.soundHash] = Time.time;
			}
		}
	}

	private List<LoopingSounds.LoopingSoundEvent> loopingSounds = new List<LoopingSounds.LoopingSoundEvent>();

	private Dictionary<HashedString, float> lastTimePlayed = new Dictionary<HashedString, float>();

	[SerializeField]
	public bool updatePosition;

	private struct LoopingSoundEvent
	{
		public string asset;

		public HandleVector<int>.Handle handle;
	}
}
