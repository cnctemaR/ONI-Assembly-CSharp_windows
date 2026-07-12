using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilList : QilNode
	{
		public QilList(QilNodeType nodeType)
			: base(nodeType)
		{
			this._members = new QilNode[4];
			this.xmlType = null;
		}

		public override XmlQueryType XmlType
		{
			get
			{
				if (this.xmlType == null)
				{
					XmlQueryType xmlQueryType = XmlQueryTypeFactory.Empty;
					if (this._count > 0)
					{
						if (this.nodeType == QilNodeType.Sequence)
						{
							for (int i = 0; i < this._count; i++)
							{
								xmlQueryType = XmlQueryTypeFactory.Sequence(xmlQueryType, this._members[i].XmlType);
							}
						}
						else if (this.nodeType == QilNodeType.BranchList)
						{
							xmlQueryType = this._members[0].XmlType;
							for (int j = 1; j < this._count; j++)
							{
								xmlQueryType = XmlQueryTypeFactory.Choice(xmlQueryType, this._members[j].XmlType);
							}
						}
					}
					this.xmlType = xmlQueryType;
				}
				return this.xmlType;
			}
		}

		public override QilNode ShallowClone(QilFactory f)
		{
			QilList qilList = (QilList)base.MemberwiseClone();
			qilList._members = (QilNode[])this._members.Clone();
			return qilList;
		}

		public override int Count
		{
			get
			{
				return this._count;
			}
		}

		public override QilNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this._count)
				{
					return this._members[index];
				}
				throw new IndexOutOfRangeException();
			}
			set
			{
				if (index >= 0 && index < this._count)
				{
					this._members[index] = value;
					this.xmlType = null;
					return;
				}
				throw new IndexOutOfRangeException();
			}
		}

		public override void Insert(int index, QilNode node)
		{
			if (index < 0 || index > this._count)
			{
				throw new IndexOutOfRangeException();
			}
			if (this._count == this._members.Length)
			{
				QilNode[] array = new QilNode[this._count * 2];
				Array.Copy(this._members, array, this._count);
				this._members = array;
			}
			if (index < this._count)
			{
				Array.Copy(this._members, index, this._members, index + 1, this._count - index);
			}
			this._count++;
			this._members[index] = node;
			this.xmlType = null;
		}

		public override void RemoveAt(int index)
		{
			if (index < 0 || index >= this._count)
			{
				throw new IndexOutOfRangeException();
			}
			this._count--;
			if (index < this._count)
			{
				Array.Copy(this._members, index + 1, this._members, index, this._count - index);
			}
			this._members[this._count] = null;
			this.xmlType = null;
		}

		private int _count;

		private QilNode[] _members;
	}
}
