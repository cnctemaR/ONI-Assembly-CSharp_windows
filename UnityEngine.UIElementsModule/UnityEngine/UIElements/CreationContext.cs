using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public struct CreationContext : IEquatable<CreationContext>
	{
		public VisualElement target { readonly get; private set; }

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal List<int> veaIdsPath
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			readonly get;
			private set; }

		internal TemplateAsset templateAsset { readonly get; private set; }

		public VisualTreeAsset visualTreeAsset { readonly get; private set; }

		public Dictionary<string, VisualElement> slotInsertionPoints { readonly get; private set; }

		internal List<CreationContext.AttributeOverrideRange> attributeOverrides { readonly get; private set; }

		internal List<CreationContext.SerializedDataOverrideRange> serializedDataOverrides { readonly get; private set; }

		internal List<string> namesPath { readonly get; private set; }

		internal bool hasOverrides
		{
			get
			{
				List<CreationContext.AttributeOverrideRange> attributeOverrides = this.attributeOverrides;
				bool flag;
				if (attributeOverrides == null || attributeOverrides.Count <= 0)
				{
					List<CreationContext.SerializedDataOverrideRange> serializedDataOverrides = this.serializedDataOverrides;
					flag = serializedDataOverrides != null && serializedDataOverrides.Count > 0;
				}
				else
				{
					flag = true;
				}
				return flag;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal CreationContext(VisualTreeAsset vta)
		{
			this = new CreationContext(null, vta, null);
		}

		internal CreationContext(Dictionary<string, VisualElement> slotInsertionPoints)
		{
			this = new CreationContext(slotInsertionPoints, null, null, null);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal CreationContext(Dictionary<string, VisualElement> slotInsertionPoints, List<CreationContext.AttributeOverrideRange> attributeOverrides)
		{
			this = new CreationContext(slotInsertionPoints, attributeOverrides, null, null);
		}

		internal CreationContext(Dictionary<string, VisualElement> slotInsertionPoints, VisualTreeAsset vta, VisualElement target)
		{
			this = new CreationContext(slotInsertionPoints, null, vta, target);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal CreationContext(Dictionary<string, VisualElement> slotInsertionPoints, List<CreationContext.AttributeOverrideRange> attributeOverrides, VisualTreeAsset vta, VisualElement target)
		{
			this = new CreationContext(slotInsertionPoints, attributeOverrides, null, vta, target, null, null, null);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal CreationContext(Dictionary<string, VisualElement> slotInsertionPoints, List<CreationContext.AttributeOverrideRange> attributeOverrides, List<CreationContext.SerializedDataOverrideRange> serializedDataOverrides, VisualTreeAsset vta, VisualElement target, List<int> veaIdsPath, List<string> namesPath, TemplateAsset ta)
		{
			this.target = target;
			this.slotInsertionPoints = slotInsertionPoints;
			this.attributeOverrides = attributeOverrides;
			this.serializedDataOverrides = serializedDataOverrides;
			this.visualTreeAsset = vta;
			this.namesPath = namesPath;
			this.veaIdsPath = veaIdsPath;
			this.templateAsset = ta;
		}

		public override bool Equals(object obj)
		{
			return obj is CreationContext && this.Equals((CreationContext)obj);
		}

		public bool Equals(CreationContext other)
		{
			return EqualityComparer<VisualElement>.Default.Equals(this.target, other.target) && EqualityComparer<VisualTreeAsset>.Default.Equals(this.visualTreeAsset, other.visualTreeAsset) && EqualityComparer<Dictionary<string, VisualElement>>.Default.Equals(this.slotInsertionPoints, other.slotInsertionPoints);
		}

		public override int GetHashCode()
		{
			int num = -2123482148;
			num = num * -1521134295 + EqualityComparer<VisualElement>.Default.GetHashCode(this.target);
			num = num * -1521134295 + EqualityComparer<VisualTreeAsset>.Default.GetHashCode(this.visualTreeAsset);
			return num * -1521134295 + EqualityComparer<Dictionary<string, VisualElement>>.Default.GetHashCode(this.slotInsertionPoints);
		}

		public static bool operator ==(CreationContext context1, CreationContext context2)
		{
			return context1.Equals(context2);
		}

		public static bool operator !=(CreationContext context1, CreationContext context2)
		{
			return !(context1 == context2);
		}

		public static readonly CreationContext Default = default(CreationContext);

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal struct AttributeOverrideRange
		{
			public AttributeOverrideRange(VisualTreeAsset sourceAsset, List<TemplateAsset.AttributeOverride> attributeOverrides)
			{
				this.sourceAsset = sourceAsset;
				this.attributeOverrides = attributeOverrides;
			}

			internal readonly VisualTreeAsset sourceAsset;

			internal readonly List<TemplateAsset.AttributeOverride> attributeOverrides;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal struct SerializedDataOverrideRange
		{
			public SerializedDataOverrideRange(VisualTreeAsset sourceAsset, List<TemplateAsset.UxmlSerializedDataOverride> attributeOverrides, int templateId)
			{
				this.sourceAsset = sourceAsset;
				this.attributeOverrides = attributeOverrides;
				this.templateId = templateId;
			}

			internal readonly VisualTreeAsset sourceAsset;

			internal readonly int templateId;

			internal readonly List<TemplateAsset.UxmlSerializedDataOverride> attributeOverrides;
		}
	}
}
