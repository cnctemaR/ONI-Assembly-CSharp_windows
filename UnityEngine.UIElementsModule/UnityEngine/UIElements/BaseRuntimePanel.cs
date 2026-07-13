using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule", "UnityEngine.VectorGraphicsModule" })]
	internal abstract class BaseRuntimePanel : Panel
	{
		public GameObject selectableGameObject
		{
			get
			{
				return this.m_SelectableGameObject;
			}
			set
			{
				bool flag = this.m_SelectableGameObject != value;
				if (flag)
				{
					this.AssignPanelToComponents(null);
					this.m_SelectableGameObject = value;
					this.AssignPanelToComponents(this);
				}
			}
		}

		public float sortingPriority
		{
			get
			{
				return this.m_SortingPriority;
			}
			set
			{
				bool flag = !Mathf.Approximately(this.m_SortingPriority, value);
				if (flag)
				{
					this.m_SortingPriority = value;
					bool flag2 = this.contextType == ContextType.Player;
					if (flag2)
					{
						UIElementsRuntimeUtility.SetPanelOrderingDirty();
					}
				}
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action destroyed;

		protected BaseRuntimePanel(ScriptableObject ownerObject, EventDispatcher dispatcher = null)
			: base(ownerObject, ContextType.Player, dispatcher)
		{
			this.m_RuntimePanelCreationIndex = BaseRuntimePanel.s_CurrentRuntimePanelCounter++;
		}

		protected override void Dispose(bool disposing)
		{
			bool disposed = base.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					Action action = this.destroyed;
					if (action != null)
					{
						action();
					}
				}
				base.Dispose(disposing);
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action drawsInCamerasChanged;

		private void InvokeDrawsInCamerasChanged()
		{
			Action action = this.drawsInCamerasChanged;
			if (action != null)
			{
				action();
			}
		}

		internal bool drawsInCameras
		{
			get
			{
				return this.m_DrawsInCameras;
			}
			set
			{
				bool flag = this.m_DrawsInCameras != value;
				if (flag)
				{
					this.m_DrawsInCameras = value;
					UIElementsRuntimeUtility.SetPanelsDrawInCameraDirty();
					this.InvokeDrawsInCamerasChanged();
				}
			}
		}

		internal float pixelsPerUnit
		{
			get
			{
				return this.m_PixelsPerUnit;
			}
			set
			{
				this.m_PixelsPerUnit = value;
			}
		}

		internal int targetDisplay { get; set; }

		internal int screenRenderingWidth
		{
			get
			{
				return BaseRuntimePanel.getScreenRenderingWidth(this.targetDisplay);
			}
		}

		internal int screenRenderingHeight
		{
			get
			{
				return BaseRuntimePanel.getScreenRenderingHeight(this.targetDisplay);
			}
		}

		internal virtual void Update()
		{
			this.TickSchedulingUpdaters();
			this.ValidateLayout();
		}

		internal static int getScreenRenderingHeight(int display)
		{
			return (display >= 0 && display < Display.displays.Length) ? Display.displays[display].renderingHeight : Screen.height;
		}

		internal static int getScreenRenderingWidth(int display)
		{
			return (display >= 0 && display < Display.displays.Length) ? Display.displays[display].renderingWidth : Screen.width;
		}

		public override void Render()
		{
			bool drawsInCameras = this.drawsInCameras;
			if (drawsInCameras)
			{
				Debug.LogError("Panel.Render() must not be called on a panel that draws in cameras.");
			}
			else
			{
				bool flag = this.ownerObject == null;
				if (!flag)
				{
					bool flag2 = this.targetTexture == null;
					if (flag2)
					{
						RenderTexture active = RenderTexture.active;
						int num = ((active != null) ? active.width : this.screenRenderingWidth);
						int num2 = ((active != null) ? active.height : this.screenRenderingHeight);
						GL.Viewport(new Rect(0f, 0f, (float)num, (float)num2));
						base.Render();
					}
					else
					{
						Camera current = Camera.current;
						RenderTexture active2 = RenderTexture.active;
						Camera.SetupCurrent(null);
						RenderTexture.active = this.targetTexture;
						GL.Viewport(new Rect(0f, 0f, (float)this.targetTexture.width, (float)this.targetTexture.height));
						base.Render();
						Camera.SetupCurrent(current);
						RenderTexture.active = active2;
					}
				}
			}
		}

		public Func<Vector2, Vector3> screenToPanelSpace
		{
			get
			{
				return this.m_ScreenToPanelSpace;
			}
			set
			{
				this.m_ScreenToPanelSpace = value ?? BaseRuntimePanel.DefaultScreenToPanelSpace;
			}
		}

		internal Vector3 ScreenToPanel(Vector2 screen)
		{
			return this.screenToPanelSpace(screen) / base.scale;
		}

		internal bool ScreenToPanel(Vector2 screenPosition, Vector2 screenDelta, out Vector3 panelPosition, bool allowOutside = false)
		{
			panelPosition = this.ScreenToPanel(screenPosition);
			bool flag = !allowOutside;
			if (flag)
			{
				Rect layout = this.visualTree.layout;
				bool flag2 = !layout.Contains(panelPosition);
				if (flag2)
				{
					return false;
				}
				Vector3 vector = this.ScreenToPanel(screenPosition - screenDelta);
				bool flag3 = !layout.Contains(vector);
				if (flag3)
				{
					return true;
				}
			}
			return true;
		}

		private void AssignPanelToComponents(BaseRuntimePanel panel)
		{
			bool flag = this.selectableGameObject == null;
			if (!flag)
			{
				List<IRuntimePanelComponent> list;
				using (CollectionPool<List<IRuntimePanelComponent>, IRuntimePanelComponent>.Get(out list))
				{
					this.selectableGameObject.GetComponents<IRuntimePanelComponent>(list);
					foreach (IRuntimePanelComponent runtimePanelComponent in list)
					{
						runtimePanelComponent.panel = panel;
					}
				}
			}
		}

		internal void PointerLeavesPanel(int pointerId)
		{
			base.ClearCachedElementUnderPointer(pointerId, null);
			base.CommitElementUnderPointers();
			PointerDeviceState.SavePointerPosition(pointerId, BaseVisualElementPanel.s_OutsidePanelCoordinates, null, this.contextType);
		}

		internal void PointerEntersPanel(int pointerId, Vector3 position)
		{
			PointerDeviceState.SavePointerPosition(pointerId, position, this, this.contextType);
		}

		private GameObject m_SelectableGameObject;

		private static int s_CurrentRuntimePanelCounter = 0;

		internal readonly int m_RuntimePanelCreationIndex;

		private float m_SortingPriority = 0f;

		internal int resolvedSortingIndex = 0;

		private bool m_DrawsInCameras;

		private float m_PixelsPerUnit = 100f;

		internal RenderTexture targetTexture = null;

		internal static readonly Func<Vector2, Vector3> DefaultScreenToPanelSpace = (Vector2 p) => p;

		private Func<Vector2, Vector3> m_ScreenToPanelSpace = BaseRuntimePanel.DefaultScreenToPanelSpace;
	}
}
