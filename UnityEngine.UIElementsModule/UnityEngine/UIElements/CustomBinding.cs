using System;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[UxmlObject]
	public abstract class CustomBinding : Binding
	{
		protected CustomBinding()
		{
			base.updateTrigger = BindingUpdateTrigger.EveryUpdate;
		}

		protected internal virtual BindingResult Update(in BindingContext context)
		{
			return new BindingResult(BindingStatus.Success, null);
		}

		[ExcludeFromDocs]
		[Serializable]
		public new abstract class UxmlSerializedData : Binding.UxmlSerializedData
		{
		}
	}
}
