using System;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Xml.Xsl.IlGen
{
	internal struct StorageDescriptor
	{
		public static StorageDescriptor None()
		{
			return default(StorageDescriptor);
		}

		public static StorageDescriptor Stack(Type itemStorageType, bool isCached)
		{
			return new StorageDescriptor
			{
				location = ItemLocation.Stack,
				itemStorageType = itemStorageType,
				isCached = isCached
			};
		}

		public static StorageDescriptor Parameter(int paramIndex, Type itemStorageType, bool isCached)
		{
			return new StorageDescriptor
			{
				location = ItemLocation.Parameter,
				locationObject = paramIndex,
				itemStorageType = itemStorageType,
				isCached = isCached
			};
		}

		public static StorageDescriptor Local(LocalBuilder loc, Type itemStorageType, bool isCached)
		{
			return new StorageDescriptor
			{
				location = ItemLocation.Local,
				locationObject = loc,
				itemStorageType = itemStorageType,
				isCached = isCached
			};
		}

		public static StorageDescriptor Current(LocalBuilder locIter, Type itemStorageType)
		{
			return new StorageDescriptor
			{
				location = ItemLocation.Current,
				locationObject = locIter,
				itemStorageType = itemStorageType
			};
		}

		public static StorageDescriptor Global(MethodInfo methGlobal, Type itemStorageType, bool isCached)
		{
			return new StorageDescriptor
			{
				location = ItemLocation.Global,
				locationObject = methGlobal,
				itemStorageType = itemStorageType,
				isCached = isCached
			};
		}

		public StorageDescriptor ToStack()
		{
			return StorageDescriptor.Stack(this.itemStorageType, this.isCached);
		}

		public StorageDescriptor ToLocal(LocalBuilder loc)
		{
			return StorageDescriptor.Local(loc, this.itemStorageType, this.isCached);
		}

		public StorageDescriptor ToStorageType(Type itemStorageType)
		{
			StorageDescriptor storageDescriptor = this;
			storageDescriptor.itemStorageType = itemStorageType;
			return storageDescriptor;
		}

		public ItemLocation Location
		{
			get
			{
				return this.location;
			}
		}

		public int ParameterLocation
		{
			get
			{
				return (int)this.locationObject;
			}
		}

		public LocalBuilder LocalLocation
		{
			get
			{
				return this.locationObject as LocalBuilder;
			}
		}

		public LocalBuilder CurrentLocation
		{
			get
			{
				return this.locationObject as LocalBuilder;
			}
		}

		public MethodInfo GlobalLocation
		{
			get
			{
				return this.locationObject as MethodInfo;
			}
		}

		public bool IsCached
		{
			get
			{
				return this.isCached;
			}
		}

		public Type ItemStorageType
		{
			get
			{
				return this.itemStorageType;
			}
		}

		private ItemLocation location;

		private object locationObject;

		private Type itemStorageType;

		private bool isCached;
	}
}
