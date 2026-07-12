using System;
using UnityEngine;

public class FoldOutPanel : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		MultiToggle componentInChildren = base.GetComponentInChildren<MultiToggle>();
		componentInChildren.onClick = (global::System.Action)Delegate.Combine(componentInChildren.onClick, new global::System.Action(this.OnClick));
		this.ToggleOpen(this.startOpen);
	}

	private void OnClick()
	{
		this.ToggleOpen(!this.panelOpen);
	}

	private void ToggleOpen(bool open)
	{
		this.panelOpen = open;
		this.container.SetActive(this.panelOpen);
		base.GetComponentInChildren<MultiToggle>().ChangeState(this.panelOpen ? 1 : 0);
	}

	private bool panelOpen = true;

	public GameObject container;

	public bool startOpen = true;
}
