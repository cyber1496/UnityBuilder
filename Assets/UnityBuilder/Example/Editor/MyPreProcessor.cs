using UnityBuilder;
using Debug = UnityEngine.Debug;

public class MyPreProcessor : IPreProcessor {
    public int PreOrder => 0;
    public void PreProcess() => Debug.Log("MyPreProcessor.PreProcess");
}
