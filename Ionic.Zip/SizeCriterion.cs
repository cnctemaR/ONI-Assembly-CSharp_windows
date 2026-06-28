using System;
using System.IO;
using System.Text;
using Ionic.Zip;

namespace Ionic
{
	internal class SizeCriterion : SelectionCriterion
	{
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("size ").Append(EnumUtil.GetDescription(this.Operator)).Append(" ")
				.Append(this.Size.ToString());
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(string filename)
		{
			FileInfo fileInfo = new FileInfo(filename);
			return this._Evaluate(fileInfo.Length);
		}

		private bool _Evaluate(long Length)
		{
			bool flag;
			switch (this.Operator)
			{
			case ComparisonOperator.GreaterThan:
				flag = Length > this.Size;
				break;
			case ComparisonOperator.GreaterThanOrEqualTo:
				flag = Length >= this.Size;
				break;
			case ComparisonOperator.LesserThan:
				flag = Length < this.Size;
				break;
			case ComparisonOperator.LesserThanOrEqualTo:
				flag = Length <= this.Size;
				break;
			case ComparisonOperator.EqualTo:
				flag = Length == this.Size;
				break;
			case ComparisonOperator.NotEqualTo:
				flag = Length != this.Size;
				break;
			default:
				throw new ArgumentException("Operator");
			}
			return flag;
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			return this._Evaluate(entry.UncompressedSize);
		}

		internal ComparisonOperator Operator;

		internal long Size;
	}
}
