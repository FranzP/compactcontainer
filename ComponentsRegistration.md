# Introduction #

One of the major improvements from the old version of CompactContainer is the introduction of programatic registration support.
The programatic registration API is **heavily** based on the [Castle Windsor](http://github.com/castleproject/Castle.InversionOfControl) API


## Single components ##
Here there are some examples of the programatic registration API:

```
container.Register(Component.For<ConcreteType>());

container.Register(Component.For<IService>().ImplementedBy<IImplementation>());

container.Register(Component.For(typeof(IService)).ImplementedBy(typeof(IImplementation));

container.Register(Component.For<IService, IFordwardService>().ImplementedBy<IImplementation>());

container.Register(Component.For<IService>().ImplementedBy<IImplementation>().Named("component1"));

container.Register(Component.For<IService>().Instance(serviceInstance));

container.Register(
   Component.For<ConcreteType>()
      .With(Parameter.ForKey("p1").EqualsTo("v1"),
            Parameter.ForKey("p2").EqualsTo("v2"))
   );
```


## Multiple components ##

This type of registration do scan assemblies for matching types and then register them in the container with the specified configuration:

This call register every component in the current assembly that implements IView with service type == typeof(IView) and component type == (actual type of the component)
```
container.Register(AllTypes.FromAssembly(Assembly.GetExecutingAssembly()).BasedOn<IView>());
```

the service type can also be selected using the method WithService:
```
container.Register(AllTypes.FromAssembly(Assembly.GetExecutingAssembly()).BasedOn<IView>().WithService(t => t.GetInterfaces().First()));
```

if you need more control over which types to register, you can use Where instead of BasedOn (in this case, the service type with which the container registers the component will be the same as the implementation type)
```
container.Register(AllTypes.FromAssembly(Assembly.GetExecutingAssembly()).Where(t => t.Name.EndsWith("View"));
```

Besides this, if need to specify a different lifestyle or to name each component with a custom name, you can specify in both cases (BasedOn & Where) a Configure delegate that gets executed for each component:
```
container.Register(AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
   .BasedOn<IView>()
   .Configure(r => r.WithLifestyle(LifestyleType.Transient)
                   .Named(r.ImplementationType + "X")
   );
```

In many cases, more than one rule should be applied to all the types of the same assembly, so in order to reduce the looping cost through each assembly's type, the BasedOn & Where staments can be chained to be executed on a single pass:
```
container.Register(AllTypes.FromAssembly(assembly)
   .BasedOn<IView>().Configure(r => r.WithLifestyle(LifestyleType.Transient))
   .BasedOn<IPresenter>()
   .Where(t => t.Name.EndsWith("Service"))
   );
```


---


Invidual components registrations can also be chained in a single ICompactContainer.Register call:
```
container.Register(
   Component.For<IService>().ImplementedBy<IImplementation>(),
   AllTypes.FromAssembly(assembly).BasedOn<IView>().BasedOn<IPresenter>(),
   Component.For<Context>()
   );
```