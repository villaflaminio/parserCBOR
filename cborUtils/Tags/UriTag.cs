using System;

namespace com.st.stcc.sdk.cbor.cborUtils.Tags
{
    internal class UriTag : ItemTag
    {
        public static ulong[] TAG_NUM = {32};

        public UriTag(ulong tagNum)
        {
            tagNumber = tagNum;
        }

        public override object processData(object data)
        {
            return new Uri(data as string);
        }

        public override bool isDataSupported(object data)
        {
            return data is string;
        }
    }
}