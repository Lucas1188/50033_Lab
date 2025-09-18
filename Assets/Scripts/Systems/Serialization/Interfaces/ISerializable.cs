public interface IGameSerializable
{
    public string GetRuntimeStateAsJson();
    public void SetRuntimeStateAsJson(string value);
}