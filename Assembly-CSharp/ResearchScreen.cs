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
		this.m_EventSystem = base.GetComponent<global::UnityEngine.EventSystems.EventSystem>();
		this.contentPositionDummy.SetLocalPosition(new Vector3(1000f, -2500f, 0f));
	}

	private IEnumerator ZoomOut()
	{
		KCanvasScaler kCanvasScaler = global::UnityEngine.Object.FindObjectOfType<KCanvasScaler>();
		this.zoomingOut = true;
		this.contentPositionDummy.transform.SetParent(this.scaleOffsetAnchor.transform);
		float zoomAmount = Mathf.Clamp(0.45f * kCanvasScaler.GetCanvasScale(), 0.1f, 1f);
		while (this.scaleOffsetAnchor.transform.localScale.x > zoomAmount)
		{
			this.scaleOffsetAnchor.transform.localScale *= 1f - Mathf.Clamp(Time.unscaledDeltaTime * 10f, 0f, 1f);
			yield return 0;
		}
		this.scaleOffsetAnchor.transform.localScale = Vector3.one * zoomAmount;
		this.contentPositionDummy.transform.SetParent(this.scaleOffsetAnchor.transform.parent);
		this.zoomingOut = false;
		yield break;
	}

	private IEnumerator ZoomIn()
	{
		KCanvasScaler kCanvasScaler = global::UnityEngine.Object.FindObjectOfType<KCanvasScaler>();
		this.zoomingIn = true;
		this.contentPositionDummy.transform.SetParent(this.scaleOffsetAnchor.transform);
		float zoomAmount = Mathf.Clamp(1f * kCanvasScaler.GetCanvasScale(), 1f, 1.6f);
		while (this.scaleOffsetAnchor.transform.localScale.x < zoomAmount)
		{
			this.scaleOffsetAnchor.transform.localScale *= 1f + Mathf.Clamp(Time.unscaledDeltaTime * 10f, 0f, 1f);
			yield return 0;
		}
		this.scaleOffsetAnchor.transform.localScale = Vector3.one * zoomAmount;
		this.contentPositionDummy.transform.SetParent(this.scaleOffsetAnchor.transform.parent);
		this.zoomingIn = false;
		yield break;
	}

	private void Update()
	{
		if (!this.isDragging && this.rightMouseDown && Vector3.Distance(this.dragStartPosition, Input.mousePosition) > 1f)
		{
			this.isDragging = true;
		}
		if (!this.zoomingIn && !this.zoomingOut)
		{
			this.scaleOffsetAnchor.SetPosition(Input.mousePosition);
			if (this.panUp)
			{
				this.contentPositionDummy.transform.position -= Vector3.up * Time.unscaledDeltaTime * this.keyboardScrollSpeed;
			}
			else if (this.panDown)
			{
				this.contentPositionDummy.transform.position += Vector3.up * Time.unscaledDeltaTime * this.keyboardScrollSpeed;
			}
			if (this.panLeft)
			{
				this.contentPositionDummy.transform.position += Vector3.right * Time.unscaledDeltaTime * this.keyboardScrollSpeed;
			}
			else if (this.panRight)
			{
				this.contentPositionDummy.transform.position -= Vector3.right * Time.unscaledDeltaTime * this.keyboardScrollSpeed;
			}
			if (Input.mouseScrollDelta.y > 0f)
			{
				base.StartCoroutine(this.ZoomIn());
			}
			else if (Input.mouseScrollDelta.y < 0f)
			{
				base.StartCoroutine(this.ZoomOut());
			}
			else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
			{
				if (this.contentPositionDummy.transform.parent != this.scaleOffsetAnchor.transform)
				{
					this.contentPositionDummy.transform.SetParent(this.scaleOffsetAnchor.transform);
				}
			}
			else if (this.contentPositionDummy.transform.parent != this.scaleOffsetAnchor.transform.parent)
			{
				this.contentPositionDummy.transform.SetParent(this.scaleOffsetAnchor.transform.parent);
			}
		}
		this.contentPositionDummy.position = this.ClampScrollToContent();
		Vector3 vector = Vector3.Lerp(this.scrollContent.transform.position, this.contentPositionDummy.transform.position, Time.unscaledDeltaTime * this.contentPositionLerpSpeed);
		this.scrollContent.transform.SetPosition(vector);
		this.scrollContent.transform.localScale = Vector3.Lerp(this.scrollContent.transform.localScale, this.contentPositionDummy.lossyScale, Time.unscaledDeltaTime * this.contentPositionLerpSpeed);
	}

	private Vector3 ClampScrollToContent()
	{
		Vector3 position = this.contentPositionDummy.position;
		if (!this.zoomingIn && !this.zoomingOut)
		{
			Vector3 vector = this.foreground.rectTransform().InverseTransformPoint(this.scrollContent.rectTransform().position);
			float num = 512f;
			float num2 = this.scrollContent.rectTransform().sizeDelta.x / 2f * this.scrollContent.transform.localScale.x - this.foreground.rectTransform().rect.width / 2f + num;
			if (vector.x > num2)
			{
				position.x -= vector.x - num2;
			}
			float num3 = -(this.scrollContent.rectTransform().sizeDelta.x / 2f * this.scrollContent.transform.localScale.x - this.foreground.rectTransform().rect.width / 2f + num);
			if (vector.x < num3)
			{
				position.x -= vector.x - num3;
			}
			float num4 = this.scrollContent.rectTransform().sizeDelta.y / 2f * this.scrollContent.transform.localScale.y - this.foreground.rectTransform().rect.height / 2f + num;
			if (vector.y > num4)
			{
				position.y -= vector.y - num4;
			}
			float num5 = -(this.scrollContent.rectTransform().sizeDelta.y / 2f * this.scrollContent.transform.localScale.y - this.foreground.rectTransform().rect.height / 2f + num);
			if (vector.y < num5)
			{
				position.y -= vector.y - num5;
			}
			if (this.scrollContent.transform.localScale.x < 0.7f && this.foreground.rectTransform().rect.width > this.scrollContent.rectTransform().rect.width * this.scrollContent.transform.localScale.x)
			{
				position.x = this.foreground.rectTransform().rect.width / 2f;
			}
		}
		return position;
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
						switch (edge.edgeType)
						{
						case ResourceTreeNode.Edge.EdgeType.PolyLineEdge:
						case ResourceTreeNode.Edge.EdgeType.QuadCurveEdge:
						case ResourceTreeNode.Edge.EdgeType.BezierEdge:
						case ResourceTreeNode.Edge.EdgeType.GenericEdge:
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
							goto IL_058C;
						}
						}
						list.AddRange(edge.path);
					}
					IL_058C:;
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
			base.StartCoroutine(this.ZoomOut());
		};
		this.zoomInButton.onClick += delegate
		{
			base.StartCoroutine(this.ZoomIn());
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
		if (show)
		{
			DetailsScreen.Instance.gameObject.SetActive(false);
		}
		else if (SelectTool.Instance.selected != null)
		{
			DetailsScreen.Instance.gameObject.SetActive(true);
			DetailsScreen.Instance.Refresh(SelectTool.Instance.selected.gameObject);
		}
		this.filterField.text = string.Empty;
		this.OnFilterChanged(string.Empty);
		this.UpdateProgressBars();
		this.UpdatePointDisplay();
		this.zoomingIn = false;
		this.zoomingOut = false;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (e.IsAction(global::Action.MouseRight))
			{
				if (!this.isDragging && e.TryConsume(global::Action.MouseRight))
				{
					this.isDragging = false;
					this.rightMouseDown = false;
					ManagementMenu.Instance.CloseAll();
					return;
				}
				this.isDragging = false;
				this.rightMouseDown = false;
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
				this.dragStartPosition = Input.mousePosition;
				this.rightMouseDown = true;
				return;
			}
			if (e.TryConsume(global::Action.ZoomIn))
			{
				this.targetContentScale = Mathf.Clamp(this.targetContentScale * (1f + Time.unscaledDeltaTime * 2.5f), 0.5f, 1f);
				return;
			}
			if (e.TryConsume(global::Action.ZoomOut))
			{
				this.targetContentScale = Mathf.Clamp(this.targetContentScale * (1f - Time.unscaledDeltaTime * 2.5f), 0.5f, 1f);
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
			ResearchEntry value = keyValuePair.Value;
			value.UpdateFilterState(filter_text);
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
	private RectTransform scaleOffsetAnchor;

	[SerializeField]
	private RectTransform contentPositionDummy;

	[SerializeField]
	private KButton zoomOutButton;

	[SerializeField]
	private KButton zoomInButton;

	private Tech currentResearch;

	public KButton CloseButton;

	private float targetContentScale = 1f;

	private GraphicRaycaster m_Raycaster;

	private PointerEventData m_PointerEventData;

	private global::UnityEngine.EventSystems.EventSystem m_EventSystem;

	private Vector3 currentScrollPosition;

	private float keyboardScrollSpeed = 1500f;

	private bool panUp;

	private bool panDown;

	private bool panLeft;

	private bool panRight;

	public float contentPositionLerpSpeed = 10f;

	private bool zoomingOut;

	private bool zoomingIn;

	private bool rightMouseDown;

	private bool isDragging;

	private Vector3 dragStartPosition;

	public enum ResearchState
	{
		Available,
		ActiveResearch,
		ResearchComplete,
		MissingPrerequisites,
		StateCount
	}
}
