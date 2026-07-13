using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[UxmlObject]
	public class Column : INotifyBindablePropertyChanged
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

		[CreateProperty]
		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				bool flag = this.m_Name == value;
				if (!flag)
				{
					this.m_Name = value;
					this.NotifyChange(ColumnDataType.Name);
					this.NotifyPropertyChanged(in Column.nameProperty);
				}
			}
		}

		[CreateProperty]
		public string title
		{
			get
			{
				return this.m_Title;
			}
			set
			{
				bool flag = this.m_Title == value;
				if (!flag)
				{
					this.m_Title = value;
					this.NotifyChange(ColumnDataType.Title);
					this.NotifyPropertyChanged(in Column.titleProperty);
				}
			}
		}

		[CreateProperty]
		public Background icon
		{
			get
			{
				return this.m_Icon;
			}
			set
			{
				bool flag = this.m_Icon == value;
				if (!flag)
				{
					this.m_Icon = value;
					this.NotifyChange(ColumnDataType.Icon);
					this.NotifyPropertyChanged(in Column.iconProperty);
				}
			}
		}

		public Comparison<int> comparison { get; set; }

		internal int index
		{
			get
			{
				Columns collection = this.collection;
				return (collection != null) ? collection.IndexOf(this) : (-1);
			}
		}

		internal int displayIndex
		{
			get
			{
				Columns collection = this.collection;
				List<Column> list = ((collection != null) ? collection.displayList : null) as List<Column>;
				return (list != null) ? list.IndexOf(this) : (-1);
			}
		}

		internal int visibleIndex
		{
			get
			{
				Columns collection = this.collection;
				List<Column> list = ((collection != null) ? collection.visibleList : null) as List<Column>;
				return (list != null) ? list.IndexOf(this) : (-1);
			}
		}

		[CreateProperty]
		public bool visible
		{
			get
			{
				return this.m_Visible;
			}
			set
			{
				bool flag = this.m_Visible == value;
				if (!flag)
				{
					this.m_Visible = value;
					this.NotifyChange(ColumnDataType.Visibility);
					this.NotifyPropertyChanged(in Column.visibleProperty);
				}
			}
		}

		[CreateProperty]
		public Length width
		{
			get
			{
				return this.m_Width;
			}
			set
			{
				bool flag = this.m_Width == value;
				if (!flag)
				{
					this.m_Width = value;
					this.desiredWidth = float.NaN;
					this.NotifyChange(ColumnDataType.Width);
					this.NotifyPropertyChanged(in Column.widthProperty);
				}
			}
		}

		[CreateProperty]
		public Length minWidth
		{
			get
			{
				return this.m_MinWidth;
			}
			set
			{
				bool flag = this.m_MinWidth == value;
				if (!flag)
				{
					this.m_MinWidth = value;
					this.NotifyChange(ColumnDataType.MinWidth);
					this.NotifyPropertyChanged(in Column.minWidthProperty);
				}
			}
		}

		[CreateProperty]
		public Length maxWidth
		{
			get
			{
				return this.m_MaxWidth;
			}
			set
			{
				bool flag = this.m_MaxWidth == value;
				if (!flag)
				{
					this.m_MaxWidth = value;
					this.NotifyChange(ColumnDataType.MaxWidth);
					this.NotifyPropertyChanged(in Column.maxWidthProperty);
				}
			}
		}

		internal float desiredWidth
		{
			get
			{
				return this.m_DesiredWidth;
			}
			set
			{
				bool flag = this.m_DesiredWidth == value;
				if (!flag)
				{
					this.m_DesiredWidth = value;
					Action<Column> action = this.resized;
					if (action != null)
					{
						action(this);
					}
				}
			}
		}

		[CreateProperty]
		public bool sortable
		{
			get
			{
				return this.m_Sortable;
			}
			set
			{
				bool flag = this.m_Sortable == value;
				if (!flag)
				{
					this.m_Sortable = value;
					this.NotifyChange(ColumnDataType.Sortable);
					this.NotifyPropertyChanged(in Column.sortableProperty);
				}
			}
		}

		[CreateProperty]
		public bool stretchable
		{
			get
			{
				return this.m_Stretchable;
			}
			set
			{
				bool flag = this.m_Stretchable == value;
				if (!flag)
				{
					this.m_Stretchable = value;
					this.NotifyChange(ColumnDataType.Stretchable);
					this.NotifyPropertyChanged(in Column.stretchableProperty);
				}
			}
		}

		[CreateProperty]
		public bool optional
		{
			get
			{
				return this.m_Optional;
			}
			set
			{
				bool flag = this.m_Optional == value;
				if (!flag)
				{
					this.m_Optional = value;
					this.NotifyChange(ColumnDataType.Optional);
					this.NotifyPropertyChanged(in Column.optionalProperty);
				}
			}
		}

		[CreateProperty]
		public bool resizable
		{
			get
			{
				return this.m_Resizable;
			}
			set
			{
				bool flag = this.m_Resizable == value;
				if (!flag)
				{
					this.m_Resizable = value;
					this.NotifyChange(ColumnDataType.Resizable);
					this.NotifyPropertyChanged(in Column.resizableProperty);
				}
			}
		}

		public string bindingPath { get; set; }

		[CreateProperty]
		public VisualTreeAsset headerTemplate
		{
			get
			{
				return this.m_HeaderTemplate;
			}
			set
			{
				bool flag = this.m_HeaderTemplate == value;
				if (!flag)
				{
					this.m_HeaderTemplate = value;
					this.NotifyChange(ColumnDataType.HeaderTemplate);
					this.NotifyPropertyChanged(in Column.headerTemplateProperty);
				}
			}
		}

		[CreateProperty]
		public VisualTreeAsset cellTemplate
		{
			get
			{
				return this.m_CellTemplate;
			}
			set
			{
				bool flag = this.m_CellTemplate == value;
				if (!flag)
				{
					this.m_CellTemplate = value;
					this.NotifyChange(ColumnDataType.CellTemplate);
					this.NotifyPropertyChanged(in Column.cellTemplateProperty);
				}
			}
		}

		public Func<VisualElement> makeHeader
		{
			get
			{
				return this.m_MakeHeader;
			}
			set
			{
				bool flag = this.m_MakeHeader == value;
				if (!flag)
				{
					this.m_MakeHeader = value;
					this.NotifyChange(ColumnDataType.HeaderTemplate);
				}
			}
		}

		public Action<VisualElement> bindHeader
		{
			get
			{
				return this.m_BindHeader;
			}
			set
			{
				bool flag = this.m_BindHeader == value;
				if (!flag)
				{
					this.m_BindHeader = value;
					this.NotifyChange(ColumnDataType.HeaderTemplate);
				}
			}
		}

		public Action<VisualElement> unbindHeader
		{
			get
			{
				return this.m_UnbindHeader;
			}
			set
			{
				bool flag = this.m_UnbindHeader == value;
				if (!flag)
				{
					this.m_UnbindHeader = value;
					this.NotifyChange(ColumnDataType.HeaderTemplate);
				}
			}
		}

		public Action<VisualElement> destroyHeader
		{
			get
			{
				return this.m_DestroyHeader;
			}
			set
			{
				bool flag = this.m_DestroyHeader == value;
				if (!flag)
				{
					this.m_DestroyHeader = value;
					this.NotifyChange(ColumnDataType.HeaderTemplate);
				}
			}
		}

		public Func<VisualElement> makeCell
		{
			get
			{
				return this.m_MakeCell;
			}
			set
			{
				bool flag = this.m_MakeCell == value;
				if (!flag)
				{
					this.m_MakeCell = value;
					this.NotifyChange(ColumnDataType.CellTemplate);
				}
			}
		}

		public Action<VisualElement, int> bindCell
		{
			get
			{
				return this.m_BindCell;
			}
			set
			{
				bool flag = this.m_BindCell == value;
				if (!flag)
				{
					this.m_BindCell = value;
					this.NotifyChange(ColumnDataType.CellTemplate);
				}
			}
		}

		public Action<VisualElement, int> unbindCell
		{
			get
			{
				return this.m_UnbindCellItem;
			}
			set
			{
				bool flag = this.m_UnbindCellItem == value;
				if (!flag)
				{
					this.m_UnbindCellItem = value;
					this.NotifyChange(ColumnDataType.CellTemplate);
				}
			}
		}

		public Action<VisualElement> destroyCell { get; set; }

		public Columns collection { get; internal set; }

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<Column, ColumnDataType> changed;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<Column> resized;

		private void NotifyChange(ColumnDataType type)
		{
			Action<Column, ColumnDataType> action = this.changed;
			if (action != null)
			{
				action(this, type);
			}
		}

		private void NotifyPropertyChanged(in BindingId property)
		{
			EventHandler<BindablePropertyChangedEventArgs> eventHandler = this.propertyChanged;
			if (eventHandler != null)
			{
				eventHandler(this, new BindablePropertyChangedEventArgs(in property));
			}
		}

		internal float GetWidth(float layoutWidth)
		{
			return (this.width.unit == LengthUnit.Pixel) ? this.width.value : (this.width.value * layoutWidth / 100f);
		}

		internal float GetMaxWidth(float layoutWidth)
		{
			return (this.maxWidth.unit == LengthUnit.Pixel) ? this.maxWidth.value : (this.maxWidth.value * layoutWidth / 100f);
		}

		internal float GetMinWidth(float layoutWidth)
		{
			return (this.minWidth.unit == LengthUnit.Pixel) ? this.minWidth.value : (this.minWidth.value * layoutWidth / 100f);
		}

		private static readonly BindingId nameProperty = "name";

		private static readonly BindingId titleProperty = "title";

		private static readonly BindingId iconProperty = "icon";

		private static readonly BindingId visibleProperty = "visible";

		private static readonly BindingId widthProperty = "width";

		private static readonly BindingId minWidthProperty = "minWidth";

		private static readonly BindingId maxWidthProperty = "maxWidth";

		private static readonly BindingId sortableProperty = "sortable";

		private static readonly BindingId stretchableProperty = "stretchable";

		private static readonly BindingId optionalProperty = "optional";

		private static readonly BindingId resizableProperty = "resizable";

		private static readonly BindingId headerTemplateProperty = "headerTemplate";

		private static readonly BindingId cellTemplateProperty = "cellTemplate";

		internal const string k_HeaderTemplateAttributeName = "header-template";

		internal const string k_CellTemplateAttributeName = "cell-template";

		internal const float kDefaultMinWidth = 35f;

		private string m_Name;

		private string m_Title;

		private Background m_Icon;

		private bool m_Visible = true;

		private Length m_Width = 0f;

		private Length m_MinWidth = 35f;

		private Length m_MaxWidth = 8388608f;

		private float m_DesiredWidth = float.NaN;

		private bool m_Stretchable;

		private bool m_Sortable = true;

		private bool m_Optional = true;

		private bool m_Resizable = true;

		private VisualTreeAsset m_HeaderTemplate;

		private VisualTreeAsset m_CellTemplate;

		private Func<VisualElement> m_MakeHeader;

		private Action<VisualElement> m_BindHeader;

		private Action<VisualElement> m_UnbindHeader;

		private Action<VisualElement> m_DestroyHeader;

		private Func<VisualElement> m_MakeCell;

		private Action<VisualElement, int> m_BindCell;

		private Action<VisualElement, int> m_UnbindCellItem;

		[ExcludeFromDocs]
		[Serializable]
		public class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(Column.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("name", "name", null, Array.Empty<string>()),
					new UxmlAttributeNames("title", "title", null, Array.Empty<string>()),
					new UxmlAttributeNames("visible", "visible", null, Array.Empty<string>()),
					new UxmlAttributeNames("width", "width", null, Array.Empty<string>()),
					new UxmlAttributeNames("minWidth", "min-width", null, Array.Empty<string>()),
					new UxmlAttributeNames("maxWidth", "max-width", null, Array.Empty<string>()),
					new UxmlAttributeNames("stretchable", "stretchable", null, Array.Empty<string>()),
					new UxmlAttributeNames("sortable", "sortable", null, Array.Empty<string>()),
					new UxmlAttributeNames("optional", "optional", null, Array.Empty<string>()),
					new UxmlAttributeNames("resizable", "resizable", null, Array.Empty<string>()),
					new UxmlAttributeNames("headerTemplate", "header-template", null, Array.Empty<string>()),
					new UxmlAttributeNames("cellTemplate", "cell-template", null, Array.Empty<string>()),
					new UxmlAttributeNames("bindingPath", "binding-path", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new Column();
			}

			public override void Deserialize(object obj)
			{
				Column column = (Column)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.name_UxmlAttributeFlags);
				if (flag)
				{
					column.name = this.name;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.title_UxmlAttributeFlags);
				if (flag2)
				{
					column.title = this.title;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.visible_UxmlAttributeFlags);
				if (flag3)
				{
					column.visible = this.visible;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.width_UxmlAttributeFlags);
				if (flag4)
				{
					column.width = this.width;
				}
				bool flag5 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.minWidth_UxmlAttributeFlags);
				if (flag5)
				{
					column.minWidth = this.minWidth;
				}
				bool flag6 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.maxWidth_UxmlAttributeFlags);
				if (flag6)
				{
					column.maxWidth = this.maxWidth;
				}
				bool flag7 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.sortable_UxmlAttributeFlags);
				if (flag7)
				{
					column.sortable = this.sortable;
				}
				bool flag8 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.stretchable_UxmlAttributeFlags);
				if (flag8)
				{
					column.stretchable = this.stretchable;
				}
				bool flag9 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.optional_UxmlAttributeFlags);
				if (flag9)
				{
					column.optional = this.optional;
				}
				bool flag10 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.resizable_UxmlAttributeFlags);
				if (flag10)
				{
					column.resizable = this.resizable;
				}
				bool flag11 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.bindingPath_UxmlAttributeFlags);
				if (flag11)
				{
					column.bindingPath = this.bindingPath;
				}
				bool flag12 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.headerTemplate_UxmlAttributeFlags) && this.headerTemplate != null;
				if (flag12)
				{
					column.headerTemplate = this.headerTemplate;
					column.makeHeader = () => this.headerTemplate.Instantiate();
				}
				bool flag13 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.cellTemplate_UxmlAttributeFlags) && this.cellTemplate != null;
				if (flag13)
				{
					column.cellTemplate = this.cellTemplate;
					column.makeCell = () => this.cellTemplate.Instantiate();
				}
			}

			[SerializeField]
			private VisualTreeAsset headerTemplate;

			[SerializeField]
			private VisualTreeAsset cellTemplate;

			[SerializeField]
			private string name;

			[SerializeField]
			private string title;

			[SerializeField]
			private string bindingPath;

			[SerializeField]
			private Length width;

			[SerializeField]
			private Length minWidth;

			[SerializeField]
			private Length maxWidth;

			[SerializeField]
			private bool visible;

			[SerializeField]
			private bool stretchable;

			[SerializeField]
			private bool sortable;

			[SerializeField]
			private bool optional;

			[SerializeField]
			private bool resizable;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags name_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags title_UxmlAttributeFlags;

			[UxmlIgnore]
			[SerializeField]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags visible_UxmlAttributeFlags;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags width_UxmlAttributeFlags;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags minWidth_UxmlAttributeFlags;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags maxWidth_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags stretchable_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags sortable_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags optional_UxmlAttributeFlags;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags resizable_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags headerTemplate_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags cellTemplate_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags bindingPath_UxmlAttributeFlags;
		}

		[Obsolete("UxmlObjectFactory<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		internal class UxmlObjectFactory<T> : UxmlObjectFactory<T, Column.UxmlObjectTraits<T>> where T : Column, new()
		{
		}

		[Obsolete("UxmlObjectFactory<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		internal class UxmlObjectFactory : Column.UxmlObjectFactory<Column>
		{
		}

		[Obsolete("UxmlObjectTraits<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		internal class UxmlObjectTraits<T> : UnityEngine.UIElements.UxmlObjectTraits<T> where T : Column
		{
			private static Length ParseLength(string str, Length defaultValue)
			{
				float num = defaultValue.value;
				LengthUnit lengthUnit = defaultValue.unit;
				int num2 = 0;
				int num3 = -1;
				for (int i = 0; i < str.Length; i++)
				{
					char c = str[i];
					bool flag = char.IsLetter(c) || c == '%';
					if (flag)
					{
						num3 = i;
						break;
					}
					num2++;
				}
				string text = str.Substring(0, num2);
				string text2 = string.Empty;
				bool flag2 = num3 > 0;
				if (flag2)
				{
					text2 = str.Substring(num3, str.Length - num3).ToLowerInvariant();
				}
				float num4;
				bool flag3 = float.TryParse(text, out num4);
				if (flag3)
				{
					num = num4;
				}
				string text3 = text2;
				string text4 = text3;
				if (!(text4 == "px"))
				{
					if (text4 == "%")
					{
						lengthUnit = LengthUnit.Percent;
					}
				}
				else
				{
					lengthUnit = LengthUnit.Pixel;
				}
				return new Length(num, lengthUnit);
			}

			public override void Init(ref T obj, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ref obj, bag, cc);
				obj.name = this.m_Name.GetValueFromBag(bag, cc);
				obj.title = this.m_Text.GetValueFromBag(bag, cc);
				obj.visible = this.m_Visible.GetValueFromBag(bag, cc);
				obj.width = Column.UxmlObjectTraits<T>.ParseLength(this.m_Width.GetValueFromBag(bag, cc), default(Length));
				obj.maxWidth = Column.UxmlObjectTraits<T>.ParseLength(this.m_MaxWidth.GetValueFromBag(bag, cc), new Length(8388608f));
				obj.minWidth = Column.UxmlObjectTraits<T>.ParseLength(this.m_MinWidth.GetValueFromBag(bag, cc), new Length(35f));
				obj.sortable = this.m_Sortable.GetValueFromBag(bag, cc);
				obj.stretchable = this.m_Stretch.GetValueFromBag(bag, cc);
				obj.optional = this.m_Optional.GetValueFromBag(bag, cc);
				obj.resizable = this.m_Resizable.GetValueFromBag(bag, cc);
				obj.bindingPath = this.m_BindingPath.GetValueFromBag(bag, cc);
				string valueFromBag = this.m_HeaderTemplateId.GetValueFromBag(bag, cc);
				bool flag = !string.IsNullOrEmpty(valueFromBag);
				if (flag)
				{
					Column.UxmlObjectTraits<T>.<>c__DisplayClass14_0 CS$<>8__locals1 = new Column.UxmlObjectTraits<T>.<>c__DisplayClass14_0();
					Column.UxmlObjectTraits<T>.<>c__DisplayClass14_0 CS$<>8__locals2 = CS$<>8__locals1;
					VisualTreeAsset visualTreeAsset = cc.visualTreeAsset;
					CS$<>8__locals2.asset = ((visualTreeAsset != null) ? visualTreeAsset.ResolveTemplate(valueFromBag) : null);
					obj.makeHeader = delegate
					{
						bool flag3 = CS$<>8__locals1.asset != null;
						VisualElement visualElement;
						if (flag3)
						{
							visualElement = CS$<>8__locals1.asset.Instantiate();
						}
						else
						{
							visualElement = new Label(BaseVerticalCollectionView.k_InvalidTemplateError);
						}
						return visualElement;
					};
				}
				string valueFromBag2 = this.m_CellTemplateId.GetValueFromBag(bag, cc);
				bool flag2 = !string.IsNullOrEmpty(valueFromBag2);
				if (flag2)
				{
					Column.UxmlObjectTraits<T>.<>c__DisplayClass14_1 CS$<>8__locals3 = new Column.UxmlObjectTraits<T>.<>c__DisplayClass14_1();
					Column.UxmlObjectTraits<T>.<>c__DisplayClass14_1 CS$<>8__locals4 = CS$<>8__locals3;
					VisualTreeAsset visualTreeAsset2 = cc.visualTreeAsset;
					CS$<>8__locals4.asset = ((visualTreeAsset2 != null) ? visualTreeAsset2.ResolveTemplate(valueFromBag2) : null);
					obj.makeCell = delegate
					{
						bool flag4 = CS$<>8__locals3.asset != null;
						VisualElement visualElement2;
						if (flag4)
						{
							visualElement2 = CS$<>8__locals3.asset.Instantiate();
						}
						else
						{
							visualElement2 = new Label(BaseVerticalCollectionView.k_InvalidTemplateError);
						}
						return visualElement2;
					};
				}
			}

			private UxmlStringAttributeDescription m_Name = new UxmlStringAttributeDescription
			{
				name = "name"
			};

			private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
			{
				name = "title"
			};

			private UxmlBoolAttributeDescription m_Visible = new UxmlBoolAttributeDescription
			{
				name = "visible",
				defaultValue = true
			};

			private UxmlStringAttributeDescription m_Width = new UxmlStringAttributeDescription
			{
				name = "width"
			};

			private UxmlStringAttributeDescription m_MinWidth = new UxmlStringAttributeDescription
			{
				name = "min-width"
			};

			private UxmlStringAttributeDescription m_MaxWidth = new UxmlStringAttributeDescription
			{
				name = "max-width"
			};

			private UxmlBoolAttributeDescription m_Stretch = new UxmlBoolAttributeDescription
			{
				name = "stretchable"
			};

			private UxmlBoolAttributeDescription m_Sortable = new UxmlBoolAttributeDescription
			{
				name = "sortable",
				defaultValue = true
			};

			private UxmlBoolAttributeDescription m_Optional = new UxmlBoolAttributeDescription
			{
				name = "optional",
				defaultValue = true
			};

			private UxmlBoolAttributeDescription m_Resizable = new UxmlBoolAttributeDescription
			{
				name = "resizable",
				defaultValue = true
			};

			private UxmlStringAttributeDescription m_HeaderTemplateId = new UxmlStringAttributeDescription
			{
				name = "header-template"
			};

			private UxmlStringAttributeDescription m_CellTemplateId = new UxmlStringAttributeDescription
			{
				name = "cell-template"
			};

			private UxmlStringAttributeDescription m_BindingPath = new UxmlStringAttributeDescription
			{
				name = "binding-path"
			};
		}
	}
}
