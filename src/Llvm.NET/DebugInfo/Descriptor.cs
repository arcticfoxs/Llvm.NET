﻿namespace Llvm.NET.DebugInfo
{
    // for now the DebugInfo hierarchy is mostly empty
    // classes. This is due to the "in transition" state
    // of the underlying LLVM C++ model. All of these
    // are just a wrapper around a Metadata* allocated
    // in the LLVM native libraries. The only properties
    // or methods exposed are those required by current
    // projects. This keeps the code churn to move into
    // 3.7 minimal while allowing us to achieve progress
    // on current projects.

    /// <summary>Array of <see cref="Type"/> nodes for use with <see cref="DebugInfoBuilder"/> methods</summary>
    public class TypeArray
    {
        internal TypeArray( LLVMMetadataRef handle )
        {
            MetadataHandle = handle;
        }

        internal LLVMMetadataRef MetadataHandle { get; }
    }

    /// <summary>Array of <see cref="Descriptor"/> nodes for use with <see cref="DebugInfoBuilder"/> methods</summary>
    public class Array
    {
        internal Array( LLVMMetadataRef handle )
        {
            MetadataHandle = handle;
        }

        internal LLVMMetadataRef MetadataHandle { get; }
    }

    /// <summary>Root of the object hierarchy for Debug information metadata nodes</summary>
    /// <remarks>
    /// A descriptor is just a wraper around a Metadata Node (MDNode in LLVM C++) and, unlike
    /// <see cref="Values.Value"/> they are not uniqued, although the underlying metadata nodes
    /// usually are. 
    /// </remarks>
    public class Descriptor
    {
        public Tag Tag => (Tag)LLVMNative.DIDescriptorGetTag( MetadataHandle );

        internal Descriptor( LLVMMetadataRef handle )
        {
            MetadataHandle = handle;
        }

        internal LLVMMetadataRef MetadataHandle { get; }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#diglobalvariable"/></summary>
    public class GlobalVariable : Descriptor
    {
        internal GlobalVariable( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#dilocalvariable"/></summary>
    public class LocalVariable : Descriptor
    {
        internal LocalVariable( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#dienumerator"/></summary>
    public class Enumerator : Descriptor
    {
        internal Enumerator( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#disubrange"/></summary>
    public class Subrange : Descriptor
    {
        internal Subrange( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary>Base class for all Debug info scopes</summary>
    public class Scope : Descriptor
    {
        internal Scope( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#dicompileunit"/></summary>
    public class CompileUnit : Scope
    {
        internal CompileUnit( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#difile"/></summary>
    public class File : Scope
    {
        internal File( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#dilexicalblock"/></summary>
    public class LexicalBlock : Scope
    {
        internal LexicalBlock( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#dilexicalblockfile"/></summary>
    public class LexicalBlockFile : Scope
    {
        internal LexicalBlockFile( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#dinamespace"/></summary>
    public class Namespace : Scope
    {
        internal Namespace( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#disubprogram"/></summary>
    public class SubProgram : Scope
    {
        internal SubProgram( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary>Base class for Debug info types</summary>
    public class Type : Scope
    {
        internal Type( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#dibasictype"/></summary>
    public class BasicType : Type
    {
        internal BasicType( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#diderivedtype"/></summary>
    public class DerivedType : Type
    {
        internal DerivedType( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#dicompositetype"/></summary>
    public class CompositeType : Type
    {
        internal CompositeType( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }

    /// <summary><see cref="http://llvm.org/docs/LangRef.html#disubroutinetype"/></summary>
    public class SubroutineType : CompositeType
    {
        internal SubroutineType( LLVMMetadataRef handle )
            : base( handle )
        {
        }
    }
}