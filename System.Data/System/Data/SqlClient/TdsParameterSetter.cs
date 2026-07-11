using System;
using Microsoft.SqlServer.Server;

namespace System.Data.SqlClient
{
	internal class TdsParameterSetter : SmiTypedGetterSetter
	{
		internal TdsParameterSetter(TdsParserStateObject stateObj, SmiMetaData md)
		{
			this._target = new TdsRecordBufferSetter(stateObj, md);
		}

		internal override bool CanGet
		{
			get
			{
				return false;
			}
		}

		internal override bool CanSet
		{
			get
			{
				return true;
			}
		}

		internal override SmiTypedGetterSetter GetTypedGetterSetter(SmiEventSink sink, int ordinal)
		{
			return this._target;
		}

		public override void SetDBNull(SmiEventSink sink, int ordinal)
		{
			this._target.EndElements(sink);
		}

		private TdsRecordBufferSetter _target;
	}
}
