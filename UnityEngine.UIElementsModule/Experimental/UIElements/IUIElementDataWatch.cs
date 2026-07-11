using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IUIElementDataWatch
	{
		IUIElementDataWatchRequest RegisterWatch(Object toWatch, Action<Object> watchNotification);

		/// <summary>
		///   <para>Unregisters a previously watched request.</para>
		/// </summary>
		/// <param name="requested">The registered request.</param>
		void UnregisterWatch(IUIElementDataWatchRequest requested);
	}
}
