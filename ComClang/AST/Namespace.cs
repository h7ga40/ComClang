using System;
using System.Collections.Generic;
using System.Linq;

namespace CppSharp.AST
{
	/// <summary>
	/// Represents a declaration context.
	/// </summary>
	public abstract class DeclarationContext : Declaration, IDeclarationContext
	{
		public bool IsAnonymous { get; set; }

		public List<Declaration> Declarations;
		public List<TypeReference> TypeReferences;

		public DeclIterator<Namespace> Namespaces {
			get { return new DeclIterator<Namespace>(Declarations); }
		}

		public int NamespaceCount => Namespaces.Count;

		public DeclIterator<Enumeration> Enums {
			get { return new DeclIterator<Enumeration>(Declarations); }
		}

		public int EnumCount => Enums.Count;

		public DeclIterator<Function> Functions {
			get { return new DeclIterator<Function>(Declarations); }
		}

		public int FunctionCount => Functions.Count;

		public DeclIterator<Class> Classes {
			get { return new DeclIterator<Class>(Declarations); }
		}

		public int ClasseCount => Classes.Count;

		public DeclIterator<Template> Templates {
			get { return new DeclIterator<Template>(Declarations); }
		}

		public int TemplateCount => Templates.Count;

		public DeclIterator<TypedefNameDecl> Typedefs {
			get { return new DeclIterator<TypedefNameDecl>(Declarations); }
		}

		public int TypedefCount => Typedefs.Count;

		public DeclIterator<Variable> Variables {
			get { return new DeclIterator<Variable>(Declarations); }
		}

		public int VariableCount => Variables.Count;

		public DeclIterator<Event> Events {
			get { return new DeclIterator<Event>(Declarations); }
		}

		public int EventCount => Events.Count;

		// Used to keep track of anonymous declarations.
		public Dictionary<string, Declaration> Anonymous;

		public int AnonymousCount => Anonymous.Count;

		// True if the context is inside an extern "C" context.
		public bool IsExternCContext { get; set; }

		public override string LogicalName {
			get { return IsAnonymous ? "<anonymous>" : base.Name; }
		}

		public override string LogicalOriginalName {
			get { return IsAnonymous ? "<anonymous>" : base.OriginalName; }
		}

		protected DeclarationContext()
		{
			Declarations = new List<Declaration>();
			TypeReferences = new List<TypeReference>();
			Anonymous = new Dictionary<string, Declaration>();
		}

		protected DeclarationContext(DeclarationContext dc)
			: base(dc)
		{
			Declarations = dc.Declarations;
			TypeReferences = new List<TypeReference>(dc.TypeReferences);
			Anonymous = new Dictionary<string, Declaration>(dc.Anonymous);
			IsAnonymous = dc.IsAnonymous;
		}

		public IEnumerable<DeclarationContext> GatherParentNamespaces()
		{
			var children = new Stack<DeclarationContext>();
			var currentNamespace = this;

			while (currentNamespace != null)
			{
				if (!(currentNamespace is TranslationUnit))
					children.Push(currentNamespace);

				currentNamespace = currentNamespace.Namespace;
			}

			return children;
		}

		public Declaration FindAnonymous(string USR)
		{
			return Anonymous.ContainsKey(USR) ? Anonymous[USR] : null;
		}

		public DeclarationContext FindDeclaration(IEnumerable<string> declarations)
		{
			DeclarationContext currentDeclaration = this;

			foreach (var declaration in declarations)
			{
				var subDeclaration = currentDeclaration.Namespaces
					.Concat<DeclarationContext>(currentDeclaration.Classes)
					.FirstOrDefault(e => e.Name.Equals(declaration));

				if (subDeclaration == null)
					return null;

				currentDeclaration = subDeclaration;
			}

			return currentDeclaration as DeclarationContext;
		}

		public Namespace FindNamespace(string name)
		{
			var namespaces = name.Split(new string[] { "::" },
				StringSplitOptions.RemoveEmptyEntries);

			return FindNamespace(namespaces);
		}

