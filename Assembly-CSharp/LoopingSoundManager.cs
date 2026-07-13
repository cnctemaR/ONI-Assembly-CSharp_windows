using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LoopingSoundManager")]
public class LoopingSoundManager : KMonoBehaviour, IRenderEveryTick
{
	public static void DestroyInstance()
	{
		LoopingSoundManager.instance = null;
	}

	protected override void OnPrefabInit()
	{
		LoopingSoundManager.instance = this;
		this.CollectParameterUpdaters();
	}

	protected override void OnSpawn()
	{
		if (SpeedControlScreen.Instance != null && Game.Instance != null)
		{
			Game.Instance.Subscribe(-1788536802, new Action<object>(LoopingSoundManager.instance.OnPauseChanged));
		}
		Game.Instance.Subscribe(1983128072, delegate(object worlds)
		{
			this.OnActiveWorldChanged();
		});
	}

	private void OnActiveWorldChanged()
	{
		this.StopAllSounds();
	}

	private void CollectParameterUpdaters()
	{
		foreach (Type type in App.GetCurrentDomainTypes())
		{
			if (!type.IsAbstract)
			{
				bool flag = false;
				Type type2 = type.BaseType;
				while (type2 != null)
				{
					if (type2 == typeof(LoopingSoundParameterUpdater))
					{
						flag = true;
						break;
					}
					type2 = type2.BaseType;
				}
				if (flag)
				{
					LoopingSoundParameterUpdater loopingSoundParameterUpdater = (LoopingSoundParameterUpdater)Activator.CreateInstance(type);
					DebugUtil.Assert(!this.parameterUpdaters.ContainsKey(loopingSoundParameterUpdater.parameter));
					this.parameterUpdaters[loopingSoundParameterUpdater.parameter] = loopingSoundParameterUpdater;
				}
			}
		}
	}

	public void UpdateFirstParameter(HandleVector<int>.Handle handle, HashedString parameter, float value)
	{
		LoopingSoundManager.Sound data = this.sounds.GetData(handle);
		data.firstParameterValue = value;
		data.firstParameter = parameter;
		if (data.IsPlaying)
		{
			data.ev.setParameterByID(this.GetSoundDescription(data.path).GetParameterId(parameter), value, false);
		}
		this.sounds.SetData(handle, data);
	}

	public void UpdateSecondParameter(HandleVector<int>.Handle handle, HashedString parameter, float value)
	{
		LoopingSoundManager.Sound data = this.sounds.GetData(handle);
		data.secondParameterValue = value;
		data.secondParameter = parameter;
		if (data.IsPlaying)
		{
			data.ev.setParameterByID(this.GetSoundDescription(data.path).GetParameterId(parameter), value, false);
		}
		this.sounds.SetData(handle, data);
	}

	public void UpdateObjectSelection(HandleVector<int>.Handle handle, Vector3 sound_pos, float vol, bool objectIsSelectedAndVisible)
	{
		LoopingSoundManager.Sound data = this.sounds.GetData(handle);
		data.pos = sound_pos;
		data.vol = vol;
		data.objectIsSelectedAndVisible = objectIsSelectedAndVisible;
		ATTRIBUTES_3D attributes_3D = sound_pos.To3DAttributes();
		if (data.IsPlaying)
		{
			data.ev.set3DAttributes(attributes_3D);
			data.ev.setVolume(vol);
		}
		this.sounds.SetData(handle, data);
	}

	public void UpdateVelocity(HandleVector<int>.Handle handle, Vector2 velocity)
	{
		LoopingSoundManager.Sound data = this.sounds.GetData(handle);
		data.velocity = velocity;
		this.sounds.SetData(handle, data);
	}

