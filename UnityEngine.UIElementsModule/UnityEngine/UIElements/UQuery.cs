using System;
using System.Collections.Generic;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	public static class UQuery
	{
		internal interface IVisualPredicateWrapper
		{
			bool Predicate(object e);
		}

		internal class IsOfType<T> : UQuery.IVisualPredicateWrapper where T : VisualElement
		{
			public bool Predicate(object e)
			{
				return e is T;
			}

			public static UQuery.IsOfType<T> s_Instance = new UQuery.IsOfType<T>();
		}

		internal class PredicateWrapper<T> : UQuery.IVisualPredicateWrapper where T : VisualElement
		{
			public PredicateWrapper(Func<T, bool> p)
			{
				this.predicate = p;
			}

			public bool Predicate(object e)
			{
				T t = e as T;
				bool flag = t != null;
				return flag && this.predicate(t);
			}

			private Func<T, bool> predicate;
		}

		internal abstract class UQueryMatcher : HierarchyTraversal
		{
			public override void Traverse(VisualElement element)
			{
				base.Traverse(element);
			}

			protected virtual bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
			{
				return false;
			}

			private static void NoProcessResult(VisualElement e, MatchResultInfo i)
			{
			}

			public override void TraverseRecursive(VisualElement element, int depth)
			{
				int count = this.m_Matchers.Count;
				int count2 = this.m_Matchers.Count;
				for (int j = 0; j < count2; j++)
				{
					RuleMatcher ruleMatcher = this.m_Matchers[j];
					bool flag = StyleSelectorHelper.MatchRightToLeft(element, ruleMatcher.complexSelector, delegate(VisualElement e, MatchResultInfo i)
					{
						UQuery.UQueryMatcher.NoProcessResult(e, i);
					});
					if (flag)
					{
						bool flag2 = this.OnRuleMatchedElement(ruleMatcher, element);
						if (flag2)
						{
							return;
						}
					}
				}
				base.Recurse(element, depth);
				bool flag3 = this.m_Matchers.Count > count;
				if (flag3)
				{
					this.m_Matchers.RemoveRange(count, this.m_Matchers.Count - count);
					return;
				}
			}

			public virtual void Run(VisualElement root, List<RuleMatcher> matchers)
			{
				this.m_Matchers = matchers;
				this.Traverse(root);
			}

			internal List<RuleMatcher> m_Matchers;
		}

		internal abstract class SingleQueryMatcher : UQuery.UQueryMatcher
		{
			public VisualElement match { get; set; }

			public override void Run(VisualElement root, List<RuleMatcher> matchers)
			{
				this.match = null;
				base.Run(root, matchers);
				this.m_Matchers = null;
			}

			public bool IsInUse()
			{
				return this.m_Matchers != null;
			}

			public abstract UQuery.SingleQueryMatcher CreateNew();
		}

		internal class FirstQueryMatcher : UQuery.SingleQueryMatcher
		{
			protected override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
			{
				bool flag = base.match == null;
				if (flag)
				{
					base.match = element;
				}
				return true;
			}

			public override UQuery.SingleQueryMatcher CreateNew()
			{
				return new UQuery.FirstQueryMatcher();
			}

			public static readonly UQuery.FirstQueryMatcher Instance = new UQuery.FirstQueryMatcher();
		}

		internal class LastQueryMatcher : UQuery.SingleQueryMatcher
		{
			protected override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
			{
				base.match = element;
				return false;
			}

			public override UQuery.SingleQueryMatcher CreateNew()
			{
				return new UQuery.LastQueryMatcher();
			}

			public static readonly UQuery.LastQueryMatcher Instance = new UQuery.LastQueryMatcher();
		}

		internal class IndexQueryMatcher : UQuery.SingleQueryMatcher
		{
			public int matchIndex
			{
				get
				{
					return this._matchIndex;
				}
				set
				{
					this.matchCount = -1;
					this._matchIndex = value;
				}
			}

			public override void Run(VisualElement root, List<RuleMatcher> matchers)
			{
				this.matchCount = -1;
				base.Run(root, matchers);
			}

			protected override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
			{
				this.matchCount++;
				bool flag = this.matchCount == this._matchIndex;
				if (flag)
				{
					base.match = element;
				}
				return this.matchCount >= this._matchIndex;
			}

			public override UQuery.SingleQueryMatcher CreateNew()
			{
				return new UQuery.IndexQueryMatcher();
			}

			public static readonly UQuery.IndexQueryMatcher Instance = new UQuery.IndexQueryMatcher();

			private int matchCount = -1;

			private int _matchIndex;
		}
	}
}
