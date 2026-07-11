using System;

namespace System.Xml
{
	internal abstract class BaseTreeIterator
	{
		internal BaseTreeIterator(DataSetMapper mapper)
		{
			this.mapper = mapper;
		}

		internal abstract XmlNode CurrentNode { get; }

		internal abstract bool Next();

		internal abstract bool NextRight();

		internal bool NextRowElement()
		{
			while (this.Next())
			{
				if (this.OnRowElement())
				{
					return true;
				}
			}
			return false;
		}

		internal bool NextRightRowElement()
		{
			return this.NextRight() && (this.OnRowElement() || this.NextRowElement());
		}

		internal bool OnRowElement()
		{
			XmlBoundElement xmlBoundElement = this.CurrentNode as XmlBoundElement;
			return xmlBoundElement != null && xmlBoundElement.Row != null;
		}

		protected DataSetMapper mapper;
	}
}
