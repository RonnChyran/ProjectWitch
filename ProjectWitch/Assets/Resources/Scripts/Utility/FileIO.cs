using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace ProjectWitch
{
    static class FileIO
    {
        //任意のオブジェクトをxmlにシリアライズする
        public static void ObjToXML<T>(Stream ofs, T data)
        {
            var writer = new StreamWriter(ofs, Encoding.UTF8);

            //xml名前空間のインスタンス
            var outNameSpace = new XmlSerializerNamespaces();
            outNameSpace.Add(string.Empty, string.Empty);

            //シリアライザのインスタンスを作成
            var outSerializer = new XmlSerializer(typeof(T));

            //オブジェクトをシリアライズ
            outSerializer.Serialize(writer, data, outNameSpace);

            //ストリームを書き込み
            writer.Flush();

            writer.Close();
        }

        //任意のオブジェクトをxmlからデシリアライズする
        public static void ObjFromXML<T>(Stream ifs, out T data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            data = (T)serializer.Deserialize(ifs);
        }

        //任意のオブジェクトをxmlにシリアライズしてファイルに保存
        public static void SaveXML<T>(string filepath, T data)
        {
            FileStream ofs = new FileStream(filepath, FileMode.Create);
            ObjToXML(ofs, data);
        }

        //任意のオブジェクトをファイルのxmlからデシリアライズする
        public static void LoadXML<T>(string filepath, out T data)
        {
            FileStream ifs = new FileStream(filepath, FileMode.Open);
            ObjFromXML(ifs, out data);
        }

        //データをxml形式の文字列に変換
        public static string ObjectToXML<T>(T data)
        {
            var stream = new MemoryStream();
            ObjToXML(stream, data);

            return Encoding.Unicode.GetString(stream.ToArray());
        }

        //xml形式の文字列からオブジェクトを復元
        public static void XMLToObject<T>(string data, out T result)
        {
            Stream stream = new MemoryStream(Encoding.Unicode.GetBytes(data));
            ObjFromXML(stream, out result);

        }
    }
}
