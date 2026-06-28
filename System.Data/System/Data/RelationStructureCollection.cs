using System;
using System.Collections;

namespace System.Data
{
	internal class RelationStructureCollection : CollectionBase
	{
		public void Add(RelationStructure rel)
		{
			base.List.Add(rel);
		}

		public RelationStructure this[int i]
		{
			get
			{
				return base.List[i] as RelationStructure;
			}
		}

		public RelationStructure this[string parent, string child]
		{
			get
			{
				foreach (object obj in base.List)
				{
					RelationStructure relationStructure = (RelationStructure)obj;
					if (relationStructure.ParentTableName == parent && relationStructure.ChildTableName == child)
					{
						return relationStructure;
					}
				}
				return null;
			}
		}
	}
}
