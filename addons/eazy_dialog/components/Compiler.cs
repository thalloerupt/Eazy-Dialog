using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using EazyDialog.Resolver;
using EazyDialog;
using System.Linq;
[Tool]
public partial class Compiler : EditorPlugin
{
    string nodeName = "START";
    Vector2 position = new Vector2(0,0);
    Dictionary<string,bool> visited  = new();
    EazyDialogResolver resolver = new EazyDialogResolver();
    Dictionary<string, Dialogue> dialogs ;
    List<string> characterNames;
    PackedScene  startNode = GD.Load<PackedScene>("res://addons/eazy_dialog/components/start_node.tscn");
    PackedScene  endNode =  GD.Load<PackedScene>("res://addons/eazy_dialog/components/end_node.tscn");
    PackedScene  dialogNode = GD.Load<PackedScene>("res://addons/eazy_dialog/components/dialog_node.tscn");
    public void ExportGraph(GraphEdit graphEdit,string savePath)
    {


        if (graphEdit == null)
        {
            GD.PrintErr("GraphEdit 未找到！");
            return;
        }

        Dictionary<string, List<string>> leftConnections = new();
        Dictionary<string, List<string>> rightConnections = new();
        Dictionary<string, string> context = new();
        Dictionary<string, string> character = new();

        List<string> dialogs = new();


        // 获取所有 GraphNode 并分类
        foreach (var child in graphEdit.GetChildren())
        {
            if (child is GraphNode graphNode)
            {
                string nodeTitle = graphNode.Title;
                dialogs.Add(nodeTitle);
                leftConnections[nodeTitle] = new List<string>();
                rightConnections[nodeTitle] = new List<string>();
                context[nodeTitle] = "";
                if (nodeTitle.Contains("Dialog"))
                {
                    string dialogContent = child.GetNode<TextEdit>("HFlowContainer/VBoxContainer/DialogContext/VBoxContainer/TextEdit").Text;
                    string characterName = child.GetNode<OptionButton>("HFlowContainer/HBoxContainer/OptionButton").Text;
                    context[nodeTitle]=dialogContent;
                    character[nodeTitle] = characterName;
                    
                }
                else{
                    context[nodeTitle] = "...";
                    character[nodeTitle] = "...";

                }
            }
        }

        // 获取连接信息
        foreach (var connection in graphEdit.GetConnectionList())
        {
            string fromNode = connection["from_node"].AsString();
            string toNode = connection["to_node"].AsString();
            int fromPort = connection["from_port"].AsInt32();
            rightConnections[fromNode].Add(toNode);
            leftConnections[toNode].Add(fromNode);

        }

        // 生成 .edm 格式文本
        string content = "";

        foreach (var dialog in dialogs)
        {
            content += $"[{dialog}]\n";
            content += $"    Character\n";
            content += $"        - {character[dialog]}\n";
            content += $"    Context\n";
            content += $"        - {context[dialog]}\n";

            // LEFT
            if (leftConnections[dialog].Count > 0)
            {
                content += $"    LEFT\n";
                foreach (var left in leftConnections[dialog])
                {
                    content += $"        - {left}\n";
                }
            }

            // RIGHT
            if (rightConnections[dialog].Count > 0)
            {
                content += $"    RIGHT\n";
                foreach (var right in rightConnections[dialog])
                {
                    content += $"       - {right}\n";
                }
            }
            content += "\n";
        }
        WriteFile(savePath,content);

  

        GD.Print("Graph 数据已成功导出:\n ", content);
    }


    private void WriteFile(String path,string content)
    {

    using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Write);
    file.StoreString(content);
    }

    public void ConnectNode(GraphEdit graphEdit){

        foreach(var _node in dialogs[nodeName].Right){
            graphEdit.ConnectNode(nodeName,0,_node,0);
        }

        foreach(var _node in dialogs[nodeName].Right){
            nodeName = _node;
            ConnectNode(graphEdit);
        }



    }

    public void AddNode(string filePath,GraphEdit graphEdit){

        if(dialogs ==null)
           dialogs = resolver.ParseFile(filePath);
           characterNames = resolver.GetCharacters(filePath);
        
        if(visited.ContainsKey(nodeName))
            return;
        else
            visited.Add(nodeName,true);


        if (nodeName.StartsWith("START")){
            GraphNode node = (GraphNode)startNode.Instantiate();
            node.Name = nodeName;
            node.Title = nodeName;
            node.PositionOffset = position;
            position.X +=300;
            graphEdit.AddChild(node);

        }else if(nodeName.StartsWith("Dialog")){
            GraphNode node = (GraphNode)dialogNode.Instantiate();
            node.Name = nodeName;
            node.Title = nodeName;
            TextEdit contentEditor = node.GetNode<TextEdit>("HFlowContainer/VBoxContainer/DialogContext/VBoxContainer/TextEdit");
            OptionButton characterNameEditor = node.GetNode<OptionButton>("HFlowContainer/HBoxContainer/OptionButton");
            contentEditor.Text = dialogs[nodeName].Content;
            foreach (var (item, index) in characterNames.Select((value, i) => (value, i))){
                characterNameEditor.AddItem(item);

            }
            characterNameEditor.Select(characterNames.IndexOf(dialogs[nodeName].Character));

            if(dialogs[nodeName].Left.Count != 0){
                GraphNode fatherNode =(GraphNode) graphEdit.GetNode( dialogs[nodeName].Left[0]);
                node.PositionOffset = new Vector2(fatherNode.PositionOffset.X+500,fatherNode.PositionOffset.Y+(dialogs[fatherNode.Name].Right.IndexOf(nodeName))*300);


            }
                
            graphEdit.AddChild(node);

        }else if(nodeName.StartsWith("END")){
            GraphNode node = (GraphNode)endNode.Instantiate();
            node.Name = nodeName;
            node.Title = nodeName;
            node.PositionOffset = position;
            if(dialogs[nodeName].Left.Count != 0){
                GraphNode fatherNode =(GraphNode) graphEdit.GetNode( dialogs[nodeName].Left[0]);
                node.PositionOffset = new Vector2(fatherNode.PositionOffset.X+500,fatherNode.PositionOffset.Y);

            }


            graphEdit.AddChild(node);
            
        }

        foreach(var _node in dialogs[nodeName].Right){


            nodeName = _node;
            FileToGraph(filePath,graphEdit);

        }

        


      

    }

    public void FileToGraph(string filePath,GraphEdit graphEdit){
        AddNode(filePath,graphEdit);
        nodeName = "START";
        ConnectNode(graphEdit);



        
    }
}
