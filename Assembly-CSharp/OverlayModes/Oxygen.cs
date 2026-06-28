using System;

namespace OverlayModes
{
	public class Oxygen : Mode
	{
		public override SimViewMode ViewMode()
		{
			return SimViewMode.OxygenMap;
		}

		public override string GetSoundName()
		{
			return "Oxygen";
		}
	}
}
