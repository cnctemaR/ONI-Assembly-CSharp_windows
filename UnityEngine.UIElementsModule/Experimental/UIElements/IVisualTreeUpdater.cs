using System;

namespace UnityEngine.Experimental.UIElements
{
	internal interface IVisualTreeUpdater : IDisposable
	{
		BaseVisualElementPanel panel { get; set; }

		string description { get; }

		void Update();

		void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType);
	}
}
