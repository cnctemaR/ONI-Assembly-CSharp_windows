using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UIElements
{
	internal class VisualElementAnimationSystem : BaseVisualTreeUpdater
	{
		public override ProfilerMarker profilerMarker
		{
			get
			{
				return VisualElementAnimationSystem.s_ProfilerMarker;
			}
		}

		private static ProfilerMarker stylePropertyAnimationProfilerMarker
		{
			get
			{
				return VisualElementAnimationSystem.s_StylePropertyAnimationProfilerMarker;
			}
		}

		public void UnregisterAnimation(IValueAnimationUpdate anim)
		{
			this.m_Animations.Remove(anim);
			this.m_IterationListDirty = true;
		}

		public void UnregisterAnimations(List<IValueAnimationUpdate> anims)
		{
			foreach (IValueAnimationUpdate valueAnimationUpdate in anims)
			{
				this.m_Animations.Remove(valueAnimationUpdate);
			}
			this.m_IterationListDirty = true;
		}

		public void RegisterAnimation(IValueAnimationUpdate anim)
		{
			this.m_Animations.Add(anim);
			this.m_HasNewAnimations = true;
			this.m_IterationListDirty = true;
		}

		public void RegisterAnimations(List<IValueAnimationUpdate> anims)
		{
			foreach (IValueAnimationUpdate valueAnimationUpdate in anims)
			{
				this.m_Animations.Add(valueAnimationUpdate);
			}
			this.m_HasNewAnimations = true;
			this.m_IterationListDirty = true;
		}

		public override void Update()
		{
			double num = base.panel.TimeSinceStartupSeconds();
			long num2 = (long)(num * 1000.0);
			bool iterationListDirty = this.m_IterationListDirty;
			if (iterationListDirty)
			{
				this.m_IterationList = this.m_Animations.ToList<IValueAnimationUpdate>();
				this.m_IterationListDirty = false;
			}
			bool flag = this.m_HasNewAnimations || this.lastUpdate != num;
			if (flag)
			{
				foreach (IValueAnimationUpdate valueAnimationUpdate in this.m_IterationList)
				{
					valueAnimationUpdate.Tick(num2);
				}
				this.m_HasNewAnimations = false;
				this.lastUpdate = num;
			}
			IStylePropertyAnimationSystem styleAnimationSystem = base.panel.styleAnimationSystem;
			using (VisualElementAnimationSystem.stylePropertyAnimationProfilerMarker.Auto())
			{
				styleAnimationSystem.Update(num);
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
		}

		private HashSet<IValueAnimationUpdate> m_Animations = new HashSet<IValueAnimationUpdate>();

		private List<IValueAnimationUpdate> m_IterationList = new List<IValueAnimationUpdate>();

		private bool m_HasNewAnimations = false;

		private bool m_IterationListDirty = false;

		private static readonly string s_Description = "UIElements.UpdateAnimation";

		private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(VisualElementAnimationSystem.s_Description);

		private static readonly string s_StylePropertyAnimationDescription = "UIElements.UpdateAnimationProperties";

		private static readonly ProfilerMarker s_StylePropertyAnimationProfilerMarker = new ProfilerMarker(VisualElementAnimationSystem.s_StylePropertyAnimationDescription);

		private double lastUpdate;
	}
}
