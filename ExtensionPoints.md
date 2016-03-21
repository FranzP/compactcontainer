# Introduction #

The container provides several API points that allows to extend or customize its behaviour.

## IActivator ##

Provides an entry point to customize the way that the container intantiates components.

You can override the default activator that will be used for every component that does not specify a more granular activator strategy:
```
container.DefaultActivator = new DefaultActivator(container);
```

Or you can specify an activator associated with a given type that will be used when the instance being created implements that type.
```
container.RegisterActivator<IMarker>(new CustomActivator(container));
container.Resolve<TypeThatImplementIMarker>();
// the responsability of creating the corresponding TypeThatImplementIMarker instance
// will be delegated to CustomActivator
```


## IDependencyResolver ##

The IDependencyResolver is responsible of providing dependencies to componentes being created.
By default, the container is configured to use CompositeDependencyResolver, that chains the execution of some others DependencyResolvers:
  * SimpleDependencyResolver - ask the container for the dependency
  * ArrayDependencyResolver - resolves all the components that implements the requested service and returns them packed in a typed array
  * ParameterDependencyResolver - inspect the component registration and look up for named parameters (see ComponentsRegistration)

The default IDependencyResolver can be overriden as follows:
```
container.DependencyResolver = new CustomDependencyResolver(container);
```


## IComponentSelector ##

It provides a way to specify which component among many that are registered with the same service type will be used depending on external context
```
class ContextualComponentSelector : IComponentSelector
{
   private Context context;

   public ContextualComponentSelector(Context context)
   {
      this.context = context;
   }

   public bool HasOpinionAbout(Type service)
   {
      return service == typeof(IRepository);
   }

   public ComponentInfo SelectComponent(Type service, IEnumerable<ComponentInfo> components)
   {
      return components.FirstOrDefault(c => c.ClassType == context.RepositoryTypeToUse);
   }
}

...

container.RegisterComponentSelector(new ContextualComponentSelector(context));
```


## IDiscoveryConvention ##

Provide fall-back behavior to use when the requested service is not registered in the container.
Care must be taken in that a component that pass through an IDiscoveryConvention before of being resolved it gets **registered** in the container.

```
public class ConcreteTypeConvention : IDiscoveryConvention
{
   public bool TryRegisterUnknownType(Type type, ICompactContainer container)
   {
      if (!(type.IsInterface || type.IsAbstract))
      {
         container.AddComponentInfo(new ComponentInfo(type.FullName, type, type, container.DefaultLifestyle));
         return true;
      }
      return false;
   }
}

...

container.AddDiscoveryConvention(new ConcreteTypeConvention());

// the following line will first register and then resolve NotRegisteredConcreteType according to ConcreteTypeConvention
container.Resolve<NotRegisteredConcreteType>();
```