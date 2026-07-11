using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ResearchEntry : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.techLineMap = new Dictionary<Tech, UILineRenderer>();
		this.BG.color = this.defaultColor;
		foreach (Tech tech in this.targetTech.requiredTech)
		{
			float num = this.targetTech.width / 2f + 18f;
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			if (tech.center.y > this.targetTech.center.y + 2f)
			{
				zero = new Vector2(0f, 20f);
				zero2 = new Vector2(0f, -20f);
			}
			else if (tech.center.y < this.targetTech.center.y - 2f)
			{
				zero = new Vector2(0f, -20f);
				zero2 = new Vector2(0f, 20f);
			}
			GameObject gameObject = Util.KInstantiateUI(this.linePrefab, this.lineContainer.gameObject, true);
			UILineRenderer component = gameObject.GetComponent<UILineRenderer>();
			component.Points = new Vector2[]
			{
				new Vector2(0f, 0f) + zero,
				new Vector2(-((this.targetTech.center.x - num - (tech.center.x + num)) / 2f), 0f) + zero,
				new Vector2(-((this.targetTech.center.x - num - (tech.center.x + num)) / 2f), tech.center.y - this.targetTech.center.y) + zero2,
				new Vector2(-(this.targetTech.center.x - num - (tech.center.x + num)) + 2f, tech.center.y - this.targetTech.center.y) + zero2
			};
			component.LineThickness = (float)this.lineThickness_inactive;
			component.color = this.inactiveLineColor;
			this.techLineMap.Add(tech, component);
		}
		this.QueueStateChanged(false);
		if (this.targetTech != null)
		{
			foreach (TechInstance techInstance in Research.Instance.GetResearchQueue())
			{
				if (techInstance.tech == this.targetTech)
				{
					this.QueueStateChanged(true);
				}
			}
		}
	}

	public void SetTech(Tech newTech)
	{
		if (newTech == null)
		{
			global::Debug.LogError("The research provided is null!");
			return;
		}
		if (this.targetTech == newTech)
		{
			return;
		}
		foreach (ResearchType researchType in Research.Instance.researchTypes.Types)
		{
			if (newTech.costsByResearchTypeID.ContainsKey(researchType.id) && newTech.costsByResearchTypeID[researchType.id] > 0f)
			{
				GameObject gameObject = Util.KInstantiateUI(this.progressBarPrefab, this.progressBarContainer.gameObject, true);
				Image image = gameObject.GetComponentsInChildren<Image>()[2];
				Image component = gameObject.transform.Find("Icon").GetComponent<Image>();
				image.color = researchType.color;
				component.sprite = researchType.sprite;
				this.progressBarsByResearchTypeID[researchType.id] = gameObject;
			}
		}
		if (this.researchScreen == null)
		{
			this.researchScreen = base.transform.parent.GetComponentInParent<ResearchScreen>();
		}
		if (newTech.IsComplete())
		{
			this.ResearchCompleted(false);
		}
		this.targetTech = newTech;
		this.researchName.text = this.targetTech.Name;
		string text = string.Empty;
		foreach (TechItem techItem in this.targetTech.unlockedItems)
		{
			KImage componentInChildrenOnly = this.GetFreeIcon().GetComponentInChildrenOnly<KImage>();
			componentInChildrenOnly.transform.parent.gameObject.SetActive(true);
			if (text != string.Empty)
			{
				text += ", ";
			}
			text += techItem.Name;
			string text2 = string.Format("{0}\n{1}", techItem.Name, techItem.description);
			componentInChildrenOnly.GetComponent<ToolTip>().toolTip = text2;
			componentInChildrenOnly.sprite = techItem.UISprite();
		}
		text = string.Format(UI.RESEARCHSCREEN_UNLOCKSTOOLTIP, text);
		this.researchName.GetComponent<ToolTip>().toolTip = string.Format("{0}\n{1}\n\n{2}", this.targetTech.Name, this.targetTech.desc, text);
		this.toggle.ClearOnClick();
		this.toggle.onClick += this.OnResearchClicked;
		this.toggle.onPointerEnter += delegate
		{
			this.researchScreen.TurnEverythingOff();
			this.OnHover(true, this.targetTech);
		};
		this.toggle.soundPlayer.AcceptClickCondition = () => !this.targetTech.IsComplete();
		this.toggle.onPointerExit += delegate
		{
			this.researchScreen.TurnEverythingOff();
		};
	}

	public void SetEverythingOff()
	{
		if (!this.isOn)
		{
			return;
		}
		this.borderHighlight.gameObject.SetActive(false);
		foreach (KeyValuePair<Tech, UILineRenderer> keyValuePair in this.techLineMap)
		{
			keyValuePair.Value.LineThickness = (float)this.lineThickness_inactive;
			keyValuePair.Value.color = this.inactiveLineColor;
		}
		this.isOn = false;
	}

	public void SetEverythingOn()
	{
		if (this.isOn)
		{
			return;
		}
		this.UpdateProgressBars();
		this.borderHighlight.gameObject.SetActive(true);
		foreach (KeyValuePair<Tech, UILineRenderer> keyValuePair in this.techLineMap)
		{
			keyValuePair.Value.LineThickness = (float)this.lineThickness_active;
			keyValuePair.Value.color = this.activeLineColor;
		}
		base.transform.SetAsLastSibling();
		this.isOn = true;
	}

	private void OnHover(bool entered, Tech hoverSource)
	{
		this.SetEverythingOn();
		foreach (Tech tech in this.targetTech.requiredTech)
		{
			ResearchEntry entry = this.researchScreen.GetEntry(tech);
			if (entry != null)
			{
				entry.OnHover(entered, this.targetTech);
			}
		}
	}

	private void OnResearchClicked()
	{
		TechInstance activeResearch = Research.Instance.GetActiveResearch();
		if (activeResearch != null && activeResearch.tech != this.targetTech)
		{
			this.researchScreen.CancelResearch();
		}
		Research.Instance.SetActiveResearch(this.targetTech, true);
		if (DebugHandler.InstantBuildMode)
		{
			Research.Instance.CompleteQueue();
		}
		this.UpdateProgressBars();
	}

	private void OnResearchCanceled()
	{
		if (this.targetTech.IsComplete())
		{
			return;
		}
		this.toggle.ClearOnClick();
		this.toggle.onClick += this.OnResearchClicked;
		this.researchScreen.CancelResearch();
		Research.Instance.CancelResearch(this.targetTech, true);
	}

	public void QueueStateChanged(bool isSelected)
	{
		if (isSelected)
		{
			if (!this.targetTech.IsComplete())
			{
				this.toggle.isOn = true;
				this.BG.color = this.pendingColor;
				this.titleBG.color = this.pendingHeaderColor;
				this.toggle.ClearOnClick();
				this.toggle.onClick += this.OnResearchCanceled;
			}
			else
			{
				this.toggle.isOn = false;
			}
			foreach (KeyValuePair<string, GameObject> keyValuePair in this.progressBarsByResearchTypeID)
			{
				Transform child = keyValuePair.Value.transform.GetChild(0);
				child.GetComponentsInChildren<Image>()[1].color = Color.white;
			}
			foreach (Image image in this.iconPanel.GetComponentsInChildren<Image>())
			{
				image.material = this.StandardUIMaterial;
			}
		}
		else if (this.targetTech.IsComplete())
		{
			this.toggle.isOn = false;
			this.BG.color = this.completedColor;
			this.titleBG.color = this.completedHeaderColor;
			this.defaultColor = this.completedColor;
			this.toggle.ClearOnClick();
			foreach (KeyValuePair<string, GameObject> keyValuePair2 in this.progressBarsByResearchTypeID)
			{
				Transform child2 = keyValuePair2.Value.transform.GetChild(0);
				child2.GetComponentsInChildren<Image>()[1].color = Color.white;
			}
			foreach (Image image2 in this.iconPanel.GetComponentsInChildren<Image>())
			{
				image2.material = this.StandardUIMaterial;
			}
		}
		else
		{
			this.toggle.isOn = false;
			this.BG.color = this.defaultColor;
			this.titleBG.color = this.incompleteHeaderColor;
			this.toggle.ClearOnClick();
			this.toggle.onClick += this.OnResearchClicked;
			foreach (KeyValuePair<string, GameObject> keyValuePair3 in this.progressBarsByResearchTypeID)
			{
				Transform child3 = keyValuePair3.Value.transform.GetChild(0);
				child3.GetComponentsInChildren<Image>()[1].color = new Color(0.52156866f, 0.52156866f, 0.52156866f);
			}
		}
	}

	public void UpdateFilterState(string filter_string)
	{
		bool flag = false;
		if (!string.IsNullOrEmpty(filter_string))
		{
			string text = UI.StripLinkFormatting(this.researchName.text).ToLower();
			flag = text.Contains(filter_string);
			if (!flag)
			{
				foreach (TechItem techItem in this.targetTech.unlockedItems)
				{
					string text2 = UI.StripLinkFormatting(techItem.Name).ToLower();
					if (text2.Contains(filter_string))
					{
						flag = true;
						break;
					}
					string text3 = UI.StripLinkFormatting(techItem.description).ToLower();
					if (text3.Contains(filter_string))
					{
						flag = true;
						break;
					}
				}
			}
		}
		this.filterHighlight.gameObject.SetActive(flag);
		this.filterLowlight.gameObject.SetActive(!flag && !string.IsNullOrEmpty(filter_string));
	}

	public void SetPercentage(float percent)
	{
	}

	public void UpdateProgressBars()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.progressBarsByResearchTypeID)
		{
			Transform child = keyValuePair.Value.transform.GetChild(0);
			float num;
			if (this.targetTech.IsComplete())
			{
				num = 1f;
				child.GetComponentInChildren<LocText>().text = this.targetTech.costsByResearchTypeID[keyValuePair.Key] + "/" + this.targetTech.costsByResearchTypeID[keyValuePair.Key];
			}
			else
			{
				TechInstance orAdd = Research.Instance.GetOrAdd(this.targetTech);
				if (orAdd == null)
				{
					continue;
				}
				child.GetComponentInChildren<LocText>().text = orAdd.progressInventory.PointsByTypeID[keyValuePair.Key] + "/" + this.targetTech.costsByResearchTypeID[keyValuePair.Key];
				num = orAdd.progressInventory.PointsByTypeID[keyValuePair.Key] / this.targetTech.costsByResearchTypeID[keyValuePair.Key];
			}
			child.GetComponentsInChildren<Image>()[2].fillAmount = num;
			child.GetComponent<ToolTip>().SetSimpleTooltip(Research.Instance.researchTypes.GetResearchType(keyValuePair.Key).description);
		}
	}

	private GameObject GetFreeIcon()
	{
		return Util.KInstantiateUI(this.iconPrefab, this.iconPanel, false);
	}

	private Image GetFreeLine()
	{
		return Util.KInstantiateUI<Image>(this.linePrefab.gameObject, base.gameObject, false);
	}

	public void ResearchCompleted(bool notify = true)
	{
		this.BG.color = this.completedColor;
		this.titleBG.color = this.completedHeaderColor;
		this.defaultColor = this.completedColor;
		if (notify)
		{
			this.unlockedTechMetric[ResearchEntry.UnlockedTechKey] = this.targetTech.Id;
			ThreadedHttps<KleiMetrics>.Instance.SendEvent(this.unlockedTechMetric);
		}
		this.toggle.ClearOnClick();
		if (notify)
		{
			ResearchCompleteMessage researchCompleteMessage = new ResearchCompleteMessage(this.targetTech);
			MusicManager.instance.PlaySong("Stinger_ResearchComplete", false);
			Messenger.Instance.QueueMessage(researchCompleteMessage);
		}
	}

	[Header("Labels")]
	[SerializeField]
	private LocText researchName;

	[Header("Transforms")]
	[SerializeField]
	private Transform progressBarContainer;

	[SerializeField]
	private Transform lineContainer;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject iconPanel;

	[SerializeField]
	private GameObject iconPrefab;

	[SerializeField]
	private GameObject linePrefab;

	[SerializeField]
	private GameObject progressBarPrefab;

	[Header("Graphics")]
	[SerializeField]
	private Image BG;

	[SerializeField]
	private Image titleBG;

	[SerializeField]
	private Image borderHighlight;

	[SerializeField]
	private Image filterHighlight;

	[SerializeField]
	private Image filterLowlight;

	[SerializeField]
	private Sprite hoverBG;

	[SerializeField]
	private Sprite completedBG;

	[Header("Colors")]
	[SerializeField]
	private Color defaultColor = Color.blue;

	[SerializeField]
	private Color completedColor = Color.yellow;

	[SerializeField]
	private Color pendingColor = Color.magenta;

	[SerializeField]
	private Color completedHeaderColor = Color.grey;

	[SerializeField]
	private Color incompleteHeaderColor = Color.grey;

	[SerializeField]
	private Color pendingHeaderColor = Color.grey;

	private Sprite defaultBG;

	[MyCmpGet]
	private KToggle toggle;

	private ResearchScreen researchScreen;

	private Dictionary<Tech, UILineRenderer> techLineMap;

	private Tech targetTech;

	private bool isOn = true;

	private Coroutine fadeRoutine;

	public Color activeLineColor;

	public Color inactiveLineColor;

	public int lineThickness_active = 6;

	public int lineThickness_inactive = 2;

	public Material StandardUIMaterial;

	private Dictionary<string, GameObject> progressBarsByResearchTypeID = new Dictionary<string, GameObject>();

	public static readonly string UnlockedTechKey = "UnlockedTech";

	private Dictionary<string, object> unlockedTechMetric = new Dictionary<string, object> { 
	{
		ResearchEntry.UnlockedTechKey,
		null
	} };
}
