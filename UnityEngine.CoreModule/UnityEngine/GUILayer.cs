using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>Component added to a camera to make it render 2D GUI elements.</para>
	/// </summary>
	[RequireComponent(typeof(Camera))]
	[Obsolete("This component is part of the legacy UI system and will be removed in a future release.")]
	public class GUILayer : Behaviour
	{
		/// <summary>
		///   <para>Get the GUI element at a specific screen position.</para>
		/// </summary>
		/// <param name="screenPosition"></param>
		public GUIElement HitTest(Vector3 screenPosition)
		{
			return this.HitTest(new Vector2(screenPosition.x, screenPosition.y));
		}

		private GUIElement HitTest(Vector2 screenPosition)
		{
			return this.HitTest_Injected(ref screenPosition);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern GUIElement HitTest_Injected(ref Vector2 screenPosition);
	}
}
