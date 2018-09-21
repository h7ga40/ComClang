namespace CppSharp.AST
{
	/// <summary>
	/// Base class for declarations which introduce a typedef-name.
	/// </summary>
	public abstract class TypedefNameDecl : Declaration, ITypedDecl, ITypedefNameDecl
	{
		public Type Type { get { return QualifiedType.Type; } }
		public QualifiedType QualifiedType { get; set; }
		public bool IsSynthetized { get; set; }
		IQualifiedType ITypedefNameDecl.QualifiedType { get => QualifiedType; set => QualifiedType = (QualifiedType)value; }
	}

	/// <summary>
	/// Represents a type definition in C++.
	/// </summary>
	public class TypedefDecl : TypedefNameDecl, ITypedefDecl
	{
		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitTypedefDecl(this);
		}
	}

	/// <summary>
	/// Represents a type alias in C++.
	/// </summary>
	public class TypeAlias : TypedefNameDecl, ITypeAlias
	{
		public TypeAliasTemplate DescribedAliasTemplate { get; set; }
		ITypeAliasTemplate ITypeAlias.DescribedAliasTemplate { get => DescribedAliasTemplate; set => DescribedAliasTemplate = (TypeAliasTemplate)value; }

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitTypeAliasDecl(this);
		}
	}
}