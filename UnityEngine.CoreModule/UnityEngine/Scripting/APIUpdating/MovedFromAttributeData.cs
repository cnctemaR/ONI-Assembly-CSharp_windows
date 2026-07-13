using System;
using UnityEngine.Bindings;

namespace UnityEngine.Scripting.APIUpdating
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct MovedFromAttributeData
	{
		public void Set(bool autoUpdateAPI, string sourceNamespace = null, string sourceAssembly = null, string sourceClassName = null)
		{
			this.className = sourceClassName;
			this.classHasChanged = this.className != null;
			this.nameSpace = sourceNamespace;
			this.nameSpaceHasChanged = this.nameSpace != null;
			this.assembly = sourceAssembly;
			this.assemblyHasChanged = this.assembly != null;
			this.autoUdpateAPI = autoUpdateAPI;
		}

		public string className;

		public string nameSpace;

		public string assembly;

		public bool classHasChanged;

		public bool nameSpaceHasChanged;

		public bool assemblyHasChanged;

		public bool autoUdpateAPI;
	}
}
