using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[UxmlObject]
	public abstract class Binding
	{
		public static void SetGlobalLogLevel(BindingLogLevel logLevel)
		{
			DataBindingManager.globalLogLevel = logLevel;
		}

		public static BindingLogLevel GetGlobalLogLevel()
		{
			return DataBindingManager.globalLogLevel;
		}

		public static void SetPanelLogLevel(IPanel panel, BindingLogLevel logLevel)
		{
			BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
			bool flag = baseVisualElementPanel != null;
			if (flag)
			{
				baseVisualElementPanel.dataBindingManager.logLevel = logLevel;
			}
		}

		public static BindingLogLevel GetPanelLogLevel(IPanel panel)
		{
			BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
			bool flag = baseVisualElementPanel != null;
			BindingLogLevel bindingLogLevel;
			if (flag)
			{
				bindingLogLevel = baseVisualElementPanel.dataBindingManager.logLevel;
			}
			else
			{
				bindingLogLevel = BindingLogLevel.None;
			}
			return bindingLogLevel;
		}

		public static void ResetPanelLogLevel(IPanel panel)
		{
			BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
			bool flag = baseVisualElementPanel != null;
			if (flag)
			{
				baseVisualElementPanel.dataBindingManager.ResetLogLevel();
			}
		}

		internal string property
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get;
			set; }

		public bool isDirty
		{
			get
			{
				return this.m_Dirty;
			}
		}

		[CreateProperty]
		public BindingUpdateTrigger updateTrigger
		{
			get
			{
				return this.m_UpdateTrigger;
			}
			set
			{
				this.m_UpdateTrigger = value;
			}
		}

		internal Binding()
		{
			this.m_Dirty = true;
		}

		public void MarkDirty()
		{
			this.m_Dirty = true;
		}

		internal void ClearDirty()
		{
			this.m_Dirty = false;
		}

		protected internal virtual void OnActivated(in BindingActivationContext context)
		{
		}

		protected internal virtual void OnDeactivated(in BindingActivationContext context)
		{
		}

		protected internal virtual void OnDataSourceChanged(in DataSourceContextChanged context)
		{
		}

		private bool m_Dirty;

		private BindingUpdateTrigger m_UpdateTrigger;

		internal const string k_UpdateTriggerTooltip = "This informs the binding system of whether the binding object should be updated on every frame, when a change occurs in the source or on every frame if change detection is impossible, and when explicitly marked as dirty.";

		[ExcludeFromDocs]
		[Serializable]
		public abstract class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(Binding.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("property", "property", null, Array.Empty<string>()),
					new UxmlAttributeNames("updateTrigger", "update-trigger", null, Array.Empty<string>())
				}, false);
			}

			public override void Deserialize(object obj)
			{
				Binding binding = (Binding)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.property_UxmlAttributeFlags);
				if (flag)
				{
					binding.property = this.property;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.updateTrigger_UxmlAttributeFlags);
				if (flag2)
				{
					binding.updateTrigger = this.updateTrigger;
				}
			}

			[HideInInspector]
			[SerializeField]
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal string property;

			[SerializeField]
			[Tooltip("This informs the binding system of whether the binding object should be updated on every frame, when a change occurs in the source or on every frame if change detection is impossible, and when explicitly marked as dirty.")]
			[HideInInspector]
			private BindingUpdateTrigger updateTrigger;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags property_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags updateTrigger_UxmlAttributeFlags;
		}
	}
}
