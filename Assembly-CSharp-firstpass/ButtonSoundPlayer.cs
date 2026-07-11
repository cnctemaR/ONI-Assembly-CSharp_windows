using System;

[Serializable]
public class ButtonSoundPlayer : WidgetSoundPlayer
{
	public override string GetDefaultPath(int idx)
	{
		return ButtonSoundPlayer.default_values[idx];
	}

	public override WidgetSoundPlayer.WidgetSoundEvent[] widget_sound_events()
	{
		return this.button_widget_sound_events;
	}

	public static string[] default_values = new string[] { "HUD_Click_Open", "HUD_Mouseover", "Negative" };

	public Func<bool> AcceptClickCondition;

	public WidgetSoundPlayer.WidgetSoundEvent[] button_widget_sound_events = new WidgetSoundPlayer.WidgetSoundEvent[]
	{
		new WidgetSoundPlayer.WidgetSoundEvent(0, "On Use", "", true),
		new WidgetSoundPlayer.WidgetSoundEvent(1, "On Pointer Enter", "", true),
		new WidgetSoundPlayer.WidgetSoundEvent(2, "On Use Rejected", "", true)
	};

	public enum SoundEvents
	{
		OnClick,
		OnPointerEnter,
		OnClick_Rejected
	}
}
