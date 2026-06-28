using System;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
	[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
	public class ValidationEventArgs : EventArgs
	{
		internal ValidationEventArgs(JsonSchemaException ex)
		{
			ValidationUtils.ArgumentNotNull(ex, "ex");
			this._ex = ex;
		}

		public JsonSchemaException Exception
		{
			get
			{
				return this._ex;
			}
		}

		public string Path
		{
			get
			{
				return this._ex.Path;
			}
		}

		public string Message
		{
			get
			{
				return this._ex.Message;
			}
		}

		private readonly JsonSchemaException _ex;
	}
}