		public Namespace FindNamespace(IEnumerable<string> namespaces)
		{
			DeclarationContext currentNamespace = this;

			foreach (var @namespace in namespaces)
			{
				var childNamespace = currentNamespace.Namespaces.Find(
					e => e.Name.Equals(@namespace));

				if (childNamespace == null)
					return null;

				currentNamespace = childNamespace;
			}

			return currentNamespace as Namespace;
		}

		public Namespace FindCreateNamespace(string name)
		{
			var @namespace = FindNamespace(name);

			if (@namespace == null)
			{
				@namespace = new Namespace
				{
					Name = name,
					Namespace = this,
				};

				Namespaces.Add(@namespace);
			}

			return @namespace;
		}

		public Enumeration FindEnum(string name, bool createDecl = false)
		{
			var entries = name.Split(new string[] { "::" },
				StringSplitOptions.RemoveEmptyEntries).ToList();

			if (entries.Count <= 1)
			{
				var @enum = Enums.Find(e => e.Name.Equals(name));

				if (@enum == null && createDecl)
				{
					@enum = new Enumeration() { Name = name, Namespace = this };
					Enums.Add(@enum);
				}

				return @enum;
			}

			var enumName = entries[entries.Count - 1];
			var namespaces = entries.Take(entries.Count - 1);

			var @namespace = FindNamespace(namespaces);
			if (@namespace == null)
				return null;

			return @namespace.FindEnum(enumName, createDecl);
		}

		public Enumeration FindEnum(IntPtr ptr)
		{
			return Enums.FirstOrDefault(f => f.OriginalPtr == ptr);
		}

		public Function FindFunction(string name, bool createDecl = false)
		{
			if (string.IsNullOrEmpty(name))
				return null;

			var entries = name.Split(new string[] { "::" },
				StringSplitOptions.RemoveEmptyEntries).ToList();

			if (entries.Count <= 1)
			{
				var function = Functions.Find(e => e.Name.Equals(name));

				if (function == null && createDecl)
				{
					function = new Function() { Name = name, Namespace = this };
					Functions.Add(function);
				}

				return function;
			}

			var funcName = entries[entries.Count - 1];
			var namespaces = entries.Take(entries.Count - 1);

			var @namespace = FindNamespace(namespaces);
			if (@namespace == null)
				return null;

			return @namespace.FindFunction(funcName, createDecl);
		}

		public Function FindFunctionByUSR(string usr)
		{
			return Functions
				.Concat(Templates.OfType<FunctionTemplate>()
					.Select(t => t.TemplatedFunction))
				.FirstOrDefault(f => f.USR == usr);
		}

		Class CreateClass(string name, bool isComplete)
		{
			var @class = new Class
			{
				Name = name,
				Namespace = this,
				IsIncomplete = !isComplete
			};

			return @class;
		}

		public Class FindClass(string name,
			StringComparison stringComparison = StringComparison.Ordinal)
		{
			if (string.IsNullOrEmpty(name)) return null;

			var entries = name.Split(new[] { "::" },
				StringSplitOptions.RemoveEmptyEntries).ToList();

			if (entries.Count <= 1)
			{
				var @class = Classes.Find(c => c.Name.Equals(name, stringComparison)) ??
							 Namespaces.Select(n => n.FindClass(name, stringComparison)).FirstOrDefault(c => c != null);
				if (@class != null)
					return @class.CompleteDeclaration == null ?
						@class : (Class)@class.CompleteDeclaration;
				return null;
			}

			var className = entries[entries.Count - 1];
			var namespaces = entries.Take(entries.Count - 1);

			DeclarationContext declContext = FindDeclaration(namespaces);
			if (declContext == null)
			{
				declContext = FindClass(entries[0]);
				if (declContext == null)
					return null;
			}

			return declContext.FindClass(className);
		}

