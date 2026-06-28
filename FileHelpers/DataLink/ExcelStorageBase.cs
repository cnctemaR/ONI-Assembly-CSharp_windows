using System;

namespace FileHelpers.DataLink
{
	public abstract class ExcelStorageBase : DataStorage
	{
		public ExcelUpdateLinksMode UpdateLinks
		{
			get
			{
				return this.mUpdateLinks;
			}
			set
			{
				this.mUpdateLinks = value;
			}
		}

		public ExcelStorageBase(Type recordType)
			: base(recordType)
		{
		}

		public ExcelStorageBase(Type recordType, int startRow, int startCol)
			: this(recordType)
		{
			this.mStartColumn = startCol;
			this.mStartRow = startRow;
		}

		public ExcelStorageBase(Type recordType, string fileName, int startRow, int startCol)
			: this(recordType, startRow, startCol)
		{
			this.mFileName = fileName;
		}

		public int StartRow
		{
			get
			{
				return this.mStartRow;
			}
			set
			{
				this.mStartRow = value;
			}
		}

		public int StartColumn
		{
			get
			{
				return this.mStartColumn;
			}
			set
			{
				this.mStartColumn = value;
			}
		}

		public int HeaderRows
		{
			get
			{
				return this.mHeaderRows;
			}
			set
			{
				this.mHeaderRows = value;
			}
		}

		public string FileName
		{
			get
			{
				return this.mFileName;
			}
			set
			{
				this.mFileName = value;
			}
		}

		public string SheetName
		{
			get
			{
				return this.mSheetName;
			}
			set
			{
				this.mSheetName = value;
			}
		}

		public bool OverrideFile
		{
			get
			{
				return this.mOverrideFile;
			}
			set
			{
				this.mOverrideFile = value;
			}
		}

		public string TemplateFile
		{
			get
			{
				return this.mTemplateFile;
			}
			set
			{
				this.mTemplateFile = value;
			}
		}

		public ExcelReadStopBehavior ExcelReadStopBehavior
		{
			get
			{
				return this.mExcelReadStopBehavior;
			}
			set
			{
				this.mExcelReadStopBehavior = value;
			}
		}

		public int ExcelReadStopAfterEmptyRows
		{
			get
			{
				return this.mExcelReadStopAfterEmptyRows;
			}
			set
			{
				this.mExcelReadStopAfterEmptyRows = value;
			}
		}

		protected bool CellIsEmpty(object row, object col)
		{
			string text = this.CellAsString(row, col);
			return string.IsNullOrEmpty(text);
		}

		protected abstract string CellAsString(object row, object col);

		protected bool RowIsEmpty(int cRow)
		{
			for (int i = this.StartColumn; i < this.StartColumn + base.RecordFieldCount; i++)
			{
				if (!this.CellIsEmpty(cRow, i))
				{
					return false;
				}
			}
			return true;
		}

		protected bool ShouldStopOnRow(int cRow)
		{
			switch (this.ExcelReadStopBehavior)
			{
			case ExcelReadStopBehavior.StopOnEmptyRow:
			{
				for (int i = cRow + (this.ExcelReadStopAfterEmptyRows - 1); i >= cRow; i--)
				{
					if (!this.RowIsEmpty(i))
					{
						return false;
					}
				}
				return true;
			}
			case ExcelReadStopBehavior.StopOnEmptyFirstCell:
				return this.CellIsEmpty(cRow, this.StartColumn);
			default:
				throw new ArgumentOutOfRangeException("Need to support new ExcelReadStopBehavior: " + this.ExcelReadStopBehavior);
			}
		}

		protected bool ShouldReadRowData(int cRow)
		{
			switch (this.ExcelReadStopBehavior)
			{
			case ExcelReadStopBehavior.StopOnEmptyRow:
				return !this.RowIsEmpty(cRow);
			case ExcelReadStopBehavior.StopOnEmptyFirstCell:
				return true;
			default:
				throw new ArgumentOutOfRangeException("Need to support new ExcelReadStopBehavior: " + this.ExcelReadStopBehavior);
			}
		}

		private ExcelUpdateLinksMode mUpdateLinks;

		private string mSheetName = string.Empty;

		private string mFileName = string.Empty;

		private int mStartRow = 1;

		private int mStartColumn = 1;

		private int mHeaderRows;

		private string mTemplateFile = string.Empty;

		private ExcelReadStopBehavior mExcelReadStopBehavior;

		private int mExcelReadStopAfterEmptyRows = 1;

		private bool mOverrideFile = true;
	}
}
