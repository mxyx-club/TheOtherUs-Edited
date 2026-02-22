using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TheOtherRoles.Modules;

public class YamlConfigManager
{
    public const string PathSeparator = ".";

    private string ConfigFileName { get; set; }
    private readonly string _configFilePath;
    private readonly Dictionary<string, ConfigOptionBase> _options = new();
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public YamlConfigManager(string name = "config.yaml")
    {
        ConfigFileName = name;
        _configFilePath = Path.Combine(Application.persistentDataPath, "TOUE", ConfigFileName);

        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    public ConfigOption<T> CreateOption<T>(string path, T defaultValue) where T : notnull
    {
        if (_options.ContainsKey(path))
        {
            if (_options[path] is ConfigOption<T> typedOption)
                return typedOption;
            throw new InvalidOperationException($"Config key '{path}' already exists with different type");
        }

        foreach (var key in _options.Keys)
        {
            if (key.StartsWith(path + PathSeparator) || path.StartsWith(key + PathSeparator))
                throw new InvalidOperationException($"Path '{path}' conflicts with existing option '{key}'");
        }

        var option = new ConfigOption<T>(path, defaultValue, UpdateAndSave);
        _options[path] = option;
        return option;
    }

    public ConfigOption<T> GetOption<T>(string path) where T : notnull
    {
        if (_options.TryGetValue(path, out var option) && option is ConfigOption<T> typedOption)
            return typedOption;
        throw new KeyNotFoundException($"Config key '{path}' not found");
    }

    public bool TryGetOption<T>(string path, out ConfigOption<T> option) where T : notnull
    {
        if (_options.TryGetValue(path, out var configOption) && configOption is ConfigOption<T> typedOption)
        {
            option = typedOption;
            return true;
        }
        option = null;
        return false;
    }

    public IEnumerable<ConfigOptionBase> GetOptionsInParent(string parentPath)
    {
        var prefix = parentPath + PathSeparator;
        return _options.Where(kvp => kvp.Key.StartsWith(prefix)).Select(kvp => kvp.Value);
    }

    public bool HasOption(string path) => _options.ContainsKey(path);

    public IEnumerable<string> GetAllKeys() => _options.Keys;

    public void Load()
    {
        try
        {
            if (!File.Exists(_configFilePath))
            {
                Save();
                return;
            }

            var yamlContent = File.ReadAllText(_configFilePath);
            var deserialized = _deserializer.Deserialize<object>(yamlContent);
            var flatConfig = FlattenNested(deserialized);

            foreach (var kvp in flatConfig)
            {
                if (_options.TryGetValue(kvp.Key, out var option))
                    option.LoadFromString(kvp.Value);
            }
        }
        catch { }
    }

    public void Save()
    {
        try
        {
            var currentFlat = new Dictionary<string, string>();
            foreach (var kvp in _options)
                currentFlat[kvp.Key] = kvp.Value.SaveToString();

            Dictionary<string, string> existingFlat = null;
            if (File.Exists(_configFilePath))
            {
                var yamlContent = File.ReadAllText(_configFilePath);
                var deserialized = _deserializer.Deserialize<object>(yamlContent);

                if (deserialized is Dictionary<object, object>)
                {
                    existingFlat = FlattenNested(deserialized);
                    existingFlat = existingFlat.Where(kvp => !string.IsNullOrEmpty(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }
            }

            var mergedFlat = existingFlat != null ? new Dictionary<string, string>(existingFlat) : new Dictionary<string, string>();
            foreach (var kvp in currentFlat)
                mergedFlat[kvp.Key] = kvp.Value;

            var nestedConfig = ConvertToNested(mergedFlat);
            var directory = Path.GetDirectoryName(_configFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var yaml = _serializer.Serialize(nestedConfig);
            File.WriteAllText(_configFilePath, yaml);
        }
        catch { }
    }

    public void Reload() => Load();

    private void UpdateAndSave(string key) => Save();

    private Dictionary<string, string> FlattenNested(object node, string currentPath = "")
    {
        var result = new Dictionary<string, string>();
        if (node == null) return result;

        switch (node)
        {
            case Dictionary<object, object> dict:
                foreach (var kvp in dict)
                {
                    var key = kvp.Key?.ToString() ?? "";
                    var newPath = string.IsNullOrEmpty(currentPath) ? key : currentPath + PathSeparator + key;
                    var childResult = FlattenNested(kvp.Value, newPath);
                    foreach (var child in childResult)
                        result[child.Key] = child.Value;
                }
                break;
            case List<object>:
                break;
            default:
                result[currentPath] = node?.ToString() ?? "";
                break;
        }
        return result;
    }

    private object ConvertToNested(Dictionary<string, string> flat)
    {
        var root = new Dictionary<string, object>();
        foreach (var kvp in flat)
        {
            var parts = kvp.Key.Split([PathSeparator], StringSplitOptions.None);
            var current = root;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                var part = parts[i];
                if (!current.TryGetValue(part, out var next))
                {
                    next = new Dictionary<string, object>();
                    current[part] = next;
                }
                else if (next is not Dictionary<string, object>)
                {
                    next = new Dictionary<string, object>();
                    current[part] = next;
                }
                current = (Dictionary<string, object>)next;
            }
            current[parts[^1]] = kvp.Value;
        }
        return root;
    }
}

public abstract class ConfigOptionBase
{
    public string Key { get; }

    protected ConfigOptionBase(string key)
    {
        Key = key;
    }

    public abstract string SaveToString();
    public abstract void LoadFromString(string value);
}

public class ConfigOption<T> : ConfigOptionBase where T : notnull
{
    private readonly Action<string> _onUpdate;

    public T Value { get; private set; }
    public T DefaultValue { get; }

    public ConfigOption(string key, T defaultValue, Action<string> onUpdate) : base(key)
    {
        if (!IsValidType())
            throw new NotSupportedException($"Type '{typeof(T).Name}' not supported");

        DefaultValue = defaultValue;
        Value = defaultValue;
        _onUpdate = onUpdate;
    }

    public void Update(T value)
    {
        if (!IsValidType())
            throw new NotSupportedException($"Type '{typeof(T).Name}' not supported");

        if (EqualityComparer<T>.Default.Equals(Value, value))
            return;

        Value = value;
        _onUpdate?.Invoke(Key);
    }

    public override string SaveToString() => Value?.ToString() ?? string.Empty;

    public override void LoadFromString(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Value = DefaultValue;
            return;
        }

        try
        {
            Value = typeof(T) switch
            {
                var t when t == typeof(string) => (T)(object)value,
                var t when t == typeof(short) => (T)(object)short.Parse(value),
                var t when t == typeof(ushort) => (T)(object)ushort.Parse(value),
                var t when t == typeof(int) => (T)(object)int.Parse(value),
                var t when t == typeof(uint) => (T)(object)uint.Parse(value),
                var t when t == typeof(long) => (T)(object)long.Parse(value),
                var t when t == typeof(ulong) => (T)(object)ulong.Parse(value),
                var t when t == typeof(float) => (T)(object)float.Parse(value),
                var t when t == typeof(double) => (T)(object)double.Parse(value),
                var t when t == typeof(bool) => (T)(object)bool.Parse(value),
                var t when t.IsEnum => TryParseEnum(value),
                _ => throw new NotSupportedException($"Type '{typeof(T).Name}' not supported")
            };
        }
        catch
        {
            Value = DefaultValue;
        }
    }

    private static bool IsValidType()
    {
        var type = typeof(T);
        return type == typeof(string) ||
               type == typeof(short) || type == typeof(ushort) ||
               type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) ||
               type == typeof(float) || type == typeof(double) ||
               type == typeof(bool) ||
               type.IsEnum;
    }

    private static T TryParseEnum(string value)
    {
        var type = typeof(T);
        if (int.TryParse(value, out var intValue))
            return (T)Enum.ToObject(type, intValue);
        return (T)Enum.Parse(type, value, true);
    }

    public void ResetToDefault() => Update(DefaultValue);

    public static implicit operator T(ConfigOption<T> option) => option.Value;
}

public static class ConfigOptionExtensions
{
    public static ConfigOption<T> GetOrCreate<T>(this YamlConfigManager manager, string path, T defaultValue) where T : notnull
    {
        if (manager.TryGetOption<T>(path, out var option)) return option;
        return manager.CreateOption(path, defaultValue);
    }
}