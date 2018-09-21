﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CppSharp.AST
{
	public class TemplateTemplateParameter : Template, ITemplateTemplateParameter
	{
		/// <summary>
		/// Whether this template template parameter is a template parameter pack.
		/// <para>template&lt;template&lt;class T&gt; ...MetaFunctions&gt; struct Apply;</para>
		/// </summary>
		public bool IsParameterPack { get; set; }

		/// <summary>
		/// Whether this parameter pack is a pack expansion.
		/// <para>A template template parameter pack is a pack expansion if its template parameter list contains an unexpanded parameter pack.</para>
		/// </summary>
		public bool IsPackExpansion { get; set; }

		/// <summary>
		/// Whether this parameter is a template template parameter pack that has a known list of different template parameter lists at different positions.
		/// A parameter pack is an expanded parameter pack when the original parameter pack's template parameter list was itself a pack expansion, and that expansion has already been expanded. For exampe, given:
		/// <para>
		/// template&lt;typename...Types&gt; struct Outer { template&lt;template&lt;Types&gt; class...Templates> struct Inner; };
		/// </para>
		/// The parameter pack Templates is a pack expansion, which expands the pack Types.When Types is supplied with template arguments by instantiating Outer, the instantiation of Templates is an expanded parameter pack.
		/// </summary>
		public bool IsExpandedParameterPack { get; set; }

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitTemplateTemplateParameterDecl(this);
		}
	}

	/// <summary>
	/// Represents a template parameter
	/// </summary>
	public class TypeTemplateParameter : Declaration, ITypeTemplateParameter
	{
		/// <summary>
		/// Get the nesting depth of the template parameter.
		/// </summary>
		public uint Depth { get; set; }

		/// <summary>
		/// Get the index of the template parameter within its parameter list.
		/// </summary>
		public uint Index { get; set; }

		/// <summary>
		/// Whether this parameter is a non-type template parameter pack.
		/// <para>
		/// If the parameter is a parameter pack, the type may be a PackExpansionType.In the following example, the Dims parameter is a parameter pack (whose type is 'unsigned').
		/// <para>template&lt;typename T, unsigned...Dims&gt; struct multi_array;</para>
		/// </para>
		/// </summary>
		public bool IsParameterPack { get; set; }

		// Generic type constraint
		public string Constraint;

		public QualifiedType DefaultArgument { get; set; }

		IQualifiedType ITypeTemplateParameter.DefaultArgument { get => DefaultArgument; set => DefaultArgument = (QualifiedType)value; }

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitTemplateParameterDecl(this);
		}
	}

	/// <summary>
	/// Represents a hard-coded template parameter
	/// </summary>
	public class NonTypeTemplateParameter : Declaration, INonTypeTemplateParameter
	{
		/// <summary>
		/// Get the nesting depth of the template parameter.
		/// </summary>
		public uint Depth { get; set; }

		/// <summary>
		/// Get the index of the template parameter within its parameter list.
		/// </summary>
		public uint Index { get; set; }

		/// <summary>
		/// Whether this parameter is a non-type template parameter pack.
		/// <para>
		/// If the parameter is a parameter pack, the type may be a PackExpansionType.In the following example, the Dims parameter is a parameter pack (whose type is 'unsigned').
		/// <para>template&lt;typename T, unsigned...Dims&gt; struct multi_array;</para>
		/// </para>
		/// </summary>
		public bool IsParameterPack { get; set; }

		public Expression DefaultArgument { get; set; }

		IExpression INonTypeTemplateParameter.DefaultArgument { get => DefaultArgument; set => DefaultArgument = (Expression)value; }

		/// <summary>
		/// Get the position of the template parameter within its parameter list.
		/// </summary>
		public uint Position { get; set; }

		/// <summary>
		/// Whether this parameter pack is a pack expansion.
		/// <para>
		/// A non-type template parameter pack is a pack expansion if its type contains an unexpanded parameter pack.In this case, we will have built a PackExpansionType wrapping the type.
		/// </para>
		/// </summary>
		public bool IsPackExpansion { get; set; }

		/// <summary>
		/// Whether this parameter is a non-type template parameter pack that has a known list of different types at different positions.
		/// <para>A parameter pack is an expanded parameter pack when the original parameter pack's type was itself a pack expansion, and that expansion has already been expanded. For example, given:</para>
		/// <para>
		/// template&lt;typename...Types&gt;
		/// struct X {
		///   template&lt;Types...Values&gt;
		///   struct Y { /* ... */ };
		/// };
		/// </para>
		/// The parameter pack Values has a PackExpansionType as its type, which expands Types.When Types is supplied with template arguments by instantiating X,
		/// the instantiation of Values becomes an expanded parameter pack.For example, instantiating X&lt;int, unsigned int&gt;
		/// results in Values being an expanded parameter pack with expansion types int and unsigned int.
		/// </summary>
		public bool IsExpandedParameterPack { get; set; }

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitNonTypeTemplateParameterDecl(this);
		}
	}

	/// <summary>
	/// The base class of all kinds of template declarations
	/// (e.g., class, function, etc.).
	/// </summary>
	public abstract class Template : Declaration, ITemplate
	{
		// Name of the declaration.
		public override string Name {
			get {
				if (TemplatedDecl != null)
					return TemplatedDecl.Name;
				return base.Name;
			}
			set {
				base.Name = value;
				if (TemplatedDecl != null)
					TemplatedDecl.Name = value;
			}
		}

		IDeclaration ITemplate.TemplatedDecl { get => TemplatedDecl; set => TemplatedDecl = (Declaration)value; }

		public int ParameterCount => Parameters.Count;

		protected Template()
		{
			Parameters = new List<Declaration>();
		}

		protected Template(Declaration decl)
		{
			TemplatedDecl = decl;
			Parameters = new List<Declaration>();
		}

		public Declaration TemplatedDecl;

		public List<Declaration> Parameters;

		public override string ToString()
		{
			return TemplatedDecl.ToString();
		}

		public IDeclaration GetParameter(int index)
		{
			return Parameters[index];
		}

		public void AddParameter(IDeclaration value)
		{
			Parameters.Add((Declaration)value);
		}
	}

	/// <summary>
	/// Declaration of a type alias template.
	/// </summary>
	public class TypeAliasTemplate : Template, ITypeAliasTemplate
	{
		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitTypeAliasTemplateDecl(this);
		}
	}

	/// <summary>
	/// Declaration of a class template.
	/// </summary>
	public class ClassTemplate : Template, IClassTemplate
	{
		public List<ClassTemplateSpecialization> Specializations;

		public Class TemplatedClass {
			get { return TemplatedDecl as Class; }
		}

		public ClassTemplate()
		{
			Specializations = new List<ClassTemplateSpecialization>();
		}

		public ClassTemplate(Declaration decl)
			: base(decl)
		{
			Specializations = new List<ClassTemplateSpecialization>();
		}

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitClassTemplateDecl(this);
		}

		public override string Name {
			get {
				if (TemplatedDecl != null)
					return TemplatedClass.Name;
				return base.Name;
			}
			set {
				if (TemplatedDecl != null)
					TemplatedClass.Name = value;
				else
					base.Name = value;
			}
		}

		public override string OriginalName {
			get {
				if (TemplatedDecl != null)
					return TemplatedClass.OriginalName;
				return base.OriginalName;
			}
			set {
				if (TemplatedDecl != null)
					TemplatedClass.OriginalName = value;
				else
					base.OriginalName = value;
			}
		}

		public int SpecializationCount => Specializations.Count;

		public ClassTemplateSpecialization FindSpecializationByUSR(string usr)
		{
			return Specializations.FirstOrDefault(spec => spec.USR == usr);
		}

		public ClassTemplatePartialSpecialization FindPartialSpecializationByUSR(string usr)
		{
			return FindSpecializationByUSR(usr) as ClassTemplatePartialSpecialization;
		}

		public IClassTemplateSpecialization GetSpecialization(int index)
		{
			return Specializations[index];
		}

		public void AddSpecialization(IClassTemplateSpecialization value)
		{
			Specializations.Add((ClassTemplateSpecialization)value);
		}

		public IClassTemplateSpecialization FindSpecialization(string usr)
		{
			return FindSpecializationByUSR(usr);
		}

		public IClassTemplatePartialSpecialization FindPartialSpecialization(string usr)
		{
			return FindPartialSpecializationByUSR(usr);
		}
	}

	/// <summary>
	/// Represents a class template specialization, which refers to a class
	/// template with a given set of template arguments.
	/// </summary>
	public class ClassTemplateSpecialization : Class, IClassTemplateSpecialization
	{
		public ClassTemplate TemplatedDecl;

		public List<TemplateArgument> Arguments;

		public TemplateSpecializationKind SpecializationKind { get; set; }

		IClassTemplate IClassTemplateSpecialization.TemplatedDecl { get => TemplatedDecl; set => TemplatedDecl = (ClassTemplate)value; }

		public int ArgumentsCount => Arguments.Count;

		public ClassTemplateSpecialization()
		{
			Arguments = new List<TemplateArgument>();
		}

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitClassTemplateSpecializationDecl(this);
		}

		public override string ToString()
		{
			var args = string.Join(", ", Arguments.Select(a => a.ToString()));
			return string.Format("{0}<{1}> [{2}]", OriginalName, args, SpecializationKind);
		}

		public ITemplateArgument GetArguments(int index)
		{
			return Arguments[index];
		}

		public void AddArguments(ITemplateArgument value)
		{
			Arguments.Add((TemplateArgument)value);
		}
	}

	/// <summary>
	/// Represents a class template partial specialization, which refers to
	/// a class template with a given partial set of template arguments.
	/// </summary>
	public class ClassTemplatePartialSpecialization : ClassTemplateSpecialization, IClassTemplatePartialSpecialization
	{
	}

	/// <summary>
	/// Declaration of a template function.
	/// </summary>
	public class FunctionTemplate : Template, IFunctionTemplate
	{
		public List<FunctionTemplateSpecialization> Specializations;

		public FunctionTemplate()
		{
			Specializations = new List<FunctionTemplateSpecialization>();
		}

		public FunctionTemplate(Declaration decl)
			: base(decl)
		{
			Specializations = new List<FunctionTemplateSpecialization>();
		}

		public Function TemplatedFunction {
			get { return TemplatedDecl as Function; }
		}

		public int SpecializationCount => Specializations.Count;

		public void AddSpecialization(IFunctionTemplateSpecialization value)
		{
			Specializations.Add((FunctionTemplateSpecialization)value);
		}

		public IFunctionTemplateSpecialization FindSpecialization(string usr)
		{
			return Specializations.FirstOrDefault(spec => spec.SpecializedFunction.USR == usr);
		}

		public IFunctionTemplateSpecialization GetSpecialization(int index)
		{
			return Specializations[index];
		}

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitFunctionTemplateDecl(this);
		}
	}

	/// <summary>
	/// Represents a function template specialization, which refers to a function
	/// template with a given set of template arguments.
	/// </summary>
	public class FunctionTemplateSpecialization : IFunctionTemplateSpecialization
	{
		public FunctionTemplate Template;

		IFunctionTemplate IFunctionTemplateSpecialization.Template { get => Template; set => Template = (FunctionTemplate)value; }

		public List<TemplateArgument> Arguments;

		public Function SpecializedFunction;

		IFunction IFunctionTemplateSpecialization.SpecializedFunction { get => SpecializedFunction; set => SpecializedFunction = (Function)value; }

		public TemplateSpecializationKind SpecializationKind { get; set; }

		public int ArgumentsCount => Arguments.Count;

		public FunctionTemplateSpecialization()
		{
			Arguments = new List<TemplateArgument>();
		}

		public FunctionTemplateSpecialization(FunctionTemplateSpecialization fts)
		{
			Template = fts.Template;
			Arguments = new List<TemplateArgument>();
			Arguments.AddRange(fts.Arguments);
			SpecializedFunction = fts.SpecializedFunction;
			SpecializationKind = fts.SpecializationKind;
		}

		public T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitFunctionTemplateSpecializationDecl(this);
		}

		public ITemplateArgument GetArguments(int index)
		{
			return Arguments[index];
		}

		public void AddArguments(ITemplateArgument value)
		{
			Arguments.Add((TemplateArgument)value);
		}
	}

	/// <summary>
	/// Represents a declaration of a variable template.
	/// </summary>
	public class VarTemplate : Template, IVarTemplate
	{
		public List<VarTemplateSpecialization> Specializations;

		public Variable TemplatedVariable {
			get { return TemplatedDecl as Variable; }
		}

		public int SpecializationCount => Specializations.Count;

		public VarTemplate()
		{
			Specializations = new List<VarTemplateSpecialization>();
		}

		public VarTemplate(Variable var) : base(var)
		{
			Specializations = new List<VarTemplateSpecialization>();
		}

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitVarTemplateDecl(this);
		}

		public IVarTemplateSpecialization FindSpecialization(string usr)
		{
			return Specializations.FirstOrDefault(cts => cts.USR == usr);
		}

		public IVarTemplateSpecialization GetSpecialization(int index)
		{
			return Specializations[index];
		}

		public void AddSpecialization(IVarTemplateSpecialization value)
		{
			Specializations.Add((VarTemplateSpecialization)value);
		}

		public IVarTemplatePartialSpecialization FindPartialSpecialization(string usr)
		{
			return (IVarTemplatePartialSpecialization)FindSpecialization(usr);
		}
	}

	/// <summary>
	/// Represents a var template specialization, which refers to a var
	/// template with a given set of template arguments.
	/// </summary>
	public class VarTemplateSpecialization : Variable, IVarTemplateSpecialization
	{
		public VarTemplate TemplatedDecl;

		public List<TemplateArgument> Arguments;

		public TemplateSpecializationKind SpecializationKind { get; set; }

		IVarTemplate IVarTemplateSpecialization.TemplatedDecl { get => TemplatedDecl; set => TemplatedDecl = (VarTemplate)value; }

		public int ArgumentsCount => Arguments.Count;

		public VarTemplateSpecialization()
		{
			Arguments = new List<TemplateArgument>();
		}

		public ITemplateArgument GetArguments(int index)
		{
			return Arguments[index];
		}

		public void AddArguments(ITemplateArgument value)
		{
			Arguments.Add((TemplateArgument)value);
		}
	}

	/// <summary>
	/// Represents a variable template partial specialization, which refers to
	/// a variable template with a given partial set of template arguments.
	/// </summary>
	public class VarTemplatePartialSpecialization : VarTemplateSpecialization, IVarTemplatePartialSpecialization
	{
	}
}
