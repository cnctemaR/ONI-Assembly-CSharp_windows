using System;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchySearch.h")]
	[RequiredByNativeCode]
	[Serializable]
	public struct HierarchySearchFilter
	{
		public static readonly ref HierarchySearchFilter Invalid
		{
			get
			{
				return ref HierarchySearchFilter.s_Invalid;
			}
		}

		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.Name);
			}
		}

		public string Name { readonly get; set; }

		public string Value { readonly get; set; }

		public float NumValue { readonly get; set; }

		public HierarchySearchFilterOperator Op { readonly get; set; }

		public static string ToString(HierarchySearchFilterOperator op)
		{
			string text;
			switch (op)
			{
			case HierarchySearchFilterOperator.Equal:
				text = "=";
				break;
			case HierarchySearchFilterOperator.Contains:
				text = ":";
				break;
			case HierarchySearchFilterOperator.Greater:
				text = ">";
				break;
			case HierarchySearchFilterOperator.GreaterOrEqual:
				text = ">=";
				break;
			case HierarchySearchFilterOperator.Lesser:
				text = "<";
				break;
			case HierarchySearchFilterOperator.LesserOrEqual:
				text = "<=";
				break;
			case HierarchySearchFilterOperator.NotEqual:
				text = "!=";
				break;
			case HierarchySearchFilterOperator.Not:
				text = "-";
				break;
			default:
				throw new NotImplementedException(string.Format("Cannot convert {0} to string", op));
			}
			return text;
		}

		public static HierarchySearchFilterOperator ToOp(string op)
		{
			uint num = global::<PrivateImplementationDetails>.ComputeStringHash(op);
			if (num <= 957132539U)
			{
				if (num <= 671913016U)
				{
					if (num != 284975636U)
					{
						if (num == 671913016U)
						{
							if (op == "-")
							{
								return HierarchySearchFilterOperator.Not;
							}
						}
					}
					else if (op == ">=")
					{
						return HierarchySearchFilterOperator.GreaterOrEqual;
					}
				}
				else if (num != 940354920U)
				{
					if (num == 957132539U)
					{
						if (op == "<")
						{
							return HierarchySearchFilterOperator.Lesser;
						}
					}
				}
				else if (op == "=")
				{
					return HierarchySearchFilterOperator.Equal;
				}
			}
			else if (num <= 1057798253U)
			{
				if (num != 990687777U)
				{
					if (num == 1057798253U)
					{
						if (op == ":")
						{
							return HierarchySearchFilterOperator.Contains;
						}
					}
				}
				else if (op == ">")
				{
					return HierarchySearchFilterOperator.Greater;
				}
			}
			else if (num != 2428715011U)
			{
				if (num == 2499223986U)
				{
					if (op == "<=")
					{
						return HierarchySearchFilterOperator.LesserOrEqual;
					}
				}
			}
			else if (op == "!=")
			{
				return HierarchySearchFilterOperator.NotEqual;
			}
			throw new NotImplementedException("Cannot convert " + op + " to SearchFilterOperator");
		}

		public override string ToString()
		{
			string text = (float.IsNaN(this.NumValue) ? this.Value : this.NumValue.ToString());
			return this.Name + HierarchySearchFilter.ToString(this.Op) + HierarchySearchFilter.QuoteStringIfNeeded(text);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal static HierarchySearchFilter CreateFilter(string name, string op, string value)
		{
			return HierarchySearchFilter.CreateFilter(name, HierarchySearchFilter.ToOp(op), value);
		}

		internal static HierarchySearchFilter CreateFilter(string name, HierarchySearchFilterOperator op, string str)
		{
			string text = str;
			float num = float.NaN;
			try
			{
				num = Convert.ToSingle(str);
				text = null;
			}
			catch (Exception)
			{
			}
			return new HierarchySearchFilter
			{
				Name = name,
				Op = op,
				Value = text,
				NumValue = num
			};
		}

		internal static string QuoteStringIfNeeded(string s)
		{
			bool flag = s.Length > 0 && s.IndexOfAny(HierarchySearchFilter.s_WhiteSpaces) != -1 && s[0] != '"';
			string text;
			if (flag)
			{
				text = "\"" + s + "\"";
			}
			else
			{
				text = s;
			}
			return text;
		}

		private static readonly char[] s_WhiteSpaces = new char[] { ' ', '\t', '\n' };

		[NoAutoStaticsCleanup]
		private static readonly HierarchySearchFilter s_Invalid;
	}
}
