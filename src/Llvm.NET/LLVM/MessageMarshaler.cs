using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Llvm.NET.Native
{
    // use with [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(MessageMarshaler))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance"
                                                    , "CA1812:AvoidUninstantiatedInternalClasses"
                                                    , Justification = "Instantiated via CustomMarshaling"
                                                    )
    ]
    internal class MessageMarshaler
        : ICustomMarshaler
    {
        public void CleanUpManagedData( object ManagedObj )
        {
        }

        public void CleanUpNativeData( IntPtr pNativeData )
        {
            NativeMethods.DisposeMessage( pNativeData );
        }

        public int GetNativeDataSize( )
        {
            throw new NotImplementedException( );
        }

        public IntPtr MarshalManagedToNative( object ManagedObj )
        {
            throw new NotImplementedException( );
        }

        public object MarshalNativeToManaged( IntPtr pNativeData )
        {
            System.Diagnostics.Debugger.Break( );
            if( pNativeData == IntPtr.Zero )
            {
                return string.Empty;
            }

            return NativeMethods.NormalizeLineEndings( pNativeData );
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "cookie" )]
        public static ICustomMarshaler GetInstance( string cookie )=> Instance.Value;

        static Lazy<MessageMarshaler> Instance = new Lazy<MessageMarshaler>( LazyThreadSafetyMode.PublicationOnly );
    }
}
