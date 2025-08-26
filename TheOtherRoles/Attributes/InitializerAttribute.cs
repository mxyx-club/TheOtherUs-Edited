namespace TheOtherRoles.Attributes;

// From TOH
[AttributeUsage(AttributeTargets.Method)]
internal class InitializerAttribute<T> : Attribute
{
    private static MethodInfo[] allInitializers;
    private MethodInfo targetMethod;
    private readonly Priority priority = Priority.Normal;
    public InitializerAttribute() : this(Priority.Normal) { }
    public InitializerAttribute(Priority priority)
    {
        this.priority = priority;
    }

    public static void Invoke()
    {
        // 初回の初期化時に初期化メソッドを探す
        if (allInitializers == null)
        {
            FindInitializers();
        }
        foreach (var initializer in allInitializers)
        {
            Info($"初始化: {initializer.DeclaringType.Name}.{initializer.Name}");
            initializer.Invoke(null, null);
        }
    }

    private static void FindInitializers()
    {
        var initializers = new HashSet<InitializerAttribute<T>>(32);

        // TownOfHost.dll内の
        var assembly = Assembly.GetExecutingAssembly();
        // 全クラス内の
        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            // 全メソッドについて
            var methods = type.GetMethods(BindingFlags.Static);
            foreach (var method in methods)
            {
                // InitializerAttributeを取得
                var attribute = method.GetCustomAttribute<InitializerAttribute<T>>();
                if (attribute != null)
                {
                    // 取得できたら登録
                    attribute.targetMethod = method;
                    initializers.Add(attribute);
                }
            }
        }
        // 見つかった初期化メソッドをpriority順に並べ替えて配列に変換
        allInitializers = initializers.OrderBy(initializer => initializer.priority).Select(initializer => initializer.targetMethod).ToArray();
    }
}

public enum Priority
{
    /// <summary>一番最初に実行される</summary>
    VeryHigh,
    /// <summary>既定値より前に実行される</summary>
    High,
    /// <summary>既定値</summary>
    Normal,
    /// <summary>既定値より後に実行される</summary>
    Low,
    /// <summary>一番最後に実行される</summary>
    VeryLow,
}