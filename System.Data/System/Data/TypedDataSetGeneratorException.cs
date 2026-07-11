using System;
using System.Collections;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class TypedDataSetGeneratorException : DataException
	{
		public TypedDataSetGeneratorException()
		{
		}

		public TypedDataSetGeneratorException(ArrayList list)
		{
		}

		protected TypedDataSetGeneratorException(SerializationInfo info, StreamingContext context)
		{
		}

		public TypedDataSetGeneratorException(string message)
		{
		}

		public TypedDataSetGeneratorException(string message, Exception innerException)
		{
		}

		public ArrayList ErrorList
		{
			get
			{
				throw null;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}
	}
}
