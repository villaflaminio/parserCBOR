namespace com.st.stcc.sdk.cbor.cborUtils.Tags
{
    internal class CBORItemTag : ItemTag
    {
        public static ulong[] TAG_NUM = {24};

        public CBORItemTag(ulong tagNum)
        {
            tagNumber = tagNum;
        }

        public override object processData(object data)
        {
            CBORDecoder decoder = new CBORDecoder((byte[]) data);
            return decoder.ReadItem();
        }

        public override bool isDataSupported(object data)
        {
            return data is byte[];
        }
    }
}