namespace UnityBuilder {
    public interface IPreProcessor {
        int PreOrder { get; }
        void PreProcess(IBuildHelper helper);
    }
}