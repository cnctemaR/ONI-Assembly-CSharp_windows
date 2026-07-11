using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IEventHandler
	{
		void SendEvent(EventBase e);

		void HandleEvent(EventBase evt);

		bool HasTrickleDownHandlers();

		bool HasBubbleUpHandlers();

		[Obsolete("Use HasTrickleDownHandlers instead of HasCaptureHandlers.")]
		bool HasCaptureHandlers();

		[Obsolete("Use HasBubbleUpHandlers instead of HasBubbleHandlers.")]
		bool HasBubbleHandlers();
	}
}
