using com.st.stcc.configuration.model;
using System;
using System.Collections.Generic;

namespace com.st.stcc.sdk.cbor
{
    public interface ICborManager
    {
        event EventHandler<STCCMQTTMessage> CborMessageDecoded;
        event EventHandler<Exception> CborMessageDecodingError;
        event EventHandler<string> CborMessageEncoded;
        event EventHandler<Exception> CborMessageEncodingError;

        Dictionary<string, object> Decode(STCCMQTTMessage message);
        string Encode(Dictionary<string, object> data);
    }
}