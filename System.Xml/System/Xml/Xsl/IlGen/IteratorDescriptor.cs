using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml.XPath;

namespace System.Xml.Xsl.IlGen
{
	internal class IteratorDescriptor
	{
		public IteratorDescriptor(GenerateHelper helper)
		{
			this.Init(null, helper);
		}

		public IteratorDescriptor(IteratorDescriptor iterParent)
		{
			this.Init(iterParent, iterParent.helper);
		}

		private void Init(IteratorDescriptor iterParent, GenerateHelper helper)
		{
			this.helper = helper;
			this.iterParent = iterParent;
		}

		public IteratorDescriptor ParentIterator
		{
			get
			{
				return this.iterParent;
			}
		}

		public bool HasLabelNext
		{
			get
			{
				return this.hasNext;
			}
		}

		public Label GetLabelNext()
		{
			return this.lblNext;
		}

		public void SetIterator(Label lblNext, StorageDescriptor storage)
		{
			this.lblNext = lblNext;
			this.hasNext = true;
			this.storage = storage;
		}

		public void SetIterator(IteratorDescriptor iterInfo)
		{
			if (iterInfo.HasLabelNext)
			{
				this.lblNext = iterInfo.GetLabelNext();
				this.hasNext = true;
			}
			this.storage = iterInfo.Storage;
		}

		public void LoopToEnd(Label lblOnEnd)
		{
			if (this.hasNext)
			{
				this.helper.BranchAndMark(this.lblNext, lblOnEnd);
				this.hasNext = false;
			}
			this.storage = StorageDescriptor.None();
		}

		public LocalBuilder LocalPosition
		{
			get
			{
				return this.locPos;
			}
			set
			{
				this.locPos = value;
			}
		}

		public void CacheCount()
		{
			this.PushValue();
			this.helper.CallCacheCount(this.storage.ItemStorageType);
		}

		public void EnsureNoCache()
		{
			if (this.storage.IsCached)
			{
				if (!this.HasLabelNext)
				{
					this.EnsureStack();
					this.helper.LoadInteger(0);
					this.helper.CallCacheItem(this.storage.ItemStorageType);
					this.storage = StorageDescriptor.Stack(this.storage.ItemStorageType, false);
					return;
				}
				LocalBuilder localBuilder = this.helper.DeclareLocal("$$$idx", typeof(int));
				this.EnsureNoStack("$$$cache");
				this.helper.LoadInteger(-1);
				this.helper.Emit(OpCodes.Stloc, localBuilder);
				Label label = this.helper.DefineLabel();
				this.helper.MarkLabel(label);
				this.helper.Emit(OpCodes.Ldloc, localBuilder);
				this.helper.LoadInteger(1);
				this.helper.Emit(OpCodes.Add);
				this.helper.Emit(OpCodes.Stloc, localBuilder);
				this.helper.Emit(OpCodes.Ldloc, localBuilder);
				this.CacheCount();
				this.helper.Emit(OpCodes.Bge, this.GetLabelNext());
				this.PushValue();
				this.helper.Emit(OpCodes.Ldloc, localBuilder);
				this.helper.CallCacheItem(this.storage.ItemStorageType);
				this.SetIterator(label, StorageDescriptor.Stack(this.storage.ItemStorageType, false));
			}
		}

		public void SetBranching(BranchingContext brctxt, Label lblBranch)
		{
			this.brctxt = brctxt;
			this.lblBranch = lblBranch;
		}

		public bool IsBranching
		{
			get
			{
				return this.brctxt > BranchingContext.None;
			}
		}

		public Label LabelBranch
		{
			get
			{
				return this.lblBranch;
			}
		}

		public BranchingContext CurrentBranchingContext
		{
			get
			{
				return this.brctxt;
			}
		}

		public StorageDescriptor Storage
		{
			get
			{
				return this.storage;
			}
			set
			{
				this.storage = value;
			}
		}

