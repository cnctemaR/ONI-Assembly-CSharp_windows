using System;
using System.Numerics.Hashing;

namespace System
{
	public readonly struct SequencePosition : IEquatable<SequencePosition>
	{
		public SequencePosition(object @object, int integer)
		{
			this._object = @object;
			this._integer = integer;
		}

		public object GetObject()
		{
			return this._object;
		}

		public int GetInteger()
		{
			return this._integer;
		}

		public bool Equals(SequencePosition other)
		{
			return this._integer == other._integer && object.Equals(this._object, other._object);
		}

		public override bool Equals(object obj)
		{
			if (obj is SequencePosition)
			{
				SequencePosition sequencePosition = (SequencePosition)obj;
				return this.Equals(sequencePosition);
			}
			return false;
		}

		public override int GetHashCode()
		{
			object @object = this._object;
			return HashHelpers.Combine((@object != null) ? @object.GetHashCode() : 0, this._integer);
		}

		private readonly object _object;

		private readonly int _integer;
	}
}
