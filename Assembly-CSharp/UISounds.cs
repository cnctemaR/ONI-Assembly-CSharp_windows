using System;
using UnityEngine;

public class UISounds : KMonoBehaviour
{
	public static UISounds Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		UISounds.Instance = this;
	}

	public static void PlaySound(UISounds.Sound sound)
	{
		UISounds.Instance.PlaySoundInternal(sound);
	}

	private void PlaySoundInternal(UISounds.Sound sound)
	{
		for (int i = 0; i < this.soundData.Length; i++)
		{
			if (this.soundData[i].sound == sound)
			{
				if (this.logSounds)
				{
					Output.Log(new object[]
					{
						"Play sound",
						this.soundData[i].name
					});
				}
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound(this.soundData[i].name, false));
			}
		}
	}

	[SerializeField]
	private bool logSounds;

	[SerializeField]
	private UISounds.SoundData[] soundData;

	public enum Sound
	{
		NegativeNotification,
		PositiveNotification,
		Select,
		Negative,
		Back,
		ClickObject,
		HUD_Mouseover,
		Object_Mouseover,
		ClickHUD
	}

	[Serializable]
	private struct SoundData
	{
		public string name;

		public UISounds.Sound sound;
	}
}
