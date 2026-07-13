using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragMe : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		Canvas canvas = DragMe.FindInParents<Canvas>(base.gameObject);
		if (canvas == null)
		{
			return;
		}
		this.m_DraggingIcon = global::UnityEngine.Object.Instantiate<GameObject>(base.gameObject, canvas.transform, false);
		GraphicRaycaster component = this.m_DraggingIcon.GetComponent<GraphicRaycaster>();
		if (component != null)
		{
			component.enabled = false;
		}
		this.m_DraggingIcon.name = "dragObj";
		this.m_DraggingIcon.transform.SetAsLastSibling();
		RectTransform component2 = this.m_DraggingIcon.GetComponent<RectTransform>();
		component2.pivot = Vector2.zero;
		component2.sizeDelta = base.GetComponent<RectTransform>().rect.size;
		this.x = this.m_DraggingIcon.transform.position.x;
		Canvas component3 = this.m_DraggingIcon.GetComponent<Canvas>();
		component3.overrideSorting = true;
		component3.sortingOrder = 99;
		if (this.dragOnSurfaces)
		{
			this.m_DraggingPlane = base.transform as RectTransform;
		}
		else
		{
			this.m_DraggingPlane = canvas.transform as RectTransform;
		}
		this.SetDraggedPosition(eventData);
		this.listener.OnBeginDrag(eventData.position);
	}

	public void OnDrag(PointerEventData data)
	{
		if (this.m_DraggingIcon != null)
		{
			this.SetDraggedPosition(data);
		}
	}

	private void SetDraggedPosition(PointerEventData data)
	{
		if (this.dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
		{
			this.m_DraggingPlane = data.pointerEnter.transform as RectTransform;
		}
		RectTransform component = this.m_DraggingIcon.GetComponent<RectTransform>();
		Vector3 vector;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(this.m_DraggingPlane, data.position, data.pressEventCamera, out vector))
		{
			vector.x = this.x + 5f;
			vector.y -= component.sizeDelta.y / 2f;
			component.position = vector;
			component.rotation = this.m_DraggingPlane.rotation;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		this.listener.OnEndDrag(eventData.position);
		if (this.m_DraggingIcon != null)
		{
			global::UnityEngine.Object.Destroy(this.m_DraggingIcon);
		}
	}

	public static T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null)
		{
			return default(T);
		}
		T t = default(T);
		Transform transform = go.transform.parent;
		while (transform != null && t == null)
		{
			t = transform.gameObject.GetComponent<T>();
			transform = transform.parent;
		}
		return t;
	}

	public bool dragOnSurfaces = true;

	private GameObject m_DraggingIcon;

	private RectTransform m_DraggingPlane;

	private float x;

	public DragMe.IDragListener listener;

	public interface IDragListener
	{
		void OnBeginDrag(Vector2 position);

		void OnEndDrag(Vector2 position);
	}
}
