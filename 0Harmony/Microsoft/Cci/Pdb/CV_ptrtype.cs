using System;

namespace Microsoft.Cci.Pdb
{
	internal enum CV_ptrtype
	{
		CV_PTR_BASE_SEG = 3,
		CV_PTR_BASE_VAL,
		CV_PTR_BASE_SEGVAL,
		CV_PTR_BASE_ADDR,
		CV_PTR_BASE_SEGADDR,
		CV_PTR_BASE_TYPE,
		CV_PTR_BASE_SELF,
		CV_PTR_NEAR32,
		CV_PTR_64 = 12,
		CV_PTR_UNUSEDPTR
	}
}
