using System;

namespace System.Threading
{
	internal struct CancellationCallbackCoreWorkArguments
	{
		public CancellationCallbackCoreWorkArguments(SparselyPopulatedArrayFragment<CancellationCallbackInfo> currArrayFragment, int currArrayIndex)
		{
			this._currArrayFragment = currArrayFragment;
			this._currArrayIndex = currArrayIndex;
		}

		internal SparselyPopulatedArrayFragment<CancellationCallbackInfo> _currArrayFragment;

		internal int _currArrayIndex;
	}
}
