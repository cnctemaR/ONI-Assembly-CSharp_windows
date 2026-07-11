using System;
using System.Runtime.Serialization;

namespace System.Transactions
{
	[MonoTODO("Not supported yet")]
	[Serializable]
	public sealed class DependentTransaction : Transaction, ISerializable
	{
		internal DependentTransaction(Transaction parent, DependentCloneOption option)
		{
		}

		internal bool Completed
		{
			get
			{
				return this.completed;
			}
		}

		[MonoTODO]
		public void Complete()
		{
			throw new NotImplementedException();
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.completed = info.GetBoolean("completed");
		}

		private bool completed;
	}
}
