namespace EventBus.Base;

/// <summary>
/// Dışarıdan gönderilen verilerin içeride tutulması için kullanıcaz
/// </summary>
public class SubscriptionInfo
{
    public Type HandlerType { get; }
    public SubscriptionInfo(Type handlerType)
    {
        HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
    }

    public static SubscriptionInfo Typed(Type handlerType)
    {
        return new SubscriptionInfo(handlerType);
    }

}
