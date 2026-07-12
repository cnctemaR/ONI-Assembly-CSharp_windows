using System;
using UnityEngine;

public class EventInfoDataHelper
{
	public static EventInfoData GenerateStoryTraitData(string titleText, string descriptionText, string buttonText, string animFileName, EventInfoDataHelper.PopupType popupType, string buttonTooltip = null, GameObject[] minions = null, global::System.Action callback = null)
	{
		EventInfoData eventInfoData = new EventInfoData(titleText, descriptionText, animFileName);
		eventInfoData.minions = minions;
		if (popupType == EventInfoDataHelper.PopupType.BEGIN)
		{
			eventInfoData.showCallback = delegate
			{
				KFMOD.PlayUISound(GlobalAssets.GetSound("StoryTrait_Activation_Popup", false));
			};
		}
		if (popupType == EventInfoDataHelper.PopupType.COMPLETE)
		{
			eventInfoData.showCallback = delegate
			{
				MusicManager.instance.PlaySong("Stinger_StoryTraitUnlock", false);
			};
		}
		EventInfoData.Option option = eventInfoData.AddOption(buttonText, null);
		option.callback = callback;
		option.tooltip = buttonTooltip;
		return eventInfoData;
	}

	public enum PopupType
	{
		BEGIN,
		NORMAL,
		COMPLETE
	}
}