	public void RenderEveryTick(float dt)
	{
		ListPool<LoopingSoundManager.Sound, LoopingSoundManager>.PooledList pooledList = ListPool<LoopingSoundManager.Sound, LoopingSoundManager>.Allocate();
		ListPool<int, LoopingSoundManager>.PooledList pooledList2 = ListPool<int, LoopingSoundManager>.Allocate();
		ListPool<int, LoopingSoundManager>.PooledList pooledList3 = ListPool<int, LoopingSoundManager>.Allocate();
		List<LoopingSoundManager.Sound> dataList = this.sounds.GetDataList();
		bool flag = Time.timeScale == 0f;
		SoundCuller soundCuller = CameraController.Instance.soundCuller;
		for (int i = 0; i < dataList.Count; i++)
		{
			LoopingSoundManager.Sound sound = dataList[i];
			if (sound.objectIsSelectedAndVisible)
			{
				sound.pos = SoundEvent.AudioHighlightListenerPosition(sound.transform.GetPosition());
				sound.vol = 1f;
			}
			else if (sound.transform != null)
			{
				sound.pos = sound.transform.GetPosition();
				sound.pos.z = 0f;
			}
			if (sound.animController != null)
			{
				Vector3 offset = sound.animController.Offset;
				sound.pos.x = sound.pos.x + offset.x;
				sound.pos.y = sound.pos.y + offset.y;
			}
			bool flag2 = !sound.IsCullingEnabled || (sound.ShouldCameraScalePosition && soundCuller.IsAudible(sound.pos, sound.falloffDistanceSq)) || soundCuller.IsAudibleNoCameraScaling(sound.pos, sound.falloffDistanceSq);
			bool isPlaying = sound.IsPlaying;
			if (flag2)
			{
				pooledList.Add(sound);
				if (!isPlaying)
				{
					SoundDescription soundDescription = this.GetSoundDescription(sound.path);
					sound.ev = KFMOD.CreateInstance(soundDescription.path);
					dataList[i] = sound;
					pooledList2.Add(i);
				}
			}
			else if (isPlaying)
			{
				pooledList3.Add(i);
			}
		}
		foreach (int num in pooledList2)
		{
			LoopingSoundManager.Sound sound2 = dataList[num];
			SoundDescription soundDescription2 = this.GetSoundDescription(sound2.path);
			sound2.ev.setPaused(flag && sound2.ShouldPauseOnGamePaused);
			sound2.pos.z = 0f;
			Vector3 pos = sound2.pos;
			if (sound2.objectIsSelectedAndVisible)
			{
				sound2.pos = SoundEvent.AudioHighlightListenerPosition(sound2.transform.GetPosition());
				sound2.vol = 1f;
			}
			else if (sound2.transform != null)
			{
				sound2.pos = sound2.transform.GetPosition();
			}
			sound2.ev.set3DAttributes(pos.To3DAttributes());
			sound2.ev.setVolume(sound2.vol);
			sound2.ev.start();
			sound2.flags |= LoopingSoundManager.Sound.Flags.PLAYING;
			if (sound2.firstParameter != HashedString.Invalid)
			{
				sound2.ev.setParameterByID(soundDescription2.GetParameterId(sound2.firstParameter), sound2.firstParameterValue, false);
			}
			if (sound2.secondParameter != HashedString.Invalid)
			{
				sound2.ev.setParameterByID(soundDescription2.GetParameterId(sound2.secondParameter), sound2.secondParameterValue, false);
			}
			LoopingSoundParameterUpdater.Sound sound3 = new LoopingSoundParameterUpdater.Sound
			{
				ev = sound2.ev,
				path = sound2.path,
				description = soundDescription2,
				transform = sound2.transform,
				objectIsSelectedAndVisible = false
			};
			foreach (SoundDescription.Parameter parameter in soundDescription2.parameters)
			{
				LoopingSoundParameterUpdater loopingSoundParameterUpdater = null;
				if (this.parameterUpdaters.TryGetValue(parameter.name, out loopingSoundParameterUpdater))
				{
					loopingSoundParameterUpdater.Add(sound3);
				}
			}
			dataList[num] = sound2;
		}
		pooledList2.Recycle();
		foreach (int num2 in pooledList3)
		{
			LoopingSoundManager.Sound sound4 = dataList[num2];
			SoundDescription soundDescription3 = this.GetSoundDescription(sound4.path);
			LoopingSoundParameterUpdater.Sound sound5 = new LoopingSoundParameterUpdater.Sound
			{
				ev = sound4.ev,
				path = sound4.path,
				description = soundDescription3,
				transform = sound4.transform,
				objectIsSelectedAndVisible = false
			};
			foreach (SoundDescription.Parameter parameter2 in soundDescription3.parameters)
			{
				LoopingSoundParameterUpdater loopingSoundParameterUpdater2 = null;
				if (this.parameterUpdaters.TryGetValue(parameter2.name, out loopingSoundParameterUpdater2))
				{
					loopingSoundParameterUpdater2.Remove(sound5);
				}
			}
			if (sound4.ShouldCameraScalePosition)
			{
				sound4.ev.stop(global::FMOD.Studio.STOP_MODE.IMMEDIATE);
			}
			else
			{
				sound4.ev.stop(global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
			sound4.flags &= ~LoopingSoundManager.Sound.Flags.PLAYING;
			sound4.ev.release();
			dataList[num2] = sound4;
		}
		pooledList3.Recycle();
		float velocityScale = TuningData<LoopingSoundManager.Tuning>.Get().velocityScale;
		foreach (LoopingSoundManager.Sound sound6 in pooledList)
		{
			ATTRIBUTES_3D attributes_3D = SoundEvent.GetCameraScaledPosition(sound6.pos, sound6.objectIsSelectedAndVisible).To3DAttributes();
			attributes_3D.velocity = (sound6.velocity * velocityScale).ToFMODVector();
			EventInstance ev = sound6.ev;
			ev.set3DAttributes(attributes_3D);
		}
		foreach (KeyValuePair<HashedString, LoopingSoundParameterUpdater> keyValuePair in this.parameterUpdaters)
		{
			keyValuePair.Value.Update(dt);
		}
		pooledList.Recycle();
	}

	public static LoopingSoundManager Get()
	{
		return LoopingSoundManager.instance;
	}

	public void StopAllSounds()
	{
		foreach (LoopingSoundManager.Sound sound in this.sounds.GetDataList())
		{
			if (sound.IsPlaying)
			{
				EventInstance eventInstance = sound.ev;
				eventInstance.stop(global::FMOD.Studio.STOP_MODE.IMMEDIATE);
				eventInstance = sound.ev;
				eventInstance.release();
			}
		}
	}

	private SoundDescription GetSoundDescription(HashedString path)
	{
		return KFMOD.GetSoundEventDescription(path);
	}

	public HandleVector<int>.Handle Add(string path, Vector3 pos, Transform transform = null, bool pause_on_game_pause = true, bool enable_culling = true, bool enable_camera_scaled_position = true, float vol = 1f, bool objectIsSelectedAndVisible = false)
	{
		SoundDescription soundEventDescription = KFMOD.GetSoundEventDescription(path);
		LoopingSoundManager.Sound.Flags flags = (LoopingSoundManager.Sound.Flags)0;
		if (pause_on_game_pause)
		{
			flags |= LoopingSoundManager.Sound.Flags.PAUSE_ON_GAME_PAUSED;
		}
		if (enable_culling)
		{
			flags |= LoopingSoundManager.Sound.Flags.ENABLE_CULLING;
		}
		if (enable_camera_scaled_position)
		{
			flags |= LoopingSoundManager.Sound.Flags.ENABLE_CAMERA_SCALED_POSITION;
		}
		KBatchedAnimController kbatchedAnimController = null;
		if (transform != null)
		{
			kbatchedAnimController = transform.GetComponent<KBatchedAnimController>();
		}
		LoopingSoundManager.Sound sound = new LoopingSoundManager.Sound
		{
			transform = transform,
			animController = kbatchedAnimController,
			falloffDistanceSq = soundEventDescription.falloffDistanceSq,
			path = path,
			pos = pos,
			flags = flags,
			firstParameter = HashedString.Invalid,
			secondParameter = HashedString.Invalid,
			vol = vol,
			objectIsSelectedAndVisible = objectIsSelectedAndVisible
		};
		return this.sounds.Allocate(sound);
	}

	public static HandleVector<int>.Handle StartSound(EventReference event_ref, Vector3 pos, bool pause_on_game_pause = true, bool enable_culling = true)
	{
		return LoopingSoundManager.StartSound(KFMOD.GetEventReferencePath(event_ref), pos, pause_on_game_pause, enable_culling);
	}

	public static HandleVector<int>.Handle StartSound(string path, Vector3 pos, bool pause_on_game_pause = true, bool enable_culling = true)
	{
		if (string.IsNullOrEmpty(path))
		{
			global::Debug.LogWarning("Missing sound");
			return HandleVector<int>.InvalidHandle;
		}
		return LoopingSoundManager.Get().Add(path, pos, null, pause_on_game_pause, enable_culling, true, 1f, false);
	}

	public static void StopSound(HandleVector<int>.Handle handle)
	{
		if (LoopingSoundManager.Get() == null)
		{
			return;
		}
		LoopingSoundManager.Sound data = LoopingSoundManager.Get().sounds.GetData(handle);
		if (data.IsPlaying)
		{
			data.ev.stop(LoopingSoundManager.Get().GameIsPaused ? global::FMOD.Studio.STOP_MODE.IMMEDIATE : global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			data.ev.release();
			SoundDescription soundEventDescription = KFMOD.GetSoundEventDescription(data.path);
			foreach (SoundDescription.Parameter parameter in soundEventDescription.parameters)
			{
				LoopingSoundParameterUpdater loopingSoundParameterUpdater = null;
				if (LoopingSoundManager.Get().parameterUpdaters.TryGetValue(parameter.name, out loopingSoundParameterUpdater))
				{
					LoopingSoundParameterUpdater.Sound sound = new LoopingSoundParameterUpdater.Sound
					{
						ev = data.ev,
						path = data.path,
						description = soundEventDescription,
						transform = data.transform,
						objectIsSelectedAndVisible = false
					};
					loopingSoundParameterUpdater.Remove(sound);
				}
			}
		}
		LoopingSoundManager.Get().sounds.Free(handle);
	}

	public static void PauseSound(HandleVector<int>.Handle handle, bool paused)
	{
		LoopingSoundManager.Sound data = LoopingSoundManager.Get().sounds.GetData(handle);
		if (data.IsPlaying)
		{
			data.ev.setPaused(paused);
		}
	}

	private void OnPauseChanged(object data)
	{
		bool value = ((Boxed<bool>)data).value;
		this.GameIsPaused = value;
		foreach (LoopingSoundManager.Sound sound in this.sounds.GetDataList())
		{
			if (sound.IsPlaying)
			{
				EventInstance ev = sound.ev;
				ev.setPaused(value && sound.ShouldPauseOnGamePaused);
			}
		}
	}

	private static LoopingSoundManager instance;

	private bool GameIsPaused;

	private Dictionary<HashedString, LoopingSoundParameterUpdater> parameterUpdaters = new Dictionary<HashedString, LoopingSoundParameterUpdater>();

	private KCompactedVector<LoopingSoundManager.Sound> sounds = new KCompactedVector<LoopingSoundManager.Sound>(0);

	public class Tuning : TuningData<LoopingSoundManager.Tuning>
	{
		public float velocityScale;
	}

	public struct Sound
	{
		public bool IsPlaying
		{
			get
			{
				return (this.flags & LoopingSoundManager.Sound.Flags.PLAYING) > (LoopingSoundManager.Sound.Flags)0;
			}
		}

		public bool ShouldPauseOnGamePaused
		{
			get
			{
				return (this.flags & LoopingSoundManager.Sound.Flags.PAUSE_ON_GAME_PAUSED) > (LoopingSoundManager.Sound.Flags)0;
			}
		}

		public bool IsCullingEnabled
		{
			get
			{
				return (this.flags & LoopingSoundManager.Sound.Flags.ENABLE_CULLING) > (LoopingSoundManager.Sound.Flags)0;
			}
		}

		public bool ShouldCameraScalePosition
		{
			get
			{
				return (this.flags & LoopingSoundManager.Sound.Flags.ENABLE_CAMERA_SCALED_POSITION) > (LoopingSoundManager.Sound.Flags)0;
			}
		}

		public EventInstance ev;

		public Transform transform;

		public KBatchedAnimController animController;

		public float falloffDistanceSq;

		public HashedString path;

		public Vector3 pos;

		public Vector2 velocity;

		public HashedString firstParameter;

		public HashedString secondParameter;

		public float firstParameterValue;

		public float secondParameterValue;

		public float vol;

		public bool objectIsSelectedAndVisible;

		public LoopingSoundManager.Sound.Flags flags;

		[Flags]
		public enum Flags
		{
			PLAYING = 1,
			PAUSE_ON_GAME_PAUSED = 2,
			ENABLE_CULLING = 4,
			ENABLE_CAMERA_SCALED_POSITION = 8
		}
	}
}
