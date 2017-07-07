using System;
using Llvm.NET;
using Llvm.NET.Types;
using Llvm.NET.Values;

namespace TestDebugInfo
{
    class X64Details
        : ITargetDependentDetails
    {
        public string Cpu => "x86-64";
        public string Features => "+sse,+sse2";
        public string ShortName => "x86";
        public string Triple => "x86_64-pc-windows-msvc18.0.0";

        public void AddABIAttributesForByValueStructure( Function function, int paramIndex )
        {
            var argType = function.Parameters[ paramIndex ].NativeType as IPointerType;
            if( argType == null || !argType.ElementType.IsStruct )
                throw new ArgumentException( "Signature for specified parameter must be a pointer to a structure" );
        }

        public void AddModuleFlags( NativeModule module )
        {
            module.AddModuleFlag( ModuleFlagBehavior.Error, "PIC Level", 2 );
        }

        public IAttributeSet BuildTargetDependentFunctionAttributes( Context ctx )
        {
            var bldr = new AttributeSet( ctx );

            bldr[ FunctionAttributeIndex.Function ].Add("disable-tail-calls", "false" );
            bldr[ FunctionAttributeIndex.Function ].Add( "less-precise-fpmad", "false" );
            bldr[ FunctionAttributeIndex.Function ].Add( "no-frame-pointer-elim", "false" );
            bldr[ FunctionAttributeIndex.Function ].Add( "no-infs-fp-math", "false" );
            bldr[ FunctionAttributeIndex.Function ].Add( "no-nans-fp-math", "false" );
            bldr[ FunctionAttributeIndex.Function ].Add( "stack-protector-buffer-size", "8" );
            bldr[ FunctionAttributeIndex.Function ].Add( "target-cpu", Cpu );
            bldr[ FunctionAttributeIndex.Function ].Add( "target-features", Features );
            bldr[ FunctionAttributeIndex.Function ].Add( "unsafe-fp-math", "false" );
            bldr[ FunctionAttributeIndex.Function ].Add( "use-soft-float", "false" );
            bldr[ FunctionAttributeIndex.Function ].Add( AttributeKind.UWTable );
            return bldr;
        }
    }
}