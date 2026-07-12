using System;
using System.Collections.Generic;
using UnityEngine.UI.Collections;

namespace UnityEngine.UI
{
	public class GraphicRegistry
	{
		protected GraphicRegistry()
		{
			GC.KeepAlive(new Dictionary<Graphic, int>());
			GC.KeepAlive(new Dictionary<ICanvasElement, int>());
			GC.KeepAlive(new Dictionary<IClipper, int>());
		}

		public static GraphicRegistry instance
		{
			get
			{
				if (GraphicRegistry.s_Instance == null)
				{
					GraphicRegistry.s_Instance = new GraphicRegistry();
				}
				return GraphicRegistry.s_Instance;
			}
		}

		public static void RegisterGraphicForCanvas(Canvas c, Graphic graphic)
		{
			if (c == null || graphic == null)
			{
				return;
			}
			IndexedSet<Graphic> indexedSet;
			GraphicRegistry.instance.m_Graphics.TryGetValue(c, out indexedSet);
			if (indexedSet != null)
			{
				indexedSet.AddUnique(graphic, true);
				GraphicRegistry.RegisterRaycastGraphicForCanvas(c, graphic);
				return;
			}
			indexedSet = new IndexedSet<Graphic>();
			indexedSet.Add(graphic);
			GraphicRegistry.instance.m_Graphics.Add(c, indexedSet);
			GraphicRegistry.RegisterRaycastGraphicForCanvas(c, graphic);
		}

		public static void RegisterRaycastGraphicForCanvas(Canvas c, Graphic graphic)
		{
			if (c == null || graphic == null || !graphic.raycastTarget)
			{
				return;
			}
			IndexedSet<Graphic> indexedSet;
			GraphicRegistry.instance.m_RaycastableGraphics.TryGetValue(c, out indexedSet);
			if (indexedSet != null)
			{
				indexedSet.AddUnique(graphic, true);
				return;
			}
			indexedSet = new IndexedSet<Graphic>();
			indexedSet.Add(graphic);
			GraphicRegistry.instance.m_RaycastableGraphics.Add(c, indexedSet);
		}

		public static void UnregisterGraphicForCanvas(Canvas c, Graphic graphic)
		{
			if (c == null || graphic == null)
			{
				return;
			}
			IndexedSet<Graphic> indexedSet;
			if (GraphicRegistry.instance.m_Graphics.TryGetValue(c, out indexedSet))
			{
				indexedSet.Remove(graphic);
				if (indexedSet.Capacity == 0)
				{
					GraphicRegistry.instance.m_Graphics.Remove(c);
				}
				GraphicRegistry.UnregisterRaycastGraphicForCanvas(c, graphic);
			}
		}

		public static void UnregisterRaycastGraphicForCanvas(Canvas c, Graphic graphic)
		{
			if (c == null || graphic == null)
			{
				return;
			}
			IndexedSet<Graphic> indexedSet;
			if (GraphicRegistry.instance.m_RaycastableGraphics.TryGetValue(c, out indexedSet))
			{
				indexedSet.Remove(graphic);
				if (indexedSet.Count == 0)
				{
					GraphicRegistry.instance.m_RaycastableGraphics.Remove(c);
				}
			}
		}

		public static void DisableGraphicForCanvas(Canvas c, Graphic graphic)
		{
			if (c == null)
			{
				return;
			}
			IndexedSet<Graphic> indexedSet;
			if (GraphicRegistry.instance.m_Graphics.TryGetValue(c, out indexedSet))
			{
				indexedSet.DisableItem(graphic);
				if (indexedSet.Capacity == 0)
				{
					GraphicRegistry.instance.m_Graphics.Remove(c);
				}
				GraphicRegistry.DisableRaycastGraphicForCanvas(c, graphic);
			}
		}

		public static void DisableRaycastGraphicForCanvas(Canvas c, Graphic graphic)
		{
			if (c == null || !graphic.raycastTarget)
			{
				return;
			}
			IndexedSet<Graphic> indexedSet;
			if (GraphicRegistry.instance.m_RaycastableGraphics.TryGetValue(c, out indexedSet))
			{
				indexedSet.DisableItem(graphic);
				if (indexedSet.Capacity == 0)
				{
					GraphicRegistry.instance.m_RaycastableGraphics.Remove(c);
				}
			}
		}

		public static IList<Graphic> GetGraphicsForCanvas(Canvas canvas)
		{
			IndexedSet<Graphic> indexedSet;
			if (GraphicRegistry.instance.m_Graphics.TryGetValue(canvas, out indexedSet))
			{
				return indexedSet;
			}
			return GraphicRegistry.s_EmptyList;
		}

		public static IList<Graphic> GetRaycastableGraphicsForCanvas(Canvas canvas)
		{
			IndexedSet<Graphic> indexedSet;
			if (GraphicRegistry.instance.m_RaycastableGraphics.TryGetValue(canvas, out indexedSet))
			{
				return indexedSet;
			}
			return GraphicRegistry.s_EmptyList;
		}

		private static GraphicRegistry s_Instance;

		private readonly Dictionary<Canvas, IndexedSet<Graphic>> m_Graphics = new Dictionary<Canvas, IndexedSet<Graphic>>();

		private readonly Dictionary<Canvas, IndexedSet<Graphic>> m_RaycastableGraphics = new Dictionary<Canvas, IndexedSet<Graphic>>();

		private static readonly List<Graphic> s_EmptyList = new List<Graphic>();
	}
}