		public Class FindClass(string name, bool isComplete,
			bool createDecl = false)
		{
			var @class = FindClass(name);

			if (@class == null)
			{
				if (createDecl)
				{
					@class = CreateClass(name, isComplete);
					Classes.Add(@class);
				}

				return @class;
			}

			if (@class.IsIncomplete == !isComplete)
				return @class;

			if (!createDecl)
				return null;

			var newClass = CreateClass(name, isComplete);

			// Replace the incomplete declaration with the complete one.
			if (@class.IsIncomplete)
			{
				@class.CompleteDeclaration = newClass;
				Classes.Replace(@class, newClass);
			}

			return newClass;
		}

		public FunctionTemplate FindFunctionTemplate(string name)
		{
			return Templates.OfType<FunctionTemplate>()
				.FirstOrDefault(t => t.Name == name);
		}

		public FunctionTemplate FindFunctionTemplateByUSR(string usr)
		{
			return Templates.OfType<FunctionTemplate>()
				.FirstOrDefault(t => t.USR == usr);
		}

		public IEnumerable<ClassTemplate> FindClassTemplate(string name)
		{
			foreach (var template in Templates.OfType<ClassTemplate>().Where(t => t.Name == name))
				yield return template;
			foreach (var @namespace in Namespaces)
				foreach (var template in @namespace.FindClassTemplate(name))
					yield return template;
		}

		public ClassTemplate FindClassTemplateByUSR(string usr)
		{
			return Templates.OfType<ClassTemplate>()
				.FirstOrDefault(t => t.USR == usr);
		}

		public TypedefNameDecl FindTypedef(string name, bool createDecl = false)
		{
			var entries = name.Split(new string[] { "::" },
				StringSplitOptions.RemoveEmptyEntries).ToList();

			if (entries.Count <= 1)
			{
				var typeDef = Typedefs.Find(e => e.Name.Equals(name));

				if (typeDef == null && createDecl)
				{
					typeDef = new TypedefDecl { Name = name, Namespace = this };
					Typedefs.Add(typeDef);
				}

				return typeDef;
			}

			var typeDefName = entries[entries.Count - 1];
			var namespaces = entries.Take(entries.Count - 1);

			var @namespace = FindNamespace(namespaces);
			if (@namespace == null)
				return null;

			return @namespace.FindTypedef(typeDefName, createDecl);
		}

		public T FindType<T>(string name) where T : Declaration
		{
			var type = FindEnum(name)
				?? FindFunction(name)
				?? (Declaration)FindClass(name)
				?? FindTypedef(name);

			return type as T;
		}

		public Enumeration FindEnumWithItem(string name)
		{
			return Enums.Find(e => e.ItemsByName.ContainsKey(name)) ??
				(from declContext in Namespaces.Union<DeclarationContext>(Classes)
				 let @enum = declContext.FindEnumWithItem(name)
				 where @enum != null
				 select @enum).FirstOrDefault();
		}

		public virtual IEnumerable<Function> FindOperator(CXXOperatorKind kind)
		{
			return Functions.Where(fn => fn.OperatorKind == kind);
		}

		public virtual IEnumerable<Function> GetOverloads(Function function)
		{
			if (function.IsOperator)
				return FindOperator(function.OperatorKind);

			return Functions.Where(fn => fn.Name == function.Name);
		}

		IDeclaration IDeclarationContext.FindAnonymous(string USR)
		{
			return FindAnonymous(USR);
		}

		INamespace IDeclarationContext.FindNamespace(string Name)
		{
			return FindNamespace(Name);
		}

		INamespace IDeclarationContext.FindNamespace(string[] names)
		{
			return FindNamespace(names);
		}

		INamespace IDeclarationContext.FindCreateNamespace(string Name)
		{
			return FindCreateNamespace(Name);
		}

		IClass IDeclarationContext.CreateClass(string Name, bool IsComplete)
		{
			return CreateClass(Name, IsComplete);
		}

