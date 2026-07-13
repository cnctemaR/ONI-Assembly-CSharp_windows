using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class StyleVariableContext
	{
		public List<StyleVariable> variables
		{
			get
			{
				return this.m_Variables;
			}
		}

		public void Add(StyleVariable sv)
		{
			StyleVariableContext.<>c__DisplayClass7_0 CS$<>8__locals1;
			CS$<>8__locals1.hash = sv.GetHashCode();
			int num = this.m_SortedHash.BinarySearch(CS$<>8__locals1.hash);
			bool flag = num >= 0;
			if (flag)
			{
				int i = this.m_Variables.Count - 1;
				bool flag2 = this.m_UnsortedHash[i] == CS$<>8__locals1.hash;
				if (flag2)
				{
					return;
				}
				for (i--; i >= 0; i--)
				{
					bool flag3 = this.m_UnsortedHash[i] == CS$<>8__locals1.hash;
					if (flag3)
					{
						this.m_VariableHash ^= StyleVariableContext.<Add>g__ComputeOrderSensitiveHash|7_0(i, ref CS$<>8__locals1);
						this.m_Variables.RemoveAt(i);
						this.m_UnsortedHash.RemoveAt(i);
						break;
					}
				}
			}
			else
			{
				this.m_SortedHash.Insert(~num, CS$<>8__locals1.hash);
			}
			this.m_VariableHash ^= StyleVariableContext.<Add>g__ComputeOrderSensitiveHash|7_0(this.m_Variables.Count, ref CS$<>8__locals1);
			this.m_Variables.Add(sv);
			this.m_UnsortedHash.Add(CS$<>8__locals1.hash);
		}

		public void AddInitialRange(StyleVariableContext other)
		{
			bool flag = other.m_Variables.Count > 0;
			if (flag)
			{
				Debug.Assert(this.m_Variables.Count == 0);
				this.m_VariableHash = other.m_VariableHash;
				this.m_Variables.AddRange(other.m_Variables);
				this.m_SortedHash.AddRange(other.m_SortedHash);
				this.m_UnsortedHash.AddRange(other.m_UnsortedHash);
			}
		}

		public void Clear()
		{
			bool flag = this.m_Variables.Count > 0;
			if (flag)
			{
				this.m_Variables.Clear();
				this.m_VariableHash = 0;
				this.m_SortedHash.Clear();
				this.m_UnsortedHash.Clear();
			}
		}

		public StyleVariableContext()
		{
			this.m_Variables = new List<StyleVariable>();
			this.m_VariableHash = 0;
			this.m_SortedHash = new List<int>();
			this.m_UnsortedHash = new List<int>();
		}

		public StyleVariableContext(StyleVariableContext other)
		{
			this.m_Variables = new List<StyleVariable>(other.m_Variables);
			this.m_VariableHash = other.m_VariableHash;
			this.m_SortedHash = new List<int>(other.m_SortedHash);
			this.m_UnsortedHash = new List<int>(other.m_UnsortedHash);
		}

		public bool TryFindVariable(string name, out StyleVariable v)
		{
			for (int i = this.m_Variables.Count - 1; i >= 0; i--)
			{
				bool flag = this.m_Variables[i].name == name;
				if (flag)
				{
					v = this.m_Variables[i];
					return true;
				}
			}
			v = default(StyleVariable);
			return false;
		}

		public int GetVariableHash()
		{
			return this.m_VariableHash;
		}

		[CompilerGenerated]
		internal static int <Add>g__ComputeOrderSensitiveHash|7_0(int index, ref StyleVariableContext.<>c__DisplayClass7_0 A_1)
		{
			return (index + 1) * A_1.hash;
		}

		public static readonly StyleVariableContext none = new StyleVariableContext();

		private int m_VariableHash;

		private List<StyleVariable> m_Variables;

		private List<int> m_SortedHash;

		private List<int> m_UnsortedHash;
	}
}
