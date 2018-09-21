using System;

namespace CppSharp.AST
{
	/// <summary>
	/// Base class that describes a preprocessed entity, which may
	/// be a preprocessor directive or macro expansion.
	/// </summary>
	public abstract class PreprocessedEntity : IPreprocessedEntity
	{
		public MacroLocation MacroLocation { get; set; } = MacroLocation.Unknown;
		public IntPtr OriginalPtr { get; set; }
		public DeclarationKind Kind { get; set; }

		public abstract T Visit<T>(IDeclVisitor<T> visitor);
	}

	/// <summary>
	/// Represents a C preprocessor macro expansion.
	/// </summary>
	public class MacroExpansion : PreprocessedEntity, IMacroExpansion
	{
		public string Name { get; set; } = "";

		// Contains the macro expansion text.
		public string Text { get; set; } = "";

		IMacroDefinition IMacroExpansion.Definition { get => Definition; set => Definition = (MacroDefinition)value; }

		public MacroDefinition Definition;

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			//return visitor.VisitMacroExpansion(this);
			return default(T);
		}

		public override string ToString()
		{
			return Text;
		}
	}

	/// <summary>
	/// Represents a C preprocessor macro definition.
	/// </summary>
	public class MacroDefinition : PreprocessedEntity, IMacroDefinition
	{
		// Contains the macro definition text.
		public string Expression { get; set; } = "";

		// Backing enumeration if one was generated.
		public Enumeration Enumeration;

		public string Name { get; set; } = "";
		public int LineNumberStart { get; set; }
		public int LineNumberEnd { get; set; }

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitMacroDefinition(this);
		}

		public override string ToString()
		{
			return string.Format("{0} = {1}", Name, Expression);
		}
	}
}
