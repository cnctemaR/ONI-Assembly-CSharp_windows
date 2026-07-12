using System;
using KSerialization;

public class TutorialMessage : GenericMessage
{
	public TutorialMessage()
	{
	}

	public TutorialMessage(Tutorial.TutorialMessages messageId, string title, string body, string tooltip, string videoClipId = null, string videoOverlayName = null, string videoTitleText = null, string icon = "", string[] overrideDLCIDs = null)
		: base(title, body, tooltip, null)
	{
		this.messageId = messageId;
		this.videoClipId = videoClipId;
		this.videoOverlayName = videoOverlayName;
		this.videoTitleText = videoTitleText;
		this.icon = icon;
		if (overrideDLCIDs != null)
		{
			this.DLCIDs = overrideDLCIDs;
		}
	}

	[Serialize]
	public Tutorial.TutorialMessages messageId;

	public string videoClipId;

	public string videoOverlayName;

	public string videoTitleText;

	public string icon;

	public string[] DLCIDs = DlcManager.AVAILABLE_ALL_VERSIONS;
}
