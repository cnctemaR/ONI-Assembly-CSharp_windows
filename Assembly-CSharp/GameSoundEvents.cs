using System;

public static class GameSoundEvents
{
	public static GameSoundEvents.Event BatteryFull = new GameSoundEvents.Event("game_triggered.battery_full");

	public static GameSoundEvents.Event BatteryWarning = new GameSoundEvents.Event("game_triggered.battery_warning");

	public static GameSoundEvents.Event BatteryDischarged = new GameSoundEvents.Event("game_triggered.battery_drained");

	public class Event
	{
		public Event(string name)
		{
			this.Name = name;
		}

		public HashedString Name;
	}
}
