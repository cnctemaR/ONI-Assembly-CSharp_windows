using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResearchScreen : KModalScreen
{
	public bool IsBeingResearched(Tech tech)
	{
		return Research.Instance.IsBeingResearched(tech);
	}

	public override float GetSortKey()
	{
		if (base.isEditing)
		{
			return 50f;
		}
		return 20f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.ConsumeMouseScroll = true;
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

	public void ZoomToTech(string techID)
	{
		Vector2 vector = this.entryMap[Db.Get().Techs.Get(techID)].rectTransform().GetLocalPosition() + new Vector2(-this.foreground.rectTransform().rect.size.x / 2f, this.foreground.rectTransform().rect.size.y / 2f);
		this.forceTargetPosition = -vector;
		this.zoomingToTarget = true;
		this.targetZoom = this.maxZoom;
	}

	private void Update()
	{
		if (!base.canvas.enabled)
		{
			return;
		}
		RectTransform component = this.scrollContent.GetComponent<RectTransform>();
		if (this.isDragging && !KInputManager.isFocused)
		{
			this.AbortDragging();
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
		if (KInputManager.currentControllerIsGamepad)
		{
			Vector2 vector4 = KInputManager.steamInputInterpreter.GetSteamCameraMovement();
			vector4 *= -1f;
			this.keyPanDelta = vector4 * Time.unscaledDeltaTime * num2 * 2f;
		}
		Vector2 vector5 = new Vector2(Mathf.Lerp(0f, this.keyPanDelta.x, Time.unscaledDeltaTime * this.keyPanEasing), Mathf.Lerp(0f, this.keyPanDelta.y, Time.unscaledDeltaTime * this.keyPanEasing));
		this.keyPanDelta -= vector5;
		Vector2 vector6 = Vector2.zero;
		if (this.isDragging)
		{
			Vector2 vector7 = KInputManager.GetMousePos() - this.dragLastPosition;
			vector6 += vector7;
			this.dragLastPosition = KInputManager.GetMousePos();
			this.dragInteria = Vector2.ClampMagnitude(this.dragInteria + vector7, 400f);
		}
		this.dragInteria *= Mathf.Max(0f, 1f - Time.unscaledDeltaTime * 4f);
		Vector2 vector8 = anchoredPosition + vector + this.keyPanDelta + vector6;
		if (!this.isDragging)
		{
			Vector2 size = base.GetComponent<RectTransform>().rect.size;
			Vector2 vector9 = new Vector2((-component.rect.size.x / 2f - 250f) * this.currentZoom, -250f * this.currentZoom);
			Vector2 vector10 = new Vector2(250f * this.currentZoom, (component.rect.size.y + 250f) * this.currentZoom - size.y);
			Vector2 vector11 = new Vector2(Mathf.Clamp(vector8.x, vector9.x, vector10.x), Mathf.Clamp(vector8.y, vector9.y, vector10.y));
			this.forceTargetPosition = new Vector2(Mathf.Clamp(this.forceTargetPosition.x, vector9.x, vector10.x), Mathf.Clamp(this.forceTargetPosition.y, vector9.y, vector10.y));
			Vector2 vector12 = vector11 + this.dragInteria - vector8;
			if (!this.panLeft && !this.panRight && !this.panUp && !this.panDown)
			{
				vector8 += vector12 * this.edgeClampFactor * Time.unscaledDeltaTime;
			}
			else
			{
				vector8 += vector12;
				if (vector12.x < 0f)
				{
					this.keyPanDelta.x = Mathf.Min(0f, this.keyPanDelta.x);
				}
				if (vector12.x > 0f)
				{
					this.keyPanDelta.x = Mathf.Max(0f, this.keyPanDelta.x);
				}
				if (vector12.y < 0f)
				{
					this.keyPanDelta.y = Mathf.Min(0f, this.keyPanDelta.y);
				}
				if (vector12.y > 0f)
				{
					this.keyPanDelta.y = Mathf.Max(0f, this.keyPanDelta.y);
				}
			}
		}
		if (this.zoomingToTarget)
		{
			vector8 = Vector2.Lerp(vector8, this.forceTargetPosition, Time.unscaledDeltaTime * 4f);
			if (Vector3.Distance(vector8, this.forceTargetPosition) < 1f || this.isDragging || this.panLeft || this.panRight || this.panUp || this.panDown)
			{
				this.zoomingToTarget = false;
			}
		}
		component.anchoredPosition = vector8;
	}

	protected override void OnSpawn()
	{
		base.Subscribe(Research.Instance.gameObject, -1914338957, new Action<object>(this.OnActiveResearchChanged));
		base.Subscribe(Game.Instance.gameObject, -107300940, new Action<object>(this.OnResearchComplete));
		base.Subscribe(Game.Instance.gameObject, -1974454597, delegate(object o)
		{
			this.Show(false);
		});
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
		base.OnSpawn();
		this.scrollContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(250f, -250f);
		this.zoomOutButton.onClick += delegate
		{
			this.ZoomOut();
		};
		this.zoomInButton.onClick += delegate
		{
			this.ZoomIn();
		};
		base.gameObject.SetActive(true);
		this.Show(false);
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		this.isDragging = true;
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		this.AbortDragging();
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
		yield return SequenceUtil.WaitForEndOfFrame;
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
		if (data is Tech)
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

	public override void Show(bool show = true)
	{
		this.mouseOver = false;
		this.scrollContentChildFitter.enabled = show;
		foreach (Canvas canvas in base.GetComponentsInChildren<Canvas>(true))
		{
			if (canvas.enabled != show)
			{
				canvas.enabled = show;
			}
		}
		CanvasGroup component = base.GetComponent<CanvasGroup>();
		if (component != null)
		{
			component.interactable = show;
			component.blocksRaycasts = show;
			component.ignoreParentGroups = true;
		}
		this.OnShow(show);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			this.sideBar.ResetFilter();
		}
		if (show)
		{
			if (DetailsScreen.Instance != null)
			{
				DetailsScreen.Instance.gameObject.SetActive(false);
			}
		}
		else if (SelectTool.Instance.selected != null && !DetailsScreen.Instance.gameObject.activeSelf)
		{
			DetailsScreen.Instance.gameObject.SetActive(true);
			DetailsScreen.Instance.Refresh(SelectTool.Instance.selected.gameObject);
		}
		this.UpdateProgressBars();
		this.UpdatePointDisplay();
	}

	private void AbortDragging()
	{
		this.isDragging = false;
		this.draggingJustEnded = true;
	}

	private void LateUpdate()
	{
		this.draggingJustEnded = false;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!base.canvas.enabled)
		{
			return;
		}
		if (!e.Consumed)
		{
			if (e.IsAction(global::Action.MouseRight) && !this.isDragging && !this.draggingJustEnded)
			{
				ManagementMenu.Instance.CloseAll();
			}
			if (e.IsAction(global::Action.MouseRight) || e.IsAction(global::Action.MouseLeft) || e.IsAction(global::Action.MouseMiddle))
			{
				this.AbortDragging();
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
		if (!base.canvas.enabled)
		{
			return;
		}
		if (!e.Consumed)
		{
			if (e.TryConsume(global::Action.MouseRight))
			{
				this.dragStartPosition = KInputManager.GetMousePos();
				this.dragLastPosition = KInputManager.GetMousePos();
				return;
			}
			if (e.TryConsume(global::Action.MouseLeft))
			{
				this.dragStartPosition = KInputManager.GetMousePos();
				this.dragLastPosition = KInputManager.GetMousePos();
				return;
			}
			if (KInputManager.GetMousePos().x > this.sideBar.rectTransform().sizeDelta.x)
			{
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

	public static bool TechPassesSearchFilter(string techID, string filterString)
	{
		if (!string.IsNullOrEmpty(filterString))
		{
			filterString = filterString.ToUpper();
			bool flag = false;
			Tech tech = Db.Get().Techs.Get(techID);
			flag = UI.StripLinkFormatting(tech.Name).ToLower().ToUpper()
				.Contains(filterString);
			if (!flag)
			{
				flag = tech.category.ToUpper().Contains(filterString);
				foreach (TechItem techItem in tech.unlockedItems)
				{
					if (SaveLoader.Instance.IsDlcListActiveForCurrentSave(techItem.dlcIds))
					{
						if (UI.StripLinkFormatting(techItem.Name).ToLower().ToUpper()
							.Contains(filterString))
						{
							flag = true;
							break;
						}
						if (UI.StripLinkFormatting(techItem.description).ToLower().ToUpper()
							.Contains(filterString))
						{
							flag = true;
							break;
						}
					}
				}
			}
			return flag;
		}
		return true;
	}

	public static bool TechItemPassesSearchFilter(string techItemID, string filterString)
	{
		if (!string.IsNullOrEmpty(filterString))
		{
			filterString = filterString.ToUpper();
			TechItem techItem = Db.Get().TechItems.Get(techItemID);
			bool flag = UI.StripLinkFormatting(techItem.Name).ToLower().ToUpper()
				.Contains(filterString);
			if (!flag)
			{
				flag = techItem.Name.ToUpper().Contains(filterString);
				flag = flag && techItem.description.ToUpper().Contains(filterString);
			}
			return flag;
		}
		return true;
	}

	private const float SCROLL_BUFFER = 250f;

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
	private KButton zoomOutButton;

	[SerializeField]
	private KButton zoomInButton;

	[SerializeField]
	private ResearchScreenSideBar sideBar;

	private Tech currentResearch;

	public KButton CloseButton;

	private GraphicRaycaster m_Raycaster;

	private PointerEventData m_PointerEventData;

	private Vector3 currentScrollPosition;

	private bool panUp;

	private bool panDown;

	private bool panLeft;

	private bool panRight;

	[SerializeField]
	private KChildFitter scrollContentChildFitter;

	private bool isDragging;

	private Vector3 dragStartPosition;

	private Vector3 dragLastPosition;

	private Vector2 dragInteria;

	private Vector2 forceTargetPosition;

	private bool zoomingToTarget;

	private bool draggingJustEnded;

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
