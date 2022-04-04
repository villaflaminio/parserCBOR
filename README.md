# CBOR

Created: March 17, 2022
Created by: flaminio villa
Tags: c#

Concise Binary Object Representation (**CBOR**) è un formato di serializzazione di dati binari liberamente basato su JSON.
Come JSON, consente la trasmissione di oggetti dati che contengono coppie nome-valore, ma in modo più estensibile, conciso,formato codificato binario. Ciò aumenta la velocità di elaborazione e trasferimento a scapito della leggibilità umana. È definito in m=IETF RFC 7049. 

Poiché l'obiettivo di RITdb è da macchina a macchina e non da persone, la scelta di un formato binario è adeguata. 

CBOR è diventato un protocollo supportato per IOT con supporto per la maggior parte dei linguaggi moderni.

^ seguito da nome avvia una mappa, + separa chiave=valore e ~ separa gli elementi dell'array.

### Examples

text representation → ^event+cmd=run+task=1000

hex representation → 9FD3656576656E7463636D646372756E647461736B6431303030FF

[19("event"), "cmd", "run", "task", "1000"]

miei test:

## Encode

```csharp
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
```

```csharp
Console.WriteLine:

A5636D6170A1666E756D626572182A646974656D6A616E7920737472696E6765617272617982F963CE6378797A65627974657343000102666E756D626572182A

{"map":{"number":42},"item":"any string","array":[999,"xyz"],"bytes":"AAEC","number":42}
```

## Decode

```csharp
string hashvalue = "A5636D6170A1666E756D626572182A646974656D6A616E7920737472696E6765617272617982F963CE6378797A65627974657343000102666E756D626572182A";
byte[] byteshw = StringToByteArray(base64);
//var cbor2 = CBORObject.DecodeSequenceFromBytes(bytes);
//var cbortr = CBORObject.DecodeSequenceFromBytes(byteshw, CBOREncodeOptions.Default);
string jsonConverted = Cbor.ToJson(byteshw);
Console.WriteLine(jsonConverted);
```

```csharp
Console.WriteLine:

{"map":{"number":42},"item":"any string","array":[999,"xyz"],"bytes":h'000102',"number":42}
```

Succesivamente è stata implementata una struttura di traduzione e decodifica interna, e utilizzando i Dynamic si ha piena libertà della struttura ottenuta.

 

```csharp
string hashvalue = "9FD3656576656E7463636D646372756E647461736B6431303030FF";
byte[] byteshw = StringToByteArray(hashvalue);

CBORDecoder decoder = new CBORDecoder(byteshw );
dynamic dataParsed = decoder.ReadItem();

```

Per comodità è possibile converitre il dynamic in json

```csharp
string dataSerialized = JsonSerializer.Serialize(dataParsed);
Console.WriteLine(dataSerialized);
```

Il flusso codificato in CBOR può essere decodificato anche da Streamç

```csharp
byte[] bytefile = FileToByteArray(@"C:\Users\file.cbor");
using (var stream = new MemoryStream(bytefile))
            {
                // Read the CBOR object from the stream
                CBORDecoder decoder = new CBORDecoder(stream );
								dynamic dataParsed = decoder.ReadItem();
             }
```

Altro esempio di decodifica può essere rappresentato dal casting da dynamic a specifico object

```csharp
byte[] bytefile = FileToByteArray(@"C:\Users\file.cbor");
CBORDecoder decoder = new CBORDecoder(bytefile);
dynamic dataParsed = decoder.ReadItem();
string dataSerialized = JsonSerializer.Serialize(dataParsed);

DeserializedObject deserializedObject = JsonSerializer.Deserialize<DeserializedObject>(dataSerialized)!;
Rootobject deserializedObject = JsonSerializer.Deserialize<Rootobject>(dataSerialized)!;

foreach (dynamic MyDynamicVar in dataParsed)
{
    Console.WriteLine("Value: {0}, Type: {1}", MyDynamicVar, MyDynamicVar.GetType());

}
IDictionary<string, object> propertyValues = (IDictionary<string, object>)dataParsed;

foreach (KeyValuePair<string, object> kvp in propertyValues)
{
    Console.WriteLine("Key = {0}, Value = {1}",
        kvp.Key, kvp.Value);
}

Rootobject theObject = (Rootobject)dataParsed;

```

le classi di riferimento sono 

```csharp
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
```
