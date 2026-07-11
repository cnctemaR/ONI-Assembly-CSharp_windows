using System;
using System.Collections;
using System.Diagnostics;
using System.Xml;

namespace System.Configuration
{
	[DebuggerDisplay("Count = {Count}")]
	public abstract class ConfigurationElementCollection : ConfigurationElement, ICollection, IEnumerable
	{
		protected ConfigurationElementCollection()
		{
		}

		protected ConfigurationElementCollection(IComparer comparer)
		{
			this.comparer = comparer;
		}

		internal override void InitFromProperty(PropertyInformation propertyInfo)
		{
			ConfigurationCollectionAttribute configurationCollectionAttribute = propertyInfo.Property.CollectionAttribute;
			if (configurationCollectionAttribute == null)
			{
				configurationCollectionAttribute = Attribute.GetCustomAttribute(propertyInfo.Type, typeof(ConfigurationCollectionAttribute)) as ConfigurationCollectionAttribute;
			}
			if (configurationCollectionAttribute != null)
			{
				this.addElementName = configurationCollectionAttribute.AddItemName;
				this.clearElementName = configurationCollectionAttribute.ClearItemsName;
				this.removeElementName = configurationCollectionAttribute.RemoveItemName;
			}
			base.InitFromProperty(propertyInfo);
		}

		public virtual ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}

		private bool IsBasic
		{
			get
			{
				return this.CollectionType == ConfigurationElementCollectionType.BasicMap || this.CollectionType == ConfigurationElementCollectionType.BasicMapAlternate;
			}
		}

