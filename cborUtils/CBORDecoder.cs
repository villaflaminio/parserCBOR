using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using com.st.stcc.sdk.cbor.cborUtils.System.Half;
using com.st.stcc.sdk.cbor.cborUtils.Tags;

namespace com.st.stcc.sdk.cbor.cborUtils
{
    /// <summary>
    ///     This class provides methods to encode and decode CBOR objects.
    ///     and to read and write CBOR objects from and to streams.
    /// </summary>
    public static class CBORDecoderExtensions
    {
        /// <summary>
        ///     Decode from byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object DecodeCBORItem(this byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            CBORDecoder decode = new CBORDecoder(ms);
            return decode.ReadItem();
        }

        /// <summary>
        ///     Decode from memory stream
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static object DecodeCBORItem(this MemoryStream ms)
        {
            CBORDecoder decode = new CBORDecoder(ms);
            return decode.ReadItem();
        }

        public static object DecodeAllCBORItems(this byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            CBORDecoder decode = new CBORDecoder(ms);
            return decode.ReadAllItems();
        }


        public static object DecodeAllCBORItems(this MemoryStream ms)
        {
            CBORDecoder decode = new CBORDecoder(ms);
            return decode.ReadAllItems();
        }
        public static byte[] ToCBOR(this string val)
        {
            MemoryStream ms = new MemoryStream();

            byte[] header = new ItemHeader(MajorType.TEXT_STRING, (ulong)val.Length).ToByteArray();

            ms.Write(header, 0, header.Length);

            ms.Write(Encoding.UTF8.GetBytes(val), 0, Encoding.UTF8.GetByteCount(val));
            //Major Type 3 (MajorType.TEXT_STRING)
            return ms.ToArray();
        }
    }

  

    /// <summary>
    ///     The class deals with decoding starting from Strem or byte arrays
    /// </summary>
    /// <returns> interpretation of the cbor encoding</returns>
    public class CBORDecoder
    {
        private Stream buffer;

        public CBORDecoder(Stream s)
        {
            TagRegistry.RegisterTagTypes();
            buffer = s;
        }

        public CBORDecoder(byte[] data)
        {
            //  TagRegistry.RegisterTagTypes();
            buffer = new MemoryStream(data);
        }

        public void SetDataSource(byte[] data)
        {
            buffer = new MemoryStream(data);
        }

        public void SetDataSource(Stream s)
        {
            buffer = s;
        }

        /// <summary>
        ///     Recursively, it parses all elements
        ///     and interprets them based on the cborType
        /// </summary>
        /// <returns>
        ///     returns a dynamic object that will be modified later,
        ///     competing for a custom structure on the interpretation of the cbor encoding
        /// </returns>
        public dynamic ReadItem()
        {
            ItemHeader header = ReadHeader();
            dynamic dataItem = null;

            switch (header.majorType)
            {
                case MajorType.UNSIGNED_INT:
                    if (header.value == 0)
                        dataItem = header.additionalInfo;
                    else
                        dataItem = header.value;
                    break;
                case MajorType.NEGATIVE_INT:
                    if (header.value == 0)
                        dataItem = (long) (header.additionalInfo + 1) * -1;
                    else
                        dataItem = (long) (header.value + 1) * -1;
                    break;
                case MajorType.BYTE_STRING:
                    ulong byteLength = header.value == 0 ? header.additionalInfo : header.value;

                    byte[] bytes = new byte[byteLength];
                    for (ulong x = 0; x < byteLength; x++) bytes[x] = (byte) buffer.ReadByte();

                    dataItem = bytes;
                    break;
                case MajorType.TEXT_STRING:
                    ulong stringLength = header.value == 0 ? header.additionalInfo : header.value;

                    byte[] data = new byte[stringLength];
                    for (ulong x = 0; x < stringLength; x++) data[x] = (byte) buffer.ReadByte();

                    dataItem = Encoding.UTF8.GetString(data);
                    break;

                case MajorType.ARRAY:
                    ArrayList array = new ArrayList();
                    if (header.indefinite == false)
                    {
                        ulong elementCount = header.additionalInfo;
                        if (header.value != 0) elementCount = header.value;

                        for (ulong x = 0; x < elementCount; x++) array.Add(ReadItem());
                    }
                    else // modification of the real cbor algorithm to fit RitDb specifications
                    {
                        array.Add("t,19");
                        while (PeekBreak() == false) array.Add(ReadItem());
                        buffer.ReadByte();
                    }

                    dataItem = array;
                    break;
                case MajorType.MAP:
                    Dictionary<string, object> dict = new Dictionary<string, object>();

                    ulong pairCount = header.value == 0 ? header.additionalInfo : header.value;
                    for (ulong x = 0; x < pairCount; x++) dict.Add((string) ReadItem(), ReadItem());

                    dataItem = dict;
                    break;
                case MajorType.FLOATING_POINT_OR_SIMPLE:
                    if (header.additionalInfo < 24)
                        switch (header.additionalInfo)
                        {
                            case 20:
                                return false;
                            case 21:
                                return true;
                            case 22:
                                return null;
                            case 23:
                                return new UndefinedValue();
                        }

                    if (header.additionalInfo == 24)
                        // no simple value in range 32-255 has been defined
                        throw new Exception();

                    if (header.additionalInfo == 25)
                    {
                        Half halfValue = Half.ToHalf(BitConverter.GetBytes(header.value), 0);

                        dataItem = (float) halfValue;
                    }
                    else if (header.additionalInfo == 26)
                    {
                        // single (32 bit) precision float value
                        dataItem = BitConverter.ToSingle(BitConverter.GetBytes(header.value), 0);
                    }
                    else if (header.additionalInfo == 27)
                    {
                        // double (64 bit) precision float value
                        dataItem = BitConverter.ToDouble(BitConverter.GetBytes(header.value), 0);
                    }
                    else
                    {
                        throw new Exception();
                    }

                    // unknown simple value type
                    break;
            }

            for (int x = header.tags.Count - 1; x >= 0; x--)
                if (header.tags[x].isDataSupported(dataItem))
                    dataItem = header.tags[x].processData(dataItem);
                else
                    throw new Exception();
            return dataItem;
        }

        /// <summary>
        ///     Help to read the header of the cbor encoding
        ///     and return the major type and additional info
        /// </summary>
        /// <returns></returns>
        public List<ItemTag> ReadTags()
        {
            List<ItemTag> tags = new List<ItemTag>();

            byte b = (byte) buffer.ReadByte();

            while (b >> 5 == 6)
            {
                ulong extraInfo = (ulong) b & 0x1f;
                ulong tagNum = 0;
                if (extraInfo >= 24 && extraInfo <= 27)
                    tagNum = readUnsigned(1 << (b - 24));
                else
                    tagNum = extraInfo;
                ItemTag tag = TagRegistry.getTagInstance(tagNum);
                tags.Add(tag);
                b = (byte) buffer.ReadByte();
            }

            buffer.Seek(-1, SeekOrigin.Current);
            return tags;
        }


        public List<object> ReadAllItems()
        {
            List<object> items = new List<object>();
            while (buffer.Position < buffer.Length) items.Add(ReadItem());

            return items;
        }

        /// <summary>
        ///     Reads the header of the cbor encoding
        /// </summary>
        /// <returns></returns>
        public ItemHeader ReadHeader()
        {
            ItemHeader header = new ItemHeader();

            header.tags = ReadTags();

            ulong size = 0;
            byte b = (byte) buffer.ReadByte();
            if (b == 0xFF)
            {
                header.breakMarker = true;
                return header;
            }

            header.majorType = (MajorType) (b >> 5);

            b &= 0x1f;
            header.additionalInfo = b;
            if (b >= 24 && b <= 27)
            {
                b = (byte) (1 << (b - 24));
                header.value = readUnsigned(b);
            }
            else if (b > 27 && b < 31)
            {
                throw new Exception();
            }
            else if (b == 31)
            {
                header.indefinite = true;
            }

            return header;
        }

        /// <summary>
        ///     peek the next byte in the buffer and return true if it is a break marker
        /// </summary>
        /// <returns></returns>
        public MajorType PeekType()
        {
            long pos = buffer.Position;
            MajorType type = ReadHeader().majorType;
            buffer.Seek(pos, SeekOrigin.Begin);
            return type;
        }

        public bool PeekBreak()
        {
            long pos = buffer.Position;
            bool isBreak = ReadHeader().breakMarker;
            buffer.Seek(pos, SeekOrigin.Begin);
            return isBreak;
        }

        public ulong PeekSize()
        {
            long pos = buffer.Position;
            ItemHeader header = ReadHeader();
            ulong size = header.value != 0 ? header.value : header.additionalInfo;
            buffer.Seek(pos, SeekOrigin.Begin);
            return size;
        }

        public bool PeekIndefinite()
        {
            long pos = buffer.Position;
            bool isIndefinite = ReadHeader().indefinite;
            buffer.Seek(pos, SeekOrigin.Begin);
            return isIndefinite;
        }

        private ulong readUnsigned(int size)
        {
            byte[] buff = new byte[8];

            buffer.Read(buff, 0, size);

            Array.Reverse(buff, 0, size);

            return BitConverter.ToUInt64(buff, 0);
        }

        /// <summary>
        ///     Print on console the result of decoding.
        /// </summary>
        /// <param name="dictionary">Decoded result from cbor decoder</param>
        /// <param name="levelCount">Mapping of depth level for print formatting</param>
        /// <param name="mapNames">Map names array for printed map naming</param>
        public static string PrintDecodedValue(dynamic dictionary, int levelCount, ArrayList mapNames)
        {
            StringBuilder s = new StringBuilder();
            if (levelCount == 0)
            {
                s.Append($"Map Name: {mapNames[0]}");
                levelCount++;
            }

            foreach (dynamic kvp in dictionary)
            {
                for (int i = 0; i < levelCount; i++) s.Append("\t");
                if (kvp.Value.GetType() == typeof(Dictionary<dynamic, dynamic>))
                {
                    s.Append($"{kvp.Key}: {{ ");
                    s.Append($"Map Name: {mapNames[levelCount]}");
                    PrintDecodedValue(kvp.Value, levelCount + 1, mapNames);
                }
                else
                {
                    s.Append(string.Format("{0}: {1}", kvp.Key, kvp.Value));
                }
            }

            for (int i = 0; i < levelCount - 1; i++) s.Append("\t");
            s.Append("}");
            s.Append("\t");
            return s.ToString();
        }

        private class UndefinedValue
        {
        }
    }
}