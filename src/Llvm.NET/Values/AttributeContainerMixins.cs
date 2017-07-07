using System;
using System.Collections.Generic;
using System.Linq;

namespace Llvm.NET.Values
{
    public static class AttributeContainerMixins
    {
        public static T AddAttributes<T>( this T self, FunctionAttributeIndex index, params AttributeKind[ ] values )
            where T : IAttributeSetContainer
        {
            if( values == null )
                throw new ArgumentNullException( nameof( values ) );

            var set = self.Attributes[ index ];
            foreach( var kind in values )
                set.Add( kind );

            return self;
        }

        public static T AddAttribute<T>( this T self, FunctionAttributeIndex index, AttributeKind value )
            where T : IAttributeSetContainer
        {
            var set = self.Attributes[ index ];
            set.Add( value );
            return self;
        }

        public static T AddAttribute<T>( this T self, FunctionAttributeIndex index, AttributeValue value )
            where T : IAttributeSetContainer
        {
            var set = self.Attributes[ index ];
            set.Add( value );
            return self;
        }

        public static T AddAttributes<T>( this T self, FunctionAttributeIndex index, params AttributeValue[ ] attributes )
            where T : IAttributeSetContainer
        {
            var set = self.Attributes[ index ];

            foreach( var attrib in attributes )
                set.Add( attrib );

            return self;
        }

        public static T AddAttributes<T>( this T self, FunctionAttributeIndex index, IAttributeCollection attributes )
            where T : IAttributeSetContainer
        {
            var set = self.Attributes[ index ];
            foreach( var attrib in attributes )
                set.Add( attrib );
            return self;
        }

        public static T AddAttributes<T>( this T self, FunctionAttributeIndex index, IAttributeSet attributes )
            where T : IAttributeSetContainer
        {
            return AddAttributes( self, index, attributes[ index ] );
        }

        public static T AddAttributes<T>( this T self, FunctionAttributeIndex index, IEnumerable<AttributeValue> attributes )
            where T : IAttributeSetContainer
        {
            var set = self.Attributes[ index ];

            foreach( var attrib in attributes )
                set.Add( attrib );

            return self;
        }

        public static T RemoveAttribute<T>( this T self, FunctionAttributeIndex index, AttributeKind kind )
            where T : IAttributeSetContainer
        {
            var set = self.Attributes[ index ];
            set.Remove( kind );
            return self;
        }

        public static T RemoveAttribute<T>( this T self, FunctionAttributeIndex index, string name )
            where T : IAttributeSetContainer
        {
            var set = self.Attributes[ index ];
            set.Remove( name );
            return self;
        }

        public static IAttributeSet Add( this IAttributeSet self, FunctionAttributeIndex index, params AttributeKind[ ] values )
        {
            if( values == null )
                throw new ArgumentNullException( nameof( values ) );

            var set = self[ index ];
            foreach( var kind in values )
                set.Add( kind );

            return self;
        }

        public static IAttributeSet Add( this IAttributeSet self, FunctionAttributeIndex index, AttributeKind value )
        {
            var set = self[ index ];
            set.Add( value );
            return self;
        }

        public static IAttributeSet Add( this IAttributeSet self, FunctionAttributeIndex index, AttributeValue value )
        {
            var set = self[ index ];
            set.Add( value );
            return self;
        }

        public static IAttributeSet Add( this IAttributeSet self, FunctionAttributeIndex index, params AttributeValue[ ] attributes )
        {
            var set = self[ index ];

            foreach( var attrib in attributes )
                set.Add( attrib );

            return self;
        }

        public static IAttributeSet Add( this IAttributeSet self, FunctionAttributeIndex index, IAttributeCollection attributes )
        {
            var set = self[ index ];
            foreach( var attrib in attributes )
                set.Add( attrib );

            return self;
        }

        public static IAttributeSet Add( this IAttributeSet self, FunctionAttributeIndex index, IEnumerable<AttributeValue> attributes )
        {
            var set = self[ index ];

            foreach( var attrib in attributes )
                set.Add( attrib );

            return self;
        }

        public static IAttributeSet Remove( this IAttributeSet self, FunctionAttributeIndex index, AttributeKind kind )
        {
            var set = self[ index ];
            set.Remove( kind );
            return self;
        }

        public static IAttributeSet Remove( this IAttributeSet self, FunctionAttributeIndex index, string name )
        {
            var set = self[ index ];
            set.Remove( name );
            return self;
        }

        public static IAttributeCollection ParameterAttributes( this IAttributeSet self, int parameterIndex )
        {
            // prevent overflow on offset addition below
            if( parameterIndex > int.MaxValue - ( int )FunctionAttributeIndex.Parameter0 )
                throw new ArgumentOutOfRangeException( nameof( parameterIndex ) );

            var index = FunctionAttributeIndex.Parameter0 + parameterIndex;
            return self[ index ];
        }

        public static bool Has(this IAttributeSet self, FunctionAttributeIndex index, AttributeKind kind )
        {
            return self[ index ].Has( kind );
        }

        public static bool Has( this IAttributeSet self, FunctionAttributeIndex index, string name )
        {
            return self[ index ].Has( name );
        }

        public static bool HasAny( this IAttributeSet self, FunctionAttributeIndex index )
        {
            return self[ index ].Any( );
        }

        public static UInt64 GetAttributeValue( this IAttributeSet self, FunctionAttributeIndex index, AttributeKind kind )
        {
            var value = self[ index ][ kind ].IntegerValue;
            if( !value.HasValue )
            {
                throw new ArgumentException( "Attribute on specified index does not have an integral value", nameof(kind) );
            }
            return value.Value;
        }

        public static string AsString( this IAttributeCollection self )
        {
            return string.Join( " ", self.Select( a => a.ToString( ) ) );
        }

        public static string AsString( this IAttributeSet self, FunctionAttributeIndex index )
        {
            return self[ index ].AsString( );
        }
    }
}