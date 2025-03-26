using Godot;
using System;
using EazyDialog;
using System.IO;
using System.Collections.Generic;

namespace EazyDialog.Resolver{
public partial class EazyDialogResolver : Node
{
    public Dictionary<string, Dialogue> ParseFile(string dialogFile){
        string mainCharacter = GetNthLastDirectory(dialogFile, 2);
        string secondaryCharacter = GetNthLastDirectory(dialogFile, 1);
        string context = LoadFile(dialogFile);
        var dialogues = new Dictionary<string, Dialogue>();
        var lines = context.Split(new[]  { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        Dialogue currentDialogue = null;
        string currentKey = null;
        string currentSection = null;
        foreach (var line in lines)
        {
            string trimmed = line.Trim();

            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                currentKey = trimmed.Trim('[', ']');
                currentDialogue = new Dialogue {  };
                dialogues[currentKey] = currentDialogue;
            }
            else if (trimmed.Equals("Character"))
            {
                currentSection = "Character";
            }
            else if (trimmed.Equals("Context"))
            {
                currentSection = "Context";
            }
            else if (trimmed.Equals("LEFT"))
            {
                currentSection = "LEFT";
            }
            else if (trimmed.Equals("RIGHT"))
            {
                currentSection = "RIGHT";
            }
            else if (trimmed.StartsWith("- "))
            {
                string content = trimmed.Substring(2).Trim();
                if (currentDialogue != null && currentSection != null)
                {
                    if(currentSection == "Character"){
                        currentDialogue.Character = content;

                    }
                    else if (currentSection == "Context")
                    {
                        currentDialogue.Content += (string.IsNullOrEmpty(currentDialogue.Content) ? "" : "\n") + content;
                    }
                    else if (currentSection == "LEFT")
                    {
                        currentDialogue.Left.Add(content);
                    }
                    else if (currentSection == "RIGHT")
                    {
                        currentDialogue.Right.Add(content);
                    }
                }
            }
            else{
                    if (currentSection == "Context")
                    {
                        currentDialogue.Content += (string.IsNullOrEmpty(currentDialogue.Content) ? "" : "\n") + trimmed;
                    }
            }
        }

        return dialogues;

    }



        private string GetNthLastDirectory(string filePath, int n)
    {
        string directoryPath = Path.GetDirectoryName(filePath); // 获取文件所在的目录
        if (directoryPath == null) return "无效路径";

        string[] folders = directoryPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (folders.Length < n) return "文件夹数量不足";

        return folders[folders.Length - n]; // 取倒数第 n 个文件夹
    }


        private string LoadFile(string filePath)
    {
        using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
        string content = file.GetAsText();
        return content;
    }




}
}