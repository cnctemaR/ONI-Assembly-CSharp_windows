using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	internal class CameraScreenRaycaster : IScreenRaycaster
	{
		public virtual void Update()
		{
			Array.Sort<Camera>(this.cameras, (Camera a, Camera b) => -a.depth.CompareTo(b.depth));
		}

		public IEnumerable<ValueTuple<Ray, Camera, bool>> MakeRay(Vector2 mousePosition, int pointerId, int? targetDisplay)
		{
			Camera camera = (this.singleCamera[0] = PointerDeviceState.GetCameraWithSoftPointerCapture(pointerId));
			return CameraScreenRaycaster.CameraRayEnumerator.GetPooled((camera != null) ? this.singleCamera : this.cameras, this.layerMask, mousePosition, targetDisplay);
		}

		private static bool IsValid(Camera camera, int layerMask, int? targetDisplay)
		{
			bool flag;
			if (camera != null && (camera.cullingMask & layerMask) != 0)
			{
				if (targetDisplay != null)
				{
					int targetDisplay2 = camera.targetDisplay;
					int? num = targetDisplay;
					flag = (targetDisplay2 == num.GetValueOrDefault()) & (num != null);
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		private static bool MakeRay(Camera camera, Vector2 mousePosition, out Ray ray)
		{
			Vector2 vector = UIElementsRuntimeUtility.PanelToScreenBottomLeftPosition(mousePosition, camera.targetDisplay);
			ray = camera.ScreenPointToRay(vector);
			return camera.pixelRect.Contains(vector);
		}

		public Camera[] cameras = Array.Empty<Camera>();

		public Camera[] singleCamera = new Camera[1];

		public int layerMask = -1;

		public class CameraRayEnumerator : IEnumerator<ValueTuple<Ray, Camera, bool>>, IEnumerator, IDisposable, IEnumerable<ValueTuple<Ray, Camera, bool>>, IEnumerable
		{
			public bool MoveNext()
			{
				bool flag;
				do
				{
					int num = this.m_Index + 1;
					this.m_Index = num;
					if (num >= this.m_Cameras.Length)
					{
						goto Block_2;
					}
					this.m_CurrentCamera = this.m_Cameras[this.m_Index];
					flag = !CameraScreenRaycaster.IsValid(this.m_CurrentCamera, this.m_LayerMask, this.m_TargetDisplay);
				}
				while (flag);
				this.m_IsInsideCameraRect = CameraScreenRaycaster.MakeRay(this.m_CurrentCamera, this.m_MousePosition, out this.m_CurrentRay);
				return true;
				Block_2:
				return false;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			public ValueTuple<Ray, Camera, bool> Current
			{
				get
				{
					return new ValueTuple<Ray, Camera, bool>(this.m_CurrentRay, this.m_CurrentCamera, this.m_IsInsideCameraRect);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public IEnumerator<ValueTuple<Ray, Camera, bool>> GetEnumerator()
			{
				return this;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public static CameraScreenRaycaster.CameraRayEnumerator GetPooled(Camera[] cameras, int layerMask, Vector2 mousePosition, int? targetDisplay)
			{
				CameraScreenRaycaster.CameraRayEnumerator cameraRayEnumerator = GenericPool<CameraScreenRaycaster.CameraRayEnumerator>.Get();
				cameraRayEnumerator.m_Cameras = cameras;
				cameraRayEnumerator.m_LayerMask = layerMask;
				cameraRayEnumerator.m_MousePosition = mousePosition;
				cameraRayEnumerator.m_TargetDisplay = targetDisplay;
				return cameraRayEnumerator;
			}

			public void Dispose()
			{
				this.Reset();
				this.m_Cameras = null;
				GenericPool<CameraScreenRaycaster.CameraRayEnumerator>.Release(this);
			}

			private Camera[] m_Cameras;

			private int m_LayerMask;

			private Vector2 m_MousePosition;

			private int? m_TargetDisplay;

			private int m_Index = -1;

			private Camera m_CurrentCamera;

			private Ray m_CurrentRay;

			private bool m_IsInsideCameraRect;
		}
	}
}
