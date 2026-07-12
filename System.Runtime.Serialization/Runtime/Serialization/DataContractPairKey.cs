using System;

namespace System.Runtime.Serialization
{
	internal class DataContractPairKey
	{
		public DataContractPairKey(object object1, object object2)
		{
			this.object1 = object1;
			this.object2 = object2;
		}

		public override bool Equals(object other)
		{
			DataContractPairKey dataContractPairKey = other as DataContractPairKey;
			return dataContractPairKey != null && ((dataContractPairKey.object1 == this.object1 && dataContractPairKey.object2 == this.object2) || (dataContractPairKey.object1 == this.object2 && dataContractPairKey.object2 == this.object1));
		}

		public override int GetHashCode()
		{
			return this.object1.GetHashCode() ^ this.object2.GetHashCode();
		}

		private object object1;

		private object object2;
	}
}
