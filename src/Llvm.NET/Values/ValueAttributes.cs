using System.Collections.Generic;

namespace Llvm.NET.Values
{
    internal class ValueAttributes
        : IAttributeSet
    {
        internal ValueAttributes( IValueWithAttributes container )
        {
            Container = container;
        }

        public IAttributeCollection this[ FunctionAttributeIndex index ]
        {
            get
            {
                if( AttributeSetMap.TryGetValue( index, out IAttributeCollection retVal ) )
                    return retVal;

                retVal = new ValueAttributeSet( Container, index );
                AttributeSetMap.Add( index, retVal );
                return retVal;
            }
        }

        private Dictionary<FunctionAttributeIndex, IAttributeCollection> AttributeSetMap = new Dictionary<FunctionAttributeIndex, IAttributeCollection>( );
        private IValueWithAttributes Container;
    }
}
