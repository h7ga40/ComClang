Type(TypeKind kind);
QualifiedType();
TagType() : base(TypeKind.Tag);
ArrayType() : base(TypeKind.Array) { }
FunctionType() : base(TypeKind.Function) { }
PointerType() : base(TypeKind.Pointer) { }
MemberPointerType() : base(TypeKind.MemberPointer) { }
TypedefType() : base(TypeKind.Typedef) { }
AttributedType() : base(TypeKind.Attributed) { }
DecayedType() : base(TypeKind.Decayed) { }
TemplateArgument() { }
TemplateSpecializationType() : base(TypeKind.TemplateSpecialization) { }
DependentTemplateSpecializationType() : base(TypeKind.TemplateParameterSubstitution) { }
TemplateParameterType() : base(TypeKind.TemplateParameter) { }
TemplateParameterSubstitutionType() : base(TypeKind.TemplateParameterSubstitution) { }
InjectedClassNameType() : base(TypeKind.InjectedClassName) { }
DependentNameType() : base(TypeKind.DependentName) { }
PackExpansionType() : base(TypeKind.PackExpansion) { }
UnaryTransformType() : base(TypeKind.UnaryTransform) { }
VectorType() : base(TypeKind.Vector) { }
BuiltinType() : base(TypeKind.Builtin) { }
VTableComponent() { }
VTableLayout() { }
VFTableInfo() { }
LayoutField() { }
LayoutBase() { }
ClassLayout() { }
Declaration(DeclarationKind kind) { }
DeclarationContext(DeclarationKind kind) : base(kind) { }
TypedefNameDecl(DeclarationKind kind) : base(kind) { }
TypedefDecl() : base(DeclarationKind.Typedef) { }
TypeAlias() : base(DeclarationKind.TypeAlias) { }
Friend() : base(DeclarationKind.Friend) { }
Statement(string str, StatementClass Class = StatementClass.Any, Declaration decl = null){}
Expression(string str, StatementClass Class = StatementClass.Any, Declaration decl = null)			: base(str, Class, decl)		{		}
BinaryOperator(string str, Expression lhs, Expression rhs, string opcodeStr)			: base(str, StatementClass.BinaryOperator)		{		}
CallExpr(string str, Declaration decl)			: base(str, StatementClass.CallExprClass, decl)		{		}
CXXConstructExpr(string str, Declaration decl = null)
: base(str, StatementClass.CXXConstructExprClass, decl)
{
}
InitListExpr(string str, Declaration decl = null)
	: base(str, StatementClass.InitListExprClass, decl)
{
}
SubStmtExpr(string str, Declaration decl = null)
	: base(str, StatementClass.SubStmtExpr, decl)
{
}
Parameter()
	: base(DeclarationKind.Parameter)
{
}
Function()
	: base(DeclarationKind.Function)
{
}
Method() { }
Enumeration() : base(DeclarationKind.Enumeration) { }
Enumeration_Item() : base(DeclarationKind.EnumerationItem) { }
Variable() : base(DeclarationKind.Variable) { }
BaseClassSpecifier() { }
Field() : base(DeclarationKind.Field) { }
AccessSpecifierDecl() : base(DeclarationKind.AccessSpecifier) { }
Class() : base(DeclarationKind.Class) { }
Template(DeclarationKind kind) : base(DeclarationKind.Template)
{
}
TypeAliasTemplate() : base(DeclarationKind.TypeAliasTemplate) { }
TemplateParameter(DeclarationKind kind) : base(kind) { }
TemplateTemplateParameter() : base(DeclarationKind.TemplateTemplateParm) { }
TypeTemplateParameter() : base(DeclarationKind.TemplateTypeParm) { }
NonTypeTemplateParameter() : base(DeclarationKind.NonTypeTemplateParm) { }
ClassTemplate() : base(DeclarationKind.ClassTemplate) { }
ClassTemplateSpecialization() { }
ClassTemplatePartialSpecialization() { }
FunctionTemplate() : base(DeclarationKind.FunctionTemplate) { }
FunctionTemplateSpecialization() { }
VarTemplate() : base(DeclarationKind.VarTemplate) { }
VarTemplateSpecialization() { }
VarTemplatePartialSpecialization() { }
Namespace() : base(DeclarationKind.Namespace) { }
PreprocessedEntity() { }
MacroDefinition() { }
MacroExpansion() { }
TranslationUnit() { }
NativeLibrary() { }
ASTContext() { }
Comment(CommentKind kind) { }
BlockContentComment() : base(CommentKind.BlockContentComment) { }
BlockContentComment(CommentKind Kind) : base(Kind) { }
FullComment() : base(CommentKind.FullComment) { }
InlineContentComment() : base(CommentKind.InlineContentComment) { }
InlineContentComment(CommentKind Kind) : base(Kind) { }
ParagraphComment() : base(CommentKind.ParagraphComment) { }
BlockCommandComment_Argument() { }
BlockCommandComment() : base(CommentKind.BlockCommandComment) { }
BlockCommandComment(CommentKind Kind) : base(Kind) { }
ParamCommandComment() : base(CommentKind.ParamCommandComment) { }
TParamCommandComment() : base(CommentKind.TParamCommandComment) { }
VerbatimBlockLineComment() : base(CommentKind.VerbatimBlockComment) { }
VerbatimBlockComment() : base(CommentKind.VerbatimBlockLineComment) { }
VerbatimLineComment() : base(CommentKind.VerbatimLineComment) { }
InlineCommandComment_Argument() { }
InlineCommandComment() : base(CommentKind.InlineCommandComment) { }
HTMLTagComment() : base(CommentKind.HTMLTagComment) { }
HTMLTagComment(CommentKind Kind) : base(Kind) { }
HTMLStartTagComment_Attribute() { }
HTMLStartTagComment() : base(CommentKind.HTMLStartTagComment) { }
HTMLEndTagComment() : base(CommentKind.HTMLEndTagComment) { }
TextComment() : base(CommentKind.TextComment) { }
RawComment() { }
