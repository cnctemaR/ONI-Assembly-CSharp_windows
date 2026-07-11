using System;
using System.Collections;

namespace System.Text.RegularExpressions.Syntax
{
	internal class CharacterClass : Expression
	{
		public CharacterClass(bool negate, bool ignore)
		{
			this.negate = negate;
			this.ignore = ignore;
			this.intervals = new IntervalCollection();
			int num = 144;
			this.pos_cats = new BitArray(num);
			this.neg_cats = new BitArray(num);
		}

		public CharacterClass(Category cat, bool negate)
			: this(false, false)
		{
			this.AddCategory(cat, negate);
		}

		public bool Negate
		{
			get
			{
				return this.negate;
			}
			set
			{
				this.negate = value;
			}
		}

		public bool IgnoreCase
		{
			get
			{
				return this.ignore;
			}
			set
			{
				this.ignore = value;
			}
		}

		public void AddCategory(Category cat, bool negate)
		{
			if (negate)
			{
				this.neg_cats[(int)cat] = true;
			}
			else
			{
				this.pos_cats[(int)cat] = true;
			}
		}

		public void AddCharacter(char c)
		{
			this.AddRange(c, c);
		}

		public void AddRange(char lo, char hi)
		{
			Interval interval = new Interval((int)lo, (int)hi);
			if (this.ignore)
			{
				if (CharacterClass.upper_case_characters.Intersects(interval))
				{
					Interval interval2;
					if (interval.low < CharacterClass.upper_case_characters.low)
					{
						interval2 = new Interval(CharacterClass.upper_case_characters.low + 32, interval.high + 32);
						interval.high = CharacterClass.upper_case_characters.low - 1;
					}
					else
					{
						interval2 = new Interval(interval.low + 32, CharacterClass.upper_case_characters.high + 32);
						interval.low = CharacterClass.upper_case_characters.high + 1;
					}
					this.intervals.Add(interval2);
				}
				else if (CharacterClass.upper_case_characters.Contains(interval))
				{
					interval.high += 32;
					interval.low += 32;
				}
			}
			this.intervals.Add(interval);
		}

		public override void Compile(ICompiler cmp, bool reverse)
		{
			IntervalCollection metaCollection = this.intervals.GetMetaCollection(new IntervalCollection.CostDelegate(CharacterClass.GetIntervalCost));
			int num = metaCollection.Count;
			for (int i = 0; i < this.pos_cats.Length; i++)
			{
				if (this.pos_cats[i] || this.neg_cats[i])
				{
					num++;
				}
			}
			if (num == 0)
			{
				return;
			}
			LinkRef linkRef = cmp.NewLink();
			if (num > 1)
			{
				cmp.EmitIn(linkRef);
			}
			foreach (object obj in metaCollection)
			{
				Interval interval = (Interval)obj;
				if (interval.IsDiscontiguous)
				{
					BitArray bitArray = new BitArray(interval.Size);
					foreach (object obj2 in this.intervals)
					{
						Interval interval2 = (Interval)obj2;
						if (interval.Contains(interval2))
						{
							for (int j = interval2.low; j <= interval2.high; j++)
							{
								bitArray[j - interval.low] = true;
							}
						}
					}
					cmp.EmitSet((char)interval.low, bitArray, this.negate, this.ignore, reverse);
				}
				else if (interval.IsSingleton)
				{
					cmp.EmitCharacter((char)interval.low, this.negate, this.ignore, reverse);
				}
				else
				{
					cmp.EmitRange((char)interval.low, (char)interval.high, this.negate, this.ignore, reverse);
				}
			}
			for (int k = 0; k < this.pos_cats.Length; k++)
			{
				if (this.pos_cats[k])
				{
					if (this.neg_cats[k])
					{
						cmp.EmitCategory(Category.AnySingleline, this.negate, reverse);
					}
					else
					{
						cmp.EmitCategory((Category)k, this.negate, reverse);
					}
				}
				else if (this.neg_cats[k])
				{
					cmp.EmitNotCategory((Category)k, this.negate, reverse);
				}
			}
			if (num > 1)
			{
				if (this.negate)
				{
					cmp.EmitTrue();
				}
				else
				{
					cmp.EmitFalse();
				}
				cmp.ResolveLink(linkRef);
			}
		}

		public override void GetWidth(out int min, out int max)
		{
			min = (max = 1);
		}

		public override bool IsComplex()
		{
			return false;
		}

		private static double GetIntervalCost(Interval i)
		{
			if (i.IsDiscontiguous)
			{
				return (double)(3 + (i.Size + 15 >> 4));
			}
			if (i.IsSingleton)
			{
				return 2.0;
			}
			return 3.0;
		}

		private const int distance_between_upper_and_lower_case = 32;

		private static Interval upper_case_characters = new Interval(65, 90);

		private bool negate;

		private bool ignore;

		private BitArray pos_cats;

		private BitArray neg_cats;

		private IntervalCollection intervals;
	}
}
