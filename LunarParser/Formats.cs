﻿using System.IO;

using LunarParser.XML;
using LunarParser.JSON;
using LunarParser.YAML;
using LunarParser.Binary;


namespace LunarParser
{
    public enum DataFormat
    {
        Unknown,
        BIN,
        XML,
        JSON,
        YAML,
    }

    public static class DataFormats
    {
        public static DataFormat GetFormatForExtension(string extension)
        {
            switch (extension)
            {
                case ".xml": return DataFormat.XML;
                case ".json": return DataFormat.JSON;
                case ".yaml": return DataFormat.YAML;
                case ".bin": return DataFormat.BIN;

                default:
                    {
                        return DataFormat.Unknown;
                    }
            }
        }

        public static DataFormat DetectFormat(string content)
        {
            int i = 0;
            while (i<content.Length)
            {
                var c = content[i];
                i++;

                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                switch (c)
                {
                    case '-': return DataFormat.YAML;
                    case '<': return DataFormat.XML;
                    case '{':
                    case '[': return DataFormat.JSON;
                    default: return DataFormat.Unknown;
                }
            }

            return DataFormat.Unknown;
        }

        public static DataNode LoadFromString(DataFormat format, string contents)
        {
            switch (format)
            {
                case DataFormat.XML: return XMLReader.ReadFromString(contents);
                case DataFormat.JSON: return JSONReader.ReadFromString(contents);
                case DataFormat.YAML: return YAMLReader.ReadFromString(contents);
                default:
                    {
                        throw new System.Exception("Format not supported");
                    }
            }
        }

        public static string SaveToString(DataFormat format, DataNode root)
        {
            switch (format)
            {
                case DataFormat.XML: return XMLWriter.WriteToString(root);
                case DataFormat.JSON: return JSONWriter.WriteToString(root);
                case DataFormat.YAML: return YAMLWriter.WriteToString(root);
                default:
                    {
                        throw new System.Exception("Format not supported");
                    }
            }
        }

        public static DataNode LoadFromString(string content)
        {
            var format = DetectFormat(content);
            return LoadFromString(format, content);
        }

        /// <summary>
        /// Loads a node tree from a file, type is based on filename extension
        /// </summary>
        public static DataNode LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException();
            }

            var extension = Path.GetExtension(fileName).ToLower();

            if (extension.Equals(".bin"))
            {
                var bytes = File.ReadAllBytes(fileName);
                return BINReader.ReadFromBytes(bytes);
            }

            var contents = File.ReadAllText(fileName);

            var format = GetFormatForExtension(extension);

            if (format == DataFormat.Unknown)
            {
                format = DetectFormat(contents);
            }

            return LoadFromString(format, contents);
        }

        public static void SaveToFile(string fileName, DataNode root)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            var format = GetFormatForExtension(extension);

            var content = SaveToString(format, root);
            File.WriteAllText(fileName, content);
        }

    }
}
