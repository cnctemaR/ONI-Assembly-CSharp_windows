using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class Panel : BaseVisualElementPanel
	{
		public Panel(ScriptableObject ownerObject, ContextType contextType, IDataWatchService dataWatch = null, EventDispatcher dispatcher = null)
		{
			this.m_VisualTreeUpdater = new VisualTreeUpdater(this);
			this.ownerObject = ownerObject;
			this.contextType = contextType;
			this.m_DataWatch = dataWatch;
			this.dispatcher = dispatcher ?? EventDispatcher.instance;
			this.repaintData = new RepaintData();
			this.cursorManager = new CursorManager();
			this.contextualMenuManager = null;
			this.m_RootContainer = new VisualElement();
			this.m_RootContainer.name = VisualElementUtils.GetUniqueName("PanelContainer");
			this.m_RootContainer.persistenceKey = "PanelContainer";
			this.visualTree.SetPanel(this);
			this.focusController = new FocusController(new VisualElementFocusRing(this.visualTree, VisualElementFocusRing.DefaultFocusOrder.ChildOrder));
			this.m_ProfileUpdateName = "PanelUpdate";
			this.m_ProfileLayoutName = "PanelLayout";
			this.m_ProfileBindingsName = "PanelBindings";
			base.allowPixelCaching = true;
		}

		public override VisualElement visualTree
		{
			get
			{
				return this.m_RootContainer;
			}
		}

		public override EventDispatcher dispatcher { get; protected set; }

		internal override IDataWatchService dataWatch
		{
			get
			{
				return this.m_DataWatch;
			}
		}

		public TimerEventScheduler timerEventScheduler
		{
			get
			{
				TimerEventScheduler timerEventScheduler;
				if ((timerEventScheduler = this.m_Scheduler) == null)
				{
					timerEventScheduler = (this.m_Scheduler = new TimerEventScheduler());
				}
				return timerEventScheduler;
			}
		}

		internal override IScheduler scheduler
		{
			get
			{
				return this.timerEventScheduler;
			}
		}

		public override ScriptableObject ownerObject { get; protected set; }

		public override ContextType contextType { get; protected set; }

		public override SavePersistentViewData savePersistentViewData { get; set; }

		public override GetViewDataDictionary getViewDataDictionary { get; set; }

		public override FocusController focusController { get; set; }

		public override EventInterests IMGUIEventInterests { get; set; }

		internal string name
		{
			get
			{
				return this.m_PanelName;
			}
			set
			{
				this.m_PanelName = value;
				if (!string.IsNullOrEmpty(this.m_PanelName))
				{
					this.m_ProfileUpdateName = string.Format("PanelUpdate.{0}", this.m_PanelName);
					this.m_ProfileLayoutName = string.Format("PanelLayout.{0}", this.m_PanelName);
					this.m_ProfileBindingsName = string.Format("PanelBindings.{0}", this.m_PanelName);
				}
				else
				{
					this.m_ProfileUpdateName = "PanelUpdate";
					this.m_ProfileLayoutName = "PanelLayout";
					this.m_ProfileBindingsName = "PanelBindings";
				}
			}
		}

		internal static TimeMsFunction TimeSinceStartup
		{
			get
			{
				return Panel.s_TimeSinceStartup;
			}
			set
			{
				if (value == null)
				{
					value = new TimeMsFunction(Panel.DefaultTimeSinceStartupMs);
				}
				Panel.s_TimeSinceStartup = value;
			}
		}

		public override bool keepPixelCacheOnWorldBoundChange
		{
			get
			{
				return this.m_KeepPixelCacheOnWorldBoundChange;
			}
			set
			{
				if (this.m_KeepPixelCacheOnWorldBoundChange != value)
				{
					this.m_KeepPixelCacheOnWorldBoundChange = value;
					if (!value)
					{
						this.m_RootContainer.IncrementVersion(VersionChangeType.Transform | VersionChangeType.Repaint);
					}
				}
			}
		}

		public override int IMGUIContainersCount { get; set; }

		internal override uint version
		{
			get
			{
				return this.m_Version;
			}
		}

		internal override uint repaintVersion
		{
			get
			{
				return this.m_RepaintVersion;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!base.disposed)
			{
				if (disposing)
				{
					this.m_VisualTreeUpdater.Dispose();
				}
				base.Dispose(disposing);
			}
		}

		public static long TimeSinceStartupMs()
		{
			return (Panel.s_TimeSinceStartup != null) ? Panel.s_TimeSinceStartup() : Panel.DefaultTimeSinceStartupMs();
		}

		internal static long DefaultTimeSinceStartupMs()
		{
			return (long)(Time.realtimeSinceStartup * 1000f);
		}

		internal static VisualElement PickAll(VisualElement root, Vector2 point, List<VisualElement> picked = null)
		{
			return Panel.PerformPick(root, point, picked);
		}

		private static VisualElement PerformPick(VisualElement root, Vector2 point, List<VisualElement> picked = null)
		{
			VisualElement visualElement;
			if (!root.visible)
			{
				visualElement = null;
			}
			else if (root.pickingMode == PickingMode.Ignore && root.shadow.childCount == 0)
			{
				visualElement = null;
			}
			else
			{
				Vector3 vector = root.WorldToLocal(point);
				bool flag = root.ContainsPoint(vector);
				if (!flag && root.ShouldClip())
				{
					visualElement = null;
				}
				else
				{
					VisualElement visualElement2 = null;
					for (int i = root.shadow.childCount - 1; i >= 0; i--)
					{
						VisualElement visualElement3 = root.shadow[i];
						VisualElement visualElement4 = Panel.PerformPick(visualElement3, point, picked);
						if (visualElement2 == null && visualElement4 != null)
						{
							visualElement2 = visualElement4;
						}
					}
					if (picked != null && root.enabledInHierarchy && root.pickingMode == PickingMode.Position && flag)
					{
						picked.Add(root);
					}
					if (visualElement2 != null)
					{
						visualElement = visualElement2;
					}
					else
					{
						PickingMode pickingMode = root.pickingMode;
						if (pickingMode != PickingMode.Position)
						{
							if (pickingMode != PickingMode.Ignore)
							{
							}
						}
						else if (flag && root.enabledInHierarchy)
						{
							return root;
						}
						visualElement = null;
					}
				}
			}
			return visualElement;
		}

		public override VisualElement LoadTemplate(string path, Dictionary<string, VisualElement> slots = null)
		{
			VisualTreeAsset visualTreeAsset = Panel.loadResourceFunc(path, typeof(VisualTreeAsset)) as VisualTreeAsset;
			VisualElement visualElement;
			if (visualTreeAsset == null)
			{
				visualElement = null;
			}
			else
			{
				visualElement = visualTreeAsset.CloneTree(slots);
			}
			return visualElement;
		}

		public override VisualElement PickAll(Vector2 point, List<VisualElement> picked)
		{
			this.ValidateLayout();
			if (picked != null)
			{
				picked.Clear();
			}
			return Panel.PickAll(this.visualTree, point, picked);
		}

		public override VisualElement Pick(Vector2 point)
		{
			return Panel.PickAll(this.visualTree, point, null);
		}

		public override void ValidateLayout()
		{
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Layout);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.TransformClip);
		}

		public override void UpdateBindings()
		{
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Bindings);
		}

		public override void ApplyStyles()
		{
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
		}

		public override void DirtyStyleSheets()
		{
			this.m_VisualTreeUpdater.DirtyStyleSheets();
		}

		public override void Repaint(Event e)
		{
			Debug.Assert(GUIClip.Internal_GetCount() == 0, "UIElement is not compatible with IMGUI GUIClips, only GUIClip.ParentClipScope");
			this.m_RepaintVersion = this.version;
			if (!Mathf.Approximately(base.currentPixelsPerPoint, GUIUtility.pixelsPerPoint))
			{
				base.currentPixelsPerPoint = GUIUtility.pixelsPerPoint;
				this.visualTree.IncrementVersion(VersionChangeType.StyleSheet);
			}
			this.repaintData.repaintEvent = e;
			this.m_VisualTreeUpdater.UpdateVisualTree();
		}

		internal override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			this.m_Version += 1U;
			this.m_VisualTreeUpdater.OnVersionChanged(ve, versionChangeType);
		}

		internal override void SetUpdater(IVisualTreeUpdater updater, VisualTreeUpdatePhase phase)
		{
			this.m_VisualTreeUpdater.SetUpdater(updater, phase);
		}

		internal override IVisualTreeUpdater GetUpdater(VisualTreeUpdatePhase phase)
		{
			return this.m_VisualTreeUpdater.GetUpdater(phase);
		}

		private VisualElement m_RootContainer;

		private VisualTreeUpdater m_VisualTreeUpdater;

		private string m_PanelName;

		private string m_ProfileUpdateName;

		private string m_ProfileLayoutName;

		private string m_ProfileBindingsName;

		private uint m_Version = 0U;

		private uint m_RepaintVersion = 0U;

		private IDataWatchService m_DataWatch;

		private TimerEventScheduler m_Scheduler;

		internal static LoadResourceFunction loadResourceFunc = null;

		private static TimeMsFunction s_TimeSinceStartup;

		private bool m_KeepPixelCacheOnWorldBoundChange;
	}
}
