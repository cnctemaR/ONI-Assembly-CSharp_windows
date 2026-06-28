using System;

namespace MIConvexHull
{
	internal sealed class ConnectorList
	{
		public FaceConnector First { get; private set; }

		private void AddFirst(FaceConnector connector)
		{
			this.First.Previous = connector;
			connector.Next = this.First;
			this.First = connector;
		}

		public void Add(FaceConnector element)
		{
			if (this.last != null)
			{
				this.last.Next = element;
			}
			element.Previous = this.last;
			this.last = element;
			if (this.First == null)
			{
				this.First = element;
			}
		}

		public void Remove(FaceConnector connector)
		{
			if (connector.Previous != null)
			{
				connector.Previous.Next = connector.Next;
			}
			else if (connector.Previous == null)
			{
				this.First = connector.Next;
			}
			if (connector.Next != null)
			{
				connector.Next.Previous = connector.Previous;
			}
			else if (connector.Next == null)
			{
				this.last = connector.Previous;
			}
			connector.Next = null;
			connector.Previous = null;
		}

		private FaceConnector last;
	}
}
