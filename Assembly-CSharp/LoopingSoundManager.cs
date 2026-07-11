using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

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
	}

	private void CollectParameterUpdaters()
	{
		foreach (Type type in App.GetCurrentDomainTypes())
		{
			if (!type.IsAbstract)
			{
				bool flag = false;
				for (Type type2 = type.BaseType; type2 != null; type2 = type2.BaseType)
				{
					if (type2 == typeof(LoopingSoundParameterUpdater))
					{
						flag = true;
						break;
					}
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
			data.ev.setParameterValueByIndex(this.GetSoundDescription(data.path).GetParameterIdx(parameter), value);
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
			data.ev.setParameterValueByIndex(this.GetSoundDescription(data.path).GetParameterIdx(parameter), value);
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
			if (sound.transform != null)
			{
				sound.pos = sound.transform.GetPosition();
				if (sound.animController != null)
				{
					Vector3 offset = sound.animController.Offset;
					sound.pos.x = sound.pos.x + offset.x;
					sound.pos.y = sound.pos.y + offset.y;
				}
			}
			bool flag2 = !sound.IsCullingEnabled || (sound.ShouldCameraScalePosition && soundCuller.IsAudible(sound.pos, sound.falloffDistanceSq)) || soundCuller.IsAudibleNoCameraScaling(sound.pos, sound.falloffDistanceSq);
			bool isPlaying = sound.IsPlaying;
			if (flag2)
			{
				pooledList.Add(sound);
				if (!isPlaying)
				{
					sound.ev = KFMOD.CreateInstance(this.GetSoundDescription(sound.path).path);
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
			SoundDescription soundDescription = this.GetSoundDescription(sound2.path);
			sound2.ev.setPaused(flag && sound2.ShouldPauseOnGamePaused);
			Vector2 vector = sound2.pos;
			if (sound2.ShouldCameraScalePosition)
			{
				vector = SoundEvent.GetCameraScaledPosition(vector);
			}
			sound2.ev.set3DAttributes(vector.To3DAttributes());
			sound2.ev.start();
			sound2.flags |= LoopingSoundManager.Sound.Flags.PLAYING;
			if (sound2.firstParameter != HashedString.Invalid)
			{
				sound2.ev.setParameterValueByIndex(soundDescription.GetParameterIdx(sound2.firstParameter), sound2.firstParameterValue);
			}
			if (sound2.secondParameter != HashedString.Invalid)
			{
				sound2.ev.setParameterValueByIndex(soundDescription.GetParameterIdx(sound2.secondParameter), sound2.secondParameterValue);
			}
			LoopingSoundParameterUpdater.Sound sound3 = new LoopingSoundParameterUpdater.Sound
			{
				ev = sound2.ev,
				path = sound2.path,
				description = soundDescription,
				transform = sound2.transform
			};
			foreach (SoundDescription.Parameter parameter in soundDescription.parameters)
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
			SoundDescription soundDescription2 = this.GetSoundDescription(sound4.path);
			LoopingSoundParameterUpdater.Sound sound5 = new LoopingSoundParameterUpdater.Sound
			{
				ev = sound4.ev,
				path = sound4.path,
				description = soundDescription2,
				transform = sound4.transform
			};
			foreach (SoundDescription.Parameter parameter2 in soundDescription2.parameters)
			{
				LoopingSoundParameterUpdater loopingSoundParameterUpdater2 = null;
				if (this.parameterUpdaters.TryGetValue(parameter2.name, out loopingSoundParameterUpdater2))
				{
					loopingSoundParameterUpdater2.Remove(sound5);
				}
			}
			if (sound4.ShouldCameraScalePosition)
			{
				sound4.ev.stop(STOP_MODE.IMMEDIATE);
			}
			else
			{
				sound4.ev.stop(STOP_MODE.ALLOWFADEOUT);
			}
			sound4.flags &= ~LoopingSoundManager.Sound.Flags.PLAYING;
			sound4.ev.release();
			dataList[num2] = sound4;
		}
		pooledList3.Recycle();
		float velocityScale = TuningData<LoopingSoundManager.Tuning>.Get().velocityScale;
		foreach (LoopingSoundManager.Sound sound6 in pooledList)
		{
			ATTRIBUTES_3D attributes_3D = SoundEvent.GetCameraScaledPosition(sound6.pos).To3DAttributes();
			attributes_3D.velocity = (sound6.velocity * velocityScale).ToFMODVector();
			sound6.ev.set3DAttributes(attributes_3D);
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
				sound.ev.stop(STOP_MODE.IMMEDIATE);
				sound.ev.release();
			}
		}
	}

	private SoundDescription GetSoundDescription(HashedString path)
	{
		return KFMOD.GetSoundEventDescription(path);
	}

	public HandleVector<int>.Handle Add(string path, Vector2 pos, Transform transform = null, bool pause_on_game_pause = true, bool enable_culling = true, bool enable_camera_scaled_position = true)
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
			secondParameter = HashedString.Invalid
		};
		return this.sounds.Allocate(sound);
	}

	public static HandleVector<int>.Handle StartSound(string path, Vector3 pos, bool pause_on_game_pause = true, bool enable_culling = true)
	{
		if (string.IsNullOrEmpty(path))
		{
			global::Debug.LogWarning("Missing sound");
			return HandleVector<int>.InvalidHandle;
		}
		return LoopingSoundManager.Get().Add(path, pos, null, pause_on_game_pause, enable_culling, true);
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
			data.ev.stop(STOP_MODE.ALLOWFADEOUT);
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
						transform = data.transform
					};
					loopingSoundParameterUpdater.Remove(sound);
				}
			}
		}
		LoopingSoundManager.Get().sounds.Free(handle);
	}

	private void OnPauseChanged(object data)
	{
		bool flag = (bool)data;
		foreach (LoopingSoundManager.Sound sound in this.sounds.GetDataList())
		{
			if (sound.IsPlaying)
			{
				sound.ev.setPaused(flag && sound.ShouldPauseOnGamePaused);
			}
		}
	}

	private static LoopingSoundManager instance;

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
				return (this.flags & LoopingSoundManager.Sound.Flags.PLAYING) != (LoopingSoundManager.Sound.Flags)0;
			}
		}

		public bool ShouldPauseOnGamePaused
		{
			get
			{
				return (this.flags & LoopingSoundManager.Sound.Flags.PAUSE_ON_GAME_PAUSED) != (LoopingSoundManager.Sound.Flags)0;
			}
		}

		public bool IsCullingEnabled
		{
			get
			{
				return (this.flags & LoopingSoundManager.Sound.Flags.ENABLE_CULLING) != (LoopingSoundManager.Sound.Flags)0;
			}
		}

		public bool ShouldCameraScalePosition
		{
			get
			{
				return (this.flags & LoopingSoundManager.Sound.Flags.ENABLE_CAMERA_SCALED_POSITION) != (LoopingSoundManager.Sound.Flags)0;
			}
		}

		public EventInstance ev;

		public Transform transform;

		public KBatchedAnimController animController;

		public float falloffDistanceSq;

		public HashedString path;

		public Vector2 pos;

		public Vector2 velocity;

		public HashedString firstParameter;

		public HashedString secondParameter;

		public float firstParameterValue;

		public float secondParameterValue;

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
