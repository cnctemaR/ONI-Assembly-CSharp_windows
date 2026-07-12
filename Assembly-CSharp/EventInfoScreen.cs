using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EventInfoScreen : KModalScreen
{
	public override bool IsModal()
	{
		return true;
	}

	public void SetEventData(EventInfoData data)
	{
		data.FinalizeText();
		this.eventHeader.text = data.title;
		this.eventDescriptionLabel.text = data.description;
		this.eventLocationLabel.text = data.location;
		this.eventTimeLabel.text = data.whenDescription;
		if (data.location.IsNullOrWhiteSpace() && data.location.IsNullOrWhiteSpace())
		{
			this.timeGroup.gameObject.SetActive(false);
		}
		if (data.options.Count == 0)
		{
			data.AddDefaultOption(null);
		}
		this.artSection.gameObject.SetActive(data.animFileName != HashedString.Invalid);
		this.SetEventDataOptions(data);
		this.SetEventDataVisuals(data);
	}

	private void SetEventDataOptions(EventInfoData data)
	{
		using (List<EventInfoData.Option>.Enumerator enumerator = data.options.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				EventInfoData.Option option = enumerator.Current;
				GameObject gameObject = global::Util.KInstantiateUI(this.optionPrefab, this.buttonsGroup, false);
				gameObject.name = "Option: " + option.mainText;
				KButton component = gameObject.GetComponent<KButton>();
				component.isInteractable = option.allowed;
				component.onClick += delegate
				{
					if (option.callback != null)
					{
						option.callback();
					}
					this.Deactivate();
				};
				if (!option.tooltip.IsNullOrWhiteSpace())
				{
					gameObject.GetComponent<ToolTip>().SetSimpleTooltip(option.tooltip);
				}
				else
				{
					gameObject.GetComponent<ToolTip>().enabled = false;
				}
				foreach (EventInfoData.OptionIcon optionIcon in option.informationIcons)
				{
					this.CreateOptionIcon(gameObject, optionIcon);
				}
				global::Util.KInstantiateUI(this.optionTextPrefab, gameObject, false).GetComponent<LocText>().text = ((option.description == null) ? ("<b>" + option.mainText + "</b>") : string.Concat(new string[] { "<b>", option.mainText, "</b>\n<i>(", option.description, ")</i>" }));
				foreach (EventInfoData.OptionIcon optionIcon2 in option.consequenceIcons)
				{
					this.CreateOptionIcon(gameObject, optionIcon2);
				}
				gameObject.SetActive(true);
			}
		}
	}

	public override void Deactivate()
	{
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().EventPopupSnapshot, STOP_MODE.ALLOWFADEOUT);
		base.Deactivate();
	}

	private void CreateOptionIcon(GameObject option, EventInfoData.OptionIcon optionIcon)
	{
		GameObject gameObject = global::Util.KInstantiateUI(this.optionIconPrefab, option, false);
		gameObject.GetComponent<ToolTip>().SetSimpleTooltip(optionIcon.tooltip);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		Image reference = component.GetReference<Image>("Mask");
		Image reference2 = component.GetReference<Image>("Border");
		Image reference3 = component.GetReference<Image>("Icon");
		if (optionIcon.sprite != null)
		{
			reference3.transform.localScale *= optionIcon.scale;
		}
		Color32 color = Color.white;
		switch (optionIcon.containerType)
		{
		case EventInfoData.OptionIcon.ContainerType.Neutral:
			reference.sprite = Assets.GetSprite("container_fill_neutral");
			reference2.sprite = Assets.GetSprite("container_border_neutral");
			if (optionIcon.sprite == null)
			{
				optionIcon.sprite = Assets.GetSprite("knob");
			}
			color = GlobalAssets.Instance.colorSet.eventNeutral;
			break;
		case EventInfoData.OptionIcon.ContainerType.Positive:
			reference.sprite = Assets.GetSprite("container_fill_positive");
			reference2.sprite = Assets.GetSprite("container_border_positive");
			reference3.rectTransform.localPosition += Vector3.down * 1f;
			if (optionIcon.sprite == null)
			{
				optionIcon.sprite = Assets.GetSprite("icon_positive");
			}
			color = GlobalAssets.Instance.colorSet.eventPositive;
			break;
		case EventInfoData.OptionIcon.ContainerType.Negative:
			reference.sprite = Assets.GetSprite("container_fill_negative");
			reference2.sprite = Assets.GetSprite("container_border_negative");
			reference3.rectTransform.localPosition += Vector3.up * 1f;
			color = GlobalAssets.Instance.colorSet.eventNegative;
			if (optionIcon.sprite == null)
			{
				optionIcon.sprite = Assets.GetSprite("cancel");
			}
			break;
		case EventInfoData.OptionIcon.ContainerType.Information:
			reference.sprite = Assets.GetSprite("requirements");
			reference2.enabled = false;
			break;
		}
		reference.color = color;
		reference3.sprite = optionIcon.sprite;
		if (optionIcon.sprite == null)
		{
			reference3.gameObject.SetActive(false);
		}
	}

	private void SetEventDataVisuals(EventInfoData data)
	{
		this.createdAnimations.ForEach(delegate(KBatchedAnimController x)
		{
			global::UnityEngine.Object.Destroy(x);
		});
		this.createdAnimations.Clear();
		KAnimFile anim = Assets.GetAnim(data.animFileName);
		if (anim == null)
		{
			global::Debug.LogWarning("Event " + data.title + " has no anim data");
			return;
		}
		KBatchedAnimController component = this.CreateAnimLayer(this.midgroundGroup, anim, data.mainAnim, null, null, null).transform.GetComponent<KBatchedAnimController>();
		if (data.minions != null)
		{
			for (int i = 0; i < data.minions.Length; i++)
			{
				if (data.minions[i] == null)
				{
					DebugUtil.LogWarningArgs(new object[] { string.Format("EventInfoScreen unable to display minion {0}", i) });
				}
				string text = string.Format("dupe{0:D2}", i + 1);
				if (component.HasAnimation(text))
				{
					this.CreateAnimLayer(this.midgroundGroup, anim, text, data.minions[i], null, null);
				}
			}
		}
		if (data.artifact != null)
		{
			string text2 = "artifact";
			if (component.HasAnimation(text2))
			{
				this.CreateAnimLayer(this.midgroundGroup, anim, text2, null, data.artifact, null);
			}
		}
	}

	private GameObject CreateAnimLayer(Transform parent, KAnimFile animFile, HashedString animName, GameObject minion = null, GameObject artifact = null, string targetSymbol = null)
	{
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.animPrefab, parent);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		this.createdAnimations.Add(component);
		if (minion != null)
		{
			component.AnimFiles = new KAnimFile[]
			{
				Assets.GetAnim("body_comp_default_kanim"),
				Assets.GetAnim("head_swap_kanim"),
				Assets.GetAnim("body_swap_kanim"),
				animFile
			};
		}
		else
		{
			component.AnimFiles = new KAnimFile[] { animFile };
		}
		gameObject.SetActive(true);
		if (minion != null)
		{
			if (this.loadMinionFromPersonalities)
			{
				component.GetComponent<UIDupeSymbolOverride>().Apply(minion.GetComponent<MinionIdentity>());
			}
			else
			{
				SymbolOverrideController component2 = component.GetComponent<SymbolOverrideController>();
				foreach (SymbolOverrideController.SymbolEntry symbolEntry in minion.GetComponent<SymbolOverrideController>().GetSymbolOverrides)
				{
					component2.AddSymbolOverride(symbolEntry.targetSymbol, symbolEntry.sourceSymbol, symbolEntry.priority);
				}
			}
			MinionConfig.CopyVisibleSymbols(gameObject, minion);
		}
		if (artifact != null)
		{
			SymbolOverrideController component3 = component.GetComponent<SymbolOverrideController>();
			KBatchedAnimController component4 = artifact.GetComponent<KBatchedAnimController>();
			string text = component4.initialAnim;
			text = text.Replace("idle_", "artifact_");
			text = text.Replace("_loop", "");
			KAnim.Build.Symbol symbol = component4.AnimFiles[0].GetData().build.GetSymbol(text);
			if (symbol != null)
			{
				component3.AddSymbolOverride("snapTo_artifact", symbol, 0);
			}
		}
		if (targetSymbol != null)
		{
			gameObject.AddOrGet<KBatchedAnimTracker>().symbol = targetSymbol;
		}
		component.Play(animName, KAnim.PlayMode.Loop, 1f, 0f);
		component.animScale = this.baseCharacterScale;
		return gameObject;
	}

	public static EventInfoScreen ShowPopup(EventInfoData eventInfoData)
	{
		EventInfoScreen eventInfoScreen = (EventInfoScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.eventInfoScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject);
		eventInfoScreen.SetEventData(eventInfoData);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().EventPopupSnapshot);
		if (eventInfoData.showCallback != null)
		{
			eventInfoData.showCallback();
		}
		if (eventInfoData.clickFocus != null)
		{
			WorldContainer myWorld = eventInfoData.clickFocus.gameObject.GetMyWorld();
			if (myWorld != null && myWorld.IsDiscovered)
			{
				CameraController.Instance.ActiveWorldStarWipe(myWorld.id, eventInfoData.clickFocus.position, 10f, null);
			}
		}
		return eventInfoScreen;
	}

	public static Notification CreateNotification(EventInfoData eventInfoData, Notification.ClickCallback clickCallback = null)
	{
		if (eventInfoData == null)
		{
			DebugUtil.LogWarningArgs(new object[] { "eventPopup is null in CreateStandardEventNotification" });
			return null;
		}
		eventInfoData.FinalizeText();
		Notification notification = new Notification(eventInfoData.title, NotificationType.Event, null, null, false, 0f, null, null, eventInfoData.clickFocus, true, false, false);
		if (clickCallback == null)
		{
			notification.customClickCallback = delegate(object data)
			{
				EventInfoScreen.ShowPopup(eventInfoData);
			};
		}
		else
		{
			notification.customClickCallback = clickCallback;
		}
		return notification;
	}

	[SerializeField]
	private float baseCharacterScale = 0.0057f;

	[FormerlySerializedAs("midgroundPrefab")]
	[FormerlySerializedAs("mid")]
	[Header("Prefabs")]
	[SerializeField]
	private GameObject animPrefab;

	[SerializeField]
	private GameObject optionPrefab;

	[SerializeField]
	private GameObject optionIconPrefab;

	[SerializeField]
	private GameObject optionTextPrefab;

	[Header("Groups")]
	[SerializeField]
	private Transform artSection;

	[SerializeField]
	private Transform midgroundGroup;

	[SerializeField]
	private GameObject timeGroup;

	[SerializeField]
	private GameObject buttonsGroup;

	[SerializeField]
	private GameObject chainGroup;

	[Header("Text")]
	[SerializeField]
	private LocText eventHeader;

	[SerializeField]
	private LocText eventTimeLabel;

	[SerializeField]
	private LocText eventLocationLabel;

	[SerializeField]
	private LocText eventDescriptionLabel;

	[SerializeField]
	private bool loadMinionFromPersonalities = true;

	[SerializeField]
	private LocText chainCount;

	[Header("Button Colour Styles")]
	[SerializeField]
	private ColorStyleSetting neutralButtonSetting;

	[SerializeField]
	private ColorStyleSetting badButtonSetting;

	[SerializeField]
	private ColorStyleSetting goodButtonSetting;

	private List<KBatchedAnimController> createdAnimations = new List<KBatchedAnimController>();
}
