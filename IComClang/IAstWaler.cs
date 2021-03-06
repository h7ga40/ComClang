	[ComVisible(true)]
	public interface IAstWalker
	{
		bool Visit(DeclWalker D);
		bool VisitAccessSpecDecl(AccessSpecDeclWalker D);
		bool VisitBindingDecl(BindingDeclWalker D);
		bool VisitBlockDecl(BlockDeclWalker D);
		bool VisitBuiltinTemplateDecl(BuiltinTemplateDeclWalker D);
		bool VisitCapturedDecl(CapturedDeclWalker D);
		bool VisitClassScopeFunctionSpecializationDecl(ClassScopeFunctionSpecializationDeclWalker D);
		bool VisitClassTemplateDecl(ClassTemplateDeclWalker D);
		bool VisitClassTemplatePartialSpecializationDecl(ClassTemplatePartialSpecializationDeclWalker D);
		bool VisitClassTemplateSpecializationDecl(ClassTemplateSpecializationDeclWalker D);
		bool VisitConstructorUsingShadowDecl(ConstructorUsingShadowDeclWalker D);
		bool VisitCXXConstructorDecl(CXXConstructorDeclWalker D);
		bool VisitCXXConversionDecl(CXXConversionDeclWalker D);
		bool VisitCXXDeductionGuideDecl(CXXDeductionGuideDeclWalker D);
		bool VisitCXXDestructorDecl(CXXDestructorDeclWalker D);
		bool VisitCXXMethodDecl(CXXMethodDeclWalker D);
		bool VisitCXXRecordDecl(CXXRecordDeclWalker D);
		bool VisitDecompositionDecl(DecompositionDeclWalker D);
		bool VisitEmptyDecl(EmptyDeclWalker D);
		bool VisitEnumConstantDecl(EnumConstantDeclWalker D);
		bool VisitEnumDecl(EnumDeclWalker D);
		bool VisitExportDecl(ExportDeclWalker D);
		bool VisitExternCContextDecl(ExternCContextDeclWalker D);
		bool VisitFieldDecl(FieldDeclWalker D);
		bool VisitFileScopeAsmDecl(FileScopeAsmDeclWalker D);
		bool VisitFriendDecl(FriendDeclWalker D);
		bool VisitFriendTemplateDecl(FriendTemplateDeclWalker D);
		bool VisitFunctionDecl(FunctionDeclWalker D);
		bool VisitFunctionTemplateDecl(FunctionTemplateDeclWalker D);
		bool VisitImplicitParamDecl(ImplicitParamDeclWalker D);
		bool VisitImportDecl(ImportDeclWalker D);
		bool VisitIndirectFieldDecl(IndirectFieldDeclWalker D);
		bool VisitLabelDecl(LabelDeclWalker D);
		bool VisitLinkageSpecDecl(LinkageSpecDeclWalker D);
		bool VisitMSPropertyDecl(MSPropertyDeclWalker D);
		bool VisitNamespaceAliasDecl(NamespaceAliasDeclWalker D);
		bool VisitNamespaceDecl(NamespaceDeclWalker D);
		bool VisitNonTypeTemplateParmDecl(NonTypeTemplateParmDeclWalker D);
		bool VisitObjCAtDefsFieldDecl(ObjCAtDefsFieldDeclWalker D);
		bool VisitObjCCategoryDecl(ObjCCategoryDeclWalker D);
		bool VisitObjCCategoryImplDecl(ObjCCategoryImplDeclWalker D);
		bool VisitObjCCompatibleAliasDecl(ObjCCompatibleAliasDeclWalker D);
		bool VisitObjCImplementationDecl(ObjCImplementationDeclWalker D);
		bool VisitObjCInterfaceDecl(ObjCInterfaceDeclWalker D);
		bool VisitObjCIvarDecl(ObjCIvarDeclWalker D);
		bool VisitObjCMethodDecl(ObjCMethodDeclWalker D);
		bool VisitObjCPropertyDecl(ObjCPropertyDeclWalker D);
		bool VisitObjCPropertyImplDecl(ObjCPropertyImplDeclWalker D);
		bool VisitObjCProtocolDecl(ObjCProtocolDeclWalker D);
		bool VisitObjCTypeParamDecl(ObjCTypeParamDeclWalker D);
		bool VisitOMPCapturedExprDecl(OMPCapturedExprDeclWalker D);
		bool VisitOMPDeclareReductionDecl(OMPDeclareReductionDeclWalker D);
		bool VisitOMPThreadPrivateDecl(OMPThreadPrivateDeclWalker D);
		bool VisitParmVarDecl(ParmVarDeclWalker D);
		bool VisitPragmaCommentDecl(PragmaCommentDeclWalker D);
		bool VisitPragmaDetectMismatchDecl(PragmaDetectMismatchDeclWalker D);
		bool VisitRecordDecl(RecordDeclWalker D);
		bool VisitStaticAssertDecl(StaticAssertDeclWalker D);
		bool VisitTemplateTemplateParmDecl(TemplateTemplateParmDeclWalker D);
		bool VisitTemplateTypeParmDecl(TemplateTypeParmDeclWalker D);
		bool VisitTranslationUnitDecl(TranslationUnitDeclWalker D);
		bool VisitTypeAliasDecl(TypeAliasDeclWalker D);
		bool VisitTypeAliasTemplateDecl(TypeAliasTemplateDeclWalker D);
		bool VisitTypedefDecl(TypedefDeclWalker D);
		bool VisitUnresolvedUsingTypenameDecl(UnresolvedUsingTypenameDeclWalker D);
		bool VisitUnresolvedUsingValueDecl(UnresolvedUsingValueDeclWalker D);
		bool VisitUsingDecl(UsingDeclWalker D);
		bool VisitUsingDirectiveDecl(UsingDirectiveDeclWalker D);
		bool VisitUsingPackDecl(UsingPackDeclWalker D);
		bool VisitUsingShadowDecl(UsingShadowDeclWalker D);
		bool VisitVarDecl(VarDeclWalker D);
		bool VisitVarTemplateDecl(VarTemplateDeclWalker D);
		bool VisitVarTemplatePartialSpecializationDecl(VarTemplatePartialSpecializationDeclWalker D);
		bool VisitVarTemplateSpecializationDecl(VarTemplateSpecializationDeclWalker D);
	}