		public IClass FindClass(IntPtr OriginalPtr, string Name, bool IsComplete)
		{
			if (String.IsNullOrEmpty(Name))
				return null;

			var entries = Name.Split(new[] { "::" }, StringSplitOptions.None);

			if (entries.Length == 1)
			{
				var _class = Classes.FirstOrDefault((klass) =>
				{
					return (OriginalPtr != IntPtr.Zero && klass.OriginalPtr == OriginalPtr) ||
						(klass.Name == Name && klass.IsIncomplete == !IsComplete);
				});

				return _class;
			}

			var className = entries[entries.Length - 1];

			var namespaces = new string[entries.Length - 1];
			for (int i = 0; i < namespaces.Length; i++)
			{
				namespaces[i] = entries[i];
			}

			var _namespace = FindNamespace(namespaces);
			if (_namespace == null)
				return null;

			return _namespace.FindClass(OriginalPtr, className, IsComplete);
		}

		public IClass FindClass(IntPtr OriginalPtr, string Name, bool IsComplete, bool Create)
		{
			var _class = (Class)FindClass(OriginalPtr, Name, IsComplete);

			if (_class == null)
			{
				if (Create)
				{
					_class = CreateClass(Name, IsComplete);
					Classes.Add(_class);
				}

				return _class;
			}

			return _class;
		}

		public ITemplate FindTemplate(string USR)
		{
			return Templates.FirstOrDefault(t => t.USR == USR);
		}

		IEnumeration IDeclarationContext.FindEnum(IntPtr OriginalPtr)
		{
			return Enums.FirstOrDefault(enumeration => enumeration.OriginalPtr == OriginalPtr);
		}

		IEnumeration IDeclarationContext.FindEnum(string Name, bool Create)
		{
			var entries = Name.Split(new[] { "::" }, StringSplitOptions.None);

			if (entries.Length == 1)
			{
				var foundEnum = Enums.FirstOrDefault((e) => e.Name == Name);

				if (foundEnum != null)
					return foundEnum;

				if (!Create)
					return null;

				var _enum = new Enumeration();
				_enum.Name = Name;
				_enum.Namespace = this;
				Enums.Add(_enum);
				return _enum;
			}

			var enumName = entries[entries.Length - 1];

			var namespaces = new string[entries.Length - 1];
			for (int i = 0; i < namespaces.Length; i++)
			{
				namespaces[i] = entries[i];
			}

			var _namespace = FindNamespace(namespaces);
			if (_namespace == null)
				return null;

			return _namespace.FindEnum(enumName, Create);
		}

		IEnumeration IDeclarationContext.FindEnumWithItem(string Name)
		{
			var foundEnumIt = Enums.FirstOrDefault(_enum => _enum.FindItemByName(Name) != null);
			if (foundEnumIt != null)
				return foundEnumIt;

			foreach (var it in Namespaces)
			{
				var foundEnum = it.FindEnumWithItem(Name);
				if (foundEnum != null)
					return foundEnum;
			}
			foreach (var it in Classes)
			{
				var foundEnum = it.FindEnumWithItem(Name);
				if (foundEnum != null)
					return foundEnum;
			}
			return null;
		}

		IFunction IDeclarationContext.FindFunction(string USR)
		{
			var foundFunction = Functions.FirstOrDefault(func => func.USR == USR);

			if (foundFunction != null)
				return foundFunction;

			var foundTemplate = Templates.FirstOrDefault((t) => t.TemplatedDecl != null && t.TemplatedDecl.USR == USR);
			if (foundTemplate != null)
				return (Function)foundTemplate.TemplatedDecl;

			return null;
		}

		ITypedefDecl IDeclarationContext.FindTypedef(string Name, bool Create)
		{
			var foundTypedef = Typedefs.FirstOrDefault(td => td.Name == Name);
			if (foundTypedef != null)
				return (ITypedefDecl)foundTypedef;

			if (!Create)
				return null;

			var tdef = new TypedefDecl();
			tdef.Name = Name;
			tdef.Namespace = this;

			return tdef;
		}

