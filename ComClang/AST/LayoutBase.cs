namespace CppSharp.AST
{
	public class LayoutBase : ILayoutBase
	{
		public uint Offset { get; set; }
		public Class Class { get; set; }
		IClass ILayoutBase.Class { get => Class; set => Class = (Class)value; }
	}
}
