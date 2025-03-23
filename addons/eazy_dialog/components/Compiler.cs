using Godot;
using System;
using System.IO;
using System.Collections.Generic;
[Tool]
public partial class Compiler : EditorPlugin
{


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
        _WriteEDM(savePath,content);

  

        GD.Print("Graph 数据已成功导出:\n ", content);
    }


    public  void _WriteEDM(String path,string content)
    {

    using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Write);
    file.StoreString(content);
    }


}
