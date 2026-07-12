using System;

namespace UnityEngine.UIElements
{
	internal interface IRuntimePanel
	{
		PanelSettings panelSettings { get; }

		GameObject selectableGameObject { get; set; }
	}
}
