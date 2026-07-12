using System;
using System.Collections;

namespace System.Xml.Xsl.Qil
{
	internal sealed class SubstitutionList
	{
		public SubstitutionList()
		{
			this._s = new ArrayList(4);
		}

		public void AddSubstitutionPair(QilNode find, QilNode replace)
		{
			this._s.Add(find);
			this._s.Add(replace);
		}

		public void RemoveLastSubstitutionPair()
		{
			this._s.RemoveRange(this._s.Count - 2, 2);
		}

		public QilNode FindReplacement(QilNode n)
		{
			for (int i = this._s.Count - 2; i >= 0; i -= 2)
			{
				if (this._s[i] == n)
				{
					return (QilNode)this._s[i + 1];
				}
			}
			return null;
		}

		private ArrayList _s;
	}
}
