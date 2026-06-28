using System;
using UnityEngine;

public class DebugOverlays : KScreen
{
	public static DebugOverlays instance { get; private set; }

	protected override void OnPrefabInit()
	{
		DebugOverlays.instance = this;
		KPopupMenu componentInChildren = base.GetComponentInChildren<KPopupMenu>();
		componentInChildren.SetOptions(new string[] { "None", "Rooms", "Lighting", "Style", "Flow" });
		KPopupMenu kpopupMenu = componentInChildren;
		kpopupMenu.OnSelect = (Action<string>)Delegate.Combine(kpopupMenu.OnSelect, new Action<string>(this.OnSelect));
		base.gameObject.SetActive(false);
	}

	private void OnSelect(string str)
	{
		switch (str)
		{
		case "None":
			SimDebugView.Instance.SetMode(SimViewMode.None);
			return;
		case "Flow":
			SimDebugView.Instance.SetMode(SimViewMode.Flow);
			return;
		case "Lighting":
			SimDebugView.Instance.SetMode(SimViewMode.Light);
			return;
		case "Rooms":
			SimDebugView.Instance.SetMode(SimViewMode.Rooms);
			return;
		}
		Debug.LogError("Unknown debug view: " + str);
	}
}
