using System;
using System.Collections.Generic;

namespace UnityEngine.Pool
{
	public class DictionaryPool<TKey, TValue> : CollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
	{
	}
}
