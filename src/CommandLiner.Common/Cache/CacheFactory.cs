using CommandLiner.Common.Cache;
using System.Collections.ObjectModel;
using System.Reflection;

namespace CommandLiner.Common.Factories;

public sealed class CacheFactory<KeyType, DelegateType>
    where KeyType : Key
    where DelegateType : notnull, Delegate
{
    private CacheFactory()
    {
    }

    public static ReadOnlyDictionary<KeyType, DelegateType> CreateFor<AttributeType>(IEnumerable<DelegateType> dictionaryValues)
        where AttributeType : CacheAttribute<KeyType>
    {
        Dictionary<KeyType, DelegateType> cache = [];

        foreach (var @delegate in dictionaryValues)
        {
            AddToCache<AttributeType>(cache, @delegate);
        }

        return cache.AsReadOnly();
    }

    private static void AddToCache<AttributeType>(Dictionary<KeyType, DelegateType> cache, DelegateType @delegate)
        where AttributeType : CacheAttribute<KeyType>
    {
        var discriminator = @delegate
            .GetMethodInfo()
            .GetCustomAttribute<AttributeType>() 
            ?? throw new InvalidOperationException($"Each delegate must have custom attribute 'Discriminator' of type {typeof(KeyType)}.");

        if (cache.TryAdd(discriminator.ToKey(), @delegate) is false)
        {
            throw new InvalidOperationException($"Duplicated 'Discriminator' for key '{discriminator.ToKey()}'.");
        }
    }
}