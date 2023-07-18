using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileManager
{
    #region Helpers
    static BinaryReader ReadFile(string path, out FileStream stream)
    {
        if (File.Exists(path)) {
            stream = File.Open(path, FileMode.Open, FileAccess.Read);
            return new BinaryReader(stream);
        } else {
            stream = null;
            return null;
        }
    }

    static BinaryWriter WriteFile(string path, out FileStream stream)
    {
        stream = File.Open(path, FileMode.CreateNew, FileAccess.Write);
        return new BinaryWriter(stream);
    }

    static void CloseFile(BinaryReader reader, FileStream stream)
    {
        reader.Close();
        stream.Close();
    }

    static void CloseFile(BinaryWriter writer, FileStream stream)
    {
        writer.Close();
        stream.Close();
    }
    #endregion

    public static List<string> ReadStringArray(string path)
    {
        List<string> list = new List<string>();

        BinaryReader reader = ReadFile(path, out FileStream stream);
        if (reader == null) return null;

        int listCount = reader.ReadInt32();

        for (int i = 0; i < listCount; ++i) {
            list.Add(reader.ReadString());
        }

        CloseFile(reader, stream);
        return list;
    }

    public static void WriteStringArray(string path, List<string> list)
    {
        BinaryWriter writer = WriteFile(path, out FileStream stream);

        writer.Write(list.Count);

        for (int i = 0; i < list.Count; ++i) {
            writer.Write(list[i]);
        }

        CloseFile(writer, stream);
    }
}
