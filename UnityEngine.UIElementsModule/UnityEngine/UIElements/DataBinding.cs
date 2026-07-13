using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[UxmlObject]
	public class DataBinding : Binding, IDataSourceProvider
	{
		internal static MethodInfo updateUIMethod
		{
			get
			{
				MethodInfo methodInfo;
				if ((methodInfo = DataBinding.s_UpdateUIMethodInfo) == null)
				{
					methodInfo = (DataBinding.s_UpdateUIMethodInfo = DataBinding.CacheReflectionInfo());
				}
				return methodInfo;
			}
		}

		private static MethodInfo CacheReflectionInfo()
		{
			foreach (MethodInfo methodInfo in typeof(DataBinding).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				bool flag = methodInfo.Name != "UpdateUI";
				if (!flag)
				{
					bool flag2 = methodInfo.GetParameters().Length != 2;
					if (!flag2)
					{
						return DataBinding.s_UpdateUIMethodInfo = methodInfo;
					}
				}
			}
			throw new InvalidOperationException("Could not find method UpdateUI by reflection. This is an internal bug. Please report using `Help > Report a Bug...` ");
		}

		[CreateProperty]
		public object dataSource { get; set; }

		public Type dataSourceType { get; set; }

		internal string dataSourceTypeString
		{
			get
			{
				return UxmlUtility.TypeToString(this.dataSourceType);
			}
			set
			{
				this.dataSourceType = UxmlUtility.ParseType(value, null);
			}
		}

		[CreateProperty]
		public PropertyPath dataSourcePath { get; set; }

		internal string dataSourcePathString
		{
			get
			{
				return this.dataSourcePath.ToString();
			}
			set
			{
				this.dataSourcePath = new PropertyPath(value);
			}
		}

		[CreateProperty]
		public BindingMode bindingMode
		{
			get
			{
				return this.m_BindingMode;
			}
			set
			{
				bool flag = this.m_BindingMode == value;
				if (!flag)
				{
					this.m_BindingMode = value;
					base.MarkDirty();
				}
			}
		}

		[CreateProperty(ReadOnly = true)]
		public ConverterGroup sourceToUiConverters
		{
			get
			{
				ConverterGroup converterGroup;
				if ((converterGroup = this.m_SourceToUiConverters) == null)
				{
					converterGroup = (this.m_SourceToUiConverters = new ConverterGroup(string.Empty, null, null));
				}
				return converterGroup;
			}
		}

		[CreateProperty(ReadOnly = true)]
		public ConverterGroup uiToSourceConverters
		{
			get
			{
				ConverterGroup converterGroup;
				if ((converterGroup = this.m_UiToSourceConverters) == null)
				{
					converterGroup = (this.m_UiToSourceConverters = new ConverterGroup(string.Empty, null, null));
				}
				return converterGroup;
			}
		}

		internal string sourceToUiConvertersString
		{
			get
			{
				return (this.m_SourceToUIConvertersString != null) ? string.Join(", ", this.m_SourceToUIConvertersString) : null;
			}
			set
			{
				this.m_SourceToUIConvertersString = UxmlUtility.ParseStringListAttribute(value);
				bool flag = this.m_SourceToUIConvertersString != null;
				if (flag)
				{
					foreach (string text in this.m_SourceToUIConvertersString)
					{
						ConverterGroup converterGroup;
						bool flag2 = ConverterGroups.TryGetConverterGroup(text, out converterGroup);
						if (flag2)
						{
							this.ApplyConverterGroupToUI(converterGroup);
						}
					}
				}
			}
		}

		internal string uiToSourceConvertersString
		{
			get
			{
				return (this.m_UiToSourceConvertersString != null) ? string.Join(", ", this.m_UiToSourceConvertersString) : null;
			}
			set
			{
				this.m_UiToSourceConvertersString = UxmlUtility.ParseStringListAttribute(value);
				bool flag = this.m_UiToSourceConvertersString != null;
				if (flag)
				{
					foreach (string text in this.m_UiToSourceConvertersString)
					{
						ConverterGroup converterGroup;
						bool flag2 = ConverterGroups.TryGetConverterGroup(text, out converterGroup);
						if (flag2)
						{
							this.ApplyConverterGroupToSource(converterGroup);
						}
					}
				}
			}
		}

		public DataBinding()
		{
			base.updateTrigger = BindingUpdateTrigger.OnSourceChanged;
		}

		public void ApplyConverterGroupToSource(ConverterGroup group)
		{
			ConverterGroup uiToSourceConverters = this.uiToSourceConverters;
			uiToSourceConverters.registry.Apply(group.registry);
		}

		public void ApplyConverterGroupToUI(ConverterGroup group)
		{
			ConverterGroup sourceToUiConverters = this.sourceToUiConverters;
			sourceToUiConverters.registry.Apply(group.registry);
		}

		protected internal virtual BindingResult UpdateUI<TValue>(in BindingContext context, ref TValue value)
		{
			VisualElement targetElement = context.targetElement;
			FocusController focusController = targetElement.focusController;
			bool flag = focusController != null && focusController.IsFocused(targetElement);
			if (flag)
			{
				Focusable leafFocusedElement = focusController.GetLeafFocusedElement();
				TextElement textElement = leafFocusedElement as TextElement;
				bool flag2;
				if (textElement != null && textElement.ClassListContains("unity-text-element--inner-input-field-component"))
				{
					IDelayedField delayedField = targetElement as IDelayedField;
					flag2 = (delayedField != null && delayedField.isDelayed) || textElement.edition.touchScreenKeyboard != null;
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					return new BindingResult(BindingStatus.Pending, null);
				}
			}
			ConverterGroup sourceToUiConverters = this.sourceToUiConverters;
			BindingId bindingId = context.bindingId;
			PropertyPath propertyPath = in bindingId;
			VisitReturnCode visitReturnCode;
			bool flag4 = sourceToUiConverters.TrySetValue<VisualElement, TValue>(ref targetElement, in propertyPath, value, out visitReturnCode);
			bool flag5 = flag4;
			BindingResult bindingResult;
			if (flag5)
			{
				bindingResult = default(BindingResult);
			}
			else
			{
				VisitReturnCode visitReturnCode2 = visitReturnCode;
				object dataSource = context.dataSource;
				propertyPath = context.dataSourcePath;
				object obj = targetElement;
				bindingId = context.bindingId;
				string setValueErrorString = DataBinding.GetSetValueErrorString<TValue>(visitReturnCode2, dataSource, in propertyPath, obj, in bindingId, value);
				bindingResult = new BindingResult(BindingStatus.Failure, setValueErrorString);
			}
			return bindingResult;
		}

		protected internal virtual BindingResult UpdateSource<TValue>(in BindingContext context, ref TValue value)
		{
			object dataSource = context.dataSource;
			ConverterGroup uiToSourceConverters = this.uiToSourceConverters;
			PropertyPath propertyPath = context.dataSourcePath;
			VisitReturnCode visitReturnCode;
			bool flag = uiToSourceConverters.TrySetValue<object, TValue>(ref dataSource, in propertyPath, value, out visitReturnCode);
			bool flag2 = flag;
			BindingResult bindingResult;
			if (flag2)
			{
				bindingResult = default(BindingResult);
			}
			else
			{
				VisitReturnCode visitReturnCode2 = visitReturnCode;
				object targetElement = context.targetElement;
				BindingId bindingId = context.bindingId;
				propertyPath = in bindingId;
				object dataSource2 = context.dataSource;
				PropertyPath dataSourcePath = context.dataSourcePath;
				BindingId bindingId2 = in dataSourcePath;
				string setValueErrorString = DataBinding.GetSetValueErrorString<TValue>(visitReturnCode2, targetElement, in propertyPath, dataSource2, in bindingId2, value);
				bindingResult = new BindingResult(BindingStatus.Failure, setValueErrorString);
			}
			return bindingResult;
		}

		internal static string GetSetValueErrorString<TValue>(VisitReturnCode returnCode, object source, in PropertyPath sourcePath, object target, in BindingId targetPath, TValue extractedValueFromSource)
		{
			string text = string.Format("[UI Toolkit] Could not set value for target of type '<b>{0}</b>' at path '<b>{1}</b>':", target.GetType().Name, targetPath);
			string text2;
			switch (returnCode)
			{
			case VisitReturnCode.Ok:
			case VisitReturnCode.NullContainer:
			case VisitReturnCode.InvalidContainerType:
				throw new InvalidOperationException(text + " internal data binding error. Please report this using the '<b>Help/Report a bug...</b>' menu item.");
			case VisitReturnCode.MissingPropertyBag:
				text2 = text + " the type '" + target.GetType().Name + "' is missing a property bag.";
				break;
			case VisitReturnCode.InvalidPath:
				text2 = text + " the path is either invalid or contains a null value.";
				break;
			case VisitReturnCode.InvalidCast:
			{
				bool isEmpty = sourcePath.IsEmpty;
				if (isEmpty)
				{
					object obj;
					bool flag = PropertyContainer.TryGetValue<object, object>(ref target, in targetPath, out obj) && obj != null;
					if (flag)
					{
						text2 = ((extractedValueFromSource == null) ? (text + " could not convert from '<b>null</b>' to '<b>" + obj.GetType().Name + "</b>'.") : string.Concat(new string[]
						{
							text,
							" could not convert from type '<b>",
							extractedValueFromSource.GetType().Name,
							"</b>' to type '<b>",
							obj.GetType().Name,
							"</b>'."
						}));
						break;
					}
				}
				IProperty property;
				bool flag2 = PropertyContainer.TryGetProperty<object>(ref source, in sourcePath, out property);
				if (flag2)
				{
					object obj2;
					bool flag3 = PropertyContainer.TryGetValue<object, object>(ref target, in targetPath, out obj2) && obj2 != null;
					if (flag3)
					{
						text2 = ((extractedValueFromSource == null) ? string.Concat(new string[]
						{
							text,
							" could not convert from '<b>null (",
							property.DeclaredValueType().Name,
							")</b>' to '<b>",
							obj2.GetType().Name,
							"</b>'."
						}) : string.Concat(new string[]
						{
							text,
							" could not convert from type '<b>",
							extractedValueFromSource.GetType().Name,
							"</b>' to type '<b>",
							obj2.GetType().Name,
							"</b>'."
						}));
						break;
					}
				}
				text2 = text + " conversion failed.";
				break;
			}
			case VisitReturnCode.AccessViolation:
				text2 = text + " the path is read-only.";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return text2;
		}

		private static MethodInfo s_UpdateUIMethodInfo;

		private BindingMode m_BindingMode;

		private ConverterGroup m_SourceToUiConverters;

		private ConverterGroup m_UiToSourceConverters;

		private List<string> m_SourceToUIConvertersString;

		private List<string> m_UiToSourceConvertersString;

		internal const string k_DataSourceTooltip = "A data source is a collection of information. By default, a binding will inherit the existing data source from the hierarchy. You can instead define another object here as the data source, or define the type of property it may be if the source is not yet available.";

		internal const string k_DataSourcePathTooltip = "The path to the value in the data source used by this binding. To see resolved bindings in the UI Builder, define a path that is compatible with the target source property.";

		internal const string k_BindingModeTooltip = "Controls how a binding is updated, which can include the direction in which data is written.";

		internal const string k_SourceToUiConvertersTooltip = "Define one or more converter groups for this binding that will be used between the data source to the target UI.";

		internal const string k_UiToSourceConvertersTooltip = "Define one or more converter groups for this binding that will be used between the target UI to the data source.";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : Binding.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(DataBinding.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("dataSourcePathString", "data-source-path", null, Array.Empty<string>()),
					new UxmlAttributeNames("dataSource", "data-source", null, Array.Empty<string>()),
					new UxmlAttributeNames("dataSourceTypeString", "data-source-type", typeof(object), Array.Empty<string>()),
					new UxmlAttributeNames("bindingMode", "binding-mode", null, Array.Empty<string>()),
					new UxmlAttributeNames("sourceToUiConvertersString", "source-to-ui-converters", null, Array.Empty<string>()),
					new UxmlAttributeNames("uiToSourceConvertersString", "ui-to-source-converters", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new DataBinding();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				DataBinding dataBinding = (DataBinding)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.dataSourcePathString_UxmlAttributeFlags);
				if (flag)
				{
					dataBinding.dataSourcePathString = this.dataSourcePathString;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.dataSource_UxmlAttributeFlags);
				if (flag2)
				{
					dataBinding.dataSource = (this.dataSource ? this.dataSource : null);
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.dataSourceTypeString_UxmlAttributeFlags);
				if (flag3)
				{
					dataBinding.dataSourceTypeString = this.dataSourceTypeString;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.bindingMode_UxmlAttributeFlags);
				if (flag4)
				{
					dataBinding.bindingMode = this.bindingMode;
				}
				bool flag5 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.uiToSourceConvertersString_UxmlAttributeFlags);
				if (flag5)
				{
					dataBinding.uiToSourceConvertersString = this.uiToSourceConvertersString;
				}
				bool flag6 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.sourceToUiConvertersString_UxmlAttributeFlags);
				if (flag6)
				{
					dataBinding.sourceToUiConvertersString = this.sourceToUiConvertersString;
				}
			}

			[Tooltip("The path to the value in the data source used by this binding. To see resolved bindings in the UI Builder, define a path that is compatible with the target source property.")]
			[UxmlAttribute("data-source-path")]
			[HideInInspector]
			[SerializeField]
			private string dataSourcePathString;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags dataSourcePathString_UxmlAttributeFlags;

			[SerializeField]
			[HideInInspector]
			[DataSourceDrawer]
			[Tooltip("A data source is a collection of information. By default, a binding will inherit the existing data source from the hierarchy. You can instead define another object here as the data source, or define the type of property it may be if the source is not yet available.")]
			private Object dataSource;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags dataSource_UxmlAttributeFlags;

			[Tooltip("A data source is a collection of information. By default, a binding will inherit the existing data source from the hierarchy. You can instead define another object here as the data source, or define the type of property it may be if the source is not yet available.")]
			[UxmlTypeReference(typeof(object))]
			[HideInInspector]
			[SerializeField]
			[UxmlAttribute("data-source-type")]
			private string dataSourceTypeString;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags dataSourceTypeString_UxmlAttributeFlags;

			[BindingModeDrawer]
			[HideInInspector]
			[SerializeField]
			[Tooltip("Controls how a binding is updated, which can include the direction in which data is written.")]
			private BindingMode bindingMode;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags bindingMode_UxmlAttributeFlags;

			[ConverterDrawer(isConverterToSource = false)]
			[UxmlAttributeBindingPath("uiToSourceConverters")]
			[Tooltip("Define one or more converter groups for this binding that will be used between the data source to the target UI.")]
			[SerializeField]
			[HideInInspector]
			[UxmlAttribute("source-to-ui-converters")]
			private string sourceToUiConvertersString;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags sourceToUiConvertersString_UxmlAttributeFlags;

			[SerializeField]
			[UxmlAttribute("ui-to-source-converters")]
			[Tooltip("Define one or more converter groups for this binding that will be used between the target UI to the data source.")]
			[HideInInspector]
			[ConverterDrawer(isConverterToSource = true)]
			[UxmlAttributeBindingPath("sourceToUiConverters")]
			private string uiToSourceConvertersString;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags uiToSourceConvertersString_UxmlAttributeFlags;
		}
	}
}
