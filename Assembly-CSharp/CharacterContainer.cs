using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContainer : KScreen
{
	public MinionStartingStats Stats
	{
		get
		{
			return this.stats;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Initialize();
		this.characterNameTitle.OnStartedEditing += this.OnStartedEditing;
		this.characterNameTitle.OnNameChanged += this.OnNameChanged;
		this.reshuffleButton.onClick += delegate
		{
			this.Reshuffle(true);
		};
		base.StartCoroutine(this.DelayedGeneration());
	}

	private IEnumerator DelayedGeneration()
	{
		yield return new WaitForEndOfFrame();
		this.GenerateCharacter(this.controller.IsStarterMinion);
		yield break;
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (this.animController != null)
		{
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (global::System.Action)Delegate.Remove(instance.OnResize, new global::System.Action(this.OnResize));
			this.animController.gameObject.DeleteObject();
			this.animController = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.controller != null)
		{
			CharacterSelectionController characterSelectionController = this.controller;
			characterSelectionController.OnLimitReachedEvent = (global::System.Action)Delegate.Remove(characterSelectionController.OnLimitReachedEvent, new global::System.Action(this.OnCharacterSelectionLimitReached));
			CharacterSelectionController characterSelectionController2 = this.controller;
			characterSelectionController2.OnLimitUnreachedEvent = (global::System.Action)Delegate.Remove(characterSelectionController2.OnLimitUnreachedEvent, new global::System.Action(this.OnCharacterSelectionLimitUnReached));
			CharacterSelectionController characterSelectionController3 = this.controller;
			characterSelectionController3.OnReshuffleEvent = (Action<bool>)Delegate.Remove(characterSelectionController3.OnReshuffleEvent, new Action<bool>(this.Reshuffle));
		}
		if (this.animController != null)
		{
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (global::System.Action)Delegate.Remove(instance.OnResize, new global::System.Action(this.OnResize));
		}
	}

	private void Initialize()
	{
		this.professionIconMap = new Dictionary<string, Sprite>();
		this.professionIcons.ForEach(delegate(CharacterContainer.ProfessionIcon ic)
		{
			this.professionIconMap.Add(ic.professionName, ic.iconImg);
		});
		this.iconGroups = new List<GameObject>();
		this.traitLabels = new List<LocText>();
		this.expectationLabels = new List<LocText>();
		if (CharacterContainer.containers == null)
		{
			CharacterContainer.containers = new List<CharacterContainer>();
		}
		CharacterContainer.containers.Add(this);
	}

	private void OnNameChanged(string newName)
	{
		this.stats.Name = newName;
		this.stats.personality.Name = newName;
		this.description.text = this.stats.personality.description;
	}

	private void OnStartedEditing()
	{
		KScreenManager.Instance.RefreshStack();
	}

	private void GenerateCharacter(bool is_starter)
	{
		int num = 0;
		do
		{
			this.stats = new MinionStartingStats(is_starter);
			num++;
		}
		while (this.IsCharacterRedundant() && num < 20);
		if (this.animController != null)
		{
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (global::System.Action)Delegate.Remove(instance.OnResize, new global::System.Action(this.OnResize));
			global::UnityEngine.Object.Destroy(this.animController.gameObject);
			this.animController = null;
		}
		this.SetAnimator();
		this.SetInfoText();
		base.StartCoroutine(this.SetAttributes());
		this.selectButton.ClearOnClick();
		if (!this.controller.IsStarterMinion)
		{
			this.selectButton.onClick += delegate
			{
				this.SelectCharacter();
			};
		}
	}

	private void OnResize()
	{
		KCanvasScaler kcanvasScaler = global::UnityEngine.Object.FindObjectOfType<KCanvasScaler>();
		this.animController.animScale = this.baseCharacterScale * (1f / kcanvasScaler.GetCanvasScale());
	}

	private void SetAnimator()
	{
		if (this.animController == null)
		{
			this.animController = Util.KInstantiateUI(EntityPrefabs.Instance.MinionSelectPreview, this.contentBody.gameObject, false).GetComponent<KBatchedAnimController>();
			this.animController.GetComponent<MinionIdentity>().addToIdentityList = this.addMinionToIdentityList;
			KCanvasScaler kcanvasScaler = global::UnityEngine.Object.FindObjectOfType<KCanvasScaler>();
			this.animController.animScale = this.baseCharacterScale * (1f / kcanvasScaler.GetCanvasScale());
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (global::System.Action)Delegate.Combine(instance.OnResize, new global::System.Action(this.OnResize));
		}
		this.stats.ApplyTraits(this.animController.gameObject);
		this.stats.ApplyRace(this.animController.gameObject);
		this.stats.ApplyAccessories(this.animController.gameObject);
		this.stats.ApplyExperience(this.animController.gameObject);
		DecorNeed decorNeed = this.animController.gameObject.AddComponent<DecorNeed>();
		decorNeed.skipUpdate = true;
		this.animController.gameObject.AddComponent<FoodQualityNeed>();
		HashedString hashedString = CharacterContainer.idleAnims[global::UnityEngine.Random.Range(0, CharacterContainer.idleAnims.Length)];
		this.idle_anim = Assets.GetAnim(hashedString);
		if (this.idle_anim != null)
		{
			this.animController.AddAnimOverrides(this.idle_anim, 0f);
		}
		this.animController.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
	}

	private void SetInfoText()
	{
		this.traitLabels.ForEach(delegate(LocText tl)
		{
			global::UnityEngine.Object.Destroy(tl.gameObject);
		});
		this.traitLabels.Clear();
		this.characterNameTitle.SetTitle(this.stats.Name);
		string professionString = this.animController.gameObject.GetAttributes().GetProfessionString();
		this.characterJob.text = professionString;
		string professionDescriptionString = this.animController.gameObject.GetAttributes().GetProfessionDescriptionString();
		this.characterJob.GetComponent<ToolTip>().toolTip = professionDescriptionString;
		for (int i = 1; i < this.stats.Traits.Count; i++)
		{
			Trait trait = this.stats.Traits[i];
			LocText locText = ((!trait.PositiveTrait) ? this.badTrait : this.goodTrait);
			LocText locText2 = Util.KInstantiateUI<LocText>(locText.gameObject, locText.transform.parent.gameObject, false);
			locText2.gameObject.SetActive(true);
			locText2.text = this.stats.Traits[i].Name;
			locText2.color = ((!trait.PositiveTrait) ? Constants.NEGATIVE_COLOR : Constants.POSITIVE_COLOR);
			locText2.GetComponent<ToolTip>().SetSimpleTooltip(trait.GetTooltip());
			this.traitLabels.Add(locText2);
		}
		this.expectationLabels.ForEach(delegate(LocText el)
		{
			global::UnityEngine.Object.Destroy(el.gameObject);
		});
		this.expectationLabels.Clear();
		foreach (Need need in this.animController.gameObject.GetComponents<Need>())
		{
			LocText locText3 = Util.KInstantiateUI<LocText>(this.expectation.gameObject, this.expectation.transform.parent.gameObject, false);
			locText3.gameObject.SetActive(true);
			AttributeInstance attributeInstance = need.GetExpectationAttribute().Lookup(this.animController);
			locText3.text = string.Format(UI.CHARACTERCONTAINER_NEED, need.Name, attributeInstance.GetFormattedValue(false));
			this.expectationLabels.Add(locText3);
			string text = attributeInstance.GetAttributeValueTooltip();
			text += UI.TOOLTIPS.TOOLTIP_SEPERATOR;
			text += need.ExpectationTooltip;
			locText3.GetComponent<ToolTip>().SetSimpleTooltip(text);
		}
		if (this.stats.stressTrait != null)
		{
			LocText locText4 = Util.KInstantiateUI<LocText>(this.expectationRight.gameObject, this.expectationRight.transform.parent.gameObject, false);
			locText4.gameObject.SetActive(true);
			locText4.text = string.Format(UI.CHARACTERCONTAINER_STRESSTRAIT, this.stats.stressTrait.Name);
			locText4.GetComponent<ToolTip>().SetSimpleTooltip(this.stats.stressTrait.GetTooltip());
			this.expectationLabels.Add(locText4);
		}
		if (this.stats.congenitaltrait != null)
		{
			LocText locText5 = Util.KInstantiateUI<LocText>(this.expectationRight.gameObject, this.expectationRight.transform.parent.gameObject, false);
			locText5.gameObject.SetActive(true);
			locText5.text = string.Format(UI.CHARACTERCONTAINER_CONGENITALTRAIT, this.stats.congenitaltrait.Name);
			locText5.GetComponent<ToolTip>().SetSimpleTooltip(this.stats.congenitaltrait.GetTooltip());
			this.expectationLabels.Add(locText5);
		}
		this.description.text = this.stats.personality.description;
	}

	private IEnumerator SetAttributes()
	{
		yield return null;
		this.iconGroups.ForEach(delegate(GameObject icg)
		{
			global::UnityEngine.Object.Destroy(icg);
		});
		this.iconGroups.Clear();
		Attributes attr = this.animController.gameObject.GetAttributes();
		List<AttributeInstance> attributes = new List<AttributeInstance>(attr.AttributeTable);
		attributes.RemoveAll((AttributeInstance at) => at.Attribute.ShowInUI != Klei.AI.Attribute.Display.Skill);
		attributes = attributes.OrderBy<AttributeInstance, string>((AttributeInstance at) => at.Name).ToList<AttributeInstance>();
		for (int i = 0; i < attributes.Count; i++)
		{
			GameObject newIconGroup = Util.KInstantiateUI(this.iconGroup.gameObject, this.iconGroup.transform.parent.gameObject, false);
			LocText label = newIconGroup.GetComponentInChildren<LocText>();
			newIconGroup.SetActive(true);
			float totalValue = attributes[i].GetTotalValue();
			if (totalValue > 0f)
			{
				label.color = Constants.POSITIVE_COLOR;
			}
			else if (totalValue == 0f)
			{
				label.color = Constants.NEUTRAL_COLOR;
			}
			else
			{
				label.color = Constants.NEGATIVE_COLOR;
			}
			label.text = string.Format(UI.CHARACTERCONTAINER_SKILL_VALUE, GameUtil.AddPositiveSign(totalValue.ToString(), totalValue > 0f), attributes[i].Name);
			AttributeInstance attribute = attributes[i];
			string tooltip = attribute.Description;
			if (attribute.Attribute.converters.Count > 0)
			{
				tooltip += "\n";
				foreach (AttributeConverter converter in attribute.Attribute.converters)
				{
					AttributeConverterInstance converter_instance = this.animController.gameObject.GetComponent<AttributeConverters>().GetConverter(converter.Id);
					tooltip = tooltip + "\n" + converter_instance.ToString();
				}
			}
			newIconGroup.GetComponent<ToolTip>().SetSimpleTooltip(tooltip);
			this.iconGroups.Add(newIconGroup);
		}
		yield break;
	}

	private void SelectCharacter()
	{
		if (this.controller != null)
		{
			this.controller.AddCharacter(this.stats);
		}
		if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
		{
			MusicManager.instance.SetSongParameter("Music_SelectDuplicant", "songSection", 1f, true);
		}
		this.selectButton.GetComponent<ImageToggleState>().SetActive();
		this.selectButton.ClearOnClick();
		this.selectButton.onClick += delegate
		{
			this.DeselectCharacter();
			if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
			{
				MusicManager.instance.SetSongParameter("Music_SelectDuplicant", "songSection", 0f, true);
			}
		};
		this.selectedBorder.SetActive(true);
		this.titleBar.color = this.selectedTitleColor;
		if (!this.animController.HasAnimation("cheer_pre"))
		{
			this.animController.AddAnims(new KAnimFile[] { this.cheer_anim_file });
		}
		this.animController.Play("cheer_pre", KAnim.PlayMode.Once, 1f, 0f);
		this.animController.Play("cheer_loop", KAnim.PlayMode.Loop, 1f, 0f);
	}

	private void DeselectCharacter()
	{
		if (this.controller != null)
		{
			this.controller.RemoveCharacter(this.stats);
		}
		this.selectButton.GetComponent<ImageToggleState>().SetInactive();
		this.selectButton.Deselect();
		this.selectButton.ClearOnClick();
		this.selectButton.onClick += delegate
		{
			this.SelectCharacter();
		};
		this.selectedBorder.SetActive(false);
		this.titleBar.color = this.deselectedTitleColor;
		this.animController.Queue("cheer_pst", KAnim.PlayMode.Once, 1f, 0f);
		this.animController.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
	}

	private void OnReplacedEvent(MinionStartingStats stats)
	{
		if (stats == this.stats)
		{
			this.DeselectCharacter();
		}
	}

	private void OnCharacterSelectionLimitReached()
	{
		if (this.controller != null && this.controller.IsSelected(this.stats))
		{
			return;
		}
		this.selectButton.ClearOnClick();
		if (this.controller.AllowsReplacing)
		{
			this.selectButton.onClick += this.ReplaceCharacterSelection;
		}
		else
		{
			this.selectButton.onClick += this.CantSelectCharacter;
		}
	}

	private void CantSelectCharacter()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
	}

	private void ReplaceCharacterSelection()
	{
		if (this.controller == null)
		{
			return;
		}
		this.controller.RemoveLast();
		this.SelectCharacter();
	}

	private void OnCharacterSelectionLimitUnReached()
	{
		if (this.controller != null && this.controller.IsSelected(this.stats))
		{
			return;
		}
		this.selectButton.ClearOnClick();
		this.selectButton.onClick += delegate
		{
			this.SelectCharacter();
		};
	}

	public void SetReshufflingState(bool enable)
	{
		this.reshuffleButton.gameObject.SetActive(enable);
	}

	private void Reshuffle(bool is_starter)
	{
		if (this.controller != null && this.controller.IsSelected(this.stats))
		{
			this.DeselectCharacter();
		}
		this.GenerateCharacter(is_starter);
	}

	public void SetController(CharacterSelectionController csc)
	{
		if (csc == this.controller)
		{
			return;
		}
		this.controller = csc;
		CharacterSelectionController characterSelectionController = this.controller;
		characterSelectionController.OnLimitReachedEvent = (global::System.Action)Delegate.Combine(characterSelectionController.OnLimitReachedEvent, new global::System.Action(this.OnCharacterSelectionLimitReached));
		CharacterSelectionController characterSelectionController2 = this.controller;
		characterSelectionController2.OnLimitUnreachedEvent = (global::System.Action)Delegate.Combine(characterSelectionController2.OnLimitUnreachedEvent, new global::System.Action(this.OnCharacterSelectionLimitUnReached));
		CharacterSelectionController characterSelectionController3 = this.controller;
		characterSelectionController3.OnReshuffleEvent = (Action<bool>)Delegate.Combine(characterSelectionController3.OnReshuffleEvent, new Action<bool>(this.Reshuffle));
		CharacterSelectionController characterSelectionController4 = this.controller;
		characterSelectionController4.OnReplacedEvent = (Action<MinionStartingStats>)Delegate.Combine(characterSelectionController4.OnReplacedEvent, new Action<MinionStartingStats>(this.OnReplacedEvent));
	}

	public void DisableSelectButton()
	{
		this.selectButton.soundPlayer.AcceptClickCondition = () => false;
		this.selectButton.GetComponent<ImageToggleState>().SetDisabled();
		this.selectButton.soundPlayer.Enabled = false;
	}

	private bool IsCharacterRedundant()
	{
		return CharacterContainer.containers.Find((CharacterContainer c) => c != null && c.stats != null && c != this && c.stats.Name == this.stats.Name) != null || Components.LiveMinionIdentities.Any<MinionIdentity>((MinionIdentity id) => id.GetProperName() == this.stats.Name);
	}

	public string GetValueColor(bool isPositive)
	{
		return (!isPositive) ? "<color=#ff2222ff>" : "<color=green>";
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		e.Consumed = true;
	}

	protected override void OnCmpEnable()
	{
		base.OnActivate();
		if (this.stats == null)
		{
			return;
		}
		this.SetAnimator();
	}

	[SerializeField]
	private GameObject contentBody;

	[SerializeField]
	private LocText characterName;

	[SerializeField]
	private EditableTitleBar characterNameTitle;

	[SerializeField]
	private LocText characterJob;

	public KAnimFile cheer_anim_file;

	public GameObject selectedBorder;

	[SerializeField]
	private Image titleBar;

	[SerializeField]
	private Color selectedTitleColor;

	[SerializeField]
	private Color deselectedTitleColor;

	[SerializeField]
	private KButton reshuffleButton;

	private KBatchedAnimController animController;

	[SerializeField]
	private GameObject iconGroup;

	private List<GameObject> iconGroups;

	[SerializeField]
	private LocText goodTrait;

	[SerializeField]
	private LocText badTrait;

	[SerializeField]
	private LocText expectation;

	[SerializeField]
	private LocText expectationRight;

	private List<LocText> expectationLabels;

	private List<LocText> traitLabels;

	[SerializeField]
	private LocText description;

	[SerializeField]
	private KToggle selectButton;

	private MinionStartingStats stats;

	private CharacterSelectionController controller;

	private static List<CharacterContainer> containers;

	private KAnimFile idle_anim;

	[HideInInspector]
	public bool addMinionToIdentityList = true;

	[SerializeField]
	private Sprite enabledSpr;

	[SerializeField]
	private List<CharacterContainer.ProfessionIcon> professionIcons;

	private Dictionary<string, Sprite> professionIconMap;

	private static HashedString[] idleAnims = new HashedString[] { "anim_idle_healthy_kanim", "anim_idle_susceptible_kanim", "anim_idle_keener_kanim", "anim_idle_coaster_kanim", "anim_idle_fastfeet_kanim", "anim_idle_breatherdeep_kanim", "anim_idle_breathershallow_kanim" };

	public float baseCharacterScale = 0.38f;

	[Serializable]
	public struct ProfessionIcon
	{
		public string professionName;

		public Sprite iconImg;
	}
}
