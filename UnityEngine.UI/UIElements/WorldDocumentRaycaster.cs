using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.UIElements
{
	[AddComponentMenu("UI Toolkit/World Document Raycaster (UI Toolkit)")]
	public class WorldDocumentRaycaster : BaseRaycaster
	{
		public override Camera eventCamera
		{
			get
			{
				return this.m_EventCamera;
			}
		}

		public Camera camera
		{
			get
			{
				return this.m_EventCamera;
			}
			set
			{
				this.m_EventCamera = value;
			}
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			BaseInputModule baseInputModule = ((EventSystem.current != null) ? EventSystem.current.currentInputModule : null);
			if (baseInputModule == null)
			{
				return;
			}
			Ray ray;
			float num;
			int num2;
			if (!this.GetWorldRay(eventData, out ray, out num, out num2))
			{
				return;
			}
			num = Mathf.Min(num, EventSystem.current.uiToolkitInterop.worldPickingMaxDistance);
			num2 &= EventSystem.current.uiToolkitInterop.worldPickingLayers;
			int num3 = baseInputModule.ConvertUIToolkitPointerId(eventData);
			Camera cameraWithSoftPointerCapture = PointerDeviceState.GetCameraWithSoftPointerCapture(num3);
			if (cameraWithSoftPointerCapture != null)
			{
				Camera camera = ((this.m_EventCamera != null) ? this.m_EventCamera : Camera.main);
				if (cameraWithSoftPointerCapture != camera)
				{
					return;
				}
			}
			Collider collider;
			UIDocument uidocument;
			VisualElement visualElement;
			float num4;
			bool flag;
			if (!WorldDocumentRaycaster.worldPicker.TryPickWithCapture(num3, ray, num, num2, out collider, out uidocument, out visualElement, out num4, out flag))
			{
				return;
			}
			resultAppendList.Add(new RaycastResult
			{
				gameObject = ((uidocument == null) ? base.gameObject : uidocument.containerPanel.selectableGameObject),
				origin = ray.origin,
				worldPosition = ray.origin + num4 * ray.direction,
				document = uidocument,
				element = visualElement,
				module = this,
				distance = num4,
				sortingOrder = (flag ? int.MaxValue : 0)
			});
		}

		protected virtual bool GetWorldRay(PointerEventData eventData, out Ray worldRay, out float maxDistance, out int layerMask)
		{
			Camera camera = ((this.m_EventCamera != null) ? this.m_EventCamera : Camera.main);
			if (camera == null)
			{
				worldRay = default(Ray);
				maxDistance = 0f;
				layerMask = 0;
				return false;
			}
			maxDistance = camera.farClipPlane;
			layerMask = camera.cullingMask;
			Vector3 relativeMousePositionForRaycast = MultipleDisplayUtilities.GetRelativeMousePositionForRaycast(eventData);
			if ((int)relativeMousePositionForRaycast.z != camera.targetDisplay)
			{
				worldRay = default(Ray);
				return false;
			}
			worldRay = camera.ScreenPointToRay(relativeMousePositionForRaycast);
			return true;
		}

		[SerializeField]
		private Camera m_EventCamera;

		private static PhysicsDocumentPicker worldPicker = new PhysicsDocumentPicker();
	}
}
