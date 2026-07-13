using System;

namespace UnityEngine.Android
{
	public enum ProcessImportance
	{
		Foreground = 100,
		ForeGroundService = 125,
		Visible = 200,
		Perceptible = 230,
		TopSleeping = 325,
		CantSaveState = 350,
		Service = 300,
		Cached = 400,
		Gone = 1000
	}
}
