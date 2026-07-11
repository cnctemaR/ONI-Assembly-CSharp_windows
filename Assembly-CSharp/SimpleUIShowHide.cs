using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SimpleUIShowHide")]
public class SimpleUIShowHide : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle multiToggle = this.toggle;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(this.OnClick));
	}

	private void OnClick()
	{
		this.toggle.NextState();
		this.content.SetActive(this.toggle.CurrentState == 0);
	}

	[MyCmpReq]
	private MultiToggle toggle;

	[SerializeField]
	public GameObject content;
}
