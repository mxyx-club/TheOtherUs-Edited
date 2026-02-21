using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TheOtherRoles.Modules;

public class YamlConfigManager : ManagerBase<YamlConfigManager>
{
    private const string ConfigFileName = "config.yaml";
    private readonly string _configFilePath;
    private readonly Dictionary<string, ConfigOptionBase> _options = new();
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public YamlConfigManager()
    {
        _configFilePath = Path.Combine(Application.persistentDataPath, "TOUE", ConfigFileName);

        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    public ConfigOption<T> CreateOption<T>(string key, T defaultValue, string description = "") where T : notnull
    {
        if (_options.ContainsKey(key))
        {
            var existingOption = _options[key];
            if (existingOption is ConfigOption<T> typedOption)
            {
                return typedOption;
            }
            throw new InvalidOperationException($"Config key '{key}' already exists with different type");
        }

        var option = new ConfigOption<T>(key, defaultValue, description, UpdateAndSave);
        _options[key] = option;
        return option;
    }

    public ConfigOption<T> GetOption<T>(string key) where T : notnull
    {
        if (_options.TryGetValue(key, out var option) && option is ConfigOption<T> typedOption)
        {
            return typedOption;
        }
        throw new KeyNotFoundException($"Config key '{key}' not found");
    }

    public bool TryGetOption<T>(string key, out ConfigOption<T> option) where T : notnull
    {
        if (_options.TryGetValue(key, out var configOption) && configOption is ConfigOption<T> typedOption)
        {
            option = typedOption;
            return true;
        }
        option = null;
        return false;
    }

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
            var configData = _deserializer.Deserialize<Dictionary<string, string>>(yamlContent);

            foreach (var kvp in configData)
            {
                if (_options.TryGetValue(kvp.Key, out var option))
                {
                    option.LoadFromString(kvp.Value);
                }
            }
        }
        catch { }
    }

    public void Save()
    {
        try
        {
            var configData = new Dictionary<string, string>();
            foreach (var kvp in _options)
            {
                configData[kvp.Key] = kvp.Value.SaveToString();
            }

            var directory = Path.GetDirectoryName(_configFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var yamlContent = _serializer.Serialize(configData);
            File.WriteAllText(_configFilePath, yamlContent);
        }
        catch
        {
        }
    }

    private void UpdateAndSave(string key) => Save();

    public IEnumerable<string> GetAllKeys() => _options.Keys;

    public void Reload() => Load();
}

public abstract class ConfigOptionBase
{
    public string Key { get; }
    public string Description { get; }

    protected ConfigOptionBase(string key, string description)
    {
        Key = key;
        Description = description;
    }

    public abstract string SaveToString();
    public abstract void LoadFromString(string value);
}

public class ConfigOption<T> : ConfigOptionBase where T : notnull
{
    private readonly Action<string> _onUpdate;

    public T Value { get; private set; }
    public T DefaultValue { get; }

    public void Update(T value)
    {
        if (!IsValidType())
        {
            throw new NotSupportedException($"Type '{typeof(T).Name}' not supported");
        }

        if (EqualityComparer<T>.Default.Equals(Value, value))
            return;

        Value = value;
        _onUpdate?.Invoke(Key);
    }

    public ConfigOption(string key, T defaultValue, string description, Action<string> onUpdate)
        : base(key, description)
    {
        if (!IsValidType())
        {
            throw new NotSupportedException($"Type '{typeof(T).Name}' not supported");
        }

        DefaultValue = defaultValue;
        Value = defaultValue;
        _onUpdate = onUpdate;
    }

    public override string SaveToString()
    {
        if (Value == null)
            return string.Empty;

        return Value?.ToString() ?? string.Empty;
    }

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
               type == typeof(short) ||
               type == typeof(ushort) ||
               type == typeof(int) ||
               type == typeof(uint) ||
               type == typeof(long) ||
               type == typeof(ulong) ||
               type == typeof(float) ||
               type == typeof(double) ||
               type == typeof(bool) ||
               type.IsEnum;
    }

    private static T TryParseEnum(string value)
    {
        var type = typeof(T);
        if (int.TryParse(value, out var intValue))
        {
            return (T)Enum.ToObject(type, intValue);
        }
        return (T)Enum.Parse(type, value, true);
    }

    public void ResetToDefault()
    {
        Update(DefaultValue);
    }

    public static implicit operator T(ConfigOption<T> option) => option.Value;
}

public static class ConfigOptionExtensions
{
    public static ConfigOption<T> GetOrCreate<T>(this YamlConfigManager manager, string key, T defaultValue, string description = "") where T : notnull
    {
        if (manager.TryGetOption<T>(key, out var option))
        {
            return option;
        }
        return manager.CreateOption(key, defaultValue, description);
    }
}
