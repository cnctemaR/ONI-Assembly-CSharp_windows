using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UIElements
{
	internal class UIToolkitInteroperabilityBridge
	{
		internal EventSystem eventSystem
		{
			get
			{
				return this.m_EventSystem;
			}
			set
			{
				if (this.m_EventSystem == value)
				{
					return;
				}
				this.m_EventSystem = value;
			}
		}

		public bool overrideUIToolkitEvents
		{
			get
			{
				return this.m_OverrideUIToolkitEvents;
			}
			internal set
			{
				this.m_OverrideUIToolkitEvents = value;
				this.ApplyOverrideUIToolkitEvents();
			}
		}

		public UIToolkitInteroperabilityBridge.EventHandlerTypes handlerTypes
		{
			get
			{
				return this.m_HandlerTypes;
			}
			internal set
			{
				this.m_HandlerTypes = value;
				this.ApplyOtherProperties();
			}
		}

		public int worldPickingLayers
		{
			get
			{
				return this.m_WorldPickingLayers;
			}
			internal set
			{
				this.m_WorldPickingLayers = value;
			}
		}

		public float worldPickingMaxDistance
		{
			get
			{
				return this.m_WorldPickingMaxDistance;
			}
			internal set
			{
				this.m_WorldPickingMaxDistance = value;
			}
		}

		public bool createDefaultPanelComponents
		{
			get
			{
				return this.m_CreateDefaultPanelComponents;
			}
			internal set
			{
				this.m_CreateDefaultPanelComponents = value;
				this.ApplyOtherProperties();
			}
		}

		private bool shouldTrackPanels
		{
			get
			{
				return this.overrideUIToolkitEvents && this.createDefaultPanelComponents && this.m_Started && this.m_Enabled;
			}
		}

		private void StartTrackingUIToolkitPanels()
		{
			if (this.m_IsTrackingPanels || !this.shouldTrackPanels)
			{
				return;
			}
			foreach (BaseRuntimePanel baseRuntimePanel in UIElementsRuntimeUtility.GetSortedPlayerPanels())
			{
				this.StartTrackingPanel(baseRuntimePanel);
			}
			UIElementsRuntimeUtility.onCreatePanel += this.StartTrackingPanel;
			this.m_IsTrackingPanels = true;
		}

		private void StartTrackingPanel(BaseRuntimePanel panel)
		{
			this.trackedPanels.Add(panel);
		}

		private void StopTrackingUIToolkitPanels()
		{
			if (!this.m_IsTrackingPanels)
			{
				return;
			}
			UIElementsRuntimeUtility.onCreatePanel -= this.StartTrackingPanel;
			this.m_IsTrackingPanels = false;
			foreach (BaseRuntimePanel baseRuntimePanel in this.trackedPanels)
			{
				this.DestroyPanelGameObject(baseRuntimePanel);
			}
			this.trackedPanels.Clear();
			this.DestroyWorldSpacePanelGameObject();
		}

		private void UpdatePanelGameObject(BaseRuntimePanel panel)
		{
			UIToolkitInteroperabilityBridge.EventHandlerTypes eventHandlerTypes = (panel.isFlat ? UIToolkitInteroperabilityBridge.EventHandlerTypes.ScreenOverlay : UIToolkitInteroperabilityBridge.EventHandlerTypes.WorldSpace);
			if ((this.m_HandlerTypes & eventHandlerTypes) != (UIToolkitInteroperabilityBridge.EventHandlerTypes)0)
			{
				this.CreatePanelGameObject(panel);
				return;
			}
			this.DestroyPanelGameObject(panel);
		}

		private void CreatePanelGameObject(BaseRuntimePanel panel)
		{
			if (panel.selectableGameObject == null)
			{
				GameObject gameObject = new GameObject(panel.name, new Type[]
				{
					typeof(PanelEventHandler),
					typeof(PanelRaycaster)
				});
				gameObject.transform.SetParent(this.m_EventSystem.transform);
				panel.selectableGameObject = gameObject;
				Action action = (this.destroyedActions[panel] = delegate
				{
					this.DestroyPanelGameObject(panel);
				});
				panel.destroyed += action;
			}
		}

		private void DestroyPanelGameObject(BaseRuntimePanel panel)
		{
			GameObject selectableGameObject = panel.selectableGameObject;
			if (selectableGameObject != null)
			{
				Action action;
				if (!this.destroyedActions.Remove(panel, out action))
				{
					return;
				}
				panel.destroyed -= action;
				panel.selectableGameObject = null;
				UIRUtility.Destroy(selectableGameObject);
			}
		}

		private void CreateWorldSpacePanelGameObject()
		{
			this.ApplyCameraProperties();
			if (this.m_WorldSpaceGo == null)
			{
				GameObject gameObject = new GameObject("WorldDocumentRaycaster");
				gameObject.transform.SetParent(this.m_EventSystem.transform);
				if (this.m_InputSettings.defaultEventCameraIsMainCamera)
				{
					gameObject.AddComponent<WorldDocumentRaycaster>();
				}
				else
				{
					foreach (Camera camera in this.m_InputSettings.eventCameras)
					{
						gameObject.AddComponent<WorldDocumentRaycaster>().camera = camera;
					}
				}
				this.m_WorldSpaceGo = gameObject;
			}
		}

		private void DestroyWorldSpacePanelGameObject()
		{
			Object worldSpaceGo = this.m_WorldSpaceGo;
			this.m_WorldSpaceGo = null;
			UIRUtility.Destroy(worldSpaceGo);
		}

		public void Start()
		{
			this.m_Started = true;
			this.StartTrackingUIToolkitPanels();
		}

		public void OnEnable()
		{
			if (this.m_Enabled)
			{
				return;
			}
			this.m_Enabled = true;
			if (PanelInputConfiguration.current != null)
			{
				this.Apply(PanelInputConfiguration.current);
			}
			PanelInputConfiguration.onApply = (Action<PanelInputConfiguration>)Delegate.Combine(PanelInputConfiguration.onApply, new Action<PanelInputConfiguration>(this.Apply));
			if (this.m_Started)
			{
				this.StartTrackingUIToolkitPanels();
			}
			if (this.m_OverrideUIToolkitEvents)
			{
				UIElementsRuntimeUtility.RegisterEventSystem(this.m_EventSystem);
			}
		}

		public void OnDisable()
		{
			if (!this.m_Enabled)
			{
				return;
			}
			this.m_Enabled = false;
			PanelInputConfiguration.onApply = (Action<PanelInputConfiguration>)Delegate.Remove(PanelInputConfiguration.onApply, new Action<PanelInputConfiguration>(this.Apply));
			this.StopTrackingUIToolkitPanels();
			UIElementsRuntimeUtility.UnregisterEventSystem(this.m_EventSystem);
		}

		public void Update()
		{
			this.UpdatePanelGameObjects();
		}

		private void Apply(PanelInputConfiguration input)
		{
			this.m_InputSettings = ((input != null) ? input.settings : PanelInputConfiguration.Settings.Default);
			this.m_OverrideUIToolkitEvents = this.m_InputSettings.panelInputRedirection != PanelInputConfiguration.PanelInputRedirection.Never;
			this.m_HandlerTypes = UIToolkitInteroperabilityBridge.EventHandlerTypes.ScreenOverlay | (this.m_InputSettings.processWorldSpaceInput ? UIToolkitInteroperabilityBridge.EventHandlerTypes.WorldSpace : ((UIToolkitInteroperabilityBridge.EventHandlerTypes)0));
			this.m_WorldPickingLayers = this.m_InputSettings.interactionLayers;
			this.m_WorldPickingMaxDistance = this.m_InputSettings.maxInteractionDistance;
			this.m_CreateDefaultPanelComponents = this.m_InputSettings.autoCreatePanelComponents;
			this.ApplyOverrideUIToolkitEvents();
			this.ApplyCameraProperties();
			this.ApplyOtherProperties();
		}

		private void ApplyOverrideUIToolkitEvents()
		{
			if (this.m_OldOverrideUIToolkitEvents == this.m_OverrideUIToolkitEvents)
			{
				return;
			}
			this.m_OldOverrideUIToolkitEvents = this.m_OverrideUIToolkitEvents;
			if (!this.m_Enabled)
			{
				return;
			}
			if (this.m_OverrideUIToolkitEvents)
			{
				UIElementsRuntimeUtility.RegisterEventSystem(this.m_EventSystem);
			}
			else
			{
				UIElementsRuntimeUtility.UnregisterEventSystem(this.m_EventSystem);
			}
			this.UpdatePanelTracking();
		}

		private void ApplyCameraProperties()
		{
			bool flag = false;
			if (this.m_OldDefaultEventCameraIsMainCamera != this.m_InputSettings.defaultEventCameraIsMainCamera)
			{
				this.m_OldDefaultEventCameraIsMainCamera = this.m_InputSettings.defaultEventCameraIsMainCamera;
				flag = true;
			}
			if (!this.m_InputSettings.defaultEventCameraIsMainCamera)
			{
				int num = 0;
				foreach (Camera camera in this.m_InputSettings.eventCameras)
				{
					num = (num * 397) ^ camera.GetHashCode();
				}
				if (this.m_OldEventCamerasHash != (long)num)
				{
					this.m_OldEventCamerasHash = (long)num;
					flag = true;
				}
			}
			else
			{
				this.m_OldEventCamerasHash = 0L;
			}
			if (flag)
			{
				this.DestroyWorldSpacePanelGameObject();
			}
		}

		private void ApplyOtherProperties()
		{
			bool flag = false;
			if (this.m_OldHandlerTypes != this.m_HandlerTypes)
			{
				this.m_OldHandlerTypes = this.m_HandlerTypes;
				flag = true;
			}
			if (this.m_OldCreateDefaultPanelComponents != this.m_CreateDefaultPanelComponents)
			{
				this.m_OldCreateDefaultPanelComponents = this.m_CreateDefaultPanelComponents;
				flag = true;
			}
			if (flag)
			{
				this.UpdatePanelTracking();
			}
		}

		private void UpdatePanelTracking()
		{
			if (this.shouldTrackPanels)
			{
				this.StartTrackingUIToolkitPanels();
				return;
			}
			this.StopTrackingUIToolkitPanels();
		}

		private void UpdatePanelGameObjects()
		{
			if (!this.m_IsTrackingPanels)
			{
				return;
			}
			bool flag = false;
			foreach (BaseRuntimePanel baseRuntimePanel in this.trackedPanels)
			{
				if (baseRuntimePanel.disposed)
				{
					this.m_PanelsToRemove.Add(baseRuntimePanel);
				}
				else
				{
					this.UpdatePanelGameObject(baseRuntimePanel);
					flag |= !baseRuntimePanel.isFlat;
				}
			}
			foreach (BaseRuntimePanel baseRuntimePanel2 in this.m_PanelsToRemove)
			{
				this.trackedPanels.Remove(baseRuntimePanel2);
			}
			this.m_PanelsToRemove.Clear();
			if (flag && (this.m_HandlerTypes & UIToolkitInteroperabilityBridge.EventHandlerTypes.WorldSpace) != (UIToolkitInteroperabilityBridge.EventHandlerTypes)0)
			{
				this.CreateWorldSpacePanelGameObject();
				return;
			}
			this.DestroyWorldSpacePanelGameObject();
		}

		private EventSystem m_EventSystem;

		private bool m_OverrideUIToolkitEvents = true;

		private UIToolkitInteroperabilityBridge.EventHandlerTypes m_HandlerTypes = UIToolkitInteroperabilityBridge.EventHandlerTypes.ScreenOverlay | UIToolkitInteroperabilityBridge.EventHandlerTypes.WorldSpace;

		private LayerMask m_WorldPickingLayers = -5;

		private float m_WorldPickingMaxDistance = float.PositiveInfinity;

		private bool m_CreateDefaultPanelComponents = true;

		private bool m_Started;

		private bool m_Enabled;

		private bool m_IsTrackingPanels;

		private GameObject m_WorldSpaceGo;

		private readonly HashSet<BaseRuntimePanel> trackedPanels = new HashSet<BaseRuntimePanel>();

		private readonly Dictionary<BaseRuntimePanel, Action> destroyedActions = new Dictionary<BaseRuntimePanel, Action>();

		private PanelInputConfiguration.Settings m_InputSettings = PanelInputConfiguration.Settings.Default;

		private bool m_OldOverrideUIToolkitEvents = true;

		private UIToolkitInteroperabilityBridge.EventHandlerTypes m_OldHandlerTypes = UIToolkitInteroperabilityBridge.EventHandlerTypes.ScreenOverlay | UIToolkitInteroperabilityBridge.EventHandlerTypes.WorldSpace;

		private bool m_OldCreateDefaultPanelComponents = true;

		private bool m_OldDefaultEventCameraIsMainCamera = true;

		private long m_OldEventCamerasHash;

		private List<BaseRuntimePanel> m_PanelsToRemove = new List<BaseRuntimePanel>();

		[Flags]
		public enum EventHandlerTypes
		{
			ScreenOverlay = 1,
			WorldSpace = 2
		}
	}
}
