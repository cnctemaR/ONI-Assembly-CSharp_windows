using System;

namespace UnityEngine.EventSystems
{
	public interface IPointerMoveHandler : IEventSystemHandler
	{
		void OnPointerMove(PointerEventData eventData);
	}
}
