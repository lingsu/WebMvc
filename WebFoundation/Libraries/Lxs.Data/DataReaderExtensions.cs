using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lxs.Data
{
    public static class DataReaderExtensions
    {
        /// <summary>
        /// 创建一个给定类型的列表 DataReader
        /// 
        /// 注意这种方法使用反射,所以这不是高性能的
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="reader"></param>
        /// <param name="fieldsToSkip"></param>
        /// <param name="piList"></param>
        /// <returns></returns>
        public static List<TType> DataReaderToObjectList<TType>(this IDataReader reader, string fieldsToSkip = null, Dictionary<string, PropertyInfo> piList = null)
            where TType : new()
        {
            if (reader == null)
                return null;

            var items = new List<TType>();

            // Create lookup list of property info objects            
            if (piList == null)
            {
                piList = new Dictionary<string, PropertyInfo>();
                var props = typeof(TType).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                    piList.Add(prop.Name.ToLower(), prop);
            }

            while (reader.Read())
            {
                var inst = new TType();
                DataReaderToObject(reader, inst, fieldsToSkip, piList);
                items.Add(inst);
            }

            return items;
        }
        /// <summary>
        /// 填充对象的属性从一个使用DataReader行
        /// 
        /// 反射匹配DataReader公共字段上
        /// 传入对象，性能保持不变。
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="instance"></param>
        /// <param name="fieldsToSkip"></param>
        /// <param name="piList"></param>
        public static void DataReaderToObject(this IDataReader reader, object instance, string fieldsToSkip = null, Dictionary<string, PropertyInfo> piList = null)
        {
            if (reader.IsClosed)
                throw new InvalidOperationException("Data reader cannot be used because it's already closed");

            if (string.IsNullOrEmpty(fieldsToSkip))
                fieldsToSkip = string.Empty;
            else
                fieldsToSkip = "," + fieldsToSkip + ",";

            fieldsToSkip = fieldsToSkip.ToLower();

            // create a dictionary of properties to look up
            // we can pass this in so we can cache the list once 
            // for a list operation 
            if (piList == null)
            {
                piList = new Dictionary<string, PropertyInfo>();
                var props = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                    piList.Add(prop.Name.ToLower(), prop);
            }

            for (int index = 0; index < reader.FieldCount; index++)
            {
                string name = reader.GetName(index).ToLower();
                if (piList.ContainsKey(name))
                {
                    var prop = piList[name];

                    if (fieldsToSkip.Contains("," + name + ","))
                        continue;

                    if ((prop != null) && prop.CanWrite)
                    {
                        var val = reader.GetValue(index);
                        prop.SetValue(instance, (val == DBNull.Value) ? null : val, null);
                    }
                }
            }

            return;
        }
    }
}
