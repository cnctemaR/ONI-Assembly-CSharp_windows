using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class PrinterceptorScreen : KModalScreen
{
	public Tag selectedEntityTag { get; private set; }

	protected override void OnActivate()
	{
		PrinterceptorScreen.Instance = this;
		this.Show(false);
		this.closeButton.ClearOnClick();
		this.closeButton.onClick += delegate
		{
			this.Show(false);
		};
	}

	public void SetTarget(HijackedHeadquarters.Instance target)
	{
		this.target = target;
		this.printButton.ClearOnClick();
		this.printButton.onClick += delegate
		{
			target.Trigger(1816718186, null);
			this.Show(false);
		};
	}

	public override float GetSortKey()
	{
		return 40f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public override void Show(bool show = true)
	{
		base.Show(show);
		if (show)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndSupplyClosetSnapshot);
			MusicManager.instance.OnSupplyClosetMenu(true, 0.5f);
			MusicManager.instance.PlaySong("Music_SupplyCloset", false);
			Image[] array = this.dataWalletIcon;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].sprite = Def.GetUISprite(DatabankHelper.ID, "ui", false).first;
			}
			this.dataWalletLabel.SetText(GameUtil.SafeStringFormat(UI.PRINTERCEPTORSCREEN.DATABANKS_AVAILABLE, new object[] { this.target.GetComponent<Storage>().GetAmountAvailable(DatabankHelper.ID).ToString() }));
			this.SelectEntity(this.selectedEntityTag);
			using (Dictionary<Tag, MultiToggle>.Enumerator enumerator = this.optionButtons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<Tag, MultiToggle> keyValuePair = enumerator.Current;
					this.RefreshOptionButton(keyValuePair.Key);
				}
				return;
			}
		}
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSupplyClosetSnapshot, STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.OnSupplyClosetMenu(false, 1f);
		if (MusicManager.instance.SongIsPlaying("Music_SupplyCloset"))
		{
			MusicManager.instance.StopSong("Music_SupplyCloset", true, STOP_MODE.ALLOWFADEOUT);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.SpawnOptionButtons();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.Show(false);
			return;
		}
		base.OnKeyDown(e);
	}

	public override void Deactivate()
	{
		this.Show(false);
	}

	private void SpawnOptionButtons()
	{
		foreach (KeyValuePair<Tag, List<EggCrackerConfig.EggData>> keyValuePair in EggCrackerConfig.EggsBySpecies)
		{
			foreach (EggCrackerConfig.EggData eggData in keyValuePair.Value)
			{
				if (eggData.isBaseMorph)
				{
					this.SpawnOptionButton(eggData.id);
				}
			}
		}
		List<Tag> list = new List<Tag>();
		list.AddRange(from x in Assets.GetPrefabsWithTag(GameTags.Seed)
			select x.GetComponent<KPrefabID>().PrefabTag);
		list.AddRange(from x in Assets.GetPrefabsWithTag(GameTags.CropSeed)
			select x.GetComponent<KPrefabID>().PrefabTag);
		foreach (Tag tag in list)
		{
			this.SpawnOptionButton(tag);
		}
		this.SelectEntity("SquirrelEgg");
		this.SpawnOptionButton("BeeBaby");
	}

	private void SpawnOptionButton(Tag id)
	{
		if (this.optionButtons.ContainsKey(id))
		{
			return;
		}
		GameObject prefab = Assets.GetPrefab(id);
		if (prefab == null)
		{
			return;
		}
		if (!Game.IsCorrectDlcActiveForCurrentSave(prefab.GetComponent<KPrefabID>()))
		{
			return;
		}
		if (prefab.HasTag(GameTags.DeprecatedContent))
		{
			return;
		}
		PlantableSeed component = prefab.GetComponent<PlantableSeed>();
		if (component != null)
		{
			GameObject prefab2 = Assets.GetPrefab(component.PlantID);
			if (prefab2 != null && prefab2.HasTag(GameTags.DeprecatedContent))
			{
				return;
			}
		}
		GameObject gameObject = global::Util.KInstantiateUI(this.optionButtonPrefab, this.optionGridContainer.gameObject, true);
		MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
		this.optionButtons.Add(id, component2);
		HierarchyReferences component3 = gameObject.GetComponent<HierarchyReferences>();
		MultiToggle multiToggle = component2;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.SelectEntity(id);
		}));
		component3.GetReference<Image>("FGIcon").sprite = Def.GetUISprite(id, "ui", false).first;
		component3.GetReference<LocText>("NameLabel").text = id.ProperName();
		component3.GetReference<Image>("ProgressOverlay").fillAmount = 0f;
		component3.GetReference<LocText>("CostLabel").text = HijackedHeadquartersConfig.GetDataBankCost(id, this.GetPrintCount(id)).ToString();
		component3.GetReference<Image>("CostIcon").sprite = Def.GetUISprite(DatabankHelper.ID, "ui", false).first;
	}

	private void RefreshOptionButton(Tag id)
	{
		this.optionButtons[id].GetComponent<HierarchyReferences>().GetReference<LocText>("CostLabel").text = HijackedHeadquartersConfig.GetDataBankCost(id, this.GetPrintCount(id)).ToString();
	}

	private void SelectEntity(Tag id)
	{
		this.selectedEntityTag = id;
		GameObject prefab = Assets.GetPrefab(this.selectedEntityTag);
		this.selectedEffectsText.text = prefab.GetComponent<InfoDescription>().description;
		this.selectedTitleText.text = prefab.GetProperName();
		this.selectedIcon.sprite = Def.GetUISprite(this.selectedEntityTag, "ui", false).first;
		if (prefab.HasTag(GameTags.Egg))
		{
			Tag spawnedCreature = prefab.GetDef<IncubationMonitor.Def>().spawnedCreature;
			this.selectedIconAlt.sprite = Def.GetUISprite(spawnedCreature, "ui", false).first;
		}
		else if (prefab.HasTag(GameTags.Seed))
		{
			this.selectedIconAlt.sprite = Def.GetUISprite(prefab.GetComponent<PlantableSeed>().PlantID, "ui", false).first;
		}
		else if (prefab.HasTag(GameTags.CropSeed))
		{
			this.selectedIconAlt.sprite = Def.GetUISprite(prefab.GetComponent<PlantableSeed>().PlantID, "ui", false).first;
		}
		else if (prefab.HasTag(GameTags.Creature))
		{
			CreatureBrain component = prefab.GetComponent<CreatureBrain>();
			this.selectedIconAlt.sprite = Def.GetUISprite(component.species, "ui", false).first;
		}
		else
		{
			this.selectedIconAlt.sprite = null;
		}
		foreach (KeyValuePair<Tag, MultiToggle> keyValuePair in this.optionButtons)
		{
			keyValuePair.Value.GetComponent<MultiToggle>().ChangeState((this.selectedEntityTag == keyValuePair.Key) ? 1 : 0);
		}
		this.selectedCostIcon.sprite = Def.GetUISprite(DatabankHelper.ID, "ui", false).first;
		this.selectedCostLabel.SetText(GameUtil.SafeStringFormat(UI.PRINTERCEPTORSCREEN.DATABANKS_COST, new object[] { HijackedHeadquartersConfig.GetDataBankCost(this.selectedEntityTag, this.GetPrintCount(this.selectedEntityTag)).ToString() }));
		this.printButton.isInteractable = this.target != null && this.target.GetComponent<Storage>().GetAmountAvailable(DatabankHelper.ID) >= (float)HijackedHeadquartersConfig.GetDataBankCost(this.selectedEntityTag, this.GetPrintCount(this.selectedEntityTag));
		this.printButton.GetComponent<ToolTip>().SetSimpleTooltip(this.printButton.isInteractable ? GameUtil.SafeStringFormat(UI.PRINTERCEPTORSCREEN.PRINT_TOOLTIP, new object[] { 25 }) : UI.PRINTERCEPTORSCREEN.PRINT_TOOLTIP_DISABLED);
	}

	private int GetPrintCount(Tag id)
	{
		if (this.target == null || !this.target.printCounts.ContainsKey(id))
		{
			return 0;
		}
		return this.target.printCounts[id];
	}

	public static PrinterceptorScreen Instance;

	[SerializeField]
	private RectTransform optionGridContainer;

	[SerializeField]
	private GameObject optionButtonPrefab;

	[SerializeField]
	private LocText selectedTitleText;

	[SerializeField]
	private Image selectedIcon;

	[SerializeField]
	private Image selectedIconAlt;

	[SerializeField]
	private LocText selectedEffectsText;

	[SerializeField]
	private LocText selectedFlavourText;

	[SerializeField]
	private KButton printButton;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private LocText dataWalletLabel;

	[SerializeField]
	private Image[] dataWalletIcon;

	[SerializeField]
	private LocText selectedCostLabel;

	[SerializeField]
	private Image selectedCostIcon;

	private const string LOCKER_MENU_MUSIC = "Music_SupplyCloset";

	private const string MUSIC_PARAMETER = "SupplyClosetView";

	[SerializeField]
	private Material desatUIMaterial;

	private HijackedHeadquarters.Instance target;

	private Dictionary<Tag, MultiToggle> optionButtons = new Dictionary<Tag, MultiToggle>();
}
