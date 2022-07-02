using System;
using System.Text;

namespace com.st.stcc.sdk.cbor.cborUtils.Tags
{
    internal class Base64Tag : ItemTag
    {
        public static ulong[] TAG_NUM = {33, 34};

        public Base64Tag(ulong tagNum)
        {
            tagNumber = tagNum;
        }

        public override object processData(object data)
        {
            if (tagNumber == 33)
            {
                string s = data as string;
                s = s.Replace("_", "/");
                s = s.Replace("-", "+");
                s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');

                byte[] decoded = Convert.FromBase64String(s);

                string decodedString = Encoding.UTF8.GetString(decoded);

                return new Uri(decodedString);
            }
            else
            {
                byte[] decoded = Convert.FromBase64String(data as string);
                return decoded;
            }
        }

        public override bool isDataSupported(object data)
        {
            return data is string;
        }
    }
}