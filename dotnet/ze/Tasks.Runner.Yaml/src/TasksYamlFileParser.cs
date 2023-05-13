using System;
using System.Linq;

using Bearz.Extra.Object;
using Bearz.Extra.Strings;

using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Ze.Tasks.Runner.Yaml;

public class TasksYamlFileParser
{
    public static YamlTaskWorkflowDefinition ParseFile(string file)
    {
        using var input = new StreamReader(file);

        // Load the stream
        var yaml = new YamlStream();
        yaml.Load(input);

        var doc = yaml.Documents.First();
        if (doc.RootNode is not YamlMappingNode map)
            throw new YamlException("Root node is not a mapping node");

        var taskCollection = new TaskCollection();
        var parser = new TasksYamlFileParser();
        if (map.Children.TryGetValue("tasks", out var node) && node is YamlSequenceNode tasksNode)
        {
            foreach (var child in tasksNode.Children)
            {
                if (child is YamlMappingNode taskNode)
                {
                    taskCollection.Add(parser.ParseTask(taskNode));
                }
            }
        }

        return new YamlTaskWorkflowDefinition(taskCollection);
    }

    public ITask ParseTask(YamlMappingNode node)
    {
        var name = string.Empty;
        var id = string.Empty;

        if (node.Children.TryGetValue("name", out var nameNode))
        {
            name = nameNode.ToString();
        }

        if (node.Children.TryGetValue("id", out var idNode))
        {
            id = idNode.ToString();
        }

        if (id.IsNullOrWhiteSpace() && name.IsNullOrWhiteSpace())
        {
            if (nameNode is null || idNode is null)
                throw new YamlException("Task must have a name or id that is not empty");

            throw new YamlException(nameNode.Start, nameNode.End, "Task must have a name or id that is not empty");
        }

        var task = new YamlShellTask(name, id);
        foreach (var child in node.Children)
        {
            switch (child.Key.ToString())
            {
                case "name":
                    continue;
                case "description":
                    task.Description = child.Value.ToString();
                    break;
                case "needs":
                case "deps":
                case "dependencies":
                    {
                        if (child.Value is YamlScalarNode { Value: { } } scalar)
                        {
                            task.Dependencies = new[] { scalar.Value };
                            continue;
                        }

                        if (child.Value is YamlSequenceNode sequence)
                        {
                            task.Dependencies = sequence
                                .Where(o => o is YamlScalarNode scalarNode3 && !scalarNode3.Value.IsNullOrWhiteSpace())
                                .Select(x => x.ToString())
                                .ToArray();
                        }
                    }

                    break;
                case "timeout":
                    if (child.Value is YamlScalarNode { Value: { } } scalarNode && int.TryParse(scalarNode.Value.Trim(), out var timeout))
                    {
                        task.Timeout = timeout;
                        continue;
                    }

                    task.Timeout = int.Parse(child.Value.ToString());
                    break;
                case "continue-on-error":
                case "continueOnError":
                    task.ContinueOnError = bool.Parse(child.Value.ToString());
                    break;
                case "inputs":
                case "with":
                    foreach (var kvp in this.VisitInputBlocks(child.Value))
                    {
                        task.Inputs[kvp.Key] = kvp.Value;
                    }

                    break;
                case "run":
                    {
                        if (child.Value is YamlScalarNode { Value: { } } scalarNode2)
                        {
                            task.Run = scalarNode2.Value;
                        }
                    }

                    break;

                case "shell":
                    {
                        if (child.Value is YamlScalarNode { Value: { } } scalarNode2)
                        {
                            task.Shell = scalarNode2.Value;
                            continue;
                        }
                    }

                    break;

                default:
                    throw new YamlException($"Unknown key: {child.Key}");
            }
        }

        return task;
    }

    private Dictionary<string, InputBlock> VisitInputBlocks(YamlNode node)
    {
        if (node is YamlScalarNode)
            throw new YamlException("Inputs must be a mapping node or sequence");

        // inputs:
        //  name: test
        //  name2:
        //      value: test
        //      description: test
        var inputs = new Dictionary<string, InputBlock>();

        if (node is YamlMappingNode map)
        {
            foreach (var child in map)
            {
                var key = child.Key;
                if (child.Value is YamlScalarNode { Value: { } } scalar)
                {
                    inputs[key.ToString()] = new InputBlock
                    {
                        Name = key.ToString(),
                        Expression = scalar.Value.ToSafeString(),
                    };
                    continue;
                }

                if (child.Value is YamlMappingNode blockNode)
                {
                    var block = new InputBlock() { Name = key.ToSafeString(), };

                    if (blockNode.Children.TryGetValue("value", out var valueNode))
                        block.Expression = valueNode.ToSafeString();

                    if (blockNode.Children.TryGetValue("description", out var descriptionNode))
                        block.Description = descriptionNode.ToSafeString();

                    if (blockNode.Children.TryGetValue("required", out var requiredNode))
                        block.Required = bool.Parse(requiredNode.ToSafeString());

                    if (blockNode.Children.TryGetValue("default", out var defaultNode))
                        block.DefaultValue = defaultNode.ToSafeString();

                    inputs[block.Name] = block;
                }

                if (child.Value is YamlSequenceNode sequenceNode)
                {
                    foreach (var item in sequenceNode)
                    {
                        if (item is YamlMappingNode childMap)
                        {
                            if (!childMap.Children.TryGetValue("name", out var nameNode))
                                throw new YamlException(item.Start, item.End, "Input block must have a name");

                            var block = new InputBlock() { Name = key.ToSafeString(), };

                            if (childMap.Children.TryGetValue("value", out var valueNode))
                                block.Expression = valueNode.ToSafeString();

                            if (childMap.Children.TryGetValue("description", out var descriptionNode))
                                block.Description = descriptionNode.ToSafeString();

                            if (childMap.Children.TryGetValue("required", out var requiredNode))
                                block.Required = bool.Parse(requiredNode.ToSafeString());

                            if (childMap.Children.TryGetValue("default", out var defaultNode))
                                block.DefaultValue = defaultNode.ToSafeString();

                            inputs[block.Name] = block;
                        }
                    }
                }
            }
        }

        return inputs;
    }
}