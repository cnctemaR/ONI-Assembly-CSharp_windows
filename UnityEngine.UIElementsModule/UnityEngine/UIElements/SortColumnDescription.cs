using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[UxmlObject]
	[Serializable]
	public class SortColumnDescription : INotifyBindablePropertyChanged
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

		[CreateProperty]
		public string columnName
		{
			get
			{
				return this.m_ColumnName;
			}
			set
			{
				bool flag = this.m_ColumnName == value;
				if (!flag)
				{
					this.m_ColumnName = value;
					Action<SortColumnDescription> action = this.changed;
					if (action != null)
					{
						action(this);
					}
					this.NotifyPropertyChanged(in SortColumnDescription.columnNameProperty);
				}
			}
		}

		[CreateProperty]
		public int columnIndex
		{
			get
			{
				return this.m_ColumnIndex;
			}
			set
			{
				bool flag = this.m_ColumnIndex == value;
				if (!flag)
				{
					this.m_ColumnIndex = value;
					Action<SortColumnDescription> action = this.changed;
					if (action != null)
					{
						action(this);
					}
					this.NotifyPropertyChanged(in SortColumnDescription.columnIndexProperty);
				}
			}
		}

		public Column column { get; internal set; }

		[CreateProperty]
		public SortDirection direction
		{
			get
			{
				return this.m_SortDirection;
			}
			set
			{
				bool flag = this.m_SortDirection == value;
				if (!flag)
				{
					this.m_SortDirection = value;
					Action<SortColumnDescription> action = this.changed;
					if (action != null)
					{
						action(this);
					}
					this.NotifyPropertyChanged(in SortColumnDescription.directionProperty);
				}
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<SortColumnDescription> changed;

		public SortColumnDescription()
		{
		}

		public SortColumnDescription(int columnIndex, SortDirection direction)
		{
			this.columnIndex = columnIndex;
			this.direction = direction;
		}

		public SortColumnDescription(string columnName, SortDirection direction)
		{
			this.columnName = columnName;
			this.direction = direction;
		}

		private void NotifyPropertyChanged(in BindingId property)
		{
			EventHandler<BindablePropertyChangedEventArgs> eventHandler = this.propertyChanged;
			if (eventHandler != null)
			{
				eventHandler(this, new BindablePropertyChangedEventArgs(in property));
			}
		}

		private static readonly BindingId columnNameProperty = "columnName";

		private static readonly BindingId columnIndexProperty = "columnIndex";

		private static readonly BindingId directionProperty = "direction";

		[SerializeField]
		private int m_ColumnIndex = -1;

		[SerializeField]
		private string m_ColumnName;

		[SerializeField]
		private SortDirection m_SortDirection;

		[ExcludeFromDocs]
		[Serializable]
		public class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(SortColumnDescription.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("columnName", "column-name", null, Array.Empty<string>()),
					new UxmlAttributeNames("columnIndex", "column-index", null, Array.Empty<string>()),
					new UxmlAttributeNames("direction", "direction", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new SortColumnDescription();
			}

			public override void Deserialize(object obj)
			{
				SortColumnDescription sortColumnDescription = (SortColumnDescription)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.columnName_UxmlAttributeFlags);
				if (flag)
				{
					sortColumnDescription.columnName = this.columnName;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.columnIndex_UxmlAttributeFlags);
				if (flag2)
				{
					sortColumnDescription.columnIndex = this.columnIndex;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.direction_UxmlAttributeFlags);
				if (flag3)
				{
					sortColumnDescription.direction = this.direction;
				}
			}

			[SerializeField]
			private string columnName;

			[SerializeField]
			private int columnIndex;

			[SerializeField]
			private SortDirection direction;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags columnName_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags columnIndex_UxmlAttributeFlags;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags direction_UxmlAttributeFlags;
		}

		[Obsolete("UxmlObjectFactory<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		internal class UxmlObjectFactory<T> : UxmlObjectFactory<T, SortColumnDescription.UxmlObjectTraits<T>> where T : SortColumnDescription, new()
		{
		}

		[Obsolete("UxmlObjectFactory<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		internal class UxmlObjectFactory : SortColumnDescription.UxmlObjectFactory<SortColumnDescription>
		{
		}

		[Obsolete("UxmlObjectTraits<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		internal class UxmlObjectTraits<T> : UnityEngine.UIElements.UxmlObjectTraits<T> where T : SortColumnDescription
		{
			public override void Init(ref T obj, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ref obj, bag, cc);
				obj.columnName = this.m_ColumnName.GetValueFromBag(bag, cc);
				obj.columnIndex = this.m_ColumnIndex.GetValueFromBag(bag, cc);
				obj.direction = this.m_SortDescription.GetValueFromBag(bag, cc);
			}

			private readonly UxmlStringAttributeDescription m_ColumnName = new UxmlStringAttributeDescription
			{
				name = "column-name"
			};

			private readonly UxmlIntAttributeDescription m_ColumnIndex = new UxmlIntAttributeDescription
			{
				name = "column-index",
				defaultValue = -1
			};

			private readonly UxmlEnumAttributeDescription<SortDirection> m_SortDescription = new UxmlEnumAttributeDescription<SortDirection>
			{
				name = "direction",
				defaultValue = SortDirection.Ascending
			};
		}
	}
}
