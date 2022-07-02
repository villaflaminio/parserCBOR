using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using com.st.stcc.configuration.model;
using com.st.stcc.sdk.cbor.cborUtils;
using com.st.stcc.sdk.cbor.logging;
using log4net;

namespace com.st.stcc.sdk.cbor
{
    public class CborManager : ICborManager
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CborManager));

        private static CborManager _instance;
        public event EventHandler<STCCMQTTMessage> CborMessageDecoded;
        public event EventHandler<Exception> CborMessageDecodingError;
        public event EventHandler<string> CborMessageEncoded;
        public event EventHandler<Exception> CborMessageEncodingError;

        #region Component constructor e singleton pattern

        private CborManager()
        {
        }

        public static CborManager GetInstance()
        {
            try
            {
                // Logging method enter
                _logger.Debug("Enter method CborManager.GetInstance");
                if (_instance == null) _instance = new CborManager();
                return _instance;
            }
            catch (Exception error)
            {
                _logger.Error(error);
                throw error;
            }

        }

        #endregion

        #region event of CborManager

        protected virtual void CborMessageDecodedEvent(STCCMQTTMessage message)
        {
            try
            {
                // Logging method enter
                _logger.Debug("Enter method CborManager.CborMessageDecodedEvent");
                _logger.Trace("Received parameters: data: " + message);

                CborMessageDecoded?.Invoke(this, message);


            }
            catch (Exception error)
            {
                _logger.Error(error);
            }
        }
        protected virtual void CborMessageDecodingErrorEvent(Exception exception)
        {
            try
            {
                // Logging method enter
                _logger.Debug("Enter method CborManager.CborMessageDecodingErrorEvent");
                _logger.Trace(string.Format("Received parameters: eventArgs: {0}", exception.Message));

                CborMessageDecodingError?.Invoke(this, exception);

            }
            catch (Exception error)
            {
                _logger.Error(error);
            }

        }
        protected virtual void CborMessageEncodedEvent(string message)
        {
            try
            {
                // Logging method enter
                _logger.Debug("Enter method CborManager.CborMessageEncoded");
                _logger.Trace(string.Format("Received parameters: eventArgs: {0}", message));
                CborMessageEncoded?.Invoke(this, message);

            }
            catch (Exception error)
            {
                _logger.Error(error);
            }

        }
        protected virtual void CborMessageEncodingErrorEvent(Exception exception)
        {
            try
            {
                // Logging method enter
                _logger.Debug("Enter method CborManager.MqttMessageQueuedEvent");
                _logger.Trace(string.Format("Received parameters: eventArgs: {0}", exception.Message));
                CborMessageEncodingError?.Invoke(this, exception);

            }
            catch (Exception error)
            {
                _logger.Error(error);
            }

        }

        #endregion

        #region method of CborManager
        static string BytesToString(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
        /// <summary>
        ///     Encrypts data into CBOR encode.
        /// </summary>
        /// <param name="data">Data we want to encode in CBOR standard</param>
        /// <returns>The string that represent encrypted data</returns>
        public string Encode(Dictionary<string, object> data)
        {
            try
            {
                _logger.Debug("Enter method CborManager.Encode");
                _logger.Trace("Received parameters: data: " + string.Join(Environment.NewLine, data.Select(a => $"{a.Key}: {a.Value}")));

                object entityNameValue = data["ENTITY_NAME"];
                data.Remove("ENTITY_NAME");

                ArrayList list = new ArrayList();
                list.Add(entityNameValue);
                list.Add(DictionaryToArrayList(data));

                byte[] dataSerialized = CborSerializer.Serialize(list);
                String bitString = BitConverter.ToString(dataSerialized);
                bitString = bitString.Replace("-", "");
                bitString = bitString.Remove(0, 4);
                bitString = "9FD3" + bitString + "FF";
                    
                return bitString;
            }
            catch (Exception error)
            {
                _logger.Error(error);
                CborMessageEncodingErrorEvent(error);
                throw error;
            }


        }


        /// <summary>
        ///     Decrypts Mqtt Cbor encoded message
        /// </summary>
        /// <param name="cborString">The MessageMqtt object that contains the CBOR encrypted message</param>
        /// <returns>It returns a Dictionary object with the decrpyted message</returns>
        public Dictionary<string, object> Decode(STCCMQTTMessage message)
        {
            try
            {
                _logger.Debug("Enter method CborManager.Decode");
                _logger.Trace(string.Format("Received parameters: cborString: {0}", message.Payload));


                byte[] byteshw = StringToByteArray(message.Payload);
                CBORDecoder decoder = new CBORDecoder(byteshw);
                Dictionary<string, object> parsedValue = ArrayListIntoDictionary((ArrayList)decoder.ReadItem());
                message.DecodedData = parsedValue;
                CborMessageDecodedEvent(message);

                return parsedValue;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                CborMessageDecodingErrorEvent(e);
                throw e;
            }

        }

        /// <summary>
        ///    Converts an ArrayList into a Dictionary object
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private ArrayList DictionaryToArrayList(Dictionary<string, object> dictionary)
        {
            try
            {
                _logger.Debug("Enter method CborManager.DictionaryToArrayList");
                _logger.Trace(string.Format("Received parameters: dictionary: {0}", dictionary));
                ArrayList arrayList = new ArrayList();
                foreach (KeyValuePair<string, object> pair in dictionary)
                {
                    _logger.Trace("Converted data: " + string.Format("Key: {0} Value: {1}", pair.Key, pair.Value));
                    if (pair.Key.Equals("ENTITY_NAME"))
                    {
                        _logger.Debug("Tag 19 , start of map");
                        arrayList.Add("t,19");
                        arrayList.Add(pair.Value);
                    }
                    else
                    {
                        arrayList.Add(pair.Key);
                        arrayList.Add(pair.Value);
                    }
                }
                _logger.Debug("Exit method CborManager.DictionaryToArrayList");
                _logger.Trace(string.Format("Returned value: {0}", string.Join(",", arrayList)));
                return arrayList;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw e;
            }
        }


        /// <summary>
        /// Method that convert an ArrayList into a dictionary.
        /// </summary>
        /// <param name="arrayList"> the cbor message as arraylist object </param>
        /// <param name="dict">the dictionary of the cbor message</param>
        private Dictionary<string, object> ArrayListIntoDictionary(ArrayList arrayList)
        {
            try
            {
                // Logging method enter
                _logger.Debug("Enter method CborManager.ArrayListIntoDictionary");
                _logger.Trace(string.Format("Received parameters: arrayList: {0}", string.Join(",", arrayList)));

                // Create the dictionary to return the result.
                Dictionary<string, object> dict = new Dictionary<string, object>();

                // Cycle each element of the arrayList to convert it into a key-value pair.
                // We skip the object at 0-index because it always be "t, 19", and we don't care about.
                for (int i = 0; i < arrayList.Count; i++)
                {
                    // The element at position 1 will be the ENTITY_NAME, so we create the key-value pair correctly, then add it into the dictionary.
                    if (i == 1)
                    {
                        _logger.Trace(string.Format("Added element to the dictionary: key: {0}, value: {1}", "ENTITY_NAME", arrayList[i]));
                        dict.Add("ENTITY_NAME", arrayList[i]);
                    }

                    // All the values are into even position, so we retrieve them and link with the previous element, their key.
                    if (i != 0 && i % 2 == 0)
                    {
                        _logger.Trace(string.Format("Added element to the dictionary: key: {0}, value: {1}", (string)arrayList[i], arrayList[i + 1]));
                        dict.Add((string)arrayList[i], arrayList[i + 1]);
                    }
                }
                // Log result.
                _logger.Trace(String.Format("Return {0}", string.Join(Environment.NewLine, dict.Select(a => $"{a.Key}: {a.Value}"))));
                _logger.Debug("End method DBManager.ArrayListIntoDictionary");
                return dict;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw e;
            }
        }

        /// <summary>
        ///     Transform hexdecimal string to ByteArray
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string hexadecimalValue)
        {
            try
            {
                _logger.Debug("Enter method CborManager.StringToByteArray");
                _logger.Trace(string.Format("Received parameters: hexadecimalValue: {0}", hexadecimalValue));


                return Enumerable.Range(0, hexadecimalValue.Length)
                                .Where(x => x % 2 == 0)
                                .Select(x => Convert.ToByte(hexadecimalValue.Substring(x, 2), 16))
                                .ToArray();
            }
            catch (Exception error)
            {
                _logger.Error(error);
                throw error;
            }

        }

        public static byte[] FileToByteArray(string filePath)
        {
            try
            {
                _logger.Debug("Enter method CborManager.FileToByteArray");
                _logger.Trace(string.Format("Received parameters: filePath: {0}", filePath));

                return File.ReadAllBytes(filePath);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw e;
            }
        }

        #endregion


    }
}