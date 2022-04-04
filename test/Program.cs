
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Formats.Cbor;
using CBOR;
using System.Dynamic;
using System.Collections;

namespace test
{
   
    class Program
    {
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        //static async Task Main(string[] args)
        public static void main()
        {
            

            ///{"map": {"number": 42}, "item": "any string", "array": [999.0, "xyz"], "bytes": h'000102', "number": 42}
           //  string hashvalue = "A5636D6170A1666E756D626572182A646974656D6A616E7920737472696E6765617272617982F963CE6378797A65627974657343000102666E756D626572182A";

            /// ^setProperites+props=^+name=foo^     [19("setProperties"), "props", [19(null), "name", "foo"]]
           string hashvalue = "9FD36D73657450726F706572746965736570726F70739FD3F6646E616D6563666F6FFFFF";

            ///  ^event+cmd=run+task=1000    [19("event"), "cmd", "run", "task", "1000"]
           // string hashvalue = "9FD3656576656E7463636D646372756E647461736B6431303030FF";


            byte[] byteshw = StringToByteArray(hashvalue);
            //string jsonConverted = Cbor.ToJson(byteshw);
            //Console.WriteLine(jsonConverted);

            //CborReader reader = new CborReader(bytes, CborConformanceMode.Strict,false);

            //Console.WriteLine(reader.PeekState());
            //Console.WriteLine(reader.ReadStartMap());
            //Console.WriteLine(reader.PeekState());
            //Console.WriteLine(reader.ReadTextString());
            //Console.WriteLine(reader.PeekState());

            CBORDecoder decoder = new CBORDecoder(byteshw);

            Object dataParsed = decoder.ReadItem();
            
            Console.WriteLine("Type = " + dataParsed.ToString());
            var dict = new Dictionary<string, object>();



            Type typeObject = dataParsed.GetType();
            Type dictType = dict.GetType();
            Type arrayListType = typeof(ArrayList);

            if (typeObject.Equals(dictType))
            {
                Dictionary<string, object> propertyValuePaires = (Dictionary<string, object>) dataParsed;

                foreach (var group in propertyValuePaires)
                {
                    Console.WriteLine("Key: {0}          Value: {1}", group.Key, group.Value);
                }

                dynamic eo = propertyValuePaires.Aggregate(new ExpandoObject() as IDictionary<string, Object>,
                                (a, p) => { a.Add(p.Key, p.Value); return a; });


                Console.WriteLine(eo);
            }else if (typeObject.Equals(arrayListType))
            {
                ArrayList arr = new ArrayList((ArrayList)dataParsed);

                foreach (object i in arr)
                {
                    Type typeI = i.GetType();

                    //if (typeI.Equals(arrayListType))
                    //{

                    //    foreach (object k in i)
                    //    {
                    //        Console.WriteLine(k);

                    //    }
                    //}
                        Console.WriteLine(i);
                }

            }

        }

        //var cbor = CBORObject.NewMap()
        //    .Add("item", "any string")
        //    .Add("number", 42)
        //    .Add("map", CBORObject.NewMap().Add("number", 42))
        //    .Add("array", CBORObject.NewArray().Add(999f).Add("xyz"))
        //    .Add("bytes", new byte[] { 0, 1, 2 });

        //// The following converts the map to CBOR
        //byte[] bytes = cbor.EncodeToBytes();

        //string base64 = Convert.ToHexString(bytes);

        //string json = cbor.ToJSONString();
        //Console.WriteLine(base64);
        //Console.WriteLine(json);



        //string hashvalue = "9FD36D73657450726F706572746965736570726F70739FD3F6646E616D6563666F6FFFFF";
        //byte[] byteshw = StringToByteArray(hashvalue);

        //using (var stream = new MemoryStream(byteshw))
        //{

        //    var cbor = CBORObject.Read(stream);
        //    Console.WriteLine(cbor.ToJSONString());

        //}


    }
}
