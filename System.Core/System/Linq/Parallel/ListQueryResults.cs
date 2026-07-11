using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class ListQueryResults<T> : QueryResults<T>
	{
		internal ListQueryResults(IList<T> source, int partitionCount, bool useStriping)
		{
			this._source = source;
			this._partitionCount = partitionCount;
			this._useStriping = useStriping;
		}

		internal override void GivePartitionedStream(IPartitionedStreamRecipient<T> recipient)
		{
			PartitionedStream<T, int> partitionedStream = this.GetPartitionedStream();
			recipient.Receive<int>(partitionedStream);
		}

		internal override bool IsIndexible
		{
			get
			{
				return true;
			}
		}

		internal override int ElementsCount
		{
			get
			{
				return this._source.Count;
			}
		}

		internal override T GetElement(int index)
		{
			return this._source[index];
		}

		internal PartitionedStream<T, int> GetPartitionedStream()
		{
			return ExchangeUtilities.PartitionDataSource<T>(this._source, this._partitionCount, this._useStriping);
		}

		private IList<T> _source;

		private int _partitionCount;

		private bool _useStriping;
	}
}
