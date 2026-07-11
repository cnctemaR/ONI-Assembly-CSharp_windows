using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Event sent after layout calculations, when the position or the dimension of an element changes. This event cannot be captured, cannot be cancelled, and it does not bubble.</para>
	/// </summary>
	public class GeometryChangedEvent : EventBase<GeometryChangedEvent>, IPropagatableEvent
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public GeometryChangedEvent()
		{
			this.Init();
		}

		/// <summary>
		///   <para>Gets an event from the event pool and initializes the event with the specified values. Use this method instead of instancing new events. Use Dispose() to release events back to the event pool.</para>
		/// </summary>
		/// <param name="oldRect">The old dimensions of the element.</param>
		/// <param name="newRect">The new dimensions of the element.</param>
		/// <returns>
		///   <para>Returns an event from the pool.</para>
		/// </returns>
		public static GeometryChangedEvent GetPooled(Rect oldRect, Rect newRect)
		{
			GeometryChangedEvent pooled = EventBase<GeometryChangedEvent>.GetPooled();
			pooled.oldRect = oldRect;
			pooled.newRect = newRect;
			return pooled;
		}

		/// <summary>
		///   <para>Resets the event values to their initial values.</para>
		/// </summary>
		protected override void Init()
		{
			base.Init();
			this.oldRect = Rect.zero;
			this.newRect = Rect.zero;
		}

		/// <summary>
		///   <para>The old dimensions of the element.</para>
		/// </summary>
		public Rect oldRect { get; private set; }

		/// <summary>
		///   <para>The new dimensions of the element.</para>
		/// </summary>
		public Rect newRect { get; private set; }
	}
}
