using System;
using Llvm.NET.Native;

namespace Llvm.NET.Values
{
    internal class ArgumentAttributes
        : IAttributeSet
    {
        internal ArgumentAttributes( Function containingFunction, uint arg )
        {
            var index = FunctionAttributeIndex.Parameter0 + (int)arg;
            Attributes = new ValueAttributeSet( containingFunction, index );
        }

        public IAttributeCollection this[ FunctionAttributeIndex index ]
        {
            get
            {
                if( index != Attributes.Index )
                    throw new ArgumentOutOfRangeException(nameof( index ));

                return Attributes;
            }
        }

        private IAttributeCollection Attributes;
    }

    /// <summary>An LLVM Value representing an Argument to a function</summary>
    public class Argument
        : Value
        , IAttributeSetContainer
    {
        /// <summary>Function this argument belongs to</summary>
        public Function ContainingFunction => FromHandle<Function>( NativeMethods.GetParamParent( ValueHandle ) );

        /// <summary>Zero based index of the argument</summary>
        public uint Index => NativeMethods.GetArgumentIndex( ValueHandle );

        /// <summary>Sets the alignment for the argument</summary>
        /// <param name="value">Alignment value for this argument</param>
        public Argument SetAlignment( uint value )
        {
            ContainingFunction.AddAttribute( FunctionAttributeIndex.Parameter0 + ( int ) Index
                                           , Context.CreateAttribute(AttributeKind.Alignment, value )
                                           );
            return this;
        }

        public IAttributeSet Attributes { get; }

        internal Argument( LLVMValueRef valueRef )
            : base( valueRef )
        {
            Attributes = new ArgumentAttributes( ContainingFunction, Index );
        }
    }
}
