using System;
using UnityEngine;

public class DebugElementMenu : KButtonMenu
{
	protected override void OnPrefabInit()
	{
		DebugElementMenu.Instance = this;
		base.OnPrefabInit();
		base.ConsumeMouseScroll = true;
	}

	protected override void OnForcedCleanUp()
	{
		DebugElementMenu.Instance = null;
		base.OnForcedCleanUp();
	}

	public void Turnoff()
	{
		this.root.gameObject.SetActive(false);
	}

	public static DebugElementMenu Instance;

	public GameObject root;
}
