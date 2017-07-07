using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Llvm.NET.Values
{
    internal class UnboundAttributeCollection
        : IAttributeCollection
    {
        internal UnboundAttributeCollection( Context context, FunctionAttributeIndex index )
        {
            Context = context;
            Index = index;
        }

        public Context Context { get; }

        public FunctionAttributeIndex Index { get; }

        public AttributeValue this[ string name ]
        {
            get
            {
                if( string.IsNullOrEmpty( name ) )
                    return null;

                return Attributes.FirstOrDefault( a => a.Name == name );
            }
        }

        public AttributeValue this[ AttributeKind kind ] => Attributes.FirstOrDefault( a => a.IsEnum && a.Kind == kind );

        public void Add( AttributeValue value )
        {
            Attributes.Add( value );
        }

        public void Add( AttributeKind kind )
        {
            kind.VerifyAttributeUsage( Index );
            Add( Context.CreateAttribute( kind ) );
        }

        public void Add( AttributeKind kind, ulong value )
        {
            kind.VerifyAttributeUsage( Index );
            Add( Context.CreateAttribute( kind, value ) );
        }

        public void Add( string name, string value )
        {
            Add( Context.CreateAttribute( name, value ) );
        }

        public IEnumerator<AttributeValue> GetEnumerator( )
        {
            return Attributes.AsEnumerable( ).GetEnumerator( );
        }

        public void Remove( AttributeValue value )
        {
            Attributes.Remove( value );
        }

        public void Remove( AttributeKind kind )
        {
            Attributes.RemoveWhere( a => a.Kind == kind );
        }

        public void Remove( string name )
        {
            // don't allow removin null or empty string
            // for names
            if( string.IsNullOrEmpty( name ) )
                return;

            Attributes.RemoveWhere( a => a.Name == name );
        }

        IEnumerator IEnumerable.GetEnumerator( )
        {
            return GetEnumerator( );
        }

        public bool Has( AttributeKind kind ) => this[ kind ] != null;
        public bool Has( string name ) => this[ name ] != null;

        private HashSet<AttributeValue> Attributes = new HashSet<AttributeValue>();
    }

    public class AttributeSet
        : IAttributeSet
        , IAttributeSetContainer
    {
        public AttributeSet( Context context )
        {
            Context = context;
        }

        public Context Context { get; }

        public IAttributeSet Attributes => this;

        public IAttributeCollection this[ FunctionAttributeIndex index ]
        {
            get
            {
                if( AttributeSetMap.TryGetValue( index, out UnboundAttributeCollection retVal ) )
                    return retVal;

                retVal = new UnboundAttributeCollection( Context, index );
                AttributeSetMap.Add( index, retVal );
                return retVal;
            }
        }

        Dictionary<FunctionAttributeIndex, UnboundAttributeCollection> AttributeSetMap
            = new Dictionary<FunctionAttributeIndex, UnboundAttributeCollection>();
    }
}
