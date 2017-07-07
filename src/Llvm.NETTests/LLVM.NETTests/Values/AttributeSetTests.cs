using System.Linq;
using Llvm.NET;
using Llvm.NET.Values;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Llvm.NETTests
{
    [TestClass]
    public class AttributeSetTests
    {
        [ClassInitialize]
        public static void ClassInitialize( TestContext testContext )
        {

        }

        [TestMethod]
        public void ParameterAttributesTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var ctx = module.Context;
                var sig = ctx.GetFunctionType( module.Context.Int8Type.CreatePointerType(), module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                // add attributes to all indices of the function
                var attributes = function.Attributes.Add( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline, AttributeKind.OptimizeNone );
                attributes = attributes.Add( FunctionAttributeIndex.ReturnType, AttributeKind.NonNull );
                attributes = attributes.Add( FunctionAttributeIndex.Parameter0, AttributeKind.InReg );

                var paramAttributes = attributes.ParameterAttributes( 0 );
                Assert.AreNotSame( attributes, paramAttributes );
                Assert.AreEqual( "inreg", paramAttributes.AsString( ) );
            }
        }

        [TestMethod]
        public void GetParameterWhenNoAttributessetForParameterTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var ctx = module.Context;
                var sig = ctx.GetFunctionType( module.Context.Int8Type.CreatePointerType(), module.Context.Int32Type, module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                // add attributes to all indices of the function
                var attributes = function.Attributes.Add( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline, AttributeKind.OptimizeNone )
                                                    .Add( FunctionAttributeIndex.ReturnType, AttributeKind.NonNull )
                                                    .Add( FunctionAttributeIndex.Parameter0, AttributeKind.InReg );

                var paramAttributes = attributes.ParameterAttributes( 0 );
                Assert.AreNotSame( attributes, paramAttributes );
                // parameterAttributes should only have attributes on the parameter index (e.g. ParameterAttributes(0) should filter them)
                Assert.AreEqual( "inreg", paramAttributes.AsString( ) );
                var shouldBeEmptyAttributes = attributes.ParameterAttributes( 1 );
                Assert.IsFalse( shouldBeEmptyAttributes.Any( ) );
            }
        }

        [TestMethod]
        public void AsStringForEmptySetIsEmptyStringTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var sig = module.Context.GetFunctionType( module.Context.Int8Type, module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                Assert.AreEqual( string.Empty, function.Attributes.AsString( FunctionAttributeIndex.Function ) );
                Assert.AreEqual( string.Empty, function.Attributes.AsString( FunctionAttributeIndex.ReturnType ) );
                Assert.AreEqual( string.Empty, function.Attributes.AsString( FunctionAttributeIndex.Parameter0 ) );
            }
        }

        [TestMethod]
        public void AsStringWithOneFunctionAttributeTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var sig = module.Context.GetFunctionType( module.Context.Int8Type, module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                function.AddAttributes( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline );
                Assert.AreEqual( "alwaysinline", function.Attributes.AsString( FunctionAttributeIndex.Function ) );
                Assert.AreEqual( string.Empty, function.Attributes.AsString( FunctionAttributeIndex.ReturnType ) );
                Assert.AreEqual( string.Empty, function.Attributes.AsString( FunctionAttributeIndex.Parameter0 ) );
            }
        }

        [TestMethod]
        public void AsStringWithTwoFunctionAttributeTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var sig = module.Context.GetFunctionType( module.Context.Int8Type, module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                function.AddAttributes( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline, AttributeKind.OptimizeNone );
                Assert.AreEqual( "alwaysinline optnone", function.Attributes.AsString( FunctionAttributeIndex.Function ) );
                Assert.AreEqual( string.Empty, function.Attributes.AsString( FunctionAttributeIndex.ReturnType ) );
                Assert.AreEqual( string.Empty, function.Attributes.AsString( FunctionAttributeIndex.Parameter0 ) );
            }
        }

        [TestMethod]
        public void AsStringWithMultipleAttributesTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var sig = module.Context.GetFunctionType( module.Context.Int8Type.CreatePointerType( ), module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                function.AddAttributes( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline, AttributeKind.OptimizeNone );
                function.AddAttribute( FunctionAttributeIndex.ReturnType, AttributeKind.NonNull );
                function.AddAttribute( FunctionAttributeIndex.Parameter0, AttributeKind.InReg );
                Assert.AreEqual( "alwaysinline optnone", function.Attributes.AsString( FunctionAttributeIndex.Function ) );
                Assert.AreEqual( "nonnull", function.Attributes.AsString( FunctionAttributeIndex.ReturnType ) );
                Assert.AreEqual( "inreg", function.Attributes.AsString( FunctionAttributeIndex.Parameter0 ) );
            }
        }

        [TestMethod]
        [Description("Verifies that AttributeSet mutation with additional enum attributes into a new set propagates existing values into the new set")]
        public void EnumFunctionAttributeMutationPropagationTest( )
        {
            using( var ctx = new Context() )
            using( var module = new NativeModule( "test", ctx ) )
            {
                var structType = ctx.CreateStructType( "testT", false, ctx.Int32Type, ctx.Int8Type.CreateArrayType( 32 ) );
                var func = module.AddFunction( "test", ctx.GetFunctionType( ctx.VoidType, structType.CreatePointerType() ) );

                var originalAttributes = func.Attributes;
                // shouldn't be created with any attributes
                Assert.IsFalse( originalAttributes.HasAny( FunctionAttributeIndex.Function ) );

                var newAttribs = func.Attributes.Add( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline );
                // result AttributeSet should be the same as the original (e.g. fluent pass through)
                Assert.IsTrue( originalAttributes.Has( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline ) );

                Assert.IsTrue( newAttribs.Has( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline ) );

                // Add another attribute
                var additionalAttributes = newAttribs.Add( FunctionAttributeIndex.Function, AttributeKind.NoUnwind );
                Assert.IsTrue( additionalAttributes.Has( FunctionAttributeIndex.Function, AttributeKind.NoUnwind ) );

                // function should still have the new ones
                Assert.IsTrue( newAttribs.Has(FunctionAttributeIndex.Function, AttributeKind.AlwaysInline ) );
                Assert.IsTrue( originalAttributes.Has( FunctionAttributeIndex.Function, AttributeKind.NoUnwind ) );
                Assert.IsTrue( newAttribs.Has( FunctionAttributeIndex.Function, AttributeKind.NoUnwind ) );
            }
        }

        [TestMethod]
        [Description("Verifies that AttributeSet mutation with additional enum attributes into a new set propagates existing values into the new set")]
        public void EnumReturnAttributeMutationPropagationTest( )
        {
            using( var ctx = new Context() )
            using( var module = new NativeModule( "test", ctx ) )
            {
                var structType = ctx.CreateStructType( "testT", false, ctx.Int32Type, ctx.Int8Type.CreateArrayType( 32 ) );
                var func = module.AddFunction( "test", ctx.GetFunctionType( ctx.VoidType, structType.CreatePointerType() ) );

                var originalAttributes = func.Attributes;
                // shouldn't be created with any attributes
                Assert.IsFalse( originalAttributes.HasAny( FunctionAttributeIndex.Function ) );

                var newAttribs = func.Attributes.Add( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline );
                // Adding the new AttributeSet should modify the original (e.g. fluent pass thru)
                Assert.IsTrue( originalAttributes.Has( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline ) );

                Assert.IsTrue( newAttribs.Has( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline ) );

                // Add another attribute
                var additionalAttributes = newAttribs.Add( FunctionAttributeIndex.Function, AttributeKind.NoUnwind );
                Assert.IsTrue( additionalAttributes.Has( FunctionAttributeIndex.Function, AttributeKind.NoUnwind ) );

                // function should the new attributes
                Assert.IsTrue( newAttribs.Has(FunctionAttributeIndex.Function, AttributeKind.AlwaysInline ) );
                Assert.IsTrue( originalAttributes.Has( FunctionAttributeIndex.Function, AttributeKind.NoUnwind ) );
                Assert.IsTrue( newAttribs.Has( FunctionAttributeIndex.Function, AttributeKind.NoUnwind ) );
            }
        }

        [TestMethod]
        [ExpectedArgumentException( "index", ExpectedExceptionMessage = "Specified parameter index exceeds the number of parameters in the function" )]
        public void OutofRangeParameterIndexTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var sig = module.Context.GetFunctionType( module.Context.Int8Type.CreatePointerType( ), module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                function.Attributes.Add( FunctionAttributeIndex.Parameter0 + 1, module.Context.CreateAttribute( AttributeKind.Alignment, 64 ) );
            }
        }

        [TestMethod]
        [ExpectedArgumentException( "index", ExpectedExceptionMessage = "Attribute not allowed on functions" )]
        public void AlignmentNotSupportedOnFunctionTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var sig = module.Context.GetFunctionType( module.Context.Int8Type.CreatePointerType( ), module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                function.Attributes.Add( FunctionAttributeIndex.Function, module.Context.CreateAttribute( AttributeKind.Alignment, 64 ) );
            }
        }

        [TestMethod]
        [ExpectedArgumentException( "index", ExpectedExceptionMessage = "Attribute not allowed on function Return" )]
        public void StackAlignmentNotSupportedOnReturnTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var sig = module.Context.GetFunctionType( module.Context.Int8Type.CreatePointerType( ), module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                function.Attributes.Add( FunctionAttributeIndex.ReturnType, module.Context.CreateAttribute( AttributeKind.StackAlignment, 64 ) );
            }
        }

        [TestMethod]
        [ExpectedArgumentException( "index", ExpectedExceptionMessage = "Attribute not allowed on function parameter" )]
        public void StackAlignmentNotSupportedOnParameterTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var sig = module.Context.GetFunctionType( module.Context.Int8Type.CreatePointerType( ), module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                function.Attributes.Add( FunctionAttributeIndex.Parameter0, module.Context.CreateAttribute( AttributeKind.StackAlignment, 64 ) );
            }
        }

        [TestMethod]
        [ExpectedArgumentException( "index", ExpectedExceptionMessage = "Attribute not allowed on functions" )]
        public void DereferenceableNotSupportedOnFunctionTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var sig = module.Context.GetFunctionType( module.Context.Int8Type.CreatePointerType( ), module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                function.Attributes.Add( FunctionAttributeIndex.Function, module.Context.CreateAttribute( AttributeKind.Dereferenceable, 64 ) );
            }
        }

        [TestMethod]
        [ExpectedArgumentException( "index", ExpectedExceptionMessage = "Attribute not allowed on functions" )]
        public void DereferenceableOrNullNotSupportedOnFunctionTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var sig = module.Context.GetFunctionType( module.Context.Int8Type.CreatePointerType( ), module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                function.Attributes.Add( FunctionAttributeIndex.Function, module.Context.CreateAttribute( AttributeKind.DereferenceableOrNull, 64 ) );
            }
        }

        [TestMethod]
        public void AddAndGetParameterAlignmentAttributeTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var ctx = module.Context;
                var sig = ctx.GetFunctionType( ctx.Int8Type.CreatePointerType(), ctx.Int32Type );
                var function = module.AddFunction( "test", sig );
                var attributes = function.Attributes.Add( FunctionAttributeIndex.Parameter0, ctx.CreateAttribute( AttributeKind.Alignment, 64) );
                var actual = attributes.GetAttributeValue( FunctionAttributeIndex.Parameter0, AttributeKind.Alignment );
                Assert.AreEqual( 64ul, actual );
            }
        }

        [TestMethod]
        public void AddAndGetParameterDereferenceableAttributeTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var ctx = module.Context;
                var sig = ctx.GetFunctionType( ctx.Int8Type.CreatePointerType(), ctx.Int32Type );
                var function = module.AddFunction( "test", sig );
                var attributes = function.Attributes.Add( FunctionAttributeIndex.Parameter0, ctx.CreateAttribute( AttributeKind.Dereferenceable, 64) );
                var actual = attributes.GetAttributeValue( FunctionAttributeIndex.Parameter0, AttributeKind.Dereferenceable );
                Assert.AreEqual( 64ul, actual );
            }
        }

        [TestMethod]
        public void AddAndGetParameterDereferenceableOrNullAttributeTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var ctx = module.Context;
                var sig = ctx.GetFunctionType( ctx.Int8Type.CreatePointerType(), ctx.Int32Type );
                var function = module.AddFunction( "test", sig );
                var attributes = function.Attributes.Add( FunctionAttributeIndex.Parameter0, ctx.CreateAttribute( AttributeKind.DereferenceableOrNull, 64) );
                var actual = attributes.GetAttributeValue( FunctionAttributeIndex.Parameter0, AttributeKind.DereferenceableOrNull );
                Assert.AreEqual( 64ul, actual );
            }
        }

        [TestMethod]
        public void AddAndGetFunctionStackAlignmentAttributeTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var ctx = module.Context;
                var sig = module.Context.GetFunctionType( ctx.Int8Type.CreatePointerType(), ctx.Int32Type );
                var function = module.AddFunction( "test", sig );
                var attributes = function.Attributes.Add( FunctionAttributeIndex.Function, ctx.CreateAttribute( AttributeKind.StackAlignment, 64) );
                var actual = attributes.GetAttributeValue( FunctionAttributeIndex.Function, AttributeKind.StackAlignment );
                Assert.AreEqual( 64ul, actual );
            }
        }

        // TODO: verify each applicability check for attributes and indices
        // e.g. attempt to apply all attributes in AttributeKind to all indices
        // and ensure invalid ones are flagged with an argument exception...

        // TODO: String attributes and values on each index...

        [TestMethod]
        public void RemoveTest( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var ctx = module.Context;
                var sig = module.Context.GetFunctionType( module.Context.Int8Type.CreatePointerType(), module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                var attributes = function.Attributes.Add( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline, AttributeKind.OptimizeNone );
                attributes = attributes.Add( FunctionAttributeIndex.ReturnType, AttributeKind.NonNull );
                attributes = attributes.Add( FunctionAttributeIndex.Parameter0, AttributeKind.InReg );

                // verify all the expected attributes are present before trying to remove one
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.ReturnType, AttributeKind.NonNull ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Parameter0, AttributeKind.InReg ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Function, AttributeKind.OptimizeNone ) );

                attributes = attributes.Remove( FunctionAttributeIndex.Function, AttributeKind.OptimizeNone );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.ReturnType, AttributeKind.NonNull ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Parameter0, AttributeKind.InReg ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline ) );
                Assert.IsFalse( attributes.Has( FunctionAttributeIndex.Function, AttributeKind.OptimizeNone ) );
            }
        }

        [TestMethod]
        public void RemoveNamedAttributeTest1( )
        {
            using( var module = new NativeModule( "test" ) )
            {
                var ctx = module.Context;
                var sig = ctx.GetFunctionType( module.Context.Int8Type.CreatePointerType(), module.Context.Int32Type );
                var function = module.AddFunction( "test", sig );
                var attributes = function.Attributes.Add( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline );
                attributes = attributes.Add( FunctionAttributeIndex.Function, module.Context.CreateAttribute( "testattr" ) );
                attributes = attributes.Add( FunctionAttributeIndex.ReturnType, AttributeKind.NonNull );
                attributes = attributes.Add( FunctionAttributeIndex.Parameter0, AttributeKind.InReg );

                // verify all the expected attributes are present before trying to remove one
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.ReturnType, AttributeKind.NonNull ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Parameter0, AttributeKind.InReg ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Function, "testattr" ) );

                attributes = attributes.Remove( FunctionAttributeIndex.Function, "testattr" );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.ReturnType, AttributeKind.NonNull ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Parameter0, AttributeKind.InReg ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline ) );
                Assert.IsFalse( attributes.Has( FunctionAttributeIndex.Function, "testattr" ) );
            }
        }

        [TestMethod]
        public void HasAnyTest( )
        {
            using( var ctx = new Context( ) )
            {
                IAttributeSet attributes = new AttributeSet( ctx );
                Assert.IsFalse( attributes.HasAny( FunctionAttributeIndex.Function ) );
                Assert.IsFalse( attributes.HasAny( FunctionAttributeIndex.ReturnType ) );
                Assert.IsFalse( attributes.HasAny( FunctionAttributeIndex.Parameter0 ) );
                Assert.IsFalse( attributes.HasAny( FunctionAttributeIndex.Parameter0 + 1 ) );

                attributes = attributes.Add( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline, AttributeKind.Builtin )
                                       .Add( FunctionAttributeIndex.ReturnType, ctx.CreateAttribute( AttributeKind.DereferenceableOrNull, 1 ) )
                                       .Add( FunctionAttributeIndex.Parameter0, AttributeKind.ByVal )
                                       .Add( FunctionAttributeIndex.Parameter0 + 1, ctx.CreateAttribute( "TestCustom" ) );

                Assert.IsTrue( attributes.HasAny( FunctionAttributeIndex.Function ) );
                Assert.IsTrue( attributes.HasAny( FunctionAttributeIndex.ReturnType ) );
                Assert.IsTrue( attributes.HasAny( FunctionAttributeIndex.Parameter0 ) );
                Assert.IsTrue( attributes.HasAny( FunctionAttributeIndex.Parameter0 + 1 ) );
            }
        }

        [TestMethod]
        public void HasTest( )
        {
            using( var ctx = new Context( ) )
            {
                IAttributeSet attributes = new AttributeSet( ctx );

                Assert.IsFalse( attributes.Has( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline ) );
                Assert.IsFalse( attributes.Has( FunctionAttributeIndex.Function, AttributeKind.Builtin ) );
                Assert.IsFalse( attributes.Has( FunctionAttributeIndex.ReturnType, AttributeKind.DereferenceableOrNull ) );
                Assert.IsFalse( attributes.Has( FunctionAttributeIndex.Parameter0, AttributeKind.ByVal ) );
                Assert.IsFalse( attributes.Has( FunctionAttributeIndex.Parameter0 + 1, "TestCustom" ) );

                attributes = attributes.Add( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline, AttributeKind.Builtin )
                                       .Add( FunctionAttributeIndex.ReturnType, ctx.CreateAttribute( AttributeKind.DereferenceableOrNull, 1 ) )
                                       .Add( FunctionAttributeIndex.Parameter0, AttributeKind.ByVal )
                                       .Add( FunctionAttributeIndex.Parameter0 + 1, ctx.CreateAttribute( "TestCustom" ) );

                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Function, AttributeKind.Builtin ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.ReturnType, AttributeKind.DereferenceableOrNull ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Parameter0, AttributeKind.ByVal ) );
                Assert.IsTrue( attributes.Has( FunctionAttributeIndex.Parameter0 + 1, "TestCustom" ) );
            }
        }

        //[TestMethod]
        //[Description( "Verifies that the AllAttributesProperty contains the attributes specified for an attribute set" )]
        //public void AttributeSetAllAttributesTest( )
        //{
        //    using( var ctx = new Context( ) )
        //    {
        //        var attributes = new AttributeSet( ctx )
        //                                .Add( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline, AttributeKind.Builtin )
        //                                .Add( FunctionAttributeIndex.ReturnType, ctx.CreateAttribute( AttributeKind.DereferenceableOrNull, 1 ) )
        //                                .Add( FunctionAttributeIndex.Parameter0, AttributeKind.ByVal )
        //                                .Add( FunctionAttributeIndex.Parameter0 + 1, "TestCustom".ToAttributeValue( ctx ) );

        //        var expectedSet = new HashSet<IndexedAttributeValue>
        //            { new IndexedAttributeValue( FunctionAttributeIndex.Function, AttributeKind.AlwaysInline.ToAttributeValue( ctx ) )
        //            , new IndexedAttributeValue( FunctionAttributeIndex.Function, AttributeKind.Builtin.ToAttributeValue( ctx ) )
        //            , new IndexedAttributeValue( FunctionAttributeIndex.ReturnType, ctx.CreateAttribute( AttributeKind.DereferenceableOrNull, 1 ) )
        //            , new IndexedAttributeValue( FunctionAttributeIndex.Parameter0, AttributeKind.ByVal.ToAttributeValue( ctx ) )
        //            , new IndexedAttributeValue( FunctionAttributeIndex.Parameter0 + 1, ctx.CreateAttribute( "TestCustom" ) )
        //            };

        //        Assert.IsTrue( expectedSet.SetEquals( attributes.AllAttributes ) );
        //    }
        //}
    }
}