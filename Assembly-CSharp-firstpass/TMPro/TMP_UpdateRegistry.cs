using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro
{
	public class TMP_UpdateRegistry
	{
		protected TMP_UpdateRegistry()
		{
			Canvas.willRenderCanvases += this.PerformUpdateForCanvasRendererObjects;
		}

		public static TMP_UpdateRegistry instance
		{
			get
			{
				if (TMP_UpdateRegistry.s_Instance == null)
				{
					TMP_UpdateRegistry.s_Instance = new TMP_UpdateRegistry();
				}
				return TMP_UpdateRegistry.s_Instance;
			}
		}

		public static void RegisterCanvasElementForLayoutRebuild(ICanvasElement element)
		{
			TMP_UpdateRegistry.instance.InternalRegisterCanvasElementForLayoutRebuild(element);
		}

		private bool InternalRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
		{
			int instanceID = (element as global::UnityEngine.Object).GetInstanceID();
			bool flag;
			if (this.m_LayoutQueueLookup.ContainsKey(instanceID))
			{
				flag = false;
			}
			else
			{
				this.m_LayoutQueueLookup[instanceID] = instanceID;
				this.m_LayoutRebuildQueue.Add(element);
				flag = true;
			}
			return flag;
		}

		public static void RegisterCanvasElementForGraphicRebuild(ICanvasElement element)
		{
			TMP_UpdateRegistry.instance.InternalRegisterCanvasElementForGraphicRebuild(element);
		}

		private bool InternalRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
		{
			int instanceID = (element as global::UnityEngine.Object).GetInstanceID();
			bool flag;
			if (this.m_GraphicQueueLookup.ContainsKey(instanceID))
			{
				flag = false;
			}
			else
			{
				this.m_GraphicQueueLookup[instanceID] = instanceID;
				this.m_GraphicRebuildQueue.Add(element);
				flag = true;
			}
			return flag;
		}

		private void PerformUpdateForCanvasRendererObjects()
		{
			for (int i = 0; i < this.m_LayoutRebuildQueue.Count; i++)
			{
				ICanvasElement canvasElement = TMP_UpdateRegistry.instance.m_LayoutRebuildQueue[i];
				canvasElement.Rebuild(CanvasUpdate.Prelayout);
			}
			if (this.m_LayoutRebuildQueue.Count > 0)
			{
				this.m_LayoutRebuildQueue.Clear();
				this.m_LayoutQueueLookup.Clear();
			}
			for (int j = 0; j < this.m_GraphicRebuildQueue.Count; j++)
			{
				ICanvasElement canvasElement2 = TMP_UpdateRegistry.instance.m_GraphicRebuildQueue[j];
				canvasElement2.Rebuild(CanvasUpdate.PreRender);
			}
			if (this.m_GraphicRebuildQueue.Count > 0)
			{
				this.m_GraphicRebuildQueue.Clear();
				this.m_GraphicQueueLookup.Clear();
			}
		}

		private void PerformUpdateForMeshRendererObjects()
		{
			global::Debug.Log("Perform update of MeshRenderer objects.", null);
		}

		public static void UnRegisterCanvasElementForRebuild(ICanvasElement element)
		{
			TMP_UpdateRegistry.instance.InternalUnRegisterCanvasElementForLayoutRebuild(element);
			TMP_UpdateRegistry.instance.InternalUnRegisterCanvasElementForGraphicRebuild(element);
		}

		private void InternalUnRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
		{
			int instanceID = (element as global::UnityEngine.Object).GetInstanceID();
			TMP_UpdateRegistry.instance.m_LayoutRebuildQueue.Remove(element);
			this.m_GraphicQueueLookup.Remove(instanceID);
		}

		private void InternalUnRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
		{
			int instanceID = (element as global::UnityEngine.Object).GetInstanceID();
			TMP_UpdateRegistry.instance.m_GraphicRebuildQueue.Remove(element);
			this.m_LayoutQueueLookup.Remove(instanceID);
		}

		private static TMP_UpdateRegistry s_Instance;

		private readonly List<ICanvasElement> m_LayoutRebuildQueue = new List<ICanvasElement>();

		private Dictionary<int, int> m_LayoutQueueLookup = new Dictionary<int, int>();

		private readonly List<ICanvasElement> m_GraphicRebuildQueue = new List<ICanvasElement>();

		private Dictionary<int, int> m_GraphicQueueLookup = new Dictionary<int, int>();
	}
}
