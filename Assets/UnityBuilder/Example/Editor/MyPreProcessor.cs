using UnityBuilder;
using Debug = UnityEngine.Debug;

public class MyPreProcessor : IPreProcessor {
    public int PreOrder => 0;
    public void PreProcess(IBuildHelper helper) => Debug.Log("MyPreProcessor.PreProcess");
}
