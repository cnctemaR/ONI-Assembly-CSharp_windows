using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[UxmlElement(null, new Type[] { typeof(Tab) })]
	public class TabView : VisualElement
	{
		public override VisualElement contentContainer
		{
			get
			{
				return this.m_ContentContainer;
			}
		}

		public VisualElement contentViewport { get; }

		internal VisualElement header
		{
			get
			{
				return this.m_HeaderContainer;
			}
		}

		internal List<Tab> tabs
		{
			get
			{
				return this.m_Tabs;
			}
		}

		internal List<VisualElement> tabHeaders
		{
			get
			{
				return this.m_TabHeaders;
			}
		}

		internal RepeatButton nextButton { get; private set; }

		internal RepeatButton previousButton { get; private set; }

		internal float scrollableWidth
		{
			get
			{
				return Mathf.Max(0f, this.m_HeaderContainer.boundingBox.width - this.contentViewport.layout.width);
			}
		}

		internal bool needsButtons
		{
			get
			{
				return this.scrollableWidth > 0.001f;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Tab, Tab> activeTabChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, int> tabReordered;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Tab, int> tabClosed;

		public Tab activeTab
		{
			get
			{
				return this.m_ActiveTab;
			}
			set
			{
				bool flag = value == null && this.m_Tabs.Count > 0;
				if (flag)
				{
					throw new NullReferenceException("Active tab cannot be null when there are available tabs.");
				}
				bool flag2 = this.m_Tabs.IndexOf(value) == -1;
				if (flag2)
				{
					throw new Exception("The tab to be set as active does not exist in this TabView.");
				}
				bool flag3 = value == this.m_ActiveTab;
				if (!flag3)
				{
					Tab activeTab = this.m_ActiveTab;
					Tab activeTab2 = this.m_ActiveTab;
					if (activeTab2 != null)
					{
						activeTab2.SetInactive();
					}
					this.m_ActiveTab = value;
					Tab activeTab3 = this.m_ActiveTab;
					if (activeTab3 != null)
					{
						activeTab3.SetActive();
					}
					bool flag4 = !this.m_ApplyingViewState;
					if (flag4)
					{
						this.SaveViewState();
					}
					Action<Tab, Tab> action = this.activeTabChanged;
					if (action != null)
					{
						action(activeTab, value);
					}
				}
			}
		}

		public int selectedTabIndex
		{
			get
			{
				bool flag = this.activeTab == null || this.m_Tabs.Count == 0;
				int num;
				if (flag)
				{
					num = -1;
				}
				else
				{
					num = this.m_Tabs.IndexOf(this.activeTab);
				}
				return num;
			}
			set
			{
				bool flag = value >= 0 && this.m_Tabs.Count > value;
				if (flag)
				{
					this.activeTab = this.m_Tabs[value];
				}
			}
		}

		[CreateProperty]
		public bool reorderable
		{
			get
			{
				return this.m_Reorderable;
			}
			set
			{
				bool flag = this.m_Reorderable == value;
				if (!flag)
				{
					this.m_Reorderable = value;
					base.EnableInClassList(TabView.reorderableUssClassName, value);
					foreach (Tab tab in this.m_Tabs)
					{
						tab.EnableTabDragHandles(value);
					}
					base.NotifyPropertyChanged(in TabView.reorderableProperty);
				}
			}
		}

		public TabView()
		{
			base.AddToClassList(TabView.ussClassName);
			this.contentViewport = new VisualElement();
			this.contentViewport.AddToClassList(TabView.viewportUssClassName);
			this.contentViewport.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
			this.contentViewport.pickingMode = PickingMode.Ignore;
			base.hierarchy.Add(this.contentViewport);
			this.m_HeaderContainer = new VisualElement
			{
				name = TabView.headerContainerClassName,
				classList = { TabView.headerContainerClassName }
			};
			this.header.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
			this.contentViewport.Add(this.m_HeaderContainer);
			this.m_ContentContainer = new TabView.TabViewContentContainer
			{
				name = TabView.contentContainerUssClassName,
				classList = { TabView.contentContainerUssClassName }
			};
			base.hierarchy.Add(this.m_ContentContainer);
			this.nextButton = new RepeatButton(new Action(this.OnNextClicked), 250L, 30L)
			{
				classList = { TabView.nextButtonUssClassName }
			};
			this.previousButton = new RepeatButton(new Action(this.OnPreviousClicked), 250L, 30L)
			{
				classList = { TabView.previousButtonUssClassName }
			};
			this.contentViewport.Add(this.nextButton);
			this.contentViewport.Add(this.previousButton);
			base.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanel), TrickleDown.NoTrickleDown);
		}

		internal override void OnViewDataReady()
		{
			try
			{
				this.m_ApplyingViewState = true;
				base.OnViewDataReady();
				string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
				this.m_ViewState = base.GetOrCreateViewData<TabView.ViewState>(this.m_ViewState, fullHierarchicalViewDataKey);
				this.m_ViewState.Apply(this);
			}
			finally
			{
				this.m_ApplyingViewState = false;
			}
		}

		private void OnDetachFromPanel(DetachFromPanelEvent evt)
		{
			bool flag = evt.originPanel == null;
			if (!flag)
			{
				this.header.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
				this.contentViewport.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
			}
		}

		private void OnGeometryChanged(GeometryChangedEvent evt)
		{
			bool flag = evt.oldRect.size == evt.newRect.size;
			if (!flag)
			{
				Vector3 translate = this.m_HeaderContainer.resolvedStyle.translate;
				Vector3 vector = translate;
				bool flag2 = (!this.needsButtons || this.nextButton.resolvedStyle.display == DisplayStyle.None) && translate.x < 0f;
				if (flag2)
				{
					vector.x = Math.Min(0f, translate.x + (this.contentViewport.worldBound.xMax - this.m_HeaderContainer.worldBound.xMax));
				}
				bool flag3 = vector != translate;
				if (flag3)
				{
					this.m_HeaderContainer.style.translate = vector;
				}
				this.UpdateButtons(vector);
			}
		}

		private void OnNextClicked()
		{
			Vector3 translate = this.m_HeaderContainer.resolvedStyle.translate;
			VisualElement visualElement = this.m_TabHeaders.Find(delegate(VisualElement tab)
			{
				bool flag2 = tab.worldBound.xMax - this.contentViewport.worldBound.xMax < 50f && this.m_TabHeaders.IndexOf(tab) != this.m_TabHeaders.Count - 1;
				return !flag2 && tab.worldBound.xMax >= this.contentViewport.worldBound.xMax;
			});
			bool flag = visualElement != null;
			if (flag)
			{
				translate.x = -(visualElement.layout.xMax + this.nextButton.layout.width - this.contentViewport.layout.xMax);
				translate.x = Mathf.Max(translate.x, -this.scrollableWidth);
				this.m_HeaderContainer.style.translate = translate;
			}
			this.UpdateButtons(translate);
		}

		private void OnPreviousClicked()
		{
			Vector3 translate = this.m_HeaderContainer.resolvedStyle.translate;
			VisualElement visualElement = this.m_TabHeaders.FindLast(delegate(VisualElement tab)
			{
				bool flag2 = this.contentViewport.worldBound.xMin - tab.worldBound.xMin < 50f && this.m_TabHeaders.IndexOf(tab) != 0;
				return !flag2 && tab.worldBound.xMin <= this.contentViewport.worldBound.xMin;
			});
			bool flag = visualElement != null;
			if (flag)
			{
				translate.x = this.contentViewport.layout.xMin + this.previousButton.layout.width - visualElement.layout.xMin;
				translate.x = Mathf.Min(translate.x, 0f);
				this.m_HeaderContainer.style.translate = translate;
			}
			this.UpdateButtons(translate);
		}

		internal void UpdateButtons(Vector3 contentTransform)
		{
			this.nextButton.style.display = ((contentTransform.x > -this.scrollableWidth) ? DisplayStyle.Flex : DisplayStyle.None);
			this.previousButton.style.display = ((contentTransform.x < 0f) ? DisplayStyle.Flex : DisplayStyle.None);
		}

		private void SaveViewState()
		{
			bool applyingViewState = this.m_ApplyingViewState;
			if (!applyingViewState)
			{
				TabView.ViewState viewState = this.m_ViewState;
				if (viewState != null)
				{
					viewState.Save(this);
				}
				base.SaveViewData();
			}
		}

		private void UpdateIndexes()
		{
			for (int i = 0; i < this.m_Tabs.Count; i++)
			{
				this.m_Tabs[i].index = i;
			}
		}

		private void OnElementAdded(VisualElement ve)
		{
			Tab tab = ve as Tab;
			bool flag = tab == null || this.m_Reordering;
			if (!flag)
			{
				VisualElement tabHeader = tab.tabHeader;
				bool flag2 = tabHeader != null;
				if (flag2)
				{
					int num = this.m_ContentContainer.IndexOf(tab);
					this.m_HeaderContainer.Insert(num, tabHeader);
					this.m_TabHeaders.Insert(num, tabHeader);
					this.m_Tabs.Insert(num, tab);
					tab.EnableTabDragHandles(this.m_Reorderable);
					tab.closed += this.OnTabClosed;
				}
				tab.selected += this.OnTabSelected;
				this.UpdateIndexes();
				bool flag3 = this.activeTab == null;
				if (flag3)
				{
					this.activeTab = tab;
				}
			}
		}

		private void OnElementRemoved(VisualElement ve)
		{
			Tab tab = ve as Tab;
			bool flag = tab == null || this.m_Reordering;
			if (!flag)
			{
				VisualElement tabHeader = tab.tabHeader;
				this.m_HeaderContainer.Remove(tabHeader);
				this.m_TabHeaders.Remove(tabHeader);
				this.m_Tabs.Remove(tab);
				tab.EnableTabDragHandles(false);
				tab.hierarchy.Insert(0, tabHeader);
				tab.SetInactive();
				this.UpdateIndexes();
				bool flag2 = this.activeTab == tab && this.m_Tabs.Count > 0;
				if (flag2)
				{
					this.activeTab = this.m_Tabs[0];
				}
				else
				{
					bool flag3 = this.m_Tabs.Count == 0;
					if (flag3)
					{
						this.m_ActiveTab = null;
					}
				}
			}
		}

		private void OnTabSelected(Tab tab)
		{
			this.activeTab = tab;
		}

		private void OnTabClosed(Tab tab)
		{
			Action<Tab, int> action = this.tabClosed;
			if (action != null)
			{
				action(tab, tab.index);
			}
		}

		public void ReorderTab(int from, int to)
		{
			VisualElement visualElement = this.m_TabHeaders[from];
			Tab tab = this.m_Tabs[from];
			bool flag = !visualElement.visible || !this.reorderable || from == to;
			if (!flag)
			{
				this.m_Reordering = true;
				this.m_TabHeaders.RemoveAt(from);
				this.m_TabHeaders.Insert(to, visualElement);
				this.m_Tabs.RemoveAt(from);
				this.m_Tabs.Insert(to, tab);
				this.m_HeaderContainer.Insert(to, visualElement);
				base.Insert(to, tab);
				this.m_Reordering = false;
				this.UpdateIndexes();
				Action<int, int> action = this.tabReordered;
				if (action != null)
				{
					action(from, to);
				}
				bool flag2 = !this.m_ApplyingViewState;
				if (flag2)
				{
					this.SaveViewState();
				}
			}
		}

		public Tab GetTab(int index)
		{
			bool flag = index < 0 || index >= this.m_Tabs.Count;
			Tab tab;
			if (flag)
			{
				tab = null;
			}
			else
			{
				tab = this.m_Tabs[index];
			}
			return tab;
		}

		public VisualElement GetTabHeader(int index)
		{
			bool flag = index < 0 || index >= this.m_Tabs.Count;
			VisualElement visualElement;
			if (flag)
			{
				visualElement = null;
			}
			else
			{
				visualElement = this.m_TabHeaders[index];
			}
			return visualElement;
		}

		internal Tab FindTabByKey(string key)
		{
			return this.m_Tabs.Find((Tab tab) => tab.viewDataKey == key);
		}

		internal static readonly BindingId reorderableProperty = "reorderable";

		public static readonly string ussClassName = "unity-tab-view";

		public static readonly string headerContainerClassName = TabView.ussClassName + "__header-container";

		public static readonly string contentContainerUssClassName = TabView.ussClassName + "__content-container";

		public static readonly string reorderableUssClassName = TabView.ussClassName + "__reorderable";

		public static readonly string verticalUssClassName = TabView.ussClassName + "__vertical";

		public static readonly string viewportUssClassName = TabView.ussClassName + "__content-viewport";

		public static readonly string nextButtonUssClassName = TabView.ussClassName + "__next-button";

		public static readonly string previousButtonUssClassName = TabView.ussClassName + "__previous-button";

		private VisualElement m_HeaderContainer;

		private VisualElement m_ContentContainer;

		private List<Tab> m_Tabs = new List<Tab>();

		private List<VisualElement> m_TabHeaders = new List<VisualElement>();

		private Tab m_ActiveTab;

		private TabView.ViewState m_ViewState;

		private bool m_ApplyingViewState;

		private bool m_Reordering;

		private const float k_SizeThreshold = 0.001f;

		private const float k_PixelThreshold = 50f;

		private bool m_Reorderable;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : VisualElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(TabView.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("reorderable", "reorderable", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new TabView();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.reorderable_UxmlAttributeFlags);
				if (flag)
				{
					TabView tabView = (TabView)obj;
					tabView.reorderable = this.reorderable;
				}
			}

			[SerializeField]
			private bool reorderable;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags reorderable_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<TabView, TabView.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				TabView tabView = (TabView)ve;
				tabView.reorderable = this.m_Reorderable.GetValueFromBag(bag, cc);
			}

			private readonly UxmlBoolAttributeDescription m_Reorderable = new UxmlBoolAttributeDescription
			{
				name = "reorderable",
				defaultValue = false
			};
		}

		[Serializable]
		private class ViewState : ISerializationCallbackReceiver
		{
			internal void Save(TabView tabView)
			{
				this.m_HasPersistedData = true;
				bool flag = tabView.m_ActiveTab != null;
				if (flag)
				{
					this.m_ActiveTabKey = tabView.m_ActiveTab.viewDataKey;
				}
				this.m_TabOrder.Clear();
				bool flag2 = !tabView.reorderable;
				if (!flag2)
				{
					foreach (Tab tab in tabView.tabs)
					{
						this.m_TabOrder.Add(tab.viewDataKey);
					}
				}
			}

			internal void Apply(TabView tabView)
			{
				bool flag = !this.m_HasPersistedData;
				if (!flag)
				{
					int num = Math.Min(this.m_TabOrder.Count, tabView.tabs.Count);
					int num2 = 0;
					Tab tab = tabView.FindTabByKey(this.m_ActiveTabKey);
					bool flag2 = tab != null;
					if (flag2)
					{
						tabView.activeTab = tab;
					}
					bool flag3 = !tabView.reorderable;
					if (!flag3)
					{
						int num3 = 0;
						while (num3 < this.m_TabOrder.Count && num2 < num)
						{
							string text = this.m_TabOrder[num3];
							Tab tab2 = tabView.FindTabByKey(text);
							bool flag4 = tab2 != null;
							if (flag4)
							{
								int num4 = tabView.tabs.IndexOf(tab2);
								tabView.ReorderTab(num4, num2++);
							}
							num3++;
						}
					}
				}
			}

			public void OnBeforeSerialize()
			{
				this.m_HasPersistedData = true;
			}

			public void OnAfterDeserialize()
			{
				this.m_HasPersistedData = true;
			}

			private bool m_HasPersistedData;

			[SerializeField]
			private List<string> m_TabOrder = new List<string>();

			[SerializeField]
			private string m_ActiveTabKey;
		}

		private class TabViewContentContainer : VisualElement
		{
			internal override void OnChildAdded(VisualElement ve)
			{
				((TabView)base.parent).OnElementAdded(ve);
			}

			internal override void OnChildRemoved(VisualElement ve)
			{
				((TabView)base.parent).OnElementRemoved(ve);
			}
		}
	}
}
