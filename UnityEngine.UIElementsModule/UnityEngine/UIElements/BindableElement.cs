using System;
using System.Diagnostics;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class BindableElement : VisualElement, IBindable
	{
		public IBinding binding { get; set; }

		public string bindingPath { get; set; }

		internal const string k_BindingPathTooltip = "Default method to define a path to a serialized property. Most often used for Editor extensions and inspectors.";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : VisualElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(BindableElement.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("bindingPath", "binding-path", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new BindableElement();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.bindingPath_UxmlAttributeFlags);
				if (flag)
				{
					BindableElement bindableElement = (BindableElement)obj;
					bindableElement.bindingPath = this.bindingPath;
				}
			}

			[BindingPathDrawer]
			[Tooltip("Default method to define a path to a serialized property. Most often used for Editor extensions and inspectors.")]
			[SerializeField]
			private string bindingPath;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags bindingPath_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<BindableElement, BindableElement.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public UxmlTraits()
			{
				this.m_PropertyPath = new UxmlStringAttributeDescription
				{
					name = "binding-path"
				};
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				string valueFromBag = this.m_PropertyPath.GetValueFromBag(bag, cc);
				bool flag = !string.IsNullOrEmpty(valueFromBag);
				if (flag)
				{
					IBindable bindable = ve as IBindable;
					bool flag2 = bindable != null;
					if (flag2)
					{
						bindable.bindingPath = valueFromBag;
					}
				}
			}

			private UxmlStringAttributeDescription m_PropertyPath;
		}
	}
}
