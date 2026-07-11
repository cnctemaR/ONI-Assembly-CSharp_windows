using System;
using System.Collections;

namespace System.Xml.Xsl.Qil
{
	internal sealed class SubstitutionList
	{
		public SubstitutionList()
		{
			this.s = new ArrayList(4);
		}

		public void AddSubstitutionPair(QilNode find, QilNode replace)
		{
			this.s.Add(find);
			this.s.Add(replace);
		}

		public void RemoveLastSubstitutionPair()
		{
			this.s.RemoveRange(this.s.Count - 2, 2);
		}

		public void RemoveLastNSubstitutionPairs(int n)
		{
			if (n > 0)
			{
				n *= 2;
				this.s.RemoveRange(this.s.Count - n, n);
			}
		}

		public QilNode FindReplacement(QilNode n)
		{
			for (int i = this.s.Count - 2; i >= 0; i -= 2)
			{
				if (this.s[i] == n)
				{
					return (QilNode)this.s[i + 1];
				}
			}
			return null;
		}

		private ArrayList s;
	}
}
