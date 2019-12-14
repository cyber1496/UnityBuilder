using System;
using System.Linq;
using System.Collections.Generic;

namespace UnityBuilder {
    using UnityEditor.Build.Reporting;
    using StandardKit;
    public enum RegisterState : int {
        Success, Duplicated,
    }
    public static class BuildProvider {
        static IBuildHelper buildeHelper = new BuildHelper();
        static IBuildLogHandler buildLogHandler = new BuildLogHandler();
        static IProcessor processor = new Processor();
        static readonly SortedDictionary<int, IPreProcessor> preProcessores = new SortedDictionary<int, IPreProcessor>();
        static readonly SortedDictionary<int, IPostProcessor> postProcessores = new SortedDictionary<int, IPostProcessor>();

        public static RegisterState RegisterBuildHelper(IBuildHelper newBuildHelper) {
            var state = buildeHelper is BuildHelper || newBuildHelper is BuildHelper ? RegisterState.Success : RegisterState.Duplicated;
            buildeHelper = newBuildHelper;
            return state;
        }
        public static RegisterState RegisterBuildLogHandler(IBuildLogHandler newBuildLogHandler) {
            var state = buildeHelper is BuildLogHandler || newBuildLogHandler is BuildLogHandler ? RegisterState.Success : RegisterState.Duplicated;
            buildLogHandler = newBuildLogHandler;
            return state;
        }
        public static RegisterState RegisterProcessor(IProcessor newProcessor) {
            var state = processor is Processor || newProcessor is BuildHelper ? RegisterState.Success : RegisterState.Duplicated;
            processor = newProcessor;
            return state;
        }
        public static RegisterState RegisterPreProcessor(IPreProcessor preProcessor) {
            var state = preProcessores.ContainsKey(preProcessor.PreOrder) ? RegisterState.Duplicated : RegisterState.Success;
            preProcessores.Add(preProcessor.PreOrder, preProcessor);
            return state;
        }
        public static RegisterState RegisterPostProcessor(IPostProcessor postProcessor) {
            var state = postProcessores.ContainsKey(postProcessor.PostOrder) ? RegisterState.Duplicated : RegisterState.Success;
            postProcessores.Add(postProcessor.PostOrder, postProcessor);
            return state;
        }
        public static void Process() {
            buildLogHandler.PreProcess(buildeHelper);
            Array.ForEach(preProcessores.Values.ToArray(), proc => proc?.PreProcess(buildeHelper));
            var result = processor.Process(buildeHelper);
            Array.ForEach(postProcessores.Values.ToArray(), proc => proc?.PostProcess(buildeHelper));
            buildLogHandler.PostProcess(buildeHelper);

        }
    }
}