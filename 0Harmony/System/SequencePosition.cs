using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System
{
	[NullableContext(2)]
	[Nullable(0)]
	internal readonly struct SequencePosition : IEquatable<global::System.SequencePosition>
	{
		public SequencePosition(object @object, int integer)
		{
			this._object = @object;
			this._integer = integer;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetObject()
		{
			return this._object;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int GetInteger()
		{
			return this._integer;
		}

		public bool Equals(global::System.SequencePosition other)
		{
			return this._integer == other._integer && object.Equals(this._object, other._object);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			if (obj is global::System.SequencePosition)
			{
				global::System.SequencePosition sequencePosition = (global::System.SequencePosition)obj;
				return this.Equals(sequencePosition);
			}
			return false;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode()
		{
			return global::System.HashCode.Combine<object, int>(this._object, this._integer);
		}

		private readonly object _object;

		private readonly int _integer;
	}
}