		public ITypeAlias FindTypeAlias(string Name, bool Create = false)
		{
			var foundTypeAlias = Typedefs.FirstOrDefault(t => t.Name == Name);
			if (foundTypeAlias != null)
				return (ITypeAlias)foundTypeAlias;

			if (!Create)
				return null;

			var talias = new TypeAlias();
			talias.Name = Name;
			talias.Namespace = this;

			return talias;
		}

		public IVariable FindVariable(string USR)
		{
			var found = Variables.FirstOrDefault(var => var.USR == USR);
			if (found != null)
				return found;

			return null;
		}

		public IFriend FindFriend(string USR)
		{
			var found = Declarations.FirstOrDefault(var => var.USR == USR);
			if (found != null)
				return (IFriend)found;

			return null;
		}

		public INamespace GetNamespace(int index)
		{
			return Namespaces[index];
		}

		public void AddNamespace(INamespace value)
		{
			Namespaces.Add((Namespace)value);
		}

		public IEnumeration GetEnum(int index)
		{
			return Enums[index];
		}

		public void AddEnum(IEnumeration value)
		{
			Enums.Add((Enumeration)value);
		}

		public IFunction GetFunction(int index)
		{
			return Functions[index];
		}

		public void AddFunction(IFunction value)
		{
			Functions.Add((Function)value);
		}

		public IClass GetClasse(int index)
		{
			return Classes[index];
		}

		public void AddClasse(IClass value)
		{
			Classes.Add((Class)value);
		}

		public ITemplate GetTemplate(int index)
		{
			return Templates[index];
		}

		public void AddTemplate(ITemplate value)
		{
			Templates.Add((Template)value);
		}

		public ITypedefDecl GetTypedef(int index)
		{
			return Typedefs[index] as ITypedefDecl;
		}

		public void AddTypedef(ITypedefDecl value)
		{
			Typedefs.Add((TypedefDecl)value);
		}

		public ITypeAlias GetTypeAliase(int index)
		{
			return Typedefs[index] as ITypeAlias;
		}

		public void AddTypeAliase(ITypeAlias value)
		{
			Typedefs.Add((TypeAlias)value);
		}

		public IVariable GetVariable(int index)
		{
			return Variables[index];
		}

		public void AddVariable(IVariable value)
		{
			Variables.Add((Variable)value);
		}

		public IFriend GetFriend(int index)
		{
			return (IFriend)Declarations[index];
		}

		public void AddFriend(IFriend value)
		{
			Declarations.Add((Friend)value);
		}

		public IDeclaration GetAnonymous(string index)
		{
			if (Anonymous.TryGetValue(USR, out var declaration))
				return declaration;
			return null;
		}

		public void AddAnonymous(string USR, IDeclaration declaration)
		{
			Anonymous.Add(USR, (Declaration)declaration);
		}

		public bool HasDeclarations {
			get {
				Func<Declaration, bool> pred = (t => t.IsGenerated);
				return Enums.Exists(pred) || HasFunctions || Typedefs.Exists(pred)
					|| Classes.Any() || Namespaces.Exists(n => n.HasDeclarations) ||
					Templates.Any(pred);
			}
		}

		public bool HasFunctions {
			get {
				Func<Declaration, bool> pred = (t => t.IsGenerated);
				return Functions.Exists(pred) || Namespaces.Exists(n => n.HasFunctions);
			}
		}

		public bool IsRoot { get { return Namespace == null; } }

		public int TypeAliaseCount => Typedefs.Count;

		public int FriendCount => Declarations.Count;
	}

	/// <summary>
	/// Represents a C++ namespace.
	/// </summary>
	public class Namespace : DeclarationContext, INamespace
	{
		public override string LogicalName {
			get { return IsInline ? string.Empty : base.Name; }
		}

		public override string LogicalOriginalName {
			get { return IsInline ? string.Empty : base.OriginalName; }
		}

		public bool IsInline { get; set; }

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitNamespace(this);
		}
	}
}
