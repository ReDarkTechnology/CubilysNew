using System;
using System.IO;
using System.Collections.Generic;

public static class Storage
{
	static bool p_dataChanged;
	static string p_dataPath = GoUpFolder(UnityEngine.Application.dataPath);
	public static string dataPath
	{
		#if UNITY_ANDROID
		get
		{
            return UnityEngine.Application.isEditor ? p_dataPath : p_dataChanged ? p_dataPath : UnityEngine.Application.persistentDataPath;
		}
		#else
		get
		{
			return UnityEngine.Application.isEditor ? p_dataPath : p_dataChanged ? p_dataPath : GoUpFolder(UnityEngine.Application.dataPath);
		}
		#endif
		
		set
		{
			p_dataChanged = true;
			p_dataPath = value;
		}
	}
	
	public static string GetPathLocal(string path)
	{
		return string.IsNullOrEmpty(path) ? path : Path.Combine(dataPath, path);
	}
	
	public static void CheckDirectoryLocal(string path)
	{
		CheckDirectory(GetPathLocal(path));
	}
	
	public static void CheckDirectory(string path)
	{
        if(!Directory.Exists(path) && !string.IsNullOrEmpty(path)) Directory.CreateDirectory(path);
	}

    public static string[] GetFilesLocal(string directory)
    {
        CheckDirectory(GetPathLocal(directory));
        return Directory.GetFiles(GetPathLocal(directory));
    }
    public static string[] GetFiles(string directory)
    {
        CheckDirectory(directory);
        return Directory.GetFiles(GetPathLocal(directory));
    }

    public static string[] GetDirectoriesLocal(string directory)
	{
		CheckDirectory(GetPathLocal(directory));
        return Directory.GetFiles(GetPathLocal(directory));
    }
    public static string[] GetDirectories(string directory)
    {
        CheckDirectory(directory);
        return Directory.GetFiles(directory);
    }

    public static bool FileExistsLocal(string path)
	{
        return File.Exists(GetPathLocal(path));
    }
    public static bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public static void WriteAllTextLocal(string path, string content)
	{
        File.WriteAllText(GetPathLocal(path), content);
    }
    public static void WriteAllText(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public static void WriteAllBytesLocal(string path, byte[] bytes)
    {
        File.WriteAllBytes(GetPathLocal(path), bytes);
    }

    public static void WriteAllBytes(string path, byte[] bytes)
    {
        File.WriteAllBytes(path, bytes);
    }

    public static string ReadAllTextLocal(string path)
	{
		return File.ReadAllText(GetPathLocal(path));
    }
    public static string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }

    public static byte[] ReadAllBytesLocal(string path)
    {
        return File.ReadAllBytes(GetPathLocal(path));
    }
    public static byte[] ReadAllBytes(string path)
    {
        return File.ReadAllBytes(path);
    }

    public static string GoUpFolder(string dir)
    {
		var e = dir.Split(new [] {'/', '\\'});
		var y = dir.Remove(dir.Length - e[e.Length - 1].Length, e[e.Length - 1].Length);
        if(y.EndsWith("\\", StringComparison.CurrentCulture) || y.EndsWith("/", StringComparison.CurrentCulture))
        {
            y = y.Remove(y.Length - 1, 1);
        }
		return y;
    }
}
