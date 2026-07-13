using System;
using System.Collections.Generic;
using Unity.Profiling;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public abstract class ImmediateModeElement : VisualElement
	{
		[CreateProperty]
		public bool cullingEnabled
		{
			get
			{
				return this.m_CullingEnabled;
			}
			set
			{
				bool flag = this.m_CullingEnabled == value;
				if (!flag)
				{
					this.m_CullingEnabled = value;
					base.IncrementVersion(VersionChangeType.Repaint);
					base.NotifyPropertyChanged(in ImmediateModeElement.cullingEnabledProperty);
				}
			}
		}

		public ImmediateModeElement()
		{
			base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(this.OnGenerateVisualContent));
			Type type = base.GetType();
			bool flag = !ImmediateModeElement.s_Markers.TryGetValue(type, out this.m_ImmediateRepaintMarker);
			if (flag)
			{
				this.m_ImmediateRepaintMarker = new ProfilerMarker(base.typeName + ".ImmediateRepaint");
				ImmediateModeElement.s_Markers[type] = this.m_ImmediateRepaintMarker;
			}
		}

		private void OnGenerateVisualContent(MeshGenerationContext mgc)
		{
			BaseRuntimePanel baseRuntimePanel = base.elementPanel as BaseRuntimePanel;
			bool flag = baseRuntimePanel != null && baseRuntimePanel.drawsInCameras;
			if (flag)
			{
				Debug.LogError("ImmediateModeElement cannot be used in a panel drawn by cameras.");
			}
			else
			{
				mgc.entryRecorder.DrawImmediate(mgc.parentEntry, new Action(this.CallImmediateRepaint), this.cullingEnabled);
			}
		}

		private void CallImmediateRepaint()
		{
			using (this.m_ImmediateRepaintMarker.Auto())
			{
				this.ImmediateRepaint();
			}
		}

		protected abstract void ImmediateRepaint();

		internal static readonly BindingId cullingEnabledProperty = "cullingEnabled";

		private static readonly Dictionary<Type, ProfilerMarker> s_Markers = new Dictionary<Type, ProfilerMarker>();

		private readonly ProfilerMarker m_ImmediateRepaintMarker;

		private bool m_CullingEnabled = false;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : VisualElement.UxmlSerializedData
		{
		}
	}
}
