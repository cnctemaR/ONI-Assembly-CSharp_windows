using System;
using Steamworks;
using UnityEngine;

public class FeedbackTextFix : MonoBehaviour
{
	private void Awake()
	{
		if (!DistributionPlatform.Initialized || !SteamUtils.IsSteamRunningOnSteamDeck())
		{
			global::UnityEngine.Object.DestroyImmediate(this);
			return;
		}
		this.locText.key = this.newKey;
	}

	public string newKey;

	public LocText locText;
}
