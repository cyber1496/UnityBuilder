using UnityBuilder;
using Debug = UnityEngine.Debug;

public class MyPostProcessor : IPostProcessor {
    public int PostOrder => 0;
    public void PostProcess(IBuildHelper helper) => Debug.Log("MyPostProcessor.PostProcess");
}
