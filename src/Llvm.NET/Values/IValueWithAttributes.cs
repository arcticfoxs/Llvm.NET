using System.Collections.Generic;

namespace Llvm.NET.Values
{
    // As of v3.9x and later Functions and call sites use distinct LLVM-C API sets for
    // manipulating attributes. Fortunately they have consistent signatures so these
    // are used to abstract the difference via derived types specialized for each case.
    internal interface IValueWithAttributes
        : IAttributeSetContainer
    {
        Context Context { get; }

        uint GetAttributeCount( FunctionAttributeIndex index );
        IEnumerable<AttributeValue> GetAttributes( FunctionAttributeIndex index );
        AttributeValue GetAttribute( FunctionAttributeIndex index, AttributeKind kind );
        AttributeValue GetAttribute( FunctionAttributeIndex index, string name );
        void Add( FunctionAttributeIndex index, AttributeValue attrib );
        void Remove( FunctionAttributeIndex index, AttributeKind kind );
        void Remove( FunctionAttributeIndex index, string name );
    }
}
