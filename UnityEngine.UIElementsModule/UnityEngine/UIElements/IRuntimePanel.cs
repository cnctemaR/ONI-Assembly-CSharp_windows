using System;

namespace UnityEngine.UIElements
{
	public interface IRuntimePanel : IPanel, IDisposable
	{
		PanelSettings panelSettings { get; }

		GameObject selectableGameObject { get; set; }
	}
}
