using UnityBuilder;
using Debug = UnityEngine.Debug;

public class MyProcessor : IProcessor {
    public void Process() => Debug.Log("MyProcessor.Process");
}