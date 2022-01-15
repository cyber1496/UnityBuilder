using UnityEditor.Build.Reporting;
namespace UnityBuilder {
    public interface IProcessor {
        BuildResult Process(IBuildHelper helper);
    }
}