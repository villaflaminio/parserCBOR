using System;
using System.Numerics;

namespace com.st.stcc.sdk.cbor.cborUtils.Tags
{
    public class BigIntegerTag : ItemTag
    {
        public static ulong[] TAG_NUM = {2, 3};

        public BigIntegerTag(ulong tagId)
        {
            tagNumber = tagId;
        }

        public override object processData(object data)
        {
            Array.Reverse((Array) data);
            BigInteger bi = new BigInteger((byte[]) data);

            if (tagNumber == 2)
                return bi;
            return BigInteger.Subtract(BigInteger.MinusOne, bi);
        }

        public override bool isDataSupported(object data)
        {
            try
            {
                byte[] dataCast = (byte[]) data;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}