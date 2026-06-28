using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.EventSystems
{
	[AddComponentMenu("Event/Physics 2D Raycaster")]
	[RequireComponent(typeof(Camera))]
	public class Physics2DRaycaster : PhysicsRaycaster
	{
		protected Physics2DRaycaster()
		{
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (!(this.eventCamera == null))
			{
				Ray ray;
				float num;
				base.ComputeRayAndDistance(eventData, out ray, out num);
				if (ReflectionMethodsCache.Singleton.getRayIntersectionAll != null)
				{
					RaycastHit2D[] array = ReflectionMethodsCache.Singleton.getRayIntersectionAll(ray, num, base.finalEventMask);
					if (array.Length != 0)
					{
						int i = 0;
						int num2 = array.Length;
						while (i < num2)
						{
							SpriteRenderer component = array[i].collider.gameObject.GetComponent<SpriteRenderer>();
							RaycastResult raycastResult = new RaycastResult
							{
								gameObject = array[i].collider.gameObject,
								module = this,
								distance = Vector3.Distance(this.eventCamera.transform.position, array[i].point),
								worldPosition = array[i].point,
								worldNormal = array[i].normal,
								screenPosition = eventData.position,
								index = (float)resultAppendList.Count,
								sortingLayer = ((!(component != null)) ? 0 : component.sortingLayerID),
								sortingOrder = ((!(component != null)) ? 0 : component.sortingOrder)
							};
							resultAppendList.Add(raycastResult);
							i++;
						}
					}
				}
			}
		}
	}
}
