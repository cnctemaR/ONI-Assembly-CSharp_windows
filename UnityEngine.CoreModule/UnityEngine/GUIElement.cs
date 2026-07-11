using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>Base class for images &amp; text strings displayed in a GUI.</para>
	/// </summary>
	[RequireComponent(typeof(Transform))]
	public class GUIElement : Behaviour
	{
		/// <summary>
		///   <para>Is a point on screen inside the element?</para>
		/// </summary>
		/// <param name="screenPosition"></param>
		/// <param name="camera"></param>
		[ExcludeFromDocs]
		public bool HitTest(Vector3 screenPosition)
		{
			return this.HitTest(new Vector2(screenPosition.x, screenPosition.y), null);
		}

		/// <summary>
		///   <para>Is a point on screen inside the element?</para>
		/// </summary>
		/// <param name="screenPosition"></param>
		/// <param name="camera"></param>
		public bool HitTest(Vector3 screenPosition, [DefaultValue("null")] Camera camera)
		{
			return this.HitTest(new Vector2(screenPosition.x, screenPosition.y), GUIElement.GetCameraOrWindowRect(camera));
		}

		/// <summary>
		///   <para>Returns bounding rectangle of GUIElement in screen coordinates.</para>
		/// </summary>
		/// <param name="camera"></param>
		public Rect GetScreenRect([DefaultValue("null")] Camera camera)
		{
			return this.GetScreenRect(GUIElement.GetCameraOrWindowRect(camera));
		}

		[ExcludeFromDocs]
		public Rect GetScreenRect()
		{
			return this.GetScreenRect(null);
		}

		private Rect GetScreenRect(Rect rect)
		{
			Rect rect2;
			this.GetScreenRect_Injected(ref rect, out rect2);
			return rect2;
		}

		private bool HitTest(Vector2 screenPosition, Rect cameraRect)
		{
			return this.HitTest_Injected(ref screenPosition, ref cameraRect);
		}

		private static Rect GetCameraOrWindowRect(Camera camera)
		{
			Rect rect;
			if (camera != null)
			{
				rect = camera.pixelRect;
			}
			else
			{
				rect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
			}
			return rect;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetScreenRect_Injected(ref Rect rect, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool HitTest_Injected(ref Vector2 screenPosition, ref Rect cameraRect);
	}
}
