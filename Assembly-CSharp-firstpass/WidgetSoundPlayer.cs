using System;

[Serializable]
public class WidgetSoundPlayer
{
	public virtual string GetDefaultPath(int idx)
	{
		return "";
	}

	public virtual WidgetSoundPlayer.WidgetSoundEvent[] widget_sound_events()
	{
		return null;
	}

	public void Play(int sound_event_idx)
	{
		if (!this.Enabled)
		{
			return;
		}
		WidgetSoundPlayer.WidgetSoundEvent widgetSoundEvent = default(WidgetSoundPlayer.WidgetSoundEvent);
		for (int i = 0; i < this.widget_sound_events().Length; i++)
		{
			if (sound_event_idx == this.widget_sound_events()[i].idx)
			{
				widgetSoundEvent = this.widget_sound_events()[sound_event_idx];
				break;
			}
		}
		if (!KInputManager.isFocused || !widgetSoundEvent.PlaySound || widgetSoundEvent.Name == null || widgetSoundEvent.Name.Length < 0 || widgetSoundEvent.Name == "")
		{
			return;
		}
		KFMOD.PlayUISound(WidgetSoundPlayer.getSoundPath((widgetSoundEvent.OverrideAssetName == "") ? this.GetDefaultPath(widgetSoundEvent.idx) : widgetSoundEvent.OverrideAssetName));
	}

	public bool Enabled = true;

	public static Func<string, string> getSoundPath;

	[Serializable]
	public struct WidgetSoundEvent
	{
		public WidgetSoundEvent(int idx, string Name, string OverrideAssetName, bool PlaySound)
		{
			this.idx = idx;
			this.Name = Name;
			this.OverrideAssetName = OverrideAssetName;
			this.PlaySound = PlaySound;
		}

		public string Name;

		public string OverrideAssetName;

		public int idx;

		public bool PlaySound;
	}
}
