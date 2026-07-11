using System;
using FMOD.Studio;
using UnityEngine;

public class CountedSoundEvent : SoundEvent
{
	public CountedSoundEvent(string file_name, string sound_name, int frame, bool do_load, bool is_looping, float min_interval, bool is_dynamic)
		: base(file_name, CountedSoundEvent.BaseSoundName(sound_name), frame, do_load, is_looping, min_interval, is_dynamic)
	{
		if (sound_name.Contains(":"))
		{
			string[] array = sound_name.Split(new char[] { ':' });
			if (array.Length != 2)
			{
				DebugUtil.LogErrorArgs(new object[]
				{
					"Invalid CountedSoundEvent parameter for",
					string.Concat(new string[]
					{
						file_name,
						".",
						sound_name,
						".",
						frame.ToString(),
						":"
					}),
					"'" + sound_name + "'"
				});
			}
			for (int i = 1; i < array.Length; i++)
			{
				this.ParseParameter(array[i]);
			}
		}
		else
		{
			DebugUtil.LogErrorArgs(new object[]
			{
				"CountedSoundEvent for",
				string.Concat(new string[]
				{
					file_name,
					".",
					sound_name,
					".",
					frame.ToString()
				}),
				" - Must specify max number of steps on event: '" + sound_name + "'"
			});
		}
	}

	private static string BaseSoundName(string sound_name)
	{
		int num = sound_name.IndexOf(":");
		if (num > 0)
		{
			return sound_name.Substring(0, num);
		}
		return sound_name;
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		if (string.IsNullOrEmpty(base.sound))
		{
			return;
		}
		if (!SoundEvent.ShouldPlaySound(behaviour.controller, base.sound, base.looping, this.isDynamic))
		{
			return;
		}
		int num = -1;
		GameObject gameObject = behaviour.controller.gameObject;
		if (this.counterModulus >= -1)
		{
			HandleVector<int>.Handle handle = GameComps.WhiteBoards.GetHandle(gameObject);
			if (!handle.IsValid())
			{
				handle = GameComps.WhiteBoards.Add(gameObject);
			}
			num = ((!GameComps.WhiteBoards.HasValue(handle, base.soundHash)) ? 0 : ((int)GameComps.WhiteBoards.GetValue(handle, base.soundHash)));
			int num2 = ((this.counterModulus != -1) ? ((num + 1) % this.counterModulus) : 0);
			GameComps.WhiteBoards.SetValue(handle, base.soundHash, num2);
		}
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		EventInstance eventInstance = SoundEvent.BeginOneShot(base.sound, position);
		if (eventInstance.isValid())
		{
			if (num >= 0)
			{
				eventInstance.setParameterValue("eventCount", (float)num);
			}
			SoundEvent.EndOneShot(eventInstance);
		}
	}

	private void ParseParameter(string param)
	{
		this.counterModulus = int.Parse(param);
		if (this.counterModulus != -1 && this.counterModulus < 2)
		{
			throw new ArgumentException("CountedSoundEvent modulus must be 2 or larger");
		}
	}

	private const int COUNTER_MODULUS_INVALID = -2147483648;

	private const int COUNTER_MODULUS_CLEAR = -1;

	private int counterModulus = int.MinValue;
}
