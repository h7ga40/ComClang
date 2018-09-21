using System;

namespace CppSharp.AST
{
	public class LayoutField : ILayoutField
	{
		public uint Offset { get; set; }
		public QualifiedType QualifiedType { get; set; }
		public string Name { get; set; } = "";
		public IntPtr FieldPtr { get; set; }
		public bool IsVTablePtr { get { return FieldPtr == IntPtr.Zero; } }
		public Expression Expression { get; set; }
		IQualifiedType ILayoutField.QualifiedType { get => QualifiedType; set => QualifiedType = (QualifiedType)value; }

		public override string ToString()
		{
			return string.Format("{0} | {1}", Offset, Name);
		}
	}
}