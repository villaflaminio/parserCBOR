﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBOR.Tags
{
    class CBORItemTag : ItemTag
    {
        public static ulong[] TAG_NUM = new ulong[] { 24 };

        public CBORItemTag(ulong tagNum)
        {
            this.tagNumber = tagNum;
        }
        public override object processData(object data)
        {
            CBORDecoder decoder = new CBORDecoder((byte[])data);
            return decoder.ReadItem();
        }

        public override bool isDataSupported(object data)
        {
            return (data is byte[]);
        }
    }
}
