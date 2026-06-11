using System;
public interface ISaveable
{
    string ID { get; }
    Type SaveType { get; }
    object SaveState();
    void LoadState(object state);
}