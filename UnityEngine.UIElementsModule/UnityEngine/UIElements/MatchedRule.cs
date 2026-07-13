using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct MatchedRule
	{
		public MatchedRule(SelectorMatchRecord matchRecord, string path)
		{
			this = default(MatchedRule);
			this.matchRecord = matchRecord;
			this.fullPath = path;
			this.lineNumber = matchRecord.complexSelector.rule.line;
			bool flag = string.IsNullOrEmpty(this.fullPath);
			if (flag)
			{
				this.displayPath = matchRecord.sheet.name + ":" + this.lineNumber.ToString();
			}
			else
			{
				bool flag2 = this.fullPath == "Library/unity editor resources";
				if (flag2)
				{
					this.displayPath = matchRecord.sheet.name + ":" + this.lineNumber.ToString();
				}
				else
				{
					this.displayPath = Path.GetFileName(this.fullPath) + ":" + this.lineNumber.ToString();
				}
			}
		}

		public override int GetHashCode()
		{
			int num = this.matchRecord.GetHashCode();
			num = (num * 397) ^ ((this.displayPath != null) ? this.displayPath.GetHashCode() : 0);
			num = (num * 397) ^ this.lineNumber;
			return (num * 397) ^ ((this.fullPath != null) ? this.fullPath.GetHashCode() : 0);
		}

		public readonly SelectorMatchRecord matchRecord;

		public readonly string displayPath;

		public readonly int lineNumber;

		public readonly string fullPath;

		public static IEqualityComparer<MatchedRule> lineNumberFullPathComparer = new MatchedRule.LineNumberFullPathEqualityComparer();

		private sealed class LineNumberFullPathEqualityComparer : IEqualityComparer<MatchedRule>
		{
			public bool Equals(MatchedRule x, MatchedRule y)
			{
				return x.lineNumber == y.lineNumber && string.Equals(x.fullPath, y.fullPath) && string.Equals(x.displayPath, y.displayPath);
			}

			public int GetHashCode(MatchedRule obj)
			{
				return obj.GetHashCode();
			}
		}
	}
}
