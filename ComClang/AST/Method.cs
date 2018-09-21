using System.Collections.Generic;
using System.Linq;
using CppSharp.AST.Extensions;

namespace CppSharp.AST
{
	public enum CXXOperatorArity
	{
		Unary,
		Binary
	}

	/// <summary>
	/// Represents a C++ record method declaration.
	/// </summary>
	public class Method : Function, IMethod
	{
		public Method()
		{
			Access = AccessSpecifier.Public;
		}

		public Method(Method method)
			: base(method)
		{
			Access = method.Access;
			IsVirtual = method.IsVirtual;
			IsConst = method.IsConst;
			IsFinal = method.IsFinal;
			IsProxy = method.IsProxy;
			IsStatic = method.IsStatic;
			MethodKind = method.MethodKind;
			IsDefaultConstructor = method.IsDefaultConstructor;
			IsCopyConstructor = method.IsCopyConstructor;
			IsMoveConstructor = method.IsMoveConstructor;
			Conversion = method.Conversion;
			SynthKind = method.SynthKind;
			AdjustedOffset = method.AdjustedOffset;
			OverriddenMethods.AddRange(method.OverriddenMethods);
			ConvertToProperty = method.ConvertToProperty;
		}

		public Method(Function function)
			: base(function)
		{

		}

		public bool IsVirtual { get; set; }
		public bool IsStatic { get; set; }
		public bool IsConst { get; set; }
		public bool IsExplicit { get; set; }

		public bool IsOverride {
			get { return isOverride ?? OverriddenMethods.Any(); }
			set { isOverride = value; }
		}

		public Method BaseMethod => OverriddenMethods.FirstOrDefault();

		// True if the method is final / sealed.
		public bool IsFinal { get; set; }

		public bool IsProxy { get; set; }

		public RefQualifier RefQualifier { get; set; }

		private CXXMethodKind kind;
		public CXXMethodKind MethodKind {
			get { return kind; }
			set {
				if (kind != value)
				{
					kind = value;
					if (kind == CXXMethodKind.Conversion)
						OperatorKind = CXXOperatorKind.Conversion;
				}
			}
		}

		public bool IsConstructor {
			get { return MethodKind == CXXMethodKind.Constructor; }
		}

		public bool IsDestructor {
			get { return MethodKind == CXXMethodKind.Destructor; }
		}

		public bool IsDefaultConstructor { get; set; }
		public bool IsCopyConstructor { get; set; }
		public bool IsMoveConstructor { get; set; }

		public MethodConversionKind Conversion { get; set; }

		public QualifiedType ConversionType { get; set; }

		IQualifiedType IMethod.ConversionType { get => ConversionType; set => ConversionType = (QualifiedType)value; }

		public Class ExplicitInterfaceImpl { get; set; }

		public int AdjustedOffset { get; set; }

		public List<Method> OverriddenMethods { get; } = new List<Method>();

		public int OverriddenMethodCount => OverriddenMethods.Count;

		public bool ConvertToProperty { get; set; }

		public Method GetRootBaseMethod()
		{
			return BaseMethod == null || BaseMethod.BaseMethod == null ?
				BaseMethod : BaseMethod.GetRootBaseMethod();
		}

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitMethodDecl(this);
		}

		public IMethod GetOverriddenMethod(int index)
		{
			return OverriddenMethods[index];
		}

		public void AddOverriddenMethod(IMethod value)
		{
			OverriddenMethods.Add((Method)value);
		}

		private bool? isOverride;
	}
}
