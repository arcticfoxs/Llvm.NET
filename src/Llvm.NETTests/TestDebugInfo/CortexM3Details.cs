using System;
using Llvm.NET;
using Llvm.NET.Types;
using Llvm.NET.Values;

namespace TestDebugInfo
{
    class CortexM3Details
        : ITargetDependentDetails
    {
        public string Cpu => "cortex-m3";
        public string Features => "+hwdiv,+strict-align";
        public string ShortName => "M3";
        public string Triple => "thumbv7m-none--eabi";

        public void AddABIAttributesForByValueStructure( Function function, int paramIndex )
        {
            // ByVal pointers indicate by value semantics. The actual semantics are along the lines of
            // "pass the arg as copy on the arguments stack and set parameter implicitly to that copy's address"
            // (src: https://github.com/ldc-developers/ldc/issues/937 ) [e.g. caller copies byval args]
            //
            // LLVM recognizes this pattern and has a pass to map to an efficient register usage whenever plausible.
            // Though it seems Clang doesn't apply the attribute in all cases, for x86 it doesn't appear to ever use
            // it, for Cortex-Mx it seems to use it only for larger structs, otherwise it uses an [ n x i32]. (Max
            // value of n is not known) and performs casts. Thus, on cortex-m the function parameters are handled
            // quite differently by clang, which seems odd to put such target dependent differences into the front-end.

            var argType = function.Parameters[ paramIndex ].NativeType as IPointerType;
            if( argType == null || !argType.ElementType.IsStruct )
                throw new ArgumentException( "Signature for specified parameter must be a pointer to a structure" );

            var layout = function.ParentModule.Layout;
            function.AddAttributes( FunctionAttributeIndex.Parameter0 + paramIndex
                                  , AttributeKind.ByVal.ToAttributeValue( function.Context )
                                  , function.Context.CreateAttribute( AttributeKind.Alignment, layout.AbiAlignmentOf( argType.ElementType ) )
                                  );
        }

        public void AddModuleFlags( NativeModule module )
        {
            // Specify ABI const sizes so linker can detect mismatches
            module.AddModuleFlag( ModuleFlagBehavior.Error, "wchar_size", 4 );
            module.AddModuleFlag( ModuleFlagBehavior.Error, "min_enum_size", 4 );
        }

        public IAttributeSet BuildTargetDependentFunctionAttributes( Context ctx )
        {
            var bldr = new AttributeSet( ctx );

            bldr[ FunctionAttributeIndex.Function ].Add( "disable-tail-calls", "false" );
            bldr[ FunctionAttributeIndex.Function ].Add( "less-precise-fpmad", "false" );
            bldr[ FunctionAttributeIndex.Function ].Add( "no-frame-pointer-elim", "true" );
            bldr[ FunctionAttributeIndex.Function ].Add( "no-frame-pointer-elim-non-leaf", string.Empty );
            bldr[ FunctionAttributeIndex.Function ].Add( "no-infs-fp-math", "false" );
            bldr[ FunctionAttributeIndex.Function ].Add( "no-nans-fp-math", "false" );
            bldr[ FunctionAttributeIndex.Function ].Add( "stack-protector-buffer-size", "8" );
            bldr[ FunctionAttributeIndex.Function ].Add( "target-cpu", Cpu );
            bldr[ FunctionAttributeIndex.Function ].Add( "target-features", Features );
            bldr[ FunctionAttributeIndex.Function ].Add( "unsafe-fp-math", "false" );
            bldr[ FunctionAttributeIndex.Function ].Add( "use-soft-float", "false" );
            return null;
        }
    }
}