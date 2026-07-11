using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class LoopingSounds : KMonoBehaviour
{
	public bool IsSoundPlaying(string path)
	{
		using (List<LoopingSounds.LoopingSoundEvent>.Enumerator enumerator = this.loopingSounds.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.asset == path)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool StartSound(string asset, AnimEventManager.EventPlayerData behaviour, EffectorValues noiseValues, bool ignore_pause = false, bool enable_camera_scaled_position = true)
	{
		if (asset == null || asset == "")
		{
			global::Debug.LogWarning("Missing sound");
			return false;
		}
		if (!this.IsSoundPlaying(asset))
		{
			LoopingSounds.LoopingSoundEvent loopingSoundEvent = new LoopingSounds.LoopingSoundEvent
			{
				asset = asset
			};
			GameObject gameObject = base.gameObject;
			this.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
			if (this.objectIsSelectedAndVisible)
			{
				this.sound_pos = SoundEvent.AudioHighlightListenerPosition(base.transform.GetPosition());
				this.vol = SoundEvent.GetVolume(this.objectIsSelectedAndVisible);
			}
			else
			{
				this.sound_pos = behaviour.GetComponent<Transform>().GetPosition();
				this.sound_pos.z = 0f;
			}
			loopingSoundEvent.handle = LoopingSoundManager.Get().Add(asset, this.sound_pos, base.transform, !ignore_pause, true, enable_camera_scaled_position, this.vol, this.objectIsSelectedAndVisible);
			this.loopingSounds.Add(loopingSoundEvent);
		}
		return true;
	}

	public bool StartSound(string asset)
	{
		if (asset == null || asset == "")
		{
			global::Debug.LogWarning("Missing sound");
			return false;
		}
		if (!this.IsSoundPlaying(asset))
		{
			LoopingSounds.LoopingSoundEvent loopingSoundEvent = new LoopingSounds.LoopingSoundEvent
			{
				asset = asset
			};
			GameObject gameObject = base.gameObject;
			this.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
			if (this.objectIsSelectedAndVisible)
			{
				this.sound_pos = SoundEvent.AudioHighlightListenerPosition(base.transform.GetPosition());
				this.vol = SoundEvent.GetVolume(this.objectIsSelectedAndVisible);
			}
			else
			{
				this.sound_pos = base.transform.GetPosition();
				this.sound_pos.z = 0f;
			}
			loopingSoundEvent.handle = LoopingSoundManager.Get().Add(asset, this.sound_pos, base.transform, true, true, true, this.vol, this.objectIsSelectedAndVisible);
			this.loopingSounds.Add(loopingSoundEvent);
		}
		return true;
	}

	public bool StartSound(string asset, bool pause_on_game_pause = true, bool enable_culling = true, bool enable_camera_scaled_position = true)
	{
		if (asset == null || asset == "")
		{
			global::Debug.LogWarning("Missing sound");
			return false;
		}
		if (!this.IsSoundPlaying(asset))
		{
			LoopingSounds.LoopingSoundEvent loopingSoundEvent = new LoopingSounds.LoopingSoundEvent
			{
				asset = asset
			};
			GameObject gameObject = base.gameObject;
			this.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
			if (this.objectIsSelectedAndVisible)
			{
				this.sound_pos = SoundEvent.AudioHighlightListenerPosition(base.transform.GetPosition());
				this.vol = SoundEvent.GetVolume(this.objectIsSelectedAndVisible);
			}
			else
			{
				this.sound_pos = base.transform.GetPosition();
				this.sound_pos.z = 0f;
			}
			loopingSoundEvent.handle = LoopingSoundManager.Get().Add(asset, this.sound_pos, base.transform, pause_on_game_pause, enable_culling, enable_camera_scaled_position, this.vol, this.objectIsSelectedAndVisible);
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
				return;
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
			global::Debug.Log("GameSoundEvent: " + ev.Name);
		}
		List<AnimEvent> events = GameAudioSheets.Get().GetEvents(ev.Name);
		if (events == null)
		{
			return;
		}
		Vector2 vector = base.transform.GetPosition();
		for (int i = 0; i < events.Count; i++)
		{
			SoundEvent soundEvent = events[i] as SoundEvent;
			if (soundEvent == null || soundEvent.sound == null)
			{
				return;
			}
			if (CameraController.Instance.IsAudibleSound(vector, soundEvent.sound))
			{
				if (AudioDebug.Get().debugGameEventSounds)
				{
					global::Debug.Log("GameSound: " + soundEvent.sound);
				}
				float num = 0f;
				if (this.lastTimePlayed.TryGetValue(soundEvent.soundHash, out num))
				{
					if (Time.time - num > soundEvent.minInterval)
					{
						SoundEvent.PlayOneShot(soundEvent.sound, vector, 1f);
					}
				}
				else
				{
					SoundEvent.PlayOneShot(soundEvent.sound, vector, 1f);
				}
				this.lastTimePlayed[soundEvent.soundHash] = Time.time;
			}
		}
	}

	public void UpdateObjectSelection(bool selected)
	{
		GameObject gameObject = base.gameObject;
		if (selected && gameObject != null && CameraController.Instance.IsVisiblePos(gameObject.transform.position))
		{
			this.objectIsSelectedAndVisible = true;
			this.sound_pos = SoundEvent.AudioHighlightListenerPosition(this.sound_pos);
			this.vol = 1f;
		}
		else
		{
			this.objectIsSelectedAndVisible = false;
			this.sound_pos = base.transform.GetPosition();
			this.sound_pos.z = 0f;
			this.vol = 1f;
		}
		for (int i = 0; i < this.loopingSounds.Count; i++)
		{
			LoopingSoundManager.Get().UpdateObjectSelection(this.loopingSounds[i].handle, this.sound_pos, this.vol, this.objectIsSelectedAndVisible);
		}
	}

	private List<LoopingSounds.LoopingSoundEvent> loopingSounds = new List<LoopingSounds.LoopingSoundEvent>();

	private Dictionary<HashedString, float> lastTimePlayed = new Dictionary<HashedString, float>();

	[SerializeField]
	public bool updatePosition;

	public float vol = 1f;

	public bool objectIsSelectedAndVisible;

	public Vector3 sound_pos;

	private struct LoopingSoundEvent
	{
		public string asset;

		public HandleVector<int>.Handle handle;
	}
}
