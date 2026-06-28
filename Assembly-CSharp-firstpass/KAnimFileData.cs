using System;
using System.Diagnostics;

[DebuggerDisplay("{name}")]
public class KAnimFileData
{
	public string name;

	public KAnim.Anim[] anims;

	public KAnim.Anim.Frame[] animFrames;

	public KAnim.Anim.FrameElement[] animFrameElements;

	public KAnim.AnimHashTable hashTable;

	public HashedString batchTag;

	public int maxVisSymbolFrames;

	public KAnim.Build build;
}
