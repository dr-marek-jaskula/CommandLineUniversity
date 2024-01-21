namespace CommandLiner.Common.Cache;

public abstract class CacheAttribute<KeyType> : Attribute
    where KeyType : Key
{
    public abstract KeyType ToKey();
}