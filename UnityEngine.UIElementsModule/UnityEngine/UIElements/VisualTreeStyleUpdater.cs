using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class VisualTreeStyleUpdater : BaseVisualTreeUpdater
	{
		public VisualTreeStyleUpdaterTraversal traversal
		{
			get
			{
				return this.m_StyleContextHierarchyTraversal;
			}
			set
			{
				this.m_StyleContextHierarchyTraversal = value;
				BaseVisualElementPanel panel = base.panel;
				if (panel != null)
				{
					panel.visualTree.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Transform);
				}
			}
		}

		public override ProfilerMarker profilerMarker
		{
			get
			{
				return VisualTreeStyleUpdater.s_ProfilerMarker;
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			bool flag = (versionChangeType & (VersionChangeType.StyleSheet | VersionChangeType.TransitionProperty)) == (VersionChangeType)0;
			if (!flag)
			{
				this.m_Version += 1U;
				bool flag2 = (versionChangeType & VersionChangeType.StyleSheet) > (VersionChangeType)0;
				if (flag2)
				{
					bool isApplyingStyles = this.m_IsApplyingStyles;
					if (isApplyingStyles)
					{
						this.m_ApplyStyleUpdateList.Add(ve);
					}
					else
					{
						this.m_StyleContextHierarchyTraversal.AddChangedElement(ve, versionChangeType);
					}
				}
				bool flag3 = (versionChangeType & VersionChangeType.TransitionProperty) > (VersionChangeType)0;
				if (flag3)
				{
					this.m_TransitionPropertyUpdateList.Add(ve);
				}
			}
		}

		public override void Update()
		{
			bool flag = this.m_Version == this.m_LastVersion;
			if (!flag)
			{
				this.m_LastVersion = this.m_Version;
				this.ApplyStyles();
				this.m_StyleContextHierarchyTraversal.Clear();
				foreach (VisualElement visualElement in this.m_ApplyStyleUpdateList)
				{
					this.m_StyleContextHierarchyTraversal.AddChangedElement(visualElement, VersionChangeType.StyleSheet);
				}
				this.m_ApplyStyleUpdateList.Clear();
				foreach (VisualElement visualElement2 in this.m_TransitionPropertyUpdateList)
				{
					bool flag2 = visualElement2.hasRunningAnimations || visualElement2.hasCompletedAnimations;
					if (flag2)
					{
						ComputedTransitionUtils.UpdateComputedTransitions(visualElement2.computedStyle);
						this.m_StyleContextHierarchyTraversal.CancelAnimationsWithNoTransitionProperty(visualElement2, visualElement2.computedStyle);
					}
				}
				this.m_TransitionPropertyUpdateList.Clear();
			}
		}

		private protected bool disposed { protected get; private set; }

		protected override void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.m_StyleContextHierarchyTraversal.Clear();
				}
				this.disposed = true;
			}
		}

		private void ApplyStyles()
		{
			Debug.Assert(base.visualTree.panel != null);
			this.m_IsApplyingStyles = true;
			this.m_StyleContextHierarchyTraversal.PrepareTraversal(base.panel, base.panel.scaledPixelsPerPoint);
			this.m_StyleContextHierarchyTraversal.Traverse(base.visualTree);
			this.m_IsApplyingStyles = false;
		}

		private HashSet<VisualElement> m_ApplyStyleUpdateList = new HashSet<VisualElement>();

		private HashSet<VisualElement> m_TransitionPropertyUpdateList = new HashSet<VisualElement>();

		private bool m_IsApplyingStyles = false;

		private uint m_Version = 0U;

		private uint m_LastVersion = 0U;

		private VisualTreeStyleUpdaterTraversal m_StyleContextHierarchyTraversal = new VisualTreeStyleUpdaterTraversal();

		private static readonly string s_Description = "UIElements.UpdateStyle";

		private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(VisualTreeStyleUpdater.s_Description);
	}
}
