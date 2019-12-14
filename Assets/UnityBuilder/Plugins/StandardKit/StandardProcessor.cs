namespace UnityBuilder {
    public class StandardProcessor : IProcessor {
        public void Process() {
            UnityEngine.Debug.Log("Process!");
        }
    }
}