		private bool IsAlternate
		{
			get
			{
				return this.CollectionType == ConfigurationElementCollectionType.AddRemoveClearMapAlternate || this.CollectionType == ConfigurationElementCollectionType.BasicMapAlternate;
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		protected virtual string ElementName
		{
			get
			{
				return string.Empty;
			}
		}

		public bool EmitClear
		{
			get
			{
				return this.emitClear;
			}
			set
			{
				this.emitClear = value;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		protected virtual bool ThrowOnDuplicate
		{
			get
			{
				return this.CollectionType == ConfigurationElementCollectionType.AddRemoveClearMap || this.CollectionType == ConfigurationElementCollectionType.AddRemoveClearMapAlternate;
			}
		}

		protected internal string AddElementName
		{
			get
			{
				return this.addElementName;
			}
			set
			{
				this.addElementName = value;
			}
		}

		protected internal string ClearElementName
		{
			get
			{
				return this.clearElementName;
			}
			set
			{
				this.clearElementName = value;
			}
		}

		protected internal string RemoveElementName
		{
			get
			{
				return this.removeElementName;
			}
			set
			{
				this.removeElementName = value;
			}
		}

		protected virtual void BaseAdd(ConfigurationElement element)
		{
			this.BaseAdd(element, this.ThrowOnDuplicate);
		}

		protected void BaseAdd(ConfigurationElement element, bool throwIfExists)
		{
			if (this.IsReadOnly())
			{
				throw new ConfigurationErrorsException("Collection is read only.");
			}
			if (this.IsAlternate)
			{
				this.list.Insert(this.inheritedLimitIndex, element);
				this.inheritedLimitIndex++;
			}
			else
			{
				int num = this.IndexOfKey(this.GetElementKey(element));
				if (num >= 0)
				{
					if (element.Equals(this.list[num]))
					{
						return;
					}
					if (throwIfExists)
					{
						throw new ConfigurationErrorsException("Duplicate element in collection");
					}
					this.list.RemoveAt(num);
				}
				this.list.Add(element);
			}
			this.modified = true;
		}

		protected virtual void BaseAdd(int index, ConfigurationElement element)
		{
			if (this.ThrowOnDuplicate && this.BaseIndexOf(element) != -1)
			{
				throw new ConfigurationErrorsException("Duplicate element in collection");
			}
			if (this.IsReadOnly())
			{
				throw new ConfigurationErrorsException("Collection is read only.");
			}
			if (this.IsAlternate && index > this.inheritedLimitIndex)
			{
				throw new ConfigurationErrorsException("Can't insert new elements below the inherited elements.");
			}
			if (!this.IsAlternate && index <= this.inheritedLimitIndex)
			{
				throw new ConfigurationErrorsException("Can't insert new elements above the inherited elements.");
			}
			this.list.Insert(index, element);
			this.modified = true;
		}

		protected internal void BaseClear()
		{
			if (this.IsReadOnly())
			{
				throw new ConfigurationErrorsException("Collection is read only.");
			}
			this.list.Clear();
			this.modified = true;
		}

		protected internal ConfigurationElement BaseGet(int index)
		{
			return (ConfigurationElement)this.list[index];
		}

		protected internal ConfigurationElement BaseGet(object key)
		{
			int num = this.IndexOfKey(key);
			if (num != -1)
			{
				return (ConfigurationElement)this.list[num];
			}
			return null;
		}

		protected internal object[] BaseGetAllKeys()
		{
			object[] array = new object[this.list.Count];
			for (int i = 0; i < this.list.Count; i++)
			{
				array[i] = this.BaseGetKey(i);
			}
			return array;
		}

		protected internal object BaseGetKey(int index)
		{
			if (index < 0 || index >= this.list.Count)
			{
				throw new ConfigurationErrorsException(string.Format("Index {0} is out of range", index));
			}
			return this.GetElementKey((ConfigurationElement)this.list[index]).ToString();
		}

		protected int BaseIndexOf(ConfigurationElement element)
		{
			return this.list.IndexOf(element);
		}

		private int IndexOfKey(object key)
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				if (this.CompareKeys(this.GetElementKey((ConfigurationElement)this.list[i]), key))
				{
					return i;
				}
			}
			return -1;
		}

		protected internal bool BaseIsRemoved(object key)
		{
			if (this.removed == null)
			{
				return false;
			}
			foreach (object obj in this.removed)
			{
				ConfigurationElement configurationElement = (ConfigurationElement)obj;
				if (this.CompareKeys(this.GetElementKey(configurationElement), key))
				{
					return true;
				}
			}
			return false;
		}

		protected internal void BaseRemove(object key)
		{
			if (this.IsReadOnly())
			{
				throw new ConfigurationErrorsException("Collection is read only.");
			}
			int num = this.IndexOfKey(key);
			if (num != -1)
			{
				this.BaseRemoveAt(num);
				this.modified = true;
			}
		}

		protected internal void BaseRemoveAt(int index)
		{
			if (this.IsReadOnly())
			{
				throw new ConfigurationErrorsException("Collection is read only.");
			}
			ConfigurationElement configurationElement = (ConfigurationElement)this.list[index];
			if (!this.IsElementRemovable(configurationElement))
			{
				throw new ConfigurationErrorsException("Element can't be removed from element collection.");
			}
			if (this.inherited != null && this.inherited.Contains(configurationElement))
			{
				throw new ConfigurationErrorsException("Inherited items can't be removed.");
			}
			this.list.RemoveAt(index);
			if (this.IsAlternate && this.inheritedLimitIndex > 0)
			{
				this.inheritedLimitIndex--;
			}
			this.modified = true;
		}

		private bool CompareKeys(object key1, object key2)
		{
			if (this.comparer != null)
			{
				return this.comparer.Compare(key1, key2) == 0;
			}
			return object.Equals(key1, key2);
		}

		public void CopyTo(ConfigurationElement[] array, int index)
		{
			this.list.CopyTo(array, index);
		}

		protected abstract ConfigurationElement CreateNewElement();

		protected virtual ConfigurationElement CreateNewElement(string elementName)
		{
			return this.CreateNewElement();
		}

		private ConfigurationElement CreateNewElementInternal(string elementName)
		{
			ConfigurationElement configurationElement;
			if (elementName == null)
			{
				configurationElement = this.CreateNewElement();
			}
			else
			{
				configurationElement = this.CreateNewElement(elementName);
			}
			configurationElement.Init();
			return configurationElement;
		}

		public override bool Equals(object compareTo)
		{
			ConfigurationElementCollection configurationElementCollection = compareTo as ConfigurationElementCollection;
			if (configurationElementCollection == null)
			{
				return false;
			}
			if (base.GetType() != configurationElementCollection.GetType())
			{
				return false;
			}
			if (this.Count != configurationElementCollection.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Count; i++)
			{
				if (!this.BaseGet(i).Equals(configurationElementCollection.BaseGet(i)))
				{
					return false;
				}
			}
			return true;
		}

		protected abstract object GetElementKey(ConfigurationElement element);

		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.Count; i++)
			{
				num += this.BaseGet(i).GetHashCode();
			}
			return num;
		}

