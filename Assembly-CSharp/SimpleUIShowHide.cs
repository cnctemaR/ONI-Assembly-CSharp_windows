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
		if (!this.saveStatePreferenceKey.IsNullOrWhiteSpace() && KPlayerPrefs.GetInt(this.saveStatePreferenceKey, 1) != 1 && this.toggle.CurrentState == 0)
		{
			this.OnClick();
		}
	}

	private void OnClick()
	{
		this.toggle.NextState();
		this.content.SetActive(this.toggle.CurrentState == 0);
		if (!this.saveStatePreferenceKey.IsNullOrWhiteSpace())
		{
			KPlayerPrefs.SetInt(this.saveStatePreferenceKey, (this.toggle.CurrentState == 0) ? 1 : 0);
		}
	}

	[MyCmpReq]
	private MultiToggle toggle;

	[SerializeField]
	public GameObject content;

	[SerializeField]
	private string saveStatePreferenceKey;

	private const int onState = 0;
}
