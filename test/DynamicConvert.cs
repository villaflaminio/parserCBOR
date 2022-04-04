using CBOR;
using PeterO.Cbor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace test
{
    class DynamicConvert
    {
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        public static byte[] FileToByteArray(string fileName)
        {

            return File.ReadAllBytes(fileName);
        }

       

        static async Task Main(string[] args)
        {

            //using (var stream = new FileStream(@"C:\Users\f.villa\Downloads\ddd\TCC_MGUI_T2K_XTRF.cbor", FileMode.Open))
            //{
            //    CBORDecoder decoder = new CBORDecoder(stream);

            //    dynamic dataParsed = decoder.ReadItem();

            //    string dataSerialized = JsonSerializer.Serialize(dataParsed);
            //    Console.WriteLine(dataSerialized);
            //}




            ///{"map": {"number": 42}, "item": "any string", "array": [999.0, "xyz"], "bytes": h'000102', "number": 42}
            //string hashvalue = "A5636D6170A1666E756D626572182A646974656D6A616E7920737472696E6765617272617982F963CE6378797A65627974657343000102666E756D626572182A";

            /// ^setProperites+props=^+name=foo^     [19("setProperties"), "props", [19(null), "name", "foo"]]
            //string hashvalue = "9FD36D73657450726F706572746965736570726F70739FD3F6646E616D6563666F6FFFFF";

            ///  ^event+cmd=run+task=1000    [19("event"), "cmd", "run", "task", "1000"]
            string hashvalue = "9FD3656576656E7463636D646372756E647461736B6431303030FF";

            //string hashvalue = "9FD3656576656E7463636D646372756E647461736B9fD3616561669fD3616561666431303030FFFF656576656E749fD3616561666431303030FFFF";

            byte[] byteshw = StringToByteArray(hashvalue);
            byte[] bytefile = FileToByteArray(@"C:\Users\f.villa\Downloads\ddd\TCC_MGUI_T2K_XTRF.cbor");
            //Console.WriteLine("Byte Array is: " + String.Join(" ", byteshw));

            CBORDecoder decoder = new CBORDecoder(bytefile);

            dynamic dataParsed = decoder.ReadItem();

            string dataSerialized = JsonSerializer.Serialize(dataParsed);
            Console.WriteLine(dataSerialized);

            //-----------utilizzando la libreria di cbor

            using (var stream = new MemoryStream(bytefile))
            {
                // Read the CBOR object from the stream
                var cbor = CBORObject.Read(stream);
                string json = cbor.ToJSONString();
                Console.WriteLine(json);
                // The rest of the example follows the one given above.
            }




            //-----------------Deserializzazione in classi

            //DeserializedObject deserializedObject = JsonSerializer.Deserialize<DeserializedObject>(dataSerialized)!;

            //Console.WriteLine(deserializedObject.bytes);

            //Rootobject deserializedObject = JsonSerializer.Deserialize<Rootobject>(dataSerialized)!;

            //Console.WriteLine(deserializedObject.Property1);


            //Console.WriteLine(obj);
            //foreach (dynamic MyDynamicVar in dataParsed)
            //{
            //    Console.WriteLine("Value: {0}, Type: {1}", MyDynamicVar, MyDynamicVar.GetType());

            //}
            //IDictionary<string, object> propertyValues = (IDictionary<string, object>) dataParsed;

            //foreach (KeyValuePair<string, object> kvp in propertyValues)
            //{
            //    Console.WriteLine("Key = {0}, Value = {1}",
            //        kvp.Key, kvp.Value);
            //}

            //Rootobject theObject = (Rootobject)dataParsed;
            //Console.WriteLine(theObject);

        }

    }


    public class Rootobject
    {
        public object[] Property1 { get; set; }
    }

    public class DeserializedObject
    {
        public Map map { get; set; }
        public string item { get; set; }
        public object[] array { get; set; }
        public string bytes { get; set; }
        public int number { get; set; }
    }

    public class Map
    {
        public int number { get; set; }
    }

}
