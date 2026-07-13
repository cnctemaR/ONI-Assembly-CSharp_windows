using System;

namespace UnityEngine.UIElements
{
	internal struct TransformData : IStyleDataGroup<TransformData>, IEquatable<TransformData>
	{
		public TransformData Copy()
		{
			return this;
		}

		public void CopyFrom(ref TransformData other)
		{
			this = other;
		}

		public static bool operator ==(TransformData lhs, TransformData rhs)
		{
			return lhs.rotate == rhs.rotate && lhs.scale == rhs.scale && lhs.transformOrigin == rhs.transformOrigin && lhs.translate == rhs.translate;
		}

		public static bool operator !=(TransformData lhs, TransformData rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(TransformData other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return !flag && obj is TransformData && this.Equals((TransformData)obj);
		}

		public override int GetHashCode()
		{
			int num = this.rotate.GetHashCode();
			num = (num * 397) ^ this.scale.GetHashCode();
			num = (num * 397) ^ this.transformOrigin.GetHashCode();
			return (num * 397) ^ this.translate.GetHashCode();
		}

		public Rotate rotate;

		public Scale scale;

		public TransformOrigin transformOrigin;

		public Translate translate;
	}
}
