using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineInternal;

namespace UnityEngine.UIElements
{
	[AddComponentMenu("UI Toolkit/Panel Raycaster (UI Toolkit)")]
	public class PanelRaycaster : BaseRaycaster, IRuntimePanelComponent
	{
		public IPanel panel
		{
			get
			{
				return this.m_Panel;
			}
			set
			{
				BaseRuntimePanel baseRuntimePanel = (BaseRuntimePanel)value;
				if (this.m_Panel != baseRuntimePanel)
				{
					this.UnregisterCallbacks();
					this.m_Panel = baseRuntimePanel;
					this.RegisterCallbacks();
				}
			}
		}

		private void RegisterCallbacks()
		{
			if (this.m_Panel != null)
			{
				this.m_Panel.destroyed += this.OnPanelDestroyed;
			}
		}

		private void UnregisterCallbacks()
		{
			if (this.m_Panel != null)
			{
				this.m_Panel.destroyed -= this.OnPanelDestroyed;
			}
		}

		private void OnPanelDestroyed()
		{
			this.panel = null;
		}

		private GameObject selectableGameObject
		{
			get
			{
				BaseRuntimePanel panel = this.m_Panel;
				if (panel == null)
				{
					return null;
				}
				return panel.selectableGameObject;
			}
		}

		public override int sortOrderPriority
		{
			get
			{
				BaseRuntimePanel panel = this.m_Panel;
				return Mathf.FloorToInt((panel != null) ? panel.sortingPriority : 0f);
			}
		}

		public override int renderOrderPriority
		{
			get
			{
				int maxValue = int.MaxValue;
				int s_ResolvedSortingIndexMax = UIElementsRuntimeUtility.s_ResolvedSortingIndexMax;
				BaseRuntimePanel panel = this.m_Panel;
				return maxValue - (s_ResolvedSortingIndexMax - ((panel != null) ? panel.resolvedSortingIndex : 0));
			}
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (this.m_Panel == null || !this.m_Panel.isFlat)
			{
				return;
			}
			int targetDisplay = this.m_Panel.targetDisplay;
			Vector3 relativeMousePositionForRaycast = MultipleDisplayUtilities.GetRelativeMousePositionForRaycast(eventData);
			Vector3 vector = relativeMousePositionForRaycast;
			Vector2 delta = eventData.delta;
			float num = (float)Screen.height;
			if (DisplayInternal.IsASecondaryDisplayIndex(targetDisplay))
			{
				num = (float)Display.displays[targetDisplay].systemHeight;
			}
			vector.y = num - vector.y;
			delta.y = -delta.y;
			BaseInputModule currentInputModule = eventData.currentInputModule;
			if (currentInputModule == null)
			{
				return;
			}
			int num2 = currentInputModule.ConvertUIToolkitPointerId(eventData);
			bool flag;
			if (!PanelRaycaster.panelPicker.TryPick((RuntimePanel)this.m_Panel, num2, vector, delta, new int?((int)relativeMousePositionForRaycast.z), out flag))
			{
				return;
			}
			resultAppendList.Add(new RaycastResult
			{
				gameObject = this.selectableGameObject,
				module = this,
				screenPosition = relativeMousePositionForRaycast,
				displayIndex = this.m_Panel.targetDisplay
			});
		}

		public override Camera eventCamera
		{
			get
			{
				return null;
			}
		}

		private BaseRuntimePanel m_Panel;

		private static ScreenOverlayPanelPicker panelPicker = new ScreenOverlayPanelPicker();
	}
}
