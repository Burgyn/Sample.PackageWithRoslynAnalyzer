namespace Sample.MyFancyPackage
{
    public interface IBusConsumerBuilder
    {
        IBusConsumerBuilder AddConsumer<TConsumer>() where TConsumer : class;
    }
}
