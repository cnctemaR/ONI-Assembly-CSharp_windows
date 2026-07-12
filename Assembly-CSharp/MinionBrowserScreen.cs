using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MinionBrowserScreen : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.gridLayouter = new GridLayouter
		{
			minCellSize = 112f,
			maxCellSize = 144f,
			targetGridLayout = this.galleryGridContent.GetComponent<GridLayoutGroup>()
		};
	}

	protected override void OnCmpEnable()
	{
		if (this.isFirstDisplay)
		{
			this.isFirstDisplay = false;
			this.PopulateGallery();
			this.RefreshPreview();
			this.cycleOutfitTypeLeft.onClick += delegate
			{
				this.CycleOutfitSelection(-1);
			};
			this.cycleOutfitTypeRight.onClick += delegate
			{
				this.CycleOutfitSelection(1);
			};
			this.editOutfitButton.onClick += this.OnClickEditOutfit;
			this.changeOutfitButton.onClick += this.OnClickChangeOutfit;
		}
		else
		{
			this.RefreshGalleryButtons();
			this.RefreshPreview();
		}
		KleiItemsStatusRefresher.AddOrGetListener(this).OnRefreshUI(delegate
		{
			this.RefreshGalleryButtons();
			this.RefreshPreview();
		});
		KleiItemsStatusRefresher.RequestRefreshFromServer();
	}

	private void Update()
	{
		this.gridLayouter.CheckIfShouldResizeGrid();
	}

	public void PopulateGallery()
	{
		foreach (KeyValuePair<string, MultiToggle> keyValuePair in this.personalities)
		{
			Util.KDestroyGameObject(keyValuePair.Value);
		}
		this.personalities.Clear();
		using (List<Personality>.Enumerator enumerator2 = Db.Get().Personalities.GetAll(true, false).GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				Personality personality = enumerator2.Current;
				GameObject gameObject = Util.KInstantiateUI(this.gridItemPrefab, this.galleryGridContent.gameObject, true);
				gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = personality.GetMiniIcon();
				gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(personality.Name);
				this.personalities.Add(personality.Id, gameObject.GetComponent<MultiToggle>());
				MultiToggle component = gameObject.GetComponent<MultiToggle>();
				component.onClick = (global::System.Action)Delegate.Combine(component.onClick, new global::System.Action(delegate
				{
					this.SelectMinion(personality.Id);
				}));
			}
		}
		this.RefreshGalleryButtons();
		this.SelectMinion(Db.Get().Personalities.resources.First<Personality>((Personality d) => !d.Disabled).Id);
	}

	private void SelectMinion(string personalityId)
	{
		this.selectedPersonalityId = personalityId;
		MinionBrowserScreen.MinionVoice.ByPersonality(personalityId).PlaySoundUI("voice_land");
		this.RefreshGalleryButtons();
		this.RefreshPreview();
	}

	private void RefreshGalleryButtons()
	{
		foreach (KeyValuePair<string, MultiToggle> keyValuePair in this.personalities)
		{
			keyValuePair.Value.ChangeState((keyValuePair.Key == this.selectedPersonalityId) ? 1 : 0);
		}
	}

	private void CycleOutfitSelection(int direction)
	{
		int num = (int)((this.selectedOutfitType + direction) % ClothingOutfitUtility.OutfitType.LENGTH);
		if (num < 0)
		{
			num++;
		}
		this.selectedOutfitType = (ClothingOutfitUtility.OutfitType)num;
		this.RefreshPreview();
	}

	public void RefreshPreview()
	{
		Personality personality = Db.Get().Personalities.Get(this.selectedPersonalityId);
		this.UIMinion.SetMinion(personality);
		this.UIMinion.ReactToPersonalityChange();
		this.detailsHeaderText.SetText(personality.Name);
		this.detailHeaderIcon.sprite = personality.GetMiniIcon();
		this.RefreshOutfitDescription();
		this.outfitTypeLabel.SetText(this.selectedOutfitType.GetName());
	}

	private void RefreshOutfitDescription()
	{
		if (this.selectedOutfitType == ClothingOutfitUtility.OutfitType.Clothing)
		{
			string outfit = Db.Get().Personalities.Get(this.selectedPersonalityId).GetOutfit(this.selectedOutfitType);
			this.selectedOutfit = ClothingOutfitTarget.TryFromId(outfit);
			this.UIMinion.SetOutfit(this.selectedOutfit);
			this.outfitDescriptionPanel.Refresh(this.selectedOutfit);
		}
	}

	private void OnClickEditOutfit()
	{
		Personality personality = Db.Get().Personalities.Get(this.selectedPersonalityId);
		OutfitDesignerScreenConfig.Minion(this.selectedOutfit, personality).ApplyAndOpenScreen();
	}

	private void OnClickChangeOutfit()
	{
		OutfitBrowserScreenConfig.Minion(Db.Get().Personalities.Get(this.selectedPersonalityId)).WithOutfit(this.selectedOutfit).ApplyAndOpenScreen();
	}

	[Header("ItemGalleryColumn")]
	[SerializeField]
	private RectTransform galleryGridContent;

	[SerializeField]
	private GameObject gridItemPrefab;

	private GridLayouter gridLayouter;

	[Header("SelectionDetailsColumn")]
	[SerializeField]
	private KleiPermitDioramaVis permitVis;

	[SerializeField]
	private UIMinion UIMinion;

	[SerializeField]
	private LocText detailsHeaderText;

	[SerializeField]
	private Image detailHeaderIcon;

	[SerializeField]
	private OutfitDescriptionPanel outfitDescriptionPanel;

	[Header("Outfit Cycler")]
	[SerializeField]
	private KButton cycleOutfitTypeLeft;

	[SerializeField]
	private KButton cycleOutfitTypeRight;

	[SerializeField]
	private LocText outfitTypeLabel;

	[SerializeField]
	private KButton editOutfitButton;

	[SerializeField]
	private KButton changeOutfitButton;

	private ClothingOutfitUtility.OutfitType selectedOutfitType;

	private Option<ClothingOutfitTarget> selectedOutfit;

	private string selectedPersonalityId;

	private Dictionary<string, MultiToggle> personalities = new Dictionary<string, MultiToggle>();

	private bool isFirstDisplay = true;

	public readonly struct MinionVoice
	{
		public MinionVoice(int voiceIndex)
		{
			this.voiceIndex = voiceIndex;
			this.voiceId = (voiceIndex + 1).ToString("D2");
			this.isValid = true;
		}

		public static MinionBrowserScreen.MinionVoice ByPersonality(string personalityId)
		{
			if (personalityId == "Jorge")
			{
				return new MinionBrowserScreen.MinionVoice(-2);
			}
			return new MinionBrowserScreen.MinionVoice(global::UnityEngine.Random.Range(0, 4));
		}

		public static MinionBrowserScreen.MinionVoice Random()
		{
			return new MinionBrowserScreen.MinionVoice(global::UnityEngine.Random.Range(0, 4));
		}

		public string GetSoundAssetName(string localName)
		{
			global::Debug.Assert(this.isValid);
			string text = localName;
			if (localName.Contains(":"))
			{
				text = localName.Split(new char[] { ':' })[0];
			}
			return StringFormatter.Combine("DupVoc_", this.voiceId, "_", text);
		}

		public string GetSoundPath(string localName)
		{
			return GlobalAssets.GetSound(this.GetSoundAssetName(localName), true);
		}

		public void PlaySoundUI(string localName)
		{
			global::Debug.Assert(this.isValid);
			string soundPath = this.GetSoundPath(localName);
			try
			{
				if (SoundListenerController.Instance == null)
				{
					KFMOD.PlayUISound(soundPath);
				}
				else
				{
					KFMOD.PlayOneShot(soundPath, SoundListenerController.Instance.transform.GetPosition(), 1f);
				}
			}
			catch
			{
				DebugUtil.LogWarningArgs(new object[] { "AUDIOERROR: Missing [" + soundPath + "]" });
			}
		}

		public readonly int voiceIndex;

		public readonly string voiceId;

		public readonly bool isValid;
	}
}
