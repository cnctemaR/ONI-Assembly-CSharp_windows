using System;
using System.Collections;

namespace System.Xml.Serialization
{
	internal class MemberMappingComparer : IComparer
	{
		public int Compare(object o1, object o2)
		{
			MemberMapping memberMapping = (MemberMapping)o1;
			MemberMapping memberMapping2 = (MemberMapping)o2;
			if (memberMapping.IsText)
			{
				if (memberMapping2.IsText)
				{
					return 0;
				}
				return 1;
			}
			else
			{
				if (memberMapping2.IsText)
				{
					return -1;
				}
				if (memberMapping.SequenceId < 0 && memberMapping2.SequenceId < 0)
				{
					return 0;
				}
				if (memberMapping.SequenceId < 0)
				{
					return 1;
				}
				if (memberMapping2.SequenceId < 0)
				{
					return -1;
				}
				if (memberMapping.SequenceId < memberMapping2.SequenceId)
				{
					return -1;
				}
				if (memberMapping.SequenceId > memberMapping2.SequenceId)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}
