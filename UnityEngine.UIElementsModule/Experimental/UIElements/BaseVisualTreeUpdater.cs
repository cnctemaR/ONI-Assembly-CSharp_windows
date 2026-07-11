using System;

namespace UnityEngine.Experimental.UIElements
{
	internal abstract class BaseVisualTreeUpdater : IVisualTreeUpdater, IDisposable
	{
		public BaseVisualElementPanel panel { get; set; }

		public VisualElement visualTree
		{
			get
			{
				return this.panel.visualTree;
			}
		}

		public abstract string description { get; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public abstract void Update();

		public abstract void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType);
	}
}
