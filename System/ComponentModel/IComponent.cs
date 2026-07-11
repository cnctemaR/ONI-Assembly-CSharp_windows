using System;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[Designer("System.ComponentModel.Design.ComponentDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(global::System.ComponentModel.Design.IDesigner))]
	[global::System.ComponentModel.Design.Serialization.RootDesignerSerializer("System.ComponentModel.Design.Serialization.RootCodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true)]
	[Designer("System.Windows.Forms.Design.ComponentDocumentDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(global::System.ComponentModel.Design.IRootDesigner))]
	[TypeConverter(typeof(ComponentConverter))]
	[ComVisible(true)]
	public interface IComponent : IDisposable
	{
		event EventHandler Disposed;

		ISite Site { get; set; }
	}
}
