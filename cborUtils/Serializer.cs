using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace com.st.stcc.sdk.cbor.cborUtils
{
    /// <summary>
    ///     The cbor serializer.
    /// </summary>
    public class CborSerializer
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The serialize.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="byte[]" />.
        /// </returns>
        public static byte[] Serialize(object obj)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                WriteObjectToStream(obj, memory);
                return memory.ToArray();
            }
        }

        /// <summary>
        ///     The write list.
        /// </summary>
        /// <param name="list">
        ///     The list.
        /// </param>
        /// <param name="writer">
        ///     The writer.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public static void WriteList<T>(List<T> list, CborWriter writer)
        {
            foreach (T element in list) WriteObject(element, writer);
        }

        /// <summary>
        ///     The write map.
        /// </summary>
        /// <param name="dictionary">
        ///     The dictionary.
        /// </param>
        /// <param name="writer">
        ///     The writer.
        /// </param>
        /// <typeparam name="K">
        /// </typeparam>
        /// <typeparam name="V">
        /// </typeparam>
        public static void WriteMap<K, V>(Dictionary<K, V> dictionary, CborWriter writer)
        {
            foreach (KeyValuePair<K, V> v in dictionary)
            {
                WriteObject(v.Key, writer);
                WriteObject(v.Value, writer);
            }
        }

        /// <summary>
        ///     The write object.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="writer">
        ///     The writer.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public static void WriteObject(dynamic obj, CborWriter writer)
        {
            if (obj == null)
            {
                writer.WriteNull();
                return;
            }

            Dictionary<string, object> dict = new Dictionary<string, object>();
            Type typeObject = obj.GetType();
            Type dictType = dict.GetType();
            Type arrayListType = typeof(ArrayList);

            // logger.debug("write some object: {0}", obj);
            if (obj == null)
            {
                writer.Write(22);
                return;
            }

            byte[] objBytes = obj as byte[];
            if (objBytes != null)
            {
                writer.Write(objBytes);
                return;
            }

            Array objArray = obj as Array;
            if (objArray != null)
            {
                writer.Write(objArray.Length);
                foreach (object element in objArray) WriteObject(element, writer);

                return;
            }

            string objString = obj as string;
            if (objString != null)
            {
                writer.Write(objString);
                return;
            }

            if (obj is bool)
            {
                writer.Write((bool) obj ? 21u : 20u);
                return;
            }

            if (obj is uint)
            {
                writer.Write((uint) obj);
                return;
            }

            if (obj is int || obj.GetType().IsEnum)
            {
                writer.Write((int) obj);
                return;
            }

            if (obj is ulong)
            {
                writer.Write((ulong) obj);
                return;
            }

            if (obj is long)
            {
                writer.Write((long) obj);
                return;
            }

            if (obj is double)
            {
                writer.Write((double) obj);
                return;
            }

            if (obj is float)
            {
                writer.Write((float) obj);
                return;
            }

            if (typeObject.Equals(dictType))
            {
                Dictionary<string, object> propertyValuePaires = (Dictionary<string, object>) obj;
                writer.BeginCollection(CBORMajorType.Map, propertyValuePaires.Count);

                //writer.WriteMap();
                foreach (KeyValuePair<string, object> entry in propertyValuePaires)
                {
                    WriteObject(entry.Key, writer);
                    WriteObject(entry.Value, writer);
                }

                writer.EndCollection();


                return;
            }

            IList objList = obj as IList;
            if (objList != null)
            {
                writer.BeginCollection(CBORMajorType.Array, objList.Count);
                //writer.WriteArray(objList.Count);
                foreach (object element in objList) WriteObject(element, writer);
                writer.EndCollection();
            }
        }

        /// <summary>
        ///     The write object to stream.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="output">
        ///     The output.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public static void WriteObjectToStream(object obj, Stream output)
        {
            using (CborWriter writer = new CborWriter(output))
            {
                WriteObject(obj, writer);
            }
        }

        #endregion
    }
}