using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro
{
	public class TMP_UpdateManager
	{
		private static TMP_UpdateManager instance
		{
			get
			{
				if (TMP_UpdateManager.s_Instance == null)
				{
					TMP_UpdateManager.s_Instance = new TMP_UpdateManager();
				}
				return TMP_UpdateManager.s_Instance;
			}
		}

		private TMP_UpdateManager()
		{
			Canvas.willRenderCanvases += this.DoRebuilds;
		}

		internal static void RegisterTextObjectForUpdate(TMP_Text textObject)
		{
			TMP_UpdateManager.instance.InternalRegisterTextObjectForUpdate(textObject);
		}

		private void InternalRegisterTextObjectForUpdate(TMP_Text textObject)
		{
			int instanceID = textObject.GetInstanceID();
			if (this.m_InternalUpdateLookup.Contains(instanceID))
			{
				return;
			}
			this.m_InternalUpdateLookup.Add(instanceID);
			this.m_InternalUpdateQueue.Add(textObject);
		}

		public static void RegisterTextElementForLayoutRebuild(TMP_Text element)
		{
			TMP_UpdateManager.instance.InternalRegisterTextElementForLayoutRebuild(element);
		}

		private void InternalRegisterTextElementForLayoutRebuild(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			if (this.m_LayoutQueueLookup.Contains(instanceID))
			{
				return;
			}
			this.m_LayoutQueueLookup.Add(instanceID);
			this.m_LayoutRebuildQueue.Add(element);
		}

		public static void RegisterTextElementForGraphicRebuild(TMP_Text element)
		{
			TMP_UpdateManager.instance.InternalRegisterTextElementForGraphicRebuild(element);
		}

		private void InternalRegisterTextElementForGraphicRebuild(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			if (this.m_GraphicQueueLookup.Contains(instanceID))
			{
				return;
			}
			this.m_GraphicQueueLookup.Add(instanceID);
			this.m_GraphicRebuildQueue.Add(element);
		}

		public static void RegisterTextElementForCullingUpdate(TMP_Text element)
		{
			TMP_UpdateManager.instance.InternalRegisterTextElementForCullingUpdate(element);
		}

		private void InternalRegisterTextElementForCullingUpdate(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			if (this.m_CullingUpdateLookup.Contains(instanceID))
			{
				return;
			}
			this.m_CullingUpdateLookup.Add(instanceID);
			this.m_CullingUpdateQueue.Add(element);
		}

		private void OnCameraPreCull()
		{
			this.DoRebuilds();
		}

		private void DoRebuilds()
		{
			for (int i = 0; i < this.m_InternalUpdateQueue.Count; i++)
			{
				this.m_InternalUpdateQueue[i].InternalUpdate();
			}
			for (int j = 0; j < this.m_LayoutRebuildQueue.Count; j++)
			{
				this.m_LayoutRebuildQueue[j].Rebuild(CanvasUpdate.Prelayout);
			}
			if (this.m_LayoutRebuildQueue.Count > 0)
			{
				this.m_LayoutRebuildQueue.Clear();
				this.m_LayoutQueueLookup.Clear();
			}
			for (int k = 0; k < this.m_GraphicRebuildQueue.Count; k++)
			{
				this.m_GraphicRebuildQueue[k].Rebuild(CanvasUpdate.PreRender);
			}
			if (this.m_GraphicRebuildQueue.Count > 0)
			{
				this.m_GraphicRebuildQueue.Clear();
				this.m_GraphicQueueLookup.Clear();
			}
			for (int l = 0; l < this.m_CullingUpdateQueue.Count; l++)
			{
				this.m_CullingUpdateQueue[l].UpdateCulling();
			}
			if (this.m_CullingUpdateQueue.Count > 0)
			{
				this.m_CullingUpdateQueue.Clear();
				this.m_CullingUpdateLookup.Clear();
			}
		}

		internal static void UnRegisterTextObjectForUpdate(TMP_Text textObject)
		{
			TMP_UpdateManager.instance.InternalUnRegisterTextObjectForUpdate(textObject);
		}

		public static void UnRegisterTextElementForRebuild(TMP_Text element)
		{
			TMP_UpdateManager.instance.InternalUnRegisterTextElementForGraphicRebuild(element);
			TMP_UpdateManager.instance.InternalUnRegisterTextElementForLayoutRebuild(element);
			TMP_UpdateManager.instance.InternalUnRegisterTextObjectForUpdate(element);
		}

		private void InternalUnRegisterTextElementForGraphicRebuild(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			this.m_GraphicRebuildQueue.Remove(element);
			this.m_GraphicQueueLookup.Remove(instanceID);
		}

		private void InternalUnRegisterTextElementForLayoutRebuild(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			this.m_LayoutRebuildQueue.Remove(element);
			this.m_LayoutQueueLookup.Remove(instanceID);
		}

		private void InternalUnRegisterTextObjectForUpdate(TMP_Text textObject)
		{
			int instanceID = textObject.GetInstanceID();
			this.m_InternalUpdateQueue.Remove(textObject);
			this.m_InternalUpdateLookup.Remove(instanceID);
		}

		private static TMP_UpdateManager s_Instance;

		private readonly HashSet<int> m_LayoutQueueLookup = new HashSet<int>();

		private readonly List<TMP_Text> m_LayoutRebuildQueue = new List<TMP_Text>();

		private readonly HashSet<int> m_GraphicQueueLookup = new HashSet<int>();

		private readonly List<TMP_Text> m_GraphicRebuildQueue = new List<TMP_Text>();

		private readonly HashSet<int> m_InternalUpdateLookup = new HashSet<int>();

		private readonly List<TMP_Text> m_InternalUpdateQueue = new List<TMP_Text>();

		private readonly HashSet<int> m_CullingUpdateLookup = new HashSet<int>();

		private readonly List<TMP_Text> m_CullingUpdateQueue = new List<TMP_Text>();

		private static ProfilerMarker k_RegisterTextObjectForUpdateMarker = new ProfilerMarker("TMP.RegisterTextObjectForUpdate");

		private static ProfilerMarker k_RegisterTextElementForGraphicRebuildMarker = new ProfilerMarker("TMP.RegisterTextElementForGraphicRebuild");

		private static ProfilerMarker k_RegisterTextElementForCullingUpdateMarker = new ProfilerMarker("TMP.RegisterTextElementForCullingUpdate");

		private static ProfilerMarker k_UnregisterTextObjectForUpdateMarker = new ProfilerMarker("TMP.UnregisterTextObjectForUpdate");

		private static ProfilerMarker k_UnregisterTextElementForGraphicRebuildMarker = new ProfilerMarker("TMP.UnregisterTextElementForGraphicRebuild");
	}
}
