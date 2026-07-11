using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Globalization;

namespace System.Data.ProviderBase
{
	internal class BasicFieldNameLookup
	{
		public BasicFieldNameLookup(string[] fieldNames)
		{
			if (fieldNames == null)
			{
				throw ADP.ArgumentNull("fieldNames");
			}
			this._fieldNames = fieldNames;
		}

		public BasicFieldNameLookup(ReadOnlyCollection<string> columnNames)
		{
			int count = columnNames.Count;
			string[] array = new string[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = columnNames[i];
			}
			this._fieldNames = array;
			this.GenerateLookup();
		}

		public BasicFieldNameLookup(IDataReader reader)
		{
			int fieldCount = reader.FieldCount;
			string[] array = new string[fieldCount];
			for (int i = 0; i < fieldCount; i++)
			{
				array[i] = reader.GetName(i);
			}
			this._fieldNames = array;
		}

		public int GetOrdinal(string fieldName)
		{
			if (fieldName == null)
			{
				throw ADP.ArgumentNull("fieldName");
			}
			int num = this.IndexOf(fieldName);
			if (-1 == num)
			{
				throw ADP.IndexOutOfRange(fieldName);
			}
			return num;
		}

		public int IndexOfName(string fieldName)
		{
			if (this._fieldNameLookup == null)
			{
				this.GenerateLookup();
			}
			int num;
			if (!this._fieldNameLookup.TryGetValue(fieldName, out num))
			{
				return -1;
			}
			return num;
		}

		public int IndexOf(string fieldName)
		{
			if (this._fieldNameLookup == null)
			{
				this.GenerateLookup();
			}
			int num;
			if (!this._fieldNameLookup.TryGetValue(fieldName, out num))
			{
				num = this.LinearIndexOf(fieldName, CompareOptions.IgnoreCase);
				if (-1 == num)
				{
					num = this.LinearIndexOf(fieldName, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
				}
			}
			return num;
		}

		protected virtual CompareInfo GetCompareInfo()
		{
			return CultureInfo.InvariantCulture.CompareInfo;
		}

		private int LinearIndexOf(string fieldName, CompareOptions compareOptions)
		{
			if (this._compareInfo == null)
			{
				this._compareInfo = this.GetCompareInfo();
			}
			int num = this._fieldNames.Length;
			for (int i = 0; i < num; i++)
			{
				if (this._compareInfo.Compare(fieldName, this._fieldNames[i], compareOptions) == 0)
				{
					this._fieldNameLookup[fieldName] = i;
					return i;
				}
			}
			return -1;
		}

		private void GenerateLookup()
		{
			int num = this._fieldNames.Length;
			Dictionary<string, int> dictionary = new Dictionary<string, int>(num);
			int num2 = num - 1;
			while (0 <= num2)
			{
				string text = this._fieldNames[num2];
				dictionary[text] = num2;
				num2--;
			}
			this._fieldNameLookup = dictionary;
		}

		private Dictionary<string, int> _fieldNameLookup;

		private readonly string[] _fieldNames;

		private CompareInfo _compareInfo;
	}
}
