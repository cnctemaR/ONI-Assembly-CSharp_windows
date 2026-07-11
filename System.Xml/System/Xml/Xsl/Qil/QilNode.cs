using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Xsl.Qil
{
	internal class QilNode : IList<QilNode>, ICollection<QilNode>, IEnumerable<QilNode>, IEnumerable
	{
		public QilNode(QilNodeType nodeType)
		{
			this.nodeType = nodeType;
		}

		public QilNode(QilNodeType nodeType, XmlQueryType xmlType)
		{
			this.nodeType = nodeType;
			this.xmlType = xmlType;
		}

		public QilNodeType NodeType
		{
			get
			{
				return this.nodeType;
			}
			set
			{
				this.nodeType = value;
			}
		}

		public virtual XmlQueryType XmlType
		{
			get
			{
				return this.xmlType;
			}
			set
			{
				this.xmlType = value;
			}
		}

		public ISourceLineInfo SourceLine
		{
			get
			{
				return this.sourceLine;
			}
			set
			{
				this.sourceLine = value;
			}
		}

		public object Annotation
		{
			get
			{
				return this.annotation;
			}
			set
			{
				this.annotation = value;
			}
		}

		public virtual QilNode DeepClone(QilFactory f)
		{
			return new QilCloneVisitor(f).Clone(this);
		}

		public virtual QilNode ShallowClone(QilFactory f)
		{
			return (QilNode)base.MemberwiseClone();
		}

		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		public virtual QilNode this[int index]
		{
			get
			{
				throw new IndexOutOfRangeException();
			}
			set
			{
				throw new IndexOutOfRangeException();
			}
		}

		public virtual void Insert(int index, QilNode node)
		{
			throw new NotSupportedException();
		}

		public virtual void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<QilNode> GetEnumerator()
		{
			return new IListEnumerator<QilNode>(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new IListEnumerator<QilNode>(this);
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public virtual void Add(QilNode node)
		{
			this.Insert(this.Count, node);
		}

		public virtual void Add(IList<QilNode> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				this.Insert(this.Count, list[i]);
			}
		}

		public virtual void Clear()
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				this.RemoveAt(i);
			}
		}

		public virtual bool Contains(QilNode node)
		{
			return this.IndexOf(node) != -1;
		}

		public virtual void CopyTo(QilNode[] array, int index)
		{
			for (int i = 0; i < this.Count; i++)
			{
				array[index + i] = this[i];
			}
		}

		public virtual bool Remove(QilNode node)
		{
			int num = this.IndexOf(node);
			if (num >= 0)
			{
				this.RemoveAt(num);
				return true;
			}
			return false;
		}

		public virtual int IndexOf(QilNode node)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (node.Equals(this[i]))
				{
					return i;
				}
			}
			return -1;
		}

		protected QilNodeType nodeType;

		protected XmlQueryType xmlType;

		protected ISourceLineInfo sourceLine;

		protected object annotation;
	}
}
