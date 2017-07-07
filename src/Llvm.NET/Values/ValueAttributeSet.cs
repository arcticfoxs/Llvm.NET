using System.Collections.Generic;
using System.Collections;

namespace Llvm.NET.Values
{
    internal class ValueAttributeSet
        : IAttributeCollection
    {
        internal ValueAttributeSet( IValueWithAttributes container, FunctionAttributeIndex index )
        {
            Container = container;
            Index = index;
        }

        public FunctionAttributeIndex Index { get; }

        public Context Context => Container.Context;

        public AttributeValue this[ string name ] => Container.GetAttribute( Index, name );

        public AttributeValue this[ AttributeKind kind ] => Container.GetAttribute(Index, kind );

        public IEnumerator<AttributeValue> GetEnumerator( )
        {
            return Container.GetAttributes( Index ).GetEnumerator();
        }

        public void Add( AttributeValue item )
        {
            Container.Add( Index, item );
        }

        public void Add( AttributeKind kind )
        {
            Add( Container.Context.CreateAttribute( kind ) );
        }

        public void Add( AttributeKind kind, ulong value )
        {
            Add( Container.Context.CreateAttribute( kind, value ) );
        }

        public void Add( string name, string value )
        {
            Add( Container.Context.CreateAttribute( name, value ) );
        }

        public void Remove( AttributeValue item )
        {
            if( item.IsEnum )
            {
                Container.Remove( Index, item.Kind );
            }
            else
            {
                Container.Remove( Index, item.Name);
            }
        }

        public void Remove( AttributeKind kind )
        {
            Container.Remove( Index, kind );
        }

        public void Remove( string name )
        {
            Container.Remove( Index, name );
        }

        IEnumerator IEnumerable.GetEnumerator( )
        {
            return GetEnumerator( );
        }

        public bool Has( AttributeKind kind )
        {
            return null != Container.GetAttribute( Index, kind );
        }

        public bool Has( string name )
        {
            return null != Container.GetAttribute( Index, name );
        }

        private readonly IValueWithAttributes Container;
    }
}
