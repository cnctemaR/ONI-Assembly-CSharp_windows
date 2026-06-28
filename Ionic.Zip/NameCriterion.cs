using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Ionic.Zip;

namespace Ionic
{
	internal class NameCriterion : SelectionCriterion
	{
		internal virtual string MatchingFileSpec
		{
			set
			{
				if (Directory.Exists(value))
				{
					this._MatchingFileSpec = ".\\" + value + "\\*.*";
				}
				else
				{
					this._MatchingFileSpec = value;
				}
				this._regexString = "^" + Regex.Escape(this._MatchingFileSpec).Replace("\\\\\\*\\.\\*", "\\\\([^\\.]+|.*\\.[^\\\\\\.]*)").Replace("\\.\\*", "\\.[^\\\\\\.]*")
					.Replace("\\*", ".*")
					.Replace("\\?", "[^\\\\\\.]") + "$";
				this._re = new Regex(this._regexString, RegexOptions.IgnoreCase);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("name ").Append(EnumUtil.GetDescription(this.Operator)).Append(" '")
				.Append(this._MatchingFileSpec)
				.Append("'");
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(string filename)
		{
			return this._Evaluate(filename);
		}

		private bool _Evaluate(string fullpath)
		{
			string text = ((this._MatchingFileSpec.IndexOf('\\') == -1) ? Path.GetFileName(fullpath) : fullpath);
			bool flag = this._re.IsMatch(text);
			if (this.Operator != ComparisonOperator.EqualTo)
			{
				flag = !flag;
			}
			return flag;
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			string text = entry.FileName.Replace("/", "\\");
			return this._Evaluate(text);
		}

		private Regex _re;

		private string _regexString;

		internal ComparisonOperator Operator;

		private string _MatchingFileSpec;
	}
}
