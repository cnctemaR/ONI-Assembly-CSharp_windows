using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Microsoft.SqlServer.Server
{
	internal class SmiMetaDataPropertyCollection
	{
		private static SmiMetaDataPropertyCollection CreateEmptyInstance()
		{
			SmiMetaDataPropertyCollection smiMetaDataPropertyCollection = new SmiMetaDataPropertyCollection();
			smiMetaDataPropertyCollection.SetReadOnly();
			return smiMetaDataPropertyCollection;
		}

		internal SmiMetaDataPropertyCollection()
		{
			this._properties = new SmiMetaDataProperty[3];
			this._isReadOnly = false;
			this._properties[0] = SmiMetaDataPropertyCollection.s_emptyDefaultFields;
			this._properties[1] = SmiMetaDataPropertyCollection.s_emptySortOrder;
			this._properties[2] = SmiMetaDataPropertyCollection.s_emptyUniqueKey;
		}

		internal SmiMetaDataProperty this[SmiPropertySelector key]
		{
			get
			{
				return this._properties[(int)key];
			}
			set
			{
				if (value == null)
				{
					throw ADP.InternalError(ADP.InternalErrorCode.InvalidSmiCall);
				}
				this.EnsureWritable();
				this._properties[(int)key] = value;
			}
		}

		internal bool IsReadOnly
		{
			get
			{
				return this._isReadOnly;
			}
		}

		internal void SetReadOnly()
		{
			this._isReadOnly = true;
		}

		private void EnsureWritable()
		{
			if (this.IsReadOnly)
			{
				throw ADP.InternalError(ADP.InternalErrorCode.InvalidSmiCall);
			}
		}

		private const int SelectorCount = 3;

		private SmiMetaDataProperty[] _properties;

		private bool _isReadOnly;

		private static readonly SmiDefaultFieldsProperty s_emptyDefaultFields = new SmiDefaultFieldsProperty(new List<bool>());

		private static readonly SmiOrderProperty s_emptySortOrder = new SmiOrderProperty(new List<SmiOrderProperty.SmiColumnOrder>());

		private static readonly SmiUniqueKeyProperty s_emptyUniqueKey = new SmiUniqueKeyProperty(new List<bool>());

		internal static readonly SmiMetaDataPropertyCollection EmptyInstance = SmiMetaDataPropertyCollection.CreateEmptyInstance();
	}
}
