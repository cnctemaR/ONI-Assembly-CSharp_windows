using System;

namespace UnityEngine.Experimental.UIElements
{
	internal sealed class VisualTreeUpdater : IDisposable
	{
		public VisualTreeUpdater(BaseVisualElementPanel panel)
		{
			this.m_Panel = panel;
			this.m_UpdaterArray = new VisualTreeUpdater.UpdaterArray();
			this.SetDefaultUpdaters();
		}

		public void Dispose()
		{
			for (int i = 0; i < 6; i++)
			{
				IVisualTreeUpdater visualTreeUpdater = this.m_UpdaterArray[i];
				visualTreeUpdater.Dispose();
			}
		}

		public void UpdateVisualTree()
		{
			for (int i = 0; i < 6; i++)
			{
				IVisualTreeUpdater visualTreeUpdater = this.m_UpdaterArray[i];
				visualTreeUpdater.Update();
			}
		}

		public void UpdateVisualTreePhase(VisualTreeUpdatePhase phase)
		{
			IVisualTreeUpdater visualTreeUpdater = this.m_UpdaterArray[phase];
			visualTreeUpdater.Update();
		}

		public void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			for (int i = 0; i < 6; i++)
			{
				IVisualTreeUpdater visualTreeUpdater = this.m_UpdaterArray[i];
				visualTreeUpdater.OnVersionChanged(ve, versionChangeType);
			}
		}

		public void DirtyStyleSheets()
		{
			VisualTreeStyleUpdater visualTreeStyleUpdater = this.m_UpdaterArray[VisualTreeUpdatePhase.Styles] as VisualTreeStyleUpdater;
			visualTreeStyleUpdater.DirtyStyleSheets();
		}

		public void SetUpdater(IVisualTreeUpdater updater, VisualTreeUpdatePhase phase)
		{
			updater.panel = this.m_Panel;
			this.m_UpdaterArray[phase] = updater;
		}

		public void SetUpdater<T>(VisualTreeUpdatePhase phase) where T : IVisualTreeUpdater, new()
		{
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
			this.SetUpdater<VisualTreePersistentDataUpdater>(VisualTreeUpdatePhase.PersistentData);
			this.SetUpdater<VisualTreeBindingsUpdater>(VisualTreeUpdatePhase.Bindings);
			this.SetUpdater<VisualTreeStyleUpdater>(VisualTreeUpdatePhase.Styles);
			this.SetUpdater<VisualTreeLayoutUpdater>(VisualTreeUpdatePhase.Layout);
			this.SetUpdater<VisualTreeTransformClipUpdater>(VisualTreeUpdatePhase.TransformClip);
			this.SetUpdater<VisualTreeRepaintUpdater>(VisualTreeUpdatePhase.Repaint);
		}

		private BaseVisualElementPanel m_Panel;

		private VisualTreeUpdater.UpdaterArray m_UpdaterArray;

		private class UpdaterArray
		{
			public UpdaterArray()
			{
				this.m_VisualTreeUpdaters = new IVisualTreeUpdater[6];
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

			private IVisualTreeUpdater[] m_VisualTreeUpdaters;
		}
	}
}
