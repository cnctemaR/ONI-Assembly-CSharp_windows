using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
		Transform transform = base.transform;
		while (this.m_Raycaster == null)
		{
			this.m_Raycaster = transform.GetComponent<GraphicRaycaster>();
			if (this.m_Raycaster == null)
			{
				transform = transform.parent;
			}
		}
	}

	private void ZoomOut()
	{
		this.targetZoom = Mathf.Clamp(this.targetZoom - this.zoomAmountPerButton, this.minZoom, this.maxZoom);
		this.zoomCenterLock = true;
	}

	private void ZoomIn()
	{
		this.targetZoom = Mathf.Clamp(this.targetZoom + this.zoomAmountPerButton, this.minZoom, this.maxZoom);
		this.zoomCenterLock = true;
	}

	private void Update()
	{
		RectTransform component = this.scrollContent.GetComponent<RectTransform>();
		if (!this.isDragging && (this.rightMouseDown || this.leftMouseDown) && Vector2.Distance(this.dragStartPosition, KInputManager.GetMousePos()) > 1f)
		{
			this.isDragging = true;
		}
		Vector2 anchoredPosition = component.anchoredPosition;
		float num = Mathf.Min(this.effectiveZoomSpeed * Time.unscaledDeltaTime, 0.9f);
		this.currentZoom = Mathf.Lerp(this.currentZoom, this.targetZoom, num);
		Vector2 vector = Vector2.zero;
		Vector2 vector2 = KInputManager.GetMousePos();
		Vector2 vector3 = (this.zoomCenterLock ? (component.InverseTransformPoint(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2))) * this.currentZoom) : (component.InverseTransformPoint(vector2) * this.currentZoom));
		component.localScale = new Vector3(this.currentZoom, this.currentZoom, 1f);
		vector = (this.zoomCenterLock ? (component.InverseTransformPoint(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2))) * this.currentZoom) : (component.InverseTransformPoint(vector2) * this.currentZoom)) - vector3;
		float num2 = this.keyboardScrollSpeed;
		if (this.panUp)
		{
			this.keyPanDelta -= Vector2.up * Time.unscaledDeltaTime * num2;
		}
		else if (this.panDown)
		{
			this.keyPanDelta += Vector2.up * Time.unscaledDeltaTime * num2;
		}
		if (this.panLeft)
		{
			this.keyPanDelta += Vector2.right * Time.unscaledDeltaTime * num2;
		}
		else if (this.panRight)
		{
			this.keyPanDelta -= Vector2.right * Time.unscaledDeltaTime * num2;
		}
		Vector2 vector4 = new Vector2(Mathf.Lerp(0f, this.keyPanDelta.x, Time.unscaledDeltaTime * this.keyPanEasing), Mathf.Lerp(0f, this.keyPanDelta.y, Time.unscaledDeltaTime * this.keyPanEasing));
		this.keyPanDelta -= vector4;
		Vector2 vector5 = Vector2.zero;
		if (this.isDragging)
		{
			Vector2 vector6 = KInputManager.GetMousePos() - this.dragLastPosition;
			vector5 += vector6;
			this.dragLastPosition = KInputManager.GetMousePos();
		}
		Vector2 vector7 = anchoredPosition + vector + this.keyPanDelta + vector5;
		if (!this.isDragging)
		{
			Vector2 vector8 = component.rect.size * -0.5f * this.currentZoom;
			Vector2 vector9 = component.rect.size * 0.5f * this.currentZoom;
			Vector2 vector10 = new Vector2(Mathf.Clamp(vector7.x, vector8.x, vector9.x), Mathf.Clamp(vector7.y, vector8.y, vector9.y)) - vector7;
			if (!this.panLeft && !this.panRight && !this.panUp && !this.panDown)
			{
				vector7 += vector10 * this.edgeClampFactor * Time.unscaledDeltaTime;
			}
			else
			{
				vector7 += vector10;
				if (vector10.x < 0f)
				{
					this.keyPanDelta.x = Mathf.Min(0f, this.keyPanDelta.x);
				}
				if (vector10.x > 0f)
				{
					this.keyPanDelta.x = Mathf.Max(0f, this.keyPanDelta.x);
				}
				if (vector10.y < 0f)
				{
					this.keyPanDelta.y = Mathf.Min(0f, this.keyPanDelta.y);
				}
				if (vector10.y > 0f)
				{
					this.keyPanDelta.y = Mathf.Max(0f, this.keyPanDelta.y);
				}
			}
		}
		component.anchoredPosition = vector7;
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
			this.filterField.text = "";
			this.OnFilterChanged("");
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
		List<TechTreeTitle> resources2 = Db.Get().TechTreeTitles.resources;
		resources2.Sort((TechTreeTitle x, TechTreeTitle y) => y.center.y.CompareTo(x.center.y));
		float num = 0f;
		float num2 = 125f;
		Vector2 vector = new Vector2(num, num2);
		for (int i = 0; i < resources2.Count; i++)
		{
			ResearchTreeTitle researchTreeTitle = Util.KInstantiateUI<ResearchTreeTitle>(this.researchTreeTitlePrefab.gameObject, this.treeTitles, false);
			TechTreeTitle techTreeTitle = resources2[i];
			researchTreeTitle.name = techTreeTitle.Name + " Title";
			Vector3 vector2 = techTreeTitle.center + vector;
			researchTreeTitle.transform.rectTransform().anchoredPosition = vector2;
			float num3 = techTreeTitle.height;
			if (i + 1 < resources2.Count)
			{
				TechTreeTitle techTreeTitle2 = resources2[i + 1];
				Vector3 vector3 = techTreeTitle2.center + vector;
				num3 += vector2.y - (vector3.y + techTreeTitle2.height);
			}
			else
			{
				num3 += 600f;
			}
			researchTreeTitle.transform.rectTransform().sizeDelta = new Vector2(techTreeTitle.width, num3);
			researchTreeTitle.SetLabel(techTreeTitle.Name);
			researchTreeTitle.SetColor(i);
		}
		List<Vector2> list = new List<Vector2>();
		float num4 = 0f;
		float num5 = 0f;
		Vector2 vector4 = new Vector2(num4, num5);
		for (int j = 0; j < resources.Count; j++)
		{
			ResearchEntry researchEntry = Util.KInstantiateUI<ResearchEntry>(this.entryPrefab.gameObject, this.scrollContent, false);
			Tech tech = resources[j];
			researchEntry.name = tech.Name + " Panel";
			Vector3 vector5 = tech.center + vector4;
			researchEntry.transform.rectTransform().anchoredPosition = vector5;
			researchEntry.transform.rectTransform().sizeDelta = new Vector2(tech.width, tech.height);
			this.entryMap.Add(tech, researchEntry);
			if (tech.edges.Count > 0)
			{
				for (int k = 0; k < tech.edges.Count; k++)
				{
					ResourceTreeNode.Edge edge = tech.edges[k];
					if (edge.path == null)
					{
						list.AddRange(edge.SrcTarget);
					}
					else
					{
						ResourceTreeNode.Edge.EdgeType edgeType = edge.edgeType;
						if (edgeType <= ResourceTreeNode.Edge.EdgeType.QuadCurveEdge || edgeType - ResourceTreeNode.Edge.EdgeType.BezierEdge <= 1)
						{
							list.Add(edge.SrcTarget[0]);
							list.Add(edge.path[0]);
							for (int l = 1; l < edge.path.Count; l++)
							{
								list.Add(edge.path[l - 1]);
								list.Add(edge.path[l]);
							}
							list.Add(edge.path[edge.path.Count - 1]);
							list.Add(edge.SrcTarget[1]);
						}
						else
						{
							list.AddRange(edge.path);
						}
					}
				}
			}
		}
		for (int m = 0; m < list.Count; m++)
		{
			list[m] = new Vector2(list[m].x, list[m].y + this.foreground.transform.rectTransform().rect.height);
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
		base.OnSpawn();
		base.Show(false);
		this.zoomOutButton.onClick += delegate
		{
			this.ZoomOut();
		};
		this.zoomInButton.onClick += delegate
		{
			this.ZoomIn();
		};
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
		TechInstance targetResearch = Research.Instance.GetTargetResearch();
		if (targetResearch != null)
		{
			this.SetActiveResearch(targetResearch.tech);
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
		if (show)
		{
			DetailsScreen.Instance.gameObject.SetActive(false);
		}
		else if (SelectTool.Instance.selected != null)
		{
			DetailsScreen.Instance.gameObject.SetActive(true);
			DetailsScreen.Instance.Refresh(SelectTool.Instance.selected.gameObject);
		}
		this.filterField.text = "";
		this.OnFilterChanged("");
		this.UpdateProgressBars();
		this.UpdatePointDisplay();
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (e.IsAction(global::Action.MouseRight) || e.IsAction(global::Action.MouseLeft))
			{
				if (!this.isDragging && e.TryConsume(global::Action.MouseRight))
				{
					this.isDragging = false;
					this.rightMouseDown = false;
					this.leftMouseDown = false;
					ManagementMenu.Instance.CloseAll();
					return;
				}
				this.isDragging = false;
				this.rightMouseDown = false;
				this.leftMouseDown = false;
			}
			if (this.panUp && e.TryConsume(global::Action.PanUp))
			{
				this.panUp = false;
				return;
			}
			if (this.panDown && e.TryConsume(global::Action.PanDown))
			{
				this.panDown = false;
				return;
			}
			if (this.panRight && e.TryConsume(global::Action.PanRight))
			{
				this.panRight = false;
				return;
			}
			if (this.panLeft && e.TryConsume(global::Action.PanLeft))
			{
				this.panLeft = false;
				return;
			}
		}
		base.OnKeyUp(e);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (e.TryConsume(global::Action.MouseRight))
			{
				this.dragStartPosition = KInputManager.GetMousePos();
				this.dragLastPosition = KInputManager.GetMousePos();
				this.rightMouseDown = true;
				return;
			}
			if (e.TryConsume(global::Action.MouseLeft))
			{
				this.dragStartPosition = KInputManager.GetMousePos();
				this.dragLastPosition = KInputManager.GetMousePos();
				this.leftMouseDown = true;
				return;
			}
			if (e.TryConsume(global::Action.ZoomIn))
			{
				this.targetZoom = Mathf.Clamp(this.targetZoom + this.zoomAmountPerScroll, this.minZoom, this.maxZoom);
				this.zoomCenterLock = false;
				return;
			}
			if (e.TryConsume(global::Action.ZoomOut))
			{
				this.targetZoom = Mathf.Clamp(this.targetZoom - this.zoomAmountPerScroll, this.minZoom, this.maxZoom);
				this.zoomCenterLock = false;
				return;
			}
			if (e.TryConsume(global::Action.Escape))
			{
				ManagementMenu.Instance.CloseAll();
				return;
			}
			if (e.TryConsume(global::Action.PanLeft))
			{
				this.panLeft = true;
				return;
			}
			if (e.TryConsume(global::Action.PanRight))
			{
				this.panRight = true;
				return;
			}
			if (e.TryConsume(global::Action.PanUp))
			{
				this.panUp = true;
				return;
			}
			if (e.TryConsume(global::Action.PanDown))
			{
				this.panDown = true;
				return;
			}
		}
		base.OnKeyDown(e);
	}

	private void OnFilterChanged(string filter_text)
	{
		filter_text = filter_text.ToLower();
		foreach (KeyValuePair<Tech, ResearchEntry> keyValuePair in this.entryMap)
		{
			keyValuePair.Value.UpdateFilterState(filter_text);
		}
	}

	[SerializeField]
	private Image BG;

	public ResearchEntry entryPrefab;

	public ResearchTreeTitle researchTreeTitlePrefab;

	public GameObject foreground;

	public GameObject scrollContent;

	public GameObject treeTitles;

	public GameObject pointDisplayCountPrefab;

	public GameObject pointDisplayContainer;

	private Dictionary<string, LocText> pointDisplayMap;

	private Dictionary<Tech, ResearchEntry> entryMap;

	[SerializeField]
	private TMP_InputField filterField;

	[SerializeField]
	private KButton filterClearButton;

	[SerializeField]
	private KButton zoomOutButton;

	[SerializeField]
	private KButton zoomInButton;

	private Tech currentResearch;

	public KButton CloseButton;

	private GraphicRaycaster m_Raycaster;

	private PointerEventData m_PointerEventData;

	private Vector3 currentScrollPosition;

	private bool panUp;

	private bool panDown;

	private bool panLeft;

	private bool panRight;

	private bool rightMouseDown;

	private bool leftMouseDown;

	private bool isDragging;

	private Vector3 dragStartPosition;

	private Vector3 dragLastPosition;

	private float targetZoom = 1f;

	private float currentZoom = 1f;

	private bool zoomCenterLock;

	private Vector2 keyPanDelta = Vector3.zero;

	[SerializeField]
	private float effectiveZoomSpeed = 5f;

	[SerializeField]
	private float zoomAmountPerScroll = 0.05f;

	[SerializeField]
	private float zoomAmountPerButton = 0.5f;

	[SerializeField]
	private float minZoom = 0.15f;

	[SerializeField]
	private float maxZoom = 1f;

	[SerializeField]
	private float keyboardScrollSpeed = 200f;

	[SerializeField]
	private float keyPanEasing = 1f;

	[SerializeField]
	private float edgeClampFactor = 0.5f;

	public enum ResearchState
	{
		Available,
		ActiveResearch,
		ResearchComplete,
		MissingPrerequisites,
		StateCount
	}
}
