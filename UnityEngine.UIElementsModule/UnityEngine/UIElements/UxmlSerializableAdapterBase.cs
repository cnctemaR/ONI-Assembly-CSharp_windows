using System;

namespace UnityEngine.UIElements
{
	internal abstract class UxmlSerializableAdapterBase
	{
		public abstract object dataBoxed { get; set; }

		public abstract object CloneInstanceBoxed(object value);
	}
}
