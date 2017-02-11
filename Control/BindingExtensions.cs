namespace MyGraph
{
    public static class BindingExtensions
    {
        public static Binding<TSource, TDestination> Bind<TSource, TDestination>(
            this TDestination destination, TSource source)
        {
            return new Binding<TSource, TDestination>(source, destination);
        }
    }
}