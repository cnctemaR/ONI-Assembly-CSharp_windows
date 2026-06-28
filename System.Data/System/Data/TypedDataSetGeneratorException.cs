using System;
using System.Collections;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class TypedDataSetGeneratorException : DataException
	{
		public TypedDataSetGeneratorException()
			: base(Locale.GetText("System error."))
		{
		}

		public TypedDataSetGeneratorException(ArrayList list)
			: base(Locale.GetText("System error."))
		{
			this.errorList = list;
		}

		protected TypedDataSetGeneratorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			int @int = info.GetInt32("KEY_ARRAYCOUNT");
			this.errorList = new ArrayList(@int);
			for (int i = 0; i < @int; i++)
			{
				this.errorList.Add(info.GetString("KEY_ARRAYVALUES" + i));
			}
		}

		public TypedDataSetGeneratorException(string error)
			: base(error)
		{
		}

		public TypedDataSetGeneratorException(string error, Exception inner)
			: base(error, inner)
		{
		}

		public ArrayList ErrorList
		{
			get
			{
				return this.errorList;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			int num = ((this.errorList == null) ? 0 : this.ErrorList.Count);
			info.AddValue("KEY_ARRAYCOUNT", num);
			for (int i = 0; i < num; i++)
			{
				info.AddValue("KEY_ARRAYVALUES" + i, this.ErrorList[i]);
			}
		}

		private readonly ArrayList errorList;
	}
}
