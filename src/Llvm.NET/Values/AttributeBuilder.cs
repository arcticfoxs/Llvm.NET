#if NO_LONGER_NECESSARY
using System;
using System.Collections.Generic;

namespace Llvm.NET.Values
{
    /// <summary>Factory for building AttributeSets, which are otherwise immutable</summary>
    /// <remarks>
    /// <para>This class is used to build immutable <see cref="AttributeSet"/> instances.</para>
    /// <para>
    /// Manipulator methods of this class return the instance of the class. This allows use
    /// in fluent style coding scenarios. (see: https://en.wikipedia.org/wiki/Fluent_interface )
    /// </para>
    /// <note type="note">
    /// It is important to keep in mind that an AttributeBuilder is dimensionless, that is,
    /// AttributeBuilder does not contain or store a <see cref="FunctionAttributeIndex"/> value.
    /// The index is applied only when creating the <see cref="AttributeSet"/> in
    /// <see cref="ToAttributeSet(FunctionAttributeIndex,Context)"/>
    /// </note>
    /// </remarks>
    public sealed class AttributeBuilder
    {
        /// <summary>Creates a new <see cref="AttributeBuilder"/> with a given <see cref="AttributeValue"/></summary>
        /// <param name="context">Context that will own he attributes created by this builder</param>
        /// <param name="value"><see cref="AttributeValue"/> to add to the builder after creating it</param>
        public AttributeBuilder( Context context, AttributeValue value )
        {
            Context = context;
            Attributes.Add( value );
        }

        /// <summary>Creates a new <see cref="AttributeBuilder"/> from a single index of an existing <see cref="AttributeSet"/></summary>
        /// <param name="attributes"><see cref="AttributeSet"/> to initialize the builder from</param>
        /// <param name="index"><see cref="FunctionAttributeIndex"/> to take from <paramref name="attributes"/></param>
        public AttributeBuilder( AttributeSet attributes, FunctionAttributeIndex index )
        {
            if( attributes == null )
                throw new ArgumentNullException( nameof( attributes ) );

            Context = attributes.Context;
            foreach( var attrib in attributes.AllAttributes )
            {
                Attributes.Add( attrib.Value );
            }
        }

        public Context Context { get; }

        /// <summary>Indicates if this Builder contains no attributes</summary>
        public bool IsEmpty => Attributes.Count == 0;

        /// <summary>Adds a new boolean attribute to this builder</summary>
        /// <param name="kind">Kind of attribute to add</param>
        /// <returns>This builder for fluent style programming</returns>
        public AttributeBuilder Add( AttributeKind kind )
        {
            if( kind.RequiresIntValue( ) )
                throw new ArgumentException( "Attribute requires a value" );

            throw new NotImplementedException( );
            //NativeMethods.AttributeBuilderAddEnum( BuilderHandle, ( LLVMAttrKind )kind );
            //return this;
        }

        /// <summary>Adds an <see cref="AttributeValue"/> to a builder</summary>
        /// <param name="value">Value to add to this builder</param>
        /// <returns>This builder for fluent style programming</returns>
        public AttributeBuilder Add( AttributeValue value )
        {
            //if( BuilderHandle.IsClosed )
            //    throw new ObjectDisposedException( nameof( AttributeBuilder ) );

            //NativeMethods.AttributeBuilderAddAttribute( BuilderHandle, value.NativeAttribute );
            //return this;
            throw new NotImplementedException( );
        }

        /// <summary>Adds a target dependent string attribute to a builder</summary>
        /// <param name="name">Name of the attribute</param>
        /// <returns>This builder for fluent style programming</returns>
        public AttributeBuilder Add( string name )
        {
            //if( BuilderHandle.IsClosed )
            //    throw new ObjectDisposedException( nameof( AttributeBuilder ) );

            return Add( name, string.Empty );
        }

        /// <summary>Adds a target dependent attribute to the builder</summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Value of the attribute</param>
        /// <returns>This builder for fluent style programming</returns>
        public AttributeBuilder Add( string name, string value )
        {
            //if( BuilderHandle.IsClosed )
            //    throw new ObjectDisposedException( nameof( AttributeBuilder ) );

            //NativeMethods.AttributeBuilderAddStringAttribute( BuilderHandle, name, value );
            //return this;
            throw new NotImplementedException( );
        }

        ///// <summary>Removes a boolean attribute from the builder</summary>
        ///// <param name="kind">Kind of attribute to remove</param>
        ///// <returns>This builder for fluent style programming</returns>
        //public AttributeBuilder Remove( AttributeKind kind )
        //{
        //    if( BuilderHandle.IsClosed )
        //        throw new ObjectDisposedException( nameof( AttributeBuilder ) );

        //    //NativeMethods.AttributeBuilderRemoveEnum( BuilderHandle, ( LLVMAttrKind )kind );
        //    //return this;
        //    throw new NotImplementedException( );
        //}

        /// <summary>Removes attributes specified in an <see cref="AttributeSet"/></summary>
        /// <param name="attributes">Attributes to remove</param>
        /// <param name="index">Index of attributes in <paramref name="attributes"/> to remove</param>
        /// <returns></returns>
        public AttributeBuilder Remove( AttributeSet attributes, FunctionAttributeIndex index )
        {
            if( attributes == null )
                throw new ArgumentNullException( nameof( attributes ) );

            //if( BuilderHandle.IsClosed )
            //    throw new ObjectDisposedException( nameof( AttributeBuilder ) );

            //NativeMethods.AttributeBuilderRemoveAttributes( BuilderHandle, attributes.NativeAttributeSet, ( uint )index );
            //return this;
            throw new NotImplementedException( );
        }