		void ICollection.CopyTo(Array arr, int index)
		{
			this.list.CopyTo(arr, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		protected virtual bool IsElementName(string elementName)
		{
			return false;
		}

		protected virtual bool IsElementRemovable(ConfigurationElement element)
		{
			return !this.IsReadOnly();
		}

		protected internal override bool IsModified()
		{
			if (this.modified)
			{
				return true;
			}
			for (int i = 0; i < this.list.Count; i++)
			{
				if (((ConfigurationElement)this.list[i]).IsModified())
				{
					this.modified = true;
					break;
				}
			}
			return this.modified;
		}

		[MonoTODO]
		public override bool IsReadOnly()
		{
			return base.IsReadOnly();
		}

		internal override void PrepareSave(ConfigurationElement parentElement, ConfigurationSaveMode mode)
		{
			ConfigurationElementCollection configurationElementCollection = (ConfigurationElementCollection)parentElement;
			base.PrepareSave(parentElement, mode);
			for (int i = 0; i < this.list.Count; i++)
			{
				ConfigurationElement configurationElement = (ConfigurationElement)this.list[i];
				object elementKey = this.GetElementKey(configurationElement);
				ConfigurationElement configurationElement2 = ((configurationElementCollection != null) ? configurationElementCollection.BaseGet(elementKey) : null);
				configurationElement.PrepareSave(configurationElement2, mode);
			}
		}

		internal override bool HasValues(ConfigurationElement parentElement, ConfigurationSaveMode mode)
		{
			ConfigurationElementCollection configurationElementCollection = (ConfigurationElementCollection)parentElement;
			if (mode == ConfigurationSaveMode.Full)
			{
				return this.list.Count > 0;
			}
			for (int i = 0; i < this.list.Count; i++)
			{
				ConfigurationElement configurationElement = (ConfigurationElement)this.list[i];
				object elementKey = this.GetElementKey(configurationElement);
				ConfigurationElement configurationElement2 = ((configurationElementCollection != null) ? configurationElementCollection.BaseGet(elementKey) : null);
				if (configurationElement.HasValues(configurationElement2, mode))
				{
					return true;
				}
			}
			return false;
		}

		protected internal override void Reset(ConfigurationElement parentElement)
		{
			bool isBasic = this.IsBasic;
			ConfigurationElementCollection configurationElementCollection = (ConfigurationElementCollection)parentElement;
			for (int i = 0; i < configurationElementCollection.Count; i++)
			{
				ConfigurationElement configurationElement = configurationElementCollection.BaseGet(i);
				ConfigurationElement configurationElement2 = this.CreateNewElementInternal(null);
				configurationElement2.Reset(configurationElement);
				this.BaseAdd(configurationElement2);
				if (isBasic)
				{
					if (this.inherited == null)
					{
						this.inherited = new ArrayList();
					}
					this.inherited.Add(configurationElement2);
				}
			}
			if (this.IsAlternate)
			{
				this.inheritedLimitIndex = 0;
			}
			else
			{
				this.inheritedLimitIndex = this.Count - 1;
			}
			this.modified = false;
		}

		protected internal override void ResetModified()
		{
			this.modified = false;
			for (int i = 0; i < this.list.Count; i++)
			{
				((ConfigurationElement)this.list[i]).ResetModified();
			}
		}

		[MonoTODO]
		protected internal override void SetReadOnly()
		{
			base.SetReadOnly();
		}

		protected internal override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
		{
			if (serializeCollectionKey)
			{
				return base.SerializeElement(writer, serializeCollectionKey);
			}
			bool flag = false;
			if (this.IsBasic)
			{
				for (int i = 0; i < this.list.Count; i++)
				{
					ConfigurationElement configurationElement = (ConfigurationElement)this.list[i];
					if (this.ElementName != string.Empty)
					{
						flag = configurationElement.SerializeToXmlElement(writer, this.ElementName) || flag;
					}
					else
					{
						flag = configurationElement.SerializeElement(writer, false) || flag;
					}
				}
			}
			else
			{
				if (this.emitClear)
				{
					writer.WriteElementString(this.clearElementName, "");
					flag = true;
				}
				if (this.removed != null)
				{
					for (int j = 0; j < this.removed.Count; j++)
					{
						writer.WriteStartElement(this.removeElementName);
						((ConfigurationElement)this.removed[j]).SerializeElement(writer, true);
						writer.WriteEndElement();
					}
					flag = flag || this.removed.Count > 0;
				}
				for (int k = 0; k < this.list.Count; k++)
				{
					((ConfigurationElement)this.list[k]).SerializeToXmlElement(writer, this.addElementName);
				}
				flag = flag || this.list.Count > 0;
			}
			return flag;
		}

		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			if (this.IsBasic)
			{
				ConfigurationElement configurationElement = null;
				if (elementName == this.ElementName)
				{
					configurationElement = this.CreateNewElementInternal(null);
				}
				if (this.IsElementName(elementName))
				{
					configurationElement = this.CreateNewElementInternal(elementName);
				}
				if (configurationElement != null)
				{
					configurationElement.DeserializeElement(reader, false);
					this.BaseAdd(configurationElement);
					this.modified = false;
					return true;
				}
			}
			else if (elementName == this.clearElementName)
			{
				reader.MoveToContent();
				if (reader.MoveToNextAttribute())
				{
					throw new ConfigurationErrorsException("Unrecognized attribute '" + reader.LocalName + "'.");
				}
				reader.MoveToElement();
				reader.Skip();
				this.BaseClear();
				this.emitClear = true;
				this.modified = false;
				return true;
			}
			else
			{
				if (elementName == this.removeElementName)
				{
					ConfigurationElementCollection.ConfigurationRemoveElement configurationRemoveElement = new ConfigurationElementCollection.ConfigurationRemoveElement(this.CreateNewElementInternal(null), this);
					configurationRemoveElement.DeserializeElement(reader, true);
					this.BaseRemove(configurationRemoveElement.KeyValue);
					this.modified = false;
					return true;
				}
				if (elementName == this.addElementName)
				{
					ConfigurationElement configurationElement2 = this.CreateNewElementInternal(null);
					configurationElement2.DeserializeElement(reader, false);
					this.BaseAdd(configurationElement2);
					this.modified = false;
					return true;
				}
			}
			return false;
		}

		protected internal override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
		{
			ConfigurationElementCollection configurationElementCollection = (ConfigurationElementCollection)sourceElement;
			ConfigurationElementCollection configurationElementCollection2 = (ConfigurationElementCollection)parentElement;
			for (int i = 0; i < configurationElementCollection.Count; i++)
			{
				ConfigurationElement configurationElement = configurationElementCollection.BaseGet(i);
				object elementKey = configurationElementCollection.GetElementKey(configurationElement);
				ConfigurationElement configurationElement2 = ((configurationElementCollection2 != null) ? configurationElementCollection2.BaseGet(elementKey) : null);
				ConfigurationElement configurationElement3 = this.CreateNewElementInternal(null);
				if (configurationElement2 != null && saveMode != ConfigurationSaveMode.Full)
				{
					configurationElement3.Unmerge(configurationElement, configurationElement2, saveMode);
					if (configurationElement3.HasValues(configurationElement2, saveMode))
					{
						this.BaseAdd(configurationElement3);
					}
				}
				else
				{
					configurationElement3.Unmerge(configurationElement, null, ConfigurationSaveMode.Full);
					this.BaseAdd(configurationElement3);
				}
			}
			if (saveMode == ConfigurationSaveMode.Full)
			{
				this.EmitClear = true;
				return;
			}
			if (configurationElementCollection2 != null)
			{
				for (int j = 0; j < configurationElementCollection2.Count; j++)
				{
					ConfigurationElement configurationElement4 = configurationElementCollection2.BaseGet(j);
					object elementKey2 = configurationElementCollection2.GetElementKey(configurationElement4);
					if (configurationElementCollection.IndexOfKey(elementKey2) == -1)
					{
						if (this.removed == null)
						{
							this.removed = new ArrayList();
						}
						this.removed.Add(configurationElement4);
					}
				}
			}
		}

		private ArrayList list = new ArrayList();

		private ArrayList removed;

		private ArrayList inherited;

		private bool emitClear;

		private bool modified;

		private IComparer comparer;

		private int inheritedLimitIndex;

		private string addElementName = "add";

		private string clearElementName = "clear";

		private string removeElementName = "remove";

		private sealed class ConfigurationRemoveElement : ConfigurationElement
		{
			internal ConfigurationRemoveElement(ConfigurationElement origElement, ConfigurationElementCollection origCollection)
			{
				this._origElement = origElement;
				this._origCollection = origCollection;
				foreach (object obj in origElement.Properties)
				{
					ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
					if (configurationProperty.IsKey)
					{
						this.properties.Add(configurationProperty);
					}
				}
			}

			internal object KeyValue
			{
				get
				{
					foreach (object obj in this.Properties)
					{
						ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
						this._origElement[configurationProperty] = base[configurationProperty];
					}
					return this._origCollection.GetElementKey(this._origElement);
				}
			}

			protected internal override ConfigurationPropertyCollection Properties
			{
				get
				{
					return this.properties;
				}
			}

			private readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

			private readonly ConfigurationElement _origElement;

			private readonly ConfigurationElementCollection _origCollection;
		}
	}
}
