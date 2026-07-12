using System;
using System.Collections;
using Database;
using UnityEngine;

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
		if (show)
		{
			base.Show(true);
			return;
		}
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
			return;
		}
		base.Show(false);
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
		if (KleiItems.InventoryData.AllItems != null)
		{
			this.PresentNextUnopenedItem(true);
			this.shouldDoCloseRoutine = true;
			return;
		}
		base.Show(false);
	}

	public void PresentNextUnopenedItem(bool firstItemPresentation = true)
	{
		foreach (KleiItems.Item item in KleiItems.InventoryData.AllItems)
		{
			if (!item.IsOpened)
			{
				this.PresentItem(item, firstItemPresentation);
				return;
			}
		}
		this.Show(false);
	}

	public void PresentItem(KleiItems.Item item, bool firstItemPresentation)
	{
		this.giftRevealed = false;
		this.giftAcknowledged = false;
		this.activePresentationRoutine = base.StartCoroutine(this.PresentItemRoutine(item, firstItemPresentation));
		this.acceptButton.ClearOnClick();
		this.acknowledgeButton.ClearOnClick();
		this.acceptButton.onClick += delegate
		{
			this.giftRevealed = true;
		};
		this.acknowledgeButton.onClick += delegate
		{
			if (this.giftRevealed)
			{
				this.giftAcknowledged = true;
			}
		};
	}

	private IEnumerator AnimateScreenInRoutine()
	{
		KFMOD.PlayUISound(GlobalAssets.GetSound("GiftItemDrop_Screen_Open", false));
		yield return Updater.Ease(delegate(Vector2 v2)
		{
			this.shieldMaskRect.sizeDelta = v2;
		}, this.shieldMaskRect.sizeDelta, new Vector2(this.shieldMaskRect.sizeDelta.x, 720f), 0.5f, Easing.CircInOut);
		yield return Updater.Ease(delegate(Vector2 v2)
		{
			this.shieldMaskRect.sizeDelta = v2;
		}, this.shieldMaskRect.sizeDelta, new Vector2(1152f, this.shieldMaskRect.sizeDelta.y), 0.25f, Easing.CircInOut);
		yield break;
	}

	private IEnumerator AnimateScreenOutRoutine()
	{
		KFMOD.PlayUISound(GlobalAssets.GetSound("GiftItemDrop_Screen_Close", false));
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

	private IEnumerator PresentItemRoutine(KleiItems.Item item, bool firstItem)
	{
		yield return null;
		if (item.ItemId == 0UL)
		{
			global::Debug.LogError("Could not find dropped item inventory.");
			yield break;
		}
		this.itemNameLabel.SetText("");
		this.itemDescriptionLabel.SetText("");
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
		PermitResource permitResource = Db.Get().Permits.Get(PermitItems.GetPermitIDByKleiItemType(item.ItemType));
		PermitPresentationInfo permitPresInfo = PermitItems.GetPermitPresentationInfo(permitResource.Id);
		this.permitVisualizer.ConfigureWith(permitResource);
		yield return this.permitVisualizer.AnimateIn();
		this.itemNameLabel.SetText(permitPresInfo.name);
		this.itemDescriptionLabel.SetText(permitPresInfo.description);
		this.itemNameLabelPosition.SetOn(this.itemNameLabel);
		this.itemDescriptionLabelPosition.SetOn(this.itemDescriptionLabel);
		yield return Updater.Parallel(new Updater[]
		{
			PresUtil.OffsetToAndFade(this.itemNameLabel.rectTransform(), animate_offset, 1f, 0.125f, Easing.CircInOut),
			PresUtil.OffsetToAndFade(this.itemDescriptionLabel.rectTransform(), animate_offset, 1f, 0.125f, Easing.CircInOut)
		});
		yield return Updater.Until(() => this.giftAcknowledged);
		this.animatedPod.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
		this.animatedPod.Queue("idle", KAnim.PlayMode.Loop, 1f, 0f);
		yield return Updater.Parallel(new Updater[]
		{
			PresUtil.OffsetFromAndFade(this.itemNameLabel.rectTransform(), animate_offset, 0f, 0.125f, Easing.CircInOut),
			PresUtil.OffsetFromAndFade(this.itemDescriptionLabel.rectTransform(), animate_offset, 0f, 0.125f, Easing.CircInOut)
		});
		this.itemNameLabel.SetText("");
		this.itemDescriptionLabel.SetText("");
		yield return this.permitVisualizer.AnimateOut();
		permitPresInfo = default(PermitPresentationInfo);
		this.PresentNextUnopenedItem(false);
		yield break;
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

	[Header("Item Info")]
	[SerializeField]
	private LocText itemNameLabel;

	[SerializeField]
	private LocText itemDescriptionLabel;

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

	public static KleiItemDropScreen Instance;

	private bool shouldDoCloseRoutine;

	private const float TEXT_AND_BUTTON_ANIMATE_OFFSET_Y = -30f;

	private PrefabDefinedUIPosition acceptButtonPosition = new PrefabDefinedUIPosition();

	private PrefabDefinedUIPosition itemNameLabelPosition = new PrefabDefinedUIPosition();

	private PrefabDefinedUIPosition itemDescriptionLabelPosition = new PrefabDefinedUIPosition();
}
