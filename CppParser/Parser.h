/************************************************************************
*
* CppSharp
* Licensed under the simplified BSD license. All rights reserved.
*
************************************************************************/

#pragma once

#include "CppParser.h"

#include <string>
#include <unordered_set>

namespace clang {
namespace CodeGen {
class CodeGenTypes;
}
struct ASTTemplateArgumentListInfo;
class FunctionTemplateSpecialization;
class FunctionTemplateSpecializationInfo;
class PreprocessingRecord;
class PreprocessedEntity;
class RawComment;
class TemplateSpecializationTypeLoc;
class TemplateArgumentList;
class VTableLayout;
class VTableComponent;
}

#define Debug printf

namespace CppSharp {
namespace CppParser {

class Parser
{
public:
	Parser(IComClangPtr pComClang, ICppParserOptionsPtr Opts);

	void Setup();
	IParserResultPtr ParseHeader(const std::vector<std::string>& SourceFiles);
	IParserResultPtr ParseLibrary(const std::string& File);
	IParserResultPtr ParseForAST(const std::vector<std::string>& SourceFiles);

private:
	bool IsSupported(const clang::NamedDecl* ND);
	bool IsSupported(const clang::CXXMethodDecl* MD);
	// AST traversers
	void WalkAST();
	IDeclarationPtr WalkDeclaration(const clang::Decl* D);
	IDeclarationPtr WalkDeclarationDef(clang::Decl* D);
	IEnumerationPtr WalkEnum(const clang::EnumDecl* ED);
	IEnumeration_ItemPtr WalkEnumItem(clang::EnumConstantDecl* ECD);
	IFunctionPtr WalkFunction(const clang::FunctionDecl* FD, bool IsDependent = false,
		bool AddToNamespace = true);
	void EnsureCompleteRecord(const clang::RecordDecl* Record, IDeclarationContextPtr NS, IClassPtr RC);
	IClassPtr GetRecord(const clang::RecordDecl* Record, bool& IsComplete);
	IClassPtr WalkRecord(const clang::RecordDecl* Record);
	void WalkRecord(const clang::RecordDecl* Record, IClassPtr RC);
	IClassPtr WalkRecordCXX(const clang::CXXRecordDecl* Record);
	void WalkRecordCXX(const clang::CXXRecordDecl* Record, IClassPtr RC);
	IClassTemplateSpecializationPtr WalkClassTemplateSpecialization(const clang::ClassTemplateSpecializationDecl* CTS);
	IClassTemplatePartialSpecializationPtr WalkClassTemplatePartialSpecialization(const clang::ClassTemplatePartialSpecializationDecl* CTS);
	IMethodPtr WalkMethodCXX(const clang::CXXMethodDecl* MD);
	IFieldPtr WalkFieldCXX(const clang::FieldDecl* FD, IClassPtr Class);
	IFunctionTemplateSpecializationPtr WalkFunctionTemplateSpec(clang::FunctionTemplateSpecializationInfo* FTS, IFunctionPtr Function);
	IVariablePtr WalkVariable(const clang::VarDecl* VD);
	void WalkVariable(const clang::VarDecl* VD, IVariablePtr Var);
	IFriendPtr WalkFriend(const clang::FriendDecl* FD);
	IRawCommentPtr WalkRawComment(const clang::RawComment* RC);
	ITypePtr WalkType(clang::QualType QualType, const clang::TypeLoc* TL = 0,
		bool DesugarType = false);
	ITemplateArgumentPtr WalkTemplateArgument(const clang::TemplateArgument& TA, clang::TemplateArgumentLoc* ArgLoc);
	ITemplateTemplateParameterPtr WalkTemplateTemplateParameter(const clang::TemplateTemplateParmDecl* TTP);
	ITypeTemplateParameterPtr WalkTypeTemplateParameter(const clang::TemplateTypeParmDecl* TTPD);
	INonTypeTemplateParameterPtr WalkNonTypeTemplateParameter(const clang::NonTypeTemplateParmDecl* TTPD);
	std::vector<IDeclaration *> WalkTemplateParameterList(const clang::TemplateParameterList* TPL);
	ITypeAliasTemplatePtr WalkTypeAliasTemplate(const clang::TypeAliasTemplateDecl* TD);
	IClassTemplatePtr WalkClassTemplate(const clang::ClassTemplateDecl* TD);
	IFunctionTemplatePtr WalkFunctionTemplate(const clang::FunctionTemplateDecl* TD);
	IVarTemplatePtr WalkVarTemplate(const clang::VarTemplateDecl* VT);
	IVarTemplateSpecializationPtr WalkVarTemplateSpecialization(const clang::VarTemplateSpecializationDecl* VTS);
	IVarTemplatePartialSpecializationPtr WalkVarTemplatePartialSpecialization(const clang::VarTemplatePartialSpecializationDecl* VTS);
	template<typename TypeLoc>
	std::vector<ITemplateArgument*> WalkTemplateArgumentList(const clang::TemplateArgumentList* TAL, TypeLoc* TSTL);
	std::vector<ITemplateArgument*> WalkTemplateArgumentList(const clang::TemplateArgumentList* TAL, const clang::ASTTemplateArgumentListInfo* TSTL);
	void WalkVTable(const clang::CXXRecordDecl* RD, IClassPtr C);
	IQualifiedTypePtr GetQualifiedType(const clang::QualType& qual, const clang::TypeLoc* TL = 0);
	void ReadClassLayout(IClassPtr Class, const clang::RecordDecl* RD, clang::CharUnits Offset, bool IncludeVirtualBases);
	ILayoutFieldPtr WalkVTablePointer(IClassPtr Class, const clang::CharUnits& Offset, const std::string& prefix);
	IVTableLayoutPtr WalkVTableLayout(const clang::VTableLayout& VTLayout);
	IVTableComponentPtr WalkVTableComponent(const clang::VTableComponent& Component);
	IPreprocessedEntityPtr WalkPreprocessedEntity(IDeclarationPtr Decl,
		clang::PreprocessedEntity* PPEntity);
	IStatementPtr WalkStatement(const clang::Stmt* Statement);
	IExpressionPtr WalkExpression(const clang::Expr* Expression);
	std::string GetStringFromStatement(const clang::Stmt* Statement);
	std::string GetFunctionBody(const clang::FunctionDecl* FD);

