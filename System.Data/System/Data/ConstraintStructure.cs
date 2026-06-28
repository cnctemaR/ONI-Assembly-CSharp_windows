using System;

namespace System.Data
{
	internal class ConstraintStructure
	{
		public ConstraintStructure(string tname, string[] cols, bool[] isAttr, string cname, bool isPK, string refName, bool isNested, bool isConstraintOnly)
		{
			this.TableName = tname;
			this.Columns = cols;
			this.IsAttribute = isAttr;
			this.ConstraintName = XmlHelper.Decode(cname);
			this.IsPrimaryKey = isPK;
			this.ReferName = refName;
			this.IsNested = isNested;
			this.IsConstraintOnly = isConstraintOnly;
		}

		public readonly string TableName;

		public readonly string[] Columns;

		public readonly bool[] IsAttribute;

		public readonly string ConstraintName;

		public readonly bool IsPrimaryKey;

		public readonly string ReferName;

		public readonly bool IsNested;

		public readonly bool IsConstraintOnly;
	}
}
