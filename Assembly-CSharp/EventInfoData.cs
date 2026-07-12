using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class EventInfoData
{
	public EventInfoData(string title, string description, HashedString animFileName)
	{
		this.title = title;
		this.description = description;
		this.animFileName = animFileName;
	}

	public List<EventInfoData.Option> GetOptions()
	{
		this.FinalizeText();
		return this.options;
	}

	public EventInfoData.Option AddOption(string mainText, string description = null)
	{
		EventInfoData.Option option = new EventInfoData.Option
		{
			mainText = mainText,
			description = description
		};
		this.options.Add(option);
		this.dirty = true;
		return option;
	}

	public EventInfoData.Option SimpleOption(string mainText, global::System.Action callback)
	{
		EventInfoData.Option option = new EventInfoData.Option
		{
			mainText = mainText,
			callback = callback
		};
		this.options.Add(option);
		this.dirty = true;
		return option;
	}

	public EventInfoData.Option AddDefaultOption(global::System.Action callback = null)
	{
		return this.SimpleOption(GAMEPLAY_EVENTS.DEFAULT_OPTION_NAME, callback);
	}

	public EventInfoData.Option AddDefaultConsiderLaterOption(global::System.Action callback = null)
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
			foreach (EventInfoData.Option option in this.options)
			{
				if (option.mainText != null)
				{
					option.mainText = option.mainText.Replace(text, keyValuePair.Value);
				}
				if (option.description != null)
				{
					option.description = option.description.Replace(text, keyValuePair.Value);
				}
				if (option.tooltip != null)
				{
					option.tooltip = option.tooltip.Replace(text, keyValuePair.Value);
				}
				foreach (EventInfoData.OptionIcon optionIcon in option.informationIcons)
				{
					if (optionIcon.tooltip != null)
					{
						optionIcon.tooltip = optionIcon.tooltip.Replace(text, keyValuePair.Value);
					}
				}
				foreach (EventInfoData.OptionIcon optionIcon2 in option.consequenceIcons)
				{
					if (optionIcon2.tooltip != null)
					{
						optionIcon2.tooltip = optionIcon2.tooltip.Replace(text, keyValuePair.Value);
					}
				}
			}
		}
	}

	public string title;

	public string description;

	public string location;

	public string whenDescription;

	public Transform clickFocus;

	public GameObject[] minions;

	public GameObject artifact;

	public HashedString animFileName;

	public HashedString mainAnim = "event";

	public Dictionary<string, string> textParameters = new Dictionary<string, string>();

	public List<EventInfoData.Option> options = new List<EventInfoData.Option>();

	public global::System.Action showCallback;

	private bool dirty;

	public class OptionIcon
	{
		public OptionIcon(Sprite sprite, EventInfoData.OptionIcon.ContainerType containerType, string tooltip, float scale = 1f)
		{
			this.sprite = sprite;
			this.containerType = containerType;
			this.tooltip = tooltip;
			this.scale = scale;
		}

		public EventInfoData.OptionIcon.ContainerType containerType;

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

	public class Option
	{
		public void AddInformationIcon(string tooltip, float scale = 1f)
		{
			this.informationIcons.Add(new EventInfoData.OptionIcon(null, EventInfoData.OptionIcon.ContainerType.Information, tooltip, scale));
		}

		public void AddPositiveIcon(Sprite sprite, string tooltip, float scale = 1f)
		{
			this.consequenceIcons.Add(new EventInfoData.OptionIcon(sprite, EventInfoData.OptionIcon.ContainerType.Positive, tooltip, scale));
		}

		public void AddNeutralIcon(Sprite sprite, string tooltip, float scale = 1f)
		{
			this.consequenceIcons.Add(new EventInfoData.OptionIcon(sprite, EventInfoData.OptionIcon.ContainerType.Neutral, tooltip, scale));
		}

		public void AddNegativeIcon(Sprite sprite, string tooltip, float scale = 1f)
		{
			this.consequenceIcons.Add(new EventInfoData.OptionIcon(sprite, EventInfoData.OptionIcon.ContainerType.Negative, tooltip, scale));
		}

		public string mainText;

		public string description;

		public string tooltip;

		public global::System.Action callback;

		public List<EventInfoData.OptionIcon> informationIcons = new List<EventInfoData.OptionIcon>();

		public List<EventInfoData.OptionIcon> consequenceIcons = new List<EventInfoData.OptionIcon>();

		public bool allowed = true;
	}
}
