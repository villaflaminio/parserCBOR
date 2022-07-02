using System;
using System.Collections.Generic;
using System.Reflection;

namespace com.st.stcc.sdk.cbor.cborUtils.Tags
{
    public class TagRegistry
    {
        public static Dictionary<ulong, Type> tagMap = new Dictionary<ulong, Type>();
        public static bool isInit;

        public static void RegisterTagTypes()
        {
            if (!isInit)
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type type in asm.GetTypes())
                    if (type.BaseType == typeof(ItemTag))
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

            isInit = true;
        }

        public static ItemTag getTagInstance(ulong tagId)
        {
            if (tagMap.ContainsKey(tagId))
                return (ItemTag) Activator.CreateInstance(tagMap[tagId], tagId);
            return new UnknownTag(tagId);
        }

        internal static void registerTag(ulong p, Type type)
        {
            if (tagMap.ContainsKey(p) == false) tagMap.Add(p, type);
        }
    }
}