	// Clang helpers
	SourceLocationKind GetLocationKind(const clang::SourceLocation& Loc);
	bool IsValidDeclaration(const clang::SourceLocation& Loc);
	std::string GetDeclMangledName(const clang::Decl* D);
	std::string GetTypeName(const clang::Type* Type);
	bool CanCheckCodeGenInfo(clang::Sema & S, const clang::Type * Ty);
	void CompleteIfSpecializationType(const clang::QualType& QualType);
	IParameterPtr WalkParameter(const clang::ParmVarDecl* PVD,
		const clang::SourceLocation& ParamStartLoc);
	void SetBody(const clang::FunctionDecl* FD, IFunctionPtr F);
	std::stack<clang::Scope> GetScopesFor(clang::FunctionDecl* FD);
	void MarkValidity(IFunctionPtr F);
	void WalkFunction(const clang::FunctionDecl* FD, IFunctionPtr F,
		bool IsDependent = false);
	void HandlePreprocessedEntities(IDeclarationPtr Decl);
	void HandlePreprocessedEntities(IDeclarationPtr Decl, clang::SourceRange sourceRange,
		MacroLocation macroLocation);
	bool GetDeclText(clang::SourceRange SR, std::string& Text);

	ITranslationUnitPtr GetTranslationUnit(clang::SourceLocation Loc,
		SourceLocationKind *Kind = 0);
	ITranslationUnitPtr GetTranslationUnit(const clang::Decl* D);

	IDeclarationContextPtr GetNamespace(const clang::Decl* D, const clang::DeclContext* Ctx);
	IDeclarationContextPtr GetNamespace(const clang::Decl* D);

	void HandleDeclaration(const clang::Decl* D, IDeclarationPtr Decl);
	void HandleOriginalText(const clang::Decl* D, IDeclarationPtr Decl);
	void HandleComments(const clang::Decl* D, IDeclarationPtr Decl);
	void HandleDiagnostics(IParserResultPtr res);

	ParserResultKind ReadSymbols(llvm::StringRef File,
		llvm::object::basic_symbol_iterator Begin,
		llvm::object::basic_symbol_iterator End,
		INativeLibraryPtr & NativeLib);
	IDeclarationPtr GetDeclarationFromFriend(clang::NamedDecl* FriendDecl);
	ParserResultKind ParseArchive(llvm::StringRef File,
		llvm::object::Archive* Archive, INativeLibraryPtr & NativeLib);
	ParserResultKind ParseSharedLib(llvm::StringRef File,
		llvm::object::ObjectFile* ObjectFile, INativeLibraryPtr & NativeLib);
	IParserTargetInfoPtr GetTargetInfo();

	int index;
	ICppParserOptionsPtr opts;
	std::unique_ptr<clang::CompilerInstance> c;
	clang::TargetCXXABI::Kind targetABI;
	clang::CodeGen::CodeGenTypes* codeGenTypes;
	std::unordered_map<const clang::TemplateTypeParmDecl*, ITypeTemplateParameter*> walkedTypeTemplateParameters;
	std::unordered_map<const clang::TemplateTemplateParmDecl*, ITemplateTemplateParameter*> walkedTemplateTemplateParameters;
	std::unordered_map<const clang::NonTypeTemplateParmDecl*, INonTypeTemplateParameter*> walkedNonTypeTemplateParameters;
	std::unordered_map<const clang::ParmVarDecl*, IParameter*> walkedParameters;
	std::unordered_set<std::string> supportedStdTypes;
	IComClangPtr pComClang;
public:
	static IParserResultPtr ParseHeader(IComClangPtr pComClang, ICppParserOptionsPtr Opts);
	static IParserResultPtr ParseLibrary(IComClangPtr pComClang, ICppParserOptionsPtr Opts);
	static IParserResultPtr ParseForAST(IComClangPtr pComClang, ICppParserOptionsPtr Opts);
};

}
}