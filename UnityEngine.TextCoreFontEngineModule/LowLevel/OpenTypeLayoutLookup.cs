using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.LowLevel
{
	[Serializable]
	internal abstract class OpenTypeLayoutLookup
	{
		public abstract void InitializeLookupDictionary();

		public virtual void UpdateRecords(int lookupIndex, uint glyphIndex)
		{
		}

		public virtual void UpdateRecords(int lookupIndex, uint glyphIndex, float emScale)
		{
		}

		public virtual void UpdateRecords(int lookupIndex, List<uint> glyphIndexes)
		{
		}

		public virtual void UpdateRecords(int lookupIndex, List<uint> glyphIndexes, float emScale)
		{
		}

		public abstract void ClearRecords();

		public uint lookupType;

		public uint lookupFlag;

		public uint markFilteringSet;
	}
}
