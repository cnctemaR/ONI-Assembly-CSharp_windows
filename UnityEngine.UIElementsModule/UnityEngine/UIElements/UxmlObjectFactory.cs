using System;

namespace UnityEngine.UIElements
{
	[Obsolete("UxmlObjectFactory<TCreatedType, TTraits> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal class UxmlObjectFactory<TCreatedType, TTraits> : BaseUxmlFactory<TCreatedType, TTraits>, IUxmlObjectFactory<TCreatedType>, IBaseUxmlObjectFactory, IBaseUxmlFactory where TCreatedType : new() where TTraits : UxmlObjectTraits<TCreatedType>, new()
	{
		public virtual TCreatedType CreateObject(IUxmlAttributes bag, CreationContext cc)
		{
			TCreatedType tcreatedType = new TCreatedType();
			this.m_Traits.Init(ref tcreatedType, bag, cc);
			return tcreatedType;
		}
	}
}
