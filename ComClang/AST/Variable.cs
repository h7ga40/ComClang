
namespace CppSharp.AST
{
	public class Variable : Declaration, ITypedDecl, IMangledDecl, IVariable
	{
		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitVariableDecl(this);
		}

		public Type Type { get { return QualifiedType.Type; } }
		public QualifiedType QualifiedType { get; set; }

		public string Mangled { get; set; } = "";

		public Expression Init { get; set; }
		IQualifiedType IVariable.QualifiedType { get => QualifiedType; set => QualifiedType = (QualifiedType)value; }
		IExpression IVariable.Init { get => Init; set => Init = (Expression)value; }
	}
}
