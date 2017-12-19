using System;
using System.Runtime.Serialization;

[Serializable]
public class OutdatedSaveMethod : Exception
{
    // Constructors
    public OutdatedSaveMethod(string message)
        : base(message)
    { }

    // Ensure Exception is Serializable
    protected OutdatedSaveMethod(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
    { }
}