        /// <summary>Removes a target dependent attribute from this builder</summary>
        /// <param name="name">Name of the attribute to remove</param>
        /// <returns>This builder for fluent style programming</returns>
        public AttributeBuilder Remove( string name )
        {
            //if( BuilderHandle.IsClosed )
            //    throw new ObjectDisposedException( nameof( AttributeBuilder ) );

            //NativeMethods.AttributeBuilderRemoveAttribute( BuilderHandle, name );
            //return this;
            throw new NotImplementedException( );
        }

        /// <summary>Merges the contents of another <see cref="AttributeBuilder"/> into this one</summary>
        /// <param name="other">Other <see cref="AttributeSet"/> to merge into this one</param>
        /// <returns>This builder for fluent style programming</returns>
        public AttributeBuilder Merge( AttributeBuilder other )
        {
            if( other == null )
                throw new ArgumentNullException( nameof( other ) );

            //if( BuilderHandle.IsClosed )
            //    throw new ObjectDisposedException( nameof( AttributeBuilder ) );

            //NativeMethods.AttributeBuilderMerge( BuilderHandle, other.BuilderHandle );
            //return this;
            throw new NotImplementedException( );
        }

        /// <summary>Removes the attributes of another builder from this one</summary>
        /// <param name="other">builder containing attributes to remove</param>
        /// <returns>This builder for fluent style programming</returns>
        public AttributeBuilder Remove( AttributeBuilder other )
        {
            if( other == null )
                throw new ArgumentNullException( nameof( other ) );

            //if( BuilderHandle.IsClosed )
            //    throw new ObjectDisposedException( nameof( AttributeBuilder ) );

            //NativeMethods.AttributeBuilderRemoveBldr( BuilderHandle, other.BuilderHandle );
            //return this;
            throw new NotImplementedException( );
        }

        /// <summary>Checks if this builder overlaps the attributes in another builder</summary>
        /// <param name="other">Other builder to check for overlap</param>
        /// <returns><see langword="true"/> if this builder overlaps <paramref name="other"/></returns>
        public bool Overlaps( AttributeBuilder other )
        {
            if( other == null )
                throw new ArgumentNullException( nameof( other ) );

            //if( BuilderHandle.IsClosed )
            //    throw new ObjectDisposedException( nameof( AttributeBuilder ) );

            //return NativeMethods.AttributeBuilderOverlaps( BuilderHandle, other.BuilderHandle );
            throw new NotImplementedException( );
        }

        ///// <summary>Checks if this builder contains a given boolean attribute</summary>
        ///// <param name="kind">Kind of attribute to test for</param>
        ///// <returns><see langword="true"/> if this builder contains <paramref name="kind"/></returns>
        //public bool Contains( AttributeKind kind )
        //{
        //    if( BuilderHandle.IsClosed )
        //        throw new ObjectDisposedException( nameof( AttributeBuilder ) );

        //    //return NativeMethods.AttributeBuilderContainsEnum( BuilderHandle, ( LLVMAttrKind )kind );
        //    throw new NotImplementedException( );
        //}

        /// <summary>Checks if this builder contains a given target dependent attribute</summary>
        /// <param name="name">Kind of attribute to test for</param>
        /// <returns><see langword="true"/> if this builder contains <paramref name="name"/></returns>
        public bool Contains( string name )
        {
            //if( BuilderHandle.IsClosed )
            //    throw new ObjectDisposedException( nameof( AttributeBuilder ) );

            //return NativeMethods.AttributeBuilderContainsName( BuilderHandle, name );
            throw new NotImplementedException( );
        }

        /// <summary>Checks if the builder contains any of the attributes in a given <see cref="AttributeSet"/></summary>
        /// <param name="attributes">Attributes to check for</param>
        /// <param name="index">Index of <paramref name="attributes"/> to use</param>
        /// <returns><see langword="true"/> if any of the attributes in the specified index of <paramref name="attributes"/> exists in this builder</returns>
        public bool HasAttributes( AttributeSet attributes, FunctionAttributeIndex index )
        {
            if( attributes == null )
                throw new ArgumentNullException( nameof( attributes ) );

            //if( BuilderHandle.IsClosed )
            //    throw new ObjectDisposedException( nameof( AttributeBuilder ) );

            //return NativeMethods.AttributeBuilderHasAttributes( BuilderHandle, attributes.NativeAttributeSet, ( uint )index );
            throw new NotImplementedException( );
        }

        /// <summary>Converts the contents of this builder to an immutable <see cref="AttributeSet"/></summary>
        /// <param name="index">Index for the attributes in the new AttributeSet</param>
        /// <param name="context"><see cref="Context"/> to use when building the resulting AttributeSet</param>
        /// <returns>New <see cref="AttributeSet"/> containing the attributes from this builder in the specified index</returns>
        public AttributeSet ToAttributeSet( FunctionAttributeIndex index, Context context )
        {
            //if( BuilderHandle.IsClosed )
            //    throw new ObjectDisposedException( nameof( AttributeBuilder ) );

            return new AttributeSet( context, index, this );
        }

        List<AttributeValue> Attributes = new List<AttributeValue>();
    }
}
#endif