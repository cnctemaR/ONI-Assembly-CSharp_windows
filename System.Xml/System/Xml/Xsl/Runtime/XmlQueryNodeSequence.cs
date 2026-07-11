using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class XmlQueryNodeSequence : XmlQuerySequence<XPathNavigator>, IList<XPathItem>, ICollection<XPathItem>, IEnumerable<XPathItem>, IEnumerable
	{
		public static XmlQueryNodeSequence CreateOrReuse(XmlQueryNodeSequence seq)
		{
			if (seq != null)
			{
				seq.Clear();
				return seq;
			}
			return new XmlQueryNodeSequence();
		}

		public static XmlQueryNodeSequence CreateOrReuse(XmlQueryNodeSequence seq, XPathNavigator navigator)
		{
			if (seq != null)
			{
				seq.Clear();
				seq.Add(navigator);
				return seq;
			}
			return new XmlQueryNodeSequence(navigator);
		}

		public XmlQueryNodeSequence()
		{
		}

		public XmlQueryNodeSequence(int capacity)
			: base(capacity)
		{
		}

		public XmlQueryNodeSequence(IList<XPathNavigator> list)
			: base(list.Count)
		{
			for (int i = 0; i < list.Count; i++)
			{
				this.AddClone(list[i]);
			}
		}

		public XmlQueryNodeSequence(XPathNavigator[] array, int size)
			: base(array, size)
		{
		}

		public XmlQueryNodeSequence(XPathNavigator navigator)
			: base(1)
		{
			this.AddClone(navigator);
		}

		public bool IsDocOrderDistinct
		{
			get
			{
				return this.docOrderDistinct == this || base.Count <= 1;
			}
			set
			{
				this.docOrderDistinct = (value ? this : null);
			}
		}

		public XmlQueryNodeSequence DocOrderDistinct(IComparer<XPathNavigator> comparer)
		{
			if (this.docOrderDistinct != null)
			{
				return this.docOrderDistinct;
			}
			if (base.Count <= 1)
			{
				return this;
			}
			XPathNavigator[] array = new XPathNavigator[base.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = base[i];
			}
			Array.Sort<XPathNavigator>(array, 0, base.Count, comparer);
			int num = 0;
			for (int i = 1; i < array.Length; i++)
			{
				if (!array[num].IsSamePosition(array[i]))
				{
					num++;
					if (num != i)
					{
						array[num] = array[i];
					}
				}
			}
			this.docOrderDistinct = new XmlQueryNodeSequence(array, num + 1);
			this.docOrderDistinct.docOrderDistinct = this.docOrderDistinct;
			return this.docOrderDistinct;
		}

		public void AddClone(XPathNavigator navigator)
		{
			base.Add(navigator.Clone());
		}

		protected override void OnItemsChanged()
		{
			this.docOrderDistinct = null;
		}

		IEnumerator<XPathItem> IEnumerable<XPathItem>.GetEnumerator()
		{
			return new IListEnumerator<XPathItem>(this);
		}

		bool ICollection<XPathItem>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		void ICollection<XPathItem>.Add(XPathItem value)
		{
			throw new NotSupportedException();
		}

		void ICollection<XPathItem>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<XPathItem>.Contains(XPathItem value)
		{
			return base.IndexOf((XPathNavigator)value) != -1;
		}

		void ICollection<XPathItem>.CopyTo(XPathItem[] array, int index)
		{
			for (int i = 0; i < base.Count; i++)
			{
				array[index + i] = base[i];
			}
		}

		bool ICollection<XPathItem>.Remove(XPathItem value)
		{
			throw new NotSupportedException();
		}

		XPathItem IList<XPathItem>.this[int index]
		{
			get
			{
				if (index >= base.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return base[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		int IList<XPathItem>.IndexOf(XPathItem value)
		{
			return base.IndexOf((XPathNavigator)value);
		}

		void IList<XPathItem>.Insert(int index, XPathItem value)
		{
			throw new NotSupportedException();
		}

		void IList<XPathItem>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public new static readonly XmlQueryNodeSequence Empty = new XmlQueryNodeSequence();

		private XmlQueryNodeSequence docOrderDistinct;
	}
}
