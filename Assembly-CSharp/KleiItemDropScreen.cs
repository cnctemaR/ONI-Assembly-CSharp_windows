using System;
using System.Collections;
using Database;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class KleiItemDropScreen : KModalScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		KleiItemDropScreen.Instance = this;
		this.closeButton.onClick += delegate
		{
			this.Show(false);
		};
		if (string.IsNullOrEmpty(KleiAccount.KleiToken))
		{
			base.Show(false);
		}
	}

	protected override void OnActivate()
	{
		KleiItemDropScreen.Instance = this;
		this.Show(false);
	}

	public override void Show(bool show = true)
	{
		if (!show)
		{
			if (this.activePresentationRoutine != null)
			{
				base.StopCoroutine(this.activePresentationRoutine);
			}
			if (this.shouldDoCloseRoutine)
			{
				this.closeButton.gameObject.SetActive(false);
				Updater.RunRoutine(this, this.AnimateScreenOutRoutine()).Then(delegate
				{
					base.Show(false);
				});
				this.shouldDoCloseRoutine = false;
			}
			else
			{
				base.Show(false);
			}
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndItemDropScreenSnapshot, STOP_MODE.ALLOWFADEOUT);
			return;
		}
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndItemDropScreenSnapshot);
		base.Show(true);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.Show(false);
		}
		base.OnKeyDown(e);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (!show)
		{
			return;
		}
		if (PermitItems.HasUnopenedItem())
		{
			this.PresentNextUnopenedItem(true);
			this.shouldDoCloseRoutine = true;
			return;
		}
		this.userMessageLabel.SetText(UI.ITEM_DROP_SCREEN.NOTHING_AVAILABLE);
		this.PresentNoItemAvailablePrompt(true);
		this.shouldDoCloseRoutine = true;
	}

	public void PresentNextUnopenedItem(bool firstItemPresentation = true)
	{
		foreach (KleiItems.ItemData itemData in PermitItems.IterateInventory())
		{
			if (!itemData.IsOpened)
			{
				this.PresentItem(itemData, firstItemPresentation);
				return;
			}
		}
		this.PresentNoItemAvailablePrompt(false);
	}

	public void PresentItem(KleiItems.ItemData item, bool firstItemPresentation)
	{
		this.userMessageLabel.SetText(UI.ITEM_DROP_SCREEN.THANKS_FOR_PLAYING);
		this.giftRevealed = false;
		this.giftAcknowledged = false;
		if (this.activePresentationRoutine != null)
		{
			base.StopCoroutine(this.activePresentationRoutine);
		}
		this.activePresentationRoutine = base.StartCoroutine(this.PresentItemRoutine(item, firstItemPresentation));
		this.acceptButton.ClearOnClick();
		this.acknowledgeButton.ClearOnClick();
		this.acceptButton.GetComponentInChildren<LocText>().SetText(UI.ITEM_DROP_SCREEN.PRINT_ITEM_BUTTON);
		this.acceptButton.onClick += delegate
		{
			this.giftRevealed = true;
			PermitItems.QueueRequestOpenOrUnboxItem(item);
		};
		this.acknowledgeButton.onClick += delegate
		{
			if (this.giftRevealed)
			{
				this.giftAcknowledged = true;
			}
		};
	}

	public void PresentNoItemAvailablePrompt(bool firstItemPresentation)
	{
		this.userMessageLabel.SetText(UI.ITEM_DROP_SCREEN.NOTHING_AVAILABLE);
		this.noItemAvailableAcknowledged = false;
		this.acknowledgeButton.ClearOnClick();
		this.acceptButton.ClearOnClick();
		this.acceptButton.GetComponentInChildren<LocText>().SetText(UI.ITEM_DROP_SCREEN.DISMISS_BUTTON);
		this.acceptButton.onClick += delegate
		{
			this.noItemAvailableAcknowledged = true;
		};
		if (this.activePresentationRoutine != null)
		{
			base.StopCoroutine(this.activePresentationRoutine);
		}
		this.activePresentationRoutine = base.StartCoroutine(this.PresentNoItemAvailableRoutine(firstItemPresentation));
	}

	private IEnumerator AnimateScreenInRoutine()
	{
		float scaleFactor = base.transform.parent.GetComponent<CanvasScaler>().scaleFactor;
		float OPEN_WIDTH = (float)Screen.width / scaleFactor;
		float num = Mathf.Clamp((float)Screen.height / scaleFactor, 720f, 900f);
		KFMOD.PlayUISound(GlobalAssets.GetSound("GiftItemDrop_Screen_Open", false));
		this.userMessageLabel.gameObject.SetActive(false);
		yield return Updater.Ease(delegate(Vector2 v2)
		{
			this.shieldMaskRect.sizeDelta = v2;
		}, this.shieldMaskRect.sizeDelta, new Vector2(this.shieldMaskRect.sizeDelta.x, num), 0.5f, Easing.CircInOut);
		yield return Updater.Ease(delegate(Vector2 v2)
		{
			this.shieldMaskRect.sizeDelta = v2;
		}, this.shieldMaskRect.sizeDelta, new Vector2(OPEN_WIDTH, this.shieldMaskRect.sizeDelta.y), 0.25f, Easing.CircInOut);
		this.userMessageLabel.gameObject.SetActive(true);
		yield break;
	}

	private IEnumerator AnimateScreenOutRoutine()
	{
		KFMOD.PlayUISound(GlobalAssets.GetSound("GiftItemDrop_Screen_Close", false));
		this.userMessageLabel.gameObject.SetActive(false);
		yield return Updater.Ease(delegate(Vector2 v2)
		{
			this.shieldMaskRect.sizeDelta = v2;
		}, this.shieldMaskRect.sizeDelta, new Vector2(8f, this.shieldMaskRect.sizeDelta.y), 0.25f, Easing.CircInOut);
		yield return Updater.Ease(delegate(Vector2 v2)
		{
			this.shieldMaskRect.sizeDelta = v2;
		}, this.shieldMaskRect.sizeDelta, new Vector2(this.shieldMaskRect.sizeDelta.x, 0f), 0.25f, Easing.CircInOut);
		yield break;
	}

	private IEnumerator PresentNoItemAvailableRoutine(bool firstItem)
	{
		yield return null;
		this.itemNameLabel.SetText("");
		this.itemDescriptionLabel.SetText("");
		this.itemRarityLabel.SetText("");
		this.itemCategoryLabel.SetText("");
		if (firstItem)
		{
			this.animatedPod.Play("idle", KAnim.PlayMode.Loop, 1f, 0f);
			this.acceptButtonRect.gameObject.SetActive(false);
			this.shieldMaskRect.sizeDelta = new Vector2(8f, 0f);
			this.shieldMaskRect.gameObject.SetActive(true);
		}
		if (firstItem)
		{
			this.closeButton.gameObject.SetActive(false);
			yield return Updater.WaitForSeconds(0.5f);
			yield return this.AnimateScreenInRoutine();
			yield return Updater.WaitForSeconds(0.125f);
			this.closeButton.gameObject.SetActive(true);
		}
		else
		{
			yield return Updater.WaitForSeconds(0.25f);
		}
		Vector2 animate_offset = new Vector2(0f, -30f);
		this.acceptButtonRect.FindOrAddComponent<CanvasGroup>().alpha = 0f;
		this.acceptButtonRect.gameObject.SetActive(true);
		this.acceptButtonPosition.SetOn(this.acceptButtonRect);
		yield return Updater.WaitForSeconds(0.75f);
		yield return PresUtil.OffsetToAndFade(this.acceptButton.rectTransform(), animate_offset, 1f, 0.125f, Easing.ExpoOut);
		yield return Updater.Until(() => this.noItemAvailableAcknowledged);
		yield return PresUtil.OffsetFromAndFade(this.acceptButton.rectTransform(), animate_offset, 0f, 0.125f, Easing.SmoothStep);
		this.Show(false);
		yield break;
	}

	private IEnumerator PresentItemRoutine(KleiItems.ItemData item, bool firstItem)
	{
		yield return null;
		if (item.ItemId == 0UL)
		{
			global::Debug.LogError("Could not find dropped item inventory.");
			yield break;
		}
		this.itemNameLabel.SetText("");
		this.itemDescriptionLabel.SetText("");
		this.itemRarityLabel.SetText("");
		this.itemCategoryLabel.SetText("");
		this.permitVisualizer.ResetState();
		if (firstItem)
		{
			this.animatedPod.Play("idle", KAnim.PlayMode.Loop, 1f, 0f);
			this.acceptButtonRect.gameObject.SetActive(false);
			this.shieldMaskRect.sizeDelta = new Vector2(8f, 0f);
			this.shieldMaskRect.gameObject.SetActive(true);
		}
		if (firstItem)
		{
			this.closeButton.gameObject.SetActive(false);
			yield return Updater.WaitForSeconds(0.5f);
			yield return this.AnimateScreenInRoutine();
			yield return Updater.WaitForSeconds(0.125f);
			this.closeButton.gameObject.SetActive(true);
		}
		else
		{
			yield return Updater.WaitForSeconds(0.25f);
		}
		Vector2 animate_offset = new Vector2(0f, -30f);
		this.acceptButtonRect.FindOrAddComponent<CanvasGroup>().alpha = 0f;
		this.acceptButtonRect.gameObject.SetActive(true);
		this.acceptButtonPosition.SetOn(this.acceptButtonRect);
		this.animatedPod.Play("powerup", KAnim.PlayMode.Once, 1f, 0f);
		this.animatedPod.Queue("working_loop", KAnim.PlayMode.Loop, 1f, 0f);
		yield return Updater.WaitForSeconds(1.25f);
		yield return PresUtil.OffsetToAndFade(this.acceptButton.rectTransform(), animate_offset, 1f, 0.125f, Easing.ExpoOut);
		yield return Updater.Until(() => this.giftRevealed);
		yield return PresUtil.OffsetFromAndFade(this.acceptButton.rectTransform(), animate_offset, 0f, 0.125f, Easing.SmoothStep);
		this.animatedPod.Play("additional_pre", KAnim.PlayMode.Once, 1f, 0f);
		this.animatedPod.Queue("working_loop", KAnim.PlayMode.Loop, 1f, 0f);
		yield return Updater.WaitForSeconds(1f);
		DropScreenPresentationInfo dropScreenPresentationInfo;
		dropScreenPresentationInfo.UseEquipmentVis = false;
		dropScreenPresentationInfo.BuildOverride = null;
		dropScreenPresentationInfo.Sprite = null;
		string name = "";
		string desc = "";
		PermitRarity rarity = PermitRarity.Unknown;
		string categoryString = "";
		string text;
		if (PermitItems.TryGetBoxInfo(item, out name, out desc, out text))
		{
			dropScreenPresentationInfo.UseEquipmentVis = false;
			dropScreenPresentationInfo.BuildOverride = null;
			dropScreenPresentationInfo.Sprite = Assets.GetSprite(text);
			rarity = PermitRarity.Loyalty;
		}
		else
		{
			PermitResource permitResource = Db.Get().Permits.Get(item.Id);
			dropScreenPresentationInfo.Sprite = permitResource.GetPermitPresentationInfo().sprite;
			dropScreenPresentationInfo.UseEquipmentVis = permitResource.Category == PermitCategory.Equipment;
			if (permitResource is EquippableFacadeResource)
			{
				dropScreenPresentationInfo.BuildOverride = (permitResource as EquippableFacadeResource).BuildOverride;
			}
			name = permitResource.Name;
			desc = permitResource.Description;
			rarity = permitResource.Rarity;
			PermitCategory category = permitResource.Category;
			if (category != PermitCategory.Building)
			{
				if (category != PermitCategory.Artwork)
				{
					if (category != PermitCategory.JoyResponse)
					{
						categoryString = PermitCategories.GetDisplayName(permitResource.Category);
					}
					else
					{
						categoryString = PermitCategories.GetDisplayName(permitResource.Category);
						if (permitResource is BalloonArtistFacadeResource)
						{
							categoryString = PermitCategories.GetDisplayName(permitResource.Category) + ": " + UI.KLEI_INVENTORY_SCREEN.CATEGORIES.JOY_RESPONSES.BALLOON_ARTIST;
						}
					}
				}
				else
				{
					categoryString = Assets.GetPrefab((permitResource as ArtableStage).prefabId).GetProperName();
				}
			}
			else
			{
				categoryString = Assets.GetPrefab((permitResource as BuildingFacadeResource).PrefabID).GetProperName();
			}
		}
		this.permitVisualizer.ConfigureWith(dropScreenPresentationInfo);
		yield return this.permitVisualizer.AnimateIn();
		KFMOD.PlayUISoundWithLabeledParameter(GlobalAssets.GetSound("GiftItemDrop_Rarity", false), "GiftItemRarity", string.Format("{0}", rarity));
		this.itemNameLabel.SetText(name);
		this.itemDescriptionLabel.SetText(desc);
		this.itemRarityLabel.SetText(rarity.GetLocStringName());
		this.itemCategoryLabel.SetText(categoryString);
		this.itemTextContainerPosition.SetOn(this.itemTextContainer);
		yield return Updater.Parallel(new Updater[] { PresUtil.OffsetToAndFade(this.itemTextContainer.rectTransform(), animate_offset, 1f, 0.125f, Easing.CircInOut) });
		yield return Updater.Until(() => this.giftAcknowledged);
		this.animatedPod.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
		this.animatedPod.Queue("idle", KAnim.PlayMode.Loop, 1f, 0f);
		yield return Updater.Parallel(new Updater[] { PresUtil.OffsetFromAndFade(this.itemTextContainer.rectTransform(), animate_offset, 0f, 0.125f, Easing.CircInOut) });
		this.itemNameLabel.SetText("");
		this.itemDescriptionLabel.SetText("");
		this.itemRarityLabel.SetText("");
		this.itemCategoryLabel.SetText("");
		yield return this.permitVisualizer.AnimateOut();
		name = null;
		desc = null;
		categoryString = null;
		this.PresentNextUnopenedItem(false);
		yield break;
	}

	public static bool HasItemsToShow()
	{
		return PermitItems.HasUnopenedItem();
	}

	[SerializeField]
	private RectTransform shieldMaskRect;

	[SerializeField]
	private KButton closeButton;

	[Header("Animated Item")]
	[SerializeField]
	private KleiItemDropScreen_PermitVis permitVisualizer;

	[SerializeField]
	private KBatchedAnimController animatedPod;

	[SerializeField]
	private LocText userMessageLabel;

	[Header("Item Info")]
	[SerializeField]
	private RectTransform itemTextContainer;

	[SerializeField]
	private LocText itemNameLabel;

	[SerializeField]
	private LocText itemDescriptionLabel;

	[SerializeField]
	private LocText itemRarityLabel;

	[SerializeField]
	private LocText itemCategoryLabel;

	[Header("Accept Button")]
	[SerializeField]
	private RectTransform acceptButtonRect;

	[SerializeField]
	private KButton acceptButton;

	[SerializeField]
	private KButton acknowledgeButton;

	private Coroutine activePresentationRoutine;

	private bool giftRevealed;

	private bool giftAcknowledged;

	private bool noItemAvailableAcknowledged;

	public static KleiItemDropScreen Instance;

	private bool shouldDoCloseRoutine;

	private const float TEXT_AND_BUTTON_ANIMATE_OFFSET_Y = -30f;

	private PrefabDefinedUIPosition acceptButtonPosition = new PrefabDefinedUIPosition();

	private PrefabDefinedUIPosition itemTextContainerPosition = new PrefabDefinedUIPosition();
}
