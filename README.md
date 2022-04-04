# parserCBOR
Types
•	6 followed by a number define a text and his length
•	8 followed by a number define an array and his length
•	9F start indefinite length array. The array has to be break with FF. It can contains every item you want before FF
•	5F start indefinite length byte string
•	BF start indefinite length map. The map has to be break with FF. It can contains every item you want before FF
•	4 followed by a number define a byte string and his length
											 | 20      | False           |
                       |         |                 |
                       | 21      | True            |
                       |         |                 |
                       | 22      | Null            |
                       |         |                 |
                       | 23      | Undefined value |
                       |         |                 |
                       | 24..31  | (Reserved)      |

With the PeterO.Cbor library it is possible to generate and encode cbor files in a simple way:
Encode
//

var cbor = CBORObject.NewMap()
                .Add("item", "any string")
                .Add("number", 42)
                .Add("map", CBORObject.NewMap().Add("number", 42))
                .Add("array", CBORObject.NewArray().Add(999f).Add("xyz"))
                .Add("bytes", new byte[] { 0, 1, 2 });

            // The following converts the map to CBOR
            byte[] bytes = cbor.EncodeToBytes();

            string base64 = Convert.ToHexString(bytes);

            string json = cbor.ToJSONString();
            Console.WriteLine(base64);
            Console.WriteLine(json);
Console.WriteLine:

A5636D6170A1666E756D626572182A646974656D6A616E7920737472696E6765617272617982F963CE6378797A65627974657343000102666E756D626572182A

{
   "map":{
      "number":42
   },
   "item":"any string",
   "array":[
      999,
      "xyz"
   ],
   "bytes":"h""000102",
   "number":42
}

Decode
public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
...
string hashvalue = "A5636D6170A1666E756D626572182A646974656D6A616E7920737472696E6765617272617982F963CE6378797A65627974657343000102666E756D626572182A";
byte[] byteshw = StringToByteArray(base64);
string jsonConverted = Cbor.ToJson(byteshw);
Console.WriteLine(jsonConverted);
Console.WriteLine:

{
   "map":{
      "number":42
   },
   "item":"any string",
   "array":[
      999,
      "xyz"
   ],
   "bytes":"h""000102",
   "number":42
}

Detail of types according to CBOR coding (RFC 7049)

