using System;
using UnityEngine;

public class TopLeftControlScreen : KScreen
{
	protected override void OnActivate()
	{
		base.OnActivate();
		TopLeftControlScreen.Instance = this;
		this.RefreshName();
	}

	public void RefreshName()
	{
		if (SaveGame.Instance != null)
		{
			this.locText.text = SaveGame.Instance.BaseName;
		}
	}

	public static TopLeftControlScreen Instance;

	[SerializeField]
	private LocText locText;
}
