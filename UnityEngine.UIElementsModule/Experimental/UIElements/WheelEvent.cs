using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Mouse wheel event.</para>
	/// </summary>
	public class WheelEvent : MouseEventBase<WheelEvent>
	{
		/// <summary>
		///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
		/// </summary>
		public WheelEvent()
		{
			this.Init();
		}

		/// <summary>
		///   <para>The amount of scrolling applied on the mouse wheel.</para>
		/// </summary>
		public Vector3 delta { get; private set; }

		/// <summary>
		///   <para>Get an event from the event pool and initialize it with the given values. Use this function instead of creating new events. Events obtained from this method should be released back to the pool using ReleaseEvent().</para>
		/// </summary>
		/// <param name="systemEvent">A wheel IMGUI event.</param>
		/// <returns>
		///   <para>A wheel event.</para>
		/// </returns>
		public new static WheelEvent GetPooled(Event systemEvent)
		{
			WheelEvent pooled = MouseEventBase<WheelEvent>.GetPooled(systemEvent);
			pooled.imguiEvent = systemEvent;
			if (systemEvent != null)
			{
				pooled.delta = systemEvent.delta;
			}
			return pooled;
		}

		/// <summary>
		///   <para>Reset the event members to their initial value.</para>
		/// </summary>
		protected override void Init()
		{
			base.Init();
			this.delta = Vector3.zero;
		}
	}
}
