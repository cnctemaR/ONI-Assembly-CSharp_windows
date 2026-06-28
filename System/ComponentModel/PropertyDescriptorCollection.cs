using System;
using System.Collections;

namespace System.ComponentModel
{
	public class PropertyDescriptorCollection : IDictionary, IList, ICollection, IEnumerable
	{
		public PropertyDescriptorCollection(PropertyDescriptor[] properties)
		{
			this.properties = new ArrayList();
			if (properties == null)
			{
				return;
			}
			this.properties.AddRange(properties);
		}

		public PropertyDescriptorCollection(PropertyDescriptor[] properties, bool readOnly)
			: this(properties)
		{
			this.readOnly = readOnly;
		}

		private PropertyDescriptorCollection()
		{
		}

		int IList.Add(object value)
		{
			return this.Add((PropertyDescriptor)value);
		}

		void IDictionary.Add(object key, object value)
		{
			if (!(value is PropertyDescriptor))
			{
				throw new ArgumentException("value");
			}
			this.Add((PropertyDescriptor)value);
		}

		void IList.Clear()
		{
			this.Clear();
		}

		void IDictionary.Clear()
		{
			this.Clear();
		}

		bool IList.Contains(object value)
		{
			return this.Contains((PropertyDescriptor)value);
		}

		bool IDictionary.Contains(object value)
		{
			return this.Contains((PropertyDescriptor)value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		[global::System.MonoTODO]
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf((PropertyDescriptor)value);
		}

		void IList.Insert(int index, object value)
		{
			this.Insert(index, (PropertyDescriptor)value);
		}

		void IDictionary.Remove(object value)
		{
			this.Remove((PropertyDescriptor)value);
		}

		void IList.Remove(object value)
		{
			this.Remove((PropertyDescriptor)value);
		}

		void IList.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return ((IList)this).IsFixedSize;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return this.readOnly;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return ((IList)this).IsReadOnly;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				string[] array = new string[this.properties.Count];
				int num = 0;
				foreach (object obj in this.properties)
				{
					PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj;
					array[num++] = propertyDescriptor.Name;
				}
				return array;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return (ICollection)this.properties.Clone();
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				if (!(key is string))
				{
					return null;
				}
				return this[(string)key];
			}
			set
			{
				if (this.readOnly)
				{
					throw new NotSupportedException();
				}
				if (!(key is string) || !(value is PropertyDescriptor))
				{
					throw new ArgumentException();
				}
				int num = this.properties.IndexOf(value);
				if (num == -1)
				{
					this.Add((PropertyDescriptor)value);
				}
				else
				{
					this.properties[num] = value;
				}
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.properties[index];
			}
			set
			{
				if (this.readOnly)
				{
					throw new NotSupportedException();
				}
				this.properties[index] = value;
			}
		}

		public int Add(PropertyDescriptor value)
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			this.properties.Add(value);
			return this.properties.Count - 1;
		}

		public void Clear()
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			this.properties.Clear();
		}

		public bool Contains(PropertyDescriptor value)
		{
			return this.properties.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			this.properties.CopyTo(array, index);
		}

		public virtual PropertyDescriptor Find(string name, bool ignoreCase)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			for (int i = 0; i < this.properties.Count; i++)
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)this.properties[i];
				if (ignoreCase)
				{
					if (string.Compare(name, propertyDescriptor.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return propertyDescriptor;
					}
				}
				else if (string.Compare(name, propertyDescriptor.Name, StringComparison.Ordinal) == 0)
				{
					return propertyDescriptor;
				}
			}
			return null;
		}

		public virtual IEnumerator GetEnumerator()
		{
			return this.properties.GetEnumerator();
		}

		public int IndexOf(PropertyDescriptor value)
		{
			return this.properties.IndexOf(value);
		}

		public void Insert(int index, PropertyDescriptor value)
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			this.properties.Insert(index, value);
		}

		public void Remove(PropertyDescriptor value)
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			this.properties.Remove(value);
		}

		public void RemoveAt(int index)
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			this.properties.RemoveAt(index);
		}

		private PropertyDescriptorCollection CloneCollection()
		{
			return new PropertyDescriptorCollection
			{
				properties = (ArrayList)this.properties.Clone()
			};
		}

		public virtual PropertyDescriptorCollection Sort()
		{
			PropertyDescriptorCollection propertyDescriptorCollection = this.CloneCollection();
			propertyDescriptorCollection.InternalSort(null);
			return propertyDescriptorCollection;
		}

		public virtual PropertyDescriptorCollection Sort(IComparer comparer)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = this.CloneCollection();
			propertyDescriptorCollection.InternalSort(comparer);
			return propertyDescriptorCollection;
		}

		public virtual PropertyDescriptorCollection Sort(string[] order)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = this.CloneCollection();
			propertyDescriptorCollection.InternalSort(order);
			return propertyDescriptorCollection;
		}

		public virtual PropertyDescriptorCollection Sort(string[] order, IComparer comparer)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = this.CloneCollection();
			if (order != null)
			{
				ArrayList arrayList = propertyDescriptorCollection.ExtractItems(order);
				propertyDescriptorCollection.InternalSort(comparer);
				arrayList.AddRange(propertyDescriptorCollection.properties);
				propertyDescriptorCollection.properties = arrayList;
			}
			else
			{
				propertyDescriptorCollection.InternalSort(comparer);
			}
			return propertyDescriptorCollection;
		}

		protected void InternalSort(IComparer ic)
		{
			if (ic == null)
			{
				ic = MemberDescriptor.DefaultComparer;
			}
			this.properties.Sort(ic);
		}

		protected void InternalSort(string[] order)
		{
			if (order != null)
			{
				ArrayList arrayList = this.ExtractItems(order);
				this.InternalSort(null);
				arrayList.AddRange(this.properties);
				this.properties = arrayList;
			}
			else
			{
				this.InternalSort(null);
			}
		}

		private ArrayList ExtractItems(string[] names)
		{
			ArrayList arrayList = new ArrayList(this.properties.Count);
			object[] array = new object[names.Length];
			for (int i = 0; i < this.properties.Count; i++)
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)this.properties[i];
				int num = Array.IndexOf<string>(names, propertyDescriptor.Name);
				if (num != -1)
				{
					array[num] = propertyDescriptor;
					this.properties.RemoveAt(i);
					i--;
				}
			}
			foreach (object obj in array)
			{
				if (obj != null)
				{
					arrayList.Add(obj);
				}
			}
			return arrayList;
		}

		internal PropertyDescriptorCollection Filter(Attribute[] attributes)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object obj in this.properties)
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj;
				if (propertyDescriptor.Attributes.Contains(attributes))
				{
					arrayList.Add(propertyDescriptor);
				}
			}
			PropertyDescriptor[] array = new PropertyDescriptor[arrayList.Count];
			arrayList.CopyTo(array);
			return new PropertyDescriptorCollection(array, true);
		}

		public int Count
		{
			get
			{
				return this.properties.Count;
			}
		}

		public virtual PropertyDescriptor this[string s]
		{
			get
			{
				return this.Find(s, false);
			}
		}

		public virtual PropertyDescriptor this[int index]
		{
			get
			{
				return (PropertyDescriptor)this.properties[index];
			}
		}

		public static readonly PropertyDescriptorCollection Empty = new PropertyDescriptorCollection(null, true);

		private ArrayList properties;

		private bool readOnly;
	}
}
