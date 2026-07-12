using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class GameplayEventPopupData
{
	public GameplayEventPopupData()
	{
	}

	public GameplayEventPopupData(GameplayEvent evt)
	{
		this.title = evt.popupTitle;
		this.description = evt.popupDescription;
		this.animFileName = evt.popupAnimFileName;
		this.backgroundFileName = evt.popupBackgroundFileName;
		this.backgroundTint = evt.popupBackgroundTint;
	}

	public List<GameplayEventPopupData.PopupOption> GetOptions()
	{
		this.FinalizeText();
		return this.options;
	}

	public GameplayEventPopupData.PopupOption AddOption(string mainText, string description = null)
	{
		GameplayEventPopupData.PopupOption popupOption = new GameplayEventPopupData.PopupOption();
		popupOption.mainText = mainText;
		popupOption.description = description;
		this.options.Add(popupOption);
		this.dirty = true;
		return popupOption;
	}

	public GameplayEventPopupData.PopupOption SimpleOption(string mainText, global::System.Action callback)
	{
		GameplayEventPopupData.PopupOption popupOption = new GameplayEventPopupData.PopupOption
		{
			mainText = mainText,
			callback = callback
		};
		this.options.Add(popupOption);
		this.dirty = true;
		return popupOption;
	}

	public GameplayEventPopupData.PopupOption AddDefaultOption(global::System.Action callback = null)
	{
		return this.SimpleOption(GAMEPLAY_EVENTS.DEFAULT_OPTION_NAME, callback);
	}

	public GameplayEventPopupData.PopupOption AddDefaultConsiderLaterOption(global::System.Action callback = null)
	{
		return this.SimpleOption(GAMEPLAY_EVENTS.DEFAULT_OPTION_CONSIDER_NAME, callback);
	}

	public void SetTextParameter(string key, string value)
	{
		this.textParameters[key] = value;
		this.dirty = true;
	}

	public void FinalizeText()
	{
		if (!this.dirty)
		{
			return;
		}
		this.dirty = false;
		foreach (KeyValuePair<string, string> keyValuePair in this.textParameters)
		{
			string text = "{" + keyValuePair.Key + "}";
			if (this.title != null)
			{
				this.title = this.title.Replace(text, keyValuePair.Value);
			}
			if (this.description != null)
			{
				this.description = this.description.Replace(text, keyValuePair.Value);
			}
			if (this.location != null)
			{
				this.location = this.location.Replace(text, keyValuePair.Value);
			}
			if (this.whenDescription != null)
			{
				this.whenDescription = this.whenDescription.Replace(text, keyValuePair.Value);
			}
			foreach (GameplayEventPopupData.PopupOption popupOption in this.options)
			{
				if (popupOption.mainText != null)
				{
					popupOption.mainText = popupOption.mainText.Replace(text, keyValuePair.Value);
				}
				if (popupOption.description != null)
				{
					popupOption.description = popupOption.description.Replace(text, keyValuePair.Value);
				}
				if (popupOption.tooltip != null)
				{
					popupOption.tooltip = popupOption.tooltip.Replace(text, keyValuePair.Value);
				}
				foreach (GameplayEventPopupData.PopupOptionIcon popupOptionIcon in popupOption.informationIcons)
				{
					if (popupOptionIcon.tooltip != null)
					{
						popupOptionIcon.tooltip = popupOptionIcon.tooltip.Replace(text, keyValuePair.Value);
					}
				}
				foreach (GameplayEventPopupData.PopupOptionIcon popupOptionIcon2 in popupOption.consequenceIcons)
				{
					if (popupOptionIcon2.tooltip != null)
					{
						popupOptionIcon2.tooltip = popupOptionIcon2.tooltip.Replace(text, keyValuePair.Value);
					}
				}
			}
		}
	}

	public string title;

	public string description;

	public string location;

	public string whenDescription;

	public Transform focus;

	public GameObject[] minions;

	public GameObject artifact;

	public HashedString animFileName;

	public HashedString backgroundFileName;

	public Color32 backgroundTint;

	public Dictionary<string, string> textParameters = new Dictionary<string, string>();

	public List<GameplayEventPopupData.PopupOption> options = new List<GameplayEventPopupData.PopupOption>();

	private bool dirty;

	public class PopupOptionIcon
	{
		public PopupOptionIcon(Sprite sprite, GameplayEventPopupData.PopupOptionIcon.ContainerType containerType, string tooltip, float scale = 1f)
		{
			this.sprite = sprite;
			this.containerType = containerType;
			this.tooltip = tooltip;
			this.scale = scale;
		}

		public GameplayEventPopupData.PopupOptionIcon.ContainerType containerType;

		public Sprite sprite;

		public string tooltip;

		public float scale;

		public enum ContainerType
		{
			Neutral,
			Positive,
			Negative,
			Information
		}
	}

	public class PopupOption
	{
		public void AddInformationIcon(string tooltip, float scale = 1f)
		{
			this.informationIcons.Add(new GameplayEventPopupData.PopupOptionIcon(null, GameplayEventPopupData.PopupOptionIcon.ContainerType.Information, tooltip, scale));
		}

		public void AddPositiveIcon(Sprite sprite, string tooltip, float scale = 1f)
		{
			this.consequenceIcons.Add(new GameplayEventPopupData.PopupOptionIcon(sprite, GameplayEventPopupData.PopupOptionIcon.ContainerType.Positive, tooltip, scale));
		}

		public void AddNeutralIcon(Sprite sprite, string tooltip, float scale = 1f)
		{
			this.consequenceIcons.Add(new GameplayEventPopupData.PopupOptionIcon(sprite, GameplayEventPopupData.PopupOptionIcon.ContainerType.Neutral, tooltip, scale));
		}

		public void AddNegativeIcon(Sprite sprite, string tooltip, float scale = 1f)
		{
			this.consequenceIcons.Add(new GameplayEventPopupData.PopupOptionIcon(sprite, GameplayEventPopupData.PopupOptionIcon.ContainerType.Negative, tooltip, scale));
		}

		public string mainText;

		public string description;

		public string tooltip;

		public global::System.Action callback;

		public List<GameplayEventPopupData.PopupOptionIcon> informationIcons = new List<GameplayEventPopupData.PopupOptionIcon>();

		public List<GameplayEventPopupData.PopupOptionIcon> consequenceIcons = new List<GameplayEventPopupData.PopupOptionIcon>();

		public bool allowed = true;
	}
}
