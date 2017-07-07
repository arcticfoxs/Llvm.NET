using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;

namespace Llvm.NET.Native
{
    internal struct LLVMVersionInfo
    {
        public readonly int Major;
        public readonly int Minor;
        public readonly int Patch;
        readonly IntPtr VersionString;

        public override string ToString( )
        {
            if( VersionString == IntPtr.Zero )
                return null;

            return Marshal.PtrToStringAnsi( VersionString );
        }

        public static implicit operator Version( LLVMVersionInfo versionInfo )
            => new Version( versionInfo.Major, versionInfo.Minor, versionInfo.Patch );
    }

    // add implicit conversions to/from C# bool for convenience
    internal partial struct LLVMBool
    {
        // sometimes LLVMBool values are actually success/failure codes
        // and thus a zero value actually means success and not false or failure.
        public bool Succeeded => Value == 0;

        public bool Failed => !Succeeded;

        public static implicit operator LLVMBool( bool value ) => new LLVMBool( value ? 1 : 0 );
        public static implicit operator bool( LLVMBool value ) => value.Value != 0;
    }

    internal partial struct LLVMMetadataRef
        : IEquatable<LLVMMetadataRef>
    {
        public static LLVMMetadataRef Zero = new LLVMMetadataRef( IntPtr.Zero );

        public override int GetHashCode( ) => Pointer.GetHashCode( );

        public override bool Equals( object obj )
        {
            if( obj is LLVMMetadataRef )
                return Equals( ( LLVMMetadataRef )obj );

            if( obj is IntPtr )
                return Pointer.Equals( obj );

            return base.Equals( obj );
        }

        public bool Equals( LLVMMetadataRef other ) => Pointer == other.Pointer;

        public static bool operator ==( LLVMMetadataRef lhs, LLVMMetadataRef rhs ) => lhs.Equals( rhs );
        public static bool operator !=( LLVMMetadataRef lhs, LLVMMetadataRef rhs ) => !lhs.Equals( rhs );
    }

    internal static class IntPtrExtensions
    {
        public static bool IsNull( this IntPtr self ) => self == IntPtr.Zero;
        public static bool IsNull( this UIntPtr self ) => self == UIntPtr.Zero;
    }

    /// <summary>Base class for LLVM disposable types that are instantiated outside of an LLVM <see cref="Llvm.NET.Context"/> and therefore won't be disposed by the context</summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    internal abstract class SafeHandleNullIsInvalid
        : SafeHandle
    {
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected SafeHandleNullIsInvalid( bool ownsHandle)
            : base( IntPtr.Zero, ownsHandle )
        {
        }

        public bool IsNull => handle.IsNull();

        public override bool IsInvalid
        {
            [SecurityCritical]
            get
            {
                return IsNull;
            }
        }
    }

