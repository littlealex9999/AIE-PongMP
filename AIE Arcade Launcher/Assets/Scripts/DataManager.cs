using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public readonly string titlesFile = "gameNames.titles";
    public readonly string gameDataFileExtension = ".gamedata";

    public readonly string workingDirectory;
    public readonly string gameDataPath;

    List<string> titles;
    public List<string> gameTitles { get { return titles; } }

    public DataManager(string applicationDataPath, string gameDataFilePath)
    {
        workingDirectory = applicationDataPath;
        gameDataPath = gameDataFilePath;

        titles = FileManager.ReadStringList(applicationDataPath, titlesFile);
        if (titles == null) titles = new List<string>();
    }

    public GameData ReadGameData(string title)
    {
        return FileManager.ReadGameData(gameDataPath + "/" + title + gameDataFileExtension);
    }

    public void WriteGameData(GameData data)
    {
        FileManager.WriteGameData(gameDataPath, data.gameTitle + gameDataFileExtension, data);
    }

    public void WriteTitles(List<GameData> data)
    {
        List<string> titlesToSave = new List<string>();

        for (int i = 0; i < data.Count; ++i) {
            titlesToSave.Add(data[i].gameTitle);
        }

        titles = titlesToSave;
        FileManager.WriteStringList(workingDirectory, titlesFile, titles);
    }
}
