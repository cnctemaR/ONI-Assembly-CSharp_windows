using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("Event/Graphic Raycaster")]
	[RequireComponent(typeof(Canvas))]
	public class GraphicRaycaster : BaseRaycaster
	{
		protected GraphicRaycaster()
		{
		}

		public override int sortOrderPriority
		{
			get
			{
				int num;
				if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					num = this.canvas.sortingOrder;
				}
				else
				{
					num = base.sortOrderPriority;
				}
				return num;
			}
		}

		public override int renderOrderPriority
		{
			get
			{
				int num;
				if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					num = this.canvas.rootCanvas.renderOrder;
				}
				else
				{
					num = base.renderOrderPriority;
				}
				return num;
			}
		}

		public bool ignoreReversedGraphics
		{
			get
			{
				return this.m_IgnoreReversedGraphics;
			}
			set
			{
				this.m_IgnoreReversedGraphics = value;
			}
		}

		public GraphicRaycaster.BlockingObjects blockingObjects
		{
			get
			{
				return this.m_BlockingObjects;
			}
			set
			{
				this.m_BlockingObjects = value;
			}
		}

		private Canvas canvas
		{
			get
			{
				Canvas canvas;
				if (this.m_Canvas != null)
				{
					canvas = this.m_Canvas;
				}
				else
				{
					this.m_Canvas = base.GetComponent<Canvas>();
					canvas = this.m_Canvas;
				}
				return canvas;
			}
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (!(this.canvas == null))
			{
				IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(this.canvas);
				if (graphicsForCanvas != null && graphicsForCanvas.Count != 0)
				{
					Camera eventCamera = this.eventCamera;
					int num;
					if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay || eventCamera == null)
					{
						num = this.canvas.targetDisplay;
					}
					else
					{
						num = eventCamera.targetDisplay;
					}
					Vector3 vector = Display.RelativeMouseAt(eventData.position);
					if (vector != Vector3.zero)
					{
						int num2 = (int)vector.z;
						if (num2 != num)
						{
							return;
						}
					}
					else
					{
						vector = eventData.position;
					}
					Vector2 vector2;
					if (eventCamera == null)
					{
						float num3 = (float)Screen.width;
						float num4 = (float)Screen.height;
						if (num > 0 && num < Display.displays.Length)
						{
							num3 = (float)Display.displays[num].systemWidth;
							num4 = (float)Display.displays[num].systemHeight;
						}
						vector2 = new Vector2(vector.x / num3, vector.y / num4);
					}
					else
					{
						vector2 = eventCamera.ScreenToViewportPoint(vector);
					}
					if (vector2.x >= 0f && vector2.x <= 1f && vector2.y >= 0f && vector2.y <= 1f)
					{
						float num5 = float.MaxValue;
						Ray ray = default(Ray);
						if (eventCamera != null)
						{
							ray = eventCamera.ScreenPointToRay(vector);
						}
						if (this.canvas.renderMode != RenderMode.ScreenSpaceOverlay && this.blockingObjects != GraphicRaycaster.BlockingObjects.None)
						{
							float num6 = 100f;
							if (eventCamera != null)
							{
								float z = ray.direction.z;
								num6 = ((!Mathf.Approximately(0f, z)) ? Mathf.Abs((eventCamera.farClipPlane - eventCamera.nearClipPlane) / z) : float.PositiveInfinity);
							}
							if (this.blockingObjects == GraphicRaycaster.BlockingObjects.ThreeD || this.blockingObjects == GraphicRaycaster.BlockingObjects.All)
							{
								if (ReflectionMethodsCache.Singleton.raycast3D != null)
								{
									RaycastHit[] array = ReflectionMethodsCache.Singleton.raycast3DAll(ray, num6, this.m_BlockingMask);
									if (array.Length > 0)
									{
										num5 = array[0].distance;
									}
								}
							}
							if (this.blockingObjects == GraphicRaycaster.BlockingObjects.TwoD || this.blockingObjects == GraphicRaycaster.BlockingObjects.All)
							{
								if (ReflectionMethodsCache.Singleton.raycast2D != null)
								{
									RaycastHit2D[] array2 = ReflectionMethodsCache.Singleton.getRayIntersectionAll(ray, num6, this.m_BlockingMask);
									if (array2.Length > 0)
									{
										num5 = array2[0].distance;
									}
								}
							}
						}
						this.m_RaycastResults.Clear();
						GraphicRaycaster.Raycast(this.canvas, eventCamera, vector, graphicsForCanvas, this.m_RaycastResults);
						int count = this.m_RaycastResults.Count;
						int i = 0;
						while (i < count)
						{
							GameObject gameObject = this.m_RaycastResults[i].gameObject;
							bool flag = true;
							if (this.ignoreReversedGraphics)
							{
								if (eventCamera == null)
								{
									Vector3 vector3 = gameObject.transform.rotation * Vector3.forward;
									flag = Vector3.Dot(Vector3.forward, vector3) > 0f;
								}
								else
								{
									Vector3 vector4 = eventCamera.transform.rotation * Vector3.forward;
									Vector3 vector5 = gameObject.transform.rotation * Vector3.forward;
									flag = Vector3.Dot(vector4, vector5) > 0f;
								}
							}
							if (flag)
							{
								float num7;
								if (eventCamera == null || this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
								{
									num7 = 0f;
								}
								else
								{
									Transform transform = gameObject.transform;
									Vector3 forward = transform.forward;
									num7 = Vector3.Dot(forward, transform.position - ray.origin) / Vector3.Dot(forward, ray.direction);
									if (num7 < 0f)
									{
										goto IL_04EA;
									}
								}
								if (num7 < num5)
								{
									RaycastResult raycastResult = new RaycastResult
									{
										gameObject = gameObject,
										module = this,
										distance = num7,
										screenPosition = vector,
										index = (float)resultAppendList.Count,
										depth = this.m_RaycastResults[i].depth,
										sortingLayer = this.canvas.sortingLayerID,
										sortingOrder = this.canvas.sortingOrder
									};
									resultAppendList.Add(raycastResult);
								}
							}
							IL_04EA:
							i++;
							continue;
							goto IL_04EA;
						}
					}
				}
			}
		}

		public override Camera eventCamera
		{
			get
			{
				Camera camera;
				if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay || (this.canvas.renderMode == RenderMode.ScreenSpaceCamera && this.canvas.worldCamera == null))
				{
					camera = null;
				}
				else
				{
					camera = ((!(this.canvas.worldCamera != null)) ? Camera.main : this.canvas.worldCamera);
				}
				return camera;
			}
		}

		private static void Raycast(Canvas canvas, Camera eventCamera, Vector2 pointerPosition, IList<Graphic> foundGraphics, List<Graphic> results)
		{
			int num = foundGraphics.Count;
			for (int i = 0; i < num; i++)
			{
				Graphic graphic = foundGraphics[i];
				if (graphic.depth != -1 && graphic.raycastTarget && !graphic.canvasRenderer.cull)
				{
					if (RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera))
					{
						if (!(eventCamera != null) || eventCamera.WorldToScreenPoint(graphic.rectTransform.position).z <= eventCamera.farClipPlane)
						{
							if (graphic.Raycast(pointerPosition, eventCamera))
							{
								GraphicRaycaster.s_SortedGraphics.Add(graphic);
							}
						}
					}
				}
			}
			GraphicRaycaster.s_SortedGraphics.Sort((Graphic g1, Graphic g2) => g2.depth.CompareTo(g1.depth));
			num = GraphicRaycaster.s_SortedGraphics.Count;
			for (int j = 0; j < num; j++)
			{
				results.Add(GraphicRaycaster.s_SortedGraphics[j]);
			}
			GraphicRaycaster.s_SortedGraphics.Clear();
		}

		protected const int kNoEventMaskSet = -1;

		[FormerlySerializedAs("ignoreReversedGraphics")]
		[SerializeField]
		private bool m_IgnoreReversedGraphics = true;

		[FormerlySerializedAs("blockingObjects")]
		[SerializeField]
		private GraphicRaycaster.BlockingObjects m_BlockingObjects = GraphicRaycaster.BlockingObjects.None;

		[SerializeField]
		protected LayerMask m_BlockingMask = -1;

		private Canvas m_Canvas;

		[NonSerialized]
		private List<Graphic> m_RaycastResults = new List<Graphic>();

		[NonSerialized]
		private static readonly List<Graphic> s_SortedGraphics = new List<Graphic>();

		public enum BlockingObjects
		{
			None,
			TwoD,
			ThreeD,
			All
		}
	}
}