    [SecurityCritical]
    internal class LLVMPassRegistryRef
        : SafeHandleNullIsInvalid
    {
        internal LLVMPassRegistryRef( )
            : base( true )
        {
        }

        internal LLVMPassRegistryRef( IntPtr handle, bool owner )
            : base( owner )
        {
            SetHandle( handle );
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required for marshaling support (used via reflection)" )]
        internal LLVMPassRegistryRef( IntPtr handle )
            : this( handle, false )
        {
        }

        [SecurityCritical]
        protected override bool ReleaseHandle( )
        {
            NativeMethods.PassRegistryDispose( handle );
            return true;
        }
    }

    // typedef struct LLVMOpaqueTriple* LLVMTripleRef;
    [SecurityCritical]
    internal class LLVMTripleRef
        : SafeHandleNullIsInvalid
    {
        internal LLVMTripleRef( )
            : base( true )
        {
        }

        internal LLVMTripleRef( IntPtr handle, bool owner )
            : base( owner )
        {
            SetHandle( handle );
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required for marshaling support (used via reflection)" )]
        internal LLVMTripleRef( IntPtr handle )
            : this( handle, false )
        {
        }

        [SecurityCritical]
        protected override bool ReleaseHandle( )
        {
            NativeMethods.DisposeTriple( handle );
            return true;
        }
    }

    [SecurityCritical]
    internal class LLVMBuilderRef
        : SafeHandleNullIsInvalid
    {
        internal LLVMBuilderRef( )
            : base( true )
        {
        }

        internal LLVMBuilderRef( IntPtr handle, bool owner )
            : base( owner )
        {
            SetHandle( handle );
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required for marshaling support (used via reflection)" )]
        internal LLVMBuilderRef( IntPtr handle )
            : this( handle, false )
        {
        }

        [SecurityCritical]
        protected override bool ReleaseHandle( )
        {
            NativeMethods.DisposeBuilder( handle );
            return true;
        }
    }

    internal static partial class NativeMethods
    {
        static void FatalErrorHandler( string Reason )
        {
            Trace.TraceError( "LLVM Fatal Error: '{0}'; Application exiting.", Reason );
            // LLVM will call exit() upon return from this function and there's no way to stop it
        }

        ///// <summary>This method is used to marshal a string when NativeMethods.DisposeMessage() is required on the string allocated from native code</summary>
        ///// <param name="msg">Pointer to the native code allocated string</param>
        ///// <returns>Managed code string marshaled from the native content</returns>
        ///// <remarks>
        ///// This method will, construct a new managed string containing the text of the string from native code, normalizing
        ///// the line endings to the current execution environments line endings (See: <see cref="Environment.NewLine"/>).
        ///// </remarks>
        //internal static string MarshalMsg( IntPtr msg )
        //{
        //    var retVal = string.Empty;
        //    if( msg != IntPtr.Zero )
        //    {
        //        try
        //        {
        //            retVal = NormalizeLineEndings( msg );
        //        }
        //        finally
        //        {
        //            DisposeMessage( msg );
        //        }
        //    }
        //    return retVal;
        //}

        /// <summary>Static constructor for NativeMethods</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations" )]
        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline" )]
        static NativeMethods( )
        {
            // force loading the appropriate architecture specific
            // DLL before any use of the wrapped inter-op APIs to
            // allow building this library as ANYCPU
            var path = Path.GetDirectoryName( Assembly.GetExecutingAssembly( ).Location );
            if( Directory.Exists( Path.Combine( path, "LibLLVM" ) ) )
            {
                LoadWin32Library( libraryPath, "LibLLVM" );
            }
            else
            {
                // fall-back to standard library search paths to allow building
                // CPU specific variants with only one native DLL without needing
                // conditional compilation on this library, which is useful for
                // unit testing or whenever the Nuget packaging isn't desired.
                LoadWin32Library( libraryPath, null );
            }

            // Verify the version of LLVM in LibLLVM
            LLVMVersionInfo versionInfo = new LLVMVersionInfo( );
            GetVersionInfo( ref versionInfo );
            if( versionInfo.Major != VersionMajor
             || versionInfo.Minor != VersionMinor
             || versionInfo.Patch < VersionPatch
              )
            {
                throw new BadImageFormatException( "Mismatched LibLLVM version" );
            }

            // initialize the static fields
            LineEndingNormalizingRegEx = new Regex( "(\r\n|\n\r|\r|\n)" );
            FatalErrorHandlerDelegate = new Lazy<LLVMFatalErrorHandler>( ( ) => FatalErrorHandler, LazyThreadSafetyMode.PublicationOnly );
            InstallFatalErrorHandler( FatalErrorHandlerDelegate.Value );
        }

        /// <summary>Dynamically loads a DLL from a directory dependent on the current architecture</summary>
        /// <param name="moduleName">name of the DLL</param>
        /// <param name="rootPath">Root path to find the DLL from</param>
        /// <returns>Handle for the DLL</returns>
        /// <remarks>
        /// <para>This method will detect the architecture the code is executing on (i.e. x86 or x64)
        /// and will load the DLL from an architecture specific sub folder of <paramref name="rootPath"/>.
        /// This allows use of AnyCPU builds and interop to simplify build processes from needing to
        /// deal with "mixed" configurations or other accidental combinations that are a pain to
        /// sort out and keep straight when the tools insist on creating AnyCPU projects and "mixed" configurations
        /// by default.</para>
        /// <para>If the <paramref name="rootPath"/>Is <see langword="null"/>, empty or all whitespace then
        /// the standard DLL search paths are used. This assumes the correct variant of the DLL is available
        /// (e.g. for a 32 bit system a 32 bit native DLL is found). This allows for either building as AnyCPU
        /// plus shipping multiple native DLLs, or building for a specific CPU type while shipping only one native
        /// DLL. Different products or projects may have different needs so this covers those cases.
        /// </para>
        /// </remarks>
        internal static IntPtr LoadWin32Library( string moduleName, string rootPath )
        {
            if( string.IsNullOrWhiteSpace( moduleName ) )
                throw new ArgumentNullException( nameof( moduleName ) );

            string libPath;
            if( string.IsNullOrWhiteSpace( rootPath ) )
                libPath = moduleName;
            else
            {
                if( Environment.Is64BitProcess )
                    libPath = Path.Combine( rootPath, "x64", moduleName );
                else
                    libPath = Path.Combine( rootPath, "x86", moduleName );
            }

            IntPtr moduleHandle = LoadLibrary( libPath );
            if( moduleHandle == IntPtr.Zero )
            {
                var lasterror = Marshal.GetLastWin32Error( );
                throw new Win32Exception( lasterror );
            }
            return moduleHandle;
        }

        [DllImport( "kernel32", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true )]
        static extern IntPtr LoadLibrary( [MarshalAs( UnmanagedType.LPStr )]string lpFileName );

        // LLVM doesn't use environment/OS specific line endings, so this will
        // normalize the line endings from strings provided by LLVM into the current
        // environment's normal format.
        internal static string NormalizeLineEndings( IntPtr llvmString )
        {
            if( llvmString == IntPtr.Zero )
                return string.Empty;

            var str = Marshal.PtrToStringAnsi( llvmString );
            return NormalizeLineEndings( str );
        }

        internal static string NormalizeLineEndings( IntPtr llvmString, int len )
        {
            if( llvmString == IntPtr.Zero || len == 0 )
                return string.Empty;

            var str = Marshal.PtrToStringAnsi( llvmString, len );
            return NormalizeLineEndings( str );
        }

        internal static string NormalizeLineEndings( string txt )
        {
            // shortcut optimization for environments that match the LLVM assumption
            if( Environment.NewLine.Length == 1 && Environment.NewLine[ 0 ] == '\n' )
                return txt;

            return LineEndingNormalizingRegEx.Replace( txt, Environment.NewLine );
        }

        // lazy initialized singleton unmanaged delegate so it is never collected
        private static Lazy<LLVMFatalErrorHandler> FatalErrorHandlerDelegate;
        private static readonly Regex LineEndingNormalizingRegEx;

        // version info for verification of matched LibLLVM
        private const int VersionMajor = 5;
        private const int VersionMinor = 0;
        private const int VersionPatch = 0;
    }
}
