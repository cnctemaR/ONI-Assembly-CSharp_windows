using System;
using Unity.Profiling;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal interface IVisualTreeUpdater : IDisposable
	{
		long FrameCount { get; set; }

		BaseVisualElementPanel panel { get; set; }

		ProfilerMarker profilerMarker { get; }

		void Update();

		void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType);
	}
}
