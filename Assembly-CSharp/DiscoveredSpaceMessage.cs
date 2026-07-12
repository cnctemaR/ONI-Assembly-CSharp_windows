using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class DiscoveredSpaceMessage : Message
{
	public DiscoveredSpaceMessage()
	{
	}

	public DiscoveredSpaceMessage(Vector3 pos)
	{
		this.cameraFocusPos = pos;
		this.cameraFocusPos.z = -40f;
	}

	public override string GetSound()
	{
		return "Discover_Space";
	}

	public override string GetMessageBody()
	{
		return MISC.NOTIFICATIONS.DISCOVERED_SPACE.TOOLTIP;
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.DISCOVERED_SPACE.NAME;
	}

	public override string GetTooltip()
	{
		return null;
	}

	public override bool IsValid()
	{
		return true;
	}

	public override void OnClick()
	{
		this.OnDiscoveredSpaceClicked();
	}

	private void OnDiscoveredSpaceClicked()
	{
		KFMOD.PlayUISound(GlobalAssets.GetSound(this.GetSound(), false));
		MusicManager.instance.PlaySong("Stinger_Surface", false);
		CameraController.Instance.SetTargetPos(this.cameraFocusPos, 8f, true);
	}

	[Serialize]
	private Vector3 cameraFocusPos;

	private const string MUSIC_STINGER = "Stinger_Surface";
}
