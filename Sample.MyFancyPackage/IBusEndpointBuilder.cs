namespace Sample.MyFancyPackage
{
    public interface IBusEndpointBuilder
    {
        IBusConsumerBuilder ConfigureSubscription<T>(string subscriptionName) where T : class;
    }
}
