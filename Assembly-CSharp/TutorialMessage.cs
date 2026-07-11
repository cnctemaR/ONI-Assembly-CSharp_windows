using System;
using KSerialization;

public class TutorialMessage : GenericMessage
{
	public TutorialMessage()
	{
	}

	public TutorialMessage(Tutorial.TutorialMessages messageId, string title, string body, string tooltip, string videoClipId = null, string videoOverlayName = null, string videoTitleText = null, string icon = "")
		: base(title, body, tooltip)
	{
		this.messageId = messageId;
		this.videoClipId = videoClipId;
		this.videoOverlayName = videoOverlayName;
		this.videoTitleText = videoTitleText;
		this.icon = icon;
	}

	[Serialize]
	public Tutorial.TutorialMessages messageId;

	public string videoClipId;

	public string videoOverlayName;

	public string videoTitleText;

	public string icon;
}
