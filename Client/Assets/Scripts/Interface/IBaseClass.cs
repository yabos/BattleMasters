public interface IBaseClass
{
    void Initialize();
    void Terminate();
}

public interface IRepository<T1, T2> : IBaseClass
{
    void Insert(T2 node);
    bool Remove(T1 index);
    bool Get(T1 index, out T2 t2);
}