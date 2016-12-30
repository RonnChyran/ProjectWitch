using System.Linq;
using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch
{
    [Serializable]
    public class FileVersion
    {
        [System.Xml.Serialization.XmlElement("major")]
        public byte Major { get; set; }
        [System.Xml.Serialization.XmlElement("minor")]
        public byte Minor { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            FileVersion v = (FileVersion)obj;
            return (Major == v.Major) && (Minor == v.Minor);
        }

        public override string ToString()
        {
            return " Major:" + Major.ToString() +
                   " Minor:" + Minor.ToString();
        }

        public override int GetHashCode()
        {
            return Major ^ Minor;
        }
    }

    static class FileIO
    {
        //ファイルのフォーマット
        public enum Format { Text, Binary }

        //フォーマットと拡張子の対応付け
        private static Dictionary<Format, string> mExt =
            new Dictionary<Format, string>()
            {
                {Format.Text, ".xml" },
                {Format.Binary,".dat" }
            };

        //任意のオブジェクトをxmlにシリアライズしてファイルに保存
        public static void SaveXML(string filepath, FileVersion version, Format format, ISaveableData data)
        {
            filepath += mExt[format];

            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                switch (format)
                {
                    case Format.Text:
                        ObjToText(fs, version, data);
                        break;
                    case Format.Binary:
                        SaveableDataToBibary(fs, data);
                        break;
                    default:
                        break;
                }
            }
        }

        //任意のオブジェクトをファイルのxmlからデシリアライズする
        public static void LoadXML<T>(string filepath, FileVersion dataversion, Format format, ref T data)
            where T : ISaveableData
        {
            filepath += mExt[format];

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                switch (format)
                {
                    case Format.Text:
                        ObjFromText(fs, dataversion, ref data);
                        break;
                    case Format.Binary:
                        SaveableDataFromBinary(fs, dataversion, ref data);
                        break;
                    default:
                        data = default(T);
                        break;
                }
            }
        }

        //任意のオブジェクトをテキストxmlにシリアライズする
        private static void ObjToText<T>(Stream stream, FileVersion version, T data)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                try
                {
                    //xml名前空間のインスタンス
                    var outNameSpace = new XmlSerializerNamespaces();
                    outNameSpace.Add(string.Empty, string.Empty);

                    //シリアライザのインスタンスを作成
                    var outSerializer = new XmlSerializer(typeof(T));

                    //オブジェクトをシリアライズ
                    outSerializer.Serialize(writer, data, outNameSpace);

                    //ストリームを書き込み
                    writer.Flush();
                }
                catch (Exception e)
                {
                    Debug.Log("ObjToTextに失敗しました。");
                    Debug.Log("Message:" + e.ToString());
                    return;
                }
            }
        }

        //任意のオブジェクトをxmlからデシリアライズする
        private static void ObjFromText<T>(Stream stream, FileVersion dataversion, ref T data)
        {
            using (var xmlReader = new XmlTextReader(stream))
            {
                try
                {
                    //バージョンのロード
                    var fileversion = new FileVersion();
                    xmlReader.Read();

                    //version要素まで飛ぶ
                    xmlReader.ReadStartElement("root_data");
                    xmlReader.ReadStartElement("version");

                    //メジャーバージョンの読出し
                    xmlReader.ReadStartElement("major");
                    fileversion.Major = byte.Parse(xmlReader.ReadString());
                    xmlReader.ReadEndElement();

                    //マイナーバージョンの読出し
                    xmlReader.ReadStartElement("minor");
                    fileversion.Minor = byte.Parse(xmlReader.ReadString());
                    xmlReader.ReadEndElement();

                    xmlReader.ReadEndElement();

                    //バージョンが違った場合はエラーとする
                    if (!dataversion.Equals(fileversion))
                    {
                        Debug.LogError(
                            "ロードファイルのバージョンが実行ファイルのバージョンと違います。" +
                            "PWSaveConverterを使って、バージョンを現在のものに合わせてください。" +
                            " dataversion : " + dataversion.ToString() +
                            " fileversion : " + fileversion.ToString()
                            );
                        data = default(T);
                        return;
                    }

                    //読出し位置を先頭に戻す
                    stream.Seek(0, SeekOrigin.Begin);

                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    data = (T)serializer.Deserialize(stream);
                }
                catch (Exception e)
                {
                    Debug.Log("ObjFromXMLに失敗しました。");
                    Debug.Log("Message:" + e.ToString());
                    data = default(T);
                    return;
                }
            }
        }

        ////任意のオブジェクトをバイナリにシリアライズする
        //private static void ObjToBinary<T>(Stream stream, FileVersion version, T data)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        try
        //        {
        //            //バージョン情報の定義
        //            //stream.Write(new byte[] { 1, 0 }, 0, 2);
        //            //stream.Flush();

        //            //バイナリフォーマッタからシリアライズする
        //            var bf = new BinaryFormatter();
        //            bf.Serialize(ms, data);
        //            ms.Flush();

        //            //メモリストリームをコピー(.NET4.0以降ならCopyToメソッドで代用可能)
        //            ms.WriteTo(stream);

        //            //フラッシュ
        //            stream.Flush();
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.Log("ObjToBinaryに失敗しました。");
        //            Debug.LogError("Message:" + e.ToString());
        //            return;
        //        }
        //    }
        //}

        ////任意のオブジェクトをバイナリからデシリアライズする
        //private static void ObjFromBinary<T>(Stream stream, FileVersion dataversion, out T data)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        try
        //        {
        //            //バイナリデータを読み出す
        //            var tmpBuf = new byte[stream.Length];
        //            stream.Read(tmpBuf, 0, tmpBuf.Length);

        //            //バージョン情報を読み出す
        //            //byte[] versionBuf = new byte[2];
        //            //stream.Read(versionBuf, 0, 2);

        //            //残りのデータをMemoryStreamへコピー
        //            var size = 1024;
        //            byte[] buffer = new byte[size];
        //            int numbytes = 0;
        //            while ((numbytes = stream.Read(buffer, 0, size)) > 0)
        //            {
        //                ms.Write(buffer, 0, numbytes);
        //            }

        //            //メモリストリームからBinaryFormatterでデシリアライズ
        //            BinaryFormatter bf = new BinaryFormatter();
        //            data = (T)bf.Deserialize(ms);

        //        }
        //        catch (Exception e)
        //        {
        //            Debug.Log("ObjFromXMLに失敗しました。");
        //            Debug.Log("Message:" + e.ToString());
        //            data = default(T);
        //            return;
        //        }
        //    }
        //}

        //ISavableDataを実装するオブジェクトをバイナリで書き出す
        private static void SaveableDataToBibary<T>(Stream stream, T data)
            where T : ISaveableData
        {
            try
            {
                byte[] buffer = data.GetSaveBytes();
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
            catch(Exception e)
            {
                Debug.LogError("SaveableDataToBinary is Failed:" +
                    " Message :" + e.Message);
            }
        }

        //ISaveableDataを実装するオブジェクトに、バイナリファイルからデータを読みだす
        private static void SaveableDataFromBinary<T>(Stream stream, FileVersion version, ref T data)
            where T : ISaveableData
        {

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            data.SetFromBytes(0, buffer);
        }

    }

    //FileIO関係の拡張メソッド
    public static class IOExtention
    {
        //stringからMemoryStreamへの変換
        public static MemoryStream ConvToMemoryStream(this string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        //MemoryStreamからstringへの変換
        public static string ConvToString(this MemoryStream ms)
        {
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}