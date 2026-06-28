using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Drawing.Design
{
	[MonoTODO("Implementation is incomplete.")]
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[Serializable]
	public class ToolboxItem : ISerializable
	{
		public ToolboxItem()
		{
		}

		public ToolboxItem(Type toolType)
		{
			this.Initialize(toolType);
		}

		public event ToolboxComponentsCreatedEventHandler ComponentsCreated;

		public event ToolboxComponentsCreatingEventHandler ComponentsCreating;

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.Serialize(info, context);
		}

		public AssemblyName AssemblyName
		{
			get
			{
				return (AssemblyName)this.properties["AssemblyName"];
			}
			set
			{
				this.SetValue("AssemblyName", value);
			}
		}

		public Bitmap Bitmap
		{
			get
			{
				return (Bitmap)this.properties["Bitmap"];
			}
			set
			{
				this.SetValue("Bitmap", value);
			}
		}

		public string DisplayName
		{
			get
			{
				return this.GetValue("DisplayName");
			}
			set
			{
				this.SetValue("DisplayName", value);
			}
		}

		public ICollection Filter
		{
			get
			{
				ICollection collection = (ICollection)this.properties["Filter"];
				if (collection == null)
				{
					collection = new ToolboxItemFilterAttribute[0];
				}
				return collection;
			}
			set
			{
				this.SetValue("Filter", value);
			}
		}

		public virtual bool Locked
		{
			get
			{
				return this.locked;
			}
		}

		public string TypeName
		{
			get
			{
				return this.GetValue("TypeName");
			}
			set
			{
				this.SetValue("TypeName", value);
			}
		}

		public string Company
		{
			get
			{
				return (string)this.properties["Company"];
			}
			set
			{
				this.SetValue("Company", value);
			}
		}

		public virtual string ComponentType
		{
			get
			{
				return ".NET Component";
			}
		}

		public AssemblyName[] DependentAssemblies
		{
			get
			{
				return (AssemblyName[])this.properties["DependentAssemblies"];
			}
			set
			{
				AssemblyName[] array = new AssemblyName[value.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = value[i];
				}
				this.SetValue("DependentAssemblies", array);
			}
		}

		public string Description
		{
			get
			{
				return (string)this.properties["Description"];
			}
			set
			{
				this.SetValue("Description", value);
			}
		}

		public bool IsTransient
		{
			get
			{
				object obj = this.properties["IsTransient"];
				return obj != null && (bool)obj;
			}
			set
			{
				this.SetValue("IsTransient", value);
			}
		}

		public IDictionary Properties
		{
			get
			{
				return this.properties;
			}
		}

		public virtual string Version
		{
			get
			{
				return string.Empty;
			}
		}

		protected void CheckUnlocked()
		{
			if (this.locked)
			{
				throw new InvalidOperationException("The ToolboxItem is locked");
			}
		}

		public IComponent[] CreateComponents()
		{
			return this.CreateComponents(null);
		}

		public IComponent[] CreateComponents(IDesignerHost host)
		{
			this.OnComponentsCreating(new ToolboxComponentsCreatingEventArgs(host));
			IComponent[] array = this.CreateComponentsCore(host);
			this.OnComponentsCreated(new ToolboxComponentsCreatedEventArgs(array));
			return array;
		}

		protected virtual IComponent[] CreateComponentsCore(IDesignerHost host)
		{
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			Type type = this.GetType(host, this.AssemblyName, this.TypeName, true);
			IComponent[] array;
			if (type == null)
			{
				array = new IComponent[0];
			}
			else
			{
				array = new IComponent[] { host.CreateComponent(type) };
			}
			return array;
		}

		protected virtual IComponent[] CreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
		{
			IComponent[] array = this.CreateComponentsCore(host);
			foreach (Component component in array)
			{
				IComponentInitializer componentInitializer = host.GetDesigner(component) as IComponentInitializer;
				componentInitializer.InitializeNewComponent(defaultValues);
			}
			return array;
		}

		public IComponent[] CreateComponents(IDesignerHost host, IDictionary defaultValues)
		{
			this.OnComponentsCreating(new ToolboxComponentsCreatingEventArgs(host));
			IComponent[] array = this.CreateComponentsCore(host, defaultValues);
			this.OnComponentsCreated(new ToolboxComponentsCreatedEventArgs(array));
			return array;
		}

		protected virtual object FilterPropertyValue(string propertyName, object value)
		{
			switch (propertyName)
			{
			case "AssemblyName":
				return (value != null) ? (value as ICloneable).Clone() : null;
			case "DisplayName":
			case "TypeName":
				return (value != null) ? value : string.Empty;
			case "Filter":
				return (value != null) ? value : new ToolboxItemFilterAttribute[0];
			}
			return value;
		}

		protected virtual void Deserialize(SerializationInfo info, StreamingContext context)
		{
			this.AssemblyName = (AssemblyName)info.GetValue("AssemblyName", typeof(AssemblyName));
			this.Bitmap = (Bitmap)info.GetValue("Bitmap", typeof(Bitmap));
			this.Filter = (ICollection)info.GetValue("Filter", typeof(ICollection));
			this.DisplayName = info.GetString("DisplayName");
			this.locked = info.GetBoolean("Locked");
			this.TypeName = info.GetString("TypeName");
		}

		public override bool Equals(object obj)
		{
			ToolboxItem toolboxItem = obj as ToolboxItem;
			return toolboxItem != null && (obj == this || (toolboxItem.AssemblyName.Equals(this.AssemblyName) && toolboxItem.Locked.Equals(this.locked) && toolboxItem.TypeName.Equals(this.TypeName) && toolboxItem.DisplayName.Equals(this.DisplayName) && toolboxItem.Bitmap.Equals(this.Bitmap)));
		}

		public override int GetHashCode()
		{
			return (this.TypeName + this.DisplayName).GetHashCode();
		}

		public Type GetType(IDesignerHost host)
		{
			return this.GetType(host, this.AssemblyName, this.TypeName, false);
		}

		protected virtual Type GetType(IDesignerHost host, AssemblyName assemblyName, string typeName, bool reference)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			if (host == null)
			{
				return null;
			}
			ITypeResolutionService typeResolutionService = host.GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
			Type type = null;
			if (typeResolutionService != null)
			{
				typeResolutionService.GetAssembly(assemblyName, true);
				if (reference)
				{
					typeResolutionService.ReferenceAssembly(assemblyName);
				}
				type = typeResolutionService.GetType(typeName, true);
			}
			else
			{
				Assembly assembly = Assembly.Load(assemblyName);
				if (assembly != null)
				{
					type = assembly.GetType(typeName);
				}
			}
			return type;
		}

		public virtual void Initialize(Type type)
		{
			this.CheckUnlocked();
			if (type == null)
			{
				return;
			}
			this.AssemblyName = type.Assembly.GetName();
			this.DisplayName = type.Name;
			this.TypeName = type.FullName;
			Image image = null;
			foreach (object obj in type.GetCustomAttributes(true))
			{
				ToolboxBitmapAttribute toolboxBitmapAttribute = obj as ToolboxBitmapAttribute;
				if (toolboxBitmapAttribute != null)
				{
					image = toolboxBitmapAttribute.GetImage(type);
					break;
				}
			}
			if (image == null)
			{
				image = ToolboxBitmapAttribute.GetImageFromResource(type, null, false);
			}
			if (image != null)
			{
				this.Bitmap = image as Bitmap;
				if (this.Bitmap == null)
				{
					this.Bitmap = new Bitmap(image);
				}
			}
			this.Filter = type.GetCustomAttributes(typeof(ToolboxItemFilterAttribute), true);
		}

		public virtual void Lock()
		{
			this.locked = true;
		}

		protected virtual void OnComponentsCreated(ToolboxComponentsCreatedEventArgs args)
		{
			if (this.ComponentsCreated != null)
			{
				this.ComponentsCreated(this, args);
			}
		}

		protected virtual void OnComponentsCreating(ToolboxComponentsCreatingEventArgs args)
		{
			if (this.ComponentsCreating != null)
			{
				this.ComponentsCreating(this, args);
			}
		}

		protected virtual void Serialize(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("AssemblyName", this.AssemblyName);
			info.AddValue("Bitmap", this.Bitmap);
			info.AddValue("Filter", this.Filter);
			info.AddValue("DisplayName", this.DisplayName);
			info.AddValue("Locked", this.locked);
			info.AddValue("TypeName", this.TypeName);
		}

		public override string ToString()
		{
			return this.DisplayName;
		}

		protected void ValidatePropertyType(string propertyName, object value, Type expectedType, bool allowNull)
		{
			if (!allowNull && value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value != null && !expectedType.Equals(value.GetType()))
			{
				string text = Locale.GetText("Type mismatch between value ({0}) and expected type ({1}).", new object[]
				{
					value.GetType(),
					expectedType
				});
				throw new ArgumentException(text, "value");
			}
		}

		protected virtual object ValidatePropertyValue(string propertyName, object value)
		{
			switch (propertyName)
			{
			case "AssemblyName":
				this.ValidatePropertyType(propertyName, value, typeof(AssemblyName), true);
				break;
			case "Bitmap":
				this.ValidatePropertyType(propertyName, value, typeof(Bitmap), true);
				break;
			case "Company":
			case "Description":
			case "DisplayName":
			case "TypeName":
				this.ValidatePropertyType(propertyName, value, typeof(string), true);
				if (value == null)
				{
					value = string.Empty;
				}
				break;
			case "IsTransient":
				this.ValidatePropertyType(propertyName, value, typeof(bool), false);
				break;
			case "Filter":
				this.ValidatePropertyType(propertyName, value, typeof(ToolboxItemFilterAttribute[]), true);
				if (value == null)
				{
					value = new ToolboxItemFilterAttribute[0];
				}
				break;
			case "DependentAssemblies":
				this.ValidatePropertyType(propertyName, value, typeof(AssemblyName[]), true);
				break;
			}
			return value;
		}

		private void SetValue(string propertyName, object value)
		{
			this.CheckUnlocked();
			this.properties[propertyName] = this.ValidatePropertyValue(propertyName, value);
		}

		private string GetValue(string propertyName)
		{
			string text = (string)this.properties[propertyName];
			return (text != null) ? text : string.Empty;
		}

		private bool locked;

		private Hashtable properties = new Hashtable();
	}
}
