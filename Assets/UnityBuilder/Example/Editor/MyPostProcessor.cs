using UnityBuilder;
using Debug = UnityEngine.Debug;

public class MyPostProcessor : IPostProcessor {
    public int PostOrder => 0;
    public void PostProcess() => Debug.Log("MyPostProcessor.PostProcess");
}
