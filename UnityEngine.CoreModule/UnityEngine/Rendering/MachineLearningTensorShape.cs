using System;

namespace UnityEngine.Rendering
{
	public struct MachineLearningTensorShape : IEquatable<MachineLearningTensorShape>
	{
		public override int GetHashCode()
		{
			return new ValueTuple<uint, uint, uint, uint, uint, uint, uint, ValueTuple<uint, uint>>(this.rank, this.D0, this.D1, this.D2, this.D3, this.D4, this.D5, new ValueTuple<uint, uint>(this.D6, this.D7)).GetHashCode();
		}

		public bool Equals(MachineLearningTensorShape other)
		{
			return this.rank == other.rank && this.D0 == other.D0 && this.D1 == other.D1 && this.D2 == other.D2 && this.D3 == other.D3 && this.D4 == other.D4 && this.D5 == other.D5 && this.D6 == other.D6 && this.D7 == other.D7;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is MachineLearningTensorShape)
			{
				MachineLearningTensorShape machineLearningTensorShape = (MachineLearningTensorShape)obj;
				flag = this.Equals(machineLearningTensorShape);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static bool operator ==(MachineLearningTensorShape lhs, MachineLearningTensorShape rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(MachineLearningTensorShape lhs, MachineLearningTensorShape rhs)
		{
			return !lhs.Equals(rhs);
		}

		public uint rank;

		public uint D0;

		public uint D1;

		public uint D2;

		public uint D3;

		public uint D4;

		public uint D5;

		public uint D6;

		public uint D7;
	}
}
