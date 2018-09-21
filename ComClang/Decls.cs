using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComClang
{
	public class AccessSpecDecl : Decl { internal AccessSpecDecl(IDecl parent) : base(DeclKind.AccessSpec, parent) { } }
	public class BlockDecl : Decl { internal BlockDecl(IDecl parent) : base(DeclKind.Block, parent) { } }
	public class CapturedDecl : Decl { internal CapturedDecl(IDecl parent) : base(DeclKind.Captured, parent) { } }
	public class ClassScopeFunctionSpecializationDecl : Decl { internal ClassScopeFunctionSpecializationDecl(IDecl parent) : base(DeclKind.ClassScopeFunctionSpecialization, parent) { } }
	public class EmptyDecl : Decl { internal EmptyDecl(IDecl parent) : base(DeclKind.Empty, parent) { } }
	public class ExportDecl : Decl { internal ExportDecl(IDecl parent) : base(DeclKind.Export, parent) { } }
	public class ExternCContextDecl : Decl { internal ExternCContextDecl(IDecl parent) : base(DeclKind.ExternCContext, parent) { } }
	public class FileScopeAsmDecl : Decl { internal FileScopeAsmDecl(IDecl parent) : base(DeclKind.FileScopeAsm, parent) { } }
	public class FriendDecl : Decl { internal FriendDecl(IDecl parent) : base(DeclKind.Friend, parent) { } }
	public class FriendTemplateDecl : Decl { internal FriendTemplateDecl(IDecl parent) : base(DeclKind.FriendTemplate, parent) { } }
	public class ImportDecl : Decl { internal ImportDecl(IDecl parent) : base(DeclKind.Import, parent) { } }
	public class LinkageSpecDecl : Decl { internal LinkageSpecDecl(IDecl parent) : base(DeclKind.LinkageSpec, parent) { } }
	public class NamedDecl : Decl
	{
		public string Name { get; private set; }

		internal NamedDecl(DeclKind kind, IDecl parent) : base(kind, parent) { }

		public override void SetName(string name)
		{
			Name = name;
		}
	}
	public class LabelDecl : NamedDecl
	{
		internal LabelDecl(IDecl parent)
			: base(DeclKind.Label, parent)
		{
		}

		public IStmt Stmt { get; set; }
	}
	public class NamespaceDecl : NamedDecl { internal NamespaceDecl(IDecl parent) : base(DeclKind.Namespace, parent) { } }
	public class NamespaceAliasDecl : NamedDecl { internal NamespaceAliasDecl(IDecl parent) : base(DeclKind.NamespaceAlias, parent) { } }
	public class ObjCCompatibleAliasDecl : NamedDecl { internal ObjCCompatibleAliasDecl(IDecl parent) : base(DeclKind.ObjCCompatibleAlias, parent) { } }
	public class ObjCContainerDecl : NamedDecl { internal ObjCContainerDecl(DeclKind kind, IDecl parent) : base(kind, parent) { } }
	public class ObjCCategoryDecl : ObjCContainerDecl { internal ObjCCategoryDecl(IDecl parent) : base(DeclKind.ObjCCategory, parent) { } }
	public class ObjCImplDecl : ObjCContainerDecl { internal ObjCImplDecl(DeclKind kind, IDecl parent) : base(kind, parent) { } }
	public class ObjCCategoryImplDecl : ObjCImplDecl { internal ObjCCategoryImplDecl(IDecl parent) : base(DeclKind.ObjCCategoryImpl, parent) { } }
	public class ObjCImplementationDecl : ObjCImplDecl { internal ObjCImplementationDecl(IDecl parent) : base(DeclKind.ObjCImplementation, parent) { } }
	public class ObjCInterfaceDecl : ObjCContainerDecl { internal ObjCInterfaceDecl(IDecl parent) : base(DeclKind.ObjCInterface, parent) { } }
	public class ObjCProtocolDecl : ObjCContainerDecl { internal ObjCProtocolDecl(IDecl parent) : base(DeclKind.ObjCProtocol, parent) { } }
	public class ObjCMethodDecl : NamedDecl
	{
		internal ObjCMethodDecl(IDecl parent)
			: base(DeclKind.ObjCMethod, parent)
		{
		}

		public IStmt Body { get; set; }
	}
	public class ObjCPropertyDecl : NamedDecl { internal ObjCPropertyDecl(IDecl parent) : base(DeclKind.ObjCProperty, parent) { } }
	public class TemplateDecl : NamedDecl { internal TemplateDecl(DeclKind kind, IDecl parent) : base(kind, parent) { } }
	public class BuiltinTemplateDecl : TemplateDecl { internal BuiltinTemplateDecl(IDecl parent) : base(DeclKind.BuiltinTemplate, parent) { } }
	public class RedeclarableTemplateDecl : TemplateDecl { internal RedeclarableTemplateDecl(DeclKind kind, IDecl parent) : base(kind, parent) { } }
	public class ClassTemplateDecl : RedeclarableTemplateDecl { internal ClassTemplateDecl(IDecl parent) : base(DeclKind.ClassTemplate, parent) { } }
	public class FunctionTemplateDecl : RedeclarableTemplateDecl { internal FunctionTemplateDecl(IDecl parent) : base(DeclKind.FunctionTemplate, parent) { } }
	public class TypeAliasTemplateDecl : RedeclarableTemplateDecl { internal TypeAliasTemplateDecl(IDecl parent) : base(DeclKind.TypeAliasTemplate, parent) { } }
	public class VarTemplateDecl : RedeclarableTemplateDecl { internal VarTemplateDecl(IDecl parent) : base(DeclKind.VarTemplate, parent) { } }
	public class TemplateTemplateParmDecl : TemplateDecl { internal TemplateTemplateParmDecl(IDecl parent) : base(DeclKind.TemplateTemplateParm, parent) { } }
	public class TypeDecl : NamedDecl { internal TypeDecl(DeclKind kind, IDecl parent) : base(kind, parent) { } }
	public class TagDecl : TypeDecl { internal TagDecl(DeclKind kind, IDecl parent) : base(kind, parent) { } }
	public class EnumDecl : TagDecl { internal EnumDecl(IDecl parent) : base(DeclKind.Enum, parent) { } }
	public class RecordDecl : TagDecl
	{
		internal RecordDecl(IDecl parent) : base(DeclKind.Record, parent) { }
		protected RecordDecl(DeclKind kind, IDecl parent) : base(kind, parent) { }
	}
	public class CXXRecordDecl : RecordDecl
	{
		internal CXXRecordDecl(IDecl parent) : base(DeclKind.CXXRecord, parent) { }
		protected CXXRecordDecl(DeclKind kind, IDecl parent) : base(kind, parent) { }
	}
	public class ClassTemplateSpecializationDecl : CXXRecordDecl
	{
		internal ClassTemplateSpecializationDecl(IDecl parent) : base(DeclKind.ClassTemplateSpecialization, parent) { }
		protected ClassTemplateSpecializationDecl(DeclKind kind, IDecl parent) : base(kind, parent) { }
	}
	public class ClassTemplatePartialSpecializationDecl : ClassTemplateSpecializationDecl { internal ClassTemplatePartialSpecializationDecl(IDecl parent) : base(DeclKind.ClassTemplatePartialSpecialization, parent) { } }
	public class TemplateTypeParmDecl : TypeDecl { internal TemplateTypeParmDecl(IDecl parent) : base(DeclKind.TemplateTypeParm, parent) { } }
	public class TypedefNameDecl : TypeDecl { internal TypedefNameDecl(DeclKind kind, IDecl parent) : base(kind, parent) { } }
	public class ObjCTypeParamDecl : TypedefNameDecl { internal ObjCTypeParamDecl(IDecl parent) : base(DeclKind.ObjCTypeParam, parent) { } }
	public class TypeAliasDecl : TypedefNameDecl { internal TypeAliasDecl(IDecl parent) : base(DeclKind.TypeAlias, parent) { } }
	public class TypedefDecl : TypedefNameDecl { internal TypedefDecl(IDecl parent) : base(DeclKind.Typedef, parent) { } }
	public class UnresolvedUsingTypenameDecl : TypeDecl { internal UnresolvedUsingTypenameDecl(IDecl parent) : base(DeclKind.UnresolvedUsingTypename, parent) { } }
	public class UsingDecl : NamedDecl { internal UsingDecl(IDecl parent) : base(DeclKind.Using, parent) { } }
	public class UsingDirectiveDecl : NamedDecl { internal UsingDirectiveDecl(IDecl parent) : base(DeclKind.UsingDirective, parent) { } }
	public class UsingPackDecl : NamedDecl { internal UsingPackDecl(IDecl parent) : base(DeclKind.UsingPack, parent) { } }
	public class UsingShadowDecl : NamedDecl
	{
		internal UsingShadowDecl(IDecl parent) : base(DeclKind.UsingShadow, parent) { }
		protected UsingShadowDecl(DeclKind kind, IDecl parent) : base(kind, parent) { }
	}
	public class ConstructorUsingShadowDecl : UsingShadowDecl { internal ConstructorUsingShadowDecl(IDecl parent) : base(DeclKind.ConstructorUsingShadow, parent) { } }
	public class ValueDecl : NamedDecl { protected ValueDecl(DeclKind kind, IDecl parent) : base(kind, parent) { } }
	public class BindingDecl : ValueDecl { internal BindingDecl(IDecl parent) : base(DeclKind.Binding, parent) { } }
	public class DeclaratorDecl : ValueDecl { internal DeclaratorDecl(DeclKind kind, IDecl parent) : base(kind, parent) { } }
	public class FieldDecl : DeclaratorDecl
	{
		internal FieldDecl(IDecl parent) : base(DeclKind.Field, parent) { }
		protected FieldDecl(DeclKind kind, IDecl parent) : base(kind, parent) { }
	}
	public class ObjCAtDefsFieldDecl : FieldDecl { internal ObjCAtDefsFieldDecl(IDecl parent) : base(DeclKind.ObjCAtDefsField, parent) { } }
	public class ObjCIvarDecl : FieldDecl { internal ObjCIvarDecl(IDecl parent) : base(DeclKind.ObjCIvar, parent) { } }

	public class FunctionDecl : DeclaratorDecl
	{
		internal FunctionDecl(IDecl parent) : base(DeclKind.Function, parent) { }
		protected FunctionDecl(DeclKind kind, IDecl parent) : base(kind, parent) { }

		public IStmt Body { get; set; }
	}

	public class CXXDeductionGuideDecl : FunctionDecl { internal CXXDeductionGuideDecl(IDecl parent) : base(DeclKind.CXXDeductionGuide, parent) { } }
	public class CXXMethodDecl : FunctionDecl
	{
		internal CXXMethodDecl(IDecl parent) : base(DeclKind.CXXMethod, parent) { }
		protected CXXMethodDecl(DeclKind kind, IDecl parent) : base(kind, parent) { }
	}
	public class CXXConstructorDecl : CXXMethodDecl { internal CXXConstructorDecl(IDecl parent) : base(DeclKind.CXXConstructor, parent) { } }
	public class CXXConversionDecl : CXXMethodDecl { internal CXXConversionDecl(IDecl parent) : base(DeclKind.CXXConversion, parent) { } }
	public class CXXDestructorDecl : CXXMethodDecl { internal CXXDestructorDecl(IDecl parent) : base(DeclKind.CXXDestructor, parent) { } }
	public class MSPropertyDecl : DeclaratorDecl { internal MSPropertyDecl(IDecl parent) : base(DeclKind.MSProperty, parent) { } }
	public class NonTypeTemplateParmDecl : DeclaratorDecl { internal NonTypeTemplateParmDecl(IDecl parent) : base(DeclKind.NonTypeTemplateParm, parent) { } }
	public class VarDecl : DeclaratorDecl
	{
		internal VarDecl(IDecl parent) : base(DeclKind.Var, parent) { }
		protected VarDecl(DeclKind kind, IDecl parent) : base(kind, parent) { }
	}
	public class DecompositionDecl : VarDecl { internal DecompositionDecl(IDecl parent) : base(DeclKind.Decomposition, parent) { } }
	public class ImplicitParamDecl : VarDecl { internal ImplicitParamDecl(IDecl parent) : base(DeclKind.ImplicitParam, parent) { } }
	public class OMPCapturedExprDecl : VarDecl { internal OMPCapturedExprDecl(IDecl parent) : base(DeclKind.OMPCapturedExpr, parent) { } }
	public class ParmVarDecl : VarDecl { internal ParmVarDecl(IDecl parent) : base(DeclKind.ParmVar, parent) { } }
	public class VarTemplateSpecializationDecl : VarDecl
	{
		internal VarTemplateSpecializationDecl(IDecl parent) : base(DeclKind.VarTemplateSpecialization, parent) { }
		protected VarTemplateSpecializationDecl(DeclKind kind, IDecl parent) : base(kind, parent) { }
	}
	public class VarTemplatePartialSpecializationDecl : VarTemplateSpecializationDecl { internal VarTemplatePartialSpecializationDecl(IDecl parent) : base(DeclKind.VarTemplatePartialSpecialization, parent) { } }
	public class EnumConstantDecl : ValueDecl { internal EnumConstantDecl(IDecl parent) : base(DeclKind.EnumConstant, parent) { } }
	public class IndirectFieldDecl : ValueDecl { internal IndirectFieldDecl(IDecl parent) : base(DeclKind.IndirectField, parent) { } }
	public class OMPDeclareReductionDecl : ValueDecl { internal OMPDeclareReductionDecl(IDecl parent) : base(DeclKind.OMPDeclareReduction, parent) { } }
	public class UnresolvedUsingValueDecl : ValueDecl { internal UnresolvedUsingValueDecl(IDecl parent) : base(DeclKind.UnresolvedUsingValue, parent) { } }
	public class OMPThreadPrivateDecl : Decl { internal OMPThreadPrivateDecl(IDecl parent) : base(DeclKind.OMPThreadPrivate, parent) { } }
	public class ObjCPropertyImplDecl : Decl { internal ObjCPropertyImplDecl(IDecl parent) : base(DeclKind.ObjCPropertyImpl, parent) { } }
	public class PragmaCommentDecl : Decl { internal PragmaCommentDecl(IDecl parent) : base(DeclKind.PragmaComment, parent) { } }
	public class PragmaDetectMismatchDecl : Decl { internal PragmaDetectMismatchDecl(IDecl parent) : base(DeclKind.PragmaDetectMismatch, parent) { } }
	public class StaticAssertDecl : Decl { internal StaticAssertDecl(IDecl parent) : base(DeclKind.StaticAssert, parent) { } }
	public class TranslationUnitDecl : Decl { internal TranslationUnitDecl(IDecl parent) : base(DeclKind.TranslationUnit, parent) { } }
}
