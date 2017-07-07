using Llvm.NET.Native;
using Llvm.NET.Values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Llvm.NET.Instructions
{
    public class Invoke
        : Terminator
        , IValueWithAttributes
    {
        public Function TargetFunction
        {
            get
            {
                if( Operands.Count < 1 )
                    return null;

                // last Operand of the instruction is the target function
                return Operands[ Operands.Count - 1 ] as Function;
            }
        }
        public IAttributeSet Attributes { get; }

        public void Add( FunctionAttributeIndex index, AttributeValue attrib )
        {
            if( attrib == null )
                throw new ArgumentNullException( nameof( attrib ) );

            attrib.VerifyValidOn( index, this );

            NativeMethods.AddCallSiteAttribute( ValueHandle, ( LLVMAttributeIndex )index, attrib.NativeAttribute );
        }

        public uint GetAttributeCount( FunctionAttributeIndex index )
        {
            return NativeMethods.GetCallSiteAttributeCount( ValueHandle, ( LLVMAttributeIndex )index );
        }

        public IEnumerable<AttributeValue> GetAttributes( FunctionAttributeIndex index )
        {
            var count = GetAttributeCount( index );
            var buffer = new LLVMAttributeRef[ count ];
            NativeMethods.GetCallSiteAttributes( ValueHandle, ( LLVMAttributeIndex )index, out buffer[ 0 ] );
            return from attribRef in buffer
                   select AttributeValue.FromHandle( Context, attribRef );
        }

        public AttributeValue GetAttribute( FunctionAttributeIndex index, AttributeKind kind )
        {
            var handle = NativeMethods.GetCallSiteEnumAttribute( ValueHandle, ( LLVMAttributeIndex )index, kind.GetEnumAttributeId( ) );
            return AttributeValue.FromHandle( Context, handle );
        }

        public AttributeValue GetAttribute( FunctionAttributeIndex index, string name )
        {
            var handle = NativeMethods.GetCallSiteStringAttribute( ValueHandle, ( LLVMAttributeIndex )index, name, ( uint )name.Length );
            return AttributeValue.FromHandle( Context, handle );
        }

        public void Remove( FunctionAttributeIndex index, AttributeKind kind )
        {
            NativeMethods.RemoveCallSiteEnumAttribute( ValueHandle, ( LLVMAttributeIndex )index, kind.GetEnumAttributeId( ) );
        }

        public void Remove( FunctionAttributeIndex index, string name )
        {
            NativeMethods.RemoveCallSiteStringAttribute( ValueHandle, ( LLVMAttributeIndex )index, name, ( uint )name.Length );
        }

        internal Invoke( LLVMValueRef valueRef )
            : base( valueRef )
        {
            Attributes = new ValueAttributes( this );
        }
    }
}
