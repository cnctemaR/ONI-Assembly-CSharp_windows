using System;
using System.Reflection;

namespace FileHelpers.DataLink
{
	public sealed class GenericDataLink
	{
		public GenericDataLink(DataStorage provider1, DataStorage provider2)
		{
			if (provider1 == null)
			{
				throw new ArgumentException("provider1 can't be null", "provider1");
			}
			this.mDataStorage1 = provider1;
			if (provider2 == null)
			{
				throw new ArgumentException("provider2 can't be null", "provider2");
			}
			this.mDataStorage2 = provider2;
			this.ValidateRecordTypes();
		}

		public object[] CopyDataFrom1To2()
		{
			object[] array = this.DataStorage1.ExtractRecords();
			this.DataStorage2.InsertRecords(array);
			return array;
		}

		public object[] CopyDataFrom2To1()
		{
			object[] array = this.DataStorage2.ExtractRecords();
			this.DataStorage1.InsertRecords(array);
			return array;
		}

		public DataStorage DataStorage1
		{
			get
			{
				return this.mDataStorage1;
			}
		}

		public DataStorage DataStorage2
		{
			get
			{
				return this.mDataStorage2;
			}
		}

		private void ValidateRecordTypes()
		{
			if (this.DataStorage1.RecordType == null)
			{
				throw new BadUsageException("DataLink1 can't have a null RecordType.");
			}
			if (this.DataStorage2.RecordType == null)
			{
				throw new BadUsageException("DataLink2 can't have a null RecordType.");
			}
			if (this.DataStorage1.RecordType != this.DataStorage2.RecordType)
			{
				this.mConvert1to2 = this.GetTransformMethod(this.DataStorage1.RecordType, this.DataStorage2.RecordType);
				if (this.mConvert1to2 == null)
				{
					throw new BadUsageException(string.Concat(new string[]
					{
						"You must to define a method in the class ",
						this.DataStorage1.RecordType.Name,
						" with the attribute [TransfortToRecord(typeof(",
						this.DataStorage2.RecordType.Name,
						"))]"
					}));
				}
				this.mConvert2to1 = this.GetTransformMethod(this.DataStorage2.RecordType, this.DataStorage1.RecordType);
				if (this.mConvert2to1 == null)
				{
					throw new BadUsageException(string.Concat(new string[]
					{
						"You must to define a method in the class ",
						this.DataStorage2.RecordType.Name,
						" with the attribute [TransfortToRecord(typeof(",
						this.DataStorage1.RecordType.Name,
						"))]"
					}));
				}
			}
		}

		private MethodInfo GetTransformMethod(Type sourceType, Type destType)
		{
			sourceType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			return null;
		}

		private DataStorage mDataStorage1;

		private DataStorage mDataStorage2;

		private MethodInfo mConvert1to2;

		private MethodInfo mConvert2to1;
	}
}
