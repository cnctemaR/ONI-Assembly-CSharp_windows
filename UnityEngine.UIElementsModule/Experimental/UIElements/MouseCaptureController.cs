using System;

namespace UnityEngine.Experimental.UIElements
{
	public static class MouseCaptureController
	{
		internal static IEventHandler mouseCapture { get; private set; }

		[Obsolete("Use IsMouseCaptured instead of IsMouseCaptureTaken.")]
		public static bool IsMouseCaptureTaken()
		{
			return MouseCaptureController.IsMouseCaptured();
		}

		public static bool IsMouseCaptured()
		{
			return MouseCaptureController.mouseCapture != null;
		}

		public static bool HasMouseCapture(this IEventHandler handler)
		{
			return MouseCaptureController.mouseCapture == handler;
		}

		[Obsolete("Use CaptureMouse instead of TakeMouseCapture.")]
		public static void TakeMouseCapture(this IEventHandler handler)
		{
			handler.CaptureMouse();
		}

		public static void CaptureMouse(this IEventHandler handler)
		{
			if (MouseCaptureController.mouseCapture != handler)
			{
				if (handler == null)
				{
					MouseCaptureController.ReleaseMouse();
				}
				else if (GUIUtility.hotControl != 0)
				{
					Debug.Log("Should not be capturing when there is a hotcontrol");
				}
				else
				{
					IEventHandler mouseCapture = MouseCaptureController.mouseCapture;
					MouseCaptureController.mouseCapture = handler;
					if (mouseCapture != null)
					{
						using (MouseCaptureOutEvent pooled = MouseCaptureEventBase<MouseCaptureOutEvent>.GetPooled(mouseCapture, MouseCaptureController.mouseCapture))
						{
							mouseCapture.SendEvent(pooled);
						}
					}
					using (MouseCaptureEvent pooled2 = MouseCaptureEventBase<MouseCaptureEvent>.GetPooled(MouseCaptureController.mouseCapture, mouseCapture))
					{
						MouseCaptureController.mouseCapture.SendEvent(pooled2);
					}
				}
			}
		}

		[Obsolete("Use ReleaseMouse instead of ReleaseMouseCapture.")]
		public static void ReleaseMouseCapture(this IEventHandler handler)
		{
			handler.ReleaseMouse();
		}

		public static void ReleaseMouse(this IEventHandler handler)
		{
			if (handler == MouseCaptureController.mouseCapture)
			{
				MouseCaptureController.ReleaseMouse();
			}
		}

		[Obsolete("Use ReleaseMouse instead of ReleaseMouseCapture.")]
		public static void ReleaseMouseCapture()
		{
			MouseCaptureController.ReleaseMouse();
		}

		public static void ReleaseMouse()
		{
			if (MouseCaptureController.mouseCapture != null)
			{
				IEventHandler mouseCapture = MouseCaptureController.mouseCapture;
				MouseCaptureController.mouseCapture = null;
				using (MouseCaptureOutEvent pooled = MouseCaptureEventBase<MouseCaptureOutEvent>.GetPooled(mouseCapture, null))
				{
					mouseCapture.SendEvent(pooled);
				}
			}
		}
	}
}
