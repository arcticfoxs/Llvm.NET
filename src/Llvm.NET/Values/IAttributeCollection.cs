using System;
using System.Collections.Generic;

namespace Llvm.NET.Values
{
    /// <summary>Interface for manipulating attributes on a single index (dimension)</summary>
    public interface IAttributeCollection
        : IEnumerable<AttributeValue>
    {
        Context Context { get; }
        AttributeValue this[ AttributeKind kind ] { get; }
        AttributeValue this[ string name ] { get; }
        FunctionAttributeIndex Index { get; }

        bool Has( AttributeKind kind );
        bool Has( string name );
        void Add( AttributeValue value );
        void Add( AttributeKind kind );
        void Add( AttributeKind kind, UInt64 value );
        void Add( string name, string value );
        void Remove( AttributeValue value );
        void Remove( AttributeKind kind );
        void Remove( string name );
    }
}
