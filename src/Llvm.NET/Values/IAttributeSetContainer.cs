namespace Llvm.NET.Values
{
    public interface IAttributeSet
    {
        /// <summary>Attributes for this container</summary>
        IAttributeCollection this[ FunctionAttributeIndex index ] { get; }
    }

    /// <summary>Interface for values containing an AttributeSet</summary>
    /// <remarks>
    /// This is used to allow consistent manipulation of attributes in
    /// a manner that is the least disruptive to existing consumers. The
    /// Attribute support went through some major changes from ~3.6->5.0.
    /// Which, has caused a significant amount of churn in the this code
    /// base to keep up without causing mass re-writes of existing code.
    /// </remarks>
    public interface IAttributeSetContainer
    {
        /// <summary>Attributes for this container</summary>
        IAttributeSet Attributes { get; }
    }
}
