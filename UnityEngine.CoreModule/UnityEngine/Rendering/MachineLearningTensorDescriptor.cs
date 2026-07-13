using System;

namespace UnityEngine.Rendering
{
	public struct MachineLearningTensorDescriptor : IEquatable<MachineLearningTensorDescriptor>
	{
		public override int GetHashCode()
		{
			return new ValueTuple<MachineLearningDataType, MachineLearningTensorShape>(this.dataType, this.shape).GetHashCode();
		}

		public bool Equals(MachineLearningTensorDescriptor other)
		{
			return this.dataType == other.dataType && this.shape.Equals(other.shape);
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is MachineLearningTensorDescriptor)
			{
				MachineLearningTensorDescriptor machineLearningTensorDescriptor = (MachineLearningTensorDescriptor)obj;
				flag = this.Equals(machineLearningTensorDescriptor);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static bool operator ==(MachineLearningTensorDescriptor lhs, MachineLearningTensorDescriptor rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(MachineLearningTensorDescriptor lhs, MachineLearningTensorDescriptor rhs)
		{
			return !lhs.Equals(rhs);
		}

		public MachineLearningTensorDescriptor(MachineLearningDataType dataType, MachineLearningTensorShape shape)
		{
			this.hasValue = true;
			this.dataType = dataType;
			this.shape = shape;
		}

		public static MachineLearningTensorDescriptor NullTensor()
		{
			return new MachineLearningTensorDescriptor
			{
				hasValue = false,
				dataType = MachineLearningDataType.Float32,
				shape = default(MachineLearningTensorShape)
			};
		}

		internal bool hasValue;

		public MachineLearningDataType dataType;

		public MachineLearningTensorShape shape;
	}
}
