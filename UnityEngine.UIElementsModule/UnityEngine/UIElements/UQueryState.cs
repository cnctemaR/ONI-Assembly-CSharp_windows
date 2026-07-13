using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public struct UQueryState<T> : IEnumerable<T>, IEnumerable, IEquatable<UQueryState<T>> where T : VisualElement
	{
		internal UQueryState(VisualElement element, List<RuleMatcher> matchers)
		{
			this.m_Element = element;
			this.m_Matchers = matchers;
		}

		public UQueryState<T> RebuildOn(VisualElement element)
		{
			return new UQueryState<T>(element, this.m_Matchers);
		}

		private T Single(UQuery.SingleQueryMatcher matcher)
		{
			bool flag = matcher.IsInUse();
			if (flag)
			{
				matcher = matcher.CreateNew();
			}
			matcher.Run(this.m_Element, this.m_Matchers);
			T t = matcher.match as T;
			matcher.match = null;
			return t;
		}

		public T First()
		{
			return this.Single(UQuery.FirstQueryMatcher.Instance);
		}

		public T Last()
		{
			return this.Single(UQuery.LastQueryMatcher.Instance);
		}

		public void ToList(List<T> results)
		{
			UQueryState<T>.s_List.matches = results;
			UQueryState<T>.s_List.Run(this.m_Element, this.m_Matchers);
			UQueryState<T>.s_List.Reset();
		}

		public List<T> ToList()
		{
			List<T> list = new List<T>();
			this.ToList(list);
			return list;
		}

		public T AtIndex(int index)
		{
			UQuery.IndexQueryMatcher instance = UQuery.IndexQueryMatcher.Instance;
			instance.matchIndex = index;
			return this.Single(instance);
		}

		public void ForEach(Action<T> funcCall)
		{
			UQueryState<T>.ActionQueryMatcher actionQueryMatcher = UQueryState<T>.s_Action;
			bool flag = actionQueryMatcher.callBack != null;
			if (flag)
			{
				actionQueryMatcher = new UQueryState<T>.ActionQueryMatcher();
			}
			try
			{
				actionQueryMatcher.callBack = funcCall;
				actionQueryMatcher.Run(this.m_Element, this.m_Matchers);
			}
			finally
			{
				actionQueryMatcher.callBack = null;
			}
		}

		public void ForEach<T2>(List<T2> result, Func<T, T2> funcCall)
		{
			UQueryState<T>.DelegateQueryMatcher<T2> delegateQueryMatcher = UQueryState<T>.DelegateQueryMatcher<T2>.s_Instance;
			bool flag = delegateQueryMatcher.callBack != null;
			if (flag)
			{
				delegateQueryMatcher = new UQueryState<T>.DelegateQueryMatcher<T2>();
			}
			try
			{
				delegateQueryMatcher.callBack = funcCall;
				delegateQueryMatcher.result = result;
				delegateQueryMatcher.Run(this.m_Element, this.m_Matchers);
			}
			finally
			{
				delegateQueryMatcher.callBack = null;
				delegateQueryMatcher.result = null;
			}
		}

		public List<T2> ForEach<T2>(Func<T, T2> funcCall)
		{
			List<T2> list = new List<T2>();
			this.ForEach<T2>(list, funcCall);
			return list;
		}

		public UQueryState<T>.Enumerator GetEnumerator()
		{
			return new UQueryState<T>.Enumerator(this);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool Equals(UQueryState<T> other)
		{
			return this.m_Element == other.m_Element && EqualityComparer<List<RuleMatcher>>.Default.Equals(this.m_Matchers, other.m_Matchers);
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is UQueryState<T>);
			return !flag && this.Equals((UQueryState<T>)obj);
		}

		public override int GetHashCode()
		{
			int num = 488160421;
			num = num * -1521134295 + EqualityComparer<VisualElement>.Default.GetHashCode(this.m_Element);
			return num * -1521134295 + EqualityComparer<List<RuleMatcher>>.Default.GetHashCode(this.m_Matchers);
		}

		public static bool operator ==(UQueryState<T> state1, UQueryState<T> state2)
		{
			return state1.Equals(state2);
		}

		public static bool operator !=(UQueryState<T> state1, UQueryState<T> state2)
		{
			return !(state1 == state2);
		}

		private static UQueryState<T>.ActionQueryMatcher s_Action = new UQueryState<T>.ActionQueryMatcher();

		private readonly VisualElement m_Element;

		internal readonly List<RuleMatcher> m_Matchers;

		private static readonly UQueryState<T>.ListQueryMatcher<T> s_List = new UQueryState<T>.ListQueryMatcher<T>();

		private static readonly UQueryState<T>.ListQueryMatcher<VisualElement> s_EnumerationList = new UQueryState<T>.ListQueryMatcher<VisualElement>();

		private class ListQueryMatcher<TElement> : UQuery.UQueryMatcher where TElement : VisualElement
		{
			public List<TElement> matches { get; set; }

			protected override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
			{
				this.matches.Add(element as TElement);
				return false;
			}

			public void Reset()
			{
				this.matches = null;
			}
		}

		private class ActionQueryMatcher : UQuery.UQueryMatcher
		{
			internal Action<T> callBack { get; set; }

			protected override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
			{
				T t = element as T;
				bool flag = t != null;
				if (flag)
				{
					this.callBack(t);
				}
				return false;
			}
		}

		private class DelegateQueryMatcher<TReturnType> : UQuery.UQueryMatcher
		{
			public Func<T, TReturnType> callBack { get; set; }

			public List<TReturnType> result { get; set; }

			protected override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
			{
				T t = element as T;
				bool flag = t != null;
				if (flag)
				{
					this.result.Add(this.callBack(t));
				}
				return false;
			}

			public static UQueryState<T>.DelegateQueryMatcher<TReturnType> s_Instance = new UQueryState<T>.DelegateQueryMatcher<TReturnType>();
		}

		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			internal Enumerator(UQueryState<T> queryState)
			{
				this.iterationList = VisualElementListPool.Get(0);
				UQueryState<T>.s_EnumerationList.matches = this.iterationList;
				UQueryState<T>.s_EnumerationList.Run(queryState.m_Element, queryState.m_Matchers);
				UQueryState<T>.s_EnumerationList.Reset();
				this.currentIndex = -1;
			}

			public T Current
			{
				get
				{
					return (T)((object)this.iterationList[this.currentIndex]);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				int num = this.currentIndex + 1;
				this.currentIndex = num;
				return num < this.iterationList.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}

			public void Dispose()
			{
				VisualElementListPool.Release(this.iterationList);
				this.iterationList = null;
			}

			private List<VisualElement> iterationList;

			private int currentIndex;
		}
	}
}
