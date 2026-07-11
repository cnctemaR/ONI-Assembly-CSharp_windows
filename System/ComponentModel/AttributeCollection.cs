using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public class AttributeCollection : ICollection, IEnumerable
	{
		internal AttributeCollection(ArrayList attributes)
		{
			if (attributes != null)
			{
				this.attrList = attributes;
			}
		}

		public AttributeCollection(params Attribute[] attributes)
		{
			if (attributes != null)
			{
				for (int i = 0; i < attributes.Length; i++)
				{
					this.attrList.Add(attributes[i]);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.attrList.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.attrList.SyncRoot;
			}
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		public static AttributeCollection FromExisting(AttributeCollection existing, params Attribute[] newAttributes)
		{
			if (existing == null)
			{
				throw new ArgumentNullException("existing");
			}
			AttributeCollection attributeCollection = new AttributeCollection(new Attribute[0]);
			attributeCollection.attrList.AddRange(existing.attrList);
			if (newAttributes != null)
			{
				attributeCollection.attrList.AddRange(newAttributes);
			}
			return attributeCollection;
		}

		public bool Contains(Attribute attr)
		{
			Attribute attribute = this[attr.GetType()];
			return attribute != null && attr.Equals(attribute);
		}

		public bool Contains(Attribute[] attributes)
		{
			if (attributes == null)
			{
				return true;
			}
			foreach (Attribute attribute in attributes)
			{
				if (!this.Contains(attribute))
				{
					return false;
				}
			}
			return true;
		}

		public void CopyTo(Array array, int index)
		{
			this.attrList.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.attrList.GetEnumerator();
		}

		public bool Matches(Attribute attr)
		{
			foreach (object obj in this.attrList)
			{
				Attribute attribute = (Attribute)obj;
				if (attribute.Match(attr))
				{
					return true;
				}
			}
			return false;
		}

		public bool Matches(Attribute[] attributes)
		{
			foreach (Attribute attribute in attributes)
			{
				if (!this.Matches(attribute))
				{
					return false;
				}
			}
			return true;
		}

		protected Attribute GetDefaultAttribute(Type attributeType)
		{
			Attribute attribute = null;
			BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public;
			FieldInfo field = attributeType.GetField("Default", bindingFlags);
			if (field == null)
			{
				ConstructorInfo constructor = attributeType.GetConstructor(Type.EmptyTypes);
				if (constructor != null)
				{
					attribute = constructor.Invoke(null) as Attribute;
				}
				if (attribute != null && !attribute.IsDefaultAttribute())
				{
					attribute = null;
				}
			}
			else
			{
				attribute = (Attribute)field.GetValue(null);
			}
			return attribute;
		}

		public int Count
		{
			get
			{
				return (this.attrList == null) ? 0 : this.attrList.Count;
			}
		}

		public virtual Attribute this[Type type]
		{
			get
			{
				Attribute attribute = null;
				if (this.attrList != null)
				{
					foreach (object obj in this.attrList)
					{
						Attribute attribute2 = (Attribute)obj;
						if (type.IsAssignableFrom(attribute2.GetType()))
						{
							attribute = attribute2;
							break;
						}
					}
				}
				if (attribute == null)
				{
					attribute = this.GetDefaultAttribute(type);
				}
				return attribute;
			}
		}

		public virtual Attribute this[int index]
		{
			get
			{
				return (Attribute)this.attrList[index];
			}
		}

		private ArrayList attrList = new ArrayList();

		public static readonly AttributeCollection Empty = new AttributeCollection(null);
	}
}
