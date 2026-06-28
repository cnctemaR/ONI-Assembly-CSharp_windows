using System;
using System.IO;
using System.Text;
using Ionic.Zip;

namespace Ionic
{
	internal class TimeCriterion : SelectionCriterion
	{
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Which.ToString()).Append(" ").Append(EnumUtil.GetDescription(this.Operator))
				.Append(" ")
				.Append(this.Time.ToString("yyyy-MM-dd-HH:mm:ss"));
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(string filename)
		{
			DateTime dateTime;
			switch (this.Which)
			{
			case WhichTime.atime:
				dateTime = File.GetLastAccessTime(filename).ToUniversalTime();
				break;
			case WhichTime.mtime:
				dateTime = File.GetLastWriteTime(filename).ToUniversalTime();
				break;
			case WhichTime.ctime:
				dateTime = File.GetCreationTime(filename).ToUniversalTime();
				break;
			default:
				throw new ArgumentException("Operator");
			}
			return this._Evaluate(dateTime);
		}

		private bool _Evaluate(DateTime x)
		{
			bool flag;
			switch (this.Operator)
			{
			case ComparisonOperator.GreaterThan:
				flag = x > this.Time;
				break;
			case ComparisonOperator.GreaterThanOrEqualTo:
				flag = x >= this.Time;
				break;
			case ComparisonOperator.LesserThan:
				flag = x < this.Time;
				break;
			case ComparisonOperator.LesserThanOrEqualTo:
				flag = x <= this.Time;
				break;
			case ComparisonOperator.EqualTo:
				flag = x == this.Time;
				break;
			case ComparisonOperator.NotEqualTo:
				flag = x != this.Time;
				break;
			default:
				throw new ArgumentException("Operator");
			}
			return flag;
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			DateTime dateTime;
			switch (this.Which)
			{
			case WhichTime.atime:
				dateTime = entry.AccessedTime;
				break;
			case WhichTime.mtime:
				dateTime = entry.ModifiedTime;
				break;
			case WhichTime.ctime:
				dateTime = entry.CreationTime;
				break;
			default:
				throw new ArgumentException("??time");
			}
			return this._Evaluate(dateTime);
		}

		internal ComparisonOperator Operator;

		internal WhichTime Which;

		internal DateTime Time;
	}
}
