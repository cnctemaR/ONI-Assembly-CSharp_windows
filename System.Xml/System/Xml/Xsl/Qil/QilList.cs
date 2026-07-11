using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilList : QilNode
	{
		public QilList(QilNodeType nodeType)
			: base(nodeType)
		{
			this.members = new QilNode[4];
			this.xmlType = null;
		}

		public override XmlQueryType XmlType
		{
			get
			{
				if (this.xmlType == null)
				{
					XmlQueryType xmlQueryType = XmlQueryTypeFactory.Empty;
					if (this.count > 0)
					{
						if (this.nodeType == QilNodeType.Sequence)
						{
							for (int i = 0; i < this.count; i++)
							{
								xmlQueryType = XmlQueryTypeFactory.Sequence(xmlQueryType, this.members[i].XmlType);
							}
						}
						else if (this.nodeType == QilNodeType.BranchList)
						{
							xmlQueryType = this.members[0].XmlType;
							for (int j = 1; j < this.count; j++)
							{
								xmlQueryType = XmlQueryTypeFactory.Choice(xmlQueryType, this.members[j].XmlType);
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
			qilList.members = (QilNode[])this.members.Clone();
			return qilList;
		}

		public override int Count
		{
			get
			{
				return this.count;
			}
		}

		public override QilNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.count)
				{
					return this.members[index];
				}
				throw new IndexOutOfRangeException();
			}
			set
			{
				if (index >= 0 && index < this.count)
				{
					this.members[index] = value;
					this.xmlType = null;
					return;
				}
				throw new IndexOutOfRangeException();
			}
		}

		public override void Insert(int index, QilNode node)
		{
			if (index < 0 || index > this.count)
			{
				throw new IndexOutOfRangeException();
			}
			if (this.count == this.members.Length)
			{
				QilNode[] array = new QilNode[this.count * 2];
				Array.Copy(this.members, array, this.count);
				this.members = array;
			}
			if (index < this.count)
			{
				Array.Copy(this.members, index, this.members, index + 1, this.count - index);
			}
			this.count++;
			this.members[index] = node;
			this.xmlType = null;
		}

		public override void RemoveAt(int index)
		{
			if (index < 0 || index >= this.count)
			{
				throw new IndexOutOfRangeException();
			}
			this.count--;
			if (index < this.count)
			{
				Array.Copy(this.members, index + 1, this.members, index, this.count - index);
			}
			this.members[this.count] = null;
			this.xmlType = null;
		}

		private int count;

		private QilNode[] members;
	}
}
