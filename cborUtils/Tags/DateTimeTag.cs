using System;
using System.Xml;

namespace com.st.stcc.sdk.cbor.cborUtils.Tags
{
    internal class DateTimeTag : ItemTag
    {
        public static ulong[] TAG_NUM = {0, 1};

        public DateTimeTag(ulong tagNum)
        {
            tagNumber = tagNum;
        }

        public override object processData(object data)
        {
            switch (tagNumber)
            {
                case 0:
                    return XmlConvert.ToDateTime(data as string);
                case 1:
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    if (data is ulong)
                        return epoch.AddSeconds((ulong) data);
                    return epoch.AddSeconds((double) data);
            }

            throw new Exception();
        }

        public override bool isDataSupported(object data)
        {
            if (tagNumber == 0) return data is string;

            bool valid = false;

            valid = data is ulong;
            if (!valid)
            {
                valid = data is double;
                if (!valid) valid = data is float;
            }

            return valid;
            throw new NotImplementedException();
        }
    }
}