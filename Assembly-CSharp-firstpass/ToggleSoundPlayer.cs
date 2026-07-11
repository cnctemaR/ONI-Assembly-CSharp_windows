using System;

[Serializable]
public class ToggleSoundPlayer : WidgetSoundPlayer
{
	public override string GetDefaultPath(int idx)
	{
		return ToggleSoundPlayer.default_values[idx];
	}

	public override WidgetSoundPlayer.WidgetSoundEvent[] widget_sound_events()
	{
		return this.toggle_widget_sound_events;
	}

	public static readonly string[] default_values = new string[] { "HUD_Click", "HUD_Click_Deselect", "HUD_Mouseover", "Negative" };

	public Func<bool> AcceptClickCondition;

	public WidgetSoundPlayer.WidgetSoundEvent[] toggle_widget_sound_events = new WidgetSoundPlayer.WidgetSoundEvent[]
	{
		new WidgetSoundPlayer.WidgetSoundEvent(0, "On Use On", "", true),
		new WidgetSoundPlayer.WidgetSoundEvent(1, "On Use Off", "", true),
		new WidgetSoundPlayer.WidgetSoundEvent(2, "On Pointer Enter", "", true),
		new WidgetSoundPlayer.WidgetSoundEvent(3, "On Use Rejected", "", true)
	};

	public enum SoundEvents
	{
		OnClick_On,
		OnClick_Off,
		OnPointerEnter,
		OnClick_Rejected
	}
}
