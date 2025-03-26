using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using EazyDialog.Resolver;

namespace EazyDialog{

public class Dialogue
{
    public string Character { get; set; }
    public string Content { get; set; }
    public List<string> Left { get; set; } = new List<string>();
    public List<string> Right { get; set; } = new List<string>();
}

namespace Runtime{


public partial class EazyDialogRuntime : Node
{

    [Signal] // ✅ Godot automatically handles delegate creation
    public delegate void DialogueSignalEventHandler(string character,string content,Godot.Collections.Array answers);

    [Signal] // ✅ Godot automatically handles delegate creation
    public delegate void DialogueEndSignalEventHandler();
    
    private static Random _random = new Random();
    private string dialogue;
    private Dictionary<string, Dialogue> dialogs = null;

    private EazyDialogResolver resolver = new EazyDialogResolver();

    public void PlayNext(string dialogFile,int index = -1){
        if(dialogs == null){
            dialogs = resolver.ParseFile(dialogFile);
            var startRight = dialogs["START"].Right;
            dialogue = startRight[0];
        }
        if(dialogue!="END"){
            string characterName = dialogs[dialogue].Character;
            string nextContent = dialogs[dialogue].Content;
            Godot.Collections.Array mAnswers = new Godot.Collections.Array();
            if(dialogs[dialogue].Right.Count >1)
                foreach (string dialogName in dialogs[dialogue].Right){
                    mAnswers.Add(dialogs[dialogName].Content);
                }
            dialogue = dialogs[dialogue].Right[0];
            EmitSignal(SignalName.DialogueSignal, characterName,nextContent,mAnswers);
        }else{
            dialogs = null;
            dialogue = null;
            EmitSignal(SignalName.DialogueEndSignal);
        }
    }



}

}
}




