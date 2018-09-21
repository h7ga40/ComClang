namespace CppSharp.AST
{
	/// <summary>
	/// Represents a a C/C++ record field Decl.
	/// </summary>
	public class Field : Declaration, ITypedDecl, IField
	{
		public Type Type { get { return QualifiedType.Type; } }
		public QualifiedType QualifiedType { get; set; }

		public Class Class { get; set; }

		public bool IsStatic { get; set; }

		public bool IsBitField { get; set; }

		public uint BitWidth { get; set; }

		IQualifiedType IField.QualifiedType { get => QualifiedType; set => QualifiedType = (QualifiedType)value; }

		IClass IField.Class { get => Class; set => Class = (Class)value; }

		public Field()
		{
		}

		public Field(string name, QualifiedType type, AccessSpecifier access)
		{
			Name = name;
			QualifiedType = type;
			Access = access;
		}

		public Field(Field field) : base(field)
		{
			QualifiedType = field.QualifiedType;
			Class = field.Class;
			IsBitField = field.IsBitField;
			BitWidth = field.BitWidth;
		}

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitFieldDecl(this);
		}
	}
}