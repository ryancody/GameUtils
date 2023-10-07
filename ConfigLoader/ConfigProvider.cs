using Newtonsoft.Json;

namespace GameUtils.Config;

public class ConfigProvider
{
    private Dictionary<string, string> configs;
    private Dictionary<string, object> cachedConfigs = new Dictionary<string, object>();

    /// <summary>
    /// Load files from many paths using System.IO
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public ConfigProvider(IEnumerable<string>? paths = null)
    {
        if (!paths?.Any() ?? true) paths = new List<string> { "." };

        configs = paths.SelectMany(path =>
        {
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException(path);

            var files = Directory.GetFiles(path, "*.json");

            return files.ToDictionary(f => Path.GetFileNameWithoutExtension(f), f => LoadContent(f));
        }).ToDictionary(i => i.Key, i => i.Value);
    }

    /// <summary>
    /// Load files from a path using System.IO
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public ConfigProvider(string path) : this(new List<string> { path }) { }

    /// <summary>
    /// Load files by passing a delegate to use custom file IO methods
    /// </summary>
    /// <param name="getConfigs"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ConfigProvider(Func<IDictionary<string, string>> getConfigs)
    {
        if (getConfigs == null) throw new ArgumentNullException();

        configs = getConfigs.Invoke().ToDictionary(i => i.Key, i => i.Value);
    }

    public static string LoadContent(string file)
    {
        using (StreamReader r = new StreamReader(file))
        {
            return r.ReadToEnd();
        }
    }

    public T Get<T>()
    {
        var name = typeof(T).Name;

        if (cachedConfigs.ContainsKey(name))
            return GetCachedConfig<T>();

        if (!configs.ContainsKey(name))
            throw new Exception($"No json config for {name}");

        var deserialized = JsonConvert.DeserializeObject<T>(configs[name])
            ?? throw new Exception($"Deserialized {name} was null");

        if (!cachedConfigs.ContainsKey(name))
            cachedConfigs.Add(name, deserialized);

        return deserialized;
    }

    private T GetCachedConfig<T>()
    {
        var name = typeof(T).Name;
        var result = (T)cachedConfigs[name]
            ?? throw new Exception($"Cached config {name} was null");

        return result;
    }
}
