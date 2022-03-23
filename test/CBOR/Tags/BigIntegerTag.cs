﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
namespace CBOR.Tags
{
    public class BigIntegerTag : ItemTag
    {
        public static ulong[] TAG_NUM = new ulong[] {2,3};
        public BigIntegerTag(ulong tagId)
        {
            this.tagNumber = tagId;
        }

        public override object processData(object data)
        {
            Array.Reverse((Array)data);
            BigInteger bi = new BigInteger((byte[])data);

            if (this.tagNumber == 2)
            {
                return bi;
            } else
            {
                return BigInteger.Subtract(BigInteger.MinusOne, bi);
            }
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
