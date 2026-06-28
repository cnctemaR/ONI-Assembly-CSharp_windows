using System;
using UnityEngine;

public class KAnimLink
{
	public KAnimLink(KAnimControllerBase master, KAnimControllerBase slave)
	{
		master.onTemperatureColorChanged += delegate(Color32 c)
		{
			if (slave != null)
			{
				slave.TemperatureColour = c;
			}
		};
		master.OnTintChanged = (Action<Color32>)Delegate.Combine(master.OnTintChanged, new Action<Color32>(delegate(Color32 c)
		{
			if (slave != null)
			{
				slave.TintColour = c;
			}
		}));
		master.OnHighlightChanged = (Action<Color32>)Delegate.Combine(master.OnHighlightChanged, new Action<Color32>(delegate(Color32 c)
		{
			if (slave != null)
			{
				slave.HighlightColour = c;
			}
		}));
	}
}
