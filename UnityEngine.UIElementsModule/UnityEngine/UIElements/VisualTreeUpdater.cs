using System;

namespace UnityEngine.UIElements
{
	internal sealed class VisualTreeUpdater : IDisposable
	{
		public long[] GetUpdatersFrameCount()
		{
			return this.m_UpdaterArray.GetUpdatersFrameCount();
		}

		public VisualTreeUpdater(BaseVisualElementPanel panel)
		{
			this.m_Panel = panel;
			this.m_UpdaterArray = new VisualTreeUpdater.UpdaterArray();
			this.SetDefaultUpdaters();
		}

		public void Dispose()
		{
			for (int i = 0; i < 8; i++)
			{
				IVisualTreeUpdater visualTreeUpdater = this.m_UpdaterArray[i];
				visualTreeUpdater.Dispose();
			}
		}

		[Obsolete("This will be removed. Please use the different update methods from Panel instead")]
		public void UpdateVisualTree()
		{
			for (int i = 0; i < 8; i++)
			{
				IVisualTreeUpdater visualTreeUpdater = this.m_UpdaterArray[i];
				using (visualTreeUpdater.profilerMarker.Auto())
				{
					visualTreeUpdater.Update();
					IVisualTreeUpdater visualTreeUpdater2 = visualTreeUpdater;
					long num = visualTreeUpdater2.FrameCount + 1L;
					visualTreeUpdater2.FrameCount = num;
				}
			}
		}

		public void UpdateVisualTreePhase(VisualTreeUpdatePhase phase)
		{
			IVisualTreeUpdater visualTreeUpdater = this.m_UpdaterArray[phase];
			using (visualTreeUpdater.profilerMarker.Auto())
			{
				visualTreeUpdater.Update();
				IVisualTreeUpdater visualTreeUpdater2 = visualTreeUpdater;
				long num = visualTreeUpdater2.FrameCount + 1L;
				visualTreeUpdater2.FrameCount = num;
			}
		}

		public void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			for (int i = 0; i < 8; i++)
			{
				IVisualTreeUpdater visualTreeUpdater = this.m_UpdaterArray[i];
				visualTreeUpdater.OnVersionChanged(ve, versionChangeType);
			}
		}

		public void SetUpdater(IVisualTreeUpdater updater, VisualTreeUpdatePhase phase)
		{
			IVisualTreeUpdater visualTreeUpdater = this.m_UpdaterArray[phase];
			if (visualTreeUpdater != null)
			{
				visualTreeUpdater.Dispose();
			}
			updater.panel = this.m_Panel;
			this.m_UpdaterArray[phase] = updater;
		}

		public void SetUpdater<T>(VisualTreeUpdatePhase phase) where T : IVisualTreeUpdater, new()
		{
			IVisualTreeUpdater visualTreeUpdater = this.m_UpdaterArray[phase];
			if (visualTreeUpdater != null)
			{
				visualTreeUpdater.Dispose();
			}
			T t = new T();
			t.panel = this.m_Panel;
			T t2 = t;
			this.m_UpdaterArray[phase] = t2;
		}

		public IVisualTreeUpdater GetUpdater(VisualTreeUpdatePhase phase)
		{
			return this.m_UpdaterArray[phase];
		}

		private void SetDefaultUpdaters()
		{
			this.SetUpdater<VisualTreeBindingsUpdater>(VisualTreeUpdatePhase.Bindings);
			this.SetUpdater<VisualTreeDataBindingsUpdater>(VisualTreeUpdatePhase.DataBinding);
			this.SetUpdater<VisualElementAnimationSystem>(VisualTreeUpdatePhase.Animation);
			this.SetUpdater<VisualTreeStyleUpdater>(VisualTreeUpdatePhase.Styles);
			this.SetUpdater<VisualTreeLayoutUpdater>(VisualTreeUpdatePhase.Layout);
			this.SetUpdater<VisualTreeHierarchyFlagsUpdater>(VisualTreeUpdatePhase.TransformClip);
			this.SetUpdater<UIRRepaintUpdater>(VisualTreeUpdatePhase.Repaint);
			this.SetUpdater<VisualTreeAuthoringUpdater>(VisualTreeUpdatePhase.Authoring);
		}

		private BaseVisualElementPanel m_Panel;

		private VisualTreeUpdater.UpdaterArray m_UpdaterArray;

		private class UpdaterArray
		{
			public UpdaterArray()
			{
				this.m_VisualTreeUpdaters = new IVisualTreeUpdater[8];
			}

			public IVisualTreeUpdater this[VisualTreeUpdatePhase phase]
			{
				get
				{
					return this.m_VisualTreeUpdaters[(int)phase];
				}
				set
				{
					this.m_VisualTreeUpdaters[(int)phase] = value;
				}
			}

			public IVisualTreeUpdater this[int index]
			{
				get
				{
					return this.m_VisualTreeUpdaters[index];
				}
				set
				{
					this.m_VisualTreeUpdaters[index] = value;
				}
			}

			public long[] GetUpdatersFrameCount()
			{
				long[] array = new long[this.m_VisualTreeUpdaters.Length];
				for (int i = 0; i < this.m_VisualTreeUpdaters.Length; i++)
				{
					array[i] = this.m_VisualTreeUpdaters[i].FrameCount;
				}
				return array;
			}

			private IVisualTreeUpdater[] m_VisualTreeUpdaters;
		}
	}
}
