namespace UnityBuilder {
    public interface IPostProcessor {
        int PostOrder { get; }
        void PostProcess();
    }
}