		public void PushValue()
		{
			switch (this.storage.Location)
			{
			case ItemLocation.Stack:
				this.helper.Emit(OpCodes.Dup);
				return;
			case ItemLocation.Parameter:
				this.helper.LoadParameter(this.storage.ParameterLocation);
				return;
			case ItemLocation.Local:
				this.helper.Emit(OpCodes.Ldloc, this.storage.LocalLocation);
				return;
			case ItemLocation.Current:
				this.helper.Emit(OpCodes.Ldloca, this.storage.CurrentLocation);
				this.helper.Call(this.storage.CurrentLocation.LocalType.GetMethod("get_Current"));
				return;
			default:
				return;
			}
		}

		public void EnsureStack()
		{
			switch (this.storage.Location)
			{
			case ItemLocation.Stack:
				return;
			case ItemLocation.Parameter:
			case ItemLocation.Local:
			case ItemLocation.Current:
				this.PushValue();
				break;
			case ItemLocation.Global:
				this.helper.LoadQueryRuntime();
				this.helper.Call(this.storage.GlobalLocation);
				break;
			}
			this.storage = this.storage.ToStack();
		}

		public void EnsureNoStack(string locName)
		{
			if (this.storage.Location == ItemLocation.Stack)
			{
				this.EnsureLocal(locName);
			}
		}

		public void EnsureLocal(string locName)
		{
			if (this.storage.Location != ItemLocation.Local)
			{
				if (this.storage.IsCached)
				{
					this.EnsureLocal(this.helper.DeclareLocal(locName, typeof(IList<>).MakeGenericType(new Type[] { this.storage.ItemStorageType })));
					return;
				}
				this.EnsureLocal(this.helper.DeclareLocal(locName, this.storage.ItemStorageType));
			}
		}

		public void EnsureLocal(LocalBuilder bldr)
		{
			if (this.storage.LocalLocation != bldr)
			{
				this.EnsureStack();
				this.helper.Emit(OpCodes.Stloc, bldr);
				this.storage = this.storage.ToLocal(bldr);
			}
		}

		public void DiscardStack()
		{
			if (this.storage.Location == ItemLocation.Stack)
			{
				this.helper.Emit(OpCodes.Pop);
				this.storage = StorageDescriptor.None();
			}
		}

		public void EnsureStackNoCache()
		{
			this.EnsureNoCache();
			this.EnsureStack();
		}

		public void EnsureNoStackNoCache(string locName)
		{
			this.EnsureNoCache();
			this.EnsureNoStack(locName);
		}

		public void EnsureLocalNoCache(string locName)
		{
			this.EnsureNoCache();
			this.EnsureLocal(locName);
		}

		public void EnsureLocalNoCache(LocalBuilder bldr)
		{
			this.EnsureNoCache();
			this.EnsureLocal(bldr);
		}

		public void EnsureItemStorageType(XmlQueryType xmlType, Type storageTypeDest)
		{
			if (!(this.storage.ItemStorageType == storageTypeDest))
			{
				if (this.storage.IsCached)
				{
					if (this.storage.ItemStorageType == typeof(XPathNavigator))
					{
						this.EnsureStack();
						this.helper.Call(XmlILMethods.NavsToItems);
						goto IL_014D;
					}
					if (storageTypeDest == typeof(XPathNavigator))
					{
						this.EnsureStack();
						this.helper.Call(XmlILMethods.ItemsToNavs);
						goto IL_014D;
					}
				}
				this.EnsureStackNoCache();
				if (this.storage.ItemStorageType == typeof(XPathItem))
				{
					if (storageTypeDest == typeof(XPathNavigator))
					{
						this.helper.Emit(OpCodes.Castclass, typeof(XPathNavigator));
					}
					else
					{
						this.helper.CallValueAs(storageTypeDest);
					}
				}
				else if (!(this.storage.ItemStorageType == typeof(XPathNavigator)))
				{
					this.helper.LoadInteger(this.helper.StaticData.DeclareXmlType(xmlType));
					this.helper.LoadQueryRuntime();
					this.helper.Call(XmlILMethods.StorageMethods[this.storage.ItemStorageType].ToAtomicValue);
				}
			}
			IL_014D:
			this.storage = this.storage.ToStorageType(storageTypeDest);
		}

		private GenerateHelper helper;

		private IteratorDescriptor iterParent;

		private Label lblNext;

		private bool hasNext;

		private LocalBuilder locPos;

		private BranchingContext brctxt;

		private Label lblBranch;

		private StorageDescriptor storage;
	}
}
