using System;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class Match : Group
	{
		private Match()
		{
			this.regex = null;
			this.machine = null;
			this.text_length = 0;
			this.groups = new GroupCollection(1, 1);
			this.groups.SetValue(this, 0);
		}

		internal Match(Regex regex, IMachine machine, string text, int text_length, int n_groups, int index, int length)
			: base(text, index, length)
		{
			this.regex = regex;
			this.machine = machine;
			this.text_length = text_length;
		}

		internal Match(Regex regex, IMachine machine, string text, int text_length, int n_groups, int index, int length, int n_caps)
			: base(text, index, length, n_caps)
		{
			this.regex = regex;
			this.machine = machine;
			this.text_length = text_length;
			this.groups = new GroupCollection(n_groups, regex.Gap);
			this.groups.SetValue(this, 0);
		}

		public static Match Empty
		{
			get
			{
				return Match.empty;
			}
		}

		[global::System.MonoTODO("not thread-safe")]
		public static Match Synchronized(Match inner)
		{
			if (inner == null)
			{
				throw new ArgumentNullException("inner");
			}
			return inner;
		}

		public virtual GroupCollection Groups
		{
			get
			{
				return this.groups;
			}
		}

		public Match NextMatch()
		{
			if (this == Match.Empty)
			{
				return Match.Empty;
			}
			int num = ((!this.regex.RightToLeft) ? (base.Index + base.Length) : base.Index);
			if (base.Length == 0)
			{
				num += ((!this.regex.RightToLeft) ? 1 : (-1));
			}
			return this.machine.Scan(this.regex, base.Text, num, this.text_length);
		}

		public virtual string Result(string replacement)
		{
			if (replacement == null)
			{
				throw new ArgumentNullException("replacement");
			}
			if (this.machine == null)
			{
				throw new NotSupportedException("Result cannot be called on failed Match.");
			}
			return this.machine.Result(replacement, this);
		}

		internal Regex Regex
		{
			get
			{
				return this.regex;
			}
		}

		private Regex regex;

		private IMachine machine;

		private int text_length;

		private GroupCollection groups;

		private static Match empty = new Match();
	}
}
