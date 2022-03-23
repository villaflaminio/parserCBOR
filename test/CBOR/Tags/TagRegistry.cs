using System;
using System.Collections.Generic;

namespace CBOR.Tags
{
    public class TagRegistry
    {
        public static Dictionary<ulong,Type> tagMap = new Dictionary<ulong, Type>();
        public static bool isInit = false;

        public static void RegisterTagTypes()
        {
            if (!isInit)
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in asm.GetTypes())
                    {
                        if (type.BaseType == typeof (ItemTag))
                        {
                            try
                            {
                                ////ulong[] tagNum = (ulong[]) type.GetField("TAG_NUM").GetValue(null);

                                //foreach (ulong l in tagNum)
                                //{
                                //    tagMap.Add(l, type);
                                //}
                            }
                            catch (Exception)
                            {
                            }

                        }
                    }
                }
            }
            isInit = true;
        }

        public static ItemTag getTagInstance(ulong tagId)
        {
            if (tagMap.ContainsKey(tagId))
            {
                return (ItemTag) Activator.CreateInstance(tagMap[tagId], tagId);
            }
            else
            {
                return new UnknownTag(tagId);
            }
            
        }

        internal static void registerTag(ulong p, Type type)
        {
            if (tagMap.ContainsKey(p) == false)
            {
                tagMap.Add(p,type);
            }
        }
    }
}