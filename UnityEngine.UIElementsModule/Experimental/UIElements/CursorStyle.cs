using System;

namespace UnityEngine.Experimental.UIElements
{
	public struct CursorStyle : IEquatable<CursorStyle>
	{
		public Texture2D texture { get; set; }

		public Vector2 hotspot { get; set; }

		internal int defaultCursorId { get; set; }

		public bool Equals(CursorStyle other)
		{
			return object.Equals(this.texture, other.texture) && this.hotspot.Equals(other.hotspot) && this.defaultCursorId == other.defaultCursorId;
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && obj is CursorStyle && this.Equals((CursorStyle)obj);
		}

		public override int GetHashCode()
		{
			int num = ((!(this.texture != null)) ? 0 : this.texture.GetHashCode());
			num = (num * 397) ^ this.hotspot.GetHashCode();
			return (num * 397) ^ this.defaultCursorId;
		}
	}
}
