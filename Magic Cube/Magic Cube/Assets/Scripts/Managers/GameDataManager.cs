using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

public class GameDataManager 
{
    private static string _dirpath = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData) + "/MagicCube/SaveData";
    private static string _filepath = "/savedata{0}.mc";
    
    internal GameDataManager()
    {
        LogManager.LogError("GameDataManager was initialized.");
    }

    public static void SaveCommands(LinkedList<ICommand> commands, int cubeSize)
    {
        if (!Directory.Exists(_dirpath))
        {
            Directory.CreateDirectory(_dirpath);
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(_dirpath + string.Format(_filepath, cubeSize));
        bf.Serialize(file, GetCommandsAsString(commands));
        file.Close();
    }

    private static string GetCommandsAsString(LinkedList<ICommand> commands)
    {
        StringBuilder sb = new StringBuilder();
        var delimiter = ',';
        foreach (var command in commands)
        {
            sb.Append(command.WriteDataToString()).Append(delimiter);
        }
        // Remove trailing comma
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }

    public static bool CheckIfSaveDataExists(int cubeSize)
    {
        var fileName = _dirpath + string.Format(_filepath, cubeSize);
        return File.Exists(fileName);
    }

    public static void DeleteCurrentSave(int cubeSize)
    {
        if (!CheckIfSaveDataExists(cubeSize))
        {
            LogManager.LogError($"Tried to delete save for CubeSize: {cubeSize} but it does not exist.");
            return;
        }
        var fileName = _dirpath + string.Format(_filepath, cubeSize);
        File.Delete(fileName);
    }
    
    public static LinkedList<ICommand> LoadCommands(int cubeSize)
    {
        LinkedList<ICommand> commands = new LinkedList<ICommand>();
        var fileName = _dirpath + string.Format(_filepath, cubeSize);
        if (File.Exists(fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(fileName, FileMode.Open);
            commands = BuildCommandListFromString((string) bf.Deserialize(file));
            file.Close();
        }
        return commands;
    }

    private static LinkedList<ICommand> BuildCommandListFromString(string commandString)
    {
        LinkedList<ICommand> list = new LinkedList<ICommand>();
        string[] commands = commandString.Split(',');
        foreach (var command in commands)
        {
            var mcc = MoveCubeCommand.BuildFromString(command);
            if (mcc != null)
            {
                list.AddLast(mcc);
            }
        }
        return list;
    }
}
