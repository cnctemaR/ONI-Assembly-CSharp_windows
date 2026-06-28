using System;

namespace MIConvexHull
{
	internal sealed class FaceList
	{
		public ConvexFaceInternal First { get; private set; }

		private void AddFirst(ConvexFaceInternal face)
		{
			face.InList = true;
			this.First.Previous = face;
			face.Next = this.First;
			this.First = face;
		}

		public void Add(ConvexFaceInternal face)
		{
			if (face.InList)
			{
				if (this.First.VerticesBeyond.Count < face.VerticesBeyond.Count)
				{
					this.Remove(face);
					this.AddFirst(face);
				}
			}
			else
			{
				face.InList = true;
				if (this.First != null && this.First.VerticesBeyond.Count < face.VerticesBeyond.Count)
				{
					this.First.Previous = face;
					face.Next = this.First;
					this.First = face;
				}
				else
				{
					if (this.last != null)
					{
						this.last.Next = face;
					}
					face.Previous = this.last;
					this.last = face;
					if (this.First == null)
					{
						this.First = face;
					}
				}
			}
		}

		public void Remove(ConvexFaceInternal face)
		{
			if (face.InList)
			{
				face.InList = false;
				if (face.Previous != null)
				{
					face.Previous.Next = face.Next;
				}
				else if (face.Previous == null)
				{
					this.First = face.Next;
				}
				if (face.Next != null)
				{
					face.Next.Previous = face.Previous;
				}
				else if (face.Next == null)
				{
					this.last = face.Previous;
				}
				face.Next = null;
				face.Previous = null;
			}
		}

		private ConvexFaceInternal last;
	}
}
