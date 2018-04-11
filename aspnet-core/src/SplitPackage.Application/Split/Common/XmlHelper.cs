using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SplitPackage.Split.Common
{
    /// <summary>
    /// XML帮助类
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// 读取Xml文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static T LoadXmlFile<T>(string filePath)
        {
            T result;
            StreamReader fs = null;
            var serializer = new XmlSerializer(typeof(T));
            try
            {
                serializer.UnknownNode += serializer_UnknownNode;
                serializer.UnknownAttribute += serializer_UnknownAttribute;

                // 读取XML文件
                fs = new StreamReader(filePath, Encoding.UTF8);

                // 反序列化为对应的对象
                result = (T)serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                //result = null;
                throw new Exception(string.Format("文件:[{0}]加载失败，错误信息:{1}", filePath, ex.Message));
                //Log.Info(string.Format("保存文件:[{0}]失败，错误信息:/r/n{1}", filePath, ex));
            }
            finally
            {
                // 解除事件绑定
                serializer.UnknownNode -= serializer_UnknownNode;
                serializer.UnknownAttribute -= serializer_UnknownAttribute;

                if (fs != null)
                {
                    // 关闭文件
                    fs.Close();
                }
            }

            //Log.Info("文件读取结束");
            return result;
        }

        /// <summary>
        /// 保存Xml文件
        /// </summary>
        /// <typeparam name="T">要保存的对象类型</typeparam>
        /// <param name="entity">要保存的对象</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否保存成功，true为保存成功，反之失败</returns>
        public static bool SaveXmlFile<T>(T entity, string filePath)
        {
            //Log.Info(string.Format("保存文件:[{0}]", filePath));

            bool result;
            var serializer = new XmlSerializer(typeof(T));
            StreamWriter write = null;
            try
            {
                write = new StreamWriter(filePath);
                serializer.Serialize(write, entity);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
                //Log.Info(string.Format("保存文件:[{0}]失败，错误信息:/r/n{1}", filePath, ex));
            }
            finally
            {
                if (write != null)
                {
                    // 关闭文件
                    write.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// 存在未知节点或者位置属性时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            if ("xsi:type".Equals(e.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            string str = string.Format("指定的XML文件中存在未知节点[{0}] 位置:[{1}]行,[{2}]列",
                                    e.Name,
                                    e.LineNumber,
                                    e.LinePosition);
            //Log.Info(str);
        }

        /// <summary>
        /// 未知属性，由于未知属性也会触发UnknownNode事件，因此本事件不做处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            if ("xsi:type".Equals(e.Attr.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            string str = string.Format("指定的XML文件中存在未知属性[{0}] 位置:[{1}]行,[{2}]列",
                                    e.Attr.Name,
                                    e.LineNumber,
                                    e.LinePosition);
            //Log.Info(str);
        }
    }
}
