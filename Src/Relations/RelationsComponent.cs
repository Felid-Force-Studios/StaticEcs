namespace FFS.Libraries.StaticEcs {
    
    public interface IEntityLinkComponent<T> : IComponent, IRefProvider<T, EntityGID> { }

    public interface IEntityLinksComponent<T> : IComponent, IRefProvider<T, ROMulti<EntityGID>> { }

}