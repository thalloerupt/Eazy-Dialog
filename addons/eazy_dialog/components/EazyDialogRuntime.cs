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
    public List<string> Selections { get; set; } = new List<string>();
    public List<string> Left { get; set; } = new List<string>();
    public List<string> Right { get; set; } = new List<string>();
}

namespace Runtime{


public partial class EazyDialogRuntime : Node
{

    [Signal] 
    public delegate void DialogueSignalEventHandler(string character,string content);
    [Signal] 
    public delegate void DialogueSelectionSignalEventHandler(Godot.Collections.Array answers);

    [Signal] 
    public delegate void DialogueEndSignalEventHandler();
    
    private static Random _random = new Random();
    private string dialogName;
    private Dictionary<string, Dialogue> dialogs = null;

    private EazyDialogResolver resolver = new EazyDialogResolver();

    public void PlayNext(string dialogFile,int index ){
        if(dialogs == null){
            dialogs = resolver.ParseFile(dialogFile);
            var startRight = dialogs["START"].Right;
            dialogName = startRight[0];
        }

        if(dialogName =="END"){
            dialogs = null;
            dialogName = null;
            EmitSignal(SignalName.DialogueEndSignal);
            return;
            
        }

        if(index != -1){
            GD.Print(index);
            dialogName =  dialogs[dialogName].Right[index];
        }

        if(dialogs[dialogName].Right.Count>0){


        if(dialogs[dialogName].Right[0].StartsWith("Mutiple")){
           
            Godot.Collections.Array mAnswers = new Godot.Collections.Array();
        
            if(dialogs[dialogs[dialogName].Right[0]].Selections.Count >0){
                foreach (string selection in dialogs[dialogs[dialogName].Right[0]].Selections){
                        mAnswers.Add(selection);
                }
                   EmitSignal(SignalName.DialogueSelectionSignal, mAnswers);}

        }}


        if(dialogName.StartsWith("Dialog")){
            string characterName = dialogs[dialogName].Character;
            string nextContent = dialogs[dialogName].Content;    
            dialogName = dialogs[dialogName].Right[0];
            EmitSignal(SignalName.DialogueSignal, characterName,nextContent);

        }
        




    }



}


}







}




