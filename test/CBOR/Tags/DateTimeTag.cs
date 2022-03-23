using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CBOR.Tags
{
    class DateTimeTag : ItemTag
    {
        public static ulong[] TAG_NUM = new ulong[]{0,1};

        public DateTimeTag(ulong tagNum)
        {
            this.tagNumber = tagNum;
        }

        public override object processData(object data)
        {
            switch (this.tagNumber)
            {
                case 0:
                    return XmlConvert.ToDateTime((data as string));
                case 1:
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    if (data is ulong)
                    {
                        return epoch.AddSeconds((ulong) data);
                    }
                    else
                    {
                        return epoch.AddSeconds((double)data);
                    }
            }
            throw new Exception();
        }

        public override bool isDataSupported(object data)
        {
            if (this.tagNumber == 0)
            {
                return (data is string);
            }
            else
            {
                bool valid = false;

                valid = (data is ulong);
                if (!valid)
                {
                    valid = (data is double);
                    if (!valid)
                    {
                        valid = (data is float);
                    }
                }
                return valid;
            }
            throw new NotImplementedException();
        }
    }
}
