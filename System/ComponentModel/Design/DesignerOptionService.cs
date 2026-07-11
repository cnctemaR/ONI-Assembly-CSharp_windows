using System;
using System.Collections;
using System.Globalization;

namespace System.ComponentModel.Design
{
	public abstract class DesignerOptionService : IDesignerOptionService
	{
		protected internal DesignerOptionService()
		{
		}

		object IDesignerOptionService.GetOptionValue(string pageName, string valueName)
		{
			if (pageName == null)
			{
				throw new ArgumentNullException("pageName");
			}
			if (valueName == null)
			{
				throw new ArgumentNullException("valueName");
			}
			PropertyDescriptor optionProperty = this.GetOptionProperty(pageName, valueName);
			if (optionProperty != null)
			{
				return optionProperty.GetValue(null);
			}
			return null;
		}

		void IDesignerOptionService.SetOptionValue(string pageName, string valueName, object value)
		{
			if (pageName == null)
			{
				throw new ArgumentNullException("pageName");
			}
			if (valueName == null)
			{
				throw new ArgumentNullException("valueName");
			}
			PropertyDescriptor optionProperty = this.GetOptionProperty(pageName, valueName);
			if (optionProperty != null)
			{
				optionProperty.SetValue(null, value);
			}
		}

		protected DesignerOptionService.DesignerOptionCollection CreateOptionCollection(DesignerOptionService.DesignerOptionCollection parent, string name, object value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (name == string.Empty)
			{
				throw new ArgumentException("name.Length == 0");
			}
			return new DesignerOptionService.DesignerOptionCollection(parent, name, value, this);
		}

		protected virtual bool ShowDialog(DesignerOptionService.DesignerOptionCollection options, object optionObject)
		{
			return false;
		}

		protected virtual void PopulateOptionCollection(DesignerOptionService.DesignerOptionCollection options)
		{
		}

		public DesignerOptionService.DesignerOptionCollection Options
		{
			get
			{
				if (this._options == null)
				{
					this._options = new DesignerOptionService.DesignerOptionCollection(null, string.Empty, null, this);
				}
				return this._options;
			}
		}

		private PropertyDescriptor GetOptionProperty(string pageName, string valueName)
		{
			string[] array = pageName.Split(new char[] { '\\' });
			DesignerOptionService.DesignerOptionCollection designerOptionCollection = this.Options;
			foreach (string text in array)
			{
				designerOptionCollection = designerOptionCollection[text];
				if (designerOptionCollection == null)
				{
					return null;
				}
			}
			return designerOptionCollection.Properties[valueName];
		}

		private DesignerOptionService.DesignerOptionCollection _options;

		[TypeConverter(typeof(TypeConverter))]
		[Editor("", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[global::System.MonoTODO("implement own TypeConverter")]
		public sealed class DesignerOptionCollection : IList, ICollection, IEnumerable
		{
			internal DesignerOptionCollection(DesignerOptionService.DesignerOptionCollection parent, string name, object propertiesProvider, DesignerOptionService service)
			{
				this._name = name;
				this._propertiesProvider = propertiesProvider;
				this._parent = parent;
				if (parent != null)
				{
					if (parent._children == null)
					{
						parent._children = new ArrayList();
					}
					parent._children.Add(this);
				}
				this._children = new ArrayList();
				this._optionService = service;
				service.PopulateOptionCollection(this);
			}

			bool IList.IsFixedSize
			{
				get
				{
					return true;
				}
			}

			bool IList.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			object IList.this[int index]
			{
				get
				{
					return this[index];
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			bool IList.Contains(object item)
			{
				return this._children.Contains(item);
			}

			int IList.IndexOf(object item)
			{
				return this._children.IndexOf(item);
			}

			int IList.Add(object item)
			{
				throw new NotSupportedException();
			}

			void IList.Remove(object item)
			{
				throw new NotSupportedException();
			}

			void IList.RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			void IList.Insert(int index, object item)
			{
				throw new NotSupportedException();
			}

			void IList.Clear()
			{
				throw new NotSupportedException();
			}

			public bool ShowDialog()
			{
				return this._optionService.ShowDialog(this, this._propertiesProvider);
			}

			public DesignerOptionService.DesignerOptionCollection this[int index]
			{
				get
				{
					return (DesignerOptionService.DesignerOptionCollection)this._children[index];
				}
			}

			public DesignerOptionService.DesignerOptionCollection this[string index]
			{
				get
				{
					foreach (object obj in this._children)
					{
						DesignerOptionService.DesignerOptionCollection designerOptionCollection = (DesignerOptionService.DesignerOptionCollection)obj;
						if (string.Compare(designerOptionCollection.Name, index, true, CultureInfo.InvariantCulture) == 0)
						{
							return designerOptionCollection;
						}
					}
					return null;
				}
			}

			public string Name
			{
				get
				{
					return this._name;
				}
			}

			public int Count
			{
				get
				{
					if (this._children != null)
					{
						return this._children.Count;
					}
					return 0;
				}
			}

			public DesignerOptionService.DesignerOptionCollection Parent
			{
				get
				{
					return this._parent;
				}
			}

			public PropertyDescriptorCollection Properties
			{
				get
				{
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this._propertiesProvider);
					ArrayList arrayList = new ArrayList(properties.Count);
					foreach (object obj in properties)
					{
						PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj;
						arrayList.Add(new DesignerOptionService.DesignerOptionCollection.WrappedPropertyDescriptor(propertyDescriptor, this._propertiesProvider));
					}
					PropertyDescriptor[] array = (PropertyDescriptor[])arrayList.ToArray(typeof(PropertyDescriptor));
					return new PropertyDescriptorCollection(array);
				}
			}

			public IEnumerator GetEnumerator()
			{
				return this._children.GetEnumerator();
			}

			public int IndexOf(DesignerOptionService.DesignerOptionCollection item)
			{
				return this._children.IndexOf(item);
			}

			public void CopyTo(Array array, int index)
			{
				this._children.CopyTo(array, index);
			}

			private string _name;

			private object _propertiesProvider;

			private DesignerOptionService.DesignerOptionCollection _parent;

			private ArrayList _children;

			private DesignerOptionService _optionService;

			public sealed class WrappedPropertyDescriptor : PropertyDescriptor
			{
				public WrappedPropertyDescriptor(PropertyDescriptor property, object component)
					: base(property.Name, new Attribute[0])
				{
					this._property = property;
					this._component = component;
				}

				public override object GetValue(object ignored)
				{
					return this._property.GetValue(this._component);
				}

				public override void SetValue(object ignored, object value)
				{
					this._property.SetValue(this._component, value);
				}

				public override bool CanResetValue(object ignored)
				{
					return this._property.CanResetValue(this._component);
				}

				public override void ResetValue(object ignored)
				{
					this._property.ResetValue(this._component);
				}

				public override bool ShouldSerializeValue(object ignored)
				{
					return this._property.ShouldSerializeValue(this._component);
				}

				public override AttributeCollection Attributes
				{
					get
					{
						return this._property.Attributes;
					}
				}

				public override bool IsReadOnly
				{
					get
					{
						return this._property.IsReadOnly;
					}
				}

				public override Type ComponentType
				{
					get
					{
						return this._property.ComponentType;
					}
				}

				public override Type PropertyType
				{
					get
					{
						return this._property.PropertyType;
					}
				}

				private PropertyDescriptor _property;

				private object _component;
			}
		}
	}
}
