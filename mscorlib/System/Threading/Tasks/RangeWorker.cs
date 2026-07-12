using System;
using System.Runtime.InteropServices;

namespace System.Threading.Tasks
{
	[StructLayout(LayoutKind.Auto)]
	internal struct RangeWorker
	{
		internal bool IsInitialized
		{
			get
			{
				return this._indexRanges != null;
			}
		}

		internal RangeWorker(IndexRange[] ranges, int nInitialRange, long nStep, bool use32BitCurrentIndex)
		{
			this._indexRanges = ranges;
			this._use32BitCurrentIndex = use32BitCurrentIndex;
			this._nCurrentIndexRange = nInitialRange;
			this._nStep = nStep;
			this._nIncrementValue = nStep;
			this._nMaxIncrementValue = 16L * nStep;
		}

		internal unsafe bool FindNewWork(out long nFromInclusiveLocal, out long nToExclusiveLocal)
		{
			int num = this._indexRanges.Length;
			IndexRange indexRange;
			long num2;
			for (;;)
			{
				indexRange = this._indexRanges[this._nCurrentIndexRange];
				if (indexRange._bRangeFinished == 0)
				{
					if (this._indexRanges[this._nCurrentIndexRange]._nSharedCurrentIndexOffset == null)
					{
						Interlocked.CompareExchange<Box<long>>(ref this._indexRanges[this._nCurrentIndexRange]._nSharedCurrentIndexOffset, new Box<long>(0L), null);
					}
					if (IntPtr.Size == 4 && this._use32BitCurrentIndex)
					{
						fixed (long* ptr = &this._indexRanges[this._nCurrentIndexRange]._nSharedCurrentIndexOffset.Value)
						{
							num2 = (long)Interlocked.Add(ref *(int*)ptr, (int)this._nIncrementValue) - this._nIncrementValue;
						}
					}
					else
					{
						num2 = Interlocked.Add(ref this._indexRanges[this._nCurrentIndexRange]._nSharedCurrentIndexOffset.Value, this._nIncrementValue) - this._nIncrementValue;
					}
					if (indexRange._nToExclusive - indexRange._nFromInclusive > num2)
					{
						break;
					}
					Interlocked.Exchange(ref this._indexRanges[this._nCurrentIndexRange]._bRangeFinished, 1);
				}
				this._nCurrentIndexRange = (this._nCurrentIndexRange + 1) % this._indexRanges.Length;
				num--;
				if (num <= 0)
				{
					goto Block_9;
				}
			}
			nFromInclusiveLocal = indexRange._nFromInclusive + num2;
			nToExclusiveLocal = nFromInclusiveLocal + this._nIncrementValue;
			if (nToExclusiveLocal > indexRange._nToExclusive || nToExclusiveLocal < indexRange._nFromInclusive)
			{
				nToExclusiveLocal = indexRange._nToExclusive;
			}
			if (this._nIncrementValue < this._nMaxIncrementValue)
			{
				this._nIncrementValue *= 2L;
				if (this._nIncrementValue > this._nMaxIncrementValue)
				{
					this._nIncrementValue = this._nMaxIncrementValue;
				}
			}
			return true;
			Block_9:
			nFromInclusiveLocal = 0L;
			nToExclusiveLocal = 0L;
			return false;
		}

		internal bool FindNewWork32(out int nFromInclusiveLocal32, out int nToExclusiveLocal32)
		{
			long num;
			long num2;
			bool flag = this.FindNewWork(out num, out num2);
			nFromInclusiveLocal32 = (int)num;
			nToExclusiveLocal32 = (int)num2;
			return flag;
		}

		internal readonly IndexRange[] _indexRanges;

		internal int _nCurrentIndexRange;

		internal long _nStep;

		internal long _nIncrementValue;

		internal readonly long _nMaxIncrementValue;

		internal readonly bool _use32BitCurrentIndex;
	}
}
