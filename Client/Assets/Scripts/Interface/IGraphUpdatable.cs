
public interface IGraphUpdatable
{
    void BhvOnEnter();

    void BhvOnLeave();

    void BhvFixedUpdate(float dt);

    void BhvLateFixedUpdate(float dt);

    void BhvUpdate(float dt);

    void BhvLateUpdate(float dt);
}