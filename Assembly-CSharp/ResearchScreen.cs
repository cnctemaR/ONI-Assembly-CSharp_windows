using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ResearchScreen : KModalScreen
{
	public bool IsBeingResearched(Tech tech)
	{
		return Research.Instance.IsBeingResearched(tech);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ConsumeMouseScroll = true;
	}

	protected override void OnSpawn()
	{
		base.Subscribe(Research.Instance.gameObject, -1914338957, new Action<object>(this.OnActiveResearchChanged));
		base.Subscribe(Game.Instance.gameObject, -107300940, new Action<object>(this.OnResearchComplete));
		base.Subscribe(Game.Instance.gameObject, -1974454597, delegate(object o)
		{
			base.Show(false);
		});
		this.filterField.placeholder.GetComponent<TextMeshProUGUI>().text = UI.FILTER;
		this.filterField.onValueChanged.AddListener(new UnityAction<string>(this.OnFilterChanged));
		this.filterClearButton.onClick += delegate
		{
			this.filterField.text = string.Empty;
			this.OnFilterChanged(string.Empty);
		};
		this.pointDisplayMap = new Dictionary<string, LocText>();
		foreach (ResearchType researchType in Research.Instance.researchTypes.Types)
		{
			this.pointDisplayMap[researchType.id] = Util.KInstantiateUI(this.pointDisplayCountPrefab, this.pointDisplayContainer, true).GetComponentInChildren<LocText>();
			this.pointDisplayMap[researchType.id].text = Research.Instance.globalPointInventory.PointsByTypeID[researchType.id].ToString();
			this.pointDisplayMap[researchType.id].transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(researchType.description);
			this.pointDisplayMap[researchType.id].transform.parent.GetComponentInChildren<Image>().sprite = researchType.sprite;
		}
		this.pointDisplayContainer.transform.parent.gameObject.SetActive(Research.Instance.UseGlobalPointInventory);
		this.entryMap = new Dictionary<Tech, ResearchEntry>();
		List<Tech> resources = Db.Get().Techs.resources;
		resources.Sort((Tech x, Tech y) => y.center.y.CompareTo(x.center.y));
		List<Vector2> list = new List<Vector2>();
		float num = 0f;
		float num2 = 0f;
		Vector2 vector = new Vector2(num, num2);
		for (int i = 0; i < resources.Count; i++)
		{
			ResearchEntry researchEntry = Util.KInstantiateUI<ResearchEntry>(this.entryPrefab.gameObject, this.scrollContent, false);
			Tech tech = resources[i];
			researchEntry.name = tech.Name + " Panel";
			Vector3 vector2 = tech.center + vector;
			researchEntry.transform.rectTransform().anchoredPosition = vector2;
			researchEntry.transform.rectTransform().sizeDelta = new Vector2(tech.width, tech.height);
			this.entryMap.Add(tech, researchEntry);
			if (tech.edges.Count > 0)
			{
				for (int j = 0; j < tech.edges.Count; j++)
				{
					ResourceTreeNode.Edge edge = tech.edges[j];
					if (edge.path == null)
					{
						list.AddRange(edge.SrcTarget);
					}
					else
					{
						switch (edge.edgeType)
						{
						case ResourceTreeNode.Edge.EdgeType.PolyLineEdge:
						case ResourceTreeNode.Edge.EdgeType.QuadCurveEdge:
						case ResourceTreeNode.Edge.EdgeType.BezierEdge:
						case ResourceTreeNode.Edge.EdgeType.GenericEdge:
						{
							list.Add(edge.SrcTarget[0]);
							list.Add(edge.path[0]);
							for (int k = 1; k < edge.path.Count; k++)
							{
								list.Add(edge.path[k - 1]);
								list.Add(edge.path[k]);
							}
							list.Add(edge.path[edge.path.Count - 1]);
							list.Add(edge.SrcTarget[1]);
							goto IL_041A;
						}
						}
						list.AddRange(edge.path);
					}
					IL_041A:;
				}
			}
		}
		for (int l = 0; l < list.Count; l++)
		{
			list[l] = new Vector2(list[l].x, list[l].y + this.foreground.transform.rectTransform().rect.height);
		}
		foreach (KeyValuePair<Tech, ResearchEntry> keyValuePair in this.entryMap)
		{
			keyValuePair.Value.SetTech(keyValuePair.Key);
		}
		this.CloseButton.soundPlayer.Enabled = false;
		this.CloseButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		base.StartCoroutine(this.WaitAndSetActiveResearch());
		ManagementMenu.Instance.AddResearchScreen(this);
		this.foreground.GetComponent<KScrollRect>().allowHorizontalScrollWheel = false;
		base.OnSpawn();
		base.Show(false);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		base.Unsubscribe(Game.Instance.gameObject, -1974454597, delegate(object o)
		{
			this.Deactivate();
		});
	}

	private IEnumerator WaitAndSetActiveResearch()
	{
		yield return new WaitForEndOfFrame();
		TechInstance tech = Research.Instance.GetTargetResearch();
		if (tech != null)
		{
			this.SetActiveResearch(tech.tech);
		}
		yield break;
	}

	public Vector3 GetEntryPosition(Tech tech)
	{
		if (!this.entryMap.ContainsKey(tech))
		{
			global::Debug.LogError("The Tech provided was not present in the dictionary");
			return Vector3.zero;
		}
		return this.entryMap[tech].transform.GetPosition();
	}

	public ResearchEntry GetEntry(Tech tech)
	{
		if (this.entryMap == null)
		{
			return null;
		}
		if (!this.entryMap.ContainsKey(tech))
		{
			global::Debug.LogError("The Tech provided was not present in the dictionary");
			return null;
		}
		return this.entryMap[tech];
	}

	public void SetEntryPercentage(Tech tech, float percent)
	{
		ResearchEntry entry = this.GetEntry(tech);
		if (entry != null)
		{
			entry.SetPercentage(percent);
		}
	}

	public void TurnEverythingOff()
	{
		foreach (KeyValuePair<Tech, ResearchEntry> keyValuePair in this.entryMap)
		{
			keyValuePair.Value.SetEverythingOff();
		}
	}

	public void TurnEverythingOn()
	{
		foreach (KeyValuePair<Tech, ResearchEntry> keyValuePair in this.entryMap)
		{
			keyValuePair.Value.SetEverythingOn();
		}
	}

	private void SelectAllEntries(Tech tech, bool isSelected)
	{
		ResearchEntry entry = this.GetEntry(tech);
		if (entry != null)
		{
			entry.QueueStateChanged(isSelected);
		}
		foreach (Tech tech2 in tech.requiredTech)
		{
			this.SelectAllEntries(tech2, isSelected);
		}
	}

	private void OnResearchComplete(object data)
	{
		Tech tech = (Tech)data;
		ResearchEntry entry = this.GetEntry(tech);
		if (entry != null)
		{
			entry.ResearchCompleted(true);
		}
		this.UpdateProgressBars();
		this.UpdatePointDisplay();
	}

	private void UpdatePointDisplay()
	{
		foreach (ResearchType researchType in Research.Instance.researchTypes.Types)
		{
			this.pointDisplayMap[researchType.id].text = string.Format("{0}: {1}", Research.Instance.researchTypes.GetResearchType(researchType.id).name, Research.Instance.globalPointInventory.PointsByTypeID[researchType.id].ToString());
		}
	}

	private void OnActiveResearchChanged(object data)
	{
		List<TechInstance> list = (List<TechInstance>)data;
		foreach (TechInstance techInstance in list)
		{
			ResearchEntry entry = this.GetEntry(techInstance.tech);
			if (entry != null)
			{
				entry.QueueStateChanged(true);
			}
		}
		this.UpdateProgressBars();
		this.UpdatePointDisplay();
		if (list.Count > 0)
		{
			this.currentResearch = list[list.Count - 1].tech;
		}
	}

	private void UpdateProgressBars()
	{
		foreach (KeyValuePair<Tech, ResearchEntry> keyValuePair in this.entryMap)
		{
			keyValuePair.Value.UpdateProgressBars();
		}
	}

	public void CancelResearch()
	{
		List<TechInstance> researchQueue = Research.Instance.GetResearchQueue();
		foreach (TechInstance techInstance in researchQueue)
		{
			ResearchEntry entry = this.GetEntry(techInstance.tech);
			if (entry != null)
			{
				entry.QueueStateChanged(false);
			}
		}
		researchQueue.Clear();
	}

	private void SetActiveResearch(Tech newResearch)
	{
		if (newResearch != this.currentResearch && this.currentResearch != null)
		{
			this.SelectAllEntries(this.currentResearch, false);
		}
		this.currentResearch = newResearch;
		if (this.currentResearch != null)
		{
			this.SelectAllEntries(this.currentResearch, true);
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		this.filterField.text = string.Empty;
		this.OnFilterChanged(string.Empty);
		this.UpdateProgressBars();
		this.UpdatePointDisplay();
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed && !this.scrollRect.isDragging && e.TryConsume(global::Action.MouseRight))
		{
			ManagementMenu.Instance.CloseAll();
			return;
		}
		base.OnKeyUp(e);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && e.TryConsume(global::Action.Escape))
		{
			ManagementMenu.Instance.CloseAll();
			return;
		}
		base.OnKeyDown(e);
	}

	private void OnFilterChanged(string filter_text)
	{
		filter_text = filter_text.ToLower();
		foreach (KeyValuePair<Tech, ResearchEntry> keyValuePair in this.entryMap)
		{
			ResearchEntry value = keyValuePair.Value;
			value.UpdateFilterState(filter_text);
		}
	}

	[SerializeField]
	private Image BG;

	public ResearchEntry entryPrefab;

	public GameObject foreground;

	public GameObject scrollContent;

	public GameObject pointDisplayCountPrefab;

	public GameObject pointDisplayContainer;

	private Dictionary<string, LocText> pointDisplayMap;

	private Dictionary<Tech, ResearchEntry> entryMap;

	[SerializeField]
	private TMP_InputField filterField;

	[SerializeField]
	private KButton filterClearButton;

	[SerializeField]
	private KScrollRect scrollRect;

	private Tech currentResearch;

	public KButton CloseButton;

	public enum ResearchState
	{
		Available,
		ActiveResearch,
		ResearchComplete,
		MissingPrerequisites,
		StateCount
	}
}
