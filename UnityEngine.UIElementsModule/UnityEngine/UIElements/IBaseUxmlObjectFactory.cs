using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[Obsolete("IBaseUxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal interface IBaseUxmlObjectFactory : IBaseUxmlFactory
	{
	}
}
