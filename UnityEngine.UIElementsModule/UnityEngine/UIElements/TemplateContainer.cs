using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Serialization;

namespace UnityEngine.UIElements
{
	[HideInInspector]
	[UxmlElement("Instance")]
	public class TemplateContainer : BindableElement
	{
		[CreateProperty(ReadOnly = true)]
		public string templateId { get; private set; }

		[CreateProperty(ReadOnly = true)]
		public VisualTreeAsset templateSource
		{
			get
			{
				return this.m_TemplateSource;
			}
			internal set
			{
				this.m_TemplateSource = value;
			}
		}

		public TemplateContainer()
			: this(null)
		{
		}

		public TemplateContainer(string templateId)
			: this(templateId, null)
		{
		}

		internal TemplateContainer(string templateId, VisualTreeAsset templateSource)
		{
			this.templateId = templateId;
			this.templateSource = templateSource;
			this.m_ContentContainer = this;
		}

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_ContentContainer;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void SetContentContainer(VisualElement content)
		{
			this.m_ContentContainer = content;
		}

		internal static readonly BindingId templateIdProperty = "templateId";

		internal static readonly BindingId templateSourceProperty = "templateSource";

		internal const string k_ElementName = "Instance";

		private VisualElement m_ContentContainer;

		private VisualTreeAsset m_TemplateSource;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BindableElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(TemplateContainer.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("template", "template", null, Array.Empty<string>()),
					new UxmlAttributeNames("templateId", "template", null, new string[] { "template" })
				}, false);
			}

			public override object CreateInstance()
			{
				return new TemplateContainer();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				TemplateContainer templateContainer = (TemplateContainer)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.template_UxmlAttributeFlags);
				if (flag)
				{
					templateContainer.templateSource = this.template;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.templateId_UxmlAttributeFlags);
				if (flag2)
				{
					templateContainer.templateId = this.templateId;
				}
			}

			[SerializeField]
			private VisualTreeAsset template;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags template_UxmlAttributeFlags;

			[FormerlySerializedAs("template")]
			[SerializeField]
			[UxmlAttribute("template")]
			[HideInInspector]
			[UxmlAttributeBindingPath("templateId")]
			private string templateId;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags templateId_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<TemplateContainer, TemplateContainer.UxmlTraits>
		{
			public override string uxmlName
			{
				get
				{
					return "Instance";
				}
			}

			public override string uxmlQualifiedName
			{
				get
				{
					return this.uxmlNamespace + "." + this.uxmlName;
				}
			}

			internal const string k_ElementName = "Instance";
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				TemplateContainer templateContainer = (TemplateContainer)ve;
				templateContainer.templateId = this.m_Template.GetValueFromBag(bag, cc);
				VisualTreeAsset visualTreeAsset = cc.visualTreeAsset;
				VisualTreeAsset visualTreeAsset2 = ((visualTreeAsset != null) ? visualTreeAsset.ResolveTemplate(templateContainer.templateId) : null);
				bool flag = visualTreeAsset2 == null;
				if (flag)
				{
					templateContainer.Add(new Label(string.Format("Unknown Template: '{0}'", templateContainer.templateId)));
				}
				else
				{
					TemplateAsset templateAsset = bag as TemplateAsset;
					List<TemplateAsset.AttributeOverride> list = ((templateAsset != null) ? templateAsset.attributeOverrides : null);
					List<CreationContext.AttributeOverrideRange> list2 = cc.attributeOverrides;
					bool flag2 = list != null;
					if (flag2)
					{
						bool flag3 = list2 == null;
						if (flag3)
						{
							list2 = new List<CreationContext.AttributeOverrideRange>();
						}
						list2.Add(new CreationContext.AttributeOverrideRange(cc.visualTreeAsset, list));
					}
					templateContainer.templateSource = visualTreeAsset2;
					visualTreeAsset2.CloneTree(ve, new CreationContext(cc.slotInsertionPoints, list2));
				}
				bool flag4 = visualTreeAsset2 == null;
				if (flag4)
				{
					Debug.LogErrorFormat("Could not resolve template with name '{0}'", new object[] { templateContainer.templateId });
				}
			}

			internal const string k_TemplateAttributeName = "template";

			private UxmlStringAttributeDescription m_Template = new UxmlStringAttributeDescription
			{
				name = "template",
				use = UxmlAttributeDescription.Use.Required
			};
		}
	}
}
