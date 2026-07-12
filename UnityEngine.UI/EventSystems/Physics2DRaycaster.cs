using System;
using System.Collections.Generic;
using UnityEngine.U2D;
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
			Ray ray = default(Ray);
			float num = 0f;
			int num2 = 0;
			if (!base.ComputeRayAndDistance(eventData, ref ray, ref num2, ref num))
			{
				return;
			}
			int num3;
			if (base.maxRayIntersections == 0)
			{
				if (ReflectionMethodsCache.Singleton.getRayIntersectionAll == null)
				{
					return;
				}
				this.m_Hits = ReflectionMethodsCache.Singleton.getRayIntersectionAll(ray, num, base.finalEventMask);
				num3 = this.m_Hits.Length;
			}
			else
			{
				if (ReflectionMethodsCache.Singleton.getRayIntersectionAllNonAlloc == null)
				{
					return;
				}
				if (this.m_LastMaxRayIntersections != this.m_MaxRayIntersections)
				{
					this.m_Hits = new RaycastHit2D[base.maxRayIntersections];
					this.m_LastMaxRayIntersections = this.m_MaxRayIntersections;
				}
				num3 = ReflectionMethodsCache.Singleton.getRayIntersectionAllNonAlloc(ray, this.m_Hits, num, base.finalEventMask);
			}
			if (num3 != 0)
			{
				int i = 0;
				int num4 = num3;
				while (i < num4)
				{
					Renderer renderer = null;
					Renderer component = this.m_Hits[i].collider.gameObject.GetComponent<Renderer>();
					if (component != null)
					{
						if (component is SpriteRenderer)
						{
							renderer = component;
						}
						if (component is SpriteShapeRenderer)
						{
							renderer = component;
						}
					}
					RaycastResult raycastResult = new RaycastResult
					{
						gameObject = this.m_Hits[i].collider.gameObject,
						module = this,
						distance = Vector3.Distance(this.eventCamera.transform.position, this.m_Hits[i].point),
						worldPosition = this.m_Hits[i].point,
						worldNormal = this.m_Hits[i].normal,
						screenPosition = eventData.position,
						displayIndex = num2,
						index = (float)resultAppendList.Count,
						sortingLayer = ((renderer != null) ? renderer.sortingLayerID : 0),
						sortingOrder = ((renderer != null) ? renderer.sortingOrder : 0)
					};
					resultAppendList.Add(raycastResult);
					i++;
				}
			}
		}

		private RaycastHit2D[] m_Hits;
	}
}
