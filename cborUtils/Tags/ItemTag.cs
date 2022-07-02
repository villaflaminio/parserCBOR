namespace com.st.stcc.sdk.cbor.cborUtils.Tags
{
    public abstract class ItemTag
    {
        public ulong tagNumber;

        public abstract object processData(object data);


        public abstract bool isDataSupported(object data);
    }
}