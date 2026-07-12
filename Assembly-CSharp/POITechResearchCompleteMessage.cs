using System;
using KSerialization;
using STRINGS;

public class POITechResearchCompleteMessage : Message
{
	public POITechResearchCompleteMessage()
	{
	}

	public POITechResearchCompleteMessage(POITechItemUnlocks.Def unlocked_items)
	{
		this.unlockedItemsdef = unlocked_items;
		this.popupName = unlocked_items.PopUpName;
		this.animName = unlocked_items.animName;
	}

	public override string GetSound()
	{
		return "AI_Notification_ResearchComplete";
	}

	public override string GetMessageBody()
	{
		string text = "";
		for (int i = 0; i < this.unlockedItemsdef.POITechUnlockIDs.Count; i++)
		{
			TechItem techItem = Db.Get().TechItems.TryGet(this.unlockedItemsdef.POITechUnlockIDs[i]);
			if (techItem != null)
			{
				text = text + "\n    • " + techItem.Name;
			}
		}
		return string.Format(MISC.NOTIFICATIONS.POIRESEARCHUNLOCKCOMPLETE.MESSAGEBODY, text);
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.POIRESEARCHUNLOCKCOMPLETE.NAME;
	}

	public override string GetTooltip()
	{
		return string.Format(MISC.NOTIFICATIONS.POIRESEARCHUNLOCKCOMPLETE.TOOLTIP, this.popupName);
	}

	public override bool IsValid()
	{
		return this.unlockedItemsdef != null;
	}

	public override bool ShowDialog()
	{
		EventInfoData eventInfoData = new EventInfoData(MISC.NOTIFICATIONS.POIRESEARCHUNLOCKCOMPLETE.NAME, this.GetMessageBody(), this.animName);
		eventInfoData.AddDefaultOption(null);
		EventInfoScreen.ShowPopup(eventInfoData);
		Messenger.Instance.RemoveMessage(this);
		return false;
	}

	public override bool ShowDismissButton()
	{
		return false;
	}

	public override NotificationType GetMessageType()
	{
		return NotificationType.Messages;
	}

	[Serialize]
	public POITechItemUnlocks.Def unlockedItemsdef;

	[Serialize]
	public string popupName;

	[Serialize]
	public string animName;
}
