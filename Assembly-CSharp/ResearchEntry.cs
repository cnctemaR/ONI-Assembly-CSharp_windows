using System;
using System.Collections;
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
		List<Image> list = new List<Image>(base.GetComponentsInChildren<Image>());
		this.originalImgColors.Clear();
		list.ForEach(delegate(Image img)
		{
			this.originalImgColors.Add(img, img.color);
		});
		foreach (Tech tech in this.targetTech.unlockedTech)
		{
			GameObject gameObject = Util.KInstantiateUI(this.linePrefab, this.lineContainer.gameObject, true);
			UILineRenderer component = gameObject.GetComponent<UILineRenderer>();
			component.Points = new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2((tech.center.x - this.targetTech.center.x - this.targetTech.width) / 2f, 0f),
				new Vector2((tech.center.x - this.targetTech.center.x - this.targetTech.width) / 2f, tech.center.y - this.targetTech.center.y),
				new Vector2(tech.center.x - this.targetTech.center.x - this.targetTech.width, tech.center.y - this.targetTech.center.y)
			};
			this.techLineMap.Add(tech, component);
		}
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

	protected override void OnCmpEnable()
	{
	}

	public void SetTech(Tech newTech)
	{
		if (newTech == null)
		{
			Debug.LogError("The research provided is null!");
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
				Image image = gameObject.GetComponentsInChildren<Image>()[1];
				Image image2 = gameObject.GetComponentsInChildren<Image>()[2];
				image.color = researchType.color;
				image2.color = researchType.color;
				this.progressBarsByResearchTypeID[researchType.id] = gameObject;
			}
		}
		if (this.researchScreen == null)
		{
			this.researchScreen = this.transform.parent.GetComponentInParent<ResearchScreen>();
		}
		if (newTech.IsComplete())
		{
			this.ResearchCompleted(false);
		}
		this.targetTech = newTech;
		this.researchName.text = this.targetTech.Name;
		string text = string.Empty;
		foreach (BuildingDef buildingDef in this.targetTech.unlockedBuildings)
		{
			KPointerImage componentInChildrenOnly = this.GetFreeIcon().GetComponentInChildrenOnly<KPointerImage>();
			componentInChildrenOnly.transform.parent.gameObject.SetActive(true);
			if (text != string.Empty)
			{
				text += ", ";
			}
			text += buildingDef.Name;
			string text2 = string.Format("{0}\n{1}", buildingDef.Name, buildingDef.Effect);
			componentInChildrenOnly.GetComponent<ToolTip>().toolTip = text2;
			componentInChildrenOnly.sprite = buildingDef.GetUISprite("ui");
			componentInChildrenOnly.ClearPointerEvents();
			componentInChildrenOnly.onPointerEnter += delegate
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
			};
		}
		text = string.Format(UI.RESEARCHSCREEN_UNLOCKSTOOLTIP, text);
		this.researchName.GetComponent<ToolTip>().toolTip = string.Format("{0}\n{1}\n\n{2}", this.targetTech.Name, this.targetTech.desc, text);
		this.button.ClearOnClick();
		this.button.onClick += this.OnResearchClicked;
		KButton kbutton = this.button;
		kbutton.onPointerEnter = (global::System.Action)Delegate.Combine(kbutton.onPointerEnter, new global::System.Action(delegate
		{
			this.researchScreen.TurnEverythingOff();
			this.OnHover(true, this.targetTech);
		}));
		KButton kbutton2 = this.button;
		kbutton2.onPointerExit = (global::System.Action)Delegate.Combine(kbutton2.onPointerExit, new global::System.Action(delegate
		{
			if (this.turnEverythingOn != null)
			{
				base.StopCoroutine(this.turnEverythingOn);
			}
			this.turnEverythingOn = base.StartCoroutine(this.TurnEverythingOnWithDelay(0.15f));
		}));
	}

	private IEnumerator TurnEverythingOnWithDelay(float delay)
	{
		float currentTime = Time.realtimeSinceStartup;
		float targetTime = currentTime + delay;
		while (currentTime < targetTime)
		{
			yield return new WaitForEndOfFrame();
			currentTime += Time.unscaledDeltaTime;
		}
		this.researchScreen.TurnEverythingOn();
		yield break;
	}

	public void SetGeneralAlpha(float alpha, Tech tech = null)
	{
		List<Image> list = new List<Image>(base.GetComponentsInChildren<Image>());
		list.ForEach(delegate(Image img)
		{
			if (tech == null)
			{
				img.SetAlpha(alpha);
			}
		});
		List<Text> list2 = new List<Text>(base.GetComponentsInChildren<Text>());
		list2.ForEach(delegate(Text txt)
		{
			txt.SetAlpha(alpha);
		});
	}

	public void SetEverythingOff()
	{
		if (this.turnEverythingOn != null)
		{
			base.StopCoroutine(this.turnEverythingOn);
			this.turnEverythingOn = null;
		}
		if (!this.isOn)
		{
			return;
		}
		List<Image> list = new List<Image>(base.GetComponentsInChildren<Image>());
		List<Text> list2 = new List<Text>(base.GetComponentsInChildren<Text>());
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.fadeRoutine = base.StartCoroutine(this.tintColors(list));
		list2.ForEach(delegate(Text txt)
		{
			txt.color = Color.white - this.tintColor;
		});
		if (!this.targetTech.IsComplete())
		{
			foreach (KeyValuePair<string, GameObject> keyValuePair in this.progressBarsByResearchTypeID)
			{
				keyValuePair.Value.GetComponentsInChildren<Image>()[1].fillAmount = 0f;
			}
		}
		this.isOn = false;
	}

	private IEnumerator tintColors(List<Image> imgs)
	{
		imgs.ForEach(delegate(Image img)
		{
			if (this.originalImgColors.ContainsKey(img))
			{
				img.color = this.originalImgColors[img];
			}
		});
		int steps = 6;
		for (int i = 0; i < steps; i++)
		{
			imgs.ForEach(delegate(Image img)
			{
				if (this.originalImgColors.ContainsKey(img))
				{
					img.color -= this.tintColor / (float)steps;
				}
			});
			yield return 0;
		}
		yield break;
	}

	public void SetEverythingOn()
	{
		if (this.turnEverythingOn != null)
		{
			base.StopCoroutine(this.turnEverythingOn);
			this.turnEverythingOn = null;
		}
		if (this.isOn)
		{
			return;
		}
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		List<Image> list = new List<Image>(base.GetComponentsInChildren<Image>());
		List<Text> list2 = new List<Text>(base.GetComponentsInChildren<Text>());
		list.ForEach(delegate(Image img)
		{
			if (this.originalImgColors.ContainsKey(img))
			{
				img.color = this.originalImgColors[img];
			}
		});
		this.UpdateProgressBars();
		list2.ForEach(delegate(Text txt)
		{
			txt.color = Color.white;
		});
		this.isOn = true;
	}

	private void SetTints(List<Image> imgs, List<Text> txts, Color tint, bool increment)
	{
		imgs.ForEach(delegate(Image img)
		{
			if (increment)
			{
				img.color += tint;
			}
			else
			{
				img.color -= tint;
			}
		});
		txts.ForEach(delegate(Text txt)
		{
			if (increment)
			{
				txt.color += tint;
			}
			else
			{
				txt.color -= tint;
			}
		});
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
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
		this.button.ClearOnClick();
		this.button.onClick += this.OnResearchClicked;
		this.researchScreen.CancelResearch();
		Research.Instance.CancelResearch(this.targetTech, true);
	}

	public void QueueStateChanged(bool isSelected)
	{
		if (isSelected && !this.targetTech.IsComplete())
		{
			this.BG.color = this.pendingColor;
			this.originalImgColors[this.BG] = this.pendingColor;
			this.button.ClearOnClick();
			this.button.onClick += this.OnResearchCanceled;
		}
		else if (this.targetTech.IsComplete())
		{
			this.BG.color = this.completedColor;
			this.originalImgColors[this.BG] = this.completedColor;
			this.defaultColor = this.completedColor;
			this.button.ClearOnClick();
		}
		else
		{
			this.BG.color = ((!this.isOn) ? (this.defaultColor - this.tintColor) : this.defaultColor);
			this.originalImgColors[this.BG] = this.defaultColor;
			this.button.ClearOnClick();
			this.button.onClick += this.OnResearchClicked;
		}
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
			child.GetComponentsInChildren<Image>()[1].fillAmount = num;
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
		this.originalImgColors[this.BG] = this.completedColor;
		this.defaultColor = this.completedColor;
		this.button.ClearOnClick();
		if (notify)
		{
			ResearchCompleteMessage researchCompleteMessage = new ResearchCompleteMessage(this.targetTech);
			MusicManager.instance.PlaySong("Stinger_ResearchComplete", false);
			Messenger.Instance.QueueMessage(researchCompleteMessage);
		}
	}

	[SerializeField]
	[Header("Labels")]
	private LocText researchName;

	[SerializeField]
	[Header("Transforms")]
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
	private Sprite hoverBG;

	[SerializeField]
	private Sprite completedBG;

	[SerializeField]
	[Header("Colors")]
	private Color defaultColor = Color.blue;

	[SerializeField]
	private Color completedColor = Color.yellow;

	[SerializeField]
	private Color pendingColor = Color.magenta;

	private Sprite defaultBG;

	private ResearchScreen researchScreen;

	[MyCmpGet]
	private KButton button;

	private Dictionary<Tech, UILineRenderer> techLineMap;

	private Tech targetTech;

	private bool isOn = true;

	private Coroutine fadeRoutine;

	public Color activeLineColor;

	public Color inactiveLineColor;

	[SerializeField]
	private Color tintColor = new Color(0.19607843f, 0.19607843f, 0.19607843f, 0.19607843f);

	private Dictionary<Image, Color> originalImgColors = new Dictionary<Image, Color>();

	private Dictionary<string, GameObject> progressBarsByResearchTypeID = new Dictionary<string, GameObject>();

	private Coroutine turnEverythingOn;
}
