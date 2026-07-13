using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal readonly struct StylePropertyValidationCollection : IEnumerable<StylePropertyValidation>, IEnumerable
	{
		public static implicit operator StylePropertyValidationCollection(List<StylePropertyValidation> validation)
		{
			return new StylePropertyValidationCollection(validation);
		}

		public static StylePropertyValidationCollection Empty { get; } = default(StylePropertyValidationCollection);

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StylePropertyValidationCollection(List<StylePropertyValidation> persistentValidation, List<StylePropertyValidation> validation)
		{
			this.m_PersistentValidation = persistentValidation;
			this.m_Validation = validation;
		}

		internal StylePropertyValidationCollection(List<StylePropertyValidation> validation)
		{
			this.m_PersistentValidation = null;
			this.m_Validation = validation;
		}

		public StylePropertyValidationCollection.Enumerator GetEnumerator()
		{
			return (this.m_PersistentValidation != null) ? new StylePropertyValidationCollection.Enumerator(this.m_PersistentValidation.GetEnumerator(), this.m_Validation.GetEnumerator()) : new StylePropertyValidationCollection.Enumerator(StylePropertyValidationCollection.s_Empty.GetEnumerator(), this.m_Validation.GetEnumerator());
		}

		IEnumerator<StylePropertyValidation> IEnumerable<StylePropertyValidation>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private static readonly List<StylePropertyValidation> s_Empty = new List<StylePropertyValidation>();

		private readonly List<StylePropertyValidation> m_PersistentValidation;

		private readonly List<StylePropertyValidation> m_Validation;

		internal struct Enumerator : IEnumerator<StylePropertyValidation>, IEnumerator, IDisposable
		{
			public StylePropertyValidation Current { readonly get; private set; }

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal Enumerator(List<StylePropertyValidation>.Enumerator persistentValidation, List<StylePropertyValidation>.Enumerator validation)
			{
				this.m_PersistentValidation = persistentValidation;
				this.m_Validation = validation;
				this.Current = null;
				this.persistent = true;
			}

			public bool MoveNext()
			{
				bool flag = this.persistent;
				bool flag2;
				if (flag)
				{
					flag2 = this.m_PersistentValidation.MoveNext();
					this.Current = this.m_PersistentValidation.Current;
					bool flag3 = !flag2;
					if (flag3)
					{
						this.persistent = false;
						flag2 = this.m_Validation.MoveNext();
						this.Current = this.m_Validation.Current;
					}
				}
				else
				{
					flag2 = this.m_Validation.MoveNext();
					this.Current = this.m_Validation.Current;
				}
				return flag2;
			}

			public void Reset()
			{
				this.persistent = true;
				this.Current = null;
				((IEnumerator)this.m_PersistentValidation).Reset();
				((IEnumerator)this.m_Validation).Reset();
			}

			public void Dispose()
			{
			}

			private List<StylePropertyValidation>.Enumerator m_PersistentValidation;

			private List<StylePropertyValidation>.Enumerator m_Validation;

			private bool persistent;
		}
	}
}
