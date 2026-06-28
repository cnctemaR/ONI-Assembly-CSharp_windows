using System;

namespace Newtonsoft.Json.Linq
{
	public class JsonMergeSettings
	{
		public MergeArrayHandling MergeArrayHandling
		{
			get
			{
				return this._mergeArrayHandling;
			}
			set
			{
				if (value < MergeArrayHandling.Concat || value > MergeArrayHandling.Merge)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._mergeArrayHandling = value;
			}
		}

		private MergeArrayHandling _mergeArrayHandling;
	}
}
