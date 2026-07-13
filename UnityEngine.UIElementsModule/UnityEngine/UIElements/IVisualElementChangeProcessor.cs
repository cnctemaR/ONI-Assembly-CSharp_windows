using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal interface IVisualElementChangeProcessor
	{
		void BeginProcessing(BaseVisualElementPanel panel);

		void ProcessChanges(BaseVisualElementPanel panel, AuthoringChanges changes);

		void EndProcessing(BaseVisualElementPanel panel);
	}
}
