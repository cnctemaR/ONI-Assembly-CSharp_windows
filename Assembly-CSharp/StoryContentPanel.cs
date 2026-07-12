using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Klei.CustomSettings;
using ProcGen;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class StoryContentPanel : KMonoBehaviour
{
	public List<string> GetActiveStories()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, StoryContentPanel.StoryState> keyValuePair in this.storyStates)
		{
			if (keyValuePair.Value == StoryContentPanel.StoryState.Guaranteed)
			{
				list.Add(keyValuePair.Key);
			}
		}
		return list;
	}

	public void Init()
	{
		this.SpawnRows();
		this.RefreshRows();
		this.RefreshDescriptionPanel();
		this.SelectDefault();
		CustomGameSettings.Instance.OnStorySettingChanged += this.OnStorySettingChanged;
	}

	public void Cleanup()
	{
		CustomGameSettings.Instance.OnStorySettingChanged -= this.OnStorySettingChanged;
	}

	private void OnStorySettingChanged(SettingConfig config, SettingLevel level)
	{
		this.storyStates[config.id] = ((level.id == "Guaranteed") ? StoryContentPanel.StoryState.Guaranteed : StoryContentPanel.StoryState.Forbidden);
		this.RefreshStoryDisplay(config.id);
	}

	private void SpawnRows()
	{
		using (List<Story>.Enumerator enumerator = Db.Get().Stories.resources.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Story story = enumerator.Current;
				GameObject gameObject = global::Util.KInstantiateUI(this.storyRowPrefab, this.storyRowContainer, true);
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				component.GetReference<LocText>("Label").SetText(Strings.Get(story.StoryTrait.name));
				MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
				component2.onClick = (global::System.Action)Delegate.Combine(component2.onClick, new global::System.Action(delegate
				{
					this.SelectRow(story.Id);
				}));
				this.storyRows.Add(story.Id, gameObject);
				component.GetReference<Image>("Icon").sprite = Assets.GetSprite(story.StoryTrait.icon);
				MultiToggle reference = component.GetReference<MultiToggle>("checkbox");
				reference.onClick = (global::System.Action)Delegate.Combine(reference.onClick, new global::System.Action(delegate
				{
					this.IncrementStorySetting(story.Id, true);
					this.RefreshStoryDisplay(story.Id);
				}));
				this.storyStates.Add(story.Id, this._defaultStoryState);
			}
		}
		this.RefreshAllStoryStates();
		this.mainScreen.RefreshStoryLabel();
	}

	private void SelectRow(string id)
	{
		this.selectedStoryId = id;
		this.RefreshRows();
		this.RefreshDescriptionPanel();
	}

	public void SelectDefault()
	{
		foreach (KeyValuePair<string, StoryContentPanel.StoryState> keyValuePair in this.storyStates)
		{
			if (keyValuePair.Value == StoryContentPanel.StoryState.Guaranteed)
			{
				this.SelectRow(keyValuePair.Key);
				return;
			}
		}
		using (Dictionary<string, StoryContentPanel.StoryState>.Enumerator enumerator = this.storyStates.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				KeyValuePair<string, StoryContentPanel.StoryState> keyValuePair2 = enumerator.Current;
				this.SelectRow(keyValuePair2.Key);
			}
		}
	}

	private void IncrementStorySetting(string storyId, bool forward = true)
	{
		int num = (int)this.storyStates[storyId];
		num += (forward ? 1 : (-1));
		if (num < 0)
		{
			num += 2;
		}
		num %= 2;
		this.SetStoryState(storyId, (StoryContentPanel.StoryState)num);
		this.mainScreen.RefreshRowsAndDescriptions();
	}

	private void SetStoryState(string storyId, StoryContentPanel.StoryState state)
	{
		this.storyStates[storyId] = state;
		SettingConfig settingConfig = CustomGameSettings.Instance.StorySettings[storyId];
		CustomGameSettings.Instance.SetStorySetting(settingConfig, this.storyStates[storyId] == StoryContentPanel.StoryState.Guaranteed);
	}

	public void SelectRandomStories(int min = 5, int max = 5, bool useBias = false)
	{
		int num = global::UnityEngine.Random.Range(min, max);
		List<Story> list = new List<Story>(Db.Get().Stories.resources);
		List<Story> list2 = new List<Story>();
		list.Shuffle<Story>();
		int num2 = 0;
		while (num2 < num && list.Count - 1 >= num2)
		{
			list2.Add(list[num2]);
			num2++;
		}
		float num3 = 0.7f;
		int num4 = list2.Count<Story>((Story x) => x.IsNew());
		if (useBias && num4 == 0 && global::UnityEngine.Random.value < num3)
		{
			List<Story> list3 = Db.Get().Stories.resources.Where<Story>((Story x) => x.IsNew()).ToList<Story>();
			list3.Shuffle<Story>();
			if (list3.Count > 0)
			{
				list2.RemoveAt(0);
				list2.Add(list3[0]);
			}
		}
		foreach (Story story in list)
		{
			this.SetStoryState(story.Id, list2.Contains(story) ? StoryContentPanel.StoryState.Guaranteed : StoryContentPanel.StoryState.Forbidden);
		}
		this.RefreshAllStoryStates();
		this.mainScreen.RefreshRowsAndDescriptions();
	}

	private void RefreshAllStoryStates()
	{
		foreach (string text in this.storyRows.Keys)
		{
			this.RefreshStoryDisplay(text);
		}
	}

	private void RefreshStoryDisplay(string id)
	{
		MultiToggle reference = this.storyRows[id].GetComponent<HierarchyReferences>().GetReference<MultiToggle>("checkbox");
		StoryContentPanel.StoryState storyState = this.storyStates[id];
		if (storyState == StoryContentPanel.StoryState.Forbidden)
		{
			reference.ChangeState(0);
			return;
		}
		if (storyState != StoryContentPanel.StoryState.Guaranteed)
		{
			return;
		}
		reference.ChangeState(1);
	}

	private void RefreshRows()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.storyRows)
		{
			keyValuePair.Value.GetComponent<MultiToggle>().ChangeState((keyValuePair.Key == this.selectedStoryId) ? 1 : 0);
		}
	}

	private void RefreshDescriptionPanel()
	{
		if (this.selectedStoryId.IsNullOrWhiteSpace())
		{
			this.selectedStoryTitleLabel.SetText("");
			this.selectedStoryDescriptionLabel.SetText("");
			return;
		}
		WorldTrait storyTrait = Db.Get().Stories.GetStoryTrait(this.selectedStoryId, true);
		this.selectedStoryTitleLabel.SetText(Strings.Get(storyTrait.name));
		this.selectedStoryDescriptionLabel.SetText(Strings.Get(storyTrait.description));
		string text = storyTrait.icon.Replace("_icon", "_image");
		this.selectedStoryImage.sprite = Assets.GetSprite(text);
	}

	public string GetTraitsString(bool tooltip = false)
	{
		int num = 0;
		int num2 = 5;
		foreach (KeyValuePair<string, StoryContentPanel.StoryState> keyValuePair in this.storyStates)
		{
			if (keyValuePair.Value == StoryContentPanel.StoryState.Guaranteed)
			{
				num++;
			}
		}
		string text = UI.FRONTEND.COLONYDESTINATIONSCREEN.STORY_TRAITS_HEADER;
		string text2;
		if (num != 0)
		{
			if (num != 1)
			{
				text2 = string.Format(UI.FRONTEND.COLONYDESTINATIONSCREEN.TRAIT_COUNT, num);
			}
			else
			{
				text2 = UI.FRONTEND.COLONYDESTINATIONSCREEN.SINGLE_TRAIT;
			}
		}
		else
		{
			text2 = UI.FRONTEND.COLONYDESTINATIONSCREEN.NO_TRAITS;
		}
		text = text + ": " + text2;
		if (num > num2)
		{
			text = text + " " + UI.FRONTEND.COLONYDESTINATIONSCREEN.TOO_MANY_TRAITS_WARNING;
		}
		if (tooltip)
		{
			foreach (KeyValuePair<string, StoryContentPanel.StoryState> keyValuePair2 in this.storyStates)
			{
				if (keyValuePair2.Value == StoryContentPanel.StoryState.Guaranteed)
				{
					WorldTrait storyTrait = Db.Get().Stories.Get(keyValuePair2.Key).StoryTrait;
					text = string.Concat(new string[]
					{
						text,
						"\n\n<b>",
						Strings.Get(storyTrait.name).String,
						"</b>\n",
						Strings.Get(storyTrait.description).String
					});
				}
			}
			if (num > num2)
			{
				text = text + "\n\n" + UI.FRONTEND.COLONYDESTINATIONSCREEN.TOO_MANY_TRAITS_WARNING_TOOLTIP;
			}
		}
		return text;
	}

	[SerializeField]
	private GameObject storyRowPrefab;

	[SerializeField]
	private GameObject storyRowContainer;

	private Dictionary<string, GameObject> storyRows = new Dictionary<string, GameObject>();

	public const int DEFAULT_RANDOMIZE_STORY_COUNT = 5;

	private Dictionary<string, StoryContentPanel.StoryState> storyStates = new Dictionary<string, StoryContentPanel.StoryState>();

	private string selectedStoryId = "";

	[SerializeField]
	private ColonyDestinationSelectScreen mainScreen;

	[Header("Trait Count")]
	[Header("SelectedStory")]
	[SerializeField]
	private Image selectedStoryImage;

	[SerializeField]
	private LocText selectedStoryTitleLabel;

	[SerializeField]
	private LocText selectedStoryDescriptionLabel;

	[SerializeField]
	private Sprite spriteForbidden;

	[SerializeField]
	private Sprite spritePossible;

	[SerializeField]
	private Sprite spriteGuaranteed;

	private StoryContentPanel.StoryState _defaultStoryState;

	private List<string> storyTraitSettings = new List<string> { "None", "Few", "Lots" };

	private enum StoryState
	{
		Forbidden,
		Guaranteed,
		LENGTH
	}
}
