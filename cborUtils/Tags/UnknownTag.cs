namespace com.st.stcc.sdk.cbor.cborUtils.Tags
{
    public class UnknownTag : ItemTag
    {
        public UnknownTag(ulong tagId)
        {
            tagNumber = tagId;
        }

        public override object processData(object data)
        {
            return data;
        }

        public override bool isDataSupported(object data)
        {
            return true;
        }
    }
}